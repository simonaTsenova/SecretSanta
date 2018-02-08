using System;

namespace SecretSanta.Models
{
    public class Invitation
    {
        public Invitation()
        {
        }

        public Invitation(Guid groupId, DateTime sentDate, string receiverId)
        {
            this.GroupId = groupId;
            this.SentDate = sentDate;
            this.ReceiverId = receiverId;
        }

        public Guid Id { get; set; }

        public DateTime SentDate { get; set; }

        public Guid GroupId { get; set; }

        public virtual Group Group { get; set; }

        public string ReceiverId { get; set; }

        public virtual User Receiver { get; set; }
    }
}
