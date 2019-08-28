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

    public class TableProductUnit : TableBase
    {
        #region Constructors

        public TableProductUnit()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnit";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUom), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom.", true);
#endif

                using (SqlCeCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT tFamily.* FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
                    using (SqlCeDataReader drFamily = cmndFamily.ExecuteReader())
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

                                //this.Save(reqData);
                                this.SaveOpt(reqData);
                            }
                        }
                    }
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingProductUomFailed), ex);
#else
                throw new Exception("Database exception occurred. Seeding product uom failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductUomFailed), ex);
#else
                throw new Exception("Seeding product uom failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUom), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom.", true);
#endif

                using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductUnit";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductUomFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging product uom failed.", ex);
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodRefno"></param>
        /// <param name="unitRefno"></param>
        /// <param name="prodUnitIsBC"></param>
        /// <returns></returns>
        public int GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC(
            int prodRefno,
            int unitRefno,
            bool prodUnitIsBC)
        {
            int prodUnitRefno = 0;

            try
            {
                using (SqlCeCommand cmndGetProductUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndGetProductUnit.CommandText = "tProductUnit";
                    cmndGetProductUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                    cmndGetProductUnit.CommandType = CommandType.TableDirect;

                    using (SqlCeDataReader drProductUnit = cmndGetProductUnit.ExecuteReader())
                    {
                        if (drProductUnit.Seek(
                            DbSeekOptions.FirstEqual,
                            new object[] { prodRefno, unitRefno, prodUnitIsBC }))
                        {
                            if (drProductUnit.Read())
                            {
                                prodUnitRefno = Convert.ToInt32(drProductUnit.GetValue(Convert.ToInt32(
                                    SysCommon.ProdUnitFields.Refno)));
                            }
                            else
                            {
#if MULTILANGUAGE
                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrproductunitreadFailed));
#else
                                throw new Exception("drProductUnit.Read() failed.");
#endif
                            }
                        }
                        else
                        {
                            throw new Exception(
                                string.Format(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductUnitRefnoFromProductRefnoAndUnitRefnoAndIsBc) +
                  Translator.GetInstance().Translate(TranslatorVocabulary.InTproductunit),
#else
"Unable to locate product unit refno from product refno and unit refno and is BC {0}, {1}, {2} " +
                                "in tProductUnit",
#endif
 prodRefno,
                                prodUnitRefno,
                                prodUnitIsBC));
                        }

                        drProductUnit.Close();
                    }
                }
            }
            catch (SqlCeException ex)
            {
                throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodunitrefnofromprodrefnoandunitrefnoandprodunitisbcFailed), ex);
#else
"Database exception occurred. GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC failed.", ex);
#endif
            }

            return prodUnitRefno;
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductUnit";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductUnits), rowsAffected),
#else
 string.Format("{0} product units.", rowsAffected),
