using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace James.Web.Models
{
    public class DatabaseInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseInitializer(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();
            if (!context.ApplicationUser.Any())
            {
                ApplicationUser admin = new ApplicationUser();
                admin.UserName = "admin";
                admin.Email = "dev@james.tk";
                context.SaveChanges();
            }
        }
    }
}
