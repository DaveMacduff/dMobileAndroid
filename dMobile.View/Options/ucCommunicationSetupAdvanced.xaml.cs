using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages;
using dMobile.View.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucCommunicationSetupAdvanced : DMobileContentPage
    {
        #region Fields

        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private int connectionAttemptsMin = 0;
        private int connectionAttemptsMax = 10;
        private int sendTimeoutSecondsMin = 0;
        private int sendTimeoutSecondsMax = 60;
        private int sendAttemptsMin = 0;
        private int sendAttemptsMax = 10;
        private int receivedAttemptsMin = 0;
        private int receivedAttemptsMax = 10;
        private int receiveTimeoutSecondsMin = 0;
        private int receiveTimeoutSecondsMax = 999;
        private int receiveBufferSizeMin = 2048;
        private int receiveBufferSizeMax = 65536;
        private SysEnvironment sysEnv;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ucCommunicationSetupAdvanced()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            // TODO: Resource strings
            this.lblConnectionAttempts.Text = "Connection Attempts";
            this.lblSendTimeOutSec.Text = "Send Timeout Seconds";
            this.lblSendAttempts.Text = "Send Attempts";
            this.lblReceiveAttempts.Text = "Receive Attempts";
            this.lblReceiveTimeOutSec.Text = "Receive Timeout Seconds";
            this.lblReceiveBufferSize.Text = "Receive Buffer Size Bytes";
            this.btnSave.Text = "Save";

            if (!DesignMode.IsDesignModeEnabled)
            {
                sysEnv = SysEnvironment.GetInstance();

                try
                {
                    this.ucCommunicationSetupAdvanced_GotFocus(this, null);
                }
                catch (Exception ex)
                {
                    // TODO: Logging
                    Console.WriteLine("ucCommunicationSetupAdvanced_GotFocus threw an error" + ex);
                }
            }

        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        await this.Close();
                    }
                }
            });
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

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

        #endregion

        #region Properties

        public bool IsChangesMade
        {
            get
            {
                return this.isChangesMade;
            }

            set
            {
                this.isChangesMade = value;
                OnPropertyChanged();
            }
        }

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

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

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Comm Setup Advanced";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                isReadyToSave = isReadyToSave && (int.Parse(this.updnConnectionAttempts.Text) >= this.connectionAttemptsMin && int.Parse(this.updnConnectionAttempts.Text) <= this.connectionAttemptsMax);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnReceiveAttempts.Text) >= this.receivedAttemptsMin && int.Parse(this.updnReceiveAttempts.Text) <= this.receivedAttemptsMax);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnReceiveBufferSize.Text) >= this.receiveBufferSizeMin && int.Parse(this.updnReceiveBufferSize.Text) <= this.receiveBufferSizeMax);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnReceiveTimeOutSec.Text) >= this.receiveTimeoutSecondsMin && int.Parse(this.updnReceiveTimeOutSec.Text) <= this.receiveTimeoutSecondsMax);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnSendAttempts.Text) >= this.sendAttemptsMin && int.Parse(this.updnSendAttempts.Text) <= sendAttemptsMax);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnSendTimeOutSec.Text) >= this.sendTimeoutSecondsMin && int.Parse(this.updnSendTimeOutSec.Text) <= this.sendTimeoutSecondsMax);
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (this.IsReadyToSave())
                {
                    this.SysEnv.CommSetup.iConnectionAttempts = Convert.ToInt32(this.updnConnectionAttempts.Text);
                    this.SysEnv.CommSetup.iReceiveAttempts = Convert.ToInt32(this.updnReceiveAttempts.Text);
                    this.SysEnv.CommSetup.iiReceiveBufferSize = Convert.ToInt32(this.updnReceiveBufferSize.Text);
                    this.SysEnv.CommSetup.iReceiveTimeOutSeconds = Convert.ToInt32(this.updnReceiveTimeOutSec.Text);
                    this.SysEnv.CommSetup.iSendAttempts = Convert.ToInt32(this.updnSendAttempts.Text);
                    this.SysEnv.CommSetup.iSendTimeOutSeconds = Convert.ToInt32(this.updnSendTimeOutSec.Text);

                    CommunicationSetup tempCommSetup = this.SysEnv.CommSetup;
                    saveComplete = this.SysEnv.CommSetup.SaveData(ref tempCommSetup);

                    this.IsChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = true;

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
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                //using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, this, ex))
                //{
                //    errRpt.ShowDialog();
                //}

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                //using (FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                //{
                //    errorReport.ShowDialog();
                //}

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucCommunicationSetupAdvanced_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.LoadData();
                this.IsControlLoaded = true;
                this.updnConnectionAttempts.Focus();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            try
            {
                // Connection Attempts
                if (true == (int.Parse(this.updnConnectionAttempts.Text) < this.connectionAttemptsMin || int.Parse(this.updnConnectionAttempts.Text) > this.connectionAttemptsMax))
                {
                    e.Cancel = true;
                    // TODO: Resource strings
                    string msg = string.Format("Connection attempts must be between {0} and {1}", this.connectionAttemptsMin, this.connectionAttemptsMax);
                    throw new MessageOnlyException(msg);
                }

                // Socket Receive Data Attempts
                if (true == (int.Parse(this.updnReceiveAttempts.Text) < this.receivedAttemptsMin || int.Parse(this.updnReceiveAttempts.Text) > this.receivedAttemptsMax))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    string msg = string.Format("Receive attempts must be between {0} and {1}", this.receivedAttemptsMin, this.receivedAttemptsMax);
                    throw new MessageOnlyException(msg);
                }

                // Socket Receive Buffer Size
                if (true == (int.Parse(this.updnReceiveBufferSize.Text) < this.receiveBufferSizeMin || int.Parse(this.updnReceiveBufferSize.Text) > this.receiveBufferSizeMax))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    string msg = string.Format("Receive buffer size must be between {0} and {1}", this.receiveBufferSizeMin, this.receiveBufferSizeMax);
                    throw new Exceptions.MessageOnlyException(msg);
                }

                // Socket Receive Time Out Seconds
                if (true == (int.Parse(this.updnReceiveTimeOutSec.Text) < this.receiveTimeoutSecondsMin || int.Parse(this.updnReceiveTimeOutSec.Text) > this.receiveTimeoutSecondsMax))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    string msg = string.Format("Receive buffer size must be between {0} and {1}", this.receiveTimeoutSecondsMin, this.receiveTimeoutSecondsMax);
                    throw new Exceptions.MessageOnlyException(msg);
                }

                // Socket Send Data Attempts
                if (true == (int.Parse(this.updnSendAttempts.Text) < this.sendAttemptsMin || int.Parse(this.updnSendAttempts.Text) > this.sendAttemptsMax))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    string msg = string.Format("Send attempts must be between {0} and {1}", this.sendAttemptsMin, this.sendAttemptsMax);
                    throw new Exceptions.MessageOnlyException(msg);
                }

                // Socket Send Time Out Seconds
                if (true == (int.Parse(this.updnSendTimeOutSec.Text) < this.sendTimeoutSecondsMin || int.Parse(this.updnSendTimeOutSec.Text) > this.sendTimeoutSecondsMax))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    string msg = string.Format("Send time out seconds must be between {0} and {1}", this.sendTimeoutSecondsMin, this.sendTimeoutSecondsMax);
                    throw new Exceptions.MessageOnlyException(msg);
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                e.Cancel = true;
                ex.Show(SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                e.Cancel = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
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
            catch (Exceptions.MessageOnlyException ex)
            {
                // Do not set the IsChangesMade. Exceptions.MessageOnlyException is really not and error. 
                // Allow the validation to handle the setting for IsChangesMade.
                ex.Show(SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.SysEnv.CommSetup = this.SysEnv.CommSetup.LoadData();

            this.updnConnectionAttempts.Text = this.SysEnv.CommSetup.iConnectionAttempts.ToString();
            this.updnReceiveAttempts.Text = this.SysEnv.CommSetup.iReceiveAttempts.ToString();
            this.updnReceiveBufferSize.Text = this.SysEnv.CommSetup.iiReceiveBufferSize.ToString();
            this.updnReceiveTimeOutSec.Text = this.SysEnv.CommSetup.iReceiveTimeOutSeconds.ToString();
            this.updnSendAttempts.Text = this.SysEnv.CommSetup.iSendAttempts.ToString();
            this.updnSendTimeOutSec.Text = this.SysEnv.CommSetup.iSendTimeOutSeconds.ToString();
        }

        #endregion

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            this.ValidateEntry(null, new CancelEventArgs());
            this.SaveChanges();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IdentifyChangesMade(null, new EventArgs());
        }
    }
}