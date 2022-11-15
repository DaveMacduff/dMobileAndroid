// <copyright file="TableProductSchemeLotNumber.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableProductSchemeLotNumber" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers.", true);

                var reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }
                }

                await this.Save(reqData);

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot numbers failed.", true);

                throw new Exception("Seeding product scheme lot numbers failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot mumbers.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumberXRef");
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumber");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot mumbers complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot numbers failed.", true);

                throw new Exception("Database exception occurred. Purging product scheme lot numbers failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeLotNumber"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} product scheme lot numbers.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Product scheme lot numbers status failed.", true);

                throw new Exception("Database exception occurred. Product scheme lot numbers status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="saveData">The saveData<see cref="string"/>.</param>
        protected override async Task Save(string saveData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref saveData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tProductSchemeLotNumber"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var progValue = 0;
                        var isSchemeLotFound = false;
                        var schemeLotCode = string.Empty;
                        var schemeLotCodeDefault = string.Empty;

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
                                var results = transaction.Query<tProductSchemeLotNumberModel>("SELECT ProductSchemeLotNumberCode FROM tProductSchemeLotNumber WHERE ProductSchemeLotNumberCode = ?", schemeLotCode);

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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers complete.", false);
                }
                catch (SQLiteException ex)
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot numbers failed.", false);

                    throw new Exception("Saving product scheme lot numbers failed." + Environment.NewLine + errMsg, ex);
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
