namespace dot_net_core_identity_udy.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string fromAddress,string toAddress, string subject, string message);
    }
}
