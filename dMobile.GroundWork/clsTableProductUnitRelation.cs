namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading;

    public class TableProductUnitRelation : TableBase
    {
        #region Constructors

        public TableProductUnitRelation()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnitRelation";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomRelations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations.", true);
#endif

                using (SqlCeCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT tFamily.* FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
                    using (SqlCeDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        string family = string.Empty;

                        while (drFamily.Read())
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            family = drFamily["FamilyCode"].ToString();

                            string recvData = SendRequest(family);

#if MULTILANGUAGE
              this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ForFamily), family), true);
#else
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family), true);
#endif

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
                    }
                }


                // Require to have product uom relations.
                if (0 >= this.DataStatus())
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.NoProductUomRelationsWhereReceived), true);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "No product uom relations where received.", true);
#endif

#if MULTILANGUAGE
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.NoProductUomRelationsWhereReceived));
#else
                    throw new Exception("No product uom relations where received.");
#endif
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomRelationsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomRelationsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingProductUomRelationsFailed), ex);
#else
                throw new Exception("Database exception occurred. Seeding product uom relations failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomRelationsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomRelationsFailed), ex);
#else
                throw new Exception("Seeding product uom relations failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomRelations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations.", true);
#endif

                using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductUnitRelation";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomRelationsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomRelationsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductUomRelationsFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging product uom relations failed.", ex);
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
                using (SqlCeCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductUnitRelation";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductUnitRelations), rowsAffected),
#else
 string.Format("{0} product unit relations.", rowsAffected),
#endif
 false);
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductUnitRelationsStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Product unit relations status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFamilyCode"></param>
        /// <returns></returns>
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomRelations), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations.", false);
#endif
                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

                        using (SqlCeCommand cmndProdUnitRel = conn.CreateCommand())
                        {
                            cmndProdUnitRel.CommandText = "tProductUnitRelation";
                            cmndProdUnitRel.CommandType = CommandType.TableDirect;
                            cmndProdUnitRel.IndexName = "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";

                            using (SqlCeDataReader drProdUnitRel = cmndProdUnitRel.ExecuteReader())
                            {
                                using (SqlCeCommand cmndProd = conn.CreateCommand())
                                {
                                    cmndProd.CommandText = "tProduct";
                                    cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                    cmndProd.CommandType = CommandType.TableDirect;

                                    using (SqlCeDataReader drProd = cmndProd.ExecuteReader())
                                    {
                                        using (SqlCeCommand cmndUnit = conn.CreateCommand())
                                        {
                                            cmndUnit.CommandText = "tUnit";
                                            cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                                            cmndUnit.CommandType = CommandType.TableDirect;

                                            using (SqlCeDataReader drUnit = cmndUnit.ExecuteReader())
                                            {
                                                using (SqlCeCommand cmndProdUnit = conn.CreateCommand())
                                                {
                                                    cmndProdUnit.CommandText = "tProductUnit";
                                                    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                                                    cmndProdUnit.CommandType = CommandType.TableDirect;

                                                    using (SqlCeDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                                                    {
                                                        using (SqlCeResultSet rs = cmndProdUnitRel.ExecuteResultSet(ResultSetOptions.Updatable))
                                                        {
                                                            SqlCeUpdatableRecord rec = rs.CreateRecord();
                                                            int prodRefno = 0;
                                                            int unitCode1Refno = 0;
                                                            int unitCode2Refno = 0;
                                                            int progValue = 0;
                                                            bool isProdUnitRelationFound = false;
                                                            int prodUnit1Refno = 0;
                                                            int prodUnit2Refno = 0;
                                                            int prodFKRefno = 0;
                                                            bool prodUnitIsBC = false;
                                                            string unitCode1 = string.Empty;
                                                            string unitCode2 = string.Empty;

                                                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                                                            {
                                                                prodFKRefno = Convert.ToInt32(drow["f03"]);
                                                                errMsg = "FK refno " + prodFKRefno;
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

                                                                unitCode1 = drow["f04"].ToString();
                                                                errMsg += Environment.NewLine + "Unit code 1 " + unitCode1;
                                                                if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode1 }))
                                                                {
                                                                    if (drUnit.Read())
                                                                    {
                                                                        unitCode1Refno = Convert.ToInt32(
                                                                          drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitz)
#else
"Unable to locate the unit refno from the unit code 1 in tUnit."
#endif
 + Environment.NewLine + errMsg);
                                                                }

                                                                prodUnitIsBC = Convert.ToBoolean(drow["f01"]);
                                                                errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

                                                                if (drProdUnit.Seek(
                                                                    DbSeekOptions.FirstEqual,
                                                                    new object[] { prodRefno, unitCode1Refno, prodUnitIsBC }))
                                                                {
                                                                    if (drProdUnit.Read())
                                                                    {
                                                                        prodUnit1Refno = Convert.ToInt32(
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductRefno)
                                      + Translator.GetInstance().Translate(TranslatorVocabulary.UnitCodeRefnoAndIsBcInTproductunit) + Environment.NewLine + errMsg);
#else
"Unable to locate the product unit refno from the product refno, "
                                                                        + "unit code 1 refno and is bc in tProductUnit." + Environment.NewLine + errMsg);
#endif
                                                                }

                                                                unitCode2 = drow["f05"].ToString();
                                                                errMsg += Environment.NewLine + "Unit code 2 " + unitCode2;
                                                                if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode2 }))
                                                                {
                                                                    if (drUnit.Read())
                                                                    {
                                                                        unitCode2Refno = Convert.ToInt32(
                                                                          drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitzz)
#else
"Unable to locate the unit refno from the unit code 2 in tUnit."
#endif
 + Environment.NewLine + errMsg);
                                                                }

                                                                if (drProdUnit.Seek(
                                                                    DbSeekOptions.FirstEqual,
                                                                    new object[] { prodRefno, unitCode2Refno, prodUnitIsBC }))
                                                                {
                                                                    if (drProdUnit.Read())
                                                                    {
                                                                        prodUnit2Refno = Convert.ToInt32(
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductRefno)
                                      + Translator.GetInstance().Translate(TranslatorVocabulary.UnitCodeRefnoAndIsBcInTproductunitz) + Environment.NewLine + errMsg);
#else
"Unable to locate the product unit refno from the product refno, "
                                                                        + "unit code 2 refno and is bc in tProductUnit." + Environment.NewLine + errMsg);
#endif
                                                                }

                                                                isProdUnitRelationFound = false;
                                                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                                                {
                                                                    cmndProdUnitRel.CommandText = "tProductUnitRelation";
                                                                    cmndProdUnitRel.CommandType = CommandType.TableDirect;
                                                                    cmndProdUnitRel.IndexName =
                                                                        "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";

                                                                    if (drProdUnitRel.Seek(
                                                                        DbSeekOptions.FirstEqual,
                                                                        new object[] { prodUnit1Refno, prodUnit2Refno }))
                                                                    {
                                                                        isProdUnitRelationFound = true;
                                                                        cmndProdUnitRel.CommandType = CommandType.Text;
                                                                        cmndProdUnitRel.IndexName = string.Empty;
                                                                        cmndProdUnitRel.CommandText = "UPDATE tProductUnitRelation "
                                                                          + "SET ProductUnitRelationFactor = " + Convert.ToDecimal(drow["f02"])
                                                                          + " WHERE ProductUnit1Refno = " + prodUnit1Refno
                                                                          + " AND ProductUnit2Refno = " + prodUnit2Refno;
                                                                        cmndProdUnitRel.ExecuteNonQuery();
                                                                    }
                                                                }

                                                                if (!isProdUnitRelationFound)
                                                                {
                                                                    cmndProdUnitRel.CommandText = "tProductUnitRelation";
                                                                    cmndProdUnitRel.CommandType = CommandType.TableDirect;
                                                                    cmndProdUnitRel.IndexName =
                                                                        "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";

                                                                    rec.SetInt32((int)SysCommon.ProdUnitRelationFields.ProdUnit1Refno, prodUnit1Refno);
                                                                    rec.SetInt32((int)SysCommon.ProdUnitRelationFields.ProdUnit2Refno, prodUnit2Refno);
                                                                    rec.SetDecimal(
                                                                      (int)SysCommon.ProdUnitRelationFields.Factor,
                                                                      Convert.ToDecimal(drow["f02"]));

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

                                drProdUnitRel.Close();
                            }
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomRelationsComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomRelationsFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations failed.", false);
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

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductUomRelationsFailed) + errMsg, ex);
#else
                throw new Exception("Database exception occurred. Saving product uom relations failed." + errMsg, ex);
#endif
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
            int recordCount = 0;
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomRelations), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations.", false);
#endif



                    using (SqlCeCommand cmndProdUnitRel = conn.CreateCommand())
                    using (SqlCeCommand cmndProd = conn.CreateCommand())
                    using (SqlCeCommand cmndUnit = conn.CreateCommand())
                    using (SqlCeCommand cmndProdUnit = conn.CreateCommand())
                    {
                        cmndProdUnitRel.CommandText = "tProductUnitRelation";
                        cmndProdUnitRel.CommandType = CommandType.TableDirect;
                        cmndProdUnitRel.IndexName = "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";

                        cmndProd.CommandText = "tProduct";
                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                        cmndProd.CommandType = CommandType.TableDirect;

                        cmndUnit.CommandText = "tUnit";
                        cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                        cmndUnit.CommandType = CommandType.TableDirect;

                        cmndProdUnit.CommandText = "tProductUnit";
                        cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                        cmndProdUnit.CommandType = CommandType.TableDirect;

                        using (SqlCeDataReader drProdUnitRel = cmndProdUnitRel.ExecuteReader())
                        using (SqlCeDataReader drProd = cmndProd.ExecuteReader())
                        using (SqlCeDataReader drUnit = cmndUnit.ExecuteReader())
                        using (SqlCeDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                        using (SqlCeResultSet rs = cmndProdUnitRel.ExecuteResultSet(ResultSetOptions.Updatable))
                        {

                            Dictionary<string, int> unitLookup = new Dictionary<string, int>();
                            int lastFKRefno = 0;
                            bool refreshOnly = SysEnv.RefreshOpt.RefreshDataChangesOnly;

                            SqlCeUpdatableRecord rec = rs.CreateRecord();
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
                                    continue;

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
                                }

                                errMsg += Environment.NewLine + "Unit code 1 " + unitCode1;
                                if (!unitLookup.TryGetValue(unitCode1, out unitCode1Refno))
                                {
                                    if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode1 }))
                                    {
                                        if (drUnit.Read())
                                        {
                                            unitCode1Refno = Convert.ToInt32(
                                              drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                                            unitLookup.Add(unitCode1, unitCode1Refno);
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitz)
#else
"Unable to locate the unit refno from the unit code 1 in tUnit."
#endif
 + Environment.NewLine + errMsg);
                                    }
                                }

                                errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

                                if (drProdUnit.Seek(
                                    DbSeekOptions.FirstEqual,
                                    new object[] { prodRefno, unitCode1Refno, prodUnitIsBC }))
                                {
                                    if (drProdUnit.Read())
                                    {
                                        prodUnit1Refno = Convert.ToInt32(
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductRefno)
                                      + Translator.GetInstance().Translate(TranslatorVocabulary.UnitCodeRefnoAndIsBcInTproductunit) + Environment.NewLine + errMsg);
#else
"Unable to locate the product unit refno from the product refno, "
                                        + "unit code 1 refno and is bc in tProductUnit." + Environment.NewLine + errMsg);
