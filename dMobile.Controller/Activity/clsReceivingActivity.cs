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
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class RecvActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public RecvActivity()
          : base()
        {
            this.FullClassName = typeof(RecvActivity).FullName;
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
            // TODO: Calculate column widths
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            Grid gridHeaderReceiving = new Grid
            {
                Padding = new Thickness(5, 5, 5, 5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            gridHeaderReceiving.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            Label labelPOReceiving = new Label { Text = "PO #", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelIssuedReceiving = new Label { Text = "Issued", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelArrivedReceiving = new Label { Text = "Arrived", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            gridHeaderReceiving.Children.Add(labelPOReceiving);
            gridHeaderReceiving.Children.Add(labelIssuedReceiving);
            gridHeaderReceiving.Children.Add(labelArrivedReceiving);

            Grid.SetColumn(labelPOReceiving, 0);
            Grid.SetColumn(labelIssuedReceiving, 1);
            Grid.SetColumn(labelArrivedReceiving, 2);

            Grid.SetRow(gridHeaderReceiving, 0);

            dataGridMainActivityHeader.Children.Add(gridHeaderReceiving);

            // Data grid
            DataTemplate dataTemplateReceiving = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5, 5, 5, 5),
                    ColumnDefinitions = 
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Star }
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

            dataGridMainActivity.ItemTemplate = dataTemplateReceiving;
            observableCollection.Clear();

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Receive receive = new Receive
                {
                    ActivityReceivingPONumber = Convert.ToString(row["ActivityReceivingPONumber"]),
                    ActivityReceivingIssuedDateTime = Convert.ToDateTime(row["ActivityReceivingIssuedDateTime"]),
                    ActivityReceivingArrivalDateTime = Convert.ToDateTime(row["ActivityReceivingArrivalDateTime"])
                };

                observableCollection.Add(receive);
            }

            return observableCollection;
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <remarks></remarks>
        //    ///
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      int colWidthFactor = 1;

        //      if (this.SysEnv.IsWindowsMobile6Device)
        //      {
        //        colWidthFactor = 2;
        //      }

        //      if (dataGridMain.TableStyles.Count > 0)
        //      {
        //        dataGridMain.TableStyles.Clear();
        //      }

        //      if (dataGridMain.TableStyles.Count > 0)
        //      {
        //        dataGridMain.TableStyles.Clear();
        //      }

        //      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //      dataGridStyle.MappingName = this.MainGridMappingName;

        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityReceivingPONumber";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Po);
        //#else
        //      dataGridCol.HeaderText = "PO #";
        //#endif
        //      dataGridCol.Width = 55 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityReceivingIssuedDateTime";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Issued);
        //#else
        //      dataGridCol.HeaderText = "Issued";
        //#endif
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityReceivingArrivalDateTime";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Arrived);
        //#else
        //      dataGridCol.HeaderText = "Arrived";
        //#endif
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridMain.TableStyles.Add(dataGridStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

        /// <summary>
        ///
        /// </summary>
        /// <param name="IsSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            ReceivingSettings receiveSettings = new ReceivingSettings().LoadData();
            LocationMap locMap = LocationMap.GetInstance();

            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
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
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
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
                sql.AppendFormat("WHERE tActivity.ActivityStatus = {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("WHERE tActivity.ActivityStatus = {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus = {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
                sql.AppendFormat("AND tActivity.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.Receiving);
            }
            else
            {
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
                sql.AppendFormat("WHERE tActivityReceiving.WarehouseLocationMapRefno = {0} ", locMap.LoginWhsLocMapRefno);
                sql.AppendFormat("AND tActivity.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.Receiving);
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);

                    this.DataViewHdrs = ds.Tables[0].DefaultView;
                    this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                    this.DataViewHdrs.Sort = receiveSettings.GetDataViewSortBy();
                    this.DataViewHdrs.RowFilter = this.GetFilter();
                }
            }

            return this.DataViewHdrs;
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
                this.DataViewHdrs.RowFilter = "ActivityReceivingPONumber LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityReceivingPONumber LIKE \'%" + searchText.Trim() + "%\'";
            }

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Receive receive = new Receive
                {
                    ActivityReceivingPONumber = Convert.ToString(row["ActivityReceivingPONumber"]),
                    ActivityReceivingIssuedDateTime = Convert.ToDateTime(row["ActivityReceivingIssuedDateTime"]),
                    ActivityReceivingArrivalDateTime = Convert.ToDateTime(row["ActivityReceivingArrivalDateTime"])
                };

                observableCollection.Add(receive);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityReceivingFKRefno"]);
            this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);
            this.PONO = Convert.ToString(this.DataViewHdrs[idx]["ActivityReceivingPONumber"]);

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
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = Convert.ToInt32(DataViewHdrs[idx]["ActivityReceivingFKRefno"]);
            this.Refno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);
            this.PONO = Convert.ToString(DataViewHdrs[idx]["ActivityReceivingPONumber"]);

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
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
        public override void Purge()
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
