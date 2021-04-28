namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;

    public class CycleActivity : Activity
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        public CycleActivity()
            : base()
        {
            this.FullClassName = typeof(CycleActivity).FullName;

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

                Label labelActivityRefno = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityCountDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityCountTypeDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityRefno.SetBinding(Label.TextProperty, new Binding("ActivityCountStringFKRefno"));
                labelActivityCountDateTime.SetBinding(Label.TextProperty, new Binding("ActivityCountDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityCountTypeDescription.SetBinding(Label.TextProperty, new Binding("ActivityCountTypeDescription"));

                gridData.Children.Add(labelActivityRefno, 0, 0);
                gridData.Children.Add(labelActivityCountDateTime, 0, 0);
                gridData.Children.Add(labelActivityCountTypeDescription, 0, 0);

                Grid.SetColumn(labelActivityRefno, 0);
                Grid.SetColumn(labelActivityCountDateTime, 1);
                Grid.SetColumn(labelActivityCountTypeDescription, 2);

                return new ViewCell { View = gridData };
            });

            resourceDictionary.Add("dataTemplate", dataTemplate);
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
            return "ActivityCountStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Cycle Counts";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource Strings
            Label labelCountNumber = new Label { Text = "Count #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountDate = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountType = new Label { Text = "Count Type", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountNumber.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountDate.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountType.Text);
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

            gridHeader.Children.Add(labelCountNumber);
            gridHeader.Children.Add(labelCountDate);
            gridHeader.Children.Add(labelCountType);

            Grid.SetColumn(labelCountNumber, 0);
            Grid.SetColumn(labelCountDate, 1);
            Grid.SetColumn(labelCountType, 2);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (Count count in this.ListHdrs)
            {
                string description = string.Empty;
                string[] descriptionSubstring = Convert.ToString(count.ActivityCountTypeDescription).Split();

                if (descriptionSubstring != null && descriptionSubstring.Length > 1)
                {
                    description = descriptionSubstring[0] + " " + descriptionSubstring[1];
                }

                //Count count = new Count
                //{
                //    ActivityCountStringFKRefno = Convert.ToString(row["ActivityCountStringFKRefno"] != DBNull.Value ? row["ActivityCountStringFKRefno"] : null),
                //    ActivityCountDateTime = Convert.ToDateTime(row["ActivityCountDateTime"] != DBNull.Value ? row["ActivityCountDateTime"] : null),
                //    ActivityCountTypeDescription = description,
                //};

                observableCollection.Add(count);

                dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(count.ActivityCountStringFKRefno);
                dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(count.ActivityCountDateTime.ToString("MM/dd/yyyy")));
                dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(count.ActivityCountTypeDescription);

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

        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <param name="dataGridMain"></param>
        //        public override void DesignDataGrid(DataGrid dataGridMain)
        //        {
        //            int colWidthFactor = 1;

        //            if (SysEnv.IsWindowsMobile6Device)
        //            {
        //                colWidthFactor = 2;
        //            }

        //            if (dataGridMain.TableStyles.Count > 0)
        //            {
        //                dataGridMain.TableStyles.Clear();
        //            }

        //            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //            dataGridStyle.MappingName = this.MainGridMappingName;

        //            DataGridTextBoxColumn dataGridCol1 = new DataGridTextBoxColumn();
        //            dataGridCol1.MappingName = "ActivityCountStringFKRefno";
        //#if MULTILANGUAGE
        //            dataGridCol1.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Countz);
        //#else
        //            dataGridCol1.HeaderText = "Count#";
        //#endif
        //            dataGridCol1.Width = 50 * colWidthFactor;
        //            dataGridStyle.GridColumnStyles.Add(dataGridCol1);

        //            DataGridTextBoxColumn dataGridCol2 = new DataGridTextBoxColumn();
        //            dataGridCol2.MappingName = "ActivityCountDateTime";
        //#if MULTILANGUAGE
        //            dataGridCol2.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
        //#else
        //            dataGridCol2.HeaderText = "Date";
        //#endif
        //            dataGridCol2.Width = 58 * colWidthFactor;
        //            dataGridStyle.GridColumnStyles.Add(dataGridCol2);

        //            DataGridTextBoxColumn dataGridCol3 = new DataGridTextBoxColumn();
        //            dataGridCol3.MappingName = "ActivityCountTypeDescription";
        //#if MULTILANGUAGE
        //            dataGridCol3.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.CountType);
        //#else
        //            dataGridCol3.HeaderText = "Count Type";
        //#endif
        //            dataGridCol3.Width = 118 * colWidthFactor;
        //            dataGridStyle.GridColumnStyles.Add(dataGridCol3);

        //            dataGridMain.TableStyles.Add(dataGridStyle);
        //            dataGridMain.RowHeadersVisible = false;
        //            dataGridMain.ColumnHeadersVisible = true;
        //        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityCount.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno AS ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno AS ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription AS ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription AS ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime AS ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityCount.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityCount.ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityCount.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityCount.ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityCount.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                sql.Append("SELECT tActivityCount.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno AS ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno AS ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription AS ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription AS ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime AS ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityCount.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityCount.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
            }

            //using (SQLiteDataAdapter dataAdapterCycle = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet dataSetCycle = new DataSet())
            //    {
            //        dataAdapterCycle.Fill(dataSetCycle);
            //        this.DataViewHdrs = dataSetCycle.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;

            //        // TODO: Fix sorting and filter
            //        CycleCountSettings cycleCountSettings = new CycleCountSettings().LoadData();
            //        this.DataViewHdrs.Sort = cycleCountSettings.GetDataViewSortBy();
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
            // TODO: verify filtering
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActFilter))
            {
                //this.DataViewHdrs.RowFilter = "ActivityCountStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
                this.ListHdrs = (List<object>)this.ListHdrs.OfType<Count>().Where(count => count.ActivityCountStringFKRefno.Contains(searchText.Trim()));
            }
            else
            {
                //this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityCountStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
                //this.ListHdrs = (List<object>)this.ListHdrs.OfType<Count>().Where(count => this.ActFilter.Contains(searchText.Trim()) && count.ActivityCountStringFKRefno.Contains(searchText.Trim()));
            }

            foreach (Count count in this.ListHdrs)
            {

                string description = string.Empty;
                string[] descriptionSubstring = Convert.ToString(count.ActivityCountTypeDescription).Split();

                if (descriptionSubstring != null && descriptionSubstring.Length > 1)
                {
                    description = descriptionSubstring[0] + " " + descriptionSubstring[1];
                }

                //Count count = new Count
                //{
                //    ActivityCountStringFKRefno = Convert.ToString(row["ActivityCountStringFKRefno"] != DBNull.Value ? row["ActivityCountStringFKRefno"] : null),
                //    ActivityCountDateTime = Convert.ToDateTime(row["ActivityCountDateTime"] != DBNull.Value ? row["ActivityCountDateTime"] : null),
                //    ActivityCountTypeDescription = description,
                //};

                observableCollection.Add(count);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectHdr(object selectedItem)
        {
            if (selectedItem is Count count)
            {
                this.FKRefno = Convert.ToInt32(count.ActivityCountFKRefno);
                this.Refno = Convert.ToInt32(count.ActivityRefno);

                // Set the selected warehouse to the activity header warehouse.
                if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(count.WarehouseLocationMapRefno))
                {
                    LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(count.WarehouseLocationMapRefno));
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
            if (selectedItem is Count count)
            {
                this.FKRefno = Convert.ToInt32(count.ActivityCountFKRefno);
                this.Refno = Convert.ToInt32(count.ActivityRefno);

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

                TableActivityCycleCount cycleCount = new TableActivityCycleCount();
                cycleCount.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                cycleCount.Request();
                cycleCount.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            TableActivityCycleCount cycleCount = new TableActivityCycleCount();
            TableActivityCycleCountItem cycleCountItem = new TableActivityCycleCountItem();
            TableActivityCycleCountDetail cycleCountDetail = new TableActivityCycleCountDetail();

            cycleCountDetail.Purge();
            await cycleCountItem.Purge();
            cycleCount.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            string filter = string.Empty;
            CycleCountSettings countSettings = new CycleCountSettings().LoadData();

            if (countSettings.IsApplyCountDate)
            {
                if (!SysCommon.IsEmptyDate(countSettings.dtCountDate))
                {
                    DateTime dt = countSettings.dtCountDate;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format("ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'", dt.Year, dt.Month, dt.Day);
                    }
                    else
                    {
                        filter += string.Format(" AND ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'", dt.Year, dt.Month, dt.Day);
                    }
                }
            }

            this.ActFilter = filter;

            return filter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public override string GetFilter(XDocument filterData)
        {
            string filter = string.Empty;
            CycleCountSettings cycleSettings = new CycleCountSettings().LoadData();

            if (cycleSettings.IsApplyCountDate)
            {
                if (string.Compare(filterData.Root.Element("count_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(filterData.Root.Element("count_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime "
                            + "<= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime "
                             + "<= '{0}-{1}-{2} 23:59:59.997'",
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
