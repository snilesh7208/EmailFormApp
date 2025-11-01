using EmailFormApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace EmailFormApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve email settings from appsettings.json
                    var senderEmail = _configuration["EmailSettings:SenderEmail"];
                    var appPassword = _configuration["EmailSettings:AppPassword"];
                    var smtpServer = _configuration["EmailSettings:SmtpServer"];
                    var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

                    var mail = new MailMessage();
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(senderEmail);  // You can send it to other addresses as well
                    mail.Subject = $"New Message from {model.Name}";
                    mail.Body = $"Name: {model.Name}\nEmail: {model.Email}\nMessage: {model.Message}";

                    using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(senderEmail, appPassword);
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mail);
                    }

                    ViewBag.Message = "Your message has been sent successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Error: {ex.Message}";
                }
            }

            return View("Contact");
        }
    }
}
