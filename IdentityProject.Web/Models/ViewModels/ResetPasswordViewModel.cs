using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
		[Display(Name = "Yeni Şifre : ")]
		public string Password { get; set; }


		[Compare(nameof(Password), ErrorMessage = "Şifreler Eşleşmiyor.")]
		[Required(ErrorMessage = "Şifre Yeniden Gir Alanı Boş Bırakılamaz.")]
		[Display(Name = "Şifreyi yeniden giriniz : ")]
		public string PasswordConfirm { get; set; }
	}
}
