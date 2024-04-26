// <copyright file="TableActivityPhysicalCount.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityPhysicalCount" />.
    /// </summary>
    public class TableActivityPhysicalCount : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPhysicalCount"/> class.
        /// </summary>
        public TableActivityPhysicalCount()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPhysicalCount";
            this.CurrentSubType = SysCommon.ActSubTypes.PhysicalCounts;
            this.LowerCaseName = "physical counts";
            this.ProperCaseName = "Physical counts";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task PurgeData()
        {
            ////var conn = DatabaseConnection.GetInstance().SQLConn;
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
            var subType = Convert.ToInt32(this.CurrentSubType);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    var query = @"
                        DELETE FROM tActivityCount
                            WHERE tActivityCount.ActivityRefno <= ?
                                OR tActivityCount.ActivityFKRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityCount
                            WHERE tActivityCount.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                    transaction.Execute(query);

                    // Regular purge.
                    query = "DELETE FROM tActivityCount WHERE tActivityCount.ActivitySubType = ?";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                    transaction.Execute(query, subType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
                }
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{sqlEx.Message}", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw;
                }
            });
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);
                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityCount"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;
                        var actCountTypeDesc = string.Empty;
                        var countFKRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityCount"].Rows)
                        {
                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                                "SELECT last_insert_rowid() FROM tActivity"));

                            countFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            const string query = @"INSERT INTO tActivityCount (
                                ActivityRefno, 
                                ActivitySubType, 
                                ActivityFKRefno, 
                                ActivityStringFKRefno, 
                                WarehouseLocationMapRefno, 
                                ActivityCountType, 
                                ActivityCountDateTime, 
                                ActivityCountTypeDescription, 
                                ActivityCountDescription
                                ) VALUES (
                                ?, 
                                ?, 
                                ?, 
                                ?, 
                                ?, 
                                ?, 
                                ?, 
                                ?, 
                                ?)";
                            var parameters = new List<object>()
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts),
                                    countFKRefno,
                                    countFKRefno.ToString(),
                                    whsLocMapRefno,
                                    Convert.ToInt32(drow["ActivityCountType"]),
                                };

                            // To remove time - convert twice
                            var dt = Convert.ToDateTime(drow["ActivityCountDateTime"]);

                            // rec.SetDateTime((int)SysCommon.ActCountFields.DateTime, DateTime.Parse(dt.ToShortDateString()));
                            parameters.Add(DateTime.Parse(dt.ToShortDateString()));

                            switch (Convert.ToInt32(drow["ActivityCountType"]))
                            {
                                case (int)SysCommon.ActCountTypeFields.PhysicalCountByWarehouse:

                                    actCountTypeDesc = "Standard Physical Count By Warehouse";
                                    break;

                                case (int)SysCommon.ActCountTypeFields.CycleCountByWarehouse:

                                    actCountTypeDesc = "Cycle Count By Warehouse";
                                    break;

                                case (int)SysCommon.ActCountTypeFields.CycleCountBySection:

                                    actCountTypeDesc = "Cycle Count By Section";
                                    break;

                                case (int)SysCommon.ActCountTypeFields.CycleCountBySlot:

                                    actCountTypeDesc = "Cycle Count By Slot";
                                    break;

                                case (int)SysCommon.ActCountTypeFields.BaseCycleCountByWarehouse:

                                    actCountTypeDesc = "Base Cycle Count By Warehouse";
                                    break;
                            }

                            parameters.Add(actCountTypeDesc);

                            // <wo="14563" comment="receiving strings up to 75 in length when database wants max 60. This is the viewable description when selecting line item." />
                            var incomingDescription = this.ReduceWhitespace(drow["ActivityCountDescription"].ToString());

                            if (incomingDescription.Length > 60)
                            {
                                incomingDescription = incomingDescription.Replace(" ", string.Empty); // remove all spaces
                            }

                            if (incomingDescription.Length > 60)
                            {
                                incomingDescription = incomingDescription.Substring(0, 60); // truncate the end if we need to.
                            }

                            parameters.Add(incomingDescription);

                            transaction.Execute(query, parameters.ToArray());

                            progValue++;
                            if (progValue % 10 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts failed.", true);

                    throw new Exception("Database exception occurred. Saving physical counts failed.", ex);
                }
            });
        }

        /// <summary>
        /// Reduces excess whitespace in a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ReduceWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var prevIsWhitespace = false;
            var charArray = input.ToCharArray();
            for (var i = 0; i < input.Length; ++i)
            {
                if (char.IsWhiteSpace(charArray[i]))
                {
                    if (prevIsWhitespace)
                    {
                        continue;
                    }

                    prevIsWhitespace = true;
                }
                else
                {
                    prevIsWhitespace = false;
                }

                sb.Append(charArray[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse($@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
                <imclient>
                <command>
                <action>REQUEST PHYSICAL COUNTS</action>
                {await SysCommon.GetRequestDefaults(false)}
                </command>
                </imclient>");

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
