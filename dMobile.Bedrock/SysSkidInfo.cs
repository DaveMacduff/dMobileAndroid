namespace dMobile.Bedrock
{
    using System;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    [Serializable]
    public class SysSkidInfo : ICloneable
    {
        #region Fields

        private SSkidInfo currSkid = new SSkidInfo();

        #endregion

        #region Properties

        public SSkidInfo CurrSkid
        {
            get
            {
                return this.currSkid;
            }

            set
            {
                this.currSkid = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Fill a new instance of SSkidInfo from xml
        /// </summary>
        /// <param name="data"></param>
        /// <returns>New instance of SSkidInfo</returns>
        /// <remarks></remarks>
        public static SSkidInfo Fill(XDocument data)
        {
            SSkidInfo skid = new SSkidInfo();
            skid.LogisticUnitSkidCode = data.Root.Element("logistic_unit_skid_code").Value;
            skid.SkidCode = data.Root.Element("skid_code").Value;
            int locMapRefno = int.Parse(data.Root.Element("location_map_refno").Value);
            skid.LocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
            int prodUnitRefno = int.Parse(data.Root.Element("product_unit_refno").Value);
            skid.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            skid.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(skid.ProdInfo.Refno);
            skid.Quantity = decimal.Parse(data.Root.Element("quantity").Value);
            skid.BaseQuantity = decimal.Parse(data.Root.Element("base_quantity").Value);
            skid.WeightQuantity = decimal.Parse(data.Root.Element("weight_quantity").Value);
            skid.IsCHEP = bool.Parse(data.Root.Element("chep").Value);
            skid.IsHold = bool.Parse(data.Root.Element("hold").Value);
            skid.Ti = int.Parse(data.Root.Element("ti").Value);
            skid.Hi = int.Parse(data.Root.Element("hi").Value);
            skid.Height = decimal.Parse(data.Root.Element("height").Value);
            skid.HeightUnit = data.Root.Element("height_unit").Value;
            skid.Cube = decimal.Parse(data.Root.Element("cube").Value);
            skid.CubeUnit = data.Root.Element("cube_unit").Value;
            skid.SkidFKRefno = int.Parse(data.Root.Element("skid_fk_refno").Value);
            skid.IsCompleted = bool.Parse(data.Root.Element("completed").Value);
            skid.IsConsumed = bool.Parse(data.Root.Element("consumed").Value);
            skid.IsPutAway = bool.Parse(data.Root.Element("putaway").Value);
            SProductSchemeData schemeData = skid.SchemeData;
            schemeData.LogisticUnitBatchLotNumber = data.Root.Element("logistic_unit_batch_lot_number").Value;
            schemeData.LogisticUnitBestBeforeSellByDate
                = DateTime.Parse(data.Root.Element("logistic_unit_best_before_sell_by_date").Value);
            schemeData.LogisticUnitProductionDate = DateTime.Parse(data.Root.Element("logistic_unit_production_date").Value);
            schemeData.VariantNumber = int.Parse(data.Root.Element("product_scheme_variant_no").Value);
            schemeData.VariantCode = data.Root.Element("product_scheme_variant_code").Value;
            schemeData.VariantDescription = data.Root.Element("product_scheme_variant_description").Value;
            skid.SchemeData = schemeData;

            return skid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static XDocument SkidInfoToXML(SysSkidInfo skid)
        {
            XDocument xmlResult = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"), string.Empty,
                new XElement("params",
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
                  new XElement("logistic_unit_batch_lot_number", skid.CurrSkid.SchemeData.LogisticUnitBatchLotNumber),
                  new XElement(
                      "logistic_unit_best_before_sell_by_date",
                      skid.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate),
                  new XElement("logistic_unit_production_date", skid.CurrSkid.SchemeData.LogisticUnitProductionDate),
                  new XElement("product_scheme_variant_no", skid.CurrSkid.SchemeData.VariantNumber),
                  new XElement("product_scheme_variant_code", skid.CurrSkid.SchemeData.VariantCode),
                  new XElement("product_scheme_variant_description", skid.CurrSkid.SchemeData.VariantDescription)));

            return xmlResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SysSkidInfo GetSkidInfoFromXML(XDocument data)
        {
            SysSkidInfo skid = new SysSkidInfo();

            skid.CurrSkid = Fill(data);

            return skid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SkidInfo"></param>
        /// <remarks></remarks>
        public static SysSkidInfo Move(SysSkidInfo skid, SLocMap moveToLocMap)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string recvData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            SysSkidInfo newSkid = skid;

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"), string.Empty,
                new XElement("imclient",
                  new XElement("command",
                    new XElement("action", "SUBMIT MOVE SKID")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", moveToLocMap.WhsFKRefno),
                    new XElement("sect_refno", moveToLocMap.SectFKRefno),
                    new XElement("slot_refno", moveToLocMap.SlotFKRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument xmlResult = new XmlDocument();
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
                    int newLocMapRefno = skid.CurrSkid.LocMapRefno;
                    SSkidInfo tempSkidInfo = skid.CurrSkid;
                    tempSkidInfo.LocMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(
                        moveToLocMap.WhsFKRefno,
                        moveToLocMap.SectFKRefno,
                        moveToLocMap.SlotFKRefno,
                        ref newLocMapRefno);

                    tempSkidInfo.LocMapRefno = newLocMapRefno;
                    newSkid.CurrSkid = tempSkidInfo;
                }
            }

            return newSkid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SkidInfo"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static SysSkidInfo SendUpdate(SysSkidInfo skid)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string recvData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"), string.Empty,
                new XElement("imclient",
                  new XElement("command",
                    new XElement("action", "SUBMIT UPDATE SKID")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
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

            recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument xmlResult = new XmlDocument();
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

            return skid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skid"></param>
        /// <returns></returns>
        public static SysSkidInfo Complete(SysSkidInfo skid)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string recvData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            LocationMap locMap = LocationMap.GetInstance();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"), string.Empty,
                new XElement("imclient",
                  new XElement("command",
                    new XElement("action", "SUBMIT COMPLETE SKID")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument xmlResult = new XmlDocument();
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
                    SSkidInfo tempSkidInfo = skid.CurrSkid;
                    tempSkidInfo.IsCompleted = true;
                    skid.CurrSkid = tempSkidInfo;
                }
            }

            return skid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skid"></param>
        /// <returns></returns>
        public static void PrintLabel(SysSkidInfo skid)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            LocationMap locMap = LocationMap.GetInstance();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"), string.Empty,
                new XElement("imclient",
                  new XElement("command",
                    new XElement("action", "SUBMIT PRINT SKID LABEL")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
                    new XElement("revision_datetime", revision),
                    new XElement("stamp", current),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            string recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument result = new XmlDocument();
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataSet GetSkids()
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string recvData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            LocationMap locMap = LocationMap.GetInstance();
            DataSet dataSetSkids = null;

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement("imclient",
                  new XElement("command", new XElement("action", "REQUEST SKIDS")),
                  new XElement("device_id", deviceID),
                  new XElement("login_id", loginID),
                  new XElement("revision_datetime", revision),
                  new XElement("stamp", current)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument result = new XmlDocument();
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
                    using (DataSet ds = new DataSet())
                    {
                        ds.ReadXml(SysCommon.GetXmlReader(ref recvData), XmlReadMode.Auto);
                        if (ds.Tables.Count != 0)
                        {
                            dataSetSkids = ds;
                        }
                    }
                }
            }

            return dataSetSkids;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skid"></param>
        /// <returns></returns>
        /// <remarks>
        /// DataSet returned contains a table called 'cskiddetail'. Lower case table name.
        /// All fields are strings because of the xml with no schema.
        /// </remarks>
        public static void GetSkidDetail(SysSkidInfo skid, ref DataSet dataSetSkidDetail)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string recvData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string current = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            LocationMap locMap = LocationMap.GetInstance();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement("imclient",
                  new XElement("command", new XElement("action", "REQUEST SKID DETAIL")),
                  new XElement("device_id", deviceID),
                  new XElement("login_id", loginID),
                  new XElement("revision_datetime", revision),
                  new XElement("stamp", current),
                  new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno),
                  new XElement("skid_fk_refno", skid.CurrSkid.SkidFKRefno),
                  new XElement("is_negate_qty", "false")));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                if (recvData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument result = new XmlDocument();
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
                    using (DataSet ds = new DataSet())
                    {
                        ds.ReadXml(SysCommon.GetXmlReader(ref recvData), XmlReadMode.Auto);
                        if (ds.Tables.Count != 0)
                        {
                            dataSetSkidDetail = ds;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlParams"></param>
        /// <remarks>
        /// parameter xml document structure
        /// <logistic_unit_skid_code>SkidInfo.CurrSkidInfo.strLogisticUnitSkidCode</logistic_unit_skid_code>
        /// <skid_fk_refno>SkidInfo.CurrSkidInfo.iSkidFKRefno</skid_fk_refno>
        /// <chep>SkidInfo.CurrSkidInfo.IsCHEP</chep>
        /// <hold>SkidInfo.CurrSkidInfo.IsHold</hold>
        /// <base_quantity>SkidInfo.CurrSkidInfo.decBaseQuantity</base_quantity>
        /// <cube>SkidInfo.CurrSkidInfo.decCube</cube>
        /// <height>SkidInfo.CurrSkidInfo.decHeight</height>
        /// <quantity>SkidInfo.CurrSkidInfo.decQuantity</quantity>
        /// <weight_quantity>SkidInfo.CurrSkidInfo.decWeightQuantity</weight_quantity>
        /// <hi>SkidInfo.CurrSkidInfo.iHi</hi>
        /// <ti>SkidInfo.CurrSkidInfo.iTi</ti>
        /// <cube_unit>SkidInfo.CurrSkidInfo.strCubeUnit</cube_unit>
        /// <height_unit>SkidInfo.CurrSkidInfo.strHeightUnit</height_unit>
        /// <skid_code>SkidInfo.CurrSkidInfo.strSkidCode</skid_code>
        /// <location_map_refno>SkidInfo.CurrSkidInfo.iLocationMapRefno</location_map_refno>
        /// <product_unit_refno>SkidInfo.CurrSkidInfo.oProductInfo.prodUnitRefno</product_unit_refno>
        /// <completed>SkidInfo.CurrSkidInfo.IsCompleted</completed>
        /// <putaway>SkidInfo.CurrSkidInfo.IsPutAway</putaway>
        /// </remarks>
        public static SysSkidInfo Create(XDocument skidData)
        {
            SysSkidInfo skid = new SysSkidInfo();
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string recvData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            ProductInfo prodInfo = ProductInfoTasks.GetInfoFromUnitRefno(
                int.Parse(skidData.Document.Root.Element("product_unit_refno").Value));

            SLocMap skidLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(
                int.Parse(skidData.Document.Root.Element("location_map_refno").Value));

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement("imclient",
                  new XElement("command",
                    new XElement("action", "SUBMIT CREATE SKID")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("chep", skidData.Document.Root.Element("chep").Value),
                    new XElement("hold", skidData.Document.Root.Element("hold").Value),
                    new XElement("base_quantity", skidData.Document.Root.Element("base_quantity").Value),
                    new XElement("cube", skidData.Document.Root.Element("cube").Value),
                    new XElement("height", skidData.Document.Root.Element("height").Value),
                    new XElement("quantity", skidData.Document.Root.Element("quantity").Value),
                    new XElement("weight_quantity", skidData.Document.Root.Element("weight_quantity").Value),
                    new XElement("hi", skidData.Document.Root.Element("hi").Value),
                    new XElement("ti", skidData.Document.Root.Element("ti").Value),
                    new XElement("cube_unit", skidData.Document.Root.Element("cube_unit").Value),
                    new XElement("height_unit", skidData.Document.Root.Element("height_unit").Value),
                    new XElement("logistic_unit_skid_code", skidData.Document.Root.Element("logistic_unit_skid_code").Value),
                    new XElement("skid_code", skidData.Document.Root.Element("skid_code").Value),
                    new XElement("whs_id", skidLocMap.WhsFKRefno),
                    new XElement("sect_refno", skidLocMap.SectFKRefno),
                    new XElement("slot_refno", skidLocMap.SlotFKRefno),
                    new XElement("product_id", prodInfo.Code),
                    new XElement("product_unit_bc", prodInfo.UnitBC),
                    new XElement("product_unit_code", prodInfo.UnitCode),
                    new XElement("completed", skid.CurrSkid.IsCompleted),
                    new XElement("putaway", skidData.Document.Root.Element("putaway").Value)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            recvData = SysCommon.SubmitComm(xmlString.ToString());
            if (recvData.Length > 0)
            {
                XmlDocument result = new XmlDocument();
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
                    skid = SysSkidInfo.GetSkidInfoFromXML(skidData);
                    SSkidInfo tempSkidInfo = skid.CurrSkid;
                    tempSkidInfo.SkidFKRefno = int.Parse(result.DocumentElement["skid_fk_refno"].InnerText);
                    tempSkidInfo.SkidCode = result.DocumentElement["skid_code"].InnerText;
                    tempSkidInfo.LogisticUnitSkidCode = result.DocumentElement["logistic_unit_skid_code"].InnerText;
                    skid.CurrSkid = tempSkidInfo;
                }
            }

            return skid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SkidInfo"></param>
        /// <remarks></remarks>
        public static void Break(SysSkidInfo skid)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string data = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            LocationMap locMap = LocationMap.GetInstance();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement("imclient",
                  new XElement("command",
                    new XElement("action", "SUBMIT BREAK SKID")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("skid_data", skid.CurrSkid.SkidCode),
                    new XElement("whs_id", locMap.CurrLocMap.WhsFKRefno)));

            request.Save(writer);
            writer.Flush();
            writer.Close();

            data = SysCommon.SubmitComm(xmlString.ToString());
            if (data.Length > 0)
            {
                if (data.Contains("<message>ERROR</message>"))
                {
                    XmlDocument xmlResult = new XmlDocument();
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
                    SSkidInfo tempSkidInfo = skid.CurrSkid;
                    tempSkidInfo.IsConsumed = true;
                    skid.CurrSkid = tempSkidInfo;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidData"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static SysSkidInfo GetSkidInfoFromBarCodeOrCode(string skidData)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = sysEnv.DeviceID;
            string revDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string loginID = sysEnv.ContactLog.LoginID.ToString();
            string currDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement("imclient",
                  new XElement("command",
                    new XElement("action", "REQUEST SKID INFO")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
                    new XElement("revision_datetime", revDateTime),
                    new XElement("stamp", currDateTime),
                    new XElement("whs_id", LocationMap.GetInstance().CurrLocMap.WhsFKRefno),
                    new XElement("skid_data", skidData)));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return RequestGetSkidInfo(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static SysSkidInfo DeepCopy(SysSkidInfo original)
        {
            XmlSerializer serializer = null;
            serializer = new XmlSerializer(original.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, original);
            stream.Position = 0;

            return (SysSkidInfo)serializer.Deserialize(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public object ShallowCopy()
        {
            return (SysSkidInfo)MemberwiseClone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return this.ShallowCopy();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlRequest"></param>
        /// <returns></returns>
        private static SysSkidInfo RequestGetSkidInfo(XDocument request)
        {
            SysSkidInfo skid = new SysSkidInfo();
            string recvData = SysCommon.SubmitComm(request.ToString());
            if (recvData.Length > 0)
            {
                XmlDocument result = new XmlDocument();
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

                SSkidInfo tmpInfo = new SSkidInfo();
                tmpInfo.SkidFKRefno = int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidfkrefno").InnerText);
                tmpInfo.SkidCode = result.SelectSingleNode("/VFPData/cskidinfo/skidcode").InnerText;
                tmpInfo.LogisticUnitSkidCode = result.SelectSingleNode("/VFPData/cskidinfo/logisticunitskidcode").InnerText;

                int locMapRefno = 0;

                tmpInfo.LocMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(
                    int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/locationmapwarehousefkrefno").InnerText),
                    int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/locationmapsectionfkrefno").InnerText),
                    int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/locationmapslotfkrefno").InnerText),
                    ref locMapRefno);

                tmpInfo.LocMapRefno = locMapRefno;

                tmpInfo.ProdInfo = new ProductInfo();
                tmpInfo.ProdInfo = ProductInfoTasks.GetInfoFromCodeBCUnit(
                    result.SelectSingleNode("/VFPData/cskidinfo/productcode").InnerText,
                    result.SelectSingleNode("/VFPData/cskidinfo/productunitbc").InnerText,
                    result.SelectSingleNode("/VFPData/cskidinfo/unitcode").InnerText);

                tmpInfo.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(tmpInfo.ProdInfo.Refno);

                tmpInfo.Quantity = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidquantity").InnerText);
                tmpInfo.BaseQuantity = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidbasequantity").InnerText);

                tmpInfo.WeightQuantity = decimal.Parse(
                    result.SelectSingleNode("/VFPData/cskidinfo/skidweightquantity").InnerText);

                tmpInfo.IsCHEP = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidischep").InnerText);
                tmpInfo.IsHold = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidisonhold").InnerText);
                tmpInfo.Ti = int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidti").InnerText);
                tmpInfo.Hi = int.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidhi").InnerText);
                tmpInfo.Height = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidheight").InnerText);
                tmpInfo.HeightUnit = result.SelectSingleNode("/VFPData/cskidinfo/skidheightunit").InnerText;
                tmpInfo.Cube = decimal.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidcube").InnerText);
                tmpInfo.CubeUnit = result.SelectSingleNode("/VFPData/cskidinfo/skidcubeunit").InnerText;
                tmpInfo.IsCompleted = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidiscompleted").InnerText);
                tmpInfo.IsConsumed = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidisconsumed").InnerText);
                tmpInfo.IsPutAway = bool.Parse(result.SelectSingleNode("/VFPData/cskidinfo/skidisputaway").InnerText);

                SProductSchemeData schemeData = tmpInfo.SchemeData;
                schemeData.LogisticUnitBatchLotNumber
                    = result.SelectSingleNode("/VFPData/cskidinfo/activitydetaillogisticunitbatchlotnumber").InnerText;

                string bb = result.SelectSingleNode(
                    "//VFPData/cskidinfo/activitydetaillogisticunitbestbeforesellbydate").InnerText;
                if (string.IsNullOrEmpty(bb))
                {
                    schemeData.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
                }
                else
                {
                    schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(bb);
                }

                string pd = result.SelectSingleNode("//VFPData/cskidinfo/activitydetaillogisticunitproductiondate").InnerText;
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
                tmpInfo.SchemeData = schemeData;

                skid.CurrSkid = tmpInfo;
            }

            return skid;
        }

        #endregion
    }
}
