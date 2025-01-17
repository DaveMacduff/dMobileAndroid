// <copyright file="SelectActivityItem.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Item;
    using Controller.Activity;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using static Bedrock.SysCommon;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    /// Perform selecting a product unit.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectActivityItem : IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectActivityItem"/> class.
        /// </summary>
        /// <param name="activity">Activity header.</param>
        /// <param name="actSubType">Activity subtype.</param>
        public SelectActivityItem(Activity activity, ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = actSubType;
            this.ActItem = ActItemFacade.GetActItem(activity, this.ActSubType);
            this.ActItem.ActHdr = activity;
            this.IsDataLoading = false;
            this.IsIndicatorRunning = false;

            ParseAndSeedBarCodeModifier();
            this.SearchTimer = new SysTimer(this.SearchTimerTickCallback);
            this.IsFirstAppearance = true;
            return;

            //////////////////////////////////////////////////////////////////////////
            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
                this.BarCodeModifier1Picker.Items.Clear();
                if (!this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    return;
                }

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

        private SysEnvironment SysEnv { get; }

        private ActSubTypes ActSubType { get; }

        private bool IsSearchEntryFocused { get; set; }

        private bool IsBarCodeModifierPickerFocus { get; set; }

        private bool IsDataLoading { get; set; }

        private string SearchTextOrig { get; set; } = string.Empty;

        private SysTimer SearchTimer { get; }

        private bool IsFirstAppearance { get; set; }

        private bool IsChangesMade { get; set; }

        private ActItem ActItem { get; }

        private BarCodeScanner Scanner { get; }

        private ObservableCollection<object> ActivityItems { get; set; } = new ObservableCollection<object>();

        private int NumberOfScreenTapsForDoubleClick { get; set; }

        private object PreviousSenderForDoubleClick { get; set; }

        private int ResultItemRefno { get; set; }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave;
            try
            {
                isReadyToSave = true;
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return isReadyToSave;
        }

        /// <summary>
        /// Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;
            try
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initialize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                await this.ScannerDisable();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            this.Scanner.ClearBarCodeScanEventHandlers();
            await this.Scanner.DisableScanning();
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
            }

            await this.Scanner.EnableScanning();
        }

        /// <summary>
        /// Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch-all
                    // in the event that IsReadyToSave just returns false.
                    throw new Exception("Data failed validation. Check your data.");
                }
            }
            catch (Exception ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return saveComplete;
        }

        /// <summary>
        /// Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties if needed.
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
                    // await this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;

                    // Re-enable the scanner, IsReadToClose calls this.ScannerDisable().
                    await this.ScannerEnable();
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return cancelChanges;
        }

        /// <summary>
        /// Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<ActivityItemRefno>" + this.ResultItemRefno + "</ActivityItemRefno>"
                + "</params>");

            return request;
        }

        /// <summary>
        /// On appearing event handler.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                this.NumberOfScreenTapsForDoubleClick = 0;
                this.SubscribeKeyUpHandlers();
                this.ListViewMainActivity.ItemTapped += this.ListViewMainActivity_ItemTapped;
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    await this.LoadAndBind();
                    await this.ScannerEnable();
                }

                if (!this.IsSearchEntryFocused)
                {
                    this.ListViewMainActivity.Focus();
                    ////await this.FindSelectedItemAsync();
                }
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(this.ActSubType, ex);
                _ = await errRpt.ShowDialog();
            }
        }

        /// <summary>
        /// The on disappearing event handler.
        /// </summary>
        protected override async void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();
                this.ListViewMainActivity.ItemTapped -= this.ListViewMainActivity_ItemTapped;
                MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
        }

        /// <summary>
        /// On back button pressed event handler.
        /// </summary>
        /// <returns><see cref="bool"/> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void BackButtonCallback()
            {
                try
                {
                    _ = base.OnBackButtonPressed();
                    _ = await this.CancelChanges();
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                }
            }
        }

        /// <summary>
        ///     SearchTimer tick call back.
        /// </summary>
        private void SearchTimerTickCallback()
        {
            this.SearchTimer.Stop();
            if (this.IsSearchEntryFocused && this.TxtSearchItem.Text != this.SearchTextOrig)
            {
                this.SearchTextOrig = this.TxtSearchItem.Text;
                this.LoadAndBind().FireAndForgetSafeAsync();
            }

            // Did not start the timer again here because the timer is started in the OnTextChange event.
        }

        private string GetControlDescription()
        {
            return "Select Activity Item";
        }

        private async Task LoadAndBind()
        {
            try
            {
                // We only want to load data if it is not already loading.
                if (!this.IsDataLoading)
                {
                    this.IsDataLoading = true;

                    // The activity indicator control messes up the search text control focus.
                    // Disable it for searching.
                    if (!this.IsSearchEntryFocused)
                    {
                        this.IsIndicatorRunning = true;
                    }

                    // Do not request data again from the server if the table is already populated.
                    this.ActItem.ListItems = await this.ActItem.GetActivityItemList(isSubmitBatch: false);

                    var mainActivityItems = this.ActItem.CreateObservableCollectionFromListItems(
                        this.ActItem.ListItems,
                        this.TxtSearchItem.Text);
                    this.ActItem.SelectedItem = this.ActItem.ListItems.FirstOrDefault();
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                    this.ActItem.ItemComment = await this.ActItem.GetComment(this.ActItem.SelectedItem);

                    var gridItems = this.ActItem.DefineGrid();
                    this.ActItem.CalcMaxDataColWidth(gridItems, this.ActItem.ListItems);
                    this.GridMainActivity.Children.Add(gridItems);
                    this.ListViewMainActivity.ItemTemplate = this.ActItem.GetDataTemplate();
                    this.ActivityItems = mainActivityItems;
                    this.ListViewMainActivity.ItemsSource = this.ActivityItems;
                    await this.FindSelectedItemAsync(this.ActItem.ItemFkRefno);
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var msg = $"Unable to load activity items.{Environment.NewLine}" +
                                    $"General exception.{Environment.NewLine}" +
                                    $"Detail:{ex.Message}{Environment.NewLine}" +
                                    "Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
            }
            finally
            {
                // The activity indicator control messes up the search text control focus. Disable for searching.
                if (!this.IsSearchEntryFocused)
                {
                    this.IsIndicatorRunning = false;
                }

                this.IsDataLoading = false;
            }
        }

        private async Task FindSelectedItemAsync(int activityItemFkRefno)
        {
            object selectedItem = null;
            var selectedIndex = -1;
            if (this.ActItem.ListItems.Count > 0)
            {
                if (this.ActItem.SelectedListIdx >= 0)
                {
                    selectedIndex = this.ActItem.SelectedListIdx;
                    selectedItem = this.ActItem.ListItems[selectedIndex];
                }

                if (selectedItem == null &&
                    activityItemFkRefno > 0)
                {
                    selectedIndex = 0;
                    foreach (var item in this.ActItem.ListItems)
                    {
                        if (activityItemFkRefno == this.ActItem.GetFkRefno(item))
                        {
                            selectedItem = item;
                            break;
                        }

                        selectedIndex++;
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    selectedIndex = 0;
                    selectedItem = this.ActItem.ListItems[selectedIndex];
                }
            }

            this.ActItem.SelectedListIdx = selectedIndex;
            await this.ActItem.AssignActivityItem(selectedItem);
            Device.BeginInvokeOnMainThread(
                () =>
                {
                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        false);
                });
        }

        private async void ListViewMainActivity_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                this.NumberOfScreenTapsForDoubleClick++;

                if (this.PreviousSenderForDoubleClick == e.Item)
                {
                    if (this.NumberOfScreenTapsForDoubleClick == 2)
                    {
                        this.NumberOfScreenTapsForDoubleClick = 0;
                        try
                        {
                            if (this.ListViewMainActivity.SelectedItem != null &&
                                Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) > -1)
                            {
                                // Close and return to the calling activity item single form.
                                this.ActItem.SelectedItem = this.ListViewMainActivity.SelectedItem;
                                this.ResultItemRefno = this.ActItem.ItemRefno;
                                await this.Close();
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            throw;
                        }
                        finally
                        {
                            await this.ScannerEnable();
                        }
                    }
                }
                else
                {
                    this.PreviousSenderForDoubleClick = e.Item;
                    this.NumberOfScreenTapsForDoubleClick = 1;
                }

                await Task.Delay(500);
                this.NumberOfScreenTapsForDoubleClick = 0;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                        return Task.CompletedTask;
                    });
            }
        }

        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(
                        async () => await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                var parse = scanArgs.ParseInfo;

                // Global location identifier
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    throw new Exception("Scanning a global location identifier is not supported here!");
                }

                // Activity fk refno
                if (!string.IsNullOrEmpty(parse.ActFkRefno))
                {
                    throw new Exception("Scanning an activity bar code is not supported here!");
                }

                // Activity item fk refno
                if (!string.IsNullOrEmpty(parse.ActItemFkRefno))
                {
                    await BarCodeHandlerLineItemIdentifier();
                }

                // Activity detail fk refno
                if (!string.IsNullOrEmpty(parse.ActDetailFkRefno))
                {
                    throw new Exception("Scanning a line item detail bar code is not supported here!");
                }

                // Shipping container scanned, skid
                if (!string.IsNullOrEmpty(parse.Sscc18))
                {
                    throw new Exception("Scanning a shipping container bar code is not supported here!");
                }

                // Product identifier
                if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                {
                    await BarCodeHandlerProductIdentifier();
                }

                async Task BarCodeHandlerProductIdentifier()
                {
                    // Identify the product from the parse info.
                    var prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);
                    var scannedProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                    // Attempt to find the activity line item.
                    var locMap = LocationMap.GetInstance();
                    var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                        prodUnitRefno,
                        locMap.CurrLocMap);
                    if (actItemFoundRefno > 0)
                    {
                        object foundItem = null;
                        foreach (var item in this.ActItem.ListItems)
                        {
                            if (((ActItem)item).ItemRefno == actItemFoundRefno)
                            {
                                foundItem = item;
                                break;
                            }
                        }

                        if (foundItem == null)
                        {
                            var msg = this.ActItem.GetItemNotFoundMessage(locMap, scannedProdInfo);
                            throw new MessageOnlyException(msg);
                        }

                        this.ResultItemRefno = ((ActItem)foundItem).ItemRefno;
                        await this.Close();
                    }
                }

                async Task BarCodeHandlerLineItemIdentifier()
                {
                    object foundItem = null;
                    var itemFkRefno = Convert.ToInt32(parse.ActItemFkRefno);
                    foreach (var item in this.ActItem.ListItems)
                    {
                        if (((ActItem)item).ItemFkRefno == itemFkRefno)
                        {
                            foundItem = item;
                            break;
                        }
                    }

                    if (foundItem == null)
                    {
                        var msg = this.ActItem.GetUnableToFindItemFkRefnoMessage(scanArgs.ParseInfo.ActItemFkRefno);
                        throw new MessageOnlyException(msg);
                    }

                    this.ResultItemRefno = ((ActItem)foundItem).ItemRefno;
                    await this.Close();
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () => await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                await this.ScannerEnable();
                if (scanArgs != null)
                {
                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;
                }
            }
        }

        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                await this.ActItem.AssignActivityItem(e.SelectedItem);
                this.ActItem.SelectedListIdx = e.SelectedItemIndex;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = errRpt.ShowDialog();
                        return Task.CompletedTask;
                    });
            }
        }

        private void TxtSearchItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            // SearchTimer was started in the focused event handler.
            // SearchTimer was stopped in the unfocused event handler.
            // SearchTimer was stopped in the timer tick call back event.
            // Start the timer when the text changes. This will reset the timer and force a load data.
            this.SearchTimer.Start();
        }

        private void TxtSearchItem_Focused(object sender, FocusEventArgs e)
        {
            this.IsSearchEntryFocused = true;
            this.SearchTimer.Start();
        }

        private void TxtSearchItem_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsSearchEntryFocused = false;
            this.SearchTimer.Stop();
        }

        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = this.BarCodeModifier1Picker.SelectedItem.ToString();
        }

        private void BarCodeModifier1Picker_Focused(object sender, FocusEventArgs e)
        {
            this.IsBarCodeModifierPickerFocus = true;
        }

        private void BarCodeModifier1Picker_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsBarCodeModifierPickerFocus = false;
        }

        /// <summary>
        ///     Subscribe the key up handlers.
        /// </summary>
        private void SubscribeKeyUpHandlers()
        {
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), this.KeyUpHandlerCallback);
        }

        /// <summary>
        ///     Key up handler call back.
        /// </summary>
        /// <param name="keyPressed"><see cref="string" /> Key pressed value.</param>
        private async void KeyUpHandlerCallback(string keyPressed)
        {
            try
            {
                if (!this.IsDataLoading)
                {
                    await this.ScannerDisable();
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (keycode)
                    {
                        case Keycode.DpadDown:
                            AppearingDpadDown();
                            break;

                        case Keycode.DpadUp:
                            AppearingDpadUp();
                            break;

                        case Keycode.Enter:
                            if (!(this.IsSearchEntryFocused || this.IsBarCodeModifierPickerFocus))
                            {
                                await AppearingEnterKey();
                            }

                            break;
                    }
                }

                return;

                //////////////////////////////////////////////////////////////////////////
                async Task AppearingEnterKey()
                {
                    if (this.ListViewMainActivity.SelectedItem == null ||
                        Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
                    {
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.GetControlDescription(),
                                    "Please select an item.",
                                    Resource.OK));
                    }
                    else
                    {
                        if (this.IsDataLoading ||
                            this.ListViewMainActivity.SelectedItem == null ||
                            Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
                        {
                            return;
                        }

                        this.ActItem.SelectedItem = this.ListViewMainActivity.SelectedItem;
                        this.ResultItemRefno = this.ActItem.ItemRefno;
                        await this.Close();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void AppearingDpadDown()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) <
                        this.ActivityItems.Count - 1)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.ActivityItems[
                                this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) + 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void AppearingDpadUp()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) > 0)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.ActivityItems[
                                this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) - 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }
    }
}
