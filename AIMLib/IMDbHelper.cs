
namespace AIMLib
{
	/// <summary>
	/// Summary description for IMDbHelper.
	/// </summary>
	public class IMDbHelper
	{
		public IMDbHelper()
		{
			//
			// TODO: To be (possibly) implemented later
			//
		}
/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		public static int getQuoteCount(string keywords)
		{
			string hitCount = "-1"; 
			//WebClient wc = new WebClient();
			string url = "http://api.search.yahoo.com/WebSearchService/V1/webSearch";
			url += "?appid=" + APPLICATION_ID;
			url += "&query=" + HttpUtility.UrlEncode(keywords);
			url += "&zip=&start=1&results=1";

			XmlTextReader xRead = null;
			//Thread.Sleep(500);

			try
			{
				xRead = new XmlTextReader(url);
			}
			catch(WebException ex)
			{
				Console.WriteLine("Error1: " + ex.Message);	
				//Thread.Sleep(500);

				// Give it one more shot...
				try
				{
					xRead = new XmlTextReader(url);
				}
				catch(WebException ex2)
				{
					Console.WriteLine("Error2: " + ex2.Message);	
					throw new WebException("Could not load XML: " + ex2.Message);
				}
				catch(Exception ex2)
				{
					Console.WriteLine("Error2: " + ex2.Message);	
					throw new WebException("Could not load XML: " + ex2.Message);
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error1: " + ex.Message);	
			}


			//Console.WriteLine("--------------------------------------------------");
			while(xRead.Read())
			{
				if (xRead.Name == "ResultSet")
				{
					//Console.WriteLine("Results: [" + xRead.GetAttribute("totalResultsAvailable") + "]");
					hitCount = xRead.GetAttribute("totalResultsAvailable");
					break;
				}
			}
			//Console.WriteLine("--------------------------------------------------");

			return int.Parse(hitCount);
		}
*/
	}

}
