// <copyright file="ReturnsActItem.cs" company="InterSect Business Systems Inc">
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
using dMobile.Resources.Types.Items;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="ReturnsActItem" />.
    /// </summary>
    public class ReturnsActItem : ActItem
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
        /// Defines the listReturnItems.
        /// </summary>
        private List<ReturnItem> listReturnItems = new List<ReturnItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnsActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public ReturnsActItem(Activity.Activity activity)
          : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Ret;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsRetCreateNewItemNoAsk;

            DefineInitialGrid();

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
                                new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                                new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                                new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                                new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                                new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                            },
                        };

                        var labelIsActivityItemReturnsComment = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelProductUnitBc = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelActivityItemReturnsQuantityReturned = new Label { VerticalOptions = LayoutOptions.Center };

                        labelIsActivityItemReturnsComment.SetBinding(Label.TextProperty, new Binding("IsActivityItemReturnsComment"));
                        labelProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                        labelProductUnitBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                        labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                        labelActivityItemReturnsQuantityReturned.SetBinding(
                            Label.TextProperty,
                            new Binding("ActivityItemReturnsQuantityReturned", BindingMode.Default, null, null, "{0:F3}"));

                        gridData.Children.Add(labelIsActivityItemReturnsComment);
                        gridData.Children.Add(labelProductCode);
                        gridData.Children.Add(labelProductUnitBc);
                        gridData.Children.Add(labelUnitCode);
                        gridData.Children.Add(labelActivityItemReturnsQuantityReturned);

                        Grid.SetColumn(labelIsActivityItemReturnsComment, 0);
                        Grid.SetColumn(labelProductCode, 1);
                        Grid.SetColumn(labelProductUnitBc, 2);
                        Grid.SetColumn(labelUnitCode, 3);
                        Grid.SetColumn(labelActivityItemReturnsQuantityReturned, 4);

                        return new ViewCell { View = gridData };
                    });

                var isAddKey = true;
                foreach (var item in this.ResourceDictionary)
                {
                    if (item.Key == "dataTemplateReturnsItem")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.ResourceDictionary.Add("dataTemplateReturnsItem", dataTemplate);
                }
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
                foreach (var item in this.ListItems.Cast<ReturnItem>())
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
                ////isCatchAsk = this.SysEnv.SysSettings.IsRetCatchAskEnabled;
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
                result = !this.SysEnv.SysSettings.IsReturnsNoCatchAskZeroEnabled;
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
                if (((ReturnItem)item).ActivityItemFKRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        /// <inheritdoc />
        public override DataTemplate GetDataTemplate()
        {
            return (DataTemplate)this.ResourceDictionary["dataTemplateReturnsItem"];
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            var comment = string.Empty;
            if (selectedActivityItem is ReturnItem returnItem)
            {
                var results = await DatabaseConnection.GetInstance().SqlConn.
                    QueryAsync<ActivityItemReturnsCommentModel>(
                        "SELECT * FROM tActivityItemReturnsComment WHERE ActivityItemRefno = ?",
                        returnItem.ActivityItemRefno);

                if (results != null && results.Count > 0)
                {
                    comment = results[0].ActivityItemReturnsCommentComment;
                }
            }

            return comment;
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
            if (this.SelectedItem is ReturnItem item)
            {
                // If the quantity is zero or negative, then the location is receiving.
                if (Math.Sign(item.ActivityItemReturnsQuantityReturned) == 0 ||
                    Math.Sign(item.ActivityItemReturnsQuantityReturned) == -1)
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
            return $"Unable to parse returns item # {actItemStringFkRefno}";
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find returns item # {actItemStringFkRefno}";
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
        public override Task Insert(XDocument data)
        {
            throw new NotImplementedException("Add new return items is not supported.");
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="xmlParam">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument xmlParam)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

                var tableActReturnsItem = new TableActivityReturnsItem();
                var tableActReturnsDetail = new TableActivityReturnsDetail();
                await tableActReturnsDetail.PurgeData();
                await tableActReturnsItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableActReturnsItem.RegisterDataAccessListenerProgressOut(this);
                await tableActReturnsItem.RequestItemData(this.ActHdr.HdrFkRefno, this.ActHdr.HdrRefno);
                tableActReturnsItem.UnregisterDataAccessListenerProgressOut(this);
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
            this.CalcMaxDataColWidth(gridItems, this.ListItems);
            gridMainActivity.Children.Add(gridItems);

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateReturnsItem"];

            return mainActivityItems;
        }

        /// <summary>
        ///     Define the grid layout for the activity item.
        /// </summary>
        /// <returns>New grid layout.</returns>
        public override Grid DefineGrid()
        {
            var labelIsActivityItemReturnsCommentHeader = new Label
                { Text = "C", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelProductCodeHeader = new Label
                { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelProductUnitBcHeader = new Label
                { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitCodeHeader = new Label
                { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelActivityItemReturnsQuantityReturnedHeader = new Label
                { Text = "Returned", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelIsActivityItemReturnsCommentHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCodeHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductUnitBcHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelActivityItemReturnsQuantityReturnedHeader.Text);
            var gridColumns = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) }
                }
            };

            gridColumns.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridColumns.Children.Add(labelIsActivityItemReturnsCommentHeader);
            gridColumns.Children.Add(labelProductCodeHeader);
            gridColumns.Children.Add(labelProductUnitBcHeader);
            gridColumns.Children.Add(labelUnitCodeHeader);
            gridColumns.Children.Add(labelActivityItemReturnsQuantityReturnedHeader);

            Grid.SetColumn(labelIsActivityItemReturnsCommentHeader, 0);
            Grid.SetColumn(labelProductCodeHeader, 1);
            Grid.SetColumn(labelProductUnitBcHeader, 2);
            Grid.SetColumn(labelUnitCodeHeader, 3);
            Grid.SetColumn(labelActivityItemReturnsQuantityReturnedHeader, 4);

            Grid.SetRow(gridColumns, 0);

            return gridColumns;
        }

        /// <summary>
        ///     Create observable collection from list items.
        /// </summary>
        /// <param name="listItems">List of item objects.</param>
        /// <returns>Created observable collection.</returns>
        public override ObservableCollection<object> CreateObservableCollectionFromListItems(List<object> listItems)
        {
            var mainActivityItems = new ObservableCollection<object>();
            foreach (var item in listItems.Cast<ReturnItem>())
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
                foreach (var item in listItems.Cast<ReturnItem>())
                {
                    mainActivityItems.Add(item);
                }
            }
            else
            {
                mainActivityItems = new ObservableCollection<object>();
                var lowerSearchText = filterSearch.ToLower().Trim();
                foreach (var item in listItems.Cast<ReturnItem>())
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
                .Cast<ReturnItem>()
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
            var returnItem = (ReturnItem)item;

            var detailWidths = new[]
            {
                gridService.CalculateTextWidth(returnItem.IsActivityItemReturnsComment),
                gridService.CalculateTextWidth(returnItem.ProductCode),
                gridService.CalculateTextWidth(returnItem.ProductUnitBC),
                gridService.CalculateTextWidth(returnItem.UnitCode),
                gridService.CalculateTextWidth($"{returnItem.ActivityItemReturnsQuantityReturned:F3}")
            };

            var columnWidths = new[]
                { this.columnWidth0, this.columnWidth1, this.columnWidth2, this.columnWidth3, this.columnWidth4 };

            for (var i = 0; i < detailWidths.Length; i++)
            {
                if (detailWidths[i] > columnWidths[i])
                {
                    columnWidths[i] = detailWidths[i];
                    columns[i].Width = new GridLength(columnWidths[i], GridUnitType.Absolute);
                }
            }

            this.columnWidth0 = columnWidths[0];
            this.columnWidth1 = columnWidths[1];
            this.columnWidth2 = columnWidths[2];
            this.columnWidth3 = columnWidths[3];
            this.columnWidth4 = columnWidths[4];
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var parameters = new List<object>();

            string query;
            if (isSubmitBatch)
            {
                query = "SELECT tActivityItemReturns.ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemFKRefno, " +
                "tActivityItemReturns.ActivityItemStringFKRefno, " +
                "tProductUnit.ProductUnitRefno AS ProductUnitRefno, " +
                "tProduct.ProductRefno AS ProductRefno, " +
                "tProduct.ProductCode AS ProductCode, " +
                "tProduct.ProductDescription AS ProductDescription, " +
                "tProduct.ProductBrandDescription AS ProductBrandDescription, " +
                "tUnit.UnitCode AS UnitCode, " +
                "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                "tActivityItemReturns.ActivityItemReturnsQuantityReturned AS ActivityItemReturnsQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned AS ActivityItemReturnsBaseQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned AS ActivityItemReturnsWeightQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode AS ActivityItemReturnsCustomerProductUnitCode, " +
                "tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno, " +
                "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
                "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
                "tActivityItem.ActivityItemStatus AS ActivityItemStatus " +
                "FROM tActivityItemReturns " +
                "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                "tActivityItemReturns.WarehouseLocationMapRefno " +
                "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "WHERE tActivityItem.ActivityRefno = ? " +
                "AND tActivityItem.ActivityItemStatus <> ? " +
                "UNION " +
                "SELECT tActivityItemReturns.ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemFKRefno, " +
                "tActivityItemReturns.ActivityItemStringFKRefno, " +
                "tProductUnit.ProductUnitRefno, " +
                "tProduct.ProductRefno, " +
                "tProduct.ProductCode, " +
                "tProduct.ProductDescription, " +
                "tProduct.ProductBrandDescription, " +
                "tUnit.UnitCode, " +
                "tProductUnit.ProductUnitBC, " +
                "tProductUnit.ProductUnitSizeDescription, " +
                "tActivityItemReturns.ActivityItemReturnsQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode, " +
                "tLocationMapWarehouse.LocationMapRefno, " +
                "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
                "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
                "tActivityItem.ActivityItemStatus " +
                "FROM tActivityItemReturns " +
                "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                "tActivityItemReturns.WarehouseLocationMapRefno " +
                "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "WHERE tActivityItem.ActivityRefno = ? " +
                "AND tActivityDetail.ActivityDetailStatus <> ? " +
                "ORDER BY tActivityItemReturns.ActivityItemRefno";

                parameters.Add(this.ActHdr.HdrRefno);
                parameters.Add(Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                query = "SELECT tActivityItemReturns.ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemFKRefno, " +
                "tActivityItemReturns.ActivityItemStringFKRefno, " +
                "tProductUnit.ProductUnitRefno AS ProductUnitRefno, " +
                "tProduct.ProductRefno AS ProductRefno, " +
                "tProduct.ProductCode AS ProductCode, " +
                "tProduct.ProductDescription AS ProductDescription, " +
                "tProduct.ProductBrandDescription AS ProductBrandDescription, " +
                "tUnit.UnitCode AS UnitCode, " +
                "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                "tActivityItemReturns.ActivityItemReturnsQuantityReturned AS ActivityItemReturnsQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned AS ActivityItemReturnsBaseQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned AS ActivityItemReturnsWeightQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode AS ActivityItemReturnsCustomerProductUnitCode, " +
                "tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno, " +
                "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
                "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
                "tActivityItem.ActivityItemStatus AS ActivityItemStatus " +
                "FROM tActivityItemReturns " +
                "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                "tActivityItemReturns.WarehouseLocationMapRefno " +
                "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "WHERE tActivityItem.ActivityRefno = ? " +
                "ORDER BY tActivityItemReturns.ActivityItemRefno";
                parameters.Add(this.ActHdr.HdrRefno);
            }

            // using (SQLiteDataAdapter dataAdapterReturnItems = new SQLiteDataAdapter(cmd))
            // {
            //    using (DataSet dataSetReturnItems = new DataSet())
            //    {
            //        dataAdapterReturnItems.Fill(dataSetReturnItems);
            //        this.DataViewItems = dataSetReturnItems.Tables[0].DefaultView;
            //        this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //        this.DataViewItems.AllowNew = true;
            //        this.DataViewItems.AllowEdit = true;
            //        this.DataViewItems.AllowDelete = true;
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //    }
            // }
            this.listReturnItems = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ReturnItem>(query, parameters.ToArray());
            this.ListItems = this.listReturnItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <inheritdoc cref="ActivityItem.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            // if (string.IsNullOrEmpty(this.ActItemFilter))
            // {
            this.ListItems = this.listReturnItems.Where(returnItem => returnItem.ProductCode.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'" + searchText.Trim() + "%\'";
            // }
            foreach (var returnItem in this.ListItems.Cast<ReturnItem>())
            {
                observableCollection.Add(returnItem);
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
            else if (selectedActivityItem is ReturnItem item)
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
            async Task AssignItemLocation(ReturnItem item)
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
            var whsLocMapRefno = locMap.WhsLocMapRefno;
            var query = "SELECT tActivityItem.ActivityItemRefno, " +
                        "tLocationMapWarehouse.LocationMapRefno " +
                        "FROM tActivityItemReturns " +
                        "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                        "tActivityItemReturns.ActivityItemRefno " +
                        "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                        "tActivityItemReturns.WarehouseLocationMapRefno " +
                        "WHERE tActivityItem.ActivityRefno = ? " +
                        "AND tActivityItemReturns.WarehouseLocationMapRefno = ? " +
                        "AND tActivityItemReturns.ProductUnitRefno = ?";

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ReturnItem>(
                query, this.ActHdr.HdrRefno, whsLocMapRefno, prodUnitRefno);
            if (results != null && results.Count > 0)
            {
                foundActItemRefno = this.ItemRefno;
            }
            else
            {
                foreach (var item in this.listReturnItems)
                {
                    if (item.LocationMapRefno == locMap.WhsLocMapRefno && item.ProductUnitRefno == prodUnitRefno)
                    {
                        foundActItemRefno = this.ItemRefno;
                        break;
                    }
                }

                // If location defaulting is on, attempt to find a matching line item by ignoring the line item
                // warehouse location. Yes this will be a problem if the sales order has multiple item lines
                // on it for the same product and different warehouse.
                // Picking is the only activity that could have this sort of problem. All other activities are
                // warehouse friend because they can't mix warehouses or are created by the mobile itself.
                if (foundActItemRefno == 0 && this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    query = $@"
                        SELECT tActivityItem.ActivityItemRefno,
                                tLocationMapWarehouse.LocationMapRefno 
                            FROM tActivityItemReturns 
                            INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemReturns.ActivityItemRefno 
                            INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemReturns.WarehouseLocationMapRefno 
                            WHERE tActivityItem.ActivityRefno = {this.ActHdr.HdrRefno} 
                            AND tActivityItemPicking.ProductUnitRefno = {prodUnitRefno}";

                    results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ReturnItem>(query);
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
                            foundActItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                        }
                    }
                    else
                    {
                        foreach (var item in this.ListItems)
                        {
                            if (item is ReturnItem pickItem)
                            {
                                if (pickItem.ProductUnitRefno == prodUnitRefno)
                                {
                                    foundActItemRefno = this.ItemRefno;
                                    break;
                                }
                            }
                        }
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
        public override Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var deviceId = this.SysEnv.DeviceId;
            var revision = this.SysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            var loginId = this.SysEnv.ContactLog.LoginID.ToString();
            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var barCodeSymbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
            var current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT SALES RETURN ITEM</action>"
                + "</command>"
                + "<device_id>" + deviceId + "</device_id>"
                + "<login_id>" + loginId + "</login_id>"
                + "<revision_datetime>" + revision + "</revision_datetime>"
                + "<stamp>" + current + "</stamp>"
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBc.ToString() + "</is_bc>"
                + "<symbology>" + barCodeSymbologyPrefix + "</symbology>"
                + "<gtin>" + prodUnitBarCodeCode + "</gtin>"
                + "<whs_id>" + this.LocMap.WhsFkRefno.ToString() + "</whs_id>"
                + "<sect_refno>" + this.LocMap.SectFkRefno.ToString() + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.SlotFkRefno.ToString() + "</slot_refno>"
                + "<refno></refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return Task.Run(() => xmlString.ToString());
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
            if (selectedActivityItem is ReturnItem returnItem)
            {
                returnItem.ActivityItemReturnsQuantityReturned = quantity;
                returnItem.ActivityItemReturnsBaseQuantityReturned = baseQuantity;
                returnItem.ActivityItemReturnsWeightQuantityReturned = weightQuantity;
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
            if (selectedActivityItem is ReturnItem returnItem)
            {
                returnItem.ActivityItemReturnsQuantityReturned += quantity;
                returnItem.ActivityItemReturnsBaseQuantityReturned += baseQuantity;
                returnItem.ActivityItemReturnsWeightQuantityReturned += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is ReturnItem returnItem)
            {
                var query = "UPDATE tActivityItemReturns " +
                            "SET ActivityItemReturnsQuantityReturned = ?, " +
                            "ActivityItemReturnsBaseQuantityReturned = ?, " +
                            "ActivityItemReturnsWeightQuantityReturned = ? " +
                            "WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    returnItem.ActivityItemReturnsQuantityReturned,
                    returnItem.ActivityItemReturnsBaseQuantityReturned,
                    returnItem.ActivityItemReturnsWeightQuantityReturned,
                    returnItem.ActivityItemRefno,
                };

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
            object tmpFkItRefno = ((ReturnItem)resourceType).ActivityItemFKRefno;
            return tmpFkItRefno == null ? 0 : Convert.ToInt32(tmpFkItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"Return # {this.ActHdr.HdrFkRefno}";
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
                    foreach (dynamic pickingItem in results)
                    {
                        // Because an agregate sum function is used in a query. There will always be a result record.
                        // This causes problems detecting if there is nothing to sum, resulting in null.
                        // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
                        msg +=
                            $"Total ordered:\r{Convert.ToInt32(Convert.ToDecimal(pickingItem.Ordered))} @ {Convert.ToDecimal(pickingItem.WeightOrdered)}{Convert.ToString(pickingItem.WtUnitCode)}\nTotal shipped:\r{Convert.ToInt32(Convert.ToDecimal(pickingItem.Shipped))} @ {Convert.ToDecimal(pickingItem.WeightShipped)}{Convert.ToString(pickingItem.WtUnitCode)}";
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new BaseException("Database exception occurred.", ex);
            }

            return msg;
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
        public override string GetOrderedExpectedOnHandTitle()
        {
            return "Ordered";
        }

        /// <inheritdoc />
        public override string GetShippedReceivedCountedTitle()
        {
            return "Shipped";
        }

        /// <inheritdoc />
        public override string GetOrderedExpectedOnHandQuantity()
        {
            const string quantity = "0.000";
            return quantity;
        }

        /// <inheritdoc />
        public override string GetShippedReceivedCountedQuantity()
        {
            var quantity = "0.000";
            if (this.SelectedItem is ReturnItem item)
            {
                quantity = $"{item.ActivityItemReturnsQuantityReturned:F3} {this.ProdInfo.UnitCode} @ " +
                           $"{item.ActivityItemReturnsWeightQuantityReturned:F3} {this.ProdInfo.WeightUnitCode}";
            }

            return quantity;
        }
    }
}
