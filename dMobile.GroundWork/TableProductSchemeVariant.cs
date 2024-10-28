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
    using Bedrock;
    using Resources.Models.Products;
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
            this.LowerCaseName = "scheme variants";
            this.ProperCaseName = "Scheme variants";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var responseData = await this.SendRequest();
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{this.LowerCaseName} seeded.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);
                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductSchemeVariantXRef");
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductSchemeVariant");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);

                throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
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
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var variants = new List<ProductSchemeVariantModel>();
                    foreach (DataRow row in rows)
                    {
                        var variant = new ProductSchemeVariantModel
                        {
                            ProductSchemeVariantCode = row["ProductSchemeVariantCode"].ToString(),
                            ProductSchemeVariantDescription = row["ProductSchemeVariantDescription"].ToString(),
                            ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"]),
                            ProductSchemeVariantIsDefault = Convert.ToBoolean(row["ProductSchemeVariantIsDefault"]),
                        };
                        variants.Add(variant);
                    }

                    await conn.InsertAllAsync(variants);

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
                        $"{variants.Count} {this.LowerCaseName}.", true);
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
                + "<action>REQUEST PRODUCT SCHEME VARIANTS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
