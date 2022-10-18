// <copyright file="TicketByProdActivity.cs" company="InterSect Business Systems Inc">
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
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="TicketByProdActivity" />.
    /// </summary>
    public class TicketByProdActivity : Activity
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
        /// Defines the listProductionTicketByProducts.
        /// </summary>
        private List<ProductionTicketByProduct> listProductionTicketByProducts = new List<ProductionTicketByProduct>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketByProdActivity"/> class.
        /// </summary>
        public TicketByProdActivity()
          : base()
        {
            this.FullClassName = typeof(TicketByProdActivity).FullName;

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
                if (item.Key == "dataTemplateByProduct")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateByProduct", dataTemplate);
            }
        }

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFKRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Ticket Additional Products";
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

            foreach (ProductionTicketByProduct productionTicketByProduct in this.ListHdrs)
            {
                // ProductionTicketByProduct productionTicket = new ProductionTicketByProduct
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
                observableCollection.Add(productionTicketByProduct);
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

            foreach (ProductionTicketByProduct productionTicket in observableCollection)
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

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateByProduct"];

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

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionByProduct.ActivityFKRefno,");
                sql.Append("tActivityProductionByProduct.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime AS ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription AS ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionByProduct ON tActivityProductionByProduct.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionByProduct.ActivityFKRefno,");
                sql.Append("tActivityProductionByProduct.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionByProduct ON tActivityProductionByProduct.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionByProduct.ActivityFKRefno,");
                sql.Append("tActivityProductionByProduct.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionByProduct ON tActivityProductionByProduct.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
            }
            else
            {
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionByProduct.ActivityFKRefno,");
                sql.Append("tActivityProductionByProduct.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime AS ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription AS ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionByProduct ON tActivityProductionByProduct.ActivityRefno ");
                sql.Append("= tActivityProduction.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            // this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            //        this.DataViewHdrs.Sort = ticketSettings.GetDataViewSortBy();
            //    }
            // }
            this.listProductionTicketByProducts = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductionTicketByProduct>(sql.ToString());

            foreach (var sort in ticketSettings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.listProductionTicketByProducts = this.listProductionTicketByProducts.OrderBy(prodTicket => prodTicket.GetType().GetProperty(sort.SortValue).GetValue(prodTicket)).ToList();
                }
                else
                {
                    this.listProductionTicketByProducts = this.listProductionTicketByProducts.OrderByDescending(prodTicket => prodTicket.GetType().GetProperty(sort.SortValue).GetValue(prodTicket)).ToList();
                }
            }

            this.GetFilter();

            this.ListHdrs = this.listProductionTicketByProducts.Cast<object>().ToList();
            return this.ListHdrs;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            this.ListHdrs = this.listProductionTicketByProducts.Where(prodTicket => prodTicket.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (ProductionTicketByProduct productionTicketByProduct in this.ListHdrs)
            {
                // ProductionTicketByProduct productionTicket = new ProductionTicketByProduct
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
                observableCollection.Add(productionTicketByProduct);
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
            if (selectedActivity is ProductionTicketByProduct header)
            {
                this.SelectedActivity = header;
                this.HdrFKRefno = header.ActivityFKRefno;
                this.HdrRefno = header.ActivityRefno;

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            header.LocationMapRefno);
                    }

                    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }

                    if (this.SkidInfo.CurrSkid != null && this.SkidInfo.CurrSkid.SkidFKRefno > 0 &&
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
        /// The GetRefnoFromRowIdx.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            if (selectedItem is ProductionTicketByProduct productionTicketByProduct)
            {
                this.HdrFKRefno = Convert.ToInt32(productionTicketByProduct.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(productionTicketByProduct.ActivityRefno);
            }

            return this.HdrRefno;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
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

                var tableActTicketByProduct = new TableActivityProductionTicketByProduct();

                tableActTicketByProduct.RegisterDataAccessListenerProgressOut((TicketByProdActivity)this);
                await tableActTicketByProduct.RequestData();
                tableActTicketByProduct.UnregisterDataAccessListenerProgressOut((TicketByProdActivity)this);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            var tableActTicketByProduct = new TableActivityProductionTicketByProduct();
            var tableActTicketByProductItem = new TableActivityProductionTicketByProductItem();
            var tableActTicketByProductDetail = new TableActivityProductionTicketByProductDetail();

            await tableActTicketByProductDetail.PurgeData();
            await tableActTicketByProductItem.PurgeData();
            await tableActTicketByProduct.PurgeData();
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

                    this.listProductionTicketByProducts = this.listProductionTicketByProducts.Where(prodTicket => prodTicket.ActivityProductionDateTime >= minDate && prodTicket.ActivityProductionDateTime <= maxDate).ToList();
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
            var ticketSettings = new ProductionTicketSettings().LoadData();

            if (ticketSettings.IsApplyProductionDateFilter)
            {
                if (string.Compare(data.Root.Element("production_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("production_date").Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listProductionTicketByProducts = this.listProductionTicketByProducts.Where(prodTicket => prodTicket.ActivityProductionDateTime >= minDate && prodTicket.ActivityProductionDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model (tActivity??Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            object tmpFKRefno = ((ProductionTicketByProduct)resourceType).ActivityFKRefno;
            return tmpFKRefno == null ? 0 : Convert.ToInt32(tmpFKRefno);
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="xmlParams">The xmlParams<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override Task<int> Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PrintLabel.
        /// </summary>
        /// <param name="xmlParam">The xmlParam<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task PrintLabel(XDocument xmlParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ChangeStatus.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public override Task<bool> ChangeStatus(object selectedActivity)
        {
            throw new NotImplementedException();
        }
    }
}
