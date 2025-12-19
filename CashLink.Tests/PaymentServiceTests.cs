using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CashLink.Api.Services;
using CashLink.Api.Models;
using CashLink.Api.Data;

namespace CashLink.Tests;

public class PaymentServiceTests : IDisposable
{
    private readonly CashLinkDbContext _context;
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<CashLinkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CashLinkDbContext(options);
        
        var logger = new LoggerFactory().CreateLogger<PaymentService>();
        _paymentService = new PaymentService(_context, logger);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
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
        Assert.True(payment.Id > 0); // Verify it was saved to DB
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
}
