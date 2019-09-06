namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.IO;

    public class TableProductUnitBarCode : TableBase
    {
        #region Constructors

        public TableProductUnitBarCode()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnitBarCode";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomBarCodes), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes.", true);
#endif

                using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT tFamily.* FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
                    using (SqliteDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        string reqData = string.Empty;
                        string family = string.Empty;

                        while (drFamily.Read())
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            family = drFamily["FamilyCode"].ToString();
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

                                this.SaveOpt(reqData);
                            }
                        }
                    }
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomBarCodesComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomBarCodesFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingProductUomBarCodesFailed), ex);
#else
                throw new Exception("Database exception occurred. Seeding product uom bar codes failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomBarCodesFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomBarCodesFailed), ex);
#else
                throw new Exception("Seeding product uom bar codes failed.", ex);
#endif
            }
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomBarCodes), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tProductUnitBarCode";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomBarCodesComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomBarCodesFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductUomBarCodesFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging product uom bar codes failed.", ex);
#endif
            }
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
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductUnitBarCode";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductUomBarCodes), rowsAffected),
#else
 string.Format("{0} product uom bar codes.", rowsAffected),
#endif
 true);
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductUomBarCodesStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Product uom bar codes status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string SendRequest(string strFamilyCode)
        {
            return this.SendComm(this.CreateRequest(strFamilyCode));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override void Save(string reqData)
        {
            SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            try
            {
                using (DataSet ds = new DataSet())
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif
                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomBarCodes), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes.", false);
#endif
                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
 string.Format("Records {0}", progMax),
#endif
 false);

                        using (SQLiteCommand cmndProdUnitBar = conn.CreateCommand())
                        {
                            cmndProdUnitBar.CommandText = "tProductUnitBarCode";
                            cmndProdUnitBar.CommandType = CommandType.TableDirect;
                            cmndProdUnitBar.IndexName = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";

                            using (SqliteDataReader drProdUnitBar = cmndProdUnitBar.ExecuteReader())
                            {
                                using (SQLiteCommand cmndProd = conn.CreateCommand())
                                {
                                    cmndProd.CommandText = "tProduct";
                                    cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                    cmndProd.CommandType = CommandType.TableDirect;

                                    using (SqliteDataReader drProd = cmndProd.ExecuteReader())
                                    {
                                        using (SQLiteCommand cmndUnit = conn.CreateCommand())
                                        {
                                            cmndUnit.CommandText = "tUnit";
                                            cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                                            cmndUnit.CommandType = CommandType.TableDirect;

                                            using (SqliteDataReader drUnit = cmndUnit.ExecuteReader())
                                            {
                                                using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
                                                {
                                                    cmndProdUnit.CommandText = "tProductUnit";
                                                    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                                                    cmndProdUnit.CommandType = CommandType.TableDirect;

                                                    using (SqliteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                                                    {
                                                        using (SqlCeResultSet rs = cmndProdUnitBar.ExecuteResultSet(ResultSetOptions.Updatable))
                                                        {
                                                            SqlCeUpdatableRecord rec = rs.CreateRecord();
                                                            int prodRefno = 0;
                                                            int prodUnitRefno = 0;
                                                            int unitRefno = 0;
                                                            int progValue = 0;
                                                            bool isProdUnitBarCodeFound = false;
                                                            int prodUnitBarCodeSymbologyEnum = 0;
                                                            string prodUnitBarCodeRaw = string.Empty;
                                                            int prodFKRefno = 0;
                                                            bool prodUnitBarCodeIsBC = false;
                                                            string unitCode = string.Empty;
                                                            string prodUnitBarCodeCode = string.Empty;
                                                            string prodUnitBarCodeModifier1 = string.Empty;

                                                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                                                            {
                                                                prodUnitBarCodeCode = drow["f04"].ToString();
                                                                prodUnitBarCodeSymbologyEnum = Convert.ToInt32(drow["f05"]);
                                                                prodUnitBarCodeModifier1 = drow["f06"].ToString();
                                                                prodUnitBarCodeRaw = string.Empty;

                                                                prodFKRefno = Convert.ToInt32(drow["f02"]);
                                                                errMsg = "FK Refno " + prodFKRefno;
                                                                if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                                                {
                                                                    if (drProd.Read())
                                                                    {
                                                                        prodRefno = Convert.ToInt32(drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                                                    }
                                                                    else
                                                                    {
#if MULTILANGUAGE
                                    throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodreadFailed) + Environment.NewLine + errMsg);
#else
                                                                        throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
#endif
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception(
#if MULTILANGUAGE
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct)
#else
"Unable to locate the product refno from the product fk refno in tProduct."
#endif
 + Environment.NewLine + errMsg);
                                                                }

                                                                unitCode = drow["f03"].ToString();
                                                                errMsg += Environment.NewLine + "Unit code " + unitCode;
                                                                if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                                                                {
                                                                    if (drUnit.Read())
                                                                    {
                                                                        unitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                                                                    }
                                                                    else
                                                                    {
#if MULTILANGUAGE
                                    throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg);
#else
                                                                        throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
#endif
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception(
#if MULTILANGUAGE
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheUnitCodeInTunit)
#else
"Unable to locate the unit refno from the unit code in tUnit"
#endif
 + Environment.NewLine + errMsg);
                                                                }

                                                                prodUnitBarCodeIsBC = Convert.ToBoolean(drow["f01"]);
                                                                errMsg += Environment.NewLine + "Bar code is bc " + prodUnitBarCodeIsBC;
                                                                if (drProdUnit.Seek(
                                                                    DbSeekOptions.FirstEqual,
                                                                    new object[] { prodRefno, unitRefno, prodUnitBarCodeIsBC }))
                                                                {
                                                                    if (drProdUnit.Read())
                                                                    {
                                                                        prodUnitRefno = Convert.ToInt32(
                                                                            drProdUnit.GetValue((int)SysCommon.ProdUnitFields.Refno));
                                                                    }
                                                                    else
                                                                    {
#if MULTILANGUAGE
                                    throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodunitreadFailed) + Environment.NewLine + errMsg);
#else
                                                                        throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
#endif
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception(
#if MULTILANGUAGE
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductRefnoUnitRefno)
                                      + Translator.GetInstance().Translate(TranslatorVocabulary.AndIsBcInTproductunit) + Environment.NewLine + errMsg);
#else
"Unable to locate the product unit refno from the product refno, unit refno "
                                                                        + "and is bc in tProductUnit" + Environment.NewLine + errMsg);
#endif
                                                                }

                                                                isProdUnitBarCodeFound = false;
                                                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                                                {
                                                                    cmndProdUnitBar.CommandText = "tProductUnitBarCode";
                                                                    cmndProdUnitBar.CommandType = CommandType.TableDirect;
                                                                    cmndProdUnitBar.IndexName =
                                                                        "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";

                                                                    if (drProdUnitBar.Seek(
                                                                        DbSeekOptions.FirstEqual,
                                                                        new object[] { prodUnitBarCodeCode, prodUnitBarCodeModifier1 }))
                                                                    {
                                                                        isProdUnitBarCodeFound = true;
                                                                        cmndProdUnitBar.CommandType = CommandType.Text;
                                                                        cmndProdUnitBar.IndexName = string.Empty;
                                                                        cmndProdUnitBar.CommandText = "UPDATE tProductUnitBarCode " +
                                                                            "SET ProductUnitRefno = " + prodUnitRefno + ", " +
                                                                            "ProductUnitBarCodeSymbologyEnum = " + prodUnitBarCodeSymbologyEnum + ", " +
                                                                            "ProductUnitBarCodeStatus = " + (int)SysCommon.ActStatus.NoChange + " " +
                                                                            "WHERE ProductUnitBarCodeCode = \'" + prodUnitBarCodeCode +
                                                                            "\' AND ProductUnitBarCodeModifier1 = \'" + prodUnitBarCodeModifier1 + "\'";
                                                                        cmndProdUnitBar.ExecuteNonQuery();
                                                                    }
                                                                }

                                                                if (!isProdUnitBarCodeFound)
                                                                {
                                                                    cmndProdUnitBar.CommandText = "tProductUnitBarCode";
                                                                    cmndProdUnitBar.CommandType = CommandType.TableDirect;
                                                                    cmndProdUnitBar.IndexName =
                                                                        "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";
                                                                    rec.SetInt32((int)SysCommon.ProdUnitBarCodeFields.ProdUnitRefno, prodUnitRefno);
                                                                    rec.SetString((int)SysCommon.ProdUnitBarCodeFields.Code, prodUnitBarCodeCode);
                                                                    rec.SetString((int)SysCommon.ProdUnitBarCodeFields.Raw, prodUnitBarCodeRaw);
                                                                    rec.SetInt32(
                                                                        (int)SysCommon.ProdUnitBarCodeFields.SymbologyEnum,
                                                                        prodUnitBarCodeSymbologyEnum);

                                                                    rec.SetString(
                                                                        (int)SysCommon.ProdUnitBarCodeFields.Modifier1,
                                                                        prodUnitBarCodeModifier1);
                                                                    rec.SetInt32(
                                                                        (int)SysCommon.ProdUnitBarCodeFields.Status,
                                                                        Convert.ToInt32(SysCommon.ActStatus.NoChange));

                                                                    rs.Insert(rec);
                                                                }

                                                                progValue++;
                                                                if (progValue % 100 == 0)
                                                                {
                                                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                                                }
                                                            }

                                                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                                                        }

                                                        drProdUnit.Close();
                                                    }
                                                }

                                                drUnit.Close();
                                            }
                                        }

                                        drProd.Close();
                                    }
                                }

                                drProdUnitBar.Close();
                            }
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomBarCodesComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomBarCodesFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes failed.", false);
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
          Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductUomBarCodesFailed) + Environment.NewLine + errMsg, 
