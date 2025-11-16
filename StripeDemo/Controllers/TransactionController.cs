using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using StripeDemo.Data;
using StripeDemo.DTOs;
using StripeDemo.Models;
using StripeDemo.Services;
using StripeDemo.ViewModels;

namespace StripeDemo.Controllers;

[Route("transaction")]
public class TransactionController(
        IStripeService stripeService,
        StoreContext context,
        IOptions<StripeOptions> stripeOptions
) : Controller {


    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent(
        [FromBody] CreatePaymentIntentRequest dto
    ) {
        
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var product = await context.Products.FindAsync(dto.ProductId);

        if (product is null) {
            return NotFound();
        }

        var paymentIntent = await stripeService.CreatePaymentIntentAsync(
            product.Price,
            stripeOptions.Value.CurrencyCode
        );

        var transaction = new Transaction(
            dto.CustomerName,
            dto.CustomerEmail,
            product.Price,
            dto.ProductId) {
            PaymentIntentId = paymentIntent.Id,
            Currency = stripeOptions.Value.CurrencyCode
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        return Ok(new {
            transactionId = transaction.Id,
            clientSecret = paymentIntent.ClientSecret
        });
    }


    [HttpPost("verify-payment")]
    public async Task<IActionResult> VerifyPayment(
        [FromBody] VerifyPaymentRequest dto
    ) {
        
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var transaction = await context.Transactions.FindAsync(dto.TransactionId);

        if (transaction is null) {
            return NotFound();
        }

        if (transaction.PaymentIntentId != dto.PaymentIntentId) {
            return BadRequest();
        }

        var paymentIntent = await stripeService.GetPaymentIntentAsync(dto.PaymentIntentId);

        transaction.Update(paymentIntent.Status);
        await context.SaveChangesAsync();

        return Ok(new {
            transactionId = transaction.Id,
            status = transaction.Status.ToString()
        });
    }


    [HttpGet("confirmation/{transactionId}")]
    public async Task<IActionResult> Confirmation(int transactionId) {
        
        var transaction = await context.Transactions.FindAsync(transactionId);

        if (transaction is null) {
            return NotFound();
        }

        var viewModel = new ConfirmationDetails(
            transaction.Id,
            transaction.Name,
            transaction.Email,
            transaction.Amount,
            transaction.Currency,
            transaction.Status.ToString(),
            transaction.DateCreated
        );

        return View(viewModel);
    }
}