namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableRoute" />
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
        /// The Request
        /// </summary>
        public override void Request()
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes.", true);

                reqData = this.Send("routes");

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    this.Save(reqData);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding routes failed.", ex);
            }
        }

        /// <summary>
        /// The GetiRouteRefnoFromRouteCode
        /// </summary>
        /// <param name="routeCode"></param>
        /// <returns></returns>
        public int GetiRouteRefnoFromRouteCode(string routeCode)
        {
            int routeRefno = 0;

            //      try
            //      {
            //        using (SQLiteCommand cmndRoute = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //        {
            //          cmndRoute.CommandText = "tRoute";
            //          cmndRoute.IndexName = "UQ_RouteOnRouteCode";
            //          cmndRoute.CommandType = CommandType.TableDirect;

            //          using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
            //          {
            //            if (drRoute.Seek(DbSeekOptions.FirstEqual, new object[] { routeCode }))
            //            {
            //              if (drRoute.Read())
            //              {
            //                routeRefno = Convert.ToInt32(drRoute.GetValue((int)SysCommon.RouteFields.Refno));
            //              }
            //              else
            //              {
            //#if MULTILANGUAGE
            //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrroutereadFailed));
            //#else
            //                throw new Exception("drRoute.Read() failed.");
            //#endif
            //              }
            //            }
            //            else
            //            {
            //              throw new Exception(
            //#if MULTILANGUAGE
            //                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateRouteRefnoFromRouteCodeInTroute), routeCode));
            //#else
            //                string.Format("Unable To Locate Route Refno From Route Code {0} In tRoute", routeCode));
            //#endif
            //            }

            //            drRoute.Close();
            //          }
            //        }
            //      }
            //      catch (SQLiteException ex)
            //      {
            //#if MULTILANGUAGE
            //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetirouterefnofromroutecodeFailed), ex);
            //#else
            //        throw new Exception("Database exception occurred. GetiRouteRefnoFromRouteCode failed.", ex);
            //#endif
            //      }

            return routeRefno;
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tRoute";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} routes.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Routes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tRoute";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging routes failed.", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndRoute = conn.CreateCommand())
            using (SQLiteCommand drRouteUpdate = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progValue = 0;
                            bool isRouteFound = false;
                            string routeDesc = string.Empty;
                            string routeDayOfWeek = string.Empty;
                            string routeCode = string.Empty;
                            int progMax = Convert.ToInt32(ds.Tables["tRoute"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
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

                                cmndRoute.CommandText = "SELECT RouteCode FROM tRoute WHERE RouteCode = @routeCode";
                                cmndRoute.CommandType = CommandType.Text;
                                cmndRoute.Parameters.Add(new SQLiteParameter("@routeCode", routeCode));

                                using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
                                {
                                    if (drRoute.Read())
                                    {
                                        isRouteFound = true;

                                        // TODO: Uncomment SysEnv.RefreshOpt.RefreshDataChangesOnly
                                        //if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                        //{
                                        drRouteUpdate.CommandText = "UPDATE tRoute SET RouteDescription = @routeDesc, RouteDayOfWeek = @routeDayOfWeek WHERE RouteCode = @routeCode";
                                        drRouteUpdate.CommandType = CommandType.Text;
                                        drRouteUpdate.Parameters.Add(new SQLiteParameter("@routeDesc", routeDesc));
                                        drRouteUpdate.Parameters.Add(new SQLiteParameter("@routeDayOfWeek", routeDayOfWeek));
                                        drRouteUpdate.Parameters.Add(new SQLiteParameter("@routeCode", routeCode));

                                        drRouteUpdate.ExecuteNonQuery();
                                        //}
                                    }

                                    drRoute.Close();
                                }

                                if (!isRouteFound)
                                {
                                    cmndRoute.CommandText = "INSERT INTO tRoute (RouteCode, RouteDescription, RouteDayOfWeek) VALUES (@routeCode, @routeDescription, @routeDayOfWeek)";
                                    cmndRoute.CommandType = CommandType.Text;
                                    cmndRoute.Parameters.Add(new SQLiteParameter("@routeCode", routeCode));
                                    cmndRoute.Parameters.Add(new SQLiteParameter("@routeDescription", routeDesc));
                                    cmndRoute.Parameters.Add(new SQLiteParameter("@routeDayOfWeek", routeDayOfWeek));

                                    cmndRoute.ExecuteNonQuery();
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

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes failed.", false);

                    throw new Exception("Database exception occurred. Saving routes failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override string Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST ROUTES</action>"
                + SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
