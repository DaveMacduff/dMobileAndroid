// <copyright file="TableActivityProductionTicketFinished.cs" company="InterSect Business Systems Inc">
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
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketFinished" />.
    /// </summary>
    public class TableActivityProductionTicketFinished : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketFinished"/> class.
        /// </summary>
        public TableActivityProductionTicketFinished()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinished";
            this.CurrentSubType = SysCommon.ActSubTypes.TicketFinished;
            this.LowerCaseName = "ticket finished products";
            this.ProperCaseName = "Ticket finished products";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
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
                        DELETE FROM tActivityProductionFinished
                            WHERE tActivityProductionFinished.ActivityRefno <= ?
                                OR tActivityProductionFinished.ActivityFKRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityProductionFinished
                            WHERE tActivityProductionFinished.ActivityRefno
                                NOT IN (SELECT tActivityProduction.ActivityRefno FROM tActivityProduction)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityProductionFinished
                            WHERE tActivityProductionFinished.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityProduction
                            WHERE tActivityProduction.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityProduction
                            WHERE tActivityProduction.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                    transaction.Execute(query);

                    // Regular purge.
                    query = "DELETE FROM tActivityProductionFinished";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = ?";
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                            var queryActivityProduction = "INSERT INTO tActivityProduction (" +
                                                          "ActivityRefno, " +
                                                          "ActivitySubType, " +
                                                          "WarehouseLocationMapRefno, " +
                                                          "ActivityProductionDateTime, " +
                                                          "ActivityProductionDescription" +
                                                          ") VALUES (" +
                                                          "?, " +
                                                          "?, " +
                                                          "?, " +
                                                          "?, " +
                                                          "?" +
                                                          ")";
                            var parametersActivityProduction = new List<object>();
                            parametersActivityProduction.Add(actRefno);
                            parametersActivityProduction.Add(Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);
                            parametersActivityProduction.Add(whsLocMapRefno);
                            var dt = DateTime.Parse(Convert.ToDateTime(drow["ActivityProductionDateTime"]).ToShortDateString());
                            parametersActivityProduction.Add(dt);
                            parametersActivityProduction.Add(drow["ActivityProductionDescription"].ToString());
                            transaction.Execute(queryActivityProduction, parametersActivityProduction.ToArray());

                            var actProductionFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            var queryActivityProductionFinished = "INSERT INTO tActivityProductionFinished (" +
                                                                  "ActivityRefno, " +
                                                                  "ActivityFKRefno, " +
                                                                  "ActivityStringFKRefno, " +
                                                                  "ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber" +
                                                                  ") VALUES (" +
                                                                  "?, " +
                                                                  "?, " +
                                                                  "?, " +
                                                                  "?" +
                                                                  ")";
                            var parametersActivityProductionFinished = new object[]
                            {
                                    actRefno,
                                    actProductionFKRefno,
                                    actProductionFKRefno.ToString(),
                                    string.Empty,
                            };

                            transaction.Execute(queryActivityProductionFinished, parametersActivityProductionFinished);

                            progValue++;
                            if (progValue % 10 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets failed.", false);

                    throw new Exception("Database exception occurred. Saving finished tickets failed.", ex);
                }
            });
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

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET FINISHED</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno)
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

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
