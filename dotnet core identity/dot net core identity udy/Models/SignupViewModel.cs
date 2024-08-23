using System.ComponentModel.DataAnnotations;

namespace dot_net_core_identity_udy.Models

{
    public class SignupViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage = "EmailAddress is invalid or missing!")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage ="Incorrect or Missing Password")]
        public string Password { get; set; }
    }
}
