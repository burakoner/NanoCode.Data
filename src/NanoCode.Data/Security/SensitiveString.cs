using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NanoCode.Data.Security
{
    public class SensitiveString
    {
        private readonly Aes aes;
        private readonly string key;
        private readonly byte[] payload;
        public SensitiveString(string secret)
        {
            this.key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            this.aes = Aes.Create("AesManaged");
            this.aes.Mode = CipherMode.CBC;
            this.aes.Padding = PaddingMode.PKCS7;
            this.aes.KeySize = 0x80;
            this.aes.BlockSize = 0x80;

            var keyBytes = Encoding.UTF8.GetBytes(this.key);
            this.aes.Key = keyBytes;
            this.aes.IV = keyBytes;

            var secretBytes = Encoding.UTF8.GetBytes(secret);
            this.payload = aes.CreateEncryptor().TransformFinalBlock(secretBytes, 0, secretBytes.Length);
        }

        public  string GetString()
        {
            byte[] TextByte = aes.CreateDecryptor().TransformFinalBlock(this.payload, 0, this.payload.Length);
            return Encoding.UTF8.GetString(TextByte); 
        }
    }

    public static class SensitiveStringExtensions
    {
        public static SensitiveString ToSensitiveString(string @this)
        {
            return new SensitiveString(@this);
        }
        public static string ToString(SensitiveString @this)
        {
            return @this.GetString();
        }
    }
}
