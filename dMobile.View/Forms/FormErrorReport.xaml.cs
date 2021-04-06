namespace dMobile.View.Forms
{
    using Android.Util;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using System;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="FormErrorReport" />
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormErrorReport : DMobileModal
    {
        /// <summary>
        /// Defines the exData
        /// </summary>
        private Exception exData = null;

        /// <summary>
        /// Defines the reportingMethod
        /// </summary>
        private string reportingMethod = string.Empty;

        /// <summary>
        /// Defines the fullClassName
        /// </summary>
        private string fullClassName = string.Empty;

        //private UserControl callingControl = null;
        /// <summary>
        /// Defines the sysEnv
        /// </summary>
        private SysEnvironment sysEnv;

        /// <summary>
        /// Defines the errMsg
        /// </summary>
        private string errMsg = string.Empty;

        /// <summary>
        /// Defines the errMsgDetail
        /// </summary>
        private string errMsgDetail = string.Empty;

        public FormErrorReport()
        {
            this.InitializeComponent();
            
            this.buttonContinue.Text = Resource.Continue;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="ex"></param>
        public FormErrorReport(Exception ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.SysEnv = SysEnvironment.GetInstance();
            //this.CallingControl = null;
            this.ExData = ex;
            this.ErrMsg = this.ExData.Message;
            this.textBoxExData.Text = this.ErrMsg;

            //SysSound snd = new SysSound("buzzh.wav");
            //snd.Play();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="ex"></param>
        public FormErrorReport(SQLiteException ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.SysEnv = SysEnvironment.GetInstance();
            //this.CallingControl = null;
            this.ExData = ex;

            this.ErrMsg = ex.Message;

            string msg = string.Empty;

            this.ErrMsgDetail = msg;
            this.textBoxExData.Text = this.ErrMsg;

            //SysSound snd = new SysSound("buzzh.wav");
            //snd.Play();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"></param>
        /// <param name="callingControl"></param>
        /// <param name="ex"></param>
        public FormErrorReport(SysCommon.ActSubTypes activitySubType, /*UserControl callingControl,*/ Exception ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.SysEnv = SysEnvironment.GetInstance();
            //this.CallingControl = callingControl;
            this.ExData = ex;

            this.ErrMsg = this.ExData.Message;
            this.textBoxExData.Text = this.ErrMsg;

            //SysSound snd = new SysSound("buzzh.wav");
            //snd.Play();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"></param>
        /// <param name="callingControl"></param>
        /// <param name="ex"></param>
        public FormErrorReport(SysCommon.ActSubTypes activitySubType, /*UserControl callingControl,*/ SQLiteException ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.SysEnv = SysEnvironment.GetInstance();
            //this.CallingControl = callingControl;
            this.ExData = ex;
            this.ErrMsg = ex.Message;

            string msg = string.Empty;

            this.ErrMsgDetail = msg;

            this.textBoxExData.Text = this.ErrMsg;

            //SysSound snd = new SysSound("buzzh.wav");
            //snd.Play();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"></param>
        /// <param name="callingControl"></param>
        /// <param name="ex"></param>
        /// <param name="fullClassName"></param>
        /// <param name="reportingMethod"></param>
        public FormErrorReport(
        SysCommon.ActSubTypes activitySubType,
        //UserControl callingControl,
        Exception ex,
        string fullClassName,
        string reportingMethod)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.SysEnv = SysEnvironment.GetInstance();
            this.FullClassName = fullClassName;
            this.ReportingMethod = reportingMethod;
            //this.CallingControl = callingControl;
            this.ExData = ex;
            this.ErrMsg = this.ExData.Message;

            this.ErrMsgDetail = "Class: " + this.FullClassName + Environment.NewLine + "Method: " + this.ReportingMethod;

            this.textBoxExData.Text = this.ErrMsg;

            //SysSound snd = new SysSound("buzzh.wav");
            //snd.Play();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"></param>
        /// <param name="callingControl"></param>
        /// <param name="ex"></param>
        /// <param name="fullClassName"></param>
        /// <param name="reportingMethod"></param>
        public FormErrorReport(
        SysCommon.ActSubTypes activitySubType,
        //UserControl callingControl,
        SQLiteException ex,
        string fullClassName,
        string reportingMethod)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.SysEnv = SysEnvironment.GetInstance();
            this.FullClassName = fullClassName;
            this.ReportingMethod = reportingMethod;
            //this.CallingControl = callingControl;
            this.ExData = ex;
            this.ErrMsg = ex.Message;

            string msg = "Class: " + this.FullClassName + Environment.NewLine + "Method: "
                + this.ReportingMethod + Environment.NewLine;

            this.ErrMsgDetail = msg;

            //SysSound snd = new SysSound("buzzh.wav");
            //snd.Play();
        }

        public override async Task<DialogResult> ShowDialog()
        {
            string msg = this.ErrMsg + Environment.NewLine + this.ErrMsgDetail;
            Exception inner = this.GetInnerExeception(this.ExData);

            while (inner != null)
            {
                msg = msg + Environment.NewLine + inner.Message;
                inner = this.GetInnerExeception(inner);
            }

            Log.Error(this.FullClassName, msg);

            if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(this.Title, msg, this.buttonContinue.Text, this.buttonSendReport.Text)))
            {
                this.buttonContinue_Click(this, new EventArgs());
            }
            else
            {
                this.ButtonSendReport_Clicked(this, new EventArgs());
            }

            return this.DialogResult;
        }

        /// <summary>
        /// Gets or sets the ErrMsg
        /// </summary>
        private string ErrMsg
        {
            get
            {
                return this.errMsg;
            }

            set
            {
                this.errMsg = value;
            }
        }

        /// <summary>
        /// Gets or sets the ErrMsgDetail
        /// </summary>
        private string ErrMsgDetail
        {
            get
            {
                return this.errMsgDetail;
            }

            set
            {
                this.errMsgDetail = value;
            }
        }

        /// <summary>
        /// Gets or sets the ReportingMethod
        /// </summary>
        private string ReportingMethod
        {
            get
            {
                return this.reportingMethod;
            }

            set
            {
                this.reportingMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the FullClassName
        /// </summary>
        private string FullClassName
        {
            get
            {
                return this.fullClassName;
            }

            set
            {
                this.fullClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets the ExData
        /// </summary>
        private Exception ExData
        {
            get
            {
                return this.exData;
            }

            set
            {
                this.exData = value;
            }
        }

        /// <summary>
        /// Gets or sets the SysEnv
        /// </summary>
        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        /// <summary>
        /// The buttonSupportDetail_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSupportDetail_Click(object sender, EventArgs e)
        {
            string msg = this.ErrMsg + Environment.NewLine + Environment.NewLine + this.ErrMsgDetail;

            Exception inner = this.GetInnerExeception(this.ExData);
            while (inner != null)
            {
                msg = msg + Environment.NewLine + inner.Message;
                inner = this.GetInnerExeception(inner);
            }

            this.textBoxExData.Text = msg ;
        }

        /// <summary>
        /// The buttonContinue_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonContinue_Click(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () => await this.Close());
        }

        /// <summary>
        /// The GetInnerExeception
        /// </summary>
        /// <param name="outer"></param>
        /// <returns></returns>
        private Exception GetInnerExeception(Exception outer)
        {
            Exception inner = null;
            if (outer != null)
            {
                inner = outer.InnerException;
            }

            return inner;
        }

        private async void ButtonSendReport_Clicked(object sender, EventArgs e)
        {
            // TODO: Send error report
            await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Error", "Not Implemented", "OK"));
        }
    }
}
