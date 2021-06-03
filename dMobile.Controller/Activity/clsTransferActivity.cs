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

    public class TransferActivity : Activity
    {
        private TransferSettings transSettings;
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<Transfer> listTransfers = new List<Transfer>();

        #region Constructors

        /// <summary>
        /// 
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
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                    }
                };

                Label labelTransferNumber = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWhs = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelDate = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelArrived = new Label { VerticalOptions = LayoutOptions.Center };

                labelTransferNumber.SetBinding(Label.TextProperty, new Binding("ActivityTransferStringFKRefno"));
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

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public TransferSettings TransSettings
        {
            get
            {
                return this.transSettings;
            }

            set
            {
                this.transSettings = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityTransferStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Transfers";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (Transfer transfer in this.ListHdrs)
            {
                //Transfer transfer = new Transfer
                //{
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    ActivityTransferArrivalDateTime = Convert.ToDateTime(row["ActivityTransferArrivalDateTime"] != DBNull.Value ? row["ActivityTransferArrivalDateTime"] : null),
                //    ActivityTransferDateTime = Convert.ToDateTime(row["ActivityTransferDateTime"] != DBNull.Value ? row["ActivityTransferDateTime"] : null),
                //    ActivityTransferDescription = Convert.ToString(row["ActivityTransferDescription"] != DBNull.Value ? row["ActivityTransferDescription"] : null),
                //    ActivityTransferFKRefno = Convert.ToInt32(row["ActivityTransferFKRefno"] != DBNull.Value ? row["ActivityTransferFKRefno"] : null),
                //    ActivityTransferStringFKRefno = Convert.ToString(row["ActivityTransferStringFKRefno"] != DBNull.Value ? row["ActivityTransferStringFKRefno"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null)
                //};

                observableCollection.Add(transfer);
            }

            // TODO: Resource Strings
            Label labelTransferNumberHeader = new Label { Text = "Transfer #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWhsHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelDateHeader = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelArrivedHeader = new Label { Text = "Arrived", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTransferNumberHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelDateHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelArrivedHeader.Text);

            // Create header data template
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                }
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

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (Transfer transfer in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(transfer.ActivityTransferStringFKRefno);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(transfer.LocationMapWarehouseFKRefno));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(transfer.ActivityTransferDateTime.ToString("MM/dd/yyyy"));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(transfer.ActivityTransferArrivalDateTime.ToString("MM/dd/yyyy"));

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

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityTransfer.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityTransfer.ActivityTransferFKRefno AS ActivityTransferFKRefno,");
                sql.Append("tActivityTransfer.ActivityTransferStringFKRefno AS ActivityTransferStringFKRefno,");
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
                sql.Append("tActivityTransfer.ActivityTransferFKRefno,");
                sql.Append("tActivityTransfer.ActivityTransferStringFKRefno,");
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
                sql.Append("tActivityTransfer.ActivityTransferFKRefno,");
                sql.Append("tActivityTransfer.ActivityTransferStringFKRefno,");
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
                sql.Append("SELECT tActivityTransfer.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityTransfer.ActivityTransferFKRefno AS ActivityTransferFKRefno,");
                sql.Append("tActivityTransfer.ActivityTransferStringFKRefno AS ActivityTransferStringFKRefno,");
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

            //using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            //        this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        TransferSettings transferSettings = new TransferSettings().LoadData();
            //        // TODO: Fix sorting
            //        this.DataViewHdrs.Sort = transferSettings.GetDataViewSortBy();
            //    }
            //}

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
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            this.ListHdrs = this.listTransfers.Where(transfer => transfer.ActivityTransferStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (Transfer transfer in this.ListHdrs)
            {
                //Transfer transfer = new Transfer
                //{
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    ActivityTransferArrivalDateTime = Convert.ToDateTime(row["ActivityTransferArrivalDateTime"] != DBNull.Value ? row["ActivityTransferArrivalDateTime"] : null),
                //    ActivityTransferDateTime = Convert.ToDateTime(row["ActivityTransferDateTime"] != DBNull.Value ? row["ActivityTransferDateTime"] : null),
                //    ActivityTransferDescription = Convert.ToString(row["ActivityTransferDescription"] != DBNull.Value ? row["ActivityTransferDescription"] : null),
                //    ActivityTransferFKRefno = Convert.ToInt32(row["ActivityTransferFKRefno"] != DBNull.Value ? row["ActivityTransferFKRefno"] : null),
                //    ActivityTransferStringFKRefno = Convert.ToString(row["ActivityTransferStringFKRefno"] != DBNull.Value ? row["ActivityTransferStringFKRefno"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null)
                //};

                observableCollection.Add(transfer);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Transfer transfer)
            {
                this.SelectedActivity = transfer;
                this.FKRefno = Convert.ToInt32(transfer.ActivityTransferFKRefno);
                this.Refno = Convert.ToInt32(transfer.ActivityRefno);

                // Set the selected warehouse to the activity header warehouse.
                if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(transfer.LocationMapRefno))
                {
                    LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(transfer.LocationMapRefno));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            if (selectedItem is Transfer transfer)
            {
                this.FKRefno = Convert.ToInt32(transfer.ActivityTransferFKRefno);
                this.Refno = Convert.ToInt32(transfer.ActivityRefno);
            }

            return this.Refno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
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
                    await tableActTransfer.Request();
                    tableActTransfer.UnregisterDataAccessListenerProgressOut((TransferActivity)this);
                }
                else
                {
                    TableActivityTransferTo tableActTransfer = new TableActivityTransferTo();
                    tableActTransfer.RegisterDataAccessListenerProgressOut((TransferActivity)this);
                    await tableActTransfer.Request();
                    tableActTransfer.UnregisterDataAccessListenerProgressOut((TransferActivity)this);
                }

            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
            {
                TableActivityTransferFrom tableActTransfer = new TableActivityTransferFrom();
                TableActivityTransferFromItem tableActTransferItem = new TableActivityTransferFromItem();
                TableActivityTransferFromDetail tableActTransferDetail = new TableActivityTransferFromDetail();
                await tableActTransferDetail.Purge();
                await tableActTransferItem.Purge();
                await tableActTransfer.Purge();
            }
            else
            {
                TableActivityTransferTo tableActTransfer = new TableActivityTransferTo();
                TableActivityTransferToItem tableActTransferItem = new TableActivityTransferToItem();
                TableActivityTransferToDetail tableActTransferDetail = new TableActivityTransferToDetail();
                await tableActTransferDetail.Purge();
                await tableActTransferItem.Purge();
                await tableActTransfer.Purge();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        #endregion
    }
}