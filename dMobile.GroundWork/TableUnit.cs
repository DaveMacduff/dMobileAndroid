// <copyright file="TableUnit.cs" company="InterSect Business Systems Inc">
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

    public class TableUnit : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableUnit"/> class.
        /// </summary>
        public TableUnit()
        {
            this.FullClassName = "dMobile.GroundWork.TableUnit";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure.", true);

                var responseData = await this.SendRequest();
                await this.Save(responseData);

                // Require to have units.
                if (await this.DataStatus() <= 0)
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "No units of measure where received.", true);

                    throw new Exception("No units of measure where received.");
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure failed.", true);

                throw new Exception("Seeding units of measure failed.", ex);
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="unitCode"></param>
        ///// <returns></returns>
        // public int GetiUnitRefnoFromUnitCode(string unitCode)
        // {
        //  int unitRefno = 0;

        // try
        //      {
        //        using (SQLiteCommand cmndUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //        {
        //          cmndUnit.CommandText = "tUnit";
        //          //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
        //          cmndUnit.CommandType = CommandType.TableDirect;

        // using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
        //          {
        //            if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
        //            {
        //              if (drUnit.Read())
        //              {
        //                unitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
        //              }
        //              else
        //              {
        // #if MULTILANGUAGE
        //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed));
        // #else
        //                throw new Exception("drUnit.Read() failed.");
        // #endif
        //              }
        //            }
        //            else
        //            {
        //              throw new Exception(
        // #if MULTILANGUAGE
        //                  string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromUnitCodeInTunit), unitCode));
        // #else
        //                  string.Format("Unable to locate the unit refno from unit code {0} in tUnit", unitCode));
        // #endif
        //            }

        // drUnit.Close();
        //          }
        //        }
        //      }
        //      catch (SQLiteException ex)
        //      {
        // #if MULTILANGUAGE
        //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetiunitrefnofromunitcodeFailed), ex);
        // #else
        //        throw new Exception("Database exception occurred. GetiUnitRefnoFromUnitCode failed.", ex);
        // #endif
        //      }

        // return unitRefno;
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tUnit"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} units of measure.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Units of measure status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tUnit");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure failed.", true);

                throw new Exception("Database exception occurred. Purging units of measure failed.", ex);
            }
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = ds.Tables["tUnit"].Rows.Count;

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var progValue = 0;
                        var isUnitFound = false;
                        var unitCode = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tUnit"].Rows)
                        {
                            unitCode = drow["UnitCode"].ToString();
                            errMsg = "Unit code \'" + unitCode + "\'";
                            isUnitFound = false;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                var results = transaction.Query<tUnitModel>("SELECT UnitCode FROM tUnit WHERE UnitCode = ?", unitCode);

                                if (results?.Count > 0)
                                {
                                    isUnitFound = true;
                                }
                            }

                            if (!isUnitFound)
                            {
                                transaction.Execute("INSERT INTO tUnit (UnitCode) VALUES (?)", unitCode);
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure failed.", false);

                    throw new Exception("Database exception occurred. Saving units of measure failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST UNITS</action>"
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
