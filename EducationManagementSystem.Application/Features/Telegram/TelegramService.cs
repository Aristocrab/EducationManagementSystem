using System.Globalization;
using EducationManagementSystem.Application.Database;
using EducationManagementSystem.Application.Features.Clock;
using EducationManagementSystem.Application.Features.Telegram.Interfaces;
using EducationManagementSystem.Application.Features.Telegram.Models;
using EducationManagementSystem.Application.Extensions;
using EducationManagementSystem.Core.Enums;
using EducationManagementSystem.Core.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Refit;

namespace EducationManagementSystem.Application.Features.Telegram;

public class TelegramService : ITelegramService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;
    private readonly ITelegramApi _telegramApi;
    private readonly IClock _clock;
    private readonly IList<string> _chatIds;

    public TelegramService(IConfiguration configuration, AppDbContext dbContext, ITelegramApi telegramApi, IClock clock)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _telegramApi = telegramApi;
        _clock = clock;
        _chatIds = configuration.GetRequiredSection("TelegramChatIds").Get<string[]>()!;
    }

    public async Task SendWeeklyReport()
    {
        var report = await GenerateWeeklyReport();
        var message = $"📊 *Weekly report for all students* 📊\n" +
                      $"📅 *Period:* {report.WeekStart:dd.MM.yyyy} – {report.WeekEnd:dd.MM.yyyy}\n" +
                      $"-----------------------------\n" +
                      $"📚 *Lessons:* {report.Lessons:N0}\n" +
                      $"✅ *Completed Lessons:* {report.CompletedLessons:N0}\n" +
                      $"💰 *Full Income:* {report.FullIncome:N0}₴\n" +
                      $"🏫 *School Income:* {report.SchoolIncome:N0}₴\n";

        await SendMessage(message, parseMode: "Markdown");
    }

    public async Task SendMonthlyReport()
    {
        var today = _clock.Now.Date;
        var tomorrow = today.AddDays(1);

        if (tomorrow.Month == today.Month)
        {
            return;
        }

        var report = await GenerateMonthlyReport();
        var message = $"📊 *Monthly report for all students* 📊\n" +
                      $"📅 *Month:* {report.MonthName}\n" +
                      $"-----------------------------\n" +
                      $"📚 *Lessons:* {report.Lessons:N0}\n" +
                      $"✅ *Completed Lessons:* {report.CompletedLessons:N0}\n" +
                      $"💰 *Full Income:* {report.FullIncome:N0}₴\n" +
                      $"🏫 *School Income:* {report.SchoolIncome:N0}₴\n" +
                      $"👩‍🎓 *Active Students:* {report.ActiveStudents:N0}\n" +
                      $"👨‍🏫 *Active Teachers:* {report.ActiveTeachers:N0}\n";

        await SendMessage(message, parseMode: "Markdown");
    }

    public async Task SendYearlyReport()
    {
        var report = await GenerateYearlyReport();
        var message = $"🥂 *Yearly report {report.YearStart.Year}* 🥂\n" +
                      $"-----------------------------\n" +
                      $"📚 *Lessons:* {report.Lessons:N0}\n" +
                      $"✅ *Completed Lessons:* {report.CompletedLessons:N0}\n" +
                      $"❌ *Canceled Lessons:* {report.CanceledLessons:N0}\n" +
                      $"💰 *Full Income:* {report.FullIncome:N0}₴\n" +
                      $"🏫 *School Income:* {report.SchoolIncome:N0}₴\n" +
                      $"👩‍🎓 *Active Students:* {report.ActiveStudents:N0}\n" +
                      $"👨‍🏫 *Active Teachers:* {report.ActiveTeachers:N0}\n";

        await SendMessage(message, parseMode: "Markdown");
    }

    private async Task<YearReport> GenerateYearlyReport()
    {
        var yearlyData = await _dbContext.Lessons
            .Where(x => x.DateTime.Year == _clock.Now.Year)
            .ToListAsync();

        var activeStudents = await _dbContext.Students
            .Where(x => x.Lessons.Any(y => y.Status == Status.Completed
                                           && y.DateTime.Month == _clock.Now.Month))
            .CountAsync();

        var activeTeachers = await _dbContext.Teachers
            .Where(x => x.Lessons.Any(y => y.Status == Status.Completed
                                           && y.DateTime.Month == _clock.Now.Month))
            .CountAsync();

        var report = new YearReport
        {
            YearStart = new DateOnly(_clock.Now.Year, 1, 1),
            YearEnd = new DateOnly(_clock.Now.Year, 12, 31),
            Lessons = yearlyData.Count,
            CompletedLessons = yearlyData.Count(x => x.Status == Status.Completed),
            CanceledLessons = yearlyData.Count(x => x.Status == Status.Cancelled),
            FullIncome = yearlyData.Where(x => x.Status == Status.Completed)
                .Sum(x => x.Price),
            SchoolIncome = yearlyData.Where(x => x.Status == Status.Completed)
                .Sum(x => x.Price - x.TeacherEarnings),
            ActiveStudents = activeStudents,
            ActiveTeachers = activeTeachers
        };

        return report;
    }

    private async Task<MonthReport> GenerateMonthlyReport()
    {
        var montlyData = await _dbContext.Lessons
            .Where(x => x.DateTime.Month == _clock.Now.Month && x.DateTime.Year == _clock.Now.Year)
            .ToListAsync();

        var activeStudents = await _dbContext.Students
            .Where(x => x.Lessons.Any(y => y.Status == Status.Completed
                                           && y.DateTime.Month == _clock.Now.Month))
            .CountAsync();

        var activeTeachers = await _dbContext.Teachers
            .Where(x => x.Lessons.Any(y => y.Status == Status.Completed
                                           && y.DateTime.Month == _clock.Now.Month))
            .CountAsync();

        var report = new MonthReport
        {
            MonthStart = new DateOnly(_clock.Now.Year, _clock.Now.Month, 1),
            MonthEnd = new DateOnly(_clock.Now.Year, _clock.Now.Month,
                DateTime.DaysInMonth(_clock.Now.Year, _clock.Now.Month)),
            MonthName = _clock.Now.ToString("MMMM", CultureInfo.InvariantCulture),
            Lessons = montlyData.Count,
            CompletedLessons = montlyData.Count(x => x.Status == Status.Completed),
            FullIncome = montlyData.Where(x => x.Status == Status.Completed).Sum(x => x.Price),
            SchoolIncome = montlyData.Where(x => x.Status == Status.Completed).Sum(x => x.Price - x.TeacherEarnings),
            ActiveStudents = activeStudents,
            ActiveTeachers = activeTeachers
        };

        return report;
    }

    private async Task<WeekReport> GenerateWeeklyReport()
    {
        var weekStart = _clock.Now.StartOfWeek();
        var weekEnd = weekStart.AddDays(6);

        var weeklyData = await _dbContext.Lessons
            .Where(x => x.DateTime >= weekStart && x.DateTime <= weekEnd)
            .ToListAsync();

        var report = new WeekReport
        {
            WeekStart = new DateOnly(weekStart.Year, weekStart.Month, weekStart.Day),
            WeekEnd = new DateOnly(weekEnd.Year, weekEnd.Month, weekEnd.Day),
            Lessons = weeklyData.Count,
            CompletedLessons = weeklyData.Count(x => x.Status == Status.Completed),
            FullIncome = weeklyData.Where(x => x.Status == Status.Completed).Sum(x => x.Price),
            SchoolIncome = weeklyData.Where(x => x.Status == Status.Completed).Sum(x => x.Price - x.TeacherEarnings)
        };

        return report;
    }

    private async Task SendMessage(string message, string? parseMode = null)
    {
        foreach (var chatId in _chatIds)
        {
            try
            {
                if (parseMode is not null)
                {
                    await _telegramApi.SendMessage(chatId, message, parseMode);
                }
                else
                {
                    await _telegramApi.SendMessage(chatId, message);
                }
            }
            catch
            {
                // ignored
            }
        }
    }

    public async Task SendPaymentNotification(Payment payment, decimal totalAmount)
    {
        var isJar = payment.Account == _configuration["MonobankJarId"];
        var isFop = payment.Account == _configuration["MonobankAccountId"];
        var action = payment.Amount > 0 ? "Поповнення" : "Зняття з";
        var accountType = isJar ? "банки" : "картки";
        if (isFop)
        {
            accountType = "ФОПа";
        }

        var accountType2 = isJar ? "банці" : "картці";
        if (isFop)
        {
            accountType2 = "ФОПі";
        }

        var telegramMessage = $"{action} {accountType}\n" +
                              $"{payment.Description}: {Math.Abs(payment.Amount)} {payment.Currency}\n" +
                              $"На {accountType2}: {payment.AccountAmount:0.00} {payment.Currency}\n" +
                              $"Сумарно: {totalAmount:0.00} {payment.Currency}";

        await SendMessage(telegramMessage);
    }

    public Task SendAutoConfirmedPaymentNotification(Payment payment, decimal totalAmount)
    {
        var isJar = payment.Account == _configuration["MonobankJarId"];
        var isFop = payment.Account == _configuration["MonobankAccountId"];
        var accountType = isJar ? "банці" : "картці";
        if (isFop)
        {
            accountType = "ФОПі";
        }

        var telegramMessage =
            $"Автоматично підтверджено платіж\n{payment.Student?.FullName}: {payment.Amount} {payment.Currency}";
        // telegramMessage += $"\nНа {accountType}: {payment.AccountAmount:0.00} {payment.Currency}";
        // telegramMessage += $"\nСумарно: {totalAmount:0.00} {payment.Currency}";

        return SendMessage(telegramMessage);
    }

    public async Task BackupDatabase()
    {
        const string backupPath = "../Database/backup.db";

        await using var source = new SqliteConnection(_configuration["DatabaseConnection"]);
        await using var destination = new SqliteConnection($"Data Source={backupPath}");

        source.Open();
        destination.Open();
        source.BackupDatabase(destination);

        await SendFileToTelegram(backupPath);
    }

    private async Task SendFileToTelegram(string filePath)
    {
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var streamPart = new StreamPart(fileStream, Path.GetFileName(filePath), "application/octet-stream");

        await _telegramApi.SendDocument(
            _configuration["TelegramBackupChatId"]!,
            streamPart,
            $"Backup {DateTime.UtcNow:dd.MM.yy}"
        );
    }
}