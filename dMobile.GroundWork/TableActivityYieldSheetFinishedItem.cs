// <copyright file="TableActivityYieldSheetFinishedItem.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models.Headers;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityYieldSheetFinishedItem" />.
    /// </summary>
    public class TableActivityYieldSheetFinishedItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityYieldSheetFinishedItem"/> class.
        /// </summary>
        public TableActivityYieldSheetFinishedItem()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.YieldSheetFinished;
            this.LowerCaseName = "cut sheet finished product";
            this.ProperCaseName = "Cut sheet finished product";
        }

        /// <inheritdoc cref="TableBase.PurgeData()"/>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} items.", true);
            await Conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    transaction.Execute(@"DELETE FROM tActivityItemBarCodeAssociationXRef
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityItemBarCodeAssociationXRef
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)");
                    transaction.Execute(@"DELETE FROM tActivityItemYieldSheetFinished
                        WHERE tActivityItemYieldSheetFinished.ActivityItemRefno <= 0 
                            OR tActivityItemYieldSheetFinished.ActivityItemFKRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityItemYieldSheetFinished
                        WHERE tActivityItemYieldSheetFinished.ActivityItemRefno
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)");
                    transaction.Execute(@"DELETE FROM tActivityItemYieldSheet
                        WHERE tActivityItemYieldSheet.ActivityItemRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityItemYieldSheet 
                        WHERE tActivityItemYieldSheet.ActivityItemRefno
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)");
                    transaction.Execute(@"DELETE FROM tActivityItem 
                        WHERE tActivityItem.ActivityRefno NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)");
                    transaction.Execute(@"DELETE FROM tActivityItem
                        WHERE tActivityItem.ActivityRefno <= 0");

                    // Regular purge.
                    transaction.Execute(@"DELETE FROM tActivityItemBarCodeAssociationXRef 
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno 
                            IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem 
                            WHERE tActivityItem.ActivityItemSubType = ?)",
                        this.CurrentSubType);
                    transaction.Execute(
                        @"DELETE FROM tActivityItemYieldSheetFinished",
                        this.CurrentSubType);
                    transaction.Execute(
                        @"DELETE FROM tActivityItemYieldSheet WHERE ActivityItemSubType = ?",
                        this.CurrentSubType);
                    transaction.Execute(
                        @"DELETE FROM tActivityItem WHERE ActivityItemSubType = ?",
                        this.CurrentSubType);

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

        /// <inheritdoc cref="TableBase.SendRequest(int)"/>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await Conn.QueryAsync<ActivityYieldSheetFinishedModel>(
                    "SELECT * FROM tActivityYieldSheetFinished WHERE ActivityFKRefno = ?",
                    Convert.ToInt32(activityFkRefno));

            int actYieldSheetFKRefno;
            if (results?.Count > 0)
            {
                actYieldSheetFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new BaseException("drFinished.Read() failed.");
            }

            return await SendComm(await this.CreateRequest(actYieldSheetFKRefno));
        }

        /// <inheritdoc cref="TableBase.Save(string, int)"/>
        protected override async Task Save(string responseData, int refno)
        {
            await Conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished cut sheet items.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var table = ds.Tables["tActivityItemYieldSheet"];
                        var progMax = table.Rows.Count;
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);
                        var progValue = 0;
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in table.Rows)
                        {
                            transaction.Execute(
                                @"INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno,
                                    ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                (int)this.CurrentSubType,
                                (int)SysCommon.ActStatus.NoChange,
                                refno,
                                0);
                            var actItemRefno = transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem");
                            var prodUnitRefno = GetProductUnitRefnoFromDataRow(drow, transaction, false);
                            var locMapRefno = GetWhsLocMapRefnoFromDataRow(drow, transaction);

                            transaction.Execute(@"INSERT INTO tActivityItemYieldSheet (ActivityItemRefno, ActivityItemSubType,
                                    ProductUnitRefno, LocationMapRefno, ActivityItemYieldSheetQuantity,
                                    ActivityItemYieldSheetBaseQuantity, ActivityItemYieldSheetWeightQuantity,
                                    ActivityItemYieldSheetDescription, ActivityItemYieldSheetQuantityExpected,
                                    ActivityItemYieldSheetBaseQuantityExpected, ActivityItemYieldSheetWeightQuantityExpected)
                                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                                actItemRefno,
                                (int)SysCommon.ActSubTypes.YieldSheetFinished,
                                prodUnitRefno,
                                locMapRefno,
                                Convert.ToDecimal(drow["ActivityItemYieldSheetQuantity"]),
                                Convert.ToDecimal(drow["ActivityItemYieldSheetBaseQuantity"]),
                                Convert.ToDecimal(drow["ActivityItemYieldSheetWeightQuantity"]),
                                drow["ActivityItemYieldSheetDescription"].ToString(),
                                Convert.ToDecimal(drow["ActivityItemYieldSheetQuantityExpected"]),
                                Convert.ToDecimal(drow["ActivityItemYieldSheetBaseQuantityExpected"]),
                                Convert.ToDecimal(drow["ActivityItemYieldSheetWeightQuantityExpected"]));

                            var actItemYieldSheetFKRefno = Convert.ToInt32(drow["ActivityItemYieldSheetFKRefno"]);
                            transaction.Execute(@"INSERT INTO tActivityItemYieldSheetFinished (ActivityItemRefno,
                                    ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)",
                                actItemRefno,
                                actItemYieldSheetFKRefno,
                                actItemYieldSheetFKRefno.ToString());

                            progValue++;
                            if ((progValue & 0x7F) == 0) // update every 128 records
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished cut sheet items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished cut sheet items failed.", false);
                    throw new BaseException("Database exception occurred. Saving finished cut sheet items failed.", ex);
                }
            });
        }

        /// <inheritdoc cref="TableBase.CreateRequest(int)"/>
        protected override async Task<string> CreateRequest(int activityFkRefno)
        {
            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("imclient");
                writer.WriteStartElement("command");
                writer.WriteElementString("action", "REQUEST CUT SHEET FINISHED ITEMS");

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

                writer.WriteElementString("refno", activityFkRefno.ToString());
                writer.WriteEndElement(); // command
                writer.WriteEndElement(); // imclient
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sb.ToString();
        }
    }
}