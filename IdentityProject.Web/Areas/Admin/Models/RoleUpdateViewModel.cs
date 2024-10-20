using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Rol ismi boş bırakılamaz.")]
        [Display(Name ="Rol İsmi: ")]
        public string Name { get; set; } = null!;
    }
}
