// <copyright file="TableProductSchemeVariantXRef.cs" company="InterSect Business Systems Inc">
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
            this.LowerCaseName = "scheme variant xrefs";
            this.ProperCaseName = "Scheme variant xrefs";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var responseData = await this.SendRequest();
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{this.LowerCaseName} seeded.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);
                throw new BaseException($"Seeding {this.LowerCaseName} failed.", ex);
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
                await Conn.ExecuteAsync("DELETE FROM tProductSchemeVariantXRef");
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref failed.", true);

                throw new BaseException("Database exception occurred. Purging product scheme variant xref failed.", ex);
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
                rowsAffected = Convert.ToInt32(await Conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeVariantXRef"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} product scheme variant xrefs.", true);
            }
            catch (SQLiteException ex)
            {
                throw new BaseException("Database exception occurred. Product scheme variant xref status failed.", ex);
            }

            return rowsAffected;
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            ////select distinct cast(iv_grade.scheme as varchar(10)) as ProductSchemeCode, ;
            ////cast(iv_grade.variant_no as integer) as ProductSchemeVariantNo;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var variantXRefs = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        variantXRefs.Add(new Seg
                        {
                            ProductSchemeCode = row["ProductSchemeCode"].ToString(),
                            ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"]),
                        });
                    }

                    var query = "CREATE TEMP TABLE Seg " +
                        "(" +
                        "ProductSchemeCode VARCHAR(10)" +
                        ",ProductSchemeVariantNo INTEGER" +
                        ")";
                    await Conn.ExecuteAsync(query);
                    await Conn.InsertAllAsync(variantXRefs);

                    query = "INSERT INTO tProductSchemeVariantXRef (" +
                        "ProductSchemeRefno" +
                        ",ProductSchemeVariantRefno" +
                        ") SELECT scheme.ProductSchemeRefno" +
                        ",schemeVar.ProductSchemeVariantRefno " +
                        "FROM Seg " +
                        "JOIN tProductScheme scheme ON scheme.ProductSchemeCode = Seg.ProductSchemeCode " +
                        "JOIN tProductSchemeVariant schemeVar ON schemeVar.ProductSchemeVariantNo = Seg.ProductSchemeVariantNo";
                    var rowsAffected = await Conn.ExecuteAsync(query);
                    if (variantXRefs.Count != rowsAffected)
                    {
                        throw new BaseException("Failed to insert all rows into tLocationMapSlot");
                    }

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await Conn.ExecuteAsync(dropTempTableQuery);

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
                        $"{variantXRefs.Count} {this.LowerCaseName}.", true);
                }
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
                + "<action>REQUEST PRODUCT SCHEME VARIANT XREF</action>"
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
            public string ProductSchemeCode { get; set; }

            public int ProductSchemeVariantNo { get; set; }
        }
    }
}
