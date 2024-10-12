using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProject.Web.Models
{
    /*IdentityDbContext kullanılırken bir adet user ve role entity'si verilir.Girilen string değeri id değeri için guid değeri 
      stringe çevirir.
    */
    public class DbContext : IdentityDbContext<User, Role, string>
    {
        public DbContext(DbContextOptions<DbContext> options):base(options)
        {
        }
    }
}
