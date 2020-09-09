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
    /// Defines the <see cref="TableProductUnitRelation" />
    /// </summary>
    public class TableProductUnitRelation : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductUnitRelation"/> class.
        /// </summary>
        public TableProductUnitRelation()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnitRelation";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations.", true);

                using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT * FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    //cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                    using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        string family = string.Empty;

                        while (drFamily.Read())
                        {
                            //GC.Collect();
                            //GC.WaitForPendingFinalizers();

                            family = drFamily["FamilyCode"].ToString();

                            string recvData = SendRequest(family);

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family), true);

                            if (0 < recvData.Length)
                            {
                                if (recvData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(recvData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                this.SaveOpt(recvData);
                            }
                        }

                        drFamily.Close();
                    }
                }


                // Require to have product uom relations.
                if (0 >= this.DataStatus())
                {
                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "No product uom relations where received.", true);

                    // TODO: Resource Strings
                    throw new Exception("No product uom relations where received.");
                }

                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations failed.", true);

                // TODO: Resource Strings
                throw new Exception("Database exception occurred. Seeding product uom relations failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations failed.", true);

                // TODO: Resource Strings
                throw new Exception("Seeding product uom relations failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductUnitRelation";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product uom relations failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductUnitRelation";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product unit relations.", rowsAffected), false);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product unit relations status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="strFamilyCode"></param>
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
            int recordCount = 0;


            using (SQLiteCommand cmndProdUnitRel = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnitRelUpdate = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations.", false);

                        //cmndProdUnitRel.CommandText = "tProductUnitRelation";
                        //cmndProdUnitRel.CommandType = CommandType.TableDirect;
                        //cmndProdUnitRel.IndexName = "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";

                        //cmndProd.CommandText = "tProduct";
                        //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                        //cmndProd.CommandType = CommandType.TableDirect;

                        //cmndUnit.CommandText = "tUnit";
                        //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                        //cmndUnit.CommandType = CommandType.TableDirect;

                        //cmndProdUnit.CommandText = "tProductUnit";
                        //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                        //cmndProdUnit.CommandType = CommandType.TableDirect;

                        //using (SQLiteDataReader drProdUnitRel = cmndProdUnitRel.ExecuteReader())
                        //using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                        //using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                        //using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                        //using (SqlCeResultSet rs = cmndProdUnitRel.ExecuteResultSet(ResultSetOptions.Updatable))
                        //{

                        Dictionary<string, int> unitLookup = new Dictionary<string, int>();
                        int lastFKRefno = 0;
                        bool refreshOnly = SysEnv.RefreshOpt.RefreshDataChangesOnly;

                        //SqlCeUpdatableRecord rec = rs.CreateRecord();
                        int prodRefno = 0;
                        int unitCode1Refno = 0;
                        int unitCode2Refno = 0;
                        bool isProdUnitRelationFound = false;
                        int prodUnit1Refno = 0;
                        int prodUnit2Refno = 0;
                        // f01
                        bool prodUnitIsBC = false;
                        // f02
                        decimal relationFactor = 0;
                        // f03
                        int prodFKRefno = 0;
                        // f04
                        string unitCode1 = string.Empty;
                        // f05
                        string unitCode2 = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        //transaction = conn.BeginTransaction();

                        while (!reader.EOF)
                        {
                            // fill one record
                            // we do this by looping until an end segment is found
                            bool recordFilled = false;
                            string elementName = string.Empty;
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
                                            prodUnitIsBC = reader.ReadContentAsBoolean();
                                            break;
                                        case "f02":
                                            relationFactor = reader.ReadContentAsDecimal();
                                            break;
                                        case "f03":
                                            prodFKRefno = reader.ReadContentAsInt();
                                            break;
                                        case "f04":
                                            unitCode1 = reader.Value;
                                            break;
                                        case "f05":
                                            unitCode2 = reader.Value;
                                            break;
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
                                {
                                    recordFilled = true; // we are full;
                                }
                            } // end of fill record

                            if (!recordFilled)
                            {
                                continue;
                            }

                            errMsg = "FK refno " + prodFKRefno;

                            if (prodFKRefno == lastFKRefno)
                            {
                                // no op because prodRefno already set to last prodRefno.
                                // this is a last object cache.
                                // curr prod refno == last prod refno
                            }
                            else
                            {
                                lastFKRefno = prodFKRefno;

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

                                    drProd.Close();
                                }
                            }

                            errMsg += Environment.NewLine + "Unit code 1 " + unitCode1;

                            if (!unitLookup.TryGetValue(unitCode1, out unitCode1Refno))
                            {
                                cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                                cmndUnit.CommandType = CommandType.Text;
                                //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                                cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", unitCode1));

                                using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                                {
                                    //if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode1 }))
                                    //{
                                    if (drUnit.Read())
                                    {
                                        unitCode1Refno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                                        unitLookup.Add(unitCode1, unitCode1Refno);
                                    }
                                    else
                                    {
                                        // TODO: Resource Strings
                                        throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource Strings
                                    //    throw new Exception("Unable to locate the unit refno from the unit code 1 in tUnit." + Environment.NewLine + errMsg);
                                    //}

                                    drUnit.Close();
                                }
                            }

                            errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

                            cmndProdUnit.CommandText = "SELECT * FROM tProductUnit WHERE ProductRefno = @productRefno AND UnitRefno = @unitRefno AND ProductUnitIsBC = @productUnitIsBC";
                            cmndProdUnit.CommandType = CommandType.Text;
                            //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", prodRefno));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", unitCode1Refno));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", prodUnitIsBC));

                            using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            {
                                //if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitCode1Refno, prodUnitIsBC }))
                                //{
                                if (drProdUnit.Read())
                                {
                                    prodUnit1Refno = Convert.ToInt32(drProdUnit.GetValue((int)SysCommon.ProdUnitFields.Refno));
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
                                //    throw new Exception("Unable to locate the product unit refno from the product refno, unit code 1 refno and is bc in tProductUnit." + Environment.NewLine + errMsg);
                                //}

                                drProdUnit.Close();
                            }

                            errMsg += Environment.NewLine + "Unit code 2 " + unitCode2;

                            if (!unitLookup.TryGetValue(unitCode2, out unitCode2Refno))
                            {
                                cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                                cmndUnit.CommandType = CommandType.Text;
                                //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                                cmndUnit.Parameters.Clear();
                                cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", unitCode2));

                                using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                                {
                                    //if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode2 }))
                                    //{
                                    if (drUnit.Read())
                                    {
                                        unitCode2Refno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                                        unitLookup.Add(unitCode2, unitCode2Refno);
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
                                    //    throw new Exception("Unable to locate the unit refno from the unit code 2 in tUnit." + Environment.NewLine + errMsg);
                                    //}

                                    drUnit.Close();
                                }
                            }

                            cmndProdUnit.CommandText = "SELECT * FROM tProductUnit WHERE ProductRefno = @productRefno AND UnitRefno = @unitRefno AND ProductUnitIsBC = @productUnitIsBC";
                            cmndProdUnit.CommandType = CommandType.Text;
                            //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                            cmndProdUnit.Parameters.Clear();
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", prodRefno));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", unitCode2Refno));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", prodUnitIsBC));

                            using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            {
                                //if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitCode2Refno, prodUnitIsBC }))
                                //{
                                if (drProdUnit.Read())
                                {
                                    prodUnit2Refno = Convert.ToInt32(drProdUnit.GetValue((int)SysCommon.ProdUnitFields.Refno));
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
                                //    throw new Exception("Unable to locate the product unit refno from the product refno, unit code 2 refno and is bc in tProductUnit." + Environment.NewLine + errMsg);
                                //}

                                drProdUnit.Close();
                            }

                            isProdUnitRelationFound = false;

                            cmndProdUnitRel.CommandText = "SELECT * FROM tProductUnitRelation WHERE ProductUnit1Refno = @productUnit1 AND ProductUnit2Refno = @productUnit2";
                            cmndProdUnitRel.CommandType = CommandType.Text;
                            //cmndProdUnitRel.IndexName = "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";
                            cmndProdUnitRel.Parameters.Add(new SQLiteParameter("@productUnit1", prodUnit1Refno));
                            cmndProdUnitRel.Parameters.Add(new SQLiteParameter("@productUnit2", prodUnit2Refno));

                            using (SQLiteDataReader drProdUnitRel = cmndProdUnitRel.ExecuteReader())
                            {
                                //if (drProdUnitRel.Seek(DbSeekOptions.FirstEqual, new object[] { prodUnit1Refno, prodUnit2Refno }))
                                if (drProdUnitRel.Read())
                                {
                                    isProdUnitRelationFound = true;

                                    if (refreshOnly)
                                    {
                                        cmndProdUnitRelUpdate.CommandText = "UPDATE tProductUnitRelation " +
                                    "SET ProductUnitRelationFactor = @relationFactor " +
                                    "WHERE ProductUnit1Refno = @prodUnit1Refno AND ProductUnit2Refno = @prodUnit2Refno";

                                    cmndProdUnitRelUpdate.CommandType = CommandType.Text;
                                    //cmndProdUnitRelUpdate.IndexName = string.Empty;
                                    cmndProdUnitRelUpdate.Parameters.Add(new SQLiteParameter("@relationFactor", relationFactor));
                                    cmndProdUnitRelUpdate.Parameters.Add(new SQLiteParameter("@prodUnit1Refno", prodUnit1Refno));
                                    cmndProdUnitRelUpdate.Parameters.Add(new SQLiteParameter("@prodUnit2Refno", prodUnit2Refno));

                                    cmndProdUnitRelUpdate.ExecuteNonQuery();
                                    }
                                }

                                drProdUnitRel.Close();
                            }

                            if (!isProdUnitRelationFound)
                            {
                                cmndProdUnitRel.CommandText = "INSERT INTO tProductUnitRelation (ProductUnit1Refno, ProductUnit2Refno, ProductUnitRelationFactor) VALUES (@productUnit1Refno, @productUnit2Refno, @productUnitRelationFactor)";
                                cmndProdUnitRel.CommandType = CommandType.Text;
                                //cmndProdUnitRel.IndexName = "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";
                                cmndProdUnitRel.Parameters.Clear();
                                cmndProdUnitRel.Parameters.Add(new SQLiteParameter("@productUnit1Refno", prodUnit1Refno));
                                cmndProdUnitRel.Parameters.Add(new SQLiteParameter("@productUnit2Refno", prodUnit2Refno));
                                cmndProdUnitRel.Parameters.Add(new SQLiteParameter("@productUnitRelationFactor", relationFactor));

                                cmndProdUnitRel.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ProdUnitRelationFields.ProdUnit1Refno, prodUnit1Refno);
                                //rec.SetInt32((int)SysCommon.ProdUnitRelationFields.ProdUnit2Refno, prodUnit2Refno);
                                //rec.SetDecimal((int)SysCommon.ProdUnitRelationFields.Factor, relationFactor);

                                //rs.Insert(rec);
                            }

                            ++recordCount;
                            if (recordCount % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);

                        unitLookup.Clear();

                        //drProdUnit.Close();
                        //drUnit.Close();
                        //drProd.Close();
                        //drProdUnitRel.Close();
                        //}
                        //}

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource Strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving product uom relations failed." + errMsg, ex);
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
                + "<action>REQUEST PRODUCT UNIT RELATIONS</action>"
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
