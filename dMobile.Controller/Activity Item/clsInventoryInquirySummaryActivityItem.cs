namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;

    public class InvInquirySummaryActItem : ActItem
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="activity"></param>
        public InvInquirySummaryActItem(Activity activity)
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
        public override int UpdateDataViewQuantity(int rowIdx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
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
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            if (prodUnitRefno != 0)
            {
                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            throw new NotImplementedException();
        }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="dataGridMain"></param>
//        /// <remarks></remarks>
//        public override void DesignDataGrid(DataGrid dataGridMain)
//        {
//            int columnWidthFactor = 1;

//            if (this.SysEnv.IsWindowsMobile6Device)
//            {
//                columnWidthFactor = 2;
//            }

//            if (dataGridMain.TableStyles.Count > 0)
//            {
//                dataGridMain.TableStyles.Clear();
//            }

//            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
//            dataGridStyle.MappingName = this.MainGridMappingName;

//            AddGridColumn(
//                (ProdInfo.Refno > 0) ? -1 : columnWidthFactor * 65,
//                "productcode",
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.ProductId),
//#else
//                "Product ID",
//#endif
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 10,
//                "productunitbc",
//                string.Empty,
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 60,
//                "activityitemquantity",
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.Quantity),
//#else
//                "Quantity",
//#endif
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 20,
//                "unitcode",
//                string.Empty,
//                dataGridStyle);


//            if (this.ProdInfo.Scheme.IsProductionDateTracked)
//            {
//                AddGridColumn(
//                    columnWidthFactor * 60,
//                    "activitydetaillogisticunitproductiondate",
//#if MULTILANGUAGE
//                    Translator.GetInstance().Translate(TranslatorVocabulary.ProdDate),
//#else
//                    "Prod Date",
//#endif
//                    dataGridStyle);
//            }

//            if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
//            {
//                AddGridColumn(
//                    columnWidthFactor * 60,
//                    "activitydetaillogisticunitbestbeforesellbydate",
//#if MULTILANGUAGE
//                    Translator.GetInstance().Translate(TranslatorVocabulary.BbsbDatez),
//#else
//                    "BB/SB Date",
//#endif
//                    dataGridStyle);
//            }

//            if (this.LocMap.CurrLocMap.IsSectionTracked)
//            {
//                AddGridColumn(
//                    columnWidthFactor * 60,
//                    "locationmapsectionfkrefno",
//#if MULTILANGUAGE
//                    Translator.GetInstance().Translate(TranslatorVocabulary.Section),
//#else
//                    "Section",
//#endif
//                    dataGridStyle);

//                if (this.LocMap.CurrLocMap.IsSlotTracked)
//                {
//                    AddGridColumn(
//                        columnWidthFactor * 60,
//                        "locationmapslotfkrefno",
//#if MULTILANGUAGE
//                        Translator.GetInstance().Translate(TranslatorVocabulary.Slot),
//#else
//                        "Slot",
//#endif
//                        dataGridStyle);
//                }
//            }

//            AddGridColumn(
//                columnWidthFactor * 60,
//                "skidcode",
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.Skidzz),
//#else
//                "Skid",
//#endif
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 60,
//                "activityitemweightquantity",
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.Weight),
//#else
//                "Weight",
//#endif
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 20,
//                "weightunitcode",
//                string.Empty,
//                dataGridStyle);

//            dataGridMain.TableStyles.Add(dataGridStyle);
//            dataGridMain.RowHeadersVisible = false;
//            dataGridMain.ColumnHeadersVisible = true;
//        }