#else
"Database exception occurred. Saving product uom bar codes failed." + Environment.NewLine + errMsg,
#endif
 ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected void SaveOpt(string reqData)
        {
            SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            bool refreshDataChangesOnly = SysEnv.RefreshOpt.RefreshDataChangesOnly;

            try
            {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif

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

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomBarCodes), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes.", false);
#endif


                    using (SQLiteCommand cmndProdUnitBar = conn.CreateCommand())
                    using (SQLiteCommand cmndProd = conn.CreateCommand())
                    using (SQLiteCommand cmndUnit = conn.CreateCommand())
                    using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
                    {
                        cmndProdUnitBar.CommandText = "tProductUnitBarCode";
                        cmndProdUnitBar.CommandType = CommandType.TableDirect;
                        cmndProdUnitBar.IndexName = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";

                        cmndProd.CommandText = "tProduct";
                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                        cmndProd.CommandType = CommandType.TableDirect;

                        cmndUnit.CommandText = "tUnit";
                        cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                        cmndUnit.CommandType = CommandType.TableDirect;

                        cmndProdUnit.CommandText = "tProductUnit";
                        cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                        cmndProdUnit.CommandType = CommandType.TableDirect;

                        using (SqliteDataReader drProdUnitBar = cmndProdUnitBar.ExecuteReader())
                        using (SqliteDataReader drProd = cmndProd.ExecuteReader())
                        using (SqliteDataReader drUnit = cmndUnit.ExecuteReader())
                        using (SqliteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                        using (SqlCeResultSet rs = cmndProdUnitBar.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                            SqlCeUpdatableRecord rec = rs.CreateRecord();
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

                            System.Collections.Generic.Dictionary<string, int> unitLookup = new System.Collections.Generic.Dictionary<string, int>();

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
                                if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                {
                                    if (drProd.Read())
                                    {
                                        prodRefno = Convert.ToInt32(drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                    }
                                    else
                                    {
#if MULTILANGUAGE
                                    throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodreadFailed) + Environment.NewLine + errMsg);
#else
                                        throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
#endif
                                    }
                                }
                                else
                                {
                                    throw new Exception(
#if MULTILANGUAGE
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct)
#else
"Unable to locate the product refno from the product fk refno in tProduct."
#endif
 + Environment.NewLine + errMsg);
                                }

                                errMsg += Environment.NewLine + "Unit code " + unitCode;
                                if (!unitLookup.TryGetValue(unitCode, out unitRefno))
                                {
                                    if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                                    {
                                        if (drUnit.Read())
                                        {
                                            unitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                                            unitLookup.Add(unitCode, unitRefno);
                                        }
                                        else
                                        {
#if MULTILANGUAGE
                                    throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg);
#else
                                            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
#endif
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheUnitCodeInTunit)
#else
"Unable to locate the unit refno from the unit code in tUnit"
#endif
 + Environment.NewLine + errMsg);
                                    }
                                }

                                errMsg += Environment.NewLine + "Bar code is bc " + prodUnitBarCodeIsBC;
                                if (drProdUnit.Seek(
                                    DbSeekOptions.FirstEqual,
                                    new object[] { prodRefno, unitRefno, prodUnitBarCodeIsBC }))
                                {
                                    if (drProdUnit.Read())
                                    {
                                        prodUnitRefno = Convert.ToInt32(
                                            drProdUnit.GetValue((int)SysCommon.ProdUnitFields.Refno));
                                    }
                                    else
                                    {
#if MULTILANGUAGE
                                    throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodunitreadFailed) + Environment.NewLine + errMsg);
