using IdentityProject.Web.Areas.Admin.Models;
using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            var Allroles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {

                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();

            return View(Allroles);
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

        public async Task< IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if(roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır");
            }

            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id , Name=roleToUpdate.Name!});

        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel roleUpdateViewModel) { 
            
            var roleToUpdate = await _roleManager.FindByIdAsync(roleUpdateViewModel.Id);

            roleToUpdate!.Name = roleUpdateViewModel.Name;

            
            await _roleManager.UpdateAsync(roleToUpdate);

            TempData["SuccessMessage"] = "Rol güncelleme işlemi başarılı.";

            return View(roleUpdateViewModel);
        }


     /*
      rol silmede sıkıntı var rol silinmiyor.
      */

        public async Task<IActionResult> RoleDelete(string roleId)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(roleId);

            if (roleToDelete == null) {
                throw new Exception("Bir hata oluştu");
               
            }

            var result =await _roleManager.DeleteAsync(roleToDelete);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction(nameof(RoleController.Index));
            }


            TempData["SuccessMessage"] = "Rol silme işlemi başarılı.";

            return RedirectToAction(nameof(RoleController.Index));
        } 
    }
}
