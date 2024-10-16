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
    using dMobile.Resources.Models.Products;
    using dMobile.Resources.Types;
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
            this.LowerCaseName = "schemes";
            this.ProperCaseName = "Schemes";
        }

        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var responseData = await this.SendRequest();
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);
                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
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
        /// <inheritdoc/>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductScheme"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} {this.LowerCaseName}.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database exception occurred. {this.ProperCaseName} status failed.", ex);
            }

            return rowsAffected;
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductScheme");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);

                throw new Exception($"Database exception occurred. Purge {this.LowerCaseName} failed.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        protected override async Task Save(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var schemes = new List<ProductSchemeModel>();
                    foreach (DataRow row in rows)
                    {
                        schemes.Add(new ProductSchemeModel
                        {
                            ProductSchemeCode = row["ProductSchemeCode"].ToString(),
                            ProductSchemeIsSerialTracked = Convert.ToBoolean(row["ProductSchemeIsSerialTracked"]),
                            ProductSchemeIsProductionDateTracked = Convert.ToBoolean(row["ProductSchemeIsProductionDateTracked"]),
                            ProductSchemeIsLotTracked = Convert.ToBoolean(row["ProductSchemeIsLotTracked"]),
                            ProductSchemeIsBestBeforeDateTracked = Convert.ToBoolean(row["ProductSchemeIsBestBeforeDateTracked"]),
                            ProductSchemeIsVariantTracked = Convert.ToBoolean(row["ProductSchemeIsVariantTracked"]),
                            ProductSchemeIsLotUniqueComponent = Convert.ToBoolean(row["ProductSchemeIsLotUniqueComponent"]),
                            ProductSchemeIsProductionDateUniqueComponent = Convert.ToBoolean(row["ProductSchemeIsProductionDateUniqueComponent"]),
                            ProductSchemeIsBestBeforeDateUniqueComponent = Convert.ToBoolean(row["ProductSchemeIsBestBeforeDateUniqueComponent"]),
                            ProductSchemeIsVariantUniqueComponent = Convert.ToBoolean(row["ProductSchemeIsVariantUniqueComponent"]),
                            ProductSchemeExtra1Type = Convert.ToInt32(row["ProductSchemeExtra1Type"]),
                            ProductSchemeExtra2Type = Convert.ToInt32(row["ProductSchemeExtra2Type"]),
                            ProductSchemeExtra3Type = Convert.ToInt32(row["ProductSchemeExtra3Type"]),
                            ProductSchemeExtra4Type = Convert.ToInt32(row["ProductSchemeExtra4Type"]),
                            ProductSchemeExtra5Type = Convert.ToInt32(row["ProductSchemeExtra5Type"]),
                            ProductSchemeExtra1Caption = row["ProductSchemeExtra1Caption"].ToString(),
                            ProductSchemeExtra2Caption = row["ProductSchemeExtra2Caption"].ToString(),
                            ProductSchemeExtra3Caption = row["ProductSchemeExtra3Caption"].ToString(),
                            ProductSchemeExtra4Caption = row["ProductSchemeExtra4Caption"].ToString(),
                            ProductSchemeExtra5Caption = row["ProductSchemeExtra5Caption"].ToString(),
                            ProductSchemeExtra1IsRequired = Convert.ToBoolean(row["ProductSchemeExtra1IsRequired"]),
                            ProductSchemeExtra2IsRequired = Convert.ToBoolean(row["ProductSchemeExtra2IsRequired"]),
                            ProductSchemeExtra3IsRequired = Convert.ToBoolean(row["ProductSchemeExtra3IsRequired"]),
                            ProductSchemeExtra4IsRequired = Convert.ToBoolean(row["ProductSchemeExtra4IsRequired"]),
                            ProductSchemeExtra5IsRequired = Convert.ToBoolean(row["ProductSchemeExtra5IsRequired"]),
                            ProductSchemeIsLotTrackedDefault = Convert.ToBoolean(row["ProductSchemeIsLotTrackedDefault"]),
                            ProductSchemeIsProductionDateTrackedDefault = Convert.ToBoolean(row["ProductSchemeIsProductionDateTrackedDefault"]),
                            ProductSchemeIsBestBeforeDateTrackedDefault = Convert.ToBoolean(row["ProductSchemeIsBestBeforeDateTrackedDefault"])
                        });
                    }

                    await conn.InsertAllAsync(schemes);
                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
                        $"{schemes.Count} {this.LowerCaseName}.", true);
                }
            }
            else
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"No {this.LowerCaseName} to save.",
                    true);
            }
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
                + "<action>REQUEST PRODUCT SCHEMES</action>"
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

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
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
