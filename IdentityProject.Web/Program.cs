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

//policy bazl� yetkilendirme yaparkan yaz�lan business kodu i�in dependancy injection
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpirationRequirementHandler>();

/*
 Cliam Bazl� Yetkilendirme
 Claim bazl� yetkilendirme i�lemleri policy yap�lar� ile yap�l�r a�a��daki policy yap�s� g�sterilmi�tir.Daha �nceden
 kullan�c� giri�i yapt���nda cookie'de �ehir bilgisininde tutulmas� i�in UserClaimProvider class'�nda gerekli 
d�zenlemeleri yapm��t�k.�imdi is bu �ehir bilgisini tutan claim'a g�re policy olu�tururak claim bazl� yetkilendirme  
yapaca��z.A�a��daki policynin ad� Karab�kPolicy'dir.Bu policy kullan�l�rsa giri�i kullan�c�n city claim'�n�n de�eri 
Karab�k de�il ise bu policy ile yetkilendirilmi� sayfalara eri�imez.Birden fazla �ehir eklemek i�in bu �ehirler
3. ,4. parametre olarak verilebilir.
 */
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Karab�kPolicy", policy =>
    {
        policy.RequireClaim("city", "Karab�k");
    });


    /*
     *Policy Bazl� yetkilendirme
      -Policy Bazl� yetkilendirmede de claim'lar kullan�l�r ancak claim'lar ile beraber bir business koduda vard�r.Bu business kodlar�n� 
       yazd���m�z class IAuthorizationRequirement interfacesini implemente etmelidir. Requirements klas�r�nde olu�turdu�umuz class'ta 
       policy bazl� yetkilendirme i�in business kodumuzu yazd�k.
      -Art�k ExchangeExpireRequrirement class�ndaki business kodu �al��acak ve ExcahngePolicy isimli policy ile yetkilendirilmi� sayfalara
       business kodundaki ko�ulu sa�layan ki�iler eri�im yapabilecekl
     */
    options.AddPolicy("ExchangePolicy", policy => {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });

});

/*�dentity ile ekleme i�lemi yap�l�rken kullan�lacak rules'lar burada ayarlanabilir.Bir Extention klas�r� olu�turup burada 
  static bir class tan�mlad�ktan sonra  olu�turulan static methoda IServiceCollection t�r�nden bir parametre vererek
  bu methoda servis ekleyebiliriz.
*/
builder.Services.AddIdentityWithExt();

/*
Uygulaman�n herhangi bir yerindeki klas�rdeki class'lar IFileProvider interfacesini injecte edersek uygulamadaki
herhangi bir klas�re eri�ilebilir.Bu kod ile referans olarak IdenityProject.Web klas�r� belirlendi. Art�k bu referans
�zerinden alt klas�rlere eri�ip istenilen kay�t yap�labilir. Resim gibi dosya i�eren inputlar i�in bu y�ntem
belirlenmelidir.
    */
 builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

//cookie ayarlar� buradan yap�l�r.
builder.Services.ConfigureApplicationCookie(options =>
{
	/*
	  RememberMe i�aretlenirse bu  60 g�n taray�c�da kay�tl� olur yani SignIn fonksiyonundaki PasswordSignInAsync methodunda
      3. parametre true gelmelidir. E�er false geliyorsa olu�an cookie taray�c� a��k oldu�u s�rece var olur.
    */
	var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "AppCookie";

    /*
     Login olmadan girilemeyen sayfalar�n url'ine girmeye �al���rsak  bu kod sayesinde identity bizi Belirledi�imiz bu login 
     sayfas�na atacakt�r. E�er login olupta eri�emedi�imiz sayfalar varsa bu sefer AccessDenied sayfas�na at�l�r�z.
     */
    options.LoginPath = new PathString("/home/SignIn");

    /*Cookie'ye ��k�� yap fonksiyonunu tan�tt���m�zda html taraf�nda asp-route-returnurl="/Home/Index" gibi  taglar ile ��k�� yapt�ktan
     sonra hangi sayfaya y�nlendirilice�ini belirleyebiliriz. Method i�inde rediractionAction kullanmad���m�zdan birden fazla 
     ��k�� yap butonu kullan�lan bir web sitesinde butonun yerine g�re farkl� sayfalara y�nlendirme yap�labilir.*/
    options.LogoutPath = new PathString("/Member/LogOut");
    
    //Bir kullan�c� eri�imi olmayan bir sayfaya ula�mak isterse AccessDenied sayfas�na y�nlendirilir.
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.SlidingExpiration = true;

}

);


var app = builder.Build();


/*
E�er proje development ortam�nda de�ilse f�rlat�lan hatalar error sayfas�na y�nlendirilir.Development modunda ise framework�n 
verece�i hata sayfas�na g�nderilir
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



//bir area olu�turuldu�unda bu �ekilde program.cs eklenir

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
