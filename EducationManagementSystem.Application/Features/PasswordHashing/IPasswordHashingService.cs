namespace EducationManagementSystem.Application.Features.PasswordHashing;

public interface IPasswordHashingService
{
    (string hash, string salt) HashPassword(string password);
    string HashPassword(string password, string salt);
}