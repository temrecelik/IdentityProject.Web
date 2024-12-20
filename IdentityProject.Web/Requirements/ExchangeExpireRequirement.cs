using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IdentityProject.Web.Requirements
{
    /*
     Policy bazlı yetkilendirme yapmak için  IAuthorizationRequirement interfacesini implementini classımıza verdikten sonra
     generic olarak aşağıdaki handler classına bu classı tekrar veririz.Handler classın içinde policy bazlı yetkilendirme için ilgili
     business kodları yazılır.

     ExchangeExpireDate claim'ından kullanıcının üye olduktan 15 gün sonrasının tarihi tutulur. Eğer şuanın tarihi bu tarihten ilerdeyse
     artık bu business yetkilendirmesine sahip sayfalara kullanıcı üye olduktan 15 gün sonra erişemeyecektir.Aşağıdaki handler'da bu policy
     bazlı yetkilendirme için kullanılacak business kodu yazılmıştır.
     */
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
    }

    public class ExchangeExpirationRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {

            if (context.User.HasClaim(x => x.Type == "ExchangeExpireDate")) { 
                context.Fail();
                return Task.CompletedTask;
            }

            Claim exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate")!;

            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate.Value)) { 
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
