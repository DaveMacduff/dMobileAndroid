// <copyright file="TableActivityCutSheetMaterial.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityCutSheetMaterial" />.
    /// </summary>
    public class TableActivityCutSheetMaterial : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetMaterial"/> class.
        /// </summary>
        public TableActivityCutSheetMaterial()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetMaterial;
            this.LowerCaseName = "sheet raw material products";
            this.ProperCaseName = "Sheet raw material products";
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
                    transaction.Execute(@"DELETE FROM tActivityCutSheetMaterial 
                        WHERE tActivityCutSheetMaterial.ActivityRefno <= 0 OR tActivityCutSheetMaterial.ActivityFKRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityCutSheetMaterial 
                        WHERE tActivityCutSheetMaterial.ActivityRefno 
                            NOT IN (SELECT tActivityCutSheet.ActivityRefno FROM tActivityCutSheet)");
                    transaction.Execute(@"DELETE FROM tActivityCutSheetMaterial 
                        WHERE tActivityCutSheetMaterial.ActivityRefno NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)");
                    transaction.Execute(@"DELETE FROM tActivityCutSheet WHERE tActivityCutSheet.ActivityRefno <= 0");
                    transaction.Execute(@"DELETE FROM tActivityCutSheet 
                        WHERE tActivityCutSheet.ActivityRefno 
                            NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)");
                    transaction.Execute(@"DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0");

                    // Regular purge.
                    transaction.Execute(@"DELETE FROM tActivityCutSheetMaterial", subType);
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
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var table = ds.Tables["tActivityCutSheet"];
                        var progMax = table.Rows.Count;
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);
                        var actRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;
                        var actCutSheetFKRefno = 0;
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in table.Rows)
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                0,
                                (int)SysCommon.ActSubTypes.CutSheetMaterial);
                            actRefno = transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity");
                            whsLocMapRefno = GetWhsLocMapRefnoFromDataRow(drow, transaction);
                            var dt = DateTime.Parse(
                                Convert.ToDateTime(drow["ActivityCutSheetDateTime"]).ToShortDateString(),
                                CultureInfo.CurrentCulture);
                            transaction.Execute(
                                @"INSERT INTO tActivityCutSheet (ActivityRefno, ActivitySubType, WarehouseLocationMapRefno, 
                                    ActivityCutSheetDateTime, ActivityCutSheetDescription) VALUES (?, ?, ?, ?, ?)",
                                actRefno,
                                (int)SysCommon.ActSubTypes.CutSheetMaterial,
                                whsLocMapRefno,
                                dt,
                                drow["ActivityCutSheetDescription"].ToString());

                            actCutSheetFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            transaction.Execute(
                                @"INSERT INTO tActivityCutSheetMaterial (ActivityRefno, ActivityFKRefno, 
                                    ActivityStringFKRefno) VALUES (?, ?, ?)",
                                actRefno,
                                actCutSheetFKRefno,
                                actCutSheetFKRefno.ToString());

                            progValue++;
                            if ((progValue & 0x7F) == 0) // update every 128 records
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cut sheet materials complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sheet materials failed.", false);
                    throw new BaseException("Database exception occurred. Saving cut sheet materials failed.", ex);
                }
            });
        }

        /// <inheritdoc/>
        protected override async Task<string> CreateRequest()
        {
            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("imclient");
                writer.WriteStartElement("command");
                writer.WriteElementString("action", "REQUEST CUT SHEET MATERIAL");

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