namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

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
        public override void Request()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product uom.", true);

                using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT * FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    //cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                    using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        string reqData = string.Empty;
                        string family = string.Empty;

                        while (drFamily.Read())
                        {
                            // TODO: Uncomment GC calls
                            //GC.Collect();
                            //GC.WaitForPendingFinalizers();

                            family = drFamily["FamilyCode"].ToString();

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family), true);

                            reqData = this.SendRequest(family);
                            if (0 < reqData.Length)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                //this.Save(reqData);
                                this.SaveOpt(reqData);
                            }
                        }

                        drFamily.Close();
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
        public override void Purge()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUom), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductUnit";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductUomFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product uom failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductUomFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging product uom failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC
        /// </summary>
        /// <param name="prodRefno"></param>
        /// <param name="unitRefno"></param>
        /// <param name="prodUnitIsBC"></param>
        /// <returns></returns>
        public int GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC(
            int prodRefno,
            int unitRefno,
            bool prodUnitIsBC)
        {
            int prodUnitRefno = 0;

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

            return prodUnitRefno;
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductUnit";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductUnits), rowsAffected),
#else
 string.Format("{0} product units.", rowsAffected),
#endif
 true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductUomStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Product uom status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override string SendRequest(string familyCode)
        {
            return this.SendComm(this.CreateRequest(familyCode));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
        }

        /// <summary>
        /// The SaveOpt
        /// </summary>
        /// <param name="reqData"></param>
        protected void SaveOpt(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            StringBuilder errMsg = new StringBuilder();
            int recordCount = 0;

            Dictionary<string, int> unitLookup = new Dictionary<string, int>();

            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnitUpdate = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
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

                        //cmndProdUnit.CommandText = "tProductUnit";
                        //cmndProdUnit.CommandType = CommandType.TableDirect;
                        //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                        //cmndProd.CommandText = "SELECT * FROM tProduct";
                        //cmndProd.CommandType = CommandType.Text;
                        //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";

                        cmndUnit.CommandText = "SELECT * FROM tUnit";
                        cmndUnit.CommandType = CommandType.Text;
                        //cmndUnit.IndexName = "UQ_UnitOnUnitCode";

                        //using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                        //using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                        using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                        {
                            while (drUnit.Read())
                            {
                                unitLookup.Add(drUnit.GetString((int)SysCommon.UnitFields.UnitCode), Convert.ToInt32(drUnit.GetValue(Convert.ToInt32(SysCommon.UnitFields.UnitRefno))));
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

                            cmndProd.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                            cmndProd.CommandType = CommandType.Text;
                            //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                            cmndProd.Parameters.Add(new SQLiteParameter("@productFKRefno", prodFKRefno));

                            using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                            {
                                //if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                //{
                                if (drProd.Read())
                                {
                                    prodRefno = Convert.ToInt32(drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                    prodCode = Convert.ToString(drProd.GetValue((int)SysCommon.ProductFields.Code));
                                }
                                else
                                {
                                    // TODO: Resource Strings
                                    throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg.ToString());
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource Strings
                                //    throw new Exception("Unable to locate the product refno using the product FK refno in tProduct." + Environment.NewLine + errMsg.ToString());
                                //}
                            }

                            errMsg.Append(Environment.NewLine).Append("Refno: \'").Append(prodRefno).Append("\'");
                            errMsg.Append(Environment.NewLine).Append("Code: \'").Append(prodCode).Append("\'");
                            errMsg.Append(Environment.NewLine).Append("Unit: \'").Append(unitCode).Append("\'");

                            unitLookup.TryGetValue(unitCode, out unitRefno);

                            //if (!unitLookup.TryGetValue(unitCode, out unitRefno))
                            //{
                            //    cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                            //    cmndUnit.CommandType = CommandType.Text;
                            //    //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                            //    cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", unitCode));

                            //    using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                            //    {
                            //        //if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                            //        //{
                            //        if (drUnit.Read())
                            //        {
                            //            unitRefno = Convert.ToInt32(drUnit.GetValue(Convert.ToInt32(SysCommon.UnitFields.UnitRefno)));
                            //            unitLookup.Add(unitCode, unitRefno);
                            //        }
                            //        else
                            //        {
                            //            // TODO: Resource Strings
                            //            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg.ToString());
                            //        }
                            //        //}
                            //        //else
                            //        //{
                            //        //    // TODO: Resource Strings
                            //        //    throw new Exception("Unable to locate the unit refno using the unit code in tUnit." + Environment.NewLine + errMsg.ToString());
                            //        //}

                            //        drUnit.Close();
                            //    }
                            //}

                            errMsg.Append(Environment.NewLine).Append("Base unit: \'").Append(baseUnitCode).Append("\'");

                            unitLookup.TryGetValue(baseUnitCode, out baseUnitRefno);

                            //if (!unitLookup.TryGetValue(baseUnitCode, out baseUnitRefno))
                            //{
                            //    cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                            //    cmndUnit.CommandType = CommandType.Text;
                            //    //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                            //    cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", baseUnitCode));

                            //    using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                            //    {
                            //        //if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { baseUnitCode }))
                            //        //{
                            //        if (drUnit.Read())
                            //        {
                            //            baseUnitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
                            //            unitLookup.Add(baseUnitCode, baseUnitRefno);
                            //        }
                            //        else
                            //        {
                            //            // TODO: Resource Strings
                            //            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg.ToString());
                            //        }
                            //        //}
                            //        //else
                            //        //{
                            //        //    throw new Exception("Unable to locate the unit refno using the base unit code in tUnit." + Environment.NewLine + errMsg.ToString());
                            //        //}

                            //        drUnit.Close();
                            //    }
                            //}

                            errMsg.Append(Environment.NewLine).Append("Weight unit: \'").Append(wtUnitCode).Append("\'");

                            unitLookup.TryGetValue(unitCode, out wtUnitRefno);
                            //if (!unitLookup.TryGetValue(wtUnitCode, out wtUnitRefno))
                            //{
                            //    cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                            //    cmndUnit.CommandType = CommandType.Text;
                            //    //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                            //    cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", wtUnitCode));

                            //    using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                            //    {
                            //        //if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { wtUnitCode }))
                            //        //{
                            //        if (drUnit.Read())
                            //        {
                            //            wtUnitRefno = Convert.ToInt32(drUnit.GetValue(Convert.ToInt32(SysCommon.UnitFields.UnitRefno)));
                            //            unitLookup.Add(wtUnitCode, wtUnitRefno);
                            //        }
                            //        else
                            //        {
                            //            // TODO: Resource Strings
                            //            throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg.ToString());
                            //        }
                            //        //}
                            //        //else
                            //        //{
                            //        //    // TODO: Resource Strings
                            //        //    throw new Exception("Unable to locate the unit refno using the weight unit code in tUnit." + Environment.NewLine + errMsg.ToString());
                            //        //}

                            //        drUnit.Close();
                            //    }
                            //}

                            isProdUnitFound = false;

                            cmndProdUnit.CommandText = "SELECT * FROM tProductUnit WHERE ProductRefno = @productRefno AND UnitRefno = @unitRefno AND ProductUnitIsBC = @productUnitIsBC";
                            cmndProdUnit.CommandType = CommandType.Text;
                            //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", prodRefno));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", unitRefno));
                            cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", isProdUnitBC));

                            using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            {
                                //if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitRefno, isProdUnitBC }))
                                if (drProdUnit.Read())
                                {
                                    isProdUnitFound = true;

                                    // TODO: Uncomment SysEnv.RefreshOpt.RefreshDataChangesOnly
                                    //if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                    //{
                                    cmndProdUnitUpdate.CommandText = @"UPDATE tProductUnit
                                            SET BaseUnitRefno =  @baseUnitRefno,
                                            WtUnitRefno = @wtUnitRefno,
                                            ProductUnitWeight = @prodUnitWeight,
                                            ProductUnitSizeDescription = @desc,
                                            ProductUnitIsSellUnit = @isSellUnit,
                                            ProductUnitIsDefault = @isDefault,
                                            ProductUnitBoxCount = @boxCount
                                             WHERE ProductRefno = @prodRefno
                                             AND UnitRefno = @unitRefno
                                             AND ProductUnitIsBC = @isBC";

                                    cmndProdUnitUpdate.CommandType = CommandType.Text;
                                    //cmndProdUnit.IndexName = string.Empty;
                                    //cmndProdUnit.Parameters.Clear();

                                    //SqlCeParameter baseRefnoParam = new SqlCeParameter("@baseUnitRefno", SqlDbType.Int);
                                    //SqlCeParameter wtRefnoParam = new SqlCeParameter("@wtUnitRefno", SqlDbType.Int);
                                    //SqlCeParameter prodWeightParam = new SqlCeParameter("@prodUnitWeight", SqlDbType.Decimal);
                                    //SqlCeParameter descParam = new SqlCeParameter("@desc", SqlDbType.NVarChar, 15);
                                    //SqlCeParameter sellUnitParam = new SqlCeParameter("@isSellUnit", SqlDbType.Bit);
                                    //SqlCeParameter defaultParam = new SqlCeParameter("@isDefault", SqlDbType.Bit);
                                    //SqlCeParameter boxCountParam = new SqlCeParameter("@boxCount", SqlDbType.Decimal);
                                    //SqlCeParameter prodRefnoParam = new SqlCeParameter("@prodRefno", SqlDbType.Int);
                                    //SqlCeParameter unitRefnoParam = new SqlCeParameter("@unitRefno", SqlDbType.Int);
                                    //SqlCeParameter brokenCaseParam = new SqlCeParameter("@isBC", SqlDbType.Bit);

                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@baseUnitRefno", baseUnitRefno));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@wtUnitRefno", wtUnitRefno));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@prodUnitWeight", prodUnitWeight));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@desc", description));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@isSellUnit", isProdUnitSellUnit));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@isDefault", isProdUnitDefault));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@boxCount", boxCount));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@prodRefno", prodRefno));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@unitRefno", unitRefno));
                                    cmndProdUnitUpdate.Parameters.Add(new SQLiteParameter("@isBC", isProdUnitBC));

                                    //cmndProdUnitUpdate.Prepare();

                                    //baseRefnoParam.Value = baseUnitRefno;
                                    //wtRefnoParam.Value = wtUnitRefno;
                                    //prodWeightParam.Value = prodUnitWeight;
                                    //descParam.Value = description;
                                    //sellUnitParam.Value = isProdUnitSellUnit;
                                    //defaultParam.Value = isProdUnitDefault;
                                    //boxCountParam.Value = boxCount;
                                    //prodRefnoParam.Value = prodRefno;
                                    //unitRefnoParam.Value = unitRefno;
                                    //brokenCaseParam.Value = isProdUnitBC;

                                    cmndProdUnitUpdate.ExecuteNonQuery();
                                    //}
                                }

                                drProdUnit.Close();
                            }

                            if (!isProdUnitFound)
                            {
                                cmndProdUnit.CommandText = "INSERT INTO tProductUnit " +
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
                                    "(@productRefno, " +
                                    "@unitRefno, " +
                                    "@baseUnitRefno, " +
                                    "@wtUnitRefno, " +
                                    "@productUnitIsBC, " +
                                    "@productUnitBC, " +
                                    "@productUnitWeight, " +
                                    "@productUnitSizeDescription, " +
                                    "@productUnitIsSellUnit, " +
                                    "@productUnitIsDefault, " +
                                    "@productUnitBoxCount)";

                                cmndProdUnit.CommandType = CommandType.Text;
                                //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                                cmndProdUnit.Parameters.Clear();
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", prodRefno));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", unitRefno));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@baseUnitRefno", baseUnitRefno));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@wtUnitRefno", wtUnitRefno));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", isProdUnitBC));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitBC", isProdUnitBC ? "X" : " "));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitWeight", prodUnitWeight));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitSizeDescription", description));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsSellUnit", isProdUnitSellUnit));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsDefault", isProdUnitDefault));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitBoxCount", boxCount));

                                cmndProdUnit.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ProdUnitFields.ProdRefno, prodRefno);
                                //rec.SetInt32((int)SysCommon.ProdUnitFields.UnitRefno, unitRefno);
                                //rec.SetInt32((int)SysCommon.ProdUnitFields.BaseUnitRefno, baseUnitRefno);
                                //rec.SetInt32((int)SysCommon.ProdUnitFields.WtUnitRefno, wtUnitRefno);
                                //rec.SetBoolean((int)SysCommon.ProdUnitFields.IsBC, isProdUnitBC);
                                //rec.SetString((int)SysCommon.ProdUnitFields.BC, isProdUnitBC ? "X" : " ");
                                //rec.SetDecimal((int)SysCommon.ProdUnitFields.Weight, prodUnitWeight);
                                //rec.SetString((int)SysCommon.ProdUnitFields.SizeDescription, description);
                                //rec.SetBoolean((int)SysCommon.ProdUnitFields.IsSellUnit, isProdUnitSellUnit);
                                //rec.SetBoolean((int)SysCommon.ProdUnitFields.IsDefault, isProdUnitDefault);
                                //rec.SetDecimal((int)SysCommon.ProdUnitFields.BoxCount, boxCount);

                                //rs.Insert(rec);
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

                        //drUnit.Close();
                        //drProd.Close();
                        //drProdUnit.Close();
                        //}

                        transaction.Commit();
                    }

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product uom failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource Strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg.ToString(), ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving product uom failed." + Environment.NewLine + errMsg.ToString(), ex);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, e.Message, true);
                    throw;
                }
            }
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
                + "<action>REQUEST PRODUCT UNITS</action>"
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
