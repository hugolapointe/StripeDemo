using Stripe;

namespace StripeDemo.Services;

public interface IStripeService {
    Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency);
    Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId);
}
