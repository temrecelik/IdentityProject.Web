using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
    public class PasswordChangeViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
        [Display(Name = "Mevcut Şifre : ")]
        public  string  PasswordOld { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
        [Display(Name = "Yeni Şifre : ")]
        public string PasswordNew { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
        [Display(Name = "Yeni Şifre Tekrar: ")]
        [Compare(nameof(PasswordNew), ErrorMessage = "Şifreler Eşleşmiyor.")]
        public string PasswordConfirm { get; set; }

    }
}
