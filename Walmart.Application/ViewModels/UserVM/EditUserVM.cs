using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.ViewModels.UserVM
{
    public class EditUserVM
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
