// <copyright file="YieldSheetMaterialActItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types.Items;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="YieldSheetMaterialActItem" />.
    /// </summary>
    public class YieldSheetMaterialActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YieldSheetMaterialActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public YieldSheetMaterialActItem(Activity.Activity activity)
          : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2YieldSheetMaterial;
            ////this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsYieldSheetMaterialCreateNewItemNoAsk;
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
                    labelQty.SetBinding(
                        Label.TextProperty,
                        new Binding("ActivityItemYieldSheetBaseQuantity", BindingMode.Default, null, null, "{0:F3}"));
                    labelExpQty.SetBinding(
                        Label.TextProperty,
                        new Binding("ActivityItemYieldSheetBaseQuantityExpected", BindingMode.Default, null, null, "{0:F3}"));

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
                    if (item.Key == "dataTemplateFinishedItem")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.ResourceDictionary.Add("dataTemplateFinishedItem", dataTemplate);
                }
            }
        }

        private double ColumnWidth0 { get; set; }

        /// <summary>
        /// Gets or sets the ColumnWidth1.
        /// </summary>
        private double ColumnWidth1 { get; set; }

        /// <summary>
        /// Gets or sets the ColumnWidth2.
        /// </summary>
        private double ColumnWidth2 { get; set; }

        /// <summary>
        /// Gets or sets the ColumnWidth3.
        /// </summary>
        private double ColumnWidth3 { get; set; }

        /// <summary>
        /// Gets or sets the ColumnWidth4.
        /// </summary>
        private double ColumnWidth4 { get; set; }

        /// <summary>
        /// Gets or sets the list of cut sheet finished items.
        /// </summary>
        private List<YieldSheetMaterialItem> ListYieldSheetMaterialItems { get; set; } = new List<YieldSheetMaterialItem>();

        /// <inheritdoc/>
        public override Color GetShippedReceivedCountedDisplayTextColor()
        {
            var displayColor = base.GetShippedReceivedCountedDisplayTextColor();
            if (this.SelectedItem is YieldSheetMaterialItem item)
            {
                displayColor = item.ActivityItemYieldSheetQuantityExpected == item.ActivityItemYieldSheetQuantity
                    ? Color.Green
                    : Color.Red;
            }

            return displayColor;
        }

        /// <inheritdoc/>
        public override void SetSelectedActivityItemFromList(int actItemFoundRefno)
        {
            this.SelectedListIdx = -1;
            if (actItemFoundRefno >= 0)
            {
                var idx = 0;
                foreach (var item in this.ListItems.Cast<YieldSheetMaterialItem>())
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

        /// <inheritdoc/>
        public override bool IsAllowAddSkidDetail()
        {
            return false;
        }

        /// <inheritdoc/>
        public override bool IsCatchAsk()
        {
            var isCatchAsk = this.ProdInfo.IsCatchAsk;
            if (isCatchAsk)
            {
                // modules are for exclusions
                ////isCatchAsk = this.SysEnv.SysSettings.IsYieldSheetMaterialCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <inheritdoc/>
        public override bool IsCatchAskZero()
        {
            var result = false;
            if (this.IsCatchAsk() && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsYieldSheetMaterialNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <inheritdoc/>
        public override int FindActivityItemIndex(int activityItemFkRefno)
        {
            var foundIdx = -1;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in this.ListItems)
            {
                foundIdx++;
                if (((YieldSheetMaterialActItem)item).ItemFkRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        /// <inheritdoc/>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is YieldSheetMaterialItem item)
            {
                if (Math.Sign(item.ActivityItemYieldSheetQuantity) == 0 ||
                    Math.Sign(item.ActivityItemYieldSheetQuantity) == 1)
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

        /// <inheritdoc/>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse cut sheet item# {actItemStringFkRefno}";
        }

        /// <inheritdoc/>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find cut sheet item# {actItemStringFkRefno}";
        }

        /// <inheritdoc/>
        public override string GetDataViewSortFkRefnoIdx()
        {
            return "ActivityItemStringFKRefno";
        }

        /// <inheritdoc/>
        public override async Task Insert(XDocument data)
        {
            var prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            await InsertActivityItemInTables(prodUnitRefno);
            AddActivityItemToList(prodUnitRefno);
            return;

            async Task InsertActivityItemInTables(int prodUnitRefno)
            {
                await Conn.RunInTransactionAsync(db =>
                {
                    const int subType = (int)SysCommon.ActSubTypes.YieldSheetMaterial;
                    const int status = (int)SysCommon.ActStatus.Inserted;
                    var hdr = this.ActHdr.HdrRefno;
                    var whsRef = LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno;
                    var desc = (data.Root.Element("ProductDescription")?.Value ?? string.Empty).TrimEnd() + " " +
                        (data.Root.Element("ProductUnitSizeDescription")?.Value ?? string.Empty);
                    db.Execute(
                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                        " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                        subType, status, hdr, 1);

                    this.ItemRefno = (int)db.ExecuteScalar<long>("SELECT last_insert_rowid()");

                    db.Execute(
                        "INSERT INTO tActivityItemYieldSheet (ActivityItemRefno, ActivityItemSubType, ProductUnitRefno," +
                        " LocationMapRefno, ActivityItemYieldSheetQuantity, ActivityItemYieldSheetBaseQuantity," +
                        " ActivityItemYieldSheetWeightQuantity, ActivityItemYieldSheetDescription," +
                        " ActivityItemYieldSheetQuantityExpected, ActivityItemYieldSheetBaseQuantityExpected," +
                        " ActivityItemYieldSheetWeightQuantityExpected) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                        this.ItemRefno, subType, prodUnitRefno, whsRef, 0m, 0m, 0m, desc, 0m, 0m, 0m);

                    var maxFk = (int)db.ExecuteScalar<long>(
                        "SELECT IFNULL(MAX(ActivityItemFKRefno), 0) FROM tActivityItemYieldSheetMaterial");
                    this.ItemFkRefno = maxFk + 1;

                    db.Execute(
                        "INSERT INTO tActivityItemYieldSheetMaterial (ActivityItemRefno, ActivityItemFKRefno," +
                        " ActivityItemStringFKRefno) VALUES (?, ?, ?)",
                        this.ItemRefno, this.ItemFkRefno, this.ItemFkRefno.ToString());
                });
            }

            void AddActivityItemToList(int prodUnitRefno)
            {
                var locMap = LocationMap.GetInstance().CurrLocMap;
                var item = new YieldSheetMaterialItem()
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
                    ActivityItemYieldSheetQuantity = 0,
                    ActivityItemYieldSheetBaseQuantity = 0,
                    ActivityItemYieldSheetWeightQuantity = 0,
                    ActivityItemYieldSheetQuantityExpected = 0,
                    ActivityItemYieldSheetBaseQuantityExpected = 0,
                    ActivityItemYieldSheetWeightQuantityExpected = 0,
                };
                this.ListItems.Add(item);
            }
        }

        /// <inheritdoc/>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

                var tableActTicketMatItem = new TableActivityYieldSheetMaterialItem();
                var tableActTicketMatDetail = new TableActivityYieldSheetMaterialDetail();
                await tableActTicketMatDetail.PurgeData();
                await tableActTicketMatItem.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableActTicketMatItem.RegisterDataAccessListenerProgressOut(this);
                await tableActTicketMatItem.RequestItemData(this.ActHdr.HdrFkRefno, this.ActHdr.HdrRefno);
                tableActTicketMatItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <inheritdoc/>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            this.PlaceNewItemsAtBottomOfList(this.ListItems);
            var mainActivityItems = this.CreateObservableCollectionFromListItems(this.ListItems);
            var gridItems = this.DefineGrid();
            this.CalcMaxDataColWidth(gridItems, mainActivityItems.ToList());
            gridMainActivity.Children.Add(gridItems);

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateFinishedItem"];

            return mainActivityItems;
        }

        /// <inheritdoc/>
        public override ObservableCollection<object> CreateObservableCollectionFromListItems(List<object> listItems)
        {
            var mainActivityItems = new ObservableCollection<object>();
            foreach (var item in listItems.Cast<YieldSheetMaterialItem>())
            {
                mainActivityItems.Add(item);
            }

            return mainActivityItems;
        }

        /// <inheritdoc/>
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
                foreach (var item in listItems.Cast<YieldSheetMaterialItem>())
                {
                    mainActivityItems.Add(item);
                }
            }
            else
            {
                mainActivityItems = new ObservableCollection<object>();
                var lowerSearchText = filterSearch.ToLower().Trim();
                foreach (var item in from item in listItems.Cast<YieldSheetMaterialItem>()
                                     where item.ProductCode.ToLower().Contains(lowerSearchText)
                                     select item)
                {
                    mainActivityItems.Add(item);
                }

                this.ListItems = mainActivityItems.ToList();
            }

            return mainActivityItems;
        }

        /// <inheritdoc/>
        public override void PlaceNewItemsAtBottomOfList(List<object> listItems)
        {
            var sortedItems = listItems
                .Cast<YieldSheetMaterialItem>()
                .OrderBy(item => item.ActivityItemFKRefno >= 0 ? 0 : 1)
                .ThenBy(item => item.ActivityItemFKRefno)
                .Cast<object>()
                .ToList();

            listItems.Clear();
            listItems.AddRange(sortedItems);
        }

        /// <inheritdoc/>
        public override void CalcMaxDataColWidth(Grid definition, List<object> itemsToMeasure)
        {
            var gridService = DependencyService.Get<IGridService>();
            var columns = definition.ColumnDefinitions;
            foreach (var item in itemsToMeasure)
            {
                this.CalcMaxDataColWidth(item, gridService, columns);
            }
        }

        /// <inheritdoc/>
        public override void CalcMaxDataColWidth(
            object item,
            IGridService gridService,
            ColumnDefinitionCollection columns)
        {
            var materialItem = (YieldSheetMaterialItem)item;

            var detailWidths = new[]
            {
                gridService.CalculateTextWidth(Convert.ToString(materialItem.ProductCode)),
                gridService.CalculateTextWidth(materialItem.ProductUnitBC),
                gridService.CalculateTextWidth(materialItem.BaseUnitCode),
                gridService.CalculateTextWidth($"{materialItem.ActivityItemYieldSheetBaseQuantity:F3}"),
                gridService.CalculateTextWidth($"{materialItem.ActivityItemYieldSheetBaseQuantityExpected:F3}")
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

        /// <inheritdoc/>
        public override Grid DefineGrid()
        {
            var labelCodeHeader = new Label
                { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelBcHeader = new Label
                { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitHeader = new Label
                { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelQtyHeader = new Label
                { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelExpQtyHeader = new Label
                { Text = "Exp Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

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
            Grid.SetColumn(labelCodeHeader, value: 0);
            Grid.SetColumn(labelBcHeader, value: 1);
            Grid.SetColumn(labelUnitHeader, value: 2);
            Grid.SetColumn(labelQtyHeader, value: 3);
            Grid.SetColumn(labelExpQtyHeader, value: 4);

            Grid.SetRow(gridHeader, value: 0);
            return gridHeader;
        }

        /// <inheritdoc/>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            // Use StringBuilder pooling and parameterized queries for better performance and security
            // Also, avoid ToString() on StringBuilder until needed
            string sql;
            object[] parameters;

            int hdrRefno = this.ActHdr.HdrRefno;
            int actStatusNoChange = Convert.ToInt32(SysCommon.ActStatus.NoChange);
            int actSubTypeYieldSheetMaterial = Convert.ToInt32(SysCommon.ActSubTypes.YieldSheetMaterial);

            if (isSubmitBatch)
            {
                sql = @"
                    SELECT tActivityItemYieldSheet.ActivityItemRefno,
                           tActivityItemYieldSheetMaterial.ActivityItemFKRefno,
                           tActivityItemYieldSheetMaterial.ActivityItemStringFKRefno,
                           tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                           tProduct.ProductRefno AS ProductRefno,
                           tProduct.ProductCode AS ProductCode,
                           tProduct.ProductDescription AS ProductDescription,
                           tUnit.UnitCode AS BaseUnitCode,
                           tProductUnit.ProductUnitBC AS ProductUnitBC,
                           tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                           tActivityItemYieldSheet.ActivityItemYieldSheetQuantity AS ActivityItemYieldSheetQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetBaseQuantity AS ActivityItemYieldSheetBaseQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetWeightQuantity AS ActivityItemYieldSheetWeightQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetQuantityExpected AS ActivityItemYieldSheetQuantityExpected,
                           tActivityItemYieldSheet.ActivityItemYieldSheetBaseQuantityExpected AS ActivityItemYieldSheetBaseQuantityExpected,
                           tActivityItemYieldSheet.ActivityItemYieldSheetWeightQuantityExpected AS ActivityItemYieldSheetWeightQuantityExpected,
                           tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           tActivityItem.ActivityItemStatus AS ActivityItemStatus
                    FROM tActivityItemYieldSheet
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemYieldSheet.ActivityItemRefno
                    INNER JOIN tActivityItemYieldSheetMaterial ON tActivityItemYieldSheetMaterial.ActivityItemRefno = tActivityItemYieldSheet.ActivityItemRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemYieldSheet.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemYieldSheet.LocationMapRefno
                    WHERE tActivityItem.ActivityRefno = ?
                      AND tActivityItem.ActivityItemStatus <> ?
                      AND tActivityItem.ActivityItemSubType = ?
                    UNION
                    SELECT tActivityItemYieldSheet.ActivityItemRefno,
                           tActivityItemYieldSheetMaterial.ActivityItemFKRefno,
                           tActivityItemYieldSheetMaterial.ActivityItemStringFKRefno,
                           tProductUnit.ProductUnitRefno,
                           tProduct.ProductRefno,
                           tProduct.ProductCode,
                           tProduct.ProductDescription,
                           tUnit.UnitCode AS BaseUnitCode,
                           tProductUnit.ProductUnitBC,
                           tProductUnit.ProductUnitSizeDescription,
                           tActivityItemYieldSheet.ActivityItemYieldSheetQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetBaseQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetWeightQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetQuantityExpected,
                           tActivityItemYieldSheet.ActivityItemYieldSheetBaseQuantityExpected,
                           tActivityItemYieldSheet.ActivityItemYieldSheetWeightQuantityExpected,
                           tLocationMapWarehouse.LocationMapRefno,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           tActivityItem.ActivityItemStatus
                    FROM tActivityItemYieldSheet
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemYieldSheet.ActivityItemRefno
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItemYieldSheet.ActivityItemRefno
                    INNER JOIN tActivityItemYieldSheetMaterial ON tActivityItemYieldSheetMaterial.ActivityItemRefno = tActivityItemYieldSheet.ActivityItemRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemYieldSheet.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemYieldSheet.LocationMapRefno
                    WHERE tActivityItem.ActivityRefno = ?
                      AND tActivityDetail.ActivityDetailStatus <> ?
                      AND tActivityDetail.ActivityDetailSubType = ?
                    ORDER BY tActivityItemYieldSheet.ActivityItemRefno";
                parameters = new object[]
                {
                    hdrRefno, actStatusNoChange, actSubTypeYieldSheetMaterial,
                    hdrRefno, actStatusNoChange, actSubTypeYieldSheetMaterial
                };
            }
            else
            {
                sql = @"
                    SELECT tActivityItemYieldSheet.ActivityItemRefno,
                           tActivityItemYieldSheetMaterial.ActivityItemFKRefno,
                           tActivityItemYieldSheetMaterial.ActivityItemStringFKRefno,
                           tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                           tProduct.ProductRefno AS ProductRefno,
                           tProduct.ProductCode AS ProductCode,
                           tProduct.ProductDescription AS ProductDescription,
                           tUnit.UnitCode AS BaseUnitCode,
                           tProductUnit.ProductUnitBC AS ProductUnitBC,
                           tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                           tActivityItemYieldSheet.ActivityItemYieldSheetQuantity AS ActivityItemYieldSheetQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetBaseQuantity AS ActivityItemYieldSheetBaseQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetWeightQuantity AS ActivityItemYieldSheetWeightQuantity,
                           tActivityItemYieldSheet.ActivityItemYieldSheetQuantityExpected AS ActivityItemYieldSheetQuantityExpected,
                           tActivityItemYieldSheet.ActivityItemYieldSheetBaseQuantityExpected AS ActivityItemYieldSheetBaseQuantityExpected,
                           tActivityItemYieldSheet.ActivityItemYieldSheetWeightQuantityExpected AS ActivityItemYieldSheetWeightQuantityExpected,
                           tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           tActivityItem.ActivityItemStatus AS ActivityItemStatus
                    FROM tActivityItemYieldSheet
                    INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemYieldSheet.ActivityItemRefno
                    INNER JOIN tActivityItemYieldSheetMaterial ON tActivityItemYieldSheetMaterial.ActivityItemRefno = tActivityItemYieldSheet.ActivityItemRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemYieldSheet.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityItemYieldSheet.LocationMapRefno
                    WHERE tActivityItem.ActivityRefno = ?
                      AND tActivityItem.ActivityItemSubType = ?
                    ORDER BY tActivityItemYieldSheet.ActivityItemRefno";
                parameters = new object[]
                {
                    hdrRefno,
                    actSubTypeYieldSheetMaterial
                };
            }

            // Use parameterized query to avoid SQL injection and improve SQLite query plan caching
            this.ListYieldSheetMaterialItems = await Conn.QueryAsync<YieldSheetMaterialItem>(sql, parameters);
            this.ListItems = this.ListYieldSheetMaterialItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <inheritdoc cref="ActivityItem.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListItems = this.ListYieldSheetMaterialItems.Where(
                productionYieldSheetMaterialItem => productionYieldSheetMaterialItem.ProductCode.ToString().ToLower().
                Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            // Add all items in dataview to observable collection for display
            foreach (var productionYieldSheetMaterialItem in this.ListItems.Cast<YieldSheetMaterialItem>())
            {
                observableCollection.Add(productionYieldSheetMaterialItem);
            }

            return observableCollection;
        }

        /// <inheritdoc/>
        public override async Task AssignActivityItem(object activityItem)
        {
            if (activityItem == null)
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
            else if (activityItem is YieldSheetMaterialItem item)
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

            async Task AssignItemLocation(YieldSheetMaterialItem item)
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
                    if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc &&
                        await LocationMapTasks.IsCompleteLocMap(itemLocMap))
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

        /// <inheritdoc/>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            // Query only the needed value with parameters and LIMIT 1
            const int subType = (int)SysCommon.ActSubTypes.YieldSheetMaterial;
            const string sql = @"
                SELECT ai.ActivityItemRefno
                FROM tActivityItemYieldSheet cs
                INNER JOIN tActivityItem ai ON ai.ActivityItemRefno = cs.ActivityItemRefno
                WHERE ai.ActivityRefno = ?
                  AND cs.LocationMapRefno = ?
                  AND cs.ProductUnitRefno = ?
                  AND ai.ActivityItemSubType = ?
                LIMIT 1";

            var refno = await Conn.ExecuteScalarAsync<int?>(
                sql,
                this.ActHdr.HdrRefno,
                locMap.WhsLocMapRefno,
                prodUnitRefno,
                subType);

            if (refno.GetValueOrDefault() > 0)
            {
                return refno.Value;
            }

            // Fallback to in-memory list (preserves original behavior)
            foreach (var item in this.ListYieldSheetMaterialItems)
            {
                if (item.LocationMapRefno == locMap.WhsLocMapRefno && item.ProductUnitRefno == prodUnitRefno)
                {
                    return this.ItemRefno;
                }
            }

            return 0;
        }

        /// <inheritdoc/>
        public async override Task<string> CreateSubmitXml(XDocument data)
        {
            // Use XmlWriter directly for fast, allocation-minimized XML generation
            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
            var requestDefaults = await SysCommon.GetRequestDefaults(false);

            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("imclient");
                writer.WriteStartElement("command");
                writer.WriteElementString("action", "SUBMIT YIELD SHEET MATERIAL ITEM");
                writer.WriteEndElement(); // command

                // Insert request defaults as raw XML
                if (!string.IsNullOrEmpty(requestDefaults))
                {
                    var defaultsDoc = XDocument.Parse("<root>" + requestDefaults + "</root>");
                    foreach (var elem in defaultsDoc.Root.Elements())
                    {
                        elem.WriteTo(writer);
                    }
                }

                writer.WriteElementString("prod_id", prodCode);
                writer.WriteElementString("is_bc", prodUnitIsBc.ToString());
                writer.WriteElementString("symbology", symbologyPrefix);
                writer.WriteElementString("gtin", barCodeCode);
                writer.WriteElementString("whs_id", string.Empty);
                writer.WriteElementString("sect_refno", string.Empty);
                writer.WriteElementString("slot_refno", string.Empty);
                writer.WriteElementString("refno", string.Empty);
                writer.WriteEndElement(); // imclient
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override void ReplaceQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            if (selectedActivityItem is YieldSheetMaterialItem productionYieldSheetMaterialItem)
            {
                productionYieldSheetMaterialItem.ActivityItemYieldSheetQuantity = quantity;
                productionYieldSheetMaterialItem.ActivityItemYieldSheetBaseQuantity = baseQuantity;
                productionYieldSheetMaterialItem.ActivityItemYieldSheetWeightQuantity = weightQuantity;
            }
        }

        /// <inheritdoc/>
        public override void AddToQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            if (selectedActivityItem is YieldSheetMaterialItem productionYieldSheetMaterialItem)
            {
                productionYieldSheetMaterialItem.ActivityItemYieldSheetQuantity += quantity;
                productionYieldSheetMaterialItem.ActivityItemYieldSheetBaseQuantity += baseQuantity;
                productionYieldSheetMaterialItem.ActivityItemYieldSheetWeightQuantity += weightQuantity;
            }
        }

        /// <inheritdoc/>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is YieldSheetMaterialItem productionYieldSheetMaterialItem)
            {
                const string query = "UPDATE tActivityItemYieldSheet SET ActivityItemYieldSheetQuantity = ?," +
                    " ActivityItemYieldSheetBaseQuantity = ?, ActivityItemYieldSheetWeightQuantity = ?" +
                    " WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    productionYieldSheetMaterialItem.ActivityItemYieldSheetQuantity,
                    productionYieldSheetMaterialItem.ActivityItemYieldSheetBaseQuantity,
                    productionYieldSheetMaterialItem.ActivityItemYieldSheetWeightQuantity,
                    productionYieldSheetMaterialItem.ActivityItemRefno,
                };

                await Conn.ExecuteAsync(query, parameters);
            }
        }

        /// <inheritdoc/>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkItRefno = ((YieldSheetMaterialItem)resourceType).ActivityItemFKRefno;
            return tmpFkItRefno == null ? 0 : Convert.ToInt32(tmpFkItRefno);
        }

        /// <inheritdoc/>
        public override string GetControlDescription()
        {
            return $"YieldSheet ticket # {this.ActHdr.HdrFkRefno}";
        }

        /// <inheritdoc/>
        public override Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<string> GetComment(object selectedActivityItem)
        {
            return Task.FromResult(string.Empty);
        }

        /// <inheritdoc/>
        public override Task Submit(XDocument data, object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<string> CreateDeleteXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task Update(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<string> GetTotalQuantity()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override DataTemplate GetDataTemplate()
        {
            return (DataTemplate)this.ResourceDictionary["dataTemplateFinishedItem"];
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
            if (this.SelectedItem is YieldSheetMaterialItem item)
            {
                quantity = $"{item.ActivityItemYieldSheetQuantityExpected:F3} {this.ProdInfo.UnitCode} @ " +
                           $"{item.ActivityItemYieldSheetWeightQuantityExpected:F3} {this.ProdInfo.WeightUnitCode}";
            }

            return quantity;
        }

        /// <inheritdoc />
        public override string GetShippedReceivedCountedQuantity()
        {
            var quantity = "0.000";
            if (this.SelectedItem is YieldSheetMaterialItem item)
            {
                quantity = $"{item.ActivityItemYieldSheetQuantity:F3} {this.ProdInfo.UnitCode} @ " +
                           $"{item.ActivityItemYieldSheetWeightQuantity:F3} {this.ProdInfo.WeightUnitCode}";
            }

            return quantity;
        }
    }
}
