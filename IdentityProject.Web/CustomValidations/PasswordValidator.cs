using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Web.CustomValidations
{

    /*
     Password için Identity'nin sağladığı validationlardan ekstra bir validation yazmak istersek IPasswordValidator
    interface'i kullanılır.Program.Cs'e extension ile bildirdik
     */
    public class PasswordValidator : IPasswordValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
        {
            var errors = new List<IdentityError>();

            if (password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new() { Code = "PasswordContainUserName", Description = "Şifre alanı kullanıcı adı içeremez." });

            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
