using Microsoft.AspNetCore.Mvc;

using StripeDemo.Data;
using StripeDemo.ViewModels;

namespace StripeDemo.Controllers;

[Route("product")]
public class ProductController(StoreContext context) : Controller {
    private readonly StoreContext Context = context;

    [HttpGet("details/{Id}")]
    public IActionResult Details(int id) {
        var product = Context.Products.Find(id);

        if (product is null) {
            return NotFound();
        }

        return View(new ProductViewModels(
            product.Id,
            product.Name,
            product.Description,
            product.Price
        ));
    }
}
