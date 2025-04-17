using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Auth.Dtos;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.PasswordHashing;
using EducationManagementSystem.Application.Features.Teachers.Dtos;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Throw;

namespace EducationManagementSystem.Application.Features.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IClock _clock;

    public AuthService(AppDbContext context, 
        IConfiguration configuration,
        IPasswordHashingService passwordHashingService,
        IClock clock)
    {
        _context = context;
        _configuration = configuration;
        _passwordHashingService = passwordHashingService;
        _clock = clock;
    }
    
    private string GenerateJwtToken(Guid teacherId, Role role)
    {
        var claims = new List<Claim>
        {
            new("teacherId", teacherId.ToString()),
            new("role", role.ToString())
        };

        var secretBytes = Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"] ?? "");
        var key = new SymmetricSecurityKey(secretBytes);
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            audience: _configuration["JwtAudience"],
            issuer: _configuration["JwtIssuer"],
            claims: claims,
            notBefore: _clock.Now,
            signingCredentials: signingCredentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }
    
    public async Task<TeacherDto> GetCurrentUser(Guid currentUserId)
    {
        var currentUser = await _context.Teachers
            .AsNoTracking()
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Student)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == currentUserId);
        
        currentUser.ThrowIfNull(_ => new UnauthorizedException("You are not logged in"));
        
        return currentUser.Adapt<TeacherDto>();
    }
    
    public async Task<string> Login(LoginDto loginDto)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Username == loginDto.Username);
        teacher.ThrowIfNull(_ => new NotFoundException("Teacher not found"));

        var passwordHash = _passwordHashingService.HashPassword(loginDto.Password, teacher.PasswordSalt);
        if (passwordHash == teacher.PasswordHash)
        {
            return GenerateJwtToken(teacher.Id, teacher.Role);
        }

        throw new WrongPasswordException("Wrong password");
    }
}