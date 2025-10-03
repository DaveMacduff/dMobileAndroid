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
    using Bedrock;
    using Resources.Models;
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
        {
            this.LowerCaseName = "routes";
            this.ProperCaseName = "Routes";
        }

        /// <summary>
        /// The Request.
        /// </summary>
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
                throw new BaseException($"Seeding {this.LowerCaseName} failed.", ex);
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
        //                throw new BaseException(Translator.GetInstance().Translate(TranslatorVocabulary.DrroutereadFailed));
        // #else
        //                throw new BaseException("drRoute.Read() failed.");
        // #endif
        //              }
        //            }
        //            else
        //            {
        //              throw new BaseException(
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
        //        throw new BaseException(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetirouterefnofromroutecodeFailed), ex);
        // #else
        //        throw new BaseException("Database exception occurred. GetiRouteRefnoFromRouteCode failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tRoute"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} routes.", true);
            }
            catch (SQLiteException ex)
            {
                throw new BaseException("Database exception occurred. Routes status failed.", ex);
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

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tRoute");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes failed.", true);

                throw new BaseException("Database exception occurred. Purging routes failed.", ex);
            }
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
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
                    var routes = new List<RouteModel>();
                    var rowsToSave = dataSet.Tables["tRoute"].Rows.Count;
                    for (var idx = 0; idx < rowsToSave; idx++)
                    {
                        var row = dataSet.Tables["tRoute"].Rows[idx];
                        routes.Add(
                            new RouteModel
                            {
                                RouteCode = row["RouteCode"].ToString(),
                                RouteDescription = row["RouteDescription"].ToString(),
                                RouteDayOfWeek = row["RouteDayOfWeek"].ToString(),
                            });
                    }

                    await conn.InsertAllAsync(routes);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rows.Count} routes.",
                    true);
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
        /// The Create.
        /// </summary>
        /// <returns></returns>
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
                + "<action>REQUEST ROUTES</action>"
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