#else
                                        throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
#endif
                                    }
                                }
                                else
                                {
                                    throw new Exception(
#if MULTILANGUAGE
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductRefnoUnitRefno)
                                      + Translator.GetInstance().Translate(TranslatorVocabulary.AndIsBcInTproductunit) + Environment.NewLine + errMsg);
#else
"Unable to locate the product unit refno from the product refno, unit refno "
                                        + "and is bc in tProductUnit" + Environment.NewLine + errMsg);
#endif
                                }

                                isProdUnitBarCodeFound = false;
                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    cmndProdUnitBar.CommandText = "tProductUnitBarCode";
                                    cmndProdUnitBar.CommandType = CommandType.TableDirect;
                                    cmndProdUnitBar.IndexName =
                                        "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";

                                    if (drProdUnitBar.Seek(
                                        DbSeekOptions.FirstEqual,
                                        new object[] { prodUnitBarCodeCode, prodUnitBarCodeModifier1 }))
                                    {
                                        isProdUnitBarCodeFound = true;
                                        cmndProdUnitBar.CommandType = CommandType.Text;
                                        cmndProdUnitBar.IndexName = string.Empty;
                                        cmndProdUnitBar.CommandText = "UPDATE tProductUnitBarCode " +
                                            "SET ProductUnitRefno = " + prodUnitRefno + ", " +
                                            "ProductUnitBarCodeSymbologyEnum = " + prodUnitBarCodeSymbologyEnum + ", " +
                                            "ProductUnitBarCodeStatus = " + (int)SysCommon.ActStatus.NoChange + " " +
                                            "WHERE ProductUnitBarCodeCode = \'" + prodUnitBarCodeCode +
                                            "\' AND ProductUnitBarCodeModifier1 = \'" + prodUnitBarCodeModifier1 + "\'";
                                        cmndProdUnitBar.ExecuteNonQuery();
                                    }
                                }

                                if (!isProdUnitBarCodeFound)
                                {
                                    cmndProdUnitBar.CommandText = "tProductUnitBarCode";
                                    cmndProdUnitBar.CommandType = CommandType.TableDirect;
                                    cmndProdUnitBar.IndexName =
                                        "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";
                                    rec.SetInt32((int)SysCommon.ProdUnitBarCodeFields.ProdUnitRefno, prodUnitRefno);
                                    rec.SetString((int)SysCommon.ProdUnitBarCodeFields.Code, prodUnitBarCodeCode);
                                    rec.SetString((int)SysCommon.ProdUnitBarCodeFields.Raw, prodUnitBarCodeRaw);
                                    rec.SetInt32(
                                        (int)SysCommon.ProdUnitBarCodeFields.SymbologyEnum,
                                        prodUnitBarCodeSymbologyEnum);

                                    rec.SetString(
                                        (int)SysCommon.ProdUnitBarCodeFields.Modifier1,
                                        prodUnitBarCodeModifier1);
                                    rec.SetInt32(
                                        (int)SysCommon.ProdUnitBarCodeFields.Status,
                                        Convert.ToInt32(SysCommon.ActStatus.NoChange));

                                    rs.Insert(rec);
                                }

                                ++recordCount;
                                if (recordCount % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                                }
                            } // end of data


                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), recordCount),
#else
 string.Format("Records {0}", recordCount),
#endif
 false);

                            unitLookup.Clear();
                            drProdUnit.Close();
                            drUnit.Close();
                            drProd.Close();
                            drProdUnitBar.Close();
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomBarCodesComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomBarCodesFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes failed.", false);
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
          Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductUomBarCodesFailed) + Environment.NewLine + errMsg, 
#else
"Database exception occurred. Saving product uom bar codes failed." + Environment.NewLine + errMsg,
#endif
 ex);
            }
        }


        /// <summary>
        /// 
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

        #endregion
    }
}