#endif
 true);
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductUomStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Product uom status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override string SendRequest(string familyCode)
        {
            return this.SendComm(this.CreateRequest(familyCode));
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUom), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom.", false);
#endif
                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

                        using (SqlCeCommand cmndProdUnit = conn.CreateCommand())
                        {
                            cmndProdUnit.CommandText = "tProductUnit";
                            cmndProdUnit.CommandType = CommandType.TableDirect;
                            cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                            using (SqlCeDataReader drProdUnit = cmndProdUnit.ExecuteReader())
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
                                                using (SqlCeResultSet rs = cmndProdUnit.ExecuteResultSet(ResultSetOptions.Updatable))
                                                {
                                                    SqlCeUpdatableRecord rec = rs.CreateRecord();

                                                    int progValue = 0;
                                                    int prodRefno = 0;
                                                    int unitRefno = 0;
                                                    int baseUnitRefno = 0;
                                                    int wtUnitRefno = 0;
                                                    bool isProdUnitFound = false;
                                                    string baseUnitCode = string.Empty;
                                                    string wtUnitCode = string.Empty;
                                                    bool isProdUnitSellUnit = false;
                                                    bool isProdUnitIsDefault = false;
                                                    int prodFKRefno = 0;
                                                    string prodCode = string.Empty;
                                                    string unitCode = string.Empty;
                                                    bool isProdUnitBC = false;
                                                    decimal boxCount = 0;

                                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);
                                                    foreach (DataRow drow in ds.Tables["seg"].Rows)
                                                    {
                                                        prodCode = string.Empty;

                                                        prodFKRefno = Convert.ToInt32(drow["f04"]);
                                                        errMsg = "FK refno: " + prodFKRefno;

                                                        isProdUnitSellUnit = Convert.ToBoolean(drow["f08"]);
                                                        errMsg += Environment.NewLine + "Is sell unit: " + isProdUnitSellUnit;

                                                        isProdUnitBC = Convert.ToBoolean(drow["f01"]);
                                                        errMsg += Environment.NewLine + "Is bc: " + isProdUnitBC;

                                                        isProdUnitIsDefault = Convert.ToBoolean(drow["f09"]);
                                                        errMsg += Environment.NewLine + "Is default: " + isProdUnitIsDefault;

                                                        boxCount = Convert.ToDecimal(drow["f10"]);
                                                        errMsg += Environment.NewLine + "Box count: " + boxCount;

                                                        if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                                        {
                                                            if (drProd.Read())
                                                            {
                                                                prodRefno = Convert.ToInt32(drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                                                prodCode = Convert.ToString(drProd.GetValue((int)SysCommon.ProductFields.Code));
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
#if MULTILANGUAGE
                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoUsingTheProductFkRefno) 
                                  + Translator.GetInstance().Translate(TranslatorVocabulary.InTproduct) + Environment.NewLine + errMsg);
#else
                                                            throw new Exception("Unable to locate the product refno using the product FK refno "
                                                                + "in tProduct." + Environment.NewLine + errMsg);
#endif
                                                        }

                                                        errMsg += Environment.NewLine + "Refno: \'" + prodRefno + "\'";
                                                        errMsg += Environment.NewLine + "Code: \'" + prodCode + "\'";

                                                        unitCode = drow["f05"].ToString();
                                                        errMsg += Environment.NewLine + "Unit: \'" + unitCode + "\'";
                                                        if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                                                        {
                                                            if (drUnit.Read())
                                                            {
                                                                unitRefno = Convert.ToInt32(drUnit.GetValue(
                                                                    Convert.ToInt32(SysCommon.UnitFields.UnitRefno)));
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
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoUsingTheUnitCodeInTunit) 
#else
"Unable to locate the unit refno using the unit code in tUnit."
#endif
 + Environment.NewLine + errMsg);
                                                        }

                                                        baseUnitCode = drow["f06"].ToString();
                                                        errMsg += Environment.NewLine + "Base unit: \'" + baseUnitCode + "\'";
                                                        if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { baseUnitCode }))
                                                        {
                                                            if (drUnit.Read())
                                                            {
                                                                baseUnitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
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
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoUsingTheBaseUnitCodeInTunit) 
#else
"Unable to locate the unit refno using the base unit code in tUnit."
#endif
 + Environment.NewLine + errMsg);
                                                        }

                                                        wtUnitCode = drow["f07"].ToString();
                                                        errMsg += Environment.NewLine + "Weight unit: \'" + wtUnitCode + "\'";
                                                        if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { wtUnitCode }))
                                                        {
                                                            if (drUnit.Read())
                                                            {
                                                                wtUnitRefno = Convert.ToInt32(
                                                                    drUnit.GetValue(Convert.ToInt32(
                                                                    SysCommon.UnitFields.UnitRefno)));
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
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoUsingTheWeightUnitCodeInTunit)
#else
"Unable to locate the unit refno using the weight unit code in tUnit."
#endif
 + Environment.NewLine + errMsg);
                                                        }

                                                        isProdUnitFound = false;

                                                        if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                                        {
                                                            cmndProdUnit.CommandText = "tProductUnit";
                                                            cmndProdUnit.CommandType = CommandType.TableDirect;
                                                            cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                                                            if (drProdUnit.Seek(
                                                                DbSeekOptions.FirstEqual,
                                                                new object[] { prodRefno, unitRefno, isProdUnitBC }))
                                                            {
                                                                isProdUnitFound = true;
                                                                cmndProdUnit.CommandType = CommandType.Text;
                                                                cmndProdUnit.IndexName = string.Empty;
                                                                cmndProdUnit.Parameters.Clear();
                                                                cmndProdUnit.Parameters.AddWithValue("@Desc", drow["f03"].ToString());

                                                                StringBuilder s = new StringBuilder();
                                                                s.Append("UPDATE tProductUnit ");
                                                                s.AppendFormat("SET BaseUnitRefno = {0},", baseUnitRefno);
                                                                s.AppendFormat("WtUnitRefno = {0},", wtUnitRefno);
                                                                s.AppendFormat("ProductUnitWeight = {0},", Convert.ToDecimal(drow["f02"]));
                                                                s.Append("ProductUnitSizeDescription = @Desc,");
                                                                s.AppendFormat("ProductUnitIsSellUnit = {0},", isProdUnitSellUnit ? 1 : 0);
                                                                s.AppendFormat("ProductUnitIsDefault = {0},", isProdUnitIsDefault ? 1 : 0);
                                                                s.AppendFormat("ProductUnitBoxCount = {0}", boxCount);
                                                                s.AppendFormat(" WHERE ProductRefno = {0}", prodRefno);
                                                                s.AppendFormat(" AND UnitRefno = {0}", unitRefno);
                                                                s.AppendFormat(" AND ProductUnitIsBC = {0}", isProdUnitBC ? 1 : 0);

                                                                cmndProdUnit.CommandText = s.ToString();

                                                                cmndProdUnit.ExecuteNonQuery();
                                                            }
                                                        }

                                                        if (!isProdUnitFound)
                                                        {
                                                            cmndProdUnit.CommandText = "tProductUnit";
                                                            cmndProdUnit.CommandType = CommandType.TableDirect;
                                                            cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                                                            rec.SetInt32((int)SysCommon.ProdUnitFields.ProdRefno, prodRefno);
                                                            rec.SetInt32((int)SysCommon.ProdUnitFields.UnitRefno, unitRefno);
                                                            rec.SetInt32((int)SysCommon.ProdUnitFields.BaseUnitRefno, baseUnitRefno);
                                                            rec.SetInt32((int)SysCommon.ProdUnitFields.WtUnitRefno, wtUnitRefno);
                                                            rec.SetBoolean((int)SysCommon.ProdUnitFields.IsBC, isProdUnitBC);
                                                            rec.SetString(
                                                                (int)SysCommon.ProdUnitFields.BC, isProdUnitBC ? "X" : " ");
                                                            rec.SetDecimal(
                                                                (int)SysCommon.ProdUnitFields.Weight,
                                                                Convert.ToDecimal(drow["f02"]));
                                                            rec.SetString(
                                                                (int)SysCommon.ProdUnitFields.SizeDescription,
                                                                drow["f03"].ToString());
                                                            rec.SetBoolean(
                                                                (int)SysCommon.ProdUnitFields.IsSellUnit,
                                                                isProdUnitSellUnit);
                                                            rec.SetBoolean(
                                                                (int)SysCommon.ProdUnitFields.IsDefault,
                                                                isProdUnitIsDefault);
                                                            rec.SetDecimal(
                                                                (int)SysCommon.ProdUnitFields.BoxCount,
                                                                boxCount);
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

                                                drUnit.Close();
                                            }
                                        }

                                        drProd.Close();
                                    }
                                }

                                drProdUnit.Close();
                            }
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductUomFailed) + Environment.NewLine + errMsg, 
#else
"Database exception occurred. Saving product uom failed." + Environment.NewLine + errMsg,
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
            //string errMsg = string.Empty;
            StringBuilder errMsg = new StringBuilder();
            int recordCount = 0;
            try
            {
                int startIndex = reqData.IndexOf("<VFPData");
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif
                using (StringReader stringReader = new StringReader(reqData.Substring(startIndex)))
                using (XmlReader reader = XmlReader.Create(stringReader))
                {

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUom), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom.", false);
#endif

                    using (SqlCeCommand cmndProdUnit = conn.CreateCommand())
                    using (SqlCeCommand cmndProd = conn.CreateCommand())
                    using (SqlCeCommand cmndUnit = conn.CreateCommand())
                    {
                        cmndProdUnit.CommandText = "tProductUnit";
                        cmndProdUnit.CommandType = CommandType.TableDirect;
                        cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                        cmndProd.CommandText = "tProduct";
                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                        cmndProd.CommandType = CommandType.TableDirect;

                        cmndUnit.CommandText = "tUnit";
                        cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                        cmndUnit.CommandType = CommandType.TableDirect;

                        using (SqlCeDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                        using (SqlCeDataReader drProd = cmndProd.ExecuteReader())
                        using (SqlCeDataReader drUnit = cmndUnit.ExecuteReader())
                        using (SqlCeResultSet rs = cmndProdUnit.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                            SqlCeUpdatableRecord rec = rs.CreateRecord();

                            int prodRefno = 0;
                            int unitRefno = 0;
                            int baseUnitRefno = 0;
                            int wtUnitRefno = 0;
                            bool isProdUnitFound = false;
                            string prodCode = string.Empty;


                            Dictionary<string, int> unitLookup = new Dictionary<string, int>();

                            // f01
                            bool isProdUnitBC = false;
                            // f02
                            decimal prodUnitWeight = 0;
                            // f03
                            string description = string.Empty;
                            // f04
                            int prodFKRefno = 0;
                            // f05
                            string unitCode = string.Empty;
                            // f06
                            string baseUnitCode = string.Empty;
                            // f07
                            string wtUnitCode = string.Empty;
                            // f08
                            bool isProdUnitSellUnit = false;
                            // f09
                            bool isProdUnitDefault = false;
                            // f10
                            decimal boxCount = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);
                            while (!reader.EOF)
                            {
                                bool recordFilled = false;
                                string elementName = string.Empty;
                                while (!recordFilled && reader.Read())
                                {
                                    // fill one record
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        elementName = reader.Name;
                                    }
                                    else if (reader.NodeType == XmlNodeType.Text)
                                    {
                                        switch (elementName)
                                        {
                                            case "f01":
                                                isProdUnitBC = reader.ReadContentAsBoolean();
                                                break;
                                            case "f02":
                                                prodUnitWeight = reader.ReadContentAsDecimal();
                                                break;
                                            case "f03":
                                                description = reader.Value;
                                                break;
                                            case "f04":
                                                prodFKRefno = reader.ReadContentAsInt();
                                                break;
                                            case "f05":
                                                unitCode = reader.Value;
                                                break;
                                            case "f06":
                                                baseUnitCode = reader.Value;
                                                break;
                                            case "f07":
                                                wtUnitCode = reader.Value;
                                                break;
                                            case "f08":
                                                isProdUnitSellUnit = reader.ReadContentAsBoolean();
                                                break;
                                            case "f09":
                                                isProdUnitDefault = reader.ReadContentAsBoolean();
                                                break;
                                            case "f10":
                                                boxCount = reader.ReadContentAsDecimal();
                                                break;
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
                                    {
                                        recordFilled = true; // we are full;
                                    }
                                } // end of fill record

                                if (!recordFilled)
                                    continue; // prevent the last record (empty) from being used (set to last due to no overwriting)

                                prodCode = string.Empty;

                                errMsg.Length = 0;
                                errMsg.Append("FK refno: ").Append(prodFKRefno);

                                errMsg.Append(Environment.NewLine).Append("Is sell unit: ").Append(isProdUnitSellUnit);
                                errMsg.Append(Environment.NewLine).Append("Is bc: ").Append(isProdUnitBC);
                                errMsg.Append(Environment.NewLine).Append("Is default: ").Append(isProdUnitDefault);
                                errMsg.Append(Environment.NewLine).Append("Box count: ").Append(boxCount);


                                if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                {
                                    if (drProd.Read())
                                    {
                                        prodRefno = Convert.ToInt32(drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                        prodCode = Convert.ToString(drProd.GetValue((int)SysCommon.ProductFields.Code));
                                    }
                                    else
                                    {
#if MULTILANGUAGE
                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodreadFailed) + Environment.NewLine + errMsg.ToString());
#else
                                        throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg.ToString());
#endif
                                    }
                                }
                                else
                                {
#if MULTILANGUAGE
                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoUsingTheProductFkRefno) 
                                  + Translator.GetInstance().Translate(TranslatorVocabulary.InTproduct) + Environment.NewLine + errMsg.ToString());
#else
                                    throw new Exception("Unable to locate the product refno using the product FK refno "
                                        + "in tProduct." + Environment.NewLine + errMsg.ToString());
#endif
                                }

                                errMsg.Append(Environment.NewLine).Append("Refno: \'").Append(prodRefno).Append("\'");
                                errMsg.Append(Environment.NewLine).Append("Code: \'").Append(prodCode).Append("\'");
                                errMsg.Append(Environment.NewLine).Append("Unit: \'").Append(unitCode).Append("\'");
                                if (!unitLookup.TryGetValue(unitCode, out unitRefno))
                                {
                                    if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                                    {
                                        if (drUnit.Read())
                                        {
                                            unitRefno = Convert.ToInt32(drUnit.GetValue(
                                                Convert.ToInt32(SysCommon.UnitFields.UnitRefno)));
                                            unitLookup.Add(unitCode, unitRefno);
                                        }
                                        else
                                        {
#if MULTILANGUAGE
                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg.ToString());
#else
                                            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg.ToString());
#endif
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoUsingTheUnitCodeInTunit) 
#else
"Unable to locate the unit refno using the unit code in tUnit."
#endif
 + Environment.NewLine + errMsg.ToString());
                                    }
                                }


                                errMsg.Append(Environment.NewLine).Append("Base unit: \'").Append(baseUnitCode).Append("\'");
                                if (!unitLookup.TryGetValue(baseUnitCode, out baseUnitRefno))
                                {
                                    if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { baseUnitCode }))
                                    {
                                        if (drUnit.Read())
                                        {
                                            baseUnitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                                            unitLookup.Add(baseUnitCode, baseUnitRefno);
                                        }
                                        else
                                        {
#if MULTILANGUAGE
                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg.ToString());
#else
                                            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg.ToString());
