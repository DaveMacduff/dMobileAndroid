namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Windows.Forms;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class ConvertUnitActDetail : ActDetail
  {
    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="_actItem"></param>
    /// <remarks></remarks>
    public ConvertUnitActDetail(ActItem _actItem)
      : base(_actItem)
    {
      this.FullClassName = typeof(ConvertUnitActDetail).FullName;
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
    /// <param name="data"></param>
    public override void Insert(XDocument data)
    {
      if (this.DataViewDetails != null)
      {
        DataRowView rowView = this.DataViewDetails.AddNew();
        rowView["CountNumber"] = this.DataViewDetails.Count;
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
        rowView["ProductSchemeVariantNumber"] = this.SchemeData.VariantNumber;
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
      if (this.DataViewDetails != null)
      {
        while (this.DataViewDetails.Count > 0)
        {
          this.DataViewDetails.Delete(0);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="data"></param>
    public override void Submit(int rowIndex, XDocument data)
    {
      throw new NotImplementedException();
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
    /// <param name="rowIdx"></param>
    /// <returns></returns>
    public override int GetRefno(int rowIdx)
    {
      return (int)this.DataViewDetails[rowIdx]["ActivityDetailRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowIdx"></param>
    /// <returns></returns>
    public override int GetFKRefno(int rowIdx)
    {
      return (int)this.DataViewDetails[rowIdx]["ActivityDetailCountFKRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      DataTable table = new DataTable("ActivityConvertUnitItem");

      table.Columns.Add("CountNumber", Type.GetType("System.Int32"));
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
      table.Columns.Add("ProductSchemeVariantNumber", Type.GetType("System.Int32"));
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
    /// <remarks></remarks>
    public override void DesignDataGrid(DataGrid dataGridMain)
    {
      int columnWidthFactor = 1;

      if (this.SysEnv.IsWindowsMobile6Device)
      {
        columnWidthFactor = 2;
      }

      if (dataGridMain.TableStyles.Count > 0)
      {
        dataGridMain.TableStyles.Clear();
      }

      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
      dataGridStyle.MappingName = this.MainGridMappingName;

      DataGridTextBoxColumn dataGridCol = null;

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "CountNumber";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Unit);
#else
      dataGridCol.HeaderText = "Unit#";
#endif
      dataGridCol.Width = (int)(40 * columnWidthFactor);
      dataGridCol.Format = "0000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityDetailQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Quantity);
#else
      dataGridCol.HeaderText = "Quantity";
#endif
      dataGridCol.Width = (int)(80 * columnWidthFactor);
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityDetailWeightQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
#else
      dataGridCol.HeaderText = "Weight";
#endif
      dataGridCol.Width = (int)(80 * columnWidthFactor);
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridMain.TableStyles.Add(dataGridStyle);
      //dataGridMain.Font = "Arial, 10pt, style=Bold"
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
      throw new NotImplementedException();
    }

    #endregion
  }
}
