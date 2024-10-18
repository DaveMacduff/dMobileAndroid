// <copyright file="TableProductUnitBarCode.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models.Products;
    using dMobile.Resources.Types;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableProductUnitBarCode" />.
    /// </summary>
    public class TableProductUnitBarCode : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductUnitBarCode"/> class.
        /// </summary>
        public TableProductUnitBarCode()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnitBarCode";
            this.LowerCaseName = "product unit bar codes";
            this.ProperCaseName = "Product unit bar codes";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} complete.", true);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tActivityItemBarCodeAssociationXRef");
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductUnitBarCode");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);

                throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductUnitBarCode"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} {this.LowerCaseName}.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database exception occurred. {this.ProperCaseName} status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest.
        /// </summary>
        /// <param name="strFamilyCode">The strFamilyCode<see cref="string"/>.</param>
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
                    var barcodes = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        barcodes.Add(new Seg
                        {
                            ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"]),
                            ProductFKRefno = Convert.ToInt32(row["ProductFKRefno"]),
                            ProductUnitCode = row["ProductUnitCode"].ToString(),
                            ProductUnitBarCodeCode = row["ProductUnitBarCodeCode"].ToString(),
                            ProductUnitBarCodeRaw = string.Empty,
                            ProductUnitBarCodeSymbologyEnum = Convert.ToInt32(row["ProductUnitBarCodeSymbologyEnum"]),
                            ProductUnitBarCodeModifier1 = row["ProductUnitBarCodeModifier1"].ToString(),
                        });
                    }

                    var query = "CREATE TEMP TABLE Seg " +
                        "(" +
                        "ProductUnitIsBC BIT NOT NULL," +
                        "ProductFKRefno INTEGER NOT NULL," +
                        "ProductUnitCode VARCHAR(2) NOT NULL," +
                        "ProductUnitBarCodeCode VARCHAR(20) NOT NULL," +
                        "ProductUnitBarCodeRaw VARCHAR(60) NOT NULL," +
                        "ProductUnitBarCodeSymbologyEnum INTEGER NOT NULL," +
                        "ProductUnitBarCodeModifier1 VARCHAR(1) NOT NULL" +
                        ")";
                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(barcodes);

                    try
                    {
                        query = "INSERT INTO tProductUnitBarCode (" +
                                "ProductUnitRefno" +
                                ",ProductUnitBarCodeCode" +
                                ",ProductUnitBarCodeRaw" +
                                ",ProductUnitBarCodeSymbologyEnum" +
                                ",ProductUnitBarCodeModifier1" +
                                ",ProductUnitBarCodeStatus)" +
                                " SELECT pu.ProductUnitRefno" +
                                ",Seg.ProductUnitBarCodeCode" +
                                ",Seg.ProductUnitBarCodeRaw" +
                                ",Seg.ProductUnitBarCodeSymbologyEnum" +
                                ",Seg.ProductUnitBarCodeModifier1" +
                                ",0 as ProductUnitBarCodeStatus" +
                                " FROM Seg" +
                                " INNER JOIN tUnit unit ON unit.UnitCode = Seg.ProductUnitCode" +
                                " INNER JOIN tProduct prod ON prod.ProductFKRefno = Seg.ProductFKRefno" +
                                " INNER JOIN tProductUnit pu ON pu.ProductRefno = prod.ProductRefno" +
                                " AND pu.UnitRefno = unit.UnitRefno" +
                                " AND pu.ProductUnitIsBC = Seg.ProductUnitIsBC";
                        var rowsAffected = await conn.ExecuteAsync(query);
                        if (barcodes.Count != rowsAffected)
                        {
                            throw new Exception("Failed to insert all rows into tProductUnitBarCode");
                        }
                    }
                    catch (SQLiteException sqlEx)
                    {
                        if (sqlEx.Message.Contains("UNIQUE constraint failed"))
                        {
                            await this.PurgeData();
                            var barCodeInfo = string.Empty;
                            try
                            {
                                query = "SELECT 0 as ProductUnitBarCodeRefno" +
                                        ",pu.ProductUnitRefno" +
                                        ",Seg.ProductUnitBarCodeCode" +
                                        ",Seg.ProductUnitBarCodeRaw" +
                                        ",Seg.ProductUnitBarCodeSymbologyEnum" +
                                        ",Seg.ProductUnitBarCodeModifier1" +
                                        ",0 as ProductUnitBarCodeStatus" +
                                        " FROM Seg" +
                                        " INNER JOIN tUnit unit ON unit.UnitCode = Seg.ProductUnitCode" +
                                        " INNER JOIN tProduct prod ON prod.ProductFKRefno = Seg.ProductFKRefno" +
                                        " INNER JOIN tProductUnit pu ON pu.ProductRefno = prod.ProductRefno" +
                                        " AND pu.UnitRefno = unit.UnitRefno" +
                                        " AND pu.ProductUnitIsBC = Seg.ProductUnitIsBC";
                                var barCodes = await conn.QueryAsync<ProductUnitBarCodeModel>(query);
                                foreach (var barCode in barCodes)
                                {
                                    barCodeInfo = $"{barCode.ProductUnitBarCodeCode} - {barCode.ProductUnitBarCodeModifier1}";
                                    await conn.InsertAsync(barCode);
                                }
                            }
                            catch (SQLiteException sqlEx2)
                            {
                                if (sqlEx2.Message.Contains("UNIQUE constraint failed"))
                                {
                                    throw new Exception($"Seeding {this.LowerCaseName} failed. Duplicate {barCodeInfo}");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            throw;
                        }
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
        protected override async Task<string> CreateRequest(string familyCode)
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
                + "<action>REQUEST PRODUCT UNIT BAR CODES</action>"
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

            public int ProductFKRefno { get; set; }

            public string ProductUnitCode { get; set; }

            public string ProductUnitBarCodeCode { get; set; }

            public string ProductUnitBarCodeRaw { get; set; }

            public int ProductUnitBarCodeSymbologyEnum { get; set; }

            public string ProductUnitBarCodeModifier1 { get; set; }
        }
    }
}
