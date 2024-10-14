using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using IdentityProject.Web.CustomValidations;
using IdentityProject.Web.Localization;

namespace IdentityProject.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void  AddIdentityWithExt(this IServiceCollection services)

        {
            services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwcxyz0123456789_.";
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

            }).AddPasswordValidator<PasswordValidator>().AddUserValidator<UserValidator>()
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>().AddEntityFrameworkStores<AppDbContext>();
            //CustomValidatorden geliyor.
        }
    }
}
