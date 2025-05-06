using System.Security.Cryptography;

namespace EducationManagementSystem.Application.Shared.PasswordHashing;

public class PasswordHashingService : IPasswordHashingService
{
    private const int SaltSize = 128 / 8;
    private const int HashSize = 256 / 8;
    private const int Iterations = 100_000;
    
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    
    public (string hash, string salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, Algorithm, HashSize);
        
        return Convert.ToBase64String(hash);
    }
}