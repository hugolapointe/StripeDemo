using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Stripe;

using StripeDemo.Data;
using StripeDemo.Models;
using StripeDemo.ViewModels;

namespace StripeDemo.Controllers;

[Route("transaction")]
public class TransactionController(IOptions<StripeOptions> stripeOptions, StoreContext context) : Controller {
    private readonly IOptions<StripeOptions> StripeOptions = stripeOptions;
    private readonly StoreContext Context = context;

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentDetails paymentDetails) {

        // Vérifier si le produit existe
        var product = Context.Products.Find(paymentDetails.ProductId);

        if (product is null) {
            return NotFound();
        }

        // Créer les options de paiement (Stripe.Net)
        var options = new PaymentIntentCreateOptions {
            Amount = (int)(product.Price * 100), // Stripe utilise les cents
            Currency = "cad",
            // Plus d'informations pourraient être ajoutées ici
        };

        // Créer l'intention de paiement avec les options (Stripe.Net)
        StripeConfiguration.ApiKey = StripeOptions.Value.SecretKey;
        var service = new PaymentIntentService();
        var paymentIntent = service.Create(options);

        if(paymentIntent is null) {
            return BadRequest();
        }

        // Créer une transaction (interne)
        var transaction = new Transaction(
            paymentDetails.CustomerName, 
            paymentDetails.CustomerEmail, 
            product.Price, 
            product.Id) {
            PaymentIntentId = paymentIntent.Id
        };

        // Enregistrer la transaction (interne)
        Context.Transactions.Add(transaction);
        await Context.SaveChangesAsync();

        // Retourner l'ID de la transaction et le clientSecret du PaymentIntent
        return Ok(new {
            transactionId = transaction.Id, // Optionnel, mais utile pour nos besoins internes
            clientSecret = paymentIntent.ClientSecret // Essentiel pour la confirmation de paiement de Stripe!
        });
    }

    [HttpPost("confirm-payment")]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDetails confirmPaymentDetails) {

        // Récupérer la transaction associée à l'ID
        var transaction = await Context.Transactions.FindAsync(confirmPaymentDetails.TransactionId);

        // Vérifier si la transaction existe
        if(transaction is null) {
            return NotFound();
        }

        // Vérifier si l'ID de paiement correspond à celui de la transaction
        if(transaction.PaymentIntentId != confirmPaymentDetails.PaymentIntentId) {
            return BadRequest();
        }

        // Vérifier la confirmation du paiement (Stripe.Net)
        StripeConfiguration.ApiKey = StripeOptions.Value.SecretKey;
        var service = new PaymentIntentService();

        var paymentIntent = service.Get(confirmPaymentDetails.PaymentIntentId);

        // Mettre à jour la transaction (interne)
        transaction.Update(paymentIntent.Status);

        if(paymentIntent is null || paymentIntent.Status != "succeeded") {
            return BadRequest();
        }

        // Retourner l'ID de la transaction et le statut de la transaction
        return Ok(new {
            transactionId = transaction.Id,
            status = transaction.Status
        });
    }

    [HttpGet("confirmation/{transactionId}")]
    public async Task<IActionResult> Confirmation(int transactionId) {

        // Récupérer la transaction associée à l'ID
        var transaction = await Context.Transactions.FindAsync(transactionId);

        // Vérifier si la transaction existe
        if(transaction is null) {
            return NotFound();
        }

        // Retourner la vue de confirmation avec les détails de la transaction
        ViewData["TransactionId"] = transaction.Id;
        return View(transaction);
    }
}