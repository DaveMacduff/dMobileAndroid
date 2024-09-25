// <copyright file="TicketFinishedActItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TicketFinishedActItem" />.
    /// </summary>
    public class TicketFinishedActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TicketFinishedActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public TicketFinishedActItem(Activity.Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(TicketFinishedActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2TicketFinished;
            this.IsAskCreateNewItem = false;

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
                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionBaseQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelExpQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionBaseQuantityExpected", BindingMode.Default, null, null, "{0:F3}"));

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
                if (item.Key == "dataTemplateFinishedItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResDict.Add("dataTemplateFinishedItem", dataTemplate);
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
        /// Defines the listProductionTicketFinishedItems.
        /// </summary>
        private List<ProductionTicketFinishedItem> ListProductionTicketFinishedItems { get; set; } =
            new List<ProductionTicketFinishedItem>();

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
                foreach (var item in this.ListItems.Cast<ProductionTicketFinishedItem>())
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
                ////isCatchAsk = this.SysEnv.SysSettings.IsTicketFinishedCatchAskEnabled;
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
                result = !this.SysEnv.SysSettings.IsTicketFinishedNoCatchAskZeroEnabled;
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
                if (((TicketFinishedActItem)item).ItemFkRefno == activityItemFkRefno)
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
            if (this.SelectedItem is ProductionTicketFinishedItem item)
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
        /// <param name="actItemStringFkRefno">The actItemStringFKRefno<see cref="string"/>.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse production item# {actItemStringFkRefno}";
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">The actItemStringFKRefno<see cref="string"/>.</param>
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
            return;

            //////////////////////////////////////////////////////////////////////////
            async Task InsertActivityItemInTables(int prodUnitRefno)
            {
                await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        transaction.Execute(
                            "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                            Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished),
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
                            Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished),
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
                            "SELECT MAX(tActivityItemProductionFinished.ActivityItemFKRefno)" +
                            $" FROM tActivityItemProductionFinished"));
                        this.ItemFkRefno = this.ItemFkRefno + 1;

                        transaction.Execute(
                            "INSERT INTO tActivityItemProductionFinished (ActivityItemRefno, ActivityItemFKRefno," +
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
                var item = new ProductionTicketFinishedItem()
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

                var tableFinishedItem = new TableActivityProductionTicketFinishedItem();
                var tableFinishedDetail = new TableActivityProductionTicketFinishedDetail();
                await tableFinishedDetail.PurgeData();
                await tableFinishedItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableFinishedItem.RegisterDataAccessListenerProgressOut(this);
                await tableFinishedItem.RequestItemData(this.ActHdr.HdrFkRefno, this.ActHdr.HdrRefno);
                tableFinishedItem.UnregisterDataAccessListenerProgressOut(this);
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

            // Add all items in dataview to observable collection for display
            foreach (var productionTicketFinishedItem in this.ListItems.Cast<ProductionTicketFinishedItem>())
            {
                // ProductionTicketFinishedItem productionTicketFinishedItem = new ProductionTicketFinishedItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemProductionQuantity = Convert.ToDecimal(row["ActivityItemProductionQuantity"] != DBNull.Value ? row["ActivityItemProductionQuantity"] : null),
                //    ActivityItemProductionBaseQuantity = Convert.ToDecimal(row["ActivityItemProductionBaseQuantity"] != DBNull.Value ? row["ActivityItemProductionBaseQuantity"] : null),
                //    ActivityItemProductionWeightQuantity = Convert.ToDecimal(row["ActivityItemProductionWeightQuantity"] != DBNull.Value ? row["ActivityItemProductionWeightQuantity"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemProductionBaseQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionBaseQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionBaseQuantityExpected"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
                //    ActivityItemProductionQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionQuantityExpected"] : null),
                //    ActivityItemProductionWeightQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionWeightQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionWeightQuantityExpected"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null)
                // };
                observableCollection.Add(productionTicketFinishedItem);
            }

            var labelCodeHeader = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelBcHeader = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelExpQtyHeader = new Label { Text = "Exp Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

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

            foreach (var productionTickedFinishedItem in observableCollection.Cast<ProductionTicketFinishedItem>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(productionTickedFinishedItem.ProductCode));
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTickedFinishedItem.ProductUnitBC);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTickedFinishedItem.BaseUnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{productionTickedFinishedItem.ActivityItemProductionBaseQuantity:F3}");
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{productionTickedFinishedItem.ActivityItemProductionBaseQuantityExpected:F3}");

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

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResDict["dataTemplateFinishedItem"];

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
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemStringFKRefno,");
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
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemStringFKRefno,");
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
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("ORDER BY tActivityItemProduction.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemStringFKRefno,");
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
                sql.Append("tLocationMapSlot.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapSection.LocationMapSectionCode + \'\\\' + tLocationMapSlot.LocationMapSlotCode ");
                sql.Append("AS LocationCode,");
                sql.Append(" \' \'  AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemStringFKRefno,");
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
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("tLocationMapSection.LocationMapSectionName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemStringFKRefno,");
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
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
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
            //    }
            // }
            this.ListProductionTicketFinishedItems = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductionTicketFinishedItem>(sql.ToString());
            this.ListItems = this.ListProductionTicketFinishedItems.Cast<object>().ToList();
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
            this.ListItems = this.ListProductionTicketFinishedItems.Where(productionTicketFinishedItem => productionTicketFinishedItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            // }

            // Add all items in dataview to observable collection for display
            foreach (var productionTicketFinishedItem in this.ListItems.Cast<ProductionTicketFinishedItem>())
            {
                // ProductionTicketFinishedItem productionTicketFinishedItem = new ProductionTicketFinishedItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemProductionQuantity = Convert.ToDecimal(row["ActivityItemProductionQuantity"] != DBNull.Value ? row["ActivityItemProductionQuantity"] : null),
                //    ActivityItemProductionBaseQuantity = Convert.ToDecimal(row["ActivityItemProductionBaseQuantity"] != DBNull.Value ? row["ActivityItemProductionBaseQuantity"] : null),
                //    ActivityItemProductionWeightQuantity = Convert.ToDecimal(row["ActivityItemProductionWeightQuantity"] != DBNull.Value ? row["ActivityItemProductionWeightQuantity"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemProductionBaseQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionBaseQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionBaseQuantityExpected"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
                //    ActivityItemProductionQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionQuantityExpected"] : null),
                //    ActivityItemProductionWeightQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionWeightQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionWeightQuantityExpected"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null)
                // };
                observableCollection.Add(productionTicketFinishedItem);
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
            if (selectedActivityItem is ProductionTicketFinishedItem item)
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
            async Task AssignItemLocation(ProductionTicketFinishedItem item)
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
            var sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemProductionFinished.ActivityItemFKRefno,");
            sql.Append("tActivityItemProduction.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemProduction ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemProduction.ActivityItemRefno ");
            sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
            sql.Append("= tActivityItemProduction.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemProduction.LocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
            sql.AppendFormat("AND tActivityItemProduction.LocationMapRefno = {0} ", locMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemProduction.ProductUnitRefno = {0} ", prodUnitRefno);
            sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductionTicketFinishedItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(results[0].LocationMapRefno));
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
                + "<action>SUBMIT PRODUCTION TICKET FINISHED ITEM</action>"
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
            if (selectedActivityItem is ProductionTicketFinishedItem productionTicketFinishedItem)
            {
                productionTicketFinishedItem.ActivityItemProductionQuantity = quantity;
                productionTicketFinishedItem.ActivityItemProductionBaseQuantity = baseQuantity;
                productionTicketFinishedItem.ActivityItemProductionWeightQuantity = weightQuantity;
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
            if (selectedActivityItem is ProductionTicketFinishedItem productionTicketFinishedItem)
            {
                productionTicketFinishedItem.ActivityItemProductionQuantity += quantity;
                productionTicketFinishedItem.ActivityItemProductionBaseQuantity += baseQuantity;
                productionTicketFinishedItem.ActivityItemProductionWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is ProductionTicketFinishedItem productionTicketFinishedItem)
            {
                var query = "UPDATE tActivityItemProduction SET ActivityItemProductionQuantity = ?," +
                            " ActivityItemProductionBaseQuantity = ?," +
                            " ActivityItemProductionWeightQuantity = ? WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    productionTicketFinishedItem.ActivityItemProductionQuantity,
                    productionTicketFinishedItem.ActivityItemProductionBaseQuantity,
                    productionTicketFinishedItem.ActivityItemProductionWeightQuantity,
                    productionTicketFinishedItem.ActivityItemRefno, };

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key reference number, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkItRefno = ((ProductionTicketFinishedItem)resourceType).ActivityItemFKRefno;
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

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
