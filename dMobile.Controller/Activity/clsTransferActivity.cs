namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;

  public class TransferActivity : Activity
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    public TransferActivity()
      : base()
    {
      this.FullClassName = typeof(TransferActivity).FullName;
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
      return "ActivityTransferStringFKRefno";
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetControlDescription()
    {
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.Transfersz);
#else
      return "Transfers";
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
//      dataGridCol.MappingName = "ActivityTransferStringFKRefno";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Transferz);
//#else
//      dataGridCol.HeaderText = "Transfer #";
//#endif
//      dataGridCol.Width = 68 * colWidthFactor;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "LocationMapWarehouseFKRefno";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Whs);
//#else
//      dataGridCol.HeaderText = "Whs";
//#endif
//      dataGridCol.Width = 35 * colWidthFactor;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityTransferDateTime";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
//#else
//      dataGridCol.HeaderText = "Date";
//#endif
//      dataGridCol.Width = 65 * colWidthFactor;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityTransferArrivalDateTime";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Arrived);
//#else
//      dataGridCol.HeaderText = "Arrived";
//#endif
//      dataGridCol.Width = 65 * colWidthFactor;
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
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        sql.Append("SELECT tActivityTransfer.ActivityRefno,");
        sql.Append("tActivityTransfer.ActivityTransferFKRefno,");
        sql.Append("tActivityTransfer.ActivityTransferStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityTransfer ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityTransfer.ActivityRefno,");
        sql.Append("tActivityTransfer.ActivityTransferFKRefno,");
        sql.Append("tActivityTransfer.ActivityTransferStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityTransfer ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityTransfer.ActivityRefno,");
        sql.Append("tActivityTransfer.ActivityTransferFKRefno,");
        sql.Append("tActivityTransfer.ActivityTransferStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityTransfer ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
        sql.Append("= tActivityItem.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
      }
      else
      {
        sql.Append("SELECT tActivityTransfer.ActivityRefno,");
        sql.Append("tActivityTransfer.ActivityTransferFKRefno,");
        sql.Append("tActivityTransfer.ActivityTransferStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityTransfer.ActivityTransferArrivalDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDateTime,");
        sql.Append("tActivityTransfer.ActivityTransferDescription,");
        sql.Append("tActivity.ActivityStatus ");
        sql.Append("FROM tActivityTransfer ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityTransfer.TransferToWarehouseLocationMapRefno ");
        sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityTransfer.ActivityRefno");
      }

      using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet ds = new DataSet())
        {
          da.Fill(ds);

          this.DataViewHdrs = ds.Tables[0].DefaultView;
          this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
          TransferSettings transferSettings = new TransferSettings().LoadData();
          this.DataViewHdrs.Sort = transferSettings.GetDataViewSortBy();
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
        this.DataViewHdrs.RowFilter = "ActivityTransferStringFKRefno LIKE \'" + searchText.Trim() + "%\'";
      }
      else
      {
        this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityTransferStringFKRefno LIKE \'" + searchText.Trim() 
            + "%\'";
      }

      return this.DataViewHdrs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    public override void SelectHdr(int idx)
    {
      this.FKRefno = (int)DataViewHdrs[idx]["ActivityTransferFKRefno"];
      this.Refno = (int)DataViewHdrs[idx]["ActivityRefno"];
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
    /// <param name="idx"></param>
    /// <returns></returns>
    public override int GetRefnoFromRowIdx(int idx)
    {
      this.FKRefno = (int)DataViewHdrs[idx]["ActivityTransferFKRefno"];
      this.Refno = (int)DataViewHdrs[idx]["ActivityRefno"];
      
      return this.Refno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
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

        TableActivityTransfer tableActTransfer = new TableActivityTransfer();

        tableActTransfer.RegisterDataAccessListenerProgressOut((TransferActivity)this);
        tableActTransfer.Request();
        tableActTransfer.UnregisterDataAccessListenerProgressOut((TransferActivity)this);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
      TableActivityTransfer tableActTransfer = new TableActivityTransfer();
      TableActivityTransferItem tableActTransferItem = new TableActivityTransferItem();
      TableActivityTransferDetail tableActTransferDetail = new TableActivityTransferDetail();

      tableActTransferDetail.Purge();
      tableActTransferItem.Purge();
      tableActTransfer.Purge();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetFilter()
    {
      string filter = string.Empty;
      TransferSettings transferSettings = new TransferSettings().LoadData();

      if (transferSettings.IsApplyTransferDateFilter)
      {
        if (!SysCommon.IsEmptyDate(transferSettings.TransferDateFilter))
        {
          DateTime dt = transferSettings.TransferDateFilter;

          // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
          if (string.IsNullOrEmpty(filter))
          {
            filter = string.Format(
                "ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                dt.Year, 
                dt.Month, 
                dt.Day);
          }
          else
          {
            filter = filter + string.Format(
                " AND ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
    /// <param name="data"></param>
    /// <returns></returns>
    public override string GetFilter(XDocument data)
    {
      string filter = string.Empty;
      TransferSettings transferSettings = new TransferSettings().LoadData();

      if (transferSettings.IsApplyTransferDateFilter)
      {
        if (string.Compare(data.Root.Element("transfer_date").Value, string.Empty) > 0)
        {
          DateTime dt = DateTime.Parse(data.Root.Element("transfer_date").Value);

          // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
          if (string.IsNullOrEmpty(filter))
          {
            filter = string.Format(
                "ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                dt.Year, 
                dt.Month, 
                dt.Day);
          }
          else
          {
            filter = filter + string.Format(
                " AND ActivityTransferDateTime >= '{0}-{1}-{2} 00:00:00.000' " 
                + "AND ActivityTransferDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
