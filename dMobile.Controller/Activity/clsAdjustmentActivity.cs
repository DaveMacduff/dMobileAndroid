namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;
    using System.Xml;

  /// <summary>
  /// 
  /// </summary>
  public class AdjActivity : Activity
  {
    #region Fields

    private AdjustmentSettings adjSettings;

    #endregion

    #region Constructors

    public AdjActivity()
      : base()
    {
      this.FullClassName = typeof(AdjActivity).FullName;
      this.AdjSettings = new AdjustmentSettings().LoadData();
    }

    #endregion

    #region Properties

    public AdjustmentSettings AdjSettings
    {
      get
      {
        return this.adjSettings;
      }

      set
      {
        this.adjSettings = value;
      }
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
      TableActivityAdjustment adj = new TableActivityAdjustment();
      TableActivityAdjustmentItem adjItem = new TableActivityAdjustmentItem();
      TableActivityAdjustmentDetail adjDetail = new TableActivityAdjustmentDetail();

      adjDetail.Purge();
      adjItem.Purge();
      adj.Purge();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetFKRefnoIdx()
    {
      return "ActivityAdjustmentStringFKRefno";
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetControlDescription()
    {
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.Adjustmentsz);
#else
      return "Adjustments";
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
//      dataGridCol.MappingName = "ActivityAdjustmentStringFKRefno";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Adj);
//#else
//      dataGridCol.HeaderText = "Adj #";
//#endif
//      dataGridCol.Width = 68 * colWidthFactor;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityAdjustmentDateTime";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Date);
//#else
//      dataGridCol.HeaderText = "Date";
//#endif
//      dataGridCol.Width = 65 * colWidthFactor;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityAdjustmentDescription";
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
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        sql.Append("SELECT tActivityAdjustment.ActivityRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
        sql.Append("tActivity.ActivityStatus");
        sql.Append(" FROM tActivityAdjustment");
        sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
        sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
        sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
        sql.Append(" UNION ");
        sql.Append(" SELECT tActivityAdjustment.ActivityRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
        sql.Append("tActivity.ActivityStatus");
        sql.Append(" FROM tActivityAdjustment");
        sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
        sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
        sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
        sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
        sql.Append(" UNION ");
        sql.Append(" SELECT tActivityAdjustment.ActivityRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
        sql.Append("tActivity.ActivityStatus");
        sql.Append(" FROM tActivityAdjustment");
        sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
        sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
        sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
        sql.Append(" INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno");
        sql.AppendFormat(" WHERE tActivityDetail.ActivityDetailStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
      }
      else
      {
        sql.Append("SELECT tActivityAdjustment.ActivityRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentFKRefno,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentStringFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
        sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
        sql.Append("tActivity.ActivityStatus");
        sql.Append(" FROM tActivityAdjustment");
        sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
        sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
      }

      using (SQLiteDataAdapter dataAdapterAdj = new SQLiteDataAdapter(
          sql.ToString(),
          DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet dataSetAdj = new DataSet())
        {
          dataAdapterAdj.Fill(dataSetAdj);

          this.DataViewHdrs = dataSetAdj.Tables[0].DefaultView;
          this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
          this.DataViewHdrs.Sort = adjSettings.GetDataViewSortBy();
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
        this.DataViewHdrs.RowFilter = "ActivityAdjustmentStringFKRefno LIKE \'" + searchText.Trim() + "%\'";
      }
      else
      {
        this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityAdjustmentStringFKRefno LIKE \'" 
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
      this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityAdjustmentFKRefno"];
      this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
      this.SelectedIndex = idx;

      // Set the selected warehouse to the activity header warehouse.
      if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != (int)this.DataViewHdrs[idx]["LocationMapRefno"])
      {
        LocationMap.GetInstance().CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(
            (int)this.DataViewHdrs[idx]["LocationMapRefno"]);
      }

      this.AdjSettings = this.AdjSettings.LoadData();
      this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)this.DataViewHdrs[idx]["ActivityAdjustmentType"];

      if (!SysCommon.IsAllowedChangeNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType))
      {
        this.AdjSettings.NegateScanQuantity = SysCommon.DefaultNegateQuantityFromAdjType(
            this.AdjSettings.AdjustmentType);
        AdjustmentSettings tempAdjSettings = this.AdjSettings;
        this.AdjSettings.SaveData(ref tempAdjSettings);
        this.AdjSettings = tempAdjSettings;
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
      this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityAdjustmentFKRefno"];
      this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];

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

        TableActivityAdjustment adj = new TableActivityAdjustment();
        adj.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
        adj.Request();
        adj.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetFilter()
    {
      string filter = string.Empty;
      AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();

      if (adjSettings.IsApplyAdjustmentType)
      {
        if (string.IsNullOrEmpty(filter))
        {
          filter = filter + string.Format("ActivityAdjustmentType = {0}", (int)adjSettings.AdjustmentType);
        }
        else
        {
          filter = filter + string.Format(" AND ActivityAdjustmentType = {0}", (int)adjSettings.AdjustmentType);
        }
      }

      if (adjSettings.IsApplyAdjustmentDate)
      {
        if (!SysCommon.IsEmptyDate(adjSettings.AdjustmentDate))
        {
          DateTime dt = adjSettings.AdjustmentDate;

          // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
          if (string.IsNullOrEmpty(filter))
          {
            filter = string.Format(
                "ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                dt.Year, 
                dt.Month, 
                dt.Day);
          }
          else
          {
            filter = filter + string.Format(
                " AND ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
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
    public override string GetFilter(XDocument xmlDoc)
    {
      string filter = string.Empty;
      AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();

      if (adjSettings.IsApplyAdjustmentType)
      {
        if (string.Compare(xmlDoc.Root.Element("adjustment_type").Value, string.Empty) > 0)
        {
          if (string.IsNullOrEmpty(filter))
          {
            filter = (string)("ActivityAdjustmentType = " + xmlDoc.Root.Element("adjustment_type").Value);
          }
          else
          {
            filter = filter + " AND ActivityAdjustmentType = " + xmlDoc.Root.Element("adjustment_type").Value;
          }
        }
      }

      if (adjSettings.IsApplyAdjustmentDate)
      {
        if (string.Compare(xmlDoc.Root.Element("adjustment_date").Value, string.Empty) > 0)
        {
          DateTime dt = DateTime.Parse(xmlDoc.Root.Element("adjustment_date").Value);

          // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
          if (string.IsNullOrEmpty(filter))
          {
            filter = string.Format(
                "ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                dt.Year, 
                dt.Month, 
                dt.Day);
          }
          else
          {
            filter = filter + string.Format(
                " AND ActivityAdjustmentDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                + "AND ActivityAdjustmentDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                dt.Year, 
                dt.Month, 
                dt.Day);
          }
        }
      }

      this.ActFilter = filter;

      return filter;
    }

    public override void PrintLabel(XDocument data)
    {
        if (AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox &&
            !SysEnv.SysSettings.IsDisconnectedMode)
        {
            int locMapRefno = 0;
            SLocMap locMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(
                int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value),
                int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value),
                0,
                ref locMapRefno);
            string reqData = string.Empty;
            PrinterSettings prnSettings = new PrinterSettings().LoadData();

            // Don't even bother sending in a print request if no quantity was entered.
            int labelQuantity = int.Parse(data.Root.Value);

            // Used abs for handling the freaky event that a negative label quantity was requested.
            if (labelQuantity > 0)
            {
                string request =
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRINT DEBOX PRODUCT LABEL</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + this.FKRefno + "</refno>")
                + ("<whs_id>" + locMap.WhsFKRefno + "</whs_id>")
                + ("<sect_refno>" + locMap.SectFKRefno + "</sect_refno>")
                + ("<label_qty>" + labelQuantity + "</label_qty>")
                + ("<mobile_printer_name>"
                + (prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty)
                + "</mobile_printer_name>")
                + "</imclient>";


                reqData = SysCommon.SubmitComm(request);
                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        throw new MessageOnlyException(
                            string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.OrderPrintShippingLabelsFailed) + Environment.NewLine + "{1}",
#else
"Order# {0} print shipping label(s) failed." + Environment.NewLine + "{1}",
#endif
 this.FKRefno,
                            docError.DocumentElement["detail"].InnerText));
                    }
                }
                else
                {
#if MULTILANGUAGE
          throw new Exception(string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.OrderPrintShippingLabelsFailedz), this.FKRefno));
#else
                    throw new Exception(string.Format("Order# {0} Print shipping label(s) failed.", this.FKRefno));
#endif
                }
            }
        }
    }

    public override bool ChangeStatus(int selectedIndex)
    {
        bool result = false;
        if (AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox &&
            !SysEnv.SysSettings.IsDisconnectedMode)
        {
            // do xml request to debox
            string xml =
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                  + "<imclient>"
                  + "<command>"
                  + "<action>COMPLETE DEBOX ADJUSTMENT</action>"
                  + "</command>"
                  + SysCommon.GetRequestDefaults(false)
                  + ("<refno>" + this.FKRefno + "</refno>")
                  + "</imclient>";

            string reqData = SysCommon.SubmitComm(xml);
            if (reqData.Length > 0)
            {
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument docError = new XmlDocument();
                    docError.LoadXml(reqData);
                    string errorDetail = docError.DocumentElement["detail"].InnerText;
                    throw new Exception(
                        string.Format("Debox failed. {0}",
                        Environment.NewLine + errorDetail));
                }
            }
            else
            {
                throw new Exception("Debox failed. No COM service response.");
            }

            result = true;
        }

        return result;
    }

    #endregion
  }
}
