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
    using Bedrock;
    using Resources.Models.Products;
    using SQLite;

    /// <summary>
    /// Represents the table for product scheme lot numbers.
    /// </summary>
    public class TableProductSchemeLotNumber : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductSchemeLotNumber"/> class.
        /// </summary>
        public TableProductSchemeLotNumber()
        {
            this.LowerCaseName = "scheme lot numbers";
            this.ProperCaseName = "Scheme lot numbers";
        }

        /// <summary>
        /// Requests data for the table.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName}.",
                    true);
                var responseData = await this.SendRequest();
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{this.LowerCaseName} seeded.",
                    true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);

                throw new BaseException($"Seeding {this.LowerCaseName} failed.", ex);
            }
        }

        /// <summary>
        /// Purges data from the table.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName}.",
                    true);

                await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumberXRef");
                await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteAsync("DELETE FROM tProductSchemeLotNumber");

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} complete.",
                    true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} failed.",
                    true);

                throw new BaseException(
                    $"Database exception occurred. Purging {this.LowerCaseName} failed.",
                    ex);
            }
        }

        /// <summary>
        /// Gets the data status of the table.
        /// </summary>
        /// <returns>The number of rows in the table.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(
                    await DatabaseConnection.GetInstance()
                        .SqlConn.ExecuteScalarAsync<int>(
                            "SELECT COUNT(*) FROM tProductSchemeLotNumber"));

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} {this.LowerCaseName}.",
                    true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{this.ProperCaseName} status failed.",
                    true);

                throw new BaseException(
                    $"Database exception occurred. {this.ProperCaseName} status failed.",
                    ex);
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
                    var schemeLots = new List<ProductSchemeLotNumberModel>();
                    foreach (DataRow row in rows)
                    {
                        schemeLots.Add(
                            new ProductSchemeLotNumberModel
                            {
                                ProductSchemeLotNumberCode =
                                    Convert.ToString(row["ProductSchemeLotNumberCode"]),
                                ProductSchemeLotNumberCodeDefault = Convert.ToString(
                                    row["ProductSchemeLotNumberCodeDefault"])
                            });
                    }

                    await Conn.InsertAllAsync(schemeLots);
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
        /// Creates a request for data.
        /// </summary>
        /// <returns>The request as a string.</returns>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>REQUEST PRODUCT SCHEME LOT NUMBERS</action>" +
                await SysCommon.GetRequestDefaults(true) +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}