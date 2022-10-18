// <copyright file="ActivityItemSingle.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity;
using dMobile.Controller.Activity_Detail;
using dMobile.Controller.Activity_Item;

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using static Android.Views.View;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="ActivityItemSingle" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItemSingle : DMobileModal, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        /// Defines the isShowDataValidMsg.
        /// </summary>
        private bool isShowDataValidMsg = true;

        /// <summary>
        /// Defines the cmdLocationText.
        /// </summary>
        private string cmdLocationText;

        /// <summary>
        /// Defines the cmdLocationIsEnabled.
        /// </summary>
        private bool cmdLocationIsEnabled;

        /// <summary>
        /// Defines the cmdAddSkidText.
        /// </summary>
        private string cmdAddSkidText;

        /// <summary>
        /// Defines the cmdAddSkidIsEnabled.
        /// </summary>
        private bool cmdAddSkidIsEnabled;

        /// <summary>
        /// Defines the cmdSelectSkidText.
        /// </summary>
        private string cmdSelectSkidText;

        /// <summary>
        /// Defines the cmdSelectSkidIsEnabled.
        /// </summary>
        private bool cmdSelectSkidIsEnabled;

        /// <summary>
        /// Defines the cmdFilterText.
        /// </summary>
        private string cmdFilterText;

        /// <summary>
        /// Defines the cmdFilterIsEnabled.
        /// </summary>
        private bool cmdFilterIsEnabled;

        /// <summary>
        /// Defines the cmdAddProductText.
        /// </summary>
        private string cmdAddProductText;

        /// <summary>
        /// Defines the cmdAddProductIsEnabled.
        /// </summary>
        private bool cmdAddProductIsEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItemSingle"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        /// <param name="actSubTypes">.</param>
        public ActivityItemSingle(Controller.Activity.Activity activity, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.cmdLocationText = "Set Location";
            this.cmdDetails.Text = "Details";
            this.cmdNext.Text = "Next";
            this.lblOrderedQuantity.Text = "Ordered";
            this.lblShippedQuantity.Text = "Shipped";
            this.lblProductBCUnit.Text = "Product";
            this.cmdAddSkidText = "Add Skid";
            this.cmdSelectSkidText = "Select Skid";
            this.cmdFilterText = "Filter";
            this.cmdAddProductText = "Add Prod";
            this.cmdPrevious.Text = "Prev";
            this.lblIsItemComment.Text = "C";

            this.LocMap = LocationMap.GetInstance();
            this.SysEnv = SysEnvironment.GetInstance();

            this.ActSubType = actSubTypes;

            // Init activity header object
            this.ActHdr = activity;

            if (this.ActHdr.SkidInfo == null)
            {
                this.ActHdr.SkidInfo = new SysSkidInfo();
            }

            // Init activity item object
            this.ActItem = ActItemFacade.GetActItem(this.ActHdr, this.ActSubType);
            this.ActItem.ActHdr = this.ActHdr;

            // Init bar code modifier
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

            // Init scanner
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.Title = this.GetControlDescription();
        }

        /// <summary>
        /// Check if options in displayActionSheet are enabled/disabled and rebuild list.
        /// </summary>
        /// <param name="propertyName">.</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (
                propertyName == nameof(this.cmdLocationText) ||
                propertyName == nameof(this.cmdLocationIsEnabled) ||
                propertyName == nameof(this.cmdFilterText) ||
                propertyName == nameof(this.cmdFilterIsEnabled) ||
                propertyName == nameof(this.cmdSelectSkidText) ||
                propertyName == nameof(this.cmdSelectSkidIsEnabled) ||
                propertyName == nameof(this.cmdAddProductText) ||
                propertyName == nameof(this.cmdAddProductIsEnabled) ||
                propertyName == nameof(this.cmdAddSkidText) ||
                propertyName == nameof(this.cmdAddSkidIsEnabled))
            {
                this.PopulateOptionsMenu();
            }
        }

        /// <summary>
        /// Populates a string array to be used in DisplayActionSheet.
        /// </summary>
        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();

            if (this.cmdLocationIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdLocationText);
            }

            if (this.cmdFilterIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdFilterText);
            }

            if (this.cmdSelectSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdSelectSkidText);
            }

            if (this.cmdAddProductIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdAddProductText);
            }

            if (this.cmdAddSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdAddSkidText);
            }
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

                if (!this.Loading)
                {
                    if (keycode == Keycode.Enter || keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            await this.Close();
                        }
                    }
                    else if (keycode == Keycode.F2)
                    {
                        await this.KeyUpF2_ShowLineItemComment(this, null);
                    }
                    else if (keycode == Keycode.F3)
                    {
                        await this.KeyUpF3_ShowTotalQuantity(this, null);
                    }
                }
            });

            try
            {
                await this.ucActivityItem_GotFocus(this, new EventArgs());
                await this.tmrActivityItemLoaded_Tick(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityItem_GotFocus threw an error " + ex, "OK"));
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
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowDataValidMsg.
        /// </summary>
        private bool IsShowDataValidMsg
        {
            get
            {
                return this.isShowDataValidMsg;
            }

            set
            {
                this.isShowDataValidMsg = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsChangesMade.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the Scanner.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = null;

        /// <summary>
        /// Gets or sets the FullClassName.
        /// </summary>
        private string FullClassName { get; set; } = "dMobile.View240x320.ucActivityItemSingle";

        /// <summary>
        /// Gets or sets the ActHdr.
        /// </summary>
        private Controller.Activity.Activity ActHdr { get; set; }

        /// <summary>
        /// Gets or sets the LocMap.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        /// Gets or sets the ActItem.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRefreshed.
        /// </summary>
        private bool IsRefreshed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsControlLoaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets the TotalTicks.
        /// </summary>
        private int TotalTicks { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether IsBarCodeHandlerRegistered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the ExtraInfoItems.
        /// </summary>
        public ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the OptionsMenuItems.
        /// </summary>
        public List<string> OptionsMenuItems { get; set; } = new List<string>();

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
            var isReadyToSave = true;

            if (isReadyToSave)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.IsItemReviewed();

                        // Added extra check for Lekker, check all items.
                        this.IsAllItemsReviewed();
                    }
                    catch (MessageOnlyException ex)
                    {
                        isReadyToSave = false;
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        isReadyToSave = false;

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                }
            }

            if (isReadyToSave)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.ItemQuantityUnderCheck();

                        // Added extra check for Lekker, check all items.
                        this.AllItemsQuantityUnderCheck();
                    }
                    catch (MessageOnlyException ex)
                    {
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                                if (await Device.InvokeOnMainThreadAsync(async () => !await this.DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                                {
                                    isReadyToSave = false;
                                    this.IsShowDataValidMsg = false;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                // MessageBox.Show(ex.Message,"Quantity Validation",MessageBoxButtons.OK,MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                                // TOOD: Resource strings
                                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Quantity Validation", ex.Message, "OK"));
                                isReadyToSave = false;
                                this.IsShowDataValidMsg = false;

                                break;

                            default:
                                throw new NotImplementedException("Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                        }
                    }
                    catch (Exception ex)
                    {
                        isReadyToSave = false;

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
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
            var isReadyToClose = true;

            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            try
            {
                await this.ScannerDisable();
            }
            catch (Exception ex)
            {
                isReadyToClose = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            // Lekker requirement to check reviewed even though we are cancelling / closing.
            if (isReadyToClose)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.IsItemReviewed();

                        // Added extra check for Lekker, check all items.
                        this.IsAllItemsReviewed();
                    }
                    catch (MessageOnlyException ex)
                    {
                        isReadyToClose = false;
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        isReadyToClose = false;

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                }
            }

            // Lekker requirement to check quantity under even though we are cancelling / closing.
            if (isReadyToClose)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.IsShowDataValidMsg = true;
                        this.ItemQuantityUnderCheck();

                        // Added extra check for Lekker, check all items.
                        this.AllItemsQuantityUnderCheck();
                    }
                    catch (MessageOnlyException ex)
                    {
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                                if (await Device.InvokeOnMainThreadAsync(async () => !await this.DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                                {
                                    isReadyToClose = false;
                                    this.IsShowDataValidMsg = false;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Quantity Validation", ex.Message, "OK"));
                                isReadyToClose = false;
                                this.IsShowDataValidMsg = false;

                                break;

                            default:

                                throw new NotImplementedException("Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                        }
                    }
                    catch (Exception ex)
                    {
                        isReadyToClose = false;

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                }
            }

            return isReadyToClose;
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            string desc;
            switch (this.ActSubType)
            {
                case SysCommon.ActSubTypes.PhysicalCounts:

                    desc = string.Format("Count # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.Receiving:

                    desc = string.Format("Receive # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.Picking:

                    desc = string.Format("Pick # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferFrom:

                    desc = string.Format("Transfer from # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferTo:

                    desc = string.Format("Transfer to # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.Returns:

                    desc = string.Format("Return # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.CycleCounts:

                    desc = string.Format("Count # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.Adjustment:

                    desc = string.Format("Adjustment # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketFinished:

                    desc = string.Format("Ticket Finished # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:

                    desc = string.Format("Ticket Material # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:

                    desc = string.Format("Ticket Add-Product # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                case SysCommon.ActSubTypes.PickingSingle:

                    desc = string.Format("Pick # {0}", this.ActItem.ActHdr.HdrFKRefno);
                    break;

                default:

                    desc = "Activity Items";
                    break;
            }

            return desc;
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
                if (await this.IsReadyToSave())
                {
                    if (this.ActItem.SelectedActivityItem != null && this.ActItem.SelectedActivityItem == this.ActItem.ListItems.Last())
                    {
                        await this.FullQuantityShippingLabels();
                    }

                    if (this.ActItem.SelectedActivityItem != null)
                    {
                        // update the header record as reviewed if all items were reviewed
                        var isReviewed = true;

                        foreach (PickingItem pickingItem in this.ActItem.ListItems)
                        {
                            isReviewed = isReviewed && Convert.ToBoolean(pickingItem.ActivityItemPickingIsReviewed);
                        }

                        (this.ActHdr.SelectedActivity as Picking).ActivityPickingIsReviewed = isReviewed;
                    }

                    // If any ship quantity exists do not clear the picker. Keep it locked.
                    var hasNotBeenPicked = true;

                    foreach (PickingItem pickingItem in this.ActItem.ListItems)
                    {
                        if (Convert.ToDecimal(pickingItem.ActivityItemPickingBaseQuantityShipped) != 0)
                        {
                            hasNotBeenPicked = false;
                            break;
                        }
                    }

                    if (hasNotBeenPicked)
                    {
                        if (!await ((PickingActivity)this.ActHdr).ClearSalesOrderBeingPicked(true))
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Clear Picker", "Unable to clear the current picker.", "OK"));
                        }
                    }

                    // Print all receiving skid labels.
                    await this.PrintSkidLabels();

                    this.IsChangesMade = false;
                    isSaveComplete = true;
                }
                else
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catch all
                        // in the event that IsReadyToSave just returns false.
                        throw new MessageOnlyException("Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }
            }
            catch (MessageOnlyException ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isSaveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isSaveComplete;
        }

        /// <summary>
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = false;

            try
            {
                if (await this.IsReadyToClose())
                {
                    if (this.ActItem.SelectedActivityItem != null && this.ActItem.SelectedActivityItem == this.ActItem.ListItems.Last())
                    {
                        await this.FullQuantityShippingLabels();
                    }

                    // Lekker requirement, cancel will update the reviewed header status.
                    // update the header record as reviewed if all items were reviewed
                    var isReviewed = true;

                    foreach (PickingItem pickingItem in this.ActItem.ListItems)
                    {
                        isReviewed = isReviewed && Convert.ToBoolean(pickingItem.ActivityItemPickingIsReviewed);
                    }

                    (this.ActHdr.SelectedActivity as Picking).ActivityPickingIsReviewed = isReviewed;

                    // Clear any properties.
                    cancelChanges = true;

                    // If any ship quantity exists do not clear the picker. Keep it locked.
                    var hasNotBeenPicked = true;

                    if (this.ActItem.SelectedActivityItem != null)
                    {
                        foreach (PickingItem pickingItem in this.ActItem.ListItems)
                        {
                            if (Convert.ToDecimal(pickingItem.ActivityItemPickingBaseQuantityShipped) != 0)
                            {
                                hasNotBeenPicked = false;
                                break;
                            }
                        }
                    }

                    if (hasNotBeenPicked)
                    {
                        if (!await ((PickingActivity)this.ActHdr).ClearSalesOrderBeingPicked(true))
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Clear Picker", "Unable to clear the current picker.", "OK"));
                        }
                    }
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
                    this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
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
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// The OnLayerStatOut.
        /// </summary>
        /// <param name="publisher">.</param>
        /// <param name="dataLayerEventArgs">.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
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
        /// The PrintSkidLabels.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task PrintSkidLabels()
        {
            // Print all receiving skid labels.
            if (this.ActSubType == SysCommon.ActSubTypes.Receiving && !this.SysEnv.SysSettings.IsDisconnectedMode && this.SysEnv.SysSettings.IsSkidTrackingModule && this.SysEnv.SysSettings.AskRecvPrintAllSkidLabelsUponSave)
            {
                if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Skid Labels", "Do you wish to print all skid label(s)?", "Yes", "No")))
                {
                    var xmlString = new StringBuilder();
                    var xml = XmlWriter.Create(xmlString);
                    var request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<command>"
                        + "<action>SUBMIT PRINT ALL SKID LABELS</action>"
                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                        + "<fk_refno>" + this.ActHdr.HdrFKRefno + "</fk_refno>"
                        + "<fk_source>PO</fk_source>"
                        + "</command>"
                        + "</imclient>");

                    request.Save(xml);
                    xml.Flush();
                    xml.Close();

                    var reqData = SysCommon.SubmitComm(xmlString.ToString());

                    if (reqData.Length > 0)
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);

                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                            if (errorDetail.Contains("MSGBOX-"))
                            {
                                throw new MessageOnlyException("Print all skid label(s) error." + Environment.NewLine + errorDetail);
                            }
                            else
                            {
                                throw new Exception("Print all skid label(s) error." + Environment.NewLine + errorDetail);
                            }
                        }
                    }
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

            // TODO: Test barcodehandler
            var msg = string.Empty;
            var prodUnitRefno = 0;
            var currentSelectedActivityItem = this.ActItem.SelectedActivityItem;
            var foundSelectedActivityItem = this.ActItem.SelectedActivityItem;
            var actItemFoundRefno = 0;
            BarCodeParse barCodeParse = null;
            var tempItemSchemeData = new ActItemSchemeData();

            try
            {
                await this.ScannerDisable();

                barCodeParse = scanArgs.ParseInfo;

                // Global location identifier.
                if (!string.IsNullOrEmpty(barCodeParse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(barCodeParse.LocationIdentifier));

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                    }
                    else
                    {
                        this.cmdLocationText = "Set Location";
                    }
                }
                else
                {
                    if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) &&
                        !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc &&
                        !(this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                        this.ActSubType == SysCommon.ActSubTypes.Receiving))
                    {
                        throw new MessageOnlyException("Location must be set first.");
                    }

                    // Product identifier.
                    if (!string.IsNullOrEmpty(barCodeParse.ProductIdentifier))
                    {
                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.
                        ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                        ////    this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////{
                        ////    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////    {
                        ////        this.ActHdr.SkidInfo = new SysSkidInfo();
                        ////        this.cmdSelectSkid.Text = string.Empty;
                        ////    }
                        ////}

                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, barCodeParse);
                        var tempProdInfo = new ProductInfo();
                        tempProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        if (this.SysEnv.SysSettings.IsPickSingleScanProdMustMatchSelProd)
                        {
                            if (this.ActItem.ProdInfo.UnitRefno != prodUnitRefno)
                            {
                                throw new MessageOnlyException(
                                    string.Format(
                                    "Scanned product code \'{0}\' does not match the currently selected one \'{1}\'",
                                     ProductInfoTasks.GetCurrReadableProdBCUnit(tempProdInfo),
                                     ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActItem.ProdInfo)));
                            }
                        }

                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                        // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                        if (actItemFoundRefno > 0)
                        {
                            // foundSelectedActivityItem = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            foundSelectedActivityItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                var isAddNewItem = false;

                                if (this.ActItem.IsAskCreateNewItem)
                                {
                                    if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                        this.GetControlDescription(),
                                        string.Format("A line item does not exist for this warehouse {0} and product \'{1}.\'" + Environment.NewLine + "Do you wish to create one?",
                                        this.LocMap.CurrLocMap.WhsCode,
                                        ProductInfoTasks.GetCurrReadableProdBCUnit(tempProdInfo)),
                                        "Yes",
                                        "No")))
                                    {
                                        isAddNewItem = true;
                                    }
                                }
                                else
                                {
                                    isAddNewItem = true;
                                }

                                if (isAddNewItem)
                                {
                                    await this.ActItem.Insert(ProductInfoTasks.GetXML(tempProdInfo), false);

                                    // This is a little dangerous we assume that the new row idx will be the last one.
                                    foundSelectedActivityItem = this.ActItem.ListItems.Last();
                                    actItemFoundRefno = this.ActItem.ItemRefno;
                                }
                                else
                                {
                                    throw new GatherProductDataException(this.ActItem.GetItemNotFoundMessage(this.LocMap, tempProdInfo), true);
                                }
                            }
                            else
                            {
                                msg = string.Format("Line item was not found for " + Environment.NewLine + "Location {0} " + Environment.NewLine + "Product {1} BC {2} UOM {3}",
                                    LocationMapTasks.GetCurrentReadableLocation(),
                                    tempProdInfo.Code,
                                    tempProdInfo.UnitBC,
                                    tempProdInfo.UnitCode);

                                throw new MessageOnlyException(msg);
                            }
                        }
                    }

                    // Activity item fk refno
                    if (!string.IsNullOrEmpty(barCodeParse.ActItemFKRefno) && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // this.ActItem.DataViewItems.Sort = this.ActItem.GetDataViewSortFKRefnoIdx();
                        if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            throw new MessageOnlyException("Scanning an line item bar code is not supported here.");
                        }
                        else
                        {
                            // foundSelectedActivityItem = this.ActItem.DataViewItems.Find(scanArgs.ParseInfo.ActItemFKRefno);
                            foundSelectedActivityItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty(this.ActItem.GetDataViewSortFKRefnoIdx())?.GetValue(activityItem)) == Convert.ToString(scanArgs.ParseInfo.ActItemFKRefno)).FirstOrDefault();

                            if (foundSelectedActivityItem != null)
                            {
                                await this.ActItem.AssignActivityItem(foundSelectedActivityItem);
                                actItemFoundRefno = this.ActItem.ItemRefno;
                                // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                // foundSelectedActivityItem = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                                foundSelectedActivityItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                            }
                            else
                            {
                                throw new MessageOnlyException(this.ActItem.GetUnableToFindItemFKRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                            }
                        }
                    }

                    // Activity detail fk refno
                    if (!string.IsNullOrEmpty(barCodeParse.ActDetailFKRefno) && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            throw new MessageOnlyException("Scanning an line item detail bar code is not supported here.");
                        }
                        else
                        {
                            // this.ActItem.DataViewItems.Sort = this.ActItem.GetDataViewSortFKRefnoIdx();
                            tempItemSchemeData = await this.ActItem.GetProdSchemeDataFromDetailFKRefno(int.Parse(barCodeParse.ActDetailFKRefno));
                            // foundSelectedActivityItem = this.ActItem.DataViewItems.Find(tempItemSchemeData.ActItemFKRefno.ToString());
                            foundSelectedActivityItem = this.ActItem.ListItems.OfType<object>().Where(activityItem =>
                            Convert.ToString(activityItem.GetType().GetProperty(this.ActItem.GetDataViewSortFKRefnoIdx())?.GetValue(activityItem)) == Convert.ToString(tempItemSchemeData.ActItemFKRefno.ToString())).FirstOrDefault();

                            if (foundSelectedActivityItem != null)
                            {
                                await this.ActItem.AssignActivityItem(foundSelectedActivityItem);
                                actItemFoundRefno = this.ActItem.ItemRefno;
                                // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                // foundSelectedActivityItem = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                                foundSelectedActivityItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                            }
                            else
                            {
                                throw new MessageOnlyException(this.ActItem.GetUnableToFindItemFKRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(barCodeParse.ActDetailFKRefno) && this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        throw new MessageOnlyException("Scanning an line item detail bar code is not supported here in disconnected mode.");
                    }

                    // Shipping containser scanned, skid
                    if (!string.IsNullOrEmpty(barCodeParse.SSCC18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            throw new MessageOnlyException("Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            throw new MessageOnlyException("Scanning a shipping container bar code is not supported in disconnected mode.");
                        }

                        this.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(barCodeParse.SSCC18);

                        if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            throw new MessageOnlyException("Skid does not exist.");
                        }

                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                        // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                        if (actItemFoundRefno > 0)
                        {
                            // foundSelectedActivityItem = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            foundSelectedActivityItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(ProductInfoTasks.GetXML(this.ActHdr.SkidInfo.CurrSkid.ProdInfo), false);

                                // This is a little dangerous we assume that the new row idx will be the last one
                                foundSelectedActivityItem = this.ActItem.ListItems.Last();
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                msg = string.Format(
                                    "Item not found for " + Environment.NewLine
                                    + "Location {0} " + Environment.NewLine + "Product {1} BC {2} Unit {3}",
                                    LocationMapTasks.GetCurrentReadableLocation(),
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Code,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitBC,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitCode);

                                throw new MessageOnlyException(msg);
                            }
                        }
                    }

                    // If an activity line item was found
                    if (actItemFoundRefno == 0 || foundSelectedActivityItem == null)
                    {
                        throw new MessageOnlyException("Line item was not found.");
                    }
                    else
                    {
                        await this.ActItem.AssignActivityItem(foundSelectedActivityItem);
                    }

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != this.ActItem.ProdInfo.UnitRefno)
                        {
                            throw new MessageOnlyException("Identified line item product does not match the selected skid product.");
                        }
                    }

                    if (string.IsNullOrEmpty(barCodeParse.SSCC18))
                    {
                        // Rule: check if the current location and product home location are the same
                        if (this.SysEnv.SysSettings.IsVerifyProductLocation &&
                            (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                            this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                            this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                            this.ActSubType == SysCommon.ActSubTypes.TransferTo))
                        {
                            // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                            int locMapRefno = await this.ActItem.GetDefaultLocMapRefno(Convert.ToInt32((foundSelectedActivityItem as dynamic).LocationMapRefno), Convert.ToInt32((foundSelectedActivityItem as dynamic).ProductRefno));

                            if (this.LocMap.CurrLocMapRefno != locMapRefno)
                            {
                                throw new MessageOnlyException("The current location does not match the product home location.");
                            }
                        }

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.
                        ////                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                        ////                            || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////                        {
                        ////                            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////                            {
                        ////                                this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////                this.cmdSelectSkid.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////                                this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////                            }
                        ////                        }

                        var isInsertReady = false;

                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);

                        // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                        // and the fact is isn't human readable friendly.
                        var schemeData = actDetail.SchemeData;

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                            schemeData.LogisticUnitBatchLotNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                            schemeData.LogisticUnitBestBeforeSellByDate = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                            schemeData.LogisticUnitProductionDate = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                            schemeData.VariantCode = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                            schemeData.VariantNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                            schemeData.VariantDescription = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                        }

                        tempItemSchemeData.ProdSchemeData = schemeData;
                        actDetail.SchemeData = schemeData;

                        isInsertReady = await SysShared.GatherProductData(barCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, actDetail, false);
                        await this.InsertActivityDetail(foundSelectedActivityItem, actDetail, barCodeParse, isInsertReady);
                    }
                    else
                    {
                        // Insert a new activity detail record
                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        var scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        var unqScheme = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        var xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                            + "<alt_qty>" + this.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                            + "<wt_qty>" + this.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                            + "<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>"
                            + "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);

                        await actDetail.InsertSkid(this.ActHdr.SkidInfo);

                        // Upon a save of completed data update the data view to to show new item and quantity change
                        this.ActItem.AddToQuantityInActivityItem(foundSelectedActivityItem, actDetail.Quantity, actDetail.BaseQuantity, actDetail.WeightQuantity);
                        currentSelectedActivityItem = foundSelectedActivityItem;
                        await this.ActItem.UpdateItemTableQuantity(currentSelectedActivityItem);
                    }

                    await this.DisplayData();

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        this.cmdAddSkidIsEnabled = true;

                        this.cmdSelectSkidText = Convert.ToString("Skid# " + this.ActHdr.SkidInfo.CurrSkid.SkidCode);
                    }
                    else
                    {
                        this.cmdAddSkidIsEnabled = false;

                        this.cmdSelectSkidText = "Select Skid";
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (IsReadyToApplyException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }

                if (foundSelectedActivityItem != null)
                {
                    this.ActItem.SelectedActivityItem = foundSelectedActivityItem;
                }
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }

                if (foundSelectedActivityItem != null)
                {
                    this.ActItem.SelectedActivityItem = foundSelectedActivityItem;
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));

                if (foundSelectedActivityItem != null)
                {
                    this.ActItem.SelectedActivityItem = foundSelectedActivityItem;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });

                if (foundSelectedActivityItem != null)
                {
                    this.ActItem.SelectedActivityItem = foundSelectedActivityItem;
                }
            }
            finally
            {
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// The DisplayData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task DisplayData()
        {
            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.cmdAddSkidIsEnabled = true;
                    this.cmdLocationIsEnabled = false;

                    this.cmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                });
            }
            else
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.cmdLocationIsEnabled = true;
                    this.cmdAddSkidIsEnabled = false;

                    this.cmdSelectSkidText = "Select Skid";
                });
            }

            if (this.LocMap.CurrLocMap.WhsLocMapRefno > 0)
            {
                var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
            }
            else
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Set Location");
            }

            _ = await Device.InvokeOnMainThreadAsync(() => this.lblProductBCUnit.Text = "Product: " + ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActItem.ProdInfo));

            var selectedActivityItem = this.ActItem.SelectedActivityItem as PickingItem;

            if (selectedActivityItem != null)
            {
                var ordQty = Convert.ToDecimal(selectedActivityItem.ActivityItemPickingQuantityOrdered);
                var ordWtQty = Convert.ToDecimal(selectedActivityItem.ActivityItemPickingWeightQuantityOrdered);

                _ = await Device.InvokeOnMainThreadAsync(() => this.lblOrderedQuantity.Text = "Ordered: " + SysCommon.NiceQuantityReadableString(this.ActItem.ProdInfo, ordQty, ordWtQty));

                var shpQty = Convert.ToDecimal(selectedActivityItem.ActivityItemPickingQuantityShipped);
                var shpWtQty = Convert.ToDecimal(selectedActivityItem.ActivityItemPickingWeightQuantityShipped);

                _ = await Device.InvokeOnMainThreadAsync(() => this.lblShippedQuantity.Text = "Shipped: " + SysCommon.NiceQuantityReadableString(this.ActItem.ProdInfo, shpQty, shpWtQty));

                if (Convert.ToString(selectedActivityItem.IsActivityItemPickingComment) == "*")
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.lblIsItemComment.Text = "C");
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.lblIsItemComment.Text = string.Empty);
                }

                selectedActivityItem.ActivityItemPickingIsReviewed = true;

                if (this.SysEnv.SysSettings.IsPickPopupCmnt)
                {
                    if (Convert.ToString(selectedActivityItem.IsActivityItemPickingComment) == "*")
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Comment", await this.ActItem.GetComment(this.ActItem.ItemRefno), "OK"));
                    }
                }

                var locCode = Convert.ToString(selectedActivityItem.LocationCode);
                var locName = Convert.ToString(selectedActivityItem.LocationName);
                var prodDesc = Convert.ToString(selectedActivityItem.ProductDescription);
                var prodUnitSizeDesc = Convert.ToString(selectedActivityItem.ProductUnitSizeDescription);
                var prodBrandDesc = Convert.ToString(selectedActivityItem.ProductBrandDescription);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ExtraInfoItems.Clear();
                    this.ExtraInfoItems.Add("DESC: " + prodDesc.Trim());
                    this.ExtraInfoItems.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " + prodBrandDesc.Trim());
                    this.ExtraInfoItems.Add("LOCATION: " + locCode.Trim() + " " + locName.Trim());
                });
            }

            if (selectedActivityItem == this.ActItem.ListItems.Last())
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdNext.IsEnabled = false);
            }
            else
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdNext.IsEnabled = true);
            }

            if (selectedActivityItem == this.ActItem.ListItems.First() || this.ActItem.ListItems.Count == 0)
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdPrevious.IsEnabled = false);
            }
            else
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdPrevious.IsEnabled = true);
            }
        }

        /// <summary>
        /// The InsertActivityDetail.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="actDetail">.</param>
        /// <param name="barCodeParse">.</param>
        /// <param name="isInsertDetailReady">The isInsertDetailReady<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task InsertActivityDetail(object selectedActivityItem, ActDetail actDetail, BarCodeParse barCodeParse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                var xmlDoc = actDetail.GetXML(actDetail);
                await actDetail.Insert(xmlDoc);

                // Upon a save of completed data update the data view to to show new item
                // and quantity change
                this.ActItem.AddToQuantityInActivityItem(selectedActivityItem, actDetail.Quantity, actDetail.BaseQuantity, actDetail.WeightQuantity);
                await this.ActItem.UpdateItemTableQuantity(selectedActivityItem);
            }

            this.ActItem.SelectedActivityItem = selectedActivityItem;

            await this.DisplayData();
        }

        /// <summary>
        /// The IsItemReviewed.
        /// </summary>
        private void IsItemReviewed()
        {
            var isReviewed = true;

            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                if (this.ActItem.SelectedActivityItem is PickingItem pickingItem)
                {
                    isReviewed = pickingItem.ActivityItemPickingIsReviewed;
                }

                if (!isReviewed)
                {
                    throw new MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// The IsAllItemsReviewed.
        /// </summary>
        private void IsAllItemsReviewed()
        {
            var isReviewed = true;

            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                foreach (PickingItem pickingItem in this.ActItem.ListItems)
                {
                    isReviewed = isReviewed && pickingItem.ActivityItemPickingIsReviewed;
                }

                if (!isReviewed)
                {
                    throw new MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// Perform quantity under check for ALL line items.
        /// </summary>
        private void AllItemsQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                foreach (PickingItem pickingItem in this.ActItem.ListItems)
                {
                    var available = pickingItem.ActivityItemPickingBaseQuantityAvailable;
                    var shipped = pickingItem.ActivityItemPickingQuantityShipped;
                    var ordered = pickingItem.ActivityItemPickingQuantityOrdered;
                    if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                    {
                        // Do not warn if product is out of stock WO# 12054
                        if (available > 0 && shipped < ordered)
                        {
                            throw new MessageOnlyException("Some line items exists where the shipped quantity is less that the quantity ordered.");
                        }
                    }
                    else
                    {
                        if (shipped < ordered)
                        {
                            throw new MessageOnlyException("Some line items exists where the shipped quantity is less that the quantity ordered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The FullQuantityShippingLabels.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task FullQuantityShippingLabels()
        {
            if (this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity)
            {
                // Only print full cases not repacks
                if (this.ActItem.ProdInfo.BoxCount < 2)
                {
                    var labelQuantity = new SLabelQuantity();
                    labelQuantity = await this.ActItem.GetLabelQuantity(!this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity);

                    var totalQty = labelQuantity.FullQuantity
                                   + labelQuantity.FreshQuantity
                                   + labelQuantity.FrozenQuantity
                                   + labelQuantity.DryQuantity
                                   + labelQuantity.LiveQuantity;

                    // GetLabelQuantity will also tell if the line item has been printed already.
                    if (!labelQuantity.IsPrinted)
                    {
                        if (totalQty > this.SysEnv.SysSettings.PickSingleShipLabelStopThresholdQuantity && this.SysEnv.SysSettings.PickSingleShipLabelStopThresholdQuantity > 0)
                        {
                            var formLblQty = new FormShipLabelQuantity();
                            {
                                if (await formLblQty.ShowDialog() == DialogResult.OK)
                                {
                                    // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                    // If itm_refno is zero then no line is updated as printed.
                                    ////adjLabel.Element("params").Element("whs_id").AddAfterSelf(
                                    ////    new XElement("itm_refno", this.ActItem.FKRefno));

                                    // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                    // If itm_refno is zero then no line is updated as printed.
                                    var request = XDocument.Parse(
                                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                        + "<params>"
                                        + await SysCommon.GetRequestDefaults(false)
                                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                                        + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                                        + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                                        + "<full_alt_qty>" + labelQuantity.FullQuantity + "</full_alt_qty>"
                                        + "<repack_alt_qty>0</repack_alt_qty>"
                                        + "<fresh_alt_qty>" + labelQuantity.FreshQuantity + "</fresh_alt_qty>"
                                        + "<frozen_alt_qty>" + labelQuantity.FrozenQuantity + "</frozen_alt_qty>"
                                        + "<dry_alt_qty>" + labelQuantity.DryQuantity + "</dry_alt_qty>"
                                        + "<live_alt_qty>" + labelQuantity.LiveQuantity + "</live_alt_qty>"
                                        + "<pallet_alt_qty>" + labelQuantity.PalletQuantity + "</pallet_alt_qty>"
                                        + "<repack_fresh_alt_qty>0</repack_fresh_alt_qty>"
                                        + "<repack_frozen_alt_qty>0</repack_frozen_alt_qty>"
                                        + "<repack_dry_alt_qty>0</repack_dry_alt_qty>"
                                        + "<repack_live_alt_qty>0</repack_live_alt_qty>"
                                        + "<bundle_alt_qty>" + labelQuantity.BundleQuantity + "</bundle_alt_qty>"
                                        + "<total_alt_qty>" + formLblQty.Quantity + "</total_alt_qty>"
                                        + "</params>");

                                    await this.ActHdr.PrintLabel(request);
                                }
                            }
                        }
                        else
                        {
                            // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                            // If itm_refno is zero then no line is updated as printed.
                            var request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<params>"
                                + await SysCommon.GetRequestDefaults(false)
                                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                                + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                                + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                                + "<full_alt_qty>" + labelQuantity.FullQuantity + "</full_alt_qty>"
                                + "<repack_alt_qty>0</repack_alt_qty>"
                                + "<fresh_alt_qty>" + labelQuantity.FreshQuantity + "</fresh_alt_qty>"
                                + "<frozen_alt_qty>" + labelQuantity.FrozenQuantity + "</frozen_alt_qty>"
                                + "<dry_alt_qty>" + labelQuantity.DryQuantity + "</dry_alt_qty>"
                                + "<live_alt_qty>" + labelQuantity.LiveQuantity + "</live_alt_qty>"
                                + "<pallet_alt_qty>" + labelQuantity.PalletQuantity + "</pallet_alt_qty>"
                                + "<repack_fresh_alt_qty>0</repack_fresh_alt_qty>"
                                + "<repack_frozen_alt_qty>0</repack_frozen_alt_qty>"
                                + "<repack_dry_alt_qty>0</repack_dry_alt_qty>"
                                + "<repack_live_alt_qty>0</repack_live_alt_qty>"
                                + "<bundle_alt_qty>" + labelQuantity.BundleQuantity + "</bundle_alt_qty>"
                                + "<total_alt_qty>" + totalQty + "</total_alt_qty>"
                                + "</params>");

                            await this.ActHdr.PrintLabel(request);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Perform quantity under check for one line item.
        /// </summary>
        private void ItemQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                if (this.ActItem.SelectedActivityItem is PickingItem pickingItem)
                {
                    var available = pickingItem.ActivityItemPickingBaseQuantityAvailable;
                    var shipped = pickingItem.ActivityItemPickingQuantityShipped;
                    var ordered = pickingItem.ActivityItemPickingQuantityOrdered;

                    if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                    {
                        // Do not warn if product is out of stock WO# 12054
                        if (available > 0 && shipped < ordered)
                        {
                            string msg;
                            if (this.SysEnv.SysSettings.IsPickQtyUnderShowBaseQtyAvail)
                            {
                                // TODO: UnitCode should be baseUnitCode, check where this is in table
                                msg = string.Format("The quantity shipped is less than the quantity ordered." + Environment.NewLine + "On Hand {0} {1}", Convert.ToDecimal(pickingItem.ActivityItemPickingBaseQuantityAvailable), Convert.ToString(pickingItem.UnitCode));
                            }
                            else
                            {
                                msg = string.Format("The quantity shipped is less than the quantity ordered." + Environment.NewLine + "On Hand {0} {1}", Convert.ToDecimal(pickingItem.ActivityItemPickingQuantityAvailable), Convert.ToString(pickingItem.UnitCode));
                            }

                            throw new MessageOnlyException(msg);
                        }
                    }
                    else
                    {
                        if (shipped < ordered)
                        {
                            throw new MessageOnlyException("The quantity shipped is less than the quantity ordered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The RefreshData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task RefreshData()
        {
            await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());

            try
            {
                await this.ActItem.Refresh(this.ActHdr.XMLDataCriteria);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), ex.Message, "OK"));
            }

            this.ActItem.ListItems = await this.ActItem.GetActivityItemList(false);

            if (this.ActItem.ListItems.Count > 0)
            {
                await this.ActItem.AssignActivityItem(this.ActItem.ListItems[0]);

                var cmnt = await this.ActItem.GetComment(Convert.ToInt32((this.ActItem.SelectedActivityItem as PickingItem).ActivityItemRefno));

                if (!string.IsNullOrEmpty(cmnt))
                {
                    if (cmnt.Length > 30)
                    {
                        for (var line = 1; line <= 3; line++)
                        {
                            this.ExtraInfoItems.Add(cmnt);
                        }
                    }
                    else
                    {
                        this.ExtraInfoItems.Add(cmnt);
                    }
                }

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Set Location");
                }

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.cmdPrevious.IsEnabled = true;
                    this.cmdNext.IsEnabled = true;
                });
            }
            else
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.cmdPrevious.IsEnabled = false;
                    this.cmdNext.IsEnabled = false;
                    this.ExtraInfoItems.Clear();
                });
            }

            await this.DisplayData();
            await this.ScannerEnable();
        }

        /// <summary>
        /// The ucActivityItem_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task ucActivityItem_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();

            this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

            if (this.IsControlLoaded == false)
            {
                this.ActItem.SelectedActivityItem = null;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                }
                else
                {
                    this.cmdLocationText = "Set Location";
                }

                this.cmdAddSkidIsEnabled = !this.SysEnv.SysSettings.IsPickDisallowManualAddProd;
                this.cmdAddProductIsEnabled = !this.SysEnv.SysSettings.IsPickDisallowManualAddProd;
            }
        }

        /// <summary>
        /// The txtSearchHdr_TextChanged.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private void txtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsChangesMade = true;
            _ = this.ActItem.Search(this.txtSearchItem.Text);
        }

        /// <summary>
        /// The tmrActivityItemLoaded_Tick.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task tmrActivityItemLoaded_Tick(object sender, EventArgs e)
        {
            if (this.IsEnabled)
            {
                await this.RefreshData();
                this.cmdLocationIsEnabled = true;
                this.cmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;
                this.cmdAddProductIsEnabled = this.ActItem.IsAllowAddItem;
                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        /// The txtSearchItem_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private void txtSearchItem_GotFocus(object sender, EventArgs e)
        {
            if (this.txtSearchItem.Text != null)
            {
                this.txtSearchItem.CursorPosition = 0;
                this.txtSearchItem.SelectionLength = this.txtSearchItem.Text.Length;
            }
        }

        /// <summary>
        /// The cmdLocation_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdLocation_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    await this.ScannerDisable();

                    // if cycle by slot
                    var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                        }
                        else
                        {
                            this.cmdLocationText = "Set Location";
                        }
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// The cmdSelectSkid_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdSelectSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    await this.ScannerDisable();

                    var form = new FormDialogContainer(EDialogTypes.SkidInfo, this.ActItem, this.ActSubType);
                    {
                        _ = await form.DisplayControl.ShowDialog();
                        var xmlDoc = await form.GetXML();
                        this.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                        if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            this.cmdAddSkidIsEnabled = true;

                            this.cmdSelectSkidText = Convert.ToString("Skid# " + this.ActHdr.SkidInfo.CurrSkid.SkidCode);

                            this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;

                            var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);

                            this.cmdLocationIsEnabled = false;
                        }
                        else
                        {
                            this.cmdAddSkidIsEnabled = false;

                            this.cmdSelectSkidText = "Select Skid";

                            this.cmdLocationIsEnabled = true;
                        }
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// The cmdAddProduct_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdAddProduct_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    var itemFoundRefno = 0;
                    var prodUnitRefno = 0;

                    await this.ScannerDisable();

                    // Not allowed to count individual boxes on a skid. Skids must be
                    // scanned as a whole unit or built in skid maintenance.

                    // DEV-827 This change to allow counting partial skids is a bad idea.
                    ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                    ////            this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                    ////{
                    ////    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    ////    {
                    ////        this.ActHdr.SkidInfo = new SysSkidInfo();
                    ////        this.cmdSelectSkid.Text = string.Empty;
                    ////    }
                    ////}

                    XDocument xmlDoc = null;

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        // activity item object needs to check if a new location and product can be added, if not add a detail
                        if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                        {
                            throw new MessageOnlyException("The location must be set.");
                        }

                        var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);

                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            xmlDoc = await form.GetXML();
                            prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                        }
                    }
                    else
                    {
                        this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        var info = this.ActHdr.SkidInfo.CurrSkid.ProdInfo;

                        xmlDoc = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<params>"
                            + "<ProductUnitRefno>" + info.UnitRefno + "</ProductUnitRefno>"
                            + "<ProductRefno>" + info.Refno + "</ProductRefno>"
                            + "<ProductCode>" + info.Code + "</ProductCode>"
                            + "<ProductDescription>" + SysCommon.AddXMLReservedChars(info.Desc) + "</ProductDescription>"
                            + "<UnitCode>" + info.UnitCode + "</UnitCode>"
                            + "<ProductUnitBC>" + info.UnitBC + "</ProductUnitBC>"
                            + "<ProductUnitSizeDescription>" + SysCommon.AddXMLReservedChars(info.UnitSizeDesc)
                            + "</ProductUnitSizeDescription>"
                            + "</params>");
                    }

                    if (prodUnitRefno != 0)
                    {
                        itemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);

                        if (itemFoundRefno > 0)
                        {
                            // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            // foundRowIdx = this.ActItem.DataViewItems.Find(itemFoundRefno);
                            // this.ActItem.CurrRowIdx = foundRowIdx;
                            this.ActItem.SelectedActivityItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(itemFoundRefno)).FirstOrDefault();
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(xmlDoc, false);

                                // This is a little dangerous we assume that the new row idx
                                // will be the last one
                                this.ActItem.SelectedActivityItem = this.ActItem.ListItems.Last();
                                itemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                throw new MessageOnlyException("Product rule says that it must exist on" +
                                    " this document already.");
                            }
                        }

                        // Rule: Check if the current location and product home location are the same.
                        if (this.SysEnv.SysSettings.IsVerifyProductLocation &&
                                      (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                                      this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                                      this.ActSubType == SysCommon.ActSubTypes.TransferTo))
                        {
                            // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                            int locMapRefno = this.ActItem.GetDefaultLocMapRefno(
                                Convert.ToInt32((this.ActItem.SelectedActivityItem as dynamic).LocationMapRefno),
                                Convert.ToInt32((this.ActItem.SelectedActivityItem as dynamic).ProductRefno));

                            if (this.LocMap.CurrLocMapRefno != locMapRefno)
                            {
                                throw new MessageOnlyException("The current location does not match the" +
                                    " product home location.");
                            }
                        }

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.
                        ////                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                        ////                                      this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////                        {
                        ////                            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////                            {
                        ////                                this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////                this.cmdSelectSkid.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////                                this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////                            }
                        ////                        }

                        ////BarCodeParse barCodeParse = BarCodeParseFacade.ParseData(string.Empty);
                        ////bool isInsertReady = false;
                        ////ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        ////actDetail.Quantity = 0;
                        ////actDetail.BaseQuantity = 0;
                        ////actDetail.WeightQuantity = 0;
                        ////actDetail.SchemeID = string.Empty;
                        ////actDetail.UniqueSchemeID = string.Empty;
                        ////actDetail.ActExtra.Text1 = string.Empty;
                        ////actDetail.ActExtra.Text2 = string.Empty;
                        ////actDetail.ActExtra.Text3 = string.Empty;
                        ////actDetail.ActExtra.Text4 = string.Empty;
                        ////actDetail.ActExtra.Text5 = string.Empty;

                        ////// Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                        ////// and the fact is isn't human readable friendly.
                        ////SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                        ////SProductSchemeData schemeData = actDetail.SchemeData;

                        ////if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////{
                        ////  schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        ////  schemeData.LogisticUnitBatchLotNumber
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                        ////  schemeData.LogisticUnitBestBeforeSellByDate
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                        ////  schemeData.LogisticUnitProductionDate
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                        ////  schemeData.VariantCode = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                        ////  schemeData.VariantNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                        ////  schemeData.VariantDescription = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                        ////}

                        ////tempItemProdSchemeData.ProdSchemeData = schemeData;
                        ////actDetail.SchemeData = schemeData;

                        ////SysShared.GatherProductData(
                        ////    barCodeParse,
                        ////    this.ActSubType,
                        ////    this.ActItem,
                        ////    tempItemProdSchemeData,
                        ////    this.CallingControl,
                        ////    ref isInsertReady,
                        ////    actDetail,
                        ////    true);

                        ////this.InsertActivityDetail(ref foundRowIdx, actDetail, barCodeParse, isInsertReady);
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (GatherProductDataException ex)
                {
                    if (!ex.SuppressError)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.PhysicalCounts:
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            break;

                        default:
                            // Performing a refresh data will remove and partially added new items or quantity.
                            await this.RefreshData();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.PhysicalCounts:
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            break;

                        default:
                            // Performing a refresh data will remove and partially added new items or quantity.
                            await this.RefreshData();
                            break;
                    }
                }
                finally
                {
                    await this.DisplayData();
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// The cmdAddSkid_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdAddSkid_Click(object sender, EventArgs e)
        {
            // TODO: Test
            if (this.IsControlLoaded)
            {
                try
                {
                    var actItemFoundRefno = 0;
                    var prodUnitRefno = 0;
                    object foundObject = 0;
                    var message = string.Empty;

                    await this.ScannerDisable();

                    if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Skid",
                        "Add the ENTIRE skid to this activity?",
                        "Yes",
                        "No")))
                    {
                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);

                        // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                        if (actItemFoundRefno > 0)
                        {
                            // foundObject = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            foundObject = this.ActItem.ListItems.OfType<object>().Where(
                                activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.
                                GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(ProductInfoTasks.GetXML(this.ActHdr.SkidInfo.CurrSkid.ProdInfo), false);

                                // This is a little dangerous we assume that the new row idx
                                // will be the last one
                                foundObject = this.ActItem.ListItems.Last();
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                message = string.Format(
                                    "Item not found for " + Environment.NewLine + "Location {0} " +
                                        Environment.NewLine + "Product {1} BC {2} Unit {3}",
                                    LocationMapTasks.GetCurrentReadableLocation(),
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Code,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitBC,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitCode);

                                throw new MessageOnlyException(message);
                            }
                        }

                        this.ActItem.SelectedActivityItem = foundObject;

                        // Insert a new activity detail record
                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        var scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        var unqScheme = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        var xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                            + "<alt_qty>" + this.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                            + "<wt_qty>" + this.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                            + "<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>"
                            + "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);
                        await actDetail.InsertSkid(this.ActHdr.SkidInfo);

                        // upon a save of completed data update the data view to to show new item
                        // and quantity change
                        this.ActItem.AddToQuantityInActivityItem(
                            foundObject,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);

                        await this.ActItem.UpdateItemTableQuantity(foundObject);
                        await this.DisplayData();
                    }
                }
                catch (IsReadyToApplyException)
                {
                    // Do nothing.
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// The CmdDetails_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void CmdDetails_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    await this.ScannerDisable();
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedActivityItem);

                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;

                        if (currSkid.ProdInfo.Refno != this.ActItem.ProdInfo.Refno)
                        {
                            throw new MessageOnlyException("Selected line item product must match the" +
                                " selected skid product.");
                        }
                    }

                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityDetail,
                        this.ActItem,
                        this.ActSubType);
                    _ = await form.DisplayControl.ShowDialog();
                    var xmlDoc = await form.GetXML();

                    this.ActItem.ReplaceQuantityInActivityItem(
                        this.ActItem.SelectedActivityItem,
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("qty").Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("base_qty").Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("wt_qty").Value), 3));

                    await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedActivityItem);

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        this.cmdAddSkidIsEnabled = true;

                        this.cmdSelectSkidText = "Skid# " + this.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        this.cmdLocationIsEnabled = false;
                    }
                    else
                    {
                        this.cmdAddSkidIsEnabled = false;

                        this.cmdSelectSkidText = "Select Skid";
                        this.cmdLocationIsEnabled = true;
                    }

                    await this.DisplayData();
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// The cmdNext_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void cmdNext_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    var isValid = true;

                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        if (this.ActItem.SelectedActivityItem != null)
                        {
                            try
                            {
                                this.ItemQuantityUnderCheck();
                            }
                            catch (Exception ex)
                            {
                                switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                                {
                                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                                        if (await Device.InvokeOnMainThreadAsync(async () => !await this.DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                                        {
                                            isValid = false;
                                        }

                                        break;

                                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Quantity Validation", ex.Message, "OK"));
                                        isValid = false;

                                        break;
                                }
                            }
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Last Item", "OK"));
                        }
                    }

                    if (isValid)
                    {
                        await this.FullQuantityShippingLabels();

                        if (this.ActItem.SelectedActivityItem != null)
                        {
                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedActivityItem);
                        }

                        if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
                            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                            {
                                var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                            }
                            else
                            {
                                this.cmdLocationText = "Set Location";
                            }
                        }
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.DisplayData();
                }
            }
        }

        /// <summary>
        /// The cmdPrevious_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void cmdPrevious_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    var isValid = true;

                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        try
                        {
                            this.ItemQuantityUnderCheck();
                        }
                        catch (Exception ex)
                        {
                            switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                            {
                                case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                                    if (await Device.InvokeOnMainThreadAsync(async () => !await this.DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                                    {
                                        isValid = false;
                                    }

                                    break;

                                case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Quantity Validation", ex.Message, "OK"));

                                    isValid = false;
                                    break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        await this.FullQuantityShippingLabels();

                        if (this.ActItem.SelectedActivityItem != null)
                        {
                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedActivityItem);
                        }

                        if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
                            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                            {
                                var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                            }
                            else
                            {
                                this.cmdLocationText = "Set Location";
                            }
                        }
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.DisplayData();
                }
            }
        }

        /// <summary>
        /// The KeyUpF2_ShowLineItemComment.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task KeyUpF2_ShowLineItemComment(object sender, KeyEventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                if (this.ActItem.SelectedActivityItem == null)
                {
                    throw new MessageOnlyException("Please select and line item to show the comments for.");
                }

                string comment = await this.ActItem.GetComment(Convert.ToInt32((this.ActItem.SelectedActivityItem as dynamic).ActivityItemRefno));

                if (!string.IsNullOrEmpty(comment))
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Item Comment", comment, "OK"));
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// The KeyUpF3_ShowTotalQuantity.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task KeyUpF3_ShowTotalQuantity(object sender, KeyEventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var msg = await this.ActItem.GetTotalQuantity();

                if (!string.IsNullOrEmpty(msg))
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), msg, "OK"));
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// The optionsMenu_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void optionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText =
                    "Location: " + Environment.NewLine + currentReadableLocation);
            });

            var result = await this.DisplayActionSheet("ActivityItemSingle Options", "Close", null, this.OptionsMenuItems.ToArray());

            if (result == this.cmdLocationText && this.cmdLocationIsEnabled)
            {
                await this.cmdLocation_Click(sender, e);
            }
            else if (result == this.cmdFilterText && this.cmdFilterIsEnabled)
            {
                // await this.cmdFilter_Clicked(sender, e);
            }
            else if (result == this.cmdSelectSkidText && this.cmdSelectSkidIsEnabled)
            {
                await this.cmdSelectSkid_Click(sender, e);
            }
            else if (result == this.cmdAddProductText && this.cmdAddProductIsEnabled)
            {
                await this.cmdAddProduct_Click(sender, e);
            }
            else if (result == this.cmdAddSkidText && this.cmdAddSkidIsEnabled)
            {
                await this.cmdAddSkid_Click(sender, e);
            }
        }

        /// <summary>
        /// The BarCodeModifier1Picker_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsChangesMade = true;
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
        }
    }
}
