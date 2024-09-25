// <copyright file="TableProductUnitRelation.cs" company="InterSect Business Systems Inc">
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
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations.", true);

                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (results?.Count > 0)
                {
                    foreach (var family in results)
                    {
                        var responseData = await this.SendRequest(family.FamilyCode);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);
                        await this.SaveOpt(responseData);
                    }

                    // Require to have product uom relations.
                    if (await this.DataStatus() <= 0)
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "No product uom relations where received.", true);

                        throw new Exception("No product uom relations where received.");
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations complete.", true);
                }
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations failed.", true);

                throw new Exception("Database exception occurred. Seeding product uom relations failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom relations failed.", true);

                throw new Exception("Seeding product uom relations failed.", ex);
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
            return await this.SendComm(await this.CreateRequest(strFamilyCode));
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
            var errMsg = string.Empty;
            var recordCount = 0;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    var startIndex = responseData.IndexOf("<VFPData", StringComparison.Ordinal);
                    var sett = new XmlReaderSettings();
                    sett.CheckCharacters = false;
                    sett.CloseInput = false;
                    sett.ConformanceLevel = ConformanceLevel.Auto;
                    sett.IgnoreComments = true;
                    sett.IgnoreProcessingInstructions = true;
                    sett.IgnoreWhitespace = false;
                    sett.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                    sett.ValidationType = ValidationType.None;

                    using var stringReader = new StringReader(responseData.Substring(startIndex));
                    using var reader = XmlReader.Create(stringReader, sett);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations.", false);

                    var unitLookup = new Dictionary<string, int>();
                    var lastFKRefno = 0;
                    var refreshOnly = this.SysEnv.RefreshOpt.RefreshDataChangesOnly;
                    var prodRefno = 0;
                    var unitCode1Refno = 0;
                    var unitCode2Refno = 0;
                    var isProdUnitRelationFound = false;
                    var prodUnit1Refno = 0;
                    var prodUnit2Refno = 0;

                    // f01
                    var prodUnitIsBC = false;

                    // f02
                    decimal relationFactor = 0;

                    // f03
                    var prodFKRefno = 0;

                    // f04
                    var unitCode1 = string.Empty;

                    // f05
                    var unitCode2 = string.Empty;

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                    while (!reader.EOF)
                    {
                        // fill one record
                        // we do this by looping until an end segment is found
                        var recordFilled = false;
                        var elementName = string.Empty;
                        while (!recordFilled && reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                elementName = reader.Name;
                            }
                            else if (reader.NodeType == XmlNodeType.Text)
                            {
                                switch (elementName)
                                {
                                    case "f01":
                                        prodUnitIsBC = reader.ReadContentAsBoolean();
                                        break;
                                    case "f02":
                                        relationFactor = reader.ReadContentAsDecimal();
                                        break;
                                    case "f03":
                                        prodFKRefno = reader.ReadContentAsInt();
                                        break;
                                    case "f04":
                                        unitCode1 = reader.Value;
                                        break;
                                    case "f05":
                                        unitCode2 = reader.Value;
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
                            continue;
                        }

                        errMsg = "FK refno " + prodFKRefno;

                        if (prodFKRefno == lastFKRefno)
                        {
                            // no op because prodRefno already set to last prodRefno.
                            // this is a last object cache.
                            // curr prod refno == last prod refno
                        }
                        else
                        {
                            lastFKRefno = prodFKRefno;
                            var resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                            if (resultsProduct?.Count > 0)
                            {
                                prodRefno = resultsProduct[0].ProductRefno;
                            }
                            else
                            {
                                throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
                            }
                        }

                        errMsg += Environment.NewLine + "Unit code 1 " + unitCode1;

                        if (!unitLookup.TryGetValue(unitCode1, out unitCode1Refno))
                        {
                            var resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit WHERE UnitCode = ?", unitCode1);

                            if (resultsUnit?.Count > 0)
                            {
                                unitCode1Refno = resultsUnit[0].UnitRefno;
                                unitLookup.Add(unitCode1, unitCode1Refno);
                            }
                            else
                            {
                                throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
                            }
                        }

                        errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

                        var resultsProductUnit = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitCode1Refno, prodUnitIsBC);

                        if (resultsProductUnit?.Count > 0)
                        {
                            prodUnit1Refno = resultsProductUnit[0].ProductUnitRefno;
                        }
                        else
                        {
                            throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                        }

                        errMsg += Environment.NewLine + "Unit code 2 " + unitCode2;

                        if (!unitLookup.TryGetValue(unitCode2, out unitCode2Refno))
                        {
                            var resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit WHERE UnitCode = ?", unitCode2);

                            if (resultsUnit?.Count > 0)
                            {
                                unitCode2Refno = resultsUnit[0].UnitRefno;
                                unitLookup.Add(unitCode2, unitCode2Refno);
                            }
                            else
                            {
                                throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                            }
                        }

                        var resultsProductUnit2 = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitCode2Refno, prodUnitIsBC);

                        if (resultsProductUnit?.Count > 0)
                        {
                            prodUnit2Refno = resultsProductUnit2[0].ProductUnitRefno;
                        }
                        else
                        {
                            throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                        }

                        isProdUnitRelationFound = false;

                        var resultsProductUnitRelation = transaction.Query<tProductUnitRelationModel>("SELECT * FROM tProductUnitRelation WHERE ProductUnit1Refno = ? AND ProductUnit2Refno = ?", prodUnit1Refno, prodUnit2Refno);

                        if (resultsProductUnitRelation?.Count > 0)
                        {
                            isProdUnitRelationFound = true;

                            if (refreshOnly)
                            {
                                transaction.Execute("UPDATE tProductUnitRelation SET ProductUnitRelationFactor = ? WHERE ProductUnit1Refno = ? AND ProductUnit2Refno = ?", relationFactor, prodUnit1Refno, prodUnit2Refno);
                            }
                        }

                        if (!isProdUnitRelationFound)
                        {
                            transaction.Execute("INSERT INTO tProductUnitRelation (ProductUnit1Refno, ProductUnit2Refno, ProductUnitRelationFactor) VALUES (?, ?, ?)", prodUnit1Refno, prodUnit2Refno, relationFactor);
                        }

                        ++recordCount;
                        if (recordCount % 100 == 0)
                        {
                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, recordCount / 10 % 100, 0, 100);
                        }
                    }

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {recordCount}", false);

                    unitLookup = null;
                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations failed.", false);

                    throw new Exception("Database exception occurred. Saving product uom relations failed." + errMsg, ex);
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
    }
}
