using Refit;

namespace EducationManagementSystem.Application.Features.Payments.Interfaces;

public interface IPaymentLinkApi
{
    [Post("/api/merchant/invoice/create")]
    Task<GetPaymentLinkResponse> GetPaymentLink([Body(buffered: true)] GetPaymentLinkRequest request, [Header("X-Token")] string authorization);
}

public class GetPaymentLinkRequest
{
    public required int Amount { get; set; }
    public required MerchantPaymInfo MerchantPaymInfo { get; set; }
    public required string WebHookUrl { get; set; }
}

public class MerchantPaymInfo
{
    public required string Destination { get; set; }
    public required string Comment { get; set; }
}

public class GetPaymentLinkResponse
{
    public required string InvoiceId { get; set; }
    public required string PageUrl { get; set; }
}