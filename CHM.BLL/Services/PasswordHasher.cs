using System.Security.Cryptography;

namespace CHM.BLL.Services;

// Kullanıcı şifrelerini veritabanına kaydetmeden önce güvenli bir şekilde şifrelemek (Hash'lemek) 
// ve giriş yaparken doğrulamasını (Verify) yapmak için kullanılan yardımcı sınıf.
public static class PasswordHasher
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit
    private const int DefaultIterations = 100_000;

    // Şifreyi PBKDF2 algoritmasını kullanarak (binlerce kez karma işleminden geçirerek) şifreler.
    // Format: PBKDF2$<iterations>$<saltBase64>$<keyBase64>
    public static string Hash(string password, int iterations = DefaultIterations)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return $"PBKDF2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    // Kullanıcının girdiği düz şifre ile veritabanındaki şifrelenmiş Hash verisini karşılaştırır.
    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash)) return false;

        var parts = storedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 4) return false;
        if (!string.Equals(parts[0], "PBKDF2", StringComparison.Ordinal)) return false;
        if (!int.TryParse(parts[1], out var iterations)) return false;

        byte[] salt;
        byte[] key;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            key = Convert.FromBase64String(parts[3]);
        }
        catch
        {
            return false;
        }

        var computed = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            key.Length);

        return CryptographicOperations.FixedTimeEquals(computed, key);
    }
}

