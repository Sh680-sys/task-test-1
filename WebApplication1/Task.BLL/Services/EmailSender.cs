using Task.BLL.Interfaces;

namespace Task.BLL.Services
{
    public class EmailSender : IEmailSender
    {
        public async ValueTask SendEmailAsync(string to, string subject, string html)
        {
            // هون بتكون منطق إرسال الإيميل الفعلي (SMTP وما إلى ذلك)
            await System.Threading.Tasks.Task.CompletedTask;
        }
    }
}