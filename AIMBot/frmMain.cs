using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using AIMLib;
using log4net;

namespace AIMBot
{
	/// <summary>
	/// Summary description for frmMain.
	/// </summary>
	public class frmMain : Form
	{
		#region Member Variables

		private string strIMLogFolderName;
		private string strIMLogFileName;
		//private string strIMMillionaireWindowName;
		//private string strIMCCWindowName;

		//private string strAutoItFile;
		private string strTempFolder;
		
		//private string strGoogleAPILicenseKey;

		private int intLastProcessedTimeStamp = -1;
		private int intLastQuestionTimeStamp = -1;

		private Container components = null;
		private FileSystemWatcher objFileSystemWatcher;

		private static bool lookupInProgress = false;
		private Label lblCurrentStatus;
		private TextBox txtCurrentStatus;
		private Label label1;
		private TextBox txtDebug;
		private TextBox txtLastRun;
		private TextBox txtLastTimeId;
		private Label label2;
		private TextBox txtLastQuestion;
		private Label label3;
		private TextBox txtLastAnswerA;
		private Label label4;
		private TextBox txtLastAnswerB;
		private Label label5;
		private TextBox txtLastAnswerC;
		private Label label6;
		private TextBox txtLastAnswerD;
		private Label label7;
		private TextBox txtQuestionType;
		private Label label8;
		private TextBox txtLastHitD;
		private TextBox txtLastHitC;
		private TextBox txtLastHitB;
		private TextBox txtLastHitA;
		private TextBox txtLastAnswer;
		private Label label9;
		private TextBox txtKeywords;
		private Label label10;
		private static readonly ILog log = LogManager.GetLogger(typeof(frmMain));

		#endregion

        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text, TextBox txt);
        
        public frmMain()
		{
			InitializeComponent();

			strIMLogFolderName = System.Configuration.ConfigurationManager.AppSettings["IMLogFolder"].ToString();
            strIMLogFileName = ConfigurationManager.AppSettings["IMLogFile"].ToString();

            this.Text = this.Text + " [" + ConfigurationManager.AppSettings["DevStage"].ToString() + "]";

            strTempFolder = ConfigurationManager.AppSettings["TempFolder"].ToString();

			loadLogFile();

			// Once the log file is ALREADY loaded, then implement the file watcher.
			objFileSystemWatcher = new FileSystemWatcher(strIMLogFolderName, strIMLogFileName);
			objFileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
			objFileSystemWatcher.EnableRaisingEvents = true;
		}

