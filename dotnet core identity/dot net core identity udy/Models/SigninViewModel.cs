using System.ComponentModel.DataAnnotations;

namespace dot_net_core_identity_udy.Models
{
    public class SigninViewModel
    {
        [Required (ErrorMessage ="Email Required!")]
        [DataType (DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password Required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
