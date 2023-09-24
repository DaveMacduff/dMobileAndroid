// <copyright file="TicketByProdActItem.cs" company="InterSect Business Systems Inc">
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
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="TicketByProdActItem" />.
    /// </summary>
    public class TicketByProdActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TicketByProdActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public TicketByProdActItem(Activity.Activity activity)
          : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2TicketByProduct;
            this.FullClassName = typeof(TicketByProdActItem).FullName;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsTicketByProductCreateNewItemNoAsk;

            // Create item template
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                    },
                };

                var labelCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelBc = new Label { VerticalOptions = LayoutOptions.Center };
                var labelUnit = new Label { VerticalOptions = LayoutOptions.Center };
                var labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                var labelExpQty = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnit.SetBinding(Label.TextProperty, new Binding("BaseUnitCode"));
                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelExpQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));

                // Add labels to grid
                gridData.Children.Add(labelCode);
                gridData.Children.Add(labelBc);
                gridData.Children.Add(labelUnit);
                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelExpQty);

                // Set label columns
                Grid.SetColumn(labelCode, 0);
                Grid.SetColumn(labelBc, 1);
                Grid.SetColumn(labelUnit, 2);
                Grid.SetColumn(labelQty, 3);
                Grid.SetColumn(labelExpQty, 4);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.ResDict)
            {
                if (item.Key == "dataTemplateByProdItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResDict.Add("dataTemplateByProdItem", dataTemplate);
            }
        }

        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Defines the columnWidth3.
        /// </summary>
        private double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Defines the columnWidth4.
        /// </summary>
        private double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary ResDict { get; } = new ResourceDictionary();

        /// <summary>
        /// Defines the listProductionTicketByProductItems.
        /// </summary>
        private List<ProductionTicketByProductItem> ListProductionTicketByProductItems { get; set; } =
            new List<ProductionTicketByProductItem>();

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
                foreach (ProductionTicketByProductItem item in this.ListItems)
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
                isCatchAsk = this.SysEnv.SysSettings.IsTicketByProductCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAskZero()
        {
            var result = this.IsCatchAsk();
            if (result && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsTicketByProductNoCatchAskZeroEnabled;
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
                if (((TicketByProdActItem)item).ItemFkRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
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
            if (this.SelectedItem is ProductionTicketByProductItem item)
            {
                if (Math.Sign(item.ActivityItemProductionQuantity) == 0 ||
                    Math.Sign(item.ActivityItemProductionQuantity) == 1)
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
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse production item# {actItemStringFkRefno}";
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find production item# {actItemStringFkRefno}";
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

            //////////////////////////////////////////////////////////////////////////
            async Task InsertActivityItemInTables(int prodUnitRefno)
            {
                await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        transaction.Execute(
                            "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                            Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
                            Convert.ToInt32(SysCommon.ActStatus.Inserted),
                            Convert.ToInt32(this.ActHdr.HdrRefno),
                            1);

                        this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT last_insert_rowid() FROM tActivityItem"));

                        var queryItemProduction = "INSERT INTO tActivityItemProduction (" +
                                                  "ActivityItemRefno, " +
                                                  "ActivityItemSubType, " +
                                                  "ProductUnitRefno, " +
                                                  "LocationMapRefno, " +
                                                  "ActivityItemProductionQuantity, " +
                                                  "ActivityItemProductionBaseQuantity, " +
                                                  "ActivityItemProductionWeightQuantity, " +
                                                  "ActivityItemProductionDescription, " +
                                                  "ActivityItemProductionQuantityExpected, " +
                                                  "ActivityItemProductionBaseQuantityExpected, " +
                                                  "ActivityItemProductionWeightQuantityExpected" +
                                                  ") VALUES (" +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?, " +
                                                  "?" +
                                                  ")";

                        var locMap = LocationMap.GetInstance().CurrLocMap;
                        var parametersItemProduction = new object[]
                        {
                            Convert.ToInt32(this.ItemRefno),
                            Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
                            Convert.ToInt32(prodUnitRefno),
                            Convert.ToInt32(locMap.WhsLocMapRefno),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                            Convert.ToString(data.Root.Element("ProductDescription").Value.ToString().TrimEnd() +
                                             " " + data.Root.Element("ProductUnitSizeDescription").Value),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                        };

                        transaction.Execute(queryItemProduction, parametersItemProduction);

                        // Get temporary item FK refno for newly inserted line item.
                        this.ItemFkRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT MAX(tActivityItemProductionByProduct.ActivityItemFKRefno)" +
                            $" FROM tActivityItemProductionByProduct"));
                        this.ItemFkRefno = this.ItemFkRefno + 1;

                        transaction.Execute(
                            "INSERT INTO tActivityItemProductionByProduct (ActivityItemRefno, ActivityItemFKRefno," +
                                " ActivityItemStringFKRefno) VALUES (?, ?, ?)",
                            Convert.ToInt32(this.ItemRefno),
                            Convert.ToInt32(this.ItemFkRefno),
                            Convert.ToString(this.ItemFkRefno));
                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();

                        Console.WriteLine(ex);
                    }
                });
            }

            //////////////////////////////////////////////////////////////////////////
            void AddActivityItemToList(int prodUnitRefno)
            {
                var locMap = LocationMap.GetInstance().CurrLocMap;
                var item = new ProductionTicketByProductItem()
                {
                    ActivityItemRefno = this.ItemRefno,
                    ActivityItemFKRefno = this.ItemFkRefno,
                    ActivityItemStringFKRefno = this.ItemFkRefno.ToString(),
                    ProductUnitRefno = prodUnitRefno,
                    ProductRefno = int.Parse(data.Root.Element("ProductRefno").Value),
                    ProductCode = data.Root.Element("ProductCode").Value,
                    ProductDescription = data.Root.Element("ProductDescription").Value,
                    BaseUnitCode = data.Root.Element("BaseUnitCode").Value,
                    ProductUnitBC = data.Root.Element("ProductUnitBC").Value,
                    ProductUnitSizeDescription = data.Root.Element("ProductUnitSizeDescription").Value,
                    LocationMapRefno = locMap.WhsLocMapRefno,
                    LocationCode = locMap.WhsCode,
                    LocationName = locMap.WhsName,
                    ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    ActivityItemProductionQuantity = 0,
                    ActivityItemProductionBaseQuantity = 0,
                    ActivityItemProductionWeightQuantity = 0,
                    ActivityItemProductionQuantityExpected = 0,
                    ActivityItemProductionBaseQuantityExpected = 0,
                    ActivityItemProductionWeightQuantityExpected = 0,
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
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

                var tableByProdItem = new TableActivityProductionTicketByProductItem();
                var tableByProdDetail = new TableActivityProductionTicketByProductDetail();
                await tableByProdDetail.PurgeData();
                await tableByProdItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableByProdItem.RegisterDataAccessListenerProgressOut((TicketByProdActItem)this);
                await tableByProdItem.RequestItemData(this.ActHdr.HdrRefno);
                tableByProdItem.UnregisterDataAccessListenerProgressOut((TicketByProdActItem)this);
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

            foreach (ProductionTicketByProductItem productionTicketByProductItem in this.ListItems)
            {
                if (productionTicketByProductItem.ActivityItemFKRefno >= 0)
                {
                    observableCollection.Add(productionTicketByProductItem);
                }

                if (productionTicketByProductItem.ActivityItemFKRefno < 0)
                {
                    observableCollection.Add(productionTicketByProductItem);
                }
            }

            var labelCodeHeader = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelBcHeader = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelExpQtyHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCodeHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBcHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelExpQtyHeader.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // Add labels to grid
            gridHeader.Children.Add(labelCodeHeader);
            gridHeader.Children.Add(labelBcHeader);
            gridHeader.Children.Add(labelUnitHeader);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelExpQtyHeader);

            // Set label columns
            Grid.SetColumn(labelCodeHeader, 0);
            Grid.SetColumn(labelBcHeader, 1);
            Grid.SetColumn(labelUnitHeader, 2);
            Grid.SetColumn(labelQtyHeader, 3);
            Grid.SetColumn(labelExpQtyHeader, 4);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (ProductionTicketByProductItem productionTicketByProductItem in observableCollection)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(productionTicketByProductItem.ProductCode));
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicketByProductItem.ProductUnitBC);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicketByProductItem.BaseUnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{productionTicketByProductItem.ActivityItemProductionQuantity:F3}");
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{productionTicketByProductItem.ActivityItemProductionWeightQuantity:F3}");

                if (detailWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }

                if (detailWidth1 > this.ColumnWidth1)
                {
                    this.ColumnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.ColumnWidth1;
                }

                if (detailWidth2 > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
                }

                if (detailWidth3 > this.ColumnWidth3)
                {
                    this.ColumnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.ColumnWidth3;
                }

                if (detailWidth4 > this.ColumnWidth4)
                {
                    this.ColumnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.ColumnWidth4;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResDict["dataTemplateByProdItem"];

            return observableCollection;
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
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity AS ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity AS ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity AS ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected AS ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected AS ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected AS ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionByProduct ");
                sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionByProduct ");
                sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(
                    "AND tActivityDetail.ActivityDetailSubType = {0} ",
                    Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.AppendFormat("ORDER BY tActivityItemProduction.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity AS ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity AS ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity AS ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected AS ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected AS ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected AS ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionByProduct ");
                sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.Append("ORDER BY tActivityItemProduction.ActivityItemRefno");
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
            this.ListProductionTicketByProductItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductionTicketByProductItem>(sql.ToString());
            this.ListItems = this.ListProductionTicketByProductItems.Cast<object>().ToList();
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
            this.ListItems = this.ListProductionTicketByProductItems.Where(prodTicketByProdItem =>
                prodTicketByProdItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().
                    Trim())).Cast<object>().ToList();
            foreach (ProductionTicketByProductItem productionTicketByProductItem in this.ListItems)
            {
                observableCollection.Add(productionTicketByProductItem);
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
            if (selectedActivityItem is ProductionTicketByProductItem item)
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
            async Task AssignItemLocation(ProductionTicketByProductItem item)
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
            sql.Append("tActivityItemProductionByProduct.ActivityItemFKRefno,");
            sql.Append("tActivityItemProduction.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemProduction ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemProduction.ActivityItemRefno ");
            sql.Append("INNER JOIN tActivityItemProductionByProduct ");
            sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
            sql.Append("= tActivityItemProduction.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemProduction.LocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
            sql.AppendFormat("AND tActivityItemProduction.LocationMapRefno = {0} ", locMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemProduction.ProductUnitRefno = {0} ", prodUnitRefno);
            sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0}", (int)SysCommon.ActSubTypes.TicketAddProduct);

            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductionTicketByProductItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.ItemFkRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.ItemRefno;
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
            var barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var strBaseUnitCode = data.Root.Element("BaseUnitCode").Value;
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRODUCTION TICKET BYPRODUCT ITEM</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBc + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<whs_id></whs_id>"
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
        public override void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is ProductionTicketByProductItem productionTicketByProductItem)
            {
                productionTicketByProductItem.ActivityItemProductionQuantity = quantity;
                productionTicketByProductItem.ActivityItemProductionBaseQuantity = baseQuantity;
                productionTicketByProductItem.ActivityItemProductionWeightQuantity = weightQuantity;
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
            if (selectedActivityItem is ProductionTicketByProductItem productionTicketByProductItem)
            {
                productionTicketByProductItem.ActivityItemProductionQuantity += quantity;
                productionTicketByProductItem.ActivityItemProductionBaseQuantity += baseQuantity;
                productionTicketByProductItem.ActivityItemProductionWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is ProductionTicketByProductItem productionTicketByProductItem)
            {
                var query = "UPDATE tActivityItemProduction SET ActivityItemProductionQuantity = ?," +
                            " ActivityItemProductionBaseQuantity = ?, ActivityItemProductionWeightQuantity = ?" +
                            " WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    productionTicketByProductItem.ActivityItemProductionQuantity,
                    productionTicketByProductItem.ActivityItemProductionBaseQuantity,
                    productionTicketByProductItem.ActivityItemProductionWeightQuantity,
                    productionTicketByProductItem.ActivityItemRefno,
                };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkItRefno = ((ProductionTicketByProductItem)resourceType).ActivityItemFKRefno;
            return tmpFkItRefno == null ? 0 : Convert.ToInt32(tmpFkItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"Production ticket # {this.ActHdr.HdrFkRefno}";
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
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFkRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
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
