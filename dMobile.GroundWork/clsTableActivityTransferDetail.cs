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
  using Microsoft.VisualBasic;

  public class TableActivityTransferDetail : TableBase
  {
    #region Constructors

    public TableActivityTransferDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityTransferDetail";
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

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details.", true);

                if (actItemRefno == 0)
                {
                    using (SQLiteCommand cmndActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndActItem.CommandText = "SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                        cmndActItem.CommandType = CommandType.Text;
                        cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding co-product materials details failed.", true);

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
                    cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType";
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
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
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailTransfer "
                        + "WHERE tActivityDetailTransfer.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + " WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType";
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
                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));

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
                        throw new Exception("drItem.Read() failed.");
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
            string errMsg = string.Empty;
            SQLiteCommand cmndDetail = conn.CreateCommand();
            SQLiteCommand cmndExtra = conn.CreateCommand();

            using (SQLiteCommand cmndDetailTransfer = conn.CreateCommand())
            using (SQLiteCommand cmndItemTransfer = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
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

                            //using (SQLiteCommand cmndDetailTransfer = conn.CreateCommand())
                            //{
                            //cmndDetailTransfer.CommandText = "tActivityDetailTransfer";
                            //cmndDetailTransfer.CommandType = CommandType.Text;
                            //cmndDetailTransfer.IndexName = "UQ_ActivityDetailTransferOnActivityDetailTransferFKRefno";

                            //using (SqlCeResultSet rsDetailTransfer = cmndDetailTransfer.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetailTransfer = rsDetailTransfer.CreateRecord();

                            //using (SQLiteCommand cmndDetail = conn.CreateCommand())
                            //{
                            //cmndDetail.CommandText = "tActivityDetail";
                            //cmndDetail.CommandType = CommandType.Text;
                            //cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";

                            //using (SqlCeResultSet rsDetail = cmndDetail.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetail = rsDetail.CreateRecord();

                            //using (SQLiteCommand cmndExtra = conn.CreateCommand())
                            //{
                            //cmndExtra.CommandText = "tActivityDetailExtra";
                            //cmndExtra.CommandType = CommandType.Text;
                            //cmndExtra.IndexName = "PK_ActivityDetailExtraOnActivityDetailRefno";

                            //using (SqlCeResultSet rsExtra = cmndExtra.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recExtra = rsExtra.CreateRecord();

                            //using (SQLiteCommand cmndItemTransfer = conn.CreateCommand())
                            //{
                            //cmndItemTransfer.CommandText = "tActivityItemTransfer";
                            //cmndItemTransfer.CommandType = CommandType.Text;
                            //cmndItemTransfer.IndexName = "PK_ActivityItemTransferOnActivityItemRefno";

                            //using (SQLiteDataReader drItemTransfer = cmndItemTransfer.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
                            //{
                            //cmndLocMapSlot.CommandText = "tLocationMapSlot";
                            //cmndLocMapSlot.CommandType = CommandType.Text;
                            //cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

                            //using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
                            //{
                            //cmndLocMapSect.CommandText = "tLocationMapSection";
                            //cmndLocMapSect.CommandType = CommandType.Text;
                            //cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

                            //using (SQLiteDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
                            //{
                            //cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
                            //cmndLocMapWhs.CommandType = CommandType.Text;
                            //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                            //using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
                            //{
                            //cmndSchemeVar.CommandText = "tProductSchemeVariant";
                            //cmndSchemeVar.CommandType = CommandType.Text;
                            //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                            //using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
                            //{
                            //cmndSchemeVarXRef.CommandText = "tActivityDetailProductSchemeVariantXRef";
                            //cmndSchemeVarXRef.CommandType = CommandType.Text;
                            //cmndSchemeVarXRef.IndexName = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

                            //using (SqlCeResultSet rsSchemeVarXRef = cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recSchemeVarXRef = rsSchemeVarXRef.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                            cmndID.CommandType = CommandType.Text;

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailTransferFKRefno = 0;
                            int productUnitRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemTransfer.CommandText = "SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemTransfer.CommandType = CommandType.Text;
                            //cmndItemTransfer.IndexName = "PK_ActivityItemTransferOnActivityItemRefno";
                            cmndItemTransfer.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemTransferRefno));

                            using (SQLiteDataReader drItemTransfer = cmndItemTransfer.ExecuteReader())
                            {
                                //if (drItemTransfer.Seek(DbSeekOptions.FirstEqual, new object[] { actItemTransferRefno }))
                                //{
                                if (!drItemTransfer.Read())
                                {
                                    // TODO: Resource strings
                                    throw new Exception("drItemTransfer.Read() failed." + Environment.NewLine + errMsg);
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource strings
                                //    throw new Exception(string.Format("Unable to locate the activity item transfer refno {0} in tActivityItemTransfer.", actItemTransferRefno));
                                //}

                                productUnitRefno = drItemTransfer.GetInt32((int)SysCommon.ActItemTransferFields.ProductUnitRefno);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndDetail.CommandText = "INSERT INTO tActivityDetail (" +
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
                               ") VALUES (" +
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
                                cmndDetail.CommandType = CommandType.Text;
                                //cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";

                                cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));
                                cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemTransferRefno));
                                cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", productUnitRefno));

                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.Transfers);
                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemTransferRefno);
                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, productUnitRefno);

                                //this.SaveDetailLocation(drow, drLocMapSlot, drLocMapSect, drLocMapWhs, ref recDetail, false);

                                int locMapRefno = 0;
                                int locMapWhsFKRefno = 0;
                                int locMapSectFKRefno = 0;
                                int locMapSlotFKRefno = 0;
                                bool useFNo = false;

                                if (useFNo)
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["f07"]);
                                    locMapSectFKRefno = Convert.ToInt32(drow["f08"]);
                                    locMapSlotFKRefno = Convert.ToInt32(drow["f09"]);
                                }
                                else
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                                    locMapSectFKRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                                    locMapSlotFKRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
                                }

                                cmndLocMapSlot.CommandText = "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @locationMapSlotFKRefno";
                                cmndLocMapSlot.CommandType = CommandType.Text;
                                //cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                                cmndLocMapSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotFKRefno", locMapSlotFKRefno));

                                cmndLocMapSect.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                                cmndLocMapSect.CommandType = CommandType.Text;
                                //cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                cmndLocMapSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", locMapSlotFKRefno));

                                cmndLocMapWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndLocMapWhs.CommandType = CommandType.Text;
                                //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndLocMapWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapWhsFKRefno));

                                using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                                using (SQLiteDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
                                using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                {
                                    locMapRefno = this.SaveDetailLocation(drow, drLocMapSlot, drLocMapSect, drLocMapWhs, /*ref recDetail, */ useFNo);
                                }

                                cmndDetail.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));

                                this.SaveDetail(drow, ref cmndDetail, false);

                                cmndDetail.ExecuteNonQuery();
                                //rsDetail.Insert(recDetail);

                                actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                actDetailTransferFKRefno = Convert.ToInt32(drow["ActivityDetailTransferFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailTransferFKRefno;

                                cmndDetailTransfer.CommandText = "INSERT INTO tActivityDetailTransfer (ActivityDetailRefno, " +
                                    "ActivityDetailTransferFKRefno, " +
                                    "ActivityDetailTransferIsConfirmed" +
                                    ") VALUES (" +
                                    "@activityDetailRefno, " +
                                    "@activityDetailTransferFKRefno, " +
                                    "@activityDetailTransferIsConfirmed)";
                                cmndDetailTransfer.CommandType = CommandType.Text;
                                //cmndDetailTransfer.IndexName = "UQ_ActivityDetailTransferOnActivityDetailTransferFKRefno";
                                cmndDetailTransfer.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                cmndDetailTransfer.Parameters.Add(new SQLiteParameter("@activityDetailTransferFKRefno", actDetailTransferFKRefno));
                                cmndDetailTransfer.Parameters.Add(new SQLiteParameter("@activityDetailTransferIsConfirmed", Convert.ToBoolean(drow["ActivityDetailTransferIsConfirmed"])));

                                cmndDetailTransfer.ExecuteNonQuery();

                                //recDetailTransfer.SetInt32((int)SysCommon.ActDetailTransferFields.ActDetailRefno, actDetailRefno);
                                //recDetailTransfer.SetInt32((int)SysCommon.ActDetailTransferFields.FKRefno, actDetailTransferFKRefno);
                                //recDetailTransfer.SetBoolean((int)SysCommon.ActDetailTransferFields.IsConfirmed, Convert.ToBoolean(drow["ActivityDetailTransferIsConfirmed"]));
                                //rsDetailTransfer.Insert(recDetailTransfer);

                                //if (this.SaveCommonDetailVariant(drow, drSchemeVar, ref recSchemeVarXRef, actDetailRefno, false))
                                //{
                                //    rsSchemeVarXRef.Insert(recSchemeVarXRef);
                                //}

                                //this.SaveDetailExtra(drow, ref recExtra, actDetailRefno, false);
                                //rsExtra.Insert(recExtra);

                                int schemeVariantNo = 0;

                                if (useFNo)
                                {
                                    schemeVariantNo = Convert.ToInt32(drow["f17"]);
                                }
                                else
                                {
                                    schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                                }

                                if (0 < schemeVariantNo)
                                {
                                    cmndSchemeVar.CommandText = "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = @productSchemeVariantNo";
                                    cmndSchemeVar.CommandType = CommandType.Text;
                                    //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
                                    cmndSchemeVar.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", schemeVariantNo));

                                    using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                                    {
                                        //if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVariantNo }))
                                        //{
                                        if (drSchemeVar.Read())
                                        {
                                            cmndSchemeVarXRef.CommandText = "INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (@activityDetailRefno, @productSchemeVariantRefno)";
                                            cmndSchemeVarXRef.CommandType = CommandType.Text;
                                            //cmndSchemeVarXRef.IndexName = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";
                                            cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                            cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeVariantRefno", drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno)));

                                            cmndSchemeVarXRef.ExecuteNonQuery();

                                            //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ActDetailRefno, actDetailRefno);
                                            //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ProdSchemeVariantRefno, drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno));

                                        }
                                        else
                                        {
                                            // TODO: Resource strings
                                            throw new Exception("drSchemeVar.Read() failed.");
                                        }
                                        //}
                                        //else
                                        //{
                                        //    // TODO: Resource strings
                                        //    throw new Exception(string.Format("Unable to locate product scheme no {0} in tProductSchemeVariant", schemeVariantNo));
                                        //}
                                    }
                                }

                                this.SaveDetailExtra(drow, ref cmndExtra, actDetailRefno, useFNo);

                                progValue++;
                                if (progValue % 100 == 0)
                                {

                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //                                                                        }
                        //                                                                    }

                        //                                                                    drSchemeVar.Close();
                        //                                                                }
                        //                                                            }

                        //                                                            drLocMapWhs.Close();
                        //                                                        }
                        //                                                    }

                        //                                                    drLocMapSect.Close();
                        //                                                }
                        //                                            }

                        //                                            drLocMapSlot.Close();
                        //                                        }
                        //                                    }

                        //                                    drItemTransfer.Close();
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);
                    }
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
          + "<action>REQUEST TRANSFER DETAILS</action>"
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
