namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class ConvertUnitActItem : ActItem
    {
        #region Constructors

        public ConvertUnitActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(ConvertUnitActItem).FullName;
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
        public override async Task Delete()
        {
            if (this.ListItems != null)
            {
                this.ListItems.Clear();
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
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
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
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            if (this.ListItems != null)
            {
                SActItemSchemeData schemeData = Common.FillSActItemSchemeDataFromXML(data);
                this.ListItems.Add(
                    new ConvertUnitItem()
                    {
                        ProductUnitRefno = this.ProdInfo.UnitRefno,
                        ProductRefno = this.ProdInfo.Refno,
                        ProductCode = this.ProdInfo.Code,
                        ProductUnitBC = this.ProdInfo.UnitBC,
                        UnitCode = this.ProdInfo.UnitCode,
                        ProductDescription = this.ProdInfo.Desc,
                        ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc,
                        ActivityDetailQuantity = schemeData.ActDetailQuantity,
                        ActivityDetailBaseQuantity = schemeData.ActDetailBaseQuantity,
                        ActivityDetailWeightQuantity = schemeData.ActDetailWeightQuantity,
                        ActivityDetailLogisticUnitBestBeforeSellByDate = schemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate,
                        ActivityDetailLogisticUnitProductionDate = schemeData.ProdSchemeData.LogisticUnitProductionDate,
                        ActivityDetailSingleTradeItemSerialNumber = schemeData.ProdSchemeData.SingleTradeItemSerialNumber,
                        ActivityDetailLogisticUnitBatchLotNumber = schemeData.ProdSchemeData.LogisticUnitBatchLotNumber,
                        ProductSchemeVariantCode = schemeData.ProdSchemeData.VariantCode,
                        ProductSchemeVariantNumber = schemeData.ProdSchemeData.VariantNumber,
                        ActivityDetailSchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(this.ProdInfo.Code, this.ProdInfo.Scheme, schemeData.ProdSchemeData),
                        ActivityDetailUniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(this.ProdInfo.Code, this.ProdInfo.Scheme, schemeData.ProdSchemeData),
                        ActivityDetailFKRefno = this.FKRefno,
                        ActivityDetailRefno = this.Refno
                    });
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            DataTable moveTable = new DataTable("ActivityConvertUnitItem");

            moveTable.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ProductCode", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductUnitBC", Type.GetType("System.String"));
            moveTable.Columns.Add("UnitCode", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductDescription", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductUnitSizeDescription", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
            moveTable.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
            moveTable.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));
            moveTable.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
            moveTable.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
            moveTable.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductSchemeVariantNumber", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));

            this.ListItems = moveTable.Select().ToList<object>();
            return this.ListItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        public override Task<int> GetRefnoFromProdUnitRefno(int productUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override Task SelectActivityItem(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pbProgressBar"></param>
        /// <remarks></remarks>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idx"></param>
        public override async Task Submit(XDocument data, object selectedActivityItem)
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
                + "<action>SUBMIT CONVERT UNIT</action>"
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
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
