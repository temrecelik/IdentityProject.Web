using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
	public class ForgetPasswordViewModel
	{
		[EmailAddress(ErrorMessage = "Mail formatı email@example.com formatında olmalıdır.")]
		[Required(ErrorMessage = "Mail Alanı Boş Bırakılamaz.")]
		[Display(Name = "Mail Adresi : ")]
		public string Email { get; set; }
	}
}
