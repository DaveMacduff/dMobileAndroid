// <copyright file="TableActivityCutSheetMaterialDetail.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models.Items;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityCutSheetMaterialDetail" />.
    /// </summary>
    public class TableActivityCutSheetMaterialDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetMaterialDetail"/> class.
        /// </summary>
        public TableActivityCutSheetMaterialDetail()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetMaterial;
            this.LowerCaseName = "sheet raw material product";
            this.ProperCaseName = "Sheet raw material product";
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
                    transaction.Execute(@"DELETE FROM tActivityDetailProductSchemeVariantXRef 
                        WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno <= 0 
                        OR tActivityDetailProductSchemeVariantRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityDetailProductSchemeVariantXRef 
                        WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno 
                        NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)");
                    transaction.Execute(@"DELETE FROM tActivityDetailExtra WHERE tActivityDetailExtra.ActivityDetailRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityDetailExtra 
                        WHERE tActivityDetailExtra.ActivityDetailRefno 
                        NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)");
                    transaction.Execute(@"DELETE FROM tActivityDetailCutSheetMaterial 
                        WHERE tActivityDetailCutSheetMaterial.ActivityDetailRefno <= 0 
                            OR tActivityDetailCutSheetMaterial.ActivityDetailFKRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityDetailCutSheetMaterial 
                        WHERE tActivityDetailCutSheetMaterial.ActivityDetailRefno 
                            NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)");
                    transaction.Execute(@"DELETE FROM tActivityDetail 
                        WHERE tActivityDetail.ActivityDetailSubType = ? 
                            AND tActivityDetail.ActivityDetailRefno <= 0", subType);
                    transaction.Execute(@"DELETE FROM tActivityDetail 
                        WHERE tActivityDetail.ActivityItemRefno 
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)", subType);

                    // Regular purge.
                    transaction.Execute(@"DELETE FROM tActivityDetailProductSchemeVariantXRef 
                        WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno 
                            IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail 
                            WHERE tActivityDetail.ActivityDetailSubType = ?)", subType);
                    transaction.Execute(@"DELETE FROM tActivityDetailCutSheetMaterial 
                            WHERE tActivityDetailCutSheetMaterial.ActivityDetailRefno 
                                IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail 
                                WHERE tActivityDetail.ActivityDetailSubType = ?)", subType);
                    transaction.Execute(@"DELETE FROM tActivityDetailExtra 
                        WHERE tActivityDetailExtra.ActivityDetailRefno 
                            IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail 
                            WHERE tActivityDetail.ActivityDetailSubType = ?)", subType);
                    transaction.Execute(@"DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?", subType);

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
            var results = await Conn.QueryAsync<ActivityItemCutSheetMaterialModel>(
                "SELECT * FROM tActivityItemCutSheetMaterial WHERE ActivityItemFKRefno = ?",
                activityFkRefno);

            int actItemMaterialFKRefno;
            if (results?.Count > 0)
            {
                actItemMaterialFKRefno = results[0].ActivityItemFKRefno;
            }
            else
            {
                throw new BaseException("drActItemMaterial.Read() failed.");
            }

            return await SendComm(await this.CreateRequest(actItemMaterialFKRefno));
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
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sheet material details.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var table = ds.Tables["tActivityDetailCutSheet"];
                        var progMax = table.Rows.Count;
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);
                        var actDetailRefno = 0;
                        var progValue = 0;
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        var results = transaction.Query<ActivityItemCutSheetModel>(
                            @"SELECT * FROM tActivityItemCutSheet WHERE ActivityItemRefno = ?",
                            refno);
                        if (results == null || results.Count == 0)
                        {
                            throw new BaseException($"No results in tActivityItemCutSheet for ActivityItemRefno");
                        }

                        foreach (DataRow drow in table.Rows)
                        {
                            var parametersInsertDetail = new List<object>
                            {
                                (int)SysCommon.ActSubTypes.CutSheetMaterial,
                                0,
                                refno,
                                Convert.ToInt32(GetReceivedDataLocation(drow, transaction, false)),
                                results[0].ProductUnitRefno
                            };
                            parametersInsertDetail.AddRange(SaveDetail(drow, transaction, false));
                            transaction.Execute(@"INSERT INTO tActivityDetail (ActivityDetailSubType, 
                                    ActivityDetailStatus, ActivityItemRefno, LocationMapRefno, ProductUnitRefno, 
                                    ActivityDetailUniqueSchemeID, ActivityDetailSchemeID, ActivityDetailQuantity, 
                                    ActivityDetailBaseQuantity, ActivityDetailWeightQuantity, ActivityDetailSingleTradeItemSerialNumber, 
                                    ActivityDetailLogisticUnitBatchLotNumber, ActivityDetailLogisticUnitBestBeforeSellByDate, 
                                    ActivityDetailLogisticUnitExpiryUseByDate, ActivityDetailLogisticUnitProductionDate, 
                                    ActivityDetailLogisticUnitDueDate, ActivityDetailLogisticUnitPackagingDate, 
                                    ActivityDetailLogisticUnitSkidNumber) 
                                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                                parametersInsertDetail.ToArray());

                            actDetailRefno = transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail");
                            var actDetailCutSheetFKRefno = Convert.ToInt32(drow["ActivityDetailCutSheetFKRefno"]);
                            transaction.Execute(@"INSERT INTO tActivityDetailCutSheetMaterial (ActivityDetailRefno, 
                                    ActivityDetailFKRefno) VALUES (?, ?)",
                                actDetailRefno,
                                actDetailCutSheetFKRefno);
                            SaveCommonDetailVariant(drow, transaction, actDetailRefno, false);
                            SaveDetailExtra(drow, transaction, actDetailRefno, false);
                            progValue++;
                            if ((progValue & 0x7F) == 0) // update every 128 records
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sheet material details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sheet material details failed.", false);
                    throw new BaseException("Database exception occurred. Saving sheet material details failed.", ex);
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
                writer.WriteElementString("action", "REQUEST CUT SHEET MATERIAL DETAILS");

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

                writer.WriteElementString("itm_refno", activityFkRefno.ToString());
                writer.WriteEndElement(); // command
                writer.WriteEndElement(); // imclient
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sb.ToString();
        }
    }
}