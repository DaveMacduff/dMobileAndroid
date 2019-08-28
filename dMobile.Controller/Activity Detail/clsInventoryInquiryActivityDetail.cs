namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class InventoryInquiryActDetail : ActDetail
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItem"></param>
    public InventoryInquiryActDetail(ActItem actItem)
      : base(actItem)
    {
      this.FullClassName = typeof(InventoryInquiryActDetail).FullName;
    }

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
      throw new NotImplementedException();
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
        this.FKRefno = (int)this.DataViewDetails[idx]["ActivityDetailFKRefno"];
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
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Refresh()
    {
      try
      {
        XDocument xmldocParams = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
            + "<params>"
            + "</params>");

        string reqData = SysCommon.SubmitComm(this.CreateSubmitXML(xmldocParams));

        this.DataViewDetails = this.GetDataView(false);
        if (reqData.Length > 0)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(reqData);
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
            using (DataSet ds = new DataSet())
            {
              ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);
              if (ds.Tables.Count != 0)
              {
                this.DataViewDetails = ds.Tables["tActivityDetail"].DefaultView;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.RefreshInventoryInquiryDetailsFailed), ex);
#else
        throw new Exception("Refresh inventory inquiry details failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public override int GetRefno(int idx)
    {
      return (int)this.DataViewDetails[idx]["ActivityDetailRefno"];
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
      DataTable dataTableDetails = new DataTable("tActivityDetail");

      dataTableDetails.Columns.Add("productcode", Type.GetType("System.String"));
      dataTableDetails.Columns.Add("productunitbc", Type.GetType("System.String"));
      dataTableDetails.Columns.Add("unitcode", Type.GetType("System.String"));
      dataTableDetails.Columns.Add("weightunitcode", Type.GetType("System.String"));
      dataTableDetails.Columns.Add("activitydetailquantity", Type.GetType("System.Decimal"));
      dataTableDetails.Columns.Add("activitydetailweightquantity", Type.GetType("System.Decimal"));
      dataTableDetails.Columns.Add("activitydetailschemeid", Type.GetType("System.String"));

      this.DataViewDetails = dataTableDetails.DefaultView;
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
      dataGridCol.MappingName = "productcode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
#else
      dataGridCol.HeaderText = "Product ID";
#endif
      if (0 < ActItem.ProdInfo.Refno)
      {
        dataGridCol.Width = -1;
      }
      else
      {
        dataGridCol.Width = (int)(65 * colWidthFactor);
      }

      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "productunitbc";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = (int)(10 * colWidthFactor);
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "activitydetailquantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Quantity);
#else
      dataGridCol.HeaderText = "Quantity";
#endif
      dataGridCol.Width = (int)(60 * colWidthFactor);
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "unitcode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = (int)(20 * colWidthFactor);
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "activitydetailweightquantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
#else
      dataGridCol.HeaderText = "Weight";
#endif
      dataGridCol.Width = (int)(60 * colWidthFactor);
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "weightunitcode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = (int)(20 * colWidthFactor);
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "activitydetailschemeid";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.SchemeId);
#else
      dataGridCol.HeaderText = "Scheme ID";
#endif
      dataGridCol.Width = (int)(500 * colWidthFactor);
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridMain.TableStyles.Add(dataGridStyle);

      // dataGridMain.Font = "Arial, 10pt, style=Bold"
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

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
          + "<imclient>"
          + "<command>"
          + "<action>REQUEST INVENTORY INQUIRY DETAIL</action>"
          + "<whs_id>0</whs_id>"
          + "<sect_refno>0</sect_refno>"
          + "</command>"
          + SysCommon.GetRequestDefaults(false)
          + ("<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>")
          + ("<bc>" + this.ActItem.ProdInfo.UnitBC + "</bc>")
          + ("<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>")
          + ("<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>")
          + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
          + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
          + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }
  }
}
