using Microsoft.AspNetCore.Identity;


/*
 User Entity'si IdentityUser'dan miras alarak bir userda olabilecek birçok property'ye erişebilir ayrıca ekstra propertylerde
 buraya eklenebilmektedir. Aşağıda IdentityUser classının içerisindeki propertyler.

public class IdentityUser<TKey> where TKey: IEquatable<TKey>
{
public IdentityUser()
public IdentityUser(string userName)
public virtual TKey Id
public virtual string? UserName
public virtual string? NormalizedUserName
public virtual string? Email
public virtual string? NormalizedEmail
public virtual bool EmailConfirmed
public virtual string? PasswordHash
public virtual string? SecurityStamp
public virtual string? ConcurrencyStamp
public virtual string? Phone Number
public virtual bool Phone NumberConfirmed
public virtual bool TwoFactorEnabled
public virtual DateTimeOffset? LockoutEnd
public virtual bool LockoutEnabled
public virtual int AccessFailedCount { get; set; }
public override string ToString()
}
 */
namespace IdentityProject.Web.Models.Entities
{
    public class User : IdentityUser
    {
      
    }
}
