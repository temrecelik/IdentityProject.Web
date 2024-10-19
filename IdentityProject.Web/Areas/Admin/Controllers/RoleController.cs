using IdentityProject.Web.Areas.Admin.Models;
using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RoleController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> RoleCreate(RoleCreateViewModel roleCreateViewModel)
        {
            var result = await _roleManager.CreateAsync(new Role() { Name = roleCreateViewModel.Name });

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                    
                }
                return View();
            }
            TempData["SuccessMessage"] = "Rol Ekleme işlemi başarılı";
            return View();

        }
    }
}
