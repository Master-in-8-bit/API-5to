using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public static class PasswordService
{
    public static string HashPassword(string password)
    {
        var salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        var passwordBytes = Encoding.UTF8.GetBytes(salt + password);
        var hash = Convert.ToHexString(SHA256.HashData(passwordBytes));

        return $"{salt}:{hash}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        var parts = storedHash.Split(':', 2);
        if (parts.Length != 2)
        {
            return false;
        }

        var salt = parts[0];
        var hash = parts[1];
        var computedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(salt + password)));

        return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
    }
}
