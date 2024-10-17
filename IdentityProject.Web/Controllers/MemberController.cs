using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

		public async Task<IActionResult> PasswordChange()
		{
			return View();
		}

	}
}
