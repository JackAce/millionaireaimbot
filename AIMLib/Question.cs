using System;
using System.Collections;
using System.Text;

namespace AIMLib
{
	/// <summary>
	/// Summary description for Question.
	/// </summary>
	public class Question
	{
		/// <summary>
		/// The SearchService dictates which internet search service you want to query to find the best results.
		/// </summary>
		public enum SearchService
		{
			Yahoo,
			Google,
			Wikipedia,
			Dictionary,			// Use only for definition lookups (TO BE IMPLEMENTED)
			IMDb,				// Use only for Movie/TV Serarches (TO BE IMPLEMENTED)
			AllMusic			// Use for Music searches (TO BE IMPLEMENTED)
		}

		/// <summary>
		/// The SearchType indicates whether we want to find the highest results or the lowest results.  We would 
		/// want to get the lowest results if the question were in the form "Which of the following are NOT..."
		/// </summary>
		public enum SearchType
		{
			FindHighestResults,
			FindLowestResults
		}

		/// <summary>
		/// This is just an enum of the letters a, b, c, and d.
		/// </summary>
		public enum AnswerCode
		{
			a,
			b,
			c,
			d
		}
/*
		public static SearchService DefaultSearchService
		{
			get {return defaultSearchService;}	
			set {defaultSearchService = value;}	
		}
		private static SearchService defaultSearchService = SearchService.Yahoo;
*/
		public const double CONFIDENCE_THRESHOLD = 0.07;				// When do we decide "this is too close a call"
		public const int HITCOUNT_THRESHOLD = 3;						// What do we consider "too few" results
		public const int HITCOUNT_INVALIDCHOICE = -1;					// How do we seed an invalid choice?
		public const int HITCOUNT_INVALIDCHOICE_REVERSE = 999999;		// How do we seed an invalid choice for a reverse search?
		
		public Question()
		{
/*
			// Get the default search service from the config file
			if (System.Configuration.ConfigurationManager.AppSettings["DefaultSearchService"] != null)
			{
				if (System.Configuration.ConfigurationManager.AppSettings["DefaultSearchService"].ToLower() == "google")
					defaultSearchService = SearchService.Google;
				else
					defaultSearchService = SearchService.Yahoo;
			}
*/
		}

		/// <summary>
		/// Gets the string that terminates the answer in question.  This ASSUMES that the raw text is in the appropriate format.
		/// It assumes that the answer prefixes are capitalized and that "Answer by typing..." is also capitalized properly.  You
		/// DON'T need to make any assumptions regarding \r, \n, \t or any spaces...they will be converted into spaces anyway.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="rawText"></param>
		/// <returns></returns>
		public static int getAnswerEndIndex(int index, string rawText)
		{
			int tempIndex = -1;
			if (index == 0)
			{
				tempIndex = rawText.IndexOf("B:");
			}
			if (index <= 1 && tempIndex < 0)
			{
				tempIndex = rawText.IndexOf("C:");
			}
			if (index <= 2 && tempIndex < 0)
			{
				tempIndex = rawText.IndexOf("D:");
			}
			if (tempIndex < 0)
			{
				tempIndex = rawText.IndexOf("Answer by typing the letter ONLY.");
			}
			if (tempIndex < 0)
			{
				tempIndex = rawText.Length;
			}
			return tempIndex;
		}

		/// <summary>
		/// Populates the array of answers.
		/// </summary>
		/// <param name="fullText"></param>
		/// <returns></returns>
		public static string[] getAnswers(string fullText)
		{
			string[] returnArray = new string[4];
			for (int i = 0; i < 4; i++)
			{
				returnArray[i] = getAnswer(fullText, i);
			}

			return returnArray;
		}

