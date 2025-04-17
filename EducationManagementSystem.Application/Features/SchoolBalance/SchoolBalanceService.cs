using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.Payments;

namespace EducationManagementSystem.Application.Features.SchoolBalance;

public class SchoolBalanceService : ISchoolBalanceService
{
    private readonly IPaymentsService _paymentsService;

    public SchoolBalanceService(IPaymentsService paymentsService)
    {
        _paymentsService = paymentsService;
    }
    
    public async Task<decimal> GetSchoolBalance(User currentUser)
    {
        return await _paymentsService.GetCardBalance(currentUser);
    }
}