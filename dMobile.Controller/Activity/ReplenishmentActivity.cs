// <copyright file="ReplenishmentActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
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
          : base()
        {
            this.FullClassName = typeof(ReplenishmentActivity).FullName;
            ////this.ReplenSettings = new ReplenishmentSettings().LoadData();
            var dataFromTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.FromColWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.FromColWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.FromColWidth2, GridUnitType.Absolute) },
                    },
                };

                var labelActivityStringFkRefno = new Label { VerticalOptions = LayoutOptions.Center };
                var labelActivityReplenishmentDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                var labelActivityReplenishmentDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityStringFkRefno.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
                labelActivityReplenishmentDateTime.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityReplenishmentDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityReplenishmentDescription.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityReplenishmentDescription"));

                gridData.Children.Add(labelActivityStringFkRefno, 0, 0);
                gridData.Children.Add(labelActivityReplenishmentDateTime, 0, 0);
                gridData.Children.Add(labelActivityReplenishmentDescription, 0, 0);

                Grid.SetColumn(labelActivityStringFkRefno, 0);
                Grid.SetColumn(labelActivityReplenishmentDateTime, 1);
                Grid.SetColumn(labelActivityReplenishmentDescription, 2);

                return new ViewCell { View = gridData };
            });

            this.ResDict.Add("dataTemplateFromReplenish", dataFromTemplate);
        }

        /// <summary>
        /// Gets or sets the replenishment settings.
        /// </summary>
        public ReplenishmentSettings ReplenSettings { get; set; }

        /// <summary>
        /// Gets or sets the list view / from grid column 0 width.
        /// </summary>
        public double FromColWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / from grid column 1 width.
        /// </summary>
        public double FromColWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / from grid column 2 width.
        /// </summary>
        public double FromColWidth2 { get; set; } = 0;

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

            var labelMoveType = new Label
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

            var labelFromAltQty = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            // Calculate text size
            this.FromColWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelMoveType.Text);
            this.FromColWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelFromLocation.Text);
            this.FromColWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelFromAltQty.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.FromColWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelMoveType);
            gridHeader.Children.Add(labelFromLocation);
            gridHeader.Children.Add(labelFromAltQty);

            Grid.SetColumn(labelMoveType, 0);
            Grid.SetColumn(labelFromLocation, 1);
            Grid.SetColumn(labelFromAltQty, 2);

            Grid.SetRow(gridHeader, 0);
            gridMainActivity.Children.Add(gridHeader);

            foreach (Replenishment replen in this.ListHdrs)
            {
                headersList.Add(replen);

                var dataFromColWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    ReplenishmentMoveType.Enum2String(replen.MoveType));
                var dataFromColWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    replen.ReadableLocationMap);
                var dataFromColWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    replen.ActivityItemReplenishmentQuantity.ToString());

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
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResDict["dataTemplateFromReplenish"];

            return headersList;
        }

        /// <summary>
        /// Get list of activity headers.
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
                select tActivityReplenishment.ActivityRefno
                        ,tActivityReplenishment.ActivitySubType
                        ,tActivityReplenishment.WarehouseLocationMapRefno
                        ,tActivityItemReplenishment.ActivityItemRefno
                        ,tActivityItemReplenishment.ActivityItemSubType
                        ,tActivityItemReplenishment.MoveType
                        ,tActivityItemReplenishment.ActivityItemFKRefno
                        ,tActivityItemReplenishment.ActivityItemFKParentRefno
                        ,tActivityItemReplenishment.ProductUnitRefno
                        ,tActivityItemReplenishment.LocationMapRefno
                        ,tActivityItemReplenishment.ActivityItemReplenishmentQuantity
                        ,tActivityItemReplenishment.ActivityItemReplenishmentBaseQuantity
                        ,tActivityItemReplenishment.ActivityItemReplenishmentWeightQuantity
                        ,tActivityDetailReplenishment.ActivityDetailFKRefno
                        ,tActivityDetailReplenishment.ActivityDetailReplenishmentIsConfirmed
                        ,tActivityDetail.ActivityDetailRefno
                        ,tActivityDetail.ActivityDetailSubType
                        ,tActivityDetail.ActivityDetailStatus
                        ,tActivityDetail.ActivityItemRefno
                        ,tActivityDetail.ActivityDetailUniqueSchemeID
                        ,tActivityDetail.ActivityDetailSchemeID
                        ,tActivityDetail.ActivityDetailQuantity
                        ,tActivityDetail.ActivityDetailBaseQuantity
                        ,tActivityDetail.ActivityDetailWeightQuantity
                        ,tActivityDetail.ActivityDetailSingleTradeItemSerialNumber
                        ,tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber
                        ,tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate
                        ,tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate
                        ,tActivityDetail.ActivityDetailLogisticUnitProductionDate
                        ,tActivityDetail.ActivityDetailLogisticUnitDueDate
                        ,tActivityDetail.ActivityDetailLogisticUnitPackagingDate
                        ,tActivityDetail.ActivityDetailLogisticUnitSkidNumber
                    from tActivityReplenishment
                    inner join tActivity on tActivity.ActivityRefno = tActivityReplenishment.ActivityRefno
                    inner join tActivityItem on tActivityItem.ActivityRefno = tActivity.ActivityRefno
                    inner join tActivityItemReplenishment on tActivityItemReplenishment.ActivityItemRefno = tActivityItem.ActivityItemRefno
                    inner join tActivityDetail on tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno
                    inner join tActivityDetailReplenishment on tActivityDetailReplenishment.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno";

            this.ListReplenishments = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Replenishment>(query);
            this.ListHdrs = this.ListReplenishments.Cast<object>().ToList();
            return this.ListHdrs;
        }

        ////public async Task<List<object>> GetActivityItemList()
        ////{

        ////}

        /// <summary>
        /// Select the adjustment activity header.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Activity header to select.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Replenishment header)
            {
                this.SelectedActivity = header;
                ////this.HdrFkRefno = header.ActivityFKRefno;
                ////this.HdrRefno = header.ActivityRefno;

                ////if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                ////{
                ////    // Set the current location to the header.
                ////    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.WarehouseLocationMapRefno &&
                ////            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                ////    {
                ////        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                ////            header.LocationMapRefno);
                ////    }

                ////    // Override and set the chosen location to the current location.
                ////    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                ////            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                ////    {
                ////        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                ////    }

                ////    // Override and set any selected skid location to the current location.
                ////    if (this.SkidInfo.CurrSkid != null && this.SkidInfo.CurrSkid.SkidFKRefno > 0 &&
                ////        this.SkidInfo.CurrSkid.LocMapRefno != header.LocationMapRefno &&
                ////        await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                ////    {
                ////        // NOTE: We may have to use a temp var to assign the current skid a new location.
                ////        this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                ////    }
                ////}

                this.ReplenSettings = this.ReplenSettings.LoadData();
            }
        }

        /// <summary>
        /// Get the activity reference number for the selected object.
        /// </summary>
        /// <param name="selectedItem"><see cref="object"/> Selected item.</param>
        /// <returns><see cref="int"/> Selected activity reference number.</returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            ////if (selectedItem is Replenishment adjustment)
            ////{
            ////    this.HdrFkRefno = Convert.ToInt32(adjustment.ActivityFKRefno);
            ////    this.HdrRefno = Convert.ToInt32(adjustment.ActivityRefno);
            ////}

            return this.HdrRefno;
        }

        /// <summary>
        /// Refresh the activity headers.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Activity header filter criteria.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);
                await this.Purge();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                var replen = new TableActivityReplenishment();
                replen.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                await replen.RequestHeaderData();
                replen.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
            }
        }

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type (tActivity??Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            object tmpFkRefno = ((Replenishment)resourceType).ActivityItemFKRefno;
            return tmpFkRefno == null ? 0 : Convert.ToInt32(tmpFkRefno);
        }

        /// <summary>
        /// Insert, create a new activity replenishment header.
        /// </summary>
        /// <param name="xmlParams">The xmlParams<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public override Task<int> Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the replenishment activity header.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get shipping label quantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

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

        /// <summary>
        /// Print labels.
        /// </summary>
        /// <param name="xmlParam"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"> Not implemented exception.</exception>
        public override Task PrintLabel(XDocument xmlParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change activity status.
        /// </summary>
        /// <param name="selectedActivity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"> Not implemented exception.</exception>
        public override Task<bool> ChangeStatus(object selectedActivity)
        {
            throw new NotImplementedException();
        }
    }
}
