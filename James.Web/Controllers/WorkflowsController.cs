using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Mvc;
using James.Web.Models;
using James.Web.Services;
using James.Web.ViewModels.Workflows;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;

namespace James.Web.Controllers
{
    public class WorkflowsController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnv;

        public WorkflowsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _hostingEnv = env;
            _userManager = userManager;
        }

        // GET: Workflows
        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel() {Workflows = new List<DetailsViewModel>(), Admin = IsAdmin()};
            var publicWorkflows =
                _context.Workflow.Include(workflow => workflow.Author).Where(workflow => workflow.Verified).OrderByDescending(workflow => workflow.Downloads);
            foreach (var workflow in publicWorkflows)
            {
                model.Workflows.Add(new DetailsViewModel
                {
                    Workflow = workflow,
                    IconPath = $"{_hostingEnv.WebRootPath}\\workflows\\{workflow.Id}\\icon.png",
                    EditAllowed = CheckAuthorization(workflow)
                });
            }
            return View(model);
        }

        // GET: Workflows
        [HttpGet("/[controller]/[action]/{author?}")]
        public IActionResult Author(string author)
        {
            ViewData["filter"] = $"Filtered by {author}";
            IndexViewModel model = new IndexViewModel() { Workflows = new List<DetailsViewModel>(), Admin = IsAdmin() };
            foreach (var workflow in _context.Workflow.Include(workflow => workflow.Author).Where(workflow => workflow.Verified && workflow.Author.UserName == author))
            {
                model.Workflows.Add(new DetailsViewModel
                {
                    Workflow = workflow,
                    IconPath = $"{_hostingEnv.WebRootPath}\\workflows\\{workflow.Id}\\icon.png",
                    EditAllowed = CheckAuthorization(workflow)
                });
            }
            return View("Index", model);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Verify()
        {
            IndexViewModel model = new IndexViewModel() { Workflows = new List<DetailsViewModel>() };
            foreach (var workflow in _context.Workflow.Include(workflow => workflow.Author).Where(workflow => !workflow.Verified))
            {
                model.Workflows.Add(new DetailsViewModel
                {
                    Workflow = workflow,
                    IconPath = $"{_hostingEnv.WebRootPath}\\workflows\\{workflow.Id}\\icon.png",
                    EditAllowed = CheckAuthorization(workflow)
                });
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public RedirectToActionResult VerifyWorkflow(int id)
        {
            Workflow workflow = _context.Workflow.Include(item => item.Author).Single(item => item.Id == id);
            if (workflow == null)
            {
                return RedirectToAction("Verify");
            }
            workflow.Verified = true;
            _context.Workflow.Update(workflow);
            _context.SaveChanges();
            return RedirectToAction("Verify");
        }

        [Authorize(Roles = "admin")]
        public RedirectToActionResult UnverifyWorkflow(int id)
        {
            Workflow workflow = _context.Workflow.Include(item => item.Author).Single(item => item.Id == id);
            if (workflow == null)
            {
                return RedirectToAction("Verify");
            }
            workflow.Verified = false;
            _context.Workflow.Update(workflow);
            _context.SaveChanges();
            return RedirectToAction("Verify");
        }

        // GET: Workflows/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || !_context.Workflow.Any(item => item.Id == id))
            {
                return HttpNotFound();
            }

            Workflow workflow = _context.Workflow.Include(item => item.Author).Single(item => item.Id == id);
            if (workflow == null || (!workflow.Verified && !CheckAuthorization(workflow)))
            {
                return HttpNotFound();
            }
            DetailsViewModel model = new DetailsViewModel
            {
                Workflow = workflow,
                IconPath = $"{_hostingEnv.WebRootPath}\\workflows\\{workflow.Id}\\icon.png",
                EditAllowed = CheckAuthorization(workflow),
                DisqusViewModel = new DisqusViewModel
                {
                    Identifier = id.ToString(),
                    Url = Request.GetDisplayUrl()
                },
                Admin = IsAdmin()
            };
            return View(model);
        }

        private bool IsAdmin()
        {
            var user = _userManager.Users.FirstOrDefault(applicationUser => applicationUser.Id == User.GetUserId());
            return user != null && (_userManager.GetRolesAsync(user).Result.Any(s => s == "admin"));
        }

        // GET: Workflows/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workflows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind(include: "Name, Description, ShortDescription")]Workflow workflow, IFormFile file)
        {
            ViewBag.Errors = new List<string>();
            ViewBag.Success = new List<string>();
            if (ModelState.IsValid && file != null)
            {
                workflow.PublishDate = DateTime.Now;
                workflow.Author = _userManager.FindByIdAsync(User.GetUserId()).Result;
                workflow.FileSize = file.Length;
                _context.Workflow.Add(workflow);
                _context.SaveChanges();
                var service = new WorkflowExtractService();
                try
                {
                    service.ExtractWorkflow(file, ref workflow, _hostingEnv);
                    ViewBag.Success.Add("Workflow successfull added!");
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (InvalidDataException)
                {
                    RemoveWorkflow(workflow);
                    ViewBag.Errors.Add("invalid format: couldn't unzip the file");
                }
                catch (FormatException)
                {
                    RemoveWorkflow(workflow);
                    ViewBag.Errors.Add("invalid format: couldn't find the config.json");
                }
            }
            return View(workflow);
        }

        private void RemoveWorkflow(Workflow workflow)
        {
            string path = $"{_hostingEnv.WebRootPath}\\workflows\\{workflow.Id}";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            _context.Workflow.Remove(workflow);
            _context.SaveChanges();
        }

        private bool CheckAuthorization(Workflow workflow)
        {
            string userId = User.GetUserId();
            var item = _context.Workflow.Include(w => w.Author).First(w => w.Id == workflow.Id);
            var user = _userManager.Users.FirstOrDefault(applicationUser => applicationUser.Id == userId);
            return user != null && (IsAdmin() || user == item.Author);
        }

        // GET: Workflows/Edit/5
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Workflow workflow = _context.Workflow.Single(m => m.Id == id);
            
            if (!CheckAuthorization(workflow))
            {
                return RedirectToAction("Index");
            }
            if (workflow == null)
            {
                return HttpNotFound();
            }
            return View(workflow);
        }

        // POST: Workflows/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit([Bind(include: "Name, Description, Id, ShortDescription")]Workflow workflow)
        {
            if (!CheckAuthorization(workflow))
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var tmp = _context.Workflow.Single(item => item.Id == workflow.Id);
                tmp.Name = workflow.Name;
                tmp.Description = workflow.Description;
                tmp.ShortDescription = workflow.ShortDescription;
                _context.Update(tmp);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workflow);
        }

        // GET: Workflows/Delete/5
        [ActionName("Delete")]
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (!CheckAuthorization(_context.Workflow.Single(item => item.Id == id)))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return HttpNotFound();
            }

            Workflow workflow = _context.Workflow.Single(m => m.Id == id);
            if (workflow == null)
            {
                return HttpNotFound();
            }

            return View(workflow);
        }

        // POST: Workflows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            Workflow workflow = _context.Workflow.Single(m => m.Id == id);
            if (!CheckAuthorization(workflow))
            {
                return RedirectToAction("Index");
            }
            RemoveWorkflow(workflow);
            _context.Workflow.Remove(workflow);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var results = _context.Workflow.Where(item => item.Id == id);
            if (!results.Any())
            {
                return HttpNotFound();
            }
            Workflow workflow = results.First();
            workflow.Downloads++;
            _context.Workflow.Update(workflow);
            _context.SaveChanges();
            return LocalRedirect($@"~/workflows/{workflow.Id}/{workflow.Name}.james");
        }
    }
}