#endif
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoUsingTheBaseUnitCodeInTunit) 
#else
"Unable to locate the unit refno using the base unit code in tUnit."
#endif
 + Environment.NewLine + errMsg.ToString());
                                    }
                                }

                                errMsg.Append(Environment.NewLine).Append("Weight unit: \'").Append(wtUnitCode).Append("\'");
                                if (!unitLookup.TryGetValue(wtUnitCode, out wtUnitRefno))
                                {
                                    if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { wtUnitCode }))
                                    {
                                        if (drUnit.Read())
                                        {
                                            wtUnitRefno = Convert.ToInt32(
                                                drUnit.GetValue(Convert.ToInt32(
                                                SysCommon.UnitFields.UnitRefno)));
                                            unitLookup.Add(wtUnitCode, wtUnitRefno);
                                        }
                                        else
                                        {
#if MULTILANGUAGE
                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg.ToString());
#else
                                            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg.ToString());
#endif
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoUsingTheWeightUnitCodeInTunit)
#else
"Unable to locate the unit refno using the weight unit code in tUnit."
#endif
 + Environment.NewLine + errMsg.ToString());
                                    }
                                }

                                isProdUnitFound = false;

                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    cmndProdUnit.CommandText = "tProductUnit";
                                    cmndProdUnit.CommandType = CommandType.TableDirect;
                                    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                                    if (drProdUnit.Seek(
                                        DbSeekOptions.FirstEqual,
                                        new object[] { prodRefno, unitRefno, isProdUnitBC }))
                                    {
                                        isProdUnitFound = true;
                                        cmndProdUnit.CommandType = CommandType.Text;
                                        cmndProdUnit.IndexName = string.Empty;
                                        cmndProdUnit.Parameters.Clear();

                                        SqlCeParameter baseRefnoParam = new SqlCeParameter("@baseUnitRefno", SqlDbType.Int);
                                        SqlCeParameter wtRefnoParam = new SqlCeParameter("@wtUnitRefno", SqlDbType.Int);
                                        SqlCeParameter prodWeightParam = new SqlCeParameter("@prodUnitWeight", SqlDbType.Decimal);
                                        SqlCeParameter descParam = new SqlCeParameter("@desc", SqlDbType.NVarChar, 15);
                                        SqlCeParameter sellUnitParam = new SqlCeParameter("@isSellUnit", SqlDbType.Bit);
                                        SqlCeParameter defaultParam = new SqlCeParameter("@isDefault", SqlDbType.Bit);
                                        SqlCeParameter boxCountParam = new SqlCeParameter("@boxCount", SqlDbType.Decimal);
                                        SqlCeParameter prodRefnoParam = new SqlCeParameter("@prodRefno", SqlDbType.Int);
                                        SqlCeParameter unitRefnoParam = new SqlCeParameter("@unitRefno", SqlDbType.Int);
                                        SqlCeParameter brokenCaseParam = new SqlCeParameter("@isBC", SqlDbType.Bit);

                                        cmndProdUnit.CommandText = @"UPDATE tProductUnit
SET BaseUnitRefno =  @baseUnitRefno,
WtUnitRefno = @wtUnitRefno,
ProductUnitWeight = @prodUnitWeight,
ProductUnitSizeDescription = @desc,
ProductUnitIsSellUnit = @isSellUnit,
ProductUnitIsDefault = @isDefault,
ProductUnitBoxCount = @boxCount
 WHERE ProductRefno = @prodRefno
 AND UnitRefno = @unitRefno
 AND ProductUnitIsBC = @isBC";

                                        cmndProdUnit.Parameters.Add(baseRefnoParam);
                                        cmndProdUnit.Parameters.Add(wtRefnoParam);
                                        cmndProdUnit.Parameters.Add(prodWeightParam);
                                        cmndProdUnit.Parameters.Add(descParam);
                                        cmndProdUnit.Parameters.Add(sellUnitParam);
                                        cmndProdUnit.Parameters.Add(defaultParam);
                                        cmndProdUnit.Parameters.Add(boxCountParam);
                                        cmndProdUnit.Parameters.Add(prodRefnoParam);
                                        cmndProdUnit.Parameters.Add(unitRefnoParam);
                                        cmndProdUnit.Parameters.Add(brokenCaseParam);

                                        cmndProdUnit.Prepare();

                                        baseRefnoParam.Value = baseUnitRefno;
                                        wtRefnoParam.Value = wtUnitRefno;
                                        prodWeightParam.Value = prodUnitWeight;
                                        descParam.Value = description;
                                        sellUnitParam.Value = isProdUnitSellUnit;
                                        defaultParam.Value = isProdUnitDefault;
                                        boxCountParam.Value = boxCount;
                                        prodRefnoParam.Value = prodRefno;
                                        unitRefnoParam.Value = unitRefno;
                                        brokenCaseParam.Value = isProdUnitBC;
                                        

                                        cmndProdUnit.ExecuteNonQuery();
                                    }
                                }

                                if (!isProdUnitFound)
                                {
                                    cmndProdUnit.CommandText = "tProductUnit";
                                    cmndProdUnit.CommandType = CommandType.TableDirect;
                                    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                                    rec.SetInt32((int)SysCommon.ProdUnitFields.ProdRefno, prodRefno);
                                    rec.SetInt32((int)SysCommon.ProdUnitFields.UnitRefno, unitRefno);
                                    rec.SetInt32((int)SysCommon.ProdUnitFields.BaseUnitRefno, baseUnitRefno);
                                    rec.SetInt32((int)SysCommon.ProdUnitFields.WtUnitRefno, wtUnitRefno);
                                    rec.SetBoolean((int)SysCommon.ProdUnitFields.IsBC, isProdUnitBC);
                                    rec.SetString(
                                        (int)SysCommon.ProdUnitFields.BC, isProdUnitBC ? "X" : " ");
                                    rec.SetDecimal(
                                        (int)SysCommon.ProdUnitFields.Weight,
                                        prodUnitWeight);
                                    rec.SetString(
                                        (int)SysCommon.ProdUnitFields.SizeDescription,
                                        description);
                                    rec.SetBoolean(
                                        (int)SysCommon.ProdUnitFields.IsSellUnit,
                                        isProdUnitSellUnit);
                                    rec.SetBoolean(
                                        (int)SysCommon.ProdUnitFields.IsDefault,
                                        isProdUnitDefault);
                                    rec.SetDecimal(
                                        (int)SysCommon.ProdUnitFields.BoxCount,
                                        boxCount);
                                    rs.Insert(rec);
                                }

                                ++recordCount;
                                if (recordCount % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                                }
                            } // end of data

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);


#if MULTILANGUAGE
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), recordCount), false);
#else
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);
#endif

                            drUnit.Close();
                            drProd.Close();
                            drProdUnit.Close();
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductUomFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom failed.", false);
#endif

                // Duplicate key violations
                if (ex.NativeError == 25016)
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg.ToString(), ex);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg.ToString(), ex);
#endif
                }

                throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductUomFailed) + Environment.NewLine + errMsg.ToString(), 
#else
"Database exception occurred. Saving product uom failed." + Environment.NewLine + errMsg.ToString(),
#endif
 ex);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
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
                + "<action>REQUEST PRODUCT UNITS</action>"
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
