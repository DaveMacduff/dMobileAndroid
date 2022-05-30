// <copyright file="ucActivityItemPurchaseOrderWorkSheetEdit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityItemPurchaseOrderWorkSheetEdit : DMobileModal, IAppArea
    {
        private bool isEscapeTrapped = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ucActivityItemPurchaseOrderWorkSheetEdit"/> class.
        /// </summary>
        public ucActivityItemPurchaseOrderWorkSheetEdit()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ucActivityItemPurchaseOrderWorkSheetEdit"/> class.
        /// </summary>
        /// <param name="_actDetail"></param>
        /// <param name="_ActivitySubTypes"></param>
        public ucActivityItemPurchaseOrderWorkSheetEdit(ActDetail _actDetail, SysCommon.ActSubTypes _ActivitySubTypes)
        {
            this.InitializeComponent();

            // Resource strings
            this.lblUnit.Text = "KG";
            this.lblQty.Text = "Quantity";
            this.btnSave.Text = "Save";
            this.btnCancel.Text = "Cancel";

            this.ExtraInfoItems = new ObservableCollection<string>();
            this.oEnvironment = SysEnvironment.GetInstance();

            this.eActivitySubType = _ActivitySubTypes;
            this.actDetailEdit = new ActDetailEdit();
            this.ActDetail = _actDetail;

            this.actDetailEdit.AltQuantity = this.ActDetail.Quantity;
            this.actDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            this.actDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;

            this.Title = this.GetControlDescription();
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

            try
            {
                await this.ucActivityDetailEdit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityHdr_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());

            base.OnDisappearing();
        }

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

        public SysEnvironment oEnvironment { get; set; }

        public SysCommon.ActSubTypes eActivitySubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        public bool IsEditTrapped { get; set; } = false;

        public bool IsEscapeTrapped
        {
            get
            {
                return this.isEscapeTrapped;
            }
            set
            {
                this.isEscapeTrapped = value;
            }
        }
        public bool IsNumericTrapped { get; set; } = false;

        public string strFullClassName { get; set; } = typeof(ucActivityItemPurchaseOrderWorkSheetEdit).FullName;

        public XDocument xmlDoc { get; set; } = null;

        public int iSelectIndex { get; set; } = 0;

        public DataTable dtActivityDetails { get; set; }

        public ActDetailEdit actDetailEdit { get; set; }

        public bool IsEnterTrapped { get; set; } = false;
        public bool IsControlLoaded { get; set; } = false;
        public bool IsChangesMade { get; set; } = false;

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
        public ActDetail ActDetail { get; set; } = null;

        public ObservableCollection<string> ExtraInfoItems { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ShowProductInfo()
        {
            // ListViewDisplayExtraInfo.Items.Clear();
            // ListViewDisplayExtraInfo.Items.Add("PROD: " + ActDetail.ActItem.ProdInfo.Code + ActDetail.ActItem.ProdInfo.UnitBC + "  " + ActDetail.ActItem.ProdInfo.Desc);
            // ListViewDisplayExtraInfo.Items.Add("SIZE: " + ActDetail.ActItem.ProdInfo.UnitSizeDesc);
            this.ExtraInfoItems.Clear();

            this.ExtraInfoItems.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + "  " + this.ActDetail.ActItem.ProdInfo.Desc);
            this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            SFillQuantity sQuantity = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)this.ActDetail.ActItem.ProdInfo, this.ActDetail.Quantity);

            this.actDetailEdit.AltQuantity = decimal.Round(sQuantity.Quantity, 3);
            this.actDetailEdit.BaseQuantity = decimal.Round(sQuantity.BaseQuantity, 3);
            this.actDetailEdit.WeightQuantity = decimal.Round(sQuantity.WeightQuantity, 3);

            this.txtAltQty.Text = this.actDetailEdit.AltQuantity.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            bool IsRetVal = true;

            // if (!Information.IsNumeric(txtAltQty.Text))
            if (!decimal.TryParse(this.txtAltQty.Text, out _))
            {
                Device.BeginInvokeOnMainThread(() => this.DisplayAlert("Error", "Invalid Quantity!", "OK"));

                IsRetVal = false;
            }
            else
            {
                this.actDetailEdit.AltQuantity = decimal.Parse(this.txtAltQty.Text);
                SFillQuantity sQuantity = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)this.ActDetail.ActItem.ProdInfo, this.actDetailEdit.AltQuantity);
                this.actDetailEdit.AltQuantity = decimal.Round(sQuantity.Quantity, 3);
                this.actDetailEdit.BaseQuantity = decimal.Round(sQuantity.BaseQuantity, 3);
                this.actDetailEdit.WeightQuantity = decimal.Round(sQuantity.WeightQuantity, 3);
            }

            return IsRetVal;
        }

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
/// <summary>
///
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
/// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool IsReadyToSave = true;

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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            return !this.IsChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "dMobile Activity Detail Edit";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool bSaveComplete = false;

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
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Data Validation", "Data Failed Validation. Check Your Data.", "OK"));
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

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
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<alt_qty>" + this.actDetailEdit.AltQuantity + "</alt_qty>" + "<wt_qty>" + this.actDetailEdit.WeightQuantity + "</wt_qty>" + "<base_qty>" + this.actDetailEdit.BaseQuantity + "</base_qty>" + "</params>");
            return request;
        }

        /// <summary>
        ///
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
        ///
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