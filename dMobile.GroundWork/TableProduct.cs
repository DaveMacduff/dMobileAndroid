// <copyright file="TableProduct.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableProduct" />.
    /// </summary>
    public class TableProduct : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProduct"/> class.
        /// </summary>
        public TableProduct() : base()
        {
            this.ProperCaseName = "Products";
            this.LowerCaseName = "products";
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

                ////var resultsFamily = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<FamilyModel>("SELECT * FROM tFamily");

                ////if (resultsFamily?.Count > 0)
                ////{
                ////    foreach (var family in resultsFamily)
                ////    {
                ////        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);
                ////        var responseData = await this.SendRequest(family.FamilyCode);
                ////        await this.SaveOpt(responseData);
                ////    }
                ////}

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{this.LowerCaseName} seeded.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);

                throw new BaseException($"Database exception occurred. Seeding {this.LowerCaseName} failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProduct");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);

                throw new BaseException($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
            }
        }

        /// <summary>
        /// The GetProdRefnoFromProdFKRefno.
        /// </summary>
        /// <param name="prodFKRefno"></param>
        /// <returns></returns>
        public async Task<int> GetProdRefnoFromProdFKRefno(int prodFKRefno)
        {
            int prodRefno;
            try
            {
                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(prodFKRefno));

                if (results?.Count > 0)
                {
                    prodRefno = results[0].ProductRefno;
                }
                else
                {
                    throw new BaseException(
                        $"Unable to locate product Refno from {this.LowerCaseName} FK Refno {prodFKRefno} in tProduct");
                }
            }
            catch (SQLiteException ex)
            {
                throw new BaseException("Database exception occurred. GetProdRefnoFromProdFKRefno failed.", ex);
            }

            return prodRefno;
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProduct"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} products.", true);
            }
            catch (SQLiteException ex)
            {
                throw new BaseException($"Database exception occurred. {this.ProperCaseName} status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SaveOpt.
        /// </summary>
        /// <param name="data"></param>
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
                    var products = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        var product = new Seg
                        {
                            ProductCode = row["ProductCode"].ToString(),
                            ProductFKRefno = Convert.ToInt32(row["ProductFKRefno"]),
                            ProductDescription = row["ProductDescription"].ToString(),
                            ProductIsCatchCount = Convert.ToBoolean(row["ProductIsCatchCount"]),
                            ProductIsCatchWeight = Convert.ToBoolean(row["ProductIsCatchWeight"]),
                            ProductIsCatchAsk = Convert.ToBoolean(row["ProductIsCatchAsk"]),
                            FamilyCode = row["ProductFamily"].ToString(),
                            ProductBrandDescription = row["ProductBrand"].ToString(),
                            ProductIsCatchAskZero = Convert.ToBoolean(row["ProductCatchAskZero"]),
                            ProductAllowConvertProduct = Convert.ToBoolean(row["ProductAllowConvertProduct"]),
                            ProductAcceptDays = Convert.ToInt32(row["ProductAcceptDays"]),
                        };
                        products.Add(product);
                    }

                    var query = "CREATE TEMP TABLE Seg"
                                + " ("
                                + "ProductFKRefno INTEGER NOT NULL"
                                + ",ProductCode VARCHAR(10) NOT NULL"
                                + ",ProductDescription VARCHAR(25) NOT NULL"
                                + ",ProductIsCatchCount BIT NOT NULL"
                                + ",ProductIsCatchWeight BIT NOT NULL"
                                + ",ProductIsCatchAsk BIT NOT NULL"
                                + ",FamilyCode VARCHAR(10) NOT NULL"
                                + ",ProductBrandDescription VARCHAR(10) NOT NULL"
                                + ",ProductIsCatchAskZero BIT NOT NULL"
                                + ",ProductAllowConvertProduct BIT NOT NULL"
                                + ",ProductAcceptDays INTEGER"
                                + ")";

                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(products);

                    query = @"INSERT INTO tProduct 
                    (FamilyRefno,
                    ProductSchemeRefno,
                    ProductCode, 
                    ProductFKRefno, 
                    ProductDescription, 
                    ProductIsCatchCount, 
                    ProductIsCatchWeight, 
                    ProductIsCatchAsk, 
                    ProductIsCatchAskZero, 
                    ProductBrandDescription, 
                    ProductAllowConvertProduct, 
                    ProductAcceptDays
                    ) SELECT
                    tFamily.FamilyRefno, 
                    0,
                    Seg.ProductCode, 
                    Seg.ProductFKRefno, 
                    Seg.ProductDescription, 
                    Seg.ProductIsCatchCount, 
                    Seg.ProductIsCatchWeight, 
                    Seg.ProductIsCatchAsk, 
                    Seg.ProductIsCatchAskZero, 
                    Seg.ProductBrandDescription, 
                    Seg.ProductAllowConvertProduct, 
                    Seg.ProductAcceptDays
                    FROM Seg
                    INNER JOIN tFamily ON tFamily.FamilyCode = Seg.FamilyCode";
                    var rowsAffected = await conn.ExecuteAsync(query);
                    if (products.Count != rowsAffected)
                    {
                        throw new BaseException("Failed to insert all rows into tProduct");
                    }

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await conn.ExecuteAsync(dropTempTableQuery);

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
                        $"{products.Count} products.", true);
                }
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
        /// <param name="strFamilyCode">The strFamilyCode<see cref="string"/>.</param>
        /// <returns></returns>
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
                $@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
                    <imclient>
                    <command>
                        <action>REQUEST PRODUCTS</action>
                        {await SysCommon.GetRequestDefaults(true)}
                        <family_code>{strFamilyCode}</family_code>
                    </command>
                    </imclient>");

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
            public string ProductCode { get; set; }

            public int ProductFKRefno { get; set; }

            public string ProductDescription { get; set; }

            public bool ProductIsCatchCount { get; set; }

            public bool ProductIsCatchWeight { get; set; }

            public bool ProductIsCatchAsk { get; set; }

            public string FamilyCode { get; set; }

            public string ProductBrandDescription { get; set; }

            public bool ProductIsCatchAskZero { get; set; }

            public bool ProductAllowConvertProduct { get; set; }

            public int ProductAcceptDays { get; set; }
        }
    }
}