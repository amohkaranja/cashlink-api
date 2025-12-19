using Microsoft.AspNetCore.Mvc;
using CashLink.Api.Models;
using CashLink.Api.Services;
namespace CashLink.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;
    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;  
    }
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            _logger.LogInformation("Creating payment from {Sender} to {Receiver} for {Amount} {Currency}", 
                request.SenderAccount, request.ReceiverAccount, request.Amount, request.Currency);

            var payment = await _paymentService.CreatePaymentAsync(request);

            return Ok(new PaymentResponse
            {
                Success = true,
                Message = "Payment created successfully",
                Data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return BadRequest(new PaymentResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentResponse>> GetPayment(int id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        
        if (payment == null)
        {
            return NotFound(new PaymentResponse
            {
                Success = false,
                Message = "Payment not found"
            });
        }

        return Ok(new PaymentResponse
        {
            Success = true,
            Message = "Payment retrieved successfully",
            Data = payment
        });
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<PaymentResponse>> UpdatePaymentStatus(int id, [FromBody] string status)
    {
        try
        {
            var payment = await _paymentService.UpdatePaymentStatusAsync(id, status);
            return Ok(new PaymentResponse
            {
                Success = true,
                Message = "Payment status updated",
                Data = payment
            });
        }
        catch (Exception ex)
        {
            return NotFound(new PaymentResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}