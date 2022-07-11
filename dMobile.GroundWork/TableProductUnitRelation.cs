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

                List<tFamilyModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (results?.Count > 0)
                {
                    foreach (tFamilyModel family in results)
                    {
                        string recvData = await this.SendRequest(family.FamilyCode);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family.FamilyCode), true);

                        if (recvData.Length > 0)
                        {
                            if (recvData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument reqErr = new XmlDocument();
                                reqErr.LoadXml(recvData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.SaveOpt(recvData);
                        }
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

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductUnitRelation");

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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductUnitRelation"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product unit relations.", rowsAffected), false);
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
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
        }

        /// <summary>
        /// The SaveOpt.
        /// </summary>
        /// <param name="reqData"></param>
        protected async Task SaveOpt(string reqData)
        {
            string errMsg = string.Empty;
            int recordCount = 0;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    int startIndex = reqData.IndexOf("<VFPData");
                    XmlReaderSettings sett = new XmlReaderSettings();
                    sett.CheckCharacters = false;
                    sett.CloseInput = false;
                    sett.ConformanceLevel = ConformanceLevel.Auto;
                    sett.IgnoreComments = true;
                    sett.IgnoreProcessingInstructions = true;
                    sett.IgnoreWhitespace = false;
                    sett.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                    sett.ValidationType = ValidationType.None;

                    using (StringReader stringReader = new StringReader(reqData.Substring(startIndex)))
                    using (XmlReader reader = XmlReader.Create(stringReader, sett))
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations.", false);

                        Dictionary<string, int> unitLookup = new Dictionary<string, int>();
                        int lastFKRefno = 0;
                        bool refreshOnly = this.SysEnv.RefreshOpt.RefreshDataChangesOnly;
                        int prodRefno = 0;
                        int unitCode1Refno = 0;
                        int unitCode2Refno = 0;
                        bool isProdUnitRelationFound = false;
                        int prodUnit1Refno = 0;
                        int prodUnit2Refno = 0;
                        // f01
                        bool prodUnitIsBC = false;
                        // f02
                        decimal relationFactor = 0;
                        // f03
                        int prodFKRefno = 0;
                        // f04
                        string unitCode1 = string.Empty;
                        // f05
                        string unitCode2 = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        while (!reader.EOF)
                        {
                            // fill one record
                            // we do this by looping until an end segment is found
                            bool recordFilled = false;
                            string elementName = string.Empty;
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
                                List<tProductModel> resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

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
                                List<tUnitModel> resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit WHERE UnitCode = ?", unitCode1);

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

                            List<tProductUnitModel> resultsProductUnit = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitCode1Refno, prodUnitIsBC);

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
                                List<tUnitModel> resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit WHERE UnitCode = ?", unitCode2);

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

                            List<tProductUnitModel> resultsProductUnit2 = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitCode2Refno, prodUnitIsBC);

                            if (resultsProductUnit?.Count > 0)
                            {
                                prodUnit2Refno = resultsProductUnit2[0].ProductUnitRefno;
                            }
                            else
                            {
                                throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                            }

                            isProdUnitRelationFound = false;

                            List<tProductUnitRelationModel> resultsProductUnitRelation = transaction.Query<tProductUnitRelationModel>("SELECT * FROM tProductUnitRelation WHERE ProductUnit1Refno = ? AND ProductUnit2Refno = ?", prodUnit1Refno, prodUnit2Refno);

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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);

                        unitLookup = null;
                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom relations complete.", false);
                    }
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
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT UNIT RELATIONS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<family_code>" + familyCode + "</family_code>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
