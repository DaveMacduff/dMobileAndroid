// <copyright file="PickSingleActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Models;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="PickSingleActItem" />.
    /// </summary>
    public class PickSingleActItem : ActItem
    {
        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double columnWidth0 = 0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double columnWidth1 = 0;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double columnWidth2 = 0;

        /// <summary>
        /// Defines the columnWidth3.
        /// </summary>
        private double columnWidth3 = 0;

        /// <summary>
        /// Defines the columnWidth4.
        /// </summary>
        private double columnWidth4 = 0;

        /// <summary>
        /// Defines the columnWidth5.
        /// </summary>
        private double columnWidth5 = 0;

        /// <summary>
        /// Defines the columnWidth6.
        /// </summary>
        private double columnWidth6 = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listPickingItems.
        /// </summary>
        private List<PickingItem> listPickingItems = new List<PickingItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PickSingleActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public PickSingleActItem(Activity.Activity activity)
          : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Pick;
            this.FullClassName = typeof(PickSingleActItem).FullName;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPickCreateNewItemNoAsk;

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth6, GridUnitType.Absolute) },
                    },
                };

                var labelActivityItemPickingIsActivityItemPickingComment = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemPickingProductCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemPickingProductUnitBc = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemPickingUnitCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemPickingQuantityOrdered = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemPickingQuantityReceived = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemPickingQuantityAvailable = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelActivityItemPickingIsActivityItemPickingComment.SetBinding(Label.TextProperty, new Binding("IsActivityItemPickingComment"));
                labelActivityItemPickingProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelActivityItemPickingProductUnitBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelActivityItemPickingUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelActivityItemPickingQuantityOrdered.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityOrdered", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemPickingQuantityReceived.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityShipped", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemPickingQuantityAvailable.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityAvailable", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelActivityItemPickingIsActivityItemPickingComment);
                gridData.Children.Add(labelActivityItemPickingProductCode);
                gridData.Children.Add(labelActivityItemPickingProductUnitBc);
                gridData.Children.Add(labelActivityItemPickingUnitCode);
                gridData.Children.Add(labelActivityItemPickingQuantityOrdered);
                gridData.Children.Add(labelActivityItemPickingQuantityReceived);

                if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                {
                    gridData.Children.Add(labelActivityItemPickingQuantityAvailable);
                }

                Grid.SetColumn(labelActivityItemPickingIsActivityItemPickingComment, 0);
                Grid.SetColumn(labelActivityItemPickingProductCode, 1);
                Grid.SetColumn(labelActivityItemPickingProductUnitBc, 2);
                Grid.SetColumn(labelActivityItemPickingUnitCode, 3);
                Grid.SetColumn(labelActivityItemPickingQuantityOrdered, 4);
                Grid.SetColumn(labelActivityItemPickingQuantityReceived, 5);

                if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                {
                    Grid.SetColumn(labelActivityItemPickingQuantityAvailable, 6);
                }

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplatePickingSingleItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplatePickingSingleItem", dataTemplate);
            }
        }

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItemFromList(int actItemFoundRefno)
        {
            this.SelectedListIdx = -1;
            if (actItemFoundRefno >= 0)
            {
                var idx = 0;
                foreach (var item in this.ListItems.Cast<PickingItem>())
                {
                    if (item.ActivityItemRefno == this.ItemRefno)
                    {
                        this.SelectedItem = item;
                        this.SelectedListIdx = idx;
                        break;
                    }

                    idx++;
                }
            }
            else
            {
                this.SelectedItem = null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allowed to add skid detail.
        /// </summary>
        /// <returns><see cref="bool"/> True if allowed to add skid detail.</returns>
        public override bool IsAllowAddSkidDetail()
        {
            return false;
        }

        /// <summary>
        /// The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public async override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            var labelQty = new SLabelQuantity();

            try
            {
                var sendReq = string.Empty;
                var responseData = string.Empty;
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);

                XDocument request = null;

                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SHIPPING LABEL ITEM QUANTITY</action>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<itm_refno>" + this.ItemFkRefno + "</itm_refno>"
                    + "</imclient>");

                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();

                sendReq = xmlString.ToString();

                responseData = SysCommon.SubmitComm(sendReq);
                var xmlResult = new XmlDocument();
                xmlResult.LoadXml(responseData);

                // Check if result is an error message
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var errorDetail = string.Format(
                        "Order# {0} request shipping label(s) quantity failed." + Environment.NewLine + "{1}",
                        this.ItemFkRefno,
                        xmlResult.DocumentElement["detail"].InnerText);
                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }
                else
                {
                    if (!isRepackQuantityOnly)
                    {
                        labelQty.FullQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/FullCaseQuantity").FirstChild.Value);

                        labelQty.DryQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/DryQuantity").FirstChild.Value);

                        labelQty.FrozenQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/FrozenQuantity").FirstChild.Value);

                        labelQty.LiveQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/LiveQuantity").FirstChild.Value);

                        labelQty.FreshQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/FreshQuantity").FirstChild.Value);

                        labelQty.BundleQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/BundleQuantity").FirstChild.Value);

                        labelQty.PalletQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/PalletQuantity").FirstChild.Value);
                    }

                    labelQty.RepackQuantity = int.Parse(
                        xmlResult.SelectSingleNode("/VFPData/RepackQuantity").FirstChild.Value);

                    labelQty.RepackDryQuantity = int.Parse(
                        xmlResult.SelectSingleNode("/VFPData/RepackDryQuantity").FirstChild.Value);

                    labelQty.RepackFrozenQuantity = int.Parse(
                        xmlResult.SelectSingleNode("/VFPData/RepackFrozenQuantity").FirstChild.Value);

                    labelQty.RepackLiveQuantity = int.Parse(
                        xmlResult.SelectSingleNode("/VFPData/RepackLiveQuantity").FirstChild.Value);

                    labelQty.RepackFreshQuantity = int.Parse(
                        xmlResult.SelectSingleNode("/VFPData/RepackFreshQuantity").FirstChild.Value);

                    // Used to determine if there should be a repack quantity gathered.
                    labelQty.IsRepack = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackCase").FirstChild.Value);
                    labelQty.IsRepackLive = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackLive").FirstChild.Value);
                    labelQty.IsRepackFrozen = bool.Parse(
                          xmlResult.SelectSingleNode("/VFPData/IsRepackFrozen").FirstChild.Value);
                    labelQty.IsRepackFresh = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackFresh").FirstChild.Value);
                    labelQty.IsRepackDry = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackDry").FirstChild.Value);
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert(this.ActHdr.GetControlDescription(), string.Format("Get full unit label quantity failed." + Environment.NewLine + " {0}", ex.Message), "OK"));
            }

            return labelQty;
        }

        /// <summary>
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAsk()
        {
            var isCatchAsk = this.ProdInfo.IsCatchAsk;
            if (isCatchAsk)
            {
                // modules are for exclusions
                ////isCatchAsk = this.SysEnv.SysSettings.IsPickSingleCatchAsk;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// Check if the product is catch count.
        /// </summary>
        /// <returns><see cref="bool"/> True if catch count, else false.</returns>
        public override bool IsCatchAskZero()
        {
            var result = false;
            if (this.IsCatchAsk() && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsPickingSingleModuleNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        /// Find the list item index of the item that matches the activity item foreign key reference number.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item foreign key reference number.</param>
        /// <returns><see cref="int"/> Activity list item found index, else -1.</returns>
        public override int FindActivityItemIndex(int activityItemFkRefno)
        {
            var foundIdx = -1;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in this.ListItems)
            {
                foundIdx++;
                if (((PickingActItem)item).ItemFkRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        /// <summary>
        /// The IsReadyToApply.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task IsReadyToApply(XDocument data)
        {
            await base.IsReadyToApply(data);

            // Weight tolerance within percentage check.
            if (this.SysEnv.SysSettings.ProdWeightUnitTolerancePercent != 0)
            {
                var weight = decimal.Parse(data.Root.Element("wt_qty").Value);
                var qty = decimal.Parse(data.Root.Element("alt_qty").Value);
                var per = this.SysEnv.SysSettings.ProdWeightUnitTolerancePercent / 100;
                var weightUpper = (this.ProdInfo.UnitGrossWeight + (this.ProdInfo.UnitGrossWeight * per)) * qty;
                var weightLower = (this.ProdInfo.UnitGrossWeight - (this.ProdInfo.UnitGrossWeight * per)) * qty;

                if (weight < weightLower || weight > weightUpper)
                {
                    var msg = "Item is outside of expected weight.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Weight Range Check", msg, "OK"));
                }
            }

            // New items are identified with 0 < this.FkRefno, do not validate.
            if (this.SysEnv.SysSettings.PickQtyOverWarn != SysCommon.ActPickingQtyOverValLevels.NoValidationOrWarning && this.ItemFkRefno > 0)
            {
                try
                {
                    this.PickingModuleQuantityOverCheck(decimal.Parse(data.Root.Element("alt_qty").Value));
                }
                catch (Exception ex)
                {
                    switch (this.SysEnv.SysSettings.PickQtyOverWarn)
                    {
                        case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndContinue:

                            if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Quantity Over", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                            {
                                throw new IsReadyToApplyException(ex.Message, true, true);
                            }
                            else
                            {
                                throw new IsReadyToApplyException(ex.Message, true, false);
                            }
                        case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndStop:
                            throw new IsReadyToApplyException(ex.Message, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFkRefno">.</param>
        /// <returns>.</returns>
        public async override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var itemSchemeData = new ActItemSchemeData();
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES ORDER ITEM FROM DETAIL</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<ser_refno>" + actDetailFkRefno + "</ser_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            var responseData = SysCommon.SubmitComm(xmlString.ToString());
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseData);
            if (responseData.Contains("<message>ERROR</message>"))
            {
                var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                if (errorDetail.Contains("MSGBOX-"))
                {
                    throw new MessageOnlyException(errorDetail);
                }
                else
                {
                    throw new Exception(errorDetail);
                }
            }
            else
            {
                var tempSchemeData = new ProductSchemeData();
                itemSchemeData.ActItemFkRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/ActivityItemFKRefno").InnerText);
                itemSchemeData.ActDetailQuantity = decimal.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailquantity").InnerText);
                itemSchemeData.UnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/unitcode").InnerText;
                itemSchemeData.ActDetailBaseQuantity = decimal.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailbasequantity").InnerText);
                itemSchemeData.BaseUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/baseunitcode").InnerText;
                itemSchemeData.ActDetailWeightQuantity = decimal.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailweightquantity").InnerText);
                itemSchemeData.WeightUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/weightunitcode").InnerText;
                itemSchemeData.LocMapWhsFkRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/locationmapwarehousefkrefno").InnerText);
                itemSchemeData.LocMapSectFkRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/locationmapsectionfkrefno").InnerText);
                itemSchemeData.LocMapSlotFkRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/locationmapslotfkrefno").InnerText);

                tempSchemeData.SingleTradeItemSerialNumber = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailsingletradeitemserialnumber").InnerText;
                tempSchemeData.LogisticUnitBatchLotNumber = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbatchlotnumber").InnerText;
                tempSchemeData.LogisticUnitSkidNumber = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitskidcode").InnerText;
                tempSchemeData.VariantCode = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantcode").InnerText;
                tempSchemeData.VariantDescription = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantdescription").InnerText;
                tempSchemeData.VariantNumber = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantno").InnerText);

                if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText == string.Empty))
                {
                    tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText);
                }

                if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText
                    == string.Empty))
                {
                    tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText);
                }

                if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText
                    == string.Empty))
                {
                    tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText);
                }

                if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitduedate").InnerText == string.Empty))
                {
                    tempSchemeData.LogisticUnitDueDate = DateTime.Parse(xmlDoc.SelectSingleNode(
                        "/VFPData/seg/activitydetaillogisticunitduedate").InnerText);
                }

                if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText
                    == string.Empty))
                {
                    tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(xmlDoc.SelectSingleNode(
                        "/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText);
                }

                itemSchemeData.ProdSchemeData = tempSchemeData;
            }

            return itemSchemeData;
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            var itemCmnt = string.Empty;

            if (selectedActivityItem is PickingItem pickingItem)
            {
                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tActivityItemPickingCommentModel>("SELECT * FROM tActivityItemPickingComment WHERE ActivityItemRefno = ?", pickingItem.ActivityItemRefno);

                if (results != null && results.Count > 0)
                {
                    itemCmnt = Convert.ToString(results[0].ActivityItemPickingCommentComment);
                }
            }

            return itemCmnt;
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse sales order item # {actItemStringFkRefno}";
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find sales order item # {actItemStringFkRefno}";
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFkRefnoIdx()
        {
            return "ActivityItemStringFKRefno";
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>

        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

                var tableActPickItem = new TableActivityPickingItem();
                var tableActPickDetail = new TableActivityPickingDetail();
                await tableActPickDetail.PurgeData();
                await tableActPickItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableActPickItem.RegisterDataAccessListenerProgressOut(this);
                await tableActPickItem.RequestItemData(this.ActHdr.HdrFkRefno, data, this.ActHdr.HdrRefno);
                tableActPickItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            var labelIsActivityItemPickingCommentHeader = new Label
                {
                    Text = "C",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };
            var labelActivityItemPickingProductCodeHeader = new Label
                {
                    Text = "Product ID",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };
            var labelActivityItemPickingProductUnitBcHeader = new Label
                {
                    Text = string.Empty,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };
            var labelActivityItemPickingUnitCodeHeader = new Label
                {
                    Text = string.Empty,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };
            var labelActivityItemPickingQuantityOrderedHeader = new Label
                {
                    Text = "Ordered",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };
            var labelActivityItemPickingQuantityReceivedHeader = new Label
                {
                    Text = "Shipped",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };
            var labelActivityItemPickingQuantityAvailableHeader = new Label
                {
                    Text = "Available",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsActivityItemPickingCommentHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingProductCodeHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingProductUnitBcHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingUnitCodeHeader.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityOrderedHeader.Text);
            this.columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityReceivedHeader.Text);
            this.columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityAvailableHeader.Text);

            // Header grid
            var gridHeaderPicking = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth6, GridUnitType.Absolute) },
                },
            };

            gridHeaderPicking.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeaderPicking.Children.Add(labelIsActivityItemPickingCommentHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingProductCodeHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingProductUnitBcHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingUnitCodeHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityOrderedHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityReceivedHeader);

            if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
            {
                gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityAvailableHeader);
            }

            Grid.SetColumn(labelIsActivityItemPickingCommentHeader, 0);
            Grid.SetColumn(labelActivityItemPickingProductCodeHeader, 1);
            Grid.SetColumn(labelActivityItemPickingProductUnitBcHeader, 2);
            Grid.SetColumn(labelActivityItemPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelActivityItemPickingQuantityOrderedHeader, 4);
            Grid.SetColumn(labelActivityItemPickingQuantityReceivedHeader, 5);

            if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
            {
                Grid.SetColumn(labelActivityItemPickingQuantityAvailableHeader, 6);
            }

            Grid.SetRow(gridHeaderPicking, 0);

            gridMainActivity.Children.Add(gridHeaderPicking);

            foreach (var pickingItem in this.ListItems.Cast<PickingItem>())
            {
                // PickingItem pickingItem = new PickingItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                //    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                //    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                //    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                //    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                //    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                //    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                //    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                //    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                // };
                observableCollection.Add(pickingItem);

                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.IsActivityItemPickingComment);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.ProductCode);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.ProductUnitBC);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.UnitCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{pickingItem.ActivityItemPickingQuantityOrdered:F3}");
                var detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{pickingItem.ActivityItemPickingWeightQuantityShipped:F3}");
                var detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{pickingItem.ActivityItemPickingQuantityAvailable:F3}");

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeaderPicking.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeaderPicking.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeaderPicking.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeaderPicking.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeaderPicking.ColumnDefinitions[4].Width = this.columnWidth4;
                }

                if (detailWidth5 > this.columnWidth5)
                {
                    this.columnWidth5 = detailWidth5;
                    gridHeaderPicking.ColumnDefinitions[5].Width = this.columnWidth5;
                }

                if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                {
                    if (detailWidth6 > this.columnWidth6)
                    {
                        this.columnWidth6 = detailWidth6;
                        gridHeaderPicking.ColumnDefinitions[6].Width = this.columnWidth6;
                    }
                }
                else
                {
                    this.columnWidth6 = 0;
                    gridHeaderPicking.ColumnDefinitions[6].Width = this.columnWidth6;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplatePickingSingleItem"];

            return observableCollection;
        }

        public override Grid DesignGrid()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Create and fill the data template.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity grid.</param>
        public override void FillDataTemplate(Grid gridMainActivity)
        {
            throw new NotImplementedException();
        }


        public override DataTemplate GetDataTemplate()
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<object> GetActivityItemList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItemPicking.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.Append("ORDER BY tActivityItemPicking.ActivityItemRefno");
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);
            //        this.DataViewItems = ds.Tables[0].DefaultView;
            //        this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //        this.DataViewItems.AllowNew = true;
            //        this.DataViewItems.AllowEdit = true;
            //        this.DataViewItems.AllowDelete = true;
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //        TODO: Fix sorting
            //    }
            // }
            this.listPickingItems = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<PickingItem>(sql.ToString());
            this.ListItems = this.listPickingItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            // if (string.IsNullOrEmpty(this.ActItemFilter))
            // {
            this.ListItems = this.listPickingItems.Where(pickingItem => pickingItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            // }
            foreach (var pickingItem in this.ListItems.Cast<PickingItem>())
            {
                // PickingItem pickingItem = new PickingItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                //    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                //    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                //    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                //    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                //    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                //    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                //    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                //    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                // };
                observableCollection.Add(pickingItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// Select an activity item.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity object.</param>
        /// <returns><see cref="Task{T}"/> Activity item foreign key reference number.</returns>
        public override async Task<int> AssignActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is PickingItem item)
            {
                this.SelectedItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFkRefno = item.ActivityItemFKRefno;
                this.ActStatus = (SysCommon.ActStatus)item.ActivityItemStatus;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);

                await AssignItemLocation(item);

                // LocationMap.GetInstance().CurrLocMap warehouse needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }

            return this.ItemFkRefno;

            //////////////////////////////////////////////////////////////////////////
            async Task AssignItemLocation(PickingItem item)
            {
                // This code needs to be before the call to GetSelectedItemLocation, because the skid location may be
                // used if the product unit and warehouse match.
                if (this.SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid &&
                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ProdInfo.Refno)
                {
                    this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                }

                var itemLocMap = await this.GetSelectedItemLocation(item.LocationMapRefno);
                if (itemLocMap == null)
                {
                    // Current location used, we need one for popup forms.
                    this.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.ActHdr.LocationMapRefno);
                    if (this.LocMap.WhsLocMapRefno == 0)
                    {
                        var defLocMapRefno = LocationMapTasks.DefaultLocMapRefnoToWhsIfUnassigned(locationMapRefno: 0);
                        this.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    }
                }
                else
                {
                    this.LocMap = itemLocMap.Clone();

                    // Override the location with the product default if exists and complete.
                    if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                    {
                        // Only assign the current location if the map is complete else leave it alone.
                        if (await LocationMapTasks.IsCompleteLocMap(itemLocMap))
                        {
                            // Update the chosen location with the current when defaulting is turned on.
                            LocationMap.GetInstance().CurrLocMap = itemLocMap.Clone();
                            LocationMap.GetInstance().ChosenLocMap = itemLocMap.Clone();

                            // Assign the skid the location, because defaulting is enabled.
                            if (this.ActHdr.SkidInfo.CurrSkid.SkidFkRefno > 0)
                            {
                                this.ActHdr.SkidInfo.CurrSkid.LocMap = itemLocMap.Clone();
                            }

                            // Update the activity item location.
                            this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item refno else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            var foundActItemRefno = this.ItemRefno;
            return foundActItemRefno;
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PICKING ITEM</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBc + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<whs_id>" + this.LocMap.WhsFkRefno + "</whs_id>"
                + "<sect_refno>" + this.LocMap.SectFkRefno + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.SlotFkRefno + "</slot_refno>"
                + "<refno></refno>" + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The RefreshQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                pickingItem.ActivityItemPickingQuantityShipped = quantity;
                pickingItem.ActivityItemPickingBaseQuantityShipped = baseQuantity;
                pickingItem.ActivityItemPickingWeightQuantityShipped = weightQuantity;
            }
        }

        /// <summary>
        /// The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void AddToQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                pickingItem.ActivityItemPickingQuantityShipped += quantity;
                pickingItem.ActivityItemPickingBaseQuantityShipped += baseQuantity;
                pickingItem.ActivityItemPickingWeightQuantityShipped += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object selectedActivityItem)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                var query = "UPDATE tActivityItemPicking SET ActivityItemPickingQuantityShipped = ?," +
                            " ActivityItemPickingBaseQuantityShipped = ?," +
                            " ActivityItemPickingWeightQuantityShipped = ? WHERE tActivityItemPicking.ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    pickingItem.ActivityItemPickingQuantityShipped,
                    pickingItem.ActivityItemPickingBaseQuantityShipped,
                    pickingItem.ActivityItemPickingWeightQuantityShipped,
                    pickingItem.ActivityItemRefno,
                };

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// The PickingModuleQuantityOverCheck.
        /// </summary>
        /// <param name="actItemPickQuantity">.</param>
        private void PickingModuleQuantityOverCheck(decimal actItemPickQuantity)
        {
            if (this.SelectedItem is PickingItem pickingItem)
            {
                var shipped = Convert.ToDecimal(pickingItem.ActivityItemPickingQuantityShipped);
                var ordered = Convert.ToDecimal(pickingItem.ActivityItemPickingQuantityOrdered);

                if (shipped + actItemPickQuantity > ordered)
                {
                    throw new Exception(
                        $"The resulting quantity shipped {shipped + actItemPickQuantity} {this.ProdInfo.UnitCode} will be greater than the quantity ordered {ordered} {this.ProdInfo.UnitCode}.");
                }
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkItRefno = ((PickingItem)resourceType).ActivityItemFKRefno;
            return tmpFkItRefno == null ? 0 : Convert.ToInt32(tmpFkItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"Pick # {this.ActHdr.HdrFkRefno}";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Submit(XDocument data, object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> CreateDeleteXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Update.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Update(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetTotalQuantity.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetTotalQuantity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is PickingItem item)
            {
                if (Math.Sign(item.ActivityItemPickingQuantityShipped) == -1)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Receiving);
                }
                else
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Picking);
                }
            }

            if (locMapRefno == 0)
            {
                locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    whsLocMapRefno,
                    prodRefno,
                    SysCommon.ProdLocMapTypes.Floor);
            }

            if (!await LocationMapTasks.IsCompleteLocMap(locMapRefno))
            {
                locMapRefno = 0;
            }

            return locMapRefno;
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
