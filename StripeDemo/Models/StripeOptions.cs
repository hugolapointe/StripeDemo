using System.ComponentModel.DataAnnotations;

namespace StripeDemo.Models;

public class StripeOptions {
    [Required]
    public string PublicKey { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string CurrencyCode { get; set; } = "cad";
}
