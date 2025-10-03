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
    using Bedrock;
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
        {
            this.LowerCaseName = "scheme lot numbers link";
            this.ProperCaseName = "Scheme lot numbers link";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref.", true);
                var responseData = await this.SendRequest();
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref failed.", true);

                throw new BaseException("Seeding product scheme lot number xref failed.", ex);
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

                await Conn.ExecuteAsync("DELETE FROM tProductSchemeLotNumber");
                await Conn.ExecuteAsync("DELETE FROM tProductSchemeLotNumberXRef");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref failed.", false);

                throw new BaseException("Database exception occurred. Purging product scheme lot number xref failed.", ex);
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
                rowsAffected = Convert.ToInt32(await Conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM tProductSchemeLotNumberXRef"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty,
                    $"{rowsAffected} product scheme lot number xrefs.", true);
            }
            catch (SQLiteException ex)
            {
                throw new BaseException("Database exception occurred. Product scheme lot number xrefs status failed.", ex);
            }

            return rowsAffected;
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var schemeLotsXRef = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        var schemeLot = new Seg
                        {
                            ProductSchemeCode = row["ProductSchemeCode"].ToString(),
                            ProductSchemeLotNumberCode = row["ProductSchemeLotNumberCode"].ToString(),
                        };

                        schemeLotsXRef.Add(schemeLot);
                    }

                    var query = "CREATE TEMP TABLE Seg "
                                + "("
                                + "ProductSchemeCode VARCHAR(10) NOT NULL,"
                                + "ProductSchemeLotNumberCode VARCHAR(20) NOT NULL"
                                + ")";
                    await Conn.ExecuteAsync(query);
                    await Conn.InsertAllAsync(schemeLotsXRef);

                    query = "INSERT INTO tProductSchemeLotNumberXRef (" +
                        "ProductSchemeRefno" +
                        ",ProductSchemeLotNumberRefno" +
                        ") SELECT tProductScheme.ProductSchemeRefno" +
                        ",tProductSchemeLotNumber.ProductSchemeLotNumberRefno" +
                        " FROM Seg " +
                        " INNER JOIN tProductScheme ON Seg.ProductSchemeCode = tProductScheme.ProductSchemeCode " +
                        " INNER JOIN tProductSchemeLotNumber ON Seg.ProductSchemeLotNumberCode = tProductSchemeLotNumber.ProductSchemeLotNumberCode";
                    var rowsAffected = await Conn.ExecuteAsync(query);
                    if (schemeLotsXRef.Count != rowsAffected)
                    {
                        throw new BaseException("Failed to insert all rows into tLocationMapSlot");
                    }

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await Conn.ExecuteAsync(dropTempTableQuery);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rows.Count} {this.LowerCaseName}.",
                    true);
            }
            else
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"No {this.LowerCaseName} to save.",
                    true);
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT SCHEME LOT NUMBER XREF</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     Class to hold the data for the temporary table.
        /// </summary>
        private class Seg
        {
            public string ProductSchemeLotNumberCode { get; set; }

            public string ProductSchemeCode { get; set; }
        }
    }
}
