using System.Text.Json.Serialization;

namespace EducationManagementSystem.Application.Features.Payments.Models;

public class ClientInfo
{
    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("webHookUrl")]
    public string? WebHookUrl { get; set; }

    [JsonPropertyName("permissions")]
    public string? Permissions { get; set; }

    [JsonPropertyName("accounts")] public List<Account> Accounts { get; set; } = [];

    [JsonPropertyName("jars")]
    public List<Account> Jars { get; set; } = [];
}