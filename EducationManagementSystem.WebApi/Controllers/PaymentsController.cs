using EducationManagementSystem.Application.Features.Payments;
using EducationManagementSystem.Application.Features.Payments.Models;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

public class PaymentsController : BaseController
{
    private readonly IPaymentsService _paymentsService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentsService paymentsService, ILogger<PaymentsController> logger)
    {
        _paymentsService = paymentsService;
        _logger = logger;
    }
    
    [HttpGet("/webhook")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Webhook()
    {
        return Ok();
    }
    
    [HttpPost("/webhook")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Webhook([FromBody] WebhookEvent webhookEvent)
    {
        try
        {
            await _paymentsService.AddPayment(webhookEvent);
        }
        catch
        {
            // ignored
        }
        
        return Ok();
    }
    
    [HttpPost("/webhookFop")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> WebhookFop([FromBody] WebhookFopEvent webhookEvent)
    {
        await _paymentsService.AddFopPayment(webhookEvent);
        
        return Ok();
    }
    
    [HttpGet("/cardBalance")]
    [SwaggerOperation(Summary = "Get card balance")]
    // [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    public async Task<decimal> GetCardBalance()
    {
        var cardBalance = await _paymentsService.GetCardBalance(CurrentUser);

        return cardBalance;
    }
    
    [HttpGet("/cardBalanceDetails")]
    [SwaggerOperation(Summary = "Get card balance")]
    // [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    public async Task<Dictionary<string, decimal>> GetCardBalanceDetails()
    {
        var cardBalance = await _paymentsService.GetCardBalanceDetails(CurrentUser);

        return cardBalance;
    }
    
    [HttpGet("/payments")]
    [SwaggerOperation(Summary = "Get all payments (paid and unpaid)")]
    public async Task<List<PaymentDto>> GetPayments()
    {
        var payments = await _paymentsService.GetPayments(CurrentUser);

        return payments;
    }
    
    [HttpPost("/payments/{paymentId}/confirm")]
    [SwaggerOperation(Summary = "Just confirm payment without changing lessons' statuses")]
    public async Task ConfirmPayment(Guid paymentId)
    {
        await _paymentsService.ConfirmPayment(paymentId, CurrentUser);
    }
    
    [HttpPost("/payments/{paymentId}/confirm/{studentId}")]
    [SwaggerOperation(Summary = "Confirm payment and make all unpaid lessons paid")]
    public async Task ConfirmPayment(Guid paymentId, Guid studentId)
    {
        await _paymentsService.ConfirmPayment(paymentId, studentId, CurrentUser);
    }
    
    [HttpDelete("/payments/{paymentId}/delete")]
    [SwaggerOperation(Summary = "Delete payment")]
    public async Task DeletePayment(Guid paymentId)
    {
        await _paymentsService.DeletePayment(paymentId, CurrentUser);
    }
    
    [HttpGet("/payments/getPaymentLink/{studentId}/{amount}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    public async Task<string> GetPaymentLink(Guid studentId, int amount)
    {
        return await _paymentsService.GetPaymentLink(studentId, amount);
    }
}