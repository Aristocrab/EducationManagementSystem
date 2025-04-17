using FluentAssertions;
using FluentValidation;
using EducationManagementSystem.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using EducationManagementSystem.Application.Features.Auth;
using EducationManagementSystem.Application.Features.Auth.Dtos;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.PasswordHashing;
using EducationManagementSystem.Application.Features.Teachers;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Exceptions;
using EducationManagementSystem.Core.Models;
using EducationManagementSystem.Tests.Shared;

namespace EducationManagementSystem.Tests;

public class AuthServiceTests : ServiceTests
{
    private Teacher _teacher = null!;
    private const string TeacherPassword = "qwertyuiop";
    private AppDbContext _dbContext = null!;
    private readonly IClock _clock = new KyivTimeClock();

    private AuthService GetAuthService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().
            UseSqlite($"Data Source = {nameof(AuthServiceTests)}.db")
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        var passwordHashingService = new PasswordHashingService();
        var (hash, salt) = passwordHashingService.HashPassword(TeacherPassword);
        _teacher = new Teacher
        {
            Id = Admin.Id,
            FullName = "Bob",
            Username = "bobbob",
            Balance = 100,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = Role.Teacher,
            RegisteredAt = _clock.Now,
            WorkingHours = ""
        };
        _dbContext.Teachers.Add(_teacher);
        _dbContext.SaveChanges();
        
        var configuration = Substitute.For<IConfiguration>();
        configuration["JwtSecretKey"].Returns("JWT_SECRET_KEY_1234567890987654321");
        configuration["JwtAudience"].Returns("JWT_AUDIENCE");
        configuration["JwtIssuer"].Returns("JWT_ISSUER");

        return new AuthService(_dbContext, configuration, new PasswordHashingService(), _clock);
    }
    
    private TeachersService GetTeachersService()
    {
        _dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nameof(AuthServiceTests))
            .Options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        var passwordHashingService = new PasswordHashingService();
        var (hash, salt) = passwordHashingService.HashPassword(TeacherPassword);
        _teacher = new Teacher
        {
            Id = Admin.Id,
            FullName = "Bob",
            Username = "bobbob",
            Balance = 100,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = Role.Teacher,
            RegisteredAt = _clock.Now,
            WorkingHours = ""
        };
        _dbContext.Teachers.Add(_teacher);
        _dbContext.SaveChanges();

        var validator = Substitute.For<IValidator<RegisterDto>>();
        
        return new TeachersService(_dbContext, validator, new PasswordHashingService());
    }

    [Fact]
    public async Task GetCurrentUser_WhenValidUserId_ShouldReturnTeacherDto()
    {
        // Arrange
        var authService = GetAuthService();

        // Act
        var result = await authService.GetCurrentUser(_teacher.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WhenValidCredentials_ShouldReturnJwtToken()
    {
        // Arrange
        var authService = GetAuthService();

        // Act
        var result = await authService.Login(new LoginDto { Username = _teacher.Username, Password = TeacherPassword });

        // Assert
        result.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task RegisterTeacher_WhenValidData_ShouldReturnJwtToken()
    {
        // Arrange
        var teachersService = GetTeachersService();
        var registerDto = new RegisterDto
        {
            FullName = "Bobert",
            Username = "bobert",
            Password = "qwertyuiop",
            WorkingHours = "10:00-21:00"
        };
        
        // Act
        await teachersService.RegisterTeacher(registerDto, Admin);
    
        // Assert
        var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Username == registerDto.Username);
        teacher.Should().NotBeNull();
        teacher!.Role.Should().Be(Role.Teacher);
    }
    
    [Fact]
    public async Task RegisterModerator_WhenValidData_ShouldReturnJwtToken()
    {
        // Arrange
        var teachersService = GetTeachersService();
        var registerDto = new RegisterDto
        {
            FullName = "Bobert",
            Username = "bobert",
            Password = "qwertyuiop",
            WorkingHours = "10:00-21:00"
        };
        
        // Act
        await teachersService.RegisterTeacher(registerDto, Admin, Role.Moderator);
    
        // Assert
        var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Username == registerDto.Username);
        teacher.Should().NotBeNull();
        teacher!.Role.Should().Be(Role.Moderator);
    }
    
    [Fact]
    public async Task EditTeacherWorkingHours_WhenValidData_ShouldReturnJwtToken()
    {
        // Arrange
        var teachersService = GetTeachersService();
        var registerDto = new RegisterDto
        {
            FullName = "Bobert",
            Username = "bobert",
            Password = "qwertyuiop",
            WorkingHours = "10:00-21:00"
        };
        await teachersService.RegisterTeacher(registerDto, Admin);
    
        // Act
        var teacher = await _dbContext.Teachers.FirstAsync(t => t.Username == registerDto.Username);
        var action = async () => 
            await teachersService.EditTeacherWorkingHours(teacher.Id, "10:00-15:30", Admin);
        
        // Assert
        await action.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task EditTeacherBalance_WhenValidData_ShouldUpdateTeacherBalance()
    {
        // Arrange
        var teachersService = GetTeachersService();
    
        // Act
        await teachersService.EditTeacherBalance(_teacher.Id, 100, Admin);
    
        // Assert
        var updatedTeacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == _teacher.Id);
        updatedTeacher.Should().NotBeNull();
        updatedTeacher!.Balance.Should().Be(100);
    }
    
    [Fact]
    public async Task DeleteTeacher_WhenValidTeacherId_ShouldDeleteTeacher()
    {
        // Arrange
        var teachersService = GetTeachersService();
    
        // Act
        await teachersService.DeleteTeacher(_teacher.Id, Admin);
    
        // Assert
        var deletedTeacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == _teacher.Id);
        deletedTeacher.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteTeacher_WhenTeacherIsAdmin_ShouldThrowException()
    {
        // Arrange
        var teachersService = GetTeachersService();
        var admin = new Teacher
        {
            Id = Guid.NewGuid(),
            FullName = "Bob",
            Username = "bobbob",
            Balance = 100,
            PasswordHash = "",
            PasswordSalt = "",
            Role = Role.Admin,
            RegisteredAt = _clock.Now,
            WorkingHours = ""
        };
        _dbContext.Teachers.Add(admin);
        await _dbContext.SaveChangesAsync();
    
        // Act
        var act = async () => await teachersService.DeleteTeacher(admin.Id, Admin);
    
        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}