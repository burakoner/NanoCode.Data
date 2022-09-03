using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace NanoCode.Data.Security
{
    public static class CryptoMethods
    {
        /// <summary>
        /// Encrypt a byte array into a byte array using a key and an IV 
        /// </summary>
        /// <param name="clearData"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            var ms = new MemoryStream();
            var alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();

            var encryptedData = ms.ToArray();
            return encryptedData;
        }

        /// <summary>
        /// Encrypt String
        /// </summary>
        /// <param name="clearText"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string Encrypt(string clearText, string Password)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            var pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            var encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Encrypt byte with string password
        /// </summary>
        /// <param name="clearData"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] clearData, string Password)
        {
            var pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        /// <summary>
        /// Encrypt a file into another file using a password 
        /// </summary>
        /// <param name="fileIn"></param>
        /// <param name="fileOut"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool Encrypt(string fileIn, string fileOut, string Password)
        {
            try
            {
                var fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
                var fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);
                var pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                var alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);
                var cs = new CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write);
                var bufferLen = 4096;
                var buffer = new byte[bufferLen];
                int bytesRead;
                do
                {
                    // read a chunk of data from the input file 
                    bytesRead = fsIn.Read(buffer, 0, bufferLen);
                    // encrypt it 
                    cs.Write(buffer, 0, bytesRead);
                }
                while (bytesRead != 0);

                cs.Close();
                fsIn.Close();

                // Return
                return true;
            }
            catch
            {
                // Return
                return false;
            }
        }

        /// <summary>
        /// Decrypt a byte array into a byte array using a key and an IV 
        /// </summary>
        /// <param name="cipherData"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            var ms = new MemoryStream();
            try
            {
                var alg = Rijndael.Create();
                alg.Key = Key;
                alg.IV = IV;
                var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(cipherData, 0, cipherData.Length);
                cs.Close();
            }
            catch
            {

            }
            var decryptedData = ms.ToArray();

            return decryptedData;
        }

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string Password)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            var decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return Encoding.Unicode.GetString(decryptedData);
        }

        /// <summary>
        /// Decrypt byte
        /// </summary>
        /// <param name="cipherData"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] cipherData, string Password)
        {
            var pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        /// <summary>
        /// Decrypt a file into another file using a password 
        /// </summary>
        /// <param name="fileIn"></param>
        /// <param name="fileOut"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool Decrypt(string fileIn, string fileOut, string Password)
        {
            try
            {
                // First we are going to open the file streams 
                var fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
                var fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);
                var pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                var alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);
                var cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write);
                var bufferLen = 4096;
                var buffer = new byte[bufferLen];
                int bytesRead;

                do
                {
                    // read a chunk of data from the input file 
                    bytesRead = fsIn.Read(buffer, 0, bufferLen);
                    // Decrypt it 
                    cs.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);

                cs.Close();
                fsIn.Close();

                // Return
                return true;
            }
            catch
            {
                // Return
                return false;
            }

        }

        /// <summary>
        /// Supported hash algorithms
        /// </summary>
        public enum HashType
        {
            HMAC, HMACMD5, HMACSHA1, HMACSHA256, HMACSHA384, HMACSHA512, /*MACTripleDES,*/ MD5, /*RIPEMD160,*/ SHA1, SHA256, SHA384, SHA512
        }

        private static byte[] GetHash(string Source, HashType hash)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(Source);

            switch (hash)
            {
                case HashType.HMAC:
                    return HMAC.Create().ComputeHash(inputBytes);

                case HashType.HMACMD5:
                    return HMACMD5.Create().ComputeHash(inputBytes);

                case HashType.HMACSHA1:
                    return HMACSHA1.Create().ComputeHash(inputBytes);

                case HashType.HMACSHA256:
                    return HMACSHA256.Create().ComputeHash(inputBytes);

                case HashType.HMACSHA384:
                    return HMACSHA384.Create().ComputeHash(inputBytes);

                case HashType.HMACSHA512:
                    return HMACSHA512.Create().ComputeHash(inputBytes);
                /*
            case HashType.MACTripleDES:
                return MACTripleDES.Create().ComputeHash(inputBytes);
                */
                case HashType.MD5:
                    return MD5.Create().ComputeHash(inputBytes);
                /*
            case HashType.RIPEMD160:
                return RIPEMD160.Create().ComputeHash(inputBytes);
                */
                case HashType.SHA1:
                    return SHA1.Create().ComputeHash(inputBytes);

                case HashType.SHA256:
                    return SHA256.Create().ComputeHash(inputBytes);

                case HashType.SHA384:
                    return SHA384.Create().ComputeHash(inputBytes);

                case HashType.SHA512:
                    return SHA512.Create().ComputeHash(inputBytes);

                default:
                    return inputBytes;
            }
        }

        /// <summary>
        /// Computes the hash of the string using a specified hash algorithm
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <param name="hashType">The hash algorithm to use</param>
        /// <returns>The resulting hash or an empty string on error</returns>
        public static string Hash(string Source, HashType hashType)
        {
            try
            {
                var hash = GetHash(Source, hashType);
                var ret = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    ret.Append(hash[i].ToString("x2"));

                return ret.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open);
            var md5 = new MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(file);
            file.Close();

            var sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Hmac256(string message, string secret)
        {
            var encoding = Encoding.UTF8;
            using (var hmac = new HMACSHA256(encoding.GetBytes(secret)))
            {
                var msg = encoding.GetBytes(message);
                var hash = hmac.ComputeHash(msg);
                return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }

        public static string Hmac384(string message, string secret)
        {
            var encoding = Encoding.UTF8;
            using (var hmac = new HMACSHA384(encoding.GetBytes(secret)))
            {
                var msg = encoding.GetBytes(message);
                var hash = hmac.ComputeHash(msg);
                return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }

        public static string Hmac512(string message, string secret)
        {
            var encoding = Encoding.UTF8;
            using (var hmac = new HMACSHA512(encoding.GetBytes(secret)))
            {
                var msg = encoding.GetBytes(message);
                var hash = hmac.ComputeHash(msg);
                return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }

        public static string Encode128(string Data, string Key)
        {
            try
            {
                var clearBytes = Encoding.Unicode.GetBytes(Data);
                var pdb = new PasswordDeriveBytes(Key, new byte[] { 0x00, 0x01, 0x02, 0x1C, 0x1D, 0x1E, 0x03, 0x04, 0x05, 0x0F, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
                var ms = new MemoryStream();
                var alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(16);
                alg.IV = pdb.GetBytes(16);
                var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
                var encryptedData = ms.ToArray();
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception Ex)
            {
                string cat = Ex.Message.ToString();
                return "Failed!";
            }
        }

        public static string Decode128(string Data, string Key)
        {
            try
            {
                var clearBytes = Convert.FromBase64String(Data);
                var pdb = new PasswordDeriveBytes(Key, new byte[] { 0x00, 0x01, 0x02, 0x1C, 0x1D, 0x1E, 0x03, 0x04, 0x05, 0x0F, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
                var ms = new MemoryStream();
                var alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(16);
                alg.IV = pdb.GetBytes(16);
                var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
                var decryptedData = ms.ToArray();
                return Encoding.Unicode.GetString(decryptedData);
            }
            catch (Exception Ex)
            {
                string cat = Ex.Message.ToString();
                return "Failed!";
            }
        }

        public static string Encode256(string Data, string Key)
        {
            try
            {
                var clearBytes = Encoding.Unicode.GetBytes(Data);
                var pdb = new PasswordDeriveBytes(Key, new byte[] { 0x00, 0x01, 0x02, 0x1C, 0x1D, 0x1E, 0x03, 0x04, 0x05, 0x0F, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
                var ms = new MemoryStream();
                var alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);
                var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
                var encryptedData = ms.ToArray();
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception Ex)
            {
                string cat = Ex.Message.ToString();
                return "Failed!";
            }
        }

        public static string Decode256(string Data, string Key)
        {
            try
            {
                var clearBytes = Convert.FromBase64String(Data);
                var pdb = new PasswordDeriveBytes(Key, new byte[] { 0x00, 0x01, 0x02, 0x1C, 0x1D, 0x1E, 0x03, 0x04, 0x05, 0x0F, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
                var ms = new MemoryStream();
                var alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);
                var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
                var decryptedData = ms.ToArray();
                return Encoding.Unicode.GetString(decryptedData);
            }
            catch (Exception Ex)
            {
                string cat = Ex.Message.ToString();
                return "Failed!";
            }
        }

        public static string ConvertStringToHex(string input, Encoding encoding)
        {
            var stringBytes = encoding.GetBytes(input);
            var sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(string hexInput, Encoding encoding)
        {
            var numberChars = hexInput.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
    }
}
