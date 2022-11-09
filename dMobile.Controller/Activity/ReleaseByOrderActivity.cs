// <copyright file="ReleaseByOrderActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="ReleaseByOrderActivity" />.
    /// </summary>
    public class ReleaseByOrderActivity : Activity
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
        /// Defines the columnWidth3.
        /// </summary>
        private double columnWidth3 = 0;

        /// <summary>
        /// Defines the columnWidth4.
        /// </summary>
        private double columnWidth4 = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listPicking.
        /// </summary>
        private List<Picking> listPicking = new List<Picking>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseByOrderActivity"/> class.
        /// </summary>
        public ReleaseByOrderActivity()
          : base()
        {
            this.FullClassName = typeof(ReleaseByOrderActivity).FullName;

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                var gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                    },
                };

                var labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                var labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityPickingOrderedDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("ActivityPickingDeliveryDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelSchemeID.SetBinding(Label.TextProperty, new Binding("RouteCode"));
                labelSchemeID.SetBinding(Label.TextProperty, new Binding("RouteStop"));

                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelSchemeID);

                Grid.SetColumn(labelQty, 0);
                Grid.SetColumn(labelWeight, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelSchemeID, 3);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplateReleaseByOrder")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateReleaseByOrder", dataTemplate);
            }
        }

        /// <summary>
        /// The ChangeStatus.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override async Task<bool> ChangeStatus(object selectedItem)
        {
            var isResult = false;

            if (this.SysEnv.SysSettings.IsRelByOrder && !this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                // if (MessageBox.Show(string.Format("Release Order# {0} To Invoice Status?", this.FKRefno), "Release Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Release Order", string.Format("Release Order# {0} To Invoice Status?", this.HdrFKRefno), "Yes", "No")))
                {
                    // GC.Collect();
                    // GC.WaitForPendingFinalizers();
                    var sendReq = string.Empty;
                    var xmlString = new StringBuilder();
                    var xml = XmlWriter.Create(xmlString);

                    var request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>RELEASE SALES ORDER BY ORDER</action>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<refno>" + this.HdrFKRefno + "</refno>"
                        + "</imclient>");

                    request.Save(xml);
                    xml.Flush();
                    xml.Close();

                    var reqData = SysCommon.SubmitComm(xmlString.ToString());

                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var docError = new XmlDocument();
                            docError.LoadXml(reqData);
                            var errorDetail = docError.DocumentElement["detail"].InnerText;

                            throw new Exception(string.Format("Release order # {0} to invoice status failed." + Environment.NewLine + errorDetail, this.HdrFKRefno, null));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Release order # {0} to invoice status failed.", this.HdrFKRefno));
                    }

                    isResult = true;
                }
            }

            return isResult;
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
            return "Release Order";
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

            foreach (Picking picking in this.ListHdrs)
            {
                // Picking picking = new Picking
                // {
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityFKRefno = Convert.ToInt32(row["ActivityFKRefno"] != DBNull.Value ? row["ActivityFKRefno"] : null),
                //    ActivityStringFKRefno = Convert.ToString(row["ActivityStringFKRefno"] != DBNull.Value ? row["ActivityStringFKRefno"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                //    ActivityPickingOrderedDateTime = Convert.ToDateTime(row["ActivityPickingOrderedDateTime"] != DBNull.Value ? row["ActivityPickingOrderedDateTime"] : null),
                //    ActivityPickingDeliveryDateTime = Convert.ToDateTime(row["ActivityPickingDeliveryDateTime"] != DBNull.Value ? row["ActivityPickingDeliveryDateTime"] : null),
                //    ActivityPickingCustomerContactName = Convert.ToString(row["ActivityPickingCustomerContactName"] != DBNull.Value ? row["ActivityPickingCustomerContactName"] : null),
                //    ActivityPickingCustomerShipToInfo = Convert.ToString(row["ActivityPickingCustomerShipToInfo"] != DBNull.Value ? row["ActivityPickingCustomerShipToInfo"] : null),
                //    RouteCode = Convert.ToString(row["RouteCode"] != DBNull.Value ? row["RouteCode"] : null),
                //    RouteDescription = Convert.ToString(row["RouteDescription"] != DBNull.Value ? row["RouteDescription"] : null),
                //    RouteDayOfWeek = Convert.ToString(row["RouteDayOfWeek"] != DBNull.Value ? row["RouteDayOfWeek"] : null),
                //    RouteStop = Convert.ToInt32(row["RouteStop"] != DBNull.Value ? row["RouteStop"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //    ActivityPickingIsReviewed = Convert.ToBoolean(row["ActivityPickingIsReviewed"] != DBNull.Value ? row["ActivityPickingIsReviewed"] : null),
                // };
                observableCollection.Add(picking);
            }

            var labelOrderNumberHeader = new Label { Text = "Order #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelOrderDateHeader = new Label { Text = "Order Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelDeliveryDateHeader = new Label { Text = "Delivery Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelRouteHeader = new Label { Text = "Route", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelStopHeader = new Label { Text = "Stop", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrderNumberHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrderDateHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelDeliveryDateHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelRouteHeader.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelStopHeader.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelOrderNumberHeader);
            gridHeader.Children.Add(labelOrderDateHeader);
            gridHeader.Children.Add(labelDeliveryDateHeader);
            gridHeader.Children.Add(labelRouteHeader);
            gridHeader.Children.Add(labelStopHeader);

            Grid.SetColumn(labelOrderNumberHeader, 0);
            Grid.SetColumn(labelOrderDateHeader, 1);
            Grid.SetColumn(labelDeliveryDateHeader, 2);
            Grid.SetColumn(labelRouteHeader, 3);
            Grid.SetColumn(labelStopHeader, 4);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (Picking picking in observableCollection)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityStringFKRefno);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingOrderedDateTime.ToString("MM/dd/yyyy"));
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingDeliveryDateTime.ToString("MM/dd/yyyy"));
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.RouteCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(picking.RouteStop));

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.columnWidth4;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateReleaseByOrder"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityPicking.ActivityRefno,");
                sql.Append("tActivityPicking.ActivityFKRefno,");
                sql.Append("tActivityPicking.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityPicking.ActivityPickingOrderedDateTime AS ActivityPickingOrderedDateTime,");
                sql.Append("tActivityPicking.ActivityPickingDeliveryDateTime AS ActivityPickingDeliveryDateTime,");
                sql.Append("tActivityPicking.ActivityPickingCustomerContactName AS ActivityPickingCustomerContactName,");
                sql.Append("tActivityPicking.ActivityPickingCustomerShipToInfo AS ActivityPickingCustomerShipToInfo,");
                sql.Append("tRoute.RouteCode AS RouteCode,");
                sql.Append("tRoute.RouteDescription AS RouteDescription,");
                sql.Append("tRoute.RouteDayOfWeek AS RouteDayOfWeek,");
                sql.Append("tActivityPickingRoute.RouteStop AS RouteStop,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityPicking");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityPicking.WarehouseLocationMapRefno");
                sql.Append(" LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno ");
                sql.Append("= tActivityPicking.ActivityRefno");
                sql.Append(" LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno");
                sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.Picking));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityPicking.ActivityRefno,");
                sql.Append("tActivityPicking.ActivityFKRefno,");
                sql.Append("tActivityPicking.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityPicking.ActivityPickingOrderedDateTime,");
                sql.Append("tActivityPicking.ActivityPickingDeliveryDateTime,");
                sql.Append("tActivityPicking.ActivityPickingCustomerContactName,");
                sql.Append("tActivityPicking.ActivityPickingCustomerShipToInfo,");
                sql.Append("tRoute.RouteCode,");
                sql.Append("tRoute.RouteDescription,");
                sql.Append("tRoute.RouteDayOfWeek,");
                sql.Append("tActivityPickingRoute.RouteStop,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityPicking");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityPicking.WarehouseLocationMapRefno");
                sql.Append(" LEFT JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.Append(" LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno ");
                sql.Append("= tActivityPicking.ActivityRefno");
                sql.Append(" LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno");
                sql.AppendFormat(" WHERE tActivity.ActivityStatus = {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivityItem.ActivityItemStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.Picking));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityPicking.ActivityRefno,");
                sql.Append("tActivityPicking.ActivityFKRefno,");
                sql.Append("tActivityPicking.ActivityStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityPicking.ActivityPickingOrderedDateTime,");
                sql.Append("tActivityPicking.ActivityPickingDeliveryDateTime,");
                sql.Append("tActivityPicking.ActivityPickingCustomerContactName,");
                sql.Append("tActivityPicking.ActivityPickingCustomerShipToInfo,");
                sql.Append("tRoute.RouteCode,");
                sql.Append("tRoute.RouteDescription,");
                sql.Append("tRoute.RouteDayOfWeek,");
                sql.Append("tActivityPickingRoute.RouteStop,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityPicking");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityPicking.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.Append(" LEFT JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno");
                sql.Append(" LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno ");
                sql.Append("= tActivityPicking.ActivityRefno");
                sql.Append(" LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno");
                sql.AppendFormat(" WHERE tActivity.ActivityStatus = {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivityItem.ActivityItemStatus = {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivity.ActivitySubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.Picking));
            }
            else
            {
                sql.Append("SELECT tActivityPicking.ActivityRefno,");
                sql.Append(" tActivityPicking.ActivityFKRefno,");
                sql.Append(" tActivityPicking.ActivityStringFKRefno,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append(" tActivityPicking.ActivityPickingOrderedDateTime AS ActivityPickingOrderedDateTime,");
                sql.Append(" tActivityPicking.ActivityPickingDeliveryDateTime AS ActivityPickingDeliveryDateTime,");
                sql.Append(" tActivityPicking.ActivityPickingCustomerContactName AS ActivityPickingCustomerContactName,");
                sql.Append(" tActivityPicking.ActivityPickingCustomerShipToInfo AS ActivityPickingCustomerShipToInfo,");
                sql.Append(" tRoute.RouteCode AS RouteCode,");
                sql.Append(" tRoute.RouteDescription AS RouteDescription,");
                sql.Append(" tRoute.RouteDayOfWeek AS RouteDayOfWeek,");
                sql.Append(" tActivityPickingRoute.RouteStop AS RouteStop,");
                sql.Append(" tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityPicking");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityPicking.WarehouseLocationMapRefno");
                sql.Append(" LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno ");
                sql.Append("= tActivityPicking.ActivityRefno");
                sql.Append(" LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno");
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            // this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = MainGridMappingName;
            //        TODO: Fix filtering and sorting
            //        PickingSettings pickingSettings = new PickingSettings().LoadData();
            //        this.DataViewHdrs.Sort = pickingSettings.GetDataViewSortBy();
            //        this.DataViewHdrs.RowFilter = this.GetFilter();
            //    }
            // }
            this.listPicking = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Picking>(sql.ToString());
            this.ListHdrs = this.listPicking.Cast<object>().ToList();
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

            // if (string.IsNullOrEmpty(ActFilter))
            // {
            // this.DataViewHdrs.RowFilter = "ActivityStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            this.ListHdrs = this.listPicking.Where(picking => picking.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewHdrs.RowFilter = ActFilter + " AND ActivityStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            // }
            foreach (Picking picking in this.ListHdrs)
            {
                // Picking picking = new Picking
                //    {
                //        ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //        ActivityFKRefno = Convert.ToInt32(row["ActivityFKRefno"] != DBNull.Value ? row["ActivityFKRefno"] : null),
                //        ActivityStringFKRefno = Convert.ToString(row["ActivityStringFKRefno"] != DBNull.Value ? row["ActivityStringFKRefno"] : null),
                //        LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //        LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                //        LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                //        ActivityPickingOrderedDateTime = Convert.ToDateTime(row["ActivityPickingOrderedDateTime"] != DBNull.Value ? row["ActivityPickingOrderedDateTime"] : null),
                //        ActivityPickingDeliveryDateTime = Convert.ToDateTime(row["ActivityPickingDeliveryDateTime"] != DBNull.Value ? row["ActivityPickingDeliveryDateTime"] : null),
                //        ActivityPickingCustomerContactName = Convert.ToString(row["ActivityPickingCustomerContactName"] != DBNull.Value ? row["ActivityPickingCustomerContactName"] : null),
                //        ActivityPickingCustomerShipToInfo = Convert.ToString(row["ActivityPickingCustomerShipToInfo"] != DBNull.Value ? row["ActivityPickingCustomerShipToInfo"] : null),
                //        RouteCode = Convert.ToString(row["RouteCode"] != DBNull.Value ? row["RouteCode"] : null),
                //        RouteDescription = Convert.ToString(row["RouteDescription"] != DBNull.Value ? row["RouteDescription"] : null),
                //        RouteDayOfWeek = Convert.ToString(row["RouteDayOfWeek"] != DBNull.Value ? row["RouteDayOfWeek"] : null),
                //        RouteStop = Convert.ToInt32(row["RouteStop"] != DBNull.Value ? row["RouteStop"] : null),
                //        ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                //        ActivityPickingIsReviewed = Convert.ToBoolean(row["ActivityPickingIsReviewed"] != DBNull.Value ? row["ActivityPickingIsReviewed"] : null),
                //    };
                observableCollection.Add(picking);
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
            if (selectedActivity is Picking header)
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
            if (selectedItem is Picking picking)
            {
                this.HdrFKRefno = Convert.ToInt32(picking.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(picking.ActivityRefno);
            }

            return this.HdrRefno;
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
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);
                await this.Purge();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                var tableActPicking = new TableActivityPicking();

                tableActPicking.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                await tableActPicking.RequestData(data);
                tableActPicking.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            var tableActPicking = new TableActivityPicking();
            var tableActPickingRoute = new TableActivityPickingRoute();
            var tableActPickingItem = new TableActivityPickingItem();
            var tableActPickingDetail = new TableActivityPickingDetail();

            await tableActPickingDetail.PurgeData();
            await tableActPickingItem.PurgeData();
            await tableActPickingRoute.PurgeData();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
            var filter = string.Empty;
            var pickingSettings = new PickingSettings().LoadData();

            if (pickingSettings.IsApplyRouteCodeFilter)
            {
                if (!string.IsNullOrEmpty(pickingSettings.strRouteCodeSelectedValue))
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter += string.Format("RouteCode = \'{0}\'", pickingSettings.strRouteCodeSelectedValue);
                    }
                    else
                    {
                        filter += string.Format(" AND RouteCode = \'{0}\'", pickingSettings.strRouteCodeSelectedValue);
                    }
                }
            }

            if (pickingSettings.IsApplyOrderedDateFilter)
            {
                if (!SysCommon.IsEmptyDate(pickingSettings.dtOrderedDateFilter))
                {
                    var dt = pickingSettings.dtOrderedDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter = filter + string.Format(
                            " AND ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            if (pickingSettings.IsApplyDeliveryDateFilter)
            {
                if (!SysCommon.IsEmptyDate(pickingSettings.dtDeliveryDateFilter))
                {
                    var dt = pickingSettings.dtDeliveryDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter = filter + string.Format(
                            " AND ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
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
            var pickingSettings = new PickingSettings().LoadData();

            if (pickingSettings.IsApplyRouteCodeFilter)
            {
                if (string.Compare(data.Root.Element("route_code").Value, string.Empty) > 0)
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = "RouteCode = \'" + data.Root.Element("route_code").Value + "\'";
                    }
                    else
                    {
                        filter = filter + " AND RouteCode = \'" + data.Root.Element("route_code").Value + "\'";
                    }
                }
            }

            if (pickingSettings.IsApplyOrderedDateFilter)
            {
                if (string.Compare(data.Root.Element("ordered_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("ordered_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            if (pickingSettings.IsApplyDeliveryDateFilter)
            {
                if (string.Compare(data.Root.Element("delivery_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("delivery_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
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
            throw new NotImplementedException();
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
    }
}
