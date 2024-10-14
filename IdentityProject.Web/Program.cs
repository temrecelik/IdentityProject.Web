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

/*�dentity ile ekleme i�lemi yap�l�rken kullan�lacak rules'lar burada ayarlanabilir.Bir Extention klas�r� olu�turup burada 
  static bir class tan�mlad�ktan sonra  olu�turulan static methoda IServiceCollection t�r�nden bir parametre vererek
  bu methoda servis ekleyebiliriz.
*/
builder.Services.AddIdentityWithExt();

//cookie ayarlar� buradan yap�l�r.
builder.Services.ConfigureApplicationCookie(options =>
{
	/*
	  RememberMe i�aretlenirse bu  60 g�n taray�c�da kay�tl� olur yani SignIn fonksiyonundaki PasswordSignInAsync methodunda
      3. parametre true gelmelidir. E�er false geliyorsa olu�an cookie taray�c� a��k oldu�u s�rece var olur.
    */
	var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "AppCookie";
    options.LoginPath = new PathString("/home/SignIn");
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.SlidingExpiration = true;

}

);


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
