// <copyright file="ActivityPurchaseOrderWorkSheet.xaml.cs" company="InterSect Business Systems Inc">
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
    using dMobile.BarCodeScanner;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Models;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityPurchaseOrderWorkSheet : DMobileContentPage, IAppArea
    {
        private int prodUnitRefno = 0;
        private bool isFirstAppearance = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityPurchaseOrderWorkSheet"/> class.
        /// </summary>
        public ActivityPurchaseOrderWorkSheet()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
/// <summary>
/// Initializes a new instance of the <see cref="ActivityPurchaseOrderWorkSheet"/> class.
/// </summary>
/// <param name="actSubType"></param>
        public ActivityPurchaseOrderWorkSheet(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.cmdAdd.Text = "Add";

            this.ExtraInfoItems = new ObservableCollection<string>();
            this.MainActivityItems = new ObservableCollection<object>();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();

            this.ActSubType = actSubType;
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);
            this.ActItem = ActItemFacade.GetActItem(this.Activity, this.ActSubType);
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);

            this.Title = this.GetControlDescription();

            this.BarCodeModifier1Picker.Items.Clear();

            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.BarCodeModifier1Picker.Items.Add(string.Empty);
                var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                foreach (var item in modifiers)
                {
                    this.BarCodeModifier1Picker.Items.Add(item);
                }
            }
            else
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
            }

            this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the saved bar code scanner enabled status.
        /// </summary>
        private bool IsSvdBarScannerEnabled { get; set; } = false;

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
                    if (keycode == Keycode.DpadDown)
                    {
                        if (this.ListViewMainActivity.SelectedItem != null && Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < this.MainActivityItems.Count - 1)
                        {
                            this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) + 1];
                            this.ListViewMainActivity.ScrollTo(this.ListViewMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                    else if (keycode == Keycode.DpadUp)
                    {
                        if (this.ListViewMainActivity.SelectedItem != null && Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) > 0)
                        {
                            this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) - 1];
                            this.ListViewMainActivity.ScrollTo(this.ListViewMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                }
            });

            try
            {
                this.Loading = true;

                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                }

                await this.ucActivityPurchaseOrderWorkSheet_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityHdr_GotFocus threw an error " + ex, "OK"));
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ucActivityHdr_LostFocus(this, new EventArgs());

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    await this.Close();
                }
            });

            return true;
        }

        private BarCodeParse BarParse { get; set; }

        private BarCodeScanner Scanner { get; set; }

        private string ItemSortByIdx { get; set; } = "ActivityItemFKRefno";

        private ActDetail ActDetail { get; set; }

        private LocationMap LocMap { get; set; }

        private SysEnvironment SysEnv { get; set; }

        private bool IsBarCodeRegistered { get; set; } = false;

        private Activity Activity { get; set; }

        private ActItem ActItem { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.PurchaseOrderWorkSheet;

        private int TotalTicks { get; set; } = 0;

        public ObservableCollection<string> ExtraInfoItems { get; set; }
        public ObservableCollection<object> MainActivityItems { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            this.Loading = true;

            await Task.Run(async () =>
            {
                var message = string.Empty;
                // int prodUnitRefno = 0;
                var actItemFoundRefno = 0;
                var currentRowIdx = Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem));

                this.BarParse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                try
                {
                    if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                    {
                        // MessageBox.Show("Scanning an Activity Item Reference # Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Scanning an Activity Item Reference # Is Not Supported Here", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                    {
                        // MessageBox.Show("Scanning an Activity Item Reference # Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Scanning an Activity Item Reference # Is Not Supported Here", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ActDetailFKRefno))
                    {
                        // MessageBox.Show("Scanning an Activity Detail Reference # Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Scanning an Activity Detail Reference # Is Not Supported Here", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                    {
                        // MessageBox.Show("Scanning a shipping container bar code is not supported here!", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Scanning a shipping container bar code is not supported here!", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                    {
                        // MessageBox.Show("Scanning a Location Identifier Bar Code Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Scanning a Location Identifier Bar Code Is Not Supported Here", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                    {
                        XDocument xmlDocProduct = null;

                        if (await this.IdentifyProductScanned(this.BarParse, xmlDocProduct))
                        {
                            var oProductInfo = new ProductInfo();
                            oProductInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.prodUnitRefno);
                            actItemFoundRefno = await this.ActItem.GetActivityItemRefno(this.prodUnitRefno);
                            // <issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(oProductInfo, this.BarParse);
                        }

                        if (actItemFoundRefno == 0 && this.prodUnitRefno != 0)
                        {
                            this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.prodUnitRefno);
                            this.ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                            // <issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(this.ActItem.ProdInfo, this.BarParse);

                            if (this.ActItem.IsAllowAddItem)
                            {
                                this.Activity.HdrFKRefno = 0;
                                this.ActItem.ItemFKRefno = 0;

                                var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" + "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" + "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>" + "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" + "<ProductUnitBarCodeCode>" + this.BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>" + "<ProductUnitBarCodeSymbologyPrefix>" + this.BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" + "<alt_qty>" + 0 + "</alt_qty>" + "<base_qty>" + 0 + "</base_qty>" + "<wt_qty>" + 0 + "</wt_qty>" + "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" + "<refno>" + this.Activity.HdrFKRefno + "</refno>" + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>" + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" + "</params>");
                                await this.ActItem.Insert(request, false);
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                message = string.Format("Item Not Found For " + Environment.NewLine + "Location {0} " + Environment.NewLine + "Product {1} BC {2} Unit {3}", LocationMapTasks.GetCurrentReadableLocation(), this.ActItem.ProdInfo.Code, this.ActItem.ProdInfo.UnitBC, this.ActItem.ProdInfo.UnitCode);
                                throw new Exception(message);
                            }
                        }

                        if (actItemFoundRefno == 0)
                        {
                            throw new Exception("Item Not Found.");
                        }
                        else
                        {
                            // TODO: Fix sorting
                            // string strOldSort = ActItem.DataViewItems.Sort;
                            // ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            // foundRowIdx = ActItem.DataViewItems.Find(actItemFoundRefno);
                            var purchaseOrderWorkSheetItem1 = this.ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemRefno == actItemFoundRefno).FirstOrDefault();

                            if (purchaseOrderWorkSheetItem1 == null)
                            {
                                throw new Exception("Unable To Indentify Item");
                            }

                            await this.ActItem.AssignActivityItem(purchaseOrderWorkSheetItem1);

                            // TODO: Fix sorting
                            // ActItem.DataViewItems.Sort = strOldSort;
                        }

                        var dialogResult = DialogResult.None;
                        XDocument xmlDocEdit = null;

                        if (actItemFoundRefno > 0)
                        {
                            // Ask for new quantity
                            var form = new FormDialogContainer(EDialogTypes.ActivityPurchaseOrderWorkSheetEdit, this.ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = await form.GetXML();

                                // TODO: Fix sorting
                                // string strOldSort = ActItem.DataViewItems.Sort;
                                // ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                var purchaseOrderWorkSheetItem2 = this.ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemRefno == actItemFoundRefno).FirstOrDefault();

                                if (purchaseOrderWorkSheetItem2 == null)
                                {
                                    throw new Exception("Unable To Indentify Item");
                                }

                                var decQty = Math.Round(decimal.Parse(xmlDocEdit.Root.Element("alt_qty").Value) + Convert.ToDecimal(purchaseOrderWorkSheetItem2.ActivityItemPurchaseOrderWorkSheetQuantityOrdered), 3);
                                var decBaseQty = Math.Round(decimal.Parse(xmlDocEdit.Root.Element("base_qty").Value) + Convert.ToDecimal(purchaseOrderWorkSheetItem2.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered), 3);
                                var decWeightQty = Math.Round(decimal.Parse(xmlDocEdit.Root.Element("wt_qty").Value) + Convert.ToDecimal(purchaseOrderWorkSheetItem2.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered), 3);

                                // TODO: Fix sorting
                                // ActItem.DataViewItems.Sort = strOldSort;
                                var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" + "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" + "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>" + "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" + "<ProductUnitBarCodeCode>" + this.BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>" + "<ProductUnitBarCodeSymbologyPrefix>" + this.BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" + "<alt_qty>" + decQty + "</alt_qty>" + "<base_qty>" + decBaseQty + "</base_qty>" + "<wt_qty>" + decWeightQty + "</wt_qty>" + "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" + "<refno>" + this.ActItem.ItemFKRefno + "</refno>" + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>" + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" + "</params>");
                                await this.ActItem.Update(request);
                            }
                        }
                        else
                        {
                            this.ActItem.ItemFKRefno = 0;

                            // Reset values
                            this.ActDetail.Quantity = 0;
                            this.ActDetail.BaseQuantity = 0;
                            this.ActDetail.WeightQuantity = 0;
                            this.ActDetail.SchemeID = string.Empty;
                            this.ActDetail.UniqueSchemeID = string.Empty;

                            var form = new FormDialogContainer(EDialogTypes.ActivityPurchaseOrderWorkSheetEdit, this.ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = await form.GetXML();
                                var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" + "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" + "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>" + "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" + "<ProductUnitBarCodeCode>" + this.BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>" + "<ProductUnitBarCodeSymbologyPrefix>" + this.BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" + "<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>" + "<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>" + "<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>" + "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" + "<refno>" + this.ActItem.ItemFKRefno + "</refno>" + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>" + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" + "</params>");
                                await this.ActItem.Insert(request, false);
                            }
                        }

                        // TODO: Fix sorting
                        // ActItem.DataViewItems.Sort = "ActivityItemFKRefno";
                        var purchaseOrderWorkSheetItem = this.ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemFKRefno == this.ActItem.ItemFKRefno).FirstOrDefault();

                        if (purchaseOrderWorkSheetItem == null)
                        {
                            throw new Exception("Unable To Indentify Item");
                        }

                        _ = await Device.InvokeOnMainThreadAsync(() => this.ListViewMainActivity.SelectedItem = purchaseOrderWorkSheetItem);
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Scanned Bar Code Type Is Not Supported Here", "OK"));
                    }
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Activity Item", ex.Message, "OK"));
                }
                finally
                {
                    await this.ScannerEnable();
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="oBarCodeParse"></param>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<bool> IdentifyProductScanned(BarCodeParse oBarCodeParse, XDocument xmlDocProduct)
        {
            var IsProductIdentified = false;
            var strMessage = string.Empty;
            var iProductUnitBarCodeRefno = 0;
            var oActivityBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
            var actItemBarCode = ActItemFacade.GetActItem(oActivityBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            var actDetailBarCode = ActDetailFacade.GetActDetail(actItemBarCode, SysCommon.ActSubTypes.BarCodeAssociation);

            // by reference params initialize
            this.prodUnitRefno = 0;

            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductUnitBarCodeModel>("SELECT * FROM tProductUnitBarCode WHERE ProductUnitBarCodeCode = ? AND ProductUnitBarCodeModifier1 = ?", oBarCodeParse.ProductIdentifier, this.SysEnv.CurrentBarCodeModifier1);

            if (results?.Count > 0)
            {
                iProductUnitBarCodeRefno = results[0].ProductUnitBarCodeRefno;
                this.prodUnitRefno = results[0].ProductUnitRefno;
            }

            // IF THE SCANNED BARCODE WAS NOT FOUND IN THE DATABASE
            if (iProductUnitBarCodeRefno == 0)
            {
                // IF YES TO JOIN BARCODE TO A PRODUCT
                strMessage = "Scanned Product Bar Code Is Not Associated To a Product." + Environment.NewLine + "Do You Wish To Associate It Now? ";

                // if (MessageBox.Show(strMessage, "Activity Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Activity Item", strMessage, "Yes", "No")))
                {
                    // CALL AND SELECTED A PRODUCT TO JOIN THE BAR CODE TOO
                    var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);

                    if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                    {
                        xmlDocProduct = await form.GetXML();
                        this.prodUnitRefno = int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value);

                        // IF A PRODUCT WAS SELECTED, CREATE THE XML FOR THE BAR CODE ACTIVITY INSERT
                        if (this.prodUnitRefno != 0)
                        {
                            var xmlDoc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<ProductUnitRefno>" + this.prodUnitRefno.ToString() + "</ProductUnitRefno>" + "<ProductUnitBarCodeRefno>" + iProductUnitBarCodeRefno.ToString() + "</ProductUnitBarCodeRefno>" + "<ProductIdentifier>" + oBarCodeParse.ProductIdentifier + "</ProductIdentifier>" + "<RawData>" + oBarCodeParse.Raw + "</RawData>" + "<enumBarCodeSymbology>" + Convert.ToInt32(oBarCodeParse.Symbology) + "</enumBarCodeSymbology>" + "<BarCodeSymbologyPrefix>" + oBarCodeParse.SymbologyPrefix + "</BarCodeSymbologyPrefix>" + "<ProductUnitBarCodeModifier1>" + this.SysEnv.CurrentBarCodeModifier1 + "</ProductUnitBarCodeModifier1>" + "</params>");

                            // INSERT A NEW 'BAR CODE ASSOCIATION' ACTIVITY
                            await actItemBarCode.Insert(xmlDoc, false);

                            IsProductIdentified = true;
                        }
                    }
                }
            }
            else
            {
                IsProductIdentified = true;
            }

            return IsProductIdentified;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            try
            {
                this.Loading = true;
                await this.Activity.Refresh(null);
                await this.ActItem.Refresh(null);

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ExtraInfoItems.Clear();

                    this.ActItem.ListItems = await this.ActItem.GetActivityItemList(false);
                    this.MainActivityItems = this.ActItem.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                    if (this.ActItem.ListItems.Count > 0 && this.MainActivityItems.Count > 0)
                    {
                        await this.ActItem.AssignActivityItem(this.MainActivityItems[0]);
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                    }
                });
            }
            catch (SQLiteException ex)
            {
                for (var curExNdx = 0; curExNdx <= ex.Data.Count; curExNdx++)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error ", ex.Data[curExNdx].ToString() + '\r', "OK"));
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message, "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Refresh", ex.Message, "OK"));
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            var IsReadyToSave = true;

            return IsReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return this.Activity.GetControlDescription();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    // this.ProdUnitRefno = 0;

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
                    // this.LoadData();
                    cancelChanges = false;
                    // this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.PurchaseOrderWorkSheet, ex);
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
            var xmldocParams = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "</params>");
            return xmldocParams;
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
        public async Task ucActivityPurchaseOrderWorkSheet_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();
            _ = await Device.InvokeOnMainThreadAsync(() => this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);
            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
            await this.RefreshData();
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
        public async Task ucActivityHdr_LostFocus(object sender, EventArgs e)
        {
            // if (tmrActivityLoaded.Enabled == false)
            // {
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }
            // }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAdd_Click(object sender, EventArgs e)
        {
            XDocument xmlDocProduct = null;
            XDocument xmlDocEdit = null;
            var iActivityItemFoundRefno = 0;
            var dialogResult = DialogResult.None;
            var strProductUnitBarCodeCode = string.Empty;
            var strBarCodeSymbologyPrefix = string.Empty;

            await this.ScannerDisable();

            try
            {
                // activity item object needs to check if a new location and product can be added, if not add a detail
                if (this.LocMap.CurrLocMap.WhsFKRefno == 0 && !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // MessageBox.Show("You Have To Set Location!");
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Set Location", "Location Must Be Set!", "OK"));
                }
                else
                {
                    var formSelectProd = new FormDialogContainer(EDialogTypes.SelectProductUnit, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);

                    _ = await Device.InvokeOnMainThreadAsync(async () => dialogResult = await formSelectProd.DisplayControl.ShowDialog());

                    if (dialogResult == DialogResult.OK)
                    {
                        xmlDocProduct = await formSelectProd.GetXML();
                        this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value));
                        this.ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);
                        iActivityItemFoundRefno = await this.ActItem.GetActivityItemRefno(int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value));

                        if (iActivityItemFoundRefno > 0)
                        {
                            // Ask for new quantity
                            var formPurchaseOrderWorkSheetEdit = new FormDialogContainer(EDialogTypes.ActivityPurchaseOrderWorkSheetEdit, this.ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await formPurchaseOrderWorkSheetEdit.DisplayControl.ShowDialog();

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = await formPurchaseOrderWorkSheetEdit.GetXML();
                                var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<ProductUnitRefno>" + xmlDocProduct.Root.Element("ProductUnitRefno").Value + "</ProductUnitRefno>" + "<ProductCode>" + xmlDocProduct.Root.Element("ProductCode").Value + "</ProductCode>" + "<ProductUnitIsBC>" + xmlDocProduct.Root.Element("ProductUnitIsBC").Value + "</ProductUnitIsBC>" + "<UnitCode>" + xmlDocProduct.Root.Element("UnitCode").Value + "</UnitCode>" + "<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>" + "<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" + "<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>" + "<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>" + "<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>" + "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" + "<refno>" + this.ActItem.ItemFKRefno + "</refno>" + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>" + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" + "</params>");
                                await this.ActItem.Update(request);
                            }
                        }
                        else
                        {
                            this.ActItem.ItemFKRefno = 0;

                            // Reset values
                            this.ActDetail.Quantity = 0;
                            this.ActDetail.BaseQuantity = 0;
                            this.ActDetail.WeightQuantity = 0;
                            this.ActDetail.SchemeID = string.Empty;
                            this.ActDetail.UniqueSchemeID = string.Empty;

                            var form = new FormDialogContainer(EDialogTypes.ActivityPurchaseOrderWorkSheetEdit, this.ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await form.DisplayControl.ShowDialog();

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = await form.GetXML();
                                var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" + "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" + "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>" + "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" + "<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>" + "<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" + "<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>" + "<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>" + "<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>" + "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" + "<refno>" + this.ActItem.ItemFKRefno + "</refno>" + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>" + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" + "</params>");

                                await this.ActItem.Insert(request, false);

                                if (this.ActItem.ListItems.Count > 0)
                                {
                                    // TODO: Fix sorting
                                    // string strOldSort = ActItem.DataViewItems.Sort;
                                    // ActItem.DataViewItems.Sort = "ActivityItemFKRefno";
                                    var purchaseOrderWorkSheetItem = this.ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemFKRefno == this.ActItem.ItemFKRefno).FirstOrDefault();

                                    if (purchaseOrderWorkSheetItem == null)
                                    {
                                        throw new Exception("Unable to Identify Activity Items");
                                    }

                                    this.ListViewMainActivity.SelectedItem = purchaseOrderWorkSheetItem;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message, "Activity Work Sheet", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Activity Work Sheet", ex.Message, "OK"));
            }

            await this.ScannerEnable();
        }

        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await this.ActItem.AssignActivityItem(e.SelectedItem);
            }
        }
    }
}