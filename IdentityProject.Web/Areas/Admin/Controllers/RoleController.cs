using IdentityProject.Web.Areas.Admin.Models;
using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IdentityProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "role-action")]
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

        /*
         Bu işlem ile sadece rolü role-action olan kullanıcılar giriş yaptıktan sonra bu sayfaya erişebilir.
         */
        [Authorize(Roles ="role-action")] 
        public IActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "role-action")]
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


        [Authorize(Roles = "role-action")]
        public async Task< IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if(roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır");
            }

            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id , Name=roleToUpdate.Name!});

        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel roleUpdateViewModel) { 
            
            var roleToUpdate = await _roleManager.FindByIdAsync(roleUpdateViewModel.Id);

            roleToUpdate!.Name = roleUpdateViewModel.Name;

            
            await _roleManager.UpdateAsync(roleToUpdate);

            TempData["SuccessMessage"] = "Rol güncelleme işlemi başarılı.";

            return View(roleUpdateViewModel);
        }


        /*
        rol silmede sıkıntı var rol silinmiyor.Parametredi Id null geliyor.
        */
        [Authorize(Roles = "role-action")]
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


		[Authorize(Roles = "role-action")]
		public async Task<IActionResult> AssignRoleToUser(string Id)
        {
        
            var currentUser =await _userManager.FindByIdAsync(Id);
            ViewBag.UserId = Id;

            var roles =await _roleManager.Roles.ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(currentUser!);  


            var roleViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel { Id = role.Id, Name = role.Name! };

                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }
               

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

		[Authorize(Roles = "role-action")]
		[HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string UserId,List<AssignRoleToUserViewModel> requestList)
        {
           var UserToAssignRoles = await _userManager.FindByIdAsync(UserId);


			foreach (var role in requestList)
            {

                if (role.Exist)
                {
                    await _userManager.RemoveFromRoleAsync(UserToAssignRoles!, role.Name);
                    var result = await _userManager.AddToRoleAsync(UserToAssignRoles!, role.Name);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);			
						}
						return View(requestList);
					}
					TempData["SuccessMessage"] = "Rol Ekleme işlemi başarılı";
				}
                else
                {

					var result = await _userManager.RemoveFromRoleAsync(UserToAssignRoles!, role.Name);
					if (!result.Succeeded)
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
						return View(requestList);
					}

					TempData["SuccessMessage"] = "Rol Ekleme işlemi başarılı";
				}
            }             

           
            

            return View(requestList);
        }
    }
}
