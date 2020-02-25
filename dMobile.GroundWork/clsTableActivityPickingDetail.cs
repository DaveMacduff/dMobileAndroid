namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityPickingDetail : TableBase
  {
    #region Constructors

    public TableActivityPickingDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPickingDetail";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details.", true);

                if (actItemRefno == 0)
                {
                    using (SQLiteCommand cmndPickItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndPickItem.CommandText = "SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = @activityItemSubType";
                        cmndPickItem.CommandType = CommandType.Text;
                        cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                        using (SQLiteDataReader drPickItem = cmndPickItem.ExecuteReader())
                        {
                            while (drPickItem.Read())
                            {
                                actItemRefno = Convert.ToInt32(drPickItem["ActivityItemRefno"]);
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

                            drPickItem.Close();
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding picking details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding picking details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sales order details failed.", ex);
            }
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailPicking "
                        + "WHERE tActivityDetailPicking.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales order details failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} Sales order details.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sales order details status failed.", ex);
            }

            return rowsAffected;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemPickRefno"></param>
        /// <returns></returns>
        protected override string Send(int actItemPickRefno)
        {
            int actItemPickFKRefno = 0;

            using (SQLiteCommand cmndItemPick = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItemPick.CommandText = "SELECT * FROM tActivityItemPicking WHERE ActivityItemRefno = @activityItemRefno";
                cmndItemPick.CommandType = CommandType.Text;
                //cmndItemPick.IndexName = "PK_ActivityItemPickingOnActivityItemRefno";
                cmndItemPick.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemPickRefno));

                using (SQLiteDataReader drItemPick = cmndItemPick.ExecuteReader())
                {
                    //if (drItemPick.Seek(DbSeekOptions.FirstEqual, new object[] { actItemPickRefno }))
                    //{
                    if (drItemPick.Read())
                    {
                        actItemPickFKRefno = Convert.ToInt32(drItemPick.GetValue((int)SysCommon.ActItemPickingFields.FKRefno));
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drItemPick.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the item picking refno {0} in tActivityItemPicking.", actItemPickRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(actItemPickFKRefno));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemPickRefno"></param>
        protected override void Save(string reqData, int actItemPickRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;
            SQLiteCommand cmndDetail = conn.CreateCommand();
            SQLiteCommand cmndExtra = conn.CreateCommand();

            using (SQLiteCommand cmndDetailPick = conn.CreateCommand())
            //using (SQLiteCommand cmndDetail = conn.CreateCommand())
            //using (SQLiteCommand cmndExtra = conn.CreateCommand())
            using (SQLiteCommand cmndItemPick = conn.CreateCommand())
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

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailPicking"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndDetailPick = conn.CreateCommand())
                            //{
                            //cmndDetailPick.CommandText = "tActivityDetailPicking";
                            //cmndDetailPick.CommandType = CommandType.Text;
                            //cmndDetailPick.IndexName = "UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno";

                            //using (SqlCeResultSet rsDetailPick = cmndDetailPick.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetailPick = rsDetailPick.CreateRecord();

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

                            //using (SQLiteCommand cmndItemPick = conn.CreateCommand())
                            //{
                            //cmndItemPick.CommandText = "tActivityItemPicking";
                            //cmndItemPick.CommandType = CommandType.Text;
                            //cmndItemPick.IndexName = "PK_ActivityItemPickingOnActivityItemRefno";

                            //using (SQLiteDataReader drItemPick = cmndItemPick.ExecuteReader())
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
                            int actDetailPickFKRefno = 0;
                            int productUnitRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemPick.CommandText = "SELECT * FROM tActivityItemPicking WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemPick.CommandType = CommandType.Text;
                            //cmndItemPick.IndexName = "PK_ActivityItemPickingOnActivityItemRefno";
                            cmndItemPick.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemPickRefno));

                            using (SQLiteDataReader drItemPick = cmndItemPick.ExecuteReader())
                            {
                                //if (drItemPick.Seek(DbSeekOptions.FirstEqual, new object[] { actItemPickRefno }))
                                //{
                                if (!drItemPick.Read())
                                {
                                    // TODO: Resource strings
                                    throw new Exception("drItemPick.Read() failed.");
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource strings
                                //    throw new Exception(string.Format("Unable to locate the activity item refno {0} in tActivityItemPicking", actItemPickRefno));
                                //}

                                productUnitRefno = drItemPick.GetInt32((int)SysCommon.ActItemPickingFields.ProdUnitRefno);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailPicking"].Rows)
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
                                //cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";
                                cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                                cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemPickRefno));
                                cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", productUnitRefno));

                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.Picking);
                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemPickRefno);
                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, productUnitRefno);

                                //this.SaveDetailLocation(drow, drLocMapSlot, drLocMapSect, drLocMapWhs, ref recDetail, false);

                                bool useFNo = false;

                                this.SaveDetailLocation(drow, ref conn, ref cmndDetail, useFNo);
                                this.SaveDetail(drow, ref cmndDetail, useFNo);

                                cmndDetail.ExecuteNonQuery();
                                //rsDetail.Insert(recDetail);

                                actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                actDetailPickFKRefno = Convert.ToInt32(drow["ActivityDetailPickingFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailPickFKRefno;

                                cmndDetailPick.CommandText = "INSERT INTO tActivityDetailPicking (" +
                                    "ActivityDetailRefno, " +
                                    "ActivityDetailPickingFKRefno, " +
                                    "ActivityDetailPickingIsConfirmed" +
                                    ") VALUES (" +
                                    "@activityDetailRefno, " +
                                    "@activityDetailPickingFKRefno, " +
                                    "@activityDetailPickingIsConfirmed" +
                                    ")";
                                cmndDetailPick.CommandType = CommandType.Text;
                                //cmndDetailPick.IndexName = "UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno";
                                cmndDetailPick.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                cmndDetailPick.Parameters.Add(new SQLiteParameter("@activityDetailPickingFKRefno", actDetailPickFKRefno));
                                cmndDetailPick.Parameters.Add(new SQLiteParameter("@activityDetailPickingIsConfirmed", Convert.ToBoolean(drow["ActivityDetailPickingIsConfirmed"])));

                                cmndDetailPick.ExecuteNonQuery();

                                //recDetailPick.SetInt32((int)SysCommon.ActDetailPickingFields.ActDetailRefno, actDetailRefno);
                                //recDetailPick.SetInt32((int)SysCommon.ActDetailPickingFields.FKRefno, actDetailPickFKRefno);
                                //recDetailPick.SetBoolean((int)SysCommon.ActDetailPickingFields.IsConfirmed, Convert.ToBoolean(drow["ActivityDetailPickingIsConfirmed"]));
                                //rsDetailPick.Insert(recDetailPick);

                                //if (this.SaveCommonDetailVariant(drow, drSchemeVar, ref recSchemeVarXRef, actDetailRefno, false))
                                //{
                                //    rsSchemeVarXRef.Insert(recSchemeVarXRef);
                                //}

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

                                this.SaveDetailExtra(drow, ref cmndExtra, actDetailRefno, false);

                                // Insert happens in this.SaveDetailExtra
                                //rsExtra.Insert(recExtra);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}
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

                        //                                    drItemPick.Close();
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details completed.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving sales order details failed." + Environment.NewLine + errMsg, ex);
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
    /// <param name="actItemPickFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actItemPickFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST SALES ORDER DETAILS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + actItemPickFKRefno.ToString() + "</itm_refno>") 
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
