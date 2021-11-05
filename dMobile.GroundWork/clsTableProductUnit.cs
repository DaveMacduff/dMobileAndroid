namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SQLite;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;

    /// <summary>
    /// Defines the <see cref="TableProductUnit" />
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
        /// The Request
        /// </summary>
        public override async Task Request()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom.", true);

                string reqData = string.Empty;
                List<tFamilyModel> resultsFamily = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tFamilyModel>("SELECT * FROM tFamily");

                if (resultsFamily?.Count > 0)
                {
                    foreach (tFamilyModel family in resultsFamily)
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
                }

                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom failed.", true);

                // TODO: Resource Strings
                throw new Exception("Database exception occurred. Seeding product uom failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom failed.", true);

                // TODO: Resource Strings
                throw new Exception("Seeding product uom failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductUnit");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom failed.", true);

                // TODO: Resource strings
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
        //public int GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC(
        //    int prodRefno,
        //    int unitRefno,
        //    bool prodUnitIsBC)
        //{
        //    int prodUnitRefno = 0;

        //            try
        //            {
        //                using (SQLiteCommand cmndGetProductUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //                {
        //                    cmndGetProductUnit.CommandText = "tProductUnit";
        //                    cmndGetProductUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
        //                    cmndGetProductUnit.CommandType = CommandType.TableDirect;

        //                    using (SQLiteDataReader drProductUnit = cmndGetProductUnit.ExecuteReader())
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
        //#if MULTILANGUAGE
        //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrproductunitreadFailed));
        //#else
        //                                throw new Exception("drProductUnit.Read() failed.");
        //#endif
        //                            }
        //                        }
        //                        else
        //                        {
        //                            throw new Exception(
        //                                string.Format(
        //#if MULTILANGUAGE
        //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductUnitRefnoFromProductRefnoAndUnitRefnoAndIsBc) +
        //                  Translator.GetInstance().Translate(TranslatorVocabulary.InTproductunit),
        //#else
        //"Unable to locate product unit refno from product refno and unit refno and is BC {0}, {1}, {2} " +
        //                                "in tProductUnit",
        //#endif
        // prodRefno,
        //                                prodUnitRefno,
        //                                prodUnitIsBC));
        //                        }

        //                        drProductUnit.Close();
        //                    }
        //                }
        //            }
        //            catch (SQLiteException ex)
        //            {
        //                throw new Exception(
        //#if MULTILANGUAGE
        //            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodunitrefnofromprodrefnoandunitrefnoandprodunitisbcFailed), ex);
        //#else
        //"Database exception occurred. GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC failed.", ex);
        //#endif
        //            }

        //    return prodUnitRefno;
        //}

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductUnit"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product units.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product uom status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(string familyCode)
        {
            return await this.SendComm(await this.CreateRequest(familyCode));
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
            StringBuilder errMsg = new StringBuilder();
            int recordCount = 0;
            Dictionary<string, int> unitLookup = new Dictionary<string, int>();

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    int startIndex = reqData.IndexOf("<VFPData");

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    using (StringReader stringReader = new StringReader(reqData.Substring(startIndex)))
                    using (XmlReader reader = XmlReader.Create(stringReader))
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom.", false);

                        List<tUnitModel> resultsUnit = transaction.Query<tUnitModel>("SELECT * FROM tUnit");

                        if (resultsUnit?.Count > 0)
                        {
                            foreach (tUnitModel unit in resultsUnit)
                            {
                                unitLookup.Add(unit.UnitCode, unit.UnitRefno);
                            }
                        }

                        int prodRefno = 0;
                        int unitRefno = 0;
                        int baseUnitRefno = 0;
                        int wtUnitRefno = 0;
                        bool isProdUnitFound = false;
                        string prodCode = string.Empty;

                        // f01
                        bool isProdUnitBC = false;
                        // f02
                        decimal prodUnitWeight = 0;
                        // f03
                        string description = string.Empty;
                        // f04
                        int prodFKRefno = 0;
                        // f05
                        string unitCode = string.Empty;
                        // f06
                        string baseUnitCode = string.Empty;
                        // f07
                        string wtUnitCode = string.Empty;
                        // f08
                        bool isProdUnitSellUnit = false;
                        // f09
                        bool isProdUnitDefault = false;
                        // f10
                        decimal boxCount = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        while (!reader.EOF)
                        {
                            bool recordFilled = false;
                            string elementName = string.Empty;

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

                            List<tProductModel> resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                            if (resultsProduct?.Count > 0)
                            {
                                prodRefno = resultsProduct[0].ProductRefno;
                                prodCode = resultsProduct[0].ProductCode;
                            }
                            else
                            {
                                // TODO: Resource Strings
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

                            List<tProductUnitModel> resultsUnitProductUnit = transaction.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", prodRefno, unitRefno, isProdUnitBC);

                            if (resultsUnitProductUnit?.Count > 0)
                            {
                                isProdUnitFound = true;

                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    string queryUpdate = @"UPDATE tProductUnit
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
                                    object[] parametersUpdate = new object[]
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
                                        Convert.ToInt32(isProdUnitBC)
                                    };

                                    transaction.Execute(queryUpdate, parametersUpdate);
                                }
                            }

                            if (!isProdUnitFound)
                            {
                                string queryInsert = "INSERT INTO tProductUnit " +
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
                                    "ProductUnitBoxCount) " +
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
                                object[] parametersInsert = new object[]
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
                                    Convert.ToDecimal(boxCount)
                                };

                                transaction.Execute(queryInsert, parametersInsert);
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

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom failed.", false);

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving product uom failed." + Environment.NewLine + errMsg.ToString(), ex);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, e.Message, true);
                    throw;
                }
            });
        }

        /// <summary>
        /// The CreateRequest
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
                + "<action>REQUEST PRODUCT UNITS</action>"
                + await SysCommon.GetRequestDefaults(true)
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
