namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class ReturnsActivity : Activity
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ReturnsActivity()
          : base()
        {
            this.FullClassName = typeof(ReturnsActivity).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                    }
                };

                Label labelActivityReturnsStringFKRefno = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityReturnsOrderedDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityReturnsDeliveryDateTime = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityReturnsStringFKRefno.SetBinding(Label.TextProperty, new Binding("ActivityReturnsStringFKRefno"));
                labelActivityReturnsOrderedDateTime.SetBinding(Label.TextProperty, new Binding("ActivityReturnsOrderedDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityReturnsDeliveryDateTime.SetBinding(Label.TextProperty, new Binding("ActivityReturnsDeliveryDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));

                gridData.Children.Add(labelActivityReturnsStringFKRefno);
                gridData.Children.Add(labelActivityReturnsOrderedDateTime);
                gridData.Children.Add(labelActivityReturnsDeliveryDateTime);

                Grid.SetColumn(labelActivityReturnsStringFKRefno, 0);
                Grid.SetColumn(labelActivityReturnsOrderedDateTime, 1);
                Grid.SetColumn(labelActivityReturnsDeliveryDateTime, 2);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityReturnsStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Returns";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Return returns = new Return
                {
                    ActivityReturnsFKRefno = Convert.ToInt32(row["ActivityReturnsFKRefno"] != DBNull.Value ? row["ActivityReturnsFKRefno"] : null),
                    ActivityReturnsStringFKRefno = Convert.ToString(row["ActivityReturnsStringFKRefno"] != DBNull.Value ? row["ActivityReturnsStringFKRefno"] : null),
                    ActivityReturnsOrderedDateTime = Convert.ToDateTime(row["ActivityReturnsOrderedDateTime"] != DBNull.Value ? row["ActivityReturnsOrderedDateTime"] : null),
                    ActivityReturnsDeliveryDateTime = Convert.ToDateTime(row["ActivityReturnsDeliveryDateTime"] != DBNull.Value ? row["ActivityReturnsDeliveryDateTime"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                    ActivityReturnsCustomerContactName = Convert.ToString(row["ActivityReturnsCustomerContactName"] != DBNull.Value ? row["ActivityReturnsCustomerContactName"] : null),
                    ActivityReturnsCustomerShipToInfo = Convert.ToString(row["ActivityReturnsCustomerShipToInfo"] != DBNull.Value ? row["ActivityReturnsCustomerShipToInfo"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                };

                observableCollection.Add(returns);
            }

            // TODO: Resource Strings
            Label labelReturnNumber = new Label { Text = "Return #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelReturnEntered = new Label { Text = "Entered", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelReturnReturned = new Label { Text = "Returned", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            
            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelReturnNumber.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelReturnEntered.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelReturnReturned.Text);

            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelReturnNumber);
            gridHeader.Children.Add(labelReturnEntered);
            gridHeader.Children.Add(labelReturnReturned);

            Grid.SetColumn(labelReturnNumber, 0);
            Grid.SetColumn(labelReturnEntered, 1);
            Grid.SetColumn(labelReturnReturned, 2);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);
            
            foreach (Return returns in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(returns.ActivityReturnsStringFKRefno);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(returns.ActivityReturnsOrderedDateTime.ToString("MM/dd/yyyy"));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(returns.ActivityReturnsDeliveryDateTime.ToString("MM/dd/yyyy"));

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
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <remarks></remarks>
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      int colWidthFactor = 1;

        //      if (this.SysEnv.IsWindowsMobile6Device)
        //      {
        //        colWidthFactor = 2;
        //      }

        //      if (dataGridMain.TableStyles.Count > 0)
        //      {
        //        dataGridMain.TableStyles.Clear();
        //      }

        //      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //      dataGridStyle.MappingName = this.MainGridMappingName;

        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityReturnsStringFKRefno";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Returnz);
        //#else
        //      dataGridCol.HeaderText = "Return #";
        //#endif
        //      dataGridCol.Width = 50 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityReturnsOrderedDateTime";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Entered);
        //#else
        //      dataGridCol.HeaderText = "Entered";
        //#endif
        //      dataGridCol.Width = 70 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityReturnsDeliveryDateTime";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Returned);
        //#else
        //      dataGridCol.HeaderText = "Returned";
        //#endif
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityReturnsCustomerShipToInfo";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ShipTo);
        //#else
        //      dataGridCol.HeaderText = "Ship To";
        //#endif
        //      dataGridCol.Width = 100 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridMain.TableStyles.Add(dataGridStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityReturns.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityReturns.ActivityReturnsFKRefno AS ActivityReturnsFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsStringFKRefno AS ActivityReturnsStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime AS ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime AS ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName AS ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo AS ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityReturns.ActivityRefno,");
                sql.Append("tActivityReturns.ActivityReturnsFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" UNION ");
                sql.Append(" SELECT tActivityReturns.ActivityRefno,");
                sql.Append("tActivityReturns.ActivityReturnsFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.Append(" INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno");
                sql.AppendFormat(" WHERE tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                sql.Append("SELECT tActivityReturns.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityReturns.ActivityReturnsFKRefno AS ActivityReturnsFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsStringFKRefno AS ActivityReturnsStringFKRefno,");
                sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime AS ActivityReturnsOrderedDateTime,");
                sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime AS ActivityReturnsDeliveryDateTime,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerContactName AS ActivityReturnsCustomerContactName,");
                sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo AS ActivityReturnsCustomerShipToInfo,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivityReturns");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityReturns.WarehouseLocationMapRefno");
                sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityReturns.ActivityRefno");
            }

            using (SQLiteDataAdapter dataAdapterReturns = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetReturns = new DataSet())
                {
                    dataAdapterReturns.Fill(dataSetReturns);

                    this.DataViewHdrs = dataSetReturns.Tables[0].DefaultView;
                    this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                    this.DataViewHdrs.Sort = new ReturnsSettings().LoadData().GetDataViewSortBy();
                }
            }

            return this.DataViewHdrs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActFilter))
            {
                this.DataViewHdrs.RowFilter = "ActivityReturnsStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityReturnsStringFKRefno LIKE \'%"
                    + searchText.Trim() + "%\'";
            }

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Return returns = new Return
                {
                    ActivityReturnsFKRefno = Convert.ToInt32(row["ActivityReturnsFKRefno"] != DBNull.Value ? row["ActivityReturnsFKRefno"] : null),
                    ActivityReturnsStringFKRefno = Convert.ToString(row["ActivityReturnsStringFKRefno"] != DBNull.Value ? row["ActivityReturnsStringFKRefno"] : null),
                    ActivityReturnsOrderedDateTime = Convert.ToDateTime(row["ActivityReturnsOrderedDateTime"] != DBNull.Value ? row["ActivityReturnsOrderedDateTime"] : null),
                    ActivityReturnsDeliveryDateTime = Convert.ToDateTime(row["ActivityReturnsDeliveryDateTime"] != DBNull.Value ? row["ActivityReturnsDeliveryDateTime"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                    ActivityReturnsCustomerContactName = Convert.ToString(row["ActivityReturnsCustomerContactName"] != DBNull.Value ? row["ActivityReturnsCustomerContactName"] : null),
                    ActivityReturnsCustomerShipToInfo = Convert.ToString(row["ActivityReturnsCustomerShipToInfo"] != DBNull.Value ? row["ActivityReturnsCustomerShipToInfo"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                };

                observableCollection.Add(returns);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityReturnsFKRefno"]);
            this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);
            this.SelectedIndex = idx;

            // Set the selected warehouse to the activity header warehouse.
            if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]))
            {
                LocationMap.GetInstance().CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityReturnsFKRefno"]);
            this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);
                this.Purge();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                TableActivityReturns tableActReturns = new TableActivityReturns();

                tableActReturns.RegisterDataAccessListenerProgressOut((ReturnsActivity)this);
                tableActReturns.Request();
                tableActReturns.UnregisterDataAccessListenerProgressOut((ReturnsActivity)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            TableActivityReturns tableActReturns = new TableActivityReturns();
            TableActivityReturnsItem tableActReturnsItem = new TableActivityReturnsItem();
            TableActivityReturnsDetail tableActReturnsDetail = new TableActivityReturnsDetail();

            tableActReturnsDetail.Purge();
            tableActReturnsItem.Purge();
            tableActReturns.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            return string.Empty;
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
            ReturnsSettings returnSettings = new ReturnsSettings().LoadData();

            if (returnSettings.IsApplyReturnedDateFilter)
            {
                if (string.Compare(data.Root.Element("returned_date").Value, string.Empty) > 0)
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = "ActivityReturnsDeliveryDateTime = \'" + DateTime.Parse(
                            data.Root.Element("returned_date").Value).ToString() + "\'";
                    }
                    else
                    {
                        filter = filter + " AND ActivityReturnsDeliveryDateTime = \'" + DateTime.Parse(
                            data.Root.Element("returned_date").Value).ToString() + "\'";
                    }
                }
            }

            if (returnSettings.IsApplyEnteredDateFilter)
            {
                if (string.Compare(data.Root.Element("entered_date").Value, string.Empty) > 0)
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = "ActivityReturnsOrderedDateTime = \'" + DateTime.Parse(
                            data.Root.Element("entered_date").Value).ToString() + "\'";
                    }
                    else
                    {
                        filter = filter + " AND ActivityReturnsOrderedDateTime = \'" + DateTime.Parse(
                            data.Root.Element("entered_date").Value).ToString() + "\'";
                    }
                }
            }

            this.ActFilter = filter;

            return filter;
        }

        #endregion
    }
}
