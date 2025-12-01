// <copyright file="CutSheetMaterialActivity.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="CutSheetMaterialActivity" />.
    /// </summary>
    public class CutSheetMaterialActivity : Activity
    {
        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private readonly ResourceDictionary resDict = new ResourceDictionary();

        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double columnWidth0 = 0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double columnWidth1 = 0;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double columnWidth2 = 0;

        /// <summary>
        /// Defines the listCutSheetMaterial.
        /// </summary>
        private List<CutSheetMaterial> listCutSheetMaterial = new List<CutSheetMaterial>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CutSheetMaterialActivity"/> class.
        /// </summary>
        public CutSheetMaterialActivity()
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
                        "ActivityCutSheetDateTime",
                        BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                    labelActivityDescription.SetBinding(Label.TextProperty, new Binding("ActivityCutSheetDescription"));

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
            return "CutSheet Materials";
        }

        /// <inheritdoc cref="Activity.DisplayExtraInfo(ObservableCollection{string})"/>
        public override void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            base.DisplayExtraInfo(extraInfo);
            if (this.SelectedActivity is CutSheetMaterial activity)
            {
                extraInfo.Add(activity.ActivityCutSheetDescription);
                extraInfo.Add("Warehouse:  " + activity.LocationMapWarehouseName);
            }
        }

        /// <inheritdoc cref="Activity.DesignGrid(ref Grid, ref ListView)"/>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var productionCutSheet in this.ListHeaders.Cast<CutSheetMaterial>())
            {
                observableCollection.Add(productionCutSheet);
            }

            var labelCutSheetNumber = new Label
            {
                Text = "CutSheet #",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelCutSheetDate = new Label
            {
                Text = "Date",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelCutSheetDescription = new Label
            {
                Text = "Description",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            // Calculate text size
            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCutSheetNumber.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCutSheetDate.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCutSheetDescription.Text);

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

            gridHeader.Children.Add(labelCutSheetNumber);
            gridHeader.Children.Add(labelCutSheetDate);
            gridHeader.Children.Add(labelCutSheetDescription);

            Grid.SetColumn(labelCutSheetNumber, 0);
            Grid.SetColumn(labelCutSheetDate, 1);
            Grid.SetColumn(labelCutSheetDescription, 2);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (var sheet in observableCollection.Cast<CutSheetMaterial>())
            {
                var dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    sheet.ActivityStringFKRefno);
                var dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    Convert.ToString(sheet.ActivityCutSheetDateTime.ToString("MM/dd/yyyy")));
                var dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    sheet.ActivityCutSheetDescription);

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
            var ticketSettings = new CutSheetSettings().LoadData();

            // Parameterized SQL to reduce allocations and improve plan caching
            string sql;
            object[] parameters;
            const int actStatusNoChange = (int)SysCommon.ActStatus.NoChange;
            const int subTypeCutSheetMaterial = (int)SysCommon.ActSubTypes.CutSheetMaterial;

            if (isDisconnectedMode)
            {
                // Use EXISTS instead of UNIONs to reduce scans
                sql = @"
                    SELECT DISTINCT
                        tActivityCutSheet.ActivityRefno,
                        tActivityCutSheetMaterial.ActivityFKRefno,
                        tActivityCutSheetMaterial.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityCutSheet.ActivityCutSheetDateTime AS ActivityCutSheetDateTime,
                        tActivityCutSheet.ActivityCutSheetDescription AS ActivityCutSheetDescription,
                        tActivity.ActivityStatus AS ActivityStatus
                    FROM tActivityCutSheet
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCutSheet.ActivityRefno
                    INNER JOIN tActivityCutSheetMaterial ON tActivityCutSheetMaterial.ActivityRefno = tActivityCutSheet.ActivityRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityCutSheet.WarehouseLocationMapRefno
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
                parameters = new object[] { subTypeCutSheetMaterial, actStatusNoChange, actStatusNoChange, actStatusNoChange };
            }
            else
            {
                sql = @"
                    SELECT
                        tActivityCutSheet.ActivityRefno,
                        tActivityCutSheetMaterial.ActivityFKRefno,
                        tActivityCutSheetMaterial.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityCutSheet.ActivityCutSheetDateTime AS ActivityCutSheetDateTime,
                        tActivityCutSheet.ActivityCutSheetDescription AS ActivityCutSheetDescription,
                        tActivity.ActivityStatus AS ActivityStatus
                    FROM tActivityCutSheet
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityCutSheet.WarehouseLocationMapRefno
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCutSheet.ActivityRefno
                    INNER JOIN tActivityCutSheetMaterial ON tActivityCutSheetMaterial.ActivityRefno = tActivityCutSheet.ActivityRefno
                    WHERE tActivity.ActivitySubType = ?";
                parameters = new object[] { subTypeCutSheetMaterial };
            }

            this.listCutSheetMaterial = await this.Conn.QueryAsync<CutSheetMaterial>(sql, parameters);

            // Replace reflection-based sorting with typed selectors
            foreach (var sort in ticketSettings.GetDataViewSortBy())
            {
                Func<CutSheetMaterial, IComparable> keySelector = sort.SortValue switch
                {
                    nameof(CutSheetMaterial.ActivityStringFKRefno) => (Func<CutSheetMaterial, IComparable>)(x => x.ActivityStringFKRefno),
                    nameof(CutSheetMaterial.ActivityCutSheetDateTime) => x => x.ActivityCutSheetDateTime,
                    nameof(CutSheetMaterial.ActivityCutSheetDescription) => x => x.ActivityCutSheetDescription,
                    nameof(CutSheetMaterial.LocationMapWarehouseName) => x => x.LocationMapWarehouseName,
                    nameof(CutSheetMaterial.ActivityStatus) => x => x.ActivityStatus,
                    nameof(CutSheetMaterial.ActivityRefno) => x => x.ActivityRefno,
                    nameof(CutSheetMaterial.ActivityFKRefno) => x => x.ActivityFKRefno,
                    _ => null,
                };

                if (keySelector == null)
                {
                    continue; // unknown column: skip to avoid reflection costs
                }

                this.listCutSheetMaterial = (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                    ? this.listCutSheetMaterial.OrderBy(keySelector).ToList()
                    : this.listCutSheetMaterial.OrderByDescending(keySelector).ToList();
            }

            this.GetFilter();

            this.ListHeaders = this.listCutSheetMaterial.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListHeaders = this.listCutSheetMaterial.Where(prodCutSheet => prodCutSheet.
                ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            foreach (var productionCutSheet in this.ListHeaders.Cast<CutSheetMaterial>())
            {
                observableCollection.Add(productionCutSheet);
            }

            return observableCollection;
        }

        /// <inheritdoc cref="Activity.SelectActivity(object)"/>/>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is CutSheetMaterial header)
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
            if (selectedActivity is CutSheetMaterial hdr)
            {
                this.HdrFkRefno = Convert.ToInt32(hdr.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(hdr.ActivityRefno);
            }
        }

        /// <inheritdoc cref="Activity.Refresh(XDocument)"/>/>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
                var tableCutSheetMaterial = new TableActivityCutSheetMaterial();
                await tableCutSheetMaterial.RequestHeaderData();
            }
        }

        /// <inheritdoc cref="Activity.Purge()"/>
        public override async Task Purge()
        {
            var tableCutSheetMaterial = new TableActivityCutSheetMaterial();
            var tableCutSheetMaterialItem = new TableActivityCutSheetMaterialItem();
            var tableCutSheetMaterialDetail = new TableActivityCutSheetMaterialDetail();

            await tableCutSheetMaterialDetail.PurgeData();
            await tableCutSheetMaterialItem.PurgeData();
            await tableCutSheetMaterial.PurgeData();
        }

        /// <inheritdoc cref="Activity.GetFilter()"/>/>
        public override void GetFilter()
        {
            var settings = new CutSheetSettings().LoadData();
            if (settings.IsApplyCutSheetDateFilter && !SysCommon.IsEmptyDateTime(settings.CutSheetDateFilter))
            {
                var dt = settings.CutSheetDateFilter;
                var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00, DateTimeKind.Local);
                var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Local);
                this.listCutSheetMaterial = this.listCutSheetMaterial.Where(
                    prodCutSheet => prodCutSheet.ActivityCutSheetDateTime >= minDate &&
                    prodCutSheet.ActivityCutSheetDateTime <= maxDate).ToList();
            }
        }

        /// <inheritdoc cref="Activity.GetFilter(XDocument)"/>/>
        public override void GetFilter(XDocument data)
        {
            var settings = new CutSheetSettings().LoadData();
            if (settings.IsApplyCutSheetDateFilter &&
                string.Compare(data.Root.Element("cutsheet_date").Value, string.Empty) > 0)
            {
                var dt = DateTime.Parse(data.Root.Element("cutsheet_date").Value, new CultureInfo("en-US"));
                var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00, DateTimeKind.Local);
                var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Local);
                this.listCutSheetMaterial = this.listCutSheetMaterial.Where(prodCutSheet =>
                    prodCutSheet.ActivityCutSheetDateTime >= minDate &&
                    prodCutSheet.ActivityCutSheetDateTime <= maxDate).ToList();
            }
        }
    }
}
