// <copyright file="TableActivityProductionTicketMaterial.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketMaterial" />.
    /// </summary>
    public class TableActivityProductionTicketMaterial : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketMaterial"/> class.
        /// </summary>
        public TableActivityProductionTicketMaterial()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterial";
            this.CurrentSubType = SysCommon.ActSubTypes.TicketMaterial;
            this.LowerCaseName = "ticket raw material products";
            this.ProperCaseName = "Ticket raw material products";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials.", true);

                var query = "DELETE FROM tActivityProductionMaterial"
                            + " WHERE tActivityProductionMaterial.ActivityRefno IN"
                            + " (SELECT tActivityProduction.ActivityRefno"
                            + " FROM tActivityProduction"
                            + " WHERE tActivityProduction.ActivitySubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials failed.", true);

                throw new Exception("Database exception occurred. Purging ticket materials failed.", ex);
            }
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;
                        var actProductionFKRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                            // To remove time - convert twice
                            var dt = Convert.ToDateTime(drow["ActivityProductionDateTime"]);

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
                            var parametersActivityProduction = new object[]
                            {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial),
                                    whsLocMapRefno,
                                    DateTime.Parse(dt.ToShortDateString()),
                                    Convert.ToString(drow["ActivityProductionDescription"]),
                            };

                            transaction.Execute(queryActivityProduction, parametersActivityProduction);

                            actProductionFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            var queryActivityProductionMaterial = "INSERT INTO tActivityProductionMaterial (" +
                                                                  "ActivityRefno, " +
                                                                  "ActivityFKRefno, " +
                                                                  "ActivityStringFKRefno " +
                                                                  ") VALUES (" +
                                                                  "?, " +
                                                                  "?, " +
                                                                  "?" +
                                                                  ")";
                            var parametersActivityProductionMaterial = new object[]
                            {
                                    actRefno,
                                    actProductionFKRefno,
                                    Convert.ToString(actProductionFKRefno),
                            };

                            transaction.Execute(queryActivityProductionMaterial, parametersActivityProductionMaterial);

                            progValue++;
                            if (progValue % 10 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials failed.", false);
                    throw new Exception("Database exception occurred. Saving ticket materials failed.", ex);
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
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET MATERIAL</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
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
