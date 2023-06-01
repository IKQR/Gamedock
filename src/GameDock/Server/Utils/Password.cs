using System;
using System.Linq;
using System.Security.Cryptography;

public static class PasswordGenerator
{
    private static readonly char[] PasswordCharsMixedCase = 
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*".ToCharArray();

    public static string GenerateStrongPassword(int length = 12)
    {
        if (length < 8 || length > 128)
            throw new ArgumentException("Password length must be between 8 and 128.");

        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[length];
        rng.GetBytes(buffer);
        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = PasswordCharsMixedCase[buffer[i] % PasswordCharsMixedCase.Length];
        }

        var password = new string(chars);

        if (password.Any(char.IsDigit) && password.Any(char.IsUpper) && 
            password.Any(char.IsLower) && password.Any(char.IsSymbol))
            return password;
        
        return GenerateStrongPassword(length);
    }
}