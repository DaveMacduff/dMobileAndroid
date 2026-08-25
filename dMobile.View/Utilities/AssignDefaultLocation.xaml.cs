// <copyright file="AssignDefaultLocation.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Utilities
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Item;
    using CustomPages;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="AssignDefaultLocation" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssignDefaultLocation : DMobileContentPage, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AssignDefaultLocation" /> class.
        /// </summary>
        public AssignDefaultLocation()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdLocation.Text = "Set Location";
            this.cmdProduct.Text = "Select Product";
            this.lblCurrentLocation.Text = "Current Default Location(s)";
            this.lblDefaultPicking.Text = "Picking";
            this.lblDefaultReceiving.Text = "Receiving";
            this.lblCurrentReceiving.Text = "Receiving";
            this.lblCurrentReceivingLocation.Text = "Not Set";
            this.lblCurrentPicking.Text = "Picking";
            this.lblCurrentPickingLocation.Text = "Not Set";
            this.chkDefaultPicking.IsToggled = true;

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = ScannerEngine.GetInstance().Scanner;
                this.LocMapDefPicking = new SLocMap();
                this.LocMapDefRecv = new SLocMap();

                ParseAndSeedBarCodeModifier();

                this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.AssignDefaultLocation);
                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.AssignDefaultLocation);
                ////this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.AssignDefaultLocation);
                this.DataCollector = new DataCollector(this.ActSubType, this.ActItem);
            }

            return;

            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.Items.Clear();

                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    this.BarCodeModifier1Picker.Items.Add(string.Empty);
                    var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                    foreach (var item in modifiers)
                    {
                        this.BarCodeModifier1Picker.Items.Add(item);
                    }
                }
                else
                {
                    this.BarCodeModifier1Picker.IsVisible = false;
                    this.BarCodeModifier1Picker.IsEnabled = false;
                }

                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
            }
        }

        /// <summary>
        ///     Gets or sets the current receiving default location map reference number.
        /// </summary>
        private int CurrLocMapDefRecvRefno { get; set; }

        /// <summary>
        ///     Gets or sets the current picking default location map reference number.
        /// </summary>
        private int CurrLocMapDefPickingRefno { get; set; }

        /// <summary>
        ///     Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; }

        /// <summary>
        ///     Gets or sets the new receiving default location map reference number.
        /// </summary>
        private int NewLocMapDefRecvRefno { get; set; }

        /// <summary>
        ///     Gets or sets the new picking default location map reference number.
        /// </summary>
        private int NewLocMapDefPickingRefno { get; set; }

        /// <summary>
        ///     Gets or sets the default receiving location map.
        /// </summary>
        private SLocMap LocMapDefRecv { get; set; }

        /// <summary>
        ///     Gets or sets the default picking location map.
        /// </summary>
        private SLocMap LocMapDefPicking { get; set; }

        /// <summary>
        /// Gets or sets the ActDetail.
        /// </summary>
        ////private ActDetail ActDetail { get; set; } = null;

        /// <summary>
        ///     Gets or sets the activity item.
        /// </summary>
        private ActItem ActItem { get; }

        /// <summary>
        ///     Gets or sets the activity.
        /// </summary>
        private Activity Activity { get; }

        /// <summary>
        ///     Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether all controls have finished loading.
        /// </summary>
        private bool IsControlLoaded { get; set; }

        /// <summary>
        ///     Gets or sets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets or sets a value indicating whether the bar code handler has been registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the saved bar code scanner enabled status.
        /// </summary>
        private bool IsSvdBarScannerEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        private DataCollector DataCollector { get; set; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.ClearBarCodeScanEventHandlers();
                await this.Scanner.DisableScanning();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                await this.Scanner.EnableScanning();
                if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                {
                    this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                }

                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        ///     The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                await CheckLocationIsComplete();

                if (this.ActItem.ProdInfo.UnitRefno == 0)
                {
                    throw new MessageOnlyException("A selected product is required.");
                }

                isReadyToSave = await CheckSelectedPickingLocation(isReadyToSave);
                isReadyToSave = await CheckSelectedReceivingLocation(isReadyToSave);
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }

            return isReadyToSave;

            async Task CheckLocationIsComplete()
            {
                if (!await LocationMapTasks.IsCompleteLocMap(this.ActItem.LocMapRefno))
                {
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                    {
                        throw new MessageOnlyException("Warehouse is required.");
                    }

                    if (this.LocMap.CurrLocMap.IsSectionTracked && this.LocMap.CurrLocMap.SectLocMapRefno <= 0)
                    {
                        throw new MessageOnlyException("Section is required.");
                    }

                    if (this.LocMap.CurrLocMap.IsSlotTracked && this.LocMap.CurrLocMap.SlotLocMapRefno <= 0)
                    {
                        throw new MessageOnlyException("Slot is required.");
                    }
                }
            }

            async Task<bool> CheckSelectedPickingLocation(bool isReadyToSave)
            {
                if (!(this.chkDefaultPicking.IsToggled || this.chkDefaultReceiving.IsToggled))
                {
                    throw new MessageOnlyException("A selected default picking or receiving option is required.");
                }

                if (isReadyToSave && this.chkDefaultPicking.IsToggled &&
                    this.CurrLocMapDefPickingRefno == this.NewLocMapDefPickingRefno)
                {
                    var msg = "The current and the new default picking locations are the same." +
                              Environment.NewLine + "Are you sure that you want to update the location anyway?";

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var result = await this.DisplayAlert("Ready To Save", msg, "Yes", "No");

                        if (!result)
                        {
                            isReadyToSave = false;
                        }
                    });
                }

                return isReadyToSave;
            }

            async Task<bool> CheckSelectedReceivingLocation(bool isReadyToSave)
            {
                if (isReadyToSave && this.chkDefaultReceiving.IsToggled &&
                    this.CurrLocMapDefRecvRefno == this.NewLocMapDefRecvRefno)
                {
                    var msg = "The current and the new default receiving locations are the same." +
                              Environment.NewLine + "Are you sure that you want to update the location anyway?";

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var result = await this.DisplayAlert("Ready To Save", msg, "Yes", "No");

                        if (!result)
                        {
                            isReadyToSave = false;
                        }
                    });
                }

                return isReadyToSave;
            }
        }

        /// <summary>
        ///     The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToClose()
        {
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initialize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            await this.ScannerDisable();

            return !this.IsChangesMade;
        }

        /// <summary>
        ///     The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
                cancelChanges = false;
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///     The GetXml.
        /// </summary>
        /// <returns>.</returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        ///     The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.IsChangesMade = false;

                    var sendRequest = string.Empty;
                    var data = string.Empty;
                    var xmlString = new StringBuilder();
                    var settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = false
                    };
                    var xml = XmlWriter.Create(xmlString, settings);

                    XDocument request = null;

                    if (this.NewLocMapDefPickingRefno == 0)
                    {
                        this.LocMapDefPicking = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            this.CurrLocMapDefPickingRefno);
                    }
                    else
                    {
                        this.LocMapDefPicking = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            this.NewLocMapDefPickingRefno);
                    }

                    if (this.NewLocMapDefRecvRefno == 0)
                    {
                        this.LocMapDefRecv =
                            await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefRecvRefno);
                    }
                    else
                    {
                        this.LocMapDefRecv =
                            await LocationMapTasks.GetSLocMapFromLocMapRefno(this.NewLocMapDefRecvRefno);
                    }

                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT ASSIGN DEFAULT PRODUCT LOCATION</action>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "</command>"
                        + "<receiving>"
                        + "<prod_refno>" + this.ActItem.ProdInfo.FkRefno + "</prod_refno>"
                        + "<is_update>" + this.chkDefaultReceiving.IsToggled + "</is_update>"
                        + "<whs_id>" + this.LocMapDefRecv.WhsFkRefno + "</whs_id>"
                        + "<section>" + this.LocMapDefRecv.SectName + "</section>"
                        + "<slot>" + this.LocMapDefRecv.SlotName + "</slot>"
                        + "</receiving>"
                        + "<picking>"
                        + "<prod_refno>" + this.ActItem.ProdInfo.FkRefno + "</prod_refno>"
                        + "<is_update>" + this.chkDefaultPicking.IsToggled + "</is_update>"
                        + "<whs_id>" + this.LocMapDefPicking.WhsFkRefno + "</whs_id>"
                        + "<section>" + this.LocMapDefPicking.SectName + "</section>"
                        + "<slot>" + this.LocMapDefPicking.SlotName + "</slot>"
                        + "</picking>"
                        + "</imclient>");

                    request.Save(xml);
                    await xml.FlushAsync();
                    xml.Close();

                    sendRequest = xmlString.ToString();

                    // Submit the xml request
                    data = SysCommon.SubmitComm(sendRequest);
                    var origLocMapDefPickingRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        this.LocMap.CurrLocMap.WhsLocMapRefno,
                        this.ActItem.ProdInfo.Refno,
                        SysCommon.ProdLocMapTypes.Picking);
                    var origLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        this.LocMap.CurrLocMap.WhsLocMapRefno,
                        this.ActItem.ProdInfo.Refno,
                        SysCommon.ProdLocMapTypes.Receiving);

                    var dataUpdate = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<param>"
                        + "<prod_refno>" + this.ActItem.ProdInfo.Refno + "</prod_refno>"
                        + "<receiving_is_update>" + this.chkDefaultReceiving.IsToggled + "</receiving_is_update>"
                        + "<receiving_warehouse_location_refno>" + this.LocMapDefRecv.WhsLocMapRefno
                        + "</receiving_warehouse_location_refno>"
                        + "<receiving_old_location_refno>" + origLocMapDefRecvRefno + "</receiving_old_location_refno>"
                        + "<receiving_new_location_refno>" + this.NewLocMapDefRecvRefno +
                        "</receiving_new_location_refno>"
                        + "<picking_is_update>" + this.chkDefaultPicking.IsToggled + "</picking_is_update>"
                        + "<picking_warehouse_location_refno>" + this.LocMapDefPicking.WhsLocMapRefno
                        + "</picking_warehouse_location_refno>"
                        + "<picking_old_location_refno>" + origLocMapDefPickingRefno + "</picking_old_location_refno>"
                        + "<picking_new_location_refno>" + this.NewLocMapDefPickingRefno +
                        "</picking_new_location_refno>"
                        + "</param>");

                    await this.ActItem.Update(dataUpdate);

                    this.CurrLocMapDefPickingRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        this.LocMap.CurrLocMap.WhsLocMapRefno,
                        this.ActItem.ProdInfo.Refno,
                        SysCommon.ProdLocMapTypes.Picking);
                    this.LocMapDefPicking = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        this.CurrLocMapDefPickingRefno);
                    this.CurrLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        this.LocMap.CurrLocMap.WhsLocMapRefno,
                        this.ActItem.ProdInfo.Refno,
                        SysCommon.ProdLocMapTypes.Receiving);
                    this.LocMapDefRecv = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        this.CurrLocMapDefRecvRefno);

                    await this.DisplayData();
                    this.lblCurrentPickingLocation.TextColor = Color.Black;
                    this.lblCurrentReceivingLocation.TextColor = Color.Black;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Assign Default Location",
                        "Product default location(s) saved.",
                        "OK"));

                    saveComplete = false; // Set false to stay in form
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///     The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Assign Default Location";
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Note: Do we want to disable the scanner here. It may not even be enabled before at all.
            // It is my understanding that the OnAppearing event only occurs once when the control is loading.
            await this.ScannerDisable();

            SubscribeKeyUpHandlers();

            this.IsIndicatorRunning = true;

            try
            {
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            this.LocMap.LoginWhsLocMapRefno);
                    }

                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text =
                        LocationMapTasks.GetCurrentReadableLocation(true));
                }

                this.IsControlLoaded = true;
                _ = this.cmdLocation.Focus();
                await this.ScannerEnable();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                this.IsFirstAppearance = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsFirstAppearance = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = "This activity unexpectedly failed to initialize correctly." +
                              " Please exit and try again.";
                    var errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.IsIndicatorRunning = false;
                await this.ScannerEnable();
                await Task.Run(() =>
                {
                    // Explicit GC, Xamarin forms not disposing of objects properly
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                });
            }

            return;

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
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
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool" />.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                    await this.ScannerDisable();
                    if (await this.CancelChanges())
                    {
                        _ = base.OnBackButtonPressed();
                        await this.Close();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    if (this.IsSvdBarScannerEnabled)
                    {
                        await this.ScannerEnable();
                    }
                }
            });

            return true;
        }

        /// <summary>
        ///     Check if changes have been made.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;

                this.SysEnv.CurrentBarCodeModifier1 = this.BarCodeModifier1Picker.SelectedItem.ToString();

                if (this.chkDefaultPicking.IsToggled)
                {
                    this.NewLocMapDefPickingRefno = this.LocMap.CurrLocMapRefno;
                }
                else
                {
                    this.NewLocMapDefPickingRefno = this.CurrLocMapDefPickingRefno;
                }

                if (this.chkDefaultReceiving.IsToggled)
                {
                    this.NewLocMapDefRecvRefno = this.LocMap.CurrLocMapRefno;
                }
                else
                {
                    this.NewLocMapDefRecvRefno = this.CurrLocMapDefRecvRefno;
                }
            }
        }

        /// <summary>
        ///     Command location click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdLocation_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            var form = new FormDialogContainer(
                EDialogTypes.SelectLocationMap,
                SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                if (this.LocMap.CurrLocMap.WhsLocMapRefno == 0)
                {
                    throw new Exception("Unexpected result this.LocMap.CurrLocMap.WhsLocMapRefno == 0.");
                }

                this.ActItem.LocMap = this.LocMap.CurrLocMap;

                if (this.chkDefaultPicking.IsToggled)
                {
                    this.NewLocMapDefPickingRefno = this.LocMap.CurrLocMapRefno;
                }
                else
                {
                    this.NewLocMapDefPickingRefno = this.CurrLocMapDefPickingRefno;
                }

                if (this.chkDefaultReceiving.IsToggled)
                {
                    this.NewLocMapDefRecvRefno = this.LocMap.CurrLocMapRefno;
                }
                else
                {
                    this.NewLocMapDefRecvRefno = this.CurrLocMapDefRecvRefno;
                }
            }

            await this.DisplayData();
            await this.ScannerEnable();
        }

        /// <summary>
        ///     Command select product click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdProduct_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            var form = new FormDialogContainer(
                EDialogTypes.SelectProduct,
                SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                var xmlDoc = await form.GetXml();
                var prodRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductRefno").Value);

                if (prodRefno == 0)
                {
                    this.CurrLocMapDefPickingRefno = 0;
                    this.CurrLocMapDefRecvRefno = 0;
                }

                if (prodRefno != 0 && this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(prodRefno);
                    this.CurrLocMapDefPickingRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        this.LocMap.CurrLocMap.WhsLocMapRefno,
                        this.ActItem.ProdInfo.Refno,
                        SysCommon.ProdLocMapTypes.Picking);
                    this.CurrLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        this.LocMap.CurrLocMap.WhsLocMapRefno,
                        this.ActItem.ProdInfo.Refno,
                        SysCommon.ProdLocMapTypes.Receiving);

                    // Assigning the new location map will only be done by the set location button.
                }

                await this.DisplayData();
            }

            await this.ScannerEnable();
        }

        /// <summary>
        ///     The BarCodeHandler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs">The scanArgs<see cref="BarCodeScannerEventArgs" />.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.IsIndicatorRunning || scanArgs == null)
            {
                return;
            }

            var prodUnitRefno = 0;
            BarCodeParse parse = null;

            try
            {
                this.IsIndicatorRunning = true;
                await this.ScannerDisable();

                parse = scanArgs.ParseInfo;

                // Global location identifier scanned
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        parse.LocationIdentifier);

                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno == 0)
                    {
                        throw new Exception("Unexpected result this.LocMap.CurrLocMap.WhsLocMapRefno == 0.");
                    }

                    this.ActItem.LocMap = this.LocMap.CurrLocMap;

                    if (this.chkDefaultPicking.IsToggled)
                    {
                        this.NewLocMapDefPickingRefno = this.LocMap.CurrLocMapRefno;
                    }
                    else
                    {
                        this.NewLocMapDefPickingRefno = this.CurrLocMapDefPickingRefno;
                    }

                    if (this.chkDefaultReceiving.IsToggled)
                    {
                        this.NewLocMapDefRecvRefno = this.LocMap.CurrLocMapRefno;
                    }
                    else
                    {
                        this.NewLocMapDefRecvRefno = this.CurrLocMapDefRecvRefno;
                    }

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayData());
                }
                else if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                {
                    prodUnitRefno = await this.DataCollector.IdentifyProductScanned(parse);
                    if (prodUnitRefno == 0)
                    {
                        this.CurrLocMapDefPickingRefno = 0;
                        this.CurrLocMapDefRecvRefno = 0;
                    }
                    else
                    {
                        this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            this.CurrLocMapDefPickingRefno =
                                await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                    this.LocMap.CurrLocMap.WhsLocMapRefno,
                                    this.ActItem.ProdInfo.Refno,
                                    SysCommon.ProdLocMapTypes.Picking);
                            this.CurrLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                this.LocMap.CurrLocMap.WhsLocMapRefno,
                                this.ActItem.ProdInfo.Refno,
                                SysCommon.ProdLocMapTypes.Receiving);
                        }
                    }

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayData());
                }
                else if (!string.IsNullOrEmpty(parse.ActItemFkRefno))
                {
                    throw new MessageOnlyException("Scanning a line item bar code is not supported here!");
                }
                else if (!string.IsNullOrEmpty(parse.ActDetailFkRefno))
                {
                    throw new MessageOnlyException("Scanning a detail bar code is not supported here!");
                }
                else if (!string.IsNullOrEmpty(parse.Sscc18))
                {
                    throw new MessageOnlyException("Scanning an shipping container bar code is not supported here!");
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     Activity default location display data.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task DisplayData()
        {
            if (this.ActItem.ProdInfo.UnitRefno != 0)
            {
                this.cmdProduct.Text = "Product " + this.ActItem.ProdInfo.Code;
                this.lblProdDesc.Text = this.ActItem.ProdInfo.Desc;
            }
            else
            {
                this.cmdProduct.Text = "Select Product";
                this.lblProdDesc.Text = string.Empty;
            }

            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation(true);
            }
            else
            {
                this.cmdLocation.Text = "Set Location";
            }

            var locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefPickingRefno);
            this.lblCurrentPickingLocation.Text = LocationMapTasks.GetReadableLocation(locMap, true);

            if (string.IsNullOrEmpty(this.lblCurrentPickingLocation.Text))
            {
                this.lblCurrentPickingLocation.Text = "Not Set";
            }

            locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefRecvRefno);
            this.lblCurrentReceivingLocation.Text = LocationMapTasks.GetReadableLocation(locMap, true);

            if (string.IsNullOrEmpty(this.lblCurrentReceivingLocation.Text))
            {
                this.lblCurrentReceivingLocation.Text = "Not Set";
            }
        }

        /// <summary>
        ///     The CmdSave_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
        private async void CmdSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }
    }
}