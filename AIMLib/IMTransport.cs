using System;
using System.IO;
using System.Threading;

namespace AIMLib
{
	/// <summary>
	/// Summary description for IMTransport.
	/// </summary>
	public class IMTransport
	{
		public IMTransport()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static void sendIMMessage(string message)
		{
			sendIMMessage(message, "");
		}
		/// <summary>
		/// Generates a temporary AutoIt script and executes it, sending the 
		/// </summary>
		/// <param name="message"></param>
		public static void sendIMMessage(string message, string debugText)
		{
			string tempScriptFile = autoItScriptFolder + "temp" + DateTime.Now.Ticks.ToString() + ".aut";
			System.IO.StreamWriter sw;
			FileStream fs = new FileStream(tempScriptFile, FileMode.CreateNew, FileAccess.Write, FileShare.None);
			sw = new StreamWriter(fs);

			// --------------------------------------------------------------------------------
			// We need to build the AutoIt script so that the bot can respond.  I guess the easiest thing to do
			// would be to create 4 scripts and just call the appropriate one based on whether the value is a, b, c, or d.
			// We would just need to make sure the trillianIMMillionaireWindowName is set appropriately (or create different ones
			// for the dev and production environments.  I don't know...Maybe it should only be created if it does not exist...then 
			// it stays there forever.  The file could be autoitscript_[trillianIMMillionaireWindowName]_[RESPONSE].aut or something
			//
			// FYI, This script does not work with AutoIt v3.  I will change this script...

			sw.WriteLine("WinActivate, " + trillianIMMillionaireWindowName);
			sw.WriteLine("SetKeyDelay, 5");
			sw.WriteLine("Send, " + message + "{ENTER}");
			sw.Close();
			sw = null;

			// --------------------------------------------------------------------------------
			// Start the AutoIt script...This is the part that sends the keystrokes back to Trillian and responds

			System.Diagnostics.Process myProc = new System.Diagnostics.Process();
			myProc.StartInfo.FileName = autoItFile;
			myProc.StartInfo.Arguments = tempScriptFile;
			myProc.Start();

			// --------------------------------------------------------------------------------
			// We want to know how the bot responded.  We aren't always near the computer...so email the results 
			// to our hero or else he will the suspense will kill him while he is at work!

			AIMLib.MailTransport.SendMail( "Millionaire Bot Debug Info", debugText );

			// --------------------------------------------------------------------------------
			// Clean up our temp files again.

			try
			{
				// Wait for AutoIt to fire up and load the aut file
				Thread.Sleep(2000);
				// Delete the file and clean up
				File.Delete(tempScriptFile);
			}
			catch (Exception ex)
			{
				// Just log and continue
				Console.WriteLine("Could not delete file: " + ex.Message);
			}
		}

		private static string autoItScriptFolder = System.Configuration.ConfigurationManager.AppSettings["AutoItScriptFolder"];
		private static string autoItFile = System.Configuration.ConfigurationManager.AppSettings["AutoItPath"];

		private static string trillianIMMillionaireWindowName = System.Configuration.ConfigurationManager.AppSettings["IMWindowTitle"];
		//private static int responseDelayInMS = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ResponseDelay"]);
		//private static string trillianIMCCWindowName = System.Configuration.ConfigurationManager.AppSettings["IMCCWindowTitle"];

	}
}
