using System.ComponentModel.DataAnnotations;

namespace online_mr_certi.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Fadlan qor password-ka cusub")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password-ku waa inuu ka badnaadaa 6 xaraf")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Fadlan ku celi password-ka")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password-ada aad qortay isku mid ma aha!")]
        public string ConfirmPassword { get; set; }
    }
}