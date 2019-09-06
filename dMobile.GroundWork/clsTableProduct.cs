namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.IO;

    public class TableProduct : TableBase
    {
        #region Constructors

        public TableProduct()
            : base()
        {
            this.FullClassName = GetType().FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request()
        {
            //System.Diagnostics.Stopwatch swTotal = new System.Diagnostics.Stopwatch();
            //System.Diagnostics.Stopwatch swPerRun = new System.Diagnostics.Stopwatch();
            //int runs = 0;
            //swTotal.Start();

            List<string> families = new List<string>();

            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProducts), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products.", true);
#endif

                using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT tFamily.* FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                    using (SqliteDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        while (drFamily.Read())
                        {
                            families.Add(drFamily["FamilyCode"].ToString());
                        }
                    }
                }

                for (int i = 0; i < families.Count; ++i)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    string family = families[i];
                    string reqData = string.Empty;
#if MULTILANGUAGE
              this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ForFamily), family), true);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family), true);
#endif

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

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingProductsFailed), ex);
#else
                throw new Exception("Database exception occurred. Seeding products failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductsFailed), ex);
#else
                throw new Exception("Seeding products failed.", ex);
#endif
            }

            //swTotal.Stop();
            //Console.WriteLine("Total time for all runs {0}ms", swTotal.ElapsedMilliseconds);
            //Console.WriteLine("Average per run {0}ms", swTotal.ElapsedMilliseconds / runs);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProducts), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProduct";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductsFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging products failed.", ex);
#endif
            }
        }

        /// <summary>
        /// 
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
                    cmndProduct.CommandText = "tProduct";
                    cmndProduct.IndexName = "UQ_ProductOnProductFKRefno";
                    cmndProduct.CommandType = CommandType.TableDirect;

                    using (SqliteDataReader drProduct = cmndProduct.ExecuteReader())
                    {
                        drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno });
                        if (drProduct.Read())
                        {
                            prodRefno = drProduct.GetInt32((int)SysCommon.ProductFields.Refno);
                        }
                        else
                        {
                            throw new Exception(
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductRefnoFromProductFkRefnoInTproduct), prodFKRefno));
#else
string.Format("Unable to locate product Refno from product FK Refno {0} in tProduct", prodFKRefno));
#endif
                        }

                        drProduct.Close();
                    }
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodrefnofromprodfkrefnoFailed), ex);
#else
                throw new Exception("Database exception occurred. GetProdRefnoFromProdFKRefno failed.", ex);
