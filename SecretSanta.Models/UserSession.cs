using System;
using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Models
{
    public class UserSession
    {
        public UserSession()
        {
        }

        public UserSession(string userId, string authToken, DateTime expiresOn)
        {
            this.UserId = userId;
            this.Authtoken = authToken;
            this.ExpiresOn = expiresOn;
        }

        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string Authtoken { get; set; }

        [Required]
        public DateTime ExpiresOn { get; set; }
    }
}
