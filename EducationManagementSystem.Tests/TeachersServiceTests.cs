using FluentAssertions;
using FluentValidation;
using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Auth.Dtos;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.PasswordHashing;
using EducationManagementSystem.Application.Features.Teachers;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models;
using EducationManagementSystem.Tests.Shared;


using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace EducationManagementSystem.Tests;

public class TeachersServiceTests : ServiceTests
{
    private readonly IClock _clock = new KyivTimeClock();
    
    private TeachersService GetTeachersService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().
            UseSqlite($"Data Source = {nameof(TeachersServiceTests)}.db")
            .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var teacher = new Teacher
        {
            Id = Admin.Id,
            FullName = "Bob",
            Username = "bobbob",
            Balance = 100,
            PasswordHash = "",
            PasswordSalt = "",
            Role = Role.Admin,
            RegisteredAt = _clock.Now,
            WorkingHours = ""
        };
        dbContext.Teachers.Add(teacher);
        dbContext.SaveChanges();

        var validator = Substitute.For<IValidator<RegisterDto>>();

        return new TeachersService(dbContext, validator, new PasswordHashingService());
    }

    [Fact]
    public async Task GetAllTeachers_ShouldReturnAllTeachers()
    {
        // Arrange
        var teachersService = GetTeachersService();

        // Act
        var teachers = await teachersService.GetAllTeachers(Admin);

        // Assert
        teachers.Should().NotBeNull();
        teachers.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetTeacherById_ShouldReturnTeacherDto()
    {
        // Arrange
        var teachersService = GetTeachersService();

        // Act
        var teacherDto = await teachersService.GetTeacherById(Admin.Id, Admin);

        // Assert
        teacherDto.Should().NotBeNull();
        teacherDto.Id.Should().Be(Admin.Id);
    }
}