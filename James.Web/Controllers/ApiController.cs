using System.Linq;
using James.Web.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

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
        public object Index(Platform os = Platform.Both, string filter = "")
        {
            return _context.Workflow
                .Where(workflow =>  workflow.Verified &&
                                    workflow.Name.StartsWith(filter ?? "") &&
                                    (workflow.Platform == os || workflow.Platform == Platform.Both))
                .Include(workflow => workflow.Author)
                .Select(
            workflow =>new
                {
                    workflow.Id,
                    workflow.Name,
                    Platform = workflow.Platform.ToString(),
                    Author = workflow.Author.UserName,
                    workflow.Downloads,
                    workflow.FileSize,
                    workflow.PublishDate,
                    workflow.ShortDescription
                }
            );
        }
    }
}
