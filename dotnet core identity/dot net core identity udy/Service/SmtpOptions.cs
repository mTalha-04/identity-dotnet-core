using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace dot_net_core_identity_udy.Service
{
    public class SmtpOptions
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public  int Port { get; set; }
    }
}
