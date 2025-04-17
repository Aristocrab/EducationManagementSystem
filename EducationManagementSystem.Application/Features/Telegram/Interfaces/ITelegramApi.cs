using Refit;

namespace EducationManagementSystem.Application.Features.Telegram.Interfaces;

public interface ITelegramApi
{
    [Post("/sendMessage")]
    Task SendMessage([AliasAs("chat_id")] string chatId, 
        [Query] string text);
    
    [Post("/sendMessage")]
    Task SendMessage([AliasAs("chat_id")] string chatId, 
        [Query] string text, 
        [AliasAs("parse_mode")] string parseMode);
    
    [Multipart]
    [Post("/sendDocument")]
    Task SendDocument(
        [AliasAs("chat_id")] string chatId,
        [AliasAs("document")] StreamPart document,
        [AliasAs("caption")] string? caption = null);
}