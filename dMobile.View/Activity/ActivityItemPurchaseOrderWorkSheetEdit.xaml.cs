// <copyright file="ActivityItemPurchaseOrderWorkSheetEdit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using Controller.Activity_Detail;
    using CustomPages.Modal;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItemPurchaseOrderWorkSheetEdit : DMobileModal, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityItemPurchaseOrderWorkSheetEdit" /> class.
        /// </summary>
        public ActivityItemPurchaseOrderWorkSheetEdit()
        {
            this.InitializeComponent();
            this.IsIndicatorRunning = false;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityItemPurchaseOrderWorkSheetEdit" /> class.
        /// </summary>
        /// <param name="_actDetail"></param>
        /// <param name="_ActivitySubTypes"></param>
        public ActivityItemPurchaseOrderWorkSheetEdit(ActDetail _actDetail, SysCommon.ActSubTypes _ActivitySubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;

            // Resource strings
            this.lblUnit.Text = "KG";
            this.lblQty.Text = "Quantity";
            this.btnSave.Text = "Save";
            this.btnCancel.Text = "Cancel";

            this.ExtraInfoItems = new ObservableCollection<string>();
            this.sysEnv = SysEnvironment.GetInstance();

            this.eActivitySubType = _ActivitySubTypes;
            this.actDetailEdit = new ActDetailEdit();
            this.ActDetail = _actDetail;

            this.actDetailEdit.AltQuantity = this.ActDetail.Quantity;
            this.actDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            this.actDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;

            this.Title = this.GetControlDescription();
        }

        public SysEnvironment sysEnv { get; set; }

        public SysCommon.ActSubTypes eActivitySubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        public bool IsEditTrapped { get; set; } = false;

        public bool IsEscapeTrapped { get; set; } = false;

        public bool IsNumericTrapped { get; set; } = false;

        public string strFullClassName { get; set; } = typeof(ActivityItemPurchaseOrderWorkSheetEdit).FullName;

        public XDocument xmlDoc { get; set; } = null;

        public int iSelectIndex { get; set; } = 0;

        public DataTable dtActivityDetails { get; set; }

        public ActDetailEdit actDetailEdit { get; set; }

        public bool IsEnterTrapped { get; set; } = false;
        public bool IsControlLoaded { get; set; }
        public bool IsChangesMade { get; set; }

        // public UserControl ucCallingControl
        // {
        //    get
        //    {
        //        return m_ucCallingControl;
        //    }
        //    set
        //    {
        //        m_ucCallingControl = value;
        //    }
        // }
        public ActDetail ActDetail { get; set; }

        public ObservableCollection<string> ExtraInfoItems { get; set; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> IsReadyToSave()
        {
            var IsReadyToSave = true;

            try
            {
                IsReadyToSave = IsReadyToSave && !(decimal.Parse(this.txtAltQty.Text) == 0);
                IsReadyToSave = IsReadyToSave && this.IsValidData();
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "IsReadyToSave", Ex) == false)
                {
                }

                IsReadyToSave = false;
            }

            return IsReadyToSave;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> IsReadyToClose()
        {
            return !this.IsChangesMade;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> SaveChanges()
        {
            var bSaveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    bSaveComplete = this.IsValidData();
                    this.IsChangesMade = false;
                }
                else
                {
                    // MessageBox.Show("Data Failed Validation. Check Your Data.");
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Data Validation", "Data Failed Validation. Check Your Data.", "OK"));
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "SaveChanges", Ex) == false)
                {
                }

                bSaveComplete = false;
            }

            return bSaveComplete;
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
                    this.actDetailEdit.SchemeData = this.ActDetail.SchemeData;
                    this.actDetailEdit.AltQuantity = this.ActDetail.Quantity;
                    this.actDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
                    this.actDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;

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
                    this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
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
        public async Task<XDocument> GetXml()
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "<alt_qty>" + this.actDetailEdit.AltQuantity + "</alt_qty>" +
                "<wt_qty>" + this.actDetailEdit.WeightQuantity + "</wt_qty>" +
                "<base_qty>" + this.actDetailEdit.BaseQuantity + "</base_qty>" +
                "</params>");
            return request;
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                try
                {
                    base.OnAppearing();

                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

                    MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.IsIndicatorRunning)
                        {
                            if (keycode == Keycode.Enter)
                            {
                                if (await this.SaveChanges())
                                {
                                    this.DialogResult = DialogResult.OK;
                                    await this.Close();
                                }
                            }
                        }
                    });

                    await this.ucActivityDetailEdit_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Error", "this.ucActivityHdr_GotFocus threw an error " + ex, "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc />
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                _ = await this.CancelChanges();
                this.DialogResult = DialogResult.None;
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void ShowProductInfo()
        {
            // ListViewDisplayExtraInfo.Items.Clear();
            // ListViewDisplayExtraInfo.Items.Add("PROD: " + ActDetail.ActItem.ProdInfo.Code + ActDetail.ActItem.ProdInfo.UnitBC + "  " + ActDetail.ActItem.ProdInfo.Desc);
            // ListViewDisplayExtraInfo.Items.Add("SIZE: " + ActDetail.ActItem.ProdInfo.UnitSizeDesc);
            this.ExtraInfoItems.Clear();

            this.ExtraInfoItems.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code +
                                    this.ActDetail.ActItem.ProdInfo.UnitBc + "  " +
                                    this.ActDetail.ActItem.ProdInfo.Desc);
            this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            var sQuantity =
                SysCommon.FillQuantityStructureFromQuantity(this.ActDetail.ActItem.ProdInfo, this.ActDetail.Quantity);

            this.actDetailEdit.AltQuantity = decimal.Round(sQuantity.Quantity, 3);
            this.actDetailEdit.BaseQuantity = decimal.Round(sQuantity.BaseQuantity, 3);
            this.actDetailEdit.WeightQuantity = decimal.Round(sQuantity.WeightQuantity, 3);

            this.txtAltQty.Text = this.actDetailEdit.AltQuantity.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            var IsRetVal = true;

            // if (!Information.IsNumeric(TxtBaseQty.Text))
            if (!decimal.TryParse(this.txtAltQty.Text, out _))
            {
                Device.BeginInvokeOnMainThread(() => this.DisplayAlert("Error", "Invalid Quantity!", "OK"));

                IsRetVal = false;
            }
            else
            {
                this.actDetailEdit.AltQuantity = decimal.Parse(this.txtAltQty.Text);
                var sQuantity = SysCommon.FillQuantityStructureFromQuantity(this.ActDetail.ActItem.ProdInfo,
                    this.actDetailEdit.AltQuantity);
                this.actDetailEdit.AltQuantity = decimal.Round(sQuantity.Quantity, 3);
                this.actDetailEdit.BaseQuantity = decimal.Round(sQuantity.BaseQuantity, 3);
                this.actDetailEdit.WeightQuantity = decimal.Round(sQuantity.WeightQuantity, 3);
            }

            return IsRetVal;
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
        public async Task ucActivityDetailEdit_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
                this.lblUnit.Text = this.ActDetail.ActItem.ProdInfo.UnitCode;

                this.LoadData();
                this.ShowProductInfo();
                _ = this.txtAltQty.Focus();
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "dMobile Activity Detail Edit";
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtAltQty_GotFocus(object sender, EventArgs e)
        {
            if (this.txtAltQty.Text != null)
            {
                this.txtAltQty.CursorPosition = 0;
                this.txtAltQty.SelectionLength = this.txtAltQty.Text.Length;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "IdentifyChangesMade", Ex) == false)
                {
                }
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

        private async void btnCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            _ = await this.CancelChanges();
            await this.Close();
        }
    }
}