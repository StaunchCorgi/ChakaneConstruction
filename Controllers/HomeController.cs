using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KCMC_Maintenance.Models;
using System.ComponentModel.Design;
using System.Net;
using System.Text.Json.Nodes;
using System.Net.Mail;
using System.Net.Mime;


namespace KCMC_Maintenance.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult GetQuote(ContactDetails details)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid request data.");
        }

        try
        {
            SendQuoteEmail(details);
            return Ok("Quote request sent successfully.");
        }
        catch (SmtpException smtpEx)
        {
            return StatusCode(500, $"Email sending failed: {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }
    private void SendQuoteEmail(ContactDetails details)
    {   
        using var mail = new MailMessage();

        mail.From = new MailAddress("palesamorele@gmail.com", "KCMC Maintenance");
        mail.To.Add("palesamorele@gmail.com");
        mail.ReplyToList.Add(new MailAddress(details.Email, details.FullName));

        mail.Subject = $"New Quote Request - {details.Service}";
        mail.Body = $@"
    New quote request received:

    Full Name: {details.FullName}
    Phone: {details.ContactNumber}
    Email: {details.Email}
    Service: {details.Service}

    Message:
    {details.Message}
    ";

        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                "palesamorele@gmail.com",
                "bxukvmdwtzdqghox"
            ),
            EnableSsl = true
        };

        smtp.Send(mail);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
