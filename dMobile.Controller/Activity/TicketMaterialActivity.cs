// <copyright file="TicketMaterialActivity.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TicketMaterialActivity" />.
    /// </summary>
    public class TicketMaterialActivity : Activity
    {
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
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listProductionTicketMaterial.
        /// </summary>
        private List<ProductionTicketMaterial> listProductionTicketMaterial = new List<ProductionTicketMaterial>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketMaterialActivity"/> class.
        /// </summary>
        public TicketMaterialActivity()
          : base()
        {
            DefineInitialGridDataTemplate();

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

                var isAddKey = true;
                foreach (var item in this.resourceDictionary)
                {
                    if (item.Key == "dataTemplateRawMaterial")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.resourceDictionary.Add("dataTemplateRawMaterial", dataTemplate);
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
            return "Ticket Materials";
        }

        /// <summary>
        /// Display the extra information for the selected activity.
        /// </summary>
        /// <param name="extraInfo">Extra information collection.</param>
        public override void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            base.DisplayExtraInfo(extraInfo);
            if (this.SelectedActivity is ProductionTicketMaterial activity)
            {
                extraInfo.Add(activity.ActivityProductionDescription);
                extraInfo.Add("Warehouse:  " + activity.LocationMapWarehouseName);
            }
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

            foreach (var productionTicket in this.ListHeaders.Cast<ProductionTicketMaterial>())
            {
                // ProductionTicketMaterial productionTicket = new ProductionTicketMaterial
                // {
                //    ActivityProductionDateTime = Convert.ToDateTime(row["ActivityProductionDateTime"] != DBNull.Value ? row["ActivityProductionDateTime"] : null),
                //    ActivityProductionDescription = Convert.ToString(row["ActivityProductionDescription"] != DBNull.Value ? row["ActivityProductionDescription"] : null),
                //    ActivityFKRefno = Convert.ToInt32(row["ActivityFKRefno"] != DBNull.Value ? row["ActivityFKRefno"] : null),
                //    ActivityStringFKRefno = Convert.ToString(row["ActivityStringFKRefno"] != DBNull.Value ? row["ActivityStringFKRefno"] : null),
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                // };
                observableCollection.Add(productionTicket);
            }

            var labelTicketNumber = new Label { Text = "Ticket #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelTicketDate = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelTicketDescription = new Label { Text = "Description", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketNumber.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketDate.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketDescription.Text);

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

            gridHeader.Children.Add(labelTicketNumber);
            gridHeader.Children.Add(labelTicketDate);
            gridHeader.Children.Add(labelTicketDescription);

            Grid.SetColumn(labelTicketNumber, 0);
            Grid.SetColumn(labelTicketDate, 1);
            Grid.SetColumn(labelTicketDescription, 2);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (var productionTicket in observableCollection.Cast<ProductionTicketMaterial>())
            {
                var dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicket.ActivityStringFKRefno);
                var dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(productionTicket.ActivityProductionDateTime.ToString("MM/dd/yyyy")));
                var dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicket.ActivityProductionDescription);

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

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateRawMaterial"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var ticketSettings = new ProductionTicketSettings().LoadData();
            var sql = new StringBuilder();

            sql.Append("SELECT tActivityProduction.ActivityRefno,");
            sql.Append("tActivityProductionMaterial.ActivityFKRefno,");
            sql.Append("tActivityProductionMaterial.ActivityStringFKRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
            sql.Append("tActivityProduction.ActivityProductionDateTime AS ActivityProductionDateTime,");
            sql.Append("tActivityProduction.ActivityProductionDescription AS ActivityProductionDescription,");
            sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
            sql.Append("FROM tActivityProduction ");
            if (isSubmitBatch)
            {
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionMaterial ON tActivityProduction.ActivityRefno ");
                sql.Append("= tActivityProductionMaterial.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionMaterial.ActivityFKRefno,");
                sql.Append("tActivityProductionMaterial.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionMaterial ON tActivityProduction.ActivityRefno ");
                sql.Append("= tActivityProductionMaterial.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionMaterial.ActivityFKRefno,");
                sql.Append("tActivityProductionMaterial.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionMaterial ON tActivityProduction.ActivityRefno ");
                sql.Append("= tActivityProductionMaterial.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
            }
            else
            {
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.AppendFormat("INNER JOIN tActivityProductionMaterial ON tActivityProduction.ActivityRefno ");
                sql.Append("= tActivityProductionMaterial.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            // this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        // TODO: Fix sorting
            //        this.DataViewHdrs.Sort = ticketSettings.GetDataViewSortBy();
            //    }
            // }
            this.listProductionTicketMaterial = await Conn.QueryAsync<ProductionTicketMaterial>(sql.ToString());

            foreach (var sort in ticketSettings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.listProductionTicketMaterial = this.listProductionTicketMaterial.OrderBy(prodTicket => prodTicket.GetType().GetProperty(sort.SortValue).GetValue(prodTicket)).ToList();
                }
                else
                {
                    this.listProductionTicketMaterial = this.listProductionTicketMaterial.OrderByDescending(prodTicket => prodTicket.GetType().GetProperty(sort.SortValue).GetValue(prodTicket)).ToList();
                }
            }

            this.GetFilter();

            this.ListHeaders = this.listProductionTicketMaterial.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            // if (string.IsNullOrEmpty(this.ActFilter))
            // {
            // this.DataViewHdrs.RowFilter = "ActivityStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            this.ListHeaders = this.listProductionTicketMaterial.Where(prodTicket => prodTicket.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            // }
            foreach (var productionTicket in this.ListHeaders.Cast<ProductionTicketMaterial>())
            {
                // ProductionTicketMaterial productionTicket = new ProductionTicketMaterial
                // {
                //    ActivityProductionDateTime = Convert.ToDateTime(row["ActivityProductionDateTime"] != DBNull.Value ? row["ActivityProductionDateTime"] : null),
                //    ActivityProductionDescription = Convert.ToString(row["ActivityProductionDescription"] != DBNull.Value ? row["ActivityProductionDescription"] : null),
                //    ActivityFKRefno = Convert.ToInt32(row["ActivityFKRefno"] != DBNull.Value ? row["ActivityFKRefno"] : null),
                //    ActivityStringFKRefno = Convert.ToString(row["ActivityStringFKRefno"] != DBNull.Value ? row["ActivityStringFKRefno"] : null),
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                // };
                observableCollection.Add(productionTicket);
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
            if (selectedActivity is ProductionTicketMaterial header)
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

        /// <summary>
        /// The AssignRefnoFromActivity.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is ProductionTicketMaterial hdr)
            {
                this.HdrFkRefno = Convert.ToInt32(hdr.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(hdr.ActivityRefno);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
                var tableTicketMaterial = new TableActivityProductionTicketMaterial();
                await tableTicketMaterial.RequestHeaderData();
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            var tableTicketMaterial = new TableActivityProductionTicketMaterial();
            var tableTicketMaterialItem = new TableActivityProductionTicketMaterialItem();
            var tableTicketMaterialDetail = new TableActivityProductionTicketMaterialDetail();

            await tableTicketMaterialDetail.PurgeData();
            await tableTicketMaterialItem.PurgeData();
            await tableTicketMaterial.PurgeData();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
            var ticketSettings = new ProductionTicketSettings().LoadData();

            if (ticketSettings.IsApplyProductionDateFilter)
            {
                if (!SysCommon.IsEmptyDateTime(ticketSettings.ProductionDateFilter))
                {
                    var dt = ticketSettings.ProductionDateFilter;
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listProductionTicketMaterial = this.listProductionTicketMaterial.Where(prodTicket => prodTicket.ActivityProductionDateTime >= minDate && prodTicket.ActivityProductionDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        public override void GetFilter(XDocument data)
        {
            var ticketSettings = new ProductionTicketSettings().LoadData();

            if (ticketSettings.IsApplyProductionDateFilter)
            {
                if (string.Compare(data.Root.Element("production_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("production_date").Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listProductionTicketMaterial = this.listProductionTicketMaterial.Where(prodTicket => prodTicket.ActivityProductionDateTime >= minDate && prodTicket.ActivityProductionDateTime <= maxDate).ToList();
                }
            }
        }
    }
}
