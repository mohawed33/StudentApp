using SQLite;
using System.Text.Json.Serialization;

namespace StudentApp.Models
{
    public enum UserRole
    {
        Teacher, // معلم
        Parent,  // ولي أمر
        Admin    // مدير
    }

    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("role")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }

        [JsonPropertyName("profileImageUrl")]
        public string ProfileImageUrl { get; set; }

        [JsonPropertyName("schoolId")]
        public string SchoolId { get; set; } // للمعلمين فقط

        [JsonPropertyName("studentIds")]
        public List<string> StudentIds { get; set; } = new List<string>(); // لأولياء الأمور فقط

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // حقول إضافية للمصادقة - لا يتم تخزينها في Firebase
        [Ignore]
        [JsonIgnore]
        public string Password { get; set; }

        [Ignore]
        [JsonIgnore]
        public string Token { get; set; }

        [Ignore]
        [JsonIgnore]
        public bool IsAuthenticated { get; set; }
    }
}
