// <copyright file="ActivityHdrPickingSelectCriteria.xaml.cs" company="InterSect Business Systems Inc">
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
    using CustomPages.Modal;
    using Forms;
    using Resources.Enums;
    using Resources.Services;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrPickingSelectCriteria : DMobileTabbedModal, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrPickingSelectCriteria" /> class.
        /// </summary>
        public ActivityHdrPickingSelectCriteria()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.lblSectionData_tabWarehouse.Text = string.Empty;
            this.lblWarehouseData_tabWarehouse.Text = string.Empty;
            this.lblSectionData_tabSection.Text = string.Empty;
            this.lblWarehouseData_tabSection.Text = string.Empty;
            this.lblSearch.Text = "Search";
            this.lblWarehouse_tabWarehouse.Text = "Warehouse";
            this.lblSection_tabWarehouse.Text = "Section";
            this.lblWarehouse_tabSection.Text = "Warehouse";
            this.lblSection_tabSection.Text = "Section";
            this.warehouseTab.Title = "Warehouse";
            this.sectionTab.Title = "Section";
            this.btnCancel_tabWarehouse.Text = "Cancel";
            this.btnSave_tabWarehouse.Text = "Save";
            this.btnCancel_tabSection.Text = "Cancel";
            this.btnSave_tabSection.Text = "Save";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = ScannerEngine.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.PickingSelectCriteria;
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMapInfo = new SLocMap();
                this.Title = this.GetControlDescription();
            }
        }

        private BarCodeParse BarParse { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; }

        private BarCodeScanner Scanner { get; }

        private SLocMap LocMapInfo { get; set; }

        private bool IsBarCodeHandlerRegistered { get; set; }

        private LocationMap LocMap { get; }

        private List<Section> ListSections { get; set; }

        private List<Warehouse> ListWarehouse { get; set; }

        private bool IsChangesMade { get; set; }

        private bool IsControlLoaded { get; set; }

        private SysEnvironment SysEnv { get; }

        public ObservableCollection<object> WarehouseObservableCollection { get; set; }
        public ObservableCollection<object> SectionObservableCollection { get; set; }
        public List<Warehouse> FilteredListWarehouse { get; set; }
        public List<Section> FilteredListSections { get; set; }

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
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.EnableScanning();
                if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                {
                    this.Scanner.BarCodeScan += this.BarCodeHandler;
                }

                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
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

            return isReadyToSave;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
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

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.LocMap.CurrLocMap = this.LocMapInfo;
                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all
                    // in the event that IsReadyToSave just returns false.
                    throw new MessageOnlyException("Data failed validation. Check your data.");
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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    if (!this.SysEnv.SysSettings.IsPickingModuleDisallowLoadingAllOrders)
                    {
                        if (await Device.InvokeOnMainThreadAsync(async () =>
                                !await this.DisplayAlert(this.GetControlDescription(),
                                    "All orders for all sections will be loaded. Continue?", "Yes", "No")))
                        {
                            cancelChanges = false;
                            this.IsChangesMade = false;
                        }
                    }

                    // Clear any properties.
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
                    await this.LoadData();
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

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public Task<XDocument> GetXml()
        {
            var locMapRefno = 0;

            if (this.LocMapInfo.SectLocMapRefno > 0)
            {
                locMapRefno = this.LocMapInfo.SectLocMapRefno;
            }
            else if (this.LocMapInfo.WhsLocMapRefno > 0)
            {
                locMapRefno = this.LocMapInfo.WhsLocMapRefno;
            }

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<LocationMapRefno>" + locMapRefno + "</LocationMapRefno>"
                + "</params>");

            return Task.FromResult(request);
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();

                    this.dgWhs.ItemsSource = this.WarehouseObservableCollection;
                    this.dgSections.ItemsSource = this.SectionObservableCollection;

                    MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.IsIndicatorRunning)
                        {
                            if (keycode == Keycode.Enter || keycode == Keycode.F1)
                            {
                                if (await this.SaveChanges())
                                {
                                    this.DialogResult = DialogResult.OK;
                                    await this.Close();
                                }
                            }
                            else if (keycode == Keycode.DpadDown)
                            {
                                if (this.CurrentPage.Title == this.warehouseTab.Title)
                                {
                                    if (this.dgWhs.SelectedItem != null &&
                                        Convert.ToInt32(
                                            this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse)) <
                                        this.WarehouseObservableCollection.Count - 1)
                                    {
                                        this.dgWhs.SelectedItem = this.WarehouseObservableCollection[
                                            this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse) +
                                            1];
                                        this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                                    }
                                }
                                else if (this.CurrentPage.Title == this.sectionTab.Title)
                                {
                                    if (this.dgSections.SelectedItem != null &&
                                        Convert.ToInt32(
                                            this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section)) <
                                        this.SectionObservableCollection.Count - 1)
                                    {
                                        this.dgSections.SelectedItem = this.SectionObservableCollection[
                                            this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section) +
                                            1];
                                        this.dgSections.ScrollTo(this.dgSections.SelectedItem, ScrollToPosition.MakeVisible,
                                            true);
                                    }
                                }
                            }
                            else if (keycode == Keycode.DpadUp)
                            {
                                if (this.CurrentPage.Title == this.warehouseTab.Title)
                                {
                                    if (this.dgWhs.SelectedItem != null && Convert.ToInt32(
                                            this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse)) >
                                        0)
                                    {
                                        this.dgWhs.SelectedItem = this.WarehouseObservableCollection[
                                            this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse) -
                                            1];
                                        this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                                    }
                                }
                                else if (this.CurrentPage.Title == this.sectionTab.Title)
                                {
                                    if (this.dgSections.SelectedItem != null && Convert.ToInt32(
                                            this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section)) >
                                        0)
                                    {
                                        this.dgSections.SelectedItem = this.SectionObservableCollection[
                                            this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section) -
                                            1];
                                        this.dgSections.ScrollTo(this.dgSections.SelectedItem, ScrollToPosition.MakeVisible,
                                            true);
                                    }
                                }
                            }
                        }
                    });

                    await this.ucActivityHdrPickingSelectCriteria_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error",
                        "this.ucActivityHdrPickingSelectCriteria_GotFocus threw an error " + ex, "OK"));
                }

                try
                {
                    await this.tabLocation_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Error", "this.tabLocation_GotFocus threw an error " + ex, "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            }
        }

        /// <inheritdoc />
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    _ = base.OnBackButtonPressed();
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
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
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Picking Selection Criteria";
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        public void InitCboSearchSection(object sender, EventArgs e)
        {
            this.cboSectionSearch.SelectedItem = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task tabLocation_GotFocus(object sender, EventArgs e)
        {
            // this.ListSections.RowFilter = "WarehouseLocationMapRefno = " +  this.LocMapInfo.WhsLocMapRefno;
            this.FilteredListSections = this.ListSections
                .Where(section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();

            this.SelectSection();

            if (this.ListSections.Count > 0)
            {
                if (this.dgSections.SelectedItem is Section section)
                {
                    this.LocMapInfo =
                        await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(section.LocationMapRefno));
                }
            }

            this.UpdateLocationDisplay();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task ucSetLocationMap_LostFocus(object sender, EventArgs e)
        {
            await this.ScannerDisable();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task ucActivityHdrPickingSelectCriteria_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                await this.ScannerEnable();
                await this.LoadData();
                this.SelectWarehouse();
                this.UpdateLocationDisplay();

                //// WO 10190 LEKKER wants to default to the section tab, Lekker only has one warehouse so we can assume it.
                //// Update Oct 9,2014 Lekker now has 2 warehouse this code will not work anymore.
                //// John said "do not fix until they complain". We could default the warehouse to the login warehouse.
                // if (this.DataViewWhs.Count == 1 && this.DataViewSections.Count > 0)
                // {
                //    //this.tabLocation.SelectedIndex = 1;
                //    this.tabSection.Focus();
                // }
                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtSearchSection_TextChanged(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                // By Code
                this.IsChangesMade = true;

                if (this.cboSectionSearch.SelectedIndex == 0)
                {
                    if (string.Compare(this.txtSearchSection.Text, string.Empty) > 0)
                    {
                        // this.ListSections.RowFilter = "LocationMapSectionCode = " + this.txtSearchSection.Text.Trim();
                        this.FilteredListSections = this.ListSections.Where(section =>
                                Convert.ToString(section.LocationMapSectionCode) == this.txtSearchSection.Text.Trim())
                            .ToList();
                    }
                }
                else
                {
                    // this.ListSections.RowFilter = "LocationMapSectionName LIKE \'" + this.txtSearchSection.Text.Trim() + "%\'";
                    this.FilteredListSections = this.ListSections.Where(section =>
                        section.LocationMapSectionName.Contains(this.txtSearchSection.Text.Trim())).ToList();
                }

                this.PopulateSectionList();
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadWhsList()
        {
            this.ListWarehouse = await LocationMapTasks.GetWhsList();
            this.FilteredListWarehouse = this.ListWarehouse;

            this.DesignGridWarehouses();
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadSections()
        {
            this.ListSections = await LocationMapTasks.GetSectList();
            this.FilteredListSections = this.ListSections
                .Where(section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();

            this.DesignGridSections();
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            var tempLocMapInfo = this.LocMap.CurrLocMap;

            // Default to the login warehouse
            tempLocMapInfo.WhsLocMapRefno = this.LocMap.LoginWhsLocMapRefno;

            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                tempLocMapInfo.WhsLocMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno;
            }

            tempLocMapInfo.SectLocMapRefno = this.LocMap.CurrLocMap.SectLocMapRefno;

            if (this.LocMap.CurrLocMap.IsSectionTracked && tempLocMapInfo.SectLocMapRefno != 0)
            {
                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(tempLocMapInfo.SectLocMapRefno);
            }
            else
            {
                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(tempLocMapInfo.WhsLocMapRefno);
            }

            // Load the dataviews and apply row filters base on the current sLocationMapInfo values
            await this.LoadWhsList();
            await this.LoadSections();
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void SelectWarehouse()
        {
            // Attempt to find last selected warehouse in warehouse view
            if (this.LocMapInfo.WhsLocMapRefno != 0)
            {
                var warehouse = this.FilteredListWarehouse
                    .Where(warehouse => warehouse.LocationMapRefno == this.LocMapInfo.WhsLocMapRefno).FirstOrDefault();

                if (warehouse != null)
                {
                    this.dgWhs.SelectedItem = warehouse;
                }
            }
            else
            {
                if (this.WarehouseObservableCollection.Count > 0)
                {
                    this.dgWhs.SelectedItem = this.WarehouseObservableCollection[0];
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void UpdateLocationDisplay()
        {
            if (this.LocMapInfo.WhsLocMapRefno == 0)
            {
                this.lblWarehouseData_tabWarehouse.Text = "Unknown";
                this.lblWarehouseData_tabWarehouse.TextColor = Color.Black;
                this.lblWarehouseData_tabSection.Text = "Unknown";
                this.lblWarehouseData_tabSection.TextColor = Color.Black;
            }
            else
            {
                this.lblWarehouseData_tabWarehouse.Text = this.LocMapInfo.WhsCode + " " + this.LocMapInfo.WhsName;
                this.lblWarehouseData_tabWarehouse.TextColor = Color.Blue;
                this.lblWarehouseData_tabSection.Text = this.LocMapInfo.WhsCode + " " + this.LocMapInfo.WhsName;
                this.lblWarehouseData_tabSection.TextColor = Color.Blue;
            }

            if (this.LocMapInfo.SectLocMapRefno == 0)
            {
                this.lblSectionData_tabWarehouse.Text = "Unknown";
                this.lblSectionData_tabWarehouse.TextColor = Color.Black;
                this.lblSectionData_tabSection.Text = "Unknown";
                this.lblSectionData_tabSection.TextColor = Color.Black;
            }
            else
            {
                this.lblSectionData_tabWarehouse.Text = this.LocMapInfo.SectCode + " " + this.LocMapInfo.SectName;
                this.lblSectionData_tabWarehouse.TextColor = Color.Blue;
                this.lblSectionData_tabSection.Text = this.LocMapInfo.SectCode + " " + this.LocMapInfo.SectName;
                this.lblSectionData_tabSection.TextColor = Color.Blue;
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void SelectSection()
        {
            // Attempt to find last selected section in section view
            if (this.LocMapInfo.SectLocMapRefno != 0 && this.dgSections.SelectedItem is Section section)
            {
                if (section.LocationMapRefno != this.LocMapInfo.SectLocMapRefno)
                {
                    var sect = this.FilteredListSections
                        .Where(section => section.LocationMapRefno == this.LocMapInfo.SectLocMapRefno).FirstOrDefault();

                    if (sect != null)
                    {
                        this.dgSections.SelectedItem = sect;
                    }
                }
            }
            else
            {
                if (this.SectionObservableCollection.Count > 0)
                {
                    this.dgSections.SelectedItem = this.SectionObservableCollection[0];
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;

                // No need to disable/enable scanner because only messagebox are called.
                // GoFocus and LostFocus events will handle the scanner
                this.BarParse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.BarParse.ActFkRefno))
                {
                    throw new MessageOnlyException("Scanning an activity header reference # is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ActItemFkRefno))
                {
                    throw new MessageOnlyException("Scanning an activity item reference # is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new MessageOnlyException("Scanning a product bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.Sscc18))
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        this.BarParse.LocationIdentifier);
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                    this.SelectWarehouse();
                    this.SelectSection();
                }
                else
                {
                    throw new MessageOnlyException("Scanned bar code type is not supported here.");
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing
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

        private void DesignGridWarehouses()
        {
            this.PopulateWarehouseList();

            var labelCodeHeader = new Label
                { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelNameHeader = new Label
                { Text = "Name", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            var columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCodeHeader.Text);
            var columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelNameHeader.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelCodeHeader);
            gridHeader.Children.Add(labelNameHeader);

            Grid.SetColumn(labelCodeHeader, 0);
            Grid.SetColumn(labelNameHeader, 1);

            Grid.SetRow(gridHeader, 0);

            this.dataGridSectionsHeader.Children.Add(gridHeader);

            foreach (Warehouse warehouse in this.WarehouseObservableCollection)
            {
                var dataColumnWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(warehouse.LocationMapWarehouseCode));
                var dataColumnWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(warehouse.LocationMapWarehouseName);

                if (dataColumnWidth0 > columnWidth0)
                {
                    columnWidth0 = dataColumnWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (dataColumnWidth1 > columnWidth1)
                {
                    columnWidth1 = dataColumnWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }
            }

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),

                    // ColumnSpacing = 15,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) }
                    }
                };

                var labelCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelName = new Label { VerticalOptions = LayoutOptions.Center };

                labelCode.SetBinding(Label.TextProperty, new Binding("LocationMapWarehouseCode"));
                labelName.SetBinding(Label.TextProperty, new Binding("LocationMapWarehouseName"));

                gridData.Children.Add(labelCode, 0, 0);
                gridData.Children.Add(labelName, 0, 0);

                Grid.SetColumn(labelCode, 0);
                Grid.SetColumn(labelName, 1);

                return new ViewCell { View = gridData };
            });

            this.dgWhs.ItemTemplate = dataTemplate;
            this.dgWhs.ItemsSource = this.WarehouseObservableCollection;
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void DesignGridSections()
        {
            this.PopulateSectionList();

            var labelCodeHeader = new Label
                { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelNameHeader = new Label
                { Text = "Name", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            var columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCodeHeader.Text);
            var columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelNameHeader.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelCodeHeader);
            gridHeader.Children.Add(labelNameHeader);

            Grid.SetColumn(labelCodeHeader, 0);
            Grid.SetColumn(labelNameHeader, 1);

            Grid.SetRow(gridHeader, 0);

            this.dataGridSectionsHeader.Children.Add(gridHeader);

            foreach (Section section in this.SectionObservableCollection)
            {
                var dataColumnWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(section.LocationMapSectionCode));
                var dataColumnWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(section.LocationMapSectionName);

                if (dataColumnWidth0 > columnWidth0)
                {
                    columnWidth0 = dataColumnWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (dataColumnWidth1 > columnWidth1)
                {
                    columnWidth1 = dataColumnWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }
            }

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),

                    // ColumnSpacing = 15,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) }
                    }
                };

                var labelCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelName = new Label { VerticalOptions = LayoutOptions.Center };

                labelCode.SetBinding(Label.TextProperty, new Binding("LocationMapSectionCode"));
                labelName.SetBinding(Label.TextProperty, new Binding("LocationMapSectionName"));

                gridData.Children.Add(labelCode, 0, 0);
                gridData.Children.Add(labelName, 0, 0);

                Grid.SetColumn(labelCode, 0);
                Grid.SetColumn(labelName, 1);

                return new ViewCell { View = gridData };
            });

            this.dgSections.ItemTemplate = dataTemplate;
            this.dgSections.ItemsSource = this.SectionObservableCollection;
        }

        private async void dgWhs_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsControlLoaded && e.SelectedItem is Warehouse warehouse)
            {
                this.IsChangesMade = true;
                this.LocMapInfo =
                    await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(warehouse.LocationMapRefno));
                await this.LoadSections();
                this.UpdateLocationDisplay();
            }
        }

        private async void dgSections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsControlLoaded && e.SelectedItem is Section section)
            {
                this.IsChangesMade = true;
                this.LocMapInfo =
                    await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(section.LocationMapRefno));
                this.UpdateLocationDisplay();
            }
        }

        private void PopulateWarehouseList()
        {
            this.WarehouseObservableCollection = new ObservableCollection<object>();

            foreach (var warehouse in this.FilteredListWarehouse)
            {
                // Warehouse warehouse = new Warehouse
                // {
                //    LocationMapWarehouseCode = Convert.ToInt32(row["LocationMapWarehouseCode"]),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"]),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"])
                // };
                this.WarehouseObservableCollection.Add(warehouse);
            }
        }

        private void PopulateSectionList()
        {
            this.SectionObservableCollection = new ObservableCollection<object>();

            foreach (var section in this.FilteredListSections)
            {
                // Section section = new Section
                // {
                //    LocationMapSectionCode = Convert.ToInt32(row["LocationMapSectionCode"]),
                //    LocationMapSectionName = Convert.ToString(row["LocationMapSectionName"]),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                //    WarehouseLocationMapRefno = Convert.ToInt32(row["WarehouseLocationMapRefno"])
                // };
                this.SectionObservableCollection.Add(section);
            }
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            _ = this.OnBackButtonPressed();
        }
    }
}