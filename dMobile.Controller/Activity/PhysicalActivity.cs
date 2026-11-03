// <copyright file="PhysicalActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Bedrock.Settings;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Headers;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="PhysicalActivity" />.
    /// </summary>
    public class PhysicalActivity : Activity
    {
        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double columnWidth0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double columnWidth1;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double columnWidth2;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listCounts.
        /// </summary>
        private List<Count> listCounts = new List<Count>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalActivity"/> class.
        /// </summary>
        public PhysicalActivity()
            : base()
        {
            DefineInitialGridDataTemplate();
            return;

            void DefineInitialGridDataTemplate()
            {
                // Data grid
                var dataTemplate = new DataTemplate(() =>
                {
                    var gridData = new Grid
                    {
                        Padding = new Thickness(5),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                        },
                    };

                    var labelActivityRefno = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityCountDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityCountTypeDescription = new Label { VerticalOptions = LayoutOptions.Center };

                    labelActivityRefno.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
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

                var isAddKey = true;
                foreach (var item in this.resourceDictionary)
                {
                    if (item.Key == "dataTemplatePhysicalCount")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.resourceDictionary.Add("dataTemplatePhysicalCount", dataTemplate);
                }
            }
        }

        /// <summary>
        /// Display the extra information for the selected activity.
        /// </summary>
        /// <param name="extraInfo">Extra information collection.</param>
        public override void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            base.DisplayExtraInfo(extraInfo);
            if (this.SelectedActivity is Count activity)
            {
                const int lineLength = 30;
                var lines = SysCommon.WordWrap2List(activity.ActivityCountDescription.TrimEnd(), lineLength);
                foreach (var line in lines)
                {
                    extraInfo.Add(line);
                }
            }
        }

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Physical Counts";
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The gridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The listViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            var labelPhysicalCountNumber = new Label { Text = "Count #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelPhysicalCountDate = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelPhysicalCountType = new Label { Text = "Count Type", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPhysicalCountNumber.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPhysicalCountDate.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPhysicalCountType.Text);
            var gridHeaderPhysicalCount = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeaderPhysicalCount.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountNumber);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountDate);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountType);

            Grid.SetColumn(labelPhysicalCountNumber, 0);
            Grid.SetColumn(labelPhysicalCountDate, 1);
            Grid.SetColumn(labelPhysicalCountType, 2);

            Grid.SetRow(gridHeaderPhysicalCount, 0);

            gridMainActivity.Children.Add(gridHeaderPhysicalCount);

            foreach (var count in this.ListHeaders.Cast<Count>())
            {
                var description = string.Empty;
                var descriptionSubstring = Convert.ToString(count.ActivityCountTypeDescription).Split();

                if (descriptionSubstring != null && descriptionSubstring.Length > 1)
                {
                    description = descriptionSubstring[0] + " " + descriptionSubstring[1];
                }

                // Count count = new Count
                // {
                //    ActivityStringFKRefno = Convert.ToString(row["ActivityStringFKRefno"]),
                //    ActivityCountDateTime = Convert.ToDateTime(row["ActivityCountDateTime"]),
                //    ActivityCountTypeDescription = description,
                // };
                observableCollection.Add(count);

                var dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(count.ActivityStringFKRefno);
                var dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(count.ActivityCountDateTime.ToString("MM/dd/yyyy")));
                var dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(count.ActivityCountTypeDescription);
                if (dataColumnWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = dataColumnWidth0;
                    gridHeaderPhysicalCount.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (dataColumnWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = dataColumnWidth1;
                    gridHeaderPhysicalCount.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (dataColumnWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = dataColumnWidth2;
                    gridHeaderPhysicalCount.ColumnDefinitions[2].Width = this.columnWidth2;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplatePhysicalCount"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var countSettings = new PhysicalCountSettings().LoadData();
            var sql = new StringBuilder();

            sql.Append("SELECT tActivityCount.ActivityRefno,");
            sql.Append("tActivityCount.ActivityFKRefno,");
            sql.Append("tActivityCount.ActivityStringFKRefno,");
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
            if (isSubmitBatch)
            {
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
                sql.AppendFormat("AND tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityCount.ActivityRefno,");
                sql.Append("tActivityCount.ActivityFKRefno,");
                sql.Append("tActivityCount.ActivityStringFKRefno,");
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
                sql.Append("tActivityCount.ActivityFKRefno,");
                sql.Append("tActivityCount.ActivityStringFKRefno,");
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
                sql.AppendFormat("WHERE tActivityCount.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
            }

            // using (SQLiteDataAdapter dataAdapterPhysical = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet dataSetPhysical = new DataSet())
            //    {
            //        dataAdapterPhysical.Fill(dataSetPhysical);

            // this.DataViewHdrs = dataSetPhysical.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        this.DataViewHdrs.Sort = countSettings.GetDataViewSortBy();
            //        this.DataViewHdrs.RowFilter = this.GetFilter();
            //    }
            // }
            this.listCounts = await this.Conn.QueryAsync<Count>(sql.ToString());

            // Fast sort without reflection
            foreach (var sort in countSettings.GetDataViewSortBy())
            {
                this.listCounts = ApplySort(
                    this.listCounts,
                    sort.SortValue,
                    sort.SortOrder == Resources.Enums.SortOrder.ASC).ToList();
            }

            this.GetFilter();

            this.ListHeaders = this.listCounts.Cast<object>().ToList();
            return this.ListHeaders;

            static IEnumerable<Count> ApplySort(IEnumerable<Count> src, string key, bool asc)
            {
                return key switch
                {
                    nameof(Count.ActivityStringFKRefno) => asc
                        ? src.OrderBy(x => x.ActivityStringFKRefno)
                        : src.OrderByDescending(x => x.ActivityStringFKRefno),
                    nameof(Count.ActivityFKRefno) => asc
                        ? src.OrderBy(x => x.ActivityFKRefno)
                        : src.OrderByDescending(x => x.ActivityFKRefno),
                    nameof(Count.ActivityCountDateTime) => asc
                        ? src.OrderBy(x => x.ActivityCountDateTime)
                        : src.OrderByDescending(x => x.ActivityCountDateTime),
                    nameof(Count.ActivityCountTypeDescription) => asc
                        ? src.OrderBy(x => x.ActivityCountTypeDescription)
                        : src.OrderByDescending(x => x.ActivityCountTypeDescription),
                    nameof(Count.ActivityCountDescription) => asc
                        ? src.OrderBy(x => x.ActivityCountDescription)
                        : src.OrderByDescending(x => x.ActivityCountDescription),
                    nameof(Count.ActivityCountType) => asc
                        ? src.OrderBy(x => x.ActivityCountType)
                        : src.OrderByDescending(x => x.ActivityCountType),
                    _ => src,   // unknown sort key: no-op
                };
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            this.ListHeaders = this.listCounts.Where(count => count.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (var count in this.ListHeaders.Cast<Count>())
            {
                // string description = string.Empty;
                // string[] descriptionSubstring = Convert.ToString(count.ActivityCountTypeDescription).Split();

                // if (descriptionSubstring != null && descriptionSubstring.Length > 1)
                // {
                //    description = descriptionSubstring[0] + " " + descriptionSubstring[1];
                // }
                observableCollection.Add(count);
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
            if (selectedActivity is Count header)
            {
                this.SelectedActivity = header;
                this.HdrFkRefno = header.ActivityFKRefno;
                this.HdrRefno = header.ActivityRefno;
                this.LocationMapRefno = header.WarehouseLocationMapRefno;

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // Set the current location to the header.
                    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.WarehouseLocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.WarehouseLocationMapRefno))
                    {
                        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            header.WarehouseLocationMapRefno);
                    }

                    // Override and set the chosen location to the current location.
                    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.WarehouseLocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.WarehouseLocationMapRefno))
                    {
                        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }

                    // Override and set any selected skid location to the current location.
                    if (this.SkidInfo.CurrSkid != null && this.SkidInfo.CurrSkid.SkidFkRefno > 0 &&
                        this.SkidInfo.CurrSkid.LocMapRefno != header.WarehouseLocationMapRefno &&
                        await LocationMapTasks.IsCompleteLocMap(header.WarehouseLocationMapRefno))
                    {
                        // NOTE: We may have to use a temp var to assign the current skid a new location.
                        this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// The AssignRefnoFromActivity.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is Count count)
            {
                this.HdrFkRefno = Convert.ToInt32(count.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(count.ActivityRefno);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode && !this.ShouldThrottleRefresh())
            {
                await this.Purge();
                var tableActPhysicalCount = new TableActivityPhysicalCount();
                await tableActPhysicalCount.RequestHeaderData();
                this.MarkRefreshed();
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            var tableActPhysicalCount = new TableActivityPhysicalCount();
            var tableActPhysicalCountItem = new TableActivityPhysicalCountItem();
            var tableActPhysicalCountDetail = new TableActivityPhysicalCountDetail();

            await tableActPhysicalCountDetail.PurgeData();
            await tableActPhysicalCountItem.PurgeData();
            await tableActPhysicalCount.PurgeData();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
            var filter = string.Empty;
            var countSettings = new PhysicalCountSettings().LoadData();

            if (countSettings.IsApplyCountDate)
            {
                if (!SysCommon.IsEmptyDate(countSettings.CountDate))
                {
                    var dt = countSettings.CountDate;
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listCounts = this.listCounts.Where(count => count.ActivityCountDateTime >= minDate && count.ActivityCountDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        public override void GetFilter(XDocument data)
        {
            var filter = string.Empty;
            var countSettings = new PhysicalCountSettings().LoadData();

            if (countSettings.IsApplyCountDate)
            {
                if (string.Compare(data.Root.Element("count_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("count_date").Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listCounts = this.listCounts.Where(count => count.ActivityCountDateTime >= minDate && count.ActivityCountDateTime <= maxDate).ToList();
                }
            }
        }
    }
}
