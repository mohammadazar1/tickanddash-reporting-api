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
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: text,
                salt: new byte[128 / 8],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
