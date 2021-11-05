namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;

    public class TableLocationMapWarehouseProductScheme : TableBase
    {
        public TableLocationMapWarehouseProductScheme()
            : base()
        {
            this.FullClassName = GetType().FullName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Request()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes.", true);

                List<tFamilyModel> resultsFamily = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (resultsFamily?.Count > 0)
                {
                    foreach (tFamilyModel family in resultsFamily)
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family.FamilyCode), true);

                        string reqData = await this.SendRequest(family.FamilyCode);

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
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes failed.", true);
                throw new Exception("Database exception occurred. Seeding warehouse product schemes failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes failed.", true);
                throw new Exception("Seeding warehouse product schemes failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapWarehouseProductScheme"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} warehouse product schemes.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Warehouse product schemes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapWarehouseProductScheme");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes failed.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes failed.", true);
                throw new Exception("Database exception occurred. Purge warehouse product schemes failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override async Task Save(string reqData)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tLocationMapWarehouseProductScheme"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", ds.Tables["tLocationMapWarehouseProductScheme"].Rows.Count), false);

                            int progValue = 0;
                            int productFKRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int schemeRefno = 0;
                            int locMapWhsRefno = 0;
                            int productRefno = 0;
                            string schemeCode = string.Empty;
                            bool isWhsProdSchemeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tLocationMapWarehouseProductScheme"].Rows)
                            {
                                schemeCode = drow["F2"].ToString();
                                errMsg = "Scheme code \'" + schemeCode + "\'";

                                locMapWhsFKRefno = Convert.ToInt32(drow["F3"]);
                                errMsg = "Warehouse FK Refno \'" + locMapWhsFKRefno.ToString() + "\'";

                                productFKRefno = Convert.ToInt32(drow["F1"]);
                                errMsg = "Product FK Refno \'" + productFKRefno.ToString() + "\'";

                                // Find the scheme
                                List<tProductSchemeModel> resultsProductScheme = transaction.Query<tProductSchemeModel>("SELECT * FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                                if (resultsProductScheme?.Count > 0)
                                {
                                    schemeRefno = resultsProductScheme[0].ProductSchemeRefno;
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Unable to locate the scheme refno using the scheme code in tProductScheme." + Environment.NewLine + errMsg);
                                }

                                // Find the product
                                List<tProductModel> resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(productFKRefno));

                                if (resultsProduct?.Count > 0)
                                {
                                    productRefno = resultsProduct[0].ProductRefno;
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    throw new Exception(string.Format("Unable to locate the product refno using the product FKRefno {0} in tProduct.", productFKRefno) + Environment.NewLine + errMsg);
                                }

                                // Find the warehouse
                                List<tLocationMapWarehouseModel> resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapWhsFKRefno));

                                if (resultsLocationMapWarehouse?.Count > 0)
                                {
                                    locMapWhsRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    throw new Exception(string.Format("Unable to locate the locationMapWarehouse location map refno using the locMapWhsFKRefno {0} in tLocationMapWarehouse.", locMapWhsFKRefno) + Environment.NewLine + errMsg);
                                }

                                isWhsProdSchemeFound = false;

                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    string query = "SELECT * FROM tLocationMapWarehouseProductScheme WHERE WarehouseLocationMapRefno = ? AND ProductRefno = ? AND ProductSchemeRefno = ?";
                                    List<tLocationMapWarehouseProductSchemeModel> resultsLocationMapWarehouseProductScheme = transaction.Query<tLocationMapWarehouseProductSchemeModel>(query, Convert.ToInt32(locMapWhsRefno), Convert.ToInt32(productRefno), Convert.ToInt32(schemeRefno));

                                    if (resultsLocationMapWarehouseProductScheme?.Count > 0)
                                    {
                                        isWhsProdSchemeFound = true;
                                    }
                                }

                                if (!isWhsProdSchemeFound)
                                {
                                    transaction.Execute("INSERT INTO tLocationMapWarehouseProductScheme (WarehouseLocationMapRefno, ProductRefno, ProductSchemeRefno) VALUES (?, ?, ?)", Convert.ToInt32(locMapWhsRefno), Convert.ToInt32(productRefno), Convert.ToInt32(schemeRefno));
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
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving warehouse product schemes failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected async override Task<string> CreateRequest(string strFamilyCode)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST WAREHOUSE PRODUCT SCHEMES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + ("<family_code>" + strFamilyCode + "</family_code>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(string familyCode)
        {
            return await this.SendComm(await this.CreateRequest(familyCode));
        }
    }
}