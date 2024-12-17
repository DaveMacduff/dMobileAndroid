// <copyright file="ActivityHdrReleaseByOrder.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using CustomPages;
    using Forms;
    using Resources.Enums;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrReleaseByOrder : DMobileContentPage, IAppArea
    {
        private readonly List<string> optionsMenuItems = new List<string>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrReleaseByOrder" /> class.
        /// </summary>
        /// <param name="actSubTypes"></param>
        public ActivityHdrReleaseByOrder(SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.cmdFilter.Text = "Filter";
            this.cmdAdd.Text = "Add";
            this.cmdAdd.IsEnabled = false;

            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;

            this.ActSubType = actSubTypes;
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);
            this.Activity.ActSubType = this.ActSubType;

            this.Title = this.GetControlDescription();
            this.ExtraInfoItems = new ObservableCollection<string>();
            this.MainActivityItems = new ObservableCollection<object>();
            this.optionsMenuItems.Add("Release order");
        }

        private BarCodeParse Parse { get; set; }

        private BarCodeScanner Scanner { get; }

        private SysEnvironment SysEnv { get; }

        private bool IsBarCodeHandlerRegistered { get; set; }

        private Activity Activity { get; }

        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.ReleaseByOrder;

        public ObservableCollection<string> ExtraInfoItems { get; set; }
        
        public ObservableCollection<object> MainActivityItems { get; set; }

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
                    this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                }

                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> IsReadyToClose()
        {
            if (this.Scanner != null)
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                await this.ScannerDisable();
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public Task<bool> SaveChanges()
        {
            return Task.FromResult(true);
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
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
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
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

                    MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.IsIndicatorRunning)
                        {
                            if (keycode == Keycode.F4 && !this.SysEnv.SysSettings.IsDisconnectedMode)
                            {
                                try
                                {
                                    await this.ReleaseOrder();
                                }
                                catch (Exception ex)
                                {
                                    await Device.InvokeOnMainThreadAsync(async () =>
                                        await this.DisplayAlert("Change Status", ex.Message, "OK"));
                                }
                            }
                        }
                    });

                    await this.ucActivityHdrReleaseByOrder_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error",
                        "this.ucActivityHdrReleaseByOrder_GotFocus threw an error " + ex, "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            }
        }

        private async Task ReleaseOrder()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.Picking)
            {
                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await this.Activity.ChangeStatus(this.ListViewMainActivity.SelectedItem)))
                {
                    await this.ScannerDisable();

                    // SHIPPING LABELS
                    if (this.SysEnv.SysSettings.CanPrintShippingLabel && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // Load activity properties, activity is an argument for the ShippingLabelControl
                        // SelectedIdx is not refreshed.
                        this.Activity.AssignRefnoFromActivity(this.ListViewMainActivity.SelectedItem);

                        var labelQuantity = new SLabelQuantity();
                        labelQuantity = await this.Activity.GetLabelQuantity(false);

                        var form = new FormDialogContainer(
                            EDialogTypes.ActivityShippingLabelEdit,
                            labelQuantity,
                            this.Activity,
                            ShippingLabelQuantitySelectionMode.BothFullRepack);

                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            var request = await form.GetXml();

                            // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                            // If itm_refno is zero then no line is updated as printed.
                            request.Element("params").Element("whs_id").AddAfterSelf(new XElement("itm_refno", 0));
                            await this.Activity.PrintLabel(request);
                        }
                    }

                    if (this.SysEnv.SysSettings.IsPickRefreshOrdAfterRel)
                    {
                        await this.RefreshData();
                    }
                    else
                    {
                        _ = this.Activity.ListHeaders.Remove(this.ListViewMainActivity.SelectedItem);

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            this.ExtraInfoItems.Clear();
                            this.MainActivityItems = this.Activity.DesignGrid(ref this.GridMainActivity,
                                ref this.ListViewMainActivity);

                            if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                            {
                                this.MainActivityItems = this.Activity.Search(this.txtSearchHdr.Text);
                            }

                            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                            if (this.Activity.ListHeaders.Count > 0 && this.MainActivityItems.Count > 0 &&
                                this.MainActivityItems[0] != null)
                            {
                                this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                                await this.Activity.SelectActivity(this.ListViewMainActivity.SelectedItem);
                            }
                        });
                    }
                }

                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ucActivityHdrReleaseByOrder_LostFocus(this, EventArgs.Empty);
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc />
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else
                {
                    _ = await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        // public UserControl GetCallingControl()
        // {
        //    UserControl control = new SalesOrdersMenu();

        // return control;
        // }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return this.Activity.GetControlDescription();
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
        public async Task ucActivityHdrReleaseByOrder_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();
            await this.RefreshData();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.MainActivityItems = this.Activity.Search(this.txtSearchHdr.Text);
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.Activity.SelectActivity(e.SelectedItem);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdFilter_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            var form = new FormDialogContainer(EDialogTypes.ActivityHdrPickingFilter, this.ActSubType);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                var xmlDoc = await form.GetXml();
                this.Activity.GetFilter(xmlDoc);

                // TODO: Fix filtering
                // this.Activity.DataViewHdrs.RowFilter = filter;
                if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                {
                    _ = this.Activity.Search(this.txtSearchHdr.Text);
                }

                if (!string.IsNullOrEmpty(xmlDoc.Root.Element("sortby").Value))
                {
                    // TODO: Fix sorting
                    // this.Activity.DataViewHdrs.Sort = xmlDoc.Root.Element("sortby").Value;
                }
            }

            await this.ScannerEnable();
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
        public async Task ucActivityHdrReleaseByOrder_LostFocus(object sender, EventArgs e)
        {
            // if (this.tmrActivityLoaded.Enabled == false)
            // {
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }

            // }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAdd_Click(object sender, EventArgs e)
        {
            XDocument xmlDoc = null;
            _ = await this.Activity.Insert(xmlDoc);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtSearchHdr_GotFocus(object sender, EventArgs e)
        {
            if (this.txtSearchHdr.Text != null)
            {
                this.txtSearchHdr.CursorPosition = 0;
                this.txtSearchHdr.SelectionLength = this.txtSearchHdr.Text.Length;
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
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            "OK"));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;

                var message = string.Empty;
                var savedViewSort = string.Empty;

                this.Parse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                if (!string.IsNullOrEmpty(this.Parse.ActFkRefno))
                {
                    // this.Parse.ActivityFKRefno e.g. format is 'OE:12345678' start at 4th pos for the fk refno
                    var actStringFKRefno = this.Parse.ActFkRefno;

                    if (!string.IsNullOrEmpty(actStringFKRefno))
                    {
                        var selectedActivity = this.Activity.ListHeaders.OfType<object>().Where(activity =>
                            Convert.ToString(activity.GetType().GetProperty(this.Activity.GetFkRefnoIdx())
                                .GetValue(activity)) == actStringFKRefno).FirstOrDefault();

                        if (selectedActivity != null)
                        {
                            this.Activity.AssignRefnoFromActivity(selectedActivity);
                            var form = new FormDialogContainer(EDialogTypes.ActivityItem, this.Activity,
                                this.ActSubType);
                            _ = await Device.InvokeOnMainThreadAsync(async () =>
                                await form.DisplayControl.ShowDialog());
                        }
                        else
                        {
                            this.Activity.HdrFkRefno = 0;
                            this.Activity.HdrRefno = 0;

                            message = $"Unable To Find Reference# {actStringFKRefno}";

                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(this.GetControlDescription(), message, "OK"));
                        }
                    }
                    else
                    {
                        message = $"Unable To Parse Reference# {actStringFKRefno}";

                        await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(this.GetControlDescription(), message, "OK"));
                    }
                }
                else if (!string.IsNullOrEmpty(this.Parse.ActItemFkRefno))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning an activity item reference # is not supported here.", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning a product bar code is not supported here.", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.Sscc18))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning a shipping container bar code is not supported here!", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning a location identifier bar code is not supported here.", "OK"));
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanned bar code type is not supported here.", "OK"));
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert("Error scanning barcode", ex.Message, "OK"));
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            this.IsIndicatorRunning = true;

            await Task.Run(async () =>
            {
                try
                {
                    await this.Activity.Refresh(null);
                }
                catch (SQLiteException ex)
                {
                    for (var curExNdx = 0; curExNdx <= ex.Data.Count; curExNdx++)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(this.GetControlDescription(),
                                Convert.ToString("Error " + Convert.ToString(ex.Data[curExNdx])), "OK"));
                    }
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(), ex.Message, "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }

                this.Activity.ListHeaders = await this.Activity.GetActivityList(false);

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ExtraInfoItems.Clear();
                    this.MainActivityItems =
                        this.Activity.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);

                    if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                    {
                        this.MainActivityItems = this.Activity.Search(this.txtSearchHdr.Text);
                    }

                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                    if (this.Activity.ListHeaders.Count > 0 && this.MainActivityItems.Count > 0 &&
                        this.MainActivityItems[0] != null)
                    {
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                        await this.Activity.SelectActivity(this.ListViewMainActivity.SelectedItem);
                    }
                });
            });

            this.IsIndicatorRunning = false;
        }

        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            var result = await this.DisplayActionSheet("Release Order Options", "Close", null,
                this.optionsMenuItems.ToArray());

            if (result == "Release order")
            {
                await this.ReleaseOrder();
            }
        }
    }
}