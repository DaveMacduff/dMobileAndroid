// <copyright file="TransferActItem.cs" company="InterSect Business Systems Inc">
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
using dMobile.Resources.Types.Items;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="TransferActItem" />.
    /// </summary>
    public class TransferActItem : ActItem
    {
        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double columnWidth0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double columnWidth1;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double columnWidth2;

        /// <summary>
        /// Defines the columnWidth3.
        /// </summary>
        private double columnWidth3;

        /// <summary>
        /// Defines the columnWidth4.
        /// </summary>
        private double columnWidth4;

        /// <summary>
        /// Defines the columnWidth5.
        /// </summary>
        private double columnWidth5;

        /// <summary>
        /// Defines the listTransferItems.
        /// </summary>
        private List<TransferItem> listTransferItems = new List<TransferItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public TransferActItem(Activity.Activity activity)
            : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Trans;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsTransCreateNewItemNoAsk;
            this.ActSubType = activity.ActSubType;

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
                                new ColumnDefinition
                                    { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                                new ColumnDefinition
                                    { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                                new ColumnDefinition
                                    { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                                new ColumnDefinition
                                    { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                                new ColumnDefinition
                                    { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                                new ColumnDefinition
                                    { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) }
                            }
                        };

                        var labelProductId = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelBc = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelRequired = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                        var labelWhs = new Label { VerticalOptions = LayoutOptions.Center };

                        labelProductId.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                        labelBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                        labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                        labelRequired.SetBinding(
                            Label.TextProperty,
                            new Binding(
                                "ActivityItemTransferRequiredQuantity",
                                BindingMode.Default,
                                null,
                                null,
                                "{0:F3}"));
                        labelQty.SetBinding(
                            Label.TextProperty,
                            new Binding("ActivityItemTransferQuantity", BindingMode.Default, null, null, "{0:F3}"));
                        labelWhs.SetBinding(Label.TextProperty, new Binding("LocationCode"));

                        gridData.Children.Add(labelProductId);
                        gridData.Children.Add(labelBc);
                        gridData.Children.Add(labelUnitCode);
                        gridData.Children.Add(labelRequired);
                        gridData.Children.Add(labelQty);
                        gridData.Children.Add(labelWhs);

                        Grid.SetColumn(labelProductId, 0);
                        Grid.SetColumn(labelBc, 1);
                        Grid.SetColumn(labelUnitCode, 2);
                        Grid.SetColumn(labelRequired, 3);
                        Grid.SetColumn(labelQty, 4);
                        Grid.SetColumn(labelWhs, 5);

                        return new ViewCell { View = gridData };
                    });

                var isAddKey = true;
                foreach (var item in this.ResourceDictionary)
                {
                    if (item.Key == "dataTemplateTransferItem")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.ResourceDictionary.Add("dataTemplateTransferItem", dataTemplate);
                }
            }
        }

        /// <inheritdoc/>
        public override Color GetShippedReceivedCountedDisplayTextColor()
        {
            var displayColor = base.GetShippedReceivedCountedDisplayTextColor();
            if (this.SelectedItem is TransferItem item)
            {
                displayColor = item.ActivityItemTransferRequiredQuantity == item.ActivityItemTransferQuantity
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
                foreach (var item in this.ListItems.Cast<TransferItem>())
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
                ////isCatchAsk = this.SysEnv.SysSettings.IsTransCatchAskEnabled;
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
            if (this.IsCatchAsk() &&
                this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsTransferNoCatchAskZeroEnabled;
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
                if (((TransferItem)item).ActivityItemFKRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        /// <inheritdoc />
        public override DataTemplate GetDataTemplate()
        {
            return (DataTemplate)this.ResourceDictionary["dataTemplateTransferItem"];
        }

        /// <summary>
        /// Check if the quantity is valid, and is ready to be used.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Quantity captured to validate.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task IsReadyToApply(XDocument data)
        {
            data = data ?? throw new ArgumentNullException(nameof(data));
            _ = data.Root ?? throw new ArgumentNullException(nameof(data.Root));
            _ = data.Root.Element("itm_refno")?.Value ??
                throw new BaseException("xml 'itm_refno' element is does not exist.");
            _ = data.Root.Element("alt_qty")?.Value ?? throw new BaseException("xml 'alt_qty' element is does not exist.");

            var itmRefno = int.Parse(data.Root.Element("itm_refno")?.Value!);
            var altQty = decimal.Parse(data.Root.Element("alt_qty")?.Value!);

            await base.IsReadyToApply(data);

            // New items are identified with 0 < this.FkRefno, do not validate.
            var showWarning = this.SysEnv.SysSettings.TransQtyOverWarn !=
                              SysCommon.ActTransfersQtyOverValLevels.NoValidationOrWarning;
            if (showWarning && this.ItemFkRefno > 0)
            {
                try
                {
                    TransferModuleQuantityOverCheck(altQty);
                }
                catch (Exception ex)
                {
                    await HandleQtyOverCheck(ex);
                }
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            void TransferModuleQuantityOverCheck(decimal actItemTransQuantity)
            {
                if (this.SelectedItem is TransferItem transferItem)
                {
                    var transferred = Math.Round(Convert.ToDecimal(transferItem.ActivityItemTransferQuantity), 3);
                    var required = Math.Round(Convert.ToDecimal(transferItem.ActivityItemTransferRequiredQuantity), 3);
                    if (transferred + actItemTransQuantity > required)
                    {
                        var msg = $"The resulting quantity transferred {transferred + actItemTransQuantity}" +
                                  $" {this.ProdInfo.UnitCode} will be greater than the quantity required {required}" +
                                  $" {this.ProdInfo.UnitCode}.";
                        throw new BaseException(msg);
                    }
                }
                else
                {
                    const string msg = "The selected item is not a transfer item type.";
                    throw new BaseException(msg);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task HandleQtyOverCheck(Exception ex)
            {
                switch (this.SysEnv.SysSettings.TransQtyOverWarn)
                {
                    case SysCommon.ActTransfersQtyOverValLevels.ValidateWarnAndContinue:

                        if (await Device.InvokeOnMainThreadAsync(
                                async () => !await Application.Current.MainPage.DisplayAlert(
                                    "Quantity Over",
                                    ex.Message + Environment.NewLine + "Continue?",
                                    "Yes",
                                    "No")))
                        {
                            throw new IsReadyToApplyException(ex.Message, true, false);
                        }

                        break;

                    case SysCommon.ActTransfersQtyOverValLevels.ValidateWarnAndStop:
                        throw new IsReadyToApplyException(ex.Message, false, false);

                    case SysCommon.ActTransfersQtyOverValLevels.NoValidationOrWarning:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to parse the transfer item # {actItemStringFkRefno}";
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return $"Unable to find the transfer item # {actItemStringFkRefno}";
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
                throw new BaseException("prodUnitRefno expected to be greater than zero.");
            }

            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            await InsertActivityItemInTables(prodUnitRefno);
            AddActivityItemToList(prodUnitRefno);
            return;

            //////////////////////////////////////////////////////////////////////////
            async Task InsertActivityItemInTables(int prodUnitRefno)
            {
                await DatabaseConnection.GetInstance()
                    .SqlConn.RunInTransactionAsync(
                        transaction =>
                        {
                            try
                            {
                                // NOTE: ActivityItemStatus is set to inserted for later submission use.
                                _ = transaction.Execute(
                                    "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                                    " ActivityRefno, ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                    (int)this.ActSubType,
                                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                                    Convert.ToInt32(this.ActHdr.HdrRefno),
                                    1);

                                this.ItemRefno = Convert.ToInt32(
                                    transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));

                                // Get temporary item FK refno for newly inserted line item.
                                // Do not use the subtype for the max FK item refno because we could have more than one
                                // subtype in the table.
                                ////this.ItemFkRefno = Convert.ToInt32(
                                ////    transaction.ExecuteScalar<int>(
                                ////        "SELECT MAX(tActivityItemTransfer.ActivityItemFKRefno)" +
                                ////        $" FROM tActivityItemTransfer WHERE tActivityItemTransfer.ActivityItemSubType" +
                                ////        $" = {(int)this.ActSubType}"));
                                this.ItemFkRefno = Convert.ToInt32(
                                    transaction.ExecuteScalar<int>(
                                        "SELECT MAX(tActivityItemTransfer.ActivityItemFKRefno) FROM tActivityItemTransfer"));
                                this.ItemFkRefno += 1;

                                var query = "INSERT INTO tActivityItemTransfer (" +
                                            "ActivityItemRefno, " +
                                            "ActivityItemSubType, " +
                                            "ActivityItemFKRefno, " +
                                            "ActivityItemStringFKRefno, " +
                                            "ProductUnitRefno, " +
                                            "TransferFromWarehouseLocationMapRefno, " +
                                            "ActivityItemTransferRequiredQuantity, " +
                                            "ActivityItemTransferQuantity, " +
                                            "ActivityItemTransferBaseQuantity, " +
                                            "ActivityItemTransferWeightQuantity" +
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
                                    Convert.ToInt32(this.ActSubType),
                                    Convert.ToInt32(this.ItemFkRefno),
                                    Convert.ToString(this.ItemFkRefno),
                                    Convert.ToInt32(prodUnitRefno),
                                    Convert.ToInt32(locMap.WhsLocMapRefno),
                                    Convert.ToDecimal(0),
                                    Convert.ToDecimal(0),
                                    Convert.ToDecimal(0),
                                    Convert.ToDecimal(0)
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
                var item = new TransferItem()
                {
                    ActivityItemRefno = this.ItemRefno,
                    ActivityItemFKRefno = this.ItemFkRefno,
                    ActivityItemStringFKRefno = this.ItemFkRefno.ToString(),
                    ProductUnitRefno = prodUnitRefno,
                    ProductRefno = int.Parse(data.Root.Element("ProductRefno").Value),
                    ProductCode = data.Root.Element("ProductCode").Value,
                    ProductDescription = data.Root.Element("ProductDescription").Value,
                    UnitCode = data.Root.Element("UnitCode").Value,
                    ProductUnitBC = data.Root.Element("ProductUnitBC").Value,
                    ProductUnitSizeDescription = data.Root.Element("ProductUnitSizeDescription").Value,
                    LocationMapRefno = locMap.WhsLocMapRefno,
                    LocationCode = locMap.WhsCode,
                    LocationName = locMap.WhsName,
                    ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    ActivityItemTransferRequiredQuantity = 0,
                    ActivityItemTransferQuantity = 0,
                    ActivityItemTransferBaseQuantity = 0,
                    ActivityItemTransferWeightQuantity = 0
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

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    var tableActTransferItem = new TableActivityTransferFromItem();
                    var tableActTransferDetail = new TableActivityTransferFromDetail();
                    await tableActTransferDetail.PurgeData();
                    await tableActTransferItem.PurgeData();

                    this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut(this);
                    await tableActTransferItem.RequestItemData(this.ActHdr.HdrFkRefno, this.ActHdr.HdrRefno);
                    tableActTransferItem.UnregisterDataAccessListenerProgressOut(this);
                }
                else
                {
                    var tableActTransferItem = new TableActivityTransferToItem();
                    var tableActTransferDetail = new TableActivityTransferToDetail();
                    await tableActTransferDetail.PurgeData();
                    await tableActTransferItem.PurgeData();

                    this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut(this);
                    await tableActTransferItem.RequestItemData(this.ActHdr.HdrFkRefno, this.ActHdr.HdrRefno);
                    tableActTransferItem.UnregisterDataAccessListenerProgressOut(this);
                }
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
            this.PlaceNewItemsAtBottomOfList(this.ListItems);
            var mainActivityItems = this.CreateObservableCollectionFromListItems(this.ListItems);
            var gridItems = this.DefineGrid();
            this.CalcMaxDataColWidth(gridItems, mainActivityItems.ToList());
            gridMainActivity.Children.Add(gridItems);

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateTransferItem"];

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
            foreach (var item in listItems.Cast<TransferItem>())
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
                foreach (var item in listItems.Cast<TransferItem>())
                {
                    mainActivityItems.Add(item);
                }
            }
            else
            {
                mainActivityItems = new ObservableCollection<object>();
                var lowerSearchText = filterSearch.ToLower().Trim();
                foreach (var item in listItems.Cast<TransferItem>())
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
                .Cast<TransferItem>()
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
            var transferItem = (TransferItem)item;

            var detailWidths = new[]
            {
                gridService.CalculateTextWidth(transferItem.ProductCode),
                gridService.CalculateTextWidth(transferItem.ProductUnitBC),
                gridService.CalculateTextWidth(transferItem.UnitCode),
                gridService.CalculateTextWidth($"{transferItem.ActivityItemTransferRequiredQuantity:F3}"),
                gridService.CalculateTextWidth($"{transferItem.ActivityItemTransferQuantity:F3}"),
                gridService.CalculateTextWidth(Convert.ToString(transferItem.LocationCode))
            };

            var columnWidths = new[]
            {
                this.columnWidth0, this.columnWidth1, this.columnWidth2, this.columnWidth3, this.columnWidth4,
                this.columnWidth5
            };

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
            this.columnWidth5 = columnWidths[5];
        }

        /// <summary>
        ///     Define the grid layout for the activity item.
        /// </summary>
        /// <returns>New grid layout.</returns>
        public override Grid DefineGrid()
        {
            var labelProductIdHeader = new Label
                { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelBcHeader = new Label
                { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitCodeHeader = new Label
                { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelRequiredHeader = new Label
                { Text = "Required", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelQtyHeader = new Label
                { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelWhsHeader = new Label
                { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIdHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBcHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelRequiredHeader.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);

            // Create header data template
            var gridColumns = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) }
                }
            };

            gridColumns.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridColumns.Children.Add(labelProductIdHeader);
            gridColumns.Children.Add(labelBcHeader);
            gridColumns.Children.Add(labelUnitCodeHeader);
            gridColumns.Children.Add(labelRequiredHeader);
            gridColumns.Children.Add(labelQtyHeader);
            gridColumns.Children.Add(labelWhsHeader);

            Grid.SetColumn(labelProductIdHeader, 0);
            Grid.SetColumn(labelBcHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelRequiredHeader, 3);
            Grid.SetColumn(labelQtyHeader, 4);
            Grid.SetColumn(labelWhsHeader, 5);

            Grid.SetRow(gridColumns, 0);
            return gridColumns;
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
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemFKRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append(
                    "tActivityItemTransfer.ActivityItemTransferRequiredQuantity AS ActivityItemTransferRequiredQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity AS ActivityItemTransferQuantity,");
                sql.Append(
                    "tActivityItemTransfer.ActivityItemTransferBaseQuantity AS ActivityItemTransferBaseQuantity,");
                sql.Append(
                    "tActivityItemTransfer.ActivityItemTransferWeightQuantity AS ActivityItemTransferWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemTransfer ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat(
                    "AND tActivityItem.ActivityItemStatus <> {0} ",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemFKRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferRequiredQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferBaseQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemTransfer ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat(
                    " AND tActivityDetail.ActivityDetailStatus <> {0} ",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" ORDER BY tActivityItemTransfer.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno, ");
                sql.Append("tActivityItemTransfer.ActivityItemFKRefno, ");
                sql.Append("tActivityItemTransfer.ActivityItemStringFKRefno, ");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno, ");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode, ");
                sql.Append("tProduct.ProductDescription AS ProductDescription, ");
                sql.Append("tUnit.UnitCode AS UnitCode, ");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, ");
                sql.Append(
                    "tActivityItemTransfer.ActivityItemTransferRequiredQuantity AS ActivityItemTransferRequiredQuantity, ");
                sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity AS ActivityItemTransferQuantity, ");
                sql.Append(
                    "tActivityItemTransfer.ActivityItemTransferBaseQuantity AS ActivityItemTransferBaseQuantity, ");
                sql.Append(
                    "tActivityItemTransfer.ActivityItemTransferWeightQuantity AS ActivityItemTransferWeightQuantity, ");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemTransfer ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.Append("ORDER BY tActivityItemTransfer.ActivityItemRefno");
            }

            this.listTransferItems =
                await DatabaseConnection.GetInstance().SqlConn.QueryAsync<TransferItem>(sql.ToString());
            this.ListItems = this.listTransferItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <inheritdoc cref="ActivityItem.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListItems = this.listTransferItems.Where(
                    transferItem => transferItem.ProductCode.ToLower()
                        .Contains(searchText.ToLower().Trim()))
                .Cast<object>()
                .ToList();

            foreach (var transferItem in this.ListItems.Cast<TransferItem>())
            {
                observableCollection.Add(transferItem);
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
            else if (selectedActivityItem is TransferItem item)
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
            async Task AssignItemLocation(TransferItem item)
            {
                // This code needs to be before the call to GetSelectedItemLocation, because the skid location may be
                // used if the product unit and warehouse match.
                if (this.SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid &&
                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ProdInfo.Refno)
                {
                    this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                }

                ////var itemLocMap = await this.GetSelectedItemLocation(item.LocationMapRefno);
                var itemLocMap = await this.GetSelectedItemLocation(this.ActHdr.LocationMapRefno);
                if (itemLocMap == null)
                {
                    // Current location used, we need one for popup forms.
                    this.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.ActHdr.LocationMapRefno);
                    if (this.LocMap.WhsLocMapRefno == 0)
                    {
                        // We need to have at least a location with a warehouse.
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
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item reference number else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            var foundActItemRefno = 0;
            const string query = "SELECT tActivityItem.ActivityItemRefno," +
                                 "tLocationMapWarehouse.LocationMapRefno " +
                                 "FROM tActivityItemTransfer " +
                                 "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemTransfer.ActivityItemRefno " +
                                 "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno " +
                                 "= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno " +
                                 "WHERE tActivityItem.ActivityRefno = ? " +
                                 "AND tActivityItemTransfer.TransferFromWarehouseLocationMapRefno = ? " +
                                 "AND tActivityItemTransfer.ProductUnitRefno = ?";
            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<TransferItem>(
                query, this.ActHdr.HdrRefno, locMap.WhsLocMapRefno, prodUnitRefno);
            if (results != null && results.Count > 0)
            {
                foundActItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
            }
            else
            {
                foreach (var item in this.listTransferItems)
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
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>SUBMIT TRANSFER ITEM</action>" +
                "</command>" +
                await SysCommon.GetRequestDefaults(false) +
                "<prod_id>" +
                data.Root.Element("ProductCode").Value.PadRight(10) +
                "</prod_id>" +
                "<alt_unit>" +
                data.Root.Element("UnitCode").Value +
                "</alt_unit>" +
                "<is_bc>" +
                bool.Parse(data.Root.Element("ProductUnitIsBC").Value) +
                "</is_bc>" +
                "<symbology>" +
                data.Root.Element("ProductUnitBarCodeSymbologyPrefix") +
                "</symbology>" +
                "<gtin>" +
                data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20) +
                "</gtin>" +
                "<whs_id></whs_id>" +
                "<sect_refno></sect_refno>" +
                "<slot_refno></slot_refno>" +
                "<refno></refno>" +
                "</imclient>");

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
            if (selectedActivityItem is TransferItem transferItem)
            {
                transferItem.ActivityItemTransferQuantity = quantity;
                transferItem.ActivityItemTransferBaseQuantity = baseQuantity;
                transferItem.ActivityItemTransferWeightQuantity = weightQuantity;
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
            if (selectedActivityItem is TransferItem transferItem)
            {
                transferItem.ActivityItemTransferQuantity += quantity;
                transferItem.ActivityItemTransferBaseQuantity += baseQuantity;
                transferItem.ActivityItemTransferWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is TransferItem transferItem)
            {
                var query = "UPDATE tActivityItemTransfer SET ActivityItemTransferQuantity = ?," +
                            " ActivityItemTransferBaseQuantity = ?, ActivityItemTransferWeightQuantity = ?" +
                            " WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    transferItem.ActivityItemTransferQuantity,
                    transferItem.ActivityItemTransferBaseQuantity,
                    transferItem.ActivityItemTransferWeightQuantity,
                    transferItem.ActivityItemRefno
                };

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkItRefno = ((TransferItem)resourceType).ActivityItemFKRefno;
            return tmpFkItRefno == null ? 0 : Convert.ToInt32(tmpFkItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"Transfer # {this.ActHdr.HdrFkRefno}";
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
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is TransferItem item)
            {
                if (this.ActHdr.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    if (Math.Sign(item.ActivityItemTransferQuantity) == 0 ||
                        Math.Sign(item.ActivityItemTransferQuantity) == 1)
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
                else if (this.ActHdr.ActSubType == SysCommon.ActSubTypes.TransferTo)
                {
                    if (Math.Sign(item.ActivityItemTransferQuantity) == 0 ||
                        Math.Sign(item.ActivityItemTransferQuantity) == 1)
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

        /// <inheritdoc />
        public override string GetOrderedExpectedOnHandTitle()
        {
            return "Ordered";
        }

        /// <inheritdoc />
        public override string GetShippedReceivedCountedTitle()
        {
            return "Transferred";
        }

        /// <inheritdoc />
        public override string GetOrderedExpectedOnHandQuantity()
        {
            var quantity = "0.000";
            if (this.SelectedItem is TransferItem item)
            {
                quantity = $"{item.ActivityItemTransferRequiredQuantity:F3} {this.ProdInfo.UnitCode} @ " +
                    $"{item.ActivityItemTransferRequiredQuantity * this.ProdInfo.UnitGrossWeight:F3} {this.ProdInfo.WeightUnitCode}";
            }

            return quantity;
        }

        /// <inheritdoc />
        public override string GetShippedReceivedCountedQuantity()
        {
            var quantity = "0.000";
            if (this.SelectedItem is TransferItem item)
            {
                quantity = $"{item.ActivityItemTransferQuantity:F3} {this.ProdInfo.UnitCode} @ " +
                           $"{item.ActivityItemTransferWeightQuantity:F3} {this.ProdInfo.WeightUnitCode}";
            }

            return quantity;
        }
    }
}