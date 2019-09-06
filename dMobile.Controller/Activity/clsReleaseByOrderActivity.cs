namespace dMobile.Controller
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;

  public class ReleaseByOrderActivity : Activity
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    public ReleaseByOrderActivity()
      : base()
    {
      this.FullClassName = typeof(ReleaseByOrderActivity).FullName;
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public override bool ChangeStatus(int idx)
    {
      bool isResult = false;

      if (this.SysEnv.SysSettings.IsRelByOrder && !this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        if (MessageBox.Show(
#if MULTILANGUAGE
            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ReleaseOrderToInvoiceStatus), this.FKRefno), 
            Translator.GetInstance().Translate(TranslatorVocabulary.ReleaseOrderz), 
#else
            string.Format("Release Order# {0} To Invoice Status?", this.FKRefno), 
            "Release Order", 
#endif
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Question, 
            MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        {

          GC.Collect();
          GC.WaitForPendingFinalizers();

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
              throw new Exception(
#if MULTILANGUAGE
                  string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ReleaseOrderToInvoiceStatusFailed) + Environment.NewLine + errorDetail, 
#else
                  string.Format("Release order # {0} to invoice status failed." + Environment.NewLine + errorDetail, 
#endif
                  this.FKRefno, 
                  null));
            }
          }
          else
          {
#if MULTILANGUAGE
            throw new Exception(string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ReleaseOrderToInvoiceStatusFailed), this.FKRefno));
#else
            throw new Exception(string.Format("Release order # {0} to invoice status failed.", this.FKRefno));
#endif
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
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.ReleaseOrderz);
#else
      return "Release Order";
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

      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityPickingStringFKRefno";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Order);
#else
      dataGridCol.HeaderText = "Order #";
#endif
      dataGridCol.Width = 50 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityPickingOrderedDateTime";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.OrderDate);
#else
      dataGridCol.HeaderText = "Order Date";
#endif
      dataGridCol.Width = 70 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityPickingDeliveryDateTime";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.DelDate);
#else
      dataGridCol.HeaderText = "Del Date";
#endif
      dataGridCol.Width = 65 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "RouteCode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Route);
#else
      dataGridCol.HeaderText = "Route";
#endif
      dataGridCol.Width = 45 * colWidthFactor;
      dataGridCol.NullText = string.Empty;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "RouteStop";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Stop);
#else
      dataGridCol.HeaderText = "Stop";
#endif
      dataGridCol.Width = 45 * colWidthFactor;
      dataGridCol.NullText = string.Empty;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridMain.TableStyles.Add(dataGridStyle);
      dataGridMain.RowHeadersVisible = false;
      dataGridMain.ColumnHeadersVisible = true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="IsSubmitBatch"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        sql.Append("SELECT tActivityPicking.ActivityRefno,");
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
        sql.Append(" LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno ");
        sql.Append("= tActivityPicking.ActivityRefno");
        sql.Append(" LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno");
        sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat(" AND tActivity.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.Picking);
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
        sql.AppendFormat(" WHERE tActivity.ActivityStatus = {0}", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat(" AND tActivityItem.ActivityItemStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat(" AND tActivity.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.Picking);
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
        sql.AppendFormat(" WHERE tActivity.ActivityStatus = {0}", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat(" AND tActivityItem.ActivityItemStatus = {0}", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat(" AND tActivityDetail.ActivityDetailStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat(" AND tActivity.ActivitySubType = {0}", (int)SysCommon.ActSubTypes.Picking);
      }
      else
      {
        sql.Append("SELECT tActivityPicking.ActivityRefno,");
        sql.Append(" tActivityPicking.ActivityPickingFKRefno,");
        sql.Append(" tActivityPicking.ActivityPickingStringFKRefno,");
        sql.Append(" tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
        sql.Append(" tLocationMapWarehouse.LocationMapWarehouseName,");
        sql.Append(" tActivityPicking.ActivityPickingOrderedDateTime,");
        sql.Append(" tActivityPicking.ActivityPickingDeliveryDateTime,");
        sql.Append(" tActivityPicking.ActivityPickingCustomerContactName,");
        sql.Append(" tActivityPicking.ActivityPickingCustomerShipToInfo,");
        sql.Append(" tRoute.RouteCode,");
        sql.Append(" tRoute.RouteDescription,");
        sql.Append(" tRoute.RouteDayOfWeek,");
        sql.Append(" tActivityPickingRoute.RouteStop,");
        sql.Append(" tActivity.ActivityStatus");
        sql.Append(" FROM tActivityPicking");
        sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno");
        sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityPicking.WarehouseLocationMapRefno");
        sql.Append(" LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno ");
        sql.Append("= tActivityPicking.ActivityRefno");
        sql.Append(" LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno");
      }

      using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet ds = new DataSet())
        {
          da.Fill(ds);

          this.DataViewHdrs = ds.Tables[0].DefaultView;
          this.DataViewHdrs.Table.TableName = MainGridMappingName;
          PickingSettings pickingSettings = new PickingSettings().LoadData();
          this.DataViewHdrs.Sort = pickingSettings.GetDataViewSortBy();
          this.DataViewHdrs.RowFilter = this.GetFilter();
        }
      }

      return DataViewHdrs;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView Search(string searchText)
    {
      if (string.IsNullOrEmpty(ActFilter))
      {
        this.DataViewHdrs.RowFilter = "ActivityPickingStringFKRefno LIKE \'" + searchText.Trim() + "%\'";
      }
      else
      {
        this.DataViewHdrs.RowFilter = ActFilter + " AND ActivityPickingStringFKRefno LIKE \'" + searchText.Trim()
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
      this.FKRefno = (int)DataViewHdrs[idx]["ActivityPickingFKRefno"];
      this.Refno = (int)DataViewHdrs[idx]["ActivityRefno"];
      this.SelectedIndex = idx;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override int GetRefnoFromRowIdx(int idx)
    {
      this.FKRefno = (int)DataViewHdrs[idx]["ActivityPickingFKRefno"];
      this.Refno = (int)DataViewHdrs[idx]["ActivityRefno"];

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
    public override void Purge()
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
            filter = filter + string.Format("RouteCode = \'{0}\'", pickingSettings.strRouteCodeSelectedValue);
          }
          else
          {
            filter = filter + string.Format(" AND RouteCode = \'{0}\'", pickingSettings.strRouteCodeSelectedValue);
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

    #endregion
  }
}
