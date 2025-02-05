namespace MailroomManagement;

using System.Net.Mail;
using System.Net;
using System.Text;

class EmailSender
{
    // Method to send an email notification
    public static void SendEmail(string senderEmail, string password, string toEmail, string subject)
    {
        try
        {
            // Create the email message
            MailMessage message = new MailMessage();
            message.From = new MailAddress(senderEmail); // Set the sender's email address
            message.To.Add(toEmail);                     // Add the recipient's email address
           
            message.Subject = subject;                   // Set the subject line of the email
           
            // Define the HTML content for the email body
            string htmlContent = @"
            <html>
                <body>
                    <p>Dear Resident,</p>
                    <p>We have received a package for you at the leasing office. Due to limited storage space, please pick up your package within <strong>5 days</strong>.</p>
                    <p>If the package is not picked up within this time frame, it will be returned to the post office.</p>
                    <p>Thank you!</p>
                    <p>BuffTeks Apartment Leasing Office</p>
                </body>
            </html>
            ";

            // AlternateView allows for specifying different versions of the email content (HTML in this case)
            message.AlternateViews.Add(new AlternateView(new MemoryStream(Encoding.UTF8.GetBytes(htmlContent)), "text/html"));

            // Configure the SMTP client to send the email
            SmtpClient smtpClient = new SmtpClient("mail.privateemail.com", 587); // Set SMTP server and port
            smtpClient.EnableSsl = true;                                          // Enable SSL encryption for security
            smtpClient.UseDefaultCredentials = false;                             // Disable default credentials
            smtpClient.Credentials = new NetworkCredential(senderEmail, password); // Use provided sender email and password for authentication

            // Send the email
            smtpClient.Send(message);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            // Catch any exceptions that occur and display an error message
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}