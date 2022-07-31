using System;
using System.Net;
using System.Threading;
using System.Web;
using System.Xml;

namespace AIMLib
{
	/// <summary>
	/// Summary description for YahooHelper.
	/// </summary>
	public class YahooHelper
	{
        public static string APPLICATION_ID = System.Configuration.ConfigurationManager.AppSettings["YahooAppID"].ToString();

		public YahooHelper()
		{
			//
			// http://api.search.yahoo.com/WebSearchService/V1/webSearch?appid=1234&query=tron&zip=&start=1&results=1
			//
		}

		/// <summary>
		/// Gets keywords separate from the answer
		/// </summary>
		/// <param name="keyWords"></param>
		/// <param name="answer"></param>
		/// <returns></returns>
		public static int getHitCount(string keyWords, string answer)
		{
			if (answer.Trim() == "")
				return Question.HITCOUNT_INVALIDCHOICE;		// If there is no answer (if it is a 50/50) then make sure we can't tie!!!

			return getHitCount("\"" + HttpUtility.HtmlEncode(answer) + "\" " + keyWords);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		public static int getHitCount(string keywords)
		{
			// No need to search if there are no keywords
			if (keywords.Trim() == "") return -1;

			string hitCount = "-1"; 

			string url = "http://api.search.yahoo.com/WebSearchService/V1/webSearch";
			url += "?appid=" + APPLICATION_ID;
			url += "&query=" + HttpUtility.UrlEncode(keywords);
			url += "&zip=&start=1&results=1";

			XmlTextReader xRead = null;

			bool hitsFetched = false;
			int retryCount = 0;
			int MAX_RETRIES = 2;
			int results = -999999;

			while (retryCount < MAX_RETRIES && !hitsFetched)
			{
				if (retryCount > 0)
				{
					// If this is a retry, then wait 1/20 of a second
					Thread.Sleep(50);
					Console.WriteLine("Now retrying...retry #" + retryCount.ToString());
				}

				// Load the URL into an XML Document
				try
				{
					xRead = new XmlTextReader(url);

					while(xRead.Read())
					{
						if (xRead.Name == "ResultSet")
						{
							hitCount = xRead.GetAttribute("totalResultsAvailable");
							results = int.Parse(hitCount);
							hitsFetched = true;
							break;
						}
					}
					if (!hitsFetched)
					{
						Console.WriteLine("Error finding the totalResultsAvailable attribute!");
					}
				}
				catch(WebException ex)
				{
					Console.WriteLine("WebException loading XML: (" + retryCount.ToString() + ") " + ex.Message);	
					//return -999999;
				}
				catch(Exception ex)
				{
					Console.WriteLine("Exception loading XML: (" + retryCount.ToString() + ") " + ex.Message);	
					//return -999999;
				}
				retryCount++;
			}

			return results;
		}
	}
}
