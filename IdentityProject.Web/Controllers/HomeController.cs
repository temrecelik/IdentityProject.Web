using IdentityProject.Web.Models;
using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _UserManager;


        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _UserManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            //itentity kütüphanesi ile createAsync direkt olarak database oluşturulan kaydı ekler.
            var identityResult = await _UserManager.CreateAsync(
                new()
                {
                    UserName = signUpViewModel.UserName,
                    PhoneNumber = signUpViewModel.PhoneNumber,
                    Email = signUpViewModel.Email
                }
                , signUpViewModel.Password);

            if (identityResult.Succeeded)
            {
                ViewBag.SuccessMessage = "Kayıt işlemi başarılı.";
                return View();
            }

            foreach (IdentityError item in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);

            }
            return View();
        }

         
        
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
