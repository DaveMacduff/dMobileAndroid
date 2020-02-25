namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class TableActivityTransferFromDetail : TableBase
    {
        #region Constructors

        public TableActivityTransferFromDetail()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferFromDetail";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request(int actItemRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details.", true);

                if (actItemRefno == 0)
                {
                    using (SQLiteCommand cmndActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndActItem.CommandType = CommandType.Text;
                        cmndActItem.CommandText = "SELECT tActivityItem.ActivityItemRefno "
                            + "FROM tActivityItem "
                            + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TransferFrom;

                        using (SQLiteDataReader drActItem = cmndActItem.ExecuteReader())
                        {
                            while (drActItem.Read())
                            {
                                actItemRefno = Convert.ToInt32(drActItem["ActivityItemRefno"]);
                                reqData = this.Send(actItemRefno);

                                if (0 < reqData.Length)
                                {
                                    if (reqData.Contains("<message>ERROR</message>"))
                                    {
                                        XmlDocument reqErr = new XmlDocument();
                                        reqErr.LoadXml(reqData);
                                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                    }

                                    this.Save(reqData, actItemRefno);
                                }
                            }

                            drActItem.Close();
                        }
                    }
                }
                else
                {
                    reqData = this.Send(actItemRefno);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        this.Save(reqData, actItemRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding transfer details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding transfer details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TransferFrom;
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
                    DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfer details.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Transfer details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TransferFrom + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailTransfer "
                        + "WHERE tActivityDetailTransfer.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TransferFrom + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TransferFrom + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = (string)("DELETE FROM tActivityDetail" +
                        " WHERE tActivityDetail.ActivityDetailSubType = " +
                        Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom));
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging transfer details failed.", ex);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string Send(int actItemRefno)
        {
            int actItemFKRefno = 0;

            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandText = "SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = @activityItemRefno";
                cmndItem.CommandType = CommandType.Text;
                //cmndItem.IndexName = "PK_ActivityItemTransferOnActivityItemRefno";
                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemRefno)));

                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    //if (drItem.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRefno }))
                    //{
                    if (drItem.Read())
                    {
                        actItemFKRefno = drItem.GetInt32((int)SysCommon.ActItemTransferFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drItem.Read() failed. " + string.Format("Unable to locate to activity item transfer refno {0} in tActivityItemTransfer.", actItemRefno));
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate to activity item transfer refno {0} in tActivityItemTransfer.", actItemRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(actItemFKRefno));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveData"></param>
        /// <param name="actItemTransferRefno"></param>
        protected override void Save(string saveData, int actItemTransferRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            SQLiteCommand cmndDetail = conn.CreateCommand();
            SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand();
            SQLiteCommand cmndExtra = conn.CreateCommand();
            string errMsg = string.Empty;

            using (SQLiteCommand cmndDetailTransfer = conn.CreateCommand())
            using (SQLiteCommand cmndItemTransfer = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref saveData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailTransfer"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                            cmndID.CommandType = CommandType.Text;

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailTransferFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemTransfer.CommandText = "SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemTransfer.CommandType = CommandType.Text;
                            //cmndItemTransfer.IndexName = "PK_ActivityItemTransferOnActivityItemRefno";
                            cmndItemTransfer.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemTransferRefno)));

                            using (SQLiteDataReader drItemTransfer = cmndItemTransfer.ExecuteReader())
                            {
                                //if (drItemTransfer.Seek(DbSeekOptions.FirstEqual, new object[] { actItemTransferRefno }))
                                //{
                                if (!drItemTransfer.Read())
                                {
                                    // TODO: Resource strings
                                    throw new Exception("drItemTransfer.Read() failed." + Environment.NewLine + errMsg + string.Format(" Unable to locate the activity item transfer refno {0} in tActivityItemTransfer.", actItemTransferRefno));
                                }
                                //else
                                //{
                                //    throw new Exception(string.Format("Unable to locate the activity item transfer refno {0} in tActivityItemTransfer.", actItemTransferRefno));
                                //}

                                foreach (DataRow drow in ds.Tables["tActivityDetailTransfer"].Rows)
                                {
                                    errMsg = "Arguments:";

                                    cmndDetail.CommandText = "INSERT INTO tActivityDetail " +
                                    "(" +
                                    "ActivityDetailSubType, " +
                                    "ActivityDetailStatus, " +
                                    "ActivityItemRefno, " +
                                    "LocationMapRefno, " +
                                    "ProductUnitRefno, " +
                                    "ActivityDetailUniqueSchemeID, " +
                                    "ActivityDetailSchemeID, " +
                                    "ActivityDetailQuantity, " +
                                    "ActivityDetailBaseQuantity, " +
                                    "ActivityDetailWeightQuantity, " +
                                    "ActivityDetailSingleTradeItemSerialNumber, " +
                                    "ActivityDetailLogisticUnitBatchLotNumber, " +
                                    "ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                                    "ActivityDetailLogisticUnitExpiryUseByDate, " +
                                    "ActivityDetailLogisticUnitProductionDate, " +
                                    "ActivityDetailLogisticUnitDueDate, " +
                                    "ActivityDetailLogisticUnitPackagingDate, " +
                                    "ActivityDetailLogisticUnitSkidNumber" +
                                    ") " +
                                    "VALUES" +
                                    "(" +
                                    "@activityDetailSubType, " +
                                    "@activityDetailStatus, " +
                                    "@activityItemRefno, " +
                                    "@locationMapRefno, " +
                                    "@productUnitRefno, " +
                                    "@activityDetailUniqueSchemeID, " +
                                    "@activityDetailSchemeID, " +
                                    "@activityDetailQuantity, " +
                                    "@activityDetailBaseQuantity, " +
                                    "@activityDetailWeightQuantity, " +
                                    "@activityDetailSingleTradeItemSerialNumber, " +
                                    "@activityDetailLogisticUnitBatchLotNumber, " +
                                    "@activityDetailLogisticUnitBestBeforeSellByDate, " +
                                    "@activityDetailLogisticUnitExpiryUseByDate, " +
                                    "@activityDetailLogisticUnitProductionDate, " +
                                    "@activityDetailLogisticUnitDueDate, " +
                                    "@activityDetailLogisticUnitPackagingDate, " +
                                    "@activityDetailLogisticUnitSkidNumber" +
                                    ")";
                                    cmndDetail.CommandType = CommandType.Text;
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemTransferRefno)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", drItemTransfer.GetInt32((int)SysCommon.ActItemTransferFields.ProductUnitRefno)));

                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.TransferFrom);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemTransferRefno);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, drItemTransfer.GetInt32((int)SysCommon.ActItemTransferFields.ProductUnitRefno));

                                    this.SaveDetailLocation(drow, ref conn, ref cmndDetail, false);
                                    this.SaveDetail(drow, ref cmndDetail, false);

                                    cmndDetail.ExecuteNonQuery();

                                    actDetailTransferFKRefno = Convert.ToInt32(drow["ActivityDetailTransferFKRefno"]);
                                    errMsg += Environment.NewLine + "Detail fk refno " + actDetailTransferFKRefno;

                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                    // TODO: double check that changes work
                                    cmndDetailTransfer.CommandText = "INSERT INTO tActivityDetailTransfer (ActivityDetailRefno, " +
                                        "ActivityDetailTransferFKRefno, " +
                                        "ActivityDetailTransferIsConfirmed, " +
                                        "ActivityDetailTransferTransModeEnum" +
                                        ") VALUES (" +
                                        "@activityDetailRefno, " +
                                        "@activityDetailTransferFKRefno, " +
                                        "@activityDetailTransferIsConfirmed, " +
                                        "@activityDetailTransferTransModeEnum)";
                                    cmndDetailTransfer.CommandType = CommandType.Text;
                                    cmndDetailTransfer.Parameters.Add(new SQLiteParameter("@activityDetailRefno", Convert.ToInt32(actDetailRefno)));
                                    cmndDetailTransfer.Parameters.Add(new SQLiteParameter("@activityDetailTransferFKRefno", Convert.ToInt32(actDetailTransferFKRefno)));
                                    cmndDetailTransfer.Parameters.Add(new SQLiteParameter("@activityDetailTransferIsConfirmed", Convert.ToBoolean(drow["ActivityDetailTransferIsConfirmed"])));
                                    cmndDetailTransfer.Parameters.Add(new SQLiteParameter("@activityDetailTransferTransModeEnum", Convert.ToInt32(drow["ActivityDetailTransferTransModeEnum"])));

                                    cmndDetailTransfer.ExecuteNonQuery();

                                    //recDetailTransfer.SetInt32((int)SysCommon.ActDetailTransferFields.ActDetailRefno, actDetailRefno);
                                    //recDetailTransfer.SetInt32((int)SysCommon.ActDetailTransferFields.FKRefno, actDetailTransferFKRefno);
                                    //recDetailTransfer.SetBoolean((int)SysCommon.ActDetailTransferFields.IsConfirmed, Convert.ToBoolean(drow["ActivityDetailTransferIsConfirmed"]));
                                    //recDetailTransfer.SetInt32((int)SysCommon.ActDetailTransferFields.TransModeEnum, Convert.ToInt32(drow["ActivityDetailTransferTransModeEnum"]));
                                    //rsDetailTransfer.Insert(recDetailTransfer);

                                    if (this.SaveCommonDetailVariant(drow, ref conn, ref cmndSchemeVarXRef, actDetailRefno, false))
                                    {
                                        cmndSchemeVarXRef.ExecuteNonQuery();
                                    }

                                    this.SaveDetailExtra(drow, ref cmndExtra, actDetailRefno, false);

                                    progValue++;

                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            }

                            transaction.Commit();
                        }
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving transfer details failed." + Environment.NewLine + errMsg, ex);
                }
                finally
                {
                    cmndDetail.Dispose();
                    cmndSchemeVarXRef.Dispose();
                    cmndExtra.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemTransferFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int actItemTransferFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TRANSFER FROM DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemTransferFKRefno.ToString() + "</itm_refno>")
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