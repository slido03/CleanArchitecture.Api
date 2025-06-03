using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}