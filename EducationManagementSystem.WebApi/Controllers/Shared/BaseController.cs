using System.Security.Claims;
using EducationManagementSystem.Application.Shared.Auth.Models;
using EducationManagementSystem.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Controllers.Shared;

[ApiController]
[Route("api")]
public class BaseController : ControllerBase
{
    private Guid CurrentUserId => GetCurrentUserId();
    private Role? CurrentUserRole => GetCurrentUserRole();
    protected User CurrentUser => new(CurrentUserId, CurrentUserRole);

    private Guid GetCurrentUserId()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
        {
            return Guid.Empty;
        }

        var userClaims = identity.Claims.ToArray();
        if (userClaims.Length == 0)
        {
            return Guid.Empty;
        }

        return Guid.Parse(userClaims.First(x => x.Type == "teacherId").Value);
    }
    
    private Role? GetCurrentUserRole()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
        {
            return null;
        }

        var userClaims = identity.Claims.ToArray();
        if (userClaims.Length == 0)
        {
            return null;
        }

        return identity.Claims.First(x => x.Type == ClaimTypes.Role).Value switch
        {
            "Admin" => Role.Admin,
            "Moderator" => Role.Moderator,
            _ => Role.Teacher
        };
    }
}