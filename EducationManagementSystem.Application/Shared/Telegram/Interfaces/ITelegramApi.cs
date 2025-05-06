using Refit;

namespace EducationManagementSystem.Application.Shared.Telegram.Interfaces;

public interface ITelegramApi
{
    [Post("/sendMessage")]
    Task SendMessage([AliasAs("chat_id")] string chatId, 
        [Query] string text);
    
    [Post("/sendMessage")]
    Task SendMessage([AliasAs("chat_id")] string chatId, 
        [Query] string text, 
        [AliasAs("parse_mode")] string parseMode);
}