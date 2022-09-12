// <copyright file="clsAdjustmentActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
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

    /// <summary>
    /// Defines the <see cref="AdjActItem" />.
    /// </summary>
    public class AdjActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjActItem"/> class.
        /// </summary>
        /// <param name="activity"><see cref="Activity"/> Activity.</param>
        public AdjActItem(Activity activity)
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

                var labelProductID = new Label { VerticalOptions = LayoutOptions.Center };
                var labelProductUnitBC = new Label { VerticalOptions = LayoutOptions.Center };
                var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                var labelWhs = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelProductID.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelProductUnitBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelQty.SetBinding(Label.TextProperty, new Binding(
                    "ActivityItemAdjustmentQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWhs.SetBinding(Label.TextProperty, new Binding("LocationCode"));

                // Add labels to grid
                gridData.Children.Add(labelProductID);
                gridData.Children.Add(labelProductUnitBC);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWhs);

                // Set label columns
                Grid.SetColumn(labelProductID, 0);
                Grid.SetColumn(labelProductUnitBC, 1);
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
        /// Gets or sets the grid column 0 width..
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width..
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width..
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width..
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width..
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary..
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets the ListAdjustmentItems
        /// Gets or sets a list of adjustment items..
        /// </summary>
        public List<AdjustmentItem> ListAdjustmentItems { get; set; } = new List<AdjustmentItem>();

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <remarks>
        /// Iterate through the ListItems looking for a matched activity item reference number.
        /// If a match is found set the SelectedActivityItem to one found in the ListItems
        /// else the SelectedActivityItem is set to null.
        /// </remarks>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItem(int actItemFoundRefno)
        {
            this.SelectedListItemIdx = -1;
            if (actItemFoundRefno >= 0)
            {
                var idx = 0;
                foreach (AdjustmentItem item in this.ListItems)
                {
                    if (item.ActivityItemRefno == this.ItemRefno)
                    {
                        this.SelectedActivityItem = item;
                        this.SelectedListItemIdx = idx;
                        break;
                    }

                    idx++;
                }
            }
            else
            {
                this.SelectedActivityItem = null;
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
                isCatchAsk = this.SysEnv.SysSettings.IsAdjustmentCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// Check if the product is catch count.
        /// </summary>
        /// <returns><see cref="bool"/> True if catch count, else false.</returns>
        public override bool IsCatchAskZero()
        {
            var result = this.IsCatchAsk();
            if (result && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsAdjustmentNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        /// Get the unable to parse item foreign key reference number message.
        /// </summary>
        /// <param name="actItemStringFKRefno"><see cref="string"/> Activity item string foreign key reference number.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to parse adjustment item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// Get the unable to find the item foreign key reference number message.
        /// </summary>
        /// <param name="actItemStringFKRefno"> Activity item string foreign key reference number.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to find adjustment item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// Get the data view sort by foreign key reference number.
        /// </summary>
        /// <returns><see cref="string"/> Sort by foreign key reference number index name.</returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemStringFKRefno";
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <remarks>Checking for existing an activity item that matches is assumed to have been performed
        /// before invoking this method, unless the askToInsertAsNewItem parameter argument is true.</remarks>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>
        /// <param name="askToInsertAsNewItem"><see cref="bool"/> Stop and ask to insert an item.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            var insertNew = true;

            var prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            if (prodUnitRefno <= 0)
            {
                throw new Exception("prodUnitRefno expected to be greater than zero.");
            }

            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            if (askToInsertAsNewItem)
            {
                var foundActItemRefno = await this.GetActivityItemRefno(prodUnitRefno);
                if (foundActItemRefno > 0)
                {
                    var msg = "A line item already exists for the location and product." + Environment.NewLine +
                              "Do you wish to create an additional one?";
                    insertNew = await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert("New Item", msg, "Yes", "No"));
                }
            }

            if (insertNew)
            {
                await InsertActivityItemInTables(prodUnitRefno);
                AddActivityItemToList(prodUnitRefno);
            }

            //////////////////////////////////////////////////////////////////////////
            async Task InsertActivityItemInTables(int prodUnitRefno)
            {
                await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        _ = transaction.Execute(
                            "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                                " ActivityRefno) VALUES (?, ?, ?)",
                            Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                            Convert.ToInt32(SysCommon.ActStatus.NoChange),
                            Convert.ToInt32(this.ActHdr.HdrRefno));

                        this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT last_insert_rowid() FROM tActivityItem"));
                        this.ItemFKRefno = this.ItemRefno * -1;

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

                        var parameters = new object[]
                        {
                            Convert.ToInt32(this.ItemRefno),
                            Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                            Convert.ToInt32(this.ItemFKRefno),
                            Convert.ToString(this.ItemFKRefno),
                            Convert.ToInt32(prodUnitRefno),
                            Convert.ToInt32(this.LocMap.WhsLocMapRefno),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                            Convert.ToDecimal(0),
                            Convert.ToString(this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc),
                        };

                        transaction.Execute(query, parameters);
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
                var adjItem = new AdjustmentItem
                {
                    ActivityItemRefno = this.ItemRefno,
                    ActivityItemFKRefno = this.ItemFKRefno,
                    ActivityItemStringFKRefno = this.ItemFKRefno.ToString(),
                    ProductUnitRefno = prodUnitRefno,
                    ProductRefno = this.ProdInfo.Refno,
                    ProductCode = this.ProdInfo.Code,
                    ProductDescription = this.ProdInfo.Desc,
                    UnitCode = this.ProdInfo.UnitCode,
                    ProductUnitBC = this.ProdInfo.UnitBC,
                    ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc,
                    LocationMapRefno = this.LocMap.WhsLocMapRefno,
                    LocationCode = this.LocMap.WhsCode,
                    LocationName = this.LocMap.WhsName,
                    ActivityItemStatus = (int)SysCommon.ActStatus.Inserted,
                    ActivityItemAdjustmentQuantity = 0,
                    ActivityItemAdjustmentBaseQuantity = 0,
                    ActivityItemAdjustmentWeightQuantity = 0,
                };

                this.ListItems.Add(adjItem);
                this.SelectedListItemIdx = this.ListItems.Count - 1;
                _ = this.AssignActivityItem(adjItem);
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
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                var adjItem = new TableActivityAdjustmentItem();
                var adjDetail = new TableActivityAdjustmentDetail();
                await adjDetail.PurgeData();
                await adjItem.PurgeData();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                adjItem.RegisterDataAccessListenerProgressOut(this);
                await adjItem.RequestData(this.ActHdr.HdrRefno);
                adjItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid GridMainActivity,
            ref ListView ListViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (AdjustmentItem adjustmentItem in this.ListItems)
            {
                observableCollection.Add(adjustmentItem);
            }

            var labelProductIdHeader = new Label
            {
                Text = "Product ID",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelBCHeader = new Label
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
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBCHeader.Text);
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
            gridHeader.Children.Add(labelBCHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWhsHeader);

            // Set label columns
            Grid.SetColumn(labelProductIdHeader, 0);
            Grid.SetColumn(labelBCHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelQtyHeader, 3);
            Grid.SetColumn(labelWhsHeader, 4);

            Grid.SetRow(gridHeader, 0);

            GridMainActivity.Children.Add(gridHeader);

            foreach (AdjustmentItem adjustmentItem in observableCollection)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.ProductCode);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.ProductUnitBC);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.UnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", adjustmentItem.ActivityItemAdjustmentQuantity));
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

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateAdjustmentItem"];

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
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
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
                sql.Append("tActivityitem.ActivityItemStatus ");
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
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
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

            this.ListAdjustmentItems = await DatabaseConnection.GetInstance().SQLConn.
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

            foreach (AdjustmentItem adjustmentItem in this.ListItems)
            {
                observableCollection.Add(adjustmentItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// Assign the activity item values from the activity item parameter arguments.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task AssignActivityItem(object activityItem)
        {
            if (activityItem is AdjustmentItem item)
            {
                this.SelectedActivityItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFKRefno = item.ActivityItemFKRefno;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                // This code needs to be before the call to GetSelectedItemLocation, because the skid location may be
                // used if the product unit and warehouse match.
                if (this.SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid &&
                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ProdInfo.Refno)
                {
                    this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                }

                var locMap = await this.GetSelectedItemLocation(item.LocationMapRefno);

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // Only assign the current location if the map is complete else leave it alone.
                    if (locMap != null && await LocationMapTasks.IsCompleteLocMap(locMap))
                    {
                        // Update the chosen location with the current when defaulting is turned on.
                        LocationMap.GetInstance().CurrLocMap = locMap.Clone();
                        LocationMap.GetInstance().ChosenLocMap = locMap.Clone();

                        // Assign the skid the location, because defaulting is enabled.
                        if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                        {
                            this.ActHdr.SkidInfo.CurrSkid.LocMap = locMap.Clone();
                        }

                        // Update the activity item location.
                        this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }
                else
                {
                    // Only assign the current location if the map is complete else leave it alone.
                    if (locMap != null && await LocationMapTasks.IsCompleteLocMap(locMap))
                    {
                        // Do not update the current, chosen or skid location when defaulting is off.
                        this.LocMap = locMap.Clone();
                    }
                }

                // LocationMap.GetInstance().CurrLocMap warehouse needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        /// <summary>
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item reference number else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno)
        {
            var foundActItemRefno = 0;
            var sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemAdjustment.ActivityItemFKRefno,");
            sql.Append("tActivityItemAdjustment.ProductUnitRefno AS ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno ");
            sql.Append("FROM tActivityItemAdjustment ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.HdrRefno);
            sql.AppendFormat(" AND tActivityItemAdjustment.WarehouseLocationMapRefno = {0}", this.LocMap.WhsLocMapRefno);
            sql.AppendFormat(" AND tActivityItemAdjustment.ProductUnitRefno = {0}", prodUnitRefno);

            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<AdjustmentItem>(
                sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.ItemFKRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.ItemRefno;
            }

            return foundActItemRefno;
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var barCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
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
                + "<is_bc>" + prodUnitIsBC + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCode + "</gtin>"
                + "<whs_id></whs_id>"
                + "<sect_refno></sect_refno>"
                + "<slot_refno></slot_refno>"
                + "<refno></refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
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
                            " ActivityItemAdjustmentBaseQuantity = ?, ActivityItemAdjustmentWeightQuantity = ?" +
                            " WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    adjustmentItem.ActivityItemAdjustmentQuantity,
                    adjustmentItem.ActivityItemAdjustmentBaseQuantity,
                    adjustmentItem.ActivityItemAdjustmentWeightQuantity,
                    adjustmentItem.ActivityItemRefno,
                };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key reference number, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            object tmpFKItRefno = ((AdjustmentItem)resourceType).ActivityItemFKRefno;
            return tmpFKItRefno == null ? 0 : Convert.ToInt32(tmpFKItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return string.Format("Adjustment # {0}", this.ActHdr.HdrFKRefno);
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFKRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<SActItemSchemeData> GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
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
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> CreateDeleteXML(XDocument data)
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
        /// <returns>The <see cref="Task{string}"/>.</returns>
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
            if (this.SelectedActivityItem is AdjustmentItem item)
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
