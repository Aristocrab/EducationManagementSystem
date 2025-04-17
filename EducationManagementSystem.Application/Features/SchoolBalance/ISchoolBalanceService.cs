using EducationManagementSystem.Application.Features.Auth.Models;

namespace EducationManagementSystem.Application.Features.SchoolBalance;

public interface ISchoolBalanceService
{
    Task<decimal> GetSchoolBalance(User currentUser);
}