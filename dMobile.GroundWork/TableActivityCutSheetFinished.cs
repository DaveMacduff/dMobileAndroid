// <copyright file="TableActivityCutSheetFinished.cs" company="InterSect Business Systems Inc">
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
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityCutSheetFinished" />.
    /// </summary>
    public class TableActivityCutSheetFinished : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetFinished"/> class.
        /// </summary>
        public TableActivityCutSheetFinished()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCutSheetFinished";
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetFinished;
            this.LowerCaseName = "cut sheet finished products";
            this.ProperCaseName = "Cut sheet finished products";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>Task operation.</returns>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details.", true);
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    var query = @"
                        DELETE FROM tActivityCutSheetFinished
                            WHERE tActivityCutSheetFinished.ActivityRefno <= ?
                                OR tActivityCutSheetFinished.ActivityFKRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityCutSheetFinished
                            WHERE tActivityCutSheetFinished.ActivityRefno
                                NOT IN (SELECT tActivityCutSheet.ActivityRefno FROM tActivityCutSheet)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityCutSheetFinished
                            WHERE tActivityCutSheetFinished.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityCutSheet
                            WHERE tActivityCutSheet.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityCutSheet
                            WHERE tActivityCutSheet.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                    transaction.Execute(query);

                    // Regular purge.
                    query = "DELETE FROM tActivityCutSheetFinished";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityCutSheet WHERE tActivityCutSheet.ActivitySubType = ?";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                    transaction.Execute(query, subType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details complete.", true);
                }
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details failed.", true);
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} details failed.", ex);
                }
            });
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished cut sheets.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityCutSheet"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityCutSheet"].Rows)
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                            const string queryActivityCutSheet = "INSERT INTO tActivityCutSheet (" +
                                "ActivityRefno, " +
                                "ActivitySubType, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityCutSheetDateTime, " +
                                "ActivityCutSheetDescription" +
                                ") VALUES (" +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?" +
                                ")";
                            var parametersActivityCutSheet = new List<object>
                            {
                                actRefno,
                                Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)
                            };

                            whsLocMapRefno = GetWhsLocMapRefnoFromDataRow(drow, transaction);
                            parametersActivityCutSheet.Add(whsLocMapRefno);
                            var dt = DateTime.Parse(Convert.ToDateTime(drow["ActivityCutSheetDateTime"]).ToShortDateString());
                            parametersActivityCutSheet.Add(dt);
                            parametersActivityCutSheet.Add(drow["ActivityCutSheetDescription"].ToString());
                            transaction.Execute(queryActivityCutSheet, parametersActivityCutSheet.ToArray());

                            var actCutSheetFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            const string queryActivityCutSheetFinished = "INSERT INTO tActivityCutSheetFinished (" +
                                "ActivityRefno, " +
                                "ActivityFKRefno, " +
                                "ActivityStringFKRefno, " +
                                "ActivityCutSheetFinishedDefaultLogisticUnitBatchLotNumber" +
                                ") VALUES (" +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?" +
                                ")";
                            var parametersActivityCutSheetFinished = new object[]
                            {
                                    actRefno,
                                    actCutSheetFKRefno,
                                    actCutSheetFKRefno.ToString(),
                                    string.Empty,
                            };

                            transaction.Execute(queryActivityCutSheetFinished, parametersActivityCutSheetFinished);

                            progValue++;
                            if (progValue % 10 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished cut sheets complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished cut sheets failed.", false);

                    throw new Exception("Database exception occurred. Saving finished cut sheets failed.", ex);
                }
            });
        }

        /// <summary>
        /// Create the request XML for cut sheet finished information.
        /// </summary>
        /// <returns>Request for cut sheet finished information.</returns>
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
                + "<action>REQUEST CUT SHEET FINISHED</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
