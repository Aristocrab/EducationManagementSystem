using EducationManagementSystem.Application.Features.SchoolBalance;
using EducationManagementSystem.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EducationManagementSystem.WebApi.Controllers;

[Route("schoolBalance")]
public class SchoolBalanceController : BaseController
{
    private readonly ISchoolBalanceService _schoolBalanceService;

    public SchoolBalanceController(ISchoolBalanceService schoolBalanceService)
    {
        _schoolBalanceService = schoolBalanceService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Get school balance")]
    public async Task<decimal> GetSchoolBalance()
    {
        return await _schoolBalanceService.GetSchoolBalance(CurrentUser);
    }
}