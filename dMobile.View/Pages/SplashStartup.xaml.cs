// <copyright file="SplashStartup.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Android.Views;
using dMobile.BarCodeScanner.BarCodeScanners;
using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages;
using dMobile.View.Forms;
using dMobile.View.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashStartup : DMobileContentPage, IAppArea, IDataNubListener
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashStartup"/> class.
        /// </summary>
        public SplashStartup()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.ExtraInfoItems = new ObservableCollection<string>();
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.listBoxStatus.ItemsSource = this.ExtraInfoItems;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
            {
                try
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter)
                    {
                        if (this.btnClose.IsEnabled)
                        {
                            this.BtnClose_Clicked(this, new EventArgs());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            await Task.Run(async () => await this.LoadData(this, new EventArgs()));
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        public bool IsErrorRaised { get; set; } = false;

        public ObservableCollection<string> ExtraInfoItems { get; set; }

        private SysEnvironment SysEnv { get; set; } = null;

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
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;
            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "dMobile Startup";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            var doc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return doc;
        }

        public void OnNubStatOut(
            IDataNubPublisher databaseNubEventPublisher,
            DataNubEventArgs databaseNubEventArgs)
        {
            this.StartUpStatusOut(this, new EventArgs(), databaseNubEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void LoadLocationMap(object sender, EventArgs e)
        {
            this.StartUpStatusOut(sender, e, "Loading location map.");

            _ = LocationMap.GetInstance();

            this.StartUpStatusOut(sender, e, "Loading location map completed.");
        }

        /// <summary>
        ///
        /// </summary>
        private void DataIntegrityCheck()
        {
            if (this.SysEnv.RefreshOpt.IsRefreshDataFailed)
            {
                // MessageBox.Show(
                //    "The last data refresh did not complete. You must refresh the data again.",
                //    this.GetControlDescription(),
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Exclamation,
                //    MessageBoxDefaultButton.Button1);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await this.DisplayAlert(this.GetControlDescription(), "The last data refresh did not complete. You must refresh the data again.", "OK");
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool SyncDateTime(object sender, EventArgs e)
        {
            var isDateTimeChanged = false;

            // if (!this.SysEnv.SysSettings.IsDisconnectedMode
            //    && (this.SysEnv.CommSetup.iHostIPAddressComponent1
            //    + this.SysEnv.CommSetup.iHostIPAddressComponent2
            //    + this.SysEnv.CommSetup.iHostIPAddressComponent3) != 0)
            // {

            // this.StartUpStatusOut(sender, e, "Attempting to sync date & time.");

            // SysSocket sysSock = new SysSocket();
            //    sysSock.BasePort = this.SysEnv.CommSetup.iBaseSocketPort;
            //    sysSock.DefSocketPort = this.SysEnv.CommSetup.iDefaultSocketPort;
            //    sysSock.MaxPorts = this.SysEnv.CommSetup.iMaxSocketPorts;
            //    sysSock.ConnTotalRetries = this.SysEnv.CommSetup.iConnectionAttempts;
            //    sysSock.SendTotalRetries = this.SysEnv.CommSetup.iSendAttempts;
            //    sysSock.RecvTotalRetries = this.SysEnv.CommSetup.iReceiveAttempts;
            //    sysSock.SendTimeOutMilliseconds = this.SysEnv.CommSetup.iSendTimeOutSeconds * 1000;
            //    sysSock.ReceiveTimeOutMilliseconds = this.SysEnv.CommSetup.iReceiveTimeOutSeconds * 1000;
            //    sysSock.RecvBuffSize = this.SysEnv.CommSetup.iiReceiveBufferSize;
            //    sysSock.ServerIP = Convert.ToString(this.SysEnv.CommSetup.iHostIPAddressComponent1.ToString() + "."
            //        + this.SysEnv.CommSetup.iHostIPAddressComponent2.ToString() + "."
            //        + this.SysEnv.CommSetup.iHostIPAddressComponent3.ToString() + "."
            //        + this.SysEnv.CommSetup.iHostIPAddressComponent4.ToString());
            //    try
            //    {
            //        System.Net.Sockets.Socket socket;
            //        socket = sysSock.Connect();

            // if (socket == null)
            //        {
            //            
            //            this.StartUpStatusOut(sender, e, "Connection failed.");
            //        }
            //        else
            //        {
            //            
            //            this.StartUpStatusOut(sender, e, "Requesting date & time.");

            // string sendRequest = this.CreateXMLSyncDateTime();
            //            int bytesSent = sysSock.SendRequest(socket, sendRequest);
            //            List<byte> recvData = sysSock.ReceiveResponse(socket);
            //            socket.Close();

            // string data = Compress.UnZip(recvData.ToArray());
            //            if (data.Contains("<message>ERROR</message>"))
            //            {
            //                XmlDocument docError = new XmlDocument();
            //                docError.LoadXml(data);

            // string errorDetail = docError.DocumentElement["detail"].InnerText;

            // 
            //                this.StartUpStatusOut(sender, e, "Request date & time error.");
            //                this.StartUpStatusOut(sender, e, errorDetail);
            //            }
            //            else
            //            {
            //                using (DataSet ds = new DataSet())
            //                {
            //                    ds.ReadXml(SysCommon.GetXmlReader(ref data), XmlReadMode.Auto);
            //                    if (ds.Tables.Count != 0)
            //                    {
            //                        //OpenNETCF.Runtime.InteropServices.SYSTEMTIME sysTime = new OpenNETCF.Runtime.InteropServices.SYSTEMTIME();
            //                        //OpenNETCF.Runtime.InteropServices.SYSTEMTIME sysTimeLocal = new OpenNETCF.Runtime.InteropServices.SYSTEMTIME();

            // foreach (DataRow drow in ds.Tables["tDateTime"].Rows)
            //                        {
            //                            //sysTime.wYear = Convert.ToInt16(drow["system_year"]);
            //                            //sysTime.wMonth = Convert.ToInt16(drow["system_month"]);
            //                            //sysTime.wDay = Convert.ToInt16(drow["system_day"]);
            //                            //sysTime.wHour = Convert.ToInt16(drow["system_hour"]);
            //                            //sysTime.wMinute = Convert.ToInt16(drow["system_minute"]);
            //                            //sysTime.wSecond = Convert.ToInt16(drow["system_second"]);
            //                            //sysTime.wDayOfWeek = Convert.ToInt16(drow["system_dayofweek"]);
            //                            //DateTimeHelper.SystemTime = sysTime.ToDateTime();

            // //sysTimeLocal.wYear = Convert.ToInt16(drow["local_year"]);
            //                            //sysTimeLocal.wMonth = Convert.ToInt16(drow["local_month"]);
            //                            //sysTimeLocal.wDay = Convert.ToInt16(drow["local_day"]);
            //                            //sysTimeLocal.wHour = Convert.ToInt16(drow["local_hour"]);
            //                            //sysTimeLocal.wMinute = Convert.ToInt16(drow["local_minute"]);
            //                            //sysTimeLocal.wSecond = Convert.ToInt16(drow["local_second"]);
            //                            //sysTime.wDayOfWeek = Convert.ToInt16(drow["local_dayofweek"]);
            //                            //DateTimeHelper.LocalTime = sysTimeLocal.ToDateTime();
            //                        }

            // this.StartUpStatusOut(sender, e, "Date & time synchronized.");
            //                        isDateTimeChanged = true;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (System.Net.Sockets.SocketException)
            //    {
            //        
            //        this.StartUpStatusOut(sender, e, "Socket exception occurred.");
            //        this.StartUpStatusOut(sender, e, "Sync date & time failed.");
            //    }
            // }
            return isDateTimeChanged;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string CreateXMLSyncDateTime()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var deviceID = SysDeviceID.GetDeviceID();

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TIME</action>"
                + "<device_id>" + deviceID + "</device_id>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task InitializeDatabase(object sender, EventArgs e)
        {
            var db = new DatabaseNub();

            this.SysEnv.RefreshOpt.IsDataCreatedInStartUp = false;

            db.RegNubListenerStatOut(this);

            this.StartUpStatusOut(sender, e, "Locating database.");

            if (File.Exists(DatabaseConnection.GetInstance().DBASCIIZ) && new FileInfo(DatabaseConnection.GetInstance().DBASCIIZ).Length > 0)
            {
                this.StartUpStatusOut(sender, e, "Database found.");
                this.StartUpStatusOut(sender, e, DatabaseConnection.GetInstance().DBASCIIZ);
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
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void InitializeScanner(object sender, EventArgs e)
        {
            this.StartUpStatusOut(sender, e, "Identifying bar code scanner.");

            var scanner = ScannerEngine.GetInstance().Scanner;

            if (scanner == null)
            {
                // MessageBox.Show(
                //    "Unable to identify bar code scanner.",
                //    this.GetControlDescription(),
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Exclamation,
                //    MessageBoxDefaultButton.Button1);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    _ = await this.DisplayAlert(this.GetControlDescription(), "Unable to identify bar code scanner.", "OK", "Cancel");
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void LoadProducts(object sender, EventArgs e)
        {
            this.StartUpStatusOut(sender, e, "Loading products.");

            ////Products productList = Products.GetInstance();
            ////productList.GetProductList();
            ////productList.GetProductUnitList();

            this.StartUpStatusOut(sender, e, "Loading products completed.");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="strMessage"></param>
        /// <remarks></remarks>
        private void StartUpStatusOut(object sender, EventArgs e, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.ExtraInfoItems.Add(message);
                this.listBoxStatus.ScrollTo(message, ScrollToPosition.MakeVisible, false);
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task LoadData(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;

                    this.StartUpStatusOut(sender, e, "Initializing environment.");

                    this.SysEnv = SysEnvironment.GetInstance();
                    this.SysEnv.IsCommunicationTestSuccessful = this.CommunicationTest(sender, e);

                    if (this.SysEnv.IsCommunicationTestSuccessful)
                    {
                        // Cannot do this
                        // this.SyncDateTime(sender, e);
                    }

                    this.InitializeScanner(sender, e);

                    await this.InitializeDatabase(sender, e);

                    if (!this.IsErrorRaised)
                    {
                        this.LoadProducts(sender, e);
                    }

                    if (!this.IsErrorRaised)
                    {
                        this.LoadLocationMap(sender, e);
                    }

                    this.DataIntegrityCheck();
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errReport = new FormErrorReport(SysCommon.ActSubTypes.SplashStartup, ex, this.GetControlDescription(), "timerStatus_Tick");
                        _ = await errReport.ShowDialog();
                        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    });
                }
                finally
                {
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CommunicationTest(object sender, EventArgs e)
        {
            var testSuccessful = false;

            if (!this.SysEnv.SysSettings.IsDisconnectedMode && (this.SysEnv.CommSetup.HostIPAddressComponent1 + this.SysEnv.CommSetup.HostIPAddressComponent2 + this.SysEnv.CommSetup.HostIPAddressComponent3) != 0)
            {
                this.StartUpStatusOut(sender, e, "Attempting communication test.");

                var sysSock = new SysSocket();

                sysSock.BasePort = this.SysEnv.CommSetup.BaseSocketPort;
                sysSock.DefSocketPort = this.SysEnv.CommSetup.DefaultSocketPort;
                sysSock.MaxPorts = this.SysEnv.CommSetup.MaxSocketPorts;
                sysSock.ConnTotalRetries = this.SysEnv.CommSetup.ConnectionAttempts;
                sysSock.SendTotalRetries = this.SysEnv.CommSetup.SendAttempts;
                sysSock.RecvTotalRetries = this.SysEnv.CommSetup.ReceiveAttempts;

                this.StartUpStatusOut(sender, e, string.Format("Base socket port: {0}", sysSock.BasePort));
                this.StartUpStatusOut(sender, e, string.Format("Default socket port: {0}", sysSock.DefSocketPort));
                this.StartUpStatusOut(sender, e, string.Format("Max socket port: {0}", sysSock.MaxPorts));
                this.StartUpStatusOut(sender, e, string.Format("Connection retries: {0}", sysSock.ConnTotalRetries));
                this.StartUpStatusOut(sender, e, string.Format("Socket send retries: {0}", sysSock.SendTotalRetries));
                this.StartUpStatusOut(sender, e, string.Format("Socket receive retries: {0}", sysSock.RecvTotalRetries));

                sysSock.SendTimeOutMilliseconds = this.SysEnv.CommSetup.SendTimeOutSeconds * 1000;

                this.StartUpStatusOut(sender, e, string.Format("Send time out: {0} seconds.", this.SysEnv.CommSetup.SendTimeOutSeconds));

                sysSock.ReceiveTimeOutMilliseconds = this.SysEnv.CommSetup.ReceiveTimeOutSeconds * 1000;

                this.StartUpStatusOut(sender, e, string.Format("Receive time out: {0} seconds.", this.SysEnv.CommSetup.ReceiveTimeOutSeconds));

                sysSock.RecvBuffSize = this.SysEnv.CommSetup.ReceiveBufferSize;

                this.StartUpStatusOut(sender, e, string.Format("Receive buffer size: {0} ", sysSock.RecvBuffSize));

                sysSock.ServerIP = this.SysEnv.CommSetup.HostIPAddressComponent1 + "."
                    + this.SysEnv.CommSetup.HostIPAddressComponent2 + "."
                    + this.SysEnv.CommSetup.HostIPAddressComponent3 + "."
                    + this.SysEnv.CommSetup.HostIPAddressComponent4;

                this.StartUpStatusOut(sender, e, string.Format("Server host IP: {0}", sysSock.ServerIP));
                this.StartUpStatusOut(sender, e, "Creating connection request.");

                try
                {
                    System.Net.Sockets.Socket socket;
                    socket = sysSock.Connect();

                    if (socket == null)
                    {
                        this.StartUpStatusOut(sender, e, "Connection failed.");
                    }
                    else
                    {
                        this.StartUpStatusOut(sender, e, "Sending device ID.");

                        var sendReq = CommunicationSetup.CreateXMLSubmitDeviceID();
                        var bytesSent = sysSock.SendRequest(socket, sendReq);

                        this.StartUpStatusOut(sender, e, string.Format("Bytes sent: {0}", bytesSent));
                        this.StartUpStatusOut(sender, e, "Waiting for response.");

                        var bytRecvData = sysSock.ReceiveResponse(socket);

                        socket.Disconnect(true);
                        socket.Close();

                        this.StartUpStatusOut(sender, e, "Socket closed.");

                        var recvData = Compress.UnZip(bytRecvData.ToArray());
                        if (recvData.Contains("<message>ERROR</message>"))
                        {
                            var docError = new XmlDocument();
                            docError.LoadXml(recvData);

                            var errorDetail = docError.DocumentElement["detail"].InnerText;

                            this.StartUpStatusOut(sender, e, "Submit device id error.");
                            this.StartUpStatusOut(sender, e, errorDetail);
                        }
                        else
                        {
                            this.StartUpStatusOut(sender, e, "Connection succeeded.");
                            testSuccessful = true;
                        }
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                    this.StartUpStatusOut(sender, e, "Socket exception occurred.");
                    this.StartUpStatusOut(sender, e, "Connection failed.");

                    // MessageBox.Show("Communication test failed.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await this.DisplayAlert(this.GetControlDescription(), "Communication test failed.", "OK");
                    });
                }
            }

            return testSuccessful;
        }

        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new NavigationPage(new LoginStartup()));
        }
    }
}
