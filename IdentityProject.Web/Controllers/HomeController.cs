using IdentityProject.Web.Models;
using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Models.ViewModels;
using IdentityProject.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace IdentityProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _UserManager; //Identity ile user ekleme işlemlerini yapan sınıftır
        private readonly SignInManager<User> _SignInManager; //Identity ile signIn işlemlerini yapmaya yarayan sınıftır.
        private readonly IEmailService _EmailService;



		public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
		{
			_logger = logger;
			_UserManager = userManager;
			_SignInManager = signInManager;
			_EmailService = emailService;
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
        
        [HttpPost]
        /*kullanıcı sadece Giriş yap butonuna basarak giriş sayfasına gelmeyebilir ekstra olarak giriş yapmadan göremeyeceği
        sayfalar içinde giriş sayfasına yönlendirilmelidir. Bu nedenle ikince parametre olarak returnUrl parametresini verdik */
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Home");
            if(signInViewModel.Password == null || signInViewModel.Email == null)
            {
                return View();
            }

            var user = await _UserManager.FindByEmailAsync(signInViewModel.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            /*3. parametre true ise kullanıcın bilgilerini cookie'de tutup tutmamaya yarar.RememberMe tiklenirse true döner
                 Version belirnenen süre boyunca kullanıcı bilgileri tarayıcı kapatılıp açılsa dahi cookie'de tutulur.
              4. parametre yanlış giriş ile ilgilidir. Eğer kullanıcı çok fazla yanlış giriş yaparsa identity özelliği olarak
                 kullanıcının giriş belirli bir müddet kilitlenir.İdentity serviste istenilen ayarlar sağlanabilir.
                 Bu işlem başarılı olursa artık login olunmuş demektir PsswordSignInAsync methodu artık kullanıcının bilgilerine 
                 bir cookie oluşturur.
            */
            var SignInResult =await _SignInManager.PasswordSignInAsync(user, signInViewModel.Password, signInViewModel.RememberMe, true);

            if (SignInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (SignInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,  "Sisteme 3 dakika boyunca giriş yapamazsınız" );
                return View();
            }

            ModelState.AddModelError(string.Empty, $"Email veya şifre yanlış.Yanlış giriş sayısı : " +
                $"{await _UserManager.GetAccessFailedCountAsync(user)}");
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

         
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var hasUser = await _UserManager.FindByEmailAsync(forgetPasswordViewModel.Email);   

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu mail adresine sahip bir kullanıcı bulunamamıştır.");
                return View();
            }

            string passwordResetToken = await _UserManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword","Home", new {UserId = hasUser.Id ,Token = passwordResetToken}
            ,HttpContext.Request.Scheme);

          

            /*bu şekilde ForgetPassword'un httpPost'u yerine HttpGet'ine yönlendirilir. Uygulama bunu başka bir sayfaya 
             yönlendiğini düşündüğü için viewBag ile alınan mesajlar tutulmaz bu nedenle mesajı TempData ile tutarız.*/

            await _EmailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email);

             TempData["success"]= $"Şifre yenileme bağlantısı {hasUser.Email} mail adresine gönderilmiştir.";
           return  RedirectToAction(nameof (ForgetPassword));
        }


        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
