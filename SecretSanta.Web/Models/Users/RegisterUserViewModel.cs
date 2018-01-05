using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Web.Models.Users
{
    public class RegisterUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [StringLength(15, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at most {2} characters long.")]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

    }
}