// <copyright file="TableLocationMapWarehouseProductScheme.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using SQLite;

    public class TableLocationMapWarehouseProductScheme : TableBase
    {
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
                var responseData = await this.SendRequest(string.Empty);
                await this.Save(responseData);

                ////this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);
                ////var resultsFamily = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");
                ////if (resultsFamily?.Count > 0)
                ////{
                ////    foreach (var family in resultsFamily)
                ////    {
                ////        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);
                ////        var responseData = await this.SendRequest(family.FamilyCode);
                ////        await this.Save(responseData);
                ////    }
                ////}

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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapWarehouseProductScheme"));

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

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapWarehouseProductScheme");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes failed.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes failed.", true);
                throw new Exception("Database exception occurred. Purge warehouse product schemes failed.", ex);
            }
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var whsProdSchemes = rows.Cast<DataRow>().Select(row => new Seg
                    {
                        ProductSchemeCode = Convert.ToString(row["SchemeCode"]),
                        ProductFKRefno = Convert.ToInt32(row["ProductFKRefno"]),
                        LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"]),
                    }).ToList();

                    var query = "CREATE TEMP TABLE Seg"
                                + "("
                                + "ProductSchemeCode VARCHAR(10) NOT NULL,"
                                + "ProductFKRefno INTEGER NOT NULL,"
                                + "LocationMapWarehouseFKRefno INTEGER NOT NULL"
                                + ")";
                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(whsProdSchemes);

                    query = "INSERT INTO tLocationMapWarehouseProductScheme (" +
                            "WarehouseLocationMapRefno" +
                            ",ProductRefno" +
                            ",ProductSchemeRefno" +
                            ") SELECT tLocationMapWarehouse.LocationMapRefno as WarehouseLocationMapRefno" +
                            ",tProduct.ProductRefno" +
                            ",tProductScheme.ProductSchemeRefno" +
                            " FROM Seg" +
                            " INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapWarehouseFKRefno = Seg.LocationMapWarehouseFKRefno" +
                            " INNER JOIN tProduct ON tProduct.ProductFKRefno = Seg.ProductFKRefno" +
                            " INNER JOIN tProductScheme ON tProductScheme.ProductSchemeCode = Seg.ProductSchemeCode";
                    await conn.ExecuteAsync(query);

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await conn.ExecuteAsync(dropTempTableQuery);

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
                        $"{whsProdSchemes.Count} warehouse product schemes.", true);
                }
                else
                {
                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
                        $"{rows.Count} warehouse product schemes.", true);
                }
            }
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
                + "<action>REQUEST WAREHOUSE PRODUCT SCHEMES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<family_code>" + strFamilyCode + "</family_code>"
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

            public int ProductFKRefno { get; set; }

            public int LocationMapWarehouseFKRefno { get; set; }
        }

    }
}