        private void SetText(string text, TextBox txt)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txt.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                txt.Text = text;
            }
        }
        
        /// <summary>
		/// Called by file monitor
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void OnChanged(object source, FileSystemEventArgs e) 
		{
			loadLogFile();
		}

		private void clearForm()
		{
			//txtCurrentStatus.Text = "";
			txtDebug.Text = "";
			//txtLastRun.Text = "";
			//txtLastTimeId.Text = "";
			txtLastQuestion.Text = "";
			txtLastAnswerA.Text = "";
			txtLastAnswerB.Text = "";
			txtLastAnswerC.Text = "";
			txtLastAnswerD.Text = "";
			txtQuestionType.Text = "";
			txtLastHitD.Text = "";
			txtLastHitC.Text = "";
			txtLastHitB.Text = "";
			txtLastHitA.Text = "";
			txtLastAnswer.Text = "";

			txtDebug.Invalidate();
			//txtLastRun.Invalidate();
			//txtLastTimeId.Invalidate();
			txtLastQuestion.Invalidate();
			txtLastAnswerA.Invalidate();
			txtLastAnswerB.Invalidate();
			txtLastAnswerC.Invalidate();
			txtLastAnswerD.Invalidate();
			txtQuestionType.Invalidate();
			txtLastHitD.Invalidate();
			txtLastHitC.Invalidate();
			txtLastHitB.Invalidate();
			txtLastHitA.Invalidate();
			txtLastAnswer.Invalidate();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		//private System.IO.File objIMLogFile;
		//private FileStream objFileStream;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtDebug = new System.Windows.Forms.TextBox();
			this.lblCurrentStatus = new System.Windows.Forms.Label();
			this.txtCurrentStatus = new System.Windows.Forms.TextBox();
			this.txtLastRun = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtLastTimeId = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtLastQuestion = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtLastAnswerA = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtLastAnswerB = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtLastAnswerC = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtLastAnswerD = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtQuestionType = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtLastHitD = new System.Windows.Forms.TextBox();
			this.txtLastHitC = new System.Windows.Forms.TextBox();
			this.txtLastHitB = new System.Windows.Forms.TextBox();
			this.txtLastHitA = new System.Windows.Forms.TextBox();
			this.txtLastAnswer = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtKeywords = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtDebug
			// 
			this.txtDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDebug.Location = new System.Drawing.Point(8, 312);
			this.txtDebug.Multiline = true;
			this.txtDebug.Name = "txtDebug";
			this.txtDebug.ReadOnly = true;
			this.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtDebug.Size = new System.Drawing.Size(451, 216);
			this.txtDebug.TabIndex = 0;
			this.txtDebug.Text = "";
			// 
			// lblCurrentStatus
			// 
			this.lblCurrentStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblCurrentStatus.Location = new System.Drawing.Point(8, 8);
			this.lblCurrentStatus.Name = "lblCurrentStatus";
			this.lblCurrentStatus.TabIndex = 2;
			this.lblCurrentStatus.Text = "Current Status:";
			// 
			// txtCurrentStatus
			// 
			this.txtCurrentStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtCurrentStatus.Location = new System.Drawing.Point(120, 8);
			this.txtCurrentStatus.Name = "txtCurrentStatus";
			this.txtCurrentStatus.ReadOnly = true;
			this.txtCurrentStatus.Size = new System.Drawing.Size(339, 20);
			this.txtCurrentStatus.TabIndex = 3;
			this.txtCurrentStatus.Text = "Idle";
			// 
			// txtLastRun
			// 
			this.txtLastRun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastRun.Location = new System.Drawing.Point(120, 32);
			this.txtLastRun.Name = "txtLastRun";
			this.txtLastRun.ReadOnly = true;
			this.txtLastRun.Size = new System.Drawing.Size(339, 20);
			this.txtLastRun.TabIndex = 5;
			this.txtLastRun.Text = "(None)";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 32);
			this.label1.Name = "label1";
			this.label1.TabIndex = 4;
			this.label1.Text = "Last Run Time:";
			// 
			// txtLastTimeId
			// 
			this.txtLastTimeId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastTimeId.Location = new System.Drawing.Point(120, 56);
			this.txtLastTimeId.Name = "txtLastTimeId";
			this.txtLastTimeId.ReadOnly = true;
			this.txtLastTimeId.Size = new System.Drawing.Size(339, 20);
			this.txtLastTimeId.TabIndex = 7;
			this.txtLastTimeId.Text = "";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.TabIndex = 6;
			this.label2.Text = "Last Time ID:";
			// 
			// txtLastQuestion
			// 
			this.txtLastQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastQuestion.Location = new System.Drawing.Point(120, 80);
			this.txtLastQuestion.Multiline = true;
			this.txtLastQuestion.Name = "txtLastQuestion";
			this.txtLastQuestion.ReadOnly = true;
			this.txtLastQuestion.Size = new System.Drawing.Size(339, 48);
			this.txtLastQuestion.TabIndex = 9;
			this.txtLastQuestion.Text = "";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(8, 80);
			this.label3.Name = "label3";
			this.label3.TabIndex = 8;
			this.label3.Text = "Last Question:";
			// 
			// txtLastAnswerA
			// 
			this.txtLastAnswerA.Location = new System.Drawing.Point(120, 160);
			this.txtLastAnswerA.Name = "txtLastAnswerA";
			this.txtLastAnswerA.ReadOnly = true;
			this.txtLastAnswerA.Size = new System.Drawing.Size(232, 20);
			this.txtLastAnswerA.TabIndex = 11;
			this.txtLastAnswerA.Text = "";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.Location = new System.Drawing.Point(80, 160);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 23);
			this.label4.TabIndex = 10;
			this.label4.Text = "A:";
			// 
			// txtLastAnswerB
			// 
			this.txtLastAnswerB.Location = new System.Drawing.Point(120, 184);
			this.txtLastAnswerB.Name = "txtLastAnswerB";
			this.txtLastAnswerB.ReadOnly = true;
			this.txtLastAnswerB.Size = new System.Drawing.Size(232, 20);
			this.txtLastAnswerB.TabIndex = 13;
			this.txtLastAnswerB.Text = "";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.Location = new System.Drawing.Point(80, 184);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(24, 23);
			this.label5.TabIndex = 12;
			this.label5.Text = "B:";
			// 
			// txtLastAnswerC
			// 
			this.txtLastAnswerC.Location = new System.Drawing.Point(120, 208);
			this.txtLastAnswerC.Name = "txtLastAnswerC";
			this.txtLastAnswerC.ReadOnly = true;
			this.txtLastAnswerC.Size = new System.Drawing.Size(232, 20);
			this.txtLastAnswerC.TabIndex = 15;
			this.txtLastAnswerC.Text = "";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(80, 208);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(24, 23);
			this.label6.TabIndex = 14;
			this.label6.Text = "C:";
			// 
			// txtLastAnswerD
			// 
			this.txtLastAnswerD.Location = new System.Drawing.Point(120, 232);
			this.txtLastAnswerD.Name = "txtLastAnswerD";
			this.txtLastAnswerD.ReadOnly = true;
			this.txtLastAnswerD.Size = new System.Drawing.Size(232, 20);
			this.txtLastAnswerD.TabIndex = 17;
			this.txtLastAnswerD.Text = "";
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label7.Location = new System.Drawing.Point(80, 232);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(24, 23);
			this.label7.TabIndex = 16;
			this.label7.Text = "D:";
			// 
			// txtQuestionType
			// 
			this.txtQuestionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQuestionType.Location = new System.Drawing.Point(120, 256);
			this.txtQuestionType.Name = "txtQuestionType";
			this.txtQuestionType.ReadOnly = true;
			this.txtQuestionType.Size = new System.Drawing.Size(339, 20);
			this.txtQuestionType.TabIndex = 19;
			this.txtQuestionType.Text = "";
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(8, 256);
			this.label8.Name = "label8";
			this.label8.TabIndex = 18;
			this.label8.Text = "Question Type:";
			// 
			// txtLastHitD
			// 
			this.txtLastHitD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastHitD.Location = new System.Drawing.Point(360, 232);
			this.txtLastHitD.Name = "txtLastHitD";
			this.txtLastHitD.ReadOnly = true;
			this.txtLastHitD.Size = new System.Drawing.Size(99, 20);
			this.txtLastHitD.TabIndex = 23;
			this.txtLastHitD.Text = "";
			// 
			// txtLastHitC
			// 
			this.txtLastHitC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastHitC.Location = new System.Drawing.Point(360, 208);
			this.txtLastHitC.Name = "txtLastHitC";
			this.txtLastHitC.ReadOnly = true;
			this.txtLastHitC.Size = new System.Drawing.Size(99, 20);
			this.txtLastHitC.TabIndex = 22;
			this.txtLastHitC.Text = "";
			// 
			// txtLastHitB
			// 
			this.txtLastHitB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastHitB.Location = new System.Drawing.Point(360, 184);
			this.txtLastHitB.Name = "txtLastHitB";
			this.txtLastHitB.ReadOnly = true;
			this.txtLastHitB.Size = new System.Drawing.Size(99, 20);
			this.txtLastHitB.TabIndex = 21;
			this.txtLastHitB.Text = "";
			// 
			// txtLastHitA
			// 
			this.txtLastHitA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastHitA.Location = new System.Drawing.Point(360, 160);
			this.txtLastHitA.Name = "txtLastHitA";
			this.txtLastHitA.ReadOnly = true;
			this.txtLastHitA.Size = new System.Drawing.Size(99, 20);
			this.txtLastHitA.TabIndex = 20;
			this.txtLastHitA.Text = "";
			// 
			// txtLastAnswer
			// 
			this.txtLastAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastAnswer.Location = new System.Drawing.Point(120, 280);
			this.txtLastAnswer.Name = "txtLastAnswer";
			this.txtLastAnswer.ReadOnly = true;
			this.txtLastAnswer.Size = new System.Drawing.Size(339, 20);
			this.txtLastAnswer.TabIndex = 25;
			this.txtLastAnswer.Text = "";
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label9.Location = new System.Drawing.Point(8, 280);
			this.label9.Name = "label9";
			this.label9.TabIndex = 24;
			this.label9.Text = "Last Answer:";
			// 
			// txtKeywords
			// 
			this.txtKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtKeywords.Location = new System.Drawing.Point(120, 136);
			this.txtKeywords.Name = "txtKeywords";
			this.txtKeywords.ReadOnly = true;
			this.txtKeywords.Size = new System.Drawing.Size(339, 20);
			this.txtKeywords.TabIndex = 27;
			this.txtKeywords.Text = "";
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label10.Location = new System.Drawing.Point(8, 136);
			this.label10.Name = "label10";
			this.label10.TabIndex = 26;
			this.label10.Text = "Google Keywords:";
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(467, 534);
			this.Controls.Add(this.txtKeywords);
			this.Controls.Add(this.txtLastAnswer);
			this.Controls.Add(this.txtLastHitD);
			this.Controls.Add(this.txtLastHitC);
			this.Controls.Add(this.txtLastHitB);
			this.Controls.Add(this.txtLastHitA);
			this.Controls.Add(this.txtQuestionType);
			this.Controls.Add(this.txtLastAnswerD);
			this.Controls.Add(this.txtLastAnswerC);
			this.Controls.Add(this.txtLastAnswerB);
			this.Controls.Add(this.txtLastAnswerA);
			this.Controls.Add(this.txtLastQuestion);
			this.Controls.Add(this.txtLastTimeId);
			this.Controls.Add(this.txtLastRun);
			this.Controls.Add(this.txtCurrentStatus);
			this.Controls.Add(this.txtDebug);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblCurrentStatus);
			this.MinimumSize = new System.Drawing.Size(475, 512);
			this.Name = "frmMain";
			this.Text = "Millionaire AIM Bot";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			
			Application.Run(new frmMain());
		}

		private void loadLogFile()
		{
			if (!lookupInProgress)
			{
				lookupInProgress = true;

				DataSet ds = new DataSet();
				//FileStream fs;
				if (File.Exists(strIMLogFolderName + strIMLogFileName))
				{
					string filter;
					string tempXMLFile = strTempFolder + "log" + DateTime.Now.Ticks.ToString() + ".xml";
					FileStream xmlFS = new FileStream(strIMLogFolderName + strIMLogFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					StreamWriter sw;
					StreamReader sr;
					FileStream fs = new FileStream(tempXMLFile, FileMode.CreateNew, FileAccess.Write, FileShare.None);

					// --------------------------------------------------------------------------------
					// Read the Trillian Pseudo-XML Log file and write it to a valid XML file.

					sr = new StreamReader(xmlFS);
					sw = new StreamWriter(fs);
					sw.WriteLine("<xmllogfile>");		// The xml document needs a root element
					sw.Write(sr.ReadToEnd());
					sw.WriteLine("</xmllogfile>");
					sw.Close();
					sw = null;
					sr.Close();
					sr = null;

					// --------------------------------------------------------------------------------
					// Once the file is ready, load it into a dataset and make an attempt to clean up.

					ds.ReadXml(tempXMLFile);

					try
					{
						File.Delete(tempXMLFile);
					}
					catch (Exception ex)
					{
						// Just log an continue
						log.Error("Could not delete tempXML file.", ex);
					}

					// --------------------------------------------------------------------------------
					// We got a new message in Trillian...was it a new question?  Get the most recent question
					// and check out the timestamp.  If it has a new timestamp, then its a new question!
					//
					// The new record should be an "incoming_privateMessage" and should contain the words
					// contestant, hot, seat, needs, and help...I would make the filter more exact, but 
					// Trillian encodes the text in a format that ADO.net doesn't like.  Plus the ADO RowFilter
					// property is really limited.  The following filter works every time, so let's use it!

					StringBuilder sb = new StringBuilder();

					filter = "type = 'incoming_privateMessage' ";
					filter += "AND text LIKE '%contestant%' ";
					filter += "AND text LIKE '%hot%' ";
					filter += "AND text LIKE '%seat%' ";
					filter += "AND text LIKE '%needs%' ";
					filter += "AND text LIKE '%help%' ";
					filter += "AND time >= '" + intLastProcessedTimeStamp.ToString() + "' ";		// Make sure we haven't already answered this question!

					ds.Tables["message"].DefaultView.RowFilter = filter;
					ds.Tables["message"].DefaultView.Sort = "time desc";

					// If there is a question found...
					if (ds.Tables["message"].DefaultView.Count > 0)
					{
						intLastQuestionTimeStamp = int.Parse(ds.Tables["message"].DefaultView[0]["time"].ToString());

						// TODO: This needs to make sense
						//txtLastTimeId.Text = intLastQuestionTimeStamp.ToString();
                        SetText(intLastQuestionTimeStamp.ToString(), txtLastTimeId);
						txtLastTimeId.Invalidate();

						// If we have NOT processed this question before
						if (intLastProcessedTimeStamp != -1 && intLastProcessedTimeStamp != intLastQuestionTimeStamp)
						{
							clearForm();
							txtCurrentStatus.Text = "Processing";

							string rawText;
							string questionText;
							string keywords;
							//string debugText;
							string[] answers = {"", "", "", ""};	
							int[] hitCounts = {-1, -1, -1, -1};
							bool isReverseSearch = false;

							DateTime procStart = DateTime.Now;
							DateTime procComplete;
							DateTime procEnd;
							TimeSpan timeElapsed;
							double msElapsed;

							int bestAnswerIndex = -1;

							rawText = ds.Tables["message"].DefaultView[0]["text"].ToString();		// Pull the URL Encoded data -- This is how Trillian stores it in the XML
							rawText = HttpUtility.UrlDecode(rawText).Trim();						// Decode and Trim it
							questionText = Question.getQuestion(rawText);

							// --------------------------------------------------------------------------------
							// A "Reverse Lookup" or "Reverse Search" is what you use when the question is in the format
							// "Which of the following is NOT..." or "Which of these items is NOT..."
							// We want to select the answer that yeilds the LOWEST hit counts.

							isReverseSearch = Question.isReverseSearch(questionText);

							// --------------------------------------------------------------------------------

							txtLastQuestion.Text = questionText;
							txtLastQuestion.Invalidate();

							keywords = Question.getKeywords(questionText);
							txtKeywords.Text = keywords;
							txtKeywords.Invalidate();
							
							sb.Append(questionText);
							sb.Append("\r\n");

							// --------------------------------------------------------------------------------
							// Parse the answers from the rawText

							answers = Question.getAnswers(rawText);
							for (int i = 0; i < 4; i++)
							{
								TextBox tempTextAnswerBox;

								switch(i)
								{
									case 0:
										tempTextAnswerBox = txtLastAnswerA;
										break;
									case 1:
										tempTextAnswerBox = txtLastAnswerB;
										break;
									case 2:
										tempTextAnswerBox = txtLastAnswerC;
										break;
									case 3:
										tempTextAnswerBox = txtLastAnswerD;
										break;
									default:
										tempTextAnswerBox = txtLastAnswerA;
										break;
								}
								tempTextAnswerBox.Text = answers[i];
								tempTextAnswerBox.Invalidate();
							}

							// --------------------------------------------------------------------------------

							//hitCounts = Question.getYahooHitCounts(keywords, answers, isReverseSearch, true);
							hitCounts = Question.getDefaultHitCounts(keywords, answers, isReverseSearch, true);

							for (int i = 0; i < 4; i++)
							{
								TextBox tempTextHitBox;
								switch(i)
								{
									case 0:
										tempTextHitBox = txtLastHitA;
										break;
									case 1:
										tempTextHitBox = txtLastHitB;
										break;
									case 2:
										tempTextHitBox = txtLastHitC;
										break;
									case 3:
										tempTextHitBox = txtLastHitD;
										break;
									default:
										tempTextHitBox = txtLastHitA;
										break;
								}

								tempTextHitBox.Text = hitCounts[i].ToString();
								tempTextHitBox.Invalidate();

								sb.Append(answers[i]);
								sb.Append(" : ");
								sb.Append(hitCounts[i].ToString());
								sb.Append("\r\n");

							}

							sb.Append("\r\n");
							if (isReverseSearch)
							{
								// A "Reverse Lookup" is what you use when the question is in the format
								// "Which of the following is NOT..." or "Which of these items is NOT..."
								// We want to select the answer that yeilds the LOWEST hit counts.

								bestAnswerIndex = Question.getLowestAnswerIndex(hitCounts);
								sb.Append("Search Results Sought: LOWEST");
								txtQuestionType.Text = "LOWEST";
							}
							else
							{
								bestAnswerIndex = Question.getHighestAnswerIndex(hitCounts);
								sb.Append("Search Results Sought: HIGHEST");
								txtQuestionType.Text = "HIGHEST";
							}
							txtQuestionType.Invalidate();
							sb.Append("\r\n");

							sb.Append("KEYWORDS: ");
							sb.Append(keywords);
							sb.Append("\r\n");

							sb.Append("\r\n");
							sb.Append("BEST ANSWER: " + Question.getAnswerCode(bestAnswerIndex) + ": " + answers[bestAnswerIndex]);
							sb.Append("\r\n");

							txtLastAnswer.Text = Question.getAnswerCode(bestAnswerIndex);
							txtLastAnswer.Invalidate();

							// --------------------------------------------------------------------------------
							// Figure out how long we have taken so far and we may need to wait before sending the IM

							procComplete = DateTime.Now;
							timeElapsed = procComplete.Subtract(procStart);
							int msDelay = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ResponseDelay"].ToString());
							msElapsed = timeElapsed.TotalMilliseconds;

							if (msDelay > msElapsed)
							{
								sb.Append("\r\n");
								sb.Append("Start: " + procStart.ToLongTimeString());
								sb.Append("\r\n");
								sb.Append("Complete: " + procComplete.ToLongTimeString());
								sb.Append("\r\n");
								sb.Append("Time elapsed: " + msElapsed.ToString() + " ms.");
								sb.Append("\r\n");
								sb.Append("Now waiting " + (msDelay - (int) msElapsed).ToString() + " ms.");
								sb.Append("\r\n");

								Thread.Sleep(msDelay - (int) msElapsed);
							}

							// --------------------------------------------------------------------------------

							// NOW SENDING MESSAGE!
							IMTransport.sendIMMessage(Question.getAnswerCode(bestAnswerIndex), sb.ToString());

							procEnd = DateTime.Now;

							txtDebug.Text = sb.ToString();
							txtCurrentStatus.Text = "Idle";

							txtLastRun.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
							txtLastRun.Invalidate();
						}

						// We don't want to process this question any more...
						intLastProcessedTimeStamp = intLastQuestionTimeStamp;
					}
					else
					{
						// If we just started this program up, trick the system to think we have 
						// processed a question already.  The next question that we get should have a 
						// higher timestamp than 1 (the timestamp is just an int value that Trillian uses).
						intLastProcessedTimeStamp = 1;
					}

				}
				else
				{
					log.Error("The IM Log file could not be found. [" + strIMLogFileName + "]");
					throw new FileNotFoundException("The IM Log file could not be found. [" + strIMLogFileName + "]", strIMLogFileName);
				}
				lookupInProgress = false;

			}
		}
	}
}
