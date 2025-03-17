using MailKit.Net.Smtp; // Este es el using correcto
using MailKit.Security;
using MimeKit;
using WebApiPropiedades.Interface;

namespace WebApiPropiedades.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = "Restablecer Contraseña";

            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
            <h1>Restablecer tu contraseña</h1>
            <p>Para restablecer tu contraseña, haz clic en el siguiente enlace:</p>
            <p><a href='{resetLink}'>Restablecer Contraseña</a></p>
            <p>Si no solicitaste restablecer tu contraseña, puedes ignorar este correo.</p>
            <p>El enlace expirará en 24 horas.</p>";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _configuration["Email:SmtpServer"],
                int.Parse(_configuration["Email:Port"]),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _configuration["Email:Username"],
                _configuration["Email:Password"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
