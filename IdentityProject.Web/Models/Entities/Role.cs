using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Web.Models.Entities
{

    /*
     Identity kütüphanesi hazır bir role entitysi bizlere sunmaktadır.
    
    public class IdentityRole: IdentityRole<string>
    {
        public IdentityRole()
        public IdentityRole(string roleName)
    }

    public  class IdentityRole<TKey> where TKey: IEquatable<TKey>
    {
        public IdentityRole()
        public IdentityRole(string roleName)
        public virtual Tkey Id
        public virtual string? Name
        public virtual string? NormalizedName
        public virtual string? ConcurrencyStamp { get; set; }
        public override string ToString()
    }
     */
    public class Role : IdentityRole
    {
    }
}
