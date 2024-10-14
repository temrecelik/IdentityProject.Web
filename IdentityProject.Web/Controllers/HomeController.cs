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

        public IActionResult SignIn()
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
            if (!ModelState.IsValid)
            {
                return View();
                
            }


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
                //Temp Data ile Mesaj SignUp'ın Get haline taşınabilmektedir.
                TempData["SuccessMessage"] = "Kayıt işlemi başarılı.";

                return RedirectToAction(nameof(HomeController.SignUp)); 
            }
          

            foreach (IdentityError item in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
                /*model state'e identity'nin kendi hataları eklenir Vieew tarafında asp-validation-summary="ModelOnly" ile 
                bu hatalar yazdırılır.Viewmodeldaki kendi belirlediğimiz validationlar ise ModelOnly  seçili olduğu için 
                burada çıkmaz all dersek çıkar. */
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
