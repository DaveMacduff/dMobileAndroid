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

    public class TicketFinishedActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TicketFinishedActivity()
          : base()
        {
            this.FullClassName = typeof(TicketFinishedActivity).FullName;
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
            return "ActivityProductionStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Tickets Finished";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                ProductionTicketFinished productionTicket = new ProductionTicketFinished
                {
                    ActivityProductionDateTime = Convert.ToDateTime(row["ActivityProductionDateTime"] != DBNull.Value ? row["ActivityProductionDateTime"] : null),
                    ActivityProductionDescription = Convert.ToString(row["ActivityProductionDescription"] != DBNull.Value ? row["ActivityProductionDescription"] : null),
                    ActivityProductionFKRefno = Convert.ToInt32(row["ActivityProductionFKRefno"] != DBNull.Value ? row["ActivityProductionFKRefno"] : null),
                    ActivityProductionStringFKRefno = Convert.ToString(row["ActivityProductionStringFKRefno"] != DBNull.Value ? row["ActivityProductionStringFKRefno"] : null),
                    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                };

                observableCollection.Add(productionTicket);
            }

            // TODO: Resource Strings
            Label labelTicketNumber = new Label { Text = "Ticket #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelTicketDate = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelTicketDescription = new Label { Text = "Description", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketNumber.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketDate.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketDescription.Text);

            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                //ColumnSpacing = 15,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelTicketNumber);
            gridHeader.Children.Add(labelTicketDate);
            gridHeader.Children.Add(labelTicketDescription);

            Grid.SetColumn(labelTicketNumber, 0);
            Grid.SetColumn(labelTicketDate, 1);
            Grid.SetColumn(labelTicketDescription, 2);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (ProductionTicketFinished productionTicket in observableCollection)
            {
                double dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicket.ActivityProductionStringFKRefno);
                double dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(productionTicket.ActivityProductionDateTime.ToString("MM/dd/yyyy")));
                double dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicket.ActivityProductionDescription);

                if (dataColumnWidth0 > columnWidth0)
                {
                    columnWidth0 = dataColumnWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (dataColumnWidth1 > columnWidth1)
                {
                    columnWidth1 = dataColumnWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (dataColumnWidth2 > columnWidth2)
                {
                    columnWidth2 = dataColumnWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }
            }

            // Data grid
            DataTemplate dataTemplateCount = new DataTemplate(() =>
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

                Label labelActivityRefno = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityRefno.SetBinding(Label.TextProperty, new Binding("ActivityProductionStringFKRefno"));
                labelActivityDateTime.SetBinding(Label.TextProperty, new Binding("ActivityProductionDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityDescription.SetBinding(Label.TextProperty, new Binding("ActivityProductionDescription"));

                gridData.Children.Add(labelActivityRefno, 0, 0);
                gridData.Children.Add(labelActivityDateTime, 0, 0);
                gridData.Children.Add(labelActivityDescription, 0, 0);

                Grid.SetColumn(labelActivityRefno, 0);
                Grid.SetColumn(labelActivityDateTime, 1);
                Grid.SetColumn(labelActivityDescription, 2);

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplateCount;

            return observableCollection;
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <remarks></remarks>
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

        //      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //      dataGridStyle.MappingName = this.MainGridMappingName;

        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityProductionStringFKRefno";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Ticket);
        //#else
        //      dataGridCol.HeaderText = "Ticket #";
        //#endif
        //      dataGridCol.Width = 68 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityProductionDateTime";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
        //#else
        //      dataGridCol.HeaderText = "Date";
        //#endif
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityProductionDescription";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Description);
        //#else
        //      dataGridCol.HeaderText = "Description";
        //#endif
        //      dataGridCol.Width = 100 * colWidthFactor;
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
            ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityProduction.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionFKRefno AS ActivityProductionFKRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno AS ActivityProductionStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime AS ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription AS ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionFKRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionFKRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
            }
            else
            {
                sql.Append("SELECT tActivityProduction.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionFKRefno AS ActivityProductionFKRefno,");
                sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno AS ActivityProductionStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime AS ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription AS ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);

                    this.DataViewHdrs = ds.Tables[0].DefaultView;
                    this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                    this.DataViewHdrs.Sort = ticketSettings.GetDataViewSortBy();
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
                this.DataViewHdrs.RowFilter = "ActivityProductionStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityProductionStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                ProductionTicketFinished productionTicket = new ProductionTicketFinished
                {
                    ActivityProductionDateTime = Convert.ToDateTime(row["ActivityProductionDateTime"] != DBNull.Value ? row["ActivityProductionDateTime"] : null),
                    ActivityProductionDescription = Convert.ToString(row["ActivityProductionDescription"] != DBNull.Value ? row["ActivityProductionDescription"] : null),
                    ActivityProductionFKRefno = Convert.ToInt32(row["ActivityProductionFKRefno"] != DBNull.Value ? row["ActivityProductionFKRefno"] : null),
                    ActivityProductionStringFKRefno = Convert.ToString(row["ActivityProductionStringFKRefno"] != DBNull.Value ? row["ActivityProductionStringFKRefno"] : null),
                    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                };

                observableCollection.Add(productionTicket);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <param name="lstDisplay"></param>
        /// <remarks></remarks>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityProductionFKRefno"]);
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
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityProductionFKRefno"]);
            this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);

            return this.Refno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlParam"></param>
        public override void Refresh(XDocument xmlParam)
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

                TableActivityProductionTicketFinished tableFinished = new TableActivityProductionTicketFinished();

                tableFinished.RegisterDataAccessListenerProgressOut((TicketFinishedActivity)this);
                tableFinished.Request();
                tableFinished.UnregisterDataAccessListenerProgressOut((TicketFinishedActivity)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            TableActivityProductionTicketFinished tableFinished = new TableActivityProductionTicketFinished();
            TableActivityProductionTicketFinishedItem tableFinishedItem = new TableActivityProductionTicketFinishedItem();
            TableActivityProductionTicketFinishedDetail tableFinishedDetail = new TableActivityProductionTicketFinishedDetail();

            tableFinishedDetail.Purge();
            tableFinishedItem.Purge();
            tableFinished.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            string filter = string.Empty;
            ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();

            if (ticketSettings.IsApplyProductionDateFilter)
            {
                if (!SysCommon.IsEmptyDateTime(ticketSettings.ProductionDateFilter))
                {
                    DateTime dt = ticketSettings.ProductionDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
            ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();

            if (ticketSettings.IsApplyProductionDateFilter)
            {
                if (string.Compare(data.Root.Element("production_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("production_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
