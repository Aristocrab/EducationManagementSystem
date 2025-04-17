using AutoFixture;
using FluentAssertions;
using FluentDate;
using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.Lessons.Predicates;
using EducationManagementSystem.Application.Features.Payments;
using EducationManagementSystem.Application.Features.Payments.Interfaces;
using EducationManagementSystem.Application.Features.Payments.Models;
using EducationManagementSystem.Application.Features.Telegram;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models;
using EducationManagementSystem.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace EducationManagementSystem.Tests;

public class PaymentsServiceTests : ServiceTests
{
    private AppDbContext _dbContext = null!;
    private const string MonobankAccountId = "Asad23u213981_zlkased123k";
    private readonly Guid _studentId = Guid.NewGuid();
    private readonly IClock _clock = new KyivTimeClock();

    private PaymentsService GetPaymentsService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().
            UseSqlite($"Data Source = {nameof(PaymentsServiceTests)}.db")
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        
        var student = new Student
        {
            Id = _studentId,
            FullName = "Bob",
            Languages = ["English"]
        };
        _dbContext.Students.Add(student);
        
        var teacher = new Teacher
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
        _dbContext.Teachers.Add(teacher);
        
        var lesson1 = new Lesson
        {
            Student = student,
            DateTime = new DateTime(2024, 07, 07, 10, 0, 0),
            Description = "Math Lesson",
            Price = 350,
            OneTime = false,
            Status = Status.Completed,
            TeacherEarnings = 200,
            Teacher = teacher,
            Duration = 1.Hours(),
            Paid = false
        };
        
        var lesson2 = new Lesson
        {
            Student = student,
            DateTime = new DateTime(2024, 07, 07, 15, 0, 0),
            Description = "Math Lesson2",
            Price = 350,
            OneTime = false,
            Status = Status.Completed,
            TeacherEarnings = 200,
            Teacher = teacher,
            Duration = 1.Hours(),
            Paid = false
        };
        _dbContext.Lessons.AddRange(lesson1, lesson2);
        
        var payment1 = new Payment
        {
            MonobankPaymentId = "",
            Amount = 700,
            IsConfirmed = false,
            Description = $"{student.FullName} ({student.Id})",
            Comment = null,
            Time = 2024.July(24),
            Currency = "₴",
            Account = $"{student.FullName} ({student.Id})",
            AccountAmount = 0,
        };
        
        var payment2 = new Payment
        {
            MonobankPaymentId = "",
            Amount = 5000,
            IsConfirmed = false,
            Description = $"Інна ({Guid.NewGuid()})",
            Comment = null,
            Time = 2024.July(24),
            Currency = "₴",
            Account = $"Інна ({Guid.NewGuid()})",
            AccountAmount = 0
        };
        _dbContext.Payments.AddRange(payment1, payment2);
        
        _dbContext.SaveChanges();

        var configuration = Substitute.For<IConfiguration>();
        configuration["MonobankAccountId"].Returns(MonobankAccountId);
        
        var paymentLinkApi = Substitute.For<IPaymentLinkApi>();
        var res = new GetPaymentLinkResponse
        {
            PageUrl = "https://payment.link",
            InvoiceId = ""
        };
        paymentLinkApi.GetPaymentLink(Arg.Any<GetPaymentLinkRequest>(), Arg.Any<string>())
            .Returns(res);

