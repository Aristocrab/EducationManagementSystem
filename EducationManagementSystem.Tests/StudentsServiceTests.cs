﻿using FluentAssertions;
using FluentAssertions.Extensions;
using FluentValidation;
using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Students;
using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Shared.Clock;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models;
using EducationManagementSystem.Tests.Shared;


using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace EducationManagementSystem.Tests;

public class StudentsServiceTests : ServiceTests
{
    private Student _student = null!;
    private readonly IClock _clock = new KyivTimeClock();
    private readonly Group _group = new()
    {
        Id = Guid.NewGuid(),
        GroupId = "ІС-12",
        Students = []
    };

    private StudentsService GetStudentsService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().
            UseSqlite($"Data Source = {nameof(StudentsServiceTests)}.db")
            .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        
        _student = new Student
        {
            Id = Admin.Id,
            FullName = "Bob",
            Group = _group
        };
        dbContext.Students.Add(_student);
        
        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            FullName = "Bob",
            Username = "bobbob",
            PasswordHash = "",
            PasswordSalt = "",
            Role = Role.Admin,
            RegisteredAt = _clock.Now,
        };
        dbContext.Teachers.Add(teacher);
        
        var lesson1 = new Lesson
        {
            Student = _student,
            DateTime = _clock.Now,
            Subject = new Subject
            {
                Id = Guid.NewGuid(),
                Title = "Math"
            },
            Description = "Math Lesson",
            Teacher = teacher,
            Duration = 1.Hours()
        };
        var lesson2 = new Lesson
        {
            Student = _student,
            Subject = new Subject
            {
                Id = Guid.NewGuid(),
                Title = "Math"
            },
            DateTime = _clock.Now.AddHours(1),
            Description = "Math Lesson",
            Teacher = teacher,
            Duration = 1.Hours(),
        };
        dbContext.Lessons.AddRange(lesson1, lesson2);
        
        dbContext.SaveChanges();
        
        return new StudentsService(dbContext, Substitute.For<IValidator<NewStudentDto>>());
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnAllStudents()
    {
        // Arrange
        var studentsService = GetStudentsService();

        // Act
        var students = await studentsService.GetAllStudents(Admin);

        // Assert
        students.Should().NotBeNull();
        students.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnStudentDto()
    {
        // Arrange
        var studentsService = GetStudentsService();

        // Act
        var studentDto = await studentsService.GetStudentById(_student.Id);

        // Assert
        studentDto.Should().NotBeNull();
    }

    [Fact]
    public async Task EditStudent_ShouldEditStudent()
    {
        // Arrange
        var studentsService = GetStudentsService();
        
        var newStudentDto = new NewStudentDto { FullName = "Bobert", Languages = [], GroupId = _group.GroupId };
        await studentsService.AddStudent(newStudentDto, Admin);

        // Act
        var updatedStudentDto = new NewStudentDto { FullName = "Bobertinyo", Languages = [], GroupId = _group.GroupId };
        await studentsService.EditStudent(_student.Id, updatedStudentDto, Admin);

        // Assert
        var updatedStudent = await studentsService.GetStudentById(_student.Id);
        updatedStudent.FullName.Should().Be(updatedStudentDto.FullName);
    }

    [Fact]
    public async Task DeleteStudent_ShouldDeleteStudent()
    {
        // Arrange
        var studentsService = GetStudentsService();
        
        var newStudentData = new NewStudentDto { FullName = "Bobert", Languages = [], GroupId = _group.GroupId };
        await studentsService.AddStudent(newStudentData, Admin);

        // Act
        await studentsService.DeleteStudent(_student.Id, Admin);

        // Assert
        var students = await studentsService.GetAllStudents(Admin);
        students.Should().NotContain(s => s.Id == _student.Id);
    }
}