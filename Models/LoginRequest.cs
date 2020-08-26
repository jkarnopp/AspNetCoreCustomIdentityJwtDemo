using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AspNetCoreCustomIdentyJwtDemo.Models
{
    public class LoginRequest
    {
        public LoginRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public LoginRequest()
        {
        }

        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}