// <copyright file="TableProductUnitBarCode.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
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
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes.", true);

                var responseData = string.Empty;
                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (results?.Count > 0)
                {
                    foreach (var family in results)
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For family {family.FamilyCode}", true);

                        responseData = await this.SendRequest(family.FamilyCode);
                        await this.SaveOpt(responseData);
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes complete.", true);
                }
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);

                throw new Exception("Database exception occurred. Seeding product uom bar codes failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);

                throw new Exception("Seeding product uom bar codes failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItemBarCodeAssociationXRef");
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductUnitBarCode");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes failed.", true);

                throw new Exception("Database exception occurred. Purging product uom bar codes failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductUnitBarCode"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} product uom bar codes.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Product uom bar codes status failed.", ex);
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
            var refreshDataChangesOnly = this.SysEnv.RefreshOpt.RefreshDataChangesOnly;
            var unitLookup = new Dictionary<string, int>();

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    var startIndex = responseData.IndexOf("<VFPData", StringComparison.Ordinal);
                    var sett = new XmlReaderSettings
                    {
                        CheckCharacters = false,
                        CloseInput = false,
                        ConformanceLevel = ConformanceLevel.Auto,
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true,
                        IgnoreWhitespace = false,
                        ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None,
                        ValidationType = ValidationType.None,
                    };

                    using var stringReader = new StringReader(responseData.Substring(startIndex));
                    using var reader = XmlReader.Create(stringReader, sett);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes.", false);

                    var resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit");

                    if (resultsUnit?.Count > 0)
                    {
                        foreach (var unit in resultsUnit)
                        {
                            unitLookup.Add(unit.UnitCode, unit.UnitRefno);
                        }
                    }

                    var prodRefno = 0;
                    var prodUnitRefno = 0;
                    var unitRefno = 0;
                    var recordCount = 0;
                    var isProdUnitBarCodeFound = false;

                    var prodUnitBarCodeRaw = string.Empty;

                    // f01
                    var prodUnitBarCodeIsBC = false;

                    // f02
                    var prodFKRefno = 0;

                    // f03
                    var unitCode = string.Empty;

                    // f04
                    var prodUnitBarCodeCode = string.Empty;

                    // f05
                    var prodUnitBarCodeSymbologyEnum = 0;

                    // f06
                    var prodUnitBarCodeModifier1 = string.Empty;

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                    while (!reader.EOF)
                    {
                        // fill one record
                        // we do this by looping until an end segment is found
                        var recordFilled = false;
                        var elementName = string.Empty;
                        prodUnitBarCodeModifier1 = string.Empty; // not always present, so we must fill it
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
                                        prodUnitBarCodeIsBC = reader.ReadContentAsBoolean();
                                        break;
                                    case "f02":
                                        prodFKRefno = reader.ReadContentAsInt();
                                        break;
                                    case "f03":
                                        unitCode = reader.Value;
                                        break;
                                    case "f04":
                                        prodUnitBarCodeCode = reader.Value;
                                        break;
                                    case "f05":
                                        prodUnitBarCodeSymbologyEnum = reader.ReadContentAsInt();
                                        break;
                                    case "f06":
                                        prodUnitBarCodeModifier1 = reader.Value;
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

                        prodUnitBarCodeRaw = string.Empty;
                        errMsg = "FK Refno " + prodFKRefno;

                        var resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                        if (resultsProduct?.Count > 0)
                        {
                            prodRefno = Convert.ToInt32(resultsProduct[0].ProductRefno);
                        }
                        else
                        {
                            throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
                        }

                        errMsg += Environment.NewLine + "Unit code " + unitCode;

                        unitLookup.TryGetValue(unitCode, out unitRefno);

                        errMsg += Environment.NewLine + "Bar code is bc " + prodUnitBarCodeIsBC;

                        var resultsProductUnit = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitRefno, Convert.ToInt32(prodUnitBarCodeIsBC));

                        if (resultsProductUnit?.Count > 0)
                        {
                            prodUnitRefno = resultsProductUnit[0].ProductUnitRefno;
                        }
                        else
                        {
                            throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                        }

                        isProdUnitBarCodeFound = false;

                        if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                        {
                            var resultsProductUnitBarcode = transaction.Query<tProductUnitBarCodeModel>("SELECT ProductUnitBarCodeCode, ProductUnitBarCodeModifier1 FROM tProductUnitBarCode WHERE ProductUnitBarCodeCode = ? AND ProductUnitBarCodeModifier1 = ?", prodUnitBarCodeCode, prodUnitBarCodeModifier1);

                            if (resultsProductUnitBarcode?.Count > 0)
                            {
                                isProdUnitBarCodeFound = true;

                                var queryUpdate = "UPDATE tProductUnitBarCode " +
                                                  "SET ProductUnitRefno = ?, " +
                                                  "ProductUnitBarCodeSymbologyEnum = ?, " +
                                                  "ProductUnitBarCodeStatus = ? " +
                                                  "WHERE ProductUnitBarCodeCode = ? " +
                                                  "AND ProductUnitBarCodeModifier1 = ?";

                                transaction.Execute(queryUpdate, prodUnitRefno, prodUnitBarCodeSymbologyEnum, Convert.ToInt32(SysCommon.ActStatus.NoChange), prodUnitBarCodeCode, prodUnitBarCodeModifier1);
                            }
                        }

                        if (!isProdUnitBarCodeFound)
                        {
                            var queryInsert = "INSERT INTO tProductUnitBarCode " +
                                              "(ProductUnitRefno, " +
                                              "ProductUnitBarCodeCode, " +
                                              "ProductUnitBarCodeRaw, " +
                                              "ProductUnitBarCodeSymbologyEnum, " +
                                              "ProductUnitBarCodeModifier1, " +
                                              "ProductUnitBarCodeStatus) " +
                                              "VALUES " +
                                              "(?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?)";

                            transaction.Execute(queryInsert, prodUnitRefno, prodUnitBarCodeCode, prodUnitBarCodeRaw, prodUnitBarCodeSymbologyEnum, prodUnitBarCodeModifier1, Convert.ToInt32(SysCommon.ActStatus.NoChange));
                        }

                        ++recordCount;
                        if (recordCount % 100 == 0)
                        {
                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, recordCount / 10 % 100, 0, 100);
                        }
                    } // end of responseData

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {recordCount}", false);

                    unitLookup = null;
                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes failed.", false);

                    throw new Exception("Database exception occurred. Saving product uom bar codes failed." + Environment.NewLine + errMsg, ex);
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
            var xml = XmlWriter.Create(xmlString);

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
            xml.Flush();
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
