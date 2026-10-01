// <copyright file="ActivityDetailSkid.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using Resources.Types.Details;
    using Forms;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="ActivityDetailSkid" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDetailSkid : IAppArea
    {
        /// <summary>
        ///     Scanner is in Add Mode (true). Scanning a bar code will add it.
        ///     Scanner is in Remove Mode (false). Scanning a bar code will remove it.
        /// </summary>
        private bool scannerModeAdd;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityDetailSkid" /> class.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem" /> Activity item.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity sub type.</param>
        /// <param name="callingActSubType"><see cref="SysCommon.ActSubTypes" /> Parent calling activity sub type.</param>
        public ActivityDetailSkid(
            ActItem actItem,
            SysCommon.ActSubTypes actSubType,
            SysCommon.ActSubTypes callingActSubType)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.cmdAdd.Text = "Add";
            this.ActSubType = actSubType;
            this.CallingActSubType = callingActSubType;

            this.ActDetail = ActDetailFacade.GetActDetail(actItem, this.ActSubType);

            var schemeData = this.ActDetail.SchemeData;
            schemeData.LogisticUnitSkidNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
            this.ActDetail.SchemeData = schemeData;
            this.DataCollector = new DataCollector(this.ActSubType, actItem);

            this.AddModeHeaderColor = Color.FromRgb(192, 255, 192);
            this.RemoveModeHeaderColor = Color.PaleVioletRed;
            this.ScannerModeAdd = true;
        }

        /// <summary>
        ///     Gets or sets the call activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes CallingActSubType { get; } = SysCommon.ActSubTypes.SkidInfo;

        /// <summary>
        ///     Gets or sets the bar code parse data.
        /// </summary>
        private BarCodeParse Parse { get; set; }

        /// <summary>
        ///     Gets or sets the bar code scanner engine.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Gets or sets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.SkidInfo;

        /// <summary>
        ///     Gets or sets the activity detail instance.
        /// </summary>
        private ActDetail ActDetail { get; }

        /// <summary>
        ///     Gets or sets the selected activity detail foreign key reference number.
        /// </summary>
        private int SelectedActivityDetailFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether ScannerModeAdd
        ///     Scanner is in Add Mode (true). Scanning a bar code will add it.
        ///     Scanner is in Remove Mode (false). Scanning a bar code will remove it.
        /// </summary>
        private bool ScannerModeAdd
        {
            get => this.scannerModeAdd;

            set
            {
                if (value != this.scannerModeAdd)
                {
                    this.scannerModeAdd = value;
                    this.UpdateScannerMode();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the display extra information items....
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets or sets the main activity items....
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        ///     Gets or sets the add mode header color....
        /// </summary>
        private Color AddModeHeaderColor { get; set; }

        /// <summary>
        ///     Gets or sets the remove mode header color....
        /// </summary>
        private Color RemoveModeHeaderColor { get; set; }

        private DataCollector DataCollector { get; set; }

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
                await this.Scanner.EnableScanning();
            }
        }

        /// <summary>
        ///     Check if the activity is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if the activity is ready to close else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
                await this.ScannerDisable();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
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
            finally
            {
                await this.ScannerEnable();
            }

            return isReadyToClose;
        }

        /// <summary>
        ///     Check if the activity is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if the activity is ready to save, else false.</returns>
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
        ///     Save the changes.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = false;

            try
            {
                await this.ScannerDisable();

                if (await this.IsReadyToSave())
                {
                    var fillQty = this.GetActivityItemQuantity();
                    var currSkidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                    currSkidInfo.Quantity = fillQty.Quantity;
                    currSkidInfo.BaseQuantity = fillQty.BaseQuantity;
                    currSkidInfo.WeightQuantity = fillQty.WeightQuantity;
                    this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid = currSkidInfo;
                    isSaveComplete = true;
                }
                else
                {
                    throw new MessageOnlyException("Data failed validation. Check your data.");
                }

                this.DisplaySummaryData();
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
            }

            return isSaveComplete;
        }

        /// <summary>
        ///     Cancel pending changes.
        /// </summary>
        /// <returns><see cref="bool" /> Return true if changes where cancelled.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                await this.ScannerDisable();

                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    var fillQty = this.GetActivityItemQuantity();
                    var currSkidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                    currSkidInfo.Quantity = fillQty.Quantity;
                    currSkidInfo.BaseQuantity = fillQty.BaseQuantity;
                    currSkidInfo.WeightQuantity = fillQty.WeightQuantity;
                    this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid = currSkidInfo;

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
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
        public Task<XDocument> GetXml()
        {
            var fillQty = this.GetActivityItemQuantity();

            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<qty>" + fillQty.Quantity + "</qty>"
                + "<base_qty>" + fillQty.BaseQuantity + "</base_qty>"
                + "<wt_qty>" + fillQty.WeightQuantity + "</wt_qty>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerStart()
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
        ///     Get the control description.
        /// </summary>
        /// <returns><see cref="string" /> Control description.</returns>
        public string GetControlDescription()
        {
            return "Skid Detail";
        }

        /// <summary>
        ///     Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();
                    await this.ScannerStart();
                    await this.RefreshData();
                    await this.ActAppearing();
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);

                    ////this.IsFirstAppearance = true;
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
                catch (CommunicationException commEx)
                {
                    // Question should a communication error, be reported as an error or just a messagebox?
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load activity details.{Environment.NewLine}" +
                                  $"Because of a communication problem.{Environment.NewLine}" +
                                  $"Detail:{commEx.Message}{Environment.NewLine}" +
                                  "Please exit and try again.";

                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK);
                    });
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load activity details.{Environment.NewLine}" +
                                  $"General exception.{Environment.NewLine}" +
                                  $"Detail:{ex.Message}{Environment.NewLine}" +
                                  "Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                    ////await this.ScannerEnable();
                    ////await Task.Run(() =>
                    ////{
                    ////    // Explicit GC, Xamarin forms not disposing of objects properly
                    ////    GC.Collect();
                    ////    GC.WaitForPendingFinalizers();
                    ////});
                }
            }
        }

        /// <summary>
        ///     Activity appearing.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task ActAppearing()
        {
            await this.ScannerDisable();
            SubscribeKeyUpHandlers();
            ////this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            ////this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            this.cmdAdd.IsEnabled = this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo;
            ////this.UpdateScannerMode();
            await this.FindSelectedItemAsync();
            return;
            ////this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (this.IsIndicatorRunning)
                    {
                        return;
                    }

                    if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            _ = await this.CancelChanges();
                        }

                        await this.Close();
                    }
                    else if (keycode == Keycode.Del || keycode == Keycode.Clear)
                    {
                        await OnAppearingDelKey();
                    }
                    else if (keycode == Keycode.DpadDown)
                    {
                        OnAppearingDpadDown();
                    }
                    else if (keycode == Keycode.DpadUp)
                    {
                        OnAppearingDpadUp();
                    }
                });
                return;

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadDown()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) <
                        this.MainActivityItems.Count - 1)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.MainActivityItems[this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) + 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadUp()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) > 0)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.MainActivityItems[this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) - 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingDelKey()
                {
                    // Instead of deleting the ActDetail, we are going to prompt the user how many boxes to remove.
                    // They remove all of them? Then delete the skid.
                    // Otherwise, need to call com server with how many were removed from the skid
                    if (this.ListViewMainActivity.SelectedItem == null)
                    {
                        return;
                    }

                    try
                    {
                        await this.ActDisappearing();

                        if (this.ListViewMainActivity.SelectedItem != null)
                        {
                            var detail = (SkidInfoDetail)this.ListViewMainActivity.SelectedItem;
                            await this.ActDetail.AssignActivityDetail(detail);
                            await PromptRemove();
                        }

                        if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.IsConsumed)
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                this.GetControlDescription(),
                                "This skid is now consumed.",
                                "OK"));

                            if (await this.SaveChanges())
                            {
                                this.DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                this.DialogResult = DialogResult.None;
                                _ = await this.CancelChanges();
                            }
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
                        await this.ActAppearing();
                    }

                    return;

                    async Task PromptRemove()
                    {
                        // picking not allowed to remove boxes from skid
                        if (this.ActDetail.Quantity == 1 || this.CallingActSubType == SysCommon.ActSubTypes.Picking)
                        {
                            if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                    this.GetControlDescription(),
                                    "Do you wish to remove this record from this skid?",
                                    "Yes",
                                    "No")))
                            {
                                // special case: only 1, so remove!
                                var fillQty = new SFillQuantity
                                {
                                    Quantity = 0,
                                    BaseQuantity = 0,
                                    WeightQuantity = 0
                                };

                                await this.RemoveQuantity(fillQty, false);
                            }
                        }
                        else
                        {
                            var form = new FormDialogContainer(
                                EDialogTypes.ActivityRemoveFromSkid,
                                this.ActDetail,
                                this.ActDetail.ActSubType);
                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                    await form.DisplayControl.ShowDialog() == DialogResult.OK))
                            {
                                var xmlDoc = await form.GetXml();

                                // Create an activity detail for it now
                                var fillQty = new SFillQuantity
                                {
                                    Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value),
                                    BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value),
                                    WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value)
                                };

                                await this.RemoveQuantity(fillQty, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Find the selected item in the data list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > 0)
            {
                // I am thinking this iteration will be fast even with 100+ records.
                // We are not triggering any UI refreshing, so it should be.
                object selectedItem = null;
                if (this.SelectedActivityDetailFKRefno != 0)
                {
                    foreach (var item in this.ActDetail.ListDetails)
                    {
                        if (this.SelectedActivityDetailFKRefno == this.ActDetail.GetFkRefno(item))
                        {
                            selectedItem = item;
                            this.ListViewMainActivity.SelectedItem = selectedItem;
                            break;
                        }
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                    await this.ActDetail.AssignActivityDetail(this.ListViewMainActivity.SelectedItem);
                }
                else
                {
                    await this.ActDetail.AssignActivityDetail(selectedItem);
                    this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
                }

                this.ListViewMainActivity.ScrollTo(selectedItem, ScrollToPosition.MakeVisible, false);
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                ////this.ListViewMainActivity.Focus();
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActDetail.SelectedDetail = null;
                this.ActDetail.DetailFkRefno = 0;
                this.ActDetail.DetailRefno = 0;
                this.ListViewMainActivity.SelectedItem = null;
            }
        }

        /// <summary>
        ///     Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ActDisappearing();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Activity disappearing.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task ActDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
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
                    // Disable the scanner even though the on disappearing event will turn it off also
                    // upon closing. SaveChanges or CancelChanges may show dialogs.
                    await this.ScannerDisable();

                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        _ = base.OnBackButtonPressed();
                        await this.Close();
                    }
                    else
                    {
                        this.DialogResult = DialogResult.None;
                        _ = base.OnBackButtonPressed();
                        _ = await this.CancelChanges();
                        await this.Close();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
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
                }
            });

            return true;
        }

        /// <summary>
        ///     Used to move the grid row to the correct position, And set the state of some controls.
        /// </summary>
        /// <param name="svdActDetailFKRefno"><see cref="int" /> Saved activity detail foreign key reference number.</param>
        /// <param name="currRowIdx"><see cref="int" /> Current row index.</param>
        private void CleanUpBarCodeHandler(int svdActDetailFKRefno, int currRowIdx)
        {
            this.ActDetail.DetailFkRefno = svdActDetailFKRefno;

            if (this.ActDetail.ListDetails.Count > 0 && currRowIdx + 1 <= this.ActDetail.ListDetails.Count &&
                this.MainActivityItems.Count > currRowIdx)
            {
                // this.ListViewMainActivity.CurrentRowIndex = currRowIdx;
                this.ListViewMainActivity.SelectedItem = this.MainActivityItems[currRowIdx];
            }
        }

        /// <summary>
        ///     Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Sender bar code scanner event arguments.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            var currRowIdx = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);
            var svdActDetailFKRefno = this.ActDetail.DetailFkRefno;
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;
                this.Parse = scanArgs.ParseInfo;

                var prodUnitRefno = 0;

                if (this.CallingActSubType != SysCommon.ActSubTypes.SkidInfo)
                {
                    throw new MessageOnlyException("Adding product to a skid can only be done in skid maintenance" +
                                                   " header screen.");
                }

                if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                {
                    throw new MessageOnlyException("Scanning a location code is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.ActFkRefno))
                {
                    throw new MessageOnlyException(
                        "Scanning an activity header reference number is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.ActItemFkRefno))
                {
                    throw new MessageOnlyException("Scanning an activity item reference number is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.Sscc18))
                {
                    throw new MessageOnlyException("Scanning an shipping container code is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
                {
                    if (!await LocationMapTasks.IsCompleteLocMap(this.ActDetail.LocMap.CurrLocMapRefno))
                    {
                        throw new MessageOnlyException("Location must be set first.");
                    }

                    prodUnitRefno = await this.DataCollector.IdentifyProductScanned(this.Parse);
                    var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                    if (prodUnitRefno != currSkid.ProdInfo.UnitRefno)
                    {
                        throw new Exception("Scanned product does not match selected skid product.");
                    }

                    var itemSchemeData = AssignSchemeData(currSkid);
                    await DataCollector.GatherProductData(
                        this.Parse,
                        itemSchemeData,
                        this.ActDetail,
                        isForceStopForEditing: false);
                    if (this.ScannerModeAdd)
                    {
                        await this.InsertActivityDetail(this.ActDetail);
                    }
                    else
                    {
                        await this.RemoveActivityDetail();
                    }

                    await this.FindSelectedItemAsync();
                }
                else
                {
                    throw new MessageOnlyException("Scanned bar code type is not supported here.");
                }

                this.DisplaySummaryData();
            }
            catch (TaskCanceledException)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);
            }
            catch (IsReadyToApplyException)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);
            }
            catch (GatherProductDataException ex)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

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

            return;

            //////////////////////////////////////////////////////////////////////////
            ActItemSchemeData AssignSchemeData(SSkidInfo currSkid)
            {
                var schemeData = this.ActDetail.SchemeData;
                schemeData.LogisticUnitSkidNumber = currSkid.SkidCode;
                schemeData.LogisticUnitBatchLotNumber = currSkid.SchemeData.LogisticUnitBatchLotNumber;
                schemeData.LogisticUnitBestBeforeSellByDate = currSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                schemeData.LogisticUnitProductionDate = currSkid.SchemeData.LogisticUnitProductionDate;
                schemeData.VariantCode = currSkid.SchemeData.VariantCode;
                schemeData.VariantNumber = currSkid.SchemeData.VariantNumber;
                schemeData.VariantDescription = currSkid.SchemeData.VariantDescription;
                this.ActDetail.SchemeData = schemeData;

                var itemSchemeData = new ActItemSchemeData
                {
                    ProdSchemeData = schemeData
                };

                return itemSchemeData;
            }
        }

        /// <summary>
        ///     Insert an activity detail.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task InsertActivityDetail(ActDetail actDetail)
        {
            actDetail.DetailFkRefno = 0;
            var xmlDoc = actDetail.GetXml(actDetail);
            await actDetail.Insert(xmlDoc);

            actDetail.DetailRefno = this.ActDetail.FindListDetailRefno(actDetail.DetailFkRefno);
            if (actDetail.DetailRefno < 0)
            {
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
        ///     Remove a scanned activity detail.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task RemoveActivityDetail()
        {
            this.ActDetail.DetailFkRefno = 0;
            var xmlDoc = this.ActDetail.GetXml(this.ActDetail);
            await this.ActDetail.FillActDetailObjFromXml(xmlDoc);
            _ = await this.ActDetail.Delete();
        }

        /// <summary>
        ///     Updates the screen based on the scanner mode.
        /// </summary>
        private void UpdateScannerMode()
        {
            if (this.ScannerModeAdd)
            {
                this.scannerModeText.Text = "Adding";
                this.cmdAdd.Text = "Add";
            }
            else
            {
                this.scannerModeText.Text = "Removing";
                this.cmdAdd.Text = "Remove";
            }
        }

        /// <summary>
        ///     Get the activity item quantity.
        /// </summary>
        /// <returns><see cref="SFillQuantity" /> Item quantity.</returns>
        private SFillQuantity GetActivityItemQuantity()
        {
            var fillQty = new SFillQuantity();

            if (this.ActDetail.ListDetails != null && this.ActDetail.ListDetails.Count > 0)
            {
                for (var i = 0; i <= this.ActDetail.ListDetails.Count - 1; i++)
                {
                    fillQty.Quantity += Convert.ToDecimal(this.ActDetail.ListDetails[i].GetType()
                        .GetProperty("ActivityDetailQuantity")?.GetValue(this.ActDetail.ListDetails[i]));
                    fillQty.BaseQuantity += Convert.ToDecimal(this.ActDetail.ListDetails[i].GetType()
                        .GetProperty("ActivityDetailBaseQuantity")?.GetValue(this.ActDetail.ListDetails[i]));
                    fillQty.WeightQuantity += Convert.ToDecimal(this.ActDetail.ListDetails[i].GetType()
                        .GetProperty("ActivityDetailWeightQuantity")?.GetValue(this.ActDetail.ListDetails[i]));
                }
            }

            return fillQty;
        }

        /// <summary>
        ///     Adds data to the bottom list box to show Product ID, Qty and Weight.
        /// </summary>
        private void DisplaySummaryData()
        {
            var skidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
            var fillQty = this.GetActivityItemQuantity();

            this.ExtraInfoItems.Clear();
            this.ExtraInfoItems.Add($"Product ID {skidInfo.ProdInfo.Code}");
            this.ExtraInfoItems.Add(
                $"Quantity {fillQty.Quantity} {skidInfo.ProdInfo.UnitCode} @ {fillQty.WeightQuantity} {skidInfo.ProdInfo.WeightUnitCode}");
        }

        /// <summary>
        ///     Refresh the activity detail.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task RefreshData()
        {
            try
            {
                await this.ScannerDisable();

                this.ActDetail.ListDetails = await this.ActDetail.GetActivityDetailList(false);
                await this.ActDetail.Refresh();

                this.MainActivityItems = this.ActDetail.DesignGrid(
                    ref this.GridMainActivity,
                    ref this.ListViewMainActivity);

                this.ExtraInfoItems.Clear();
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > 0)
                {
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                    await this.ActDetail.AssignActivityDetail(this.ListViewMainActivity.SelectedItem);
                }

                this.DisplaySummaryData();
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
            }
        }

        /// <summary>
        ///     Search text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void TxtSearchHdr_TextChanged(object sender, EventArgs e)
        {
            this.MainActivityItems = this.ActDetail.Search(this.txtSearchDetail.Text);
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
        }

        /// <summary>
        ///     Removes the quantity from a line.
        /// </summary>
        /// <param name="fillQty">.</param>
        /// <param name="modifyQuantity">The modifyQuantity<see cref="bool" />.</param>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task RemoveQuantity(SFillQuantity fillQty, bool modifyQuantity)
        {
            var data = new XDocument();
            var xmlReq = await this.ActDetail.CreateDeleteXml(data);

            // replace default fields with fillQty
            if (modifyQuantity)
            {
                var xdoc = new XmlDocument();
                xdoc.LoadXml(xmlReq);
                xdoc.DocumentElement["alt_qty"].InnerText = fillQty.Quantity.ToString();
                xdoc.DocumentElement["base_qty"].InnerText = fillQty.BaseQuantity.ToString();
                xdoc.DocumentElement["wt_qty"].InnerText = fillQty.WeightQuantity.ToString();

                xmlReq = xdoc.OuterXml;
            }

            var responseData = SysCommon.SubmitComm(xmlReq);
            SysCommon.CheckCommResponseForErrors(responseData);
            await this.RefreshData();
        }

        /// <summary>
        ///     Search detail go focus event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void TxtSearchDetail_GotFocus(object sender, EventArgs e)
        {
            if (this.txtSearchDetail.Text != null)
            {
                this.txtSearchDetail.CursorPosition = 0;
                this.txtSearchDetail.SelectionLength = this.txtSearchDetail.Text.Length;
            }
        }

        /// <summary>
        ///     CmdAdd_Click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdAdd_Click(object sender, EventArgs e)
        {
            if (!this.ScannerModeAdd)
            {
                await this.RemoveActivityDetail();
                return;
            }

            try
            {
                if (!this.ActDetail.ActItem.IsAllowAddSkidDetail())
                {
                    throw new MessageOnlyException("Adding detail to a skid is not allowed here at this time.");
                }

                // Check to see if grid is empty. If so, don't attempt to retrieve details.
                if (this.MainActivityItems.Count != 0)
                {
                    var detail = new Resources.Types.Details.ActivityDetail();
                    this.ActDetail.ClearProperties();
                }

                await this.ActDisappearing();

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

                var parse = BarCodeParseFacade.ParseData(string.Empty);
                await DataCollector.GatherProductData(
                    parse,
                    FillSchemeData(),
                    this.ActDetail,
                    isForceStopForEditing: false);
                await this.InsertActivityDetail(this.ActDetail);
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
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
                await this.ActAppearing();
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            ActItemSchemeData FillSchemeData()
            {
                var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                // Did not use currSkid.LogisticUnitSkidCode because of the length of the skid code
                // and the fact is isn't human readable friendly.
                var schemeData = this.ActDetail.SchemeData;
                schemeData.LogisticUnitSkidNumber = currSkid.SkidCode;
                schemeData.LogisticUnitBatchLotNumber = currSkid.SchemeData.LogisticUnitBatchLotNumber;
                schemeData.LogisticUnitBestBeforeSellByDate = currSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                schemeData.LogisticUnitProductionDate = currSkid.SchemeData.LogisticUnitProductionDate;
                schemeData.VariantCode = currSkid.SchemeData.VariantCode;
                schemeData.VariantNumber = currSkid.SchemeData.VariantNumber;
                schemeData.VariantDescription = currSkid.SchemeData.VariantDescription;

                var tempItemProdSchemeData = new ActItemSchemeData
                {
                    ProdSchemeData = schemeData
                };

                this.ActDetail.SchemeData = schemeData;
                return tempItemProdSchemeData;
            }
        }

        /// <summary>
        ///     List view main activity item selected event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event arguments.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActDetail.AssignActivityDetail(e.SelectedItem);
            this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
            this.DisplaySummaryData();
        }

        /// <summary>
        ///     Scanner mode text click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void ScannerModeText_Clicked(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
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
            }
        }
    }
}