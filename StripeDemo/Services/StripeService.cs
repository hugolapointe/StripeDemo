using Microsoft.Extensions.Options;

using Stripe;

using StripeDemo.Models;

namespace StripeDemo.Services;

public class StripeService(
    IOptions<StripeOptions> stripeOptions
) : IStripeService {

    private readonly string ApiKey = stripeOptions.Value.SecretKey;

    public async Task<PaymentIntent> CreatePaymentIntentAsync(
        decimal amount, string currency) {

        var options = new PaymentIntentCreateOptions {
            Amount = (long)(amount * 100),
            Currency = currency.ToLowerInvariant(),
        };

        var requestOptions = new RequestOptions { ApiKey = ApiKey };
        var service = new PaymentIntentService();

        return await service.CreateAsync(options, requestOptions);
    }


    public async Task<PaymentIntent> GetPaymentIntentAsync(
        string paymentIntentId) {

        var requestOptions = new RequestOptions { ApiKey = ApiKey };
        var service = new PaymentIntentService();

        return await service.GetAsync(paymentIntentId, null, requestOptions);
    }
}
