// <copyright file="TableFamily.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using Bedrock;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Resources.Models.Products;

    public class TableFamily : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableFamily"/> class.
        /// </summary>
        public TableFamily()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableFamily";
            this.LowerCaseName = "families";
            this.ProperCaseName = "Families";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task RequestHeaderData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);

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

                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
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
                rowsAffected = Convert.ToInt32(
                    await DatabaseConnection.GetInstance()
                        .SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tFamily"));

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} families.",
                    true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Families status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tFamily");

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Purging families complete.",
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
                    "Purging families failed.",
                    true);

                throw new Exception("Database exception occurred. Purging families failed.", ex);
            }
        }

        /// <summary>
        ///     The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var families = new List<FamilyModel>();
                var rowsToSave = dataSet.Tables["tFamily"].Rows.Count;
                for (var idx = 0; idx < rowsToSave; idx++)
                {
                    var row = dataSet.Tables["tFamily"].Rows[idx];
                    families.Add(
                        new FamilyModel
                        {
                            FamilyCode = row["FamilyCode"].ToString()
                        });
                }

                await conn.InsertAllAsync(families);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{families.Count} {this.LowerCaseName}.",
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
                "<action>REQUEST FAMILIES</action>" +
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