using EducationManagementSystem.Core.Models;

namespace EducationManagementSystem.Application.Features.Telegram;

public interface ITelegramService
{
    Task SendWeeklyReport();
    Task SendMonthlyReport();
    Task SendYearlyReport();
    Task SendPaymentNotification(Payment payment, decimal totalAmount);
    Task SendAutoConfirmedPaymentNotification(Payment payment, decimal totalAmount);
    Task BackupDatabase();
}