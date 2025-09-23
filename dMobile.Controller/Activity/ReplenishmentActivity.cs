// <copyright file="ReplenishmentActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Bedrock.Settings;
    using dMobile.GroundWork;
    using dMobile.Resources;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types.Headers;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="ReplenishmentActivity" />.
    /// </summary>
    public class ReplenishmentActivity : Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplenishmentActivity"/> class.
        /// </summary>
        public ReplenishmentActivity()
        {
            DefineInitialGridDataTemplate();

            void DefineInitialGridDataTemplate()
            {
                var dataTemplate = new DataTemplate(() =>
                {
                    var gridData = new Grid
                    {
                        Padding = new Thickness(5),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(this.FromColWidth0, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.FromColWidth1, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.FromColWidth2, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.FromColWidth3, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.FromColWidth4, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.FromColWidth5, GridUnitType.Absolute) },
                        },
                    };

                    var labelMoveTypeString = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelFromLocation = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelProductBc = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelProductUnit = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelProductAltQty = new Label { VerticalOptions = LayoutOptions.Center };

                    labelMoveTypeString.SetBinding(Label.TextProperty, new Binding("MoveTypeString"));
                    labelFromLocation.SetBinding(Label.TextProperty, new Binding("ReadableLocationMap"));
                    labelProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                    labelProductBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                    labelProductUnit.SetBinding(Label.TextProperty, new Binding("ProductUnitCode"));
                    labelProductAltQty.SetBinding(Label.TextProperty, new Binding("ActivityItemReplenishmentQuantity"));

                    gridData.Children.Add(labelMoveTypeString, 0, 0);
                    gridData.Children.Add(labelFromLocation, 0, 0);
                    gridData.Children.Add(labelProductCode, 0, 0);
                    gridData.Children.Add(labelProductBc, 0, 0);
                    gridData.Children.Add(labelProductUnit, 0, 0);
                    gridData.Children.Add(labelProductAltQty, 0, 0);

                    Grid.SetColumn(labelMoveTypeString, 0);
                    Grid.SetColumn(labelFromLocation, 1);
                    Grid.SetColumn(labelProductCode, 2);
                    Grid.SetColumn(labelProductBc, 3);
                    Grid.SetColumn(labelProductUnit, 4);
                    Grid.SetColumn(labelProductAltQty, 5);

                    return new ViewCell { View = gridData };
                });

                var isAddKey = true;
                foreach (var item in this.ResDict)
                {
                    if (item.Key == "dataTemplateFromReplenish")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.ResDict.Add("dataTemplateFromReplenish", dataTemplate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the replenishment settings.
        /// </summary>
        public ReplenishmentSettings ReplenSettings { get; set; } = new ReplenishmentSettings();

        /// <summary>
        /// Gets or sets the list view / from grid column 0 width. MoveType.
        /// </summary>
        public double FromColWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / from grid column 1 width. From Location.
        /// </summary>
        public double FromColWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / from grid column 2 width. Product Code.
        /// </summary>
        public double FromColWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / from grid column 2 width. Product BC.
        /// </summary>
        public double FromColWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / from grid column 2 width. Product unit.
        /// </summary>
        public double FromColWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / from grid column 2 width. Alt Qty.
        /// </summary>
        public double FromColWidth5 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / to grid column 0 width.
        /// </summary>
        public double ToColWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / to grid column 1 width.
        /// </summary>
        public double ToColWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary of string for resource objects.
        /// </summary>
        public ResourceDictionary ResDict { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets a list of from headers.
        /// </summary>
        public List<Replenishment> ListReplenishments { get; set; } = new List<Replenishment>();

        /// <summary>
        /// Purge adjustment headers, items and detail.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task Purge()
        {
            var replenishment = new TableActivityReplenishment();
            await replenishment.PurgeData();
        }

        /// <summary>
        /// Get the activity header foreign key reference index name.
        /// </summary>
        /// <returns><see cref="string"/> Index name.</returns>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// Get the activity header control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public override string GetControlDescription()
        {
            return "Replenishment";
        }

        /// <summary>
        /// Design the activity header grid.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity header grid.</param>
        /// <param name="listViewMainActivity"><see cref="ListView"/> Main activity list.</param>
        /// <returns><see cref="ObservableCollection{object}"/> List of activity headers.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid gridMainActivity,
            ref ListView listViewMainActivity)
        {
            var headersList = new ObservableCollection<object>();

            var labelMoveTypeString = new Label
            {
                Text = "Type",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelFromLocation = new Label
            {
                Text = "From",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelFromProductCode = new Label
            {
                Text = "Product",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelFromProductBC = new Label
            {
                Text = "BC",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelFromProductUnit = new Label
            {
                Text = "Unit",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelFromAltQty = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            // Calculate text size
            this.FromColWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelMoveTypeString.Text);
            this.FromColWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelFromLocation.Text);
            this.FromColWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelFromProductCode.Text);
            this.FromColWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelFromProductBC.Text);
            this.FromColWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelFromProductUnit.Text);
            this.FromColWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelFromAltQty.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth5, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelMoveTypeString);
            gridHeader.Children.Add(labelFromLocation);
            gridHeader.Children.Add(labelFromProductCode);
            gridHeader.Children.Add(labelFromProductBC);
            gridHeader.Children.Add(labelFromProductUnit);
            gridHeader.Children.Add(labelFromAltQty);

            Grid.SetColumn(labelMoveTypeString, 0);
            Grid.SetColumn(labelFromLocation, 1);
            Grid.SetColumn(labelFromProductCode, 2);
            Grid.SetColumn(labelFromProductBC, 3);
            Grid.SetColumn(labelFromProductUnit, 4);
            Grid.SetColumn(labelFromAltQty, 5);

            Grid.SetRow(gridHeader, 0);
            gridMainActivity.Children.Add(gridHeader);

            foreach (var replen in this.ListHeaders.Cast<Replenishment>())
            {
                headersList.Add(replen);

                var dataFromColWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(replen.MoveTypeString);
                var dataFromColWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(replen.ReadableLocationMap);
                var dataFromColWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(replen.ProductCode);
                var dataFromColWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(replen.ProductUnitBC);
                var dataFromColWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(replen.ProductUnitCode);
                var dataFromColWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{replen.ActivityItemReplenishmentQuantity:F3}");

                if (dataFromColWidth0 > this.FromColWidth0)
                {
                    this.FromColWidth0 = dataFromColWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.FromColWidth0;
                }

                if (dataFromColWidth1 > this.FromColWidth1)
                {
                    this.FromColWidth1 = dataFromColWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.FromColWidth1;
                }

                if (dataFromColWidth2 > this.FromColWidth2)
                {
                    this.FromColWidth2 = dataFromColWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.FromColWidth2;
                }

                if (dataFromColWidth3 > this.FromColWidth3)
                {
                    this.FromColWidth3 = dataFromColWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.FromColWidth3;
                }

                if (dataFromColWidth4 > this.FromColWidth4)
                {
                    this.FromColWidth4 = dataFromColWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.FromColWidth4;
                }

                if (dataFromColWidth5 > this.FromColWidth5)
                {
                    this.FromColWidth5 = dataFromColWidth5;
                    gridHeader.ColumnDefinitions[5].Width = this.FromColWidth5;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResDict["dataTemplateFromReplenish"];

            return headersList;
        }

        /// <summary>
        /// Get list of from move types.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool"/> Is submitting batch mode.</param>
        /// <returns><see cref="List{object}"/> List of headers.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            ////var replenSettings = new ReplenishmentSettings().LoadData();

            if (isSubmitBatch)
            {
                throw new NotImplementedException("Replenishment batching is currently not supported.");
            }

            var query = @"
                select replen.ActivityRefno
                        ,replen.ActivitySubType
                        ,replen.WarehouseLocationMapRefno
                        ,replenItem.ActivityItemRefno
                        ,replenItem.ActivityItemSubType
                        ,replenItem.MoveType
                        ,cast('' as varchar(10)) as MoveTypeString
                        ,replenItem.ActivityItemFKRefno
                        ,replenItem.ActivityItemFKParRefno
                        ,replenItem.ProductUnitRefno
                        ,cast('' as varchar(10)) as ProductCode
                        ,cast('' as varchar(1)) as ProductUnitBC
                        ,cast('' as varchar(2)) as ProductUnitCode
                        ,replenItem.ProductUnitRefno
                        ,replenItem.ProductUnitRefno
                        ,replenItem.LocationMapRefno
                        ,cast('' as varchar(30)) as ReadableLocationMap
                        ,replenItem.ActivityItemReplenishmentQuantity
                        ,replenItem.ActivityItemReplenishmentBaseQuantity
                        ,replenItem.ActivityItemReplenishmentWeightQuantity
                        ,replenDetail.ActivityDetailFKRefno
                        ,replenDetail.ActivityDetailReplenishmentIsConfirmed
                        ,detail.ActivityDetailRefno
                        ,detail.ActivityDetailSubType
                        ,detail.ActivityDetailStatus
                        ,detail.ActivityItemRefno
                        ,detail.ActivityDetailUniqueSchemeID
                        ,detail.ActivityDetailSchemeID
                        ,detail.ActivityDetailQuantity
                        ,detail.ActivityDetailBaseQuantity
                        ,detail.ActivityDetailWeightQuantity
                        ,detail.ActivityDetailSingleTradeItemSerialNumber
                        ,detail.ActivityDetailLogisticUnitBatchLotNumber
                        ,detail.ActivityDetailLogisticUnitBestBeforeSellByDate
                        ,detail.ActivityDetailLogisticUnitExpiryUseByDate
                        ,detail.ActivityDetailLogisticUnitProductionDate
                        ,detail.ActivityDetailLogisticUnitDueDate
                        ,detail.ActivityDetailLogisticUnitPackagingDate
                        ,detail.ActivityDetailLogisticUnitSkidNumber
                    from tActivityReplenishment replen
                    inner join tActivity on tActivity.ActivityRefno = replen.ActivityRefno
                    inner join tActivityItem item on item.ActivityRefno = tActivity.ActivityRefno
                    inner join tActivityItemReplenishment replenItem on replenItem.ActivityItemRefno = item.ActivityItemRefno
                    inner join tActivityDetail detail on detail.ActivityItemRefno = item.ActivityItemRefno
                    inner join tActivityDetailReplenishment replenDetail on replenDetail.ActivityDetailRefno = detail.ActivityDetailRefno";

            this.ListReplenishments = await Conn.QueryAsync<Replenishment>(query);

            foreach (var item in this.ListReplenishments)
            {
                item.MoveTypeString = ReplenishmentMoveType.Enum2String(item.MoveType);
                var tmpLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(item.LocationMapRefno);
                item.ReadableLocationMap = LocationMapTasks.GetReadableLocation(tmpLocMap, isSepSlotCoords: false);

                var prodBCUnit = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);
                item.ProductCode = prodBCUnit.Code;
                item.ProductUnitBC = prodBCUnit.UnitBc;
                item.ProductUnitCode = prodBCUnit.UnitCode;
            }

            var from = from item in this.ListReplenishments
                                where item.ActivityItemFKRefno == item.ActivityItemFKParRefno
                                select item;
            this.ListHeaders = from.Cast<object>().ToList();

            var to = from item in this.ListReplenishments
                     where item.ActivityItemFKRefno != item.ActivityItemFKParRefno
                     select item;
            this.ListItems = to.Cast<object>().ToList();

            return this.ListHeaders;
        }

        /// <summary>
        /// Select the adjustment activity header.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Activity header to select.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Replenishment header)
            {
                this.SelectedActivity = header;
                this.HdrFkRefno = header.ActivityItemFKRefno;
                this.HdrRefno = header.ActivityRefno;
                this.LocationMapRefno = header.LocationMapRefno;
                ////this.ReplenSettings = this.ReplenSettings.LoadData();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the activity reference number for the selected object.
        /// </summary>
        /// <param name="selectedItem"><see cref="object"/> Selected item.</param>
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is Replenishment hdr)
            {
                this.HdrFkRefno = Convert.ToInt32(hdr.ActivityItemFKRefno);
                this.HdrRefno = Convert.ToInt32(hdr.ActivityRefno);
            }
        }

        /// <summary>
        /// Refresh the activity headers.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Activity header filter criteria.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
                var replen = new TableActivityReplenishment();
                await replen.RequestHeaderData();
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity replenishment filter.
        /// </summary>
        /// <exception cref="NotImplementedException"> Not implemented exception.</exception>
        public override void GetFilter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity replenishment filter.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Prefilter criteria.</param>
        /// <exception cref="NotImplementedException"> Not implemented exception.</exception>
        public override void GetFilter(XDocument data)
        {
            throw new NotImplementedException();
        }
    }
}
