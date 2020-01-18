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
    /// Defines the <see cref="TableProductSchemeVariantXRef" />
    /// </summary>
    public class TableProductSchemeVariantXRef : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductSchemeVariantXRef"/> class.
        /// </summary>
        public TableProductSchemeVariantXRef()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductSchemeVariantXRef";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref.", true);

                reqData = this.Send("product scheme variant xref");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding product scheme variant xref failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    // Note should tProductSchemeVariants be purged too?
                    cmndPurge.CommandText = "DELETE FROM tProductSchemeVariantXRef";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product scheme variant xref failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductSchemeVariantXRef";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product scheme variant xrefs.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product scheme variant xref status failed.", ex);
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

            using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
            using (SQLiteCommand cmndScheme = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xrefs.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeVariantXRef"].Rows.Count);

                            // TODO: Resource String
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            cmndSchemeVarXRef.CommandText = "SELECT * FROM tProductSchemeVariantXRef " +
                            "WHERE ProductSchemeRefno = @productSchemeRefno AND ProductSchemeVariantRefno = @productSchemeVariantRefno";
                            cmndSchemeVarXRef.CommandType = CommandType.Text;

                            cmndScheme.CommandText = "SELECT * FROM tProductScheme WHERE ProductSchemeCode = @productSchemeCode";
                            //cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                            cmndScheme.CommandType = CommandType.Text;

                            cmndSchemeVar.CommandText = "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = @productSchemeVariantNo";
                            //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
                            cmndSchemeVar.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rs = cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int schemeRefno = 0;
                            int schemeVarRefno = 0;
                            int progValue = 0;
                            bool isSchemeVarXRef = false;
                            string schemeCode = string.Empty;
                            int schemeVariantNo = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tProductSchemeVariantXRef"].Rows)
                            {
                                schemeCode = drow["ProductSchemeCode"].ToString();
                                errMsg = "Code \'" + schemeCode + "\'";

                                cmndScheme.Parameters.Add(new SQLiteParameter("productSchemeCode", schemeCode));

                                using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
                                {
                                    //if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                                    //{
                                    if (drScheme.Read())
                                    {
                                        schemeRefno = Convert.ToInt32(drScheme.GetValue((int)SysCommon.ProdSchemeFields.Refno));
                                    }
                                    else
                                    {
                                        // TODO: Resource String
                                        throw new Exception("drScheme.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    throw new Exception("Unable to locate the product scheme code in tProductScheme." + Environment.NewLine + errMsg);
                                    //}

                                    drScheme.Close();
                                }

                                schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                                errMsg += Environment.NewLine + "Variant no " + schemeVariantNo;

                                cmndSchemeVar.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", schemeVariantNo));

                                using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                                {
                                    //if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVariantNo }))
                                    //{
                                    if (drSchemeVar.Read())
                                    {
                                        schemeVarRefno = Convert.ToInt32(drSchemeVar.GetValue((int)SysCommon.ProdSchemeVariantFields.Refno));
                                    }
                                    else
                                    {
                                        // TODO: Resource String
                                        throw new Exception("drSchemeVar.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    throw new Exception("Unable to locate the product scheme variant number in tProductSchemeVariant." + Environment.NewLine + errMsg);
                                    //}

                                    drSchemeVar.Close();
                                }

                                isSchemeVarXRef = false;

                                // TODO: Uncomment SysEnv.RefreshOpt.RefreshDataChangesOnly
                                //if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                //{

                                //cmndSchemeVarXRef.IndexName = "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno";

                                //cmndSchemeVarXRef.CommandText = "tProductSchemeVariantXRef";
                                //cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
                                //cmndSchemeVarXRef.IndexName = "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno";
                                cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeRefno", schemeRefno));
                                cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeVariantRefno", schemeVarRefno));

                                using (SQLiteDataReader drSchemeVarXRef = cmndSchemeVarXRef.ExecuteReader())
                                {
                                    //if (drSchemeVarXRef.Seek(DbSeekOptions.FirstEqual, new object[] { schemeRefno, schemeVarRefno }))
                                    if (drSchemeVarXRef.Read())
                                    {
                                        isSchemeVarXRef = true;
                                    }

                                    drSchemeVarXRef.Close();
                                }
                                //}

                                if (!isSchemeVarXRef)
                                {
                                    cmndSchemeVarXRef.Parameters.Clear();
                                    cmndSchemeVarXRef.CommandText = "INSERT INTO tProductSchemeVariantXRef (ProductSchemeRefno, ProductSchemeVariantRefno) VALUES (@productSchemeRefno, @productSchemeVariantRefno)";
                                    cmndSchemeVarXRef.CommandType = CommandType.Text;
                                    //cmndSchemeVarXRef.IndexName = "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno";
                                    cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeRefno", schemeRefno));
                                    cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeVariantRefno", schemeVarRefno));

                                    cmndSchemeVarXRef.ExecuteNonQuery();

                                    //rec.SetInt32((int)SysCommon.ProdSchemeVariantXRefFields.ProdSchemeRefno, schemeRefno);
                                    //rec.SetInt32((int)SysCommon.ProdSchemeVariantXRefFields.ProdSchemeVariantRefno, schemeVarRefno);

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

                            //drSchemeVar.Close();
                            //}

                            //}

                            //drScheme.Close();
                            //}
                            //}

                            //drSchemeVarXRef.Close();
                        }

                        transaction.Commit();

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xref complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xref failed.", false);

                    throw new Exception("Database exception occurred. Saving product scheme variant xref failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST PRODUCT SCHEME VARIANT XREF</action>"
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
