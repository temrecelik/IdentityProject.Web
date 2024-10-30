using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityProject.Web.ClaimProvider
{
    //Kullanıcı login olduğunda kullanıcı için oluşturulan cookie'nin içeriği burada düzenlenir
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<User> _userManager;

        public UserClaimProvider(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            /*
             Default Cooki'ye Claims Ekleme 
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

            Program.Cs'e scoped geçmemiz gereklidir.
            */
            var identityUser = principal.Identity as ClaimsIdentity;      

            var currentUser = await _userManager.FindByNameAsync(identityUser.Name);

            if (currentUser == null) 
             return principal;                   

            if (currentUser.City == null)
                return principal;

            if (principal.HasClaim(x => x.Type != "city"))
                {
                    Claim cityClaim = new Claim("city", currentUser.City);
                    identityUser.AddClaim(cityClaim);
                }

            return principal;

        }
    }
}
