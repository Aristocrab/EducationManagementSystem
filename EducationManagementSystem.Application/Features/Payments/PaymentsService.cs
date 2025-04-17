using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Auth.Models;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.Lessons.Predicates;
using EducationManagementSystem.Application.Features.Payments.Interfaces;
using EducationManagementSystem.Application.Features.Payments.Models;
using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Telegram;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Core.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Throw;

namespace EducationManagementSystem.Application.Features.Payments;

public class PaymentsService : IPaymentsService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentsService> _logger;
    private readonly ITelegramService _telegramService;
    private readonly IPaymentLinkApi _paymentLinkApi;
    private readonly IClock _clock;

    public PaymentsService(AppDbContext dbContext, 
        IConfiguration configuration, 
        ILogger<PaymentsService> logger, 
        ITelegramService telegramService,
        IPaymentLinkApi paymentLinkApi,
        IClock clock)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
        _telegramService = telegramService;
        _paymentLinkApi = paymentLinkApi;
        _clock = clock;
    }

    public async Task<decimal> GetCardBalance(User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var fopBalance = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankAccountId"])
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        var oldCardBalance = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankOldAccountId"])
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        var jarBalance = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankJarId"])
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        return fopBalance + oldCardBalance + jarBalance;
    }

    public async Task<Dictionary<string, decimal>> GetCardBalanceDetails(User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var fopBalance = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankAccountId"])
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        var oldCardBalance = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankOldAccountId"])
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        var jarBalance = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankJarId"])
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        return new Dictionary<string, decimal> 
        {
            { "ФОП", fopBalance },
            { "Картка Моно", oldCardBalance },
            { "Банка", jarBalance }
        };
    }

    public async Task<List<PaymentDto>> GetPayments(User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var payments = 
            await _dbContext.Payments
                .Include(x => x.Student)
                .OrderByDescending(x => x.Time)
                .Take(75)
                .ProjectToType<PaymentDto>()
                .ToListAsync();

        foreach (var payment in payments)
        {
            payment.SupposedStudents = await GetSupposedStudents(payment);
        }

        return payments;
    }

    private async Task<List<StudentDto>> GetSupposedStudents(PaymentDto payment)
    {
        if(payment.Amount <= 0 || payment.IsConfirmed) return [];
        
        var supposedStudents = await _dbContext.Students
            .Include(x => x.Lessons)
            .Where(x => (decimal)x.Lessons
                .AsQueryable()
                .Where(LessonPredicates.UnpaidLessonPredicate(_clock))
                .Sum(z => (double)z.Price) == payment.Amount)
            .ProjectToType<StudentDto>()
            .ToListAsync();
    
        // if(supposedStudents.Count == 0)
        // {
        //     var student = await _dbContext.Payments
        //         .Include(x => x.Student)
        //         .Where(x => x.Id != payment.Id && x.Account == payment.Account && x.Amount > 0)
        //         .Where(x => x.Student != null)
        //         .Select(x => x.Student)
        //         .ProjectToType<StudentDto>()
        //         .FirstOrDefaultAsync();
        //
        //     if (student is not null)
        //     {
        //         payment.Student = student;
        //         payment.AmountWarning = true;
        //     }
        // }

        return supposedStudents;
    }

    public async Task AddPayment(WebhookEvent newPayment)
    {
        if (newPayment.Data.Account == _configuration["MonobankAccountId"] 
            || newPayment.Data.Account == _configuration["MonobankOldAccountId"]
            || newPayment.Data.Account == _configuration["MonobankJarId"])
        {
            var webhookData = newPayment.Data.StatementItem;
            
            if(_dbContext.Payments.Any(x => x.MonobankPaymentId == webhookData.Id))
            {
                return;
            }

            if (webhookData.Comment is not null)
            {
                try
                {
                    await TryAutoConfirmPayment(newPayment.Data);
                    return;
                } 
                catch (Exception e)
                {
                    _logger.LogError("Failed to auto-confirm payment: {Message}", e.Message);
                }
            }

            var payment = new Payment
            {
                MonobankPaymentId = webhookData.Id,
                Description = webhookData.Description,
                Comment = webhookData.Comment,
                Amount = webhookData.Amount / 100m,
                Time = DateTimeOffset.FromUnixTimeSeconds(webhookData.Time).DateTime,
                Currency = ToCurrencySymbol(webhookData.CurrencyCode),
                Account = newPayment.Data.Account,
                AccountAmount = webhookData.Balance / 100m
            };
            
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            await _telegramService.SendPaymentNotification(payment, 
                await GetCardBalance(User.Admin));

            _logger.LogInformation("New transaction: {PaymentDescription} {PaymentAmount}{PaymentCurrency}", 
                payment.Description,
                payment.Amount,
                payment.Currency);
        }
    }
    
    public async Task AddFopPayment(WebhookFopEvent newPayment)
    {
        if(newPayment.Status != "success") return;
        
        if(_dbContext.Payments.Any(x => x.MonobankPaymentId == newPayment.InvoiceId))
        {
            return;
        }

        try
        {
            await TryAutoConfirmPayment(newPayment);
            return;
        } 
        catch (Exception e)
        {
            _logger.LogError("Failed to auto-confirm payment: {Message}", e.Message);
        }

        var lastPaymentAccAmount = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankAccountId"]!)
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        var payment = new Payment
        {
            MonobankPaymentId = newPayment.InvoiceId,
            Description = newPayment.Destination,
            Comment = newPayment.Destination,
            Amount = newPayment.Amount / 100m,
            Time = newPayment.CreatedDate,
            Currency = "₴",
            Account = _configuration["MonobankAccountId"]!,
            AccountAmount = lastPaymentAccAmount + newPayment.Amount / 100m
        };
            
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        await _telegramService.SendPaymentNotification(payment, 
            await GetCardBalance(User.Admin));

        _logger.LogInformation("New transaction: {PaymentDescription} {PaymentAmount}{PaymentCurrency}", 
            payment.Description,
            payment.Amount,
            payment.Currency);
        
    }

    private static string ToCurrencySymbol(int code)
    {
        return code switch
        {
            980 => "₴",
            840 => "$",
            978 => "€",
            _ => code.ToString()
        };
    }

    private async Task TryAutoConfirmPayment(WebhookData webhookData)
    {
        var id = Guid.Parse(webhookData.StatementItem.Comment!.AsSpan(webhookData.StatementItem.Comment!.Length - 37, 36));
        
        var student = await _dbContext.Students
            .Include(x => 
                x.Lessons.AsQueryable().Where(LessonPredicates.UnpaidLessonPredicate(_clock)))
            .FirstOrDefaultAsync(x => x.Id == id);
        student.ThrowIfNull();

        var amount = webhookData.StatementItem.Amount / 100m;
        var unpaidLessons = student.Lessons
            .Sum(x => x.Price);
        
        unpaidLessons.Throw()
            .IfNotEquals(amount);
        
        var payment = new Payment
        {
            MonobankPaymentId = webhookData.StatementItem.Id,
            Description = webhookData.StatementItem.Description,
            Comment = webhookData.StatementItem.Comment,
            Amount = amount,
            Time = DateTimeOffset.FromUnixTimeSeconds(webhookData.StatementItem.Time).DateTime,
            Currency = ToCurrencySymbol(webhookData.StatementItem.CurrencyCode),
            IsConfirmed = true,
            AutoConfirmed = true,
            Student = student,
            Account = webhookData.Account,
            AccountAmount = webhookData.StatementItem.Balance / 100m
        };
        _dbContext.Payments.Add(payment);

        foreach (var lesson in student.Lessons)
        {
            lesson.Paid = true;
        }
        await _dbContext.SaveChangesAsync(); 
        
        await _telegramService.SendAutoConfirmedPaymentNotification(payment, 
            await GetCardBalance(User.Admin));
        
        _logger.LogInformation("New payment auto-confirmed: {StudentName} {PaymentAmount}{PaymentCurrency}", 
            student.FullName,
            payment.Amount,
            payment.Currency);
    }
    
    private async Task TryAutoConfirmPayment(WebhookFopEvent webhookData)
    {
        var id = Guid.Parse(webhookData.Destination.AsSpan(webhookData.Destination.Length - 37, 36));
        
        var student = await _dbContext.Students
            .Include(x => x.Lessons
                .AsQueryable().Where(LessonPredicates.UnpaidLessonPredicate(_clock)))
            .FirstOrDefaultAsync(x => x.Id == id);
        student.ThrowIfNull();

        var amount = webhookData.Amount / 100m;
        var unpaidLessons = student.Lessons
            .Sum(x => x.Price);
        
        unpaidLessons.Throw()
            .IfNotEquals(amount);
        
        var lastPaymentAccAmount = (await _dbContext.Payments
            .Where(x => x.Account == _configuration["MonobankAccountId"])
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync())?.AccountAmount ?? 0;
        
        var payment = new Payment
        {
            MonobankPaymentId = webhookData.InvoiceId,
            Description = webhookData.Destination,
            Comment = webhookData.Destination,
            Amount = amount,
            Time = webhookData.CreatedDate,
            Currency = "₴",
            IsConfirmed = true,
            AutoConfirmed = true,
            Student = student,
            Account = _configuration["MonobankAccountId"]!,
            AccountAmount = lastPaymentAccAmount
            // AccountAmount = lastPaymentAccAmount + amount
        };
        _dbContext.Payments.Add(payment);

        foreach (var lesson in student.Lessons)
        {
            lesson.Paid = true;
        }
        await _dbContext.SaveChangesAsync(); 
        
        await _telegramService.SendAutoConfirmedPaymentNotification(payment, 
            await GetCardBalance(User.Admin));
        
        _logger.LogInformation("New fop payment auto-confirmed: {StudentName} {PaymentAmount}{PaymentCurrency}", 
            student.FullName,
            payment.Amount,
            payment.Currency);
    }

    public async Task ConfirmPayment(Guid paymentId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var payment = await _dbContext.Payments
            .Include(x => x.Student)
            .FirstOrDefaultAsync(x => x.Id == paymentId);
        payment.ThrowIfNull();
        
        payment.IsConfirmed = !payment.IsConfirmed;

        if (!payment.IsConfirmed)
        {
            payment.Student = null;
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task ConfirmPayment(Guid paymentId, Guid studentId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var payment = await _dbContext.Payments.FindAsync(paymentId);
        payment.ThrowIfNull();
        
        var student = await _dbContext.Students
            .Include(x => 
                x.Lessons.AsQueryable().Where(LessonPredicates.UnpaidLessonPredicate(_clock)))
            .FirstOrDefaultAsync(x => x.Id == studentId);
        student.ThrowIfNull();
        
        payment.IsConfirmed = true;
        payment.Student = student;
        foreach (var lesson in student.Lessons)
        {
            lesson.Paid = true;
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public Task DeletePayment(Guid paymentId, User currentUser)
    {
        currentUser.Throw().IfNotAdminOrModerator();
        
        var payment = _dbContext.Payments.Find(paymentId);
        payment.ThrowIfNull();
        
        _dbContext.Payments.Remove(payment);
        
        return _dbContext.SaveChangesAsync();
    }

    public async Task<string> GetPaymentLink(Guid studentId, decimal amount)
    {
        var student = await _dbContext.Students
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == studentId);
        student.ThrowIfNull();
        
        var req = new GetPaymentLinkRequest
        {
            Amount = (int)amount * 100,
            MerchantPaymInfo = new MerchantPaymInfo
            {
                Destination = $"{student.FullName} ({student.Id})",
                Comment = $"{student.FullName} ({student.Id})"
            },
            WebHookUrl = _configuration["WebhookFopUrl"]!
        };
        
        var url = await _paymentLinkApi.GetPaymentLink(
            req, _configuration["PaymentLinkToken"]!);
        
        _logger.LogInformation("Payment link generated for {StudentName} {Amount}{Currency} - {Url}", 
            student.FullName, amount, "UAH", url.PageUrl);
        
        return url.PageUrl;
    }
}