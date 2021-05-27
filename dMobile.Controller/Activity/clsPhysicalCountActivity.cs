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

    /// <summary>
    /// 
    /// </summary>
    public class PhysicalActivity : Activity
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        private List<Count> listCounts = new List<Count>();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public PhysicalActivity()
            : base()
        {
            this.FullClassName = typeof(PhysicalActivity).FullName;

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
            return "Physical Counts";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource Strings
            Label labelPhysicalCountNumber = new Label { Text = "Count #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPhysicalCountDate = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPhysicalCountType = new Label { Text = "Count Type", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPhysicalCountNumber.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPhysicalCountDate.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPhysicalCountType.Text);
            double dataColumnWidth0 = 0;
            double dataColumnWidth1 = 0;
            double dataColumnWidth2 = 0;

            Grid gridHeaderPhysicalCount = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                }
            };

            gridHeaderPhysicalCount.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountNumber);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountDate);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountType);

            Grid.SetColumn(labelPhysicalCountNumber, 0);
            Grid.SetColumn(labelPhysicalCountDate, 1);
            Grid.SetColumn(labelPhysicalCountType, 2);

            Grid.SetRow(gridHeaderPhysicalCount, 0);

            dataGridMainActivityHeader.Children.Add(gridHeaderPhysicalCount);

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
                //    ActivityCountStringFKRefno = Convert.ToString(row["ActivityCountStringFKRefno"]),
                //    ActivityCountDateTime = Convert.ToDateTime(row["ActivityCountDateTime"]),
                //    ActivityCountTypeDescription = description,
                //};

                observableCollection.Add(count);

                dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(count.ActivityCountStringFKRefno);
                dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(count.ActivityCountDateTime.ToString("MM/dd/yyyy")));
                dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(count.ActivityCountTypeDescription);

                if (dataColumnWidth0 > columnWidth0)
                {
                    columnWidth0 = dataColumnWidth0;
                    gridHeaderPhysicalCount.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (dataColumnWidth1 > columnWidth1)
                {
                    columnWidth1 = dataColumnWidth1;
                    gridHeaderPhysicalCount.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (dataColumnWidth2 > columnWidth2)
                {
                    columnWidth2 = dataColumnWidth2;
                    gridHeaderPhysicalCount.ColumnDefinitions[2].Width = columnWidth2;
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
        /// <remarks></remarks>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            PhysicalCountSettings countSettings = new PhysicalCountSettings().LoadData();
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
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
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
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
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
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
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
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityCount.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
            }

            //using (SQLiteDataAdapter dataAdapterPhysical = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet dataSetPhysical = new DataSet())
            //    {
            //        dataAdapterPhysical.Fill(dataSetPhysical);

            //        this.DataViewHdrs = dataSetPhysical.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        this.DataViewHdrs.Sort = countSettings.GetDataViewSortBy();
            //        this.DataViewHdrs.RowFilter = this.GetFilter();
            //    }
            //}

            this.listCounts = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Count>(sql.ToString());

            foreach (Sort sort in countSettings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.listCounts = this.listCounts.OrderBy(adj => adj.GetType().GetProperty(sort.SortValue).GetValue(adj)).ToList();
                }
                else
                {
                    this.listCounts = this.listCounts.OrderByDescending(adj => adj.GetType().GetProperty(sort.SortValue).GetValue(adj)).ToList();
                }
            }

            this.GetFilter();

            this.ListHdrs = this.listCounts.Cast<object>().ToList();
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

            this.ListHdrs = this.listCounts.Where(count => count.ActivityCountStringFKRefno.Contains(searchText.Trim())).Cast<object>().ToList();

            foreach (Count count in this.ListHdrs)
            {
                string description = string.Empty;
                string[] descriptionSubstring = Convert.ToString(count.ActivityCountTypeDescription).Split();

                if (descriptionSubstring != null && descriptionSubstring.Length > 1)
                {
                    description = descriptionSubstring[0] + " " + descriptionSubstring[1];
                }

                observableCollection.Add(count);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Count count)
            {
                this.SelectedActivity = count;
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
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
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
                TableActivityPhysicalCount tableActPhysicalCount = new TableActivityPhysicalCount();

                tableActPhysicalCount.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                await tableActPhysicalCount.Request();
                tableActPhysicalCount.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            TableActivityPhysicalCount tableActPhysicalCount = new TableActivityPhysicalCount();
            TableActivityPhysicalCountItem tableActPhysicalCountItem = new TableActivityPhysicalCountItem();
            TableActivityPhysicalCountDetail tableActPhysicalCountDetail = new TableActivityPhysicalCountDetail();

            await tableActPhysicalCountDetail.Purge();
            await tableActPhysicalCountItem.Purge();
            await tableActPhysicalCount.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override void GetFilter()
        {
            string filter = string.Empty;
            PhysicalCountSettings countSettings = new PhysicalCountSettings().LoadData();

            if (countSettings.IsApplyCountDate)
            {
                if (!SysCommon.IsEmptyDate(countSettings.dtCountDate))
                {
                    DateTime dt = countSettings.dtCountDate;
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listCounts = this.listCounts.Where(count => count.ActivityCountDateTime >= minDate && count.ActivityCountDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override void GetFilter(XDocument data)
        {
            string filter = string.Empty;
            PhysicalCountSettings countSettings = new PhysicalCountSettings().LoadData();

            if (countSettings.IsApplyCountDate)
            {
                if (string.Compare(data.Root.Element("count_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("count_date").Value);
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listCounts = this.listCounts.Where(count => count.ActivityCountDateTime >= minDate && count.ActivityCountDateTime <= maxDate).ToList();
                }
            }
        }

        #endregion
    }
}
