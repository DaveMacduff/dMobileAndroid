// <copyright file="CommunicationSetupAdvanced.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using CustomPages;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommunicationSetupAdvanced : DMobileContentPage
    {
        private bool isChangesMade;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommunicationSetupAdvanced" /> class.
        /// </summary>
        public CommunicationSetupAdvanced()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            this.IsIndicatorRunning = false;
            this.lblSendTimeOutSec.Text = "Send Timeout Seconds";
            this.lblReceiveTimeOutSec.Text = "Receive Timeout Seconds";
            this.lblReceiveBufferSize.Text = "Receive Buffer Size Bytes";
            this.btnSave.Text = "Save";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();

                try
                {
                    this.ucCommunicationSetupAdvanced_GotFocus(this, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ucCommunicationSetupAdvanced_GotFocus threw an error" + ex);
                }
            }
        }

        public int ReceiveBufferSizeMax { get; } = 65536;

        public int ReceiveBufferSizeMin { get; } = 2048;

        public int ReceiveTimeoutSecondsMax { get; } = 30;

        public int ReceiveTimeoutSecondsMin { get; } = 0;

        public int SendTimeoutSecondsMax { get; } = 10;

        public int SendTimeoutSecondsMin { get; } = 0;

        public bool IsChangesMade
        {
            get => this.isChangesMade;

            set
            {
                this.isChangesMade = value;
                this.OnPropertyChanged();
            }
        }

        private bool IsControlLoaded { get; set; }

        private SysEnvironment SysEnv { get; }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                base.OnAppearing();

                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    async keyPressed =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.Enter ||
                            keycode == Keycode.F1)
                        {
                            if (await this.SaveChanges())
                            {
                                await this.Close();
                            }
                        }
                    });

                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc />
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    _ = base.OnBackButtonPressed();

                    if (await this.SaveChanges())
                    {
                        await this.Close();
                    }
                    else if (await this.CancelChanges())
                    {
                        await this.Close();
                    }
                });

            return true;
        }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Comm Setup Advanced";
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = int.Parse(this.updnReceiveBufferSize.Text) >=
                                this.ReceiveBufferSizeMin &&
                                int.Parse(this.updnReceiveBufferSize.Text) <=
                                this.ReceiveBufferSizeMax;
                isReadyToSave = isReadyToSave &&
                                int.Parse(this.updnReceiveTimeOutSec.Text) >=
                                this.ReceiveTimeoutSecondsMin &&
                                int.Parse(this.updnReceiveTimeOutSec.Text) <=
                                this.ReceiveTimeoutSecondsMax;
                isReadyToSave = isReadyToSave &&
                                int.Parse(this.updnSendTimeOutSec.Text) >=
                                this.SendTimeoutSecondsMin &&
                                int.Parse(this.updnSendTimeOutSec.Text) <=
                                this.SendTimeoutSecondsMax;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToSave = false;
                _ = Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                Device.BeginInvokeOnMainThread(
                    async () =>
                    {
                        var errorReport = new FormErrorReport(
                            SysCommon.ActSubTypes.CommunicationSetup, /*this,*/
                            ex);
                        _ = await errorReport.ShowDialog();
                    });
            }

            return isReadyToSave;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (this.IsReadyToSave())
                {
                    this.SysEnv.CommSetup.ReceiveBufferSize =
                        Convert.ToInt32(this.updnReceiveBufferSize.Text);
                    this.SysEnv.CommSetup.ReceiveTimeOutSeconds =
                        Convert.ToInt32(this.updnReceiveTimeOutSec.Text);
                    this.SysEnv.CommSetup.SendTimeOutSeconds =
                        Convert.ToInt32(this.updnSendTimeOutSec.Text);

                    var tempCommSetup = this.SysEnv.CommSetup;
                    saveComplete = this.SysEnv.CommSetup.SaveData(ref tempCommSetup);

                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () => await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Data failed validation. Check your data.",
                            "OK"));
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errorReport = new FormErrorReport(
                            SysCommon.ActSubTypes.RefreshData, /*this,*/
                            ex);
                        _ = await errorReport.ShowDialog();
                    });
            }

            return saveComplete;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
                if (this.IsChangesMade)
                {
                    // Perform any checks or changes needed to allow a rollback or form close.
                    // Set IsReadyToClose = false if checks fail.
                }

                if (isReadyToClose)
                {
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initialize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                // using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, this, ex))
                // {
                //    errRpt.ShowDialog();
                // }
                Device.BeginInvokeOnMainThread(
                    async () =>
                    {
                        var errorReport = new FormErrorReport(
                            SysCommon.ActSubTypes.RefreshData, /*this,*/
                            ex);
                        _ = await errorReport.ShowDialog();
                    });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Expect the control to close.
                }
                else
                {
                    // If changes where made.
                    // By reloading, restoring the last valid state we should be able to close the control
                    // on the second CancelChanges attempt.
                    // Methods that could be called to restore the control to a closable state.
                    // ClearProperties();
                    // DisplayData();
                    this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                // using (FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                // {
                //    errorReport.ShowDialog();
                // }
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errorReport = new FormErrorReport(
                            SysCommon.ActSubTypes.CommunicationSetup, /*this,*/
                            ex);
                        _ = await errorReport.ShowDialog();
                    });
            }

            return cancelChanges;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public async Task<XDocument> GetXML()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucCommunicationSetupAdvanced_GotFocus(object sender, EventArgs e)
        {
            this.LoadData();
            this.IsControlLoaded = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            try
            {
                // Socket Receive Buffer Size
                if (int.Parse(this.updnReceiveBufferSize.Text) < this.ReceiveBufferSizeMin ||
                    int.Parse(this.updnReceiveBufferSize.Text) > this.ReceiveBufferSizeMax)
                {
                    e.Cancel = true;

                    var msg =
                        $"Receive buffer size must be between {this.ReceiveBufferSizeMin} and {this.ReceiveBufferSizeMax}";
                    throw new MessageOnlyException(msg);
                }

                // Socket Receive Time Out Seconds
                if (int.Parse(this.updnReceiveTimeOutSec.Text) < this.ReceiveTimeoutSecondsMin ||
                    int.Parse(this.updnReceiveTimeOutSec.Text) > this.ReceiveTimeoutSecondsMax)
                {
                    e.Cancel = true;

                    var msg =
                        $"Receive buffer size must be between {this.ReceiveTimeoutSecondsMin} and {this.ReceiveTimeoutSecondsMax}";
                    throw new MessageOnlyException(msg);
                }

                // Socket Send Time Out Seconds
                if (int.Parse(this.updnSendTimeOutSec.Text) < this.SendTimeoutSecondsMin ||
                    int.Parse(this.updnSendTimeOutSec.Text) > this.SendTimeoutSecondsMax)
                {
                    e.Cancel = true;

                    var msg =
                        $"Send time out seconds must be between {this.SendTimeoutSecondsMin} and {this.SendTimeoutSecondsMax}";
                    throw new MessageOnlyException(msg);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                e.Cancel = true;
                _ = Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                e.Cancel = false;

                Device.BeginInvokeOnMainThread(
                    async () =>
                    {
                        var errorReport = new FormErrorReport(
                            SysCommon.ActSubTypes.CommunicationSetup, /*this,*/
                            ex);
                        _ = await errorReport.ShowDialog();
                    });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);

                // Do not set the IsChangesMade. MessageOnlyException is really not and error.
                // Allow the validation to handle the setting for IsChangesMade.
                _ = Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(
                    async () =>
                    {
                        var errorReport = new FormErrorReport(
                            SysCommon.ActSubTypes.CommunicationSetup, /*this,*/
                            ex);
                        _ = await errorReport.ShowDialog();
                    });
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.SysEnv.CommSetup = this.SysEnv.CommSetup.LoadData();

            this.updnReceiveBufferSize.Text = this.SysEnv.CommSetup.ReceiveBufferSize.ToString();
            this.updnReceiveTimeOutSec.Text =
                this.SysEnv.CommSetup.ReceiveTimeOutSeconds.ToString();
            this.updnSendTimeOutSec.Text = this.SysEnv.CommSetup.SendTimeOutSeconds.ToString();
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            this.ValidateEntry(null, new CancelEventArgs());
            _ = await this.SaveChanges();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IdentifyChangesMade(null, EventArgs.Empty);
        }
    }
}