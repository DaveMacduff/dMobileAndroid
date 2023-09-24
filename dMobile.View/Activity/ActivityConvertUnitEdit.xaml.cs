// <copyright file="ActivityConvertUnitEdit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Controller.Activity_Detail;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertUnitEdit : DMobileModal, IAppArea
    {
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityConvertUnitEdit"/> class.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail"/> Activity detail.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        public ActivityConvertUnitEdit(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.LblWtUnit.Text = "KG";
            this.LblUnit.Text = "KG";
            this.LblWeight.Text = "Weight";
            this.LblQty.Text = "Quantity";
            this.LblSplitToSingles.Text = "Split To Singles";
            this.LblEquals.Text = "Equals";
            this.LblFromUnitDesc.Text = "From Unit";
            this.CmdSave.Text = "Save";
            this.CmdCancel.Text = "Cancel";

            this.SysEnv = SysEnvironment.GetInstance();
            this.ActSubType = actSubTypes;
            this.ActDetailEdit = new ActDetailEdit();
            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

            this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
            this.ActDetailEdit.AltQuantity = this.ActDetail.Quantity;
            this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
        }

        private SysEnvironment SysEnv { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.ConvertUnit;

        private DataView DataViewSerialNumbers { get; set; } = null;

        private DataView DataViewLots { get; set; } = null;

        private DataView DataViewVariants { get; set; } = null;

        private DataTable DataTableActDetail { get; set; }

        private ActDetailEdit ActDetailEdit { get; set; }

        private bool IsControlLoaded { get; set; } = false;

        private bool IsChangesMade { get; set; } = false;

        private ActDetail ActDetail { get; set; } = null;

        public ObservableCollection<string> ExtraInfoItems { get => this.itemsList; set => this.itemsList = value; }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);
            });

            try
            {
                this.ucActivityDetailEdit_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "this.ucActivityDetailEdit_GotFocus threw an error " + ex,
                    "OK"));
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
                else
                {
                    this.DialogResult = DialogResult.None;
                    _ = await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
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
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
                isReadyToSave = isReadyToSave && !(decimal.Parse(this.TxtAltQty.Text) == 0 && decimal.Parse(this.TxtWtQty.Text) == 0);
                isReadyToSave = isReadyToSave && this.IsValidData();
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
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
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = false;

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
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
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
            return "Convert Units Edit";
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
                    saveComplete = this.IsValidData();
                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Data Validation", "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return saveComplete;
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
                if (await this.IsReadyToClose())
                {
                    this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
                    this.ActDetailEdit.AltQuantity = this.ActDetail.Quantity;
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
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
        /// <inheritdoc/>
        public async Task<XDocument> GetXML()
        {
            var unqScheme = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            var scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            var schemeData = this.ActDetailEdit.SchemeData;

            var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                          + "<params>"
                                          + "<serial>" + schemeData.SingleTradeItemSerialNumber + "</serial>"
                                          + "<pdate>" + schemeData.LogisticUnitProductionDate + "</pdate>"
                                          + "<lot_no>" + schemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                                          + "<bdate>" + schemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                                          + "<variant_code>" + schemeData.VariantCode + "</variant_code>"
                                          + "<variant_no>" + schemeData.VariantNumber + "</variant_no>"
                                          + "<alt_qty>" + this.ActDetailEdit.AltQuantity + "</alt_qty>"
                                          + "<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>"
                                          + "<base_qty>" + this.ActDetailEdit.BaseQuantity + "</base_qty>"
                                          + "<scheme_id>" + scheme + "</scheme_id>"
                                          + "<unq_scheme_id>" + unqScheme + "</unq_scheme_id>"
                                          + "<split>" + this.ChkSplitToSingles.IsToggled + "</split>"
                                          + "</params>");

            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            var isRetVal = true;

            this.ActDetailEdit.AltQuantity = decimal.Parse(this.TxtAltQty.Text);
            this.ActDetailEdit.WeightQuantity = decimal.Parse(this.TxtWtQty.Text);

            if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
            {
                this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.WeightQuantity;
            }
            else
            {
                if (this.ActDetailEdit.AltQuantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.AltQuantity * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.AltQuantity;
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
            this.LblFromUnitDesc.Text = string.Format(
                "{0} {1} @ {2} {3}",
                (this.ActDetail.ActItem.ListItems[0] as dynamic).ActivityDetailQuantity,
                (this.ActDetail.ActItem.ListItems[0] as dynamic).UnitCode,
                (this.ActDetail.ActItem.ListItems[0] as dynamic).ActivityDetailWeightQuantity,
                this.ActDetail.ActItem.ProdInfo.WeightUnitCode);

            this.ExtraInfoItems = new ObservableCollection<string>();
            this.ExtraInfoItems.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + "  " + this.ActDetail.ActItem.ProdInfo.Desc);
            this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);

            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
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
                        this.ActDetailEdit.AltQuantity = this.ActDetail.WeightQuantity;
                    }
                    else
                    {
                        this.ActDetailEdit.AltQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
                    }
                }
                else
                {
                    this.ActDetailEdit.AltQuantity = 1;
                }
            }
            else
            {
                this.ActDetailEdit.AltQuantity = this.ActDetail.Quantity;
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
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
                    }
                }
                else
                {
                    if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB")
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
                    }
                    else
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
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

            this.ActDetailEdit.AltQuantity = decimal.Round(this.ActDetailEdit.AltQuantity, 3);
            this.ActDetailEdit.BaseQuantity = decimal.Round(this.ActDetailEdit.BaseQuantity, 3);
            this.ActDetailEdit.WeightQuantity = decimal.Round(this.ActDetailEdit.WeightQuantity, 3);

            this.TxtAltQty.Text = this.ActDetailEdit.AltQuantity.ToString();
            this.TxtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityDetailEdit_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
                this.LblUnit.Text = this.ActDetail.ActItem.ProdInfo.UnitCode;
                this.LblWtUnit.Text = this.ActDetail.ActItem.ProdInfo.WeightUnitCode;

                if (!this.ActDetail.ActItem.ProdInfo.IsCatchWeight || this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    this.TxtWtQty.IsEnabled = false;
                }

                this.LoadData();
                this.ShowProductInfo();

                _ = this.TxtAltQty.Focus();
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
                if (!string.IsNullOrEmpty(this.TxtAltQty.Text))
                {
                    qtyEntered = Convert.ToDecimal(this.TxtAltQty.Text);
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
                // Otherwise the weight = quantity * weightPerOneQty
                wtQty = qtyEntered * this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
            }

            this.TxtWtQty.Text = Math.Round(wtQty, 3).ToString();
        }

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
            _ = await this.CancelChanges();
            await this.Close();
        }
    }
}