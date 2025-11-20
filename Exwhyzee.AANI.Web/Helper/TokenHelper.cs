using System.Security.Cryptography;
using System.Text;

namespace Exwhyzee.AANI.Web.Helper
{
    public static class TokenHelper
    {
        // Generate a numeric token with the given digits (7 by default).
        // Uses RandomNumberGenerator; leading zeros are allowed.
        public static string GenerateNumericToken(int digits = 7)
        {
            if (digits <= 0) throw new ArgumentOutOfRangeException(nameof(digits));
            var sb = new StringBuilder(digits);
            var buf = new byte[digits];
            RandomNumberGenerator.Fill(buf);
            for (int i = 0; i < digits; i++)
            {
                // map to 0-9
                sb.Append((buf[i] % 10).ToString());
            }
            return sb.ToString();
        }

        // Compute HMACSHA256 of token using configured secret; returns base64 string
        public static string ComputeHmacSha256(string token, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey ?? throw new ArgumentNullException(nameof(secretKey)));
            using var hmac = new HMACSHA256(keyBytes);
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hash = hmac.ComputeHash(tokenBytes);
            return Convert.ToBase64String(hash);
        }
    }
}
