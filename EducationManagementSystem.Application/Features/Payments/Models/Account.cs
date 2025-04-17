using System.Text.Json.Serialization;

namespace EducationManagementSystem.Application.Features.Payments.Models;

public class Account
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("balance")]
    public int? Balance { get; set; }
}