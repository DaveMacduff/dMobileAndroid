namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class InvInquiryActItem : ActItem
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public InvInquiryActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(CycleActItem).FullName;
            this.IsAskCreateNewItem = false;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override int UpdateDataViewQuantity(
            int rowIdx,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override void Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            if (prodUnitRefno != 0)
            {
                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
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
            dataGridCol.MappingName = "productcode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
#else
            dataGridCol.HeaderText = "Product ID";
#endif
            if (0 < ProdInfo.Refno)
            {
                dataGridCol.Width = -1;
            }
            else
            {
                dataGridCol.Width = (int)(65 * columnWidthFactor);
            }

            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "productunitbc";
            dataGridCol.HeaderText = string.Empty;
            dataGridCol.Width = (int)(10 * columnWidthFactor);
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "activityitemquantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Quantity);
#else
            dataGridCol.HeaderText = "Quantity";
#endif
            dataGridCol.Width = (int)(60 * columnWidthFactor);
            dataGridCol.Format = "0.000";
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "unitcode";
            dataGridCol.HeaderText = string.Empty;
            dataGridCol.Width = (int)(20 * columnWidthFactor);
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "activityitemweightquantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
#else
            dataGridCol.HeaderText = "Weight";
#endif
            dataGridCol.Width = (int)(60 * columnWidthFactor);
            dataGridCol.Format = "0.000";
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "weightunitcode";
            dataGridCol.HeaderText = string.Empty;
            dataGridCol.Width = (int)(20 * columnWidthFactor);
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridMain.TableStyles.Add(dataGridStyle);

            // dataGridMain.Font = "Arial, 10pt, style=Bold"
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
            DataTable dataTableItems = new DataTable("tActivityItem");

            dataTableItems.Columns.Add("productcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("productunitbc", Type.GetType("System.String"));
            dataTableItems.Columns.Add("unitcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("weightunitcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("activityitemquantity", Type.GetType("System.Decimal"));
            dataTableItems.Columns.Add("activityitemweightquantity", Type.GetType("System.Decimal"));

            this.DataViewItems = dataTableItems.DefaultView;
            this.DataViewItems.Table.TableName = this.MainGridMappingName;

            return this.DataViewItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromProdUnitRefno(int productUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectItem(int idx)
        {
            this.CurrRowIdx = idx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Refresh(XDocument data)
        {
            try
            {
                string reqData = SysCommon.SubmitComm(this.CreateSubmitXML(data));

                this.DataViewItems = this.GetDataView(false);
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);
                        throw new Exception(xmlDoc.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        using (DataSet ds = new DataSet())
                        {
                            ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);
                            if (ds.Tables.Count != 0)
                            {
                                this.DataViewItems = ds.Tables["tActivityItem"].DefaultView;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.RefreshInventoryInquiryItemsFailed), ex);
#else
                throw new Exception("Refresh inventory inquiry items failed.", ex);
#endif
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

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST INVENTORY INQUIRY SUMMARY</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + this.ProdInfo.Code + "</prod_id>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
