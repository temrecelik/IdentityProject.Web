using IdentityProject.Web.Extensions;
using IdentityProject.Web.Models;
using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.OptionsModels;
using IdentityProject.Web.Services;
using Microsoft.AspNetCore.Identity;
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
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

/*Ýdentity ile ekleme iþlemi yapýlýrken kullanýlacak rules'lar burada ayarlanabilir.Bir Extention klasörü oluþturup burada 
  static bir class tanýmladýktan sonra  oluþturulan static methoda IServiceCollection türünden bir parametre vererek
  bu methoda servis ekleyebiliriz.
*/
builder.Services.AddIdentityWithExt();

//cookie ayarlarý buradan yapýlýr.
builder.Services.ConfigureApplicationCookie(options =>
{
	/*
	  RememberMe iþaretlenirse bu  60 gün tarayýcýda kayýtlý olur yani SignIn fonksiyonundaki PasswordSignInAsync methodunda
      3. parametre true gelmelidir. Eðer false geliyorsa oluþan cookie tarayýcý açýk olduðu sürece var olur.
    */
	var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "AppCookie";

    /*
     Login olmadan girilemeyen sayfalarýn url'ine girmeye çalýþýrsak  bu kod sayesinde identity bizi Belirlediðimiz bu login 
     sayfasýna atacaktýr.
     */
    options.LoginPath = new PathString("/home/SignIn");

    /*Cookie'ye çýkýþ yap fonksiyonunu tanýttýðýmýzda html tarafýnda asp-route-returnurl="/Home/Index" gibi  taglar ile çýkýþ yaptýktan
     sonra hangi sayfaya yönlendiriliceðini belirleyebiliriz. Method içinde rediractionAction kullanmadýðýmýzdan birden fazla 
     çýkýþ yap butonu kullanýlan bir web sitesinde butonun yerine göre farklý sayfalara yönlendirme yapýlabilir.*/
    options.LogoutPath = new PathString("/Member/LogOut");
    
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.SlidingExpiration = true;

}

);


var app = builder.Build();


/*
Eðer proje development ortamýnda deðilse fýrlatýlan hatalar error sayfasýna yönlendirilir.Development modunda ise frameworkün 
vereceði hata sayfasýna gönderilir
*/
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
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
