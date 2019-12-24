namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableProductSchemeLotNumber" />
    /// </summary>
    public class TableProductSchemeLotNumber : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductSchemeLotNumber"/> class.
        /// </summary>
        public TableProductSchemeLotNumber()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductSchemeLotNumber";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            string reqData = string.Empty;

            // TODO: Uncomment GC calls
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumbers), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers.", true);
#endif

                reqData = this.Send("product scheme lot numbers");
                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }
                }

                this.Save(reqData);

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumbersComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers complete.", true);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumbersFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumbersFailed), ex);
#else
                throw new Exception("Seeding product scheme lot numbers failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeLotMumbers), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot mumbers.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductSchemeLotNumberXRef";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tProductSchemeLotNumber";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeLotMumbersComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot mumbers complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeLotNumbersFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot numbers failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductSchemeLotNumbersFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging product scheme lot numbers failed.", ex);
#endif
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductSchemeLotNumber";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductSchemeLotNumbers), rowsAffected),
#else
              string.Format("{0} product scheme lot numbers.", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ProductSchemeLotNumbersStatusFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Product scheme lot numbers status failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductSchemeLotNumbersStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Product scheme lot numbers status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="saveData">The saveData<see cref="string"/></param>
        protected override void Save(string saveData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            try
            {
                using (DataSet ds = new DataSet())
                {
                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref saveData), XmlReadMode.Auto);

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers.", false);

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["tProductSchemeLotNumber"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                        using (SQLiteCommand cmndSchemeLot = conn.CreateCommand())
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            //cmndSchemeLot.CommandText = "tProductSchemeLotNumber";
                            //cmndSchemeLot.CommandType = CommandType.TableDirect;
                            //cmndSchemeLot.IndexName = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode";

                            //using (SQLiteDataReader drSchemeLot = cmndSchemeLot.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rs = cmndSchemeLot.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int progValue = 0;
                            bool isSchemeLotFound = false;
                            string schemeLotCode = string.Empty;
                            string schemeLotCodeDefault = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tProductSchemeLotNumber"].Rows)
                            {
                                schemeLotCode = drow["ProductSchemeLotNumberCode"].ToString();
                                errMsg = "Scheme lot code \'" + schemeLotCode + "\'";
                                schemeLotCodeDefault = drow["ProductSchemeLotNumberCodeDefault"].ToString();
                                errMsg += Environment.NewLine + "Scheme lot code default " + schemeLotCodeDefault;

                                isSchemeLotFound = false;

                                // TODO: Uncomment SysEnv.RefreshOpt.RefreshDataChangesOnly
                                //if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                //{
                                cmndSchemeLot.CommandText = "SELECT ProductSchemeLotNumberCode FROM tProductSchemeLotNumber WHERE ProductSchemeLotNumberCode = @productSchemeLotNumberCode";
                                cmndSchemeLot.CommandType = CommandType.Text;
                                //cmndSchemeLot.IndexName = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode";
                                cmndSchemeLot.Parameters.Add(new SQLiteParameter("@productSchemeLotNumberCode", schemeLotCode));

                                using (SQLiteDataReader drSchemeLot = cmndSchemeLot.ExecuteReader())
                                {
                                    //if (drSchemeLot.Seek(DbSeekOptions.FirstEqual, new object[] { schemeLotCode }))
                                    if (drSchemeLot.Read())
                                    {
                                        isSchemeLotFound = true;
                                    }

                                    drSchemeLot.Close();
                                }
                                //}

                                if (!isSchemeLotFound)
                                {
                                    cmndSchemeLot.CommandText = "INSERT INTO tProductSchemeLotNumber (ProductSchemeLotNumberCode, ProductSchemeLotNumberCodeDefault) VALUES (@productSchemeLotNumberCode, @productSchemeLotNumberCodeDefault)";
                                    cmndSchemeLot.CommandType = CommandType.Text;
                                    //cmndSchemeLot.IndexName = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode";
                                    cmndSchemeLot.Parameters.Add(new SQLiteParameter("@productSchemeLotNumberCode", schemeLotCode));
                                    cmndSchemeLot.Parameters.Add(new SQLiteParameter("@productSchemeLotNumberCodeDefault", schemeLotCodeDefault));

                                    cmndSchemeLot.ExecuteNonQuery();

                                    //rec.SetString((int)SysCommon.ProdSchemeLotNumberFields.Code, schemeLotCode);
                                    //rec.SetString((int)SysCommon.ProdSchemeLotNumberFields.CodeDefault, schemeLotCodeDefault);

                                    //rs.Insert(rec);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            //}

                            //drSchemeLot.Close();
                            //}

                            transaction.Commit();
                        }
                    }

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers complete.", false);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers failed.", false);

                //// Duplicate key violations
                //if (ex.NativeError == 25016)
                //{
                //    // TODO: Resource Strings
                //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                //}

                // TODO: Resource Strings
                throw new Exception("Saving product scheme lot numbers failed." + Environment.NewLine + errMsg, ex);
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override string Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT SCHEME LOT NUMBERS</action>"
                + SysCommon.GetRequestDefaults(true)
                + "</command>" + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
