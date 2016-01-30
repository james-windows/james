using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using James.Web.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace James.Web.Controllers
{
    public class ApiController : Controller
    {
        private ApplicationDbContext _context;
        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public object Index(string filter = "")
        {
            return _context.Workflow.Where(workflow => workflow.Verified && workflow.Name.StartsWith(filter ?? "")).Include(workflow => workflow.Author).Select(
            workflow =>new
                {
                    Name = workflow.Name,
                    Author = workflow.Author.UserName,
                    Downloads = workflow.Downloads,
                    Filesize = workflow.FileSize,
                    PublishDate = workflow.PublishDate,
                    Id = workflow.Id,
                    ShortDescription = workflow.ShortDescription
                }
            );
        }
    }
}
