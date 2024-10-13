using IdentityProject.Web.Areas.Admin.Models;
using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task <IActionResult> UserList()
        {
            var UserList = await _userManager.Users.ToListAsync();

            var UserViewModelList = UserList.Select(x => new UserViewModel()
            {
               UserId = x.Id,
               UserName = x.UserName,
               Email = x.Email
            }
            ).ToList();


                return View(UserViewModelList);
        }
    }
}
