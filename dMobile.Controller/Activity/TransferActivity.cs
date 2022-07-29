// <copyright file="TransferActivity.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TransferActivity" />.
    /// </summary>
    public class TransferActivity : Activity
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
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listTransfers.
        /// </summary>
        private List<Transfer> listTransfers = new List<Transfer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferActivity"/> class.
        /// </summary>
        public TransferActivity()
            : base()
        {
            this.FullClassName = typeof(TransferActivity).FullName;
            this.TransSettings = new TransferSettings().LoadData();

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
                        new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    },
                };

                Label labelTransferNumber = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWhs = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelDate = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelArrived = new Label { VerticalOptions = LayoutOptions.Center };

                labelTransferNumber.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
                labelWhs.SetBinding(Label.TextProperty, new Binding("LocationMapWarehouseFKRefno"));
                labelDate.SetBinding(Label.TextProperty, new Binding("ActivityTransferDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelArrived.SetBinding(Label.TextProperty, new Binding("ActivityTransferArrivalDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));

                gridData.Children.Add(labelTransferNumber);
                gridData.Children.Add(labelWhs);
                gridData.Children.Add(labelDate);
                gridData.Children.Add(labelArrived);

                Grid.SetColumn(labelTransferNumber, 0);
                Grid.SetColumn(labelWhs, 1);
                Grid.SetColumn(labelDate, 2);
                Grid.SetColumn(labelArrived, 3);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        /// <summary>
        /// Gets or sets the TransSettings.
        /// </summary>
        public TransferSettings TransSettings { get; set; }

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFKRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Transfers";
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (Transfer transfer in this.ListHdrs)
            {
                // Transfer transfer = new Transfer
                // {
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    ActivityTransferArrivalDateTime = Convert.ToDateTime(row["ActivityTransferArrivalDateTime"] != DBNull.Value ? row["ActivityTransferArrivalDateTime"] : null),
                //    ActivityTransferDateTime = Convert.ToDateTime(row["ActivityTransferDateTime"] != DBNull.Value ? row["ActivityTransferDateTime"] : null),
                //    ActivityTransferDescription = Convert.ToString(row["ActivityTransferDescription"] != DBNull.Value ? row["ActivityTransferDescription"] : null),
                //    ActivityFKRefno = Convert.ToInt32(row["ActivityFKRefno"] != DBNull.Value ? row["ActivityFKRefno"] : null),
                //    ActivityStringFKRefno = Convert.ToString(row["ActivityStringFKRefno"] != DBNull.Value ? row["ActivityStringFKRefno"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null)
                // };
                observableCollection.Add(transfer);
            }

            Label labelTransferNumberHeader = new Label { Text = "Transfer #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWhsHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelDateHeader = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelArrivedHeader = new Label { Text = "Arrived", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTransferNumberHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelDateHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelArrivedHeader.Text);

            // Create header data template
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelTransferNumberHeader);
            gridHeader.Children.Add(labelWhsHeader);
            gridHeader.Children.Add(labelDateHeader);
            gridHeader.Children.Add(labelArrivedHeader);

            Grid.SetColumn(labelTransferNumberHeader, 0);
            Grid.SetColumn(labelWhsHeader, 1);
            Grid.SetColumn(labelDateHeader, 2);
            Grid.SetColumn(labelArrivedHeader, 3);

            Grid.SetRow(gridHeader, 0);

            GridMainActivity.Children.Add(gridHeader);

            foreach (Transfer transfer in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(transfer.ActivityStringFKRefno);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(transfer.LocationMapWarehouseFKRefno));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(transfer.ActivityTransferDateTime.ToString("MM/dd/yyyy"));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(transfer.ActivityTransferArrivalDateTime.ToString("MM/dd/yyyy"));

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

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
                }
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityTransfer.ActivityRefno,");
                sql.Append("tActivityTransfer.ActivityFKRefno,");
                sql.Append("tActivityTransfer.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime AS ActivityTransferArrivalDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDateTime AS ActivityTransferDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDescription AS ActivityTransferDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityTransfer ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityTransfer.ActivityRefno,");
                sql.Append("tActivityTransfer.ActivityFKRefno,");
                sql.Append("tActivityTransfer.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityTransfer ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityTransfer.ActivityRefno,");
                sql.Append("tActivityTransfer.ActivityFKRefno,");
                sql.Append("tActivityTransfer.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityTransfer ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivityTransfer.ActivitySubType = {0}", Convert.ToInt32(this.ActSubType));
            }
            else
            {
                sql.Append("SELECT tActivityTransfer.ActivityRefno,");
                sql.Append("tActivityTransfer.ActivityFKRefno,");
                sql.Append("tActivityTransfer.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime AS ActivityTransferArrivalDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDateTime AS ActivityTransferDateTime,");
                sql.Append("tActivityTransfer.ActivityTransferDescription AS ActivityTransferDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityTransfer ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno");
                sql.AppendFormat(" AND tActivityTransfer.ActivitySubType = {0}", Convert.ToInt32(this.ActSubType));
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            // this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        TransferSettings transferSettings = new TransferSettings().LoadData();
            //        // TODO: Fix sorting
            //        this.DataViewHdrs.Sort = transferSettings.GetDataViewSortBy();
            //    }
            // }
            this.listTransfers = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Transfer>(sql.ToString());

            foreach (Sort sort in new TransferSettings().LoadData().GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.listTransfers = this.listTransfers.OrderBy(transfer => transfer.GetType().GetProperty(sort.SortValue).GetValue(transfer)).ToList();
                }
                else
                {
                    this.listTransfers = this.listTransfers.OrderByDescending(transfer => transfer.GetType().GetProperty(sort.SortValue).GetValue(transfer)).ToList();
                }
            }

            this.GetFilter();

            this.ListHdrs = this.listTransfers.Cast<object>().ToList();
            return this.ListHdrs;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            this.ListHdrs = this.listTransfers.Where(transfer => transfer.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (Transfer transfer in this.ListHdrs)
            {
                // Transfer transfer = new Transfer
                // {
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    ActivityTransferArrivalDateTime = Convert.ToDateTime(row["ActivityTransferArrivalDateTime"] != DBNull.Value ? row["ActivityTransferArrivalDateTime"] : null),
                //    ActivityTransferDateTime = Convert.ToDateTime(row["ActivityTransferDateTime"] != DBNull.Value ? row["ActivityTransferDateTime"] : null),
                //    ActivityTransferDescription = Convert.ToString(row["ActivityTransferDescription"] != DBNull.Value ? row["ActivityTransferDescription"] : null),
                //    ActivityFKRefno = Convert.ToInt32(row["ActivityFKRefno"] != DBNull.Value ? row["ActivityFKRefno"] : null),
                //    ActivityStringFKRefno = Convert.ToString(row["ActivityStringFKRefno"] != DBNull.Value ? row["ActivityStringFKRefno"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null)
                // };
                observableCollection.Add(transfer);
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
            if (selectedActivity is Transfer header)
            {
                this.SelectedActivity = header;
                this.HdrFKRefno = header.ActivityFKRefno;
                this.HdrRefno = header.ActivityRefno;

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            header.LocationMapRefno);
                    }

                    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }

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
        /// The GetRefnoFromRowIdx.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            if (selectedItem is Transfer transfer)
            {
                this.HdrFKRefno = Convert.ToInt32(transfer.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(transfer.ActivityRefno);
            }

            return this.HdrRefno;
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
                await this.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    TableActivityTransferFrom tableActTransfer = new TableActivityTransferFrom();
                    tableActTransfer.RegisterDataAccessListenerProgressOut((TransferActivity)this);
                    await tableActTransfer.RequestData();
                    tableActTransfer.UnregisterDataAccessListenerProgressOut((TransferActivity)this);
                }
                else
                {
                    TableActivityTransferTo tableActTransfer = new TableActivityTransferTo();
                    tableActTransfer.RegisterDataAccessListenerProgressOut((TransferActivity)this);
                    await tableActTransfer.RequestData();
                    tableActTransfer.UnregisterDataAccessListenerProgressOut((TransferActivity)this);
                }
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
            {
                TableActivityTransferFrom tableActTransfer = new TableActivityTransferFrom();
                TableActivityTransferFromItem tableActTransferItem = new TableActivityTransferFromItem();
                TableActivityTransferFromDetail tableActTransferDetail = new TableActivityTransferFromDetail();
                await tableActTransferDetail.PurgeData();
                await tableActTransferItem.PurgeData();
                await tableActTransfer.PurgeData();
            }
            else
            {
                TableActivityTransferTo tableActTransfer = new TableActivityTransferTo();
                TableActivityTransferToItem tableActTransferItem = new TableActivityTransferToItem();
                TableActivityTransferToDetail tableActTransferDetail = new TableActivityTransferToDetail();
                await tableActTransferDetail.PurgeData();
                await tableActTransferItem.PurgeData();
                await tableActTransfer.PurgeData();
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
            TransferSettings transferSettings = new TransferSettings().LoadData();

            if (transferSettings.IsApplyTransferDateFilter)
            {
                if (!SysCommon.IsEmptyDate(transferSettings.TransferDateFilter))
                {
                    DateTime dt = transferSettings.TransferDateFilter;
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listTransfers = this.listTransfers.Where(transfer => transfer.ActivityTransferDateTime >= minDate && transfer.ActivityTransferDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">.</param>
        public override void GetFilter(XDocument data)
        {
            TransferSettings transferSettings = new TransferSettings().LoadData();

            if (transferSettings.IsApplyTransferDateFilter)
            {
                if (string.Compare(data.Root.Element("transfer_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("transfer_date").Value);
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listTransfers = this.listTransfers.Where(transfer => transfer.ActivityTransferDateTime >= minDate && transfer.ActivityTransferDateTime <= maxDate).ToList();
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
            object tmpFKRefno = ((Transfer)resourceType).ActivityFKRefno;
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
