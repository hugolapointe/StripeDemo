using Microsoft.AspNetCore.Mvc;

using StripeDemo.Data;
using StripeDemo.ViewModels;

namespace StripeDemo.Controllers;


[Route("product")]
public class ProductController(
    StoreContext context
    ) : Controller {

    [HttpGet("details/{id}")]
    public IActionResult Details(int id) {
        var product = context.Products.Find(id);

        if (product is null) {
            return NotFound();
        }

        var viewModel = new ProductDetails(
            product.Id,
            product.Name,
            product.Description,
            product.Price
        );

        return View(viewModel);
    }
}
