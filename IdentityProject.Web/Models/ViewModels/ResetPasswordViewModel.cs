using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
	public class ResetPasswordViewModel
	{
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
		[Display(Name = "Yeni Şifre : ")]
		public string Password { get; set; }


		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Şifreler Eşleşmiyor.")]
		[Required(ErrorMessage = "Şifre Yeniden Gir Alanı Boş Bırakılamaz.")]
		[Display(Name = "Şifreyi yeniden giriniz : ")]
		public string PasswordConfirm { get; set; }
	}
}
