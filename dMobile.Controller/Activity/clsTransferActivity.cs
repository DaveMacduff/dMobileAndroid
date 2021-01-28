namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class TransferActivity : Activity
    {
        private TransferSettings transSettings;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TransferActivity()
            : base()
        {
            this.FullClassName = typeof(TransferActivity).FullName;
            this.TransSettings = new TransferSettings().LoadData();
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
            
            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Transfer transfer = new Transfer
                {
                    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                    ActivityTransferArrivalDateTime = Convert.ToDateTime(row["ActivityTransferArrivalDateTime"] != DBNull.Value ? row["ActivityTransferArrivalDateTime"] : null),
                    ActivityTransferDateTime = Convert.ToDateTime(row["ActivityTransferDateTime"] != DBNull.Value ? row["ActivityTransferDateTime"] : null),
                    ActivityTransferDescription = Convert.ToString(row["ActivityTransferDescription"] != DBNull.Value ? row["ActivityTransferDescription"] : null),
                    ActivityTransferFKRefno = Convert.ToInt32(row["ActivityTransferFKRefno"] != DBNull.Value ? row["ActivityTransferFKRefno"] : null),
                    ActivityTransferStringFKRefno = Convert.ToString(row["ActivityTransferStringFKRefno"] != DBNull.Value ? row["ActivityTransferStringFKRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null)
                };

                observableCollection.Add(transfer);
            }

            // TODO: Resource Strings
            Label labelTransferNumberHeader = new Label { Text = "Transfer #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWhsHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelDateHeader = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelArrivedHeader = new Label { Text = "Arrived", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            
            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTransferNumberHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelDateHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelArrivedHeader.Text);

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

            dataGridMainActivity.ItemTemplate = dataTemplate;

            return observableCollection;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <remarks></remarks>
        //public override void DesignDataGrid(DataGrid dataGridMain)
        //{
        //    int colWidthFactor = 1;

        //    if (this.SysEnv.IsWindowsMobile6Device)
        //    {
        //        colWidthFactor = 2;
        //    }

        //    if (dataGridMain.TableStyles.Count > 0)
        //    {
        //        dataGridMain.TableStyles.Clear();
        //    }

        //    DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //    dataGridStyle.MappingName = this.MainGridMappingName;

        //    DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ActivityTransferStringFKRefno";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Transferz);
        //    #else
        //        dataGridCol.HeaderText = "Transfer #";
        //    #endif

        //    dataGridCol.Width = 68 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "LocationMapWarehouseFKRefno";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Whs);
        //    #else
        //        dataGridCol.HeaderText = "Whs";
        //    #endif

        //    dataGridCol.Width = 35 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ActivityTransferDateTime";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
        //    #else
        //        dataGridCol.HeaderText = "Date";
        //    #endif

        //    dataGridCol.Width = 65 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ActivityTransferArrivalDateTime";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Arrived);
        //    #else
        //        dataGridCol.HeaderText = "Arrived";
        //    #endif

        //    dataGridCol.Width = 65 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridMain.TableStyles.Add(dataGridStyle);
        //    dataGridMain.RowHeadersVisible = false;
        //    dataGridMain.ColumnHeadersVisible = true;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
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

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);

                    this.DataViewHdrs = ds.Tables[0].DefaultView;
                    this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                    TransferSettings transferSettings = new TransferSettings().LoadData();
                    this.DataViewHdrs.Sort = transferSettings.GetDataViewSortBy();
                }
            }

            return this.DataViewHdrs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search( string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActFilter))
            {
                this.DataViewHdrs.RowFilter = "ActivityTransferStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityTransferStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }
            
            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Transfer transfer = new Transfer
                {
                    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                    ActivityTransferArrivalDateTime = Convert.ToDateTime(row["ActivityTransferArrivalDateTime"] != DBNull.Value ? row["ActivityTransferArrivalDateTime"] : null),
                    ActivityTransferDateTime = Convert.ToDateTime(row["ActivityTransferDateTime"] != DBNull.Value ? row["ActivityTransferDateTime"] : null),
                    ActivityTransferDescription = Convert.ToString(row["ActivityTransferDescription"] != DBNull.Value ? row["ActivityTransferDescription"] : null),
                    ActivityTransferFKRefno = Convert.ToInt32(row["ActivityTransferFKRefno"] != DBNull.Value ? row["ActivityTransferFKRefno"] : null),
                    ActivityTransferStringFKRefno = Convert.ToString(row["ActivityTransferStringFKRefno"] != DBNull.Value ? row["ActivityTransferStringFKRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null)
                };

                observableCollection.Add(transfer);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityTransferFKRefno"]);
            this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);
            this.SelectedIndex = idx;

            // Set the selected warehouse to the activity header warehouse.
            if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]))
            {
                LocationMap.GetInstance().CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = Convert.ToInt32(DataViewHdrs[idx]["ActivityTransferFKRefno"]);
            this.Refno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);

            return this.Refno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);
                this.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    TableActivityTransferFrom tableActTransfer = new TableActivityTransferFrom();
                    tableActTransfer.RegisterDataAccessListenerProgressOut((TransferActivity)this);
                    tableActTransfer.Request();
                    tableActTransfer.UnregisterDataAccessListenerProgressOut((TransferActivity)this);
                }
                else
                {
                    TableActivityTransferTo tableActTransfer = new TableActivityTransferTo();
                    tableActTransfer.RegisterDataAccessListenerProgressOut((TransferActivity)this);
                    tableActTransfer.Request();
                    tableActTransfer.UnregisterDataAccessListenerProgressOut((TransferActivity)this);
                }

            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
            {
                TableActivityTransferFrom tableActTransfer = new TableActivityTransferFrom();
                TableActivityTransferFromItem tableActTransferItem = new TableActivityTransferFromItem();
                TableActivityTransferFromDetail tableActTransferDetail = new TableActivityTransferFromDetail();
                tableActTransferDetail.Purge();
                tableActTransferItem.Purge();
                tableActTransfer.Purge();
            }
            else
            {
                TableActivityTransferTo tableActTransfer = new TableActivityTransferTo();
                TableActivityTransferToItem tableActTransferItem = new TableActivityTransferToItem();
                TableActivityTransferToDetail tableActTransferDetail = new TableActivityTransferToDetail();
                tableActTransferDetail.Purge();
                tableActTransferItem.Purge();
                tableActTransfer.Purge();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            string filter = string.Empty;
            TransferSettings transferSettings = new TransferSettings().LoadData();

            if (transferSettings.IsApplyTransferDateFilter)
            {
                if (!SysCommon.IsEmptyDate(transferSettings.TransferDateFilter))
                {
                    DateTime dt = transferSettings.TransferDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
        /// <param name="data"></param>
        /// <returns></returns>
        public override string GetFilter(XDocument data)
        {
            string filter = string.Empty;
            TransferSettings transferSettings = new TransferSettings().LoadData();

            if (transferSettings.IsApplyTransferDateFilter)
            {
                if (string.Compare(data.Root.Element("transfer_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("transfer_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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