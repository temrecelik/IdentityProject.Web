using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel() { }

        public SignUpViewModel(string userName, string email, string phoneNumber, string password, string passwordConfirm)
        {
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            PasswordConfirm = passwordConfirm;
        }

        //validasyonlar razor page'de asp-validation-for tagı ile kullanılırlar.(Required,Compare, etc...)

        [Required(ErrorMessage ="Kullanıcı Adı Boş Bırakılamaz")]
        [Display(Name = "Kullanıcı Adı: ")] //view tarafında asp-for ile bu stringlere ulaşılabilir.
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage ="Mail formatı email@example.com formatında olmalıdır.")]
        [Required(ErrorMessage = "Mail Alanı Boş Bırakılamaz.")]
        [Display(Name = "Mail Adresi : ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz.")]
        [Display(Name = "Telefon Numarası: ")]
        public string PhoneNumber { get; set; }

     
        [Required(ErrorMessage = "Parola Boş Bırakılamaz.")]
        [Display(Name = "Parola : ")]
        public string Password { get; set; }


        [Compare(nameof(Password),ErrorMessage ="Parolalar Eşleşmiyor.")]
        [Required(ErrorMessage = "Parolayı Yeniden Gir Alanı Boş Bırakılamaz.")]
        [Display(Name = "Parolayı yeniden giriniz : ")]
        public string PasswordConfirm { get; set; }
    }
}

