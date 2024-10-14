using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Web.Controllers
{
	public class MemberController : Controller
	{
		private readonly SignInManager<User> _signInManager;

		public MemberController(SignInManager<User> signInManager)
		{
			_signInManager = signInManager;
		}

		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index","Home");
		}
	}
}
