using System.Threading.Tasks;

namespace Task.BLL.Interfaces
{
    public interface IEmailSender
    {
        ValueTask SendEmailAsync(string to, string subject, string html);
    }
}