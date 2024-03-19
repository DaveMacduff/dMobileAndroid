// <copyright file="TableProductSchemeVariant.cs" company="InterSect Business Systems Inc">
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

    public class TableProductSchemeVariant : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductSchemeVariant"/> class.
        /// </summary>
        public TableProductSchemeVariant()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductSchemeVariant";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants.", true);

                var responseData = await this.SendRequest();
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants failed.", true);

                throw new Exception("Seeding product scheme variants failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductSchemeVariantXRef");
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductSchemeVariant");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants failed.", true);

                throw new Exception("Database exception occurred. Purging product scheme variants failed.", ex);
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="schemeVarNo"></param>
        ///// <returns></returns>
        // public int GetProdSchemeVarRefnoFromProdSchemeVarNo(int schemeVarNo)
        // {
        //    int schemeVarRefno = 0;

        // try
        //      {
        //        using (SQLiteCommand cmndSchemeVar = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //        {
        //          cmndSchemeVar.CommandText = "tProductSchemeVariant";
        //          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
        //          cmndSchemeVar.CommandType = CommandType.TableDirect;

        // using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
        //          {
        //            if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVarNo }))
        //            {
        //              if (drSchemeVar.Read())
        //              {
        //                schemeVarRefno = Convert.ToInt32(drSchemeVar.GetValue((int)SysCommon.ProdSchemeVariantFields.Refno));
        //              }
        //              else
        //              {
        // #if MULTILANGUAGE
        //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemevarreadFailed));
        // #else
        //                throw new Exception("drSchemeVar.Read() failed.");
        // #endif
        //              }
        //            }
        //            else
        //            {
        //              throw new Exception(
        //                  string.Format(
        // #if MULTILANGUAGE
        //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductSchemeVariantNumberInTproductschemevariant),
        // #else
        //                  "Unable To Locate Product Scheme Variant Number {0} In tProductSchemeVariant",
        // #endif
        //                  schemeVarNo));
        //            }

        // drSchemeVar.Close();
        //          }
        //        }
        //      }
        //      catch (SQLiteException ex)
        //      {
        // #if MULTILANGUAGE
        //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodschemevarrefnofromprodschemevarnoFailed), ex);
        // #else
        //        throw new Exception("Database exception occurred. GetProdSchemeVarRefnoFromProdSchemeVarNo failed.", ex);
        // #endif
        //      }
        //    return schemeVarRefno;
        // }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductSchemeVariant"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} product scheme variants.", false);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Product scheme variants status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task Save(string responseData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tProductSchemeVariant"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var progValue = 0;
                        var isSchemeVarFound = false;
                        var schemeVarCode = string.Empty;
                        var schemeVarDesc = string.Empty;
                        var schemeVarNo = 0;
                        var schemeVarIsDefault = false;

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

                            var resultsProductSchemeVariant = transaction.Query<tProductSchemeVariantModel>("SELECT ProductSchemeVariantNo FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", schemeVarNo);

                            if (resultsProductSchemeVariant?.Count > 0)
                            {
                                isSchemeVarFound = true;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    var queryUpdate = "UPDATE tProductSchemeVariant "
                                                      + "SET ProductSchemeVariantCode = \'" + schemeVarCode + "\', "
                                                      + "ProductSchemeVariantDescription = \'" + schemeVarDesc + "\', "
                                                      + "ProductSchemeVariantIsDefault = " + Convert.ToString(schemeVarIsDefault ? 1 : 0)
                                                      + " WHERE ProductSchemeVariantNo = " + schemeVarNo;

                                    transaction.Execute(queryUpdate);
                                }
                            }

                            if (!isSchemeVarFound)
                            {
                                var queryInsert = "INSERT INTO tProductSchemeVariant (ProductSchemeVariantCode, ProductSchemeVariantDescription, ProductSchemeVariantNo, ProductSchemeVariantIsDefault) VALUES (?, ?, ?, ?)";

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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);
                }
                catch (SQLiteException ex)
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);

                    throw new Exception("Database exception occurred. Saving product scheme variants failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task<string> CreateRequest()
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
                + "<action>REQUEST PRODUCT SCHEME VARIANTS</action>"
                + await SysCommon.GetRequestDefaults(true)
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

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
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