#endif
            }

            return prodRefno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.
                    CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProduct";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Products), rowsAffected), true);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} products.", rowsAffected), true);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductsStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Products status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override string SendRequest(string familyCode)
        {
            return this.SendComm(this.CreateRequest(familyCode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void Save(string data)
        {
            string errMsg = string.Empty;
            SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

            try
            {
                using (DataSet ds = new DataSet())
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif
                    ds.ReadXml(SysCommon.GetXmlReader(ref data), XmlReadMode.Auto);

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProducts), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products.", false);
#endif
                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                        DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
 string.Format("Records {0}", progMax),
#endif
 false);

                        using (SQLiteCommand cmndProd = conn.CreateCommand())
                        using (SQLiteCommand cmndScheme = conn.CreateCommand())
                        using (SQLiteCommand cmndFamily = conn.CreateCommand())
                        {
                            cmndProd.CommandText = "tProduct";
                            cmndProd.CommandType = CommandType.TableDirect;
                            cmndProd.IndexName = "UQ_ProductOnProductFKRefno";

                            cmndScheme.CommandText = "tProductScheme";
                            cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                            cmndScheme.CommandType = CommandType.TableDirect;

                            cmndFamily.CommandText = "tFamily";
                            cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
                            cmndFamily.CommandType = CommandType.TableDirect;

                            using (SqliteDataReader drProduct = cmndProd.ExecuteReader())
                            using (SqliteDataReader drScheme = cmndScheme.ExecuteReader())
                            using (SqliteDataReader drFamily = cmndFamily.ExecuteReader())
                            using (SqlCeResultSet rs = cmndProd.ExecuteResultSet(ResultSetOptions.Updatable))
                            {
                                int familyRefno = 0;
                                string familyCode = string.Empty;
                                int schemeRefno = 0;
                                string schemeCode = string.Empty;
                                int prodFKRefno = 0;
                                bool isProdFound = false;
                                int progValue = 0;
                                SqlCeUpdatableRecord rec = rs.CreateRecord();

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                foreach (DataRow drow in ds.Tables["seg"].Rows)
                                {
                                    prodFKRefno = Convert.ToInt32(drow["f02"]);
                                    errMsg = "FK refno " + prodFKRefno;
                                    schemeCode = drow["f07"].ToString();
                                    errMsg = Environment.NewLine + "Scheme code " + schemeCode;

                                    if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                                    {
                                        if (drScheme.Read())
                                        {
                                            schemeRefno = drScheme.GetInt32((int)SysCommon.ProdSchemeFields.Refno);
                                        }
                                        else
                                        {
                                            throw new Exception("drScheme.Read()" + Environment.NewLine + errMsg);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeCodeInTproductscheme)
#else
"Unable to locate the product scheme code in tProductScheme."
#endif
 + Environment.NewLine + errMsg);
                                    }

                                    familyCode = drow["f08"].ToString();
                                    errMsg = Environment.NewLine + "Family code " + familyCode;
                                    if (drFamily.Seek(DbSeekOptions.FirstEqual, new object[] { familyCode }))
                                    {
                                        if (drFamily.Read())
                                        {
                                            familyRefno = drFamily.GetInt32((int)SysCommon.FamilyFields.Refno);
                                        }
                                        else
                                        {
                                            throw new Exception("drFamily.Read()" + Environment.NewLine + errMsg);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheFamilyCodeInTfamily)
#else
"Unable to locate the family code in tFamily."
#endif
 + Environment.NewLine + errMsg);
                                    }

                                    isProdFound = false;
                                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                    {
                                        cmndProd.CommandText = "tProduct";
                                        cmndProd.CommandType = CommandType.TableDirect;
                                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                        if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                        {

                                            isProdFound = true;
                                            cmndProd.CommandType = CommandType.Text;
                                            cmndProd.IndexName = string.Empty;
                                            cmndProd.Parameters.Clear();
                                            cmndProd.Parameters.AddWithValue("@Desc", drow["f03"].ToString());
                                            cmndProd.Parameters.AddWithValue("@BrandDesc", drow["f09"].ToString());
                                            cmndProd.CommandText = "UPDATE tProduct" +
                                                " SET ProductSchemeRefno = " + schemeRefno + "," +
                                                " FamilyRefno = " + familyRefno + "," +
                                                " ProductCode = \'" + drow["f01"].ToString() + "\'," +
                                                " ProductDescription = @Desc," +
                                                " ProductBrandDescription = @BrandDesc," +
                                                " ProductIsCatchCount = " + Convert.ToString(
                                                Convert.ToBoolean(drow["f04"]) ? 1 : 0) + "," +
                                                " ProductIsCatchWeight = " + Convert.ToString(
                                                Convert.ToBoolean(drow["f05"]) ? 1 : 0) + "," +
                                                " ProductIsCatchAsk = " + Convert.ToString(
                                                Convert.ToBoolean(drow["f06"]) ? 1 : 0) + "," +
                                                " ProductIsCatchAskZero = " + Convert.ToString(
                                                Convert.ToBoolean(drow["f10"]) ? 1 : 0) + "," +
                                                " ProductAllowConvertProduct = " + Convert.ToString(
                                                Convert.ToBoolean(drow["f11"]) ? 1 : 0) +
                                                " WHERE ProductFKRefno = " + prodFKRefno;

                                            cmndProd.ExecuteNonQuery();
                                        }
                                    }

                                    if (!isProdFound)
                                    {
                                        cmndProd.CommandText = "tProduct";
                                        cmndProd.CommandType = CommandType.TableDirect;
                                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                        rec.SetInt32((int)SysCommon.ProductFields.ProdSchemeRefno, schemeRefno);
                                        rec.SetString((int)SysCommon.ProductFields.Code, drow["f01"].ToString());
                                        rec.SetInt32((int)SysCommon.ProductFields.FKRefno, prodFKRefno);
                                        rec.SetString((int)SysCommon.ProductFields.Description, drow["f03"].ToString());
                                        rec.SetBoolean((int)SysCommon.ProductFields.IsCatchWeight, Convert.ToBoolean(drow["f05"]));
                                        rec.SetBoolean((int)SysCommon.ProductFields.IsCatchCount, Convert.ToBoolean(drow["f04"]));
                                        rec.SetBoolean((int)SysCommon.ProductFields.IsCatchAsk, Convert.ToBoolean(drow["f06"]));
                                        rec.SetInt32((int)SysCommon.ProductFields.FamilyRefno, familyRefno);
                                        rec.SetString((int)SysCommon.ProductFields.BrandDescription, drow["f09"].ToString());
                                        rec.SetBoolean((int)SysCommon.ProductFields.IsCatchAskZero, Convert.ToBoolean(drow["f10"]));
                                        rec.SetBoolean((int)SysCommon.ProductFields.AllowConvertProduct, Convert.ToBoolean(drow["f11"]));

                                        rs.Insert(rec);
                                    }

                                    progValue++;
                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);

                                drFamily.Close();

                                drScheme.Close();

                                drProduct.Close();
                            }
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductsComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductsFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products failed.", false);
#endif

                // Duplicate key violations
                if (ex.NativeError == 25016)
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
#endif
                }

                throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductsFailed) + Environment.NewLine + errMsg,
