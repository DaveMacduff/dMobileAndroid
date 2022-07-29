// <copyright file="ActivityMoveInventory.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
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
        /// Defines the optionsMenuItems.
        /// </summary>
        private List<string> optionsMenuItems = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityMoveInventory"/> class.
        /// </summary>
        public ActivityMoveInventory()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityMoveInventory"/> class.
        /// </summary>
        /// <param name="actSubType">.</param>
        public ActivityMoveInventory(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.cmdFromLocation.Text = "Set From Location";
            this.cmdAdd.Text = "Add";
            this.cmdToLocation.Text = "Set To Location";
            this.lblScanToLocation.Text = "Scan To Loc.";

            this.MainActivityItems = new ObservableCollection<object>();
            this.ExtraInfoItems = new ObservableCollection<string>();

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SelFromLocMap = new SLocMap();
            this.SelToLocMap = new SLocMap();

            this.BarCodeModifier1Picker.Items.Clear();

            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.BarCodeModifier1Picker.Items.Add(string.Empty);
                string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                foreach (string item in modifiers)
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

            this.ActSubType = actSubType;
            this.Activity = ActivityFacade.GetActivity(actSubType);
            this.Activity.SkidInfo = new SysSkidInfo();
            this.ActItem = ActItemFacade.GetActItem(this.Activity, actSubType);
            this.ActItem.ActHdr = this.Activity;
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, actSubType);
            this.ActDetail.ActItem = this.ActItem;
            this.ActDetail.ActExtra = new ActDetailExtra();

            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.Title = this.GetControlDescription();

            this.optionsMenuItems.Add("Save");
            this.optionsMenuItems.Add("Cancel");
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Del || keycode == Keycode.Clear || keycode == Keycode.Back)
                {
                    if (this.ListViewMainActivity.SelectedItem != null && this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) != -1 && await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Do you wish to delete this record?", "Yes", "No")))
                    {
                        // TODO: Test
                        this.ActDetail.ListDetails.RemoveAt(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem));
                    }
                }
            });

            try
            {
                this.Loading = true;
                await Task.Run(async () => await this.ucActivityMoveInventory_GotFocus());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityMoveInventory_GotFocus threw an error " + ex, "OK"));
            }
            finally
            {
                this.Loading = false;
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
        }

        /// <summary>
        /// The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.CancelChanges())
                {
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Gets or sets the Scanner.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = null;

        /// <summary>
        /// Gets or sets the SelToCurrLocMapRefno.
        /// </summary>
        private int SelToCurrLocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the SelFromCurrLocMapRefno.
        /// </summary>
        private int SelFromCurrLocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the ActDetail.
        /// </summary>
        private ActDetail ActDetail { get; set; } = null;

        /// <summary>
        /// Gets or sets the ActItem.
        /// </summary>
        private ActItem ActItem { get; set; } = null;

        /// <summary>
        /// Gets or sets the Activity.
        /// </summary>
        private Activity Activity { get; set; } = null;

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
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsControlLoaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsChangesMade.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the ProdUnitRefno.
        /// </summary>
        private int ProdUnitRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the ProdUnitBarCodeRefno.
        /// </summary>
        private int ProdUnitBarCodeRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the BarCodeParse.
        /// </summary>
        private BarCodeParse BarCodeParse { get; set; } = null;

        /// <summary>
        /// Gets or sets the ExtraInfoItems.
        /// </summary>
        public ObservableCollection<string> ExtraInfoItems { get; set; }

        /// <summary>
        /// Gets or sets the MainActivityItems.
        /// </summary>
        public ObservableCollection<object> MainActivityItems { get; set; }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            if (this.ActDetail.ListDetails.Count > 0)
            {
                if (!await LocationMapTasks.IsCompleteLocMap(this.SelFromLocMap))
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Send Moves", "Invalid Move \'From\' Location", "OK"));
                }
                else if (!await LocationMapTasks.IsCompleteLocMap(this.SelToLocMap))
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Send Moves", "Invalid Move \'To\' Location", "OK"));
                }
                else if (this.SelFromLocMap.WhsLocMapRefno != this.SelToLocMap.WhsLocMapRefno)
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Send Moves", "The \'From\' And \'To\' Warehouse(s) Must Be The Same", "OK"));
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
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }

            return true;
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Move Inventory";
        }

        /// <summary>
        /// The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            bool bSaveComplete = false;

            try
            {
                this.Loading = true;

                await Task.Run(async () =>
                {
                    if (await this.IsReadyToSave())
                    {
                        if (this.ActDetail.ListDetails != null && this.ActDetail.ListDetails.Count > 0)
                        {
                            for (int idx = 0; idx < this.ActDetail.ListDetails.Count; idx++)
                            {
                                try
                                {
                                    if (this.ActDetail.ListDetails.Count > 0)
                                    {
                                        this.ActItem.ItemFKRefno = 0;
                                        this.ActItem.ItemRefno = 0;
                                        ProductInfo oProductInfo = this.ActItem.ProdInfo;
                                        oProductInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(this.ActDetail.ListDetails[idx].GetType().GetProperty("ProductUnitRefno")?.GetValue(this.ActDetail.ListDetails[idx])));
                                        oProductInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(oProductInfo.Refno);
                                        this.ActItem.ProdInfo = oProductInfo;
                                        this.ActDetail = await this.ActDetail.GetInfo(this.ActDetail.ListDetails[idx], false);
                                        XDocument xmlParam = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<param>" + "<whs_id>" + this.SelFromLocMap.WhsFKRefno.ToString() + "</whs_id>" + "<sect_refno>" + this.SelFromLocMap.SectFKRefno.ToString() + "</sect_refno>" + "<slot_refno>" + this.SelFromLocMap.SlotFKRefno.ToString() + "</slot_refno>" + "<to_sect_refno>" + this.SelToLocMap.SectFKRefno.ToString() + "</to_sect_refno>" + "<to_slot_refno>" + this.SelToLocMap.SlotFKRefno.ToString() + "</to_slot_refno>" + "</param>");

                                        await this.ActDetail.Submit(this.ActDetail.ListDetails[idx], xmlParam);
                                        idx--;
                                    }
                                    else
                                    {
                                        goto endOfForLoop;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    bool result = await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Move Submission", Convert.ToString("Submit Move Failed" + Environment.NewLine + ex.Message), "Cancel", "Retry"));

                                    if (result)
                                    {
                                        bSaveComplete = false;
                                        goto endOfForLoop;
                                    }
                                    else
                                    {
                                        if (this.ActDetail.ListDetails.Count > 0)
                                        {
                                            idx--;
                                        }
                                    }
                                }
                            }

                        endOfForLoop:

                            if (this.ActDetail.ListDetails.Count > 0)
                            {
                                bSaveComplete = false;
                            }
                            else
                            {
                                if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Move Submission", "Finished Moves?", "Yes", "No")))
                                {
                                    bSaveComplete = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Validation", "Data Failed Validation. Check Your Data.", "OK"));
                    }
                });
            }
            catch (Exception ex)
            {
                bSaveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.Loading = false;
            }

            return bSaveComplete;
        }

        /// <summary>
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    // this.ProdUnitRefno = 0;

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
                    // this.LoadData();
                    cancelChanges = false;
                    // this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.MoveInventory, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// The GetXML.
        /// </summary>
        /// <returns>.</returns>
        public async Task<XDocument> GetXML()
        {
            XDocument xmlDoc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "</params>");
            return xmlDoc;
        }

        /// <summary>
        /// The OnLayerStatOut.
        /// </summary>
        /// <param name="publisher">.</param>
        /// <param name="dataLayerEventArgs">.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            this.ExtraInfoItems.Clear();

            // Tokenize data longer than 30 characters for display, wrap to next line
            const int tokenLength = 30;

            if (dataLayerEventArgs.StatusOutMsg.Length > tokenLength)
            {
                int iTokens = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dataLayerEventArgs.StatusOutMsg.Length / tokenLength)));
                for (var iTokenIdx = 0; iTokenIdx <= (iTokens * tokenLength) - 1; iTokenIdx++)
                {
                    int iTokenLength = Convert.ToInt32(dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx > tokenLength ? tokenLength : dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx);
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
        /// The OnLayerProgOut.
        /// </summary>
        /// <param name="publisher">.</param>
        /// <param name="dataLayerEventArgs">.</param>
        public void OnLayerProgOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// The cmdFromLocation_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void cmdFromLocation_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                    this.SelFromLocMap = this.LocMap.CurrLocMap.Clone();
                    this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "From: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "Set From Location");
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "Error selecting to location." + Environment.NewLine + ex.Message, "OK"));
            }
            finally
            {
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// The cmdToLocation_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void cmdToLocation_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                    this.SelToLocMap = this.LocMap.CurrLocMap.Clone();
                    this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "To: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set To Location");
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "Error selecting to location." + Environment.NewLine + ex.Message, "OK"));
            }
            finally
            {
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// The ucActivityMoveInventory_GotFocus.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task ucActivityMoveInventory_GotFocus()
        {
            _ = await Device.InvokeOnMainThreadAsync(() => this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);

            if (this.IsControlLoaded == false)
            {
                this.SelFromLocMap = this.LocMap.CurrLocMap.Clone();
                this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "From: " + LocationMapTasks.GetCurrentReadableLocation());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "Set From Location");
                }

                this.SelToLocMap = this.LocMap.CurrLocMap.Clone();
                this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "To: " + LocationMapTasks.GetCurrentReadableLocation());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set To Location");
                }

                await this.RefreshData();
                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        /// The cmdAdd_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void cmdAdd_Click(object sender, EventArgs e)
        {
            int prodUnitRefno = 0;

            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);

                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    XDocument xmlDoc = await form.GetXML();
                    prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                    object infoObject = null;

                    await this.ActItem.Insert(xmlDoc, false);

                    if (this.ActDetail.ListDetails.Count > 0)
                    {
                        infoObject = this.ActDetail.ListDetails[0];
                    }

                    this.ActDetail = await this.ActDetail.GetInfo(infoObject, true);

                    SFillQuantity fillQty = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)this.ActItem.ProdInfo, 1);
                    SActItemSchemeData tempItemSchemeData = new SActItemSchemeData
                    {
                        ActDetailQuantity = fillQty.Quantity,
                        ActDetailBaseQuantity = fillQty.BaseQuantity,
                        ActDetailWeightQuantity = fillQty.WeightQuantity,
                    };

                    bool isInsertReady = false;
                    int foundRowIdx = -1;

                    if (this.ActDetail.ListDetails.Count > 0)
                    {
                        foundRowIdx = this.ActDetail.ListDetails.Count - 1;
                    }

                    this.BarCodeParse = BarCodeParseFacade.ParseData(string.Empty);

                    this.ActDetail.Quantity = 0;
                    this.ActDetail.BaseQuantity = 0;
                    this.ActDetail.WeightQuantity = 0;
                    this.ActDetail.SchemeID = string.Empty;
                    this.ActDetail.UniqueSchemeID = string.Empty;
                    this.ActDetail.ActExtra.Text1 = string.Empty;
                    this.ActDetail.ActExtra.Text2 = string.Empty;
                    this.ActDetail.ActExtra.Text3 = string.Empty;
                    this.ActDetail.ActExtra.Text4 = string.Empty;
                    this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);

                    isInsertReady = await SysShared.GatherProductData(this.BarCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, this.ActDetail, true);
                    await this.InsertActivityDetail(foundRowIdx, this.ActDetail, this.BarCodeParse, isInsertReady);
                }
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                });

                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// The InsertActivityDetail.
        /// </summary>
        /// <param name="rowIdx">The rowIdx<see cref="int"/>.</param>
        /// <param name="actDetail">.</param>
        /// <param name="parse">The parse<see cref="BarCodeParse"/>.</param>
        /// <param name="isInsertDetailReady">The isInsertDetailReady<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task InsertActivityDetail(int rowIdx, ActDetail actDetail, BarCodeParse parse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);
                await this.ActDetail.Insert(xmlDoc);

                if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > this.ActDetail.ListDetails.Count - 1)
                {
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.ActDetail.ListDetails.Count - 1];
                }
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
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            this.Loading = true;
            XDocument xmlDoc = null;
            int prodUnitRefno = 0;
            // int rowIdx = this.ListViewMainActivity.CurrentRowIndex;
            int rowIdx = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);

            try
            {
                this.BarCodeParse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                if (!string.IsNullOrEmpty(this.BarCodeParse.LocationIdentifier))
                {
                    if (this.chkScanToLocation.IsChecked)
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.BarCodeParse.LocationIdentifier));
                        this.SelToLocMap = this.LocMap.CurrLocMap.Clone();

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "To: " + LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set To Location");
                        }
                    }
                    else
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.BarCodeParse.LocationIdentifier));
                        this.SelFromLocMap = this.LocMap.CurrLocMap.Clone();

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "From: " + LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "Set From Location");
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.BarCodeParse.ProductIdentifier))
                    {
                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarCodeParse);

                        xmlDoc = XDocument.Parse(
                              "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                              + "<params>"
                              + "<ProductUnitRefno>" + prodUnitRefno + "</ProductUnitRefno>"
                              + "</params>");

                        await this.ActItem.Insert(xmlDoc, false);

                        object infoObject = null;

                        if (this.ActDetail.ListDetails.Count > 0)
                        {
                            infoObject = this.ActDetail.ListDetails[0];
                        }

                        this.ActDetail = await this.ActDetail.GetInfo(infoObject, true);

                        SActItemSchemeData tempItemSchemeData = new SActItemSchemeData();
                        bool isInsertReady = false;

                        int foundRowIdx = -1;

                        if (this.ActDetail.ListDetails.Count > 0)
                        {
                            foundRowIdx = this.ActDetail.ListDetails.Count - 1;
                        }

                        isInsertReady = await SysShared.GatherProductData(this.BarCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, this.ActDetail, false);
                        await this.InsertActivityDetail(foundRowIdx, this.ActDetail, this.BarCodeParse, isInsertReady);
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
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));

                if (this.ActDetail.ListDetails.Count > 0 && rowIdx >= 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.ListViewMainActivity.SelectedItem = this.MainActivityItems[rowIdx]);
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });

                if (this.ActDetail.ListDetails.Count > 0 && rowIdx >= 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.ListViewMainActivity.SelectedItem = this.MainActivityItems[rowIdx]);
                }
            }
            finally
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                });

                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// The RefreshData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task RefreshData()
        {
            await this.ScannerDisable();
            _ = await this.ActDetail.GetActivityDetailList(false);

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                this.ExtraInfoItems.Clear();

                this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > 0)
                {
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                    await this.ActDetail.SelectActivityDetail(this.ActDetail.ListDetails[this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)]);
                }
            });

            await this.ScannerEnable();
        }

        /// <summary>
        /// The OptionsMenu_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            string result = await this.DisplayActionSheet("Move Inventory Options", "Close", null, this.optionsMenuItems.ToArray());

            if (result == "Save")
            {
                if (await this.SaveChanges())
                {
                    await this.Close();
                }
            }
            else if (result == "Cancel")
            {
                if (await this.CancelChanges())
                {
                    await this.Close();
                }
            }
        }
    }
}
