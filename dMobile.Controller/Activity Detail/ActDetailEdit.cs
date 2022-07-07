// <copyright file="ActDetailEdit.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Types;

    /// <summary>
    /// Defines the <see cref="ActDetailEdit" /> activity detail editing.
    /// </summary>
    public class ActDetailEdit
    {
        /// <summary>
        /// Gets or sets the activity detail extra data.
        /// </summary>
        public ActDetailExtra ActExtra { get; set; } = new ActDetailExtra();

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public decimal AltQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the base quantity.
        /// </summary>
        public decimal BaseQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the weight quantity.
        /// </summary>
        public decimal WeightQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product scheme data.
        /// </summary>
        public SProductSchemeData SchemeData { get; set; } = new SProductSchemeData();

        /// <summary>
        /// Get a list of scheme logistic unit product variants.
        /// </summary>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="Task{List(ProductVariant)}"/> List of logistic unit product variants.</returns>
        public async Task<List<ProductVariant>> GetVariantList(int prodRefno)
        {
            SLocMap currLocMap = LocationMap.GetInstance().CurrLocMap;

            StringBuilder query = new StringBuilder();
            query.Append("SELECT tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
            query.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
            query.Append("tProductSchemeVariant.ProductSchemeVariantCode + \'  \' ");
            query.Append("+ tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDisplay ");
            query.Append("FROM tProduct ");
            query.Append("INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno ");
            query.Append("LEFT JOIN tProductSchemeVariantXRef ON tProductSchemeVariantXRef.ProductSchemeRefno = ");
            query.Append("tLocationMapWarehouseProductScheme.ProductSchemeRefno ");
            query.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = ");
            query.Append("tProductSchemeVariantXRef.ProductSchemeVariantRefno ");
            query.AppendFormat("WHERE tProduct.ProductRefno = {0}", prodRefno);
            query.AppendFormat(" AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = {0}", currLocMap.WhsLocMapRefno);
            query.Append(" ORDER BY tProductSchemeVariant.ProductSchemeVariantCode");

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductVariant>(query.ToString());
        }

        /// <summary>
        /// Get a list of logistic unit batch lot numbers to suggest.
        /// </summary>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="Task{List(LotNumber)}"/> List of logistic unit batch lot numbers to suggest.</returns>
        public async Task<List<LotNumber>> GetLotList(int prodRefno)
        {
            // TODO: Put a number of lot max check, if tProductSchemeLotNumber is > 1000 only load the Detail lots not the
            // tProductSchemeLotNumber the exception would be receiving. The supplier Lot needs to exist in the list for
            // a lookup match.
            StringBuilder query = new StringBuilder();
            query.Append("SELECT DISTINCT tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber ");
            query.Append("FROM tActivityDetail ");
            query.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
            query.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
            query.AppendFormat("WHERE tProduct.ProductRefno = {0} ", prodRefno);
            query.Append("UNION ");
            query.Append("SELECT tProductSchemeLotNumber.ProductSchemeLotNumberCode ");
            query.Append("AS ActivityDetailLogisticUnitBatchLotNumber ");
            query.Append("FROM tProductSchemeLotNumber ");
            query.Append("ORDER BY ActivityDetailLogisticUnitBatchLotNumber");

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<LotNumber>(query.ToString());
        }

        /// <summary>
        /// Get a list of single trade item serial numbers.
        /// </summary>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns>.</returns>
        public async Task<List<SerialNumber>> GetSerialNumberList(int prodRefno)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT DISTINCT tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber ");
            query.Append("FROM tActivityDetail ");
            query.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
            query.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
            query.AppendFormat("WHERE tProduct.ProductRefno = {0} ", prodRefno);
            query.Append("ORDER BY tActivityDetail.ActivityDetailSingleTradeItemSerialNumber");

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<SerialNumber>(query.ToString());
        }

        /// <summary>
        /// Get XML data from the activity detail edit.
        /// </summary>
        /// <param name="detailEdit"><see cref="ActDetailEdit"/> Activity detail edit object.</param>
        /// <returns><see cref="XDocument"/> Activity detail edit XML data.</returns>
        public Task<XDocument> GetXMLFromActivityDetailEdit(ActDetailEdit detailEdit)
        {
            bool isBatchPrintProductLabel = SysEnvironment.GetInstance().SysSettings.IsPrintProductLabelsAsBatch;

            XDocument xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<ser_refno>0</ser_refno>"
                + "<refno>0</refno>"
                + "<serial>" + detailEdit.SchemeData.SingleTradeItemSerialNumber + "</serial>"
                + "<pdate>" + detailEdit.SchemeData.LogisticUnitProductionDate + "</pdate>"
                + "<lot_no>" + detailEdit.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                + "<bdate>" + detailEdit.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                + "<variant_code>" + detailEdit.SchemeData.VariantCode + "</variant_code>"
                + "<variant_no>" + detailEdit.SchemeData.VariantNumber + "</variant_no>"
                + "<alt_qty>" + detailEdit.AltQuantity + "</alt_qty>"
                + "<wt_qty>" + detailEdit.WeightQuantity + "</wt_qty>"
                + "<base_qty>" + detailEdit.BaseQuantity + "</base_qty>"
                + "<is_batch_print_product_label>" + isBatchPrintProductLabel + "</is_batch_print_product_label>"
                + "<extra1>" + detailEdit.ActExtra.Text1 + "</extra1>"
                + "<extra2>" + detailEdit.ActExtra.Text2 + "</extra2>"
                + "<extra3>" + detailEdit.ActExtra.Text3 + "</extra3>"
                + "<extra4>" + detailEdit.ActExtra.Text4 + "</extra4>"
                + "<extra5>" + detailEdit.ActExtra.DateTime1 + "</extra5>"
                + "</params>");

            return Task.FromResult(xmlDoc);
        }
    }
}
