// <copyright file="clsCommon.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Detail;
using dMobile.Controller.Activity_Item;

namespace dMobile.Controller
{
    using System;
    using System.Reflection;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    /// <summary>
    /// Common shared methods.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Fill product scheme data from XDocument data.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Scheme data.</param>
        /// <returns><see cref="SProductSchemeData"/> Product scheme data.</returns>
        public static SProductSchemeData FillSProductSchemeDataFromXML(XDocument data)
        {
            var schemeData = new SProductSchemeData
            {
                LogisticUnitBestBeforeSellByDate = DateTime.Parse(data.Root.Element("bdate").Value),
                LogisticUnitDueDate = DateTime.Now,
                LogisticUnitExpiryUseByDate = DateTime.Now,
                LogisticUnitPackagingDate = DateTime.Now,
                LogisticUnitProductionDate = DateTime.Parse(data.Root.Element("pdate").Value),
                LogisticUnitBatchLotNumber = data.Root.Element("lot_no").Value,
                SingleTradeItemSerialNumber = data.Root.Element("serial").Value,
                VariantCode = data.Root.Element("variant_code").Value,
                VariantDescription = string.Empty,
                VariantNumber = int.Parse(data.Root.Element("variant_no").Value),
            };

            return schemeData;
        }

        /// <summary>
        /// Fill activity item scheme data from XDocument data.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Scheme data.</param>
        /// <returns><see cref="SActItemSchemeData"/> Activity item, product scheme data.</returns>
        public static SActItemSchemeData FillSActItemSchemeDataFromXML(XDocument data)
        {
            var actItemSchemeData = new SActItemSchemeData
            {
                ProdSchemeData = FillSProductSchemeDataFromXML(data),
                ActDetailBaseQuantity = decimal.Parse(data.Root.Element("base_qty").Value),
                ActDetailQuantity = decimal.Parse(data.Root.Element("alt_qty").Value),
                ActDetailWeightQuantity = decimal.Parse(data.Root.Element("wt_qty").Value),
            };

            var actDetailExtra = new ActDetailExtra
            {
                Text1 = data.Root.Element("extra1").Value,
                Text2 = data.Root.Element("extra2").Value,
                Text3 = data.Root.Element("extra3").Value,
                Text4 = data.Root.Element("extra4").Value,
                DateTime1 = DateTime.Parse(data.Root.Element("extra5").Value),
            };

            actItemSchemeData.ActDetailExtra = actDetailExtra;

            return actItemSchemeData;
        }

        /// <summary>
        /// Get the assembly version.
        /// </summary>
        /// <returns><see cref="string"/> Assembly version.</returns>
        public string GetAssemblyVersion()
        {
            var major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }
    }
}
