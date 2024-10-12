using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
    public class SignUpViewModel
    {
        
        [Display(Name = "Kullanıcı Adı: ")] //view tarafında asp-for ile bu stringlere ulaşılabilir.

        public string UserName { get; set; }

        [Display(Name = "Mail Adresi : ")]
        public string Email { get; set; }

        [Display(Name = "Telefon Numarası: ")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Parola : ")]
        public string Password { get; set; }

        [Display(Name = "Parolayı yeniden giriniz : ")]
        public string PasswordConfirm { get; set; }
    }
}

