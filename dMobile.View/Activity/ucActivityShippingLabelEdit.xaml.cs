using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityShippingLabelEdit : DMobileModal, IAppArea
    {
        #region Fields

        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.NotApplicable;
        private SysEnvironment sysEnv;
        private SysCommon.SLabelQuantity labelQtyEdit = new SysCommon.SLabelQuantity();
        private LocationMap locMap = LocationMap.GetInstance();
        private bool isFirstAppearance = true;
        Controller.Activity Activity;
        SysCommon.ShippingLabelQuantitySelectionMode SelectMode;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actSubTypes"></param>
        /// <param name="callingControl"></param>
        /// <param name="labelQtyEdit"></param>
        /// <param name="activity"></param>
        /// <param name="selectMode"></param>
        public ucActivityShippingLabelEdit(SysCommon.ActSubTypes actSubTypes, SysCommon.SLabelQuantity labelQtyEdit, Controller.Activity activity, SysCommon.ShippingLabelQuantitySelectionMode selectMode)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblFullUnitQuantity.Text = "Full Cases";
            this.lblRepackQuantity.Text = "Repacks";
            this.lblOrderNumber.Text = "Order";
            this.lblFreshAltQty.Text = "Fresh";
            this.lblFrozenAltQty.Text = "Frozen";
            this.lblDryAltQty.Text = "Dry";
            this.lblLiveAltQty.Text = "Live";
            this.lblPalletAltQty.Text = "Pallets";
            this.cmdLocation.Text = "Set Location";
            this.lblTotal.Text = "Total";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.Activity = activity;
                this.SelectMode = selectMode;
                this.LabelQtyEdit = labelQtyEdit;
                this.SysEnv = SysEnvironment.GetInstance();
                this.ActSubType = actSubTypes;
            }
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
                    if (keycode == Keycode.F1)
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
                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;

                    if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                        await Device.InvokeOnMainThreadAsync(() => cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                    }

                    await Device.InvokeOnMainThreadAsync(() => this.lblOrderNumber.Text = Convert.ToString("Order # " + this.Activity.FKRefno.ToString()));

                    if (this.SelectMode == SysCommon.ShippingLabelQuantitySelectionMode.BothFullRepack || this.SelectMode == SysCommon.ShippingLabelQuantitySelectionMode.FullOnly)
                    {
                        this.txtFullCaseAltQty.IsEnabled = this.SysEnv.SysSettings.IsFullQuantityEnabled;
                        this.txtFreshAltQty.IsEnabled = this.SysEnv.SysSettings.IsFreshQuantityEnabled;
                        this.txtDryAltQty.IsEnabled = this.SysEnv.SysSettings.IsDryQuantityEnabled;
                        this.txtLiveAltQty.IsEnabled = this.SysEnv.SysSettings.IsLiveQuantityEnabled;
                        this.txtFrozenAltQty.IsEnabled = this.SysEnv.SysSettings.IsFrozenQuantityEnabled;
                        this.txtPalletAltQty.IsEnabled = this.SysEnv.SysSettings.IsPalletQuantityEnabled;
                    }

                    if (this.SelectMode == SysCommon.ShippingLabelQuantitySelectionMode.BothFullRepack || this.SelectMode == SysCommon.ShippingLabelQuantitySelectionMode.RepackOnly)
                    {
                        this.txtRepackAltQty.IsEnabled = this.SysEnv.SysSettings.IsRepackQuantityEnabled;
                        this.txtRepackFreshAltQty.IsEnabled = this.SysEnv.SysSettings.IsFreshRepackQuantityEnabled;
                        this.txtRepackDryAltQty.IsEnabled = this.SysEnv.SysSettings.IsDryRepackQuantityEnabled;
                        this.txtRepackLiveAltQty.IsEnabled = this.SysEnv.SysSettings.IsLiveRepackQuantityEnabled;
                        this.txtRepackFrozenAltQty.IsEnabled = this.SysEnv.SysSettings.IsFrozenRepackQuantityEnabled;
                    }
                }

                this.ucActivityShippingLabelEdit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityShippingLabelEdit_GotFocus threw an error " + ex, "OK"));
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

        private LocationMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }

        private SysCommon.SLabelQuantity LabelQtyEdit
        {
            get
            {
                return this.labelQtyEdit;
            }

            set
            {
                this.labelQtyEdit = value;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityShippingLabelEdit_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    this.LoadData();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                if (string.IsNullOrEmpty(this.txtFullCaseAltQty.Text))
                {
                    this.txtFullCaseAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtRepackAltQty.Text))
                {
                    this.txtRepackAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtDryAltQty.Text))
                {
                    this.txtDryAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtFreshAltQty.Text))
                {
                    this.txtFreshAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtFrozenAltQty.Text))
                {
                    this.txtFrozenAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtLiveAltQty.Text))
                {
                    this.txtLiveAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtRepackDryAltQty.Text))
                {
                    this.txtRepackDryAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtRepackFreshAltQty.Text))
                {
                    this.txtRepackFreshAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtRepackFrozenAltQty.Text))
                {
                    this.txtRepackFrozenAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtRepackLiveAltQty.Text))
                {
                    this.txtRepackLiveAltQty.Text = "0";
                }

                if (string.IsNullOrEmpty(this.txtPalletAltQty.Text))
                {
                    this.txtPalletAltQty.Text = "0";
                }

                this.IsValidData();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Shipping Label Edit";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = true;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.IsChangesMade = false;
                }
                else
                {
                    saveComplete = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                + ("<full_alt_qty>" + this.LabelQtyEdit.FullQuantity + "</full_alt_qty>")
                + ("<repack_alt_qty>" + this.LabelQtyEdit.RepackQuantity + "</repack_alt_qty>")
                + ("<fresh_alt_qty>" + this.LabelQtyEdit.FreshQuantity + "</fresh_alt_qty>")
                + ("<frozen_alt_qty>" + this.LabelQtyEdit.FrozenQuantity + "</frozen_alt_qty>")
                + ("<dry_alt_qty>" + this.LabelQtyEdit.DryQuantity + "</dry_alt_qty>")
                + ("<live_alt_qty>" + this.LabelQtyEdit.LiveQuantity + "</live_alt_qty>")
                + ("<pallet_alt_qty>" + this.LabelQtyEdit.PalletQuantity + "</pallet_alt_qty>")
                + ("<repack_fresh_alt_qty>" + this.LabelQtyEdit.RepackFreshQuantity + "</repack_fresh_alt_qty>")
                + ("<repack_frozen_alt_qty>" + this.LabelQtyEdit.RepackFrozenQuantity + "</repack_frozen_alt_qty>")
                + ("<repack_dry_alt_qty>" + this.LabelQtyEdit.RepackDryQuantity + "</repack_dry_alt_qty>")
                + ("<repack_live_alt_qty>" + this.LabelQtyEdit.RepackLiveQuantity + "</repack_live_alt_qty>")
                + ("<bundle_alt_qty>" + this.LabelQtyEdit.BundleQuantity + "</bundle_alt_qty>")
                + ("<total_alt_qty>" + this.GetTotalQty() + "</total_alt_qty>")
                + "</params>");

            return request;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        /// Returns the sum of all quantities.
        /// </summary>
        /// <returns></returns>
        private int GetTotalQty()
        {
            // Bundle and Pallet not include for the purpose of printing shipping labels.
            ////this.LabelQtyEdit.BundleQuantity + this.LabelQtyEdit.PalletQuantity

            return this.labelQtyEdit.DryQuantity
                + this.LabelQtyEdit.FreshQuantity
                + this.LabelQtyEdit.FrozenQuantity
                + this.LabelQtyEdit.FullQuantity
                + this.LabelQtyEdit.LiveQuantity
                + this.LabelQtyEdit.RepackDryQuantity
                + this.LabelQtyEdit.RepackFreshQuantity
                + this.LabelQtyEdit.RepackFrozenQuantity
                + this.LabelQtyEdit.RepackLiveQuantity
                + this.LabelQtyEdit.RepackQuantity;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.txtFullCaseAltQty.Text = this.LabelQtyEdit.FullQuantity.ToString();
            this.txtRepackAltQty.Text = this.LabelQtyEdit.RepackQuantity.ToString();
            this.txtDryAltQty.Text = this.LabelQtyEdit.DryQuantity.ToString();
            this.txtFreshAltQty.Text = this.LabelQtyEdit.FreshQuantity.ToString();
            this.txtFrozenAltQty.Text = this.LabelQtyEdit.FrozenQuantity.ToString();
            this.txtLiveAltQty.Text = this.LabelQtyEdit.LiveQuantity.ToString();
            this.txtRepackDryAltQty.Text = this.LabelQtyEdit.RepackDryQuantity.ToString();
            this.txtRepackFreshAltQty.Text = this.LabelQtyEdit.RepackFreshQuantity.ToString();
            this.txtRepackFrozenAltQty.Text = this.LabelQtyEdit.RepackFrozenQuantity.ToString();
            this.txtRepackLiveAltQty.Text = this.LabelQtyEdit.RepackLiveQuantity.ToString();
            this.txtPalletAltQty.Text = this.labelQtyEdit.PalletQuantity.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataFullCase()
        {
            if (0 > decimal.Parse(this.txtFullCaseAltQty.Text) && 0 != this.LabelQtyEdit.FullQuantity)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Full case quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.FullQuantity = int.Parse(this.txtFullCaseAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataRepack()
        {
            if (0 >= int.Parse(this.txtRepackAltQty.Text) && this.LabelQtyEdit.IsRepack)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Repack quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.RepackQuantity = int.Parse(this.txtRepackAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataDry()
        {
            if (int.Parse(this.txtDryAltQty.Text) < 0 && this.LabelQtyEdit.DryQuantity != 0)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Dry quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.DryQuantity = int.Parse(this.txtDryAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataFresh()
        {
            if (int.Parse(this.txtFreshAltQty.Text) < 0 && this.LabelQtyEdit.FreshQuantity != 0)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Fresh quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.FreshQuantity = int.Parse(this.txtFreshAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataFrozen()
        {
            if (decimal.Parse(this.txtFrozenAltQty.Text) < 0 && this.LabelQtyEdit.FrozenQuantity != 0)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Frozen quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.FrozenQuantity = int.Parse(this.txtFrozenAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataLive()
        {
            if (decimal.Parse(this.txtLiveAltQty.Text) < 0 && this.LabelQtyEdit.LiveQuantity != 0)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Live quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.LiveQuantity = int.Parse(this.txtLiveAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataRepackDry()
        {
            if (0 >= int.Parse(this.txtRepackDryAltQty.Text) && this.LabelQtyEdit.IsRepackDry)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Repack dry quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.RepackDryQuantity = int.Parse(this.txtRepackDryAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataRepackFresh()
        {
            if (0 >= int.Parse(this.txtRepackFreshAltQty.Text) && this.LabelQtyEdit.IsRepackFresh)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Repack fresh quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.RepackFreshQuantity = int.Parse(this.txtRepackFreshAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataRepackFrozen()
        {
            if (0 >= int.Parse(this.txtRepackFrozenAltQty.Text) && this.LabelQtyEdit.IsRepackFrozen)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Repack frozen quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.RepackFrozenQuantity = int.Parse(this.txtRepackFrozenAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataRepackLive()
        {
            if (0 >= int.Parse(this.txtRepackLiveAltQty.Text) && this.LabelQtyEdit.IsRepackLive)
            {
                // TODO: Resource stringss
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Repack live quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.RepackLiveQuantity = int.Parse(this.txtRepackLiveAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void IsValidDataPallet()
        {
            if (decimal.Parse(this.txtPalletAltQty.Text) < 0 && this.LabelQtyEdit.PalletQuantity != 0)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Invalid quantity entered. Pallet quantity must be greater than zero.");
            }
            else
            {
                SysCommon.SLabelQuantity lbl = this.LabelQtyEdit;
                lbl.PalletQuantity = int.Parse(this.txtPalletAltQty.Text);
                this.LabelQtyEdit = lbl;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private void IsValidData()
        {
            this.IsValidDataFullCase();
            this.IsValidDataRepack();
            this.IsValidDataDry();
            this.IsValidDataFresh();
            this.IsValidDataFrozen();
            this.IsValidDataLive();
            this.IsValidDataRepackDry();
            this.IsValidDataRepackFresh();
            this.IsValidDataRepackFrozen();
            this.IsValidDataRepackLive();
            this.IsValidDataPallet();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFullCaseAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtFullCaseAltQty.SelectAll();

            if (this.txtFullCaseAltQty.Text != null)
            {
                this.txtFullCaseAltQty.CursorPosition = 0;
                this.txtFullCaseAltQty.SelectionLength = this.txtFullCaseAltQty.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtRepackAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtRepackAltQty.SelectAll();

            if (this.txtRepackAltQty.Text != null)
            {
                this.txtRepackAltQty.CursorPosition = 0;
                this.txtRepackAltQty.SelectionLength = this.txtRepackAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFreshAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtFreshAltQty.SelectAll();

            if (this.txtFreshAltQty.Text != null)
            {
                this.txtFreshAltQty.CursorPosition = 0;
                this.txtFreshAltQty.SelectionLength = this.txtFreshAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFrozenAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtFrozenAltQty.SelectAll();

            if (this.txtFrozenAltQty.Text != null)
            {
                this.txtFrozenAltQty.CursorPosition = 0;
                this.txtFrozenAltQty.SelectionLength = this.txtFrozenAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDryAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtDryAltQty.SelectAll();

            if (this.txtDryAltQty.Text != null)
            {
                this.txtDryAltQty.CursorPosition = 0;
                this.txtDryAltQty.SelectionLength = this.txtDryAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLiveAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtLiveAltQty.SelectAll();

            if (this.txtLiveAltQty.Text != null)
            {
                this.txtLiveAltQty.CursorPosition = 0;
                this.txtLiveAltQty.SelectionLength = this.txtLiveAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRepackFreshAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtRepackFreshAltQty.SelectAll();

            if (this.txtRepackFreshAltQty.Text != null)
            {
                this.txtRepackFreshAltQty.CursorPosition = 0;
                this.txtRepackFreshAltQty.SelectionLength = this.txtRepackFreshAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRepackFrozenAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtRepackFrozenAltQty.SelectAll();

            if (this.txtRepackFrozenAltQty.Text != null)
            {
                this.txtRepackFrozenAltQty.CursorPosition = 0;
                this.txtRepackFrozenAltQty.SelectionLength = this.txtRepackFrozenAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRepackDryAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtRepackDryAltQty.SelectAll();

            if (this.txtRepackDryAltQty.Text != null)
            {
                this.txtRepackDryAltQty.CursorPosition = 0;
                this.txtRepackDryAltQty.SelectionLength = this.txtRepackDryAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRepackLiveAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtRepackLiveAltQty.SelectAll();

            if (this.txtRepackLiveAltQty.Text != null)
            {
                this.txtRepackLiveAltQty.CursorPosition = 0;
                this.txtRepackLiveAltQty.SelectionLength = this.txtRepackLiveAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPalletAltQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtPalletAltQty.SelectAll();

            if (this.txtPalletAltQty.Text != null)
            {
                this.txtPalletAltQty.CursorPosition = 0;
                this.txtPalletAltQty.SelectionLength = this.txtPalletAltQty.Text.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdLocation_Click(object sender, EventArgs e)
        {
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
            }
            else
            {
                if (this.LocMap.CurrLocMap.WhsLocMapRefno == 0)
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
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
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                await this.Close();
            }
        }

        #endregion
    }
}