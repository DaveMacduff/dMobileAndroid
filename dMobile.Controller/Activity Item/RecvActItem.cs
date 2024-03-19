// <copyright file="RecvActItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="RecvActItem" />.
    /// </summary>
    public class RecvActItem : ActItem
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
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="RecvActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public RecvActItem(Activity.Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(RecvActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Receiving;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsReceivingCreateNewItemNoAsk;

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
                    },
                };

                var labelIsActivityItemReceivingCommentItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelProductCodeItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelProductUnitBcItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelUnitCodeItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemReceivingQuantityOrderedItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityItemReceivingQuantityReceivedItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelIsActivityItemReceivingCommentItem.SetBinding(Label.TextProperty, new Binding("IsActivityItemReceivingComment"));
                labelProductCodeItem.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelProductUnitBcItem.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCodeItem.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelActivityItemReceivingQuantityOrderedItem.SetBinding(Label.TextProperty, new Binding("ActivityItemReceivingQuantityOrdered", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemReceivingQuantityReceivedItem.SetBinding(Label.TextProperty, new Binding("ActivityItemReceivingQuantityReceived", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelIsActivityItemReceivingCommentItem);
                gridData.Children.Add(labelProductCodeItem);
                gridData.Children.Add(labelProductUnitBcItem);
                gridData.Children.Add(labelUnitCodeItem);
                gridData.Children.Add(labelActivityItemReceivingQuantityOrderedItem);
                gridData.Children.Add(labelActivityItemReceivingQuantityReceivedItem);

                Grid.SetColumn(labelIsActivityItemReceivingCommentItem, 0);
                Grid.SetColumn(labelProductCodeItem, 1);
                Grid.SetColumn(labelProductUnitBcItem, 2);
                Grid.SetColumn(labelUnitCodeItem, 3);
                Grid.SetColumn(labelActivityItemReceivingQuantityOrderedItem, 4);
                Grid.SetColumn(labelActivityItemReceivingQuantityReceivedItem, 5);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplateReceivingItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateReceivingItem", dataTemplate);
            }
        }

        /// <summary>
        /// Defines the listReceiveItems.
        /// </summary>
        private List<ReceiveItem> ListReceiveItems { get; set; } = new List<ReceiveItem>();

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
                foreach (ReceiveItem item in this.ListItems)
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
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAsk()
        {
            var isCatchAsk = this.ProdInfo.IsCatchAsk;

            if (isCatchAsk)
            {
                // modules are for exclusions
                ////isCatchAsk = this.SysEnv.SysSettings.IsReceivingCatchAskEnabled;
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
                result = !this.SysEnv.SysSettings.IsReceivingNoCatchAskZeroEnabled;
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
                if (((ReceiveItem)item).ActivityItemFKRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
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

                var responseData = SysCommon.SubmitComm(sendReq);
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var docError = new XmlDocument();
                    docError.LoadXml(responseData);
                    var errorDetail = docError.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }
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
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE PURCHASE ORDER DETAIL SKID</action>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.WhsFkRefno + "</whs_id>"
                    + "<refno>" + this.ActHdr.HdrFkRefno + "</refno>"
                    + "<itm_refno>" + this.ItemFkRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }
            else
            {
                throw new NotImplementedException("Only skid deletion is supported.");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            var cmmt = string.Empty;

            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tActivityItemReceivingCommentModel>(
                    "SELECT * FROM tActivityItemReceivingComment WHERE ActivityItemRefno = ?",
                    receiveItem.ActivityItemRefno);

                if (results != null && results.Count > 0)
                {
                    cmmt = results[0].ActivityItemReceivingCommentComment;
                }
            }

            return cmmt;
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

            if (this.SelectedItem is ReceiveItem item)
            {
                if (Math.Sign(item.ActivityItemReceivingQuantityOrdered) == 0 ||
                    Math.Sign(item.ActivityItemReceivingQuantityOrdered) == 1)
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
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">The actItemStringFKRefno<see cref="string"/>.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse purchase order item# {actItemStringFkRefno}";
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">The actItemStringFKRefno<see cref="string"/>.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find purchase order item# {actItemStringFkRefno}";
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
        /// <param name="data">.</param>
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

                var tableActRecvItem = new TableActivityReceivingItem();
                var tableActRecvDetail = new TableActivityReceivingDetail();
                await tableActRecvDetail.PurgeData();
                await tableActRecvItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableActRecvItem.RegisterDataAccessListenerProgressOut((RecvActItem)this);
                await tableActRecvItem.RequestItemData(this.ActHdr.HdrRefno);
                tableActRecvItem.UnregisterDataAccessListenerProgressOut((RecvActItem)this);
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
            var receiveItems = new ObservableCollection<object>();

            foreach (ReceiveItem receiveItem in this.ListItems)
            {
                if (receiveItem.ActivityItemFKRefno >= 0)
                {
                    receiveItems.Add(receiveItem);
                }
            }

            // Add negative new line items to the bottom.
            foreach (ReceiveItem receiveItem in this.ListItems)
            {
                if (receiveItem.ActivityItemFKRefno < 0)
                {
                    receiveItems.Add(receiveItem);
                }
            }

            var labelIsItemComment = new Label
            {
                Text = "C",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsItemComment.Text);

            var labelProductCode = new Label
            {
                Text = "Product ID",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCode.Text);

            var labelProductUnitBc = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductUnitBc.Text);

            var labelUnitCode = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCode.Text);

            var labelQuantityOrdered = new Label
            {
                Text = "Ordered",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQuantityOrdered.Text);

            var labelQuantityReceived = new Label
            {
                Text = "Received",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            this.columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQuantityReceived.Text);

            // Header grid
            var gridItems = new Grid
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
                },
            };

            gridItems.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridItems.Children.Add(labelIsItemComment);
            gridItems.Children.Add(labelProductCode);
            gridItems.Children.Add(labelProductUnitBc);
            gridItems.Children.Add(labelUnitCode);
            gridItems.Children.Add(labelQuantityOrdered);
            gridItems.Children.Add(labelQuantityReceived);

            Grid.SetColumn(labelIsItemComment, 0);
            Grid.SetColumn(labelProductCode, 1);
            Grid.SetColumn(labelProductUnitBc, 2);
            Grid.SetColumn(labelUnitCode, 3);
            Grid.SetColumn(labelQuantityOrdered, 4);
            Grid.SetColumn(labelQuantityReceived, 5);

            Grid.SetRow(gridItems, 0);

            gridMainActivity.Children.Add(gridItems);

            foreach (ReceiveItem receiveItem in receiveItems)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    receiveItem.IsActivityItemReceivingComment);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.ProductCode);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.ProductUnitBC);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.UnitCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{receiveItem.ActivityItemReceivingQuantityOrdered:F3}");
                var detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{receiveItem.ActivityItemReceivingQuantityReceived:F3}");

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridItems.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridItems.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridItems.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridItems.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridItems.ColumnDefinitions[4].Width = this.columnWidth4;
                }

                if (detailWidth5 > this.columnWidth5)
                {
                    this.columnWidth5 = detailWidth5;
                    gridItems.ColumnDefinitions[5].Width = this.columnWidth5;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateReceivingItem"];

            return receiveItems;
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
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql.Append("SELECT recvItem.ActivityItemRefno,");
                sql.Append("recvItem.ActivityItemFKRefno,");
                sql.Append("recvItem.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("recvItem.ActivityItemReceivingQuantityOrdered");
                sql.Append("- recvItem.ActivityItemReceivingQuantityReceivedToDate ");
                sql.Append("AS ActivityItemReceivingQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityOrdered ");
                sql.Append("- recvItem.ActivityItemReceivingBaseQuantityReceivedToDate ");
                sql.Append("AS ActivityItemReceivingBaseQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityOrdered ");
                sql.Append("- recvItem.ActivityItemReceivingWeightQuantityReceivedToDate ");
                sql.Append("AS ActivityItemReceivingWeightQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceived AS ActivityItemReceivingQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityReceived AS ActivityItemReceivingBaseQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceived AS ActivityItemReceivingWeightQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingVendorProductUnitCode AS ActivityItemReceivingVendorProductUnitCode,");
                sql.Append("recvItem.ActivityItemReceivingDescription AS ActivityItemReceivingDescription,");
                sql.Append("whs.LocationMapRefno AS LocationMapRefno,");
                sql.Append("whs.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("whs.LocationMapWarehouseName AS LocationName,");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceivedToDate AS ActivityItemReceivingQuantityReceivedToDate,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceivedToDate AS ActivityItemReceivingWeightQuantityReceivedToDate,");
                sql.Append("CASE WHEN itemComment.ActivityItemReceivingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemReceivingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemReceiving recvItem ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("recvItem.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
                sql.Append("recvItem.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse whs ON whs.LocationMapRefno = ");
                sql.Append("recvItem.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemReceivingComment itemComment ON itemComment.ActivityItemRefno = ");
                sql.Append("recvItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT recvItem.ActivityItemRefno,");
                sql.Append("recvItem.ActivityItemFKRefno,");
                sql.Append("recvItem.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("recvItem.ActivityItemReceivingQuantityOrdered - ");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityOrdered - ");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingBaseQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityOrdered - ");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingWeightQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingVendorProductUnitCode,");
                sql.Append("recvItem.ActivityItemReceivingDescription,");
                sql.Append("whs.LocationMapRefno,");
                sql.Append("whs.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("whs.LocationMapWarehouseName AS LocationName,");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceivedToDate,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceivedToDate,");
                sql.Append("CASE WHEN itemComment.ActivityItemReceivingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemReceivingComment,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemReceiving recvItem ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("recvItem.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = ");
                sql.Append("recvItem.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
                sql.Append("recvItem.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse whs ON whs.LocationMapRefno = ");
                sql.Append("recvItem.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemReceivingComment itemComment ON itemComment.ActivityItemRefno = ");
                sql.Append("recvItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus = {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.Append("ORDER BY recvItem.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT recvItem.ActivityItemRefno,");
                sql.Append("recvItem.ActivityItemFKRefno,");
                sql.Append("recvItem.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("recvItem.ActivityItemReceivingQuantityOrdered - ");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityOrdered - ");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingBaseQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityOrdered - ");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingWeightQuantityOrdered,");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceived AS ActivityItemReceivingQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingBaseQuantityReceived AS ActivityItemReceivingBaseQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceived AS ActivityItemReceivingWeightQuantityReceived,");
                sql.Append("recvItem.ActivityItemReceivingVendorProductUnitCode AS ActivityItemReceivingVendorProductUnitCode,");
                sql.Append("recvItem.ActivityItemReceivingDescription AS ActivityItemReceivingDescription,");
                sql.Append("whs.LocationMapRefno AS LocationMapRefno,");
                sql.Append("whs.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("whs.LocationMapWarehouseName AS LocationName,");
                sql.Append("recvItem.ActivityItemReceivingQuantityReceivedToDate AS ActivityItemReceivingQuantityReceivedToDate,");
                sql.Append("recvItem.ActivityItemReceivingWeightQuantityReceivedToDate AS ActivityItemReceivingWeightQuantityReceivedToDate,");
                sql.Append("CASE WHEN itemComment.ActivityItemReceivingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemReceivingComment ");
                sql.Append("FROM tActivityItemReceiving recvItem ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("recvItem.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
                sql.Append("recvItem.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse whs ON whs.LocationMapRefno = ");
                sql.Append("recvItem.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemReceivingComment itemComment ON itemComment.ActivityItemRefno = ");
                sql.Append("recvItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.Append("ORDER BY recvItem.ActivityItemRefno");
            }

            this.ListReceiveItems = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ReceiveItem>(sql.ToString());
            this.ListItems = this.ListReceiveItems.Cast<object>().ToList();
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

            this.ListItems = this.ListReceiveItems.Where(receiveItem => receiveItem.ProductCode.ToLower().Contains(
                searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (ReceiveItem receiveItem in this.ListItems)
            {
                observableCollection.Add(receiveItem);
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
            if (selectedActivityItem is ReceiveItem item)
            {
                this.SelectedItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFkRefno = item.ActivityItemFKRefno;
                ////this.ActStatus = (SysCommon.ActStatus)item.ActivityItemStatus;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);

                await AssignItemLocation(item);

                // LocationMap.GetInstance().CurrLocMap warehouse needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }

            return this.ItemFkRefno;

            //////////////////////////////////////////////////////////////////////////
            async Task AssignItemLocation(ReceiveItem item)
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
                        // We need to have at least a location with a warehouse.
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
                            if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
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
            var sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemReceiving.ActivityItemFKRefno,");
            sql.Append("tActivityItemReceiving.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemReceiving ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
            sql.Append("tActivityItemReceiving.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
            sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
            sql.AppendFormat("AND tActivityItemReceiving.WarehouseLocationMapRefno = {0} ", locMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemReceiving.ProductUnitRefno = {0}", prodUnitRefno);

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ReceiveItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.ItemFkRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.ItemRefno;
            }
            else
            {
                foreach (var item in this.ListReceiveItems)
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
            }

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
            var prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT RECEIVING ITEM</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBc.ToString() + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + prodUnitBarCodeCode + "</gtin>" + "<whs_id></whs_id>"
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
            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                receiveItem.ActivityItemReceivingQuantityReceived = quantity;
                receiveItem.ActivityItemReceivingBaseQuantityReceived = baseQuantity;
                receiveItem.ActivityItemReceivingWeightQuantityReceived = weightQuantity;
            }
        }

        /// <summary>
        /// The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void AddToQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                receiveItem.ActivityItemReceivingQuantityReceived += quantity;
                receiveItem.ActivityItemReceivingBaseQuantityReceived += baseQuantity;
                receiveItem.ActivityItemReceivingWeightQuantityReceived += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is ReceiveItem receiveItem)
            {
                var query = "UPDATE tActivityItemReceiving SET ActivityItemReceivingQuantityReceived = ?," +
                            " ActivityItemReceivingBaseQuantityReceived = ?, ActivityItemReceivingWeightQuantityReceived =" +
                            " ? WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    receiveItem.ActivityItemReceivingQuantityReceived,
                    receiveItem.ActivityItemReceivingBaseQuantityReceived,
                    receiveItem.ActivityItemReceivingWeightQuantityReceived,
                    receiveItem.ActivityItemRefno,
                };

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// The GetTotalQuantity.
        /// </summary>
        /// <returns>.</returns>
        public override async Task<string> GetTotalQuantity()
        {
            var msg = "Total received:";
            var query = "SELECT SUM(tActivityItemReceiving.ActivityItemReceivingQuantityOrdered) AS "
                        + "Ordered,"
                        + "SUM(tActivityItemReceiving.ActivityItemReceivingQuantityReceived) AS Received "
                        + "FROM tActivityItem "
                        + "INNER JOIN tActivityItemReceiving ON tActivityItemReceiving.ActivityItemRefno = "
                        + "tActivityItem.ActivityItemRefno "
                        + $"WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno}";

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<TotalQuantity>(query);

            if (results != null && results.Count > 0)
            {
                foreach (var result in results)
                {
                    // Because an aggregate sum function is used in a query. There will always be a result record.
                    // This causes problems detecting if there is a valid result.
                    // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
                    msg += $"\r{Convert.ToString(result.Ordered)} / {Convert.ToString(result.Received)}";
                }
            }

            return msg;
        }

        /// <summary>
        /// The IsReadyToApply.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task IsReadyToApply(XDocument data)
        {
            await base.IsReadyToApply(data);

            // New items are identified with 0 < this.FkRefno, do not validate.
            if (this.SysEnv.SysSettings.RecvQtyOverWarn != SysCommon.ActReceivingQtyOverValLevels.NoValidationOrWarning
                && this.ItemFkRefno > 0)
            {
                try
                {
                    this.ReceivingModuleQuantityOverCheck(decimal.Parse(data.Root.Element("alt_qty").Value));
                }
                catch (Exception ex)
                {
                    switch (this.SysEnv.SysSettings.RecvQtyOverWarn)
                    {
                        case SysCommon.ActReceivingQtyOverValLevels.ValidateWarnAndContinue:

                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    "Quantity Over",
                                    ex.Message + Environment.NewLine + "Continue?",
                                    "Yes",
                                    "No")))
                            {
                                throw new IsReadyToApplyException(ex.Message, true, true);
                            }
                            else
                            {
                                throw new IsReadyToApplyException(ex.Message, true, false);
                            }

                        case SysCommon.ActReceivingQtyOverValLevels.ValidateWarnAndStop:
                            throw new IsReadyToApplyException(ex.Message, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// The ReceivingModuleQuantityOverCheck.
        /// </summary>
        /// <param name="actItemRecvQuantity">The actItemRecvQuantity<see cref="decimal"/>.</param>
        private void ReceivingModuleQuantityOverCheck(decimal actItemRecvQuantity)
        {
            if (this.SelectedItem is ReceiveItem receiveItem)
            {
                var received = Convert.ToDecimal(receiveItem.ActivityItemReceivingQuantityReceived);
                var ordered = Convert.ToDecimal(receiveItem.ActivityItemReceivingQuantityOrdered);

                if (received + actItemRecvQuantity > ordered)
                {
                    throw new Exception($"The resulting quantity received {received + actItemRecvQuantity}" +
                        $" {this.ProdInfo.UnitCode} will be greater than the quantity ordered {ordered}" +
                        $" {this.ProdInfo.UnitCode}.");
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
            object tmpFkItRefno = ((ReceiveItem)resourceType).ActivityItemFKRefno;
            return tmpFkItRefno == null ? 0 : Convert.ToInt32(tmpFkItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            ////return $"Receive # {this.ActHdr.HdrFkRefno}";
            var poNumber = ((Receive)this.ActHdr.SelectedActivity).ActivityReceivingPONumber;
            return $"PO number # {poNumber}";
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFkRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
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
        /// The Update.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Update(XDocument data)
        {
            throw new NotImplementedException();
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
    }
}
