// <copyright file="TableActivityAdjustment.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityAdjustment : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityAdjustment"/> class.
        /// </summary>
        public TableActivityAdjustment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustment";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task RequestData()
        {

            // GC.Collect();
            // GC.WaitForPendingFinalizers();
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments.", true);

                var reqData = await this.SendComm(await this.CreateRequest());

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments failed.", true);

                throw new Exception("Seeding adjustments failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments.", true);

                var query = "DELETE FROM tActivityAdjustment WHERE tActivityAdjustment.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments failed.", true);

                throw new Exception("Database exception occurred. Purge adjustments failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                var query = "SELECT COUNT(tActivity.ActivityRefno)"
                            + " FROM tActivity"
                            + " WHERE tActivity.ActivitySubType = ?";

                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, (int)SysCommon.ActSubTypes.Adjustment);

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} adjustment(s)", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Adjustments status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task Save(string reqData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityAdjustment"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actRefno = 0;
                            var progValue = 0;
                            var whsLocMapRefno = 0;
                            var actAdjTypeDesc = string.Empty;
                            var adjFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityAdjustment"].Rows)
                            {
                                var warehouseResults = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]));

                                if (warehouseResults != null && warehouseResults.Count > 0)
                                {
                                    whsLocMapRefno = warehouseResults[0].LocationMapRefno;
                                }
                                else
                                {
                                    throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                }

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

                                adjFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                var dt = Convert.ToDateTime(drow["ActivityAdjustmentDateTime"]);

                                errMsg += Environment.NewLine + "FK refno " + adjFKRefno;
                                errMsg += Environment.NewLine + "Adj type \'" + actAdjTypeDesc + "\'";

                                var sql = "INSERT INTO tActivityAdjustment (ActivityRefno, " +
                                          "ActivitySubType, " +
                                          "ActivityFKRefno, " +
                                          "ActivityStringFKRefno, " +
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

                                var parameters = new object[]
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                                    adjFKRefno,
                                    adjFKRefno.ToString(),
                                    Convert.ToInt32(drow["ActivityAdjustmentType"]),
                                    whsLocMapRefno,
                                    DateTime.Parse(dt.ToShortDateString()),
                                    Convert.ToString(drow["ActivityAdjustmentDescription"]),
                                    actAdjTypeDesc,
                                };

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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments complete.", true);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments failed.", true);

                    throw new Exception("Database exception occurred. Saving adjustments failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST ADJUSTMENTS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
