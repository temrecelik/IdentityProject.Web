using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityProject.Web.ClaimProvider
{
    /*IClaimsTransformation classı ile Kullanıcı login olduğunda kullanıcı için oluşturulan cookie'nin içeriği burada düzenlenir.
      Yani buradali TransformAsync methodu kullanıcı giriş yaptığında 
     çalışır ve kullanıcının cookie'sinin içerisindeki claimları düzenler yani cookie'yi düzenlenmiş olur.*/
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<User> _userManager;

        public UserClaimProvider(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        //parametredeki principal nesnesi kullanıcının cookie'sinde bulunan claim'lardır.
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            /*
             Default Cookie'ye Claims Ekleme Ya da Çıkarma İşlemleri
             bu işlem ile kullanıcı login olduktan sonra cookie'deki claimları düzenleme işlemi yapılır.
             Öncelikle identityUser ile grişi yapmış kullanıcının cooki'sindeki claim bilgilerini aldık
             claimdaki name bilgisine göre UserManager ile kullanıcı bulduk eğer kullanıcı ve şehir 
             bilgisi null ise direkt claim'ları geri döndük şehir bilgisi var ise bu şehir bilgisi ile
             bir claim oluşturup cooki'ye ekledik artık giren kullanıcının şehir bilgisine göre claim
             bazlı yetkilendirme yapılabilir.

            Not:Şehir bilgisi veri tabanında user tablosunda tutulduğu için tekrar claims tablosundan
            tutmak mantıklı değildir nedeni biri güncellendiğinde diğerininde güncelleme maliyeti vardır.
            Direkt olarak user tablosundan alıp default cookie claim olarak şehir bilgilisini ekleyebiliriz.
            User tablosunda olmayan bir veriyi cookie ile almak istersek bu sefer user ile ilgili bu bilgiyi
            claim tablosuna ekleyip alabiliriz.

            Program.Cs'e scoped geçerek framework'e dahil etmemiz gereklidir.
            */
            var identityPrincipal = principal.Identity as ClaimsIdentity;      

            var currentUser = await _userManager.FindByNameAsync(identityPrincipal.Name);

            if (currentUser == null) 
             return principal;                   

            if (currentUser.City == null)
             return principal;

            /*Aşağıdaki işlem ile  type'ı yani key'i city olan bir claim yok ise bu claim oluşturup bu claim'ın value değerini
             *user tablosundaki city bilgisini ekler. Ve oluşturulan bu claim cooki'ye eklenir. Yani giriş yapan kullanıcıların 
             city değeri null değilse cookie'sini city claim'ı eklenmiş olur.
            */
            if (principal.HasClaim(x => x.Type != "city"))
            {
                Claim cityClaim = new Claim("city", currentUser.City);
                identityPrincipal.AddClaim(cityClaim);     
            }

            return principal;

        }
    }
}
