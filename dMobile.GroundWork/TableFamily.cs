// <copyright file="TableFamily.cs" company="InterSect Business Systems Inc">
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

    public class TableFamily : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableFamily"/> class.
        /// </summary>
        public TableFamily()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableFamily";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task RequestHeaderData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families.", true);

                var responseData = await this.SendRequest();
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families failed.", true);

                throw new Exception("Seeding families failed.", ex);
            }
        }

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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tFamily"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} families.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Families status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tFamily");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families failed.", true);

                throw new Exception("Database exception occurred. Purging families failed.", ex);
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="familyCode"></param>
        ///// <returns></returns>
        // public int GetiFamilyRefnoFromFamilyCode(string familyCode)
        // {
        //    int familyRefno = 0;

        // //      try
        //    //      {
        //    //        using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //    //        {
        //    //          cmndFamily.CommandText = "tFamily";
        //    //          cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
        //    //          cmndFamily.CommandType = CommandType.TableDirect;

        // //          using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
        //    //          {
        //    //            if (drFamily.Seek(DbSeekOptions.FirstEqual, new object[] { familyCode }))
        //    //            {
        //    //              if (drFamily.Read())
        //    //              {
        //    //                familyRefno = Convert.ToInt32(drFamily.GetValue((int)SysCommon.FamilyFields.Refno));
        //    //              }
        //    //              else
        //    //              {
        //    //#if MULTILANGUAGE
        //    //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrfamilyreadFailed));
        //    //#else
        //    //                throw new Exception("drFamily.Read() failed.");
        //    //#endif
        //    //              }
        //    //            }
        //    //            else
        //    //            {
        //    //              throw new Exception(
        //    //                  string.Format(
        //    //#if MULTILANGUAGE
        //    //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheFamilyRefnoFromFamilyCodeInTfamily),
        //    //#else
        //    //                  "Unable to locate the family refno from family code {0} in tFamily.",
        //    //#endif
        //    //                  familyCode));
        //    //            }

        // //            drFamily.Close();
        //    //          }
        //    //        }
        //    //      }
        //    //      catch (SQLiteException ex)
        //    //      {
        //    //#if MULTILANGUAGE
        //    //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetifamilyrefnofromfamilycodeFailed), ex);
        //    //#else
        //    //        throw new Exception("Database exception occurred. GetiFamilyRefnoFromFamilyCode failed.", ex);
        //    //#endif
        //    //      }

        // return familyRefno;
        // }

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task Save(string responseData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = ds.Tables["tFamily"].Rows.Count;

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var progValue = 0;
                        var isFamilyFound = false;
                        var familyCode = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tFamily"].Rows)
                        {
                            familyCode = drow["FamilyCode"].ToString();
                            errMsg = "Family \'" + familyCode + "\'";
                            isFamilyFound = false;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                var results = transaction.Query<tFamilyModel>("SELECT FamilyCode FROM tFamily WHERE FamilyCode = ?", familyCode);

                                if (results?.Count > 0)
                                {
                                    isFamilyFound = true;
                                }
                            }

                            if (!isFamilyFound)
                            {
                                transaction.Execute("INSERT INTO tFamily (FamilyCode) VALUES (?)", familyCode);
                            }

                            progValue++;
                            if (progValue % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families failed.", false);

                    throw new Exception("Database exception occurred. Saving families failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST FAMILIES</action>"
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
