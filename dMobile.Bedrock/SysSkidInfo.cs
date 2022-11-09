// <copyright file="SysSkidInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using static dMobile.Bedrock.LocationMapTasks;

    /// <summary>
    /// Defines the <see cref="SysSkidInfo" /> Skid information methods.
    /// </summary>
    [Serializable]
    public class SysSkidInfo : ICloneable
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
        public static XDocument SkidInfoToXML(SysSkidInfo skid)
        {
            var xmlResult = new XDocument(
                new XDeclaration(
                    "1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "params",
                    new XElement("skid_fk_refno", skid.CurrSkid.SkidFKRefno),
                    new XElement("chep", skid.CurrSkid.IsCHEP),
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
                    new XElement("whs_id", skid.CurrSkid.LocMap.WhsFKRefno),
                    new XElement("sect_refno", skid.CurrSkid.LocMap.SectFKRefno),
                    new XElement("slot_refno", skid.CurrSkid.LocMap.SlotFKRefno),
                    new XElement("product_unit_refno", skid.CurrSkid.ProdInfo.UnitRefno),
                    new XElement("product_id", skid.CurrSkid.ProdInfo.Code),
                    new XElement("product_unit_bc", skid.CurrSkid.ProdInfo.UnitBC),
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
        public static async Task<SysSkidInfo> GetSkidInfoFromXML(XDocument data)
        {
            var skid = new SysSkidInfo();
            skid.CurrSkid = await FillSkidInfo(data);
            return skid;

            //////////////////////////////////////////////////////////////////////////
            static async Task<SSkidInfo> FillSkidInfo(XDocument data)
            {
                var skid = new SSkidInfo
                {

                    // Identifier information.
                    SkidFKRefno = int.Parse(data.Root.Element("skid_fk_refno").Value),
                    LogisticUnitSkidCode = data.Root.Element("logistic_unit_skid_code").Value,
                    SkidCode = data.Root.Element("skid_code").Value,
                };

                // Location.
                var locMapRefno = int.Parse(data.Root.Element("location_map_refno").Value);
                skid.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                // Product unit.
                var prodUnitRefno = int.Parse(data.Root.Element("product_unit_refno").Value);
                skid.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                skid.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(skid.ProdInfo.Refno);

                // Quantity
                skid.Quantity = decimal.Parse(data.Root.Element("quantity").Value);
                skid.BaseQuantity = decimal.Parse(data.Root.Element("base_quantity").Value);
                skid.WeightQuantity = decimal.Parse(data.Root.Element("weight_quantity").Value);

                // Attributes.
                skid.IsCHEP = bool.Parse(data.Root.Element("chep").Value);
                skid.IsHold = bool.Parse(data.Root.Element("hold").Value);
                skid.Ti = int.Parse(data.Root.Element("ti").Value);
                skid.Hi = int.Parse(data.Root.Element("hi").Value);
                skid.Height = decimal.Parse(data.Root.Element("height").Value);
                skid.HeightUnit = data.Root.Element("height_unit").Value;
                skid.Cube = decimal.Parse(data.Root.Element("cube").Value);
                skid.CubeUnit = data.Root.Element("cube_unit").Value;
                skid.IsCompleted = bool.Parse(data.Root.Element("completed").Value);
                skid.IsConsumed = bool.Parse(data.Root.Element("consumed").Value);
                skid.IsPutAway = bool.Parse(data.Root.Element("putaway").Value);

                // Default inventory detail, used with creating new skids.
                var schemeData = skid.SchemeData;
                schemeData.LogisticUnitBatchLotNumber =
                    data.Root.Element("logistic_unit_batch_lot_number").Value;
                schemeData.LogisticUnitBestBeforeSellByDate
                    = DateTime.Parse(data.Root.Element("logistic_unit_best_before_sell_by_date").Value);
                schemeData.LogisticUnitProductionDate = DateTime.Parse(
                    data.Root.Element("logistic_unit_production_date").Value);
                schemeData.VariantNumber = int.Parse(data.Root.Element("product_scheme_variant_no").Value);
                schemeData.VariantCode = data.Root.Element("product_scheme_variant_code").Value;
                schemeData.VariantDescription = data.Root.Element("product_scheme_variant_description").Value;
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
                        new XElement("device_id", sysEnv.DeviceID),
                        new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                        new XElement("revision_datetime", revision),
                        new XElement("stamp", current),
                        new XElement("skid_data", skid.CurrSkid.SkidCode),
                        new XElement("whs_id", moveToLocMap.WhsFKRefno),
                        new XElement("sect_refno", moveToLocMap.SectFKRefno),
                        new XElement("slot_refno", moveToLocMap.SlotFKRefno)));

                request.Save(writer);
                writer.Flush();
                writer.Close();
            }

            //////////////////////////////////////////////////////////////////////////
            static async Task UpdateSkidWithNewLocation(SysSkidInfo skid, SLocMap moveToLocMap)
            {
                var tempSkidInfo = skid.CurrSkid;
                var sLocMapAndRefno = await GetSLocMapFromLocMapFkRefno(
                    moveToLocMap.WhsFKRefno,
                    moveToLocMap.SectFKRefno,
                    moveToLocMap.SlotFKRefno);
                tempSkidInfo.LocMap = sLocMapAndRefno.SLocMap;
                skid.CurrSkid = tempSkidInfo;
            }

            //////////////////////////////////////////////////////////////////////////
            static void PerformMoveRequest(StringBuilder xmlString)
            {
                var recvData = SysCommon.SubmitComm(xmlString.ToString());
                if (recvData.Length <= 0)
                {
                    throw new MessageOnlyException("Move skid request failed. Check communications.");
                }

                if (recvData.Contains("<message>ERROR</message>"))
                {
                    var xmlResult = new XmlDocument();
                    xmlResult.LoadXml(recvData);
                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(xmlResult.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(xmlResult.DocumentElement["detail"].InnerText);
                    }
                }
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
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("chep", skid.CurrSkid.IsCHEP),
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
                    new XElement("whs_id", skid.CurrSkid.LocMap.WhsFKRefno),
                    new XElement("sect_refno", skid.CurrSkid.LocMap.SectFKRefno),
                    new XElement("slot_refno", skid.CurrSkid.LocMap.SlotFKRefno),
                    new XElement("product_id", skid.CurrSkid.ProdInfo.Code),
                    new XElement("product_unit_bc", skid.CurrSkid.ProdInfo.UnitBC),
                    new XElement("product_unit_code", skid.CurrSkid.ProdInfo.UnitCode),
                    new XElement("completed", skid.CurrSkid.IsCompleted),
                    new XElement("skid_fk_refno", skid.CurrSkid.SkidFKRefno),
                    new XElement("putaway", skid.CurrSkid.IsPutAway)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    var xmlResult = new XmlDocument();
                    xmlResult.LoadXml(recvData);
                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(xmlResult.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(xmlResult.DocumentElement["detail"].InnerText);
                    }
                }
            }
            else
            {
                throw new MessageOnlyException("Update skid request failed. Check communications.");
            }
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
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    var xmlResult = new XmlDocument();
                    xmlResult.LoadXml(recvData);

                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(xmlResult.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(xmlResult.DocumentElement["detail"].InnerText);
                    }
                }
                else
                {
                    var tempSkidInfo = skid.CurrSkid;
                    tempSkidInfo.IsCompleted = true;
                    skid.CurrSkid = tempSkidInfo;
                }
            }
            else
            {
                throw new MessageOnlyException("Complete skid request failed. Check communications.");
            }

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
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    var result = new XmlDocument();
                    result.LoadXml(recvData);
                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(result.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(result.DocumentElement["detail"].InnerText);
                    }
                }
            }
            else
            {
                throw new MessageOnlyException("Print skid label request failed. Check communications.");
            }
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
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    var result = new XmlDocument();
                    result.LoadXml(recvData);
                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(result.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(result.DocumentElement["detail"].InnerText);
                    }
                }
                else
                {
                    using var ds = new DataSet();
                    ds.ReadXml(SysCommon.GetXmlReader(ref recvData), XmlReadMode.Auto);
                    if (ds.Tables.Count != 0)
                    {
                        dataSetSkids = ds;
                    }
                }
            }
            else
            {
                throw new MessageOnlyException("Request available skids failed. Check communications.");
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
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno),
                    new XElement("skid_fk_refno", skid.CurrSkid.SkidFKRefno),
                    new XElement("is_negate_qty", "false")));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    var result = new XmlDocument();
                    result.LoadXml(recvData);
                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(result.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(result.DocumentElement["detail"].InnerText);
                    }
                }
                else
                {
                    using var ds = new DataSet();
                    ds.ReadXml(SysCommon.GetXmlReader(ref recvData), XmlReadMode.Auto);
                    if (ds.Tables.Count != 0)
                    {
                        dataSetSkidDetail = ds;
                    }
                }
            }
            else
            {
                throw new MessageOnlyException("Request skid detail failed. Check communications.");
            }
        }

        /// <summary>
        /// Create a skid.
        /// </summary>
        /// <param name="skidData"><see cref="XDocument"/> Skid information data to create a skid from.</param>
        /// <returns><see cref="Task{SysSkidInfo}"/> Created skid information.</returns>
        public static async Task<SysSkidInfo> Create(XDocument skidData)
        {
            var root = skidData.Document.Root;
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            using var writer = XmlWriter.Create(xmlString);
            var revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(
                int.Parse(root.Element("product_unit_refno").Value));

            var skidLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                int.Parse(root.Element("location_map_refno").Value));
            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "SUBMIT CREATE SKID")),
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("chep", root.Element("chep").Value),
                    new XElement("hold", root.Element("hold").Value),
                    new XElement("base_quantity", root.Element("base_quantity").Value),
                    new XElement("cube", root.Element("cube").Value),
                    new XElement("height", root.Element("height").Value),
                    new XElement("quantity", root.Element("quantity").Value),
                    new XElement("weight_quantity", root.Element("weight_quantity").Value),
                    new XElement("hi", root.Element("hi").Value),
                    new XElement("ti", root.Element("ti").Value),
                    new XElement("cube_unit", root.Element("cube_unit").Value),
                    new XElement("height_unit", root.Element("height_unit").Value),
                    new XElement("logistic_unit_skid_code", root.Element("logistic_unit_skid_code").Value),
                    new XElement("skid_code", root.Element("skid_code").Value),
                    new XElement("whs_id", skidLocMap.WhsFKRefno),
                    new XElement("sect_refno", skidLocMap.SectFKRefno),
                    new XElement("slot_refno", skidLocMap.SlotFKRefno),
                    new XElement("product_id", prodInfo.Code),
                    new XElement("product_unit_bc", prodInfo.UnitBC),
                    new XElement("product_unit_code", prodInfo.UnitCode),
                    new XElement("completed", root.Element("completed").Value),
                    new XElement("putaway", root.Element("putaway").Value)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            var recvData = SysCommon.SubmitComm(xmlString.ToString());

            SysSkidInfo skid;
            if (recvData.Length > 0)
            {
                var result = new XmlDocument();
                result.LoadXml(recvData);
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(result.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(result.DocumentElement["detail"].InnerText);
                    }
                }
                else
                {
                    skid = await SysSkidInfo.GetSkidInfoFromXML(skidData);
                    var tempSkidInfo = skid.CurrSkid;
                    tempSkidInfo.SkidFKRefno = int.Parse(result.DocumentElement["skid_fk_refno"].InnerText);
                    tempSkidInfo.SkidCode = result.DocumentElement["skid_code"].InnerText;
                    tempSkidInfo.LogisticUnitSkidCode = result.DocumentElement["logistic_unit_skid_code"].InnerText;
                    skid.CurrSkid = tempSkidInfo;
                }
            }
            else
            {
                throw new MessageOnlyException("Request create skid failed. Check communications.");
            }

            return skid;
        }

        /// <summary>
        /// request to break the skid.
        /// </summary>
        /// <param name="skid"><see cref="SysSkidInfo"/> Skid information to break.</param>
        public static void Break(SysSkidInfo skid)
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
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno)));

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
                        throw new MessageOnlyException(xmlResult.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(xmlResult.DocumentElement["detail"].InnerText);
                    }
                }
                else
                {
                    var tempSkidInfo = skid.CurrSkid;
                    tempSkidInfo.IsConsumed = true;
                    skid.CurrSkid = tempSkidInfo;
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
        public static async Task<SysSkidInfo> GetSkidInfoFromBarCodeOrCode(string skidData)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement("command", new XElement("action", "REQUEST SKID INFO")),
                    new XElement("device_id", sysEnv.DeviceID),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("whs_id", LocationMap.GetInstance().CurrLocMap.WhsFKRefno),
                    new XElement("skid_data", skidData)));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            var skid = new SysSkidInfo();
            var recvData = SysCommon.SubmitComm(request.ToString());
            if (recvData.Length > 0)
            {
                var result = new XmlDocument();
                result.LoadXml(recvData);
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    if (recvData.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(result.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        throw new Exception(result.DocumentElement["detail"].InnerText);
                    }
                }

                skid.CurrSkid = await FillSkidInfo(result);
            }
            else
            {
                throw new MessageOnlyException("Request skid info failed. Check communications.");
            }

            return skid;

            static async Task<SSkidInfo> FillSkidInfo(XmlDocument result)
            {
                var skidInfo = new SSkidInfo
                {
                    SkidFKRefno = int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidfkrefno").InnerText),
                    SkidCode = result.SelectSingleNode("/VFPData/cskidinfo/skidcode").InnerText,
                    LogisticUnitSkidCode = result.SelectSingleNode("/VFPData/cskidinfo/logisticunitskidcode").InnerText,
                    Quantity = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidquantity").InnerText),
                    BaseQuantity = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidbasequantity").InnerText),
                    WeightQuantity = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidweightquantity").InnerText),
                    IsCHEP = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidischep").InnerText),
                    IsHold = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidisonhold").InnerText),
                    Ti = int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidti").InnerText),
                    Hi = int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidhi").InnerText),
                    Height = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidheight").InnerText),
                    HeightUnit = result.SelectSingleNode("/VFPData/cskidinfo/skidheightunit").InnerText,
                    Cube = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidcube").InnerText),
                    CubeUnit = result.SelectSingleNode("/VFPData/cskidinfo/skidcubeunit").InnerText,
                    IsCompleted = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidiscompleted").InnerText),
                    IsConsumed = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidisconsumed").InnerText),
                    IsPutAway = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidisputaway").InnerText),
                };

                var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                    int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/locationmapwarehousefkrefno").InnerText),
                    int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/locationmapsectionfkrefno").InnerText),
                    int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/locationmapslotfkrefno").InnerText));

                skidInfo.LocMap = sLocMapAndRefno.SLocMap;

                skidInfo.ProdInfo = new ProductInfo();
                skidInfo.ProdInfo = await ProductInfoTasks.GetInfoFromCodeBCUnit(
                    result.SelectSingleNode("/VFPData/cskidinfo/productcode").InnerText,
                    result.SelectSingleNode("/VFPData/cskidinfo/productunitbc").InnerText,
                    result.SelectSingleNode("/VFPData/cskidinfo/unitcode").InnerText);

                skidInfo.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(skidInfo.ProdInfo.Refno);

                var schemeData = skidInfo.SchemeData;
                schemeData.LogisticUnitBatchLotNumber
                    = result.SelectSingleNode("/VFPData/cskidinfo/activitydetaillogisticunitbatchlotnumber").InnerText;

                var bb = result.SelectSingleNode(
                    "//VFPData/cskidinfo/activitydetaillogisticunitbestbeforesellbydate").InnerText;
                if (string.IsNullOrEmpty(bb))
                {
                    schemeData.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
                }
                else
                {
                    schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(bb);
                }

                var pd = result.SelectSingleNode("//VFPData/cskidinfo/activitydetaillogisticunitproductiondate").InnerText;
                if (string.IsNullOrEmpty(pd))
                {
                    schemeData.LogisticUnitProductionDate = new DateTime(1900, 1, 1);
                }
                else
                {
                    schemeData.LogisticUnitProductionDate = DateTime.Parse(pd);
                }

                schemeData.VariantNumber = int.Parse(
                    result.SelectSingleNode("//VFPData/cskidinfo/productschemevariantno").InnerText);
                schemeData.VariantCode = result.SelectSingleNode("//VFPData/cskidinfo/productschemevariantcode").InnerText;
                schemeData.VariantDescription
                    = result.SelectSingleNode("//VFPData/cskidinfo/productschemevariantdescription").InnerText;
                skidInfo.SchemeData = schemeData;

                return skidInfo;
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
        /// <param name="original"><see cref="SysSkidInfo"/> Original skid information.</param>
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
    }
}
