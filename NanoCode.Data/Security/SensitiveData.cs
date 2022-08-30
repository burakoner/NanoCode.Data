using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NanoCode.Data.Security
{
    public static class SensitiveData
    {
        private static (string Salt, List<string> Chunks) GenerateChunks(string text)
        {
            var rnd = new Random();
            var textLength = text.Length;
            var chunks = Math.Max(1, textLength / 24);
            var chunkLengthMin = (int)(textLength / (0.75 * chunks));
            var chunkLengthMax = (int)(textLength / (chunks / 2.0));
            var remainingLength = textLength;
            var cursorPosition = 0;

            var loopList = new List<string>();
            var loopSalt = string.Empty;
            for (var i = 1; i <= chunks; i++)
            {
                // Chunk Length
                var chunkLength = rnd.Next(chunkLengthMin, chunkLengthMax);
                chunkLength = Math.Min(chunkLength, remainingLength);
                if (i == chunks) chunkLength = remainingLength;

                // Lengths
                var headLength = rnd.Next(10, 100 - 10 - chunkLength);
                var bodyLength = chunkLength;
                var tailLength = 100 - headLength - bodyLength;

                // Data
                var head = PasswordGenerator.Generate(headLength, true, true, true, true);
                var body = bodyLength > 0 ? text.Substring(cursorPosition, bodyLength) : string.Empty;
                var tail = PasswordGenerator.Generate(tailLength, true, true, true, true);

                // Loop Variables
                loopList.Add(head + body + tail);
                loopSalt += headLength.ToString().PadLeft(2, '0') + bodyLength.ToString().PadLeft(2, '0') + tailLength.ToString().PadLeft(2, '0');

                // Prepare for the next turn
                remainingLength -= chunkLength;
                cursorPosition += chunkLength;
            }

            // Change Order
            var orderList = new List<int>();
            for (var i = 1; i <= chunks; i++) orderList.Add(i);
            orderList.Shuffle();

            // Build Return Variables
            var orderSalt = string.Empty;
            var returnSalt = loopSalt;
            var returnList = new List<string>();
            foreach (var o in orderList)
            {
                orderSalt += o.ToString().PadLeft(3, '0');
                returnList.Add(loopList[o - 1]);
            }
            returnSalt = orderSalt + returnSalt;

            // Return
            return (returnSalt, returnList);
        }

        private static string RecoverChunks(List<string> chunks)
        {
            // Check Point
            if (chunks == null) return string.Empty;
            if (chunks.Count < 3) return string.Empty;

            // Arrange
            var md5 = chunks[0];
            var salt = chunks[1];
            var count = chunks.Count - 2;
            if (salt.Length != count * 9) return string.Empty;

#if RELEASE
            try
            {
#endif
            // Order Salt -> List
            var orderSalt = salt.Substring(0, count * 3);
            var orderList = new List<int>();
            for (var i = 0; i < count; i++)
                orderList.Add(Convert.ToInt32(orderSalt.Substring(i * 3, 3)));

            // Data Salt -> List
            var dataSalt = new List<string>();
            for (var i = 0; i < count; i++)
                dataSalt.Add(salt.Substring((count * 3) + 6 * i, 6));

            // Extract Encrypted Text
            var encrypted = string.Empty;
            for (var i = 1; i <= count; i++)
            {
                var index = orderList.IndexOf(i);
                var data = chunks[index + 2];
                var key = dataSalt[i - 1];
                var headLength = Convert.ToInt32(key.Substring(0, 2));
                var bodyLength = Convert.ToInt32(key.Substring(2, 2));
                var tailLength = Convert.ToInt32(key.Substring(4, 2));
                var body = data.Substring(headLength, bodyLength);
                encrypted += body;
            }

            // Return
            return encrypted;
#if RELEASE
            }
            catch
            {
                return string.Empty;
            }
#endif
        }

        public static List<string> SetCredentials<TPoco, TCredentials>(TPoco poco, TCredentials credentials)
        {
            #region Check Point
            if (poco == null)
                throw new Exception("poco is null");

            if (credentials == null)
                throw new Exception("credentials is null");

            // Get Properties
            var properties = poco.GetType().GetProperties();

            // Find CAT
            PropertyInfo catProperty = null;
            var catValue = 0L;
            foreach (var property in properties)
            {
                if (property.Name == "CAT")
                {
                    if (property.PropertyType != typeof(long))
                        throw new Exception("Invalid type for CAT");

                    catProperty = property;
                    catValue = (long)property.GetValue(poco);
                    break;
                }
            }

            // Find CREDENTIALS
            PropertyInfo credentialsProperty = null;
            var credentialsValue = string.Empty;
            foreach (var property in properties)
            {
                if (property.Name == "CREDENTIALS")
                {
                    if (property.PropertyType != typeof(string))
                        throw new Exception("Invalid type for CREDENTIALS");

                    credentialsProperty = property;
                    credentialsValue = (string)property.GetValue(poco);
                }
            }

            // Check Point
            if (catProperty == null)
                throw new Exception("CAT is missing");

            // Check Point
            if (credentialsProperty == null)
                throw new Exception("CREDENTIALS is missing");

            // Check Point
            if (catValue == 0)
                throw new Exception("CAT value is wrong");
            #endregion

            // Prepare
            var creds = new List<string>();

            // Action
            var json = JsonConvert.SerializeObject(credentials);
            var encrypted = Cryptology.Encrypt(json, catValue.ToString());
            var chunks = SensitiveData.GenerateChunks(encrypted);
            var md5 = Cryptology.Hash(json, Cryptology.HashType.MD5);
            var salt = chunks.Salt;

            // List
            creds.Add(md5);
            creds.Add(salt);
            foreach (var item in chunks.Chunks) creds.Add(item);
            credentialsProperty.SetValue(poco, JsonConvert.SerializeObject(creds));

            // Return
            return creds;
        }

        public static TCredentials GetCredentials<TPoco, TCredentials>(TPoco poco)
        {
            #region Check Point
            if (poco == null)
                throw new Exception("poco is null");

            //if (credentials == null)
            //    throw new Exception("credentials is null");

            // Get Properties
            var properties = poco.GetType().GetProperties();

            // Find CAT
            PropertyInfo catProperty = null;
            var catValue = 0L;
            foreach (var property in properties)
            {
                if (property.Name == "CAT")
                {
                    if (property.PropertyType != typeof(long))
                        throw new Exception("Invalid type for CAT");

                    catProperty = property;
                    catValue = (long)property.GetValue(poco);
                    break;
                }
            }

            // Find CREDENTIALS
            PropertyInfo credentialsProperty = null;
            var credentialsValue = string.Empty;
            foreach (var property in properties)
            {
                if (property.Name == "CREDENTIALS")
                {
                    if (property.PropertyType != typeof(string))
                        throw new Exception("Invalid type for CREDENTIALS");

                    credentialsProperty = property;
                    credentialsValue = (string)property.GetValue(poco);
                }
            }

            // Check Point
            if (catProperty == null)
                throw new Exception("CAT is missing");

            // Check Point
            if (credentialsProperty == null)
                throw new Exception("CREDENTIALS is missing");

            // Check Point
            if (catValue == 0)
                throw new Exception("CAT value is wrong");
            #endregion

            // Action
            var encrypted = SensitiveData.RecoverChunks(JsonConvert.DeserializeObject<List<string>>(credentialsValue));
            var decrypted = Cryptology.Decrypt(encrypted, catValue.ToString());
            var credentials = JsonConvert.DeserializeObject<TCredentials>(decrypted);

            // Return
            return credentials;
        }

        /*
        public static List<string> SetCredentials(this SYS_BROKER @this, QueueCredentials credentials)
        {
            if (@this.CAT == 0)
                throw new Exception("CAT is missing");

            if (credentials == null)
                throw new Exception("credentials is null");

            var creds = new List<string>();

            // Action
            var json = JsonConvert.SerializeObject(credentials);
            var encrypted = Crypto.Encrypt(json, @this.CAT.ToString());
            var chunks = SensitiveData.GenerateChunks(encrypted);
            var md5 = Crypto.Hash(json, Crypto.HashType.MD5);
            var salt = chunks.Salt;

            // List
            creds.Add(md5);
            creds.Add(salt);
            foreach (var item in chunks.Chunks) creds.Add(item);
            @this.CREDENTIALS = JsonConvert.SerializeObject(creds);

            // Return
            return creds;
        }

        public static QueueCredentials GetCredentials(this SYS_BROKER @this)
        {
            if (@this.CAT == 0) throw new Exception("CAT is missing");
            if (string.IsNullOrEmpty(@this.CREDENTIALS)) throw new Exception("CREDENTIALS is missing");

            var encrypted = SensitiveData.RecoverChunks(JsonConvert.DeserializeObject<List<string>>(@this.CREDENTIALS));
            var decrypted = Crypto.Decrypt(encrypted, @this.CAT.ToString());
            var credentials = JsonConvert.DeserializeObject<QueueCredentials>(decrypted);

            return credentials;
        }

        public static List<string> SetCredentials(this SYS_DATABASE @this, DatabaseCredentials credentials)
        {
            if (@this.CAT == 0)
                throw new Exception("CAT is missing");

            if (credentials == null)
                throw new Exception("credentials is null");

            var creds = new List<string>();

#if METHOD01
            // Encrypt
            var enc_Engine = Crypto.Encrypt(credentials.Engine.GetLabel(), this.CAT.ToString());
            var enc_Host = Crypto.Encrypt(credentials.Host, this.CAT.ToString());
            var enc_Port = Crypto.Encrypt(credentials.Port.ToString(), this.CAT.ToString());
            var enc_Catalog = Crypto.Encrypt(credentials.Catalog, this.CAT.ToString());
            var enc_Username = Crypto.Encrypt(credentials.Username, this.CAT.ToString());
            var enc_Password = Crypto.Encrypt(credentials.Password, this.CAT.ToString());
            var enc_Mars = Crypto.Encrypt(credentials.MultipleActiveResultSets.ToString(), this.CAT.ToString());

            // Generate Chunks
            var chunk_Engine = this.GenerateChunks(enc_Engine);
            var chunk_Host = this.GenerateChunks(enc_Host);
            var chunk_Port = this.GenerateChunks(enc_Port);
            var chunk_Catalog = this.GenerateChunks(enc_Catalog);
            var chunk_Username = this.GenerateChunks(enc_Username);
            var chunk_Password = this.GenerateChunks(enc_Password);
            var chunk_Mars = this.GenerateChunks(enc_Mars);

            // Signature & Salt
            var body =
                credentials.Engine.GetLabel() +
                credentials.Host +
                credentials.Port.ToString() +
                credentials.Catalog +
                credentials.Username +
                credentials.Password +
                credentials.MultipleActiveResultSets.ToString();
            var sign = Crypto.Hash(body, Crypto.HashType.MD5);
            var salt = 
                chunk_Engine.Salt +
                chunk_Host.Salt +
                chunk_Port.Salt +
                chunk_Catalog.Salt +
                chunk_Username.Salt +
                chunk_Password.Salt +
                chunk_Mars.Salt;

            // Generate List
            creds.Add(sign);
            creds.Add(salt);
            foreach (var item in chunk_Engine.List) creds.Add(item);
            foreach (var item in chunk_Host.List) creds.Add(item);
            foreach (var item in chunk_Port.List) creds.Add(item);
            foreach (var item in chunk_Catalog.List) creds.Add(item);
            foreach (var item in chunk_Username.List) creds.Add(item);
            foreach (var item in chunk_Password.List) creds.Add(item);
            foreach (var item in chunk_Mars.List) creds.Add(item);

            // Set List
            @this.CREDENTIALS = JsonConvert.SerializeObject(creds);
#endif
            // Action
            var json = JsonConvert.SerializeObject(credentials);
            var encrypted = Crypto.Encrypt(json, @this.CAT.ToString());
            var chunks = SensitiveData.GenerateChunks(encrypted);
            var md5 = Crypto.Hash(json, Crypto.HashType.MD5);
            var salt = chunks.Salt;

            // List
            creds.Add(md5);
            creds.Add(salt);
            foreach (var item in chunks.Chunks) creds.Add(item);
            @this.CREDENTIALS = JsonConvert.SerializeObject(creds);

            // Return
            return creds;
        }

        public static DatabaseCredentials GetCredentials(this SYS_DATABASE @this)
        {
            if (@this.CAT == 0) throw new Exception("CAT is missing");
            if (string.IsNullOrEmpty(@this.CREDENTIALS)) throw new Exception("CREDENTIALS is missing");

            var encrypted = SensitiveData.RecoverChunks(JsonConvert.DeserializeObject<List<string>>(@this.CREDENTIALS));
            var decrypted = Crypto.Decrypt(encrypted, @this.CAT.ToString());
            var credentials = JsonConvert.DeserializeObject<DatabaseCredentials>(decrypted);

            return credentials;
        }
        */
    }

    public static partial class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rnd = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rnd.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
