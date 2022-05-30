// <copyright file="FormErrorReport.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Forms
{
    using System;
    using System.Threading.Tasks;
    using Android.Util;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="FormErrorReport"/> Error reporting form.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormErrorReport : DMobileModal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        public FormErrorReport()
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonContinue.Text = Resource.Continue;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> Exception object.</param>
        public FormErrorReport(Exception ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.ExData = ex;
            this.ErrMsg = this.ExData.Message;
            this.textBoxExData.Text = this.ErrMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="errMsg"><see cref="string"/>. </param>
        /// <param name="exData"><see cref="Exception"/> Exception object used for the detail message.</param>
        public FormErrorReport(string errMsg, Exception exData)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.ExData = exData;
            this.ErrMsg = errMsg;
            this.textBoxExData.Text = this.ErrMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> Exception object.</param>
        public FormErrorReport(SQLiteException ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;

            this.ExData = ex;
            this.ErrMsg = ex.Message;
            this.ErrMsgDetail = string.Empty;
            this.textBoxExData.Text = this.ErrMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <param name="ex"><see cref="Exception"/> Exception object.</param>
        public FormErrorReport(SysCommon.ActSubTypes activitySubType, Exception ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;
            this.ExData = ex;
            this.ErrMsg = this.ExData.Message;
            this.ErrMsgDetail = string.Empty;
            this.textBoxExData.Text = this.ErrMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <param name="ex"><see cref="Exception"/> Exception object.</param>
        public FormErrorReport(SysCommon.ActSubTypes activitySubType, SQLiteException ex)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;
            this.ExData = ex;
            this.ErrMsg = ex.Message;
            this.ErrMsgDetail = string.Empty;
            this.textBoxExData.Text = this.ErrMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <param name="ex"><see cref="Exception"/> Exception object.</param>
        /// <param name="fullClassName"><see cref="string"/> Full class name.</param>
        /// <param name="reportingMethod"><see cref="string"/> Reporting method.</param>
        public FormErrorReport(
            SysCommon.ActSubTypes activitySubType,
            Exception ex,
            string fullClassName,
            string reportingMethod)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;
            this.FullClassName = fullClassName;
            this.ReportingMethod = reportingMethod;
            this.ExData = ex;
            this.ErrMsg = this.ExData.Message;
            this.ErrMsgDetail = "Class: " + this.FullClassName + Environment.NewLine + "Method: " +
                this.ReportingMethod;
            this.textBoxExData.Text = this.ErrMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormErrorReport"/> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <param name="ex"><see cref="Exception"/> Exception object.</param>
        /// <param name="fullClassName"><see cref="string"/> Full class name.</param>
        /// <param name="reportingMethod"><see cref="string"/> Reporting method.</param>
        public FormErrorReport(
            SysCommon.ActSubTypes activitySubType,
            SQLiteException ex,
            string fullClassName,
            string reportingMethod)
        {
            this.InitializeComponent();

            this.Title = Resource.ExceptionReport;
            this.buttonSendReport.Text = Resource.SendReport;
            this.buttonSupportDetail.Text = Resource.SupportDetail;
            this.buttonContinue.Text = Resource.Continue;
            this.FullClassName = fullClassName;
            this.ReportingMethod = reportingMethod;
            this.ExData = ex;
            this.ErrMsg = ex.Message;
            this.ErrMsgDetail = "Class: " + this.FullClassName + Environment.NewLine + "Method: "
                + this.ReportingMethod + Environment.NewLine;
            this.textBoxExData.Text = this.ErrMsg;
        }

        /// <summary>
        /// Gets or sets the ErrMsg.
        /// </summary>
        private string ErrMsg { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ErrMsgDetail.
        /// </summary>
        private string ErrMsgDetail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ReportingMethod.
        /// </summary>
        private string ReportingMethod { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the FullClassName.
        /// </summary>
        private string FullClassName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ExData.
        /// </summary>
        private Exception ExData { get; set; }

        /// <summary>
        /// Display the error report to the screen.
        /// </summary>
        /// <returns><see cref="Task{DialogResult}"/> Dialog response result.</returns>
        public override async Task<DialogResult> ShowDialog()
        {
            string msg = this.ErrMsg + Environment.NewLine + this.ErrMsgDetail;
            Exception inner = this.GetInnerExeception(this.ExData);

            while (inner != null)
            {
                msg = msg + Environment.NewLine + inner.Message;
                inner = this.GetInnerExeception(inner);
            }

            _ = Log.Error(this.FullClassName, msg);

            if (await Device.InvokeOnMainThreadAsync(async () =>
                await Application.Current.MainPage.DisplayAlert(
                    this.Title,
                    msg,
                    this.buttonContinue.Text,
                    this.buttonSendReport.Text)))
            {
                this.ButtonContinue_Click(this, new EventArgs());
            }
            else
            {
                this.ButtonSendReport_Clicked(this, new EventArgs());
            }

            return this.DialogResult;
        }

        /// <summary>
        /// Support detail button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private void ButtonSupportDetail_Click(object sender, EventArgs e)
        {
            string msg = this.ErrMsg + Environment.NewLine + Environment.NewLine + this.ErrMsgDetail;

            Exception inner = this.GetInnerExeception(this.ExData);
            while (inner != null)
            {
                msg = msg + Environment.NewLine + inner.Message;
                inner = this.GetInnerExeception(inner);
            }

            this.textBoxExData.Text = msg;
        }

        /// <summary>
        /// Continue button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private void ButtonContinue_Click(object sender, EventArgs e)
        {
            // TODO: Continue may not be a good idea. Forcing a close would be one possible option.
        }

        /// <summary>
        /// Get the inner exception.
        /// </summary>
        /// <param name="outer"><see cref="Exception"/> Outer exception object.</param>
        /// <returns><see cref="Exception"/> Inner exception if any, else null.</returns>
        private Exception GetInnerExeception(Exception outer)
        {
            Exception inner = null;
            if (outer != null)
            {
                inner = outer.InnerException;
            }

            return inner;
        }

        /// <summary>
        /// Send report button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private async void ButtonSendReport_Clicked(object sender, EventArgs e)
        {
            // TODO: Send error report
            await Device.InvokeOnMainThreadAsync(async () =>
            await Application.Current.MainPage.DisplayAlert("Error", "Not Implemented.", "OK"));
        }
    }
}
