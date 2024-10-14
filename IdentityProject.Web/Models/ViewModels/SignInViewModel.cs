using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel(){ } //boş constructur Identity için gerekli

       
        

        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [EmailAddress(ErrorMessage = "Mail formatı email@example.com formatında olmalıdır.")]
        [Required(ErrorMessage = "Mail Alanı Boş Bırakılamaz.")]
        [Display(Name = "Mail Adresi : ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
        [Display(Name = "Şifre : ")]
        public string Password { get; set; }


        public bool RememberMe { get; set; }    

    }
}
