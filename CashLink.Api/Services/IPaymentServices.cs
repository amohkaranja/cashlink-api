using CashLink.Api.Models;
namespace  CashLink.Api.Services;
public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(CreatePaymentRequest request);
    Task<Payment?> GetPaymentAsync(int id);
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task<Payment> UpdatePaymentStatusAsync(int id, string status);
}