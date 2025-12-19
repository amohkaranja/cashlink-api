using CashLink.Api.Data;
using CashLink.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CashLink.Api.Services;
public class PaymentService : IPaymentService
{
    private  static readonly List<Payment> _payments = new();
    private static int _nextId = 1;
     private readonly CashLinkDbContext _context;
    private readonly ILogger<PaymentService> _logger;
    public PaymentService(CashLinkDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest request)
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

        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment created: {TransactionRef}", payment.TransactionRef);

        // Simulate async processing
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            payment.Status = "Completed";
            await _context.SaveChangesAsync();
            _logger.LogInformation("Payment completed: {TransactionRef}", payment.TransactionRef);
        });

        return payment;
    }
   public async Task<Payment?> GetPaymentAsync(int id)
    {
        return await _context.Payments.FindAsync(id);
    }

   public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _context.Payments
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

     public async Task<Payment> UpdatePaymentStatusAsync(int id, string status)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            throw new Exception("Payment not found");

        payment.Status = status;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment {Id} status updated to {Status}", id, status);
        
        return payment;
    }
}