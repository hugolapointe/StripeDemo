using Microsoft.EntityFrameworkCore;

using StripeDemo.Data;
using StripeDemo.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StoreContext>(options => {
    options.UseInMemoryDatabase("StoreStripeDb");
});

builder.Services.Configure<StripeOptions>(options => {
    builder.Configuration.GetSection("StripeOptions").Bind(options);
});

var app = builder.Build();
DataSeeder.Seed(app);

if (!app.Environment.IsDevelopment()) {
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
