using StripeDemo.Models;

namespace StripeDemo.Data;

public static class DataSeeder {

    public static void Seed(WebApplication app) {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

        if (context.Products.Any()) {
            return;
        }

        var product = new Product {
            Id = 1,
            Name = "RubberDuck",
            Description = "Rencontrez le Rubber Duck, le compagnon de codage ultime qui boostera votre débogage ! Ce génie en plastique, silencieux mais incroyablement perspicace, transformera vos monologues de programmation en solutions brillantes. N'attendez plus : adoptez un canard, et dites adieu aux bugs ! 🦆💻🚀",
            Price = 10.00m
        };

        context.Products.Add(product);
        context.SaveChanges();
    }
}
