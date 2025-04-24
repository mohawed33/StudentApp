using SQLite;
using System.Text.Json.Serialization;

namespace StudentApp.Models
{
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonPropertyName("studentId")]
        public string StudentId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("schoolNumber")]
        public string SchoolNumber { get; set; } // الرقم المدرسي

        [JsonPropertyName("grade")]
        public string Grade { get; set; } // الصف

        [JsonPropertyName("section")]
        public string Section { get; set; } // الشعبة

        [JsonPropertyName("parentId")]
        public string ParentId { get; set; } // معرف ولي الأمر

        [JsonPropertyName("parentName")]
        public string ParentName { get; set; } // اسم ولي الأمر

        [JsonPropertyName("parentPhone")]
        public string ParentPhone { get; set; } // رقم هاتف ولي الأمر

        [JsonPropertyName("parentEmail")]
        public string ParentEmail { get; set; } // بريد ولي الأمر

        [JsonPropertyName("profileImageUrl")]
        public string ProfileImageUrl { get; set; } // صورة الطالب

        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; } // تاريخ الميلاد

        [JsonPropertyName("address")]
        public string Address { get; set; } // العنوان

        [JsonPropertyName("notes")]
        public string Notes { get; set; } // ملاحظات

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // خصائص محسوبة
        [Ignore]
        [JsonIgnore]
        public int Age => DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

        [Ignore]
        [JsonIgnore]
        public string FullGradeInfo => $"{Grade} - {Section}";
    }
}
