using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Dapper;
using CashLink.Api.Services;
using CashLink.Api.Models;
using System.Data;

namespace CashLink.Tests;

// SQLite connection factory that keeps connection open
public class TestConnectionFactory : IDbConnectionFactory
{
    private readonly IDbConnection _connection;

    public TestConnectionFactory(IDbConnection connection)
    {
        _connection = connection;
    }

    public IDbConnection CreateConnection()
    {
        // Always return the SAME open connection
        // Don't open/close it - keep it alive for the test
        return _connection;
    }
}

public class PaymentServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests()
    {
        // Create in-memory connection and KEEP IT OPEN
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Create table
        _connection.Execute(@"
            CREATE TABLE Payments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TransactionRef TEXT NOT NULL UNIQUE,
                SenderAccount TEXT NOT NULL,
                ReceiverAccount TEXT NOT NULL,
                Amount REAL NOT NULL,
                Currency TEXT NOT NULL,
                Status TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                Description TEXT
            );
        ");

        var logger = new LoggerFactory().CreateLogger<PaymentService>();
        var connectionFactory = new TestConnectionFactory(_connection);
        _paymentService = new PaymentService(connectionFactory, logger);
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnPayment()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            SenderAccount = "254712345678",
            ReceiverAccount = "254787654321",
            Amount = 1000,
            Currency = "KES",
            Description = "Test payment"
        };

        // Act
        var payment = await _paymentService.CreatePaymentAsync(request);

        // Assert
        Assert.NotNull(payment);
        Assert.Equal(request.SenderAccount, payment.SenderAccount);
        Assert.Equal(request.ReceiverAccount, payment.ReceiverAccount);
        Assert.Equal(request.Amount, payment.Amount);
        Assert.Equal("Pending", payment.Status);
        Assert.True(payment.Id > 0);
        Assert.NotEmpty(payment.TransactionRef);
    }

    [Fact]
    public async Task GetPayment_ShouldReturnPayment_WhenExists()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            SenderAccount = "254712345678",
            ReceiverAccount = "254787654321",
            Amount = 500,
            Currency = "KES"
        };
        var created = await _paymentService.CreatePaymentAsync(request);

        // Act
        var payment = await _paymentService.GetPaymentAsync(created.Id);

        // Assert
        Assert.NotNull(payment);
        Assert.Equal(created.Id, payment.Id);
        Assert.Equal(created.TransactionRef, payment.TransactionRef);
    }

    [Fact]
    public async Task GetPayment_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var payment = await _paymentService.GetPaymentAsync(9999);

        // Assert
        Assert.Null(payment);
    }

    [Fact]
    public async Task GetAllPayments_ShouldReturnMultiplePayments()
    {
        // Arrange
        await _paymentService.CreatePaymentAsync(new CreatePaymentRequest
        {
            SenderAccount = "254711111111",
            ReceiverAccount = "254722222222",
            Amount = 1000,
            Currency = "KES"
        });

        await _paymentService.CreatePaymentAsync(new CreatePaymentRequest
        {
            SenderAccount = "254733333333",
            ReceiverAccount = "254744444444",
            Amount = 2000,
            Currency = "KES"
        });

        // Act
        var payments = await _paymentService.GetAllPaymentsAsync();

        // Assert
        Assert.NotNull(payments);
        Assert.Equal(2, payments.Count());
    }

    [Fact]
    public async Task UpdatePaymentStatus_ShouldUpdateStatus()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            SenderAccount = "254712345678",
            ReceiverAccount = "254787654321",
            Amount = 1000,
            Currency = "KES"
        };
        var created = await _paymentService.CreatePaymentAsync(request);

        // Act
        var updated = await _paymentService.UpdatePaymentStatusAsync(created.Id, "Completed");

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Completed", updated.Status);
    }

    [Fact]
    public async Task UpdatePaymentStatus_ShouldThrowException_WhenNotExists()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _paymentService.UpdatePaymentStatusAsync(9999, "Completed")
        );
    }
}
