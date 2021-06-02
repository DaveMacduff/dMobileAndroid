using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using System.Collections.ObjectModel;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityConvertUnitEdit : DMobileModal, IAppArea
    {
        #region Fields

        private ActDetailEdit actDetailEdit;
        private DataTable dataTableActDetail;
        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        private DataView dataViewVariants = null;
        private DataView dataViewLots = null;
        private DataView dataViewSerialNumbers = null;
        private ActDetail actDetail = null;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.ConvertUnit;
        private SysEnvironment sysEnv;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityConvertUnitEdit(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblWtUnit.Text = "KG";
            this.lblUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.lblQty.Text = "Quantity";
            this.lblSplitToSingles.Text = "Split To Singles";
            this.lblEquals.Text = "Equals";
            this.lblFromUnitDesc.Text = "From Unit";
            this.cmdSave.Text = "Save";
            this.cmdCancel.Text = "Cancel";

            this.SysEnv = SysEnvironment.GetInstance();
            this.ActSubType = actSubTypes;
            this.ActDetailEdit = new ActDetailEdit();
            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

            this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
            this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
            this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
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
            });

            try
            {
                this.ucActivityDetailEdit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityDetailEdit_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
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
                base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                    await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        #endregion

        #region Properties

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

        private SysCommon.ActSubTypes ActSubType
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

        private DataView DataViewSerialNumbers
        {
            get
            {
                return this.dataViewSerialNumbers;
            }

            set
            {
                this.dataViewSerialNumbers = value;
            }
        }

        private DataView DataViewLots
        {
            get
            {
                return this.dataViewLots;
            }

            set
            {
                this.dataViewLots = value;
            }
        }

        private DataView DataViewVariants
        {
            get
            {
                return this.dataViewVariants;
            }

            set
            {
                this.dataViewVariants = value;
            }
        }

        private DataTable DataTableActDetail
        {
            get
            {
                return this.dataTableActDetail;
            }

            set
            {
                this.dataTableActDetail = value;
            }
        }

        private ActDetailEdit ActDetailEdit
        {
            get
            {
                return this.actDetailEdit;
            }

            set
            {
                this.actDetailEdit = value;
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

        private ActDetail ActDetail
        {
            get
            {
                return this.actDetail;
            }

            set
            {
                this.actDetail = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }

        #endregion

        #region Public Members

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
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
                isReadyToSave = isReadyToSave && !(decimal.Parse(this.txtAltQty.Text) == 0 && decimal.Parse(this.txtWtQty.Text) == 0);
                isReadyToSave = isReadyToSave && this.IsValidData();
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = false;

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
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Convert Units Edit";
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
                    saveComplete = this.IsValidData();
                    this.IsChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Data Validation", "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            return saveComplete;
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
                if (await this.IsReadyToClose())
                {
                    this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
                    this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
                    this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
                }
                else
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
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
            string unqScheme = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            string scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData); SProductSchemeData schemeData = this.ActDetailEdit.SchemeData;

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<serial>" + schemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<pdate>" + schemeData.LogisticUnitProductionDate + "</pdate>")
                + ("<lot_no>" + schemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<bdate>" + schemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
                + ("<variant_code>" + schemeData.VariantCode + "</variant_code>")
                + ("<variant_no>" + schemeData.VariantNumber + "</variant_no>")
                + ("<alt_qty>" + this.ActDetailEdit.Quantity + "</alt_qty>")
                + ("<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>")
                + ("<base_qty>" + this.ActDetailEdit.BaseQuantity + "</base_qty>")
                + ("<scheme_id>" + scheme + "</scheme_id>")
                + ("<unq_scheme_id>" + unqScheme + "</unq_scheme_id>")
                + ("<split>" + this.chkSplitToSingles.IsToggled + "</split>")
                + "</params>");

            return request;
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            bool isRetVal = true;

            this.ActDetailEdit.Quantity = decimal.Parse(this.txtAltQty.Text);
            this.ActDetailEdit.WeightQuantity = decimal.Parse(this.txtWtQty.Text);

            if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
            {
                this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.WeightQuantity;
            }
            else
            {
                if (this.ActDetailEdit.Quantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity;
                }
            }

            return isRetVal;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ShowProductInfo()
        {
            this.lblFromUnitDesc.Text = string.Format(
                "{0} {1} @ {2} {3}",
                (this.ActDetail.ActItem.ListItems[0] as dynamic).ActivityDetailQuantity,
                (this.ActDetail.ActItem.ListItems[0] as dynamic).UnitCode,
                (this.ActDetail.ActItem.ListItems[0] as dynamic).ActivityDetailWeightQuantity,
                this.ActDetail.ActItem.ProdInfo.WeightUnitCode);

            this.ItemsList = new ObservableCollection<string>();
            this.ItemsList.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + "  " + this.ActDetail.ActItem.ProdInfo.Desc);
            this.ItemsList.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);

            this.lstDisplay.ItemsSource = this.ItemsList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            // Default quantities if zero
            if (this.ActDetail.Quantity == 0)
            {
                if (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.WeightQuantity != 0)
                    {
                        this.ActDetailEdit.Quantity = this.ActDetail.WeightQuantity;
                    }
                    else
                    {
                        this.ActDetailEdit.Quantity = this.ActDetail.ActItem.ProdInfo.UnitWeight;
                    }
                }
                else
                {
                    this.ActDetailEdit.Quantity = 1;
                }
            }
            else
            {
                this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
            }

            if (this.ActDetail.WeightQuantity == 0)
            {
                if (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.Quantity != 0)
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.Quantity;
                    }
                    else
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitWeight;
                    }
                }
                else
                {
                    if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB")
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitWeight;
                    }
                    else
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitWeight;
                    }
                }
            }
            else
            {
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
            }

            if (this.ActDetail.BaseQuantity == 0)
            {
                if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.WeightQuantity;
                }
                else
                {
                    if (this.ActDetail.Quantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                    {
                        this.ActDetailEdit.BaseQuantity = this.ActDetail.Quantity * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                    }
                    else
                    {
                        this.ActDetailEdit.BaseQuantity = this.ActDetail.Quantity;
                    }
                }
            }
            else
            {
                this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            }

            this.ActDetailEdit.Quantity = decimal.Round(this.ActDetailEdit.Quantity, 3);
            this.ActDetailEdit.BaseQuantity = decimal.Round(this.ActDetailEdit.BaseQuantity, 3);
            this.ActDetailEdit.WeightQuantity = decimal.Round(ActDetailEdit.WeightQuantity, 3);

            this.txtAltQty.Text = this.ActDetailEdit.Quantity.ToString();
            this.txtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityDetailEdit_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;
                    this.lblUnit.Text = this.ActDetail.ActItem.ProdInfo.UnitCode;
                    this.lblWtUnit.Text = this.ActDetail.ActItem.ProdInfo.WeightUnitCode;

                    if (!this.ActDetail.ActItem.ProdInfo.IsCatchWeight || (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG"))
                    {
                        this.txtWtQty.IsEnabled = false;
                    }

                    this.LoadData();
                    this.ShowProductInfo();

                    this.txtAltQty.Focus();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtAltQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IdentifyChangesMade(sender, e);
            decimal qtyEntered = 0;

            try
            {
                if (!string.IsNullOrEmpty(this.txtAltQty.Text))
                {
                    qtyEntered = Convert.ToDecimal(this.txtAltQty.Text);
                }
            }
            catch
            {
            }

            decimal wtQty;

            if (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
            {
                // if the unit code is in LBs or KGs then just use that value
                wtQty = qtyEntered;
            }
            else
            {
                // Otherwise the weigth = quantity * weightPerOneQty
                wtQty = qtyEntered * this.ActDetail.ActItem.ProdInfo.UnitWeight;
            }

            this.txtWtQty.Text = Math.Round(wtQty, 3).ToString();
        }

        #endregion


        private async void cmdSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void cmdCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await this.CancelChanges();
            await this.Close();
        }
    }
}