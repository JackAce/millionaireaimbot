using System;
using System.Threading;

namespace AIMLib
{
	/// <summary>
	/// Summary description for GoogleHelper.
	/// </summary>
	public class GoogleHelper
	{
        public static string API_LICENSE_KEY = System.Configuration.ConfigurationManager.AppSettings["GoogleAPILicenseKey"].ToString();

		public GoogleHelper()
		{

		}

		/// <summary>
		/// Gets keywords separate from the answer
		/// </summary>
		/// <param name="keyWords"></param>
		/// <param name="answer"></param>
		/// <returns></returns>
		public static int getHitCount(string keyWords, string answer, int defaultHitCount)
		{
			if (answer.Trim() == "")
				return Question.HITCOUNT_INVALIDCHOICE;		// If there is no answer (if it is a 50/50) then make sure we can't tie!!!

			return getHitCount("\"" + answer + "\" " + keyWords, defaultHitCount);
		}

		/// <summary>
		/// Gets the results of all keywords
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		public static int getHitCount(string keywords, int defaultHitCount)
		{
			// No need to search if there are no keywords
			if (keywords.Trim() == "") return defaultHitCount;

			bool hitsFetched = false;
			int retryCount = 0;
			int MAX_RETRIES = 2;
			int results = defaultHitCount;

			keywords = keywords.Replace("  ", " ");

			while (retryCount < MAX_RETRIES && !hitsFetched)
			{
				if (retryCount > 0)
				{
					// If this is a retry, then wait 1/20 of a second
					Thread.Sleep(50);
					Console.WriteLine("..now retrying...retry #" + retryCount.ToString());
				}

				Google.GoogleSearchService gss = new Google.GoogleSearchService();
				try 
				{
					// Invoke the search method
					Google.GoogleSearchResult r = gss.doGoogleSearch(
						API_LICENSE_KEY, 
						keywords,			// FYI -- GOOGLE ONLY ALLOWS 10 WORDS IN THE KEYWORD LIST!!!
						0, 
						1, 
						true,				// false documentFiltering 
						"",					// Restriction Codes
						true,				// SafeSearch?  AKA "Do you want to hide pr0n?"
						"lang_en",			// lr = Language Restriction..."lang_en" = English
						"",					// ie = Input Encoding
						"");				// oe = Output Encoding

					// Extract the estimated number of results for the search and display it
					results = r.estimatedTotalResultsCount;
					hitsFetched = true;
				}
				catch (System.Web.Services.Protocols.SoapException ex) 
				{
					Console.WriteLine("GoogleHelper SoapException: " + ex.Message);
				}
				catch (System.Net.WebException ex) 
				{
					Console.WriteLine("GoogleHelper WebException: " + ex.Message);
				}
				catch (Exception ex) 
				{
					Console.WriteLine("GoogleHelper Exception: " + ex.Message);
				}

				retryCount++;
			}

			return results;
		}

		/*
		public static int getHitCount(string keywords, bool isReverseSearch)
		{
			
		}
		*/

	}
}
