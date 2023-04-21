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

    /// <summary>
    /// Defines the <see cref="TableActivityAdjustment" />.
    /// </summary>
    public class TableActivityAdjustment : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityAdjustment"/> class.
        /// </summary>
        public TableActivityAdjustment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustment";
            this.CurrentSubType = SysCommon.ActSubTypes.Adjustment;
            this.LowerCaseName = "adjusments";
            this.ProperCaseName = "Adjustments";
        }

        /// <summary>
        /// Purge the activity header data.
        /// </summary>
        /// <returns><see cref="Task"/> Task complete.</returns>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
                var query = "DELETE FROM tActivityAdjustment WHERE tActivityAdjustment.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    query,
                    Convert.ToInt32(this.CurrentSubType));

                query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    query,
                    Convert.ToInt32(this.CurrentSubType));
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException sqlEx)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{sqlEx.Message}", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                throw;
            }
            catch
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task Save(string responseData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments.", false);

                    // Receiving schema but no responseData results in no table being created in the dataset.
                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityAdjustment"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;
                        var actAdjTypeDesc = string.Empty;
                        var adjFKRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityAdjustment"].Rows)
                        {
                            actRefno = 0;
                            actAdjTypeDesc = string.Empty;
                            adjFKRefno = 0;
                            whsLocMapRefno = 0;

                            var warehouseResults = transaction.Query<tLocationMapWarehouseModel>(
                                "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                                Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]));

                            if (warehouseResults != null && warehouseResults.Count > 0)
                            {
                                whsLocMapRefno = warehouseResults[0].LocationMapRefno;
                            }
                            else
                            {
                                throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                            }

                            errMsg = "Arguments:";

                            transaction.Execute(
                                "INSERT INTO tActivity(ActivityStatus, ActivitySubType) VALUES(?, ?)",
                                0,
                                Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));
                            actAdjTypeDesc = GetAdjustmentTypeDescription(drow);
                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                                "SELECT last_insert_rowid() FROM tActivity"));

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
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{sqlEx.Message}", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments failed.", true);
                    throw;
                }
            });

            //////////////////////////////////////////////////////////////////////////
            static string GetAdjustmentTypeDescription(DataRow drow)
            {
                string actAdjTypeDesc = string.Empty;
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

                return actAdjTypeDesc;
            }
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
                + "<action>REQUEST ADJUSTMENTS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno)
        {
            throw new NotImplementedException();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