#else
"Database exception occurred. Saving products failed." + Environment.NewLine + errMsg,
#endif
 ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected void SaveOpt(string data)
        {
            string errMsg = string.Empty;
            int processedCount = 0;
            SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

            bool refreshDataChangesOnly = SysEnv.RefreshOpt.RefreshDataChangesOnly;

            try
            {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif

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


#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProducts), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products.", false);
#endif


                    Dictionary<string, int> schemeCache = new Dictionary<string, int>();
                    Dictionary<string, int> familyCache = new Dictionary<string, int>();

                    using (SQLiteCommand cmndProd = conn.CreateCommand())
                    using (SQLiteCommand cmndScheme = conn.CreateCommand())
                    using (SQLiteCommand cmndFamily = conn.CreateCommand())
                    {
                        cmndProd.CommandText = "tProduct";
                        cmndProd.CommandType = CommandType.TableDirect;
                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";

                        cmndScheme.CommandText = "tProductScheme";
                        cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                        cmndScheme.CommandType = CommandType.TableDirect;

                        cmndFamily.CommandText = "tFamily";
                        cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
                        cmndFamily.CommandType = CommandType.TableDirect;

                        using (SqliteDataReader drProduct = cmndProd.ExecuteReader())
                        using (SqliteDataReader drScheme = cmndScheme.ExecuteReader())
                        using (SqliteDataReader drFamily = cmndFamily.ExecuteReader())
                        using (SqlCeResultSet rs = cmndProd.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
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

                            SqlCeUpdatableRecord rec = rs.CreateRecord();

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

                                // Get schemeRefno from schemeCode
                                if (!schemeCache.TryGetValue(schemeCode, out schemeRefno))
                                {
                                    if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                                    {
                                        if (drScheme.Read())
                                        {
                                            schemeRefno = drScheme.GetInt32((int)SysCommon.ProdSchemeFields.Refno);
                                            schemeCache.Add(schemeCode, schemeRefno);
                                        }
                                        else
                                        {
                                            throw new Exception("drScheme.Read()" + Environment.NewLine + errMsg);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeCodeInTproductscheme)
#else
"Unable to locate the product scheme code in tProductScheme."
#endif
 + Environment.NewLine + errMsg);
                                    }
                                }

                                if (!familyCache.TryGetValue(familyCode, out familyRefno))
                                {
                                    // Get familyRefno from familyCode
                                    if (drFamily.Seek(DbSeekOptions.FirstEqual, new object[] { familyCode }))
                                    {
                                        if (drFamily.Read())
                                        {
                                            familyRefno = drFamily.GetInt32((int)SysCommon.FamilyFields.Refno);
                                            familyCache.Add(familyCode, familyRefno);
                                        }
                                        else
                                        {
                                            throw new Exception("drFamily.Read()" + Environment.NewLine + errMsg);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheFamilyCodeInTfamily)
#else
"Unable to locate the family code in tFamily."
#endif
 + Environment.NewLine + errMsg);
                                    }
                                }


                                isProdFound = false;
                                if (refreshDataChangesOnly)
                                {
                                    cmndProd.CommandText = "tProduct";
                                    cmndProd.CommandType = CommandType.TableDirect;
                                    cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                    if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                    {

                                        isProdFound = true;
                                        cmndProd.CommandType = CommandType.Text;
                                        cmndProd.IndexName = string.Empty;
                                        cmndProd.Parameters.Clear();

                                        // For optimal performance set parameter type and maximum size
                                        // https://msdn.microsoft.com/en-us/library/system.data.sqlserverce.SQLiteCommand.parameters(v=vs.100).aspx
                                        SqlCeParameter schemeRefnoParam = new SqlCeParameter("@SchemeRefno", SqlDbType.Int);
                                        SqlCeParameter prodCodeParam = new SqlCeParameter("@ProdCode", SqlDbType.NVarChar, 10);
                                        SqlCeParameter famRefnoParam = new SqlCeParameter("@FamilyRefno", SqlDbType.Int);
                                        SqlCeParameter descParam = new SqlCeParameter("@Desc", SqlDbType.NVarChar, 25);
                                        SqlCeParameter brandDescParam = new SqlCeParameter("@BrandDesc", SqlDbType.NVarChar, 10);
                                        SqlCeParameter catchCountParam = new SqlCeParameter("@CatchCount", SqlDbType.Bit);
                                        SqlCeParameter catchWeightParam = new SqlCeParameter("@CatchWeight", SqlDbType.Bit);
                                        SqlCeParameter catchAskParam = new SqlCeParameter("@CatchAsk", SqlDbType.Bit);
                                        SqlCeParameter catchAskZeroParam = new SqlCeParameter("@CatchAskZero", SqlDbType.Bit);
                                        SqlCeParameter allowConvertParam = new SqlCeParameter("@AllowConvert", SqlDbType.Bit);

                                        cmndProd.Parameters.Add(schemeRefnoParam);
                                        cmndProd.Parameters.Add(prodCodeParam);
                                        cmndProd.Parameters.Add(famRefnoParam);
                                        cmndProd.Parameters.Add(descParam);
                                        cmndProd.Parameters.Add(brandDescParam);
                                        cmndProd.Parameters.Add(catchCountParam);
                                        cmndProd.Parameters.Add(catchWeightParam);
                                        cmndProd.Parameters.Add(catchAskParam);
                                        cmndProd.Parameters.Add(catchAskZeroParam);
                                        cmndProd.Parameters.Add(allowConvertParam);

                                        //cmndProd.Parameters.AddWithValue("@Desc", description);
                                        //cmndProd.Parameters.AddWithValue("@BrandDesc", brandDescription);
                                        // TODO make these all parameterized inputs
                                        cmndProd.CommandText = "UPDATE tProduct" +
                                            " SET ProductSchemeRefno = @SchemeRefno," +
                                            " FamilyRefno = @FamilyRefno," +
                                            " ProductCode = @ProdCode," +
                                            " ProductDescription = @Desc," +
                                            " ProductBrandDescription = @BrandDesc," +
                                            " ProductIsCatchCount = @CatchCount," +
                                            " ProductIsCatchWeight = @CatchWeight," +
                                            " ProductIsCatchAsk = @CatchAsk," +
                                            " ProductIsCatchAskZero = @CatchAskZero," +
                                            " ProductAllowConvertProduct = @AllowConvert" +
                                            " WHERE ProductFKRefno = " + prodFKRefno;

                                        // Calling prepare after having set the command text and parameters
                                        cmndProd.Prepare();


                                        schemeRefnoParam.Value = schemeRefno;
                                        prodCodeParam.Value = prodCode;
                                        famRefnoParam.Value = familyRefno;
                                        descParam.Value = description;
                                        brandDescParam.Value = brandDescription;
                                        catchCountParam.Value = isCatchCount;
                                        catchWeightParam.Value = isCatchWeight;
                                        catchAskParam.Value = isCatchAsk;
                                        catchAskZeroParam.Value = isCatchAskZero;
                                        allowConvertParam.Value = isAllowConvertProduct;


                                        cmndProd.ExecuteNonQuery();
                                    }
                                }

                                if (!isProdFound)
                                {
                                    cmndProd.CommandText = "tProduct";
                                    cmndProd.CommandType = CommandType.TableDirect;
                                    cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                    rec.SetInt32((int)SysCommon.ProductFields.ProdSchemeRefno, schemeRefno);
                                    rec.SetString((int)SysCommon.ProductFields.Code, prodCode);
                                    rec.SetInt32((int)SysCommon.ProductFields.FKRefno, prodFKRefno);
                                    rec.SetString((int)SysCommon.ProductFields.Description, description);
                                    rec.SetBoolean((int)SysCommon.ProductFields.IsCatchWeight, isCatchWeight);
                                    rec.SetBoolean((int)SysCommon.ProductFields.IsCatchCount, isCatchCount);
                                    rec.SetBoolean((int)SysCommon.ProductFields.IsCatchAsk, isCatchAsk);
                                    rec.SetInt32((int)SysCommon.ProductFields.FamilyRefno, familyRefno);
                                    rec.SetString((int)SysCommon.ProductFields.BrandDescription, brandDescription);
                                    rec.SetBoolean((int)SysCommon.ProductFields.IsCatchAskZero, isCatchAskZero);
                                    rec.SetBoolean((int)SysCommon.ProductFields.AllowConvertProduct, isAllowConvertProduct);

                                    rs.Insert(rec);
                                }

                                ++processedCount;
                                if (processedCount % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, (processedCount / 10) % 100, 0, 100);
                                }
                                progValue++;

                            } // end while loop of reading data in

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100); // complete
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", processedCount), false);

                            drFamily.Close();
                            drScheme.Close();
                            drProduct.Close();
                        } // using SQL
                    } // using commands
                } // using xml stream reader

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductsComplete), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products complete.", false);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductsFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products failed.", false);
#endif

                // Duplicate key violations
                if (ex.NativeError == 25016)
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
#endif
                }

                throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductsFailed) + Environment.NewLine + errMsg,
#else
"Database exception occurred. Saving products failed." + Environment.NewLine + errMsg,
#endif
 ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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

        #endregion
    }
}
