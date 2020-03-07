namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableProductUnitBarCode" />
    /// </summary>
    public class TableProductUnitBarCode : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductUnitBarCode"/> class.
        /// </summary>
        public TableProductUnitBarCode()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnitBarCode";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes.", true);

                using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT * FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    //cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                    using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        string reqData = string.Empty;
                        string family = string.Empty;

                        while (drFamily.Read())
                        {
                            // GC.Collect();
                            // GC.WaitForPendingFinalizers();

                            family = drFamily["FamilyCode"].ToString();

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family), true);

                            reqData = this.SendRequest(family);

                            if (0 < reqData.Length)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                this.SaveOpt(reqData);
                            }
                        }
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding product uom bar codes failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding product uom bar codes failed.", ex);
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tProductUnitBarCode";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product uom bar codes failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductUnitBarCode";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product uom bar codes.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product uom bar codes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="strFamilyCode">The strFamilyCode<see cref="string"/></param>
        /// <returns></returns>
        protected override string SendRequest(string strFamilyCode)
        {
            return this.SendComm(this.CreateRequest(strFamilyCode));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
        }

        /// <summary>
        /// The SaveOpt
        /// </summary>
        /// <param name="reqData"></param>
        protected void SaveOpt(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;
            bool refreshDataChangesOnly = SysEnv.RefreshOpt.RefreshDataChangesOnly;
            Dictionary<string, int> unitLookup = new Dictionary<string, int>();

            using (SQLiteCommand cmndProdUnitBar = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnitBarUpdate = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    int startIndex = reqData.IndexOf("<VFPData");
                    XmlReaderSettings sett = new XmlReaderSettings();
                    sett.CheckCharacters = false;
                    sett.CloseInput = false;
                    sett.ConformanceLevel = ConformanceLevel.Auto;
                    sett.IgnoreComments = true;
                    sett.IgnoreProcessingInstructions = true;
                    sett.IgnoreWhitespace = false;
                    sett.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                    sett.ValidationType = ValidationType.None;

                    using (StringReader stringReader = new StringReader(reqData.Substring(startIndex)))
                    using (XmlReader reader = XmlReader.Create(stringReader, sett))
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes.", false);
                        //cmndProdUnitBar.CommandText = "tProductUnitBarCode";
                        //cmndProdUnitBar.CommandType = CommandType.Text;
                        //cmndProdUnitBar.IndexName = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";

                        //cmndProd.CommandText = "tProduct";
                        //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                        //cmndProd.CommandType = CommandType.Text;

                        cmndUnit.CommandText = "SELECT * FROM tUnit";
                        cmndUnit.CommandType = CommandType.Text;
                        //cmndUnit.IndexName = "UQ_UnitOnUnitCode";

                        //cmndProdUnit.CommandText = "tProductUnit";
                        //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                        //cmndProdUnit.CommandType = CommandType.Text;

                        //using (SQLiteDataReader drProdUnitBar = cmndProdUnitBar.ExecuteReader())
                        //using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                        using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                        //using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                        //using (SqlCeResultSet rs = cmndProdUnitBar.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                            while (drUnit.Read())
                            {
                                unitLookup.Add(drUnit.GetString((int)SysCommon.UnitFields.UnitCode), Convert.ToInt32(drUnit.GetValue(Convert.ToInt32(SysCommon.UnitFields.UnitRefno))));
                            }
                        }

                        //SqlCeUpdatableRecord rec = rs.CreateRecord();
                        int prodRefno = 0;
                        int prodUnitRefno = 0;
                        int unitRefno = 0;
                        int recordCount = 0;
                        bool isProdUnitBarCodeFound = false;

                        string prodUnitBarCodeRaw = string.Empty;

                        // f01
                        bool prodUnitBarCodeIsBC = false;
                        // f02
                        int prodFKRefno = 0;
                        // f03
                        string unitCode = string.Empty;
                        // f04
                        string prodUnitBarCodeCode = string.Empty;
                        // f05
                        int prodUnitBarCodeSymbologyEnum = 0;
                        // f06
                        string prodUnitBarCodeModifier1 = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        while (!reader.EOF)
                        {
                            // fill one record
                            // we do this by looping until an end segment is found
                            bool recordFilled = false;
                            string elementName = string.Empty;
                            prodUnitBarCodeModifier1 = string.Empty; // not always present, so we must fill it
                            while (!recordFilled && reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    elementName = reader.Name;
                                }
                                else if (reader.NodeType == XmlNodeType.Text)
                                {
                                    switch (elementName)
                                    {
                                        case "f01":
                                            prodUnitBarCodeIsBC = reader.ReadContentAsBoolean();
                                            break;
                                        case "f02":
                                            prodFKRefno = reader.ReadContentAsInt();
                                            break;
                                        case "f03":
                                            unitCode = reader.Value;
                                            break;
                                        case "f04":
                                            prodUnitBarCodeCode = reader.Value;
                                            break;
                                        case "f05":
                                            prodUnitBarCodeSymbologyEnum = reader.ReadContentAsInt();
                                            break;
                                        case "f06":
                                            prodUnitBarCodeModifier1 = reader.Value;
                                            break;
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
                                {
                                    recordFilled = true; // we are full;
                                }
                            } // end of fill record

                            if (!recordFilled)
                                continue;

                            prodUnitBarCodeRaw = string.Empty;
                            errMsg = "FK Refno " + prodFKRefno;

                            cmndProd.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                            cmndProd.CommandType = CommandType.Text;
                            //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                            cmndProd.Parameters.Add(new SQLiteParameter("@productFKRefno", prodFKRefno));

                            using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                            {
                                //if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                //{
                                if (drProd.Read())
                                {
                                    prodRefno = Convert.ToInt32(drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                }
                                else
                                {
                                    // TODO: Resource Strings
                                    throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource Strings
                                //    throw new Exception("Unable to locate the product refno from the product fk refno in tProduct." + Environment.NewLine + errMsg);
                                //}
                            }

                            errMsg += Environment.NewLine + "Unit code " + unitCode;

                            unitLookup.TryGetValue(unitCode, out unitRefno);

                            //if (!unitLookup.TryGetValue(unitCode, out unitRefno))
                            //{
                            //    //if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                            //    //{
                            //        if (drUnit.Read())
                            //        {
                            //            unitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                            //            unitLookup.Add(unitCode, unitRefno);
                            //        }
                            //        else
                            //        {
                            //            // TODO: Resource Strings
                            //            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
                            //        }
                            //    //}
                            //    //else
                            //    //{
                            //    //    // TODO: Resource Strings
                            //    //    throw new Exception("Unable to locate the unit refno from the unit code in tUnit" + Environment.NewLine + errMsg);
                            //    //}
                            //}

                            errMsg += Environment.NewLine + "Bar code is bc " + prodUnitBarCodeIsBC;

                            cmndProdUnit.CommandText = "SELECT * FROM tProductUnit WHERE ProductRefno = @productRefno AND UnitRefno = @unitRefno AND ProductUnitIsBC = @productUnitIsBC";
                            cmndProdUnit.CommandType = CommandType.Text;
                            //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", Convert.ToInt32(prodRefno)));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", Convert.ToInt32(unitRefno)));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", Convert.ToInt32(prodUnitBarCodeIsBC)));

                            using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            {
                                //if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitRefno, prodUnitBarCodeIsBC }))
                                //{
                                if (drProdUnit.Read())
                                {
                                    prodUnitRefno = Convert.ToInt32(drProdUnit.GetValue((int)SysCommon.ProdUnitFields.Refno));
                                }
                                else
                                {
                                    // TODO: Resource Strings
                                    throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource Strings
                                //    throw new Exception("Unable to locate the product unit refno from the product refno, unit refno and is bc in tProductUnit" + Environment.NewLine + errMsg);
                                //}
                            }

                            isProdUnitBarCodeFound = false;

                            if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                cmndProdUnitBar.CommandText = "SELECT ProductUnitBarCodeCode, ProductUnitBarCodeModifier1 FROM tProductUnitBarCode WHERE ProductUnitBarCodeCode = @productUnitBarCodeCode AND ProductUnitBarCodeModifier1 = @productUnitBarCodeModifier1";
                                cmndProdUnitBar.CommandType = CommandType.Text;
                                //cmndProdUnitBar.IndexName = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitBarCodeCode", prodUnitBarCodeCode));
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitBarCodeModifier1", prodUnitBarCodeModifier1));

                                using (SQLiteDataReader drProdUnitBar = cmndProdUnitBar.ExecuteReader())
                                {
                                    //if (drProdUnitBar.Seek(DbSeekOptions.FirstEqual, new object[] { prodUnitBarCodeCode, prodUnitBarCodeModifier1 }))
                                    if (drProdUnitBar.Read())
                                    {
                                        isProdUnitBarCodeFound = true;

                                        cmndProdUnitBarUpdate.CommandText = "UPDATE tProductUnitBarCode " +
                                            "SET ProductUnitRefno = @prodUnitRefno, " +
                                            "ProductUnitBarCodeSymbologyEnum = @prodUnitBarCodeSymbologyEnum, " +
                                            "ProductUnitBarCodeStatus = @productUnitBarCodeStatus " +
                                            "WHERE ProductUnitBarCodeCode = @prodUnitBarCodeCode " +
                                            "AND ProductUnitBarCodeModifier1 = @prodUnitBarCodeModifier1";
                                        cmndProdUnitBarUpdate.CommandType = CommandType.Text;
                                        cmndProdUnitBarUpdate.Parameters.Add(new SQLiteParameter("@prodUnitRefno", prodUnitRefno));
                                        cmndProdUnitBarUpdate.Parameters.Add(new SQLiteParameter("@prodUnitBarCodeSymbologyEnum", prodUnitBarCodeSymbologyEnum));
                                        cmndProdUnitBarUpdate.Parameters.Add(new SQLiteParameter("@productUnitBarCodeStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                                        cmndProdUnitBarUpdate.Parameters.Add(new SQLiteParameter("@prodUnitBarCodeCode", prodUnitBarCodeCode));
                                        cmndProdUnitBarUpdate.Parameters.Add(new SQLiteParameter("@prodUnitBarCodeModifier1", prodUnitBarCodeModifier1));

                                        cmndProdUnitBarUpdate.ExecuteNonQuery();
                                    }
                                }
                            }

                            if (!isProdUnitBarCodeFound)
                            {
                                cmndProdUnitBar.CommandText = "INSERT INTO tProductUnitBarCode " +
                                    "(ProductUnitRefno, " +
                                    "ProductUnitBarCodeCode, " +
                                    "ProductUnitBarCodeRaw, " +
                                    "ProductUnitBarCodeSymbologyEnum, " +
                                    "ProductUnitBarCodeModifier1, " +
                                    "ProductUnitBarCodeStatus) " +
                                    "VALUES " +
                                    "(@productUnitRefno, " +
                                    "@productUnitBarCodeCode, " +
                                    "@productUnitBarCodeRaw, " +
                                    "@productUnitBarCodeSymbologyEnum, " +
                                    "@productUnitBarCodeModifier1, " +
                                    "@productUnitBarCodeStatus)";
                                cmndProdUnitBar.CommandType = CommandType.Text;
                                //cmndProdUnitBar.IndexName = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitRefno", prodUnitRefno));
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitBarCodeCode", prodUnitBarCodeCode));
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitBarCodeRaw", prodUnitBarCodeRaw));
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitBarCodeSymbologyEnum", prodUnitBarCodeSymbologyEnum));
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitBarCodeModifier1", prodUnitBarCodeModifier1));
                                cmndProdUnitBar.Parameters.Add(new SQLiteParameter("@productUnitBarCodeStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));

                                cmndProdUnitBar.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ProdUnitBarCodeFields.ProdUnitRefno, prodUnitRefno);
                                //rec.SetString((int)SysCommon.ProdUnitBarCodeFields.Code, prodUnitBarCodeCode);
                                //rec.SetString((int)SysCommon.ProdUnitBarCodeFields.Raw, prodUnitBarCodeRaw);
                                //rec.SetInt32((int)SysCommon.ProdUnitBarCodeFields.SymbologyEnum, prodUnitBarCodeSymbologyEnum);
                                //rec.SetString((int)SysCommon.ProdUnitBarCodeFields.Modifier1, prodUnitBarCodeModifier1);
                                //rec.SetInt32((int)SysCommon.ProdUnitBarCodeFields.Status, Convert.ToInt32(SysCommon.ActStatus.NoChange));

                                //rs.Insert(rec);
                            }

                            ++recordCount;
                            if (recordCount % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                            }
                        } // end of data

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);

                        unitLookup.Clear();
                        //drProdUnit.Close();
                        //drUnit.Close();
                        //drProd.Close();
                        //drProdUnitBar.Close();
                        //}

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource Strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving product uom bar codes failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override string CreateRequest(string familyCode)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT UNIT BAR CODES</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<family_code>" + familyCode + "</family_code>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
