using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using AIMLib;
using NUnit.Framework;

namespace AIMBotTests
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class GoogleTests
	{
		public GoogleTests()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_00_Lookup_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_00_Lookup_Test");
			Console.WriteLine("");

			int testGoogleHits = GoogleHelper.getHitCount("gilligan's island", -1);
			Console.WriteLine("Total Hits: " + testGoogleHits.ToString());
			Assert.IsTrue(testGoogleHits > 0);
		}

		/// <summary>
		/// Just queries a bunch of different years in a row.  This is to test timeouts and other possible issues.
		/// </summary>
		[Test]
		public void Google_10_BruteForce_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_10_BruteForce_Test");
			Console.WriteLine("");

			for (int i = 1950; i < 1991; i++)
			{
				int testGoogleHits = GoogleHelper.getHitCount("\"Year " + i.ToString() + "\"", -1);
				Console.WriteLine("Year " + i.ToString() + ": " + testGoogleHits.ToString() + " hits");
			}
			Assert.IsTrue(true);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_10_OneShot_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_10_OneShot_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_20_ReverseSearch_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_20_ReverseSearch_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_REVERSESEARCH'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_20_5050_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_20_5050_Accuracy_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_5050'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_30_ConsecutiveCapitals_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_30_ConsecutiveCapitals_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_CONSECUTIVECAPITALS'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_30_ContainsLink_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_30_ContainsLink_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_CONTAINSLINK'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_30_HasHalfDashedWord_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_30_HasHalfDashedWord_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_HASHALFDASH'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_30_InTheSong_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_30_InTheSong_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_INTHESONG'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_30_ReverseSearch5050_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_30_ReverseSearch5050_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_REVERSESEARCH5050'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Google_99_Accuracy_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Google_99_Accuracy_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc NOT LIKE 'ONESHOT%'"));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		private bool filterTest(string filter)
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("");

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

					if (this.checkAccuracy(tempFullText, tempRealAnswer))
					{
						++totalCorrect;
					}
					++totalQuestions;
					Console.WriteLine("");
					Console.WriteLine("Current Accuracy: " + totalCorrect.ToString() + "/" + totalQuestions.ToString());

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

			// I want to do better than 65%
			return correctPercentage > TestGeneral.ACCURACY_THRESHOLD;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fullText"></param>
		/// <param name="realAnswer"></param>
		/// <returns></returns>
		private bool checkAccuracy(string fullText, string realAnswer)
		{
			return TestGeneral.checkAccuracy(fullText, realAnswer, Question.SearchService.Google);
			/*
			Console.WriteLine("==================================================");

			string tempQuestion = Question.getQuestion(fullText);
			string tempKeywords = Question.getKeywords(tempQuestion);
			string[] answerList = Question.getAnswers(fullText);
			bool isReverseLookup = Question.isReverseSearch(tempQuestion);
			int[] hitCountList = Question.getGoogleHitCounts(tempKeywords, answerList, isReverseLookup, true);

			int bestAnswerIndex = Question.getBestAnswerIndex(tempQuestion, hitCountList);

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
			return Question.getAnswerCode(bestAnswerIndex) == realAnswer;
			*/
		}

	}
}
