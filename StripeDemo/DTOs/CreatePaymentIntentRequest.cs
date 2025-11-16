using System.ComponentModel.DataAnnotations;

namespace StripeDemo.DTOs;

public record CreatePaymentIntentRequest(

    [Required]
    string CustomerName,

    [Required]
    [EmailAddress]
    string CustomerEmail,

    [Required]
    int ProductId
);
