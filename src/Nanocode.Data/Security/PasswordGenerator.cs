using System;
using System.Security.Cryptography;
using System.Text;

namespace Nanocode.Data.Security
{
    public class PasswordGenerator
    {
        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_LCASE = "abcdefghijklmnopqrstuvwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "1234567890";
        private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>public string CreatePassword(int length)
        public static string Generate(int pwdLength, bool lowerCaseChars, bool upperCaseChars, bool numericChars, bool specialChars)
        {
            string ValidChars = "";
            if (lowerCaseChars) ValidChars += PASSWORD_CHARS_LCASE;
            if (upperCaseChars) ValidChars += PASSWORD_CHARS_UCASE;
            if (numericChars) ValidChars += PASSWORD_CHARS_NUMERIC;
            if (specialChars) ValidChars += PASSWORD_CHARS_SPECIAL;

            StringBuilder res = new StringBuilder();
            /**/
            Random rnd = new Random();
            while (0 < pwdLength--)
            {
                res.Append(ValidChars[rnd.Next(ValidChars.Length)]);
            }
            /**/
            /*
            while (0 < pwdLength--)
            {
                res.Append(ValidChars[RandomNumberGenerator.GetInt32(ValidChars.Length)]);
            }
            */
            return res.ToString();
        }

    }
}
