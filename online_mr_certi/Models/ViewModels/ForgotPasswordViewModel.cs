using System.ComponentModel.DataAnnotations;

namespace online_mr_certi.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Fadlan qor email-kaaga")]
        [EmailAddress(ErrorMessage = "Email-ku ma aha mid sax ah")]
        public string Email { get; set; }
    }
}