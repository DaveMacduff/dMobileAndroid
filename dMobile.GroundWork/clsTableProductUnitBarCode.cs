namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Collections.Generic;
    using SQLite;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;

    /// <summary>
    /// Defines the <see cref="TableProductUnitBarCode" />
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
        /// The Request
        /// </summary>
        public override async Task Request()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes.", true);

                string reqData = string.Empty;
                List<tFamilyModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (results?.Count > 0)
                {
                    foreach (tFamilyModel family in results)
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family.FamilyCode), true);

                        reqData = await this.SendRequest(family.FamilyCode);

                        if (0 < reqData.Length)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument reqErr = new XmlDocument();
                                reqErr.LoadXml(reqData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.SaveOpt(reqData);
                        }
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes complete.", true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding product uom bar codes failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom bar codes failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding product uom bar codes failed.", ex);
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItemBarCodeAssociationXRef");
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductUnitBarCode");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom bar codes failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product uom bar codes failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductUnitBarCode"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product uom bar codes.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product uom bar codes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="strFamilyCode">The strFamilyCode<see cref="string"/></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(string strFamilyCode)
        {
            return await this.SendComm(this.CreateRequest(strFamilyCode));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
        }

        /// <summary>
        /// The SaveOpt
        /// </summary>
        /// <param name="reqData"></param>
        protected async Task SaveOpt(string reqData)
        {
            string errMsg = string.Empty;
            bool refreshDataChangesOnly = SysEnv.RefreshOpt.RefreshDataChangesOnly;
            Dictionary<string, int> unitLookup = new Dictionary<string, int>();

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // TODO: Resource Strings
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
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes.", false);

                        List<tUnitModel> resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit");

                        if (resultsUnit?.Count > 0)
                        {
                            foreach (tUnitModel unit in resultsUnit)
                            {
                                unitLookup.Add(unit.UnitCode, unit.UnitRefno);
                            }
                        }

                        int prodRefno = 0;
                        int prodUnitRefno = 0;
                        int unitRefno = 0;
                        int recordCount = 0;
                        bool isProdUnitBarCodeFound = false;

                        string prodUnitBarCodeRaw = string.Empty;

                        // f01
                        bool prodUnitBarCodeIsBC = false;
                        // f02
                        int prodFKRefno = 0;
                        // f03
                        string unitCode = string.Empty;
                        // f04
                        string prodUnitBarCodeCode = string.Empty;
                        // f05
                        int prodUnitBarCodeSymbologyEnum = 0;
                        // f06
                        string prodUnitBarCodeModifier1 = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        while (!reader.EOF)
                        {
                            // fill one record
                            // we do this by looping until an end segment is found
                            bool recordFilled = false;
                            string elementName = string.Empty;
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
                                continue;

                            prodUnitBarCodeRaw = string.Empty;
                            errMsg = "FK Refno " + prodFKRefno;

                            List<tProductModel> resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                            if (resultsProduct?.Count > 0)
                            {
                                prodRefno = Convert.ToInt32(resultsProduct[0].ProductRefno);
                            }
                            else
                            {
                                // TODO: Resource Strings
                                throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
                            }

                            errMsg += Environment.NewLine + "Unit code " + unitCode;

                            unitLookup.TryGetValue(unitCode, out unitRefno);

                            errMsg += Environment.NewLine + "Bar code is bc " + prodUnitBarCodeIsBC;

                            List<tProductUnitModel> resultsProductUnit = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitRefno, Convert.ToInt32(prodUnitBarCodeIsBC));

                            if (resultsProductUnit?.Count > 0)
                            {
                                prodUnitRefno = resultsProductUnit[0].ProductUnitRefno;
                            }
                            else
                            {
                                // TODO: Resource Strings
                                throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                            }

                            isProdUnitBarCodeFound = false;

                            if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                List<tProductUnitBarCodeModel> resultsProductUnitBarcode = transaction.Query<tProductUnitBarCodeModel>("SELECT ProductUnitBarCodeCode, ProductUnitBarCodeModifier1 FROM tProductUnitBarCode WHERE ProductUnitBarCodeCode = ? AND ProductUnitBarCodeModifier1 = ?", prodUnitBarCodeCode, prodUnitBarCodeModifier1);

                                if (resultsProductUnitBarcode?.Count > 0)
                                {
                                    isProdUnitBarCodeFound = true;

                                    string queryUpdate = "UPDATE tProductUnitBarCode " +
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
                                string queryInsert = "INSERT INTO tProductUnitBarCode " +
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
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                            }
                        } // end of data

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);

                        unitLookup = null;
                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom bar codes failed.", false);

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving product uom bar codes failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override string CreateRequest(string familyCode)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT UNIT BAR CODES</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<family_code>" + familyCode + "</family_code>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
