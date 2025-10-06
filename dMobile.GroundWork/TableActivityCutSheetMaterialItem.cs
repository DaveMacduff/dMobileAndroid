// <copyright file="TableActivityCutSheetMaterialItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityCutSheetMaterialItem" />.
    /// </summary>
    public class TableActivityCutSheetMaterialItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetMaterialItem"/> class.
        /// </summary>
        public TableActivityCutSheetMaterialItem()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetMaterial;
            this.LowerCaseName = "sheet raw material product";
            this.ProperCaseName = "Sheet raw material product";
        }

        /// <inheritdoc/>
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
                    transaction.Execute(@"DELETE FROM tActivityItemCutSheetMaterial 
                        WHERE tActivityItemCutSheetMaterial.ActivityItemRefno <= 0 
                            OR tActivityItemCutSheetMaterial.ActivityItemFKRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityItemCutSheetMaterial 
                        WHERE tActivityItemCutSheetMaterial.ActivityItemRefno 
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)");
                    transaction.Execute(@"DELETE FROM tActivityItemCutSheet WHERE tActivityItemCutSheet.ActivityItemRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityItemCutSheet 
                        WHERE tActivityItemCutSheet.ActivityItemRefno 
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)");
                    transaction.Execute(@"DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno 
                        NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)");
                    transaction.Execute(@"DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0");

                    // Regular purge.
                    transaction.Execute(@"DELETE FROM tActivityItemBarCodeAssociationXRef 
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno 
                            IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem 
                            WHERE tActivityItem.ActivityItemSubType = ?)", this.CurrentSubType);
                    transaction.Execute(@"DELETE FROM tActivityItemCutSheetMaterial", this.CurrentSubType);
                    transaction.Execute(@"DELETE FROM tActivityItemCutSheet WHERE ActivityItemSubType = ?", this.CurrentSubType);
                    transaction.Execute(@"DELETE FROM tActivityItem WHERE ActivityItemSubType = ?", this.CurrentSubType);

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
            var results = await Conn.QueryAsync<ActivityCutSheetMaterialModel>(
                "SELECT * FROM tActivityCutSheetMaterial WHERE ActivityFKRefno = ?",
                activityFkRefno);

            int actCutSheetFKRefno;
            if (results?.Count > 0)
            {
                actCutSheetFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new BaseException(
                    "No results in tActivityCutSheetMaterial for ActivityRefno " +
                    activityFkRefno.ToString() +
                    "Send");
            }

            return await SendComm(await this.CreateRequest(actCutSheetFKRefno));
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
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sheet material items.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var table = ds.Tables["tActivityItemCutSheet"];
                        var progMax = table.Rows.Count;
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);
                        var actItemRefno = 0;
                        var progValue = 0;
                        var prodUnitRefno = 0;
                        var locMapRefno = 0;
                        var actItemCutSheetFKRefno = 0;
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in table.Rows)
                        {
                            transaction.Execute(@"INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, 
                                    ActivityRefno, ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                (int)this.CurrentSubType,
                                (int)SysCommon.ActStatus.NoChange,
                                refno,
                                0);
                            actItemRefno = transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem");
                            prodUnitRefno = GetProductUnitRefnoFromDataRow(drow, transaction, false);
                            locMapRefno = GetWhsLocMapRefnoFromDataRow(drow, transaction);
                            transaction.Execute(@"INSERT INTO tActivityItemCutSheet (ActivityItemRefno, 
                                    ActivityItemSubType, ProductUnitRefno, LocationMapRefno, 
                                    ActivityItemCutSheetQuantity, ActivityItemCutSheetBaseQuantity, 
                                    ActivityItemCutSheetWeightQuantity, ActivityItemCutSheetDescription, 
                                    ActivityItemCutSheetQuantityExpected, ActivityItemCutSheetBaseQuantityExpected, 
                                    ActivityItemCutSheetWeightQuantityExpected) 
                                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                                actItemRefno,
                                (int)this.CurrentSubType,
                                prodUnitRefno,
                                locMapRefno,
                                Convert.ToDecimal(drow["ActivityItemCutSheetQuantity"]),
                                Convert.ToDecimal(drow["ActivityItemCutSheetBaseQuantity"]),
                                Convert.ToDecimal(drow["ActivityItemCutSheetWeightQuantity"]),
                                drow["ActivityItemCutSheetDescription"].ToString(),
                                Convert.ToDecimal(drow["ActivityItemCutSheetQuantityExpected"]),
                                Convert.ToDecimal(drow["ActivityItemCutSheetBaseQuantityExpected"]),
                                Convert.ToDecimal(drow["ActivityItemCutSheetWeightQuantityExpected"]));

                            actItemCutSheetFKRefno = Convert.ToInt32(drow["ActivityItemCutSheetFKRefno"]);
                            transaction.Execute(@"INSERT INTO tActivityItemCutSheetMaterial (ActivityItemRefno, 
                                    ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)",
                                actItemRefno,
                                actItemCutSheetFKRefno,
                                actItemCutSheetFKRefno.ToString());

                            progValue++;
                            if ((progValue & 0x7F) == 0) // update every 128 records
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sheet material items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sheet material items failed.", false);
                    throw new BaseException("Database exception occurred. Saving sheet material items failed.", ex);
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
                writer.WriteElementString("action", "REQUEST CUT SHEET MATERIAL ITEMS");

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