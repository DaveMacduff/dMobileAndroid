namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;

    public class ReturnsActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ReturnsActivity()
          : base()
        {
            this.FullClassName = typeof(ReturnsActivity).FullName;
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
                sql.Append("SELECT tActivityReturns.ActivityRefno,");
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
                sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat(" WHERE tActivityDetail.ActivityDetailStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
            }
            else
            {
                sql.Append("SELECT tActivityReturns.ActivityRefno,");
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
        public override DataView Search(string searchText)
        {
            if (string.IsNullOrEmpty(this.ActFilter))
            {
                this.DataViewHdrs.RowFilter = "ActivityReturnsStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityReturnsStringFKRefno LIKE \'%"
                    + searchText.Trim() + "%\'";
            }

            return this.DataViewHdrs;
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
