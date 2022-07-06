// <copyright file="ActivityRemoveFromSkid.xaml.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityRemoveFromSkid : DMobileModal, IAppArea
    {
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
/// <summary>
/// Initializes a new instance of the <see cref="ActivityRemoveFromSkid"/> class.
/// </summary>
/// <param name="actDetail"></param>
/// <param name="actSubTypes"></param>
        public ActivityRemoveFromSkid(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblWtUnit.Text = "KG";
            this.lblUnit.Text = "Bx";
            this.lblWeight.Text = "Weight";
            this.lblQty.Text = "Quantity";
            this.btnSave.Text = "Save";
            this.btnCancel.Text = "Cancel";

            if (!DesignMode.IsDesignModeEnabled)
            {
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
                    if (keycode == Keycode.F1 || keycode == Keycode.Enter)
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
                this.ucActivityRemoveFromSkid_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityRemoveFromSkid_GotFocus threw an error " + ex, "OK"));
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

        private SysEnvironment SysEnv { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private ActDetailEdit ActDetailEdit { get; set; }

        private bool IsControlLoaded { get; set; } = false;

        private bool IsChangesMade { get; set; } = false;

        private ActDetail ActDetail { get; set; } = null;

        public ObservableCollection<string> ExtraInfoItems { get => this.itemsList; set => this.itemsList = value; }

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
                isReadyToSave = true;// (decimal.Parse(this.txtAltQty.Text) != 0) && this.IsValidData();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
        public string GetControlDescription()
        {
            return "Remove From Skid";
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
                    saveComplete = await this.IsValidData();
                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<alt_qty>" + this.ActDetailEdit.AltQuantity + "</alt_qty>"
                + "<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>"
                + "<base_qty>" + this.ActDetailEdit.BaseQuantity + "</base_qty>"
                + "</params>");

            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<bool> IsValidData()
        {
            bool isRetVal = false;

            decimal qty = decimal.Parse(this.txtAltQty.Text);
            decimal lineQty = this.ActDetail.Quantity;
            // We assign how much we want to remove
            decimal newQty = qty;
            string baseUnit = this.ActDetail.ActItem.ProdInfo.BaseUnitCode;
            decimal newWtQty = decimal.Parse(this.txtWtQty.Text);
            decimal newBaseQty = this.ActDetail.BaseQuantity;

            if (baseUnit == "LB" || baseUnit == "KG")
            {
                newBaseQty = newWtQty;
            }
            else
            {
                if (this.ActDetailEdit.AltQuantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    newBaseQty = newQty * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    newBaseQty = newQty;
                }
            }

            if (qty == lineQty)
            {
                this.ActDetailEdit.AltQuantity = newQty;
                this.ActDetailEdit.WeightQuantity = newWtQty;
                this.ActDetailEdit.BaseQuantity = newBaseQty;

                isRetVal = true;
            }
            else if (qty == 0)
            {
                // Exit without doing anything
                isRetVal = true;
            }
            else if (qty > lineQty)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Cannot remove more than exists on skid!", "OK"));
            }
            else if (qty < 0)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Cannot remove negative quantity!", "OK"));
            }
            else
            {
                if (Convert.ToInt32(qty) != qty)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Cannot remove partial boxes!", "OK"));
                }
                else
                {
                    // Good
                    this.ActDetailEdit.AltQuantity = newQty;
                    this.ActDetailEdit.WeightQuantity = newBaseQty;
                    this.ActDetailEdit.BaseQuantity = newBaseQty;
                    isRetVal = true;
                }
            }

            return isRetVal;
        }

        private async Task PromptConsumeSkid()
        {
            if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Do You Wish To Consume This Entire Skid?", "Yes", "No")))
            {
                await this.ActDetail.Delete();

                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.IsConsumed)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "This skid is now consumed.", "OK"));

                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        await this.Close();
                    }
                    else
                    {
                        this.DialogResult = DialogResult.None;
                        _ = await this.CancelChanges();
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplaySummary()
        {
            SSkidInfo skid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

            this.ExtraInfoItems.Clear();
            this.ExtraInfoItems.Add("Product: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + "  " + this.ActDetail.ActItem.ProdInfo.Desc);
            this.ExtraInfoItems.Add(string.Format("Production Date: {0}", this.ActDetail.SchemeData.LogisticUnitProductionDate));
            this.ExtraInfoItems.Add(string.Format("Quantity {0} {1} @ {2} {3}", this.ActDetail.Quantity, skid.ProdInfo.UnitCode, this.ActDetail.WeightQuantity, skid.ProdInfo.WeightUnitCode));
            this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
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
            this.ActDetailEdit.AltQuantity = 0;
            this.ActDetailEdit.BaseQuantity = 0;

            this.txtAltQty.Text = "0";
            this.txtWtQty.Text = "0";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityRemoveFromSkid_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
                this.lblUnit.Text = this.ActDetail.ActItem.ProdInfo.UnitCode;
                this.lblWtUnit.Text = this.ActDetail.ActItem.ProdInfo.WeightUnitCode;

                if (!this.ActDetail.ActItem.ProdInfo.IsCatchWeight ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    // disable when not catch weight, or when qty is LB/KG
                    this.txtWtQty.IsEnabled = false;
                }

                this.LoadData();
                this.DisplaySummary();

                _ = this.txtAltQty.Focus();
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
                wtQty = qtyEntered * this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
            }

            this.txtWtQty.Text = Math.Round(wtQty, 3).ToString();
        }

        private void txtWtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtAltQty_Focused(object sender, FocusEventArgs e)
        {
            if (this.txtAltQty.Text != null)
            {
                this.txtAltQty.CursorPosition = 0;
                this.txtAltQty.SelectionLength = this.txtAltQty.Text.Length;
            }
        }

        private void txtWtQty_Focused(object sender, FocusEventArgs e)
        {
            if (this.txtWtQty.Text != null)
            {
                this.txtWtQty.CursorPosition = 0;
                this.txtWtQty.SelectionLength = this.txtWtQty.Text.Length;
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
        }
    }
}