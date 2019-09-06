namespace dMobile.Controller
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class MoveInventoryActDetail : ActDetail
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityItem"></param>
    public MoveInventoryActDetail(ActItem activityItem)
      : base(activityItem)
    {
      this.FullClassName = typeof(MoveInventoryActDetail).FullName;
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <param name="skidInfo"></param>
    /// <remarks></remarks>
    public override void InsertSkid(SysSkidInfo skidInfo)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="skidInfo"></param>
    /// <remarks></remarks>
    public override void DeleteSkid(SysSkidInfo skidInfo)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public override void Insert(XDocument data)
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
        rowView["ActivityDetailSchemeID"] = this.SchemeID;
        rowView["ActivityDetailUniqueSchemeID"] = this.UniqueSchemeID;
        rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"] = this.SchemeData.LogisticUnitBestBeforeSellByDate;
        rowView["ActivityDetailLogisticUnitProductionDate"] = this.SchemeData.LogisticUnitProductionDate;
        rowView["ActivityDetailSingleTradeItemSerialNumber"] = this.SchemeData.SingleTradeItemSerialNumber;
        rowView["ActivityDetailLogisticUnitBatchLotNumber"] = this.SchemeData.LogisticUnitBatchLotNumber;
        rowView["ProductSchemeVariantCode"] = this.SchemeData.VariantCode;
        rowView["ProductSchemeVariantNo"] = this.SchemeData.VariantNumber;
        rowView["ActivityDetailFKRefno"] = this.FKRefno;
        rowView["ActivityDetailRefno"] = this.Refno;

        rowView.EndEdit();
      }
    }

    /// <summary>
    ///
    /// </summary>
    public override void Delete()
    {
      throw new NotImplementedException();
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
        this.FKRefno = (int)DataViewDetails[idx]["ActivityDetailFKRefno"];
      }

      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="data"></param>
    public override void Submit(int rowIdx, XDocument data)
    {
      this.GetInfo(rowIdx, false);

      XmlDocument xmlDoc = new XmlDocument();
      string xmlSend = this.CreateSubmitXML(data);
      string reqData = SysCommon.SubmitComm(xmlSend);

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

          ////if (this.DataViewDetails != null)
          ////{
          ////  this.DataViewDetails[rowIdx].BeginEdit();
          ////  ////this.DataViewDetails[rowIdx]["ActivityDetailAdjustmentFKRefno"] = this.FKRefno;
          ////  this.DataViewDetails[rowIdx]["ActivityDetailStatus"] = SysCommon.ActStatus.NoChange;
          ////  this.DataViewDetails[rowIdx].EndEdit();
          ////}

          ////using (SQLiteCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          ////{
          ////  cmndDetail.CommandType = CommandType.Text;
          ////  cmndDetail.CommandText = "UPDATE tActivityDetail "
          ////      + "SET tActivityDetail.ActivityDetailStatus = " + (int)SysCommon.ActStatus.NoChange
          ////      + "WHERE tActivityDetail.ActivityDetailRefno = " + this.Refno;
          ////  cmndDetail.ExecuteNonQuery();
          ////}
        }
      }

      this.DataViewDetails.Delete(rowIdx);
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
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override int GetRefno(int selectedIndex)
    {
      return (int)DataViewDetails[selectedIndex]["ActivityDetailRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public override int GetFKRefno(int idx)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    public override DataView GetDataView(bool isSubmitBatch)
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
      table.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
      table.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
      table.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
      table.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
      table.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
      table.Columns.Add("ProductSchemeVariantNo", Type.GetType("System.Int32"));
      table.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
      table.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));

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

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ProductCode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Codezz);
#else
      dataGridCol.HeaderText = "Code";
#endif
      dataGridCol.Width = (int)(62 * colWidthFactor);
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ProductUnitBC";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
#else
      dataGridCol.HeaderText = "BC";
#endif
      dataGridCol.Width = (int)(20 * colWidthFactor);
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "UnitCode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = (int)(22 * colWidthFactor);
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

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
    /// <param name="idx"></param>
    public override void SelectDetail(int idx)
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
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      string production = string.Empty;
      string bestBefore = string.Empty;

      if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
      {
        bestBefore = this.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy");
      }

      if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
      {
        production = this.SchemeData.LogisticUnitProductionDate.ToString("MM/dd/yyyy");
      }

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>SUBMIT MOVE</action>" 
          + "<whs_id>0</whs_id>" 
          + "<sect_refno>0</sect_refno>" 
          + "</command>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>") 
          + ("<is_bc>" + this.ActItem.ProdInfo.UnitIsBC + "</is_bc>") 
          + ("<whs_id>" + data.Root.Element("whs_id").Value + "</whs_id>") 
          + ("<sect_refno>" + data.Root.Element("sect_refno").Value + "</sect_refno>") 
          + ("<slot_refno>" + data.Root.Element("slot_refno").Value + "</slot_refno>") 
          + ("<to_sect_refno>" + data.Root.Element("to_sect_refno").Value + "</to_sect_refno>") 
          + ("<to_slot_refno>" + data.Root.Element("to_slot_refno").Value + "</to_slot_refno>") 
          + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>") 
          + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>") 
          + ("<alt_qty>" + this.Quantity + "</alt_qty>") 
          + ("<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>") 
          + ("<base_qty>" + this.BaseQuantity + "</base_qty>") 
          + ("<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>") 
          + ("<wt_qty>" + this.WeightQuantity + "</wt_qty>") 
          + ("<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") 
          + ("<serial>" + this.SchemeData.SingleTradeItemSerialNumber + "</serial>") 
          + ("<lot_no>" + this.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>") 
          + ("<pdate>" + production + "</pdate>") 
          + ("<bdate>" + bestBefore + "</bdate>") 
          + ("<variant_no>" + this.SchemeData.VariantNumber + "</variant_no>") 
          + ("<ser_refno>" + this.FKRefno + "</ser_refno>") 
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
