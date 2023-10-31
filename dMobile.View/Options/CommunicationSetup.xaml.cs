// <copyright file="CommunicationSetup.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommunicationSetup : DMobileContentPage, IAppArea, ISysSocketEventListener
    {
        private ObservableCollection<string> itemsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSetup"/> class.
        /// </summary>
        public CommunicationSetup()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            this.lblMobileServerIP.Text = "Mobile Server IP Address";
            this.lblBasePort.Text = "Base Port";
            this.lblMaxPorts.Text = "Max Ports";
            this.lblDefaultPort.Text = "Default Port";
            this.cmdAdvanced.Text = "Advanced";
            this.cmdConnect.Text = "Connect";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.SysSock = new SysSocket();
                this.ExtraInfoItems = new ObservableCollection<string>();
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        await this.Close();
                    }
                }
            });

            try
            {
                await this.ucCommunicationSetup_GotFocus(this, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ucCommunicationSetup_GotFocus threw an error" + ex);
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc/>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
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

        private SysSocket SysSock { get; set; }

        private bool IsControlLoaded { get; set; } = false;

        private bool IsChangesMade { get; set; } = false;

        private SysEnvironment SysEnv { get; set; }

        public ObservableCollection<string> ExtraInfoItems
        {
            get => this.itemsList;
            set => this.itemsList = value;
        }

        private int MinIPAddress { get; } = 0;

        private int MaxIPAddress { get; } = 255;

        private int MinPort { get; } = 0;

        private int MaxPort { get; } = 20000;

        private int MaxNumberOfPorts { get; } = 100;

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
            return "Communication Setup";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                var maxPort = Convert.ToInt32(int.Parse(this.updnBasePort.Text) + int.Parse(this.updnMaxPorts.Text) - 1);

                isReadyToSave = isReadyToSave && int.Parse(this.updnBasePort.Text) >= this.MinPort && int.Parse(this.updnBasePort.Text) <= this.MaxPort;
                isReadyToSave = isReadyToSave && int.Parse(this.updnMaxPorts.Text) >= this.MinPort && int.Parse(this.updnMaxPorts.Text) <= this.MaxNumberOfPorts;
                isReadyToSave = isReadyToSave && int.Parse(this.updnIP1.Text) >= this.MinIPAddress && int.Parse(this.updnIP1.Text) <= this.MaxIPAddress;
                isReadyToSave = isReadyToSave && int.Parse(this.updnIP2.Text) >= this.MinIPAddress && int.Parse(this.updnIP2.Text) <= this.MaxIPAddress;
                isReadyToSave = isReadyToSave && int.Parse(this.updnIP3.Text) >= this.MinIPAddress && int.Parse(this.updnIP3.Text) <= this.MaxIPAddress;
                isReadyToSave = isReadyToSave && int.Parse(this.updnIP4.Text) >= this.MinIPAddress && int.Parse(this.updnIP4.Text) <= this.MaxIPAddress;
                isReadyToSave = isReadyToSave && int.Parse(this.updnDefaultPort.Text) >= int.Parse(this.updnBasePort.Text) && int.Parse(this.updnDefaultPort.Text) <= this.MaxPort;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.SysEnv.CommSetup.HostIPAddressComponent1 = int.Parse(this.updnIP1.Text);
                    this.SysEnv.CommSetup.HostIPAddressComponent2 = int.Parse(this.updnIP2.Text);
                    this.SysEnv.CommSetup.HostIPAddressComponent3 = int.Parse(this.updnIP3.Text);
                    this.SysEnv.CommSetup.HostIPAddressComponent4 = int.Parse(this.updnIP4.Text);
                    this.SysEnv.CommSetup.BaseSocketPort = int.Parse(this.updnBasePort.Text);
                    this.SysEnv.CommSetup.DefaultSocketPort = int.Parse(this.updnDefaultPort.Text);
                    this.SysEnv.CommSetup.MaxSocketPorts = int.Parse(this.updnMaxPorts.Text);

                    var tempCommSetup = this.SysEnv.CommSetup;
                    isSaveComplete = this.SysEnv.CommSetup.SaveData(ref tempCommSetup);

                    this.IsChangesMade = false;
                }
                else
                {
                    throw new MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return isSaveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
                await Device.InvokeOnMainThreadAsync(async () =>
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

                // using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                // {
                //    errRpt.ShowDialog();
                // }
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
                await Device.InvokeOnMainThreadAsync(async () =>
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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<XDocument> GetXML()
        {
            var xmldocParams = XDocument.Parse(
                      "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                      + "<params>"
                      + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ucCommunicationSetup_GotFocus(object sender, EventArgs e)
        {
            this.LoadData();
            this.IsControlLoaded = true;
            _ = this.updnIP1.Focus();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            try
            {
                // Socket Base Port
                // if (sender == this.updnBasePort)
                // {
                if ((int.Parse(this.updnBasePort.Text) < this.MinPort || int.Parse(this.updnBasePort.Text) > this.MaxPort) == true)
                {
                    e.Cancel = true;
                    throw new MessageOnlyException($"Base port must be between {this.MinPort} and {this.MaxPort}");
                }

                // }

                // Socket Max Ports
                // if (sender == this.updnMaxPorts)
                // {
                if ((int.Parse(this.updnMaxPorts.Text) < this.MinPort || int.Parse(this.updnMaxPorts.Text) > this.MaxNumberOfPorts) == true)
                {
                    e.Cancel = true;

                    throw new MessageOnlyException($"port must be between {this.MinPort} and {this.MaxNumberOfPorts}");
                }

                // }

                // IP Address Component 1
                // if (sender == this.updnIP1)
                // {
                if ((int.Parse(this.updnIP1.Text) < this.MinIPAddress || int.Parse(this.updnIP1.Text) > this.MaxIPAddress) == true)
                {
                    e.Cancel = true;

                    throw new MessageOnlyException(
                        $"IP address component must be between {this.MinIPAddress} and {this.MaxIPAddress}");
                }

                // }

                // IP Address Component 2
                // if (sender == this.updnIP2)
                // {
                if ((int.Parse(this.updnIP2.Text) < this.MinIPAddress || int.Parse(this.updnIP2.Text) > this.MaxIPAddress) == true)
                {
                    e.Cancel = true;

                    throw new MessageOnlyException(
                        $"IP address component must be between {this.MinIPAddress} and {this.MaxIPAddress}");
                }

                // }

                // IP Address Component 3
                // if (sender == this.updnIP3)
                // {
                if ((int.Parse(this.updnIP3.Text) < this.MinIPAddress || int.Parse(this.updnIP3.Text) > this.MaxIPAddress) == true)
                {
                    e.Cancel = true;

                    throw new MessageOnlyException(
                        $"IP address component must be between {this.MinIPAddress} and {this.MaxIPAddress}");
                }

                // }

                // IP Address Component 4
                // if (sender == this.updnIP4)
                // {
                if ((int.Parse(this.updnIP4.Text) < this.MinIPAddress || int.Parse(this.updnIP4.Text) > this.MaxIPAddress) == true)
                {
                    e.Cancel = true;

                    throw new MessageOnlyException(
                        $"IP address component must be between {this.MinIPAddress} and {this.MaxIPAddress}");
                }

                // }

                // Default Socket Port
                // if (sender == this.updnDefaultPort)
                // {
                var maxPort = (int)(int.Parse(this.updnBasePort.Text) + int.Parse(this.updnMaxPorts.Text) - 1);

                if ((int.Parse(this.updnDefaultPort.Text) < int.Parse(this.updnBasePort.Text) || int.Parse(this.updnDefaultPort.Text) > this.MaxPort) == true)
                {
                    e.Cancel = true;

                    throw new MessageOnlyException(
                        $"Default port must be between {this.updnBasePort.Text} and {this.MaxPort}");
                }

                // }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                e.Cancel = true;
                _ = Device.InvokeOnMainThreadAsync(async () =>
                      await Application.Current.MainPage.DisplayAlert(
                          this.GetControlDescription(),
                          msg,
                          Resource.OK));
            }
            catch (Exception ex)
            {
                e.Cancel = false;

                // using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                // {
                //    errRpt.ShowDialog();
                // }
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void IdentifyChangesMade(object sender, EventArgs e)
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
                _ = Device.InvokeOnMainThreadAsync(async () =>
                      await Application.Current.MainPage.DisplayAlert(
                          this.GetControlDescription(),
                          msg,
                          Resource.OK));
            }
            catch (Exception ex)
            {
                // using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                // {
                //    errRpt.ShowDialog();
                // }
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAdvanced_Click(object sender, EventArgs e)
        {
            // Needed to check if the enter key was pressed last, because it seems that
            // SelectNextControl causes a click event to fire if it is the next control
            var setupAdvanced = new CommunicationSetupAdvanced();
            await setupAdvanced.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdConnect_Click(object sender, EventArgs e)
        {
            this.ValidateEntry(this, new CancelEventArgs());

            if (await this.SaveChanges())
            {
                await this.Connect();
                var status = new CommunicationSetupStatus();
                status.ExtraInfoItems = this.ExtraInfoItems;
                _ = await status.ShowDialog();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sysSocketEventPublisher"></param>
        /// <param name="sysSocketEventArgs"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public void OnSysSocketStatusOut(ISysSocketEventPublisher sysSocketEventPublisher, SysSocketEventArgs sysSocketEventArgs)
        {
            this.AddCommMsg(this, null, sysSocketEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.SysEnv.CommSetup = this.SysEnv.CommSetup.LoadData();

            this.updnIP1.Text = this.SysEnv.CommSetup.HostIPAddressComponent1.ToString();
            this.updnIP2.Text = this.SysEnv.CommSetup.HostIPAddressComponent2.ToString();
            this.updnIP3.Text = this.SysEnv.CommSetup.HostIPAddressComponent3.ToString();
            this.updnIP4.Text = this.SysEnv.CommSetup.HostIPAddressComponent4.ToString();
            this.updnBasePort.Text = this.SysEnv.CommSetup.BaseSocketPort.ToString();
            this.updnDefaultPort.Text = this.SysEnv.CommSetup.DefaultSocketPort.ToString();
            this.updnMaxPorts.Text = this.SysEnv.CommSetup.MaxSocketPorts.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task Connect()
        {
            this.IsPageLoading = true;

            await Task.Run(() =>
            {
                string sendReq;
                Socket socket;
                int bytesSent;
                List<byte> bytRecvData;
                var responseData = string.Empty;

                this.ExtraInfoItems.Clear();

                // this.lstCommMsg.ItemsSource = null;
                this.SysEnv.SysSock.BasePort = this.SysEnv.CommSetup.BaseSocketPort;

                this.AddCommMsg(this, EventArgs.Empty, $"Base socket port: {this.SysEnv.SysSock.BasePort}");

                this.SysEnv.SysSock.DefSocketPort = this.SysEnv.CommSetup.DefaultSocketPort;

                this.AddCommMsg(this, EventArgs.Empty, $"Default socket port: {this.SysEnv.SysSock.DefSocketPort}");

                this.SysEnv.SysSock.MaxPorts = this.SysEnv.CommSetup.MaxSocketPorts;

                this.AddCommMsg(this, EventArgs.Empty, $"Max socket port: {this.SysEnv.SysSock.MaxPorts}");

                this.SysEnv.SysSock.ConnTotalRetries = this.SysEnv.CommSetup.ConnectionAttempts;

                this.AddCommMsg(this, EventArgs.Empty, $"Connection retries: {this.SysEnv.SysSock.ConnTotalRetries}");

                this.SysEnv.SysSock.SendTotalRetries = this.SysEnv.CommSetup.SendAttempts;

                this.AddCommMsg(this, EventArgs.Empty, $"Socket send retries: {this.SysEnv.SysSock.SendTotalRetries}");

                this.SysEnv.SysSock.RecvTotalRetries = this.SysEnv.CommSetup.ReceiveAttempts;

                this.AddCommMsg(this, EventArgs.Empty,
                    $"Socket receive retries: {this.SysEnv.SysSock.RecvTotalRetries}");

                this.SysEnv.SysSock.SendTimeOutMilliseconds = this.SysEnv.CommSetup.SendTimeOutSeconds * 1000;

                this.AddCommMsg(this, EventArgs.Empty,
                    $"Send time out: {this.SysEnv.CommSetup.SendTimeOutSeconds} seconds.");

                this.SysEnv.SysSock.ReceiveTimeOutMilliseconds = this.SysEnv.CommSetup.ReceiveTimeOutSeconds * 1000;

                this.AddCommMsg(this, EventArgs.Empty,
                    $"Receive time out: {this.SysEnv.CommSetup.ReceiveTimeOutSeconds} seconds.");

                this.SysEnv.SysSock.RecvBuffSize = this.SysEnv.CommSetup.ReceiveBufferSize;

                this.AddCommMsg(this, EventArgs.Empty, $"Receive buffer size: {this.SysEnv.SysSock.RecvBuffSize} ");

                this.SysEnv.SysSock.ServerIP = this.SysEnv.CommSetup.HostIPAddressComponent1
                    + "." + this.SysEnv.CommSetup.HostIPAddressComponent2 + "."
                    + this.SysEnv.CommSetup.HostIPAddressComponent3 + "."
                    + this.SysEnv.CommSetup.HostIPAddressComponent4;

                this.AddCommMsg(this, EventArgs.Empty, $"Server host ip: {this.SysEnv.SysSock.ServerIP}");
                this.AddCommMsg(this, EventArgs.Empty, "Creating connection request.");

                sendReq = Bedrock.CommunicationSetup.CreateXMLSubmitDeviceID();

                try
                {
                    this.SysSock.RegSockListenerStatusOut((ISysSocketEventListener)this);
                    this.SysEnv.IsCommunicationTestSuccessful = false;
                    socket = this.SysEnv.SysSock.Connect();

                    if (socket == null)
                    {
                        this.AddCommMsg(this, EventArgs.Empty, "Connection failed.");
                    }
                    else
                    {
                        this.AddCommMsg(this, EventArgs.Empty, "Sending device id.");
                        bytesSent = this.SysEnv.SysSock.SendRequest(socket, sendReq);

                        this.AddCommMsg(this, EventArgs.Empty, $"Bytes sent: {bytesSent}");

                        this.AddCommMsg(this, EventArgs.Empty, "Waiting for response.");
                        bytRecvData = this.SysEnv.SysSock.ReceiveResponse(socket);

                        socket.Disconnect(true);
                        socket.Close();

                        this.AddCommMsg(this, EventArgs.Empty, "Socket closed.");

                        responseData = Compress.UnZip(bytRecvData.ToArray());

                        if (responseData.Contains("<message>ERROR</message>"))
                        {
                            var docError = new XmlDocument();
                            docError.LoadXml(responseData);
                            var errorDetail = docError.DocumentElement["detail"].InnerText;

                            this.AddCommMsg(this, EventArgs.Empty, "Submit device id error.");
                            this.AddCommMsg(this, EventArgs.Empty, errorDetail);
                        }
                        else
                        {
                            this.AddCommMsg(this, EventArgs.Empty, "Connection succeeded.");
                            this.SysEnv.IsCommunicationTestSuccessful = true;
                        }
                    }
                }
                catch (SocketException ex)
                {
                    this.AddCommMsg(this, EventArgs.Empty, "Socket exception occurred.");
                    this.AddCommMsg(this, EventArgs.Empty, "Connection failed.");

                    // MessageBox.Show(string.Format("Code {0}, {1}", ex.ErrorCode, ex.Message));
                    Device.BeginInvokeOnMainThread(async () => await this.DisplayAlert("Error",
                        $"Code {ex.ErrorCode}, {ex.Message}", "OK"));
                }
                finally
                {
                    this.SysSock.UnregSockListenerStatusOut((ISysSocketEventListener)this);
                }
            });

            this.IsPageLoading = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <remarks></remarks>
        private void AddCommMsg(object sender, EventArgs e, string message)
        {
            this.ExtraInfoItems.Add(message);
        }

        private void UpdnIP1_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP1.CursorPosition = 0;
            this.updnIP1.SelectionLength = this.updnIP1.Text.Length;
        }

        private void UpdnIP2_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP2.CursorPosition = 0;
            this.updnIP2.SelectionLength = this.updnIP2.Text.Length;
        }

        private void UpdnIP3_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP3.CursorPosition = 0;
            this.updnIP3.SelectionLength = this.updnIP3.Text.Length;
        }

        private void UpdnIP4_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP4.CursorPosition = 0;
            this.updnIP4.SelectionLength = this.updnIP4.Text.Length;
        }

        private void UpdnBasePort_Focused(object sender, FocusEventArgs e)
        {
            this.updnBasePort.CursorPosition = 0;
            this.updnBasePort.SelectionLength = this.updnBasePort.Text.Length;
        }

        private void UpdnMaxPorts_Focused(object sender, FocusEventArgs e)
        {
            this.updnMaxPorts.CursorPosition = 0;
            this.updnMaxPorts.SelectionLength = this.updnMaxPorts.Text.Length;
        }

        private void UpdnDefaultPort_Focused(object sender, FocusEventArgs e)
        {
            this.updnDefaultPort.CursorPosition = 0;
            this.updnDefaultPort.SelectionLength = this.updnDefaultPort.Text.Length;
        }
    }
}
