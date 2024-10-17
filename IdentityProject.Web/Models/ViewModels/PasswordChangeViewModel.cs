using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
    public class PasswordChangeViewModel
    {


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz.")]
        [Display(Name = "Mevcut Şifre : ")]
        [MinLength(6, ErrorMessage = "En az 6 karakter giriniz")]
        public string PasswordOld { get; set; } = null!;



        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni Şifre Boş Bırakılamaz.")]
        [Display(Name = "Yeni Şifre : ")]
        [MinLength(6, ErrorMessage = "En az 6 karakter giriniz")]
        public string PasswordNew { get; set; } = null!;

       
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni Şifre Tekrar Alanı Boş Bırakılamaz.")]
        [Display(Name = "Yeni Şifre Tekrar: ")]
        [Compare(nameof(PasswordNew), ErrorMessage = "Şifreler Eşleşmiyor.")]
        [MinLength(6, ErrorMessage = "En az 6 karakter giriniz")]
        public string PasswordConfirm { get; set; }= null!;

    }
}
