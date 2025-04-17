using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.Payments.Models;

namespace EducationManagementSystem.Application.Features.Payments;

public interface IPaymentsService
{
    Task<decimal> GetCardBalance(User currentUser);
    Task<Dictionary<string, decimal>> GetCardBalanceDetails(User currentUser);
    Task<List<PaymentDto>> GetPayments(User currentUser);
    Task AddPayment(WebhookEvent newPayment);
    Task AddFopPayment(WebhookFopEvent newPayment);
    Task ConfirmPayment(Guid paymentId, User currentUser);
    Task ConfirmPayment(Guid paymentId, Guid studentId, User currentUser);
    Task DeletePayment(Guid paymentId, User currentUser);
    Task<string> GetPaymentLink(Guid studentId, decimal amount);
}