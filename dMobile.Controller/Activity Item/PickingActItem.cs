// <copyright file="PickingActItem.cs" company="InterSect Business Systems Inc">
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
using dMobile.Resources.Models.Items;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Details;
using dMobile.Resources.Types.Items;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="PickingActItem" />.
    /// </summary>
    public class PickingActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public PickingActItem(Activity.Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(PickingActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Pick;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPickCreateNewItemNoAsk;

            DefineInitialGrid();
            return;

            ///////////////////////////////////////////////////////////
            void DefineInitialGrid()
            {
                var dataTemplate = new DataTemplate(
                    () =>
                    {
                        var gridData = new Grid
                        {
                            Padding = new Thickness(5),
                            ColumnDefinitions =
                            {
                                new ColumnDefinition
                                {
                                    Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute)
                                },
                                new ColumnDefinition
                                {
                                    Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute)
                                },
                                new ColumnDefinition
                                {
                                    Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute)
                                },
                                new ColumnDefinition
                                {
                                    Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute)
                                },
                                new ColumnDefinition
                                {
                                    Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute)
                                },
                                new ColumnDefinition
                                {
                                    Width = new GridLength(this.ColumnWidth5, GridUnitType.Absolute)
                                },
                                new ColumnDefinition
                                    { Width = new GridLength(this.ColumnWidth6, GridUnitType.Absolute) }
                            }
                        };

                        var labelActivityItemPickingIsActivityItemPickingComment = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        };
                        var labelActivityItemPickingProductCode = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        };
                        var labelActivityItemPickingProductUnitBc = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        };
                        var labelActivityItemPickingUnitCode = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        };
                        var labelActivityItemPickingQuantityOrdered = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        };
                        var labelActivityItemPickingQuantityReceived = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        };
                        var labelActivityItemPickingQuantityAvailable = new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Start
                        };
                        labelActivityItemPickingIsActivityItemPickingComment.SetBinding(
                            Label.TextProperty,
                            new Binding("IsActivityItemPickingComment"));
                        labelActivityItemPickingProductCode.SetBinding(
                            Label.TextProperty,
                            new Binding("ProductCode"));
                        labelActivityItemPickingProductUnitBc.SetBinding(
                            Label.TextProperty,
                            new Binding("ProductUnitBC"));
                        labelActivityItemPickingUnitCode.SetBinding(
                            Label.TextProperty,
                            new Binding("UnitCode"));
                        labelActivityItemPickingQuantityOrdered.SetBinding(
                            Label.TextProperty,
                            new Binding(
                                "ActivityItemPickingQuantityOrdered",
                                BindingMode.Default,
                                null,
                                null,
                                "{0:F3}"));
                        labelActivityItemPickingQuantityReceived.SetBinding(
                            Label.TextProperty,
                            new Binding(
                                "ActivityItemPickingQuantityShipped",
                                BindingMode.Default,
                                null,
                                null,
                                "{0:F3}"));
                        labelActivityItemPickingQuantityAvailable.SetBinding(
                            Label.TextProperty,
                            new Binding(
                                "ActivityItemPickingQuantityAvailable",
                                BindingMode.Default,
                                null,
                                null,
                                "{0:F3}"));

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

                // TODO: Change dictionary key to string variable
                var isAddKey = true;
                foreach (var item in this.ResourceDictionary)
                {
                    if (item.Key == "dataTemplatePickingItem")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.ResourceDictionary.Add("dataTemplatePickingItem", dataTemplate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the grid column 0 width.
        /// </summary>
        private double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width.
        /// </summary>
        private double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width.
        /// </summary>
        private double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width.
        /// </summary>
        private double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width.
        /// </summary>
        private double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 5 width.
        /// </summary>
        private double ColumnWidth5 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 6 width.
        /// </summary>
        private double ColumnWidth6 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary.
        /// </summary>
        private ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets the list picking items.
        /// </summary>
        private List<PickingItem> ListPickingItems { get; set; } = new List<PickingItem>();

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
        /// The GetItemNotFoundMessage.
        /// </summary>
        /// <param name="locMap">.</param>
        /// <param name="prodInfo">.</param>
        /// <returns>.</returns>
        public override string GetItemNotFoundMessage(LocationMap locMap, ProductInfo prodInfo)
        {
            var msg = base.GetItemNotFoundMessage(locMap, prodInfo);

            if (this.SysEnv.SysSettings.IsPickOverrideMsg1)
            {
                msg = $"Product scanned, code \'{prodInfo.Code}\' bc \'{prodInfo.UnitBc}\' unit" +
                    $" \'{prodInfo.UnitCode}\', is not found on this order for warehouse {locMap.CurrLocMap.WhsFkRefno}.";
            }

            return msg;
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
                ////isCatchAsk = this.SysEnv.SysSettings.IsPickCatchAsk;
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
                result = !this.SysEnv.SysSettings.IsPickingNoCatchAskZeroEnabled;
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

                decimal per;

                // Check weight tolerance
                if (this.ProdInfo.WeightTolerance != 0)
                {
                    per = this.ProdInfo.WeightTolerance / 100;
                }
                else
                {
                    per = this.SysEnv.SysSettings.ProdWeightUnitTolerancePercent / 100;
                }

                var weightUpper = (this.ProdInfo.UnitGrossWeight + (this.ProdInfo.UnitGrossWeight * per)) * qty;
                var weightLower = (this.ProdInfo.UnitGrossWeight - (this.ProdInfo.UnitGrossWeight * per)) * qty;

                if (weight < weightLower || weight > weightUpper)
                {
                    // clsPickingSingleAcivityitem is different, it only raises a message box as a warning, but continues.
                    // Lekker changed its mind again and only wanting a warning rather than stop.
                    // TODO: Figure out why raising a MessageOnlyException is not being caught as such, but rather
                    // as a System.Exception.
                    throw new MessageOnlyException("Item is outside of expected weight.");
                }
            }

            // New items are identified with 0 < this.FkRefno, do not validate.
            if (this.SysEnv.SysSettings.PickQtyOverWarn != SysCommon.ActPickingQtyOverValLevels.NoValidationOrWarning
                && this.ItemFkRefno > 0)
            {
                try
                {
                    this.PickingModuleQuantityOverCheck(decimal.Parse(data.Root.Element("alt_qty").Value));
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                    switch (this.SysEnv.SysSettings.PickQtyOverWarn)
                    {
                        case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndContinue:
                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    "Quantity Over",
                                    msg + Environment.NewLine + "Continue?",
                                    "Yes",
                                    "No")))
                            {
                                throw new IsReadyToApplyException(msg, true, true);
                            }
                            else
                            {
                                throw new IsReadyToApplyException(msg, true, false);
                            }

                        case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndStop:
                            throw new IsReadyToApplyException(msg, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="activityDetailFkRefno">.</param>
        /// <returns>.</returns>
        public override async Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int activityDetailFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var itemSchemeData = new ActItemSchemeData();

            var xmlRequest = $@"
                <?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
                <imclient>
                    <command>
                        <action>REQUEST SALES ORDER ITEM FROM DETAIL</action>
                        {await SysCommon.GetRequestDefaults(false)}
                        <ser_refno>{activityDetailFkRefno}</ser_refno>
                    </command>
                </imclient>";

            var request = XDocument.Parse(xmlRequest);
            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            var responseData = SysCommon.SubmitComm(xmlString.ToString());
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseData);
            var tempSchemeData = new ProductSchemeData();
            itemSchemeData.ActItemFkRefno = int.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/ActivityItemFKRefno")?.InnerText);
            itemSchemeData.ActDetailQuantity = decimal.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailquantity").InnerText);
            itemSchemeData.UnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/unitcode").InnerText;
            itemSchemeData.ActDetailBaseQuantity = decimal.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailbasequantity").InnerText);
            itemSchemeData.BaseUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/baseunitcode").InnerText;
            itemSchemeData.ActDetailWeightQuantity = decimal.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailweightquantity").InnerText);
            itemSchemeData.WeightUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/weightunitcode").InnerText;
            itemSchemeData.LocMapWhsFkRefno = int.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/locationmapwarehousefkrefno").InnerText);
            itemSchemeData.LocMapSectFkRefno = int.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/locationmapsectionfkrefno").InnerText);
            itemSchemeData.LocMapSlotFkRefno = int.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/locationmapslotfkrefno").InnerText);
            tempSchemeData.SingleTradeItemSerialNumber
                = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailsingletradeitemserialnumber").InnerText;
            tempSchemeData.LogisticUnitBatchLotNumber
                = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbatchlotnumber").InnerText;
            tempSchemeData.LogisticUnitSkidNumber
                = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitskidcode").InnerText;
            tempSchemeData.VariantCode = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantcode").InnerText;
            tempSchemeData.VariantDescription
                = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantdescription").InnerText;
            tempSchemeData.VariantNumber = int.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantno").InnerText);

            if (!(xmlDoc.SelectSingleNode(
                "/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText == string.Empty))
            {
                tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(
                    xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText);
            }

            if (!(xmlDoc.SelectSingleNode(
                "/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText == string.Empty))
            {
                tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(
                    xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText);
            }

            if (!(xmlDoc.SelectSingleNode(
                "/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText == string.Empty))
            {
                tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(
                    xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText);
            }

            if (!(xmlDoc.SelectSingleNode(
                "/VFPData/seg/activitydetaillogisticunitduedate").InnerText == string.Empty))
            {
                tempSchemeData.LogisticUnitDueDate = DateTime.Parse(
                    xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitduedate").InnerText);
            }

            if (!(xmlDoc.SelectSingleNode(
                "/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText == string.Empty))
            {
                tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(
                    xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText);
            }

            itemSchemeData.ProdSchemeData = tempSchemeData;
            itemSchemeData.ActDetailExtra.Text1 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra1").InnerText;
            itemSchemeData.ActDetailExtra.Text2 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra2").InnerText;
            itemSchemeData.ActDetailExtra.Text3 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra3").InnerText;
            itemSchemeData.ActDetailExtra.Text4 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra4").InnerText;

            if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra5").InnerText == string.Empty))
            {
                itemSchemeData.ActDetailExtra.DateTime1 = DateTime.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra5").InnerText);
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
            var comment = string.Empty;
            if (selectedActivityItem is PickingItem pickingItem)
            {
                var results = await DatabaseConnection.GetInstance().
                SqlConn.QueryAsync<ActivityItemPickingCommentModel>(
                    "SELECT * FROM tActivityItemPickingComment WHERE ActivityItemRefno = ?",
                    Convert.ToInt32(pickingItem.ActivityItemRefno));
                if (results != null && results.Count > 0)
                {
                    comment = Convert.ToString(results[0].ActivityItemPickingCommentComment);
                }
            }

            return comment;
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse sales order item # {actItemStringFkRefno}.";
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find sales order item # {actItemStringFkRefno}.";
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
            var prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            if (prodUnitRefno <= 0)
            {
                throw new Exception("prodUnitRefno expected to be greater than zero.");
            }

            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            await InsertActivityItemInTables(prodUnitRefno);
            AddActivityItemToList(prodUnitRefno);
            return;

            //////////////////////////////////////////////////////////////////////////
            async Task InsertActivityItemInTables(int prodUnitRefno)
            {
                await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        // NOTE: ActivityItemStatus is set to inserted for later submission use.
                        _ = transaction.Execute(@"INSERT INTO tActivityItem (
                                ActivityItemSubType
                                ,ActivityItemStatus
                                ,ActivityRefno
                                ,ActivityItemIsAdded
                                ) VALUES (?, ?, ?, ?)",
                            Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                            Convert.ToInt32(SysCommon.ActStatus.Inserted),
                            Convert.ToInt32(this.ActHdr.HdrRefno),
                            1);

                        this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT last_insert_rowid() FROM tActivityItem"));

                        // Get temporary item FK refno for newly inserted line item.
                        this.ItemFkRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            @"SELECT MAX(tActivityItemCount.ActivityItemFKRefno)
                              FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemSubType" +
                            $" = {(int)SysCommon.ActSubTypes.Picking}"));
                        this.ItemFkRefno = this.ItemFkRefno + 1;

                        var query = @"
                            INSERT INTO tActivityItemPicking (
                                    ActivityItemRefno, 
                                    ActivityItemSubType, 
                                    ActivityItemFKRefno, 
                                    ActivityItemStringFKRefno, 
                                    ProductUnitRefno, 
                                    WarehouseLocationMapRefno, 
                                    ActivityItemPickingQuantityOrdered, 
                                    ActivityItemPickingBaseQuantityOrdered, 
                                    ActivityItemPickingWeightQuantityOrdered, 
                                    ActivityItemPickingQuantityShipped, 
                                    ActivityItemPickingBaseQuantityShipped, 
                                    ActivityItemPickingWeightQuantityShipped, 
                                    ActivityItemPickingCustomerProductUnitCode, 
                                    ActivityItemPickingDescription, 
                                    ActivityItemPickingQuantityAvailable, 
                                    ActivityItemPickingBaseQuantityAvailable, 
                                    ActivityItemPickingWeightQuantityAvailable, 
                                    ActivityItemPickingIsReviewed
                                ) VALUES (
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?, 
                                    ?)";

                        var locMap = LocationMap.GetInstance().CurrLocMap;
                        var parameters = new object[]
                        {
                            Convert.ToInt32(this.ItemRefno),
                            Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                            Convert.ToInt32(this.ItemFkRefno),
                            Convert.ToString(this.ItemFkRefno),
                            Convert.ToInt32(prodUnitRefno),
                            Convert.ToInt32(locMap.WhsLocMapRefno),
                            Convert.ToInt32(0),
                            Convert.ToInt32(0),
                            Convert.ToInt32(0),
                            Convert.ToInt32(0),
                            Convert.ToInt32(0),
                            Convert.ToInt32(0),
                            Convert.ToString(string.Empty),
                            Convert.ToString(this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc),
                            Convert.ToInt32(0),
                            Convert.ToInt32(0),
                            Convert.ToInt32(0),
                            Convert.ToInt32(false),
                        };

                        transaction.Execute(query, parameters);
                        transaction.Commit();
                    }
                    catch (SQLiteException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                });
            }

            //////////////////////////////////////////////////////////////////////////
            void AddActivityItemToList(int prodUnitRefno)
            {
                var locMap = LocationMap.GetInstance().CurrLocMap;
                var item = new PickingItem()
                {
                    ActivityItemRefno = this.ItemRefno,
                    ActivityItemFKRefno = this.ItemFkRefno,
                    ActivityItemStringFKRefno = this.ItemFkRefno.ToString(),
                    ProductUnitRefno = prodUnitRefno,
                    ProductRefno = this.ProdInfo.Refno,
                    ProductCode = this.ProdInfo.Code,
                    ProductDescription = this.ProdInfo.Desc,
                    UnitCode = this.ProdInfo.UnitCode,
                    ProductUnitBC = this.ProdInfo.UnitBc,
                    ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc,
                    LocationMapRefno = locMap.WhsLocMapRefno,
                    LocationCode = locMap.WhsCode,
                    LocationName = locMap.WhsName,
                    ActivityItemPickingCustomerProductUnitCode = string.Empty,
                    ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    IsActivityItemPickingComment = string.Empty,
                    ActivityItemPickingQuantityOrdered = 0,
                    ActivityItemPickingBaseQuantityOrdered = 0,
                    ActivityItemPickingWeightQuantityOrdered = 0,
                    ActivityItemPickingQuantityShipped = 0,
                    ActivityItemPickingBaseQuantityShipped = 0,
                    ActivityItemPickingWeightQuantityShipped = 0,
                    ActivityItemPickingQuantityAvailable = 0,
                    ActivityItemPickingBaseQuantityAvailable = 0,
                    ActivityItemPickingWeightQuantityAvailable = 0,
                };

                this.ListItems.Add(item);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            var tableActPickingItem = new TableActivityPickingItem();
            var tableActPickingDetail = new TableActivityPickingDetail();
            tableActPickingItem.RegisterDataAccessListenerProgressOut(this);
            this.DataLayerProgressOutHandler(this, EventArgs.Empty, 1);
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await tableActPickingDetail.PurgeData();
                await tableActPickingItem.PurgeData();
            }
            this.DataLayerProgressOutHandler(this, EventArgs.Empty, 3);
            await tableActPickingItem.RequestItemData(this.ActHdr.HdrFkRefno, data, this.ActHdr.HdrRefno);
            this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            tableActPickingItem.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            ////var observableCollection = new ObservableCollection<Object>(this.ListItems);

            // This code is optimized, it iterates over this.ListItems only once.
            var items = new ObservableCollection<Object>();
            var positiveRefnoItems = new List<PickingItem>();
            var negativeRefnoItems = new List<PickingItem>();
            foreach (var pickingItem in this.ListItems.Cast<PickingItem>())
            {
                if (pickingItem.ActivityItemFKRefno >= 0)
                {
                    positiveRefnoItems.Add(pickingItem);
                }
                else
                {
                    negativeRefnoItems.Add(pickingItem);
                }
            }

            foreach (var item in positiveRefnoItems.Cast<PickingItem>())
            {
                items.Add(item);
            }

            foreach (var item in negativeRefnoItems.Cast<PickingItem>())
            {
                items.Add(item);
            }

            var labelIsActivityItemPickingCommentHeader = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelActivityItemPickingProductCodeHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelActivityItemPickingProductUnitBcHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelActivityItemPickingUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelActivityItemPickingQuantityOrderedHeader = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelActivityItemPickingQuantityReceivedHeader = new Label { Text = "Shipped", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelActivityItemPickingQuantityAvailableHeader = new Label { Text = "Available", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsActivityItemPickingCommentHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingProductCodeHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingProductUnitBcHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingUnitCodeHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityOrderedHeader.Text);
            this.ColumnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityReceivedHeader.Text);
            this.ColumnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityAvailableHeader.Text);

            var gridItems = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth6, GridUnitType.Absolute) },
                },
            };

            gridItems.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridItems.Children.Add(labelIsActivityItemPickingCommentHeader);
            gridItems.Children.Add(labelActivityItemPickingProductCodeHeader);
            gridItems.Children.Add(labelActivityItemPickingProductUnitBcHeader);
            gridItems.Children.Add(labelActivityItemPickingUnitCodeHeader);
            gridItems.Children.Add(labelActivityItemPickingQuantityOrderedHeader);
            gridItems.Children.Add(labelActivityItemPickingQuantityReceivedHeader);

            if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
            {
                gridItems.Children.Add(labelActivityItemPickingQuantityAvailableHeader);
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

            Grid.SetRow(gridItems, 0);

            gridMainActivity.Children.Add(gridItems);

            var gridService = DependencyService.Get<IGridService>();
            var columnDefinitions = gridItems.ColumnDefinitions;
            var isPickShowItemQtyAvail = this.SysEnv.SysSettings.IsPickShowItemQtyAvail;
            for (var item = 0; item < this.ListItems.Count; item++)
            {
                var pickingItem = (PickingItem)this.ListItems[item];
                var detailWidths = new[]
                {
                    gridService.CalculateTextWidth(pickingItem.IsActivityItemPickingComment),
                    gridService.CalculateTextWidth(pickingItem.ProductCode),
                    gridService.CalculateTextWidth(pickingItem.ProductUnitBC),
                    gridService.CalculateTextWidth(pickingItem.UnitCode),
                    gridService.CalculateTextWidth($"{pickingItem.ActivityItemPickingQuantityOrdered:F3}"),
                    gridService.CalculateTextWidth($"{pickingItem.ActivityItemPickingWeightQuantityShipped:F3}"),
                    gridService.CalculateTextWidth($"{pickingItem.ActivityItemPickingQuantityAvailable:F3}")
                };

                if (detailWidths[0] > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidths[0];
                    columnDefinitions[0].Width = this.ColumnWidth0;
                }

                if (detailWidths[1] > this.ColumnWidth1)
                {
                    this.ColumnWidth1 = detailWidths[1];
                    columnDefinitions[1].Width = this.ColumnWidth1;
                }

                if (detailWidths[2] > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = detailWidths[2];
                    columnDefinitions[2].Width = this.ColumnWidth2;
                }

                if (detailWidths[3] > this.ColumnWidth3)
                {
                    this.ColumnWidth3 = detailWidths[3];
                    columnDefinitions[3].Width = this.ColumnWidth3;
                }

                if (detailWidths[4] > this.ColumnWidth4)
                {
                    this.ColumnWidth4 = detailWidths[4];
                    columnDefinitions[4].Width = this.ColumnWidth4;
                }

                if (detailWidths[5] > this.ColumnWidth5)
                {
                    this.ColumnWidth5 = detailWidths[5];
                    columnDefinitions[5].Width = this.ColumnWidth5;
                }

                if (isPickShowItemQtyAvail)
                {
                    if (detailWidths[6] > this.ColumnWidth6)
                    {
                        this.ColumnWidth6 = detailWidths[6];
                        columnDefinitions[6].Width = this.ColumnWidth6;
                    }
                }
                else
                {
                    this.ColumnWidth6 = 0;
                    columnDefinitions[6].Width = this.ColumnWidth6;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplatePickingItem"];

            return items;
        }

        public override Grid DesignGrid()
        {
            throw new NotImplementedException();
        }

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
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var sql = string.Empty;
            if (isSubmitBatch)
            {
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql = $@"
                    SELECT tActivityItemPicking.ActivityItemRefno,
                        tActivityItemPicking.ActivityItemFKRefno,
                        tActivityItemPicking.ActivityItemStringFKRefno,
                        tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                        tProduct.ProductRefno AS ProductRefno,
                        tProduct.ProductCode AS ProductCode,
                        tProduct.ProductDescription AS ProductDescription,
                        tProduct.ProductBrandDescription AS ProductBrandDescription,
                        tUnit.UnitCode AS UnitCode,
                        tProductUnit.ProductUnitBC AS ProductUnitBC,
                        tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                        tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                        tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed,
                        CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL 
                        THEN ' ' ELSE '*' END AS IsActivityItemPickingComment,
                        tActivityItem.ActivityItemStatus AS ActivityItemStatus 
                    FROM tActivityItemPicking 
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno 
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno 
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemPicking.WarehouseLocationMapRefno 
                    LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno} 
                        AND tActivityItem.ActivityItemStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)}
                    UNION 
                    SELECT tActivityItemPicking.ActivityItemRefno,
                        tActivityItemPicking.ActivityItemFKRefno,
                        tActivityItemPicking.ActivityItemStringFKRefno,
                        tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                        tProduct.ProductRefno AS ProductRefno,
                        tProduct.ProductCode AS ProductCode,
                        tProduct.ProductDescription AS ProductDescription,
                        tProduct.ProductBrandDescription AS ProductBrandDescription,
                        tUnit.UnitCode AS UnitCode,
                        tProductUnit.ProductUnitBC AS ProductUnitBC,
                        tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                        tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                        tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed,
                        CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL 
                            THEN ' '
                            ELSE '*'
                            END AS IsActivityItemPickingComment,
                        tActivityItem.ActivityItemStatus AS ActivityItemStatus 
                    FROM tActivityItemPicking 
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno 
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno 
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemPicking.WarehouseLocationMapRefno 
                    LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno} 
                        AND tActivityDetail.ActivityDetailStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)}";
            }
            else
            {
                sql = $@"
                    SELECT tActivityItemPicking.ActivityItemRefno,
                        tActivityItemPicking.ActivityItemFKRefno,
                        tActivityItemPicking.ActivityItemStringFKRefno,
                        tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                        tProduct.ProductRefno AS ProductRefno,
                        tProduct.ProductCode AS ProductCode,
                        tProduct.ProductDescription AS ProductDescription,
                        tProduct.ProductBrandDescription AS ProductBrandDescription,
                        tUnit.UnitCode AS UnitCode,
                        tProductUnit.ProductUnitBC AS ProductUnitBC,
                        tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                        tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,
                        tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,
                        tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                        tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,
                        tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed,
                        CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL 
                            THEN ' '
                            ELSE '*'
                            END AS IsActivityItemPickingComment,
                        tActivityItem.ActivityItemStatus AS ActivityItemStatus 
                    FROM tActivityItemPicking 
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno 
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno 
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemPicking.WarehouseLocationMapRefno 
                    LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno}";
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
            //        TODO: Fix sorting
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //    }
            // }
            this.ListPickingItems = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<PickingItem>(sql.ToString());
            this.ListPickingItems = this.ListPickingItems.OrderBy(x => x.ActivityItemRefno).ToList();
            this.ListItems = this.ListPickingItems.Cast<object>().ToList();
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

            this.ListItems = this.ListPickingItems.Where(pickingItem => pickingItem.ProductCode.ToString().ToLower()
                .Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (var pickingItem in this.ListItems.Cast<PickingItem>())
            {
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
        /// <returns><see cref="int"/> Found activity item reference number else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            var foundActItemRefno = 0;

            var sql = $@"
                SELECT tActivityItem.ActivityItemRefno,
                        tActivityItemPicking.ActivityItemFKRefno,
                        tActivityItemPicking.ProductUnitRefno,
                        tLocationMapWarehouse.LocationMapRefno 
                    FROM tActivityItemPicking 
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemPicking.WarehouseLocationMapRefno 
                    WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno} 
                        AND tActivityItemPicking.WarehouseLocationMapRefno = {locMap.WhsLocMapRefno} 
                        AND tActivityItemPicking.ProductUnitRefno = {prodUnitRefno}";

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<PickingItem>(sql);
            if (results != null && results.Count > 0)
            {
                this.ItemFkRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                foundActItemRefno = this.ItemRefno;
            }
            else
            {
                foreach (var item in this.ListPickingItems)
                {
                    if (item.LocationMapRefno == locMap.WhsLocMapRefno && item.ProductUnitRefno == prodUnitRefno)
                    {
                        this.ItemRefno = item.ActivityItemRefno;
                        this.ItemFkRefno = item.ActivityItemFKRefno;
                        this.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(item.ProductRefno);
                        this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                        foundActItemRefno = this.ItemRefno;
                        break;
                    }
                }

                // If location defaulting is on, attempt to find a matching line item by ignoring the line item
                // warehouse location. Yes this will be a problem if the sales order has multiple item lines
                // on it for the same product and different warehouse.
                // Picking is the only activity that could have this sort of problem. All other activities are
                // warehouse friend because the can't mix warehouses or are created by the mobile itself.
                if (foundActItemRefno == 0 && this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    sql = $@"
                        SELECT tActivityItem.ActivityItemRefno,
                                tActivityItemPicking.ActivityItemFKRefno,
                                tActivityItemPicking.ProductUnitRefno,
                                tLocationMapWarehouse.LocationMapRefno 
                            FROM tActivityItemPicking 
                            INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemPicking.ActivityItemRefno 
                            INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemPicking.WarehouseLocationMapRefno 
                            WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno} 
                            AND tActivityItemPicking.ProductUnitRefno = {prodUnitRefno}";

                    results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<PickingItem>(sql);
                    if (results != null && results.Count > 0)
                    {
                        var locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                            Convert.ToInt32(results[0].LocationMapRefno),
                            prodUnitRefno,
                            SysCommon.ProdLocMapTypes.Picking);

                        if (!await LocationMapTasks.IsCompleteLocMap(locMapRefno))
                        {
                            // Default location to the floor.
                            locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                Convert.ToInt32(results[0].LocationMapRefno),
                                prodUnitRefno,
                                SysCommon.ProdLocMapTypes.Floor);
                        }

                        if (await LocationMapTasks.IsCompleteLocMap(locMapRefno))
                        {
                            LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                            LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap;
                            this.LocMap = LocationMap.GetInstance().CurrLocMap;
                            this.ItemFkRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                            this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                            foundActItemRefno = this.ItemRefno;
                        }
                    }
                    else
                    {
                        foreach (var item in this.ListPickingItems)
                        {
                            if (item.ProductUnitRefno == prodUnitRefno)
                            {
                                this.ItemRefno = item.ActivityItemRefno;
                                this.ItemFkRefno = item.ActivityItemFKRefno;
                                this.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(item.ProductRefno);
                                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                                foundActItemRefno = this.ItemRefno;
                                break;
                            }
                        }
                    }
                }
            }

            return foundActItemRefno;
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var sendReq = await this.CreateDeleteXml(XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<skid_data>" + this.ActHdr.SkidInfo.CurrSkid.SkidCode + "</skid_data>"
                + "</params>"));

                _ = SysCommon.SubmitComm(sendReq);
            }

            return 0;
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateDeleteXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                throw new NotImplementedException("Only skid deletion is supported.");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE SALES ORDER DETAIL SKID</action>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.WhsFkRefno + "</whs_id>"
                    + "<refno>" + this.ActHdr.HdrFkRefno + "</refno>"
                    + "<itm_refno>" + this.ItemFkRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
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
                + "<sect_refno></sect_refno>"
                + "<slot_refno></slot_refno>"
                + "<refno></refno>"
                + "</imclient>");

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
        public override void ReplaceQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
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
        /// <param name="quantity">The quantity<see cref="decimal"/>.</param>
        /// <param name="baseQuantity">The baseQuantity<see cref="decimal"/>.</param>
        /// <param name="weightQuantity">The weightQuantity<see cref="decimal"/>.</param>
        public override void AddToQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                //// Update UI here?
                this.ListPickingItems.First(x => x.ActivityItemFKRefno == pickingItem.ActivityItemFKRefno)
                    .ActivityItemPickingQuantityShipped += quantity;
                this.ListPickingItems.First(x => x.ActivityItemFKRefno == pickingItem.ActivityItemFKRefno)
                    .ActivityItemPickingBaseQuantityShipped += quantity;
                this.ListPickingItems.First(x => x.ActivityItemFKRefno == pickingItem.ActivityItemFKRefno)
                    .ActivityItemPickingWeightQuantityShipped += quantity;

                this.ListItems = this.ListPickingItems.Cast<object>().ToList();
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is PickingItem pickingItem)
            {
                var query = "UPDATE tActivityItemPicking SET ActivityItemPickingQuantityShipped = ?," +
                            " ActivityItemPickingBaseQuantityShipped = ?, ActivityItemPickingWeightQuantityShipped = ?" +
                            " WHERE tActivityItemPicking.ActivityItemRefno = ?";
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
        /// /// </summary>
        /// <param name="actFkRefno"></param>
        /// <returns></returns>
        /// <summary>
        ///.
        /// </summary>
        /// <param name="actFkRefno">.</param>
        /// <returns>.</returns>
        public async Task<string> CreateRequestTotalQuantityXml(int actFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PICKING TOTAL QUANTITY</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + actFkRefno + "</refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The GetTotalQuantity.
        /// </summary>
        /// <returns>.</returns>
        public override async Task<string> GetTotalQuantity()
        {
            var msg = string.Empty;

            try
            {
                var query = "SELECT SUM(tActivityItemPicking.ActivityItemPickingQuantityOrdered) AS Ordered,"
                            + "SUM(tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered) AS WeightOrdered,"
                            + "SUM(tActivityItemPicking.ActivityItemPickingQuantityShipped) AS Shipped,"
                            + "SUM(tActivityItemPicking.ActivityItemPickingWeightQuantityShipped) AS WeightShipped,"
                            + "tUnit.UnitCode AS WtUnitCode "
                            + "FROM tActivityItem "
                            + "INNER JOIN tActivityItemPicking ON tActivityItemPicking.ActivityItemRefno = "
                            + "tActivityItem.ActivityItemRefno "
                            + "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno "
                            + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.WtUnitRefno "
                            + $"WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno} "
                            + "GROUP BY tUnit.UnitCode";

                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<PickingItemSummary>(query);

                if (results != null)
                {
                    foreach (dynamic pickingItem in results.Cast<PickingDetail>())
                    {
                        // Because an aggregate sum function is used in a query. There will always be a result record.
                        // This causes problems detecting if there is nothing to sum, resulting in null.
                        // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
                        msg +=
                            $"Total ordered:\r{Convert.ToInt32(Convert.ToDecimal(pickingItem.Ordered))} @ {Convert.ToDecimal(pickingItem.WeightOrdered)}{Convert.ToString(pickingItem.WtUnitCode)}\nTotal shipped:\r{Convert.ToInt32(Convert.ToDecimal(pickingItem.Shipped))} @ {Convert.ToDecimal(pickingItem.WeightShipped)}{Convert.ToString(pickingItem.WtUnitCode)}";
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred.", ex);
            }

            return msg;
        }

        /// <summary>
        /// The PickingModuleQuantityOverCheck.
        /// </summary>
        /// <param name="actItemPickQuantity">.</param>
        /// <exception cref="MessageOnlyException"> Quantity shipped greater message.</exception>
        private void PickingModuleQuantityOverCheck(decimal actItemPickQuantity)
        {
            if (this.SelectedItem is PickingItem pickingItem)
            {
                var shipped = Convert.ToDecimal(pickingItem.ActivityItemPickingQuantityShipped);
                var ordered = Convert.ToDecimal(pickingItem.ActivityItemPickingQuantityOrdered);

                if (shipped + actItemPickQuantity > ordered)
                {
                    throw new MessageOnlyException(
                        $"The resulting quantity shipped {shipped + actItemPickQuantity} {this.ProdInfo.UnitCode}" +
                        $" will be greater than the quantity ordered {ordered} {this.ProdInfo.UnitCode}.");
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
            return Convert.ToInt32(tmpFkItRefno);
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
        /// The Update.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Update(XDocument data)
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

        /// <summary>
        /// The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
