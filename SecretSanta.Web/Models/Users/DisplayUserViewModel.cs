namespace SecretSanta.Web.Models.Users
{
    public class DisplayUserViewModel
    {
        public DisplayUserViewModel(string email, string firstName, string lastName, string displayName, string userName)
        {
            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DisplayName = displayName;
            this.UserName = userName;
        }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string UserName { get; set; }
    }
}