        return new PaymentsService(_dbContext, 
            configuration, 
            Substitute.For<ILogger<PaymentsService>>(), 
            Substitute.For<ITelegramService>(),
            paymentLinkApi,
            _clock);
    }

    [Fact]
    public async Task GetPayments_PaymentWithSameAmountAsUserUnpaidLessons_ShouldHaveSupposedStudentId()
    {
        // Arrange
        var service = GetPaymentsService();
        
        // Act
        var payments = await service.GetPayments(Admin);
        
        // Assert
        payments.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPayments_PaymentWithUnidentifiableAmount_ShouldNotHaveSupposedStudentId()
    {
        // Arrange
        var service = GetPaymentsService();
        
        // Act
        var payments = await service.GetPayments(Admin);
        
        // Assert
        payments.Should().NotBeEmpty();
        var payment = payments.Find(x => x.Amount == 5000);
        payment!.SupposedStudents.Should().BeEmpty();
    }

    [Fact]
    public async Task AddPayment_ShouldAddPayment()
    {
        // Arrange
        var service = GetPaymentsService();
        var fixture = new Fixture();
        var statementItem = fixture.Create<StatementItem>();
        statementItem.Comment = null;
        
        var webhookEvent = new WebhookEvent
        {
            Type = "StatementItem",
            Data = new WebhookData
            {
                Account = MonobankAccountId,
                StatementItem = statementItem
            }
        };
        
        // Act
        await service.AddPayment(webhookEvent);
        var payment = await _dbContext.Payments
            .FirstOrDefaultAsync(x => x.MonobankPaymentId == statementItem.Id);
        
        // Assert
        payment.Should().NotBeNull();
        payment!.IsConfirmed.Should().BeFalse();
    }
    
    [Fact]
    public async Task AddPayment_WithCommentForAllLessons_ShouldAutoCompletePayment()
    {
        // Arrange
        var service = GetPaymentsService();
        var fixture = new Fixture();
        var statementItem = fixture.Create<StatementItem>();
        
        var studentUnpaidLessonsBefore = (await _dbContext.Students
            .Include(x => x.Lessons)
            .FirstAsync(x => x.Id == _studentId))
            .Lessons
            .Count(x => !x.Paid);

        statementItem.Comment = $"Bob ({_studentId})";
        statementItem.Amount = 700 * 100;
        
        var webhookEvent = new WebhookEvent
        {
            Type = "StatementItem",
            Data = new WebhookData
            {
                Account = MonobankAccountId,
                StatementItem = statementItem
            }
        };
        
        // Act
        await service.AddPayment(webhookEvent);
        var payment = await _dbContext.Payments
            .Include(payment => payment.Student)
            .FirstOrDefaultAsync(x => x.MonobankPaymentId == statementItem.Id);
        
        // Assert
        payment.Should().NotBeNull();
        payment!.IsConfirmed.Should().BeTrue();
        payment.AutoConfirmed.Should().BeTrue();
        payment.Student.Should().NotBeNull();
        payment.Student!.Id.Should().Be(_studentId);

        studentUnpaidLessonsBefore.Should().Be(2);
        
        var student = await _dbContext.Students
            .Include(x => x.Lessons)
            .FirstAsync(x => x.Id == _studentId);
        student.Lessons
            .AsQueryable().Count(LessonPredicates.UnpaidLessonPredicate(_clock))
            .Should().Be(0);
    }
    
    [Fact]
    public async Task ConfirmPayment_ShouldConfirmPayment()
    {
        // Arrange
        var service = GetPaymentsService();
        var payment = await _dbContext.Payments.FirstAsync(x => x.Amount == 700);
        
        // Act
        await service.ConfirmPayment(payment.Id, Admin);
        var confirmedPayment = await _dbContext.Payments.FindAsync(payment.Id);
        
        // Assert
        confirmedPayment!.IsConfirmed.Should().BeTrue();
    }
    
    [Fact]
    public async Task ConfirmPaymentWithStudentId_ShouldConfirmPaymentAndMarkLessonsPaid()
    {
        // Arrange
        var service = GetPaymentsService();
        var student = await _dbContext.Students
            .Include(x => x.Lessons)
            .FirstAsync(x => x.Id == _studentId);
        var payment = await _dbContext.Payments.FirstAsync(x => x.Amount == 5000);
        
        // Act
        await service.ConfirmPayment(payment.Id, _studentId, Admin);
        var confirmedPayment = await _dbContext.Payments.FindAsync(payment.Id);
        
        // Assert
        confirmedPayment!.IsConfirmed.Should().BeTrue();
        student.Lessons.AsQueryable().Where(LessonPredicates.UnpaidLessonPredicate(_clock))
            .Should().BeEmpty();
    }
}