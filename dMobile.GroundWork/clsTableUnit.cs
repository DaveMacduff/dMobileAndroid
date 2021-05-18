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

    public class TableUnit : TableBase
    {
        #region Constructors

        public TableUnit()
        {
            this.FullClassName = "dMobile.GroundWork.TableUnit";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure.", true);

                reqData = await this.Send("units of measure");

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

                // Require to have units.
                if (0 >= await this.DataStatus())
                {
                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "No units of measure where received.", true);

                    // TODO: Resource strings
                    throw new Exception("No units of measure where received.");
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding units of measure failed.", ex);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="unitCode"></param>
        ///// <returns></returns>
        //public int GetiUnitRefnoFromUnitCode(string unitCode)
        //{
        //  int unitRefno = 0;

        //      try
        //      {
        //        using (SQLiteCommand cmndUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //        {
        //          cmndUnit.CommandText = "tUnit";
        //          //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
        //          cmndUnit.CommandType = CommandType.TableDirect;

        //          using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
        //          {
        //            if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
        //            {
        //              if (drUnit.Read())
        //              {
        //                unitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
        //              }
        //              else
        //              {
        //#if MULTILANGUAGE
        //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed));
        //#else
        //                throw new Exception("drUnit.Read() failed.");
        //#endif
        //              }
        //            }
        //            else
        //            {
        //              throw new Exception(
        //#if MULTILANGUAGE
        //                  string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromUnitCodeInTunit), unitCode));
        //#else
        //                  string.Format("Unable to locate the unit refno from unit code {0} in tUnit", unitCode));
        //#endif
        //            }

        //            drUnit.Close();
        //          }
        //        }
        //      }
        //      catch (SQLiteException ex)
        //      {
        //#if MULTILANGUAGE
        //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetiunitrefnofromunitcodeFailed), ex);
        //#else
        //        throw new Exception("Database exception occurred. GetiUnitRefnoFromUnitCode failed.", ex);
        //#endif
        //      }

        //  return unitRefno;
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tUnit"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} units of measure.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Units of measure status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource String
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tUnit");

                // TODO: Resource String
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource String
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure failed.", true);

                // TODO: Resource String
                throw new Exception("Database exception occurred. Purging units of measure failed.", ex);
            }
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = ds.Tables["tUnit"].Rows.Count;

                            // TODO: Resource String
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int progValue = 0;
                            bool isUnitFound = false;
                            string unitCode = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tUnit"].Rows)
                            {
                                unitCode = drow["UnitCode"].ToString();
                                errMsg = "Unit code \'" + unitCode + "\'";
                                isUnitFound = false;

                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    List<tUnitModel> results = transaction.Query<tUnitModel>("SELECT UnitCode FROM tUnit WHERE UnitCode = ?", unitCode);

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

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure failed.", false);

                    // TODO: Resource String
                    throw new Exception("Database exception occurred. Saving units of measure failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST UNITS</action>"
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
