using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Models.Enums;
using IdentityProject.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityProject.Web.Controllers
{
	[Authorize]
	public class MemberController : Controller
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;

        public MemberController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /*Çıkış yapma işlemi Authorize olmalıdır çünkü giriş yapmadan çıkış yapılmaz Identity'den gelen özelliktir.LogOut fonksiyonunda
          görüldüğü gibi bir yönlendirme yazmadık yani bu fonksiyonun view sadece çalışır çıkış yapar ama çıkış yaptıktan sonra
		  hangi sayfaya yönlenmesi gerektiğini asp-route-returnUrl tagı ile belirtmemiz gereklidir.*/
		

        public async Task /*<IActionResult>*/ LogOut()
		{
			await _signInManager.SignOutAsync();
			//return RedirectToAction("Index","Home");
		}

		
		public  async Task<IActionResult> Index()
		{

			var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
			if (currentUser != null)
			{
				UserInformationViewModel userInformationViewModel = new()
				{
					UserName = currentUser.UserName!,
					Email = currentUser.Email!,
					phoneNumber = currentUser.PhoneNumber,
				};
				return View(userInformationViewModel);
			}
			else
				ModelState.AddModelError(string.Empty, "Kullanıcı bilgileri bulunamadı.");
			return View();
		}

		public IActionResult PasswordChange() {  return View(); }	


		[HttpPost]
		public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
		{
			if (!ModelState.IsValid) {
				return View();
			}

			var currentUser =await _userManager.FindByNameAsync(User.Identity!.Name!);

			var checkoldPassword =await _userManager.CheckPasswordAsync(currentUser!, passwordChangeViewModel.PasswordOld);

			if(!checkoldPassword)
			{
				ModelState.AddModelError(string.Empty, "Mevcut Şifre yanlış");
				return View();
			}

			var result = await _userManager.ChangePasswordAsync(currentUser!,passwordChangeViewModel.PasswordOld ,passwordChangeViewModel.PasswordNew);


			if (!result.Succeeded)
			{

				foreach (var error in result.Errors)
				{

					ModelState.AddModelError(string.Empty, error.Description);
					return View();
				}

			}
				/*Şifre değiştiği için cookie'nin yenilenmesi gerekli bu nedenle bu iki kodla oturum kapatım yeni şifreyle 
				 tekrar açtık yani cookie yenilenenerek yeni şifreyi aldı.SecurityStamp Değerinide değiştirdikki diğer uygulamardan
				 oturum sonlandırsın*/
				await _userManager.UpdateSecurityStampAsync(currentUser!);
				await _signInManager.SignOutAsync();
				await _signInManager.PasswordSignInAsync(currentUser!,passwordChangeViewModel.PasswordNew , true,false);

				TempData["SuccessMessage"] = "Şifreniz başarılı bir şekilde değiştirilmiştir.";

				return View();




			}


		public IActionResult UserEdit()
		{
			ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
			

			
			return View(); 
		}
		}

		


	}

