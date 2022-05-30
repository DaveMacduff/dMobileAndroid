// <copyright file="clsProductionTicketMaterialActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

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
            this.FullClassName = typeof(TicketMaterialActivity).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    },
                };

                Label labelActivityRefno = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityRefno.SetBinding(Label.TextProperty, new Binding("ActivityProductionStringFKRefno"));
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

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFKRefnoIdx()
        {
            return "ActivityProductionStringFKRefno";
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
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (ProductionTicketMaterial productionTicket in this.ListHdrs)
            {
                // ProductionTicketMaterial productionTicket = new ProductionTicketMaterial
                // {
                //    ActivityProductionDateTime = Convert.ToDateTime(row["ActivityProductionDateTime"] != DBNull.Value ? row["ActivityProductionDateTime"] : null),
                //    ActivityProductionDescription = Convert.ToString(row["ActivityProductionDescription"] != DBNull.Value ? row["ActivityProductionDescription"] : null),
                //    ActivityProductionFKRefno = Convert.ToInt32(row["ActivityProductionFKRefno"] != DBNull.Value ? row["ActivityProductionFKRefno"] : null),
                //    ActivityProductionStringFKRefno = Convert.ToString(row["ActivityProductionStringFKRefno"] != DBNull.Value ? row["ActivityProductionStringFKRefno"] : null),
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                // };
                observableCollection.Add(productionTicket);
            }

            Label labelTicketNumber = new Label { Text = "Ticket #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelTicketDate = new Label { Text = "Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelTicketDescription = new Label { Text = "Description", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketNumber.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketDate.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelTicketDescription.Text);

            Grid gridHeader = new Grid
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

            GridMainActivity.Children.Add(gridHeader);

            foreach (ProductionTicketMaterial productionTicket in observableCollection)
            {
                double dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicket.ActivityProductionStringFKRefno);
                double dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(productionTicket.ActivityProductionDateTime.ToString("MM/dd/yyyy")));
                double dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicket.ActivityProductionDescription);

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

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityProduction.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityProductionMaterial.ActivityProductionFKRefno AS ActivityProductionFKRefno,");
                sql.Append("tActivityProductionMaterial.ActivityProductionStringFKRefno AS ActivityProductionStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityProduction.ActivityProductionDateTime AS ActivityProductionDateTime,");
                sql.Append("tActivityProduction.ActivityProductionDescription AS ActivityProductionDescription,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus ");
                sql.Append("FROM tActivityProduction ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
                sql.Append("INNER JOIN tActivityProductionMaterial ON tActivityProduction.ActivityRefno ");
                sql.Append("= tActivityProductionMaterial.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityProduction.ActivityRefno,");
                sql.Append("tActivityProductionMaterial.ActivityProductionFKRefno,");
                sql.Append("tActivityProductionMaterial.ActivityProductionStringFKRefno,");
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
                sql.Append("tActivityProductionMaterial.ActivityProductionFKRefno,");
                sql.Append("tActivityProductionMaterial.ActivityProductionStringFKRefno,");
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
                sql.Append("SELECT tActivityProduction.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityProductionMaterial.ActivityProductionFKRefno AS ActivityProductionFKRefno,");
                sql.Append("tActivityProductionMaterial.ActivityProductionStringFKRefno AS ActivityProductionStringFKRefno,");
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
            this.listProductionTicketMaterial = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductionTicketMaterial>(sql.ToString());

            foreach (Sort sort in ticketSettings.GetDataViewSortBy())
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

            this.ListHdrs = this.listProductionTicketMaterial.Cast<object>().ToList();
            return this.ListHdrs;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // if (string.IsNullOrEmpty(this.ActFilter))
            // {
            // this.DataViewHdrs.RowFilter = "ActivityProductionStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            this.ListHdrs = this.listProductionTicketMaterial.Where(prodTicket => prodTicket.ActivityProductionStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityProductionStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            // }
            foreach (ProductionTicketMaterial productionTicket in this.ListHdrs)
            {
                // ProductionTicketMaterial productionTicket = new ProductionTicketMaterial
                // {
                //    ActivityProductionDateTime = Convert.ToDateTime(row["ActivityProductionDateTime"] != DBNull.Value ? row["ActivityProductionDateTime"] : null),
                //    ActivityProductionDescription = Convert.ToString(row["ActivityProductionDescription"] != DBNull.Value ? row["ActivityProductionDescription"] : null),
                //    ActivityProductionFKRefno = Convert.ToInt32(row["ActivityProductionFKRefno"] != DBNull.Value ? row["ActivityProductionFKRefno"] : null),
                //    ActivityProductionStringFKRefno = Convert.ToString(row["ActivityProductionStringFKRefno"] != DBNull.Value ? row["ActivityProductionStringFKRefno"] : null),
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
            if (selectedActivity is ProductionTicketMaterial productionTicketMaterial)
            {
                this.SelectedActivity = productionTicketMaterial;
                this.FKRefno = Convert.ToInt32(productionTicketMaterial.ActivityProductionFKRefno);
                this.Refno = Convert.ToInt32(productionTicketMaterial.ActivityRefno);

                // Set the selected warehouse to the activity header warehouse.
                if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(productionTicketMaterial.LocationMapRefno))
                {
                    LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(productionTicketMaterial.LocationMapRefno));
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
            if (selectedItem is ProductionTicketMaterial productionTicketMaterial)
            {
                this.FKRefno = Convert.ToInt32(productionTicketMaterial.ActivityProductionFKRefno);
                this.Refno = Convert.ToInt32(productionTicketMaterial.ActivityRefno);
            }

            return this.Refno;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
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

                TableActivityProductionTicketMaterial tableTicketMaterial = new TableActivityProductionTicketMaterial();

                tableTicketMaterial.RegisterDataAccessListenerProgressOut((TicketMaterialActivity)this);
                await tableTicketMaterial.RequestData();
                tableTicketMaterial.UnregisterDataAccessListenerProgressOut((TicketMaterialActivity)this);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            TableActivityProductionTicketMaterial tableTicketMaterial = new TableActivityProductionTicketMaterial();
            TableActivityProductionTicketMaterialItem tableTicketMaterialItem = new TableActivityProductionTicketMaterialItem();
            TableActivityProductionTicketMaterialDetail tableTicketMaterialDetail = new TableActivityProductionTicketMaterialDetail();

            await tableTicketMaterialDetail.PurgeData();
            await tableTicketMaterialItem.PurgeData();
            await tableTicketMaterial.PurgeData();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
            ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();

            if (ticketSettings.IsApplyProductionDateFilter)
            {
                if (!SysCommon.IsEmptyDateTime(ticketSettings.ProductionDateFilter))
                {
                    DateTime dt = ticketSettings.ProductionDateFilter;
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

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
            ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();

            if (ticketSettings.IsApplyProductionDateFilter)
            {
                if (string.Compare(data.Root.Element("production_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("production_date").Value);
                    DateTime minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    DateTime maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listProductionTicketMaterial = this.listProductionTicketMaterial.Where(prodTicket => prodTicket.ActivityProductionDateTime >= minDate && prodTicket.ActivityProductionDateTime <= maxDate).ToList();
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
            object tmpFKRefno = ((ProductionTicketMaterial)resourceType).ActivityProductionFKRefno;
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
