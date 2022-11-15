// <copyright file="TableActivityProductionTicketMaterialItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketMaterialItem" />.
    /// </summary>
    public class TableActivityProductionTicketMaterialItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketMaterialItem"/> class.
        /// </summary>
        public TableActivityProductionTicketMaterialItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterialItem";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        public override async Task RequestData(int actRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items.", true);

                string reqData;
                if (actRefno == 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                    if (results?.Count > 0)
                    {
                        foreach (var activityModel in results)
                        {
                            actRefno = Convert.ToInt32(activityModel.ActivityRefno);
                            reqData = await this.SendRequest(actRefno);

                            if (reqData.Length > 0)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    var reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                await this.Save(reqData, actRefno);
                            }
                        }
                    }
                }
                else
                {
                    reqData = await this.SendRequest(actRefno);

                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actRefno);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items failed.", true);

                throw new Exception("Database exception occurred. Seeding ticket material items failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items failed.", true);

                throw new Exception("Seeding ticket material items failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} ticket material items.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Material ticket items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items.", true);

            try
            {
                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                            + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                            + "(SELECT tActivityItem.ActivityItemRefno "
                            + "FROM tActivityItem "
                            + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityItemProductionMaterial "
                    + "WHERE tActivityItemProductionMaterial.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno "
                    + "FROM tActivityItem "
                    + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityItemProduction "
                    + "WHERE tActivityItemProduction.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno "
                    + "FROM tActivityItem "
                    + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items failed.", true);

                throw new Exception("Database exception occurred. Purging ticket material items failed.", ex);
            }
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityProductionMaterialModel>("SELECT * FROM tActivityProductionMaterial WHERE ActivityRefno = ?", actRefno);

            int actProductionFKRefno;
            if (results?.Count > 0)
            {
                actProductionFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception("No results in tActivityProductionMaterial for ActivityRefno " + actRefno + Environment.NewLine + this.FullClassName + "Send");
            }

            return await this.SendComm(await this.CreateRequest(actProductionFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        protected override async Task Save(string reqData, int actRefno)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actItemRefno = 0;
                        var progValue = 0;
                        var prodUnitRefno = 0;
                        var locMapRefno = 0;
                        var actItemProductionFKRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                        {
                            transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial), Convert.ToInt32(SysCommon.ActStatus.NoChange), actRefno);
                            actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);
                            var queryActivityItemProduction = "INSERT INTO tActivityItemProduction (" +
                                                              "ActivityItemRefno, " +
                                                              "ActivityItemSubType, " +
                                                              "ProductUnitRefno, " +
                                                              "LocationMapRefno, " +
                                                              "ActivityItemProductionQuantity, " +
                                                              "ActivityItemProductionBaseQuantity, " +
                                                              "ActivityItemProductionWeightQuantity, " +
                                                              "ActivityItemProductionDescription, " +
                                                              "ActivityItemProductionQuantityExpected, " +
                                                              "ActivityItemProductionBaseQuantityExpected, " +
                                                              "ActivityItemProductionWeightQuantityExpected" +
                                                              ") VALUES (" +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?, " +
                                                              "?" +
                                                              ")";
                            var parametersActivityItemProduction = new object[]
                            {
                                    actItemRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial),
                                    prodUnitRefno,
                                    locMapRefno,
                                    Convert.ToDecimal(drow["ActivityItemProductionQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]),
                                    Convert.ToString(drow["ActivityItemProductionDescription"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"]),
                            };

                            transaction.Execute(queryActivityItemProduction, parametersActivityItemProduction);

                            actItemProductionFKRefno = Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                            transaction.Execute("INSERT INTO tActivityItemProductionMaterial (ActivityItemRefno, ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)", actItemRefno, actItemProductionFKRefno, Convert.ToString(actItemProductionFKRefno));

                            progValue++;
                            if (progValue % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items failed.", false);
                    throw new Exception("Database exception occurred. Saving ticket material items failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="actProductionFKRefno">The actProductionFKRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int actProductionFKRefno)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET MATERIAL ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + actProductionFKRefno + "</refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
