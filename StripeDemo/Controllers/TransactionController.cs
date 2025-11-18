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
        [FromBody] PaymentIntentRequest request
    ) {
        
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var paymentIntent = await stripeService.CreatePaymentIntentAsync(
            request.Amount,
            stripeOptions.Value.CurrencyCode
        );

        var transaction = new Transaction(
            request.CustomerName,
            request.CustomerEmail,
            request.Amount) {
            PaymentIntentId = paymentIntent.Id
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
        [FromBody] VerifyPaymentRequest request
    ) {
        
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var transaction = await context.Transactions.FindAsync(request.TransactionId);

        if (transaction is null) {
            return NotFound();
        }

        if (transaction.PaymentIntentId != request.PaymentIntentId) {
            return BadRequest();
        }

        var paymentIntent = await stripeService.GetPaymentIntentAsync(request.PaymentIntentId);

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
            transaction.CustomerName,
            transaction.CustomerEmail,
            transaction.Amount,
            transaction.Status.ToString(),
            transaction.DateCreated
        );

        return View(viewModel);
    }
}