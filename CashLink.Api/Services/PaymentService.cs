using System.Data;
using Dapper;
using CashLink.Api.Models;

namespace CashLink.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IDbConnectionFactory connectionFactory, ILogger<PaymentService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var payment = new Payment
        {
            TransactionRef = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString()[..8].ToUpper()}",
            SenderAccount = request.SenderAccount,
            ReceiverAccount = request.ReceiverAccount,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        const string sql = @"
            INSERT INTO Payments (TransactionRef, SenderAccount, ReceiverAccount, Amount, Currency, Status, CreatedAt, Description)
            VALUES (@TransactionRef, @SenderAccount, @ReceiverAccount, @Amount, @Currency, @Status, @CreatedAt, @Description);
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        ";

        var connection = _connectionFactory.CreateConnection();
        payment.Id = await connection.ExecuteScalarAsync<int>(sql, payment);

        _logger.LogInformation("Payment created: {TransactionRef} with ID: {Id}", payment.TransactionRef, payment.Id);

        // Simulate async processing
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            await UpdatePaymentStatusAsync(payment.Id, "Completed");
        });

        return payment;
    }

    public async Task<Payment?> GetPaymentAsync(int id)
    {
        const string sql = "SELECT * FROM Payments WHERE Id = @Id";

        var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        const string sql = "SELECT * FROM Payments ORDER BY CreatedAt DESC";

        var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Payment>(sql);
    }

    public async Task<Payment> UpdatePaymentStatusAsync(int id, string status)
    {
        const string sql = @"
            UPDATE Payments 
            SET Status = @Status 
            WHERE Id = @Id;
            
            SELECT * FROM Payments WHERE Id = @Id;
        ";

        var connection = _connectionFactory.CreateConnection();
        var payment = await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { Id = id, Status = status });

        if (payment == null)
            throw new Exception("Payment not found");

        _logger.LogInformation("Payment {Id} status updated to {Status}", id, status);

        return payment;
    }
}
