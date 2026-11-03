// <copyright file="YieldSheetFinishedActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
    /// Defines the <see cref="YieldSheetFinishedActivity" />.
    /// </summary>
    public class YieldSheetFinishedActivity : Activity
    {
        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private readonly ResourceDictionary resDict = new ResourceDictionary();

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
        /// Defines the listYieldSheetFinished.
        /// </summary>
        private List<YieldSheetFinished> listYieldSheetFinished = new List<YieldSheetFinished>();

        /// <summary>
        /// Initializes a new instance of the <see cref="YieldSheetFinishedActivity"/> class.
        /// </summary>
        public YieldSheetFinishedActivity()
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
                    var labelActivityDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityDescription = new Label { VerticalOptions = LayoutOptions.Center };

                    labelActivityRefno.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
                    labelActivityDateTime.SetBinding(Label.TextProperty, new Binding(
                        "ActivityYieldSheetDateTime",
                        BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                    labelActivityDescription.SetBinding(Label.TextProperty, new Binding("ActivityYieldSheetDescription"));

                    gridData.Children.Add(labelActivityRefno, 0, 0);
                    gridData.Children.Add(labelActivityDateTime, 0, 0);
                    gridData.Children.Add(labelActivityDescription, 0, 0);

                    Grid.SetColumn(labelActivityRefno, 0);
                    Grid.SetColumn(labelActivityDateTime, 1);
                    Grid.SetColumn(labelActivityDescription, 2);

                    return new ViewCell { View = gridData };
                });

                var isAddKey = true;
                foreach (var item in this.resDict)
                {
                    if (item.Key == "dataTemplateRawMaterial")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.resDict.Add("dataTemplateRawMaterial", dataTemplate);
                }
            }
        }

        /// <inheritdoc cref="Activity.GetFkRefnoIdx()"/>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <inheritdoc cref="Activity.GetControlDescription()"/>
        public override string GetControlDescription()
        {
            return "YieldSheet Materials";
        }

        /// <inheritdoc cref="Activity.DisplayExtraInfo(ObservableCollection{string})"/>
        public override void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            base.DisplayExtraInfo(extraInfo);
            if (this.SelectedActivity is YieldSheetFinished activity)
            {
                extraInfo.Add(activity.ActivityYieldSheetDescription);
                extraInfo.Add("Warehouse:  " + activity.LocationMapWarehouseName);
            }
        }

        /// <inheritdoc cref="Activity.DesignGrid(ref Grid, ref ListView)"/>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var productionYieldSheet in this.ListHeaders.Cast<YieldSheetFinished>())
            {
                observableCollection.Add(productionYieldSheet);
            }

            var labelYieldSheetNumber = new Label
            {
                Text = "YieldSheet #",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelYieldSheetDate = new Label
            {
                Text = "Date",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelYieldSheetDescription = new Label
            {
                Text = "Description",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            // Calculate text size
            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelYieldSheetNumber.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelYieldSheetDate.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelYieldSheetDescription.Text);

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

            gridHeader.Children.Add(labelYieldSheetNumber);
            gridHeader.Children.Add(labelYieldSheetDate);
            gridHeader.Children.Add(labelYieldSheetDescription);

            Grid.SetColumn(labelYieldSheetNumber, 0);
            Grid.SetColumn(labelYieldSheetDate, 1);
            Grid.SetColumn(labelYieldSheetDescription, 2);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (var sheet in observableCollection.Cast<YieldSheetFinished>())
            {
                var dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    sheet.ActivityStringFKRefno);
                var dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    Convert.ToString(sheet.ActivityYieldSheetDateTime.ToString("MM/dd/yyyy")));
                var dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    sheet.ActivityYieldSheetDescription);

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

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resDict["dataTemplateRawMaterial"];

            return observableCollection;
        }

        /// <inheritdoc cref="Activity.GetActivityList(bool)"/>
        public override async Task<List<object>> GetActivityList(bool isDisconnectedMode)
        {
            var ticketSettings = new YieldSheetSettings().LoadData();

            // Parameterized SQL to reduce allocations and improve plan caching
            string sql;
            object[] parameters;
            const int actStatusNoChange = (int)SysCommon.ActStatus.NoChange;
            const int subTypeYieldSheetFinished = (int)SysCommon.ActSubTypes.YieldSheetFinished;

            if (isDisconnectedMode)
            {
                // Use EXISTS instead of UNIONs to reduce scans
                sql = @"
                    SELECT DISTINCT
                        tActivityYieldSheet.ActivityRefno,
                        tActivityYieldSheetFinished.ActivityFKRefno,
                        tActivityYieldSheetFinished.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityYieldSheet.ActivityYieldSheetDateTime AS ActivityYieldSheetDateTime,
                        tActivityYieldSheet.ActivityYieldSheetDescription AS ActivityYieldSheetDescription,
                        tActivity.ActivityStatus AS ActivityStatus
                    FROM tActivityYieldSheet
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    INNER JOIN tActivityYieldSheetFinished ON tActivityYieldSheetFinished.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityYieldSheet.WarehouseLocationMapRefno
                    WHERE tActivity.ActivitySubType = ?
                      AND (
                            tActivity.ActivityStatus <> ?
                            OR EXISTS (
                                SELECT 1 FROM tActivityItem ai
                                WHERE ai.ActivityRefno = tActivity.ActivityRefno
                                  AND ai.ActivityItemStatus <> ?
                            )
                            OR EXISTS (
                                SELECT 1 FROM tActivityItem ai
                                INNER JOIN tActivityDetail ad ON ad.ActivityItemRefno = ai.ActivityItemRefno
                                WHERE ai.ActivityRefno = tActivity.ActivityRefno
                                  AND ad.ActivityDetailStatus <> ?
                            )
                      )";
                parameters = new object[] { subTypeYieldSheetFinished, actStatusNoChange, actStatusNoChange, actStatusNoChange };
            }
            else
            {
                sql = @"
                    SELECT
                        tActivityYieldSheet.ActivityRefno,
                        tActivityYieldSheetFinished.ActivityFKRefno,
                        tActivityYieldSheetFinished.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityYieldSheet.ActivityYieldSheetDateTime AS ActivityYieldSheetDateTime,
                        tActivityYieldSheet.ActivityYieldSheetDescription AS ActivityYieldSheetDescription,
                        tActivity.ActivityStatus AS ActivityStatus
                    FROM tActivityYieldSheet
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityYieldSheet.WarehouseLocationMapRefno
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    INNER JOIN tActivityYieldSheetFinished ON tActivityYieldSheetFinished.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    WHERE tActivity.ActivitySubType = ?";
                parameters = new object[] { subTypeYieldSheetFinished };
            }

            this.listYieldSheetFinished = await this.Conn.QueryAsync<YieldSheetFinished>(sql, parameters);

            // Replace reflection-based sorting with typed selectors
            foreach (var sort in ticketSettings.GetDataViewSortBy())
            {
                Func<YieldSheetFinished, IComparable> keySelector = sort.SortValue switch
                {
                    nameof(YieldSheetFinished.ActivityStringFKRefno) => (Func<YieldSheetFinished, IComparable>)(x => x.ActivityStringFKRefno),
                    nameof(YieldSheetFinished.ActivityYieldSheetDateTime) => x => x.ActivityYieldSheetDateTime,
                    nameof(YieldSheetFinished.ActivityYieldSheetDescription) => x => x.ActivityYieldSheetDescription,
                    nameof(YieldSheetFinished.LocationMapWarehouseName) => x => x.LocationMapWarehouseName,
                    nameof(YieldSheetFinished.ActivityStatus) => x => x.ActivityStatus,
                    nameof(YieldSheetFinished.ActivityRefno) => x => x.ActivityRefno,
                    nameof(YieldSheetFinished.ActivityFKRefno) => x => x.ActivityFKRefno,
                    _ => null,
                };

                if (keySelector == null)
                {
                    continue; // unknown column: skip to avoid reflection costs
                }

                this.listYieldSheetFinished = (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                    ? this.listYieldSheetFinished.OrderBy(keySelector).ToList()
                    : this.listYieldSheetFinished.OrderByDescending(keySelector).ToList();
            }

            this.GetFilter();

            this.ListHeaders = this.listYieldSheetFinished.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListHeaders = this.listYieldSheetFinished.Where(prodYieldSheet => prodYieldSheet.
                ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            foreach (var productionYieldSheet in this.ListHeaders.Cast<YieldSheetFinished>())
            {
                observableCollection.Add(productionYieldSheet);
            }

            return observableCollection;
        }

        /// <inheritdoc cref="Activity.SelectActivity(object)"/>/>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is YieldSheetFinished header)
            {
                this.SelectedActivity = header;
                this.HdrFkRefno = header.ActivityFKRefno;
                this.HdrRefno = header.ActivityRefno;
                this.LocationMapRefno = header.LocationMapRefno;

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // Set the current location to the header.
                    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            header.LocationMapRefno);
                    }

                    // Override and set the chosen location to the current location.
                    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }

                    // Override and set any selected skid location to the current location.
                    if (this.SkidInfo.CurrSkid != null && this.SkidInfo.CurrSkid.SkidFkRefno > 0 &&
                        this.SkidInfo.CurrSkid.LocMapRefno != header.LocationMapRefno &&
                        await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        // NOTE: We may have to use a temp var to assign the current skid a new location.
                        this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }
            }
        }

        /// <inheritdoc cref="Activity.AssignRefnoFromActivity(object)"/>
        public override void AssignRefnoFromActivity(object selectedActivity)
        {
            if (selectedActivity is YieldSheetFinished hdr)
            {
                this.HdrFkRefno = Convert.ToInt32(hdr.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(hdr.ActivityRefno);
            }
        }

        /// <inheritdoc cref="Activity.Refresh(XDocument)"/>/>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode && !this.ShouldThrottleRefresh())
            {
                await this.Purge();
                var tableYieldSheetFinished = new TableActivityYieldSheetFinished();
                await tableYieldSheetFinished.RequestHeaderData();
                this.MarkRefreshed();
            }
        }

        /// <inheritdoc cref="Activity.Purge()"/>
        public override async Task Purge()
        {
            var tableYieldSheetFinished = new TableActivityYieldSheetFinished();
            var tableYieldSheetFinishedItem = new TableActivityYieldSheetFinishedItem();
            var tableYieldSheetFinishedDetail = new TableActivityYieldSheetFinishedDetail();

            await tableYieldSheetFinishedDetail.PurgeData();
            await tableYieldSheetFinishedItem.PurgeData();
            await tableYieldSheetFinished.PurgeData();
        }

        /// <inheritdoc cref="Activity.GetFilter()"/>/>
        public override void GetFilter()
        {
            var settings = new YieldSheetSettings().LoadData();
            if (settings.IsApplyYieldSheetDateFilter && !SysCommon.IsEmptyDateTime(settings.YieldSheetDateFilter))
            {
                var dt = settings.YieldSheetDateFilter;
                var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00, DateTimeKind.Local);
                var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Local);
                this.listYieldSheetFinished = this.listYieldSheetFinished.Where(
                    prodYieldSheet => prodYieldSheet.ActivityYieldSheetDateTime >= minDate &&
                    prodYieldSheet.ActivityYieldSheetDateTime <= maxDate).ToList();
            }
        }

        /// <inheritdoc cref="Activity.GetFilter(XDocument)"/>/>
        public override void GetFilter(XDocument data)
        {
            var settings = new YieldSheetSettings().LoadData();
            if (settings.IsApplyYieldSheetDateFilter &&
                string.Compare(data.Root.Element("YieldSheet_date").Value, string.Empty) > 0)
            {
                var dt = DateTime.Parse(data.Root.Element("YieldSheet_date").Value, new CultureInfo("en-US"));
                var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00, DateTimeKind.Local);
                var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Local);
                this.listYieldSheetFinished = this.listYieldSheetFinished.Where(prodYieldSheet =>
                    prodYieldSheet.ActivityYieldSheetDateTime >= minDate &&
                    prodYieldSheet.ActivityYieldSheetDateTime <= maxDate).ToList();
            }
        }
    }
}
