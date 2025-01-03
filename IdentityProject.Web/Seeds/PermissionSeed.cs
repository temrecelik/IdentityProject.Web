﻿using IdentityProject.Web.Models;
using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.Permissions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityProject.Web.Seeds
{
    public class PermissionSeed
    {
        /*
         Aşağıdaki method Uygulama ayağa kalktığında bir kere çalışır. SeedData'lar hem veri tabanının sağlıklı 
         çalışıp çalışmadığını kontrol etmek için kullanılır. Hem de veri tabanında bulunacak sabit değerlerin elle 
         girilmek yerine kod tarafından girilmesini sağlamak için kullanılır. 
         Burada biz Stock Order Catalog entity'leri için crud işlemlerini user'lar sahip olduğu role göre yapacaktır.
         Diyelimki basic-role isimli rol sadece read işlemi claimlarına'ına sahipken admin-role tüm crud işlemlerini
         yapabilecek claim'lara sahiptir. Bu rolllerdeki bu claimlar birer permission'dır ve seed data olarak 
         bu rollere eklenir. Bu permissionları Permissions class'larında const oluşturduk. Artık yapıyı 
         StockControllar'da  Create işlemi yapmak için kullanıcının bu işlemi ilgili claim'ı yani permission'ını 
         içeren role sahip olması gereklidir şeklinde düşünebiliriz.
         Bu methodunun program çalıştırıldığında çalışabilmesi için program.cs'de çağırıp çalıştırmalıyız bu
         nedenle static yapıdadır direkt olarak nesne üretmeden PermissionSeed class'ı üzerinden çağrılıp 
         çalıştırılır.
        */
        public static async Task Seed(RoleManager<Role> roleManager)
        {
            var hasbasicRole = await roleManager.RoleExistsAsync("BasicRole");

            if(!hasbasicRole)
            {
                await roleManager.CreateAsync(new Role() { Name = "BasicRole" });
                var basicRole =await roleManager.FindByNameAsync("BasicRole");
                await AddReadPermission(basicRole!,roleManager);
              
            }

            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");

            if (!hasAdvancedRole)
            {
                await roleManager.CreateAsync(new Role() { Name = "AdvancedRole" });
                var AdvancedRole = await roleManager.FindByNameAsync("AdvancedRole");
                await AddReadPermission(AdvancedRole!, roleManager);
                await AddWritePermission(AdvancedRole!, roleManager);
                await AddUpdatePermission(AdvancedRole!, roleManager);  

            }


            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new Role() { Name = "AdminRole" });
                var AdminRole = await roleManager.FindByNameAsync("AdminRole");
                await AddReadPermission(AdminRole!, roleManager);
                await AddWritePermission(AdminRole!, roleManager);
                await AddUpdatePermission(AdminRole!, roleManager);
                await AddDeletePermission(AdminRole!, roleManager); 

            }


        }

        public static async Task AddReadPermission(Role role, RoleManager<Role> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Read));
        }

        public static async Task AddWritePermission(Role role, RoleManager<Role> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Write));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Write));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Write));
        }

        public static async Task AddUpdatePermission(Role role, RoleManager<Role> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Update));
        }

        public static async Task AddDeletePermission(Role role, RoleManager<Role> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Delete));
        }



    }
}
