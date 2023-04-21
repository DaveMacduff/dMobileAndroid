// <copyright file="SubmitBatchFilter.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmitBatchFilter : DMobileModal, IAppArea
    {
        private Dictionary<string, string> statusFilters = new Dictionary<string, string>();
        private Dictionary<string, string> subTypeFilters = new Dictionary<string, string>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="callingControl"></param>
        /// <remarks></remarks>
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitBatchFilter"/> class.
        /// </summary>
        public SubmitBatchFilter()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblStatus.Text = "Status";
            this.lblFilterActivity.Text = "Activity";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            this.SysEnv = SysEnvironment.GetInstance();

            this.ActStatus.Clear();
            _ = this.ActStatus.Add(new ComboBoxMembers("All", Convert.ToInt32(SysCommon.ActStatus.NoChange).ToString()));
            _ = this.ActStatus.Add(new ComboBoxMembers("Inserted", Convert.ToInt32(SysCommon.ActStatus.Inserted).ToString()));
            _ = this.ActStatus.Add(new ComboBoxMembers("Updated", Convert.ToInt32(SysCommon.ActStatus.Updated).ToString()));
            _ = this.ActStatus.Add(new ComboBoxMembers("Deleted", Convert.ToInt32(SysCommon.ActStatus.Deleted).ToString()));
            this.cboActivityStatus.ItemsSource = this.ActStatus;
            this.cboActivityStatus.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboActivityStatus.SelectedIndex = 0;

            this.statusFilters.Add("All", Convert.ToInt32(SysCommon.ActStatus.NoChange).ToString());
            this.statusFilters.Add("Inserted", Convert.ToInt32(SysCommon.ActStatus.Inserted).ToString());
            this.statusFilters.Add("Updated", Convert.ToInt32(SysCommon.ActStatus.Updated).ToString());
            this.statusFilters.Add("Deleted", Convert.ToInt32(SysCommon.ActStatus.Deleted).ToString());

            this.ActSubType.Clear();
            _ = this.ActSubType.Add(new ComboBoxMembers("Picking", Convert.ToInt32(SysCommon.ActSubTypes.Picking).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Returns", Convert.ToInt32(SysCommon.ActSubTypes.Returns).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Receiving", Convert.ToInt32(SysCommon.ActSubTypes.Receiving).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Transfers", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Cycle Counts", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Physical Counts", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Bar Code Association", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Adjustments", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Ticket Finished", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Ticket Material", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Ticket Add-Product", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("Co-Product Add-Product", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct).ToString()));
            _ = this.ActSubType.Add(new ComboBoxMembers("PO Work Sheet", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet).ToString()));
            this.cboActivitySubType.ItemsSource = this.ActSubType;
            this.cboActivitySubType.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboActivitySubType.SelectedIndex = 0;

            this.subTypeFilters.Add("Picking", Convert.ToInt32(SysCommon.ActSubTypes.Picking).ToString());
            this.subTypeFilters.Add("Returns", Convert.ToInt32(SysCommon.ActSubTypes.Returns).ToString());
            this.subTypeFilters.Add("Receiving", Convert.ToInt32(SysCommon.ActSubTypes.Receiving).ToString());
            this.subTypeFilters.Add("Transfers", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom).ToString());
            this.subTypeFilters.Add("Cycle Counts", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts).ToString());
            this.subTypeFilters.Add("Physical Counts", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts).ToString());
            this.subTypeFilters.Add("Bar Code Association", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation).ToString());
            this.subTypeFilters.Add("Adjustments", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment).ToString());
            this.subTypeFilters.Add("Ticket Finished", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished).ToString());
            this.subTypeFilters.Add("Ticket Material", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial).ToString());
            this.subTypeFilters.Add("Ticket Add-Product", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct).ToString());
            this.subTypeFilters.Add("Co-Product Add-Product", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct).ToString());
            this.subTypeFilters.Add("PO Work Sheet", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet).ToString());
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

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
                }
            });

            try
            {
                this.ucSubmitBatchFilter_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucSubmitBatchFilter_GotFocus threw an error " + ex, "OK"));
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

        /// <inheritdoc/>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        private SubmitBatchSettings SubmitBatchSettings { get; set; } = new SubmitBatchSettings();

        private bool IsChangesMade { get; set; } = false;

        private ArrayList ActSubType { get; set; } = new ArrayList();

        private ArrayList ActStatus { get; set; } = new ArrayList();

        private bool IsControlLoaded { get; set; } = false;

        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

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
        /// <inheritdoc/>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    this.LoadData();
                    this.cboActivitySubType.SelectedIndex = this.SubmitBatchSettings.iActivitySubTypeIdx;
                    this.cboActivityStatus.SelectedIndex = this.SubmitBatchSettings.iActivityStatusIdx;
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Submit Batch Filter";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<XDocument> GetXML()
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<activity_status>" + (int)this.SubmitBatchSettings.eActivityStatus + "</activity_status>"
                + "<activity_subtype>" + (int)this.SubmitBatchSettings.eActivitySubType + "</activity_subtype>"
                + "</params>");

            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    // this.SubmitBatchSettings.eActivitySubType = (SysCommon.ActSubTypes)(Convert.ToInt32(((ComboBoxMembers)(this.cboActivitySubType.Items[this.cboActivitySubType.SelectedIndex])).ValueMember));
                    this.SubmitBatchSettings.eActivitySubType = (SysCommon.ActSubTypes)Convert.ToInt32(this.subTypeFilters[this.cboActivitySubType.Items[this.cboActivitySubType.SelectedIndex]]);
                    this.SubmitBatchSettings.iActivitySubTypeIdx = this.cboActivitySubType.SelectedIndex;

                    // this.SubmitBatchSettings.eActivityStatus = (SysCommon.ActStatus)(Convert.ToInt32(((ComboBoxMembers)(this.cboActivityStatus.Items[this.cboActivityStatus.SelectedIndex])).ValueMember));
                    this.SubmitBatchSettings.eActivityStatus = (SysCommon.ActStatus)Convert.ToInt32(this.statusFilters[this.cboActivityStatus.Items[this.cboActivityStatus.SelectedIndex]]);
                    this.SubmitBatchSettings.iActivityStatusIdx = this.cboActivityStatus.SelectedIndex;

                    var tempBatchSettings = this.SubmitBatchSettings;
                    _ = this.SubmitBatchSettings.SaveData(ref tempBatchSettings);
                    this.SubmitBatchSettings = tempBatchSettings;
                    saveComplete = true;
                    this.IsChangesMade = false;
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSubmitBatchFilter_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.LoadData();
            }

            this.cboActivitySubType.SelectedIndex = this.SubmitBatchSettings.iActivitySubTypeIdx;
            this.cboActivityStatus.SelectedIndex = this.SubmitBatchSettings.iActivityStatusIdx;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.SubmitBatchSettings = this.SubmitBatchSettings.LoadData();
            this.IsControlLoaded = true;
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                await this.Close();
            }
        }
    }
}