using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IdentityProject.Web.Requirements
{
    public class ViolancePageRequirement : IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }    
    }
    /*
     Bu method a thresholdAge parametre olarak gönderilir.Bu method bir policy için hazırlanmış bir methoddur. Bu methodu kullanılan
     policy program.cs'de tanımladık. Bu policy ile yetkilendirilen sayfaya giriş yapan kullanıcının yaşı verilen parametredeki yaştan 
     küçük ise sayfaya erişemez büyük ise erişebilir. Yetişkin içeriklerine sahip sayfalar bu şekilde oluşturulabilir.
     */

    public class ViolancePageReqirementHandler : AuthorizationHandler<ViolancePageRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolancePageRequirement requirement)
        {
            

            if(!context.User.HasClaim(x => x.Type == "BirthDay"))
            {
                context.Fail(); 
                return Task.CompletedTask;
            }

            Claim BirthDay = context.User.FindFirst("BirthDay")!;
            var today = DateTime.Now;
            var BirthDate = Convert.ToDateTime(BirthDay.Value);
            var age = today.Year - BirthDate.Year;

            //şubatın dört yılda bir 29 çekmesiyle alakalı sorgu
            if (BirthDate > today.AddYears(-age)) age--;

            if(requirement.ThresholdAge > age)
            {
                context.Fail(); 
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;  
            
        }
    }
}
