namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

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
        public override async Task Request()
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref.", true);

                reqData = await this.Send("product scheme lot number xref");

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
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
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumber");
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumberXRef");

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
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeLotNumberXRef"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product scheme lot number xrefs.", rowsAffected), true);
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
        protected override async Task Save(string reqData)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
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

                                List<tProductSchemeModel> resultsProductScheme = transaction.Query<tProductSchemeModel>("SELECT * FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                                if (resultsProductScheme?.Count > 0)
                                {
                                    schemeRefno = resultsProductScheme[0].ProductSchemeRefno;
                                }
                                else
                                {
                                    // TODO: Resource Strings
                                    throw new Exception("drScheme.Read() failed." + Environment.NewLine + errMsg);
                                }

                                schemeLotCode = Convert.ToString(drow["ProductSchemeLotNumberCode"]);
                                errMsg += Environment.NewLine + "Scheme lot code \'" + schemeLotCode + "\'";

                                List<tProductSchemeLotNumberModel> resultsProductSchemeLotNumber = transaction.Query<tProductSchemeLotNumberModel>("SELECT * FROM tProductSchemeLotNumber WHERE ProductSchemeLotNumberCode = ?", schemeLotCode);

                                if (resultsProductSchemeLotNumber?.Count > 0)
                                {
                                    schemeLotRefno = resultsProductSchemeLotNumber[0].ProductSchemeLotNumberRefno;
                                }
                                else
                                {
                                    // TODO: Resource Strings
                                    throw new Exception("drSchemeLot.Read() failed." + Environment.NewLine + errMsg);
                                }

                                isSchemeLotXRef = false;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    List<tProductSchemeLotNumberXRefModel> resultsProductSchemeLotNumberXRef = transaction.Query<tProductSchemeLotNumberXRefModel>("SELECT ProductSchemeRefno, ProductSchemeLotNumberRefno FROM tProductSchemeLotNumberXRef WHERE ProductSchemeRefno = ? AND ProductSchemeLotNumberRefno = ?", schemeRefno, schemeLotRefno);

                                    if (resultsProductSchemeLotNumberXRef?.Count > 0)
                                    {
                                        isSchemeLotXRef = true;
                                    }
                                }

                                if (!isSchemeLotXRef)
                                {
                                    transaction.Execute("INSERT INTO tProductSchemeLotNumberXRef (ProductSchemeRefno, ProductSchemeLotNumberRefno) VALUES (?, ?)", schemeRefno, schemeLotRefno);
                                }

                                progValue++;

                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);

                        }

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
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> Create()
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
