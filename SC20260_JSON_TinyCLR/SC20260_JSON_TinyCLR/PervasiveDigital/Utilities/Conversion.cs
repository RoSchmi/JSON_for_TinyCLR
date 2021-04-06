// Conversion.cs

//https://github.com/PervasiveDigital/serialwifi/blob/master/src/Common/PervasiveDigital.Utility.Shared/Conversion.cs

using System;
//using Microsoft.SPOT;

namespace PervasiveDigital.Utilities
{
    public static class Conversion
    {
        private const string HexDigits = "0123456789abcdef";

        public static string ToHex(this byte b)
        {
            return HexDigits[b >> 4].ToString() + HexDigits[b & 0x0f];
        }
    }
}

