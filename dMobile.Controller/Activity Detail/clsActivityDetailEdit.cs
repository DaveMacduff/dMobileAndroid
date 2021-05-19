namespace dMobile.Controller
{
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using dMobile.Resources.Types;

    public class ActDetailEdit
    {
        #region Fields

        private SProductSchemeData schemeData = new SProductSchemeData();
        private decimal quantity = 0;
        private decimal baseQuantity = 0;
        private decimal weightQuantity = 0;
        private ActDetailExtra actExtra = new ActDetailExtra();

        #endregion

        #region Properties

        public ActDetailExtra ActExtra
        {
            get
            {
                return this.actExtra;
            }

            set
            {
                this.actExtra = value;
            }
        }

        public decimal Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                this.quantity = value;
            }
        }

        public decimal BaseQuantity
        {
            get
            {
                return this.baseQuantity;
            }

            set
            {
                this.baseQuantity = value;
            }
        }

        public decimal WeightQuantity
        {
            get
            {
                return this.weightQuantity;
            }

            set
            {
                this.weightQuantity = value;
            }
        }

        public SProductSchemeData SchemeData
        {
            get
            {
                return this.schemeData;
            }

            set
            {
                this.schemeData = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodRefno"></param>
        /// <returns></returns>
        public async Task<List<ProductVariant>> GetVariantList(int prodRefno)
        {
            SLocMap currLocMap = LocationMap.GetInstance().CurrLocMap;

            StringBuilder s = new StringBuilder();
            s.Append("SELECT tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
            s.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
            s.Append("tProductSchemeVariant.ProductSchemeVariantCode + \'  \' ");
            s.Append("+ tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDisplay ");
            s.Append("FROM tProduct ");
            s.Append("INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno ");
            s.Append("LEFT JOIN tProductSchemeVariantXRef ON tProductSchemeVariantXRef.ProductSchemeRefno = ");
            s.Append("tLocationMapWarehouseProductScheme.ProductSchemeRefno ");
            s.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = ");
            s.Append("tProductSchemeVariantXRef.ProductSchemeVariantRefno ");
            s.AppendFormat("WHERE tProduct.ProductRefno = {0}", prodRefno);
            s.AppendFormat(" AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = {0}", currLocMap.WhsLocMapRefno);
            s.Append(" ORDER BY tProductSchemeVariant.ProductSchemeVariantCode");

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductVariant>(s.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodRefno"></param>
        /// <returns></returns>
        public async Task<List<LotNumber>> GetLotList(int prodRefno)
        {
            // TODO Put a number of lot max check, if tProductSchemeLotNumber is > 1000 only load the Detail lots not the
            // tProductSchemeLotNumber the exception would be receiving. The supplier Lot needs to exist in the list for
            // a lookup match.
            StringBuilder s = new StringBuilder();
            s.Append("SELECT DISTINCT tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber ");
            s.Append("FROM tActivityDetail ");
            s.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
            s.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
            s.AppendFormat("WHERE tProduct.ProductRefno = {0} ", prodRefno);
            s.Append("UNION ");
            s.Append("SELECT tProductSchemeLotNumber.ProductSchemeLotNumberCode ");
            s.Append("AS ActivityDetailLogisticUnitBatchLotNumber ");
            s.Append("FROM tProductSchemeLotNumber ");
            s.Append("ORDER BY ActivityDetailLogisticUnitBatchLotNumber");

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<LotNumber>(s.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodRefno"></param>
        /// <returns></returns>
        public async Task<List<SerialNumber>> GetSerialNumberList(int prodRefno)
        {
            StringBuilder s = new StringBuilder();
            s.Append("SELECT DISTINCT tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber ");
            s.Append("FROM tActivityDetail ");
            s.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
            s.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
            s.AppendFormat("WHERE tProduct.ProductRefno = {0} ", prodRefno);
            s.Append("ORDER BY tActivityDetail.ActivityDetailSingleTradeItemSerialNumber");

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<SerialNumber>(s.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detailEdit"></param>
        /// <returns></returns>
        public XDocument GetXMLFromActivityDetailEdit(ActDetailEdit detailEdit)
        {
            bool isBatchPrintProductLabel = SysEnvironment.GetInstance().SysSettings.IsPrintProductLabelsAsBatch;

            XDocument xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<ser_refno>0</ser_refno>"
                + "<refno>0</refno>"
                + ("<serial>" + detailEdit.SchemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<pdate>" + detailEdit.SchemeData.LogisticUnitProductionDate + "</pdate>")
                + ("<lot_no>" + detailEdit.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<bdate>" + detailEdit.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
                + ("<variant_code>" + detailEdit.SchemeData.VariantCode + "</variant_code>")
                + ("<variant_no>" + detailEdit.SchemeData.VariantNumber + "</variant_no>")
                + ("<alt_qty>" + detailEdit.Quantity + "</alt_qty>")
                + ("<wt_qty>" + detailEdit.WeightQuantity + "</wt_qty>")
                + ("<base_qty>" + detailEdit.BaseQuantity + "</base_qty>")
                + ("<is_batch_print_product_label>" + isBatchPrintProductLabel + "</is_batch_print_product_label>")
                + ("<extra1>" + detailEdit.ActExtra.Text1 + "</extra1>")
                + ("<extra2>" + detailEdit.ActExtra.Text2 + "</extra2>")
                + ("<extra3>" + detailEdit.ActExtra.Text3 + "</extra3>")
                + ("<extra4>" + detailEdit.ActExtra.Text4 + "</extra4>")
                + ("<extra5>" + detailEdit.ActExtra.DateTime5 + "</extra5>")
                + "</params>");

            return xmlDoc;
        }

        #endregion
    }
}