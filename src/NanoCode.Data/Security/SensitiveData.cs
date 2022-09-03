using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NanoCode.Data.Security
{
    public static class SensitiveData
    {
        public static (List<string> Chunks, string Salt) Encode(object data, string key)
        {
            // Action
            var json = JsonConvert.SerializeObject(data);
            var encrypted = CryptoMethods.Encrypt(json, key);
            var chunks = GenerateChunks(encrypted);
            // var md5 = Cryptology.Hash(json, Cryptology.HashType.MD5);

            // Return
            return (chunks.Chunks, chunks.Salt);
        }

        public static T Decode<T>(List<string> chunks, string salt, string key)
        {
            // Action
            var encrypted = RecoverChunks(chunks, salt);
            var decrypted = CryptoMethods.Decrypt(encrypted, key);
            var data = JsonConvert.DeserializeObject<T>(decrypted);

            // Return
            return data;
        }

        private static (List<string> Chunks, string Salt) GenerateChunks(string json)
        {
            var rnd = new Random();
            var textLength = json.Length;
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
                var body = bodyLength > 0 ? json.Substring(cursorPosition, bodyLength) : string.Empty;
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
            return (returnList, returnSalt);
        }

        private static string RecoverChunks(List<string> chunks, string salt)
        {
            // Check Point
            if (chunks == null) return string.Empty;
            if (chunks.Count < 1) return string.Empty;

            // Arrange
            var count = chunks.Count;
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
                var data = chunks[index];
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

    }

    public static class ListExtensions
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
