using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Helpers
{
    public static class StringExt
    {
        public static string Hash(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text), "String cannot be null for hashing");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("String cannot be empty or whitespace for hashing", nameof(text));
            }

            try
            {
                var salt = new byte[128 / 8];
                var hashedBytes = KeyDerivation.Pbkdf2(
                    password: text,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8);

                if (hashedBytes == null || hashedBytes.Length == 0)
                {
                    throw new InvalidOperationException("Hashing produced null or empty result");
                }

                return Convert.ToBase64String(hashedBytes);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to hash string: {ex.Message}", ex);
            }
        }
    }
}