		/// <summary>
		/// Gets the hit counts based on the config settings.  The default SearchService is used first.
		/// </summary>
		/// <param name="keywords"></param>
		/// <param name="answerList"></param>
		/// <param name="isReverseLookup"></param>
		/// <param name="getSecondOpinion"></param>
		/// <returns></returns>
		public static int[] getDefaultHitCounts(string keywords, string[] answerList, bool isReverseLookup, bool getSecondOpinion)
		{
            if (System.Configuration.ConfigurationManager.AppSettings["DefaultSearchService"].ToLower() == "yahoo")
			{
				return getYahooHitCounts(keywords, answerList, isReverseLookup, getSecondOpinion);
			}
			else
			{
				return getGoogleHitCounts(keywords, answerList, isReverseLookup, getSecondOpinion);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keywords"></param>
		/// <param name="answerList"></param>
		/// <returns></returns>
		public static int[] getGoogleHitCounts(string keywords, string[] answerList, bool isReverseLookup, bool getSecondOpinion)
		{
			int[] yahooHitCounts = new int[4];
			int[] googleHitCounts = new int[4];
			int[] finalHitCounts = new int[4];

			int yahooTotalHits;
			int googleTotalHits;
			string strippedKeywords = removeCommonWords(keywords.Replace("\"", ""));			// This is used if we don't get too many hits
			double yahooConfidenceIndex = 0.0;
			double googleConfidenceIndex = 0.0;
			int defaultHits;
			if (isReverseLookup)
				defaultHits = HITCOUNT_INVALIDCHOICE_REVERSE;
			else
				defaultHits = HITCOUNT_INVALIDCHOICE;

			// --------------------------------------------------------------------------------
			// Remove any quotes from the answers

			for (int i = 0; i < 4; i++)
			{
				answerList[i] = answerList[i].Replace("\"", "");
			}

			// --------------------------------------------------------------------------------
			// Perform the basic Google search

			Console.WriteLine("");
			Console.WriteLine("Search option 1: Google Basic");
			for (int i = 0; i < 4; i++)
			{
				googleHitCounts[i] = GoogleHelper.getHitCount(keywords, answerList[i], defaultHits); 
			}
			googleTotalHits = getTotalHitCount(googleHitCounts);
			googleConfidenceIndex = Question.getConfidenceIndex(googleHitCounts, isReverseLookup);

			// --------------------------------------------------------------------------------
			// If these search results aren't so hot, then try removing quotes if they are part of the keywords.

			if (keywords.IndexOf("-") > -1 && googleTotalHits < 3)
			{
				Console.WriteLine("");
				Console.WriteLine("Search option 2: Google Dashless");
				Console.WriteLine("Not enough hits (" + googleTotalHits.ToString() + ")");		// WORDS WITH DASHES STRIPPED (e.g. \"ivory-tickling\")");
				for (int i = 0; i < 4; i++)
				{
					googleHitCounts[i] = GoogleHelper.getHitCount(removeHyphenatedWords(keywords), answerList[i], defaultHits); 
				}
				googleTotalHits = getTotalHitCount(googleHitCounts);
				googleConfidenceIndex = Question.getConfidenceIndex(googleHitCounts, isReverseLookup);
			}

			if (keywords.IndexOf("\"") > -1 && googleTotalHits < 3)
			{
				Console.WriteLine("");
				Console.WriteLine("Search option 3: Google Quoteless");
				Console.WriteLine("Not enough hits (" + googleTotalHits.ToString() + "): QUOTES STRIPPED");
				for (int i = 0; i < 4; i++)
				{
					googleHitCounts[i] = GoogleHelper.getHitCount(strippedKeywords, answerList[i], defaultHits); 
				}
				googleTotalHits = getTotalHitCount(googleHitCounts);
				googleConfidenceIndex = Question.getConfidenceIndex(googleHitCounts, isReverseLookup);
			}

			finalHitCounts = googleHitCounts;

			// --------------------------------------------------------------------------------
			// How confident are we?  Do we need a second opinion?

			if (getSecondOpinion)
			{
				if (googleTotalHits < 3 || googleConfidenceIndex < CONFIDENCE_THRESHOLD)
				{
					// --------------------------------------------------------------------------------
					// Perform the basic Yahoo! search

					Console.WriteLine("");
					Console.WriteLine("Search option 4: Yahoo! Basic");

					for (int i = 0; i < 4; i++)
					{
						yahooHitCounts[i] = YahooHelper.getHitCount(keywords, answerList[i]); 
					}
					yahooTotalHits = getTotalHitCount(yahooHitCounts);
					yahooConfidenceIndex = Question.getConfidenceIndex(yahooHitCounts, isReverseLookup);

					// --------------------------------------------------------------------------------
					// If these search results aren't so hot, then try removing quotes if they are part of the keywords.

					if (keywords.IndexOf("-") > -1 && yahooTotalHits < 3)
					{
						Console.WriteLine("");
						Console.WriteLine("Search option 5: Yahoo! Dashless");
						Console.WriteLine("Not enough hits (" + yahooTotalHits.ToString() + ")");		// WORDS WITH DASHES STRIPPED (e.g. \"ivory-tickling\")");
						for (int i = 0; i < 4; i++)
						{
							yahooHitCounts[i] = YahooHelper.getHitCount(removeHyphenatedWords(keywords), answerList[i]); 
						}
						yahooTotalHits = getTotalHitCount(yahooHitCounts);
						yahooConfidenceIndex = Question.getConfidenceIndex(yahooHitCounts, isReverseLookup);
					}

					if (keywords.IndexOf("\"") > -1 && yahooTotalHits < 3)
					{
						Console.WriteLine("");
						Console.WriteLine("Search option 6: Yahoo! Quoteless");
						Console.WriteLine("Not enough hits (" + yahooTotalHits.ToString() + "): QUOTES STRIPPED");

						for (int i = 0; i < 4; i++)
						{
							yahooHitCounts[i] = YahooHelper.getHitCount(strippedKeywords, answerList[i]); 
						}
						yahooTotalHits = getTotalHitCount(yahooHitCounts);
						yahooConfidenceIndex = Question.getConfidenceIndex(yahooHitCounts, isReverseLookup);
					}

					// --------------------------------------------------------------------------------
					// Compare the two confidence levels

					if (yahooTotalHits > 3 && yahooConfidenceIndex > googleConfidenceIndex)
						finalHitCounts = yahooHitCounts;
					else if (googleTotalHits <= 3 && googleTotalHits <= 3 && yahooConfidenceIndex > googleConfidenceIndex)
						finalHitCounts = yahooHitCounts;

				}
				
			}

			return finalHitCounts;
		}

		/// <summary>
		/// Yahoo should be the default SearchService since it returns the best search results to 
		/// </summary>
		/// <param name="keywords"></param>
		/// <param name="answerList"></param>
		/// <returns></returns>
		public static int[] getYahooHitCounts(string keywords, string[] answerList, bool isReverseLookup, bool getSecondOpinion)
		{
			int[] yahooHitCounts = new int[4];
			int[] googleHitCounts = new int[4];
			int[] finalHitCounts = new int[4];

			int yahooTotalHits;
			int googleTotalHits;
			string strippedKeywords = removeCommonWords(keywords.Replace("\"", ""));			// This is used if we don't get too many hits
			double yahooConfidenceIndex = 0.0;
			double googleConfidenceIndex = 0.0;
			int defaultHits;
			if (isReverseLookup)
				defaultHits = HITCOUNT_INVALIDCHOICE_REVERSE;
			else
				defaultHits = HITCOUNT_INVALIDCHOICE;

			// --------------------------------------------------------------------------------
			// Remove any quotes from the answers

			for (int i = 0; i < 4; i++)
			{
				answerList[i] = answerList[i].Replace("\"", "");
			}

			// --------------------------------------------------------------------------------
			// Perform the basic Yahoo! search

			Console.WriteLine("");
			Console.WriteLine("Search option 1: Yahoo! Basic");
			for (int i = 0; i < 4; i++)
			{
				yahooHitCounts[i] = YahooHelper.getHitCount(keywords, answerList[i]); 
			}
			yahooTotalHits = getTotalHitCount(yahooHitCounts);
			yahooConfidenceIndex = Question.getConfidenceIndex(yahooHitCounts, isReverseLookup);

			// --------------------------------------------------------------------------------
			// If these search results aren't so hot, then try removing words with a dash
			// This is also known as the "ivory-tickling" tweak.

			if (keywords.IndexOf("-") > -1 && yahooTotalHits < HITCOUNT_THRESHOLD)
			{
				Console.WriteLine("");
				Console.WriteLine("Search option 2: Yahoo! Dashless");
				Console.WriteLine("Not enough hits (" + yahooTotalHits.ToString() + ")");		// WORDS WITH DASHES STRIPPED (e.g. \"ivory-tickling\")");
				for (int i = 0; i < 4; i++)
				{
					yahooHitCounts[i] = YahooHelper.getHitCount(removeHyphenatedWords(keywords), answerList[i]); 
				}
				yahooTotalHits = getTotalHitCount(yahooHitCounts);
				yahooConfidenceIndex = Question.getConfidenceIndex(yahooHitCounts, isReverseLookup);
			}

			// --------------------------------------------------------------------------------
			// If these search results aren't so hot, then try removing quotes if they are part of the keywords.

			if (keywords.IndexOf("\"") > -1 && yahooTotalHits < HITCOUNT_THRESHOLD)
			{
				Console.WriteLine("");
				Console.WriteLine("Search option 3: Yahoo! Quoteless");
				Console.WriteLine("Not enough hits (" + yahooTotalHits.ToString() + "): QUOTES STRIPPED");
				for (int i = 0; i < 4; i++)
				{
					yahooHitCounts[i] = YahooHelper.getHitCount(strippedKeywords, answerList[i]); 
				}
				yahooTotalHits = getTotalHitCount(yahooHitCounts);
				yahooConfidenceIndex = Question.getConfidenceIndex(yahooHitCounts, isReverseLookup);
			}

			finalHitCounts = yahooHitCounts;

			// --------------------------------------------------------------------------------
			// How confident are we?  Do we need a second opinion?

			if (getSecondOpinion)
			{
				if (yahooTotalHits < HITCOUNT_THRESHOLD || yahooConfidenceIndex < CONFIDENCE_THRESHOLD)
				{
					// --------------------------------------------------------------------------------
					// Perform the basic Google! search

					Console.WriteLine("");
					Console.WriteLine("Search option 4: Google Basic");

					for (int i = 0; i < 4; i++)
					{
						googleHitCounts[i] = GoogleHelper.getHitCount(keywords, answerList[i], defaultHits); 
					}
					googleTotalHits = getTotalHitCount(googleHitCounts);
					googleConfidenceIndex = Question.getConfidenceIndex(googleHitCounts, isReverseLookup);

					if (keywords.IndexOf("-") > -1 && googleTotalHits < HITCOUNT_THRESHOLD)
					{
						Console.WriteLine("");
						Console.WriteLine("Search option 5: Google Dashless");
						Console.WriteLine("Not enough hits (" + googleTotalHits.ToString() + ")");		// WORDS WITH DASHES STRIPPED (e.g. \"ivory-tickling\")");
						for (int i = 0; i < 4; i++)
						{
							googleHitCounts[i] = GoogleHelper.getHitCount(removeHyphenatedWords(keywords), answerList[i], defaultHits); 
						}
						googleTotalHits = getTotalHitCount(googleHitCounts);
						googleConfidenceIndex = Question.getConfidenceIndex(googleHitCounts, isReverseLookup);
					}

					// --------------------------------------------------------------------------------
					// If these search results aren't so hot, then try removing quotes if they are part of the keywords.

					if (keywords.IndexOf("\"") > -1 && googleTotalHits < HITCOUNT_THRESHOLD)
					{
						Console.WriteLine("");
						Console.WriteLine("Search option 6: Google Quoteless");
						Console.WriteLine("Not enough hits (" + googleTotalHits.ToString() + "): QUOTES STRIPPED");

						for (int i = 0; i < 4; i++)
						{
							googleHitCounts[i] = GoogleHelper.getHitCount(strippedKeywords, answerList[i], defaultHits); 
						}
						googleTotalHits = getTotalHitCount(googleHitCounts);
						googleConfidenceIndex = Question.getConfidenceIndex(googleHitCounts, isReverseLookup);
					}

					// --------------------------------------------------------------------------------
					// Compare the two confidence levels

					// If the google search had a good hit count OR it had a better confidence index
					if (googleTotalHits > HITCOUNT_THRESHOLD && (googleConfidenceIndex > yahooConfidenceIndex || yahooTotalHits <= HITCOUNT_THRESHOLD))
						finalHitCounts = googleHitCounts;
					else if (yahooTotalHits <= HITCOUNT_THRESHOLD && googleTotalHits <= HITCOUNT_THRESHOLD && googleConfidenceIndex > yahooConfidenceIndex)
						finalHitCounts = googleHitCounts;

				}
				
			}

			return finalHitCounts;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fullText"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected static string getAnswer(string fullText, int index)
		{
			// "What landlocked state's official bird is the sea gull? A: Arizona B: Utah C: Kansas D: South Dakota Answer by typing the letter ONLY.  Please answer right away."
			string answerBeginText;
			//string answerEndText;
			int answerBeginIndex;
			int answerEndIndex;

			string tempAnswer;

			switch(index)
			{
				case 0:
					answerBeginText = "A:";
					//answerEndText = "B:";		// We CAN'T assume that the next answer is "B:" because it may have been eliminated by a 50/50 lifeline
					break;
				case 1:
					answerBeginText = "B:";
					//answerEndText = "C:";
					break;
				case 2:
					answerBeginText = "C:";
					//answerEndText = "D:";
					break;
				case 3:
					answerBeginText = "D:";
					//answerEndText = "Answer by typing the letter ONLY.";		// I guess we can (somewhat) that this should be here, but just put the logic in the getAnswerEndIndex function
					break;
				default:
					throw new ArgumentException("The answer index you provided (" + index.ToString() + ") is not valid.  It must be 0-3.");
			}

			answerBeginIndex = fullText.IndexOf(answerBeginText);
			answerEndIndex = Question.getAnswerEndIndex(index, fullText);

			// --------------------------------------------------------------------------------
			// If we run into any issues that we can't interpret, just return a blank string...
			
			if (answerBeginIndex < 0)
			{
				return "";
			}
			if (answerEndIndex < 0)
			{
				// If we can't find the end text, then just assume the rest of the rawText is the rest of the answer
				answerEndIndex = fullText.Length;
			}

			if (answerBeginIndex > 0 && answerEndIndex > 0 && answerEndIndex > answerBeginIndex)
				tempAnswer = fullText.Substring(answerBeginIndex + answerBeginText.Length, answerEndIndex - (answerBeginIndex + answerBeginText.Length)).Trim();
			else
				tempAnswer = "";

			// --------------------------------------------------------------------------------
			// If the answer is just a number (specifically 0-10), convert it to a word otherwise, you may get 
			// skewed astronomical numbers.
			//
			// I don't think this really helps...Look at the butter question...
			//
			// In the U.S. a standard stick of butter is equal to how many cups?  A: 1/4 B: 1/2 C: 3/4 D: 1
			//
			// We will always get 1 or "one" even though the actual answer is 1/2.

			switch(tempAnswer)
			{
				case "0":
					tempAnswer = "zero";
					break;
				case "1":
					tempAnswer = "one";
					break;
				case "2":
					tempAnswer = "two";
					break;
				case "3":
					tempAnswer = "three";
					break;
				case "4":
					tempAnswer = "four";
					break;
				case "5":
					tempAnswer = "five";
					break;
				case "6":
					tempAnswer = "six";
					break;
				case "7":
					tempAnswer = "seven";
					break;
				case "8":
					tempAnswer = "eight";
					break;
				case "9":
					tempAnswer = "nine";
					break;
				case "10":
					tempAnswer = "ten";
					break;
			}

			// --------------------------------------------------------------------------------

			tempAnswer = fixUnicodeChars(tempAnswer);
			return tempAnswer;
		}

		/// <summary>
		/// Extracts the question text from the full text sent via IM.  This includes the "?"
		/// </summary>
		/// <param name="fullText"></param>
		/// <returns></returns>
		public static string getQuestion(string fullText)
		{
			//" Hi!  A contestant is in the hot seat and needs your help.   "
			int questionStartIndex = fullText.IndexOf(".");

			int questionMarkIndexLast = fullText.LastIndexOf("?");
			int questionMarkIndexFirst = fullText.IndexOf("?");
			int questionMarkIndex = questionMarkIndexLast;
			int tempIndex;

			if (questionMarkIndexLast != questionMarkIndexFirst)
			{
				tempIndex = fullText.IndexOf(":");

				// Check to see whether the last question mark is after the first colon
				if (questionMarkIndexLast > tempIndex && questionMarkIndexFirst < tempIndex)
					questionMarkIndex = questionMarkIndexFirst;
			}

			if (questionMarkIndex > 0 && questionStartIndex > 0)
			{
				fullText = fullText.Substring(questionStartIndex + 1, questionMarkIndex - questionStartIndex).Trim();
				fullText = fullText.Replace("\r", " ");
				fullText = fullText.Replace("\n", " ");
				fullText = fullText.Replace("\t", " ");
			}

			fullText = removeDoubleSpaces(fullText).Trim();

			return fullText;
		}

		/// <summary>
		/// The getQuestion function replaces \r, \n, and \t with a single space.  The removeCommonWords function replaces
		/// common words with a space.  We do this several other places, too.  This function cleans up the double spaces and 
		/// replaces them with one single space.
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		public static string removeDoubleSpaces(string keywords)
		{
			while(keywords.IndexOf("  ") >= 0)
			{
				keywords = keywords.Replace("  ", " ");
			}
			return keywords;
		}

		/// <summary>
		/// The regular expression is built from settings in the app.config file.  This config file just contains a 
		/// pipe delimited string of words that do nothing but skew our results.  The common words are replaced with a single 
		/// space.
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		public static string removeCommonWords(string keywords)
		{
			string commonWordList = System.Configuration.ConfigurationManager.AppSettings["CommonWordList"].ToLower();

			//keywords = " " + keywords + " ";
			keywords = System.Text.RegularExpressions.Regex.Replace(keywords, "\\b(" + commonWordList + ")\\b", " ");

			return keywords;
		}

		/// <summary>
		/// Pulls the keywords from the raw question text.  This assumes that we want to group consecutive capitalized words 
		/// in quotes.
		/// </summary>
		/// <param name="rawQuestion"></param>
		/// <returns></returns>
		public static string getKeywords(string rawQuestion)
		{
			return getKeywords(rawQuestion, true);
		}

		/// <summary>
		/// This does additional string parsing, performing the following actions:
		/// * Cleaning the string of \r, \n, and \t
		/// * Taking out "U.S." from the string...perhaps we should just add it to the config file...
		/// * Preserving quotes in the question (this will GREATLY improve the accuracy)
		/// * Attempting to add "smart quotes" when consecutive words are capitalized (this usually helps accuracy)
		/// * Replacing uncommon "Millionairesque" words with synonyms that are likely to be part of search results (e.g. "best-seller" -> "book").
		/// </summary>
		/// <param name="rawQuestion"></param>
		/// <param name="groupCapitals"></param>
		/// <returns></returns>
		public static string getKeywords(string rawQuestion, bool groupCapitals)
		{
			ArrayList phraseList = new ArrayList();

			rawQuestion = rawQuestion.Replace("\r", " ");		// Remove line returns
			rawQuestion = rawQuestion.Replace("\n", " ");		// Remove page feeds
			rawQuestion = rawQuestion.Replace("\t", " ");		// Remove tabs

			// Remove HTML tags -- It is possible that the IM might contain a link
			rawQuestion = System.Text.RegularExpressions.Regex.Replace(rawQuestion, (@"<[^>]+>|</[^>]+>"), "");

			// --------------------------------------------------------------------------------
			// Crack the string on the quote character.  Add each of the quoted phrases to a phrase list.

			string[] keywordArray = rawQuestion.Split('\"');

			rawQuestion = "";
			for (int i = 0; i < keywordArray.Length; i++)
			{
				if (i % 2 == 0)
				{
					// This is a non quoted portion of the text
					if (rawQuestion != "")
						rawQuestion = rawQuestion + " ";

					rawQuestion = rawQuestion + keywordArray[i].Trim();
				}
				else
				{
					// This a quoted phrase
					phraseList.Add(keywordArray[i].Trim());
				}
			}

			// --------------------------------------------------------------------------------
			// Remove this worthless word before we group capitals into quotes
			
			rawQuestion = rawQuestion.Replace(" U.S. ", " ");	// This keyword is useless TODO: Handle "In the U.S., a standard..."

			// --------------------------------------------------------------------------------
			// If any consecutive words are capitalized, put them in quotes.

			if (groupCapitals)
			{
				rawQuestion = encloseConsecutiveCapitalWordsInQuotes(rawQuestion);	

				// Let's RESPLIT the rawQuestion because we may have added some phrases

				keywordArray = rawQuestion.Split('\"');

				rawQuestion = "";
				for (int i = 0; i < keywordArray.Length; i++)
				{
					if (i % 2 == 0)
					{
						// This is a non quoted portion of the text
						if (rawQuestion != "")
							rawQuestion = rawQuestion + " ";

						rawQuestion = rawQuestion + keywordArray[i];
					}
					else
					{
						// This a quoted phrase
						phraseList.Add(keywordArray[i]);
					}
				}
			}

			// --------------------------------------------------------------------------------
			// We need to do this before we perform the "ToLower()" function.  This is because we want to
			// preserve hyphenated words that are capitalized.

			rawQuestion = removeHyphenatedWords(rawQuestion);		// Are we sure we want to do this?

			// Replace certain phrases with better keywords...for example:

			// "in the folk song..." should be replaced with "lyrics"
			// "in the hit song..." should be replaced with "lyrics"
			// "in the disco song..." should be replaced with "lyrics"

			rawQuestion = " " + rawQuestion.ToLower() + " ";	// Pad the string with spaces
			rawQuestion = fixUnicodeChars(rawQuestion);

			rawQuestion = System.Text.RegularExpressions.Regex.Replace(rawQuestion, "\\b(in the [\\w]* song)\\b", "lyrics");

			//rawQuestion = rawQuestion.Replace("\"", " ");		// This may make things less accurate
			
			rawQuestion = rawQuestion.Replace(",\"", "\"");
			
			rawQuestion = rawQuestion.Replace("\'s ", " ");
			rawQuestion = rawQuestion.Replace("s\' ", " ");
			rawQuestion = rawQuestion.Replace(" d.c. ", " dxxxxxcxxxxx ");		// Hide me
			rawQuestion = rawQuestion.Replace(" l.a. ", " lxxxxxaxxxxx ");		// Hide me
			//rawQuestion = rawQuestion.Replace(" u.s. ", " uxxxxxsxxxxx ");	// Hide me
			rawQuestion = rawQuestion.Replace(" u.n. ", " uxxxxxnxxxxx ");		// Hide me
			rawQuestion = rawQuestion.Replace("-", "dashdashdash");				// Hide me -- This prevents the creation of half-dashed words

			rawQuestion = rawQuestion.Replace(". ", " ");
			rawQuestion = rawQuestion.Replace(", ", " ");
			rawQuestion = rawQuestion.Replace("?", " ");
			rawQuestion = rawQuestion.Replace(": ", " ");
			//rawQuestion = rawQuestion.Replace("???", "e");
			rawQuestion = rawQuestion.Replace("ï¿½", "e");		// This is the non unicode bastardization of the é
			//rawQuestion = rawQuestion.Replace("ï¿½", "o");

			rawQuestion = rawQuestion.Replace(" bestseller ", " book ");
			rawQuestion = rawQuestion.Replace(" best-selling ", " book ");

			rawQuestion = rawQuestion.Replace(" charttopper ", " song ");
			rawQuestion = rawQuestion.Replace(" chart-topping ", " song ");

			//rawQuestion = rawQuestion.Replace("in the xxxxx song", " lyrics ");

			// TODO: Replace 200x and 19xx with "Year 200x" and "Year 19xx" or some shit like that

			rawQuestion = removeCommonWords(rawQuestion);

			rawQuestion = rawQuestion.Replace("dxxxxxcxxxxx", "d.c.");	// Show me again
			rawQuestion = rawQuestion.Replace("lxxxxxaxxxxx", "l.a.");	// Show me again
			rawQuestion = rawQuestion.Replace("uxxxxxsxxxxx", "u.s.");	// Show me again
			rawQuestion = rawQuestion.Replace("uxxxxxnxxxxx", "u.n.");	// Show me again
			rawQuestion = rawQuestion.Replace("dashdashdash", "-");		// Show me again -- This prevents the creation of half-dashed words

			// Strip out any of the double spaces
			rawQuestion = removeDoubleSpaces(rawQuestion);

			rawQuestion = rawQuestion.Trim();

			for (int i = 0; i < phraseList.Count; i++)
			{
				rawQuestion = "\"" + phraseList[i] + "\" " + rawQuestion;
			}
			rawQuestion = rawQuestion.Replace(",\"", "\"");				// If there is a period before the end of a quote, then remove it
			rawQuestion = rawQuestion.Replace(".\"", "\"");				// If there is a comma before the end of a quote, remove it

			return rawQuestion;
		}

		/// <summary>
		/// Returns the question with consecutive words in quotes
		/// </summary>
		/// <param name="rawQuestion"></param>
		/// <returns></returns>
		private static string encloseConsecutiveCapitalWordsInQuotes(string rawQuestion)
		{
			int currUpperIndexStart = -1;

			/* Assume this is already done
			rawQuestion = rawQuestion.Replace("\r", " ");		// Remove line returns
			rawQuestion = rawQuestion.Replace("\n", " ");		// Remove page feeds
			rawQuestion = rawQuestion.Replace("\t", " ");		// Remove tabs
			*/

			// Strip off the question mark
			rawQuestion = rawQuestion.Substring(0, rawQuestion.Length - 1);

			rawQuestion = removeDoubleSpaces(rawQuestion).Trim();
			string[] wordArray = rawQuestion.Split(' ');			// This is an array of all the words in the 

			int tempAsc = -1;

			// --------------------------------------------------------------------------------
			// Loop through all of the words starting with the second word (the first word is always capitalized and should be ignored)

			for (int i = 1; i < wordArray.Length; i++)		// Don't start with the first word?
			{
				tempAsc = (int) wordArray[i].Substring(0, 1).ToCharArray()[0];

				// if (tempAsc >= (int) 'A' && tempAsc <= (int) 'Z' && wordArray[i].IndexOf(",") < 0)	// Make sure the word doesn't contain a comma,  this would indicate a list of cities or something
				if (tempAsc >= (int) 'A' && tempAsc <= (int) 'Z')
				{
					// Make sure the word doesn't contain a comma,  this would indicate a list of cities or something
					if (wordArray[i].IndexOf(",") < 0)
					{
						// There is no comma in this word -- what to do?

						if (currUpperIndexStart == -1)
							currUpperIndexStart = i;				// This is the start of a potential phrase
						else if (i == wordArray.Length - 1)
						{
							// If this is the last word in the question
							wordArray[currUpperIndexStart] = "\"" + wordArray[currUpperIndexStart];
							wordArray[i] = wordArray[i] + "\"";
						}
					}
					else if (currUpperIndexStart > -1)
					{
						// Terminate the word phrase in quotes --
						wordArray[currUpperIndexStart] = "\"" + wordArray[currUpperIndexStart];
						wordArray[i] = wordArray[i] + "\"";
						currUpperIndexStart = -1;				// Zero out the currUpperIndexStart
					}

				}
				else
				{
					// This is a lower case word, so close out the 
					if (currUpperIndexStart != -1 && i > currUpperIndexStart + 1)
					{
						wordArray[currUpperIndexStart] = "\"" + wordArray[currUpperIndexStart];
						wordArray[i - 1] = wordArray[i - 1] + "\"";
					}
					currUpperIndexStart = -1;
				}
			}

			// --------------------------------------------------------------------------------
			// Rebuild the keyword list

			rawQuestion = "";
			for (int i = 0; i < wordArray.Length; i++)
			{
				rawQuestion += wordArray[i] + " ";
			}
			rawQuestion = rawQuestion.Trim();

			return rawQuestion;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		public static string removeHyphenatedWords(string keywords)
		{
			return removeHyphenatedWords(keywords, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		public static string removeHyphenatedWords(string keywords, bool preserveCapitals)
		{
			// TODO: DO NOT REMOVE THE WORD IF BOTH WORDS ARE CAPITALIZED
			// e.g. Tatanaka-Iyotanka

			keywords = removeDoubleSpaces(keywords);
			string[] words = keywords.Split(' ');
			bool quotesOn = false;
			string retVal = "";
			int hyphenIndex;
			bool wordIsRemovable;

			for (int i = 0; i < words.Length; i++)
			{
				wordIsRemovable = false;
				if (words[i].IndexOf('\"') > -1)
				{
					quotesOn = !quotesOn;
				}

				hyphenIndex = words[i].IndexOf('-');

				if (preserveCapitals && hyphenIndex != -1 && words[i].Length > hyphenIndex)
				{
					char characterAfterHyphen = words[i].ToCharArray()[hyphenIndex + 1];

					// If the second word is capitalized, then DON'T remove it...(you really don't need to check the first)
					// e.g. "Band-Aid"

					if ((int) characterAfterHyphen >= (int)'a' && (int) characterAfterHyphen <= 'z')
						wordIsRemovable = true;
				}

				if (quotesOn || !wordIsRemovable)
				{
					// Only remove the word if the hyphenated words are not capitalized
					if (retVal != "")
						retVal += " ";

					retVal += words[i];
				}
			}
			return retVal;
		}

		/// <summary>
		/// Out of the total search results, what is this answer's percentage?
		/// </summary>
		/// <param name="hitCount"></param>
		/// <param name="rank">0, 1, 2 or 3</param>
		/// <param name="isReverseLookup"></param>
		/// <returns></returns>
		public static double getHitPercentage(int[] hitCount, int rank, bool isReverseLookup)
		{
			int indexHits = -1;
			int answerIndex = -1;
			int totalHits = 0;
			double retVal = 0.0;

			answerIndex = getRankedAnswerIndex(hitCount, 3-rank, isReverseLookup);
			indexHits = hitCount[answerIndex];
			totalHits = getTotalHitCount(hitCount);

			if (!isReverseLookup)
			{
				//answerIndex = getHighestAnswerIndex(hitCount, rank, isReverseLookup);
				//indexHits = hitCount[answerIndex];
				//totalHits = getTotalHitCount(hitCount);
	
				if (totalHits > 0)
					retVal = indexHits / totalHits;
			}
			else
			{
				//answerIndex = getHighestAnswerIndex(hitCount, 3-rank, isReverseLookup);
				//indexHits = hitCount[answerIndex];
				//totalHits = getTotalHitCount(hitCount);

				if (totalHits > 0)
					retVal = 1.0 - (indexHits / totalHits);
			}

			return retVal;
		}

/*
		/// <summary>
		/// How confident are we that this is accurate?
		/// </summary>
		/// <param name="hitCount"></param>
		/// <param name="isReverseLookup"></param>
		/// <returns></returns>
		public static double getHitPercentage(int[] hitCount, bool isReverseLookup)
		{
			int maxHits = -1;
			int answerIndex = -1;
			int totalHits = 0;
			double retVal = 0.0;

			if (!isReverseLookup)
			{
				maxHits = hitCount[answerIndex];
				totalHits = getTotalHitCount(hitCount);
	
				if (totalHits > 0)
					retVal = maxHits / totalHits;
			}
			else
			{
				answerIndex = getLowestAnswerIndex(hitCount);
				maxHits = hitCount[answerIndex];
				totalHits = getTotalHitCount(hitCount);

				if (totalHits > 0)
					retVal = 1.0 - (maxHits / totalHits);
			}

			return retVal;
		}
*/

		/// <summary>
		/// Gets the best answer index for the question in question.
		/// </summary>
		/// <param name="questionText"></param>
		/// <param name="hitCount"></param>
		/// <returns></returns>
		public static int getBestAnswerIndex(string questionText, int[] hitCount)
		{
			bool isReverseLookup = isReverseSearch(questionText);
			//return getBestAnswerIndex(hitCount, isReverseLookup);
			if (isReverseLookup)
				return getRankedAnswerIndex(hitCount, 3, isReverseLookup);
			else
				return getRankedAnswerIndex(hitCount, 0, isReverseLookup);
		}

		/// <summary>
		/// Attempts to remove wonkiness from each answer.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string fixUnicodeChars(string text)
		{
			string retVal = text;
			retVal = retVal.Replace("kï¿½w", "kow");			// i.e. Krakow, Poland
			retVal = retVal.Replace("ï¿½", "e");				// i.e. Beyonce, Cafe
			return retVal;
		}

		/// <summary>
		/// This is just the hit percent difference between 1st and 2nd place.  If there is NO difference, then
		/// results 1 and 2 returned the same number of hits.  If the difference is less than CONFIDENCE_THRESHOLD (7%),
		/// then we should probably get a "second opinion" from another search service.
		/// </summary>
		/// <param name="hitCount"></param>
		/// <param name="isReverseLookup"></param>
		/// <returns></returns>
		public static double getConfidenceIndex(int[] hitCount, bool isReverseLookup)
		{
			double firstPercentage = 0.0;
			double secondPercentage = 0.0;
			double percentageDiff = 0.0;

			int firstHitCount;
			int secondHitCount;
			int totalHits;

			string firstAnswerCode;
			string secondAnswerCode;

			if (!isReverseLookup)
			{
				firstHitCount = hitCount[getRankedAnswerIndex(hitCount, 0, isReverseLookup)];
				firstAnswerCode = getAnswerCode(getRankedAnswerIndex(hitCount, 0, isReverseLookup));
				secondHitCount = hitCount[getRankedAnswerIndex(hitCount, 1, isReverseLookup)];
				secondAnswerCode = getAnswerCode(getRankedAnswerIndex(hitCount, 1, isReverseLookup));
			}
			else
			{
				firstHitCount = hitCount[getRankedAnswerIndex(hitCount, 3, isReverseLookup)];
				firstAnswerCode = getAnswerCode(getRankedAnswerIndex(hitCount, 3, isReverseLookup));
				secondHitCount = hitCount[getRankedAnswerIndex(hitCount, 2, isReverseLookup)];
				secondAnswerCode = getAnswerCode(getRankedAnswerIndex(hitCount, 2, isReverseLookup));
			}
			totalHits = getTotalHitCount(hitCount);

			if (totalHits > 0)
			{
				firstPercentage = (double) firstHitCount / totalHits;
				secondPercentage = (double) secondHitCount / totalHits;
				percentageDiff = firstPercentage - secondPercentage;

				if (isReverseLookup)
				{
					percentageDiff *= -1.0;				// Flip this for reverse lookup questions
				}

				Console.WriteLine("...Confidence Test: 1st: [" + firstAnswerCode + "] " + firstHitCount.ToString() + ": " + firstPercentage.ToString());
				Console.WriteLine("...Confidence Test: 2nd: [" + secondAnswerCode + "] " + secondHitCount.ToString() + ": " + secondPercentage.ToString());
				Console.WriteLine("...Confidence Test: Diff: " + percentageDiff.ToString());

				/*
				if (totalHits > 3)
					percentageDiff += 1;		// Give more points to searches with hits > 3 -- DONT DO THIS BECAUSE THE THRESHOLD IS CONFIDENCE_THRESHOLD, NOT 1 + CONFIDENCE_THRESHOLD
				*/

				return percentageDiff;
			}
			else
			{
				return 0.0;
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hitCount"></param>
		/// <param name="isReverseLookup"></param>
		/// <returns></returns>
		public static int getBestAnswerIndex(int[] hitCount, bool isReverseLookup)
		{
			if (isReverseLookup)
			{
				Console.WriteLine("Getting Lowest Answer Index...");
				return getRankedAnswerIndex(hitCount, 4, isReverseLookup);
			}
			else
			{
				return getRankedAnswerIndex(hitCount, 0, isReverseLookup);
			}
		}
/*
		public static int getHighestAnswerIndex(int[] hitCountList, int rank)
		{
			return getHighestAnswerIndex(hitCountList, rank, false);
		}
*/
		/// <summary>
		/// Ugh, I thought it would be easy to just pop the results into a sorted array...The problem is that
		/// * The hit counts (the index of the sortedlist) must be unique
		/// * If this is a 50/50, we still need to add the non selectable items, because we usually need to select the 3rd and 4th choices
		/// * For 50/50 PLUS reverse lookups, we have to flip the hitCount values to HITCOUNT_INVALIDCHOICE_REVERSE so that they become undesirable
		/// </summary>
		/// <param name="hitCountList"></param>
		/// <param name="rank"></param>
		/// <returns></returns>
		public static int getRankedAnswerIndex(int[] hitCountList, int rank, bool isReverseLookup)
		{
			SortedList hitList = new SortedList();
			int temp = 0;
			double tempIndex = 0.0;

			for (int i = 0; i < hitCountList.Length; i++)
			{
				if (hitCountList[i] < 0 && isReverseLookup)
				{
					tempIndex = (double) HITCOUNT_INVALIDCHOICE_REVERSE - ((double) i / 10);					// This is a reverse lookup AND a non pickable answer
				}
/*
				else if (hitCountList[i] == HITCOUNT_INVALIDCHOICE && isReverseLookup)
				{
					tempIndex = (double) HITCOUNT_INVALIDCHOICE_REVERSE - ((double) i / 10);					// This is a reverse lookup AND a non pickable answer
				}
				else if (hitCountList[i] == HITCOUNT_INVALIDCHOICE_REVERSE && isReverseLookup)
				{
					tempIndex = (double) HITCOUNT_INVALIDCHOICE_REVERSE - ((double) i / 10);					// This is a reverse lookup AND a non pickable answer
				}
*/
				else
				{
					tempIndex = hitCountList[i] + 1.0 - ((double) i / 10);		// This will guarantee uniqueness
				}
				hitList.Add(tempIndex, i);
			}

			foreach (double key in hitList.Keys)
			{
				if (temp == 3 - rank)
					return int.Parse(hitList[key].ToString());

				temp++;
			}

			return 0;		// We should not ever fall here, but just in case
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hitCountList"></param>
		/// <returns></returns>
		public static int getLowestAnswerIndex(int[] hitCountList)
		{
			return getRankedAnswerIndex(hitCountList, 3, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hitCountList"></param>
		/// <returns></returns>
		public static int getHighestAnswerIndex(int[] hitCountList)
		{
			return getRankedAnswerIndex(hitCountList, 0, true);
		}

		/// <summary>
		/// Returns the total hits for the hit list
		/// </summary>
		/// <param name="hitCount"></param>
		/// <returns></returns>
		public static int getTotalHitCount(int[] hitCount)
		{
			int totalHits = 0;

			for (int i = 0; i < hitCount.Length; i++)
			{
				if (hitCount[i] != HITCOUNT_INVALIDCHOICE)
					totalHits += hitCount[i];
			}

			return totalHits;
		}
/*
		/// <summary>
		/// Returns the total hits for the hit list
		/// </summary>
		/// <param name="answerList"></param>
		/// <returns></returns>
		public static int getTotalHitCount(Answer[] answerList)
		{
			int totalHits = 0;

			for (int i = 0; i < answerList.Length; i++)
			{
				totalHits += answerList[i].HitCount;
			}

			return totalHits;
		}
*/
		#region Simple Lookup Functions

		/// <summary>
		/// Returns the letter (a-d) that represents the answer index (0-3)
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string getAnswerCode(int index)
		{
			switch(index)
			{
				case 0:
					return "a";
				case 1:
					return "b";
				case 2:
					return "c";
				case 3:
					return "d";
				default:
					throw new ArgumentException("You have specified an invalid index [" + index.ToString() + "].", index.ToString());
			}
		}

		/// <summary>
		/// Returns true if the question is of the format:
		/// "Which of the following is NOT..." or
		/// "Which of these [items] are NOT..." or
		/// "Which of these [items] is NOT..." or
		/// </summary>
		/// <param name="questionText"></param>
		/// <returns></returns>
		public static bool isReverseSearch(string questionText)
		{
			bool retVal = false;

			if (questionText.ToUpper().IndexOf("WHICH OF THE FOLLOWING IS NOT") >= 0)
			{
				retVal = true;
			}
			else if
				(
					// TODO: Use regex, you moron
					(
					questionText.ToUpper().IndexOf("WHICH OF THE FOLLOWING ") >= 0
					|| questionText.ToUpper().IndexOf("WHICH OF THESE ") >= 0
					)
				&&
					(
						questionText.ToUpper().IndexOf(" IS NOT ") >= 0 
						|| questionText.ToUpper().IndexOf(" ARE NOT ") >= 0 
						|| questionText.ToUpper().IndexOf(" DOES NOT ") >= 0 
						|| questionText.ToUpper().IndexOf(" WAS NOT ") >= 0 
						|| questionText.ToUpper().IndexOf(" WERE NOT ") >= 0 
						|| questionText.ToUpper().IndexOf(" ALL BUT ") >= 0
					)
				)
			{
				retVal = true;
			}

			return retVal;
		}

		#endregion
	}

/*
	public class Answer
	{
		private int index;
		private int hitCount;
		private string text;

		public int Index
		{
			get {return index;}
			set {index = value;}
		}

		public int HitCount
		{
			get {return hitCount;}
			set {hitCount = value;}
		}

		public string Text
		{
			get {return text;}
			set {text = value;}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public Answer()
		{
			index = -1;
			hitCount = 0;
			text = "";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ix"></param>
		/// <param name="hc"></param>
		/// <param name="tx"></param>
		public Answer(int ix, int hc, string tx)
		{
			index = ix;
			hitCount = hc;
			text = tx;
		}

		public int Compare(Answer x, Answer y) 
		{ 
			if (x.hitCount > y.hitCount)
				return 1;
			else if (x.hitCount < y.hitCount)
				return -1;
			else //if (x.hitCount == y.hitCount)
				return 0;
		} 
	}
*/

}
