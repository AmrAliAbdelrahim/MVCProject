using Demo.DAL.Models;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helpers
{
	public static class EmailSettings
	{
		public static void SendEmail(Email email)
		{
			var Clint = new SmtpClient("smtp.gmail.com", 587);
			Clint.EnableSsl = true;
			Clint.Credentials = new NetworkCredential("amr607048@gmail.com", "ifrgxnlauvkpbvjy");//i f r g x n l a u v k p b v j y
            Clint.Send("amr607048@gmail.com", email.To,email.Subject,email.Body);

		}
	}
}
