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
    /// Defines the <see cref="TableProductSchemeLotNumberXRef" />
    /// </summary>
    public class TableProductSchemeLotNumberXRef : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductSchemeLotNumberXRef"/> class.
        /// </summary>
        public TableProductSchemeLotNumberXRef()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductSchemeLotNumberXRef";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref.", true);

                reqData = this.Send("product scheme lot number xref");

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    this.Save(reqData);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding product scheme lot number xref failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductSchemeLotNumber";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tProductSchemeLotNumberXRef";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref failed.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref failed.", false);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product scheme lot number xref failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductSchemeLotNumberXRef";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product scheme lot number xrefs.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product scheme lot number xrefs status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndGetScheme = conn.CreateCommand())
            using (SQLiteCommand cmndGetSchemeLot = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeLotXRef = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot number xref.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeLotNumberXRef"].Rows.Count);

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //cmndSchemeLotXRef.CommandText = "tProductSchemeLotNumberXRef";
                            //cmndSchemeLotXRef.CommandType = CommandType.TableDirect;
                            //cmndSchemeLotXRef.IndexName = "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno";

                            //using (SQLiteDataReader drSchemeLotXRef = cmndSchemeLotXRef.ExecuteReader())
                            //{
                            //{
                            //cmndGetScheme.CommandText = "tProductScheme";
                            //cmndGetScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                            //cmndGetScheme.CommandType = CommandType.TableDirect;

                            //using (SQLiteDataReader drScheme = cmndGetScheme.ExecuteReader())
                            //{
                            //    {
                            //cmndGetSchemeLot.CommandText = "tProductSchemeLotNumber";
                            //cmndGetSchemeLot.IndexName = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode";
                            //cmndGetSchemeLot.CommandType = CommandType.TableDirect;

                            //using (SQLiteDataReader drSchemeLot = cmndGetSchemeLot.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rs = cmndSchemeLotXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int schemeRefno = 0;
                            int schemeLotRefno = 0;
                            int progValue = 0;
                            bool isSchemeLotXRef = false;
                            string schemeCode = string.Empty;
                            string schemeLotCode = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tProductSchemeLotNumberXRef"].Rows)
                            {
                                schemeCode = Convert.ToString(drow["ProductSchemeCode"]);
                                errMsg = "Scheme code \'" + schemeCode + "\'";

                                cmndGetScheme.CommandText = "SELECT * FROM tProductScheme WHERE ProductSchemeCode = @productSchemeCode";
                                cmndGetScheme.CommandType = CommandType.Text;
                                //cmndGetScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                                cmndGetScheme.Parameters.Add(new SQLiteParameter("@productSchemeCode", schemeCode));

                                using (SQLiteDataReader drScheme = cmndGetScheme.ExecuteReader())
                                {
                                    //if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                                    //{
                                    if (drScheme.Read())
                                    {
                                        schemeRefno = Convert.ToInt32(drScheme.GetValue(Convert.ToInt32(SysCommon.ProdSchemeFields.Refno)));
                                    }
                                    else
                                    {
                                        // TODO: Resource Strings
                                        throw new Exception("drScheme.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource Strings
                                    //    throw new Exception("Unable to locate the product scheme code in tProductScheme." + Environment.NewLine + errMsg);
                                    //}

                                    drScheme.Close();
                                }

                                schemeLotCode = Convert.ToString(drow["ProductSchemeLotNumberCode"]);
                                errMsg += Environment.NewLine + "Scheme lot code \'" + schemeLotCode + "\'";

                                cmndGetSchemeLot.CommandText = "SELECT * FROM tProductSchemeLotNumber WHERE ProductSchemeLotNumberCode = @productSchemeLotNumberCode";
                                cmndGetSchemeLot.CommandType = CommandType.Text;
                                //cmndGetSchemeLot.IndexName = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode";
                                cmndGetSchemeLot.Parameters.Add(new SQLiteParameter("@productSchemeLotNumberCode", schemeLotCode));

                                using (SQLiteDataReader drSchemeLot = cmndGetSchemeLot.ExecuteReader())
                                {
                                    //if (drSchemeLot.Seek(DbSeekOptions.FirstEqual, new object[] { schemeLotCode }))
                                    //{
                                    if (drSchemeLot.Read())
                                    {
                                        schemeLotRefno = Convert.ToInt32(drSchemeLot.GetValue(Convert.ToInt32(SysCommon.ProdSchemeLotNumberFields.Refno)));
                                    }
                                    else
                                    {
                                        // TODO: Resource Strings
                                        throw new Exception("drSchemeLot.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource Strings
                                    //    throw new Exception("Unable to locate the product scheme lot number code in tProductSchemeLotNumber." + Environment.NewLine + errMsg);
                                    //}

                                    drSchemeLot.Close();
                                }

                                isSchemeLotXRef = false;

                                // TODO: Uncomment SysEnv.RefreshOpt.RefreshDataChangesOnly
                                //if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                //{
                                cmndSchemeLotXRef.CommandText = "SELECT ProductSchemeRefno, ProductSchemeLotNumberRefno FROM tProductSchemeLotNumberXRef WHERE ProductSchemeRefno = @productSchemeRefno AND ProductSchemeLotNumberRefno = @productSchemeLotNumberRefno";
                                cmndSchemeLotXRef.CommandType = CommandType.Text;
                                //cmndSchemeLotXRef.IndexName = "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno";
                                cmndSchemeLotXRef.Parameters.Add(new SQLiteParameter("@productSchemeRefno", schemeRefno));
                                cmndSchemeLotXRef.Parameters.Add(new SQLiteParameter("@productSchemeLotNumberRefno", schemeLotRefno));

                                using (SQLiteDataReader drSchemeLotXRef = cmndSchemeLotXRef.ExecuteReader())
                                {
                                    //if (drSchemeLotXRef.Seek(DbSeekOptions.FirstEqual, new object[] { schemeRefno, schemeLotRefno }))
                                    if (drSchemeLotXRef.Read())
                                    {
                                        isSchemeLotXRef = true;
                                    }

                                    drSchemeLotXRef.Close();
                                }
                                //}

                                if (!isSchemeLotXRef)
                                {
                                    cmndSchemeLotXRef.CommandText = "INSERT INTO tProductSchemeLotNumberXRef (ProductSchemeRefno, ProductSchemeLotNumberRefno) VALUES (@productSchemeRefno, @productSchemeLotNumberRefno)";
                                    cmndSchemeLotXRef.CommandType = CommandType.Text;
                                    //cmndSchemeLotXRef.IndexName = "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno";
                                    cmndSchemeLotXRef.Parameters.Add(new SQLiteParameter("@productSchemeRefno", schemeRefno));
                                    cmndSchemeLotXRef.Parameters.Add(new SQLiteParameter("@productSchemeLotNumberRefno", schemeLotRefno));

                                    cmndSchemeLotXRef.ExecuteNonQuery();

                                    //rec.SetInt32(Convert.ToInt32(SysCommon.ProdSchemeLotNumberXRefFields.ProdSchemeRefno), schemeRefno);
                                    //rec.SetInt32(Convert.ToInt32(SysCommon.ProdSchemeLotNumberXRefFields.ProdSchemeLotNumberRefno), schemeLotRefno);

                                    //rs.Insert(rec);
                                }

                                progValue++;

                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);

                        }

                        //drSchemeLot.Close();
                        //}
                        //}
                        //drScheme.Close();
                        //}
                        //}
                        //drSchemeLotXRef.Close();
                        //}
                        //}
                        //}

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot number xref complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request product scheme lot number xref failed.", false);

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Request product scheme lot number xref failed." + errMsg, ex);
                }
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
                + "<action>REQUEST PRODUCT SCHEME LOT NUMBER XREF</action>"
                + SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
