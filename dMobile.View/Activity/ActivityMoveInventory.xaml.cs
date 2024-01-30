// <copyright file="ActivityMoveInventory.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller;
    using dMobile.Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using Resources.Enums;
    using Resources.Types;
    using CustomPages;
    using Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="ActivityMoveInventory" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityMoveInventory : DMobileContentPage, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityMoveInventory"/> class.
        /// </summary>
        public ActivityMoveInventory()
        {
            this.InitializeComponent();
            this.IsIndicatorRunning = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityMoveInventory"/> class.
        /// </summary>
        /// <param name="actSubType">.</param>
        public ActivityMoveInventory(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.LblScanToLocation.Text = "Scan To Loc.";
            this.CmdFromLocation.Text = "Set From Location";
            this.CmdAdd.Text = "Add";
            this.CmdToLocation.Text = "Set To Location";
            this.CmdPerformMove.Text = "Perform Move";

            this.MainActivityItems = new ObservableCollection<object>();
            this.ExtraInfoItems = new ObservableCollection<string>();
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SelFromLocMap = new SLocMap();
            this.SelToLocMap = new SLocMap();
            this.Scanner = ScannerEngine.GetInstance().Scanner;

            // Setup the activities.
            this.ActSubType = actSubType;
            this.Activity = ActivityFacade.GetActivity(actSubType);
            this.ActItem = ActItemFacade.GetActItem(this.Activity, actSubType);
            this.ActItem.ActHdr = this.Activity;
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, actSubType);
            this.ActDetail.ActItem = this.ActItem;
            this.ActDetail.ActExtra = new ActDetailExtra();

            CreateOptionsMenu();
            ParseAndSeedBarCodeModifier();
            return;

            //////////////////////////////////////////////////////////////////////////
            void CreateOptionsMenu()
            {
                this.OptionsMenuItems.Clear();
                this.OptionsMenuItems.Add("Save");
                this.OptionsMenuItems.Add("Cancel");
            }

            //////////////////////////////////////////////////////////////////////////
            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
                this.BarCodeModifier1Picker.Items.Clear();

                // ReSharper disable once InvertIf
                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    this.BarCodeModifier1Picker.Items.Add(string.Empty);
                    var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');
                    foreach (var item in modifiers)
                    {
                        this.BarCodeModifier1Picker.Items.Add(item);
                    }

                    if (this.BarCodeModifier1Picker.Items.Count > 0)
                    {
                        this.BarCodeModifier1Picker.IsVisible = true;
                        this.BarCodeModifier1Picker.IsEnabled = true;
                    }

                    this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
                }
            }
        }

        /// <summary>
        /// Gets the option menu items list.
        /// </summary>
        private List<string> OptionsMenuItems { get; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Gets or sets the selected activity detail foreign key reference number.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private int SelectedActivityDetailFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the Scanner.
        /// </summary>
        private BarCodeScanner Scanner { get; set; }

        /// <summary>
        /// Gets or sets the SelToCurrLocMapRefno.
        /// </summary>
        private int SelToCurrLocMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the SelFromCurrLocMapRefno.
        /// </summary>
        private int SelFromCurrLocMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the ActDetail.
        /// </summary>
        private ActDetail ActDetail { get; set; }

        /// <summary>
        /// Gets or sets the ActItem.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the Activity.
        /// </summary>
        private Activity Activity { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.MoveInventory;

        /// <summary>
        /// Gets or sets the LocMap.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        /// Gets or sets the SelToLocMap.
        /// </summary>
        private SLocMap SelToLocMap { get; set; }

        /// <summary>
        /// Gets or sets the SelFromLocMap.
        /// </summary>
        private SLocMap SelFromLocMap { get; set; }

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsBarCodeHandlerRegistered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsControlLoaded.
        /// </summary>
        private bool IsControlLoaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsChangesMade.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the BarCodeParse.
        /// </summary>
        private BarCodeParse BarCodeParse { get; set; }

        /// <summary>
        /// Gets or sets the model bound to view, activity items extra information. Changes made will update the UI.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; }

        /// <summary>
        /// Gets or sets the model bound to view, activity items list view. Changes made will update the UI.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; }

        /// <summary>
        /// Un-register the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                if (this.IsBarCodeHandlerRegistered)
                {
                    this.Scanner.ClearBarCodeScanEventHandlers();
                    await this.Scanner.DisableScanning();
                    this.IsBarCodeHandlerRegistered = false;
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                if (!this.IsBarCodeHandlerRegistered)
                {
                    await this.Scanner.EnableScanning();
                    if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                    {
                        this.Scanner.BarCodeScan += this.BarCodeHandler;
                    }

                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;

            // If slot tracked validate full slot quantity move and non full slot.
            if (this.ActDetail.ListDetails.Count > 0 || this.SelFromLocMap.IsSlotTracked)
            {
                if (!await LocationMapTasks.IsCompleteLocMap(this.SelFromLocMap))
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Send Moves",
                        "Invalid Move \'From\' Location",
                        "OK"));
                }
                else if (!await LocationMapTasks.IsCompleteLocMap(this.SelToLocMap))
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Send Moves",
                        "Invalid Move \'To\' Location",
                        "OK"));
                }
                else if (this.SelFromLocMap.WhsLocMapRefno != this.SelToLocMap.WhsLocMapRefno)
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Send Moves",
                        "The \'From\' And \'To\' Warehouse(s) must be the same.",
                        "OK"));
                }
                else if (this.SelFromCurrLocMapRefno == this.SelToCurrLocMapRefno)
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Send Moves",
                        "The \'From\' And \'To\' Location must not be the same.",
                        "OK"));
                }
            }

            return isReadyToSave;
        }

        /// <summary>
        /// The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = false;

            try
            {
                if (!await this.IsReadyToSave())
                {
                    throw new MessageOnlyException("Data Failed Validation. Check Your Data.");
                }

                if (this.ActDetail.ListDetails != null && this.ActDetail.ListDetails.Count > 0)
                {
                    await PerformMoves();
                    if (this.ActDetail.ListDetails.Count > 0)
                    {
                        isSaveComplete = false;
                    }
                    else
                    {
                        if (!await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                "Move Submission",
                                "Continue performing moves?",
                                "Yes",
                                "No")))
                        {
                            isSaveComplete = true;
                        }
                    }
                }
                else
                {
                    if (this.LocMap.CurrLocMap.IsSlotTracked)
                    {
                        throw new MessageOnlyException("Please add some detail to move.");
                    }

                    if (this.SysEnv.SysSettings.IsDisableMoveAllSlotQuantityOption)
                    {
                        throw new MessageOnlyException("Moving all slot contents is currently not enabled.");
                    }

                    if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Move Submission",
                        "Do you wish to move ALL of the slot contents?",
                        "Yes",
                        "No")))
                    {
                        await PerformMoveAll();
                        if (!await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                "Move Submission",
                                "Continue performing moves?",
                                "Yes",
                                "No")))
                        {
                            isSaveComplete = true;
                        }
                    }
                }
            }
            catch (CancelOperationException)
            {
                isSaveComplete = false;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message
                    : msgEx.Message["MSGBOX-".Length..];
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            //////////////////////////////////////////////////////////////////////////
            async Task PerformMoveAll()
            {
                try
                {
                    this.ActItem.ItemFkRefno = 0;
                    this.ActItem.ItemRefno = 0;
                    var xmlParam = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                        "<param>" + "<whs_id>" + this.SelFromLocMap.WhsFKRefno.ToString() +
                        "</whs_id>" + "<sect_refno>" + this.SelFromLocMap.SectFKRefno.ToString() +
                        "</sect_refno>" + "<slot_refno>" + this.SelFromLocMap.SlotFKRefno.ToString() +
                        "</slot_refno>" + "<to_sect_refno>" + this.SelToLocMap.SectFKRefno.ToString() +
                        "</to_sect_refno>" + "<to_slot_refno>" + this.SelToLocMap.SlotFKRefno.ToString() +
                        "</to_slot_refno>" + "</param>");

                    await this.ActDetail.Submit(null, xmlParam);

                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            "Move completed.",
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    var result = await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            "Move Submission",
                            Convert.ToString("Submit Move Failed" + Environment.NewLine + ex.Message),
                            "Cancel",
                            "Continue"));
                    if (result)
                    {
                        isSaveComplete = false;
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task PerformMoves()
            {
                for (var idx = 0; idx < this.ActDetail.ListDetails.Count; idx++)
                {
                    try
                    {
                        if (((MoveDetail)this.ActDetail.ListDetails[idx]).ActivityDetailStatus !=
                            (int)SysCommon.ActStatus.NoChange)
                        {
                            this.ActItem.ItemFkRefno = 0;
                            this.ActItem.ItemRefno = 0;
                            var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(
                                this.ActDetail.ListDetails[idx].GetType().GetProperty(
                                    "ProductUnitRefno")?.GetValue(this.ActDetail.ListDetails[idx])));
                            prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);
                            this.ActItem.ProdInfo = prodInfo;

                            var detail = (Resources.Types.ActivityDetail)this.ActDetail.ListDetails[idx];
                            await this.ActDetail.AssignActivityDetail(detail);
                            var xmlParam = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                "<param>" + "<whs_id>" + this.SelFromLocMap.WhsFKRefno.ToString() +
                                "</whs_id>" + "<sect_refno>" + this.SelFromLocMap.SectFKRefno.ToString() +
                                "</sect_refno>" + "<slot_refno>" + this.SelFromLocMap.SlotFKRefno.ToString() +
                                "</slot_refno>" + "<to_sect_refno>" + this.SelToLocMap.SectFKRefno.ToString() +
                                "</to_sect_refno>" + "<to_slot_refno>" + this.SelToLocMap.SlotFKRefno.ToString() +
                                "</to_slot_refno>" + "</param>");

                            await this.ActDetail.Submit(this.ActDetail.ListDetails[idx], xmlParam);

                            ////((MoveDetail)this.ActDetail.ListDetails[idx]).ActivityDetailStatus = (int)SysCommon.ActStatus.NoChange;
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    "Move completed.",
                                    Resource.OK));
                        }
                    }
                    catch (Exception ex)
                    {
                        var result = await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(
                                "Move Submission",
                                Convert.ToString("Submit Move Failed" + Environment.NewLine + ex.Message),
                                "Cancel",
                                "Continue"));
                        if (result)
                        {
                            isSaveComplete = false;
                            break;
                        }
                    }
                }
            }

            return isSaveComplete;
        }

        /// <summary>
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    // Expect the control to close.
                }
                else
                {
                    cancelChanges = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.MoveInventory, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// The GetXML.
        /// </summary>
        /// <returns>.</returns>
#pragma warning disable CS1998
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998
        {
            var xmlDoc =
                XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>");
            return xmlDoc;
        }

        /// <summary>
        /// The OnLayerStatOut.
        /// </summary>
        /// <param name="publisher">.</param>
        /// <param name="dataLayerEventArgs"> Data layer event arguments.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            this.ExtraInfoItems.Clear();

            // Tokenize data longer than 30 characters for display, wrap to next line
            const int tokenLength = 30;

            if (dataLayerEventArgs.StatusOutMsg.Length > tokenLength)
            {
                var iTokens = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dataLayerEventArgs.StatusOutMsg.Length / tokenLength)));
                for (var iTokenIdx = 0; iTokenIdx <= (iTokens * tokenLength) - 1; iTokenIdx++)
                {
                    var iTokenLength = Convert.ToInt32(dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx > tokenLength
                        ? tokenLength : dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx);
                    this.ExtraInfoItems.Add(dataLayerEventArgs.StatusOutMsg.Substring(Convert.ToInt32(iTokenIdx), iTokenLength));
                    iTokenIdx += tokenLength;
                }
            }
            else
            {
                this.ExtraInfoItems.Add(dataLayerEventArgs.StatusOutMsg);
            }
        }

        /// <summary>
        /// The OnLayerProgressOut.
        /// </summary>
        /// <param name="publisher">.</param>
        /// <param name="dataLayerEventArgs"> Data layer event arguments.</param>
        public void OnLayerProgressOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                base.OnAppearing();
                try
                {
                    this.IsIndicatorRunning = true;
                    if (this.IsFirstAppearance)
                    {
                        this.IsFirstAppearance = false;
                        this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

                        await ScannerStart();

                        if (this.MainActivityItems == null || this.MainActivityItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => this.ExtraInfoItems.Clear());
                        }
                        else
                        {
                            await this.ActAppearing();
                        }
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
                catch (CommunicationException commEx)
                {
                    this.IsFirstAppearance = true;

                    // Question should a communication error, be reported as an error or just a messagebox?
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load activity moves.{Environment.NewLine}" +
                            $"Because of a communication problem.{Environment.NewLine}" +
                            $"Detail:{commEx.Message}{Environment.NewLine}" +
                            "Please exit and try again.";

                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK);
                    });
                }
                catch (Exception ex)
                {
                    this.IsFirstAppearance = true;
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load activity moves.{Environment.NewLine}" +
                            $"General exception.{Environment.NewLine}" +
                            $"Detail:{ex.Message}{Environment.NewLine}" +
                            "Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }

                try
                {
                    await Task.Run(async () => await this.ActivityMoveInventory_GotFocus());
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Error",
                        "this.ActivityMoveInventory_GotFocus threw an error " + ex,
                        "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task ScannerStart()
            {
                if (this.Scanner != null)
                {
                    await this.Scanner.EnableScanning();
                    if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                    {
                        this.Scanner.BarCodeScan += this.BarCodeHandler;
                    }

                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Back button pressed handler, performs cancel.
        /// </summary>
        /// <returns>The <see cref="bool"/> True if cancel successful else false.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(PerformClose);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void PerformClose()
            {
                _ = base.OnBackButtonPressed();
                if (await this.CancelChanges())
                {
                    await this.ActDisappearing();
                    await this.Close();
                }
            }
        }

        /// <summary>
        /// Data grid activity detail selected detail event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated the event.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs"/>.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActDetail.AssignActivityDetail(e.SelectedItem);
            this.ActDetail.SelectedListIdx = e.SelectedItemIndex;
            this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
        }

        /// <summary>
        /// Find the selected item in the data list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActDetail.ListDetails.Count > 0)
            {
                // I am thinking this iteration will be fast even with 100+ records.
                // We are not triggering any UI refreshing, so it should be.
                this.ActDetail.SelectedListIdx = -1;
                object selectedItem = null;
                if (this.SelectedActivityDetailFKRefno != 0)
                {
                    for (var index = 0; index < this.ActDetail.ListDetails.Count; index++)
                    {
                        var item = this.ActDetail.ListDetails[index];
                        if (this.SelectedActivityDetailFKRefno == this.ActDetail.GetFkRefno(item))
                        {
                            selectedItem = item;
                            this.ActDetail.SelectedListIdx++;
                            break;
                        }
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the last one in the list.
                    this.ActDetail.SelectedListIdx = this.ActDetail.ListDetails.Count - 1;
                    selectedItem = this.ActDetail.ListDetails[this.ActDetail.SelectedListIdx];
                    await this.ActDetail.AssignActivityDetail(selectedItem);
                    this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
                }
                else
                {
                    await this.ActDetail.AssignActivityDetail(selectedItem);
                    this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = selectedItem;
                    this.ListViewMainActivity.ScrollTo(selectedItem, ScrollToPosition.MakeVisible, false);
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActDetail.SelectedListIdx = -1;
                this.ActDetail.SelectedDetail = null;
                this.ActDetail.DetailFkRefno = 0;
                this.ActDetail.DetailRefno = 0;
                this.SelectedActivityDetailFKRefno = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = null;
                });
            }
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        private string GetControlDescription()
        {
            return "Move Inventory";
        }

        /// <summary>
        /// Subscribe key up handlers, other events and find the selected item using this.SelectedActivityItemFKRefno.
        /// </summary>
        private async Task ActAppearing()
        {
            await this.ScannerEnable();
            SubscribeKeyUpHandlers();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

            await this.FindSelectedItemAsync();
            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    KeyUpCallback);
                return;

                //////////////////////////////////////////////////////////////////////////
                async void KeyUpCallback(string keyPressed)
                {
                    ////var svdIsScanReg = this.IsBarCodeHandlerRegistered;
                    await this.ScannerDisable();

                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    await KeyUpHandlerDelKey();
                    return;

                    //////////////////////////////////////////////////////////////////////////
                    async Task KeyUpHandlerDelKey()
                    {
                        if (keycode == Keycode.Del || keycode == Keycode.Clear || keycode == Keycode.Back)
                        {
                            var selectedIdx = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);
                            if (this.ListViewMainActivity.SelectedItem != null && selectedIdx > -1)
                            {
                                if (await Device.InvokeOnMainThreadAsync(async () =>
                                        await this.DisplayAlert(
                                            this.GetControlDescription(),
                                            "Do you wish to delete this record?",
                                            "Yes",
                                            "No")))
                                {
                                    this.ActDetail.ListDetails.RemoveAt(selectedIdx);
                                    this.MainActivityItems.RemoveAt(selectedIdx);
                                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribe key up handlers, turn off scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task ActDisappearing()
        {
            await this.ScannerDisable();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        /// <summary>
        /// From location click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdFromLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    return;
                }

                this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                this.SelFromLocMap = this.LocMap.CurrLocMap.Clone();
                this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                // Display the selected from location.
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdFromLocation.Text = "From: " +
                        LocationMapTasks.GetCurrentReadableLocation());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdFromLocation.Text = "Set From Location");
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "Error selecting to location." + Environment.NewLine + ex.Message,
                    "OK"));
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// Command to location click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdToLocation_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsIndicatorRunning = true;
                await this.ScannerDisable();

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                if (await form.DisplayControl.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                this.SelToLocMap = this.LocMap.CurrLocMap.Clone();
                this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdToLocation.Text = "To: " +
                        LocationMapTasks.GetCurrentReadableLocation());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdToLocation.Text = "Set To Location");
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "Error selecting to location." + Environment.NewLine + ex.Message,
                    "OK"));
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// The ActivityMoveInventory_GotFocus.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task ActivityMoveInventory_GotFocus()
        {
            _ = await Device.InvokeOnMainThreadAsync(() =>
                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);

            if (this.IsControlLoaded == false)
            {
                await SetFromLocationInfo();
                await SetToLocationInfo();
                await this.RefreshData();
                this.IsControlLoaded = true;

                //////////////////////////////////////////////////////////////////////////
                async Task SetFromLocationInfo()
                {
                    this.SelFromLocMap = this.LocMap.CurrLocMap.Clone();
                    this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.CmdFromLocation.Text = "From: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.CmdFromLocation.Text = "Set From Location");
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task SetToLocationInfo()
                {
                    this.SelToLocMap = this.LocMap.CurrLocMap.Clone();
                    this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.CmdToLocation.Text = "To: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.CmdToLocation.Text = "Set To Location");
                    }
                }
            }
        }

        /// <summary>
        /// Add click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ActDisappearing();

                //--- Perform validations to see if add can be performed.
                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMap))
                {
                    throw new MessageOnlyException("Location must be set first.");
                }

                //--- Get the product information to insert.
                var prodInfo = await GetProductInfo();

                //--- Create the activity item using the location and product information.
                await this.ActItem.Insert(prodInfo);

                //--- Read and assign the activity detail from the first existing detail found.
                var moveDetail = (MoveDetail)this.ActDetail.ListDetails.FirstOrDefault() ?? new MoveDetail();
                moveDetail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
                this.ActDetail.ClearProperties();

                // --- Gather the activity detail to insert.
                var isInsertReady = await GatherActivityDetail();

                //--- Perform a few more gathered detail validations.
                // Scheme data has already been validated.
                // Moves could be used to fix quantities if we allowed less than or equal to zero values.
                // This moves module will not be used for this purpose. Rather counts should be used to reconcile quantity.
                if (this.ActDetail.Quantity <= 0 || this.ActDetail.BaseQuantity <= 0 ||
                    this.ActDetail.WeightQuantity <= 0)
                {
                    throw new MessageOnlyException("Move quantity must be greater than zero.");
                }

                await this.InsertActivityDetail(this.ActDetail, isInsertReady);
            }
            catch (CancelOperationException)
            {
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errorReport = new FormErrorReport(this.ActSubType, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message
                    : msgEx.Message["MSGBOX-".Length..];
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ActAppearing();
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> GatherActivityDetail()
            {
                //--- Gather new activity detail to be inserted.
                this.ActDetail.Quantity = 0;
                this.ActDetail.BaseQuantity = 0;
                this.ActDetail.WeightQuantity = 0;
                this.ActDetail.SchemeId = string.Empty;
                this.ActDetail.UniqueSchemeId = string.Empty;
                this.ActDetail.ActExtra.Text1 = string.Empty;
                this.ActDetail.ActExtra.Text2 = string.Empty;
                this.ActDetail.ActExtra.Text3 = string.Empty;
                this.ActDetail.ActExtra.Text4 = string.Empty;
                this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);
                var fillQty = SysCommon.FillQuantityStructureFromQuantity(this.ActItem.ProdInfo, 1);
                var tempItemSchemeData = new ActItemSchemeData
                {
                    ActDetailQuantity = fillQty.Quantity,
                    ActDetailBaseQuantity = fillQty.BaseQuantity,
                    ActDetailWeightQuantity = fillQty.WeightQuantity,
                    ProdSchemeData = new ProductSchemeData(),
                };
                this.BarCodeParse = BarCodeParseFacade.ParseData(string.Empty);
                var canInsert = await SysShared.GatherProductData(
                    this.BarCodeParse,
                    this.ActSubType,
                    this.ActItem,
                    tempItemSchemeData,
                    this.ActDetail);
                return canInsert;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<XDocument> GetProductInfo()
            {
                // Get the product unit to add the detail for.
                var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);
                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    throw new CancelOperationException();
                }

                var prodInfo = await form.GetXML();
                _ = prodInfo.Root ?? throw new Exception("Unexpected xmlDoc.Root equal to null.");
                _ = prodInfo.Root.Element((XName)"ProductUnitRefno") ?? throw new Exception(
                    "Unexpected xmlDoc.Root.Element((XName)'ProductUnitRefno') equal to null.");
                return prodInfo;
            }
        }

        /// <summary>
        /// Insert activity detail.
        /// </summary>
        /// <param name="actDetail">.</param>
        /// <param name="isInsertDetailReady">The isInsertDetailReady<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task InsertActivityDetail(ActDetail actDetail, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                actDetail.DetailFkRefno = 0;
                var inserted = this.ActDetail.GatherDetail();
                this.ActDetail.ListDetails.Add(inserted);
                this.SelectedActivityDetailFKRefno = inserted.ActivityDetailFKRefno;
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ListViewMainActivity.ItemTemplate = this.ActDetail.DesignGrid(
                        ref this.GridMainActivity,
                        inserted,
                        this.MainActivityItems);
                });

                await this.FindSelectedItemAsync();
            }
        }

        /// <summary>
        /// The BarCodeHandler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs">The scanArgs<see cref="BarCodeScannerEventArgs"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            this.IsIndicatorRunning = true;
            var listItemRowIdx = 0;

            try
            {
                await this.ActDisappearing();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.BarCodeParse = scanArgs.ParseInfo;
                if (!string.IsNullOrEmpty(this.BarCodeParse.LocationIdentifier))
                {
                    await GlobalLocationNumberHandler();
                }
                else if (!string.IsNullOrEmpty(this.BarCodeParse.ProductIdentifier))
                {
                    await ProductIdentifierHandler();
                }
                else if (!string.IsNullOrEmpty(this.BarCodeParse.ActFKRefno))
                {
                    throw new MessageOnlyException("Scanning an header reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarCodeParse.ActItemFKRefno))
                {
                    throw new MessageOnlyException("Scanning an item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarCodeParse.SSCC18))
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code # is not supported here.");
                }
                else
                {
                    throw new MessageOnlyException("Scanning bar code type is not supported here.");
                }

                // If no item is selected rowIdx will be -1, IndexOf seems to handle SelectedItem == null;
                listItemRowIdx = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message
                    : msgEx.Message["MSGBOX-".Length..];
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));

                if (this.ActDetail.ListDetails.Count >= (listItemRowIdx + 1) && listItemRowIdx >= 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems[listItemRowIdx]);
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });

                if (this.ActDetail.ListDetails.Count >= (listItemRowIdx + 1) && listItemRowIdx >= 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems[listItemRowIdx]);
                }
            }
            finally
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                });

                await this.ActAppearing();
                this.IsIndicatorRunning = false;
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task ProductIdentifierHandler()
            {
                // Identify the product scanned.
                var prodUnitReferenceNumber = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarCodeParse);
                if (prodUnitReferenceNumber <= 0)
                {
                    throw new MessageOnlyException("Unable to identify the product scanned.");
                }

                // Create a new activity item for the product unit scanned.
                var xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<ProductUnitRefno>" + prodUnitReferenceNumber + "</ProductUnitRefno>"
                    + "</params>");
                await this.ActItem.Insert(xmlDoc);

                var actDetail = new MoveDetail();
                if (this.ActDetail.ListDetails.Count > 0)
                {
                    actDetail = (MoveDetail)this.ActDetail.ListDetails.First();
                }

                this.ActDetail.ClearProperties();

                var tempItemSchemeData = new ActItemSchemeData();
                var isInsertReady = await SysShared.GatherProductData(
                    this.BarCodeParse,
                    this.ActSubType,
                    this.ActItem,
                    tempItemSchemeData,
                    this.ActDetail);
                await this.InsertActivityDetail(this.ActDetail, isInsertReady);
            }

            //////////////////////////////////////////////////////////////////////////
            async Task GlobalLocationNumberHandler()
            {
                if (this.ChkScanToLocation.IsChecked)
                {
                    await AssignToLocMap();
                }
                else
                {
                    await AssignFromLocMap();
                }

                return;

                //////////////////////////////////////////////////////////////////////////
                async Task AssignFromLocMap()
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                            this.BarCodeParse.LocationIdentifier));
                    this.SelFromLocMap = this.LocMap.CurrLocMap.Clone();
                    this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.CmdFromLocation.Text = "From: "
                            + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.CmdFromLocation.Text = "Set From Location");
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task AssignToLocMap()
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                            this.BarCodeParse.LocationIdentifier));
                    this.SelToLocMap = this.LocMap.CurrLocMap.Clone();
                    this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.CmdToLocation.Text = "To: "
                            + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.CmdToLocation.Text = "Set To Location");
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the activity list and redraw the list view.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task RefreshData()
        {
            _ = await this.ActDetail.GetActivityDetailList(false);

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                this.ExtraInfoItems.Clear();

                this.MainActivityItems = this.ActDetail.DesignGrid(
                    ref this.GridMainActivity,
                    ref this.ListViewMainActivity);
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > 0)
                {
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                    var indexOf = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);
                    var detail = this.ActDetail.ListDetails[indexOf];
                    await this.ActDetail.AssignActivityDetail(detail);
                }
            });
        }

        /// <summary>
        /// Option menu clicked event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            var result = await this.DisplayActionSheet(
                "Move Inventory Options",
                "Close",
                null,
                this.OptionsMenuItems.ToArray());

            if (result == "Save")
            {
                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else
                {
                    this.MainActivityItems.Clear();
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                }
            }
            else if (result == "Cancel")
            {
                if (await this.CancelChanges())
                {
                    await this.ActDisappearing();
                    await this.Close();
                }
            }
        }

        /// <summary>
        /// Perform move clicked handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdPerformMove_Clicked(object sender, EventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            this.CmdPerformMove.IsEnabled = false;
            if (await this.SaveChanges())
            {
                await this.Close();
            }
            else
            {
                this.MainActivityItems.Clear();
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            }

            this.CmdPerformMove.IsEnabled = true;
        }
    }
}
