namespace dMobile.Controller
{
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml.Linq;
    using System.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using System.Xml;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using dMobile.Resources.Types;
    using dMobile.Resources.Services;
    using System.Collections.Generic;
    using static dMobile.Bedrock.LocationMapTasks;

    /// <summary>
    /// 
    /// </summary>
    public class AdjActivity : Activity
    {
        #region Fields

        private AdjustmentSettings adjSettings;
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #endregion

        #region Constructors

        public AdjActivity()
          : base()
        {
            this.FullClassName = typeof(AdjActivity).FullName;
            this.AdjSettings = new AdjustmentSettings().LoadData();

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

                Label labelActivityAdjustmentStringFKRefno = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityAdjustmentDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityAdjustmentDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityAdjustmentStringFKRefno.SetBinding(Label.TextProperty, new Binding("ActivityAdjustmentStringFKRefno"));
                labelActivityAdjustmentDateTime.SetBinding(Label.TextProperty, new Binding("ActivityAdjustmentDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityAdjustmentDescription.SetBinding(Label.TextProperty, new Binding("ActivityAdjustmentDescription"));

                gridData.Children.Add(labelActivityAdjustmentStringFKRefno, 0, 0);
                gridData.Children.Add(labelActivityAdjustmentDateTime, 0, 0);
                gridData.Children.Add(labelActivityAdjustmentDescription, 0, 0);

                Grid.SetColumn(labelActivityAdjustmentStringFKRefno, 0);
                Grid.SetColumn(labelActivityAdjustmentDateTime, 1);
                Grid.SetColumn(labelActivityAdjustmentDescription, 2);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Properties
        
        public AdjustmentSettings AdjSettings
        {
            get
            {
                return this.adjSettings;
            }

            set
            {
                this.adjSettings = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            TableActivityAdjustment adj = new TableActivityAdjustment();
            TableActivityAdjustmentItem adjItem = new TableActivityAdjustmentItem();
            TableActivityAdjustmentDetail adjDetail = new TableActivityAdjustmentDetail();

            await adjDetail.Purge();
            adjItem.Purge();
            await adj.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityAdjustmentStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Adjustments";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource Strings
            Label labelAdjustmentNumber = new Label { Text = "Adj #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelAdjustmentDate = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelAdjustmentDescription = new Label { Text = "Description", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelAdjustmentNumber.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelAdjustmentDate.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelAdjustmentDescription.Text);
            double dataColumnWidth0 = 0;
            double dataColumnWidth1 = 0;
            double dataColumnWidth2 = 0;

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

            gridHeader.Children.Add(labelAdjustmentNumber);
            gridHeader.Children.Add(labelAdjustmentDate);
            gridHeader.Children.Add(labelAdjustmentDescription);

            Grid.SetColumn(labelAdjustmentNumber, 0);
            Grid.SetColumn(labelAdjustmentDate, 1);
            Grid.SetColumn(labelAdjustmentDescription, 2);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (dynamic row in this.ListHdrs)
            {
                Adjustment adjustment = new Adjustment
                {
                    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                    ActivityAdjustmentFKRefno = Convert.ToInt32(row["ActivityAdjustmentFKRefno"] != DBNull.Value ? row["ActivityAdjustmentFKRefno"] : null),
                    ActivityAdjustmentType = Convert.ToInt32(row["ActivityAdjustmentType"] != DBNull.Value ? row["ActivityAdjustmentType"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                    ActivityAdjustmentStringFKRefno = Convert.ToString(row["ActivityAdjustmentStringFKRefno"] != DBNull.Value ? row["ActivityAdjustmentStringFKRefno"] : null),
                    ActivityAdjustmentDescription = Convert.ToString(row["ActivityAdjustmentDescription"] != DBNull.Value ? row["ActivityAdjustmentDescription"] : null),
                    ActivityAdjustmentTypeDescription = Convert.ToString(row["ActivityAdjustmentTypeDescription"] != DBNull.Value ? row["ActivityAdjustmentTypeDescription"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                    ActivityAdjustmentDateTime = Convert.ToDateTime(row["ActivityAdjustmentDateTime"] != DBNull.Value ? row["ActivityAdjustmentDateTime"] : null)
                };

                observableCollection.Add(adjustment);

                dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustment.ActivityAdjustmentStringFKRefno);
                dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(adjustment.ActivityAdjustmentDateTime.ToString("MM/dd/yyyy")));
                dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustment.ActivityAdjustmentDescription);

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

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

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
        //      dataGridCol.MappingName = "ActivityAdjustmentStringFKRefno";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Adj);
        //#else
        //      dataGridCol.HeaderText = "Adj #";
        //#endif
        //      dataGridCol.Width = 68 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityAdjustmentDateTime";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
        //#else
        //      dataGridCol.HeaderText = "Date";
        //#endif
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityAdjustmentDescription";
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
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override async Task<List<object>> GetList(bool isSubmitBatch)
        {
            AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityAdjustment.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno AS ActivityAdjustmentFKRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno AS ActivityAdjustmentStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime AS ActivityAdjustmentDateTime,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDescription AS ActivityAdjustmentDescription,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentType AS ActivityAdjustmentType,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription AS ActivityAdjustmentTypeDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityAdjustment");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityAdjustment.ActivityRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityAdjustment");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityAdjustment.ActivityRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityAdjustment");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno");
                sql.AppendFormat(" WHERE tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                sql.Append("SELECT tActivityAdjustment.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno AS ActivityAdjustmentFKRefno,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno AS ActivityAdjustmentStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime AS ActivityAdjustmentDateTime,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentDescription AS ActivityAdjustmentDescription,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentType AS ActivityAdjustmentType,");
                sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription AS ActivityAdjustmentTypeDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityAdjustment");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
            }

            //using (SQLiteDataAdapter dataAdapterAdj = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet dataSetAdj = new DataSet())
            //    {
            //        dataAdapterAdj.Fill(dataSetAdj);
            //        this.DataViewHdrs = dataSetAdj.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            // TODO: Fix sorting
            //        this.DataViewHdrs.Sort = adjSettings.GetDataViewSortBy();
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
            // TODO: Test search
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActFilter))
            {
                //this.DataViewHdrs.RowFilter = "ActivityAdjustmentStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
                this.ListHdrs = (List<object>)this.ListHdrs.OfType<Adjustment>().Where(adj => (adj.ActivityAdjustmentStringFKRefno as string).Contains(searchText.Trim()));

            }
            else
            {
                //this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityAdjustmentStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
                this.ListHdrs = (List<object>)this.ListHdrs.OfType<Adjustment>().Where(adj => this.ActFilter.Contains(searchText.Trim()) && (adj.ActivityAdjustmentStringFKRefno as string).Contains(searchText.Trim()));
            }

            foreach (Adjustment adjustment in this.ListHdrs)
            {
                //Adjustment adjustment = new Adjustment
                //{
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityAdjustmentFKRefno = Convert.ToInt32(row["ActivityAdjustmentFKRefno"] != DBNull.Value ? row["ActivityAdjustmentFKRefno"] : null),
                //    ActivityAdjustmentType = Convert.ToInt32(row["ActivityAdjustmentType"] != DBNull.Value ? row["ActivityAdjustmentType"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    ActivityAdjustmentStringFKRefno = Convert.ToString(row["ActivityAdjustmentStringFKRefno"] != DBNull.Value ? row["ActivityAdjustmentStringFKRefno"] : null),
                //    ActivityAdjustmentDescription = Convert.ToString(row["ActivityAdjustmentDescription"] != DBNull.Value ? row["ActivityAdjustmentDescription"] : null),
                //    ActivityAdjustmentTypeDescription = Convert.ToString(row["ActivityAdjustmentTypeDescription"] != DBNull.Value ? row["ActivityAdjustmentTypeDescription"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                //    ActivityAdjustmentDateTime = Convert.ToDateTime(row["ActivityAdjustmentDateTime"] != DBNull.Value ? row["ActivityAdjustmentDateTime"] : null)
                //};

                observableCollection.Add(adjustment);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <param name="lstDisplay"></param>
        /// <remarks></remarks>
        public override async Task SelectHdr(object selectedItem)
        {
            if (selectedItem is Adjustment adjustment)
            {
                //this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityAdjustmentFKRefno"]);
                //this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);
                this.FKRefno = adjustment.ActivityAdjustmentFKRefno;
                this.Refno = adjustment.ActivityRefno;

                // Set the selected warehouse to the activity header warehouse.
                if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(adjustment.LocationMapRefno))
                {
                    LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(adjustment.LocationMapRefno));
                }

                this.AdjSettings = this.AdjSettings.LoadData();
                this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)Convert.ToInt32(adjustment.ActivityAdjustmentType);

                if (!SysCommon.IsAllowedChangeNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType))
                {
                    this.AdjSettings.NegateScanQuantity = SysCommon.DefaultNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType);
                    AdjustmentSettings tempAdjSettings = this.AdjSettings;
                    this.AdjSettings.SaveData(ref tempAdjSettings);
                    this.AdjSettings = tempAdjSettings;
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
            if (selectedItem is Adjustment adjustment)
            {
                this.FKRefno = Convert.ToInt32(adjustment.ActivityAdjustmentFKRefno);
                this.Refno = Convert.ToInt32(adjustment.ActivityRefno);
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

                TableActivityAdjustment adj = new TableActivityAdjustment();
                adj.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                await adj.Request();
                adj.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
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
            AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();

            if (adjSettings.IsApplyAdjustmentType)
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter += string.Format("ActivityAdjustmentType = {0}", Convert.ToInt32(adjSettings.AdjustmentType));
                }
                else
                {
                    filter += string.Format(" AND ActivityAdjustmentType = {0}", Convert.ToInt32(adjSettings.AdjustmentType));
                }
            }

            if (adjSettings.IsApplyAdjustmentDate)
            {
                if (!SysCommon.IsEmptyDate(adjSettings.AdjustmentDate))
                {
                    DateTime dt = adjSettings.AdjustmentDate;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
        public override string GetFilter(XDocument xmlDoc)
        {
            string filter = string.Empty;
            AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();

            if (adjSettings.IsApplyAdjustmentType)
            {
                if (string.Compare(xmlDoc.Root.Element("adjustment_type").Value, string.Empty) > 0)
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = Convert.ToString(("ActivityAdjustmentType = " + xmlDoc.Root.Element("adjustment_type").Value));
                    }
                    else
                    {
                        filter += " AND ActivityAdjustmentType = " + xmlDoc.Root.Element("adjustment_type").Value;
                    }
                }
            }

            if (adjSettings.IsApplyAdjustmentDate)
            {
                if (string.Compare(xmlDoc.Root.Element("adjustment_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(xmlDoc.Root.Element("adjustment_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter = filter + string.Format(
                            " AND ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            this.ActFilter = filter;

            return filter;
        }

        public override async Task PrintLabel(XDocument data)
        {
            if (AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox && !SysEnv.SysSettings.IsDisconnectedMode)
            {
                int locMapRefno = 0;
                SLocMapAndRefno sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFKRefno(
                    int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value),
                    int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value),
                    0,
                    locMapRefno);
                SLocMap locMap = sLocMapAndRefno.SLocMap;
                string reqData = string.Empty;
                PrinterSettings prnSettings = new PrinterSettings().LoadData();

                // Don't even bother sending in a print request if no quantity was entered.
                int labelQuantity = int.Parse(data.Root.Value);

                // Used abs for handling the freaky event that a negative label quantity was requested.
                if (labelQuantity > 0)
                {
                    string request =
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT PRINT DEBOX PRODUCT LABEL</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + this.FKRefno + "</refno>")
                    + ("<whs_id>" + locMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + locMap.SectFKRefno + "</sect_refno>")
                    + ("<label_qty>" + labelQuantity + "</label_qty>")
                    + ("<mobile_printer_name>"
                    + (prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty)
                    + "</mobile_printer_name>")
                    + "</imclient>";

                    reqData = SysCommon.SubmitComm(request);
     
                    if (reqData.Length > 0)
                    {
                        // check if result is an error message
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument docError = new XmlDocument();
                            docError.LoadXml(reqData);
                            throw new MessageOnlyException(string.Format("Order# {0} print shipping label(s) failed." + Environment.NewLine + "{1}", this.FKRefno, docError.DocumentElement["detail"].InnerText));
                        }
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception(string.Format("Order# {0} Print shipping label(s) failed.", this.FKRefno));
                    }
                }
            }
        }

        public override async Task<bool> ChangeStatus(int selectedIndex)
        {
            bool result = false;
            if (AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox && !SysEnv.SysSettings.IsDisconnectedMode)
            {
                // do xml request to debox
                string xml =
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                      + "<imclient>"
                      + "<command>"
                      + "<action>COMPLETE DEBOX ADJUSTMENT</action>"
                      + "</command>"
                      + SysCommon.GetRequestDefaults(false)
                      + ("<refno>" + this.FKRefno + "</refno>")
                      + "</imclient>";

                string reqData = SysCommon.SubmitComm(xml);
                
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;
                        
                        throw new Exception(string.Format("Debox failed. {0}", Environment.NewLine + errorDetail));
                    }
                }
                else
                {
                    throw new Exception("Debox failed. No COM service response.");
                }

                result = true;
            }

            return result;
        }

        #endregion
    }
}
