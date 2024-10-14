using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Web.Localization
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {
        /*Identity'nin verdiği errorları türkçeleştirme Methodları override ederek içeriğini değiştirebiliriz böylece otomatik 
          hazırlanmış validation hata mesajları yerine bizim belirlediğimiz hata mesajları fırlatılır.*/

        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DuplicateUserName", Description = $"{userName} kullanıcı adı daha önce alınmış." };
            //return base.DuplicateUserName(userName);
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new() { Code = "DuplicateEmail", Description = $"{email} mail adresi zaten kullanılmış." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordTooShort", Description = "Şifre en az 6 karakter olmalıdır." };
            //return base.PasswordTooShort(length);
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new() { Code = "PasswordRequiresLower", Description = "Şifrede a-z aralğında en az bir küçük harf bulunmalıdır." };
            //return base.PasswordRequiresLower();
        }
    }
}