#endif
                                }

                                errMsg += Environment.NewLine + "Unit code 2 " + unitCode2;
                                if (!unitLookup.TryGetValue(unitCode2, out unitCode2Refno))
                                {
                                    if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode2 }))
                                    {
                                        if (drUnit.Read())
                                        {
                                            unitCode2Refno = Convert.ToInt32(
                                              drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                                            unitLookup.Add(unitCode2, unitCode2Refno);
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitzz)
#else
"Unable to locate the unit refno from the unit code 2 in tUnit."
#endif
 + Environment.NewLine + errMsg);
                                    }
                                }

                                if (drProdUnit.Seek(
                                    DbSeekOptions.FirstEqual,
                                    new object[] { prodRefno, unitCode2Refno, prodUnitIsBC }))
                                {
                                    if (drProdUnit.Read())
                                    {
                                        prodUnit2Refno = Convert.ToInt32(
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
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductRefno)
                                      + Translator.GetInstance().Translate(TranslatorVocabulary.UnitCodeRefnoAndIsBcInTproductunitz) + Environment.NewLine + errMsg);
#else
"Unable to locate the product unit refno from the product refno, "
                                        + "unit code 2 refno and is bc in tProductUnit." + Environment.NewLine + errMsg);
#endif
                                }

                                isProdUnitRelationFound = false;
                                if (refreshOnly)
                                {
                                    cmndProdUnitRel.CommandText = "tProductUnitRelation";
                                    cmndProdUnitRel.CommandType = CommandType.TableDirect;
                                    cmndProdUnitRel.IndexName =
                                        "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";

                                    if (drProdUnitRel.Seek(
                                        DbSeekOptions.FirstEqual,
                                        new object[] { prodUnit1Refno, prodUnit2Refno }))
                                    {
                                        isProdUnitRelationFound = true;
                                        cmndProdUnitRel.CommandType = CommandType.Text;
                                        cmndProdUnitRel.IndexName = string.Empty;
                                        cmndProdUnitRel.CommandText = "UPDATE tProductUnitRelation "
                                          + "SET ProductUnitRelationFactor = " + relationFactor
                                          + " WHERE ProductUnit1Refno = " + prodUnit1Refno
                                          + " AND ProductUnit2Refno = " + prodUnit2Refno;
                                        cmndProdUnitRel.ExecuteNonQuery();
                                    }
                                }

                                if (!isProdUnitRelationFound)
                                {
                                    cmndProdUnitRel.CommandText = "tProductUnitRelation";
                                    cmndProdUnitRel.CommandType = CommandType.TableDirect;
                                    cmndProdUnitRel.IndexName =
                                        "PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno";

                                    rec.SetInt32((int)SysCommon.ProdUnitRelationFields.ProdUnit1Refno, prodUnit1Refno);
                                    rec.SetInt32((int)SysCommon.ProdUnitRelationFields.ProdUnit2Refno, prodUnit2Refno);
                                    rec.SetDecimal((int)SysCommon.ProdUnitRelationFields.Factor, relationFactor);

                                    rs.Insert(rec);
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

                            drProdUnit.Close();
                            drUnit.Close();
                            drProd.Close();
                            drProdUnitRel.Close();
                        }
                    }



#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomRelationsComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomRelationsFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations failed.", false);
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

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductUomRelationsFailed) + errMsg, ex);
#else
                throw new Exception("Database exception occurred. Saving product uom relations failed." + errMsg, ex);
#endif
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

        #endregion
    }
}
