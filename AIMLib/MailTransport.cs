using System;
using System.Net.Mail;
using System.Configuration;

namespace AIMLib
{
	/// <summary>
	/// Summary description for MailTransport.
	/// </summary>
	public class MailTransport
	{
		public MailTransport()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		

		/// <summary>
		/// Sends an email to to our hero to let him know how the bot did.
		/// </summary>
		/// <param name="fromAddress"></param>
		/// <param name="toAddress"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public static void SendMail(string fromAddress, string toAddress, string subject, string body)
		{
			// --------------------------------------------------------------------------------
			// Add the timestamp?  This is important if we have multiple bots running and we want to know who 
			// answered first.  This assumes the computer clocks are somewhat synchronized.
			//
			// TODO: Use a string builder?

			body += "\r\n";
			body += "\r\n";
			body += "Bot: ";
            body += System.Configuration.ConfigurationManager.AppSettings["BotName"];
			body += "\r\n";
			body += DateTime.Now.ToLongTimeString();
			body += "\r\n";
			body += DateTime.Now.Ticks.ToString();

            subject += " [" + System.Configuration.ConfigurationManager.AppSettings["BotName"] + "]";

			// --------------------------------------------------------------------------------
			// Build the email

            MailMessage objMailMessage = new MailMessage(fromAddress, toAddress, subject, body);
            System.Net.Mail.SmtpClient smtpClient = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SMTPServer"]);

			// --------------------------------------------------------------------------------
			// Send the email...It is important not to get hung up on this step since it is just diagnostic.
			// Throw it in a try/catch and just log if there are problems.

			try
			{
                smtpClient.Send(objMailMessage);
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error: Could not send mail: [" + ex.Message + "]");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toAddress"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public static void SendMail(string toAddress, string subject, string body)
		{
            string fromAddress = System.Configuration.ConfigurationManager.AppSettings["SMTPFrom"];
			SendMail(fromAddress, toAddress, subject, body);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public static void SendMail(string subject, string body)
		{
            string toAddress = System.Configuration.ConfigurationManager.AppSettings["SMTPTo"];
			SendMail(toAddress, subject, body);
		}



	}
}
