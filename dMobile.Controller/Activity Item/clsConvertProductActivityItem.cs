namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Text;    
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class ConvertProductActItem : ActItem
    {

        private int fromProdRefno;
        private int toProdRefno;

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="activity"></param>
        public ConvertProductActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(ConvertProductActItem).FullName;
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
                isCatchAsk = this.SysEnv.SysSettings.IsConvertUnitCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsConvertUnitModuleNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Delete()
        {
            if (this.DataViewItems != null)
            {
                while (this.DataViewItems.Count > 0)
                {
                    this.DataViewItems.Delete(0);
                }
            }
        }

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
        /// <remarks></remarks>
        public override void Insert(XDocument data, bool askToInsertAsNewItem)
        {
            if (this.DataViewItems != null)
            {
                DataRowView rowView = this.DataViewItems.AddNew();
                rowView.BeginEdit();

                rowView["from_prod_fk_refno"] = Convert.ToInt32(data.Root.Element("from_prod_fk_refno").Value);
                rowView["to_prod_fk_refno"] = Convert.ToInt32(data.Root.Element("to_prod_fk_refno").Value);
                rowView["location_map_refno"] = Convert.ToInt32(data.Root.Element("location_map_refno").Value);


                rowView.EndEdit();
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="dataGridMain"></param>
        ///// <remarks></remarks>
        //public override void DesignDataGrid(DataGrid dataGridMain)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            DataTable moveTable = new DataTable("ActivityConvertProductItem");

            moveTable.Columns.Add("from_prod_fk_refno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("to_prod_fk_refno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("location_map_refno", Type.GetType("System.Int32"));
            

            this.DataViewItems = moveTable.DefaultView;
            this.DataViewItems.Table.TableName = this.MainGridMappingName;

            return this.DataViewItems;
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
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        public override int GetRefnoFromProdUnitRefno(int productUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectItem(int idx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pbProgressBar"></param>
        /// <remarks></remarks>
        public override void Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idx"></param>
        public override void Submit(XDocument data, int idx)
        {
            string reqData = SysCommon.SubmitComm(this.CreateSubmitXML(data));

            if (reqData.Length > 0)
            {
                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument xmlError = new XmlDocument();
                    xmlError.LoadXml(reqData);
                    string errorDetail = xmlError.DocumentElement["detail"].InnerText;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder strXML = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(strXML);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool isBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                + "<command>"
                + "<action>SUBMIT CONVERT PRODUCT</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + isBC.ToString() + "</is_bc>")
                + ("<symbology>" + symbologyPrefix + "</symbology>")
                + ("<alt_unit>" + unitCode + "</alt_unit>")
                + ("<gtin>" + barCodeCode + "</gtin>")
                + ("<modifier1>" + this.SysEnv.CurrentBarCodeModifier1 + "</modifier1>")
                + "</imclient>");

            request.Save(writer);
            writer.Flush();
            writer.Close();

            return strXML.ToString();
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

        /// <summary>
        /// Gets the FROM product refno.
        /// </summary>
        public int FromProdRefno
        {
            get
            {
                return this.fromProdRefno;
            }
            set
            {
                this.fromProdRefno = value;
            }
        }

        /// <summary>
        /// Gets the TO product refno.
        /// </summary>
        public int ToProdRefno
        {
            get
            {
                return this.toProdRefno;
            }
            set
            {
                this.toProdRefno = value;
            }
        }
    }
}
