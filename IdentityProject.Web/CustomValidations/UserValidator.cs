using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Web.CustomValidations
{
    public class UserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            //Kullanıcı adı sayı ile başlayamaz 

            var errors = new List<IdentityError>();
            var isNumaric = int.TryParse(user!.UserName[0].ToString() , out _);

            if (isNumaric)
            {
                errors.Add(new() { Code="FirstLettercontainsDigit" , Description="Kullanıcı adının ilk karakteri sayısal ifade içeremez"});

            }
            if (errors.Any()) { 
            
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));    

            }

            return Task.FromResult(IdentityResult.Success);
           
        }
    }
}
