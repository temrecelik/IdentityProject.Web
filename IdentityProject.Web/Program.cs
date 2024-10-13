using IdentityProject.Web.Extensions;
using IdentityProject.Web.Models;
using IdentityProject.Web.Models.Entities;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
}
);

/*Ýdentity ile ekleme iþlemi yapýlýrken kullanýlacak rules'lar burada ayarlanabilir.Bir Extention klasörü oluþturup burada 
  static bir class tanýmladýktan sonra  oluþturulan static methoda IServiceCollection türünden bir parametre vererek
  bu methoda servis ekleyebiliriz.
*/
builder.Services.AddIdentityWithExt();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



//bir area oluþturulduðunda bu þekilde program.cs eklenir

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




app.Run();
