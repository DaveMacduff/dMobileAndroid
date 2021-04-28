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
    using dMobile.Resources.Models;
    using System.Collections.Generic;

    public class TableActivityAdjustment : TableBase
    {
        #region Constructors

        public TableActivityAdjustment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustment";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request()
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments.", true);

                reqData = this.SendComm(this.Create());

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding adjustments failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments.", true);

                //cmndPurge.CommandText = "DELETE FROM tActivityAdjustment WHERE tActivityAdjustment.ActivitySubType = @activitySubType";
                //cmndPurge.ExecuteNonQuery();

                //cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                //cmndPurge.ExecuteNonQuery();

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityAdjustment WHERE tActivityAdjustment.ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purge adjustments failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(tActivity.ActivityRefno) FROM tActivity WHERE tActivity.ActivitySubType = ?"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} adjustment(s)", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Adjustments status failed.", ex);
            }

            return rowsAffected;
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
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityAdjustment"].Rows.Count);

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            string actAdjTypeDesc = string.Empty;
                            int adjFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityAdjustment"].Rows)
                            {
                                List<tLocationMapWarehouseModel> warehouseResults = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]));

                                if (warehouseResults != null && warehouseResults.Count > 0)
                                {
                                    whsLocMapRefno = warehouseResults[0].LocationMapRefno;
                                }
                                else
                                {
                                    // TODO: Resource String
                                    throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                }

                                // TODO: Resource Strings
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivity(ActivityStatus, ActivitySubType) VALUES(?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                                switch (Convert.ToInt32(drow["ActivityAdjustmentType"]))
                                {
                                    case (int)SysCommon.AdjTypes.Inventory:
                                        actAdjTypeDesc = "Inventory";
                                        break;

                                    case (int)SysCommon.AdjTypes.UnitConversion:
                                        actAdjTypeDesc = "Unit Converstion";
                                        break;

                                    case (int)SysCommon.AdjTypes.Rework:
                                        actAdjTypeDesc = "Rework";
                                        break;

                                    case (int)SysCommon.AdjTypes.PartialRework:
                                        actAdjTypeDesc = "Partial Rework";
                                        break;

                                    case (int)SysCommon.AdjTypes.Production:
                                        actAdjTypeDesc = "Production";
                                        break;

                                    case (int)SysCommon.AdjTypes.PartialProduction:
                                        actAdjTypeDesc = "Partial Production";
                                        break;

                                    case (int)SysCommon.AdjTypes.Disposal:
                                        actAdjTypeDesc = "Disposal";
                                        break;

                                    case (int)SysCommon.AdjTypes.Fresh2Frozen:
                                        actAdjTypeDesc = "Fresh To Frozen";
                                        break;

                                    case (int)SysCommon.AdjTypes.Transfer:
                                        actAdjTypeDesc = "Transfer";
                                        break;

                                    case (int)SysCommon.AdjTypes.PackOff:
                                        actAdjTypeDesc = "Pack Off";
                                        break;

                                    case (int)SysCommon.AdjTypes.LostFound:
                                        actAdjTypeDesc = "Lost And Found";
                                        break;

                                    case (int)SysCommon.AdjTypes.CycleCount:
                                        actAdjTypeDesc = "Cycle Count";
                                        break;

                                    case (int)SysCommon.AdjTypes.MoveToDeli:
                                        actAdjTypeDesc = "Move To Deli";
                                        break;

                                    case (int)SysCommon.AdjTypes.PeriodicCount:
                                        actAdjTypeDesc = "Periodic Count";
                                        break;

                                    case (int)SysCommon.AdjTypes.Manufacture:
                                        actAdjTypeDesc = "Manufacture";
                                        break;

                                    case (int)SysCommon.AdjTypes.Requisition:
                                        actAdjTypeDesc = "Requisition";
                                        break;

                                    case (int)SysCommon.AdjTypes.AutoConvertTo:
                                        actAdjTypeDesc = "Auto Convert To";
                                        break;

                                    case (int)SysCommon.AdjTypes.AutoCreateFrom:
                                        actAdjTypeDesc = "Auto Create From";
                                        break;

                                    case (int)SysCommon.AdjTypes.Debox:
                                        actAdjTypeDesc = "Debox";
                                        break;

                                    case (int)SysCommon.AdjTypes.ConvertSkid:
                                        actAdjTypeDesc = "Convert Skid";
                                        break;
                                }

                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));

                                adjFKRefno = Convert.ToInt32(drow["ActivityAdjustmentFKRefno"]);
                                DateTime dt = Convert.ToDateTime(drow["ActivityAdjustmentDateTime"]);

                                // TODO: Resource Strings
                                errMsg += Environment.NewLine + "FK refno " + adjFKRefno;
                                errMsg += Environment.NewLine + "Adj type \'" + actAdjTypeDesc + "\'";

                                string sql = "INSERT INTO tActivityAdjustment (ActivityRefno, " +
                                "ActivitySubType, " +
                                "ActivityAdjustmentFKRefno, " +
                                "ActivityAdjustmentStringFKRefno, " +
                                "ActivityAdjustmentType, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityAdjustmentDateTime, " +
                                "ActivityAdjustmentDescription, " +
                                "ActivityAdjustmentTypeDescription) " +
                                "VALUES " +
                                "(?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?)";

                                List<object> parameters = new List<object>()
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                                    adjFKRefno,
                                    adjFKRefno.ToString(),
                                    Convert.ToInt32(drow["ActivityAdjustmentType"]),
                                    whsLocMapRefno,
                                    DateTime.Parse(dt.ToShortDateString()),
                                    Convert.ToString(drow["ActivityAdjustmentDescription"]),
                                    actAdjTypeDesc
                                };

                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment)));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentFKRefno", adjFKRefno));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentStringFKRefno", adjFKRefno.ToString()));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentType", Convert.ToInt32(drow["ActivityAdjustmentType"])));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentDateTime", DateTime.Parse(dt.ToShortDateString())));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentDescription", drow["ActivityAdjustmentDescription"].ToString()));
                                //cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentTypeDescription", actAdjTypeDesc));
                                //cmndAdj.ExecuteNonQuery();

                                transaction.Execute(sql, parameters);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments complete.", true);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments failed.", true);

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving adjustments failed." + Environment.NewLine + errMsg, ex);
                }
                //}
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST ADJUSTMENTS</action>"
                + SysCommon.GetRequestDefaults(false)
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
