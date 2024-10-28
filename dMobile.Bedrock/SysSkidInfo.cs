// <copyright file="SysSkidInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using static LocationMapTasks;

    /// <summary>
    /// Defines the <see cref="SysSkidInfo" /> Skid information methods.
    /// </summary>
    [Serializable]
    public class SysSkidInfo : ICloneable, IEquatable<SysSkidInfo>
    {
        /// <summary>
        /// Gets or sets the CurrSkid.
        /// </summary>
        public SSkidInfo CurrSkid { get; set; } = new SSkidInfo();

        /// <summary>
        /// Make a skid XDocument from a skid information object.
        /// </summary>
        /// <param name="skid"><see cref="SysSkidInfo"/> Skid information.</param>
        /// <returns><see cref="XDocument"/> Created skid XDocument.</returns>
        public static XDocument SkidInfoToXml(SysSkidInfo skid)
        {
            var xmlResult = new XDocument(
                new XDeclaration(
                    "1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "params",
                    new XElement("skid_fk_refno", skid.CurrSkid.SkidFkRefno),
                    new XElement("chep", skid.CurrSkid.IsChep),
                    new XElement("hold", skid.CurrSkid.IsHold),
                    new XElement("base_quantity", skid.CurrSkid.BaseQuantity),
                    new XElement("cube", skid.CurrSkid.Cube),
                    new XElement("height", skid.CurrSkid.Height),
                    new XElement("quantity", skid.CurrSkid.Quantity),
                    new XElement("weight_quantity", skid.CurrSkid.WeightQuantity),
                    new XElement("hi", skid.CurrSkid.Hi),
                    new XElement("ti", skid.CurrSkid.Ti),
                    new XElement("cube_unit", skid.CurrSkid.CubeUnit),
                    new XElement("height_unit", skid.CurrSkid.HeightUnit),
                    new XElement("logistic_unit_skid_code", skid.CurrSkid.LogisticUnitSkidCode),
                    new XElement("skid_code", skid.CurrSkid.SkidCode),
                    new XElement("location_map_refno", skid.CurrSkid.LocMapRefno),
                    new XElement("whs_id", skid.CurrSkid.LocMap.WhsFkRefno),
                    new XElement("sect_refno", skid.CurrSkid.LocMap.SectFkRefno),
                    new XElement("slot_refno", skid.CurrSkid.LocMap.SlotFkRefno),
                    new XElement("product_unit_refno", skid.CurrSkid.ProdInfo.UnitRefno),
                    new XElement("product_id", skid.CurrSkid.ProdInfo.Code),
                    new XElement("product_unit_bc", skid.CurrSkid.ProdInfo.UnitBc),
                    new XElement("product_unit_code", skid.CurrSkid.ProdInfo.UnitCode),
                    new XElement("completed", skid.CurrSkid.IsCompleted),
                    new XElement("consumed", skid.CurrSkid.IsConsumed),
                    new XElement("putaway", skid.CurrSkid.IsPutAway),
                    new XElement(
                        "logistic_unit_batch_lot_number",
                        skid.CurrSkid.SchemeData.LogisticUnitBatchLotNumber),
                    new XElement(
                        "logistic_unit_best_before_sell_by_date",
                        skid.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate),
                    new XElement(
                        "logistic_unit_production_date",
                        skid.CurrSkid.SchemeData.LogisticUnitProductionDate),
                    new XElement("product_scheme_variant_no", skid.CurrSkid.SchemeData.VariantNumber),
                    new XElement("product_scheme_variant_code", skid.CurrSkid.SchemeData.VariantCode),
                    new XElement(
                        "product_scheme_variant_description",
                        skid.CurrSkid.SchemeData.VariantDescription)));

            return xmlResult;
        }

        /// <summary>
        /// Convert xml skid information into a skid information object.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Skid information.</param>
        /// <returns><see cref="SysSkidInfo"/> Create skid information object.</returns>
        public static async Task<SysSkidInfo> GetSkidInfoFromXml(XDocument data)
        {
            var skid = new SysSkidInfo
            {
                CurrSkid = await FillSkidInfo(data)
            };
            return skid;

            //////////////////////////////////////////////////////////////////////////
            static async Task<SSkidInfo> FillSkidInfo(XDocument data)
            {
                if (!(data is { Root: { } }))
                {
                    throw new ArgumentNullException(nameof(data));
                }

                var skid = new SSkidInfo
                {
                    SkidFkRefno = int.Parse(data.Root.Element("skid_fk_refno") !.Value),
                    LogisticUnitSkidCode = data.Root.Element("logistic_unit_skid_code") !.Value,
                    SkidCode = data.Root.Element("skid_code") !.Value,
                };

                // Location.
                var locMapRefno = int.Parse(data.Root.Element("location_map_refno") !.Value);
                skid.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                // Product unit.
                var prodUnitRefno = int.Parse(data.Root.Element("product_unit_refno") !.Value);
                skid.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                skid.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(skid.ProdInfo.Refno);

                // Quantity
                skid.Quantity = decimal.Parse(data.Root.Element("quantity") !.Value);
                skid.BaseQuantity = decimal.Parse(data.Root.Element("base_quantity") !.Value);
                skid.WeightQuantity = decimal.Parse(data.Root.Element("weight_quantity") !.Value);

                // Attributes.
                skid.IsChep = bool.Parse(data.Root.Element("chep") !.Value);
                skid.IsHold = bool.Parse(data.Root.Element("hold") !.Value);
                skid.Ti = int.Parse(data.Root.Element("ti") !.Value);
                skid.Hi = int.Parse(data.Root.Element("hi") !.Value);
                skid.Height = decimal.Parse(data.Root.Element("height") !.Value);
                skid.HeightUnit = data.Root.Element("height_unit") !.Value;
                skid.Cube = decimal.Parse(data.Root.Element("cube") !.Value);
                skid.CubeUnit = data.Root.Element("cube_unit") !.Value;
                skid.IsCompleted = bool.Parse(data.Root.Element("completed") !.Value);
                skid.IsConsumed = bool.Parse(data.Root.Element("consumed") !.Value);
                skid.IsPutAway = bool.Parse(data.Root.Element("putaway") !.Value);

                // Default inventory detail, used with creating new skids.
                var schemeData = skid.SchemeData;
                schemeData.LogisticUnitBatchLotNumber = data.Root.Element("logistic_unit_batch_lot_number") !.Value;
                schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(
                    data.Root.Element("logistic_unit_best_before_sell_by_date") !.Value);
                schemeData.LogisticUnitProductionDate = DateTime.Parse(
                    data.Root.Element("logistic_unit_production_date") !.Value);
                schemeData.VariantNumber = int.Parse(data.Root.Element("product_scheme_variant_no") !.Value);
                schemeData.VariantCode = data.Root.Element("product_scheme_variant_code") !.Value;
                schemeData.VariantDescription = data.Root.Element("product_scheme_variant_description") !.Value;
                skid.SchemeData = schemeData;
                return skid;
            }
        }

        /// <summary>
        /// Move a skid.
        /// </summary>
        /// <param name="skid"><see cref="SysSkidInfo"/> Skid information to move.</param>
        /// <param name="moveToLocMap"><see cref="SLocMap"/> Destination location map.</param>
        /// <returns><see cref="Task{SysSkidInfo}"/> Moved updated skid information.</returns>
        public static async Task<SysSkidInfo> Move(SysSkidInfo skid, SLocMap moveToLocMap)
        {
            CreateMoveRequest(skid, moveToLocMap, out var xmlString);
            PerformMoveRequest(xmlString);
            await UpdateSkidWithNewLocation(skid, moveToLocMap);
            return skid;

            //////////////////////////////////////////////////////////////////////////
            static void CreateMoveRequest(
                SysSkidInfo skid,
                SLocMap moveToLocMap,
                out StringBuilder xmlString)
            {
                var sysEnv = SysEnvironment.GetInstance();
                xmlString = new StringBuilder();
                var writer = XmlWriter.Create(xmlString);
                var revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                var current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

                var request = new XDocument(
                    new XDeclaration("1.0", "utf-16", "yes"),
                    string.Empty,
                    new XElement(
                        "imclient",
                        new XElement("command", new XElement("action", "SUBMIT MOVE SKID")),
                        new XElement("device_id", sysEnv.DeviceId),
                        new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                        new XElement("revision_datetime", revision),
                        new XElement("stamp", current),
                        new XElement("skid_data", skid.CurrSkid.SkidCode),
                        new XElement("whs_id", moveToLocMap.WhsFkRefno),
                        new XElement("sect_refno", moveToLocMap.SectFkRefno),
                        new XElement("slot_refno", moveToLocMap.SlotFkRefno)));

                request.Save(writer);
                writer.Flush();
                writer.Close();
            }

            //////////////////////////////////////////////////////////////////////////
            static async Task UpdateSkidWithNewLocation(SysSkidInfo skid, SLocMap moveToLocMap)
            {
                var tempSkidInfo = skid.CurrSkid;
                var sLocMapAndRefno = await GetSLocMapFromLocMapFkRefno(
                    moveToLocMap.WhsFkRefno,
                    moveToLocMap.SectFkRefno,
                    moveToLocMap.SlotFkRefno);
                tempSkidInfo.LocMap = sLocMapAndRefno.SLocMap;
                skid.CurrSkid = tempSkidInfo;
            }

            //////////////////////////////////////////////////////////////////////////
            static void PerformMoveRequest(StringBuilder xmlString)
            {
                _ = SysCommon.SubmitComm(xmlString.ToString());
            }
        }

        /// <summary>
        /// Send skid update request.
        /// </summary>
        /// <param name="skid"><see cref="SysSkidInfo"/> Skid information to send.</param>
        public static void SendUpdate(SysSkidInfo skid)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "SUBMIT UPDATE SKID")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("chep", skid.CurrSkid.IsChep),
                    new XElement("hold", skid.CurrSkid.IsHold),
                    new XElement("base_quantity", skid.CurrSkid.BaseQuantity),
                    new XElement("cube", skid.CurrSkid.Cube),
                    new XElement("height", skid.CurrSkid.Height),
                    new XElement("quantity", skid.CurrSkid.Quantity),
                    new XElement("weight_quantity", skid.CurrSkid.WeightQuantity),
                    new XElement("hi", skid.CurrSkid.Hi),
                    new XElement("ti", skid.CurrSkid.Ti),
                    new XElement("cube_unit", skid.CurrSkid.CubeUnit),
                    new XElement("height_unit", skid.CurrSkid.HeightUnit),
                    new XElement("logistic_unit_skid_code", skid.CurrSkid.LogisticUnitSkidCode),
                    new XElement("skid_code", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", skid.CurrSkid.LocMap.WhsFkRefno),
                    new XElement("sect_refno", skid.CurrSkid.LocMap.SectFkRefno),
                    new XElement("slot_refno", skid.CurrSkid.LocMap.SlotFkRefno),
                    new XElement("product_id", skid.CurrSkid.ProdInfo.Code),
                    new XElement("product_unit_bc", skid.CurrSkid.ProdInfo.UnitBc),
                    new XElement("product_unit_code", skid.CurrSkid.ProdInfo.UnitCode),
                    new XElement("completed", skid.CurrSkid.IsCompleted),
                    new XElement("skid_fk_refno", skid.CurrSkid.SkidFkRefno),
                    new XElement("putaway", skid.CurrSkid.IsPutAway)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            _ = SysCommon.SubmitComm(xmlString.ToString());
        }

        /// <summary>
        /// Request to mark the skid as complete.
        /// </summary>
        /// <param name="skid"><see cref="SysSkidInfo"/> Skid information to mark complete.</param>
        /// <returns><see cref="SysSkidInfo"/> Skid marked as completed.</returns>
        public static SysSkidInfo Complete(SysSkidInfo skid)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var locMap = LocationMap.GetInstance();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "SUBMIT COMPLETE SKID")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFkRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            _ = SysCommon.SubmitComm(xmlString.ToString());
            var tempSkidInfo = skid.CurrSkid;
            tempSkidInfo.IsCompleted = true;
            skid.CurrSkid = tempSkidInfo;

            return skid;
        }

        /// <summary>
        /// Print a skid label.
        /// </summary>
        /// <param name="skid"><see cref="SysSkidInfo"/> Skid information.</param>
        public static void PrintLabel(SysSkidInfo skid)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var locMap = LocationMap.GetInstance();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "SUBMIT PRINT SKID LABEL")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFkRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            _ = SysCommon.SubmitComm(xmlString.ToString());
        }

        /// <summary>
        /// Request list of available skids.
        /// </summary>
        /// <returns><see cref="DataSet"/> Data set of available skids.</returns>
        public static DataSet GetSkids()
        {
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            DataSet dataSetSkids = null;

            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "REQUEST SKIDS")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var responseData = SysCommon.SubmitComm(xmlString.ToString());
            using var ds = new DataSet();
            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (ds.Tables.Count != 0)
            {
                dataSetSkids = ds;
            }

            return dataSetSkids;
        }

        /// <summary>
        /// Request the skid detail.
        /// </summary>
        /// <param name="skid"><see cref="SysSkidInfo"/> Skid information.</param>
        /// <param name="dataSetSkidDetail"><see cref="DataSet"/> Skid detail.</param>
        public static void GetSkidDetail(SysSkidInfo skid, ref DataSet dataSetSkidDetail)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var locMap = LocationMap.GetInstance();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "REQUEST SKID DETAIL")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFkRefno),
                    new XElement("skid_fk_refno", skid.CurrSkid.SkidFkRefno),
                    new XElement("is_negate_qty", "false")));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var responseData = SysCommon.SubmitComm(xmlString.ToString());
            using var ds = new DataSet();
            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (ds.Tables.Count != 0)
            {
                dataSetSkidDetail = ds;
            }
        }

        /// <summary>
        /// Create a skid.
        /// </summary>
        /// <param name="skidData"><see cref="XDocument"/> Skid information data to create a skid from.</param>
        /// <returns><see cref="Task{SysSkidInfo}"/> Created skid information.</returns>
        public static async Task<SysSkidInfo> Create(XDocument skidData)
        {
            if (!(skidData is { Document: { } }))
            {
                throw new ArgumentNullException(nameof(skidData));
            }

            var root = skidData.Document.Root;
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(
                int.Parse(root!.Element("product_unit_refno") !.Value));

            var skidLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                int.Parse(root.Element("location_map_refno") !.Value));
            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "SUBMIT CREATE SKID")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("chep", root.Element("chep") !.Value),
                    new XElement("hold", root.Element("hold") !.Value),
                    new XElement("base_quantity", root.Element("base_quantity") !.Value),
                    new XElement("cube", root.Element("cube") !.Value),
                    new XElement("height", root.Element("height") !.Value),
                    new XElement("quantity", root.Element("quantity") !.Value),
                    new XElement("weight_quantity", root.Element("weight_quantity") !.Value),
                    new XElement("hi", root.Element("hi") !.Value),
                    new XElement("ti", root.Element("ti") !.Value),
                    new XElement("cube_unit", root.Element("cube_unit") !.Value),
                    new XElement("height_unit", root.Element("height_unit") !.Value),
                    new XElement("logistic_unit_skid_code", root.Element("logistic_unit_skid_code") !.Value),
                    new XElement("skid_code", root.Element("skid_code") !.Value),
                    new XElement("whs_id", skidLocMap.WhsFkRefno),
                    new XElement("sect_refno", skidLocMap.SectFkRefno),
                    new XElement("slot_refno", skidLocMap.SlotFkRefno),
                    new XElement("product_id", prodInfo.Code),
                    new XElement("product_unit_bc", prodInfo.UnitBc),
                    new XElement("product_unit_code", prodInfo.UnitCode),
                    new XElement("completed", root.Element("completed") !.Value),
                    new XElement("putaway", root.Element("putaway") !.Value)));

            request.Save(writer);
            await writer.FlushAsync();
            writer.Close();

            var responseData = SysCommon.SubmitComm(xmlString.ToString());
            var result = new XmlDocument();
            result.LoadXml(responseData);
            var skid = await GetSkidInfoFromXml(skidData);
            var tempSkidInfo = skid.CurrSkid;
            tempSkidInfo.SkidFkRefno = int.Parse(result.DocumentElement!["skid_fk_refno"] !.InnerText);
            tempSkidInfo.SkidCode = result.DocumentElement!["skid_code"] !.InnerText;
            tempSkidInfo.LogisticUnitSkidCode = result.DocumentElement!["logistic_unit_skid_code"] !.InnerText;
            skid.CurrSkid = tempSkidInfo;
            return skid;
        }

        /// <summary>
        /// Request to break the skid.
        /// </summary>
        /// <param name="skidInfo"><see cref="SSkidInfo"/> Skid to break.</param>
        public static void Break(SSkidInfo skidInfo)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var locMap = LocationMap.GetInstance();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "SUBMIT BREAK SKID")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("skid_data", skidInfo),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFkRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var data = SysCommon.SubmitComm(xmlString.ToString());
            if (data.Length > 0)
            {
                if (data.Contains("<message>ERROR</message>"))
                {
                    var xmlResult = new XmlDocument();
                    xmlResult.LoadXml(data);
                    if (data.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(xmlResult.DocumentElement!["detail"] !.InnerText);
                    }
                    else
                    {
                        throw new Exception(xmlResult.DocumentElement!["detail"] !.InnerText);
                    }
                }
                else
                {
                    var tempSkidInfo = skidInfo;
                    tempSkidInfo.IsConsumed = true;
                }
            }
            else
            {
                throw new MessageOnlyException("Request break skid failed. Check communications.");
            }
        }

        /// <summary>
        /// Get skid information from a skid bar code or skid code.
        /// </summary>
        /// <param name="skidData"><see cref="string"/> Skid bar code or skid code.</param>
        /// <returns><see cref="SysSkidInfo"/> Skid information found.</returns>
        [SuppressMessage("ReSharper", "StringLiteralTypo", Justification = "XML data elements")]
        public static async Task<SysSkidInfo> GetSkidInfoFromBarCodeOrCode(string skidData)
        {
            var request = await CreateSkidInfoRequest();
            var skid = new SysSkidInfo();
            var responseData = SysCommon.SubmitComm(request.ToString());
            if (responseData.Length > 0)
            {
                var result = new XmlDocument();
                result.LoadXml(responseData);
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var msg = result.DocumentElement?["detail"] !.InnerText;
                    if (responseData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(msg);
                    }
                    else
                    {
                        throw new Exception(msg);
                    }
                }

                skid.CurrSkid = await FillSkidInfo(result);
            }
            else
            {
                throw new MessageOnlyException("Request skid info failed. Check communications.");
            }

            return skid;

            //////////////////////////////////////////////////////////////////////////
            static async Task<SSkidInfo> FillSkidInfo(XmlDocument result)
            {
                result = result ?? throw new ArgumentNullException(nameof(result));

                var skidInfo = new SSkidInfo();
                var nodeName = @"/VFPData/cskidinfo/skidfkrefno";
                skidInfo.SkidFkRefno = int.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = @"/VFPData/cskidinfo/skidcode";
                skidInfo.SkidCode = result.SelectSingleNode(nodeName) !.InnerText;

                nodeName = "/VFPData/cskidinfo/logisticunitskidcode";
                skidInfo.LogisticUnitSkidCode = result.SelectSingleNode(nodeName) !.InnerText;

                nodeName = "/VFPData/cskidinfo/skidquantity";
                skidInfo.Quantity = decimal.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidbasequantity";
                skidInfo.BaseQuantity = decimal.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidweightquantity";
                skidInfo.WeightQuantity = decimal.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidischep";
                skidInfo.IsChep = bool.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidisonhold";
                skidInfo.IsHold = bool.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidti";
                skidInfo.Ti = int.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidhi";
                skidInfo.Hi = int.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidheight";
                skidInfo.Height = decimal.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidheightunit";
                skidInfo.HeightUnit = result.SelectSingleNode(nodeName) !.InnerText;

                nodeName = "/VFPData/cskidinfo/skidcube";
                skidInfo.Cube = decimal.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidcubeunit";
                skidInfo.CubeUnit = result.SelectSingleNode(nodeName) !.InnerText;

                nodeName = "/VFPData/cskidinfo/skidiscompleted";
                skidInfo.IsCompleted = bool.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidisconsumed";
                skidInfo.IsConsumed = bool.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                nodeName = "/VFPData/cskidinfo/skidisputaway";
                skidInfo.IsPutAway = bool.Parse(result.SelectSingleNode(nodeName) !.InnerText);

                const string whsNodeName = "/VFPData/cskidinfo/locationmapwarehousefkrefno";
                const string sectNodeName = "/VFPData/cskidinfo/locationmapsectionfkrefno";
                const string slotNodeName = "/VFPData/cskidinfo/locationmapslotfkrefno";
                var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                    int.Parse(result.SelectSingleNode(whsNodeName) !.InnerText),
                    int.Parse(result.SelectSingleNode(sectNodeName) !.InnerText),
                    int.Parse(result.SelectSingleNode(slotNodeName) !.InnerText));
                skidInfo.LocMap = sLocMapAndRefno.SLocMap;

                const string prodNodeName = "/VFPData/cskidinfo/productcode";
                const string unitBcNodeName = "/VFPData/cskidinfo/productunitbc";
                const string unitCodeNodeName = "/VFPData/cskidinfo/unitcode";
                skidInfo.ProdInfo = await ProductInfoTasks.GetInfoFromCodeBcUnit(
                    result.SelectSingleNode(prodNodeName) !.InnerText,
                    result.SelectSingleNode(unitBcNodeName) !.InnerText,
                    result.SelectSingleNode(unitCodeNodeName) !.InnerText);

                skidInfo.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(skidInfo.ProdInfo.Refno);
                skidInfo.ProdInfo.Unit2BaseUnitFactor = await ProductInfoTasks.GetUnitRelationFactor(skidInfo.ProdInfo);

                var schemeData = skidInfo.SchemeData;
                const string batchLotNodeName = "/VFPData/cskidinfo/activitydetaillogisticunitbatchlotnumber";
                schemeData.LogisticUnitBatchLotNumber = result.SelectSingleNode(batchLotNodeName) !.InnerText;

                const string bbNodeName = "/VFPData/cskidinfo/activitydetaillogisticunitbestbeforesellbydate";
                var bb = result.SelectSingleNode(bbNodeName) !.InnerText;
                schemeData.LogisticUnitBestBeforeSellByDate = string.IsNullOrEmpty(bb) ? new DateTime(1900, 1, 1)
                    : DateTime.Parse(bb);

                const string pdNodeName = "/VFPData/cskidinfo/activitydetaillogisticunitproductiondate";
                var pd = result.SelectSingleNode(pdNodeName) !.InnerText;
                schemeData.LogisticUnitProductionDate = string.IsNullOrEmpty(pd) ? new DateTime(1900, 1, 1)
                    : DateTime.Parse(pd);

                const string variantNodeName = "/VFPData/cskidinfo/productschemevariantno";
                schemeData.VariantNumber = int.Parse(result.SelectSingleNode(variantNodeName) !.InnerText);
                const string variantCodeNodeName = "/VFPData/cskidinfo/productschemevariantcode";
                schemeData.VariantCode = result.SelectSingleNode(variantCodeNodeName) !.InnerText;
                const string variantDescNodeName = "/VFPData/cskidinfo/productschemevariantdescription";
                schemeData.VariantDescription = result.SelectSingleNode(variantDescNodeName) !.InnerText;
                skidInfo.SchemeData = schemeData;

                return skidInfo;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<XDocument> CreateSkidInfoRequest()
            {
                var sysEnv = SysEnvironment.GetInstance();
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);

                var revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                var currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

                var requestInfo = new XDocument(
                    new XDeclaration("1.0", "utf-16", "yes"),
                    string.Empty,
                    new XElement(
                        "imclient",
                        new XElement("command", new XElement("action", "REQUEST SKID INFO")),
                        new XElement("device_id", sysEnv.DeviceId),
                        new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                        new XElement("revision_datetime", revDateTime),
                        new XElement("stamp", currDateTime),
                        new XElement("whs_id", LocationMap.GetInstance().CurrLocMap.WhsFkRefno),
                        new XElement("skid_data", skidData)));

                requestInfo.Save(xml);
                await xml.FlushAsync();
                xml.Close();
                return requestInfo;
            }
        }

        /// <summary>
        /// Perform a skid information deep copy.
        /// </summary>
        /// <param name="original"><see cref="SysSkidInfo"/> Original skid information.</param>
        /// <returns><see cref="SysSkidInfo"/> New copy of the original skid information.</returns>
        public static SysSkidInfo DeepCopy(SysSkidInfo original)
        {
            var serializer = new XmlSerializer(original.GetType());
            var stream = new MemoryStream();
            serializer.Serialize(stream, original);
            stream.Position = 0;

            return (SysSkidInfo)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Perform a skid information shallow copy.
        /// </summary>
        /// <returns><see cref="SysSkidInfo"/> New copy of the original skid information.</returns>
        public object ShallowCopy()
        {
            return (SysSkidInfo)this.MemberwiseClone();
        }

        /// <summary>
        /// The Clone.
        /// </summary>
        /// <returns><see cref="object"/> Shallow copy of this.</returns>
        object ICloneable.Clone()
        {
            return this.ShallowCopy();
        }

        /// <summary>
        ///     Check if an instance is the same as this instance.
        /// </summary>
        /// <param name="other"><see cref="SysSkidInfo" /> Skid information to compare.</param>
        /// <returns><see cref="bool" /> True if the instances are the same else false.</returns>
        public bool Equals(SysSkidInfo other)
        {
            return other != null && other.CurrSkid.Equals(this.CurrSkid);
        }
    }
}
