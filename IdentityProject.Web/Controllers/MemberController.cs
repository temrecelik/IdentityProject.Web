using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Web.Controllers
{
	[Authorize]
	public class MemberController : Controller
	{
		private readonly SignInManager<User> _signInManager;

		public MemberController(SignInManager<User> signInManager)
		{
			_signInManager = signInManager;
		}

		/*Çıkış yapma işlemi Authorize olmalıdır çünkü giriş yapmadan çıkış yapılmaz*/

		public async Task /*<IActionResult>*/ LogOut()
		{
			await _signInManager.SignOutAsync();
			//return RedirectToAction("Index","Home");
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
