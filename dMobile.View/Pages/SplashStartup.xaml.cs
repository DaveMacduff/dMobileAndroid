// <copyright file="SplashStartup.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.OS;
    using Android.Views;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Forms;
    using GroundWork;
    using Resources.Enums;
    using Views;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Startup splash.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashStartup : IAppArea, IDataNubListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SplashStartup" /> class.
        /// </summary>
        public SplashStartup()
        {
            this.InitializeComponent();
            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.StartupItems = new ObservableCollection<string>();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether an error was raised during startup loading data in memory.
        /// </summary>
        private bool IsErrorRaised { get; set; }

        /// <summary>
        /// Gets the startup items to display.
        /// </summary>
        private ObservableCollection<string> StartupItems { get; }

        /// <summary>
        /// Gets or sets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Representing the asynchronous operation.</returns>
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Representing the asynchronous operation.</returns>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsReadyToSave()
        {
            const bool isReadyToSave = true;
            return Task.FromResult(isReadyToSave);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> CancelChanges()
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<XDocument> GetXml()
        {
            var doc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return doc;
        }

        /// <inheritdoc />
        public void OnNubStatOut(IDataNubPublisher databaseNubEventPublisher, DataNubEventArgs databaseNubEventArgs)
        {
            this.StartUpStatusOut(this, EventArgs.Empty, databaseNubEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                base.OnAppearing();
                SubscribeKeyUpHandlers();
                this.listBoxStatus.ItemsSource = this.StartupItems;
                await Task.Run(async () => await LoadData(this, EventArgs.Empty));
                this.IsIndicatorRunning = false;
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), keyPressed =>
                {
                    try
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.Enter)
                        {
                            if (this.btnClose.IsEnabled)
                            {
                                this.BtnClose_Clicked(this, EventArgs.Empty);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }

            //////////////////////////////////////////////////////////////////////////
            async Task LoadData(object sender, EventArgs e)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        this.StartUpStatusOut(sender, e, "Initializing environment.");
                        this.SysEnv.IsCommunicationTestSuccessful = this.CommunicationTest(sender, e);
                        this.InitializeScanner(sender, e);
                        await this.InitializeDatabase(sender, e);
                        _ = LocationMap.GetInstance();
                        this.DataIntegrityCheck();
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errReport = new FormErrorReport(
                                SysCommon.ActSubTypes.SplashStartup,
                                ex,
                                this.GetControlDescription(),
                                "Startup loading data.");
                            _ = await errReport.ShowDialog();
                            Process.KillProcess(Process.MyPid());
                        });
                    }
                });
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

        /// <summary>
        ///     Check if refresh data worked.
        /// </summary>
        private void DataIntegrityCheck()
        {
            if (this.SysEnv.RefreshOpt.IsRefreshDataFailed)
            {
                Device.BeginInvokeOnMainThread(DisplayRefreshAgainMessage);
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async void DisplayRefreshAgainMessage()
            {
                await this.DisplayAlert(
                    this.GetControlDescription(),
                    "The last data refresh did not complete. You must refresh the data again.",
                    "OK");
            }
        }

        /// <summary>
        /// Get the control form description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        private string GetControlDescription()
        {
            return "dMobile Startup";
        }

        /// <summary>
        /// Initialize the database.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private async Task InitializeDatabase(object sender, EventArgs e)
        {
            var db = new DatabaseNub();

            this.SysEnv.RefreshOpt.IsDataCreatedInStartUp = false;

            db.RegNubListenerStatOut(this);

            this.StartUpStatusOut(sender, e, "Locating database.");

            if (File.Exists(DatabaseConnection.GetInstance().DatabaseFullPath) &&
                new FileInfo(DatabaseConnection.GetInstance().DatabaseFullPath).Length > 0)
            {
                this.StartUpStatusOut(sender, e, "Database found.");
                this.StartUpStatusOut(sender, e, DatabaseConnection.GetInstance().DatabaseFullPath);
            }
            else
            {
                this.StartUpStatusOut(sender, e, "Database missing.");
                this.StartUpStatusOut(sender, e, "Creating database.");

                await db.CreateNewDatabase();
                await db.CreateTables();

                this.StartUpStatusOut(sender, e, "Seeding system users.");

                var users = new TableContactUsers();
                await users.SeedContactUsersWithSystemUsers();

                this.IsErrorRaised = db.ErrorRaised;

                if (this.IsErrorRaised)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await this.DisplayAlert(this.GetControlDescription(), "Database creation failed.", "OK");
                    });
                }
                else
                {
                    this.StartUpStatusOut(sender, e, "Database creation completed.");

                    this.SysEnv.RefreshOpt.IsDataCreatedInStartUp = true;
                }
            }

            var tempRefreshDataOptions = this.SysEnv.RefreshOpt;
            _ = this.SysEnv.RefreshOpt.SaveData(ref tempRefreshDataOptions);
            db.UnregNubListenerStatOut(this);
        }

        /// <summary>
        /// Initial the scanner engine.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private void InitializeScanner(object sender, EventArgs e)
        {
            this.StartUpStatusOut(sender, e, "Identifying bar code scanner.");

            var scanner = ScannerEngine.GetInstance().Scanner;

            if (scanner == null)
            {
                Device.BeginInvokeOnMainThread(DisplayScannerUnknown);
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async void DisplayScannerUnknown()
            {
                _ = await this.DisplayAlert(
                    this.GetControlDescription(),
                    "Unable to identify bar code scanner.",
                    "OK",
                    "Cancel");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="strMessage"></param>
        /// <remarks></remarks>
        private void StartUpStatusOut(object sender, EventArgs e, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.StartupItems.Add(message);
                this.listBoxStatus.ScrollTo(message, ScrollToPosition.MakeVisible, false);
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CommunicationTest(object sender, EventArgs e)
        {
            var testSuccessful = false;

            if (!this.SysEnv.SysSettings.IsDisconnectedMode && this.SysEnv.CommSetup.HostIpAddressComponent1 +
                this.SysEnv.CommSetup.HostIpAddressComponent2 + this.SysEnv.CommSetup.HostIpAddressComponent3 != 0)
            {
                this.StartUpStatusOut(sender, e, "Attempting communication test.");

                var sysSock = new SysSocket
                {
                    BasePort = this.SysEnv.CommSetup.BaseSocketPort,
                    DefSocketPort = this.SysEnv.CommSetup.DefaultSocketPort,
                    MaxPorts = this.SysEnv.CommSetup.MaxSocketPorts,
                    ConnTotalRetries = this.SysEnv.CommSetup.ConnectionAttempts,
                    SendTotalRetries = this.SysEnv.CommSetup.SendAttempts,
                    RecvTotalRetries = this.SysEnv.CommSetup.ReceiveAttempts,
                };

                this.StartUpStatusOut(sender, e, $"Base socket port: {sysSock.BasePort}");
                this.StartUpStatusOut(sender, e, $"Default socket port: {sysSock.DefSocketPort}");
                this.StartUpStatusOut(sender, e, $"Max socket port: {sysSock.MaxPorts}");
                this.StartUpStatusOut(sender, e, $"Connection retries: {sysSock.ConnTotalRetries}");
                this.StartUpStatusOut(sender, e, $"Socket send retries: {sysSock.SendTotalRetries}");
                this.StartUpStatusOut(sender, e, $"Socket receive retries: {sysSock.RecvTotalRetries}");
                this.StartUpStatusOut(sender, e, $"Send time out: {this.SysEnv.CommSetup.SendTimeOutSeconds} seconds.");
                this.StartUpStatusOut(sender, e, $"Receive time out: {this.SysEnv.CommSetup.ReceiveTimeOutSeconds} seconds.");

                sysSock.RecvBuffSize = this.SysEnv.CommSetup.ReceiveBufferSize;
                this.StartUpStatusOut(sender, e, $"Receive buffer size: {sysSock.RecvBuffSize} ");

                sysSock.ServerIP = this.SysEnv.CommSetup.HostIpAddressComponent1 + "."
                    + this.SysEnv.CommSetup.HostIpAddressComponent2 + "."
                    + this.SysEnv.CommSetup.HostIpAddressComponent3 + "."
                    + this.SysEnv.CommSetup.HostIpAddressComponent4;

                this.StartUpStatusOut(sender, e, $"Server host IP: {sysSock.ServerIP}");
                this.StartUpStatusOut(sender, e, "Creating connection request.");

                try
                {
                    var socket = sysSock.Connect();
                    if (socket == null)
                    {
                        this.StartUpStatusOut(sender, e, "Connection failed.");
                    }
                    else
                    {
                        this.StartUpStatusOut(sender, e, "Sending device identifier.");

                        var sendReq = CommunicationSetup.CreateXmlSubmitDeviceId();
                        var bytesSent = sysSock.SendRequest(socket, sendReq);
                        this.StartUpStatusOut(sender, e, $"Bytes sent: {bytesSent}");

                        this.StartUpStatusOut(sender, e, "Waiting for response.");
                        var receivedData = sysSock.ReceiveResponse(socket);
                        socket.Disconnect(true);
                        socket.Close();

                        this.StartUpStatusOut(sender, e, "Socket closed.");

                        var responseData = Compress.UnZip(receivedData.ToArray());
                        if (responseData.Contains("<message>ERROR</message>"))
                        {
                            var docError = new XmlDocument();
                            docError.LoadXml(responseData);
                            if (docError.DocumentElement != null)
                            {
                                var errorDetail = docError.DocumentElement["detail"]?.InnerText;
                                this.StartUpStatusOut(sender, e, "Submit device id error.");
                                this.StartUpStatusOut(sender, e, errorDetail);
                            }
                        }
                        else
                        {
                            this.StartUpStatusOut(sender, e, "Connection succeeded.");
                            testSuccessful = true;
                        }
                    }
                }
                catch (SocketException)
                {
                    this.StartUpStatusOut(sender, e, "Socket exception occurred.");
                    this.StartUpStatusOut(sender, e, "Connection failed.");
                    Device.BeginInvokeOnMainThread(DisplayCommunicationTestFailed);
                }
            }

            return testSuccessful;

            //////////////////////////////////////////////////////////////////////////
            async void DisplayCommunicationTestFailed()
            {
                await this.DisplayAlert(this.GetControlDescription(), "Communication test failed.", "OK");
            }
        }

        /// <summary>
        /// Button close click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new NavigationPage(new LoginStartup()));
        }
    }
}