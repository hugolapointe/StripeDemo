using System.ComponentModel.DataAnnotations;

namespace StripeDemo.DTOs;

public record PaymentIntentRequest(

    [Required]
    string CustomerName,

    [Required]
    [EmailAddress]
    string CustomerEmail,

    [Required]
    decimal Amount
);
