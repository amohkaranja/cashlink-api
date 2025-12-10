using CashLink.Api.Models;

namespace CashLink.Api.Services;
public class PaymentService : IPaymentService
{
    private  static readonly List<Payment> _payments = new();
    private static int _nextId = 1;
    public Task<Payment> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var payment = new Payment
        {
            Id = _nextId++,
            TransactionRef = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{_nextId}",
            SenderAccount = request.SenderAccount,
            ReceiverAccount = request.ReceiverAccount,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _payments.Add(payment);
        
        // Simulate processing
        Task.Run(async () =>
        {
            await Task.Delay(2000);
            payment.Status = "Completed";
        });

        return Task.FromResult(payment);
    }
    public Task<Payment?> GetPaymentAsync(int id)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(payment);
    }
    public Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return Task.FromResult(_payments.AsEnumerable());
    }
    public Task<Payment> UpdatePaymentStatusAsync(int id, string status)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == id);
        if (payment == null)
            throw new Exception("Payment not found");

        payment.Status = status;
        return Task.FromResult(payment);
    }
}