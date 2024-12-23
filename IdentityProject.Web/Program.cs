using IdentityProject.Web.ClaimProvider;
using IdentityProject.Web.Extensions;
using IdentityProject.Web.Models;
using IdentityProject.Web.Models.Entities;
using IdentityProject.Web.OptionsModels;
using IdentityProject.Web.Requirements;
using IdentityProject.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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
builder.Services.AddScoped<IClaimsTransformation,UserClaimProvider>();

//policy bazlý yetkilendirme yaparkan yazýlan business kodu için dependancy injection
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpirationRequirementHandler>();

/*
 Cliam Bazlý Yetkilendirme
 Claim bazlý yetkilendirme iþlemleri policy yapýlarý ile yapýlýr aþaðýdaki policy yapýsý gösterilmiþtir.Daha önceden
 kullanýcý giriþi yaptýðýnda cookie'de þehir bilgisininde tutulmasý için UserClaimProvider class'ýnda gerekli 
düzenlemeleri yapmýþtýk.Þimdi is bu þehir bilgisini tutan claim'a göre policy oluþtururak claim bazlý yetkilendirme  
yapacaðýz.Aþaðýdaki policynin adý KarabükPolicy'dir.Bu policy kullanýlýrsa giriþi kullanýcýn city claim'ýnýn deðeri 
Karabük deðil ise bu policy ile yetkilendirilmiþ sayfalara eriþimez.Birden fazla þehir eklemek için bu þehirler
3. ,4. parametre olarak verilebilir.
 */
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("KarabükPolicy", policy =>
    {
        policy.RequireClaim("city", "Karabük");
    });


    /*
     *Policy Bazlý yetkilendirme
      -Policy Bazlý yetkilendirmede de claim'lar kullanýlýr ancak claim'lar ile beraber bir business koduda vardýr.Bu business kodlarýný 
       yazdýðýmýz class IAuthorizationRequirement interfacesini implemente etmelidir. Requirements klasöründe oluþturduðumuz class'ta 
       policy bazlý yetkilendirme için business kodumuzu yazdýk.
      -Artýk ExchangeExpireRequrirement classýndaki business kodu çalýþacak ve ExcahngePolicy isimli policy ile yetkilendirilmiþ sayfalara
       business kodundaki koþulu saðlayan kiþiler eriþim yapabilecekl
     */
    options.AddPolicy("ExchangePolicy", policy => {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });

});

/*Ýdentity ile ekleme iþlemi yapýlýrken kullanýlacak rules'lar burada ayarlanabilir.Bir Extention klasörü oluþturup burada 
  static bir class tanýmladýktan sonra  oluþturulan static methoda IServiceCollection türünden bir parametre vererek
  bu methoda servis ekleyebiliriz.
*/
builder.Services.AddIdentityWithExt();

/*
Uygulamanýn herhangi bir yerindeki klasördeki class'lar IFileProvider interfacesini injecte edersek uygulamadaki
herhangi bir klasöre eriþilebilir.Bu kod ile referans olarak IdenityProject.Web klasörü belirlendi. Artýk bu referans
üzerinden alt klasörlere eriþip istenilen kayýt yapýlabilir. Resim gibi dosya içeren inputlar için bu yöntem
belirlenmelidir.
    */
 builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

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
     sayfasýna atacaktýr. Eðer login olupta eriþemediðimiz sayfalar varsa bu sefer AccessDenied sayfasýna atýlýrýz.
     */
    options.LoginPath = new PathString("/home/SignIn");

    /*Cookie'ye çýkýþ yap fonksiyonunu tanýttýðýmýzda html tarafýnda asp-route-returnurl="/Home/Index" gibi  taglar ile çýkýþ yaptýktan
     sonra hangi sayfaya yönlendiriliceðini belirleyebiliriz. Method içinde rediractionAction kullanmadýðýmýzdan birden fazla 
     çýkýþ yap butonu kullanýlan bir web sitesinde butonun yerine göre farklý sayfalara yönlendirme yapýlabilir.*/
    options.LogoutPath = new PathString("/Member/LogOut");
    
    //Bir kullanýcý eriþimi olmayan bir sayfaya ulaþmak isterse AccessDenied sayfasýna yönlendirilir.
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");
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
