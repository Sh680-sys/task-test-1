using System;

namespace DAL.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        // توكن عشوائي طويل
        public string Token { get; set; }

        // متى ينتهي
        public DateTime Expires { get; set; }

        // لو تم إلغاءه يدوياً (logout أو revoke)
        public bool Revoked { get; set; }

        // علاقتنا باليوزر
        public int UserId { get; set; }
        public User User { get; set; }

        // تتبع المصدر لمزيد من الأمان
        public DateTime CreatedAt { get; set; }
        public string CreatedByIp { get; set; }

        // اختياري: سبب الإلغاء
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; } // لو نبدله بتوكن جديد (rotation)
    }
}