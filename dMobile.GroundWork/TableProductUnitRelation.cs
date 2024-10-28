// <copyright file="TableProductUnitRelation.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableProductUnitRelation" />.
    /// </summary>
    public class TableProductUnitRelation : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductUnitRelation"/> class.
        /// </summary>
        public TableProductUnitRelation()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnitRelation";
            this.LowerCaseName = "product unit relations";
            this.ProperCaseName = "Product unit relations";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var responseData = await this.SendRequest(string.Empty);
                await this.SaveOpt(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{this.LowerCaseName} seeded.", true);

                ////var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<FamilyModel>("SELECT * FROM tFamily");

                ////if (results?.Count > 0)
                ////{
                ////    foreach (var family in results)
                ////    {
                ////        var responseData = await this.SendRequest(family.FamilyCode);

                ////        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);
                ////        await this.SaveOpt(responseData);
                ////    }

                ////    // Require to have product uom relations.
                ////    if (await this.DataStatus() <= 0)
                ////    {
                ////        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "No product uom relations where received.", true);

                ////        throw new Exception("No product uom relations where received.");
                ////    }

                ////    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations complete.", true);
                ////}
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);

                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductUnitRelation");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom relations failed.", true);

                throw new Exception("Database exception occurred. Purging product uom relations failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductUnitRelation"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} product unit relations.", false);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Product unit relations status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest.
        /// </summary>
        /// <param name="strFamilyCode"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(string strFamilyCode)
        {
            return await SendComm(await this.CreateRequest(strFamilyCode));
        }

        /// <summary>
        /// The SaveOpt.
        /// </summary>
        /// <param name="responseData"></param>
        protected async Task SaveOpt(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var prodUnitRelations = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        prodUnitRelations.Add(new Seg
                        {
                            ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"]),
                            SellToBaseFactor = Convert.ToDecimal(row["SellToBaseFactor"]),
                            ProductFKRefno = Convert.ToInt32(row["ProductFKRefno"]),
                            ProductUnitCode = row["ProductUnitCode"].ToString(),
                            ProductBaseUnitCode = row["ProductBaseUnitCode"].ToString(),
                        });
                    }

                    var query = "CREATE TEMP TABLE Seg (" +
                        "ProductUnitIsBC BIT NOT NULL" +
                        ",SellToBaseFactor DECIMAL NOT NULL" +
                        ",ProductFKRefno INTEGER NOT NULL" +
                        ",ProductUnitCode VARCHAR(2) NOT NULL" +
                        ",ProductBaseUnitCode VARCHAR(2) NOT NULL)";
                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(prodUnitRelations);

                    query = "INSERT INTO tProductUnitRelation (" +
                        "ProductUnit1Refno" +
                        ",ProductUnit2Refno" +
                        ",ProductUnitRelationFactor" +
                        ") SELECT " +
                        "pu1.ProductUnitRefno" +
                        ",pu2.ProductUnitRefno" +
                        ",Seg.SellToBaseFactor " +
                        "FROM Seg" +
                        " INNER JOIN tProduct prod ON prod.ProductFKRefno = Seg.ProductFKRefno" +
                        " INNER JOIN tProductUnit pu1 ON pu1.ProductRefno = prod.ProductRefno" +
                            " AND pu1.UnitRefno = (SELECT UnitRefno FROM tUnit WHERE UnitCode = Seg.ProductUnitCode)" +
                            " AND pu1.ProductUnitIsBC = Seg.ProductUnitIsBC" +
                        " INNER JOIN tProductUnit pu2 ON pu2.ProductRefno = prod.ProductRefno" +
                            " AND pu2.UnitRefno = (SELECT UnitRefno FROM tUnit WHERE UnitCode = Seg.ProductBaseUnitCode)" +
                            " AND pu2.ProductUnitIsBC = Seg.ProductUnitIsBC";
                    var rowsAffected = await conn.ExecuteAsync(query);
                    if (prodUnitRelations.Count != rowsAffected)
                    {
                        throw new Exception("Failed to insert all rows into tProductUnitRelation");
                    }

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await conn.ExecuteAsync(dropTempTableQuery);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rows.Count} {this.LowerCaseName}.", true);
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
        /// The CreateRequest.
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected async override Task<string> CreateRequest(string familyCode)
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
                + "<action>REQUEST PRODUCT UNIT RELATIONS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<family_code>" + familyCode + "</family_code>"
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
            public bool ProductUnitIsBC { get; set; }

            public decimal SellToBaseFactor { get; set; }

            public int ProductFKRefno { get; set; }

            public string ProductUnitCode { get; set; }

            public string ProductBaseUnitCode { get; set; }
        }
    }
}
