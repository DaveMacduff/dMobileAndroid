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
    using dMobile.Resources.Models;
    using System.Collections.Generic;

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
        public override async Task Request()
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers.", true);

                reqData = await this.Send("product scheme lot numbers");

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }
                }

                await this.Save(reqData);

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding product scheme lot numbers failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot mumbers.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumberXRef");
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumber");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot mumbers complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot numbers failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product scheme lot numbers failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeLotNumber"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product scheme lot numbers.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Product scheme lot numbers status failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product scheme lot numbers status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="saveData">The saveData<see cref="string"/></param>
        protected override async Task Save(string saveData)
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

                        ds.ReadXml(SysCommon.GetXmlReader(ref saveData), XmlReadMode.Auto);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeLotNumber"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int progValue = 0;
                            bool isSchemeLotFound = false;
                            string schemeLotCode = string.Empty;
                            string schemeLotCodeDefault = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tProductSchemeLotNumber"].Rows)
                            {
                                schemeLotCode = Convert.ToString(drow["ProductSchemeLotNumberCode"]);
                                errMsg = "Scheme lot code \'" + schemeLotCode + "\'";
                                schemeLotCodeDefault = Convert.ToString(drow["ProductSchemeLotNumberCodeDefault"]);
                                errMsg += Environment.NewLine + "Scheme lot code default " + schemeLotCodeDefault;

                                isSchemeLotFound = false;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    List<tProductSchemeLotNumberModel> results = transaction.Query<tProductSchemeLotNumberModel>("SELECT ProductSchemeLotNumberCode FROM tProductSchemeLotNumber WHERE ProductSchemeLotNumberCode = ?", schemeLotCode);

                                    if (results?.Count > 0)
                                    {
                                        isSchemeLotFound = true;
                                    }
                                }

                                if (!isSchemeLotFound)
                                {
                                    transaction.Execute("INSERT INTO tProductSchemeLotNumber (ProductSchemeLotNumberCode, ProductSchemeLotNumberCodeDefault) VALUES (?, ?)", schemeLotCode, schemeLotCodeDefault);
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers failed.", false);

                    // TODO: Resource Strings
                    throw new Exception("Saving product scheme lot numbers failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST PRODUCT SCHEME LOT NUMBERS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>" + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
