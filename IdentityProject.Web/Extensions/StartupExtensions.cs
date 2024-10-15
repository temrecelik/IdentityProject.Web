using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using IdentityProject.Web.CustomValidations;
using IdentityProject.Web.Localization;
using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void  AddIdentityWithExt(this IServiceCollection services)

        {
            /*Parola yenile bağlantısındaki tokenin ömrünü bu şekilde ayarlarız 1 saat sonra link geçersiz olur.*/
			services.Configure<DataProtectionTokenProviderOptions>(options =>
			{
				options.TokenLifespan = TimeSpan.FromSeconds(1);
			});


			services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwcxyz0123456789_.";
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

                //3 kere başarısız giriş yapan kullanıcı 3 dakika kitlenir sisteme giriş yapamaz
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;


            })  .AddPasswordValidator<PasswordValidator>()
                .AddUserValidator<UserValidator>()
                .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
			//CustomValidatorden geliyor.

			/*
              AddDefaultTokenProviders():Şifremi unuttum kısmında şifre sıfırlama işleminde token kullanmaya yarar mail adresine
              gönderilen şifre sıfırlama sayfasının ömrünü belirlemeye yarar.
             */

		}
	}
}
