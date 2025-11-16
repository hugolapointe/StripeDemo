using Microsoft.EntityFrameworkCore;
using StripeDemo.Data;
using StripeDemo.Models;
using StripeDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseInMemoryDatabase("StoreStripeDb");
});

builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection("StripeOptions"));

builder.Services.AddScoped<IStripeService, StripeService>();

var app = builder.Build();

DataSeeder.Seed(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
