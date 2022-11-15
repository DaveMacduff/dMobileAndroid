// <copyright file="TableLocationMapWarehouseProductScheme.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    public class TableLocationMapWarehouseProductScheme : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapWarehouseProductScheme"/> class.
        /// </summary>
        public TableLocationMapWarehouseProductScheme()
            : base()
        {
            this.FullClassName = this.GetType().FullName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes.", true);

                var resultsFamily = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (resultsFamily?.Count > 0)
                {
                    foreach (var family in resultsFamily)
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);

                        var reqData = await this.SendRequest(family.FamilyCode);

                        if (reqData.Length > 0)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                var reqErr = new XmlDocument();
                                reqErr.LoadXml(reqData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.Save(reqData);
                        }
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes failed.", true);
                throw new Exception("Database exception occurred. Seeding warehouse product schemes failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes failed.", true);
                throw new Exception("Seeding warehouse product schemes failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapWarehouseProductScheme"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} warehouse product schemes.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Warehouse product schemes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapWarehouseProductScheme");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes failed.", true);
            }
            catch (SQLiteException ex)
            {
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
        /// <inheritdoc/>
        protected override async Task Save(string reqData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tLocationMapWarehouseProductScheme"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty,
                                $"Records {ds.Tables["tLocationMapWarehouseProductScheme"].Rows.Count}", false);

                            var progValue = 0;
                            var productFKRefno = 0;
                            var locMapWhsFKRefno = 0;
                            var schemeRefno = 0;
                            var locMapWhsRefno = 0;
                            var productRefno = 0;
                            var schemeCode = string.Empty;
                            var isWhsProdSchemeFound = false;

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
                                var resultsProductScheme = transaction.Query<tProductSchemeModel>("SELECT * FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                                if (resultsProductScheme?.Count > 0)
                                {
                                    schemeRefno = resultsProductScheme[0].ProductSchemeRefno;
                                }
                                else
                                {
                                    throw new Exception("Unable to locate the scheme refno using the scheme code in tProductScheme." + Environment.NewLine + errMsg);
                                }

                                // Find the product
                                var resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(productFKRefno));

                                if (resultsProduct?.Count > 0)
                                {
                                    productRefno = resultsProduct[0].ProductRefno;
                                }
                                else
                                {
                                    throw new Exception(
                                        $"Unable to locate the product refno using the product FKRefno {productFKRefno} in tProduct." + Environment.NewLine + errMsg);
                                }

                                // Find the warehouse
                                var resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapWhsFKRefno));

                                if (resultsLocationMapWarehouse?.Count > 0)
                                {
                                    locMapWhsRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                }
                                else
                                {
                                    throw new Exception(
                                        $"Unable to locate the locationMapWarehouse location map refno using the locMapWhsFKRefno {locMapWhsFKRefno} in tLocationMapWarehouse." + Environment.NewLine + errMsg);
                                }

                                isWhsProdSchemeFound = false;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    var query = "SELECT * FROM tLocationMapWarehouseProductScheme WHERE WarehouseLocationMapRefno = ? AND ProductRefno = ? AND ProductSchemeRefno = ?";
                                    var resultsLocationMapWarehouseProductScheme = transaction.Query<tLocationMapWarehouseProductSchemeModel>(query, Convert.ToInt32(locMapWhsRefno), Convert.ToInt32(productRefno), Convert.ToInt32(schemeRefno));

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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes failed.", false);

                    throw new Exception("Database exception occurred. Saving warehouse product schemes failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected async override Task<string> CreateRequest(string strFamilyCode)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST WAREHOUSE PRODUCT SCHEMES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<family_code>" + strFamilyCode + "</family_code>"
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
        /// <inheritdoc/>
        protected override async Task<string> SendRequest(string familyCode)
        {
            return await this.SendComm(await this.CreateRequest(familyCode));
        }
    }
}