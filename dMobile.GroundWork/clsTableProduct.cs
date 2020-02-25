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
    /// Defines the <see cref="TableProduct" />
    /// </summary>
    public class TableProduct : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProduct"/> class.
        /// </summary>
        public TableProduct() : base()
        {
            this.FullClassName = GetType().FullName;
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            //System.Diagnostics.Stopwatch swTotal = new System.Diagnostics.Stopwatch();
            //System.Diagnostics.Stopwatch swPerRun = new System.Diagnostics.Stopwatch();
            //int runs = 0;
            //swTotal.Start();

            List<string> families = new List<string>();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products.", true);

                using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT * FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    //cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                    using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        while (drFamily.Read())
                        {
                            families.Add(drFamily["FamilyCode"].ToString());
                        }

                        drFamily.Close();
                    }
                }

                for (int i = 0; i < families.Count; ++i)
                {
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();

                    string family = families[i];
                    string reqData = string.Empty;

                    // TODO: Resource strings
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
                        //swPerRun.Reset();
                        //swPerRun.Start();
                        //this.Save(reqData);
                        this.SaveOpt(reqData);
                        //swPerRun.Stop();
                        //++runs;
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding products failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding products failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProduct";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging products failed.", ex);
            }
        }

        /// <summary>
        /// The GetProdRefnoFromProdFKRefno
        /// </summary>
        /// <param name="prodFKRefno"></param>
        /// <returns></returns>
        public int GetProdRefnoFromProdFKRefno(int prodFKRefno)
        {
            int prodRefno = 0;

            try
            {
                using (SQLiteCommand cmndProduct = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndProduct.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                    cmndProduct.CommandType = CommandType.Text;
                    //cmndProduct.IndexName = "UQ_ProductOnProductFKRefno";
                    cmndProduct.Parameters.Add(new SQLiteParameter("@productFKRefno", Convert.ToInt32(prodFKRefno)));

                    using (SQLiteDataReader drProduct = cmndProduct.ExecuteReader())
                    {
                        //drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno });
                        if (drProduct.Read())
                        {
                            prodRefno = drProduct.GetInt32((int)SysCommon.ProductFields.Refno);
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exception(string.Format("Unable to locate product Refno from product FK Refno {0} in tProduct", prodFKRefno));
                        }

                        drProduct.Close();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. GetProdRefnoFromProdFKRefno failed.", ex);
            }

            return prodRefno;
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProduct";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} products.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Products status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override string SendRequest(string familyCode)
        {
            return this.SendComm(this.CreateRequest(familyCode));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="data"></param>
        protected override void Save(string data)
        {
        }

        /// <summary>
        /// The SaveOpt
        /// </summary>
        /// <param name="data"></param>
        protected void SaveOpt(string data)
        {
            int processedCount = 0;
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            bool refreshDataChangesOnly = SysEnv.RefreshOpt.RefreshDataChangesOnly;

            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndProdGet = conn.CreateCommand())
            using (SQLiteCommand cmndProdUpdate = conn.CreateCommand())
            using (SQLiteCommand cmndProdInsert = conn.CreateCommand())
            using (SQLiteCommand cmndScheme = conn.CreateCommand())
            using (SQLiteCommand cmndFamily = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    //Dictionary<string, int> schemeCache = new Dictionary<string, int>();
                    Dictionary<string, int> familyCache = new Dictionary<string, int>();
                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    int startIndex = data.IndexOf("<VFPData");
                    XmlReaderSettings sett = new XmlReaderSettings();
                    sett.CheckCharacters = false;
                    sett.CloseInput = false;
                    sett.ConformanceLevel = ConformanceLevel.Auto;
                    sett.IgnoreComments = true;
                    sett.IgnoreProcessingInstructions = true;
                    sett.IgnoreWhitespace = false;
                    sett.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                    sett.ValidationType = ValidationType.None;

                    using (StringReader stringReader = new StringReader(data.Substring(startIndex)))
                    using (XmlReader reader = XmlReader.Create(stringReader, sett))
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products.", false);

                        //cmndProd.CommandText = "SELECT * FROM tProduct";
                        //cmndProd.CommandType = CommandType.Text;
                        ////cmndProd.IndexName = "UQ_ProductOnProductFKRefno";

                        //cmndScheme.CommandText = "SELECT * FROM tProductScheme";
                        //cmndScheme.CommandType = CommandType.Text;
                        //cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";

                        cmndFamily.CommandText = "SELECT * FROM tFamily";
                        cmndFamily.CommandType = CommandType.Text;
                        //cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                        //using (SQLiteDataReader drProduct = cmndProd.ExecuteReader())
                        //using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
                        using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
                        {
                            //while (drScheme.Read())
                            //{
                            //    schemeCache.Add(drScheme.GetString((int)SysCommon.ProdSchemeFields.Code), drScheme.GetInt32((int)SysCommon.ProdSchemeFields.Refno));
                            //}

                            while (drFamily.Read())
                            {
                                familyCache.Add(drFamily.GetString((int)SysCommon.FamilyFields.Code), drFamily.GetInt32((int)SysCommon.FamilyFields.Refno));
                            }
                        }

                        bool isProdFound = false;
                        int progValue = 0;
                        int familyRefno = 0;
                        int schemeRefno = 0;

                        // f01
                        string prodCode = string.Empty;

                        // f02
                        int prodFKRefno = 0;

                        // f03
                        string description = string.Empty;

                        // f04
                        bool isCatchCount = false;

                        // f05
                        bool isCatchWeight = false;

                        // f06
                        bool isCatchAsk = false;

                        // f07
                        string schemeCode = string.Empty;

                        // f08
                        string familyCode = string.Empty;

                        // f09
                        string brandDescription = string.Empty;

                        // f10
                        bool isCatchAskZero = false;

                        // f11
                        bool isAllowConvertProduct = false;

                        //SqlCeUpdatableRecord rec = rs.CreateRecord();

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

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
                                            prodCode = reader.Value;
                                            break;
                                        case "f02":
                                            prodFKRefno = reader.ReadContentAsInt();
                                            break;
                                        case "f03":
                                            description = reader.Value;
                                            break;
                                        case "f04":
                                            isCatchCount = reader.ReadContentAsBoolean();
                                            break;
                                        case "f05":
                                            isCatchWeight = reader.ReadContentAsBoolean();
                                            break;
                                        case "f06":
                                            isCatchAsk = reader.ReadContentAsBoolean();
                                            break;
                                        case "f07":
                                            schemeCode = reader.Value;
                                            break;
                                        case "f08":
                                            familyCode = reader.Value;
                                            break;
                                        case "f09":
                                            brandDescription = reader.Value;
                                            break;
                                        case "f10":
                                            isCatchAskZero = reader.ReadContentAsBoolean();
                                            break;
                                        case "f11":
                                            isAllowConvertProduct = reader.ReadContentAsBoolean();
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

                            //schemeCache.TryGetValue(schemeCode, out schemeRefno);
                            familyCache.TryGetValue(familyCode, out familyRefno);


                            isProdFound = false;

                            cmndProdGet.CommandText = "SELECT ProductFKRefno FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                            cmndProdGet.CommandType = CommandType.Text;
                            //cmndProdGet.IndexName = "UQ_ProductOnProductFKRefno";
                            cmndProdGet.Parameters.Add(new SQLiteParameter("@productFKRefno", prodFKRefno));

                            using (SQLiteDataReader drProduct = cmndProdGet.ExecuteReader())
                            {
                                //if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                if (drProduct.Read())
                                {
                                    isProdFound = true;

                                    if (refreshDataChangesOnly)
                                    {
                                        cmndProdUpdate.CommandText = "UPDATE tProduct" +
                                        " SET FamilyRefno = @familyRefno," +
                                        " ProductCode = @prodCode," +
                                        " ProductDescription = @desc," +
                                        " ProductBrandDescription = @brandDesc," +
                                        " ProductIsCatchCount = @catchCount," +
                                        " ProductIsCatchWeight = @catchWeight," +
                                        " ProductIsCatchAsk = @catchAsk," +
                                        " ProductIsCatchAskZero = @catchAskZero," +
                                        " ProductAllowConvertProduct = @allowConvert" +
                                        " WHERE ProductFKRefno = @prodFKRefno";

                                        cmndProdUpdate.CommandType = CommandType.Text;
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@familyRefno", familyRefno));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@prodCode", prodCode));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@desc", description));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@brandDesc", brandDescription));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@catchCount", isCatchCount));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@catchWeight", isCatchWeight));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@catchAsk", isCatchAsk));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@catchAskZero", isCatchAskZero));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@allowConvert", isAllowConvertProduct));
                                        cmndProdUpdate.Parameters.Add(new SQLiteParameter("@prodFKRefno", prodFKRefno));

                                        cmndProdUpdate.ExecuteNonQuery();
                                    }
                                }
                            }

                            //}
                            if (!isProdFound)
                            {
                                //using (SQLiteCommand cmndProdInsert = new SQLiteConnection(DatabaseConnection.GetInstance().SQLConn).CreateCommand())
                                //{
                                cmndProdInsert.CommandText = "INSERT INTO tProduct " +
                                "(ProductSchemeRefno, " +
                                "FamilyRefno, " +
                                "ProductCode, " +
                                "ProductFKRefno, " +
                                "ProductDescription, " +
                                "ProductIsCatchCount, " +
                                "ProductIsCatchWeight, " +
                                "ProductIsCatchAsk, " +
                                "ProductIsCatchAskZero, " +
                                "ProductBrandDescription, " +
                                "ProductAllowConvertProduct) " +
                                "VALUES " +
                                "(@productSchemeRefno, " +
                                "@familyRefno, " +
                                "@productCode, " +
                                "@productFKRefno, " +
                                "@productDescription, " +
                                "@productIsCatchCount, " +
                                "@productIsCatchWeight, " +
                                "@productIsCatchAsk, " +
                                "@productIsCatchAskZero, " +
                                "@productBrandDescription, " +
                                "@productAllowConvertProduct)";

                                cmndProdInsert.CommandType = CommandType.Text;
                                //cmndProdInsert.IndexName = "UQ_ProductOnProductFKRefno";
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productSchemeRefno", schemeRefno));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@familyRefno", familyRefno));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productCode", prodCode));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productFKRefno", prodFKRefno));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productDescription", description));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productIsCatchCount", isCatchCount));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productIsCatchWeight", isCatchWeight));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productIsCatchAsk", isCatchAsk));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productIsCatchAskZero", isCatchAskZero));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productBrandDescription", brandDescription));
                                cmndProdInsert.Parameters.Add(new SQLiteParameter("@productAllowConvertProduct", isAllowConvertProduct));

                                cmndProdInsert.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ProductFields.ProdSchemeRefno, schemeRefno);
                                //rec.SetString((int)SysCommon.ProductFields.Code, prodCode);
                                //rec.SetInt32((int)SysCommon.ProductFields.FKRefno, prodFKRefno);
                                //rec.SetString((int)SysCommon.ProductFields.Description, description);
                                //rec.SetBoolean((int)SysCommon.ProductFields.IsCatchWeight, isCatchWeight);
                                //rec.SetBoolean((int)SysCommon.ProductFields.IsCatchCount, isCatchCount);
                                //rec.SetBoolean((int)SysCommon.ProductFields.IsCatchAsk, isCatchAsk);
                                //rec.SetInt32((int)SysCommon.ProductFields.FamilyRefno, familyRefno);
                                //rec.SetString((int)SysCommon.ProductFields.BrandDescription, brandDescription);
                                //rec.SetBoolean((int)SysCommon.ProductFields.IsCatchAskZero, isCatchAskZero);
                                //rec.SetBoolean((int)SysCommon.ProductFields.AllowConvertProduct, isAllowConvertProduct);
                                //rs.Insert(rec);
                                //}
                            }

                            ++processedCount;

                            if (processedCount % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, (processedCount / 10) % 100, 0, 100);
                            }

                            progValue++;

                        } // end while loop of reading data in

                        // TODO: Resource strings
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100); // complete
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", processedCount), false);

                        //drFamily.Close();
                        //drScheme.Close();
                        //drProduct.Close();
                        //} // using SQL
                        //} // using commands
                    } // using xml stream reader

                    transaction.Commit();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource Strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving products failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="strFamilyCode">The strFamilyCode<see cref="string"/></param>
        /// <returns></returns>
        protected override string CreateRequest(string strFamilyCode)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTS</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<family_code>" + strFamilyCode + "</family_code>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
