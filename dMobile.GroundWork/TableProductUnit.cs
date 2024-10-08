// <copyright file="TableProductUnit.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Models;
    using dMobile.Resources.Types;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableProductUnit" />.
    /// </summary>
    public class TableProductUnit : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductUnit"/> class.
        /// </summary>
        public TableProductUnit()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductUnit";
            this.LowerCaseName = "product units";
            this.ProperCaseName = "Product Units";
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

                ////await this.SaveOpt(responseData);
                ////var responseData = string.Empty;
                ////var resultsFamily = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<FamilyModel>("SELECT * FROM tFamily");

                ////if (resultsFamily?.Count > 0)
                ////{
                ////    foreach (var family in resultsFamily)
                ////    {
                ////        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);
                ////        responseData = await this.SendRequest(family.FamilyCode);
                ////        await this.SaveOpt(responseData);
                ////    }
                ////}

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);

                throw new Exception($"Database exception occurred. Seeding {this.LowerCaseName} failed.", ex);
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

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductUnit");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
            }
        }

        ///// <summary>
        ///// The GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC
        ///// </summary>
        ///// <param name="prodRefno"></param>
        ///// <param name="unitRefno"></param>
        ///// <param name="prodUnitIsBC"></param>
        ///// <returns></returns>
        // public int GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC(
        //    int prodRefno,
        //    int unitRefno,
        //    bool prodUnitIsBC)
        // {
        //    int prodUnitRefno = 0;

        // try
        //            {
        //                using (SQLiteCommand cmndGetProductUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //                {
        //                    cmndGetProductUnit.CommandText = "tProductUnit";
        //                    cmndGetProductUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
        //                    cmndGetProductUnit.CommandType = CommandType.TableDirect;

        // using (SQLiteDataReader drProductUnit = cmndGetProductUnit.ExecuteReader())
        //                    {
        //                        if (drProductUnit.Seek(
        //                            DbSeekOptions.FirstEqual,
        //                            new object[] { prodRefno, unitRefno, prodUnitIsBC }))
        //                        {
        //                            if (drProductUnit.Read())
        //                            {
        //                                prodUnitRefno = Convert.ToInt32(drProductUnit.GetValue(Convert.ToInt32(
        //                                    SysCommon.ProdUnitFields.Refno)));
        //                            }
        //                            else
        //                            {
        // #if MULTILANGUAGE
        //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrproductunitreadFailed));
        // #else
        //                                throw new Exception("drProductUnit.Read() failed.");
        // #endif
        //                            }
        //                        }
        //                        else
        //                        {
        //                            throw new Exception(
        //                                string.Format(
        // #if MULTILANGUAGE
        //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductUnitRefnoFromProductRefnoAndUnitRefnoAndIsBc) +
        //                  Translator.GetInstance().Translate(TranslatorVocabulary.InTproductunit),
        // #else
        // "Unable to locate product unit refno from product refno and unit refno and is BC {0}, {1}, {2} " +
        //                                "in tProductUnit",
        // #endif
        // prodRefno,
        //                                prodUnitRefno,
        //                                prodUnitIsBC));
        //                        }

        // drProductUnit.Close();
        //                    }
        //                }
        //            }
        //            catch (SQLiteException ex)
        //            {
        //                throw new Exception(
        // #if MULTILANGUAGE
        //            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodunitrefnofromprodrefnoandunitrefnoandprodunitisbcFailed), ex);
        // #else
        // "Database exception occurred. GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC failed.", ex);
        // #endif
        //            }

        // return prodUnitRefno;
        // }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductUnit"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} product units.", true);
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
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(string familyCode)
        {
            return await this.SendComm(await this.CreateRequest(familyCode));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override Task Save(string responseData)
        {
            return Task.CompletedTask;
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
                    var prodUnits = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        prodUnits.Add(new Seg
                        {
                            IsBCUnit = Convert.ToBoolean(row["IsBCUnit"]),
                            BaseWeight = Convert.ToDecimal(row["BaseWeight"]),
                            UnitSize = Convert.ToString(row["UnitSize"]),
                            ProductFKRefno = Convert.ToInt32(row["ProductFKRefno"]),
                            UnitCode = Convert.ToString(row["UnitCode"]),
                            BaseUnitCode = Convert.ToString(row["BaseUnitCode"]),
                            WeightUnitCode = Convert.ToString(row["WeightUnitCode"]),
                            IsSellUnit = Convert.ToBoolean(row["IsSellUnit"]),
                            IsDefault = Convert.ToBoolean(row["IsDefault"]),
                            BoxCount = Convert.ToDecimal(row["BoxCount"]),
                            WeightTolerance = Convert.ToDecimal(row["WeightTolerance"]),
                        });
                    }

                    var query = "CREATE TEMP TABLE Seg ("
                                + "IsBCUnit BIT, "
                                + "BaseWeight NUMERIC(11, 3), "
                                + "UnitSize VARCHAR(15), "
                                + "ProductFKRefno INTEGER, "
                                + "UnitCode VARCHAR(2), "
                                + "BaseUnitCode VARCHAR(2), "
                                + "WeightUnitCode VARCHAR(2), "
                                + "IsSellUnit BIT, "
                                + "IsDefault BIT, "
                                + "BoxCount NUMERIC(10, 4), "
                                + "WeightTolerance NUMERIC(6, 3))";
                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(prodUnits);

                    query = "INSERT INTO tProductUnit (" +
                        "ProductRefno" +
                        ",UnitRefno" +
                        ",BaseUnitRefno" +
                        ",WtUnitRefno" +
                        ",ProductUnitIsBC" +
                        ",ProductUnitBC" +
                        ",ProductUnitWeight" +
                        ",ProductUnitSizeDescription" +
                        ",ProductUnitIsSellUnit" +
                        ",ProductUnitIsDefault" +
                        ",ProductUnitBoxCount" +
                        ",ProductUnitWeightTolerance)" +
                        " SELECT tProduct.ProductRefno" +
                        ",SellUnit.UnitRefno" +
                        ",BaseUnit.UnitRefno as BaseUnitRefno" +
                        ",WeightUnit.UnitRefno as WtUnitRefno" +
                        ",Seg.IsBCUnit" +
                        ",CASE WHEN Seg.IsBCUnit = 1 THEN 'X' ELSE '' END as ProductUnitBC" +
                        ",Seg.BaseWeight as ProductUnitWeight" +
                        ",Seg.UnitSize as ProductUnitSizeDescription" +
                        ",Seg.IsSellUnit" +
                        ",Seg.IsDefault" +
                        ",Seg.BoxCount" +
                        ",Seg.WeightTolerance" +
                        " FROM Seg " +
                        " INNER JOIN tProduct ON tProduct.ProductFKRefno = Seg.ProductFKRefno" +
                        " INNER JOIN tUnit SellUnit ON SellUnit.UnitCode = Seg.UnitCode" +
                        " INNER JOIN tUnit BaseUnit ON BaseUnit.UnitCode = Seg.UnitCode" +
                        " INNER JOIN tUnit WeightUnit ON WeightUnit.UnitCode = Seg.UnitCode";
                    var rowsAffected = await conn.ExecuteAsync(query);
                    if (prodUnits.Count != rowsAffected)
                    {
                        throw new Exception("Failed to insert all rows into tProductUnit");
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
                + "<action>REQUEST PRODUCT UNITS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<family_code>" + familyCode + "</family_code>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Class to hold the data for the temporary table.
        /// </summary>
        private class Seg
        {
            public bool IsBCUnit { get; set; }

            public decimal BaseWeight { get; set; }

            public string UnitSize { get; set; }

            public int ProductFKRefno { get; set; }

            public string UnitCode { get; set; }

            public string BaseUnitCode { get; set; }

            public string WeightUnitCode { get; set; }

            public bool IsSellUnit { get; set; }

            public bool IsDefault { get; set; }

            public decimal BoxCount { get; set; }

            public decimal WeightTolerance { get; set; }
        }
    }
}
