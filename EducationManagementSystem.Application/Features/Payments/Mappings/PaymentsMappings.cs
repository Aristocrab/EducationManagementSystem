using EducationManagementSystem.Application.Features.Payments.Models;
using EducationManagementSystem.Core.Models;
using Mapster;

namespace EducationManagementSystem.Application.Features.Payments.Mappings;

public class PaymentsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment, PaymentDto>()
            .Map(dest => dest.Name,
                src => GetStudentName(src));
    }
    
    private static string GetStudentName(Payment payment)
    {
        // "Від: Марина" -> "Марина"
        return payment.Description.StartsWith("Від: ") 
            ? payment.Description[4..]
            : payment.Description;
    }
}