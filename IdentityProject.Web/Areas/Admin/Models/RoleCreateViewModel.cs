using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {

        [Required(ErrorMessage ="Role isim alanı boş bırakılamaz.")]
   
        public string Name { get; set; }    
    }
}
