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

  public class AdjActDetail : ActDetail
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityItem"></param>
    public AdjActDetail(ActItem activityItem)
      : base(activityItem)
    {
      this.FullClassName = typeof(AdjActDetail).FullName;
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skidInfo"></param>
    public override void InsertSkid(SysSkidInfo skidInfo)
    {
      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
          + "<params>"
          + ("<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>")
          + "</params>");

      string xmlSend = this.CreateSubmitXML(request);
      string reqData = SysCommon.SubmitComm(xmlSend);
      if (reqData.Length > 0)
      {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(reqData);

        // check if result is an error message
        if (reqData.Contains("<message>ERROR</message>"))
        {
          string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
          if (errorDetail.Contains("MSGBOX-"))
          {
            throw new MessageOnlyException(errorDetail);
          }
          else
          {
            throw new Exception(errorDetail);
          }
        }
        else
        {
          this.IsUpdateView = true;
          this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
          this.NewItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
          this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
          this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
          this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);

          // NOTE: should we have sent the extra detail from the comm server? should we load the extra detail
          if (this.NewItemFKRefno != this.ActItem.FKRefno)
          {
            // Update the Activity Item quantity Adjustment
            using (SqlCeCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndItem.CommandType = CommandType.Text;
              cmndItem.IndexName = string.Empty;
              cmndItem.CommandText = "UPDATE tActivityItemAdjustment "
                  + "SET ActivityItemAdjustmentStringFKRefno = \'" + this.NewItemFKRefno + "\', "
                  + "ActivityItemAdjustmentFKRefno = " + this.NewItemFKRefno
                  + " WHERE tActivityItemAdjustment.ActivityItemRefno = " + this.ActItem.Refno;
              cmndItem.ExecuteNonQuery();

              this.ActItem.FKRefno = this.NewItemFKRefno;
              string oldSort = this.ActItem.DataViewItems.Sort;
              this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
              var rowIdx = this.ActItem.DataViewItems.Find(new object[] { this.ActItem.Refno });
              if (rowIdx != -1)
              {
                DataRowView rowView = this.ActItem.DataViewItems[rowIdx];
                rowView.BeginEdit();
                rowView["ActivityItemAdjustmentFKRefno"] = this.NewItemFKRefno;
                rowView["ActivityItemAdjustmentStringFKRefno"] = this.NewItemFKRefno.ToString();
                rowView.EndEdit();
              }

              this.ActItem.DataViewItems.Sort = oldSort;
            }
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skidInfo"></param>
    public override void DeleteSkid(SysSkidInfo skidInfo)
    {
      if (!SysEnv.SysSettings.IsDisconnectedMode)
      {
        string xmlDelete = this.CreateDeleteXML(
            XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
        string reqData = SysCommon.SubmitComm(xmlDelete);
        if (reqData.Length > 0)
        {
          // Check if result is an error message
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
    public override void Insert(XDocument data)
    {
      this.FillActDetailObjFromXML(data);

      if (this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        int actDetailRefno = this.InsertDetailDisconnected();

        using (SqlCeCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndDetail.CommandText = "tActivityDetailAdjustment";
          cmndDetail.CommandType = CommandType.TableDirect;
          cmndDetail.IndexName = "UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno";

          using (SqlCeResultSet rsDetail = cmndDetail.ExecuteResultSet(ResultSetOptions.Updatable))
          {
            SqlCeUpdatableRecord recDetail = rsDetail.CreateRecord();

            recDetail.SetInt32((int)SysCommon.ActDetailAdjFields.ActDetailRefno, actDetailRefno);
            recDetail.SetInt32((int)SysCommon.ActDetailAdjFields.FKRefno, this.FKRefno);

            rsDetail.Insert(recDetail);

            this.IsUpdateView = true;
          }
        }
      }
      else
      {
        if (this.InsertDetail(
            this.CreateSubmitXML(
            XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"))))
        {
          using (SqlCeCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndItem.IndexName = string.Empty;
            cmndItem.CommandType = CommandType.Text;
            cmndItem.CommandText = "UPDATE tActivityItemAdjustment "
                + "SET ActivityItemAdjustmentStringFKRefno = \'" + this.NewItemFKRefno + "\',"
                + "ActivityItemAdjustmentFKRefno = " + this.NewItemFKRefno
                + " WHERE tActivityItemAdjustment.ActivityItemRefno = " + this.ActItem.Refno;
            cmndItem.ExecuteNonQuery();
          }

          string oldSort = this.ActItem.DataViewItems.Sort;
          this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

          int rowIdx = this.ActItem.DataViewItems.Find(new object[] { this.ActItem.Refno });
          if (rowIdx != -1)
          {
            DataRowView rowView = this.ActItem.DataViewItems[rowIdx];
            rowView.BeginEdit();
            rowView["ActivityItemAdjustmentFKRefno"] = this.NewItemFKRefno;
            rowView["ActivityItemAdjustmentStringFKRefno"] = this.NewItemFKRefno.ToString();
            rowView.EndEdit();
          }

          this.ActItem.DataViewItems.Sort = oldSort;
        }
      }

      this.InsertUpdateDataViewDetail("ActivityDetailAdjustmentFKRefno", "ActivityDetailAdjustmentFKRefno");
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Delete()
    {
      if (!this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        string xmlDelete = this.CreateDeleteXML(
            XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

        string reqData = SysCommon.SubmitComm(xmlDelete);
        if (reqData.Length > 0)
        {
          // check if result is an error message
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

      // delete row from DataView
      string mySort = this.DataViewDetails.Sort;
      this.DataViewDetails.Sort = "ActivityDetailAdjustmentFKRefno";
      int rowIdx = this.DataViewDetails.Find(FKRefno);
      if (rowIdx >= 0)
      {
        this.DataViewDetails.Delete(rowIdx);
      }

      this.DataViewDetails.Sort = mySort;

      // Delete from database
      using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndPurge.CommandType = CommandType.Text;
        cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
            + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = " + this.Refno;
        cmndPurge.ExecuteNonQuery();

        cmndPurge.CommandText = "DELETE FROM tActivityDetailAdjustment "
            + "WHERE tActivityDetailAdjustment.ActivityDetailRefno = " + this.Refno;
        cmndPurge.ExecuteNonQuery();

        cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
            + "WHERE tActivityDetailExtra.ActivityDetailRefno = " + this.Refno;
        cmndPurge.ExecuteNonQuery();

        cmndPurge.CommandText = "DELETE FROM tActivityDetail "
            + "WHERE tActivityDetail.ActivityDetailRefno = " + this.Refno;
        cmndPurge.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowIdx"></param>
    /// <param name="data"></param>
    public override void Submit(int rowIdx, XDocument data)
    {
      XmlDocument xmlDoc = new XmlDocument();
      string strXmlSend = this.CreateSubmitXML(
          XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
      string reqData = SysCommon.SubmitComm(strXmlSend);

      if (reqData.Length > 0)
      {
        xmlDoc.LoadXml(reqData);

        // check if result is an error message
        if (reqData.Contains("<message>ERROR</message>"))
        {
          string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
          if (errorDetail.Contains("MSGBOX-"))
          {
            throw new MessageOnlyException(errorDetail);
          }
          else
          {
            throw new Exception(errorDetail);
          }
        }
        else
        {
          this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

          if (this.DataViewDetails != null)
          {
            this.DataViewDetails[rowIdx].BeginEdit();
            this.DataViewDetails[rowIdx]["ActivityDetailAdjustmentFKRefno"] = this.FKRefno;
            this.DataViewDetails[rowIdx]["ActivityDetailStatus"] = SysCommon.ActStatus.NoChange;
            this.DataViewDetails[rowIdx].EndEdit();
          }

          using (SqlCeCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndDetail.CommandType = CommandType.Text;
            cmndDetail.CommandText = "UPDATE tActivityDetail "
                + "SET tActivityDetail.ActivityDetailStatus = " + (int)SysCommon.ActStatus.NoChange
                + "WHERE tActivityDetail.ActivityDetailRefno = " + this.Refno;
            cmndDetail.ExecuteNonQuery();
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="isAdd"></param>
    /// <returns></returns>
    public override ActDetail GetInfo(int idx, bool isAdd)
    {
      base.GetInfo(idx, isAdd);

      if (!isAdd)
      {
        this.FKRefno = (int)this.DataViewDetails[idx]["ActivityDetailAdjustmentFKRefno"];
        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)DataViewDetails[idx]["LocationMapRefno"]);
      }

      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Refresh()
    {
      if (this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
      }
      else
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

        TableActivityAdjustmentDetail adjDetail = new TableActivityAdjustmentDetail();

        adjDetail.Purge();

        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

        adjDetail.RegisterDataAccessListenerProgressOut((AdjActDetail)this);
        adjDetail.Request(this.ActItem.Refno);
        adjDetail.UnregisterDataAccessListenerProgressOut((AdjActDetail)this);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override int GetRefno(int selectedIndex)
    {
      return (int)DataViewDetails[selectedIndex]["ActivityDetailRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override int GetFKRefno(int selectedIndex)
    {
      return (int)DataViewDetails[selectedIndex]["ActivityDetailAdjustmentFKRefno"];
    }

    /// <summary>
    /// Get a data view of the activity detail.
    /// </summary>
    /// <param name="isSubmitBatch"></param>
    /// <remarks>The sql queries use a UNION this is needed to support base distrib-u-tec with a WMS Map turned on. 
    /// The map could include slots.
    /// Because the current location in dMobile may be a slot, base distrib-u-tec does not contain a slot location
    /// where the detail is stored, only the warehouse. So a union including the warehouse is needed to included 
    /// the received data from the base to match up with location refno. Have a default location on the slot level
    /// really only shows the user where to find the product. However the detail sent is lost if base wms is being 
    /// used.</remarks>
    /// <returns>Data view result of the query.</returns>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
        sql.Append("tActivityDetail.LocationMapRefno,");
        sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
        sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityDetail.ActivityDetailQuantity,");
        sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
        sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
        sql.Append("tActivityDetail.ActivityDetailSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
        sql.Append("tLocationMapSlot.LocationMapSlotCode AS LocationCode,");
        sql.Append("CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,");
        sql.Append("\'Slot\' AS Location,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
        sql.Append("tActivityDetail.ActivityDetailStatus,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
        sql.Append("FROM tActivityDetailAdjustment ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
        sql.Append("= tActivityDetail.LocationMapRefno ");
        sql.Append("INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tProduct.ProductSchemeRefno ");
        sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
        sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
        sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
        sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
        sql.Append("= tActivityDetail.ActivityDetailRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
        sql.Append("tActivityDetail.LocationMapRefno,");
        sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
        sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityDetail.ActivityDetailQuantity,");
        sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
        sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
        sql.Append("tActivityDetail.ActivityDetailSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
        sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
        sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
        sql.Append("\'Section\' AS Location,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
        sql.Append("tActivityDetail.ActivityDetailStatus,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
        sql.Append("FROM tActivityDetailAdjustment ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
        sql.Append("= tActivityDetail.LocationMapRefno ");
        sql.Append("INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tProduct.ProductSchemeRefno ");
        sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
        sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
        sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
        sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
        sql.Append("= tActivityDetail.ActivityDetailRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
        sql.Append("tActivityDetail.LocationMapRefno,");
        sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
        sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityDetail.ActivityDetailQuantity,");
        sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
        sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
        sql.Append("tActivityDetail.ActivityDetailSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("\'Whs\' AS Location,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
        sql.Append("tActivityDetail.ActivityDetailStatus,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
        sql.Append("FROM tActivityDetailAdjustment ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityDetail.LocationMapRefno ");
        sql.Append("INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tProduct.ProductSchemeRefno ");
        sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
        sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
        sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
        sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
        sql.Append("= tActivityDetail.ActivityDetailRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
      }
      else
      {
        sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
        sql.Append("tActivityDetail.LocationMapRefno,");
        sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
        sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityDetail.ActivityDetailQuantity,");
        sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
        sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
        sql.Append("tActivityDetail.ActivityDetailSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
        sql.Append("tLocationMapSlot.LocationMapSlotCode AS LocationCode,");
        sql.Append("CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,");
        sql.Append("\'Slot\' AS Location,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
        sql.Append("tActivityDetail.ActivityDetailStatus,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
        sql.Append("FROM tActivityDetailAdjustment ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
        sql.Append("= tActivityDetail.LocationMapRefno ");
        sql.Append("INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tProduct.ProductSchemeRefno ");
        sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
        sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
        sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
        sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
        sql.Append("= tActivityDetail.ActivityDetailRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
        sql.Append("tActivityDetail.LocationMapRefno,");
        sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
        sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityDetail.ActivityDetailQuantity,");
        sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
        sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
        sql.Append("tActivityDetail.ActivityDetailSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
        sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
        sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
        sql.Append("\'Section\' AS Location,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
        sql.Append("tActivityDetail.ActivityDetailStatus,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
        sql.Append("FROM tActivityDetailAdjustment ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
        sql.Append("= tActivityDetail.LocationMapRefno ");
        sql.Append("INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tProduct.ProductSchemeRefno ");
        sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
        sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
        sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
        sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
        sql.Append("= tActivityDetail.ActivityDetailRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
        sql.Append("tActivityDetail.LocationMapRefno,");
        sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
        sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityDetail.ActivityDetailQuantity,");
        sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
        sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
        sql.Append("tActivityDetail.ActivityDetailSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
        sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
        sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("\'Whs\' AS Location,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
        sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
        sql.Append("tActivityDetail.ActivityDetailStatus,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
        sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
        sql.Append("FROM tActivityDetailAdjustment ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
        sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityDetail.LocationMapRefno ");
        sql.Append("INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tProduct.ProductSchemeRefno ");
        sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
        sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
        sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
        sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
        sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
        sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
        sql.Append("= tActivityDetail.ActivityDetailRefno ");
        sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
        sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
      }

      using (SqlCeDataAdapter da = new SqlCeDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet ds = new DataSet())
        {
          da.Fill(ds);
          this.DataViewDetails = ds.Tables[0].DefaultView;
          this.DataViewDetails.Table.TableName = this.MainGridMappingName;
        }
      }

      return this.DataViewDetails;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dataGridMain"></param>
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
      dataGridCol.MappingName = "ActivityDetailQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
#else
      dataGridCol.HeaderText = "Qty";
#endif
      dataGridCol.Width = 50 * colWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityDetailWeightQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
#else
      dataGridCol.HeaderText = "Weight";
#endif
      dataGridCol.Width = 55 * colWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "UnitCode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = 30 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityDetailSchemeID";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.SchemeId);
#else
      dataGridCol.HeaderText = "Scheme ID";
#endif
      dataGridCol.Width = 500 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridMain.TableStyles.Add(dataGridStyle);
      dataGridMain.RowHeadersVisible = false;
      dataGridMain.ColumnHeadersVisible = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public override DataView Search(string searchText)
    {
      this.DataViewDetails.RowFilter = "ActivityAdjustmentStringFKRefno LIKE \'" + searchText.Trim() + "%\'";

      return this.DataViewDetails;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    public override void SelectDetail(int idx)
    {
      if (idx >= 0)
      {
        this.SelectedLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)DataViewDetails[idx]["LocationMapRefno"]);
        this.FKRefno = (int)DataViewDetails[idx]["ActivityDetailAdjustmentFKRefno"];
        this.Refno = (int)DataViewDetails[idx]["ActivityDetailRefno"];
        this.NewItemFKRefno = 0;
        this.SchemeID = (string)DataViewDetails[idx]["ActivityDetailSchemeID"];
        this.UniqueSchemeID = (string)DataViewDetails[idx]["ActivityDetailUniqueSchemeID"];
        this.Quantity = (decimal)DataViewDetails[idx]["ActivityDetailQuantity"];
        this.BaseQuantity = (decimal)DataViewDetails[idx]["ActivityDetailBaseQuantity"];
        this.WeightQuantity = (decimal)DataViewDetails[idx]["ActivityDetailWeightQuantity"];
        this.ActExtra.Text1 = (string)DataViewDetails[idx]["ActivityDetailExtra1Text"];
        this.ActExtra.Text2 = (string)DataViewDetails[idx]["ActivityDetailExtra2Text"];
        this.ActExtra.Text3 = (string)DataViewDetails[idx]["ActivityDetailExtra3Text"];
        this.ActExtra.Text4 = (string)DataViewDetails[idx]["ActivityDetailExtra4Text"];
        this.ActExtra.Text5 = (string)DataViewDetails[idx]["ActivityDetailExtra5Text"];
      }
      else
      {
        this.ClearProperties();
      }
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
      SysCommon.AdjTypes adjType = SysCommon.AdjTypes.Inventory;
      AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();

      using (SqlCeCommand cmndAdj = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndAdj.CommandText = "SELECT tActivityAdjustment.ActivityAdjustmentType FROM tActivityAdjustment "
            + "WHERE tActivityAdjustment.ActivityRefno = " + this.ActItem.ActHdr.Refno;
        cmndAdj.CommandType = CommandType.Text;
        adjType = (SysCommon.AdjTypes)cmndAdj.ExecuteScalar();
      }

      XDocument request = null;
      if (data.Document.Root.Element("skid_data") == null)
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
            + "<imclient>"
            + "<command>"
            + "<action>SUBMIT ADJUSTMENT DETAIL</action>"
            + "<whs_id>0</whs_id>"
            + "<sect_refno>0</sect_refno>"
            + "</command>"
            + SysCommon.GetRequestDefaults(false)
            + this.GetRequestDetailDefaults()
            + ("<adjustment_type>" + (int)adjType + "</adjustment_type>")
            + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
            + "</imclient>");
      }
      else
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
            + "<imclient>"
            + "<command>"
            + "<action>SUBMIT ADJUSTMENT DETAIL SKID</action>"
            + "<whs_id>0</whs_id>"
            + "<sect_refno>0</sect_refno>"
            + "</command>"
            + SysCommon.GetRequestDefaults(false)
            + ("<adjustment_type>" + (int)adjType + "</adjustment_type>")
            + ("<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>")
            + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
            + ("<refno>" + this.ActItem.ActHdr.FKRefno.ToString() + "</refno>")
            + ("<itm_refno>" + this.ActItem.FKRefno.ToString() + "</itm_refno>")
            + ("<is_negate_qty>" + adjSettings.NegateScanQuantity + "</is_negate_qty>")
            + ("<extra1>" + this.ActExtra.Text1 + "</extra1>")
            + ("<extra2>" + this.ActExtra.Text2 + "</extra2>")
            + ("<extra3>" + this.ActExtra.Text3 + "</extra3>")
            + ("<extra4>" + this.ActExtra.Text4 + "</extra4>")
            + ("<extra5>" + this.ActExtra.Text5 + "</extra5>")
            + "</imclient>");
      }

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
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
      SysCommon.AdjTypes adjType = SysCommon.AdjTypes.Inventory;

      using (SqlCeCommand cmndAdj = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndAdj.CommandText = "SELECT tActivityAdjustment.ActivityAdjustmentType FROM tActivityAdjustment "
            + "WHERE tActivityAdjustment.ActivityRefno = " + this.ActItem.ActHdr.Refno;
        cmndAdj.CommandType = CommandType.Text;
        adjType = (SysCommon.AdjTypes)cmndAdj.ExecuteScalar();
      }

      XDocument request = null;
      if (data.Document.Root.Element("skid_data") == null)
      {
        if (this.SysEnv.SysSettings.IsDisconnectedMode)
        {
          request = XDocument.Parse(
              "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
              + "<imclient>"
              + "<command>"
              + "<action>DELETE ADJUSTMENT DETAIL</action>"
              + "<whs_id>0</whs_id>"
              + "<sect_refno>0</sect_refno>"
              + "</command>"
              + SysCommon.GetRequestDefaults(false)
              + this.GetRequestDetailDefaults()
              + ("<adjustment_type>" + (int)adjType + "</adjustment_type>")
              + "<skid_fk_refno>0</skid_fk_refno>"
              + "</imclient>");
        }
        else
        {
          request = XDocument.Parse(
              "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
              + "<imclient>"
              + "<command>"
              + "<action>DELETE ADJUSTMENT DETAIL</action>"
              + "<whs_id>0</whs_id>"
              + "<sect_refno>0</sect_refno>"
              + "</command>"
              + SysCommon.GetRequestDefaults(false)
              + this.GetRequestDetailDefaults()
              + ("<adjustment_type>" + (int)adjType + "</adjustment_type>")
              + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
              + "</imclient>");
        }
      }
      else
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
            + "<imclient>"
            + "<command>"
            + "<action>DELETE ADJUSTMENT DETAIL SKID</action>"
            + "<whs_id>0</whs_id>"
            + "<sect_refno>0</sect_refno>"
            + "</command>"
            + SysCommon.GetRequestDefaults(false)
            + ("<adjustment_type>" + (int)adjType + "</adjustment_type>")
            + ("<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>")
            + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
            + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
            + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
            + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
            + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
            + "<is_negate_qty>false</is_negate_qty>"
            + ("<extra1>" + this.ActExtra.Text1 + "</extra1>")
            + ("<extra2>" + this.ActExtra.Text2 + "</extra2>")
            + ("<extra3>" + this.ActExtra.Text3 + "</extra3>")
            + ("<extra4>" + this.ActExtra.Text4 + "</extra4>")
            + ("<extra5>" + this.ActExtra.Text5 + "</extra5>")

            + "</imclient>");
      }

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
