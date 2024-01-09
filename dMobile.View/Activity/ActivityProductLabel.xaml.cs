// <copyright file="ActivityProductLabel.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using CustomPages.Modal;
    using Forms;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="ActivityProductLabel" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityProductLabel : DMobileModal, IAppArea
    {
        /// <summary>
        ///     Defines the itemsList.
        /// </summary>
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityProductLabel" /> class.
        /// </summary>
        public ActivityProductLabel()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.lblWtUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.lblVariantCode.Text = "Variant";
            this.lblBestBeforeDate.Text = "Best Before";
            this.lblLotNo.Text = "LotNo";
            this.lblProductionDate.Text = "Production";
            this.lblSerialNo.Text = "Serial No";
            this.cmdSelectProduct.Text = "Select Product";
            this.lblQuantity.Text = "Quantity";
            this.cmdSave.Text = "Save";

            this.SysEnv = SysEnvironment.GetInstance();
            var act = ActivityFacade.GetActivity(this.ActSubType);
            var actItem = ActItemFacade.GetActItem(act, this.ActSubType);
            this.ActDetail = ActDetailFacade.GetActDetail(actItem, this.ActSubType);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityProductLabel" /> class.
        /// </summary>
        /// <param name="actDetail">.</param>
        /// <param name="actSubTypes">.</param>
        public ActivityProductLabel(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.lblWtUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.lblVariantCode.Text = "Variant";
            this.lblBestBeforeDate.Text = "Best Before";
            this.lblLotNo.Text = "LotNo";
            this.lblProductionDate.Text = "Production";
            this.lblSerialNo.Text = "Serial No";
            this.cmdSelectProduct.Text = "Select Product";
            this.lblQuantity.Text = "Quantity";
            this.cmdSave.Text = "Save";

            this.SysEnv = SysEnvironment.GetInstance();

            this.ActSubType = actSubTypes;

            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

            this.ActDetailEdit = new ActDetailEdit();
            this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
            this.ActDetailEdit.AltQuantity = this.ActDetail.Quantity;
            this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
            this.ActDetailEdit.ActExtra.Text1 = this.ActDetail.ActExtra.Text1;
            this.ActDetailEdit.ActExtra.Text2 = this.ActDetail.ActExtra.Text2;
            this.ActDetailEdit.ActExtra.Text3 = this.ActDetail.ActExtra.Text3;
            this.ActDetailEdit.ActExtra.Text4 = this.ActDetail.ActExtra.Text4;
            this.ActDetailEdit.ActExtra.DateTime1 = this.ActDetail.ActExtra.DateTime1;
        }

        /// <summary>
        ///     Gets or sets the SysEnv.
        /// </summary>
        public SysEnvironment SysEnv { get; set; }

        /// <summary>
        ///     Gets or sets the ActSubType.
        /// </summary>
        public SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.PrintProductLabel;

        /// <summary>
        ///     Gets or sets a value indicating whether IsEditTrapped.
        /// </summary>
        public bool IsEditTrapped { get; set; } = false;

        /// <summary>
        ///     Gets or sets the ListSerialNumbers.
        /// </summary>
        public List<SerialNumber> ListSerialNumbers { get; set; }

        /// <summary>
        ///     Gets or sets the ListLots.
        /// </summary>
        public List<LotNumber> ListLots { get; set; }

        /// <summary>
        ///     Gets or sets the ListVariants.
        /// </summary>
        public List<ProductVariant> ListVariants { get; set; }

        /// <summary>
        ///     Gets or sets the ActDetailEdit.
        /// </summary>
        public ActDetailEdit ActDetailEdit { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether IsControlLoaded.
        /// </summary>
        public bool IsControlLoaded { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether IsChangesMade.
        /// </summary>
        public bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets the ActDetail.
        /// </summary>
        public ActDetail ActDetail { get; set; }

        /// <summary>
        ///     Gets or sets the ExtraInfoItems.
        /// </summary>
        public ObservableCollection<string> ExtraInfoItems
        {
            get => this.itemsList;
            set => this.itemsList = value;
        }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        ///     The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                isReadyToSave = await this.IsValidData();
            }
            catch (IsReadyToApplyException ex)
            {
                if (ex.SuppressError && ex.IsOverContinue)
                {
                    isReadyToSave = true;
                }
                else if (ex.SuppressError && !ex.IsOverContinue)
                {
                    isReadyToSave = false;
                }
                else
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
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
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///     The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
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
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///     The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    if (await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(this.GetControlDescription(), "Print product labels?", "Yes",
                                "No")))
                    {
                        var sendReq = await this.ActDetail.CreateSubmitProductLabelXml(await this.GetXml());
                        SysCommon.SubmitComm(sendReq);
                        this.IsChangesMade = false;
                        saveComplete = true;
                    }
                }
                else
                {
                    throw new MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (CommunicationException commEx)
            {
                var msg = commEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
                    ? commEx.Message
                    : commEx.Message["MSGBOX-".Length..];
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///     The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
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
                    this.ActDetailEdit.ActExtra.Text1 = this.ActDetail.ActExtra.Text1;
                    this.ActDetailEdit.ActExtra.Text2 = this.ActDetail.ActExtra.Text2;
                    this.ActDetailEdit.ActExtra.Text3 = this.ActDetail.ActExtra.Text3;
                    this.ActDetailEdit.ActExtra.Text4 = this.ActDetail.ActExtra.Text4;
                    this.ActDetailEdit.ActExtra.DateTime1 = this.ActDetail.ActExtra.DateTime1;
                }
                else
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });

                cancelChanges = false;
            }

            return cancelChanges;
        }

        /// <summary>
        ///     The GetXML.
        /// </summary>
        /// <returns>.</returns>
        public async Task<XDocument> GetXml()
        {
            return await this.ActDetailEdit.GetXmlFromActivityDetailEdit(this.ActDetailEdit);
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
                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

                    MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.IsIndicatorRunning)
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

                    await this.ucActivityProductLabel_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Error", "this.ucActivityProductLabel_GotFocus threw an error " + ex,
                            "OK"));
                }
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool" />.</returns>
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

        /// <summary>
        ///     The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Product Label Edit";
        }

        /// <summary>
        ///     The txtWtQty_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        public void txtWtQty_GotFocus(object sender, EventArgs e)
        {
            if (this.txtWtQty.Text != null)
            {
                this.txtWtQty.CursorPosition = 0;
                this.txtWtQty.SelectionLength = this.txtWtQty.Text.Length;
            }
        }

        /// <summary>
        ///     The txtQuantity_GotFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
        private void txtQuantity_GotFocus(object sender, EventArgs e)
        {
            if (this.txtAltQuantity.Text != null)
            {
                this.txtAltQuantity.CursorPosition = 0;
                this.txtAltQuantity.SelectionLength = this.txtAltQuantity.Text.Length;
            }
        }

        /// <summary>
        ///     ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <summary>
        ///     .
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task ucActivityProductLabel_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                var scheme = prodInfo.Scheme;

                this.cmdSelectProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(prodInfo);

                this.lblWtUnit.Text = prodInfo.WeightUnitCode;

                if (!(scheme.IsBestBeforeDateTracked || scheme.LabelInfo.IsBestBeforeDateTracked))
                {
                    this.dtBestBeforeDate.IsEnabled = false;
                }

                if (!(scheme.IsProductionDateTracked || scheme.LabelInfo.IsProductionDateTracked))
                {
                    this.dtProductionDate.IsEnabled = false;
                }

                if (!(scheme.IsLotTracked || scheme.LabelInfo.IsLotTracked))
                {
                    this.cboLotNo.IsEnabled = false;
                }

                if (!(scheme.IsSerialTracked || scheme.LabelInfo.IsSerialTracked))
                {
                    this.cboSerialNo.IsEnabled = false;
                }

                if (!scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked)
                {
                    this.cboVariantCode.IsEnabled = false;
                }

                if (!prodInfo.IsCatchWeight || prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    this.txtWtQty.IsEnabled = false;
                }

                await this.GetProductVariantList(prodInfo.Refno);
                await this.GetSerialNumberList(prodInfo.Refno);
                await this.GetLotNumberList(prodInfo.Refno);

                this.LoadData();
                this.ShowProductInfo();

                this.IsControlLoaded = true;

                _ = this.txtAltQuantity.Focus();
            }
        }

        /// <summary>
        ///     The LoadData.
        /// </summary>
        private void LoadData()
        {
            var prodInfo = this.ActDetail.ActItem.ProdInfo;
            var scheme = prodInfo.Scheme;

            if (scheme.IsBestBeforeDateTracked || scheme.LabelInfo.IsBestBeforeDateTracked)
            {
                if (!SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
                {
                    this.dtBestBeforeDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate;
                }
            }

            if (scheme.IsProductionDateTracked || scheme.LabelInfo.IsProductionDateTracked)
            {
                if (!SysCommon.IsEmptyDate(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
                {
                    this.dtProductionDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitProductionDate;
                }
            }

            if (scheme.IsSerialTracked || scheme.LabelInfo.IsSerialTracked)
            {
                if (string.IsNullOrEmpty(this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber))
                {
                    var schemeData = this.ActDetailEdit.SchemeData;
                    schemeData.SingleTradeItemSerialNumber =
                        ProductInfoTasks.GetProdSchemeSingleTradeItemSerialNumber();
                    this.ActDetailEdit.SchemeData = schemeData;
                }

                this.cboSerialNo.Text = this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber;
            }

            if (scheme.IsLotTracked || scheme.LabelInfo.IsLotTracked)
            {
                this.cboLotNo.SelectedItem = this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber;
            }

            if (scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked)
            {
                this.cboVariantCode.SelectedItem = this.ActDetailEdit.SchemeData.VariantNumber;
            }

            // Default quantities if zero
            if (this.ActDetail.Quantity == 0)
            {
                if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.WeightQuantity != 0)
                    {
                        this.ActDetailEdit.AltQuantity = this.ActDetail.WeightQuantity;
                    }
                    else
                    {
                        this.ActDetailEdit.AltQuantity = prodInfo.UnitGrossWeight;
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
                if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.Quantity != 0)
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.Quantity;
                    }
                    else
                    {
                        if (!(this.SysEnv.SysSettings.IsSetCatchWeightZero && prodInfo.IsCatchWeight))
                        {
                            this.ActDetailEdit.WeightQuantity = prodInfo.UnitGrossWeight;
                        }
                    }
                }
                else
                {
                    if (prodInfo.BaseUnitCode == "KG" || prodInfo.BaseUnitCode == "LB")
                    {
                        if (!(this.SysEnv.SysSettings.IsSetCatchWeightZero && prodInfo.IsCatchWeight))
                        {
                            this.ActDetailEdit.WeightQuantity = prodInfo.UnitGrossWeight;
                        }
                    }
                    else
                    {
                        this.ActDetailEdit.WeightQuantity = prodInfo.UnitGrossWeight;
                    }
                }
            }
            else
            {
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
            }

            if (this.ActDetail.BaseQuantity == 0)
            {
                if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.WeightQuantity;
                }
                else
                {
                    if (this.ActDetail.Quantity != 0 && prodInfo.Unit2BaseUnitFactor != 0)
                    {
                        if (this.SysEnv.SysSettings.IsSetCatchCountZero && prodInfo.IsCatchCount)
                        {
                            this.ActDetailEdit.BaseQuantity = this.ActDetail.Quantity * prodInfo.Unit2BaseUnitFactor;
                        }
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

            this.txtAltQuantity.Text = this.ActDetailEdit.AltQuantity.ToString();
            this.txtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString();
        }

        /// <summary>
        ///     The GetProductVariantList.
        /// </summary>
        /// <param name="prodRefno">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task GetProductVariantList(int prodRefno)
        {
            this.ListVariants = await this.ActDetailEdit.GetVariantList(prodRefno);
            var productVariantList = new ObservableCollection<object>();

            foreach (var productVariant in this.ListVariants)
            {
                // ProductVariant productVariant = new ProductVariant()
                // {
                //    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                //    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                //    ProductSchemeVariantDisplay = Convert.ToString(row["ProductSchemeVariantDisplay"] != DBNull.Value ? row["ProductSchemeVariantDisplay"] : null),
                // };
                productVariantList.Add(productVariant);
            }

            this.cboVariantCode.ItemDisplayBinding = new Binding("ProductSchemeVariantDisplay");
            this.cboVariantCode.ItemsSource = productVariantList;
        }

        /// <summary>
        ///     The GetLotNumberList.
        /// </summary>
        /// <param name="prodRefno">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task GetLotNumberList(int prodRefno)
        {
            this.ListLots = await this.ActDetailEdit.GetLotList(prodRefno);
            var lotNumberList = new ObservableCollection<object>();

            foreach (var lotNumber in this.ListLots)
            {
                // LotNumber lotNumber = new LotNumber()
                // {
                //    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null)
                // };
                lotNumberList.Add(lotNumber);
            }

            this.cboLotNo.ItemDisplayBinding = new Binding("ActivityDetailLogisticUnitBatchLotNumber");
            this.cboLotNo.ItemsSource = lotNumberList;
        }

        /// <summary>
        ///     The GetSerialNumberList.
        /// </summary>
        /// <param name="prodRefno">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task GetSerialNumberList(int prodRefno)
        {
            this.ListSerialNumbers = await this.ActDetailEdit.GetSerialNumberList(prodRefno);
            var serialNumberList = new ObservableCollection<SerialNumber>();
            var selectedSerialNumberItem = this.cboSerialNo.Text;
            var selectedSerialNumber = new SerialNumber();

            if (selectedSerialNumberItem != null)
            {
                selectedSerialNumber.ActivityDetailSingleTradeItemSerialNumber =
                    Convert.ToString(selectedSerialNumberItem);
                serialNumberList.Add(selectedSerialNumber);
            }

            foreach (var serialNumber in this.ListSerialNumbers)
            {
                // SerialNumber serialNumber = new SerialNumber()
                // {
                //    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null)
                // };
                serialNumberList.Add(serialNumber);
            }

            this.cboSerialNo.ItemDisplayBinding = new Binding("ActivityDetailSingleTradeItemSerialNumber");
            this.cboSerialNo.ItemsSource = serialNumberList;

            if (selectedSerialNumber.ActivityDetailSingleTradeItemSerialNumber != null)
            {
                this.cboSerialNo.Text = selectedSerialNumber.ActivityDetailSingleTradeItemSerialNumber;
            }
        }

        /// <summary>
        ///     The IsValidData.
        /// </summary>
        /// <returns>.</returns>
        private async Task<bool> IsValidData()
        {
            var isRetVal = true;
            var scheme = this.ActDetail.ActItem.ProdInfo.Scheme;
            var s = this.ActDetailEdit.SchemeData;
            s.SingleTradeItemSerialNumber = this.cboSerialNo.Text;
            s.LogisticUnitBatchLotNumber = this.cboLotNo.Text;

            if (scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked)
            {
                var variant = this.cboVariantCode != null ? this.cboVariantCode.SelectedItem as ProductVariant : null;
                s.VariantNumber = Convert.ToInt32(variant != null ? variant.ProductSchemeVariantNo : -1);
                s = await this.ActDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(s);
            }

            s.LogisticUnitProductionDate = this.dtProductionDate.Date;
            s.LogisticUnitBestBeforeSellByDate = this.dtBestBeforeDate.Date;
            this.ActDetailEdit.SchemeData = s;

            this.ActDetailEdit.AltQuantity = decimal.Parse(this.txtAltQuantity.Text);

            if (this.ActDetailEdit.AltQuantity == 0)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(this.GetControlDescription(), "Invalid Quantity.", "OK"));
                isRetVal = false;
            }

            this.ActDetailEdit.WeightQuantity = decimal.Parse(this.txtWtQty.Text);

            if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" ||
                this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
            {
                this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.WeightQuantity;
            }
            else
            {
                if (this.ActDetailEdit.AltQuantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.AltQuantity *
                                                      this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.AltQuantity;
                }
            }

            if (isRetVal)
            {
                if ((scheme.IsSerialTracked || scheme.LabelInfo.IsSerialTracked) &&
                    this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber == string.Empty)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(), "Serial number required.", "OK"));
                    isRetVal = false;
                }
            }

            if (isRetVal)
            {
                if ((scheme.IsLotTracked || scheme.LabelInfo.IsLotTracked) &&
                    this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber == string.Empty)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(), "Lot number required.", "OK"));
                    isRetVal = false;
                }
            }

            if (isRetVal)
            {
                if ((scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked) &&
                    this.ActDetailEdit.SchemeData.VariantCode == string.Empty)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(), "Variant code required.", "OK"));
                    isRetVal = false;
                }
            }

            if (scheme.IsBestBeforeDateTracked &&
                SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                throw new MessageOnlyException("Best before date required.");
            }

            if (this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
            {
                var lower = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                var upper = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);

                if (this.dtBestBeforeDate.Date < lower || this.dtBestBeforeDate.Date > upper)
                {
                    throw new MessageOnlyException("The entered best before date is outside an acceptable range.");
                }
            }

            if (scheme.IsProductionDateTracked &&
                SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
            {
                throw new MessageOnlyException("Production date required.");
            }

            if (this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
            {
                var lower = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                var upper = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);

                if (this.dtProductionDate.Date < lower || this.dtProductionDate.Date > upper)
                {
                    throw new MessageOnlyException("The entered production date is outside an acceptable range.");
                }
            }

            if (isRetVal)
            {
                this.ActDetail.SchemeId = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                    this.ActDetail.ActItem.ProdInfo.Code,
                    this.ActDetail.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);

                this.ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                    this.ActDetail.ActItem.ProdInfo.Code,
                    this.ActDetail.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<itm_refno>" + this.ActDetail.ActItem.ItemFkRefno + "</itm_refno>"
                    + "<alt_qty>" + this.ActDetailEdit.AltQuantity + "</alt_qty>"
                    + "<wt_qty>" + this.ActDetailEdit.AltQuantity + "</wt_qty>"
                    + "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeId +
                    "</ActivityDetailUniqueSchemeID>"
                    + "</imclient>");

                await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
            }

            return isRetVal;
        }

        /// <summary>
        ///     The ShowProductInfo.
        /// </summary>
        private void ShowProductInfo()
        {
            this.ExtraInfoItems.Clear();

            this.ExtraInfoItems.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code +
                                    this.ActDetail.ActItem.ProdInfo.UnitBC + "  " +
                                    this.ActDetail.ActItem.ProdInfo.Desc);
            this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
        }

        /// <summary>
        ///     The cmdSelectProduct_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void cmdSelectProduct_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl causes a click event to fire if it is the next control
                var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXML();
                    var prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);

                    if (prodUnitRefno != 0)
                    {
                        var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        this.ActDetail.ActItem.ProdInfo = prodInfo;

                        this.cmdSelectProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(prodInfo);
                    }
                }
            }
        }

        /// <summary>
        ///     The txtQuantity_TextChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            this.IsChangesMade = true;

            decimal qtyEntered = 0;

            try
            {
                if (!string.IsNullOrEmpty(this.txtAltQuantity.Text))
                {
                    qtyEntered = Convert.ToDecimal(this.txtAltQuantity.Text);
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

        /// <summary>
        ///     The cmdSave_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
        private async void cmdSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }
    }
}