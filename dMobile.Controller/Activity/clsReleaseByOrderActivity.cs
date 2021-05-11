namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using Xamarin.Forms;
    using dMobile.Resources.Types;
    using dMobile.Resources.Services;
    using System.Collections.Generic;
    using System.Linq;

    public class ReleaseByOrderActivity : Activity
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ReleaseByOrderActivity()
          : base()
        {
            this.FullClassName = typeof(ReleaseByOrderActivity).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                    }
                };

                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityPickingStringFKRefno"));
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

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override async Task<bool> ChangeStatus(object selectedItem)
        {
            bool isResult = false;

            if (this.SysEnv.SysSettings.IsRelByOrder && !this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                //if (MessageBox.Show(string.Format("Release Order# {0} To Invoice Status?", this.FKRefno), "Release Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)

                // TODO: Resource Strings
                if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Release Order", string.Format("Release Order# {0} To Invoice Status?", this.FKRefno), "Yes", "No")))
                {
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();

                    string sendReq = string.Empty;
                    StringBuilder xmlString = new StringBuilder();
                    XmlWriter xml = XmlWriter.Create(xmlString);

                    XDocument request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>RELEASE SALES ORDER BY ORDER</action>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + ("<refno>" + this.FKRefno + "</refno>")
                        + "</imclient>");

                    request.Save(xml);
                    xml.Flush();
                    xml.Close();

                    string reqData = SysCommon.SubmitComm(xmlString.ToString());
                    
                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument docError = new XmlDocument();
                            docError.LoadXml(reqData);
                            string errorDetail = docError.DocumentElement["detail"].InnerText;
                            
                            // TODO: Resource strings
                            throw new Exception(string.Format("Release order # {0} to invoice status failed." + Environment.NewLine + errorDetail, this.FKRefno, null));
                        }
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception(string.Format("Release order # {0} to invoice status failed.", this.FKRefno));
                    }

                    isResult = true;
                }
            }

            return isResult;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityPickingStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Release Order";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (Picking picking in this.ListHdrs)
            {
                //Picking picking = new Picking
                //{
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityPickingFKRefno = Convert.ToInt32(row["ActivityPickingFKRefno"] != DBNull.Value ? row["ActivityPickingFKRefno"] : null),
                //    ActivityPickingStringFKRefno = Convert.ToString(row["ActivityPickingStringFKRefno"] != DBNull.Value ? row["ActivityPickingStringFKRefno"] : null),
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
                //};

                observableCollection.Add(picking);
            }

            // TODO: Resource strings
            Label labelOrderNumberHeader = new Label { Text = "Order #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelOrderDateHeader = new Label { Text = "Order Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelDeliveryDateHeader = new Label { Text = "Delivery Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelRouteHeader = new Label { Text = "Route", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelStopHeader = new Label { Text = "Stop", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrderNumberHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrderDateHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelDeliveryDateHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelRouteHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelStopHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                }
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

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (Picking picking in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingStringFKRefno);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingOrderedDateTime.ToString("MM/dd/yyyy"));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingDeliveryDateTime.ToString("MM/dd/yyyy"));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.RouteCode);
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(picking.RouteStop));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }
                
                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="IsSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityPicking.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityPicking.ActivityPickingFKRefno AS ActivityPickingFKRefno,");
                sql.Append("tActivityPicking.ActivityPickingStringFKRefno AS ActivityPickingStringFKRefno,");
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
                sql.Append("tActivityPicking.ActivityPickingFKRefno,");
                sql.Append("tActivityPicking.ActivityPickingStringFKRefno,");
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
                sql.Append("tActivityPicking.ActivityPickingFKRefno,");
                sql.Append("tActivityPicking.ActivityPickingStringFKRefno,");
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
                sql.Append("SELECT tActivityPicking.ActivityRefno AS ActivityRefno,");
                sql.Append(" tActivityPicking.ActivityPickingFKRefno AS ActivityPickingFKRefno,");
                sql.Append(" tActivityPicking.ActivityPickingStringFKRefno AS ActivityPickingStringFKRefno,");
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

            //using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);

            //        this.DataViewHdrs = ds.Tables[0].DefaultView;
            //        this.DataViewHdrs.Table.TableName = MainGridMappingName;
            //        TODO: Fix filtering and sorting
            //        PickingSettings pickingSettings = new PickingSettings().LoadData();
            //        this.DataViewHdrs.Sort = pickingSettings.GetDataViewSortBy();
            //        this.DataViewHdrs.RowFilter = this.GetFilter();
            //    }
            //}

            this.ListHdrs = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<object>(sql.ToString());
            return this.ListHdrs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(ActFilter))
            {
                //this.DataViewHdrs.RowFilter = "ActivityPickingStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
                this.ListHdrs = (List<object>)this.ListHdrs.OfType<Picking>().Where(picking => picking.ActivityPickingStringFKRefno.Contains(searchText.Trim()));
            }
            else
            {
                // TODO: Fix filtering
                //this.DataViewHdrs.RowFilter = ActFilter + " AND ActivityPickingStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }

            foreach (Picking picking in this.ListHdrs)
            {
                //    Picking picking = new Picking
                //    {
                //        ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //        ActivityPickingFKRefno = Convert.ToInt32(row["ActivityPickingFKRefno"] != DBNull.Value ? row["ActivityPickingFKRefno"] : null),
                //        ActivityPickingStringFKRefno = Convert.ToString(row["ActivityPickingStringFKRefno"] != DBNull.Value ? row["ActivityPickingStringFKRefno"] : null),
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
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Picking picking)
            {
                this.SelectedActivity = picking;
                this.FKRefno = Convert.ToInt32(picking.ActivityPickingFKRefno);
                this.Refno = Convert.ToInt32(picking.ActivityRefno);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            if (selectedItem is Picking picking)
            {
                this.FKRefno = Convert.ToInt32(picking.ActivityPickingFKRefno);
                this.Refno = Convert.ToInt32(picking.ActivityRefno);
            }

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
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

                TableActivityPicking tableActPicking = new TableActivityPicking();

                tableActPicking.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                tableActPicking.RequestData(data);
                tableActPicking.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            TableActivityPicking tableActPicking = new TableActivityPicking();
            TableActivityPickingRoute tableActPickingRoute = new TableActivityPickingRoute();
            TableActivityPickingItem tableActPickingItem = new TableActivityPickingItem();
            TableActivityPickingDetail tableActPickingDetail = new TableActivityPickingDetail();

            tableActPickingDetail.Purge();
            tableActPickingItem.Purge();
            tableActPickingRoute.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            string filter = string.Empty;
            PickingSettings pickingSettings = new PickingSettings().LoadData();

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
                    DateTime dt = pickingSettings.dtOrderedDateFilter;

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
                    DateTime dt = pickingSettings.dtDeliveryDateFilter;

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

            this.ActFilter = filter;

            return filter;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            string filter = string.Empty;
            PickingSettings pickingSettings = new PickingSettings().LoadData();

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
                    DateTime dt = DateTime.Parse(data.Root.Element("ordered_date").Value);

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
                    DateTime dt = DateTime.Parse(data.Root.Element("delivery_date").Value);

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

            this.ActFilter = filter;

            return filter;
        }

        #endregion
    }
}
