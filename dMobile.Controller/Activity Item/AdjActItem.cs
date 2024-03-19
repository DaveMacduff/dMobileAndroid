// <copyright file="AdjActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Defines the <see cref="AdjActItem" />.
    /// </summary>
    public class AdjActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjActItem"/> class.
        /// </summary>
        /// <param name="activity"><see cref="Activity"/> Activity.</param>
        public AdjActItem(Activity.Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(AdjActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Adjustments;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsAdjustmentCreateNewItemNoAsk;

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

                var labelProductId = new Label { VerticalOptions = LayoutOptions.Center };
                var labelProductUnitBc = new Label { VerticalOptions = LayoutOptions.Center };
                var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                var labelWhs = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelProductId.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelProductUnitBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelQty.SetBinding(Label.TextProperty, new Binding(
                    "ActivityItemAdjustmentQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWhs.SetBinding(Label.TextProperty, new Binding("LocationCode"));

                // Add labels to grid
                gridData.Children.Add(labelProductId);
                gridData.Children.Add(labelProductUnitBc);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWhs);

                // Set label columns
                Grid.SetColumn(labelProductId, 0);
                Grid.SetColumn(labelProductUnitBc, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelQty, 3);
                Grid.SetColumn(labelWhs, 4);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplateAdjustmentItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplateAdjustmentItem", dataTemplate);
            }
        }

        /// <summary>
        /// Gets or sets the grid column 0 width.
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width.
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width.
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width.
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width.
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary.
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets a list of adjustment items.
        /// </summary>
        public List<AdjustmentItem> ListAdjustmentItems { get; set; } = new List<AdjustmentItem>();

        /// <summary>
        /// Set the activity item selected activity item and selected activity item index.
        /// </summary>
        /// <remarks>
        /// Iterate through the ListItems looking for a matched activity item reference number.
        /// If a match is found set the SelectedItem to one found in the ListItems
        /// else the SelectedItem is set to null and the selected list item index to -1.
        /// </remarks>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItemFromList(int actItemFoundRefno)
        {
            this.SelectedListIdx = -1;
            if (actItemFoundRefno >= 0)
            {
                var idx = 0;
                foreach (AdjustmentItem item in this.ListItems)
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
        /// Check if the product is catch ask.
        /// </summary>
        /// <returns><see cref="bool"/> True if catch ask, else false.</returns>
        public override bool IsCatchAsk()
        {
            var isCatchAsk = this.ProdInfo.IsCatchAsk;
            if (isCatchAsk)
            {
                // modules are for exclusions
                ////isCatchAsk = this.SysEnv.SysSettings.IsAdjustmentCatchAskEnabled;
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
                result = !this.SysEnv.SysSettings.IsAdjustmentNoCatchAskZeroEnabled;
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
            foreach (var item in this.ListItems)
            {
                foundIdx++;
                if (((AdjustmentItem)item).ActivityItemFKRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        /// <summary>
        /// Get the unable to parse item foreign key reference number message.
        /// </summary>
        /// <param name="actItemStringFkRefno"><see cref="string"/> Activity item string foreign key reference number.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse adjustment item # {actItemStringFkRefno}";
        }

        /// <summary>
        /// Get the unable to find the item foreign key reference number message.
        /// </summary>
        /// <param name="actItemStringFkRefno"> Activity item string foreign key reference number.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find adjustment item # {actItemStringFkRefno}";
        }

        /// <summary>
        /// Get the data view sort by foreign key reference number.
        /// </summary>
        /// <returns><see cref="string"/> Sort by foreign key reference number index name.</returns>
        public override string GetDataViewSortFkRefnoIdx()
        {
            return "ActivityItemStringFKRefno";
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <remarks>Checking for existing an activity item that matches is assumed to have been performed
        /// before invoking this method, unless the askToInsertAsNewItem parameter argument is true.</remarks>
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
                        _ = transaction.Execute(
                            "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                                " ActivityRefno, ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                            Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                            Convert.ToInt32(SysCommon.ActStatus.Inserted),
                            Convert.ToInt32(this.ActHdr.HdrRefno),
                            1);

                        this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT last_insert_rowid() FROM tActivityItem"));

                        // Get temporary item FK refno for newly inserted line item.
                        this.ItemFkRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT MAX(tActivityItemAdjustment.ActivityItemFKRefno)" +
                            $" FROM tActivityItemAdjustment WHERE tActivityItemAdjustment.ActivityItemSubType" +
                            $" = {(int)SysCommon.ActSubTypes.Adjustment}"));
                        this.ItemFkRefno = this.ItemFkRefno + 1;

                        // After insert the page refresh will load the list view in the proper order. 
                        // Inserted at the bottom.
                        var query = "INSERT INTO tActivityItemAdjustment (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemFKRefno, " +
                                    "ActivityItemStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemAdjustmentQuantity, " +
                                    "ActivityItemAdjustmentBaseQuantity, " +
                                    "ActivityItemAdjustmentWeightQuantity, " +
                                    "ActivityItemAdjustmentDescription" +
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
                                    "?" +
                                    ")";

                        var locMap = LocationMap.GetInstance().CurrLocMap;
                        var parameters = new object[]
                        {
                            Convert.ToInt32(this.ItemRefno),
                            Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                            Convert.ToInt32(this.ItemFkRefno),
                            Convert.ToString(this.ItemFkRefno),
                            Convert.ToInt32(prodUnitRefno),
                            Convert.ToInt32(locMap.WhsLocMapRefno),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                            Convert.ToString(this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc),
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
                var adjItem = new AdjustmentItem()
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
                    ActivityItemStatus = (int)SysCommon.ActStatus.Inserted,
                    ActivityItemAdjustmentQuantity = 0,
                    ActivityItemAdjustmentBaseQuantity = 0,
                    ActivityItemAdjustmentWeightQuantity = 0,
                };

                this.ListItems.Add(adjItem);
                this.ListAdjustmentItems.Add(adjItem);
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

                var adjItem = new TableActivityAdjustmentItem();
                var adjDetail = new TableActivityAdjustmentDetail();
                await adjDetail.PurgeData();
                await adjItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                adjItem.RegisterDataAccessListenerProgressOut(this);
                await adjItem.RequestItemData(this.ActHdr.HdrRefno);
                adjItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid gridMainActivity,
            ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var adjustmentItem in this.ListItems.Cast<AdjustmentItem>())
            {
                if (adjustmentItem.ActivityItemFKRefno >= 0)
                {
                    observableCollection.Add(adjustmentItem);
                }
            }

            // Add negative new line items to the bottom.
            foreach (var adjustmentItem in this.ListItems.Cast<AdjustmentItem>())
            {
                if (adjustmentItem.ActivityItemFKRefno < 0)
                {
                    observableCollection.Add(adjustmentItem);
                }
            }

            var labelProductIdHeader = new Label
            {
                Text = "Product ID",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelBcHeader = new Label
            {
                Text = "BC",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelQtyHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelWhsHeader = new Label
            {
                Text = "Whs",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIdHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBcHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);

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
            gridHeader.Children.Add(labelProductIdHeader);
            gridHeader.Children.Add(labelBcHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWhsHeader);

            // Set label columns
            Grid.SetColumn(labelProductIdHeader, 0);
            Grid.SetColumn(labelBcHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelQtyHeader, 3);
            Grid.SetColumn(labelWhsHeader, 4);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (AdjustmentItem adjustmentItem in observableCollection)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.ProductCode);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.ProductUnitBC);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.UnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{adjustmentItem.ActivityItemAdjustmentQuantity:F3}");
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    Convert.ToString(adjustmentItem.LocationCode));

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

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateAdjustmentItem"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno, ");
                sql.Append("tActivityItemAdjustment.ActivityItemFKRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity AS ActivityItemAdjustmentQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity AS ActivityItemAdjustmentBaseQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity AS ActivityItemAdjustmentWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemAdjustment ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemFKRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemAdjustment ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItemAdjustment.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemFKRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity AS ActivityItemAdjustmentQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity AS ActivityItemAdjustmentBaseQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity AS ActivityItemAdjustmentWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemAdjustment ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.Append("ORDER BY tActivityItemAdjustment.ActivityItemRefno");
            }

            this.ListAdjustmentItems = await DatabaseConnection.GetInstance().SqlConn.
                QueryAsync<AdjustmentItem>(sql.ToString());
            this.ListItems = this.ListAdjustmentItems.Cast<object>().ToList();

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
            this.ListItems = this.ListAdjustmentItems.Where(adjItem => adjItem.ProductCode.
                ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (var adjustmentItem in this.ListItems.Cast<AdjustmentItem>())
            {
                observableCollection.Add(adjustmentItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// Assign the activity item values from the activity item parameter arguments.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Activity item.</param>
        /// <returns><see cref="Task{T}"/> Activity item foreign key reference number.</returns>
        public override async Task<int> AssignActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is AdjustmentItem item)
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
                // This code needs to be before the call to GetSelectedItemLocation below, because the skid location may be
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
        /// <param name="locMap"><see cref="SLocMap"/> Location map structure.</param>
        /// <returns><see cref="int"/> Found activity item reference number else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            var foundActItemRefno = 0;

            var sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemAdjustment.ActivityItemFKRefno,");
            sql.Append("tActivityItemAdjustment.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemAdjustment ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.HdrRefno);
            sql.AppendFormat(" AND tActivityItemAdjustment.WarehouseLocationMapRefno = {0}", locMap.WhsLocMapRefno);
            sql.AppendFormat(" AND tActivityItemAdjustment.ProductUnitRefno = {0}", prodUnitRefno);

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<AdjustmentItem>(
                sql.ToString());

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
                foreach (var item in this.ListAdjustmentItems)
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
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var barCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT ADJUSTMENT ITEM</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBc + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCode + "</gtin>"
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
        /// Replaces the activity item quantities. Does not add to the existing values.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item to change.</param>
        /// <param name="quantity"><see cref="decimal"/> Alt quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        public override void ReplaceQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            if (selectedActivityItem is AdjustmentItem adjustmentItem)
            {
                adjustmentItem.ActivityItemAdjustmentQuantity = quantity;
                adjustmentItem.ActivityItemAdjustmentBaseQuantity = baseQuantity;
                adjustmentItem.ActivityItemAdjustmentWeightQuantity = weightQuantity;
            }
        }

        /// <summary>
        /// Add quantity to the existing activity item quantities. Does not reset existing values.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item to change.</param>
        /// <param name="quantity"><see cref="decimal"/> Alt quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        public override void AddToQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            if (selectedActivityItem is AdjustmentItem adjustmentItem)
            {
                adjustmentItem.ActivityItemAdjustmentQuantity += quantity;
                adjustmentItem.ActivityItemAdjustmentBaseQuantity += baseQuantity;
                adjustmentItem.ActivityItemAdjustmentWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is AdjustmentItem adjustmentItem)
            {
                var query = "UPDATE tActivityItemAdjustment SET ActivityItemAdjustmentQuantity = ?," +
                            " ActivityItemAdjustmentBaseQuantity = ?," +
                            " ActivityItemAdjustmentWeightQuantity = ?" +
                            " WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    adjustmentItem.ActivityItemAdjustmentQuantity,
                    adjustmentItem.ActivityItemAdjustmentBaseQuantity,
                    adjustmentItem.ActivityItemAdjustmentWeightQuantity,
                    adjustmentItem.ActivityItemRefno,
                };

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key reference number, else 0 if not found.</returns>
        public override int GetFkRefno(object selectedItem)
        {
            var itemFkRefno = 0;
            if (selectedItem is AdjustmentItem adjustmentItem)
            {
                this.ItemFkRefno = Convert.ToInt32(adjustmentItem.ActivityItemFKRefno);
                this.ItemRefno = Convert.ToInt32(adjustmentItem.ActivityItemRefno);
                itemFkRefno = this.ItemFkRefno;
            }

            return itemFkRefno;
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"Adjustment # {this.ActHdr.HdrFkRefno}";
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
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is AdjustmentItem item)
            {
                if (Math.Sign(item.ActivityItemAdjustmentQuantity) == 0 ||
                    Math.Sign(item.ActivityItemAdjustmentQuantity) == 1)
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

        /// <summary>
        /// Gets or sets a value indicating whether allowed to add skid detail.
        /// </summary>
        /// <returns><see cref="bool"/> True if allowed to add skid detail.</returns>
        public override bool IsAllowAddSkidDetail()
        {
            return false;
        }
    }
}
