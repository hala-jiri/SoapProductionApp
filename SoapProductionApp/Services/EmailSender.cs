using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Logování nebo jen simulace odeslání e-mailu
        Console.WriteLine($"Fake email sent to {email}: {subject}");
        return Task.CompletedTask;
    }
}