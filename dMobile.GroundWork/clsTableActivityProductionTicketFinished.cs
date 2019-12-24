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
    /// Defines the <see cref="TableActivityProductionTicketFinished" />
    /// </summary>
    public class TableActivityProductionTicketFinished : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketFinished"/> class.
        /// </summary>
        public TableActivityProductionTicketFinished()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinished";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished tickets.", true);

                reqData = this.Send("finished ticket");
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished tickets complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished tickets failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding finished tickets failed.", ex);
            }
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
                    cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno)"
                        + " FROM tActivity"
                        + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FinishedTickets), rowsAffected),
#else
              string.Format("{0} finished tickets.", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredFinishedTicketsStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Finished tickets status failed.", ex);
#endif
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
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket.", true);

                    cmndPurge.CommandText = "DELETE FROM tActivityProductionFinished"
                        + " WHERE tActivityProductionFinished.ActivityRefno IN"
                        + " (SELECT tActivityProduction.ActivityRefno"
                        + " FROM tActivityProduction"
                        + " WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityProduction "
                        + "WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity "
                        + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket complete.", true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket complete.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging finished ticket complete.", ex);
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

            using (SQLiteCommand cmndProduction = conn.CreateCommand())
            using (SQLiteCommand cmndFinished = conn.CreateCommand())
            using (SQLiteCommand cmndActivity = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndProduction = conn.CreateCommand())
                            //{
                            //cmndProduction.CommandText = "tActivityProduction";
                            //cmndProduction.CommandType = CommandType.Text;
                            //cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";

                            //using (SQLiteCommand cmndFinished = conn.CreateCommand())
                            //{
                            //cmndFinished.CommandText = "tActivityProductionFinished";
                            //cmndFinished.CommandType = CommandType.Text;
                            //cmndFinished.IndexName = "UQ_ActivityProductionFinishedOnActivityProductionFKRefno";

                            //using (SqlCeResultSet rsFinished = cmndFinished.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recFinished = rsFinished.CreateRecord();

                            //using (SQLiteCommand cmndActivity = conn.CreateCommand())
                            //{
                            //cmndActivity.CommandText = "tActivity";
                            //cmndActivity.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsActivity = cmndActivity.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActivity = rsActivity.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            //cmndID.CommandText = "SELECT @@IDENTITY";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SQLiteCommand cmndWhs = conn.CreateCommand())
                            //{
                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                            //cmndWhs.CommandType = CommandType.Text;

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rsProduction = cmndProduction.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recProduction = rsProduction.CreateRecord();

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndActivity.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndActivity.CommandType = CommandType.Text;
                                cmndActivity.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndActivity.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));

                                cmndActivity.ExecuteNonQuery();

                                //recActivity.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.TicketFinished);
                                //recActivity.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //rsActivity.Insert(recActivity);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                cmndProduction.CommandText = "INSERT INTO tActivityProduction (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityProductionDateTime, " +
                                    "ActivityProductionDescription" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityProductionDateTime, " +
                                    "@activityProductionDescription" +
                                    ")";
                                cmndProduction.CommandType = CommandType.Text;
                                //cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));

                                //recProduction.SetInt32((int)SysCommon.ActProdFields.ActRefno, actRefno);
                                //recProduction.SetInt32((int)SysCommon.ActProdFields.ActSubType, (int)SysCommon.ActSubTypes.TicketFinished);

                                cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"])));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                }

                                cmndProduction.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                //recProduction.SetInt32((int)SysCommon.ActProdFields.WhsLocMapRefno, whsLocMapRefno);

                                DateTime dt = DateTime.Parse(Convert.ToDateTime(drow["ActivityProductionDateTime"]).ToShortDateString());
                                errMsg += Environment.NewLine + "Date " + dt.ToShortDateString();

                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityProductionDateTime", dt));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityProductionDescription", drow["ActivityProductionDescription"].ToString()));

                                cmndProduction.ExecuteNonQuery();

                                //recProduction.SetDateTime((int)SysCommon.ActProdFields.DateTime, dt);
                                //recProduction.SetString((int)SysCommon.ActProdFields.Description, drow["ActivityProductionDescription"].ToString());
                                //rsProduction.Insert(recProduction);

                                int actProductionFKRefno = Convert.ToInt32(drow["ActivityProductionFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + actProductionFKRefno;

                                cmndFinished.CommandText = "INSERT INTO tActivityProductionFinished (" +
                                    "ActivityRefno, " +
                                    "ActivityProductionFKRefno, " +
                                    "ActivityProductionStringFKRefno, " +
                                    "ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activityProductionFKRefno, " +
                                    "@activityProductionStringFKRefno, " +
                                    "@activityProductionFinishedDefaultLogisticUnitBatchLotNumber" +
                                    ")";
                                cmndFinished.CommandType = CommandType.Text;
                                //cmndFinished.IndexName = "UQ_ActivityProductionFinishedOnActivityProductionFKRefno";
                                cmndFinished.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndFinished.Parameters.Add(new SQLiteParameter("@activityProductionFKRefno", actProductionFKRefno));
                                cmndFinished.Parameters.Add(new SQLiteParameter("@activityProductionStringFKRefno", actProductionFKRefno.ToString()));
                                cmndFinished.Parameters.Add(new SQLiteParameter("@activityProductionFinishedDefaultLogisticUnitBatchLotNumber", string.Empty));

                                cmndFinished.ExecuteNonQuery();

                                //recFinished.SetInt32((int)SysCommon.ActProdFinishedFields.ActivityRefno, actRefno);
                                //recFinished.SetInt32((int)SysCommon.ActProdFinishedFields.FKRefno, actProductionFKRefno);
                                //recFinished.SetString((int)SysCommon.ActProdFinishedFields.StringFKRefno, actProductionFKRefno.ToString());
                                //recFinished.SetString((int)SysCommon.ActProdFinishedFields.DefaultLogisticUnitBatchLotNumber, string.Empty);
                                //rsFinished.Insert(recFinished);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            transaction.Commit();

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        //drWhs.Close();
                        //}
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();

                    cmndProduction.Dispose();
                    cmndFinished.Dispose();
                    cmndActivity.Dispose();
                    cmndID.Dispose();
                    cmndWhs.Dispose();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets failed.", false);


                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving finished tickets failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST PRODUCTION TICKET FINISHED</action>"
                + SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
