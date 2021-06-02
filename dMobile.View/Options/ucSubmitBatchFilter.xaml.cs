using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSubmitBatchFilter : DMobileModal, IAppArea
    {
        #region Fields

        private bool isControlLoaded = false;
        private SysEnvironment sysEnv = SysEnvironment.GetInstance();
        private ArrayList actStatus = new ArrayList();
        private ArrayList actSubType = new ArrayList();
        private bool isChangesMade = false;
        private SubmitBatchSettings submitBatchSettings = new SubmitBatchSettings();
        private Dictionary<string, string> statusFilters = new Dictionary<string, string>();
        private Dictionary<string, string> subTypeFilters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="callingControl"></param>
        /// <remarks></remarks>
        public ucSubmitBatchFilter()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblStatus.Text = "Status";
            this.lblFilterActivity.Text = "Activity";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            this.SysEnv = SysEnvironment.GetInstance();

            // TODO: Resource strings
            this.ActStatus.Clear();
            this.ActStatus.Add(new ComboBoxMembers("All", Convert.ToInt32(SysCommon.ActStatus.NoChange).ToString()));
            this.ActStatus.Add(new ComboBoxMembers("Inserted", Convert.ToInt32(SysCommon.ActStatus.Inserted).ToString()));
            this.ActStatus.Add(new ComboBoxMembers("Updated", Convert.ToInt32(SysCommon.ActStatus.Updated).ToString()));
            this.ActStatus.Add(new ComboBoxMembers("Deleted", Convert.ToInt32(SysCommon.ActStatus.Deleted).ToString()));
            this.cboActivityStatus.ItemsSource = this.ActStatus;
            this.cboActivityStatus.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboActivityStatus.SelectedIndex = 0;

            // TODO: Resource strings
            this.statusFilters.Add("All", Convert.ToInt32(SysCommon.ActStatus.NoChange).ToString());
            this.statusFilters.Add("Inserted", Convert.ToInt32(SysCommon.ActStatus.Inserted).ToString());
            this.statusFilters.Add("Updated", Convert.ToInt32(SysCommon.ActStatus.Updated).ToString());
            this.statusFilters.Add("Deleted", Convert.ToInt32(SysCommon.ActStatus.Deleted).ToString());

            this.ActSubType.Clear();
            this.ActSubType.Add(new ComboBoxMembers("Picking", Convert.ToInt32(SysCommon.ActSubTypes.Picking).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Returns", Convert.ToInt32(SysCommon.ActSubTypes.Returns).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Receiving", Convert.ToInt32(SysCommon.ActSubTypes.Receiving).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Transfers", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Cycle Counts", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Physical Counts", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Bar Code Association", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Adjustments", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Ticket Finished", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Ticket Material", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Ticket Add-Product", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("Co-Product Add-Product", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct).ToString()));
            this.ActSubType.Add(new ComboBoxMembers("PO Work Sheet", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet).ToString()));
            this.cboActivitySubType.ItemsSource = this.ActSubType;
            this.cboActivitySubType.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboActivitySubType.SelectedIndex = 0;

            // TODO: Resource strings
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

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

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
                this.ucSubmitBatchFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucSubmitBatchFilter_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

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

        #endregion

        #region Properties

        private SubmitBatchSettings SubmitBatchSettings
        {
            get
            {
                return this.submitBatchSettings;
            }

            set
            {
                this.submitBatchSettings = value;
            }
        }

        private bool IsChangesMade
        {
            get
            {
                return this.isChangesMade;
            }

            set
            {
                this.isChangesMade = value;
            }
        }

        private ArrayList ActSubType
        {
            get
            {
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
            }
        }

        private ArrayList ActStatus
        {
            get
            {
                return this.actStatus;
            }

            set
            {
                this.actStatus = value;
            }
        }

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        #endregion

        #region Public Members

        #region IAppArea Members

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
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    await errRpt.ShowDialog();
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
            // TODO: Resource strings
            return "Submit Batch Filter";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<activity_status>" + (int)SubmitBatchSettings.eActivityStatus + "</activity_status>")
                + ("<activity_subtype>" + (int)SubmitBatchSettings.eActivitySubType + "</activity_subtype>")
                + "</params>");

            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = true;

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
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    //this.SubmitBatchSettings.eActivitySubType = (SysCommon.ActSubTypes)(Convert.ToInt32(((ComboBoxMembers)(this.cboActivitySubType.Items[this.cboActivitySubType.SelectedIndex])).ValueMember));
                    this.SubmitBatchSettings.eActivitySubType = (SysCommon.ActSubTypes)Convert.ToInt32(this.subTypeFilters[this.cboActivitySubType.Items[this.cboActivitySubType.SelectedIndex]]);
                    this.SubmitBatchSettings.iActivitySubTypeIdx = this.cboActivitySubType.SelectedIndex;

                    //this.SubmitBatchSettings.eActivityStatus = (SysCommon.ActStatus)(Convert.ToInt32(((ComboBoxMembers)(this.cboActivityStatus.Items[this.cboActivityStatus.SelectedIndex])).ValueMember));
                    this.SubmitBatchSettings.eActivityStatus = (SysCommon.ActStatus)Convert.ToInt32(this.statusFilters[this.cboActivityStatus.Items[this.cboActivityStatus.SelectedIndex]]);
                    this.SubmitBatchSettings.iActivityStatusIdx = this.cboActivityStatus.SelectedIndex;

                    SubmitBatchSettings tempBatchSettings = this.SubmitBatchSettings;
                    this.SubmitBatchSettings.SaveData(ref tempBatchSettings);
                    this.SubmitBatchSettings = tempBatchSettings;
                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSubmitBatchFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.LoadData();
                }

                this.cboActivitySubType.SelectedIndex = this.SubmitBatchSettings.iActivitySubTypeIdx;
                this.cboActivityStatus.SelectedIndex = this.SubmitBatchSettings.iActivityStatusIdx;
            }
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

        #endregion

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