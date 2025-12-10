namespace CashLink.Api.Models;
public class Payment
{
    public int Id { get; set; }
    public string TransactionRef { get; set; } = string.Empty;
    public string SenderAccount { get; set; } = string.Empty;
    public string ReceiverAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "KES";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
}

public class CreatePaymentRequest
{
    public string SenderAccount { get; set; } = string.Empty;
    public string ReceiverAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "KES";
    public string? Description { get; set; }
}

public class PaymentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Payment? Data { get; set; }
}