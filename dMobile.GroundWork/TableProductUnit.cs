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
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom.", true);

                var responseData = string.Empty;
                var resultsFamily = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (resultsFamily?.Count > 0)
                {
                    foreach (var family in resultsFamily)
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);
                        responseData = await this.SendRequest(family.FamilyCode);
                        await this.SaveOpt(responseData);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom failed.", true);

                throw new Exception("Database exception occurred. Seeding product uom failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom failed.", true);

                throw new Exception("Seeding product uom failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductUnit");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom failed.", true);

                throw new Exception("Database exception occurred. Purging product uom failed.", ex);
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
                throw new Exception("Database exception occurred. Product uom status failed.", ex);
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
        protected override async Task Save(string responseData)
        {
        }

        /// <summary>
        /// The SaveOpt.
        /// </summary>
        /// <param name="responseData"></param>
        protected async Task SaveOpt(string responseData)
        {
            var errMsg = new StringBuilder();
            var recordCount = 0;
            var unitLookup = new Dictionary<string, int>();

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    var startIndex = responseData.IndexOf("<VFPData", StringComparison.Ordinal);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    using var stringReader = new StringReader(responseData.Substring(startIndex));
                    using var reader = XmlReader.Create(stringReader);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom.", false);

                    var resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit");

                    if (resultsUnit?.Count > 0)
                    {
                        foreach (var unit in resultsUnit)
                        {
                            unitLookup.Add(unit.UnitCode, unit.UnitRefno);
                        }
                    }

                    var prodRefno = 0;
                    var unitRefno = 0;
                    var baseUnitRefno = 0;
                    var wtUnitRefno = 0;
                    var isProdUnitFound = false;
                    var prodCode = string.Empty;

                    // f01
                    var isProdUnitBC = false;

                    // f02
                    decimal prodUnitWeight = 0;

                    // f03
                    var description = string.Empty;

                    // f04
                    var prodFKRefno = 0;

                    // f05
                    var unitCode = string.Empty;

                    // f06
                    var baseUnitCode = string.Empty;

                    // f07
                    var wtUnitCode = string.Empty;

                    // f08
                    var isProdUnitSellUnit = false;

                    // f09
                    var isProdUnitDefault = false;

                    // f10
                    decimal boxCount = 0;

                    // f11
                    var weightTolerance = 0.00M;

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                    while (!reader.EOF)
                    {
                        prodRefno = 0;
                        unitRefno = 0;
                        baseUnitRefno = 0;
                        wtUnitRefno = 0;
                        isProdUnitFound = false;
                        prodCode = string.Empty;
                        isProdUnitBC = false;
                        prodUnitWeight = 0;
                        description = string.Empty;
                        prodFKRefno = 0;
                        unitCode = string.Empty;
                        baseUnitCode = string.Empty;
                        wtUnitCode = string.Empty;
                        isProdUnitSellUnit = false;
                        isProdUnitDefault = false;
                        boxCount = 0;
                        weightTolerance = 0.00M;

                        var recordFilled = false;
                        var elementName = string.Empty;

                        while (!recordFilled && reader.Read())
                        {
                            // fill one record
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                elementName = reader.Name;
                            }
                            else if (reader.NodeType == XmlNodeType.Text)
                            {
                                switch (elementName)
                                {
                                    case "f01":
                                        isProdUnitBC = reader.ReadContentAsBoolean();
                                        break;
                                    case "f02":
                                        prodUnitWeight = reader.ReadContentAsDecimal();
                                        break;
                                    case "f03":
                                        description = reader.Value;
                                        break;
                                    case "f04":
                                        prodFKRefno = reader.ReadContentAsInt();
                                        break;
                                    case "f05":
                                        unitCode = reader.Value;
                                        break;
                                    case "f06":
                                        baseUnitCode = reader.Value;
                                        break;
                                    case "f07":
                                        wtUnitCode = reader.Value;
                                        break;
                                    case "f08":
                                        isProdUnitSellUnit = reader.ReadContentAsBoolean();
                                        break;
                                    case "f09":
                                        isProdUnitDefault = reader.ReadContentAsBoolean();
                                        break;
                                    case "f10":
                                        boxCount = reader.ReadContentAsDecimal();
                                        break;
                                    case "f11":
                                        weightTolerance = reader.ReadContentAsDecimal();
                                        break;
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
                            {
                                recordFilled = true; // we are full;
                            }
                        } // end of fill record

                        if (!recordFilled)
                        {
                            continue; // prevent the last record (empty) from being used (set to last due to no overwriting)
                        }

                        prodCode = string.Empty;

                        errMsg.Length = 0;
                        errMsg.Append("FK refno: ").Append(prodFKRefno);

                        errMsg.Append(Environment.NewLine).Append("Is sell unit: ").Append(isProdUnitSellUnit);
                        errMsg.Append(Environment.NewLine).Append("Is bc: ").Append(isProdUnitBC);
                        errMsg.Append(Environment.NewLine).Append("Is default: ").Append(isProdUnitDefault);
                        errMsg.Append(Environment.NewLine).Append("Box count: ").Append(boxCount);

                        var resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                        if (resultsProduct?.Count > 0)
                        {
                            prodRefno = resultsProduct[0].ProductRefno;
                            prodCode = resultsProduct[0].ProductCode;
                        }
                        else
                        {
                            throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg.ToString());
                        }

                        errMsg.Append(Environment.NewLine).Append("Refno: \'").Append(prodRefno).Append("\'");
                        errMsg.Append(Environment.NewLine).Append("Code: \'").Append(prodCode).Append("\'");
                        errMsg.Append(Environment.NewLine).Append("Unit: \'").Append(unitCode).Append("\'");

                        unitLookup.TryGetValue(unitCode, out unitRefno);

                        errMsg.Append(Environment.NewLine).Append("Base unit: \'").Append(baseUnitCode).Append("\'");
                        unitLookup.TryGetValue(baseUnitCode, out baseUnitRefno);

                        errMsg.Append(Environment.NewLine).Append("Weight unit: \'").Append(wtUnitCode).Append("\'");
                        unitLookup.TryGetValue(wtUnitCode, out wtUnitRefno);

                        isProdUnitFound = false;

                        var resultsUnitProductUnit = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitRefno, isProdUnitBC);

                        if (resultsUnitProductUnit?.Count > 0)
                        {
                            isProdUnitFound = true;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                var queryUpdate = @"UPDATE tProductUnit
                                            SET BaseUnitRefno =  ?,
                                            WtUnitRefno = ?,
                                            ProductUnitWeight = ?,
                                            ProductUnitSizeDescription = ?,
                                            ProductUnitIsSellUnit = ?,
                                            ProductUnitIsDefault = ?,
                                            ProductUnitBoxCount = ?
                                             WHERE ProductRefno = ?
                                             AND UnitRefno = ?
                                             AND ProductUnitIsBC = ?";
                                var parametersUpdate = new object[]
                                {
                                        Convert.ToInt32(baseUnitRefno),
                                        Convert.ToInt32(wtUnitRefno),
                                        Convert.ToDecimal(prodUnitWeight),
                                        Convert.ToString(description),
                                        Convert.ToInt32(isProdUnitSellUnit),
                                        Convert.ToInt32(isProdUnitDefault),
                                        Convert.ToDecimal(boxCount),
                                        Convert.ToInt32(prodRefno),
                                        Convert.ToInt32(unitRefno),
                                        Convert.ToInt32(isProdUnitBC),
                                };

                                transaction.Execute(queryUpdate, parametersUpdate);
                            }
                        }

                        if (!isProdUnitFound)
                        {
                            var queryInsert = "INSERT INTO tProductUnit " +
                                              "(ProductRefno, " +
                                              "UnitRefno, " +
                                              "BaseUnitRefno, " +
                                              "WtUnitRefno, " +
                                              "ProductUnitIsBC, " +
                                              "ProductUnitBC, " +
                                              "ProductUnitWeight, " +
                                              "ProductUnitSizeDescription, " +
                                              "ProductUnitIsSellUnit, " +
                                              "ProductUnitIsDefault, " +
                                              "ProductUnitBoxCount," +
                                              "ProductUnitWeightTolerance) " +
                                              "VALUES " +
                                              "(?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?," +
                                              "?)";
                            var parametersInsert = new object[]
                            {
                                    Convert.ToInt32(prodRefno),
                                    Convert.ToInt32(unitRefno),
                                    Convert.ToInt32(baseUnitRefno),
                                    Convert.ToInt32(wtUnitRefno),
                                    Convert.ToInt32(isProdUnitBC),
                                    Convert.ToString(isProdUnitBC ? "X" : string.Empty),
                                    Convert.ToDecimal(prodUnitWeight),
                                    Convert.ToString(description),
                                    Convert.ToInt32(isProdUnitSellUnit),
                                    Convert.ToInt32(isProdUnitDefault),
                                    Convert.ToDecimal(boxCount),
                                    Convert.ToDecimal(weightTolerance),
                            };

                            transaction.Execute(queryInsert, parametersInsert);
                        }

                        ++recordCount;
                        if (recordCount % 100 == 0)
                        {
                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, recordCount / 10 % 100, 0, 100);
                        }
                    } // end of responseData

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {recordCount}", false);

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom failed.", false);

                    throw new Exception("Database exception occurred. Saving product uom failed." + Environment.NewLine + errMsg.ToString(), ex);
                }
                catch (Exception e)
                {
                    // MessageBox.Show(e.Message);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, e.Message, true);
                    throw;
                }
            });
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

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
