// <copyright file="CycleActivity.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="CycleActivity" />.
    /// </summary>
    public class CycleActivity : Activity
    {
        private readonly ResourceDictionary resourceDictionary = new ResourceDictionary();

        private double columnWidth0;

        private double columnWidth1;

        private double columnWidth2;

        private List<Count> listCounts = new List<Count>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleActivity"/> class.
        /// </summary>
        public CycleActivity()
        {
            this.CycleCountSettings = new CycleCountSettings().LoadData();
            DefineInitialGridDataTemplate();

            void DefineInitialGridDataTemplate()
            {
                var dataTemplate = new DataTemplate(
                    () =>
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
                        labelActivityCountDateTime.SetBinding(
                            Label.TextProperty,
                            new Binding("ActivityCountDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                        labelActivityCountTypeDescription.SetBinding(
                            Label.TextProperty,
                            new Binding("ActivityCountTypeDescription"));

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
                    if (item.Key == "dataTemplateCycleCount")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.resourceDictionary.Add("dataTemplateCycleCount", dataTemplate);
                }
            }
        }

        private CycleCountSettings CycleCountSettings { get; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <inheritdoc />
        public override string GetControlDescription()
        {
            return "Cycle Counts";
        }

        /// <inheritdoc />
        public override ObservableCollection<object> DesignGrid(ref Grid mainGrid, ref ListView mainListView)
        {
            var observableCollection = new ObservableCollection<object>();
            var labelCountNumber = new Label
            {
                Text = "Count #",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelCountDate = new Label
            {
                Text = "Date",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelCountType = new Label
            {
                Text = "Count Type",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            // Calculate text size
            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountNumber.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountDate.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountType.Text);
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelCountNumber);
            gridHeader.Children.Add(labelCountDate);
            gridHeader.Children.Add(labelCountType);

            Grid.SetColumn(labelCountNumber, 0);
            Grid.SetColumn(labelCountDate, 1);
            Grid.SetColumn(labelCountType, 2);

            Grid.SetRow(gridHeader, 0);

            mainGrid.Children.Add(gridHeader);

            foreach (var count in this.ListHeaders.Cast<Count>())
            {
                observableCollection.Add(count);
                var dataColumnWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(count.ActivityStringFKRefno);
                var dataColumnWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(count.ActivityCountDateTime.ToString("MM/dd/yyyy")));
                var dataColumnWidth2 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(count.ActivityCountTypeDescription);
                if (dataColumnWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = dataColumnWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (dataColumnWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = dataColumnWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (dataColumnWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = dataColumnWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }
            }

            mainListView.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateCycleCount"];

            return observableCollection;
        }

        /// <inheritdoc />
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
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
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
            sql.Append("tActivityCount.WarehouseLocationMapRefno ");
            if (isSubmitBatch)
            {
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
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
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityCount.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
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
                sql.AppendFormat("WHERE tActivityCount.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
            }

            this.listCounts = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<Count>(sql.ToString());
            foreach (var sort in this.CycleCountSettings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.listCounts = this.listCounts.OrderBy(
                        adj => adj.GetType().GetProperty(sort.SortValue) !.GetValue(adj)).ToList();
                }
                else
                {
                    this.listCounts = this.listCounts.OrderByDescending(
                        adj => adj.GetType().GetProperty(sort.SortValue) !.GetValue(adj)).ToList();
                }
            }

            this.GetFilter();

            this.ListHeaders = this.listCounts.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            // TODO: verify filtering
            var observableCollection = new ObservableCollection<object>();

            this.ListHeaders = this.listCounts.Where(count => count.ActivityStringFKRefno.ToLower().
                Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (var count in this.ListHeaders.Cast<Count>())
            {
                observableCollection.Add(count);
            }

            return observableCollection;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is Count hdr)
            {
                this.HdrFkRefno = hdr.ActivityFKRefno;
                this.HdrRefno = hdr.ActivityRefno;
            }
        }

        /// <inheritdoc />
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
                var cycleCount = new TableActivityCycleCount();
                await cycleCount.RequestHeaderData();
            }
        }

        /// <inheritdoc />
        public override async Task Purge()
        {
            var cycleCount = new TableActivityCycleCount();
            var cycleCountItem = new TableActivityCycleCountItem();
            var cycleCountDetail = new TableActivityCycleCountDetail();

            await cycleCountDetail.PurgeData();
            await cycleCountItem.PurgeData();
            await cycleCount.PurgeData();
        }

        /// <inheritdoc />
        public override void GetFilter()
        {
            var filter = string.Empty;
            var cycleSettings = new CycleCountSettings().LoadData();

            if (this.CycleCountSettings.IsApplyCountDate)
            {
                if (!SysCommon.IsEmptyDate(cycleSettings.CountDate))
                {
                    var dt = cycleSettings.CountDate;
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    // if (string.IsNullOrEmpty(filter))
                    // {
                    //    filter = string.Format("ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'", dt.Year, dt.Month, dt.Day);
                    // }
                    // else
                    // {
                    //    filter += string.Format(" AND ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'", dt.Year, dt.Month, dt.Day);
                    // }
                    this.listCounts = this.listCounts.Where(count => count.ActivityCountDateTime >= minDate && count.ActivityCountDateTime <= maxDate).ToList();
                }
            }
        }

        /// <inheritdoc />
        public override void GetFilter(XDocument filterData)
        {
            var filter = string.Empty;
            var cycleSettings = new CycleCountSettings().LoadData();

            if (cycleSettings.IsApplyCountDate)
            {
                if (string.Compare(filterData.Root.Element("count_date") !.Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(filterData.Root.Element("count_date") !.Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    // if (string.IsNullOrEmpty(filter))
                    // {
                    //    filter = string.Format("ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime " + "<= '{0}-{1}-{2} 23:59:59.997'", dt.Year, dt.Month, dt.Day);
                    // }
                    // else
                    // {
                    //    filter += string.Format(" AND ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' AND ActivityCountDateTime " + "<= '{0}-{1}-{2} 23:59:59.997'", dt.Year, dt.Month, dt.Day);
                    // }
                    this.listCounts = this.listCounts.Where(count => count.ActivityCountDateTime >= minDate && count.ActivityCountDateTime <= maxDate).ToList();
                }
            }
        }
    }
}
