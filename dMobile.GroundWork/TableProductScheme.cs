// <copyright file="TableProductScheme.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    public class TableProductScheme : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductScheme"/> class.
        /// </summary>
        public TableProductScheme()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductScheme";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product schemes.", true);

                var reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product schemes complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product schemes failed.", true);

                throw new Exception("Seeding product schemes failed.", ex);
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="schemeCode"></param>
        ///// <returns></returns>
        // public int GetProdSchemeRefnoFromProdSchemeCode(string schemeCode)
        // {
        //  int schemeRefno = 0;

        // try
        //      {
        //        using (SQLiteCommand cmndScheme = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //        {
        //          cmndScheme.CommandText = "tProductScheme";
        //          cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
        //          cmndScheme.CommandType = CommandType.TableDirect;

        // using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
        //          {
        //            if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
        //            {
        //              if (drScheme.Read())
        //              {
        //                schemeRefno = Convert.ToInt32(drScheme.GetValue((int)SysCommon.ProdSchemeFields.Refno));
        //              }
        //              else
        //              {
        //                throw new Exception(
        // #if MULTILANGUAGE
        //                    Translator.GetInstance().Translate(TranslatorVocabulary.DrschemereadFailed) + Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.SchemeCode) + schemeCode + "\'");
        // #else
        //                    "drScheme.Read() failed." + Environment.NewLine + "Scheme code \'" + schemeCode + "\'");
        // #endif
        //              }
        //            }
        //            else
        //            {
        //              throw new Exception(
        // #if MULTILANGUAGE
        //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeCodeInTproductscheme)
        //                  + Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.SchemeCode) + schemeCode + "\'");
        // #else
        //                  "Unable to locate the product scheme code in tProductScheme."
        //                  + Environment.NewLine + "Scheme code \'" + schemeCode + "\'");
        // #endif
        //            }

        // drScheme.Close();
        //          }
        //        }
        //      }
        //      catch (SQLiteException ex)
        //      {
        // #if MULTILANGUAGE
        //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodschemerefnofromprodschemecodeFailed), ex);
        // #else
        //        throw new Exception("Database exception occurred. GetProdSchemeRefnoFromProdSchemeCode failed.", ex);
        // #endif
        //      }

        // return schemeRefno;
        // }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductScheme"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product schemes.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Product schemes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductScheme");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes failed.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes failed.", true);

                throw new Exception("Database exception occurred. Purge product schemes failed.", ex);
            }
        }

        // /// <summary>
        //    ///
        //    /// </summary>
        //    /// <param name="reqData"></param>
        //    /// <remarks></remarks>
        protected override async Task Save(string reqData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tProductScheme"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", ds.Tables["tProductScheme"].Rows.Count), false);

                            var progValue = 0;
                            var schemeCode = string.Empty;
                            var isSchemeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tProductScheme"].Rows)
                            {
                                schemeCode = drow["ProductSchemeCode"].ToString();
                                errMsg = "Scheme code \'" + schemeCode + "\'";
                                isSchemeFound = false;
                                var resultsProductScheme = transaction.Query<tProductSchemeModel>("SELECT ProductSchemeCode FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                                if (resultsProductScheme?.Count > 0)
                                {
                                    isSchemeFound = true;

                                    if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                    {
                                        var s = new StringBuilder();
                                        s.Append("UPDATE tProductScheme ");
                                        s.AppendFormat(
                                            "SET ProductSchemeIsSerialTracked = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsSerialTracked"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsProductionDateTracked = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsProductionDateTracked"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsLotTracked = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsLotTracked"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsBestBeforeDateTracked = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTracked"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsVariantTracked = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsVariantTracked"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsLotUniqueComponent = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsLotUniqueComponent"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsProductionDateUniqueComponent = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsProductionDateUniqueComponent"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsBestBeforeDateUniqueComponent = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateUniqueComponent"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsVariantUniqueComponent = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsVariantUniqueComponent"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeExtra1Type = {0},",
                                            Convert.ToInt32(drow["ProductSchemeExtra1Type"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra2Type = {0},",
                                            Convert.ToInt32(drow["ProductSchemeExtra2Type"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra3Type = {0},",
                                            Convert.ToInt32(drow["ProductSchemeExtra3Type"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra4Type = {0},",
                                            Convert.ToInt32(drow["ProductSchemeExtra4Type"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra5Type = {0},",
                                            Convert.ToInt32(drow["ProductSchemeExtra5Type"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra1Caption = \'{0}\',",
                                            Convert.ToString(drow["ProductSchemeExtra1Caption"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra2Caption = \'{0}\',",
                                            Convert.ToString(drow["ProductSchemeExtra2Caption"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra3Caption = \'{0}\',",
                                            Convert.ToString(drow["ProductSchemeExtra3Caption"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra4Caption = \'{0}\',",
                                            Convert.ToString(drow["ProductSchemeExtra4Caption"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra5Caption = \'{0}\',",
                                            Convert.ToString(drow["ProductSchemeExtra5Caption"]));
                                        s.AppendFormat(
                                            "ProductSchemeExtra1IsRequired = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeExtra1IsRequired"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeExtra2IsRequired = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeExtra2IsRequired"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeExtra3IsRequired = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeExtra3IsRequired"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeExtra4IsRequired = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeExtra4IsRequired"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeExtra5IsRequired = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeExtra5IsRequired"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsLotTrackedDefault = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsLotTrackedDefault"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsProductionDateTrackedDefault = {0},",
                                            Convert.ToBoolean(drow["ProductSchemeIsProductionDateTrackedDefault"]) ? 1 : 0);
                                        s.AppendFormat(
                                            "ProductSchemeIsBestBeforeDateTrackedDefault = {0} ",
                                            Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTrackedDefault"]) ? 1 : 0);
                                        s.AppendFormat("WHERE ProductSchemeCode = \'{0}\'", schemeCode);

                                        transaction.Execute(s.ToString());
                                    }
                                }

                                if (!isSchemeFound)
                                {
                                    var queryInsert = "INSERT INTO tProductScheme " +
                                                      "(ProductSchemeCode, " +
                                                      "ProductSchemeIsSerialTracked, " +
                                                      "ProductSchemeIsProductionDateTracked, " +
                                                      "ProductSchemeIsLotTracked, " +
                                                      "ProductSchemeIsBestBeforeDateTracked, " +
                                                      "ProductSchemeIsVariantTracked, " +
                                                      "ProductSchemeIsLotUniqueComponent, " +
                                                      "ProductSchemeIsProductionDateUniqueComponent, " +
                                                      "ProductSchemeIsBestBeforeDateUniqueComponent, " +
                                                      "ProductSchemeIsVariantUniqueComponent, " +
                                                      "ProductSchemeExtra1Type, " +
                                                      "ProductSchemeExtra2Type, " +
                                                      "ProductSchemeExtra3Type, " +
                                                      "ProductSchemeExtra4Type, " +
                                                      "ProductSchemeExtra5Type, " +
                                                      "ProductSchemeExtra1Caption, " +
                                                      "ProductSchemeExtra2Caption, " +
                                                      "ProductSchemeExtra3Caption, " +
                                                      "ProductSchemeExtra4Caption, " +
                                                      "ProductSchemeExtra5Caption, " +
                                                      "ProductSchemeExtra1IsRequired, " +
                                                      "ProductSchemeExtra2IsRequired, " +
                                                      "ProductSchemeExtra3IsRequired, " +
                                                      "ProductSchemeExtra4IsRequired, " +
                                                      "ProductSchemeExtra5IsRequired," +
                                                      "ProductSchemeIsLotTrackedDefault, " +
                                                      "ProductSchemeIsProductionDateTrackedDefault, " +
                                                      "ProductSchemeIsBestBeforeDateTrackedDefault) " +
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
                                                      "?, " +
                                                      "?, " +
                                                      "?, " +
                                                      "?, " +
                                                      "?, " +
                                                      "?, " +
                                                      "?, " +
                                                      "?, " +
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
                                        schemeCode,
                                        Convert.ToBoolean(drow["ProductSchemeIsSerialTracked"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsProductionDateTracked"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsLotTracked"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTracked"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsVariantTracked"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsLotUniqueComponent"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsProductionDateUniqueComponent"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateUniqueComponent"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsVariantUniqueComponent"]),
                                        Convert.ToInt32(drow["ProductSchemeExtra1Type"]),
                                        Convert.ToInt32(drow["ProductSchemeExtra2Type"]),
                                        Convert.ToInt32(drow["ProductSchemeExtra3Type"]),
                                        Convert.ToInt32(drow["ProductSchemeExtra4Type"]),
                                        Convert.ToInt32(drow["ProductSchemeExtra5Type"]),
                                        Convert.ToString(drow["ProductSchemeExtra1Caption"]),
                                        Convert.ToString(drow["ProductSchemeExtra2Caption"]),
                                        Convert.ToString(drow["ProductSchemeExtra3Caption"]),
                                        Convert.ToString(drow["ProductSchemeExtra4Caption"]),
                                        Convert.ToString(drow["ProductSchemeExtra5Caption"]),
                                        Convert.ToBoolean(drow["ProductSchemeExtra1IsRequired"]),
                                        Convert.ToBoolean(drow["ProductSchemeExtra2IsRequired"]),
                                        Convert.ToBoolean(drow["ProductSchemeExtra3IsRequired"]),
                                        Convert.ToBoolean(drow["ProductSchemeExtra4IsRequired"]),
                                        Convert.ToBoolean(drow["ProductSchemeExtra5IsRequired"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsLotTrackedDefault"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsProductionDateTrackedDefault"]),
                                        Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTrackedDefault"]),
                                    };

                                    transaction.Execute(queryInsert, parametersInsert);
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes failed.", false);

                    throw new Exception("Database exception occurred. Saving product schemes failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT SCHEMES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
