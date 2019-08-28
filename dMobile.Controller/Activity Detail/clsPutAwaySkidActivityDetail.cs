namespace dMobile.Controller
{
  using System;
  using System.Data;
  
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class PutAwaySkidActDetail : ActDetail
  {
    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="actItem"></param>
    /// <remarks></remarks>
    public PutAwaySkidActDetail(ActItem actItem)
      : base(actItem)
    {
      this.FullClassName = typeof(PutAwaySkidActDetail).FullName;
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skidInfo"></param>
    public override void InsertSkid(SysSkidInfo skidInfo)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skidInfo"></param>
    public override void DeleteSkid(SysSkidInfo skidInfo)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="detailData"></param>
    public override void Insert(XDocument detailData)
    {
      if (this.DataViewDetails != null)
      {
        DataRowView rowView = this.DataViewDetails.AddNew();
        rowView["ProductUnitRefno"] = this.ActItem.ProdInfo.UnitRefno;
        rowView["ProductRefno"] = this.ActItem.ProdInfo.Refno;
        rowView["ProductCode"] = this.ActItem.ProdInfo.Code;
        rowView["ProductUnitBC"] = this.ActItem.ProdInfo.UnitBC;
        rowView["UnitCode"] = this.ActItem.ProdInfo.UnitCode;
        rowView["ProductDescription"] = this.ActItem.ProdInfo.Desc;
        rowView["ProductUnitSizeDescription"] = this.ActItem.ProdInfo.UnitSizeDesc;

        rowView["ActivityDetailQuantity"] = Math.Round(this.Quantity, 3);
        rowView["ActivityDetailBaseQuantity"] = Math.Round(this.BaseQuantity, 3);
        rowView["ActivityDetailWeightQuantity"] = Math.Round(this.WeightQuantity, 3);

        rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"] = this.SchemeData.LogisticUnitBestBeforeSellByDate;
        rowView["ActivityDetailLogisticUnitProductionDate"] = this.SchemeData.LogisticUnitProductionDate;
        rowView["ActivityDetailSingleTradeItemSerialNumber"] = this.SchemeData.SingleTradeItemSerialNumber;
        rowView["ActivityDetailLogisticUnitBatchLotNumber"] = this.SchemeData.LogisticUnitBatchLotNumber;
        rowView["ProductSchemeVariantCode"] = this.SchemeData.VariantCode;
        rowView["ProductSchemeVariantNo"] = this.SchemeData.VariantNumber;
        rowView["ActivityDetailLogisticUnitSkidNumber"] = this.SchemeData.LogisticUnitSkidNumber;
        rowView["ActivityDetailSchemeID"] = this.SchemeID;
        rowView["ActivityDetailUniqueSchemeID"] = this.UniqueSchemeID;

        rowView["ActivityDetailFKRefno"] = this.FKRefno;
        rowView["ActivityDetailRefno"] = this.Refno;

        rowView["ActivityDetailExtra1Text"] = this.ActExtra.Text1;
        rowView["ActivityDetailExtra2Text"] = this.ActExtra.Text2;
        rowView["ActivityDetailExtra3Text"] = this.ActExtra.Text3;
        rowView["ActivityDetailExtra4Text"] = this.ActExtra.Text4;
        rowView["ActivityDetailExtra5Text"] = this.ActExtra.Text5;

        string locCode = string.Empty;
        string locName = string.Empty;
        
        if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
        {
          locCode = this.LocMap.CurrLocMap.SlotCode.ToString();
          locName = this.LocMap.CurrLocMap.SlotName;
        }
        else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
        {
          locCode = this.LocMap.CurrLocMap.SectCode.ToString();
          locName = this.LocMap.CurrLocMap.SectName;
        }
        else
        {
          locCode = this.LocMap.CurrLocMap.WhsCode.ToString();
          locName = this.LocMap.CurrLocMap.WhsName;
        }

        rowView["LocationMapRefno"] = this.LocMap.CurrLocMapRefno;
        rowView["LocationCode"] = locCode;
        rowView["LocationName"] = locName;

        rowView.EndEdit();
      }
    }

    /// <summary>
    ///
    /// </summary>
    public override void Delete()
    {
      for (int i = 0; i <= this.DataViewDetails.Count; i++)
      {
        this.DataViewDetails.Delete(0);
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
        this.FKRefno = (int)this.DataViewDetails[idx]["ActivityDetailFKRefno"];
        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(
            (int)this.DataViewDetails[idx]["LocationMapRefno"]);
      }

      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="data"></param>
    public override void Submit(int idx, XDocument data)
    {
      this.GetInfo(idx, false);

      XmlDocument xmlDoc = new XmlDocument();
      string xmlSend = this.CreateSubmitXML(data);
      string reqData = SysCommon.SubmitComm(xmlSend);

      if (reqData.Length > 0)
      {
        xmlDoc.LoadXml(reqData);

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
            this.DataViewDetails[idx].BeginEdit();
            ////this.DataViewDetails[idx]["ActivityDetailProductionFinishedFKRefno"] = this.FKRefno;
            this.DataViewDetails[idx]["ActivityDetailStatus"] = SysCommon.ActStatus.NoChange;
            this.DataViewDetails[idx].EndEdit();
          }

          using (SqlCeCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndDetail.CommandType = CommandType.Text;
            cmndDetail.CommandText = "UPDATE tActivityDetail "
                + "SET tActivityDetail.ActivityDetailStatus = " + (int)SysCommon.ActStatus.NoChange
                + " WHERE tActivityDetail.ActivityDetailRefno = " + this.Refno;
            cmndDetail.ExecuteNonQuery();
          }
        }
      }

      this.DataViewDetails.Delete(idx);
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Refresh()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public override int GetRefno(int idx)
    {
      return (int)DataViewDetails[idx]["ActivityDetailRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override int GetFKRefno(int selectedIndex)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    public override System.Data.DataView GetDataView(bool isSubmitBatch)
    {
      DataTable table = new DataTable("ActivityMoveDetail");

      table.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32"));
      table.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
      table.Columns.Add("ProductCode", Type.GetType("System.String"));
      table.Columns.Add("ProductUnitBC", Type.GetType("System.String"));
      table.Columns.Add("UnitCode", Type.GetType("System.String"));
      table.Columns.Add("ProductDescription", Type.GetType("System.String"));
      table.Columns.Add("ProductUnitSizeDescription", Type.GetType("System.String"));

      table.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
      table.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
      table.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));

      table.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
      table.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
      table.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
      table.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
      table.Columns.Add("ProductSchemeVariantNo", Type.GetType("System.Int32"));
      table.Columns.Add("ActivityDetailLogisticUnitSkidNumber", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));

      table.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
      table.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));

      table.Columns.Add("ActivityDetailExtra1Text", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailExtra2Text", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailExtra3Text", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailExtra4Text", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailExtra5Text", Type.GetType("System.String"));

      table.Columns.Add("LocationMapRefno", Type.GetType("System.Int32"));
      table.Columns.Add("LocationCode", Type.GetType("System.String"));
      table.Columns.Add("LocationName", Type.GetType("System.String"));

      this.DataViewDetails = table.DefaultView;
      this.DataViewDetails.Table.TableName = this.MainGridMappingName;

      return this.DataViewDetails;
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

      DataGridTextBoxColumn dataGridCol = null;

      ////dataGridCol = New DataGridTextBoxColumn()
      ////dataGridCol.MappingName = "ProductCode"
      ////dataGridCol.HeaderText = "Code"
      ////dataGridCol.Width = 62 * colWidthFactor
      ////dataGridStyle.GridColumnStyles.Add(dataGridCol)

      ////dataGridCol = New DataGridTextBoxColumn()
      ////dataGridCol.MappingName = "ProductUnitBC"
      ////dataGridCol.HeaderText = "BC"
      ////dataGridCol.Width = 20 * colWidthFactor
      ////dataGridStyle.GridColumnStyles.Add(dataGridCol)

      ////dataGridCol = New DataGridTextBoxColumn()
      ////dataGridCol.MappingName = "UnitCode"
      ////dataGridCol.HeaderText = string.Empty
      ////dataGridCol.Width = 22 * colWidthFactor
      ////dataGridStyle.GridColumnStyles.Add(dataGridCol)

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityDetailQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
#else
      dataGridCol.HeaderText = "Qty";
#endif
      dataGridCol.Width = (int)(50 * colWidthFactor);
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityDetailWeightQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
#else
      dataGridCol.HeaderText = "Weight";
#endif
      dataGridCol.Width = (int)(55 * colWidthFactor);
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityDetailSchemeID";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.SchemeId);
#else
      dataGridCol.HeaderText = "Scheme ID";
#endif
      dataGridCol.Width = (int)(500 * colWidthFactor);
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
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    public override void SelectDetail(int selectedIndex)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateDeleteXML(XDocument data)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateSubmitXML(XDocument data)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
