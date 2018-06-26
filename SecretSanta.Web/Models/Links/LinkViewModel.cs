using System;

namespace SecretSanta.Web.Models.Links
{
    public class LinkViewModel
    {
        public LinkViewModel(string receiver)
        {
            this.Receiver = receiver;
        }

        public string Receiver { get; set; }
    }
}