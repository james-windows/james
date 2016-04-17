using System.Linq;
using James.Web.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace James.Web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var publicWorkflows =
                _context.Workflow.Include(workflow => workflow.Author).Where(workflow => workflow.Verified).OrderByDescending(workflow => workflow.Downloads);
            ViewBag.Windows = publicWorkflows.Count(workflow => workflow.Platform == Platform.Windows);
            ViewBag.OSX = publicWorkflows.Count(workflow => workflow.Platform == Platform.OSX);
            ViewBag.Both = publicWorkflows.Count(workflow => workflow.Platform == Platform.Both);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Impressum()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet("/[controller]/[action]")]
        public IActionResult Start()
        {
            if (!_context.ApplicationUser.Any())
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO [dbo].[AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName]) VALUES (N'283ee99a-2fb9-420a-887c-02e367439a1a', 0, N'c3564ff5-163f-405d-b6f1-d020503ac8b4', N'dev@james.tk', 0, 1, NULL, N'DEV@JAMES.TK', N'DEV@JAMES.TK', N'AQAAAAEAACcQAAAAEAbJG01mxrmrjqTaNySoYdsNCQcADkrjhZbHdo8MJ/jS/hsMNlctngBGsEkmGr2o3Q==', NULL, 0, N'62c4f1f8-8353-4166-ac49-7c5e86165578', 0, N'dev@james.tk')");
                _context.Database.ExecuteSqlCommand("INSERT INTO [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'1', NULL, N'admin', N'admin')");
                _context.Database.ExecuteSqlCommand("INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'283ee99a-2fb9-420a-887c-02e367439a1a', N'1')");
            }
            return View("Index");
        }
    }
}