//        private static void AddGridColumn(
//            int columnWidth,
//            string mappingName,
//            string headerText,
//            DataGridTableStyle dataGridStyle)
//        {
//            DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = mappingName;
//            dataGridCol.HeaderText = headerText;
//            dataGridCol.Width = columnWidth;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);
//        }

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
            dataTableItems.Columns.Add("activitydetaillogisticunitproductiondate", Type.GetType("System.String"));
            dataTableItems.Columns.Add("activitydetaillogisticunitbestbeforesellbydate", Type.GetType("System.String"));
            dataTableItems.Columns.Add("locationmapwarehousefkrefno", Type.GetType("System.String"));
            dataTableItems.Columns.Add("locationmapsectionfkrefno", Type.GetType("System.String"));
            dataTableItems.Columns.Add("locationmapslotfkrefno", Type.GetType("System.String"));
            dataTableItems.Columns.Add("skidcode", Type.GetType("System.String"));
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
        public override ObservableCollection<object> Search(string searchText)
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
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

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
                        this.DataLayerProgressOutHandler(this, new EventArgs(), 50);

                        using (DataSet ds = new DataSet())
                        {
                            ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                            if (ds.Tables.Count != 0)
                            {
                                this.DataViewItems = ds.Tables["tActivityItem"].DefaultView;
                            }
                        }

                        this.DataLayerProgressOutHandler(this, new EventArgs(), 80);
                    }
                }

                this.DataLayerProgressOutHandler(this, new EventArgs(), 95);
                FormatGrid();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 100);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                throw new Exception("Refresh inventory inquiry summary items failed.", ex);
            }
        }

        /// <summary>
        /// Hides the extra columns, changes the sectionFKRefno and slotFKRefno code to names.
        /// </summary>
        private void FormatGrid()
        {

            if (this.LocMap.CurrLocMap.IsSectionTracked)
            {
                // Format Section column
                // Use a dictionary to only hit the database once per unique section number this call.
                Dictionary<int, string> names = new Dictionary<int, string>();
                int rowCount = this.DataViewItems.Table.Rows.Count;

                for (int i = 0; i < rowCount; ++i)
                {
                    int sectionFKRefno = Convert.ToInt32(this.DataViewItems.Table.Rows[i]["locationmapsectionfkrefno"]);
                    string sectionName;

                    if (names.ContainsKey(sectionFKRefno))
                    {
                        sectionName = names[sectionFKRefno];
                    }
                    else
                    {
                        sectionName = LocationMapTasks.GetSectionName(sectionFKRefno);
                        names.Add(sectionFKRefno, sectionName);
                    }

                    this.DataViewItems.Table.Rows[i]["locationmapsectionfkrefno"] = sectionName;
                }

                names.Clear();
            }

            if (this.LocMap.CurrLocMap.IsSlotTracked)
            {
                // Format Slot column
                // Use a dictionary to only hit the database once per unique slot number this call.
                Dictionary<int, string> names = new Dictionary<int, string>();
                int rowCount = this.DataViewItems.Table.Rows.Count;

                for (int i = 0; i < rowCount; ++i)
                {
                    int slotFKRefno = Convert.ToInt32(this.DataViewItems.Table.Rows[i]["locationmapslotfkrefno"]);
                    string slotName;

                    if (names.ContainsKey(slotFKRefno))
                    {
                        slotName = names[slotFKRefno];
                    }
                    else
                    {
                        slotName = LocationMapTasks.GetSlotName(slotFKRefno);
                        names.Add(slotFKRefno, slotName);
                    }

                    this.DataViewItems.Table.Rows[i]["locationmapslotfkrefno"] = slotName;
                }

                names.Clear();
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

            // WO 15547, WO 13770
            string sectionRefno;
            string slotRefno;

            if (SysEnvironment.GetInstance().SysSettings.InventoryInquirySearchEntireWarehouse)
            {
                sectionRefno = string.Empty;
                slotRefno = string.Empty;
            }
            else
            {
                sectionRefno = this.LocMap.CurrLocMap.SectFKRefno.ToString();
                slotRefno = this.LocMap.CurrLocMap.SlotFKRefno.ToString();
            }

            string sectionXml = "<sect_refno>" + sectionRefno + "</sect_refno>";
            string slotXml = "<slot_refno>" + slotRefno + "</slot_refno>";

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST INVENTORY INQUIRY SUMMARY SHORTCUT</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + this.ProdInfo.Code + "</prod_id>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + sectionXml
                + slotXml
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