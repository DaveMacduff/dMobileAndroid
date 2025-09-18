// <copyright file="CutSheetFinishedActivity.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="CutSheetFinishedActivity" />.
    /// </summary>
    public class CutSheetFinishedActivity : Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CutSheetFinishedActivity"/> class.
        /// </summary>
        public CutSheetFinishedActivity()
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
                        new Binding("ActivityCutSheetDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
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

        private List<CutSheetFinished> ListCutSheetFinished { get; set; } = new List<CutSheetFinished>();

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
            if (this.SelectedActivity is CutSheetFinished activity)
            {
                extraInfo.Add(activity.ActivityCutSheetDescription);
                extraInfo.Add("Warehouse:  " + activity.LocationMapWarehouseName);
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var sheet in this.ListHeaders.Cast<CutSheetFinished>())
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

            foreach (var cutSheet in observableCollection.Cast<CutSheetFinished>())
            {
                var dataColumn0Width = DependencyService.Get<IGridService>().CalculateTextWidth(
                    cutSheet.ActivityStringFKRefno);

                var dataColumn1Width = DependencyService.Get<IGridService>().CalculateTextWidth(
                    Convert.ToString(cutSheet.ActivityCutSheetDateTime.ToString("MM/dd/yyyy")));

                var dataColumn2Width = DependencyService.Get<IGridService>().CalculateTextWidth(
                    cutSheet.ActivityCutSheetDescription);

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

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task<List<object>> GetActivityList(bool isDisconnectedMode)
        {
            var settings = new CutSheetSettings().LoadData();
            var sql = new StringBuilder();

            sql.Append("SELECT tActivityCutSheet.ActivityRefno,");
            sql.Append("tActivityCutSheetFinished.ActivityFKRefno,");
            sql.Append("tActivityCutSheetFinished.ActivityStringFKRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
            sql.Append("tActivityCutSheet.ActivityCutSheetDateTime AS ActivityCutSheetDateTime,");
            sql.Append("tActivityCutSheet.ActivityCutSheetDescription AS ActivityCutSheetDescription,");
            sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
            sql.Append("FROM tActivityCutSheet ");
            if (isDisconnectedMode)
            {
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCutSheet.ActivityRefno ");
                sql.Append("INNER JOIN tActivityCutSheetFinished ON tActivityCutSheetFinished.ActivityRefno ");
                sql.Append("= tActivityCutSheet.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCutSheet.WarehouseLocationMapRefno ");
                sql.AppendFormat(
                    "WHERE tActivity.ActivityStatus <> {0} ",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(
                    "AND tActivity.ActivitySubType = {0} ",
                    Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityCutSheet.ActivityRefno,");
                sql.Append("tActivityCutSheetFinished.ActivityFKRefno,");
                sql.Append("tActivityCutSheetFinished.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCutSheet.ActivityCutSheetDateTime,");
                sql.Append("tActivityCutSheet.ActivityCutSheetDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCutSheet ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCutSheet.ActivityRefno ");
                sql.Append("INNER JOIN tActivityCutSheetFinished ON tActivityCutSheetFinished.ActivityRefno ");
                sql.Append("= tActivityCutSheet.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCutSheet.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat(
                    "WHERE tActivityItem.ActivityItemStatus <> {0} ",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(
                    "AND tActivity.ActivitySubType = {0} ",
                    Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityCutSheet.ActivityRefno,");
                sql.Append("tActivityCutSheetFinished.ActivityFKRefno,");
                sql.Append("tActivityCutSheetFinished.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCutSheet.ActivityCutSheetDateTime,");
                sql.Append("tActivityCutSheet.ActivityCutSheetDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCutSheet ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCutSheet.ActivityRefno ");
                sql.Append("INNER JOIN tActivityCutSheetFinished ON tActivityCutSheetFinished.ActivityRefno ");
                sql.Append("= tActivityCutSheet.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCutSheet.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItem.ActivityItemRefno ");
                sql.AppendFormat(
                    "WHERE tActivityDetail.ActivityDetailStatus <> {0} ",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(
                    "AND tActivity.ActivitySubType = {0}",
                    Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished));
            }
            else
            {
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCutSheet.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCutSheet.ActivityRefno ");
                sql.Append("INNER JOIN tActivityCutSheetFinished ON tActivityCutSheetFinished.ActivityRefno ");
                sql.Append("= tActivityCutSheet.ActivityRefno ");
                sql.AppendFormat(
                    "WHERE tActivity.ActivitySubType = {0}",
                    Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished));
            }

            this.ListCutSheetFinished = await this.Conn.QueryAsync<CutSheetFinished>(
                sql.ToString());
            foreach (var sort in settings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.ListCutSheetFinished = this.ListCutSheetFinished.OrderBy(adj => adj.GetType().
                        GetProperty(sort.SortValue).GetValue(adj)).ToList();
                }
                else
                {
                    this.ListCutSheetFinished = this.ListCutSheetFinished.OrderByDescending(adj => adj.GetType().
                        GetProperty(sort.SortValue).GetValue(adj)).ToList();
                }
            }

            this.GetFilter();

            this.ListHeaders = this.ListCutSheetFinished.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListHeaders = this.ListCutSheetFinished.Where(
                prodTicket => prodTicket.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).
                Cast<object>().ToList();

            foreach (var productionTicketFinished in this.ListHeaders.Cast<CutSheetFinished>())
            {
                observableCollection.Add(productionTicketFinished);
            }

            return observableCollection;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is CutSheetFinished header)
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
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is CutSheetFinished hdr)
            {
                this.HdrFkRefno = Convert.ToInt32(hdr.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(hdr.ActivityRefno);
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task Refresh(XDocument xmlParam)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
                var tableFinished = new TableActivityCutSheetFinished();
                await tableFinished.RequestHeaderData();
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override async Task Purge()
        {
            var tableFinished = new TableActivityCutSheetFinished();
            var tableFinishedItem = new TableActivityCutSheetFinishedItem();
            var tableFinishedDetail = new TableActivityCutSheetFinishedDetail();

            await tableFinishedDetail.PurgeData();
            await tableFinishedItem.PurgeData();
            await tableFinished.PurgeData();
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override void GetFilter()
        {
            var ticketSettings = new CutSheetSettings().LoadData();

            if (ticketSettings.IsApplyCutSheetDateFilter)
            {
                if (!SysCommon.IsEmptyDateTime(ticketSettings.CutSheetDateFilter))
                {
                    var dt = ticketSettings.CutSheetDateFilter;
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.ListCutSheetFinished = this.ListCutSheetFinished.Where(
                        prodTicket => prodTicket.ActivityCutSheetDateTime >= minDate &&
                        prodTicket.ActivityCutSheetDateTime <= maxDate).ToList();
                }
            }
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override void GetFilter(XDocument data)
        {
            var ticketSettings = new CutSheetSettings().LoadData();

            if (ticketSettings.IsApplyCutSheetDateFilter)
            {
                if (string.Compare(data.Root.Element("cutsheet_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("cutsheet_date").Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.ListCutSheetFinished = this.ListCutSheetFinished.Where(
                        prodTicket => prodTicket.ActivityCutSheetDateTime >= minDate &&
                        prodTicket.ActivityCutSheetDateTime <= maxDate).ToList();
                }
            }
        }
    }
}
