using EducationManagementSystem.Application.Shared.PasswordHashing;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducationManagementSystem.Application.Database;

public class DatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHashingService _passwordHashingService;

    public DatabaseSeeder(AppDbContext dbContext, IConfiguration configuration, IPasswordHashingService passwordHashingService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _passwordHashingService = passwordHashingService;
    }
    
    public void FillAdmins()
    {
        _dbContext.Database.Migrate();
        
        if (!_dbContext.Schools.Any())
        {
            _dbContext.Schools.Add(new School
            {
                SchoolName = "KPI"
            });
            _dbContext.SaveChanges();
        }
        
        if (_dbContext.Teachers.Any()) return;
        
        var adminsFromConfig = _configuration
            .GetSection("Admins")
            .Get<List<Dictionary<string, string>>>()!;
        
        var admins = new List<Teacher>();
        foreach (var admin in adminsFromConfig)
        {        
            var (hash, salt) = _passwordHashingService.HashPassword(admin["Password"]);
            var teacher = new Teacher
            {
                Id = Guid.Parse(admin["Id"]),
                Role = Role.Admin,
                FullName = admin["FullName"],
                Username = admin["Username"],
                PasswordHash = hash,
                PasswordSalt = salt,
                RegisteredAt = DateTime.UtcNow
            };
            admins.Add(teacher);
        }
        
        _dbContext.Teachers.AddRange(admins);
        _dbContext.SaveChanges();
    }
}