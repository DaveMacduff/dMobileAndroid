// <copyright file="YieldSheetMaterialActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
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

    /// <summary>
    /// Defines the <see cref="YieldSheetMaterialActivity" />.
    /// </summary>
    public class YieldSheetMaterialActivity : Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YieldSheetMaterialActivity"/> class.
        /// </summary>
        public YieldSheetMaterialActivity()
        {
            DefineInitialGridDataTemplate();

            void DefineInitialGridDataTemplate()
            {
                var dataTemplate = new DataTemplate(() =>
                {
                    var gridData = new Grid
                    {
                        Padding = new Thickness(5),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(this.Column0Width, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.Column1Width, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.Column2Width, GridUnitType.Absolute) },
                        },
                    };

                    var labelActivityRefno = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityDescription = new Label { VerticalOptions = LayoutOptions.Center };

                    labelActivityRefno.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
                    labelActivityDateTime.SetBinding(
                        Label.TextProperty,
                        new Binding("ActivityYieldSheetDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
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
                foreach (var item in this.ResDict)
                {
                    if (item.Key == "dataTemplateFinished")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.ResDict.Add("dataTemplateFinished", dataTemplate);
                }
            }
        }

        private List<YieldSheetMaterial> ListYieldSheetMaterial { get; set; } = new List<YieldSheetMaterial>();

        private ResourceDictionary ResDict { get; } = new ResourceDictionary();

        private double Column0Width { get; set; } = 0;

        private double Column1Width { get; set; } = 0;

        private double Column2Width { get; set; } = 0;

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override string GetControlDescription()
        {
            return "Sheets Finished";
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            base.DisplayExtraInfo(extraInfo);
            if (this.SelectedActivity is YieldSheetMaterial activity)
            {
                extraInfo.Add(activity.ActivityYieldSheetDescription);
                extraInfo.Add("Warehouse:  " + activity.LocationMapWarehouseName);
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task<List<object>> GetActivityList(bool isDisconnectedMode)
        {
            var settings = new YieldSheetSettings().LoadData();
            var actNoChange = (int)SysCommon.ActStatus.NoChange;
            var cutSheetFinished = (int)SysCommon.ActSubTypes.YieldSheetMaterial;

            string sql;
            object[] parameters;

            if (isDisconnectedMode)
            {
                // One pass using EXISTS instead of UNIONs
                sql = @"
                    SELECT DISTINCT
                        tActivityYieldSheet.ActivityRefno,
                        tActivityYieldSheetMaterial.ActivityFKRefno,
                        tActivityYieldSheetMaterial.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityYieldSheet.ActivityYieldSheetDateTime AS ActivityYieldSheetDateTime,
                        tActivityYieldSheet.ActivityYieldSheetDescription AS ActivityYieldSheetDescription,
                        tActivity.ActivityStatus AS ActivityStatus
                    FROM tActivityYieldSheet
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    INNER JOIN tActivityYieldSheetMaterial ON tActivityYieldSheetMaterial.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityYieldSheet.WarehouseLocationMapRefno
                    WHERE tActivity.ActivitySubType = ?
                      AND (
                            tActivity.ActivityStatus <> ?
                            OR EXISTS (
                                SELECT 1
                                FROM tActivityItem ai
                                WHERE ai.ActivityRefno = tActivity.ActivityRefno
                                  AND ai.ActivityItemStatus <> ?
                            )
                            OR EXISTS (
                                SELECT 1
                                FROM tActivityItem ai
                                INNER JOIN tActivityDetail ad ON ad.ActivityItemRefno = ai.ActivityItemRefno
                                WHERE ai.ActivityRefno = tActivity.ActivityRefno
                                  AND ad.ActivityDetailStatus <> ?
                            )
                      )";
                parameters = new object[] { cutSheetFinished, actNoChange, actNoChange, actNoChange };
            }
            else
            {
                sql = @"
                    SELECT
                        tActivityYieldSheet.ActivityRefno,
                        tActivityYieldSheetMaterial.ActivityFKRefno,
                        tActivityYieldSheetMaterial.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityYieldSheet.ActivityYieldSheetDateTime AS ActivityYieldSheetDateTime,
                        tActivityYieldSheet.ActivityYieldSheetDescription AS ActivityYieldSheetDescription,
                        tActivity.ActivityStatus AS ActivityStatus
                    FROM tActivityYieldSheet
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityYieldSheet.WarehouseLocationMapRefno
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    INNER JOIN tActivityYieldSheetMaterial ON tActivityYieldSheetMaterial.ActivityRefno = tActivityYieldSheet.ActivityRefno
                    WHERE tActivity.ActivitySubType = ?";
                parameters = new object[] { cutSheetFinished };
            }

            var list = await this.Conn.QueryAsync<YieldSheetMaterial>(sql, parameters);

            // Fast sort without reflection
            foreach (var sort in settings.GetDataViewSortBy())
            {
                list = ApplySort(list, sort.SortValue, sort.SortOrder == Resources.Enums.SortOrder.ASC).ToList();
            }

            this.ListYieldSheetMaterial = list;
            this.GetFilter(); // Consider pushing this date filter into SQL too
            this.ListHeaders = this.ListYieldSheetMaterial.Cast<object>().ToList();
            return this.ListHeaders;

            static IEnumerable<YieldSheetMaterial> ApplySort(IEnumerable<YieldSheetMaterial> src, string key, bool asc)
            {
                return key switch
                {
                    nameof(YieldSheetMaterial.ActivityStringFKRefno) => asc
                        ? src.OrderBy(x => x.ActivityStringFKRefno)
                        : src.OrderByDescending(x => x.ActivityStringFKRefno),
                    nameof(YieldSheetMaterial.ActivityYieldSheetDateTime) => asc
                        ? src.OrderBy(x => x.ActivityYieldSheetDateTime)
                        : src.OrderByDescending(x => x.ActivityYieldSheetDateTime),
                    nameof(YieldSheetMaterial.ActivityYieldSheetDescription) => asc
                        ? src.OrderBy(x => x.ActivityYieldSheetDescription)
                        : src.OrderByDescending(x => x.ActivityYieldSheetDescription),
                    nameof(YieldSheetMaterial.LocationMapWarehouseName) => asc
                        ? src.OrderBy(x => x.LocationMapWarehouseName)
                        : src.OrderByDescending(x => x.LocationMapWarehouseName),
                    _ => src,   // unknown sort key: no-op
                };
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListHeaders = this.ListYieldSheetMaterial.Where(
                prodTicket => prodTicket.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).
                Cast<object>().ToList();

            foreach (var productionTicketFinished in this.ListHeaders.Cast<YieldSheetMaterial>())
            {
                observableCollection.Add(productionTicketFinished);
            }

            return observableCollection;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is YieldSheetMaterial header)
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
                    if (this.SkidInfo.CurrSkid?.SkidFkRefno > 0 &&
                        this.SkidInfo.CurrSkid.LocMapRefno != header.LocationMapRefno &&
                        await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        // NOTE: We may have to use a temp var to assign the current skid a new location.
                        this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override void AssignRefnoFromActivity(object selectedActivity)
        {
            if (selectedActivity is YieldSheetMaterial hdr)
            {
                this.HdrFkRefno = Convert.ToInt32(hdr.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(hdr.ActivityRefno);
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
                var tableFinished = new TableActivityYieldSheetMaterial();
                await tableFinished.RequestHeaderData();
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task Purge()
        {
            var tableFinished = new TableActivityYieldSheetMaterial();
            var tableFinishedItem = new TableActivityYieldSheetMaterialItem();
            var tableFinishedDetail = new TableActivityYieldSheetMaterialDetail();

            await tableFinishedDetail.PurgeData();
            await tableFinishedItem.PurgeData();
            await tableFinished.PurgeData();
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override void GetFilter()
        {
            var ticketSettings = new YieldSheetSettings().LoadData();

            if (ticketSettings.IsApplyYieldSheetDateFilter && !SysCommon.IsEmptyDateTime(ticketSettings.YieldSheetDateFilter))
            {
                var dt = ticketSettings.YieldSheetDateFilter;
                var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00, DateTimeKind.Local);
                var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Local);

                this.ListYieldSheetMaterial = this.ListYieldSheetMaterial.Where(
                    prodTicket => prodTicket.ActivityYieldSheetDateTime >= minDate &&
                    prodTicket.ActivityYieldSheetDateTime <= maxDate).ToList();
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override void GetFilter(XDocument data)
        {
            var ticketSettings = new YieldSheetSettings().LoadData();

            if (ticketSettings.IsApplyYieldSheetDateFilter && string.Compare(data.Root.Element("cutsheet_date").Value, string.Empty) > 0)
            {
                var dt = DateTime.Parse(data.Root.Element("cutsheet_date").Value, new CultureInfo("en-US"));
                var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00, DateTimeKind.Local);
                var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Local);

                this.ListYieldSheetMaterial = this.ListYieldSheetMaterial.Where(
                    prodTicket => prodTicket.ActivityYieldSheetDateTime >= minDate &&
                    prodTicket.ActivityYieldSheetDateTime <= maxDate).ToList();
            }
        }

        /// <summary>
        /// Change the activity header status.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Selected activity to change the status on.</param>
        /// <returns>True if successful else false.</returns>
        public override async Task<bool> ChangeStatus(object selectedActivity)
        {
            var result = false;
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                result = await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        "Complete this cut sheet?",
                        "Yes", "No"));
                if (result)
                {
                    // do xml request to debox
                    var xml = "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>COMPLETE CUT SHEET</action>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<refno>" + this.HdrFkRefno + "</refno>"
                        + "</imclient>";
                    _ = SysCommon.SubmitComm(xml);
                    result = true;
                }
            }

            return result;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var sheet in this.ListHeaders.Cast<YieldSheetMaterial>())
            {
                observableCollection.Add(sheet);
            }

            var labelSheetNumber = new Label
            {
                Text = "Sheet #",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelSheetDate = new Label
            {
                Text = "Date",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelSheetDescription = new Label
            {
                Text = "Description",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            // Calculate text size
            this.Column0Width = DependencyService.Get<IGridService>().CalculateTextWidth(labelSheetNumber.Text);
            this.Column1Width = DependencyService.Get<IGridService>().CalculateTextWidth(labelSheetDate.Text);
            this.Column2Width = DependencyService.Get<IGridService>().CalculateTextWidth(labelSheetDescription.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.Column0Width, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.Column1Width, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.Column2Width, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelSheetNumber);
            gridHeader.Children.Add(labelSheetDate);
            gridHeader.Children.Add(labelSheetDescription);

            Grid.SetColumn(labelSheetNumber, 0);
            Grid.SetColumn(labelSheetDate, 1);
            Grid.SetColumn(labelSheetDescription, 2);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (var cutSheet in observableCollection.Cast<YieldSheetMaterial>())
            {
                var dataColumn0Width = DependencyService.Get<IGridService>().CalculateTextWidth(
                    cutSheet.ActivityStringFKRefno);

                var dataColumn1Width = DependencyService.Get<IGridService>().CalculateTextWidth(
                    Convert.ToString(cutSheet.ActivityYieldSheetDateTime.ToString("MM/dd/yyyy")));

                var dataColumn2Width = DependencyService.Get<IGridService>().CalculateTextWidth(
                    cutSheet.ActivityYieldSheetDescription);

                if (dataColumn0Width > this.Column0Width)
                {
                    this.Column0Width = dataColumn0Width;
                    gridHeader.ColumnDefinitions[0].Width = this.Column0Width;
                }

                if (dataColumn1Width > this.Column1Width)
                {
                    this.Column1Width = dataColumn1Width;
                    gridHeader.ColumnDefinitions[1].Width = this.Column1Width;
                }

                if (dataColumn2Width > this.Column2Width)
                {
                    this.Column2Width = dataColumn2Width;
                    gridHeader.ColumnDefinitions[2].Width = this.Column2Width;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResDict["dataTemplateFinished"];

            return observableCollection;
        }
    }
}
