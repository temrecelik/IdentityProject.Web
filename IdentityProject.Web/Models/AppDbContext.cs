using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProject.Web.Models
{
    /*IdentityDbContext kullanılırken bir adet user ve role entity'si verilir.Girilen string değeri id değeri için guid değeri 
      stringe çevirir.
    */
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }
    }
}
