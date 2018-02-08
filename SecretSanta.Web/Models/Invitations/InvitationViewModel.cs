using System;

namespace SecretSanta.Web.Models.Invitations
{
    public class InvitationViewModel
    {
        public InvitationViewModel(Guid id, DateTime sentDate, string groupName, string receiver)
        {
            this.Id = id;
            this.SentDate = sentDate;
            this.GroupName = groupName;
            this.Receiver = receiver;
        }

        public Guid Id { get; set; }

        public DateTime SentDate { get; set; }

        public string GroupName { get; set; }

        public string Receiver { get; set; }
    }
}