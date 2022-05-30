// <copyright file="clsTableProductSchemeVariantXRef.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableProductSchemeVariantXRef" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref.", true);

                string reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref failed.", true);

                throw new Exception("Seeding product scheme variant xref failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref.", true);

                // Note should tProductSchemeVariants be purged too?
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeVariantXRef");
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref failed.", true);

                throw new Exception("Database exception occurred. Purging product scheme variant xref failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeVariantXRef"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product scheme variant xrefs.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Product scheme variant xref status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save.
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xrefs.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeVariantXRef"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

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

                                List<tProductSchemeModel> resultsProductScheme = transaction.Query<tProductSchemeModel>("SELECT * FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                                if (resultsProductScheme?.Count > 0)
                                {
                                    schemeRefno = resultsProductScheme[0].ProductSchemeRefno;
                                }
                                else
                                {
                                    throw new Exception("drScheme.Read() failed." + Environment.NewLine + errMsg);
                                }

                                schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                                errMsg += Environment.NewLine + "Variant no " + schemeVariantNo;

                                List<tProductSchemeVariantModel> resultsProductSchemeVariant = transaction.Query<tProductSchemeVariantModel>("SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", schemeVariantNo);

                                if (resultsProductSchemeVariant?.Count > 0)
                                {
                                    schemeVarRefno = resultsProductSchemeVariant[0].ProductSchemeVariantRefno;
                                }
                                else
                                {
                                    throw new Exception("drSchemeVar.Read() failed." + Environment.NewLine + errMsg);
                                }

                                isSchemeVarXRef = false;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    List<tProductSchemeVariantXRefModel> resultsProductSchemeVariantXRef = transaction.Query<tProductSchemeVariantXRefModel>("SELECT * FROM tProductSchemeVariantXRef WHERE ProductSchemeRefno = ? AND ProductSchemeVariantRefno = ?", schemeRefno, schemeVarRefno);

                                    if (resultsProductSchemeVariantXRef?.Count > 0)
                                    {
                                        isSchemeVarXRef = true;
                                    }
                                }

                                if (!isSchemeVarXRef)
                                {
                                    transaction.Execute("INSERT INTO tProductSchemeVariantXRef (ProductSchemeRefno, ProductSchemeVariantRefno) VALUES (?, ?)", schemeRefno, schemeVarRefno);
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xref complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xref failed.", false);

                    throw new Exception("Database exception occurred. Saving product scheme variant xref failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT SCHEME VARIANT XREF</action>"
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
