namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;

  public class RecvActItem : ActItem
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activity"></param>
    public RecvActItem(Activity activity)
      : base(activity)
    {
      this.FullClassName = typeof(RecvActItem).FullName;
      this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Receiving;
      this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsReceivingCreateNewItemNoAsk;
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override bool IsCatchAsk()
    {
      bool isCatchAsk = base.IsCatchAsk();
      if (isCatchAsk)
      {
          // modules are for exclusions
          isCatchAsk = this.SysEnv.SysSettings.IsReceivingCatchAskEnabled;
      }

      return isCatchAsk;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool IsCatchAskZero()
    {
      bool isCatchAskZero = base.IsCatchAskZero();
      if (isCatchAskZero)
      {
        isCatchAskZero = !this.SysEnv.SysSettings.IsReceivingNoCatchAskZeroEnabled;
      }

      return isCatchAskZero;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Delete()
    {
      if (!this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        string sendReq = this.CreateDeleteXML(XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
            + "<params>"
            + "<skid_data>" + this.ActHdr.SkidInfo.CurrSkid.SkidCode + "</skid_data>"
            + "</params>"));

        string reqData = SysCommon.SubmitComm(sendReq);
        if (reqData.Length > 0)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument docError = new XmlDocument();
            docError.LoadXml(reqData);
            string errorDetail = docError.DocumentElement["detail"].InnerText;
            if (errorDetail.Contains("MSGBOX-"))
            {
              throw new MessageOnlyException(errorDetail);
            }
            else
            {
              throw new Exception(errorDetail);
            }
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateDeleteXML(XDocument data)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      XDocument request = null;

      if (data.Document.Root.Element("skid_data") == null)
      {
        request = XDocument.Parse( 
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
            + "<imclient>" 
            + "<command>" 
            + "<action>DELETE PURCHASE ORDER DETAIL SKID</action>" 
            + "</command>" 
            + SysCommon.GetRequestDefaults(false)
            + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>" 
            + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
            + "<refno>" + this.ActHdr.FKRefno + "</refno>"
            + "<itm_refno>" + this.FKRefno + "</itm_refno>" 
            + "<is_negate_qty>false</is_negate_qty>" 
            + "</imclient>");
      }
      else
      {
#if MULTILANGUAGE
        throw new NotImplementedException(Translator.GetInstance().Translate(TranslatorVocabulary.OnlySkidDeletionIsSupported));
#else
        throw new NotImplementedException("Only skid deletion is supported.");
#endif
      }

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSelectedIndex"></param>
    /// <returns></returns>
    public override string GetComment(int selectedIndex)
    {
      string cmmt = base.GetComment(selectedIndex);
      //using (SQLiteCommand cmndItemRecvCmnt = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      //{
      //  cmndItemRecvCmnt.CommandType = CommandType.TableDirect;
      //  cmndItemRecvCmnt.CommandText = "tActivityItemReceivingComment";
      //  cmndItemRecvCmnt.IndexName = "PK_ActivityItemReceivingCommentOnActivityItemRefno";

      //  using (SQLiteDataReader drItemRecvCmnt = cmndItemRecvCmnt.ExecuteReader())
      //  {
      //    if (drItemRecvCmnt.Seek(DbSeekOptions.FirstEqual, new object[] { selectedIndex }))
      //    {
      //      if (drItemRecvCmnt.Read())
      //      {
      //        cmmt = drItemRecvCmnt["ActivityItemReceivingCommentComment"].ToString();
      //      }
      //    }
      //  }
      //}

      return cmmt;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="whsLocMapRefno"></param>
    /// <param name="prodRefno"></param>
    /// <returns></returns>
    public override int GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
    {
      int locMapRefno = LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
          whsLocMapRefno,
          prodRefno,
          SysCommon.ProdLocMapTypes.Receiving);

      return locMapRefno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParsePurchaseOrderItem), actItemStringFKRefno);
#else
      return string.Format("Unable to parse purchase order item# {0}", actItemStringFKRefno);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindPurchaseOrderItem), actItemStringFKRefno);
#else
      return string.Format("Unable to find purchase order item# {0}", actItemStringFKRefno);
#endif
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetDataViewSortFKRefnoIdx()
    {
      return "ActivityItemReceivingStringFKRefno";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="askToInsertAsNewItem"></param>
    public override void Insert(XDocument data, bool askToInsertAsNewItem)
    {
      throw new NotImplementedException();
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

        TableActivityRecevingItem tableActRecvItem = new TableActivityRecevingItem();
        TableActivityReceivingDetail tableActRecvDetail = new TableActivityReceivingDetail();
        tableActRecvDetail.Purge();
        tableActRecvItem.Purge();

        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

        tableActRecvItem.RegisterDataAccessListenerProgressOut((RecvActItem)this);
        tableActRecvItem.Request(this.ActHdr.Refno);
        tableActRecvItem.UnregisterDataAccessListenerProgressOut((RecvActItem)this);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataGridMain"></param>
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
      dataGridCol.MappingName = "ActivityItemRefno";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = -1;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "IsActivityItemReceivingComment";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.C);
#else
      dataGridCol.HeaderText = "C";
#endif
      dataGridCol.Width = 12 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ProductCode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
#else
      dataGridCol.HeaderText = "Product ID";
#endif
      dataGridCol.Width = 65 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ProductUnitBC";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = 20 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "UnitCode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = 22 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityItemReceivingQuantityOrdered";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Ordered);
#else
      dataGridCol.HeaderText = "Ordered";
#endif
      dataGridCol.Width = 52 * colWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityItemReceivingQuantityReceived";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Received);
#else
      dataGridCol.HeaderText = "Received";
#endif
      dataGridCol.Width = 52 * colWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridMain.TableStyles.Add(dataGridStyle);
      dataGridMain.RowHeadersVisible = false;
      dataGridMain.ColumnHeadersVisible = true;
    }

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
        // Gather any newly added line items.
        // Also perform union into tActivityDetail to check of detail exists.
        sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityOrdered");
        sql.Append("- tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate ");
        sql.Append("AS ActivityItemReceivingQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityOrdered ");
        sql.Append("- tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceivedToDate ");
        sql.Append("AS ActivityItemReceivingBaseQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityOrdered ");
        sql.Append("- tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate ");
        sql.Append("AS ActivityItemReceivingWeightQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingVendorProductUnitCode,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingDescription,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate,");
        sql.Append("CASE WHEN tActivityItemReceivingComment.ActivityItemReceivingCommentComment IS NULL ");
        sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemReceivingComment,");
        sql.Append("tActivityItem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemReceiving ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
        sql.Append("tActivityItemReceiving.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemReceivingComment ON tActivityItemReceivingComment.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", ActHdr.Refno);
        sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
        sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityOrdered - ");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate AS ");
        sql.Append("ActivityItemReceivingQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityOrdered - ");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceivedToDate AS ");
        sql.Append("ActivityItemReceivingBaseQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityOrdered - ");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate AS ");
        sql.Append("ActivityItemReceivingWeightQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingVendorProductUnitCode,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingDescription,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate,");
        sql.Append("CASE WHEN tActivityItemReceivingComment.ActivityItemReceivingCommentComment IS NULL ");
        sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemReceivingComment,");
        sql.Append("tActivityItem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemReceiving ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
        sql.Append("tActivityItemReceiving.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemReceivingComment ON tActivityItemReceivingComment.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityItem.ActivityItemStatus = {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.Receiving);
        sql.Append("ORDER BY tActivityItemReceiving.ActivityItemRefno");
      }
      else
      {
        sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingStringFKRefno,");
       sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityOrdered - ");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate AS ");
        sql.Append("ActivityItemReceivingQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityOrdered - ");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceivedToDate AS ");
        sql.Append("ActivityItemReceivingBaseQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityOrdered - ");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate AS ");
        sql.Append("ActivityItemReceivingWeightQuantityOrdered,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceived,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingVendorProductUnitCode,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingDescription,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate,");
        sql.Append("CASE WHEN tActivityItemReceivingComment.ActivityItemReceivingCommentComment IS NULL THEN \' \' ");
        sql.Append("ELSE \'*\' END AS IsActivityItemReceivingComment ");
        sql.Append("FROM tActivityItemReceiving ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
        sql.Append("tActivityItemReceiving.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemReceivingComment ON tActivityItemReceivingComment.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.Append("ORDER BY tActivityItemReceiving.ActivityItemRefno");
      }

      using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet ds = new DataSet())
        {
          da.Fill(ds);
          this.DataViewItems = ds.Tables[0].DefaultView;
          this.DataViewItems.Table.TableName = this.MainGridMappingName;
          this.DataViewItems.AllowNew = true;
          this.DataViewItems.AllowEdit = true;
          this.DataViewItems.AllowDelete = true;
          this.DataViewItems.Sort = "ActivityItemRefno";
        }
      }

      return this.DataViewItems;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public override DataView Search(string searchText)
    {
      if (string.IsNullOrEmpty(this.ActItemFilter))
      {
        this.DataViewItems.RowFilter = "ProductCode LIKE \'" + searchText.Trim() + "%\'";
      }
      else
      {
        this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'" + searchText.Trim() + "%\'";
      }

      return this.DataViewItems;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    public override void SelectItem(int idx)
    {
      base.SelectItem(idx);
      this.FKRefno = (int)DataViewItems[idx]["ActivityItemReceivingFKRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productUnitRefno"></param>
    /// <returns></returns>
    public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
    {
      int foundActItemRefno = 0;

      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT tActivityItem.ActivityItemRefno,");
        sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno,");
        sql.Append("tActivityItemReceiving.ProductUnitRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("FROM tActivityItemReceiving ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
        sql.Append("tActivityItemReceiving.ActivityItemRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat(
            "AND tActivityItemReceiving.WarehouseLocationMapRefno = {0} ", 
            this.LocMap.CurrLocMap.WhsLocMapRefno);
        sql.AppendFormat("AND tActivityItemReceiving.ProductUnitRefno = {0}", prodUnitRefno);

        cmndItem.CommandType = CommandType.Text;
        cmndItem.CommandText = sql.ToString();

        using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
        {
          if (drItem.Read())
          {
            ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
            this.Refno = (int)drItem["ActivityItemRefno"];
            this.FKRefno = (int)drItem["ActivityItemReceivingFKRefno"];
            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)drItem["ProductUnitRefno"]);
            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            foundActItemRefno = this.Refno;
          }

          drItem.Close();
        }
      }

      return foundActItemRefno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateSubmitXML(XDocument data)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
      string prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
      bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
      string unitCode = data.Root.Element("UnitCode").Value;
      string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>SUBMIT RECEIVING ITEM</action>" 
          + "</command>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<prod_id>" + prodCode + "</prod_id>")
          + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>")
          + ("<symbology>" + symbologyPrefix + "</symbology>")
          + ("<gtin>" + prodUnitBarCodeCode + "</gtin>") + "<whs_id></whs_id>"
          + "<sect_refno></sect_refno>"
          + "<slot_refno></slot_refno>"
          + "<refno></refno>"
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      this.DataViewItems[idx]["ActivityItemReceivingQuantityReceived"] = quantity;
      this.DataViewItems[idx]["ActivityItemReceivingBaseQuantityReceived"] = baseQuantity;
      this.DataViewItems[idx]["ActivityItemReceivingWeightQuantityReceived"] = weightQuantity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowIdx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    /// <returns></returns>
    public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      this.DataViewItems[idx].BeginEdit();
      this.DataViewItems[idx]["ActivityItemReceivingQuantityReceived"] 
          = (decimal)DataViewItems[idx]["ActivityItemReceivingQuantityReceived"] + quantity;
      this.DataViewItems[idx]["ActivityItemReceivingBaseQuantityReceived"] = 
          (decimal)DataViewItems[idx]["ActivityItemReceivingBaseQuantityReceived"] + baseQuantity;
      this.DataViewItems[idx]["ActivityItemReceivingWeightQuantityReceived"] = 
          (decimal)DataViewItems[idx]["ActivityItemReceivingWeightQuantityReceived"] + weightQuantity;
      this.DataViewItems[idx].EndEdit();

      return idx;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <param name="activityItemRefno"></param>
    public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
    {
      using (SQLiteCommand cmndItemRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItemRecv.CommandType = CommandType.Text;
        //cmndItemRecv.IndexName = string.Empty;
        cmndItemRecv.CommandText = "UPDATE tActivityItemReceiving "
            + "SET ActivityItemReceivingQuantityReceived = ActivityItemReceivingQuantityReceived + " 
            + this.DataViewItems[idx]["ActivityItemReceivingQuantityReceived"] + "," 
            + "ActivityItemReceivingBaseQuantityReceived = ActivityItemReceivingBaseQuantityReceived + " 
            + this.DataViewItems[idx]["ActivityItemReceivingBaseQuantityReceived"] + "," 
            + "ActivityItemReceivingWeightQuantityReceived = ActivityItemReceivingWeightQuantityReceived + " 
            + this.DataViewItems[idx]["ActivityItemReceivingWeightQuantityReceived"] +
            " WHERE ActivityItemRefno = " + actItemRefno;
        cmndItemRecv.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetTotalQuantity()
    {
      string msg = string.Empty;

      using (SQLiteCommand cmndItemRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItemRecv.CommandType = System.Data.CommandType.Text;
        //cmndItemRecv.IndexName = string.Empty;
        cmndItemRecv.CommandText = "SELECT SUM(tActivityItemReceiving.ActivityItemReceivingQuantityOrdered) AS "
            + "Ordered,"
            + "SUM(tActivityItemReceiving.ActivityItemReceivingQuantityReceived) AS Received "
            + "FROM tActivityItem "
            + "INNER JOIN tActivityItemReceiving ON tActivityItemReceiving.ActivityItemRefno = "
            + "tActivityItem.ActivityItemRefno "
            + string.Format("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.Refno);

        using (SQLiteDataReader dr = cmndItemRecv.ExecuteReader())
        {
#if MULTILANGUAGE
          msg = Translator.GetInstance().Translate(TranslatorVocabulary.TotalReceived);
#else
          msg = "Total received:";
#endif
          while (dr.Read())
          {
            // Because an agregate sum function is used in a query. There will always be a result record.
            // This causes problems detecting if there is a valid result. 
            // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
            msg = msg + string.Format(
                "\r{0} / {1}",
                Convert.ToString(dr["Ordered"]),
                Convert.ToString(dr["Received"]));
          }
        }
      }

      return msg;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override void IsReadyToApply(XDocument data)
    {
      base.IsReadyToApply(data);

      // New items are identified with 0 < this.FKRefno, do not validate.
      if (this.SysEnv.SysSettings.RecvQtyOverWarn
          != SysCommon.ActReceivingQtyOverValLevels.NoValidationOrWarning
          && 0 < this.FKRefno)
      {
        try
        {
          this.ReceivingModuleQuantityOverCheck(
              int.Parse(data.Root.Element("itm_refno").Value),
              decimal.Parse(data.Root.Element("alt_qty").Value));
        }
        catch (Exception ex)
        {
          switch (this.SysEnv.SysSettings.RecvQtyOverWarn)
          {
            case SysCommon.ActReceivingQtyOverValLevels.ValidateWarnAndContinue:
              if (MessageBox.Show(
#if MULTILANGUAGE
                  ex.Message + Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.Continue),
                  Translator.GetInstance().Translate(TranslatorVocabulary.QuantityOver),
#else
                  ex.Message + Environment.NewLine + "Continue?",
                  "Quantity Over",
#endif
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Question,
                  MessageBoxDefaultButton.Button2) == DialogResult.No)
              {
                throw new IsReadyToApplyException(ex.Message, true, false);
              }
              else
              {
                ////throw new IsReadyToApplyException(ex.Message, true, true);
              }

              break;

            case SysCommon.ActReceivingQtyOverValLevels.ValidateWarnAndStop:
              throw new IsReadyToApplyException(ex.Message, false, false);
          }
        }
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemPickFKRefno"></param>
    /// <param name="actItemPickQuantity"></param>
    private void ReceivingModuleQuantityOverCheck(int actItemRecvFKRefno, decimal actItemRecvQuantity)
    {
      if (this.CurrRowIdx >= 0 && this.CurrRowIdx < this.DataViewItems.Count)
      {
        decimal received = (decimal)this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityReceived"];
        decimal ordered = (decimal)this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityOrdered"];

        if (received + actItemRecvQuantity > ordered)
        {
          throw new Exception(
              string.Format(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.TheResultingQuantityReceivedWillBeGreaterThanTheQuantityOrdered),
#else
              "The resulting quantity received {0} {1} will be greater than the quantity ordered {2} {3}.",
#endif
              received + actItemRecvQuantity,
              this.ProdInfo.UnitCode,
              ordered,
              this.ProdInfo.UnitCode));
        }
      }
    }

    #endregion
  }
}
