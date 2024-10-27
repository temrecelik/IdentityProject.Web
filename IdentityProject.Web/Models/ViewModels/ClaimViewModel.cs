namespace IdentityProject.Web.Models.ViewModels
{
    public class ClaimViewModel
    {
        /*cookileri uygulama kendini de oluşturur bazenden üçünü taraf uygulamalardan cookie oluşur Issuer değeri
        bu claim'ın cooki'sinin kim tarafından oluşturulduğu bilgisini tutan property'dir.*/
        public string Issuer { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
