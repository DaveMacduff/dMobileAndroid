// <copyright file="ReturnsActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="ReturnsActivity" />.
    /// </summary>
    public class ReturnsActivity : Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnsActivity"/> class.
        /// </summary>
        public ReturnsActivity()
          : base()
        {
            this.FullClassName = typeof(ReturnsActivity).FullName;

            // Data grid
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
                    },
                };

                var stringFkRefno = new Label { VerticalOptions = LayoutOptions.Center };
                var orderedDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                var deliveryDateTime = new Label { VerticalOptions = LayoutOptions.Center };

                stringFkRefno.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
                orderedDateTime.SetBinding(Label.TextProperty, new Binding(
                    "ActivityReturnsOrderedDateTime",
                    BindingMode.Default,
                    null,
                    null,
                    "{0:MM/dd/yyyy}"));
                deliveryDateTime.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityReturnsDeliveryDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));

                gridData.Children.Add(stringFkRefno);
                gridData.Children.Add(orderedDateTime);
                gridData.Children.Add(deliveryDateTime);

                Grid.SetColumn(stringFkRefno, 0);
                Grid.SetColumn(orderedDateTime, 1);
                Grid.SetColumn(deliveryDateTime, 2);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplateReturns")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplateReturns", dataTemplate);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        public double ColumnWidth2 { get; set; } = 0;

        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        public List<Return> ListReturns { get; set; } = new List<Return>();

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Returns";
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The gridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The listViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid gridMainActivity,
            ref ListView listViewMainActivity)
        {
            var orderReturns = new ObservableCollection<object>();

            foreach (Return orderReturn in this.ListHeaders)
            {
                orderReturns.Add(orderReturn);
            }

            var returnNumber = new Label
            {
                Text = "Return #",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var returnEntered = new Label
            {
                Text = "Entered",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var returnReturned = new Label
            {
                Text = "Returned",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(returnNumber.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(returnEntered.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(returnReturned.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(returnNumber);
            gridHeader.Children.Add(returnEntered);
            gridHeader.Children.Add(returnReturned);

            Grid.SetColumn(returnNumber, 0);
            Grid.SetColumn(returnEntered, 1);
            Grid.SetColumn(returnReturned, 2);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (Return orderReturn in orderReturns)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    orderReturn.ActivityStringFKRefno);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    orderReturn.ActivityReturnsOrderedDateTime.ToString("MM/dd/yyyy"));
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    orderReturn.ActivityReturnsDeliveryDateTime.ToString("MM/dd/yyyy"));

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
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateReturns"];

            return orderReturns;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityReturns.ActivityRefno,");
                sql.Append("tActivityReturns.ActivityFKRefno,");
                sql.Append("tActivityReturns.ActivityStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime AS ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime AS ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName AS ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo AS ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityReturns.ActivityRefno,");
                sql.Append("tActivityReturns.ActivityFKRefno,");
                sql.Append("tActivityReturns.ActivityStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityReturns.ActivityRefno,");
                sql.Append("tActivityReturns.ActivityFKRefno,");
                sql.Append("tActivityReturns.ActivityStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.Append(" INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno");
                sql.AppendFormat(" WHERE tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                sql.Append("SELECT tActivityReturns.ActivityRefno,");
                sql.Append("tActivityReturns.ActivityFKRefno,");
                sql.Append("tActivityReturns.ActivityStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime AS ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime AS ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName AS ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo AS ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
            }

            this.ListReturns = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Return>(sql.ToString());

            foreach (var sort in new ReturnsSettings().LoadData().GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.ListReturns = this.ListReturns.OrderBy(ret => ret.GetType().GetProperty(sort.SortValue).
                        GetValue(ret)).ToList();
                }
                else
                {
                    this.ListReturns = this.ListReturns.OrderByDescending(ret => ret.GetType().
                        GetProperty(sort.SortValue).GetValue(ret)).ToList();
                }
            }

            this.GetFilter();

            this.ListHeaders = this.ListReturns.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            this.ListHeaders = this.ListReturns.Where(returns => returns.ActivityStringFKRefno.ToLower().
                Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (Return returns in this.ListHeaders)
            {
                observableCollection.Add(returns);
            }

            return observableCollection;
        }

        /// <summary>
        /// The SelectActivity.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Return header)
            {
                this.SelectedActivity = header;
                this.HdrFkRefno = header.ActivityFKRefno;
                this.HdrRefno = header.ActivityRefno;
                this.LocationMapRefno = header.LocationMapRefno;

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // Set the current location to the header.
                    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            header.LocationMapRefno);
                    }

                    // Override and set the chosen location to the current location.
                    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }

                    // Override and set any selected skid location to the current location.
                    if (this.SkidInfo.CurrSkid != null && this.SkidInfo.CurrSkid.SkidFKRefno > 0 &&
                        this.SkidInfo.CurrSkid.LocMapRefno != header.LocationMapRefno &&
                        await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        // NOTE: We may have to use a temp var to assign the current skid a new location.
                        this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// The AssignRefnoFromActivity.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is Return returns)
            {
                this.HdrFkRefno = Convert.ToInt32(returns.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(returns.ActivityRefno);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
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
                await this.Purge();
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                var tableActReturns = new TableActivityReturns();

                tableActReturns.RegisterDataAccessListenerProgressOut((ReturnsActivity)this);
                await tableActReturns.RequestHeaderData();
                tableActReturns.UnregisterDataAccessListenerProgressOut((ReturnsActivity)this);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            var tableActReturns = new TableActivityReturns();
            var tableActReturnsItem = new TableActivityReturnsItem();
            var tableActReturnsDetail = new TableActivityReturnsDetail();

            await tableActReturnsDetail.PurgeData();
            await tableActReturnsItem.PurgeData();
            await tableActReturns.PurgeData();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        public override void GetFilter(XDocument data)
        {
            var returnSettings = new ReturnsSettings().LoadData();

            if (returnSettings.IsApplyReturnedDateFilter)
            {
                if (string.Compare(data.Root.Element("returned_date").Value, string.Empty) > 0)
                {
                    this.ListReturns = this.ListReturns.Where(
                        ret => ret.ActivityReturnsDeliveryDateTime ==
                        DateTime.Parse(data.Root.Element("returned_date").Value)).ToList();
                }
            }

            if (returnSettings.IsApplyEnteredDateFilter)
            {
                if (string.Compare(data.Root.Element("entered_date").Value, string.Empty) > 0)
                {
                    this.ListReturns = this.ListReturns.Where(
                        ret => ret.ActivityReturnsOrderedDateTime ==
                        DateTime.Parse(data.Root.Element("entered_date").Value)).ToList();
                }
            }
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="xmlParams">The xmlParams<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public override Task<int> Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PrintLabel.
        /// </summary>
        /// <param name="xmlParam">The xmlParam<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task PrintLabel(XDocument xmlParam)
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

        /// <summary>
        /// The ChangeStatus.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public override Task<bool> ChangeStatus(object selectedActivity)
        {
            throw new NotImplementedException();
        }
    }
}
