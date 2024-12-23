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
using System.Security.Claims;

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
            //returnUrl null ise Home/Index sayfasının linkini alır.
            returnUrl ??= Url.Action("Index", "Home");
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

            if (SignInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Sisteme 3 dakika boyunca giriş yapamazsınız");
                return View();
            }

            if (!SignInResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, $"Email veya şifre yanlış.Yanlış giriş sayısı : " +
                $"{await _UserManager.GetAccessFailedCountAsync(user)}");
                return View();

            }
           
            if(user.BirthDay.HasValue)
            {
                /*login olduktan sonra başarılı ise claims'lar ile tekrar bir signIn işlemi daha yaptırıyoruz ve User'ın Birthday
                 property'si var ise claim olarak cookie'sine ekleniyor*/
                    await _SignInManager.SignInWithClaimsAsync(user, signInViewModel.RememberMe, new[]
                    {new Claim ("BirthDay" , user.BirthDay.Value.ToString())});
            }

             return Redirect(returnUrl!);    	
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

               
            /*Policy Bazlı Yetkilendirme için Kullanıcının üye oluş tarihini Claim Tablosuna Ekleme                  
               *Burada kullanıcı artık üye olmuştur  kullanıcının üye olma tarihininin 15 gün ilerisini claim tablosuna kaydedip
                ücretli bir sayfayı yada özelliği 15 gün boyunca gösterebiliriz.Bu örnek bir business'dır.
                Bu policy bazlı yetkilendirmedir.

               *Normalde kullanıcı giriş yaptığında identity user tablosundaki name, username, email gibi
                bazı propertyleri claim olarak cooki'ye ekler. Eğer cookie'de ekstra bir claim tutmak istersek
                kullanıcı üye olduğunda aşağıdaki gibi AddClaim işlemi yapmalıyız. Bu şekilde üye olan bir kullanıcı
                giriş yaptığında artık onun için bir ExchangeExpireDate adında da bir claim oluşur ve cookie'ye
                eklenir. Bu claim ile de bir policy bazlı yetkilendirme ya da claim bazlı yetkilendirme yapılabilir.       
             */
            if (identityResult.Succeeded)
            {
                //üye olma tarihini claim olarak alıyoruz kullanıcının cookie'sine ekleliyoruz.
                var exchangeCliam = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(15).ToString());
                var user =await _UserManager.FindByNameAsync(signUpViewModel.UserName);
                var claimResult =await _UserManager.AddClaimAsync(user!, exchangeCliam);

                if (!claimResult.Succeeded)
                {
                    foreach(var error in claimResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View();  
                    }
                }

                //Temp Data ile Mesaj SignUp'ın GetRequest'ine taşınabilmektedir.
                TempData["SuccessMessage"] = "Kayıt işlemi başarılı.";

                return RedirectToAction(nameof(HomeController.SignUp)); 
            }
          
            foreach (IdentityError item in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
                /*model state'e identity'nin kendi hataları eklenir View tarafında asp-validation-summary="ModelOnly" ile 
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

            await _EmailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

             TempData["success"]= $"Şifre yenileme bağlantısı {hasUser.Email} mail adresine gönderilmiştir.";
           return  RedirectToAction(nameof (ForgetPassword));
        }





        //ResetPassword sayfaında direkt olarak bu parametreler ille token ve userId'ye erişilir yani otomatik mapleme yapılır.
        public IActionResult ResetPassword(string userId ,string token)
        {
            //ResetPassword fonksiyonu için userID ve token'ı ResetPassword HttpPost'a taşımak içine TempData kullandık
            TempData["UserId"] = userId;
            TempData["token"] = token;  
            
           

            return View();

            

        }

        [HttpPost]
        public async Task <IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var userId = TempData["UserId"];
            var token = TempData["token"];

         

            if (userId == null || token == null) {
                //ModelState.AddModelError(string.Empty, "Bir şeyler ters gitti. Şifre sıfırlamak için tekrar deneyiniz.");
                //return View();
                throw new Exception("Bir şeyler ters gitti. Şifre sıfırlamak için tekrar deneyiniz.");
              
            }

            var hasUser = await _UserManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null) {
                ModelState.AddModelError(string.Empty, "Bir şeyler ters gitti. Şifre sıfırlamak için tekrar deneyiniz");
                return View();
            }

            //burada identity ile password resetlenebiliyor
            IdentityResult result =await _UserManager.ResetPasswordAsync(hasUser, token!.ToString()!, resetPasswordViewModel.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifre yenileme işlemi başarılı";
                return RedirectToAction("SignIn", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();  
            }

             

            
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
