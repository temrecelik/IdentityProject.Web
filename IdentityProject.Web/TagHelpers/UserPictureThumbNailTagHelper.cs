using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace IdentityProject.Web.TagHelpers
{

    /*
     Eğer bir kişi profil fotoğrafı belirlememiş ise otomatik olarak bu kişiye sayfanın düzenin bozulmaması için
     defalut bir profil fotoğrafı atanır insan silüeti gibi.Eğer belirlemiş ise bu sefer belirlediği profil fotoğrafı
     url ile wwwroot'a kaydetme işleminde sonra buradaki url ile alınır. Bu işlem için view'a bir işi kodu yazılır
     View'lara iş kodu yazılması sağlıklı bir davranış olmadığı için bu işlemler taghelper'lar ile gerçekleştirilir.

     Not:TagHelper'lar yazıldıktan sonra  bu tag helpers classının bulunduğu projenin naspace'i
        _ViewImports klösörüne eklenmelidir.
                 @addTagHelper *, IdentityProject.Web


     */
    public class UserPictureThumbNailTagHelper :TagHelper
    {
        public string? PictureUrl { get; set; } 

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            

            if (string.IsNullOrEmpty(PictureUrl))
            {
                output.Attributes.SetAttribute("src", "/userpictures/default_userpicture.png");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/userpictures/{PictureUrl}");
            }

            base.Process(context, output);
        }

    }
}
