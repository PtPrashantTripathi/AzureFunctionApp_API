using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PosMaster
{
    /// <summary>
    /// Helps in decrypting and encrypting data
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EncodingForBase64
    {
        /// <summary>
        /// Encrypt the input token
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static string EncodeBase64(string Token)
        {
            if (Token == null)
                return null;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Token));
        }

        /// <summary>
        /// Decrypt the input token
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static string DecodeBase64(string Token)
        {
            if (Token == null)
                return null;
            return Encoding.UTF8.GetString(Convert.FromBase64String(Token));
        }

    }
}
