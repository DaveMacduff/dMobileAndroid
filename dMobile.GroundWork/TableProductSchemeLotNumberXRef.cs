// <copyright file="TableProductSchemeLotNumberXRef.cs" company="InterSect Business Systems Inc">
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

    /// <summary>
    /// Defines the <see cref="TableProductSchemeLotNumberXRef" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref.", true);

                var reqData = await this.SendRequest();
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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref failed.", true);

                throw new Exception("Seeding product scheme lot number xref failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumber");
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumberXRef");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref failed.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref failed.", false);

                throw new Exception("Database exception occurred. Purging product scheme lot number xref failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeLotNumberXRef"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty,
                    $"{rowsAffected} product scheme lot number xrefs.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Product scheme lot number xrefs status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot number xref.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tProductSchemeLotNumberXRef"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var schemeRefno = 0;
                        var schemeLotRefno = 0;
                        var progValue = 0;
                        var isSchemeLotXRef = false;
                        var schemeCode = string.Empty;
                        var schemeLotCode = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tProductSchemeLotNumberXRef"].Rows)
                        {
                            schemeCode = Convert.ToString(drow["ProductSchemeCode"]);
                            errMsg = "Scheme code \'" + schemeCode + "\'";

                            var resultsProductScheme = transaction.Query<tProductSchemeModel>("SELECT * FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                            if (resultsProductScheme?.Count > 0)
                            {
                                schemeRefno = resultsProductScheme[0].ProductSchemeRefno;
                            }
                            else
                            {
                                throw new Exception("drScheme.Read() failed." + Environment.NewLine + errMsg);
                            }

                            schemeLotCode = Convert.ToString(drow["ProductSchemeLotNumberCode"]);
                            errMsg += Environment.NewLine + "Scheme lot code \'" + schemeLotCode + "\'";

                            var resultsProductSchemeLotNumber = transaction.Query<tProductSchemeLotNumberModel>("SELECT * FROM tProductSchemeLotNumber WHERE ProductSchemeLotNumberCode = ?", schemeLotCode);

                            if (resultsProductSchemeLotNumber?.Count > 0)
                            {
                                schemeLotRefno = resultsProductSchemeLotNumber[0].ProductSchemeLotNumberRefno;
                            }
                            else
                            {
                                throw new Exception("drSchemeLot.Read() failed." + Environment.NewLine + errMsg);
                            }

                            isSchemeLotXRef = false;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                var resultsProductSchemeLotNumberXRef = transaction.Query<tProductSchemeLotNumberXRefModel>("SELECT ProductSchemeRefno, ProductSchemeLotNumberRefno FROM tProductSchemeLotNumberXRef WHERE ProductSchemeRefno = ? AND ProductSchemeLotNumberRefno = ?", schemeRefno, schemeLotRefno);

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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot number xref complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request product scheme lot number xref failed.", false);

                    throw new Exception("Database exception occurred. Request product scheme lot number xref failed." + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT SCHEME LOT NUMBER XREF</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
