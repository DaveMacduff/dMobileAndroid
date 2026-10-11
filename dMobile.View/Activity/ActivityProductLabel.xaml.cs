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
    using dMobile.BarCodeScanner.BarCodeScanners;
    using Forms;
    using Resources.Enums;
    using Resources.Types;
    using Resources.Types.Products;
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
        ///     Initializes a new instance of the <see cref="ActivityProductLabel" /> class.
        /// </summary>
        public ActivityProductLabel()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.SysEnv = SysEnvironment.GetInstance();
            var act = ActivityFacade.GetActivity(this.ActSubType);
            var actItem = ActItemFacade.GetActItem(act, this.ActSubType);
            this.ActDetail = ActDetailFacade.GetActDetail(actItem, this.ActSubType);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityProductLabel" /> class.
        /// </summary>
        /// <param name="actDetail">Activity detail instance.</param>
        /// <param name="actSubTypes">Activity sub type.</param>
        public ActivityProductLabel(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.SysEnv = SysEnvironment.GetInstance();
            this.ActSubType = actSubTypes;
            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

            this.ActDetailEdit = new ActDetailEdit
            {
                SchemeData = this.ActDetail.LabelSchemeData,
                Quantity = this.ActDetail.Quantity,
                BaseQuantity = this.ActDetail.BaseQuantity,
                WeightQuantity = this.ActDetail.WeightQuantity,
                ActExtra =
                {
                    Text1 = this.ActDetail.ActExtra.Text1,
                    Text2 = this.ActDetail.ActExtra.Text2,
                    Text3 = this.ActDetail.ActExtra.Text3,
                    Text4 = this.ActDetail.ActExtra.Text4,
                    DateTime1 = this.ActDetail.ActExtra.DateTime1
                }
            };
        }

        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        private SysEnvironment SysEnv { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.PrintProductLabel;

        private List<SerialNumber> ListSerialNumbers { get; set; }

        private List<LotNumber> ListLots { get; set; }

        private List<ProductVariant> ListVariants { get; set; }

        private ActDetailEdit ActDetailEdit { get; set; }

        private bool IsFirstAppearance { get; set; }

        private bool IsChangesMade { get; set; }

        private ActDetail ActDetail { get; set; }

        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            this.Scanner.ClearBarCodeScanEventHandlers();
            await this.Scanner.DisableScanning();
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                await this.Scanner.EnableScanning();
            }
        }

        /// <summary>
        ///     Is ready to save the data.
        /// </summary>
        /// <returns>True if ready else false.</returns>
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
        ///     Is ready to close the control.
        /// </summary>
        /// <returns>True else false.</returns>
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
        ///     Save any changes made to the control. Also prompts the user to print product labels.
        /// </summary>
        /// <returns>True if saved else false.</returns>
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
        ///     Cancel any changes made to the control.
        /// </summary>
        /// <returns>True if changes were cancelled else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (await this.IsReadyToClose())
                {
                    this.ActDetailEdit.SchemeData = this.ActDetail.LabelSchemeData;
                    this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
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
        ///     Get the XML representation of the activity detail edit.
        /// </summary>
        /// <returns>XML representation.</returns>
        public async Task<XDocument> GetXml()
        {
            return await SysShared.GetXmlFromActivityDetailEdit(this.ActDetailEdit);
        }

        /// <summary>
        ///     Get the control description.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Product Label Edit";
        }

        /// <summary>
        ///     Weight quantity text box got focus event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        public void TxtWtQty_GotFocus(object sender, EventArgs e)
        {
            if (this.txtWtQty.Text != null)
            {
                this.txtWtQty.CursorPosition = 0;
                this.txtWtQty.SelectionLength = this.txtWtQty.Text.Length;
            }
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                this.SubscribeKeyUpHandlers();
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    this.LoadAndBind();
                    await this.ScannerEnable();
                    var prodInfo = this.ActDetail.ActItem.ProdInfo;
                    this.cmdSelectProduct.Text = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                    this.lblWtUnit.Text = prodInfo.WeightUnitCode;
                    this.dtBestBeforeDate.IsEnabled = prodInfo.Scheme.LabelInfo.IsBestBeforeDateTracked;
                    this.dtProductionDate.IsEnabled = prodInfo.Scheme.LabelInfo.IsProductionDateTracked;
                    this.txtLotNo.IsEnabled = prodInfo.Scheme.LabelInfo.IsLotTracked;
                    this.txtSerialNo.IsEnabled = prodInfo.Scheme.LabelInfo.IsSerialTracked;
                    this.txtVariantCode.IsEnabled = prodInfo.Scheme.LabelInfo.IsVariantTracked;
                    this.txtWtQty.IsEnabled = prodInfo.IsCatchWeight;
                    ShowProductInfo();
                }

                _ = this.txtAltQuantity.Focus();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert("Error", "this.ucActivityProductLabel_GotFocus threw an error " + ex,
                        "OK"));
            }

            return;

            void ShowProductInfo()
            {
                this.ExtraInfoItems.Clear();
                this.ExtraInfoItems.Add(
                    "PROD: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBc + "  " +
                    this.ActDetail.ActItem.ProdInfo.Desc);
                this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            SysCommon.ShutdownConnection();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool" />.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            async void BackButtonCallback()
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
            }
        }

        /// <summary>
        ///     Subscribe the key up handlers.
        /// </summary>
        private void SubscribeKeyUpHandlers()
        {
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
        }

        /// <summary>
        ///     The TxtQuantity_GotFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
        private void TxtQuantity_GotFocus(object sender, EventArgs e)
        {
            if (this.txtAltQuantity.Text != null)
            {
                this.txtAltQuantity.CursorPosition = 0;
                this.txtAltQuantity.SelectionLength = this.txtAltQuantity.Text.Length;
            }
        }

        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            // If no asynchronous operations are needed, return a completed task.
            await Task.CompletedTask;
        }

        /// <summary>
        ///     The LoadData.
        /// </summary>
        private void LoadAndBind()
        {
            var prodInfo = this.ActDetail.ActItem.ProdInfo;
            var scheme = prodInfo.Scheme;

            if (scheme.LabelInfo.IsBestBeforeDateTracked && !SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                this.dtBestBeforeDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate;
            }

            if (scheme.LabelInfo.IsProductionDateTracked && !SysCommon.IsEmptyDate(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
            {
                this.dtProductionDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitProductionDate;
            }

            if (scheme.LabelInfo.IsSerialTracked)
            {
                if (string.IsNullOrEmpty(this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber))
                {
                    var schemeData = this.ActDetailEdit.SchemeData;
                    schemeData.SingleTradeItemSerialNumber =
                        ProductInfoTasks.GetProdSchemeSingleTradeItemSerialNumber();
                    this.ActDetailEdit.SchemeData = schemeData;
                }

                this.txtSerialNo.Text = this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber;
            }

            if (scheme.LabelInfo.IsLotTracked)
            {
                this.txtLotNo.Text = this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber;
            }

            if (scheme.LabelInfo.IsVariantTracked)
            {
                this.txtVariantCode.Text = this.ActDetailEdit.SchemeData.VariantCode;
            }

            // Default quantities if zero
            if (this.ActDetail.Quantity == 0)
            {
                if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.WeightQuantity != 0)
                    {
                        this.ActDetailEdit.Quantity = this.ActDetail.WeightQuantity;
                    }
                    else
                    {
                        this.ActDetailEdit.Quantity = prodInfo.UnitGrossWeight;
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

            this.ActDetailEdit.Quantity = decimal.Round(this.ActDetailEdit.Quantity, 3);
            this.ActDetailEdit.BaseQuantity = decimal.Round(this.ActDetailEdit.BaseQuantity, 3);
            this.ActDetailEdit.WeightQuantity = decimal.Round(this.ActDetailEdit.WeightQuantity, 3);

            this.txtAltQuantity.Text = this.ActDetailEdit.Quantity.ToString();
            this.txtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
        }

        /// <summary>
        ///     The IsValidData.
        /// </summary>
        /// <returns>.</returns>
        private async Task<bool> IsValidData()
        {
            var isRetVal = true;
            var schemeData = this.ActDetailEdit.SchemeData;
            schemeData.SingleTradeItemSerialNumber = this.txtSerialNo.Text;
            schemeData.LogisticUnitBatchLotNumber = this.txtLotNo.Text;
            schemeData.LogisticUnitProductionDate = this.dtProductionDate.Date;
            schemeData.LogisticUnitBestBeforeSellByDate = this.dtBestBeforeDate.Date;

            var scheme = this.ActDetail.ActItem.ProdInfo.Scheme;
            if (scheme.LabelInfo.IsVariantTracked)
            {
                if (string.IsNullOrEmpty(this.txtVariantCode.Text))
                {
                    schemeData.VariantCode = string.Empty;
                }
                else
                {
                    schemeData.VariantCode = this.txtVariantCode.Text.Trim();
                }

                schemeData = await this.ActDetail.FillSchemeDataVarInfoFromSchemeDataVarCode(schemeData);
            }

            this.ActDetailEdit.SchemeData = schemeData;

            this.ActDetailEdit.Quantity = decimal.Parse(this.txtAltQuantity.Text);

            if (this.ActDetailEdit.Quantity == 0)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(this.GetControlDescription(), "Invalid quantity.", "OK"));
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
                if (this.ActDetailEdit.Quantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity *
                                                      this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity;
                }
            }

            if (isRetVal && scheme.LabelInfo.IsSerialTracked &&
                    this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber?.Length == 0)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(this.GetControlDescription(), "Serial number required.", "OK"));
                isRetVal = false;
            }

            if (isRetVal && scheme.LabelInfo.IsLotTracked &&
                    this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber == string.Empty)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(this.GetControlDescription(), "Lot number required.", "OK"));
                isRetVal = false;
            }

            if (isRetVal && scheme.LabelInfo.IsVariantTracked &&
                    this.ActDetailEdit.SchemeData.VariantCode == string.Empty)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(this.GetControlDescription(), "Variant code required.", "OK"));
                isRetVal = false;
            }

            if (scheme.LabelInfo.IsBestBeforeDateTracked &&
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

            if (scheme.LabelInfo.IsProductionDateTracked &&
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
                this.ActDetail.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                    this.ActDetail.ActItem.ProdInfo.Code,
                    this.ActDetail.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);

                this.ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                    this.ActDetail.ActItem.ProdInfo.Code,
                    this.ActDetail.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<itm_refno>" + this.ActDetail.ActItem.ItemFkRefno + "</itm_refno>"
                    + "<alt_qty>" + this.ActDetailEdit.Quantity + "</alt_qty>"
                    + "<wt_qty>" + this.ActDetailEdit.Quantity + "</wt_qty>"
                    + "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeId +
                    "</ActivityDetailUniqueSchemeID>"
                    + "</imclient>");

                await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
            }

            return isRetVal;
        }

        /// <summary>
        ///     The CmdSelectProduct_Click click event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        private async void CmdSelectProduct_Click(object sender, EventArgs e)
        {
            if (this.IsFirstAppearance)
            {
                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl causes a click event to fire if it is the next control
                var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXml();
                    var prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);

                    if (prodUnitRefno != 0)
                    {
                        var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        this.ActDetail.ActItem.ProdInfo = prodInfo;

                        this.cmdSelectProduct.Text = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                    }
                }
            }
        }

        /// <summary>
        ///     The TxtQuantity_TextChanged text change event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        private void TxtQuantity_TextChanged(object sender, EventArgs e)
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
        ///     The CmdSave_Clicked click event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        private async void CmdSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }
    }
}