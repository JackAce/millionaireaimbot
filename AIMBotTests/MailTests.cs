using System;
using AIMLib;
using NUnit.Framework;

namespace AIMBotTests
{
	/// <summary>
	/// Summary description for MailTests.
	/// </summary>
	[TestFixture]
	public class MailTests
	{
		public MailTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Mail_Send_4Parameter_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Mail_Send_4Parameter_Test");
			Console.WriteLine("");

			try
			{
				MailTransport.SendMail("from@cf9.com", "eh@cf9.com", "Test: Mail_Send_4Parameters", "test body");
			}
			catch(Exception ex)
			{
				Assert.Fail("Error: Could not send mail: [" + ex.Message + "]");
			}

			Assert.IsTrue(true);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Mail_Send_3Parameter_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Mail_Send_3Parameter_Test");
			Console.WriteLine("");

			try
			{
				MailTransport.SendMail("eh@cf9.com", "Test: Mail_Send_3Parameters", "test body");
			}
			catch(Exception ex)
			{
				Assert.Fail("Error: Could not send mail: [" + ex.Message + "]");
			}

			Assert.IsTrue(true);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Mail_Send_2Parameter_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Mail_Send_2Parameter_Test");
			Console.WriteLine("");

			try
			{
				MailTransport.SendMail("Test: Mail_Send_2Parameters", "test body");
			}
			catch(Exception ex)
			{
				Assert.Fail("Error: Could not send mail: [" + ex.Message + "]");
			}

			Assert.IsTrue(true);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Mail_Send_BruteForce_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Mail_Send_BruteForce_Test");
			Console.WriteLine("");

			string tempSubject = "Test: Mail_Send_BruteForce_Test: ";

			for (int i = 0; i < 20; i++)
			{
				try
				{
					Console.WriteLine("Sending Email: " + tempSubject + "Test [" + i.ToString() + "]");
					MailTransport.SendMail(tempSubject + "Test [" + i.ToString() + "]", "test body");
				}
				catch(Exception ex)
				{
					Assert.Fail("Error: Could not send mail: [" + ex.Message + "]");
					return;
				}
			}

			Assert.IsTrue(true);
		}

	}
}
