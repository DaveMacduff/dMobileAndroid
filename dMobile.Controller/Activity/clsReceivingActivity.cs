// <copyright file="clsReceivingActivity.cs" company="InterSect Business Systems Inc">
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
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="RecvActivity" />.
    /// </summary>
    public class RecvActivity : Activity
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
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listReceives.
        /// </summary>
        private List<Receive> listReceives = new List<Receive>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RecvActivity"/> class.
        /// </summary>
        public RecvActivity()
          : base()
        {
            this.FullClassName = typeof(RecvActivity).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    },
                };

                Label labelActivityReceivingPONumber = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                Label labelActivityReceivingIssuedDateTime = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                Label labelActivityReceivingArrivalDateTime = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                labelActivityReceivingPONumber.SetBinding(Label.TextProperty, new Binding("ActivityReceivingPONumber"));
                labelActivityReceivingIssuedDateTime.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityReceivingIssuedDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityReceivingArrivalDateTime.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityReceivingArrivalDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));

                gridData.Children.Add(labelActivityReceivingPONumber);
                gridData.Children.Add(labelActivityReceivingIssuedDateTime);
                gridData.Children.Add(labelActivityReceivingArrivalDateTime);

                Grid.SetColumn(labelActivityReceivingPONumber, 0);
                Grid.SetColumn(labelActivityReceivingIssuedDateTime, 1);
                Grid.SetColumn(labelActivityReceivingArrivalDateTime, 2);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        /// <summary>
        /// Gets or sets the PONO.
        /// </summary>
        public string PONO { get; set; } = string.Empty;

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFKRefnoIdx()
        {
            return "ActivityReceivingStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Receiving";
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
        {
            Label labelPOReceiving = new Label
            {
                Text = "PO #",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelIssuedReceiving = new Label
            {
                Text = "Issued",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelArrivedReceiving = new Label
            {
                Text = "Arrived",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPOReceiving.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIssuedReceiving.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelArrivedReceiving.Text);
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelPOReceiving);
            gridHeader.Children.Add(labelIssuedReceiving);
            gridHeader.Children.Add(labelArrivedReceiving);

            Grid.SetColumn(labelPOReceiving, 0);
            Grid.SetColumn(labelIssuedReceiving, 1);
            Grid.SetColumn(labelArrivedReceiving, 2);

            Grid.SetRow(gridHeader, 0);

            GridMainActivity.Children.Add(gridHeader);

            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            foreach (Receive receive in this.ListHdrs)
            {
                observableCollection.Add(receive);
            }

            foreach (Receive receive in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    receive.ActivityReceivingPONumber);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    receive.ActivityReceivingIssuedDateTime.ToString("MM/dd/yyyy"));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    receive.ActivityReceivingArrivalDateTime.ToString("MM/dd/yyyy"));

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            ReceivingSettings receiveSettings = new ReceivingSettings().LoadData();
            LocationMap locMap = LocationMap.GetInstance();

            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityReceiving.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingFKRefno AS ActivityReceivingFKRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingStringFKRefno AS ActivityReceivingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityReceiving.ActivityReceivingPONumber AS ActivityReceivingPONumber,");
                sql.Append("tActivityReceiving.ActivityReceivingIssuedDateTime AS ActivityReceivingIssuedDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingArrivalDateTime AS ActivityReceivingArrivalDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorContactName AS ActivityReceivingVendorContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierContactName AS ActivityReceivingVendorSupplierContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierGTINCompanyPrefix AS ActivityReceivingVendorSupplierGTINCompanyPrefix,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityReceiving ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReceiving.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityReceiving.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityReceiving.ActivityRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingFKRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityReceiving.ActivityReceivingPONumber,");
                sql.Append("tActivityReceiving.ActivityReceivingIssuedDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingArrivalDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierGTINCompanyPrefix,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityReceiving ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReceiving.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityReceiving.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus = {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityReceiving.ActivityRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingFKRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityReceiving.ActivityReceivingPONumber,");
                sql.Append("tActivityReceiving.ActivityReceivingIssuedDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingArrivalDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierGTINCompanyPrefix,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityReceiving ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReceiving.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityReceiving.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ");
                sql.Append("ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus = {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus = {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
            }
            else
            {
                sql.Append("SELECT tActivityReceiving.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingFKRefno AS ActivityReceivingFKRefno,");
                sql.Append("tActivityReceiving.ActivityReceivingStringFKRefno AS ActivityReceivingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityReceiving.ActivityReceivingPONumber AS ActivityReceivingPONumber,");
                sql.Append("tActivityReceiving.ActivityReceivingIssuedDateTime AS ActivityReceivingIssuedDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingArrivalDateTime AS ActivityReceivingArrivalDateTime,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorContactName AS ActivityReceivingVendorContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierContactName AS ActivityReceivingVendorSupplierContactName,");
                sql.Append("tActivityReceiving.ActivityReceivingVendorSupplierGTINCompanyPrefix AS ActivityReceivingVendorSupplierGTINCompanyPrefix,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityReceiving ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReceiving.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityReceiving.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityReceiving.WarehouseLocationMapRefno = {0} ", locMap.LoginWhsLocMapRefno);
                sql.AppendFormat("AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            // this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        // TODO: Fix sorting and filter
            //        this.DataViewHdrs.Sort = receiveSettings.GetDataViewSortBy();
            //        this.DataViewHdrs.RowFilter = this.GetFilter();
            //    }
            // }
            this.listReceives = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Receive>(sql.ToString());

            foreach (Sort sort in receiveSettings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.listReceives = this.listReceives.OrderBy(receive => receive.GetType().GetProperty(sort.SortValue).GetValue(receive)).ToList();
                }
                else
                {
                    this.listReceives = this.listReceives.OrderByDescending(receive => receive.GetType().GetProperty(sort.SortValue).GetValue(receive)).ToList();
                }
            }

            this.GetFilter();

            this.ListHdrs = this.listReceives.Cast<object>().ToList();
            return this.ListHdrs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            this.ListHdrs = this.listReceives.Where(receive => receive.ActivityReceivingPONumber.ToLower().Contains(
                searchText.ToLower().Trim())).Cast<object>().ToList();
            foreach (Receive receive in this.ListHdrs)
            {
                observableCollection.Add(receive);
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
            if (selectedActivity is Receive receive)
            {
                this.SelectedActivity = receive;
                this.HdrFKRefno = Convert.ToInt32(receive.ActivityReceivingFKRefno);
                this.HdrRefno = Convert.ToInt32(receive.ActivityRefno);
                this.PONO = Convert.ToString(receive.ActivityReceivingPONumber);

                // Set the selected warehouse to the activity header warehouse.
                if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(receive.WarehouseLocationMapRefno))
                {
                    LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(receive.WarehouseLocationMapRefno));
                }
            }
        }

        /// <summary>
        /// The GetRefnoFromRowIdx.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            if (selectedItem is Receive receive)
            {
                this.HdrFKRefno = Convert.ToInt32(receive.ActivityReceivingFKRefno);
                this.HdrRefno = Convert.ToInt32(receive.ActivityRefno);
                this.PONO = Convert.ToString(receive.ActivityReceivingPONumber);
            }

            return this.HdrRefno;
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
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);
                await this.Purge();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                TableActivityReceiving tableActReceiving = new TableActivityReceiving();

                tableActReceiving.RegisterDataAccessListenerProgressOut((RecvActivity)this);
                await tableActReceiving.RequestData();
                tableActReceiving.UnregisterDataAccessListenerProgressOut((RecvActivity)this);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            TableActivityReceiving tableActReceiving = new TableActivityReceiving();
            TableActivityRecevingItem tableActReceivingItem = new TableActivityRecevingItem();
            TableActivityReceivingDetail tableActReceivingDetail = new TableActivityReceivingDetail();

            await tableActReceivingDetail.PurgeData();
            await tableActReceivingItem.PurgeData();
            await tableActReceiving.PurgeData();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
            ReceivingSettings recvSettings = new ReceivingSettings().LoadData();

            if (recvSettings.IsApplyIssuedDateFilter)
            {
                if (!SysCommon.IsEmptyDate(recvSettings.dtIssuedDateFilter))
                {
                    DateTime dt = recvSettings.dtIssuedDateFilter;
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listReceives = this.listReceives.Where(receive => receive.ActivityReceivingIssuedDateTime >= minDate && receive.ActivityReceivingIssuedDateTime <= maxDate).ToList();
                }
            }

            if (recvSettings.IsApplyArrivalDateFilter)
            {
                if (!SysCommon.IsEmptyDate(recvSettings.dtArrivalDateFilter))
                {
                    DateTime dt = recvSettings.dtArrivalDateFilter;
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listReceives = this.listReceives.Where(receive => receive.ActivityReceivingArrivalDateTime >= minDate && receive.ActivityReceivingArrivalDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        public override void GetFilter(XDocument data)
        {
            ReceivingSettings recvSettings = new ReceivingSettings().LoadData();

            if (recvSettings.IsApplyIssuedDateFilter)
            {
                if (string.Compare(data.Root.Element("issued_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("issued_date").Value);
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listReceives = this.listReceives.Where(receive => receive.ActivityReceivingIssuedDateTime >= minDate && receive.ActivityReceivingIssuedDateTime <= maxDate).ToList();
                }
            }

            if (recvSettings.IsApplyArrivalDateFilter)
            {
                if (string.Compare(data.Root.Element("arrival_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("arrival_date").Value);
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listReceives = this.listReceives.Where(receive => receive.ActivityReceivingArrivalDateTime >= minDate && receive.ActivityReceivingArrivalDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model (tActivity??Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            object tmpFKRefno = ((Receive)resourceType).ActivityReceivingFKRefno;
            return tmpFKRefno == null ? 0 : Convert.ToInt32(tmpFKRefno);
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="xmlParams">The xmlParams<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
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
