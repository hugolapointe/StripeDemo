using System.ComponentModel.DataAnnotations;

namespace StripeDemo.DTOs;

public record VerifyPaymentRequest(

    [Required]
    int TransactionId,

    [Required]
    string PaymentIntentId
);
