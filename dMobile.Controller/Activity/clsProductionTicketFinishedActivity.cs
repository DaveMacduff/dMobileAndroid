namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;

  public class TicketFinishedActivity : Activity
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    public TicketFinishedActivity()
      : base()
    {
      this.FullClassName = typeof(TicketFinishedActivity).FullName;
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
      return "ActivityProductionStringFKRefno";
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetControlDescription()
    {
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.TicketsFinished);
#else
      return "Tickets Finished";
#endif
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
//      dataGridCol.MappingName = "ActivityProductionStringFKRefno";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Ticket);
//#else
//      dataGridCol.HeaderText = "Ticket #";
//#endif
//      dataGridCol.Width = 68 * colWidthFactor;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityProductionDateTime";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
//#else
//      dataGridCol.HeaderText = "Date";
//#endif
//      dataGridCol.Width = 65 * colWidthFactor;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityProductionDescription";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Description);
//#else
//      dataGridCol.HeaderText = "Description";
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
    /// <param name="IsSubmitBatch"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        sql.Append("SELECT tActivityProduction.ActivityRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionFKRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityProduction.ActivityProductionDateTime,");
        sql.Append("tActivityProduction.ActivityProductionDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityProduction ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
        sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
        sql.Append("= tActivityProduction.ActivityRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", (int)SysCommon.ActSubTypes.TicketFinished);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityProduction.ActivityRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionFKRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityProduction.ActivityProductionDateTime,");
        sql.Append("tActivityProduction.ActivityProductionDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityProduction ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
        sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
        sql.Append("= tActivityProduction.ActivityRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat("AND tActivity.ActivitySubType = {0} ", (int)SysCommon.ActSubTypes.TicketFinished);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityProduction.ActivityRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionFKRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityProduction.ActivityProductionDateTime,");
        sql.Append("tActivityProduction.ActivityProductionDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityProduction ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
        sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
        sql.Append("= tActivityProduction.ActivityRefno ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
        sql.Append("= tActivityItem.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat("AND tActivity.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.TicketFinished);
      }
      else
      {
        sql.Append("SELECT tActivityProduction.ActivityRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionFKRefno,");
        sql.Append("tActivityProductionFinished.ActivityProductionStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityProduction.ActivityProductionDateTime,");
        sql.Append("tActivityProduction.ActivityProductionDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityProduction ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityProduction.WarehouseLocationMapRefno ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityProduction.ActivityRefno ");
        sql.Append("INNER JOIN tActivityProductionFinished ON tActivityProductionFinished.ActivityRefno ");
        sql.Append("= tActivityProduction.ActivityRefno ");
        sql.AppendFormat("WHERE tActivity.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.TicketFinished);
      }

      using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet ds = new DataSet())
        {
          da.Fill(ds);

          this.DataViewHdrs = ds.Tables[0].DefaultView;
          this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
          this.DataViewHdrs.Sort = ticketSettings.GetDataViewSortBy();
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
        this.DataViewHdrs.RowFilter = "ActivityProductionStringFKRefno LIKE \'" + searchText.Trim() + "%\'";
      }
      else
      {
        this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityProductionStringFKRefno LIKE \'" 
            + searchText.Trim() + "%\'";
      }

      return this.DataViewHdrs;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="i"></param>
    /// <param name="lstDisplay"></param>
    /// <remarks></remarks>
    public override void SelectHdr(int idx)
    {
      this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityProductionFKRefno"];
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
      this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityProductionFKRefno"];
      this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];

      return this.Refno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlParam"></param>
    public override void Refresh(XDocument xmlParam)
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

        TableActivityProductionTicketFinished tableFinished = new TableActivityProductionTicketFinished();

        tableFinished.RegisterDataAccessListenerProgressOut((TicketFinishedActivity)this);
        tableFinished.Request();
        tableFinished.UnregisterDataAccessListenerProgressOut((TicketFinishedActivity)this);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
      TableActivityProductionTicketFinished tableFinished = new TableActivityProductionTicketFinished();
      TableActivityProductionTicketFinishedItem tableFinishedItem = new TableActivityProductionTicketFinishedItem();
      TableActivityProductionTicketFinishedDetail tableFinishedDetail 
          = new TableActivityProductionTicketFinishedDetail();

      tableFinishedDetail.Purge();
      tableFinishedItem.Purge();
      tableFinished.Purge();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetFilter()
    {
      string filter = string.Empty;
      ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();

      if (ticketSettings.IsApplyProductionDateFilter)
      {
        if (!SysCommon.IsEmptyDateTime(ticketSettings.ProductionDateFilter))
        {
          DateTime dt = ticketSettings.ProductionDateFilter;

          // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
          if (string.IsNullOrEmpty(filter))
          {
            filter = string.Format(
                "ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                dt.Year, 
                dt.Month, 
                dt.Day);
          }
          else
          {
            filter = filter + string.Format(
                " AND ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
      ProductionTicketSettings ticketSettings = new ProductionTicketSettings().LoadData();

      if (ticketSettings.IsApplyProductionDateFilter)
      {
        if (string.Compare(data.Root.Element("production_date").Value, string.Empty) > 0)
        {
          DateTime dt = DateTime.Parse(data.Root.Element("production_date").Value);

          // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
          if (string.IsNullOrEmpty(filter))
          {
            filter = string.Format(
                "ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                dt.Year, 
                dt.Month, 
                dt.Day);
          }
          else
          {
            filter = filter + string.Format(
                " AND ActivityProductionDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityProductionDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
