namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;

    /// <summary>
    /// 
    /// </summary>
    public class PhysicalActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public PhysicalActivity()
            : base()
        {
            this.FullClassName = typeof(PhysicalActivity).FullName;
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
            return "ActivityCountStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.PhysicalCounts);
#else
            return "Physical Counts";
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void DesignDataGrid(DataGrid dataGridMain)
        {
            int colWidthFactor = 1;

            if (this.SysEnv.IsWindowsMobile6Device)
            {
                colWidthFactor = 2;
            }

            if (dataGridMain.TableStyles.Count > 0)
            {
                dataGridMain.TableStyles.Clear();
            }

            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
            dataGridStyle.MappingName = this.MainGridMappingName;

            DataGridTextBoxColumn dataGridCol1 = new DataGridTextBoxColumn();
            dataGridCol1.MappingName = "ActivityCountStringFKRefno";
#if MULTILANGUAGE
      dataGridCol1.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Countz);
#else
            dataGridCol1.HeaderText = "Count#";
#endif
            dataGridCol1.Width = 50 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol1);

            DataGridTextBoxColumn dataGridCol2 = new DataGridTextBoxColumn();
            dataGridCol2.MappingName = "ActivityCountDateTime";
#if MULTILANGUAGE
      dataGridCol2.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
#else
            dataGridCol2.HeaderText = "Date";
#endif
            dataGridCol2.Width = 58 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol2);

            DataGridTextBoxColumn dataGridCol3 = new DataGridTextBoxColumn();
            dataGridCol3.MappingName = "ActivityCountTypeDescription";
#if MULTILANGUAGE
      dataGridCol3.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.CountType);
#else
            dataGridCol3.HeaderText = "Count Type";
#endif
            dataGridCol3.Width = 118 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol3);

            dataGridMain.TableStyles.Add(dataGridStyle);
            dataGridMain.RowHeadersVisible = false;
            dataGridMain.ColumnHeadersVisible = true;
        }

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
                sql.Append("SELECT tActivityCount.ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivity.ActivityStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityCount.ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityCount.ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItem.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivity.ActivitySubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
            }
            else
            {
                sql.Append("SELECT tActivityCount.ActivityRefno,");
                sql.Append("tActivityCount.ActivityCountFKRefno,");
                sql.Append("tActivityCount.ActivityCountStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityCount.ActivityCountTypeDescription,");
                sql.Append("tActivityCount.ActivityCountDescription,");
                sql.Append("tActivityCount.ActivityCountDateTime,");
                sql.Append("tActivity.ActivityStatus ");
                sql.Append("FROM tActivityCount ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityCount.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityCount.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityCount.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.PhysicalCounts);
            }

            using (SQLiteDataAdapter dataAdapterPhysical = new SQLiteDataAdapter(
                sql.ToString(),
                DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetPhysical = new DataSet())
                {
                    dataAdapterPhysical.Fill(dataSetPhysical);

                    this.DataViewHdrs = dataSetPhysical.Tables[0].DefaultView;
                    this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                    PhysicalCountSettings countSettings = new PhysicalCountSettings().LoadData();
                    this.DataViewHdrs.Sort = countSettings.GetDataViewSortBy();
                    this.DataViewHdrs.RowFilter = this.GetFilter();
                }
            }

            return this.DataViewHdrs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView Search(string searchText)
        {
            if (string.IsNullOrEmpty(this.ActFilter))
            {
                this.DataViewHdrs.RowFilter = "ActivityCountStringFKRefno LIKE \'" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityCountStringFKRefno LIKE \'"
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
            this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityCountFKRefno"];
            this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
            this.SelectedIndex = idx;

            // Set the selected warehouse to the activity header warehouse.
            if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != (int)this.DataViewHdrs[idx]["LocationMapRefno"])
            {
                LocationMap.GetInstance().CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(
                    (int)this.DataViewHdrs[idx]["LocationMapRefno"]);
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
            this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityCountFKRefno"];
            this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];

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
                TableActivityPhysicalCount tableActPhysicalCount = new TableActivityPhysicalCount();

                tableActPhysicalCount.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                tableActPhysicalCount.Request();
                tableActPhysicalCount.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            TableActivityPhysicalCount tableActPhysicalCount = new TableActivityPhysicalCount();
            TableActivityPhysicalCountItem tableActPhysicalCountItem = new TableActivityPhysicalCountItem();
            TableActivityPhysicalCountDetail tableActPhysicalCountDetail = new TableActivityPhysicalCountDetail();

            tableActPhysicalCountDetail.Purge();
            tableActPhysicalCountItem.Purge();
            tableActPhysicalCount.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            string filter = string.Empty;
            PhysicalCountSettings countSettings = new PhysicalCountSettings().LoadData();

            if (countSettings.IsApplyCountDate)
            {
                if (!SysCommon.IsEmptyDate(countSettings.dtCountDate))
                {
                    DateTime dt = countSettings.dtCountDate;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter = filter + string.Format(
                            " AND ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + " AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
            PhysicalCountSettings countSettings = new PhysicalCountSettings().LoadData();

            if (countSettings.IsApplyCountDate)
            {
                if (string.Compare(data.Root.Element("count_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("count_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter = filter + string.Format(
                            " AND ActivityCountDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityCountDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
