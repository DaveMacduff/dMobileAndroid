// <copyright file="TableActivityCutSheetFinished.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
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
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetFinished;
            this.LowerCaseName = "cut sheet finished products";
            this.ProperCaseName = "Cut sheet finished products";
        }

        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            var subType = (int)this.CurrentSubType;
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details.", true);
            await Conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    transaction.Execute(@"DELETE FROM tActivityCutSheetFinished 
                        WHERE tActivityCutSheetFinished.ActivityRefno <= 0 OR tActivityCutSheetFinished.ActivityFKRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityCutSheetFinished 
                        WHERE tActivityCutSheetFinished.ActivityRefno 
                            NOT IN (SELECT tActivityCutSheet.ActivityRefno FROM tActivityCutSheet)");
                    transaction.Execute(@"DELETE FROM tActivityCutSheetFinished 
                        WHERE tActivityCutSheetFinished.ActivityRefno NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)");
                    transaction.Execute(@"DELETE FROM tActivityCutSheet WHERE tActivityCutSheet.ActivityRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityCutSheet 
                        WHERE tActivityCutSheet.ActivityRefno NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)");
                    transaction.Execute(@"DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0");

                    // Regular purge.
                    transaction.Execute(@"DELETE FROM tActivityCutSheetFinished", subType);
                    transaction.Execute(@"DELETE FROM tActivityCutSheet WHERE tActivityCutSheet.ActivitySubType = ?", subType);
                    transaction.Execute(@"DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?", subType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details complete.", true);
                }
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details failed.", true);
                    throw new BaseException($"Database exception occurred. Purging {this.LowerCaseName} details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new BaseException($"Database exception occurred. Purging {this.LowerCaseName} details failed.", ex);
                }
            });
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            await Conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);
                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished cut sheets.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var table = ds.Tables["tActivityCutSheet"];
                        var progMax = table.Rows.Count;
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);
                        var actRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in table.Rows)
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                0,
                                (int)SysCommon.ActSubTypes.CutSheetFinished);
                            actRefno = transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity");
                            var parametersActivityCutSheet = new List<object>
                            {
                                actRefno,
                                (int)SysCommon.ActSubTypes.CutSheetFinished
                            };
                            whsLocMapRefno = GetWhsLocMapRefnoFromDataRow(drow, transaction);
                            parametersActivityCutSheet.Add(whsLocMapRefno);
                            var dt = DateTime.Parse(Convert.ToDateTime(drow["ActivityCutSheetDateTime"]).ToShortDateString(), CultureInfo.CurrentCulture);
                            parametersActivityCutSheet.Add(dt);
                            parametersActivityCutSheet.Add(drow["ActivityCutSheetDescription"].ToString());
                            transaction.Execute(@"INSERT INTO tActivityCutSheet (ActivityRefno, ActivitySubType,
                                    WarehouseLocationMapRefno, ActivityCutSheetDateTime, ActivityCutSheetDescription)
                                    VALUES (?, ?, ?, ?, ?)",
                                parametersActivityCutSheet.ToArray());

                            var actCutSheetFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            var parametersActivityCutSheetFinished = new object[]
                            {
                                actRefno,
                                actCutSheetFKRefno,
                                actCutSheetFKRefno.ToString(),
                                string.Empty,
                            };
                            transaction.Execute(@"INSERT INTO tActivityCutSheetFinished (ActivityRefno, 
                                    ActivityFKRefno, ActivityStringFKRefno, ActivityCutSheetFinishedDefaultLogisticUnitBatchLotNumber) 
                                    VALUES (?, ?, ?, ?)",
                                parametersActivityCutSheetFinished);

                            progValue++;
                            if ((progValue & 0x7F) == 0) // update every 128 records
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
                    throw new BaseException("Database exception occurred. Saving finished cut sheets failed.", ex);
                }
            });
        }

        /// <inheritdoc cref="TableBase.CreateRequest()"/>
        protected override async Task<string> CreateRequest()
        {
            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("imclient");
                writer.WriteStartElement("command");
                writer.WriteElementString("action", "REQUEST CUT SHEET FINISHED");

                // Insert request defaults as raw XML
                var requestDefaults = await SysCommon.GetRequestDefaults(false);
                if (!string.IsNullOrEmpty(requestDefaults))
                {
                    var defaultsDoc = XDocument.Parse("<root>" + requestDefaults + "</root>");
                    foreach (var elem in defaultsDoc.Root.Elements())
                    {
                        elem.WriteTo(writer);
                    }
                }

                writer.WriteEndElement(); // command
                writer.WriteEndElement(); // imclient
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sb.ToString();
        }
    }
}
