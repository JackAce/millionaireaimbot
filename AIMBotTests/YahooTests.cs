using System;
using System.Data;
using NUnit.Framework;
using AIMLib;

namespace AIMBotTests
{
	/// <summary>
	/// Summary description for YahooTests.
	/// </summary>
	[TestFixture]
	public class YahooTests
	{
		public YahooTests()
		{

		}

		/// <summary>
		/// This just hits the yahoo service and tries to get a hit count.  This should be the simplest test to run.
		/// </summary>
		[Test]
		public void Yahoo_00_HitCount_Test()
		{
			string keywordList;
			int hitCount = 0;

			keywordList = "fat lipase digestive enzyme body break down food component";
			hitCount = AIMLib.YahooHelper.getHitCount(keywordList);

			Console.WriteLine("Yahoo HitCount for [" + keywordList + "]: " + hitCount.ToString());

			keywordList = "\"dr pepper\" \"secret syrup\" one jelly belly jelly beans flavored drink";
			hitCount = AIMLib.YahooHelper.getHitCount(keywordList);

			Console.WriteLine("Yahoo HitCount for [" + keywordList + "]: " + hitCount.ToString());

			Assert.IsTrue(hitCount > 0);

		}

		/// <summary>
		/// Just queries a bunch of different years in a row.  This is to test timeouts and other possible issues.
		/// </summary>
		[Test, Category("Long Test")]
		public void Yahoo_01_BruteForce_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_01_BruteForce_Test");
			Console.WriteLine("");

			for (int i = 1950; i < 1991; i++)
			{
				int testHits = YahooHelper.getHitCount("\"Year " + i.ToString() + "\"");
				Console.WriteLine("Year " + i.ToString() + ": " + testHits.ToString() + " hits");
			}
			Assert.IsTrue(true);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_10_OneShot_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_01_OneShot_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_20_ReverseSearch_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_02_ReverseSearch_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_REVERSESEARCH'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_20_5050_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_20_5050_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_5050'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_30_ContainsLink_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_30_ContainsLink_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_CONTAINSLINK'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_30_HasHalfDashedWord_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_30_HasHalfDashedWord_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_HASHALFDASH'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_30_InTheSong_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_30_InTheSong_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_INTHESONG'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_30_ConsecutiveCapitals_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_30_ConsecutiveCapitals_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_CONSECUTIVECAPITALS'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Yahoo_30_ReverseSearch5050_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_30_ReverseSearch5050_Test");
			Console.WriteLine("");

			Assert.IsTrue(filterTest("desc = 'ONESHOT_REVERSESEARCH5050'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test, Category("Long Test")]
		public void Yahoo_99_Accuracy_Test()
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("Yahoo_99_Accuracy_Test");
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
			return TestGeneral.checkAccuracy(fullText, realAnswer, Question.SearchService.Yahoo);
			/*
			Console.WriteLine("==================================================");

			string tempQuestion = Question.getQuestion(fullText);
			bool isReverseLookup = Question.isReverseSearch(tempQuestion);
			string tempKeywords = Question.getKeywords(tempQuestion);
			string[] answerList = Question.getAnswers(fullText);
			int[] hitCountList = Question.getYahooHitCounts(tempKeywords, answerList, isReverseLookup, true);

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
			return Question.getAnswerCode(bestAnswerIndex) == realAnswer;
			*/
		}

	}
}
