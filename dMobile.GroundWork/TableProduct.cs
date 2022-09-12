// <copyright file="TableProduct.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableProduct" />.
    /// </summary>
    public class TableProduct : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProduct"/> class.
        /// </summary>
        public TableProduct() : base()
        {
            this.FullClassName = this.GetType().FullName;
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products.", true);

                var resultsFamily = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (resultsFamily?.Count > 0)
                {
                    foreach (var family in resultsFamily)
                    {
                        var reqData = string.Empty;

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family.FamilyCode), true);

                        reqData = await this.SendRequest(family.FamilyCode);

                        if (reqData.Length > 0)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                var reqErr = new XmlDocument();
                                reqErr.LoadXml(reqData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.SaveOpt(reqData);
                        }
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products failed.", true);

                throw new Exception("Database exception occurred. Seeding products failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding products failed.", true);

                throw new Exception("Seeding products failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProduct");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging products failed.", true);

                throw new Exception("Database exception occurred. Purging products failed.", ex);
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
                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(prodFKRefno));

                if (results?.Count > 0)
                {
                    prodRefno = results[0].ProductRefno;
                }
                else
                {
                    throw new Exception(string.Format("Unable to locate product Refno from product FK Refno {0} in tProduct", prodFKRefno));
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. GetProdRefnoFromProdFKRefno failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProduct"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} products.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Products status failed.", ex);
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
        /// <param name="data"></param>
        protected override async Task Save(string data)
        {
        }

        /// <summary>
        /// The SaveOpt.
        /// </summary>
        /// <param name="data"></param>
        protected async Task SaveOpt(string data)
        {
            var processedCount = 0;
            var errMsg = string.Empty;
            var refreshDataChangesOnly = this.SysEnv.RefreshOpt.RefreshDataChangesOnly;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    var dictionaryFamily = new Dictionary<string, int>();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    var startIndex = data.IndexOf("<VFPData");
                    var sett = new XmlReaderSettings();
                    sett.CheckCharacters = false;
                    sett.CloseInput = false;
                    sett.ConformanceLevel = ConformanceLevel.Auto;
                    sett.IgnoreComments = true;
                    sett.IgnoreProcessingInstructions = true;
                    sett.IgnoreWhitespace = false;
                    sett.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                    sett.ValidationType = ValidationType.None;

                    using (var stringReader = new StringReader(data.Substring(startIndex)))
                    using (var reader = XmlReader.Create(stringReader, sett))
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products.", false);

                        var resultsFamily = transaction.Query<tFamilyModel>("SELECT * FROM tFamily");

                        if (resultsFamily?.Count > 0)
                        {
                            foreach (var family in resultsFamily)
                            {
                                dictionaryFamily.Add(family.FamilyCode, family.FamilyRefno);
                            }
                        }

                        var isProdFound = false;
                        var progValue = 0;
                        var familyRefno = 0;
                        var schemeRefno = 0;

                        // f01
                        var prodCode = string.Empty;

                        // f02
                        var prodFKRefno = 0;

                        // f03
                        var description = string.Empty;

                        // f04
                        var isCatchCount = false;

                        // f05
                        var isCatchWeight = false;

                        // f06
                        var isCatchAsk = false;

                        // f07
                        var schemeCode = string.Empty;

                        // f08
                        var familyCode = string.Empty;

                        // f09
                        var brandDescription = string.Empty;

                        // f10
                        var isCatchAskZero = false;

                        // f11
                        var isAllowConvertProduct = false;

                        // SqlCeUpdatableRecord rec = rs.CreateRecord();
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
                                            prodCode = reader.Value;
                                            break;
                                        case "f02":
                                            prodFKRefno = reader.ReadContentAsInt();
                                            break;
                                        case "f03":
                                            description = reader.Value;
                                            break;
                                        case "f04":
                                            isCatchCount = reader.ReadContentAsBoolean();
                                            break;
                                        case "f05":
                                            isCatchWeight = reader.ReadContentAsBoolean();
                                            break;
                                        case "f06":
                                            isCatchAsk = reader.ReadContentAsBoolean();
                                            break;
                                        case "f07":
                                            schemeCode = reader.Value;
                                            break;
                                        case "f08":
                                            familyCode = reader.Value;
                                            break;
                                        case "f09":
                                            brandDescription = reader.Value;
                                            break;
                                        case "f10":
                                            isCatchAskZero = reader.ReadContentAsBoolean();
                                            break;
                                        case "f11":
                                            isAllowConvertProduct = reader.ReadContentAsBoolean();
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

                            dictionaryFamily.TryGetValue(familyCode, out familyRefno);
                            isProdFound = false;

                            var resultsProduct = transaction.Query<tProductModel>("SELECT ProductFKRefno FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                            if (resultsProduct?.Count > 0)
                            {
                                isProdFound = true;

                                if (refreshDataChangesOnly)
                                {
                                    var queryUpdate = "UPDATE tProduct" +
                                                      " SET FamilyRefno = ?," +
                                                      " ProductCode = ?," +
                                                      " ProductDescription = ?," +
                                                      " ProductBrandDescription = ?," +
                                                      " ProductIsCatchCount = ?," +
                                                      " ProductIsCatchWeight = ?," +
                                                      " ProductIsCatchAsk = ?," +
                                                      " ProductIsCatchAskZero = ?," +
                                                      " ProductAllowConvertProduct = ?" +
                                                      " WHERE ProductFKRefno = ?";
                                    var parametersUpdate = new object[]
                                    {
                                        familyRefno,
                                        prodCode,
                                        description,
                                        brandDescription,
                                        isCatchCount,
                                        isCatchWeight,
                                        isCatchAsk,
                                        isCatchAskZero,
                                        isAllowConvertProduct,
                                        prodFKRefno,
                                    };

                                    transaction.Execute(queryUpdate, parametersUpdate);
                                }
                            }

                            if (!isProdFound)
                            {
                                var queryInsert = "INSERT INTO tProduct " +
                                                  "(ProductSchemeRefno, " +
                                                  "FamilyRefno, " +
                                                  "ProductCode, " +
                                                  "ProductFKRefno, " +
                                                  "ProductDescription, " +
                                                  "ProductIsCatchCount, " +
                                                  "ProductIsCatchWeight, " +
                                                  "ProductIsCatchAsk, " +
                                                  "ProductIsCatchAskZero, " +
                                                  "ProductBrandDescription, " +
                                                  "ProductAllowConvertProduct) " +
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
                                                  "?)";
                                var parametersInsert = new object[]
                                {
                                    schemeRefno,
                                    familyRefno,
                                    prodCode,
                                    prodFKRefno,
                                    description,
                                    isCatchCount,
                                    isCatchWeight,
                                    isCatchAsk,
                                    isCatchAskZero,
                                    brandDescription,
                                    isAllowConvertProduct,
                                };

                                transaction.Execute(queryInsert, parametersInsert);
                            }

                            ++processedCount;
                            if (processedCount % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, processedCount / 10 % 100, 0, 100);
                            }

                            progValue++;
                        } // end while loop of reading data in

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100); // complete
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", processedCount), false);
                    } // using xml stream reader

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving products failed.", false);

                    throw new Exception("Database exception occurred. Saving products failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The CreateRequest.
        /// </summary>
        /// <param name="strFamilyCode">The strFamilyCode<see cref="string"/>.</param>
        /// <returns></returns>
        protected async override Task<string> CreateRequest(string strFamilyCode)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<family_code>" + strFamilyCode + "</family_code>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
