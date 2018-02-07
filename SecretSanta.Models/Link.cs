using System;
using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Models
{
    public class Link
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }

        public virtual Group Group { get; set; }

        public string SenderId { get; set; }

        public virtual User Sender { get; set; }

        public string ReceiverId { get; set; }

        public virtual User Receiver { get; set; }
    }
}
