// <copyright file="POWorkSheetActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity_Item
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Activity;
    using Bedrock;
    using GroundWork;
    using Resources.Services;
    using Resources.Types;
    using SQLite;
    using Xamarin.Forms;

    /// <summary>
    ///     Defines the <see cref="PoWorkSheetActItem" />.
    /// </summary>
    public class PoWorkSheetActItem : ActItem
    {
        /// <summary>
        ///     Defines the columnWidth0.
        /// </summary>
        private double columnWidth0;

        /// <summary>
        ///     Defines the columnWidth1.
        /// </summary>
        private double columnWidth1;

        /// <summary>
        ///     Defines the columnWidth2.
        /// </summary>
        private double columnWidth2;

        /// <summary>
        ///     Defines the listPurchaseOrderWorkSheetItems.
        /// </summary>
        private List<PurchaseOrderWorkSheetItem> listPurchaseOrderWorkSheetItems =
            new List<PurchaseOrderWorkSheetItem>();

        /// <summary>
        ///     Defines the resourceDictionary.
        /// </summary>
        private readonly ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        ///     Initializes a new instance of the <see cref="PoWorkSheetActItem" /> class.
        /// </summary>
        /// <param name="activity">.</param>
        public PoWorkSheetActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(PoWorkSheetActItem).FullName;

            // Always allow adding product
            this.IsAllowAddItem = true;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPOWorkSheetCreateNewItemNoAsk;

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
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) }
                    }
                };

                var labelActivityPoProductId = new Label
                    { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityPoOrdered = new Label
                    { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelActivityPoWeight = new Label
                    { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelActivityPoProductId.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelActivityPoOrdered.SetBinding(Label.TextProperty,
                    new Binding("ActivityItemPurchaseOrderWorkSheetQuantityOrdered", BindingMode.Default, null, null,
                        "{0:F3}"));
                labelActivityPoWeight.SetBinding(Label.TextProperty,
                    new Binding("ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered", BindingMode.Default, null,
                        null, "{0:F3}"));

                gridData.Children.Add(labelActivityPoProductId);
                gridData.Children.Add(labelActivityPoOrdered);
                gridData.Children.Add(labelActivityPoWeight);

                Grid.SetColumn(labelActivityPoProductId, 0);
                Grid.SetColumn(labelActivityPoOrdered, 1);
                Grid.SetColumn(labelActivityPoWeight, 2);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplatePOWorkSheetItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplatePOWorkSheetItem", dataTemplate);
            }
        }

        /// <summary>
        ///     Gets or sets the NewActItemFKRefno.
        /// </summary>
        public int NewActItemFkRefno { get; set; }

        /// <summary>
        ///     Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int" /> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItemFromList(int actItemFoundRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether allowed to add skid detail.
        /// </summary>
        /// <returns><see cref="bool" /> True if allowed to add skid detail.</returns>
        public override bool IsAllowAddSkidDetail()
        {
            return false;
        }

        /// <summary>
        ///     The IsCatchAsk.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAsk()
        {
            var isCatchAsk = this.ProdInfo.IsCatchAsk;

            if (isCatchAsk)
            {
                // modules are for exclusions
                ////isCatchAsk = this.SysEnv.SysSettings.IsPOWorkSheetCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        ///     Check if the product is catch count.
        /// </summary>
        /// <returns><see cref="bool" /> True if catch count, else false.</returns>
        public override bool IsCatchAskZero()
        {
            var result = false;
            if (this.IsCatchAsk() && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsPOWorkSheetNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        ///     Find the list item index of the item that matches the activity item foreign key reference number.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int" /> Activity item foreign key reference number.</param>
        /// <returns><see cref="int" /> Activity list item found index, else -1.</returns>
        public override int FindActivityItemIndex(int activityItemFkRefno)
        {
            var foundIdx = -1;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in this.ListItems)
            {
                foundIdx++;
                if (((PoWorkSheetActItem)item).ItemFkRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        /// <summary>
        ///     The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse purchase order work sheet item# {actItemStringFkRefno}";
        }

        /// <summary>
        ///     The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find purchase order work sheet item# {actItemStringFkRefno}";
        }

        /// <summary>
        ///     The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFkRefnoIdx()
        {
            return "ActivityItemStringFKRefno";
        }

        /// <summary>
        ///     Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument" /> Adjustment activity information to insert.</param>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public override async Task Insert(XDocument data)
        {
            data = data ?? throw new ArgumentNullException(nameof(data));

            var prodUnitRefno = int.Parse(data.Root!.Element("ProductUnitRefno") !.Value);
            if (prodUnitRefno <= 0)
            {
                throw new Exception("prodUnitRefno expected to be greater than zero.");
            }

            var altQty = Math.Round(decimal.Parse(data.Root.Element("alt_qty") !.Value), 3);
            var baseQty = Math.Round(decimal.Parse(data.Root.Element("base_qty") !.Value), 3);
            var wtQty = Math.Round(decimal.Parse(data.Root.Element("wt_qty") !.Value), 3);
            var productCode = data.Root.Element("ProductCode") !.Value;
            var unitCode = data.Root.Element("UnitCode") !.Value;

            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            var productRefno = this.ProdInfo.Refno;

            await InsertActivityItemInTables();
            var xmlSend = await this.CreateSubmitXml(data);
            var responseData = SysCommon.SubmitComm(xmlSend);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseData);

            // check if result is an error message
            if (responseData.Contains("<message>ERROR</message>"))
            {
                var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                if (errorDetail.Contains("MSGBOX-"))
                {
                    throw new MessageOnlyException(errorDetail);
                }

                throw new Exception(errorDetail);
            }

            this.NewActItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
            this.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
            this.ActHdr.HdrFkRefno = this.ItemFkRefno;

            AddActivityItemToList();
            return;

            //////////////////////////////////////////////////////////////////////////
            async Task InsertActivityItemInTables()
            {
                await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        // NOTE: ActivityItemStatus is set to inserted for later submission use.
                        var query = @"INSERT INTO tActivityItem
                                (ActivityItemSubType
                                ,ActivityItemStatus
                                ,ActivityRefno
                                ,ActivityItemIsAdded)
                                VALUES
                                (?, ?, ?, ?)";
                        _ = transaction.Execute(
                            query,
                            Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet),
                            Convert.ToInt32(SysCommon.ActStatus.Inserted),
                            Convert.ToInt32(this.ActHdr.HdrRefno),
                            1);

                        query = "SELECT last_insert_rowid() FROM tActivityItem";
                        this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(query));

                        // Get temporary item FK refno for newly inserted line item.
                        query = "SELECT MAX(tActivityItemPurchaseOrderWorkSheet.ActivityItemFKRefno)" +
                                " FROM tActivityItemPurchaseOrderWorkSheet" +
                                " WHERE tActivityItemPurchaseOrderWorkSheet.ActivityItemSubType" +
                                $" = {(int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet}";
                        this.ItemFkRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(query));
                        this.ItemFkRefno++;

                        query = "INSERT INTO tActivityItemPurchaseOrderWorkSheet (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemFKRefno, " +
                                "ActivityItemStringFKRefno, " +
                                "ProductUnitRefno, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityItemPurchaseOrderWorkSheetQuantityOrdered, " +
                                "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered, " +
                                "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered" +
                                ") VALUES (" +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?)";

                        var locMap = LocationMap.GetInstance().CurrLocMap;
                        var parameters = new object[]
                        {
                            Convert.ToInt32(this.ItemRefno),
                            Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet),
                            Convert.ToInt32(this.ItemFkRefno),
                            Convert.ToString(this.ItemFkRefno),
                            Convert.ToInt32(prodUnitRefno),
                            Convert.ToInt32(locMap.WhsLocMapRefno),
                            altQty,
                            baseQty,
                            wtQty
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
            void AddActivityItemToList()
            {
                var locMap = LocationMap.GetInstance().CurrLocMap;
                var item = new PurchaseOrderWorkSheetItem
                {
                    ActivityItemRefno = this.ItemRefno,
                    ActivityItemFKRefno = this.ItemFkRefno,
                    ActivityItemStringFKRefno = this.ItemFkRefno.ToString(),
                    ProductUnitRefno = prodUnitRefno,
                    ProductRefno = productRefno,
                    ProductCode = productCode,
                    ProductDescription = ProdInfo.Desc,
                    UnitCode = unitCode,
                    ProductUnitSizeDescription = ProdInfo.UnitSizeDesc,
                    LocationMapRefno = locMap.WhsLocMapRefno,
                    LocationCode = locMap.WhsCode,
                    LocationName = locMap.WhsName,
                    ActivityItemStatus = (int)SysCommon.ActStatus.Inserted,
                    ActivityItemPurchaseOrderWorkSheetQuantityOrdered = Math.Round(altQty, 3),
                    ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = Math.Round(baseQty, 3),
                    ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = Math.Round(wtQty, 3)
                };

                this.ListItems.Add(item);
            }
        }

        /// <summary>
        ///     The Update.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public override async Task Update(XDocument data)
        {
            var decQty = Math.Round(decimal.Parse(data.Root.Element("alt_qty").Value), 3);
            var decBaseQty = Math.Round(decimal.Parse(data.Root.Element("base_qty").Value), 3);
            var decWeightQty = Math.Round(decimal.Parse(data.Root.Element("wt_qty").Value), 3);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        // Update the Activity Item quantity shipped
                        var query = "UPDATE tActivityItemPurchaseOrderWorkSheet SET " +
                                    "ActivityItemPurchaseOrderWorkSheetQuantityOrdered = ?, " +
                                    "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = ?, " +
                                    "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = ? " +
                                    "WHERE ActivityItemRefno = ?";
                        transaction.Execute(query, decQty, decBaseQty, decWeightQty, this.ItemRefno);
                        transaction.Execute(
                            "UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?",
                            Convert.ToInt32(SysCommon.ActStatus.Updated), this.ItemRefno);
                        transaction.Execute("UPDATE tActivity SET ActivityStatus = ? WHERE ActivityRefno = ?",
                            Convert.ToInt32(SysCommon.ActStatus.Updated), this.ActHdr.HdrRefno);
                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
                    }
                });
            }
            else
            {
                var xmlSend = await this.CreateSubmitXml(data);
                var responseData = SysCommon.SubmitComm(xmlSend);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseData);

                // check if result is an error message
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }

                    throw new Exception(errorDetail);
                }

                this.NewActItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                this.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                this.ActHdr.HdrFkRefno = this.ItemFkRefno;
            }

            if (this.ListItems != null)
            {
                // TODO: Fix sorting
                // DataRowView rowView = null;
                // string oldSort = this.DataViewItems.Sort;
                // int rowIdx = 0;
                // this.DataViewItems.Sort = "ActivityItemFKRefno";
                // rowIdx = DataViewItems.Find(new object[] { this.FkRefno });
                var purchaseOrderWorkSheetItem = this.ListItems.OfType<PurchaseOrderWorkSheetItem>()
                    .Where(worksheetItem => worksheetItem.ActivityItemFKRefno == this.ItemFkRefno).FirstOrDefault();
                var toBeUpdated = false;

                if (purchaseOrderWorkSheetItem == null)
                {
                    purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem();
                    purchaseOrderWorkSheetItem.ActivityItemRefno = this.ItemRefno;
                    purchaseOrderWorkSheetItem.ProductUnitRefno = this.ProdInfo.UnitRefno;
                    purchaseOrderWorkSheetItem.ProductRefno = this.ProdInfo.Refno;
                    purchaseOrderWorkSheetItem.ProductCode = this.ProdInfo.Code;
                    purchaseOrderWorkSheetItem.ProductDescription = this.ProdInfo.Desc;
                    purchaseOrderWorkSheetItem.ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc;
                    purchaseOrderWorkSheetItem.UnitCode = this.ProdInfo.UnitCode;
                    purchaseOrderWorkSheetItem.ActivityItemFKRefno = this.ItemFkRefno;
                    purchaseOrderWorkSheetItem.ActivityItemStringFKRefno = this.ItemFkRefno.ToString();

                    toBeUpdated = true;
                }

                var locMapRefno = LocationMapTasks.DefaultLocMapRefnoToWhsIfUnassigned(0);
                var locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                purchaseOrderWorkSheetItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                purchaseOrderWorkSheetItem.LocationMapRefno = locMap.WhsLocMapRefno;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered = decQty;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = decBaseQty;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = decWeightQty;
                purchaseOrderWorkSheetItem.LocationCode = locMap.WhsLocMapRefno;
                purchaseOrderWorkSheetItem.LocationName = locMap.WhsName;

                // TODO: Fix sorting
                // this.DataViewItems.Sort = oldSort;
                if (toBeUpdated)
                {
                    this.ListItems.Add(purchaseOrderWorkSheetItem);
                }
            }
        }

        /// <summary>
        ///     The Submit.
        /// </summary>
        /// <param name="data">.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object" />.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public override async Task Submit(XDocument data, object selectedActivityItem)
        {
            var xmlDoc = new XmlDocument();
            var xmlSend = await this.CreateSubmitXml(data);
            var responseData = SysCommon.SubmitComm(xmlSend);
            xmlDoc.LoadXml(responseData);
            this.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                purchaseOrderWorkSheetItem.ActivityItemFKRefno = this.ItemFkRefno;
                purchaseOrderWorkSheetItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
            }

            await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                "UPDATE tActivityItem SET tActivityItem.ActivityItemStatus = ? WHERE ActivityItemRefno = ?",
                (int)SysCommon.ActStatus.NoChange,
                this.ItemRefno);
        }

        /// <summary>
        ///     The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

                var tableActPoWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();
                await tableActPoWorkSheetItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableActPoWorkSheetItem.RegisterDataAccessListenerProgressOut(this);
                await tableActPoWorkSheetItem.RequestItemData(this.ActHdr.HdrFkRefno, this.ActHdr.HdrRefno);
                tableActPoWorkSheetItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <summary>
        ///     The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The GridMainActivity<see cref="Grid" />.</param>
        /// <param name="listViewMainActivity">The ListViewMainActivity<see cref="ListView" />.</param>
        /// <returns>The <see cref="ObservableCollection{object}" />.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity,
            ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var purchaseOrderWorkSheetItem in this.ListItems.Cast<PurchaseOrderWorkSheetItem>())
            {
                observableCollection.Add(purchaseOrderWorkSheetItem);
            }

            var labelProductId = new Label
            {
                Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold
            };
            var labelOrdered = new Label
            {
                Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold
            };
            var labelWeight = new Label
            {
                Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold
            };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductId.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrdered.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeight.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelProductId);
            gridHeader.Children.Add(labelOrdered);
            gridHeader.Children.Add(labelWeight);

            Grid.SetColumn(labelProductId, 0);
            Grid.SetColumn(labelOrdered, 1);
            Grid.SetColumn(labelWeight, 2);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (var purchaseOrderWorkSheetItem in observableCollection.Cast<PurchaseOrderWorkSheetItem>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(purchaseOrderWorkSheetItem.ProductCode);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered:F3}");
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered:F3}");

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplatePOWorkSheetItem"];

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
        ///     The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool" />.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append(
                    "tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append(
                    "tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append(
                    "tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append(
                    "tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append(
                    "tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append(
                    "tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.Append("ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }

            this.listPurchaseOrderWorkSheetItems = await DatabaseConnection.GetInstance().SqlConn
                .QueryAsync<PurchaseOrderWorkSheetItem>(sql.ToString());
            this.ListItems = this.listPurchaseOrderWorkSheetItems.OrderBy(poItem => poItem.ActivityItemFKRefno)
                .Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        ///     The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            // if (string.IsNullOrEmpty(this.ActItemFilter))
            // {
            this.ListItems = this.listPurchaseOrderWorkSheetItems.Where(purchaseOrderWorkSheetItem =>
                    purchaseOrderWorkSheetItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().Trim()))
                .Cast<object>().ToList();

            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            // }
            foreach (var purchaseOrderWorkSheetItem in this.ListItems.Cast<PurchaseOrderWorkSheetItem>())
            {
                observableCollection.Add(purchaseOrderWorkSheetItem);
            }

            return observableCollection;
        }

        /// <summary>
        ///     Select an activity item.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object" /> Selected activity object.</param>
        /// <returns><see cref="Task{T}" /> Activity item foreign key reference number.</returns>
        public override async Task<int> AssignActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is PurchaseOrderWorkSheetItem item)
            {
                this.SelectedItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFkRefno = item.ActivityItemFKRefno;
                this.ActStatus = (SysCommon.ActStatus)item.ActivityItemStatus;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);

                await AssignItemLocation();

                // LocationMap.GetInstance().CurrLocMap warehouse needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }

            return this.ItemFkRefno;

            //////////////////////////////////////////////////////////////////////////
            async Task AssignItemLocation()
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
                        var defLocMapRefno = LocationMapTasks.DefaultLocMapRefnoToWhsIfUnassigned(0);
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
        ///     Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int" /> Product unit reference number.</param>
        /// <returns><see cref="int" /> Found activity item reference number else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            var foundActItemRefno = 0;
            var sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemFKRefno,");
            sql.Append("tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
            sql.AppendFormat(
                "WHERE tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno = {0} ",
                locMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno = {0} ", prodUnitRefno);

            var results = await DatabaseConnection.GetInstance().SqlConn
                .QueryAsync<PurchaseOrderWorkSheetItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.LocMap =
                    await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(results[0].LocationMapRefno));
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.ItemFkRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo =
                    await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.ItemRefno;
            }

            return foundActItemRefno;
        }

        /// <summary>
        ///     The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
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
                + "<action>SUBMIT PURCHASE ORDER WORK SHEET DETAIL</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBc + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<whs_id>" + this.LocMap.WhsFkRefno + "</whs_id>"
                + "<refno>" + this.ItemFkRefno + "</refno>"
                + "<itm_refno>" + this.ItemFkRefno + "</itm_refno>"
                + "<alt_qty>" + data.Root.Element("alt_qty").Value + "</alt_qty>"
                + "<base_qty>" + data.Root.Element("base_qty").Value + "</base_qty>"
                + "<wt_qty>" + data.Root.Element("wt_qty").Value + "</wt_qty>"
                + "<alt_unit>" + this.ProdInfo.UnitCode + "</alt_unit>"
                + "<base_unit>" + this.ProdInfo.BaseUnitCode + "</base_unit>"
                + "<wt_unit>" + this.ProdInfo.WeightUnitCode + "</wt_unit>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     The RefreshQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object" />.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity,
            decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered = quantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = baseQuantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = weightQuantity;
            }
        }

        /// <summary>
        ///     The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object" />.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void AddToQuantityInActivityItem(object selectedActivityItem, decimal quantity,
            decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered += quantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered += baseQuantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered += weightQuantity;
            }
        }

        /// <summary>
        ///     Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object" /> Activity item.</param>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                var query = "UPDATE tActivityItemPurchaseOrderWorkSheet SET " +
                            "ActivityItemPurchaseOrderWorkSheetQuantityOrdered = ?, " +
                            "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = ?, " +
                            "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = ? " +
                            "WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered,
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,
                    purchaseOrderWorkSheetItem.ActivityItemRefno
                };

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        ///     Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object" /> Resource model type.</param>
        /// <returns><see cref="int" /> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkItRefno = ((PurchaseOrderWorkSheetItem)resourceType).ActivityItemFKRefno;
            return Convert.ToInt32(tmpFkItRefno);
        }

        /// <summary>
        ///     Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string" /> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"PO worksheet # {this.ActHdr.HdrFkRefno}";
        }

        /// <summary>
        ///     The Delete.
        /// </summary>
        /// <returns>The <see cref="int" />.</returns>
        public override Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFkRefno">The actDetailFKRefno<see cref="int" />.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}" />.</returns>
        public override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object" />.</param>
        /// <returns>The <see cref="string" />.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument" />.</param>
        /// <returns>The <see cref="string" />.</returns>
        public override Task<string> CreateDeleteXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The GetTotalQuantity.
        /// </summary>
        /// <returns>The <see cref="string" />.</returns>
        public override Task<string> GetTotalQuantity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int" /> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int" /> Product reference number.</param>
        /// <returns><see cref="int" /> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is PurchaseOrderWorkSheetItem item)
            {
                if (Math.Sign(item.ActivityItemPurchaseOrderWorkSheetQuantityOrdered) == 0 ||
                    Math.Sign(item.ActivityItemPurchaseOrderWorkSheetQuantityOrdered) == 1)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Picking);
                }
                else
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Receiving);
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
        ///     The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool" />.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}" />.</returns>
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