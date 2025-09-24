// <copyright file="SysShared.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using Bedrock;
    using Controller.Activity_Detail;
    using CustomPages;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    ///     Defines the <see cref="SysShared" />.
    /// </summary>
    public class SysShared : DMobileContentPage
    {
        /// <summary>
        ///     Activity type. Sometimes we need to know what type of activity we are working with.
        /// </summary>
        public enum ActivityType
        {
            /// <summary>
            ///     Activity type is a header.
            /// </summary>
            Header = 0,

            /// <summary>
            ///    Activity type is items.
            /// </summary>
            Items = 1,

            /// <summary>
            ///    Activity type is details.
            /// </summary>
            Details = 2,

            /// <summary>
            ///   Activity type is details edit.
            /// </summary>
            DetailsEdit = 3,

            /// <summary>
            ///   Activity type is header filter.
            /// </summary>
            HeaderFilter = 4,
        }

        /// <summary>
        /// Get XML data from the activity detail edit.
        /// </summary>
        /// <param name="detailEdit"><see cref="ActDetailEdit"/> Activity detail edit object.</param>
        /// <returns><see cref="XDocument"/> Activity detail edit XML data.</returns>
        public static Task<XDocument> GetXmlFromActivityDetailEdit(ActDetailEdit detailEdit)
        {
            var isBatchPrintProductLabel = SysEnvironment.GetInstance().SysSettings.IsPrintProductLabelsAsBatch;

            // Use XmlWriter and StringBuilder for faster, safer XML generation
            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = false, Indent = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("params");
                writer.WriteElementString("ser_refno", "0");
                writer.WriteElementString("refno", "0");
                writer.WriteElementString("serial", detailEdit.SchemeData.SingleTradeItemSerialNumber ?? string.Empty);
                writer.WriteElementString("pdate", detailEdit.SchemeData.LogisticUnitProductionDate.ToString("o"));
                writer.WriteElementString("lot_no", detailEdit.SchemeData.LogisticUnitBatchLotNumber ?? string.Empty);
                writer.WriteElementString("bdate", detailEdit.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("o"));
                writer.WriteElementString("variant_code", detailEdit.SchemeData.VariantCode ?? string.Empty);
                writer.WriteElementString("variant_no", detailEdit.SchemeData.VariantNumber.ToString());
                writer.WriteElementString("alt_qty", detailEdit.Quantity.ToString());
                writer.WriteElementString("wt_qty", detailEdit.WeightQuantity.ToString());
                writer.WriteElementString("base_qty", detailEdit.BaseQuantity.ToString());
                writer.WriteElementString("is_batch_print_product_label", isBatchPrintProductLabel.ToString());
                writer.WriteElementString("extra1", detailEdit.ActExtra.Text1 ?? string.Empty);
                writer.WriteElementString("extra2", detailEdit.ActExtra.Text2 ?? string.Empty);
                writer.WriteElementString("extra3", detailEdit.ActExtra.Text3 ?? string.Empty);
                writer.WriteElementString("extra4", detailEdit.ActExtra.Text4 ?? string.Empty);
                writer.WriteElementString("extra5", detailEdit.ActExtra.DateTime1.ToString("o"));
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }

            var xmlDoc = XDocument.Parse(sb.ToString());
            return Task.FromResult(xmlDoc);
        }

        /// <summary>
        /// Get XML data from the activity detail edit.
        /// </summary>
        /// <param name="detail"><see cref="ActDetail"/> Activity detail object.</param>
        /// <returns><see cref="XDocument"/> Activity detail XML data.</returns>
        public static Task<XDocument> GetXmlFromActivityDetailLabel(ActDetail detail)
        {
            var isBatchPrintProductLabel = SysEnvironment.GetInstance().SysSettings.IsPrintProductLabelsAsBatch;

            // Use XmlWriter and StringBuilder for efficient XML generation
            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = false, Indent = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("params");
                writer.WriteElementString("ser_refno", "0");
                writer.WriteElementString("refno", "0");
                writer.WriteElementString("serial", detail.LabelSchemeData.SingleTradeItemSerialNumber ?? string.Empty);
                writer.WriteElementString("pdate", detail.LabelSchemeData.LogisticUnitProductionDate.ToString("o"));
                writer.WriteElementString("lot_no", detail.LabelSchemeData.LogisticUnitBatchLotNumber ?? string.Empty);
                writer.WriteElementString("bdate", detail.LabelSchemeData.LogisticUnitBestBeforeSellByDate.ToString("o"));
                writer.WriteElementString("variant_code", detail.LabelSchemeData.VariantCode ?? string.Empty);
                writer.WriteElementString("variant_no", detail.LabelSchemeData.VariantNumber.ToString());
                writer.WriteElementString("alt_qty", detail.Quantity.ToString());
                writer.WriteElementString("wt_qty", detail.WeightQuantity.ToString());
                writer.WriteElementString("base_qty", detail.BaseQuantity.ToString());
                writer.WriteElementString("is_batch_print_product_label", isBatchPrintProductLabel.ToString());
                writer.WriteElementString("extra1", detail.ActExtra.Text1 ?? string.Empty);
                writer.WriteElementString("extra2", detail.ActExtra.Text2 ?? string.Empty);
                writer.WriteElementString("extra3", detail.ActExtra.Text3 ?? string.Empty);
                writer.WriteElementString("extra4", detail.ActExtra.Text4 ?? string.Empty);
                writer.WriteElementString("extra5", detail.ActExtra.DateTime1.ToString("o"));
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }

            var xmlDoc = XDocument.Parse(sb.ToString());
            return Task.FromResult(xmlDoc);
        }

        /// <summary>
        ///     The GetAssemblyVersion.
        /// </summary>
        /// <returns>.</returns>
        public static string GetAssemblyVersion()
        {
            var major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return $"{major}.{minor}.{build}.{revision}";
        }
    }
}