// <copyright file="CutSheetFinishedActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Items;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="CutSheetFinishedActItem" />.
    /// </summary>
    public class CutSheetFinishedActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CutSheetFinishedActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public CutSheetFinishedActItem(Activity.Activity activity)
          : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2CutSheetFinished;
            ////this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsCutSheetFinishedCreateNewItemNoAsk;
            this.IsAskCreateNewItem = false;
            DefineInitialGrid();

            void DefineInitialGrid()
            {
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
                    labelQty.SetBinding(Label.TextProperty, new Binding("ActivityItemCutSheetBaseQuantity", BindingMode.Default, null, null, "{0:F3}"));
                    labelExpQty.SetBinding(Label.TextProperty, new Binding("ActivityItemCutSheetBaseQuantityExpected", BindingMode.Default, null, null, "{0:F3}"));

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
                foreach (var item in this.ResourceDictionary)
                {
                    if (item.Key == "dataTemplateRawFinishedItem")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.ResourceDictionary.Add("dataTemplateRawFinishedItem", dataTemplate);
                }
            }
        }

        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double ColumnWidth0 { get; set; }

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double ColumnWidth1 { get; set; }

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double ColumnWidth2 { get; set; }

        /// <summary>
        /// Defines the columnWidth3.
        /// </summary>
        private double ColumnWidth3 { get; set; }

        /// <summary>
        /// Defines the columnWidth4.
        /// </summary>
        private double ColumnWidth4 { get; set; }

        /// <summary>
        /// Defines the listCutSheetFinishedItems.
        /// </summary>
        private List<CutSheetFinishedItem> ListCutSheetFinishedItems { get; set; } =
            new List<CutSheetFinishedItem>();

        /// <inheritdoc/>
        public override Color GetShippedReceivedCountedDisplayTextColor()
        {
            var displayColor = base.GetShippedReceivedCountedDisplayTextColor();
            if (this.SelectedItem is CutSheetFinishedItem item)
            {
                displayColor = item.ActivityItemCutSheetQuantityExpected == item.ActivityItemCutSheetQuantity
                    ? Color.Green
                    : Color.Red;
            }

            return displayColor;
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
                foreach (var item in this.ListItems.Cast<CutSheetFinishedItem>())
                {
                    if (item.ActivityItemRefno == actItemFoundRefno)
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
                ////isCatchAsk = this.SysEnv.SysSettings.IsCutSheetFinishedCatchAskEnabled;
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
                result = !this.SysEnv.SysSettings.IsCutSheetFinishedNoCatchAskZeroEnabled;
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
                if (((CutSheetFinishedActItem)item).ItemFkRefno == activityItemFkRefno)
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
            if (this.SelectedItem is CutSheetFinishedItem item)
            {
                if (Math.Sign(item.ActivityItemCutSheetQuantity) == 0 ||
                    Math.Sign(item.ActivityItemCutSheetQuantity) == 1)
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
            return;

            //////////////////////////////////////////////////////////////////////////
            async Task InsertActivityItemInTables(int prodUnitRefno)
            {
                await Conn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        transaction.Execute(
                            "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                            Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished),
                            Convert.ToInt32(SysCommon.ActStatus.Inserted),
                            Convert.ToInt32(this.ActHdr.HdrRefno),
                            1);

                        this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT last_insert_rowid() FROM tActivityItem"));

                        var queryItemCutSheet = "INSERT INTO tActivityItemCutSheet (" +
                                                  "ActivityItemRefno, " +
                                                  "ActivityItemSubType, " +
                                                  "ProductUnitRefno, " +
                                                  "LocationMapRefno, " +
                                                  "ActivityItemCutSheetQuantity, " +
                                                  "ActivityItemCutSheetBaseQuantity, " +
                                                  "ActivityItemCutSheetWeightQuantity, " +
                                                  "ActivityItemCutSheetDescription, " +
                                                  "ActivityItemCutSheetQuantityExpected, " +
                                                  "ActivityItemCutSheetBaseQuantityExpected, " +
                                                  "ActivityItemCutSheetWeightQuantityExpected" +
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
                        var parametersItemCutSheet = new object[]
                        {
                            Convert.ToInt32(this.ItemRefno),
                            Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished),
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

                        transaction.Execute(queryItemCutSheet, parametersItemCutSheet);

                        // Get temporary item FK refno for newly inserted line item.
                        this.ItemFkRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                            "SELECT MAX(tActivityItemCutSheetFinished.ActivityItemFKRefno)" +
                            $" FROM tActivityItemCutSheetFinished"));
                        this.ItemFkRefno = this.ItemFkRefno + 1;

                        transaction.Execute(
                            "INSERT INTO tActivityItemCutSheetFinished (ActivityItemRefno, ActivityItemFKRefno," +
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
                var item = new CutSheetFinishedItem()
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
                    ActivityItemCutSheetQuantity = 0,
                    ActivityItemCutSheetBaseQuantity = 0,
                    ActivityItemCutSheetWeightQuantity = 0,
                    ActivityItemCutSheetQuantityExpected = 0,
                    ActivityItemCutSheetBaseQuantityExpected = 0,
                    ActivityItemCutSheetWeightQuantityExpected = 0,
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

                var tableActTicketMatItem = new TableActivityCutSheetFinishedItem();
                var tableActTicketMatDetail = new TableActivityCutSheetFinishedDetail();
                await tableActTicketMatDetail.PurgeData();
                await tableActTicketMatItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableActTicketMatItem.RegisterDataAccessListenerProgressOut(this);
                await tableActTicketMatItem.RequestItemData(this.ActHdr.HdrFkRefno, this.ActHdr.HdrRefno);
                tableActTicketMatItem.UnregisterDataAccessListenerProgressOut(this);
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
            this.PlaceNewItemsAtBottomOfList(this.ListItems);
            var mainActivityItems = this.CreateObservableCollectionFromListItems(this.ListItems);
            var gridItems = this.DefineGrid();
            this.CalcMaxDataColWidth(gridItems, mainActivityItems.ToList());
            gridMainActivity.Children.Add(gridItems);

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateRawFinishedItem"];

            return mainActivityItems;
        }

        /// <summary>
        ///     Create observable collection from list items.
        /// </summary>
        /// <param name="listItems">List of item objects.</param>
        /// <returns>Created observable collection.</returns>
        public override ObservableCollection<object> CreateObservableCollectionFromListItems(List<object> listItems)
        {
            var mainActivityItems = new ObservableCollection<object>();
            foreach (var item in listItems.Cast<CutSheetFinishedItem>())
            {
                mainActivityItems.Add(item);
            }

            return mainActivityItems;
        }

        /// <summary>
        ///     Create observable collection from list items.
        /// </summary>
        /// <param name="listItems">List of item objects.</param>
        /// <param name="filterSearch">Search filter text.</param>
        /// <returns>Created observable collection.</returns>
        public override ObservableCollection<object> CreateObservableCollectionFromListItems(
            List<object> listItems,
            string filterSearch)
        {
            // This code is optimized, it iterates over this.ListItems only once.
            // It separates the list items into two lists, one for positive refno and one for negative refno.
            // It then adds the positive refno items first, then the negative refno items.
            // This is done to ensure that the negative refno items "new items" are displayed at the bottom of the list.
            var mainActivityItems = new ObservableCollection<object>();
            if (string.IsNullOrEmpty(filterSearch) || string.IsNullOrWhiteSpace(filterSearch))
            {
                foreach (var item in listItems.Cast<CutSheetFinishedItem>())
                {
                    mainActivityItems.Add(item);
                }
            }
            else
            {
                mainActivityItems = new ObservableCollection<object>();
                var lowerSearchText = filterSearch.ToLower().Trim();
                foreach (var item in listItems.Cast<CutSheetFinishedItem>())
                {
                    if (item.ProductCode.ToLower().Contains(lowerSearchText))
                    {
                        mainActivityItems.Add(item);
                    }
                }

                this.ListItems = mainActivityItems.ToList();
            }

            return mainActivityItems;
        }

        /// <summary>
        ///     Sort the list items and place new items at the bottom of the list.
        /// </summary>
        /// <param name="listItems">List of items to sort.</param>
        /// <remarks>
        /// New items with negative foreign key item reference numbers will be placed at the bottom of
        /// the new collection. The list items will not be sorted to match the new collection.
        /// </remarks>
        public override void PlaceNewItemsAtBottomOfList(List<object> listItems)
        {
            var sortedItems = listItems
                .Cast<CutSheetFinishedItem>()
                .OrderBy(item => item.ActivityItemFKRefno >= 0 ? 0 : 1)
                .ThenBy(item => item.ActivityItemFKRefno)
                .Cast<object>()
                .ToList();

            listItems.Clear();
            listItems.AddRange(sortedItems);
        }

        /// <summary>
        ///     Calculate the maximum data column widths.
        /// </summary>
        /// <param name="definition">Grid layout definition.</param>
        /// <param name="itemsToMeasure">List of data items to measure in order to determine the max column width.</param>
        public override void CalcMaxDataColWidth(Grid definition, List<object> itemsToMeasure)
        {
            var gridService = DependencyService.Get<IGridService>();
            var columns = definition.ColumnDefinitions;
            foreach (var item in itemsToMeasure)
            {
                this.CalcMaxDataColWidth(item, gridService, columns);
            }
        }

        /// <summary>
        ///    Calculate the maximum data column widths for one item.
        /// </summary>
        /// <param name="item">Item to compare to the max width.</param>
        /// <param name="gridService">Grid service.</param>
        /// <param name="columns">Column definitions to compare.</param>
        public override void CalcMaxDataColWidth(
            object item,
            IGridService gridService,
            ColumnDefinitionCollection columns)
        {
            var materialItem = (CutSheetFinishedItem)item;

            var detailWidths = new[]
            {
                gridService.CalculateTextWidth(Convert.ToString(materialItem.ProductCode)),
                gridService.CalculateTextWidth(materialItem.ProductUnitBC),
                gridService.CalculateTextWidth(materialItem.BaseUnitCode),
                gridService.CalculateTextWidth($"{materialItem.ActivityItemCutSheetBaseQuantity:F3}"),
                gridService.CalculateTextWidth($"{materialItem.ActivityItemCutSheetBaseQuantityExpected:F3}")
            };

            var columnWidths = new[] { this.ColumnWidth0, this.ColumnWidth1, this.ColumnWidth2, this.ColumnWidth3, this.ColumnWidth4 };

            for (int i = 0; i < detailWidths.Length; i++)
            {
                if (detailWidths[i] > columnWidths[i])
                {
                    columnWidths[i] = detailWidths[i];
                    columns[i].Width = new GridLength(columnWidths[i], GridUnitType.Absolute);
                }
            }

            this.ColumnWidth0 = columnWidths[0];
            this.ColumnWidth1 = columnWidths[1];
            this.ColumnWidth2 = columnWidths[2];
            this.ColumnWidth3 = columnWidths[3];
            this.ColumnWidth4 = columnWidths[4];
        }

        /// <summary>
        ///     Define the grid layout for the activity item.
        /// </summary>
        /// <returns>New grid layout.</returns>
        public override Grid DefineGrid()
        {
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
            return gridHeader;
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>List of activity items.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            // Use StringBuilder pooling and parameterized queries for better performance and security
            // Also, avoid ToString() on StringBuilder until needed
            string sql;
            object[] parameters;

            int hdrRefno = this.ActHdr.HdrRefno;
            int actStatusNoChange = Convert.ToInt32(SysCommon.ActStatus.NoChange);
            int actSubTypeCutSheetFinished = Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished);

            if (isSubmitBatch)
            {
                sql = @"
                    SELECT tActivityItemCutSheet.ActivityItemRefno,
                           tActivityItemCutSheetFinished.ActivityItemFKRefno,
                           tActivityItemCutSheetFinished.ActivityItemStringFKRefno,
                           tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                           tProduct.ProductRefno AS ProductRefno,
                           tProduct.ProductCode AS ProductCode,
                           tProduct.ProductDescription AS ProductDescription,
                           tUnit.UnitCode AS BaseUnitCode,
                           tProductUnit.ProductUnitBC AS ProductUnitBC,
                           tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                           tActivityItemCutSheet.ActivityItemCutSheetQuantity AS ActivityItemCutSheetQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetBaseQuantity AS ActivityItemCutSheetBaseQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetWeightQuantity AS ActivityItemCutSheetWeightQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetQuantityExpected AS ActivityItemCutSheetQuantityExpected,
                           tActivityItemCutSheet.ActivityItemCutSheetBaseQuantityExpected AS ActivityItemCutSheetBaseQuantityExpected,
                           tActivityItemCutSheet.ActivityItemCutSheetWeightQuantityExpected AS ActivityItemCutSheetWeightQuantityExpected,
                           tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           tActivityItem.ActivityItemStatus AS ActivityItemStatus
                    FROM tActivityItemCutSheet
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemCutSheet.ActivityItemRefno
                    INNER JOIN tActivityItemCutSheetFinished ON tActivityItemCutSheetFinished.ActivityItemRefno = tActivityItemCutSheet.ActivityItemRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCutSheet.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemCutSheet.LocationMapRefno
                    WHERE tActivityItem.ActivityRefno = ?
                      AND tActivityItem.ActivityItemStatus <> ?
                      AND tActivityItem.ActivityItemSubType = ?
                    UNION
                    SELECT tActivityItemCutSheet.ActivityItemRefno,
                           tActivityItemCutSheetFinished.ActivityItemFKRefno,
                           tActivityItemCutSheetFinished.ActivityItemStringFKRefno,
                           tProductUnit.ProductUnitRefno,
                           tProduct.ProductRefno,
                           tProduct.ProductCode,
                           tProduct.ProductDescription,
                           tUnit.UnitCode AS BaseUnitCode,
                           tProductUnit.ProductUnitBC,
                           tProductUnit.ProductUnitSizeDescription,
                           tActivityItemCutSheet.ActivityItemCutSheetQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetBaseQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetWeightQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetQuantityExpected,
                           tActivityItemCutSheet.ActivityItemCutSheetBaseQuantityExpected,
                           tActivityItemCutSheet.ActivityItemCutSheetWeightQuantityExpected,
                           tLocationMapWarehouse.LocationMapRefno,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           tActivityItem.ActivityItemStatus
                    FROM tActivityItemCutSheet
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemCutSheet.ActivityItemRefno
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItemCutSheet.ActivityItemRefno
                    INNER JOIN tActivityItemCutSheetFinished ON tActivityItemCutSheetFinished.ActivityItemRefno = tActivityItemCutSheet.ActivityItemRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCutSheet.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemCutSheet.LocationMapRefno
                    WHERE tActivityItem.ActivityRefno = ?
                      AND tActivityDetail.ActivityDetailStatus <> ?
                      AND tActivityDetail.ActivityDetailSubType = ?
                    ORDER BY tActivityItemCutSheet.ActivityItemRefno";
                parameters = new object[]
                {
                    hdrRefno, actStatusNoChange, actSubTypeCutSheetFinished,
                    hdrRefno, actStatusNoChange, actSubTypeCutSheetFinished
                };
            }
            else
            {
                sql = @"
                    SELECT tActivityItemCutSheet.ActivityItemRefno,
                           tActivityItemCutSheetFinished.ActivityItemFKRefno,
                           tActivityItemCutSheetFinished.ActivityItemStringFKRefno,
                           tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                           tProduct.ProductRefno AS ProductRefno,
                           tProduct.ProductCode AS ProductCode,
                           tProduct.ProductDescription AS ProductDescription,
                           tUnit.UnitCode AS BaseUnitCode,
                           tProductUnit.ProductUnitBC AS ProductUnitBC,
                           tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                           tActivityItemCutSheet.ActivityItemCutSheetQuantity AS ActivityItemCutSheetQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetBaseQuantity AS ActivityItemCutSheetBaseQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetWeightQuantity AS ActivityItemCutSheetWeightQuantity,
                           tActivityItemCutSheet.ActivityItemCutSheetQuantityExpected AS ActivityItemCutSheetQuantityExpected,
                           tActivityItemCutSheet.ActivityItemCutSheetBaseQuantityExpected AS ActivityItemCutSheetBaseQuantityExpected,
                           tActivityItemCutSheet.ActivityItemCutSheetWeightQuantityExpected AS ActivityItemCutSheetWeightQuantityExpected,
                           tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           tActivityItem.ActivityItemStatus AS ActivityItemStatus
                    FROM tActivityItemCutSheet
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemCutSheet.ActivityItemRefno
                    INNER JOIN tActivityItemCutSheetFinished ON tActivityItemCutSheetFinished.ActivityItemRefno = tActivityItemCutSheet.ActivityItemRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCutSheet.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemCutSheet.LocationMapRefno
                    WHERE tActivityItem.ActivityRefno = ?
                      AND tActivityItem.ActivityItemSubType = ?
                    ORDER BY tActivityItemCutSheet.ActivityItemRefno";
                parameters = new object[]
                {
                    hdrRefno,
                    actSubTypeCutSheetFinished
                };
            }

            // Use parameterized query to avoid SQL injection and improve SQLite query plan caching
            this.ListCutSheetFinishedItems = await Conn.QueryAsync<CutSheetFinishedItem>(sql, parameters);
            this.ListItems = this.ListCutSheetFinishedItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <inheritdoc cref="ActivityItem.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListItems = this.ListCutSheetFinishedItems.Where(
                productionCutSheetFinishedItem => productionCutSheetFinishedItem.ProductCode.ToString().ToLower().
                Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            // Add all items in dataview to observable collection for display
            foreach (var productionCutSheetFinishedItem in this.ListItems.Cast<CutSheetFinishedItem>())
            {
                observableCollection.Add(productionCutSheetFinishedItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// Select an activity item.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity object.</param>
        public override async Task AssignActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem == null)
            {
                this.SelectedListIdx = -1;
                this.SelectedItem = null;
                this.ItemRefno = 0;
                this.ItemFkRefno = 0;
                this.ProdInfo = new ProductInfo
                {
                    Scheme = new ProdScheme()
                };
            }
            else if (selectedActivityItem is CutSheetFinishedItem item)
            {
                this.SelectedListIdx = this.ListItems.IndexOf(item);
                this.SelectedItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFkRefno = item.ActivityItemFKRefno;
                this.ActStatus = (SysCommon.ActStatus)item.ActivityItemStatus;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);

                await AssignItemLocation(item);

                // LocationMap.GetInstance().CurrLocMap warehouse needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task AssignItemLocation(CutSheetFinishedItem item)
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
            sql.Append("SELECT tActivityItem.ActivityItemRefno, ");
            sql.Append("tActivityItemCutSheetFinished.ActivityItemFKRefno, ");
            sql.Append("tActivityItemCutSheet.ProductUnitRefno, ");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemCutSheet ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemCutSheet.ActivityItemRefno ");
            sql.Append("INNER JOIN tActivityItemCutSheetFinished ON tActivityItemCutSheetFinished.ActivityItemRefno ");
            sql.Append("= tActivityItemCutSheet.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemCutSheet.LocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
            sql.AppendFormat("AND tActivityItemCutSheet.LocationMapRefno = {0} ", locMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemCutSheet.ProductUnitRefno = {0} ", prodUnitRefno);
            sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished));

            var results = await Conn.QueryAsync<CutSheetFinishedItem>(sql.ToString());
            if (results != null && results.Count > 0)
            {
                foundActItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
            }
            else
            {
                foreach (var item in this.ListCutSheetFinishedItems)
                {
                    if (item.LocationMapRefno == locMap.WhsLocMapRefno && item.ProductUnitRefno == prodUnitRefno)
                    {
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
            var barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var baseUnitCode = data.Root.Element("BaseUnitCode").Value;
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRODUCTION TICKET MATERIAL ITEM</action>"
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
            if (selectedActivityItem is CutSheetFinishedItem productionCutSheetFinishedItem)
            {
                productionCutSheetFinishedItem.ActivityItemCutSheetQuantity = quantity;
                productionCutSheetFinishedItem.ActivityItemCutSheetBaseQuantity = baseQuantity;
                productionCutSheetFinishedItem.ActivityItemCutSheetWeightQuantity = weightQuantity;
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
            if (selectedActivityItem is CutSheetFinishedItem productionCutSheetFinishedItem)
            {
                productionCutSheetFinishedItem.ActivityItemCutSheetQuantity += quantity;
                productionCutSheetFinishedItem.ActivityItemCutSheetBaseQuantity += baseQuantity;
                productionCutSheetFinishedItem.ActivityItemCutSheetWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is CutSheetFinishedItem productionCutSheetFinishedItem)
            {
                var query = "UPDATE tActivityItemCutSheet SET ActivityItemCutSheetQuantity = ?," +
                            " ActivityItemCutSheetBaseQuantity = ?, ActivityItemCutSheetWeightQuantity = ?" +
                            " WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    productionCutSheetFinishedItem.ActivityItemCutSheetQuantity,
                    productionCutSheetFinishedItem.ActivityItemCutSheetBaseQuantity,
                    productionCutSheetFinishedItem.ActivityItemCutSheetWeightQuantity,
                    productionCutSheetFinishedItem.ActivityItemRefno, };

                await Conn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkItRefno = ((CutSheetFinishedItem)resourceType).ActivityItemFKRefno;
            return tmpFkItRefno == null ? 0 : Convert.ToInt32(tmpFkItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"CutSheet ticket # {this.ActHdr.HdrFkRefno}";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override Task<int> Delete()
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
            return Task.FromResult(string.Empty);
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

        /// <inheritdoc />
        public override DataTemplate GetDataTemplate()
        {
            return (DataTemplate)this.ResourceDictionary["dataTemplateRawFinishedItem"];
        }

        /// <inheritdoc />
        public override string GetOrderedExpectedOnHandTitle()
        {
            return "Expected";
        }

        /// <inheritdoc />
        public override string GetShippedReceivedCountedTitle()
        {
            return "Finished";
        }

        /// <inheritdoc />
        public override string GetOrderedExpectedOnHandQuantity()
        {
            var quantity = "0.000";
            if (this.SelectedItem is CutSheetFinishedItem item)
            {
                quantity = $"{item.ActivityItemCutSheetQuantityExpected:F3} {this.ProdInfo.UnitCode} @ " +
                           $"{item.ActivityItemCutSheetWeightQuantityExpected:F3} {this.ProdInfo.WeightUnitCode}";
            }

            return quantity;
        }

        /// <inheritdoc />
        public override string GetShippedReceivedCountedQuantity()
        {
            var quantity = "0.000";
            if (this.SelectedItem is CutSheetFinishedItem item)
            {
                quantity = $"{item.ActivityItemCutSheetQuantity:F3} {this.ProdInfo.UnitCode} @ " +
                           $"{item.ActivityItemCutSheetWeightQuantity:F3} {this.ProdInfo.WeightUnitCode}";
            }

            return quantity;
        }
    }
}
