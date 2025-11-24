using System.Threading.Tasks;
using Task.BLL.Interfaces;

namespace Task.BLL.Services
{
    public class EmailSender : IEmailSender
    {
        public async ValueTask SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Your email sending logic here
            await System.Threading.Tasks.Task.CompletedTask;
        }

        ValueTask IEmailSender.SendEmailAsync(string to, string subject, string html)
        {
            throw new NotImplementedException();
        }
    }
}