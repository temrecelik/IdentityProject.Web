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

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
        [Display(Name = "Şifre : ")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Şifreler Eşleşmiyor.")]
        [Required(ErrorMessage = "Şifre Yeniden Gir Alanı Boş Bırakılamaz.")]
        [Display(Name = "Şifre yeniden giriniz : ")]
        public string PasswordConfirm { get; set; }
    }
}

