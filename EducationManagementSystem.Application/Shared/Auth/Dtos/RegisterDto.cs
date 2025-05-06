namespace EducationManagementSystem.Application.Shared.Auth.Dtos;

public class RegisterDto
{
    public required string FullName { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string WorkingHours { get; init; }
    public string? MessengerLink { get; init; }
}