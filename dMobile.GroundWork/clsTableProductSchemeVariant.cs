namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

    public class TableProductSchemeVariant : TableBase
    {
        #region Constructors

        public TableProductSchemeVariant()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductSchemeVariant";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Request()
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants.", true);

                reqData = await this.Send("product scheme variants");

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding product scheme variants failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeVariantXRef");
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductSchemeVariant");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product scheme variants failed.", ex);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="schemeVarNo"></param>
        ///// <returns></returns>
        //public int GetProdSchemeVarRefnoFromProdSchemeVarNo(int schemeVarNo)
        //{
        //    int schemeVarRefno = 0;

        //      try
        //      {
        //        using (SQLiteCommand cmndSchemeVar = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //        {
        //          cmndSchemeVar.CommandText = "tProductSchemeVariant";
        //          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
        //          cmndSchemeVar.CommandType = CommandType.TableDirect;

        //          using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
        //          {
        //            if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVarNo }))
        //            {
        //              if (drSchemeVar.Read())
        //              {
        //                schemeVarRefno = Convert.ToInt32(drSchemeVar.GetValue((int)SysCommon.ProdSchemeVariantFields.Refno));
        //              }
        //              else
        //              {
        //#if MULTILANGUAGE
        //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemevarreadFailed));
        //#else
        //                throw new Exception("drSchemeVar.Read() failed.");
        //#endif
        //              }
        //            }
        //            else
        //            {
        //              throw new Exception(
        //                  string.Format(
        //#if MULTILANGUAGE
        //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductSchemeVariantNumberInTproductschemevariant), 
        //#else
        //                  "Unable To Locate Product Scheme Variant Number {0} In tProductSchemeVariant", 
        //#endif
        //                  schemeVarNo));
        //            }

        //            drSchemeVar.Close();
        //          }
        //        }
        //      }
        //      catch (SQLiteException ex)
        //      {
        //#if MULTILANGUAGE
        //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodschemevarrefnofromprodschemevarnoFailed), ex);
        //#else
        //        throw new Exception("Database exception occurred. GetProdSchemeVarRefnoFromProdSchemeVarNo failed.", ex);
        //#endif
        //      }
        //    return schemeVarRefno;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeVariant"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product scheme variants.", rowsAffected), false);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product scheme variants status failed.", ex);
            }

            return rowsAffected;
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override async Task Save(string reqData)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {

                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeVariant"].Rows.Count);

                            // TODO: Resource String
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int progValue = 0;
                            bool isSchemeVarFound = false;
                            string schemeVarCode = string.Empty;
                            string schemeVarDesc = string.Empty;
                            int schemeVarNo = 0;
                            bool schemeVarIsDefault = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tProductSchemeVariant"].Rows)
                            {
                                schemeVarCode = drow["ProductSchemeVariantCode"].ToString();
                                errMsg = "Variant code \'" + schemeVarCode + "\'";
                                schemeVarDesc = drow["ProductSchemeVariantDescription"].ToString();
                                errMsg += Environment.NewLine + "Variant desc \'" + schemeVarDesc + "\'";
                                schemeVarNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                                errMsg += Environment.NewLine + "Variant no " + schemeVarNo;
                                schemeVarIsDefault = Convert.ToBoolean(drow["ProductSchemeVariantIsDefault"]);
                                errMsg += Environment.NewLine + "Variant is default " + schemeVarIsDefault;
                                isSchemeVarFound = false;

                                List<tProductSchemeVariantModel> resultsProductSchemeVariant = transaction.Query<tProductSchemeVariantModel>("SELECT ProductSchemeVariantNo FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", schemeVarNo);

                                if (resultsProductSchemeVariant?.Count > 0)
                                {
                                    isSchemeVarFound = true;

                                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                    {
                                        string queryUpdate = "UPDATE tProductSchemeVariant "
                                            + "SET ProductSchemeVariantCode = \'" + schemeVarCode + "\', "
                                            + "ProductSchemeVariantDescription = \'" + schemeVarDesc + "\', "
                                            + "ProductSchemeVariantIsDefault = " + Convert.ToString(schemeVarIsDefault ? 1 : 0)
                                            + " WHERE ProductSchemeVariantNo = " + schemeVarNo;

                                        transaction.Execute(queryUpdate);
                                    }
                                }

                                if (!isSchemeVarFound)
                                {
                                    string queryInsert = "INSERT INTO tProductSchemeVariant (ProductSchemeVariantCode, ProductSchemeVariantDescription, ProductSchemeVariantNo, ProductSchemeVariantIsDefault) VALUES (?, ?, ?, ?)";

                                    transaction.Execute(queryInsert, schemeVarCode, schemeVarDesc, schemeVarNo, schemeVarIsDefault);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            }
                        }

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving product scheme variants failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT SCHEME VARIANTS</action>"
                + SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        #endregion
    }
}
