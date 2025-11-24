using DAL.Entities;
using System;

namespace Task.DAL.Entities
{
    // سجل بسيط لتخزين توكن تأكيد الايميل
    public class EmailConfirmationToken
    {
        public int Id { get; set; }
        public Guid Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; }

        // relation to user
        public int UserId { get; set; }
        public User User { get; set; }
    }
}