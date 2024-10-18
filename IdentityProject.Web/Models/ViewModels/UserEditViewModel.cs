using IdentityProject.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Models.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
        [Display(Name = "Kullanıcı Adı: ")] //view tarafında asp-for ile bu stringlere ulaşılabilir.
        public string UserName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Mail formatı email@example.com formatında olmalıdır.")]
        [Required(ErrorMessage = "Mail Alanı Boş Bırakılamaz.")]
        [Display(Name = "Mail Adresi: ")]
        public string Email { get; set; } =null!;

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz.")]
        [Display(Name = "Telefon Numarası: ")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;


        [Display(Name = "Doğum Tarihi: ")]
        [DataType(DataType.Date)]
        public  DateTime? BirthDate { get; set; }

        [Display(Name = "Şehir: ")]
        public string? city { get; set; }

        [Display(Name = "Cinsiyet: ")]
        public Gender? gender { get; set; }

        [Display(Name = "Profil Fotoğrafı: ")]
        public IFormFile? Picture { get; set; }




    }
}
