using Nanocode.Data.Security;
using Newtonsoft.Json;
using System;

namespace Nanocode.Data.Examples
{
    public class Program
    {
        static void Main(string[] args)
        {
            SqlServerNanoCredentials.DefaultCredentials = new SqlServerNanoCredentials();
            var creds = new SqlServerNanoCredentials
            {
                Engine = "Engine",
                Host = "Host",
                Port = 1433,
                Database = "Database",
                Username = "Username",
                Password = "Password",
                MultipleActiveResultSets = true,
            };

            var enc = SensitiveData.Encode(creds, "1234567890");
            var dec = SensitiveData.Decode<SqlServerNanoCredentials>(enc.Chunks, enc.Salt, "1234567890");










            Console.WriteLine("Hello, World!");
        }
    }


    public class SqlServerNanoCredentials
    {
        [JsonIgnore]
        public static SqlServerNanoCredentials DefaultCredentials { get; set; }

        [JsonProperty("engine")]
        public string Engine { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("database")]
        public string Database { get; set; }

        [JsonProperty("user")]
        public string Username { get; set; }

        [JsonProperty("pass")]
        public string Password { get; set; }

        [JsonProperty("mars")]
        public bool MultipleActiveResultSets { get; set; }

        [JsonIgnore]
        public string ConnectionString
        {
            get
            {
                // Arrange
                var _conn = "";

                // Action
                var _port = this.Port > 0 ? "," + this.Port.ToString() : "";
                _conn =
                    $"Server={this.Host}{_port}; " +
                    $"Database={this.Database}; " +
                    $"User Id={this.Username}; " +
                    $"Password={this.Password}; " +
                    $"MultipleActiveResultSets={this.MultipleActiveResultSets.ToString().ToLowerInvariant()}";

                // Return
                return _conn;
            }
        }
    }
}