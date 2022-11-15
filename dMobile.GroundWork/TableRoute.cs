// <copyright file="TableRoute.cs" company="InterSect Business Systems Inc">
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

    /// <summary>
    /// Defines the <see cref="TableRoute" />.
    /// </summary>
    public class TableRoute : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRoute"/> class.
        /// </summary>
        public TableRoute()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableRoute";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes.", true);

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes failed.", true);

                throw new Exception("Seeding routes failed.", ex);
            }
        }

        /// <summary>
        /// The GetiRouteRefnoFromRouteCode.
        /// </summary>
        /// <param name="routeCode"></param>
        /// <returns></returns>
        // public int GetiRouteRefnoFromRouteCode(string routeCode)
        // {
        //    int routeRefno = 0;

        // try
        //      {
        //        using (SQLiteCommand cmndRoute = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //        {
        //          cmndRoute.CommandText = "tRoute";
        //          cmndRoute.IndexName = "UQ_RouteOnRouteCode";
        //          cmndRoute.CommandType = CommandType.TableDirect;

        // using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
        //          {
        //            if (drRoute.Seek(DbSeekOptions.FirstEqual, new object[] { routeCode }))
        //            {
        //              if (drRoute.Read())
        //              {
        //                routeRefno = Convert.ToInt32(drRoute.GetValue((int)SysCommon.RouteFields.Refno));
        //              }
        //              else
        //              {
        // #if MULTILANGUAGE
        //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrroutereadFailed));
        // #else
        //                throw new Exception("drRoute.Read() failed.");
        // #endif
        //              }
        //            }
        //            else
        //            {
        //              throw new Exception(
        // #if MULTILANGUAGE
        //                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateRouteRefnoFromRouteCodeInTroute), routeCode));
        // #else
        //                string.Format("Unable To Locate Route Refno From Route Code {0} In tRoute", routeCode));
        // #endif
        //            }

        // drRoute.Close();
        //          }
        //        }
        //      }
        //      catch (SQLiteException ex)
        //      {
        // #if MULTILANGUAGE
        //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetirouterefnofromroutecodeFailed), ex);
        // #else
        //        throw new Exception("Database exception occurred. GetiRouteRefnoFromRouteCode failed.", ex);
        // #endif
        //      }

        // return routeRefno;
        // }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tRoute"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} routes.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Routes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tRoute");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes failed.", true);

                throw new Exception("Database exception occurred. Purging routes failed.", ex);
            }
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progValue = 0;
                        var isRouteFound = false;
                        var routeDesc = string.Empty;
                        var routeDayOfWeek = string.Empty;
                        var routeCode = string.Empty;
                        var progMax = Convert.ToInt32(ds.Tables["tRoute"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tRoute"].Rows)
                        {
                            routeCode = Convert.ToString(drow["RouteCode"]);
                            errMsg = "Route code \'" + routeCode + "\'";
                            routeDayOfWeek = Convert.ToString(drow["RouteDayOfWeek"]);
                            errMsg += Environment.NewLine + "Route dow \'" + routeDayOfWeek + "\'";
                            routeDesc = Convert.ToString(drow["RouteDescription"]);
                            errMsg += Environment.NewLine + "Route desc \'" + routeDesc + "\'";

                            isRouteFound = false;

                            var resultsRoute = transaction.Query<tRouteModel>("SELECT RouteCode FROM tRoute WHERE RouteCode = ?", routeCode);

                            if (resultsRoute?.Count > 0)
                            {
                                isRouteFound = true;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    transaction.Execute("UPDATE tRoute SET RouteDescription = ?, RouteDayOfWeek = ? WHERE RouteCode = ?", routeDesc, routeDayOfWeek, routeCode);
                                }
                            }

                            if (!isRouteFound)
                            {
                                transaction.Execute("INSERT INTO tRoute (RouteCode, RouteDescription, RouteDayOfWeek) VALUES (?, ?, ?)", routeCode, routeDesc, routeDayOfWeek);
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes failed.", false);

                    throw new Exception("Database exception occurred. Saving routes failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST ROUTES</action>"
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
