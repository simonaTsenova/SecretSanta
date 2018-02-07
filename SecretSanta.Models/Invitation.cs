using System;
using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Models
{
    public class Invitation
    {
        public Guid Id { get; set; }

        public DateTime SentDate { get; set; }

        public Guid GroupId { get; set; }

        public virtual Group Group { get; set; }

        public string ReceiverId { get; set; }

        public virtual User Receiver { get; set; }
    }
}
