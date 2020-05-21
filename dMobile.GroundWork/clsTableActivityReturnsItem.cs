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

    public class TableActivityReturnsItem : TableBase
    {
        public TableActivityReturnsItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItem";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request(int actRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndReturns = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndReturns.CommandType = CommandType.Text;
                        cmndReturns.CommandText = "SELECT tActivity.ActivityRefno"
                            + " FROM tActivity"
                            + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Returns;

                        using (SQLiteDataReader drReturns = cmndReturns.ExecuteReader())
                        {
                            while (drReturns.Read())
                            {
                                actRefno = Convert.ToInt32(drReturns["ActivityRefno"]);

                                reqData = this.Send(actRefno);
                                
                                if (0 < reqData.Length)
                                {
                                    if (reqData.Contains("<message>ERROR</message>"))
                                    {
                                        XmlDocument reqErr = new XmlDocument();
                                        reqErr.LoadXml(reqData);
                                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                    }

                                    this.Save(reqData, actRefno);
                                }
                            }

                            drReturns.Close();
                        }
                    }
                }
                else
                {
                    reqData = this.Send(actRefno);
                    
                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        this.Save(reqData, actRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding sales return items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sales return items failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno "
                        + "FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemReturnsComment";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemReturns "
                          + "WHERE tActivityItemReturns.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales return items failed.", ex);
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
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) "
                        + "FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} Sales return items.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sales return items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string Send(int actRefno)
        {
            int returnsFKRefno = 0;

            using (SQLiteCommand cmndReturns = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndReturns.CommandText = "SELECT * FROM tActivityReturns WHERE ActivityRefno = @activityRefno";
                cmndReturns.CommandType = CommandType.Text;
                //cmndReturns.IndexName = "PK_ActivityReturnsOnActivityRefno";
                cmndReturns.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));


                using (SQLiteDataReader drReturns = cmndReturns.ExecuteReader())
                {
                    //if (drReturns.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
                    //{
                    if (drReturns.Read())
                    {
                        returnsFKRefno = drReturns.GetInt32((int)SysCommon.ActReturnsFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drReturns.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the sales return refno {0} in tActivityReturns", actRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(returnsFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override void Save(string reqData, int actRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndRetItem = conn.CreateCommand())
            using (SQLiteCommand cmndRetItemComment = conn.CreateCommand())
            using (SQLiteCommand cmndItem = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemReturns"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

                            //cmndProdUnit.CommandText = "tProductUnit";
                            //cmndProdUnit.CommandType = CommandType.Text;
                            //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                            //cmndProd.CommandText = "tProduct";
                            //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                            //cmndProd.CommandType = CommandType.Text;

                            //cmndUnit.CommandText = "tUnit";
                            //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                            //cmndUnit.CommandType = CommandType.Text;

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsRetItemComment = cmndRetItemComment.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recRetItemComment = rsRetItemComment.CreateRecord();

                            //using (SqlCeResultSet rsRetItem = cmndRetItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recRetItem = rsRetItem.CreateRecord();
                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            int actItemReturnsFKRefno = 0;

                            string actItemRetComment = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemReturns"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                                cmndItem.CommandType = CommandType.Text;
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(0)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));

                                cmndItem.ExecuteNonQuery();

                                //recItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Returns);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsItem.Insert(recItem);

                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref conn, ref errMsg, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndLocMapWhs);

                                actItemReturnsFKRefno = Convert.ToInt32(drow["ActivityItemReturnsFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + actItemReturnsFKRefno;

                                cmndRetItem.CommandText = "INSERT INTO tActivityItemReturns (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemReturnsFKRefno, " +
                                    "ActivityItemReturnsStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemReturnsQuantityReturned, " +
                                    "ActivityItemReturnsBaseQuantityReturned, " +
                                    "ActivityItemReturnsWeightQuantityReturned, " +
                                    "ActivityItemReturnsCustomerProductUnitCode, " +
                                    "ActivityItemReturnsDescription" +
                                    ") VALUES (" +
                                    "@activityItemRefno, " +
                                    "@activityItemSubType, " +
                                    "@activityItemReturnsFKRefno, " +
                                    "@activityItemReturnsStringFKRefno, " +
                                    "@productUnitRefno, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityItemReturnsQuantityReturned, " +
                                    "@activityItemReturnsBaseQuantityReturned, " +
                                    "@activityItemReturnsWeightQuantityReturned, " +
                                    "@activityItemReturnsCustomerProductUnitCode, " +
                                    "@activityItemReturnsDescription" +
                                    ")";
                                cmndRetItem.CommandType = CommandType.Text;
                                //cmndRetItem.IndexName = "UQ_ActivityItemReturnsOnActivityItemReturnsFKRefno";
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemRefno)));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemReturnsFKRefno", Convert.ToInt32(actItemReturnsFKRefno)));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemReturnsStringFKRefno", Convert.ToString(actItemReturnsFKRefno)));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(locMapRefno)));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemReturnsQuantityReturned", Convert.ToDecimal(drow["ActivityItemReturnsQuantityReturned"])));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemReturnsBaseQuantityReturned", Convert.ToDecimal(drow["ActivityItemReturnsBaseQuantityReturned"])));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemReturnsWeightQuantityReturned", Convert.ToDecimal(drow["ActivityItemReturnsWeightQuantityReturned"])));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemReturnsCustomerProductUnitCode", Convert.ToString(drow["ActivityItemReturnsCustomerProductUnitCode"])));
                                cmndRetItem.Parameters.Add(new SQLiteParameter("@activityItemReturnsDescription", Convert.ToString(drow["ActivityItemReturnsDescription"])));

                                cmndRetItem.ExecuteNonQuery();

                                //recRetItem.SetInt32((int)SysCommon.ActItemReturnsFields.ActItemRefno, actItemRefno);
                                //recRetItem.SetInt32((int)SysCommon.ActItemReturnsFields.ActItemSubType, (int)SysCommon.ActSubTypes.Returns);
                                //recRetItem.SetInt32((int)SysCommon.ActItemReturnsFields.FKRefno, actItemReturnsFKRefno);
                                //recRetItem.SetString((int)SysCommon.ActItemReturnsFields.StringFKRefno, Convert.ToString(actItemReturnsFKRefno));
                                //recRetItem.SetInt32((int)SysCommon.ActItemReturnsFields.ProdUnitRefno, prodUnitRefno);
                                //recRetItem.SetInt32((int)SysCommon.ActItemReturnsFields.WhsLocMapRefno, locMapRefno);
                                //recRetItem.SetString((int)SysCommon.ActItemReturnsFields.CustomerProdUnitCode, drow["ActivityItemReturnsCustomerProductUnitCode"].ToString());
                                //recRetItem.SetString((int)SysCommon.ActItemReturnsFields.Description, drow["ActivityItemReturnsDescription"].ToString());
                                //recRetItem.SetDecimal((int)SysCommon.ActItemReturnsFields.QuantityReturned, Convert.ToDecimal(drow["ActivityItemReturnsQuantityReturned"]));
                                //recRetItem.SetDecimal((int)SysCommon.ActItemReturnsFields.BaseQuantityReturned, Convert.ToDecimal(drow["ActivityItemReturnsBaseQuantityReturned"]));
                                //recRetItem.SetDecimal((int)SysCommon.ActItemReturnsFields.WeightQuantityReturned, Convert.ToDecimal(drow["ActivityItemReturnsWeightQuantityReturned"]));
                                //rsRetItem.Insert(recRetItem);

                                actItemRetComment = Convert.ToString(drow["ActivityItemReturnsComment"]);

                                if (!string.IsNullOrEmpty(actItemRetComment))
                                {
                                    cmndRetItemComment.CommandText = "INSERT INTO tActivityItemReturnsComment (ActivityItemRefno, ActivityItemReturnsCommentComment) VALUES (@activityItemRefno, @activityItemReturnsCommentComment)";
                                    cmndRetItemComment.CommandType = CommandType.Text;
                                    //cmndRetItemComment.IndexName = "PK_ActivityItemReturnsCommentOnActivityItemRefno";
                                    cmndRetItemComment.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemRefno)));
                                    cmndRetItemComment.Parameters.Add(new SQLiteParameter("@activityItemReturnsCommentComment", Convert.ToString(actItemRetComment)));

                                    cmndRetItemComment.ExecuteNonQuery();

                                    //recRetItemComment.SetInt32((int)SysCommon.ActItemReturnsCommentFields.ActItemRefno, actItemRefno);
                                    //recRetItemComment.SetString((int)SysCommon.ActItemReturnsCommentFields.Comment, actItemRetComment);
                                    //rsRetItemComment.Insert(recRetItemComment);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }

                    transaction.Commit();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving sales return items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnsFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int returnsFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES RETURN ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + returnsFKRefno + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
