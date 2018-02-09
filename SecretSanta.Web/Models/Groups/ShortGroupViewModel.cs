namespace SecretSanta.Web.Models.Groups
{
    public class ShortGroupViewModel
    {
        public ShortGroupViewModel(string name, string admin)
        {
            this.GroupName = name;
            this.Admin = admin;
        }

        public string GroupName { get; set; }

        public string Admin { get; set; }
    }
}