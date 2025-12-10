using Xunit;
using CashLink.Api.Services;
using CashLink.Api.Models;

namespace CashLink.Tests;

public class PaymentServiceTests
{
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentService = new PaymentService();
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
    }
    [Fact]
    public async Task GetPayment_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var payment = await _paymentService.GetPaymentAsync(9999);

        // Assert
        Assert.Null(payment);
    }

    // Additional tests for GetPaymentAsync, GetAllPaymentsAsync, UpdatePaymentStatusAsync can be added here
}