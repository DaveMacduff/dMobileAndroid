namespace dMobile.Controller
{
    using System;
    using System.Reflection;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class Common
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SProductSchemeData FillSProductSchemeDataFromXML(XDocument data)
        {
            SProductSchemeData schemeData = new SProductSchemeData();
            schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(data.Root.Element("bdate").Value);
            schemeData.LogisticUnitDueDate = DateTime.Now;
            schemeData.LogisticUnitExpiryUseByDate = DateTime.Now;
            schemeData.LogisticUnitExpiryUseByDate = DateTime.Now;
            schemeData.LogisticUnitPackagingDate = DateTime.Now;
            schemeData.LogisticUnitProductionDate = DateTime.Parse(data.Root.Element("pdate").Value);
            schemeData.LogisticUnitBatchLotNumber = data.Root.Element("lot_no").Value;
            ////schemeData.LogisticUnitSkidNumber = data.Root.Element("skid_no").Value;
            schemeData.SingleTradeItemSerialNumber = data.Root.Element("serial").Value;
            schemeData.VariantCode = data.Root.Element("variant_code").Value;
            schemeData.VariantDescription = string.Empty;
            schemeData.VariantNumber = int.Parse(data.Root.Element("variant_no").Value);

            return schemeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SActItemSchemeData FillSActItemSchemeDataFromXML(XDocument data)
        {
            SActItemSchemeData actItemSchemeData = new SActItemSchemeData();
            actItemSchemeData.ProdSchemeData = FillSProductSchemeDataFromXML(data);
            actItemSchemeData.ActDetailBaseQuantity = decimal.Parse(data.Root.Element("base_qty").Value);
            actItemSchemeData.ActDetailQuantity = decimal.Parse(data.Root.Element("alt_qty").Value);
            actItemSchemeData.ActDetailWeightQuantity = decimal.Parse(data.Root.Element("wt_qty").Value);

            ActDetailExtra actDetailExtra = new ActDetailExtra();
            actDetailExtra.Text1 = data.Root.Element("extra1").Value;
            actDetailExtra.Text2 = data.Root.Element("extra2").Value;
            actDetailExtra.Text3 = data.Root.Element("extra3").Value;
            actDetailExtra.Text4 = data.Root.Element("extra4").Value;
            actDetailExtra.DateTime5 = DateTime.Parse(data.Root.Element("extra5").Value);
            actItemSchemeData.ActDetailExtra = actDetailExtra;

            return actItemSchemeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetAssemblyVersion()
        {
            int major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            int minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            int revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }
    }
}
