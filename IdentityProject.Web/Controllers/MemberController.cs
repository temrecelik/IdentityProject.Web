using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Models.Enums;
using IdentityProject.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace IdentityProject.Web.Controllers
{
	[Authorize]
	public class MemberController : Controller
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<User> signInManager, UserManager<User> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
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
					PictureUrl = currentUser.Picture,
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


		public async Task <IActionResult> UserEdit()
		{
			ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
			var currentuser = await _userManager.FindByNameAsync(User.Identity!.Name!);
			TempData["gender"] = currentuser!.Gender;

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

			if (currentUser == null) {
				return View();
			}

			var UserEditViewModel = new UserEditViewModel()
			{
				UserName = currentUser!.UserName!,
				gender = currentUser!.Gender,
                Email = currentUser!.Email!,
				PhoneNumber = currentUser!.PhoneNumber!,
				BirthDate = currentUser!.BirthDay,
				city = currentUser!.City

			};

			
			return View(UserEditViewModel); 

			
		}
		[HttpPost]
		public async Task<IActionResult> UserEdit(UserEditViewModel userEditViewModel)
		{
			if(!ModelState.IsValid) return View();

			var currentUser =await _userManager.FindByNameAsync(User.Identity!.Name!);

			currentUser!.UserName = userEditViewModel.UserName;
			currentUser.PhoneNumber = userEditViewModel.PhoneNumber;
			currentUser.City = userEditViewModel.city;
			currentUser.Email = userEditViewModel.Email;
			currentUser.Gender = userEditViewModel.gender;
			currentUser.BirthDay = userEditViewModel.BirthDate;


		

			if (userEditViewModel.Picture != null && userEditViewModel.Picture.Length > 0) {

				var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
				//profil fotoğrafını kullanılan bir isme yazılmaması için unieqe bir isim üretiyoruz.Dosyanın uzantısını
				//da modelden alırız.
				var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension
						(userEditViewModel.Picture.FileName)}";

				var newPicturePath = Path.Combine(wwwrootFolder.First
					(x => x.Name=="userpictures").PhysicalPath!,randomFileName);

				//Profil Fotoğrafını userpicture klasörüne kaydetme işlemi
				using var stream = new FileStream(newPicturePath,FileMode.Create);
				await userEditViewModel.Picture.CopyToAsync(stream);

				currentUser.Picture = randomFileName;
			}

				var updateToUserResult = await _userManager.UpdateAsync(currentUser);

			if (!updateToUserResult.Succeeded)
			{
				foreach (var errors in updateToUserResult.Errors)
				{
					ModelState.AddModelError(string.Empty, errors.Description);
					return View();
				}
			}

				await _userManager.UpdateSecurityStampAsync(currentUser);
					await _signInManager.SignOutAsync();
					await _signInManager.SignInAsync(currentUser, true);

					TempData["SuccessMessage"] = "Profil bilgileriniz güncellenmiştir.";
					return View(userEditViewModel);

				

        }


		/*
		 Eğer ki bir kullanıcı erişimi olmayan bir sayfaya gitmeye çalışır ise AcessDenied sayfasına yönlendirilir.
		 */
		public  IActionResult AccessDenied(string returnUrl)
		{
			return View();
		}


		}

		


	}

