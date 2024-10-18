// <copyright file="TableActivityAdjustment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using dMobile.Resources.Models.LocationMap;
    using Resources.Models;
    using SQLite;

    /// <summary>
    ///     Defines the <see cref="TableActivityAdjustment" />.
    /// </summary>
    public class TableActivityAdjustment : TableBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableActivityAdjustment" /> class.
        /// </summary>
        public TableActivityAdjustment()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustment";
            this.CurrentSubType = SysCommon.ActSubTypes.Adjustment;
            this.LowerCaseName = "adjustments";
            this.ProperCaseName = "Adjustments";
        }

        /// <summary>
        ///     Purge the activity header data.
        /// </summary>
        /// <returns><see cref="Task" /> Task complete.</returns>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName}.",
                true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        var query = @"
                            DELETE FROM tActivityAdjustment
                                WHERE tActivityAdjustment.ActivityRefno <= 0
                                    OR tActivityAdjustment.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                            DELETE FROM tActivityAdjustment
                                WHERE tActivityAdjustment.ActivityRefno
                                    NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // REGULAR PURGE
                        query =
                            "DELETE FROM tActivityAdjustment WHERE tActivityAdjustment.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} complete.",
                            true);
                    }
                    catch (SQLiteException sqlEx)
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Database exception occurred.",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"{sqlEx.Message}",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw;
                    }
                    catch
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw;
                    }
                });
        }

        protected override async Task Save(string responseData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            using var ds = new DataSet();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Reading received responseData.",
                                false);

                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving adjustments.",
                                false);

                            // Receiving schema but no responseData results in no table being created in the dataset.
                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityAdjustment"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var progValue = 0;
                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityAdjustment"].Rows)
                                {
                                    var warehouseResults =
                                        transaction.Query<LocationMapWarehouseModel>(
                                            "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                                            Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]));

                                    int whsLocMapRefno;
                                    if (warehouseResults != null &&
                                        warehouseResults.Count > 0)
                                    {
                                        whsLocMapRefno = warehouseResults[0].LocationMapRefno;
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "drWhs.Read() failed." + Environment.NewLine + errMsg);
                                    }

                                    errMsg = "Arguments:";

                                    transaction.Execute(
                                        "INSERT INTO tActivity(ActivityStatus, ActivitySubType) VALUES(?, ?)",
                                        0,
                                        Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));
                                    var actAdjTypeDesc = GetAdjustmentTypeDescription(drow);
                                    var actRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivity"));

                                    var adjFkRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                    var dt = Convert.ToDateTime(drow["ActivityAdjustmentDateTime"]);

                                    errMsg += Environment.NewLine + "FK refno " + adjFkRefno;
                                    errMsg += Environment.NewLine +
                                              "Adj type \'" +
                                              actAdjTypeDesc +
                                              "\'";

                                    const string sql =
                                        "INSERT INTO tActivityAdjustment (" +
                                        "ActivityRefno, " +
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
                                        adjFkRefno,
                                        adjFkRefno.ToString(),
                                        Convert.ToInt32(drow["ActivityAdjustmentType"]),
                                        whsLocMapRefno,
                                        DateTime.Parse(dt.ToShortDateString()),
                                        Convert.ToString(drow["ActivityAdjustmentDescription"]),
                                        actAdjTypeDesc
                                    };

                                    transaction.Execute(sql, parameters);

                                    progValue++;
                                    if (progValue % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            progValue,
                                            0,
                                            progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    progMax,
                                    0,
                                    progMax);
                            }

                            transaction.Commit();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving adjustments complete.",
                                true);
                        }
                        catch (SQLiteException sqlEx)
                        {
                            transaction.Rollback();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                true);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                $"{sqlEx.Message}",
                                true);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving adjustments failed.",
                                true);
                            throw;
                        }
                    });
            return;

            //////////////////////////////////////////////////////////////////////////
            static string GetAdjustmentTypeDescription(DataRow drow)
            {
                var actAdjTypeDesc = string.Empty;
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

        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>REQUEST ADJUSTMENTS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}