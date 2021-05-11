namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;

    public class RecvActivity : Activity
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        /// 
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
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                    }
                };

                Label labelActivityReceivingPONumber = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityReceivingIssuedDateTime = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityReceivingArrivalDateTime = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelActivityReceivingPONumber.SetBinding(Label.TextProperty, new Binding("ActivityReceivingPONumber"));
                labelActivityReceivingIssuedDateTime.SetBinding(Label.TextProperty, new Binding("ActivityReceivingIssuedDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityReceivingArrivalDateTime.SetBinding(Label.TextProperty, new Binding("ActivityReceivingArrivalDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));

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

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityReceivingStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Receiving";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            
            foreach (Receive receive in this.ListHdrs)
            {
                //Receive receive = new Receive
                //{
                //    ActivityReceivingPONumber = Convert.ToString(row["ActivityReceivingPONumber"]),
                //    ActivityReceivingIssuedDateTime = Convert.ToDateTime(row["ActivityReceivingIssuedDateTime"]),
                //    ActivityReceivingArrivalDateTime = Convert.ToDateTime(row["ActivityReceivingArrivalDateTime"])
                //};

                observableCollection.Add(receive);
            }

            // TODO: Resource Strings
            Label labelPOReceiving = new Label { Text = "PO #", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelIssuedReceiving = new Label { Text = "Issued", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelArrivedReceiving = new Label { Text = "Arrived", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPOReceiving.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIssuedReceiving.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelArrivedReceiving.Text);

            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelPOReceiving);
            gridHeader.Children.Add(labelIssuedReceiving);
            gridHeader.Children.Add(labelArrivedReceiving);

            Grid.SetColumn(labelPOReceiving, 0);
            Grid.SetColumn(labelIssuedReceiving, 1);
            Grid.SetColumn(labelArrivedReceiving, 2);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);
            
            foreach (Receive receive in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(receive.ActivityReceivingPONumber);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(receive.ActivityReceivingIssuedDateTime.ToString("MM/dd/yyyy"));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(receive.ActivityReceivingArrivalDateTime.ToString("MM/dd/yyyy"));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="IsSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
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

            //using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            //        this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        // TODO: Fix sorting and filter
            //        this.DataViewHdrs.Sort = receiveSettings.GetDataViewSortBy();
            //        this.DataViewHdrs.RowFilter = this.GetFilter();
            //    }
            //}

            this.ListHdrs = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<object>(sql.ToString());
            return this.ListHdrs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActFilter))
            {
                //this.DataViewHdrs.RowFilter = "ActivityReceivingPONumber LIKE \'%" + searchText.Trim() + "%\'";
                this.ListHdrs = (List<object>)this.ListHdrs.OfType<Receive>().Where(receive => receive.ActivityReceivingPONumber.Contains(searchText.Trim()));
            }
            else
            {
                // TODO: Fix filtering
                //this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityReceivingPONumber LIKE \'%" + searchText.Trim() + "%\'";
            }

            foreach (Receive receive in this.ListHdrs)
            {
                //Receive receive = new Receive
                //{
                //    ActivityReceivingPONumber = Convert.ToString(row["ActivityReceivingPONumber"]),
                //    ActivityReceivingIssuedDateTime = Convert.ToDateTime(row["ActivityReceivingIssuedDateTime"]),
                //    ActivityReceivingArrivalDateTime = Convert.ToDateTime(row["ActivityReceivingArrivalDateTime"])
                //};

                observableCollection.Add(receive);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Receive receive)
            {
                this.SelectedActivity = receive;
                this.FKRefno = Convert.ToInt32(receive.ActivityReceivingFKRefno);
                this.Refno = Convert.ToInt32(receive.ActivityRefno);
                this.PONO = Convert.ToString(receive.ActivityReceivingPONumber);

                // Set the selected warehouse to the activity header warehouse.
                if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(receive.WarehouseLocationMapRefno))
                {
                    LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(receive.WarehouseLocationMapRefno));
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            if (selectedItem is Receive receive)
            {
                this.FKRefno = Convert.ToInt32(receive.ActivityReceivingFKRefno);
                this.Refno = Convert.ToInt32(receive.ActivityRefno);
                this.PONO = Convert.ToString(receive.ActivityReceivingPONumber);
            }

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
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
                tableActReceiving.Request();
                tableActReceiving.UnregisterDataAccessListenerProgressOut((RecvActivity)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            TableActivityReceiving tableActReceiving = new TableActivityReceiving();
            TableActivityRecevingItem tableActReceivingItem = new TableActivityRecevingItem();
            TableActivityReceivingDetail tableActReceivingDetail = new TableActivityReceivingDetail();

            tableActReceivingDetail.Purge();
            tableActReceivingItem.Purge();
            tableActReceiving.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            string filter = string.Empty;
            ReceivingSettings recvSettings = new ReceivingSettings().LoadData();

            if (recvSettings.IsApplyIssuedDateFilter)
            {
                if (!SysCommon.IsEmptyDate(recvSettings.dtIssuedDateFilter))
                {
                    DateTime dt = recvSettings.dtIssuedDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityReceivingIssuedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingIssuedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityReceivingIssuedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingIssuedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            if (recvSettings.IsApplyArrivalDateFilter)
            {
                if (!SysCommon.IsEmptyDate(recvSettings.dtArrivalDateFilter))
                {
                    DateTime dt = recvSettings.dtArrivalDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityReceivingArrivalDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingArrivalDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityReceivingArrivalDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingArrivalDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            this.ActFilter = filter;

            return filter;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            string filter = string.Empty;
            ReceivingSettings recvSettings = new ReceivingSettings().LoadData();

            if (recvSettings.IsApplyIssuedDateFilter)
            {
                if ((string.Compare(data.Root.Element("issued_date").Value, string.Empty) > 0))
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("issued_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityReceivingIssuedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingIssuedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityReceivingIssuedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingIssuedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            if (recvSettings.IsApplyArrivalDateFilter)
            {
                if ((string.Compare(data.Root.Element("arrival_date").Value, string.Empty) > 0))
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("arrival_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityReceivingArrivalDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingArrivalDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityReceivingArrivalDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityReceivingArrivalDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            this.ActFilter = filter;

            return filter;
        }

        #endregion
    }
}
