using System;
using System.Data;
using System.IO;
using System.Configuration;
using AIMLib;

namespace AIMBotTests
{
	/// <summary>
	/// Summary description for General.
	/// </summary>
	public class TestGeneral
	{
		public static double ACCURACY_THRESHOLD = 0.65;

		public TestGeneral()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static bool filterTest(string filter)
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("");
			Console.Out.Flush();

			double totalQuestions = 0;
			double totalCorrect = 0;
			double correctPercentage = 1.00;

			string tempFullText;
			string tempRealAnswer;

			DataSet ds = TestGeneral.getTestQuestionDataSet();

			ds.Tables[0].DefaultView.RowFilter = filter;
			if (ds.Tables[0].DefaultView.Count > 0)
			{
				// If there is a question found...Anything with a question mark
				for (int i = 0; i < ds.Tables["question"].DefaultView.Count; i++)
				{
					tempFullText = ds.Tables["question"].DefaultView[i]["text"].ToString();
					tempRealAnswer = ds.Tables["question"].DefaultView[i]["answer"].ToString().ToLower();

					if (checkAccuracy(tempFullText, tempRealAnswer))
					{
						++totalCorrect;
					}
					++totalQuestions;
					Console.WriteLine("");
					Console.WriteLine("Current Accuracy: " + totalCorrect.ToString() + "/" + totalQuestions.ToString());
					Console.Out.Flush();

				}

			}
			else
			{
				throw new Exception("No records to test.");
			}

			if (totalQuestions > 0)
				correctPercentage = totalCorrect / totalQuestions;

			Console.WriteLine("==================================================");
			Console.WriteLine("RUN COMPLETE");
			Console.WriteLine("");
			Console.WriteLine("Total Correct / Total Questions: " + totalCorrect.ToString() + "/" + totalQuestions.ToString());
			Console.WriteLine("Accuracy: " + correctPercentage.ToString());
			Console.WriteLine("==================================================");
			Console.Out.Flush();

			// I want to do better than 65%
			return correctPercentage > TestGeneral.ACCURACY_THRESHOLD;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fullText"></param>
		/// <param name="realAnswer"></param>
		/// <returns></returns>
		public static bool checkAccuracy(string fullText, string realAnswer)
		{
			if (System.Configuration.ConfigurationManager.AppSettings["DefaultSearchService"].ToLower() == "yahoo")
				return checkAccuracy(fullText, realAnswer, Question.SearchService.Yahoo);
			else
				return checkAccuracy(fullText, realAnswer, Question.SearchService.Google);
		}

		/// <summary>
		/// Gets the accuracy based on the specified search service
		/// </summary>
		/// <param name="fullText"></param>
		/// <param name="realAnswer"></param>
		/// <param name="searchService"></param>
		/// <returns></returns>
		public static bool checkAccuracy(string fullText, string realAnswer, Question.SearchService searchService)
		{
			Console.WriteLine("==================================================");
			//Console.WriteLine("");

			//string tempRealAnswer = "b";
			string tempQuestion = Question.getQuestion(fullText);
			bool isReverseLookup = Question.isReverseSearch(tempQuestion);
			string tempKeywords = Question.getKeywords(tempQuestion);
			string[] answerList = Question.getAnswers(fullText);
			int[] hitCountList;

			if (searchService == Question.SearchService.Yahoo)
				hitCountList = Question.getYahooHitCounts(tempKeywords, answerList, isReverseLookup, true);
			else
				hitCountList = Question.getGoogleHitCounts(tempKeywords, answerList, isReverseLookup, true);

			int bestAnswerIndex = Question.getBestAnswerIndex(tempQuestion, hitCountList);

			//Console.WriteLine("Question: " + totalQuestions.ToString());
			Console.WriteLine("");
			Console.WriteLine(tempQuestion);
			Console.WriteLine("");
			for (int j = 0; j < 4; j++)
			{
				Console.WriteLine(Question.getAnswerCode(j) + ": " + answerList[j] + " [" + hitCountList[j] + " hits]");
			}
			Console.WriteLine("");
			if (isReverseLookup)
				Console.WriteLine("Question Type: Reverse Search");
			else
				Console.WriteLine("Question Type: Get Highest");

			Console.WriteLine("Keywords: " + tempKeywords);
			Console.WriteLine("SELECTED: " + Question.getAnswerCode(bestAnswerIndex) + " vs. " + realAnswer);
			if (Question.getAnswerCode(bestAnswerIndex) != realAnswer)
			{
				Console.WriteLine("-----> INCORRECT <-----");
			}

			//Console.WriteLine("Confidence: " + Question.getConfidenceIndex(hitCountList, isReverseLookup).ToString());
			Console.Out.Flush();
			return Question.getAnswerCode(bestAnswerIndex) == realAnswer;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static DataSet getTestQuestionDataSet()
		{
			string testXMLFile = ConfigurationSettings.AppSettings["TestQuestionPath"];
			DataSet ds = new DataSet();

			if (File.Exists(testXMLFile))
			{
				try
				{
					ds.ReadXml(testXMLFile);
				}
				catch (Exception ex)
				{
					Console.WriteLine("ERROR: Could not load tempXML file. [" + ex.Message + "]", ex);
					throw new FileLoadException("ERROR: Could not load tempXML file. [" + ex.Message + "]", testXMLFile);
				}

			}
			else
			{
				throw new System.IO.FileNotFoundException("Could not find the test XML file");
			}

			return ds;
		}
	}
}
