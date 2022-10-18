// <copyright file="TableActivityTransferToItem.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityTransferToItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransferToItem"/> class.
        /// </summary>
        public TableActivityTransferToItem()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferToItem";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task RequestData(int actRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items.", true);

                string reqData;
                if (actRefno == 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items failed.", true);

                throw new Exception("Database exception occurred. Seeding transfer items failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items failed.", true);

                throw new Exception("Seeding transfer items failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items.", true);

            try
            {
                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef"
                            + " WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN"
                            + " (SELECT tActivityItem.ActivityItemRefno"
                            + " FROM tActivityItem"
                            + " WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                query = "DELETE FROM tActivityItemTransfer WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items failed.", true);

                throw new Exception("Database exception occurred. Purging transfer items failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfer items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Transfer items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> SendRequest(int actRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityTransferModel>("SELECT * FROM tActivityTransfer WHERE ActivityRefno = ?", Convert.ToInt32(actRefno));

            int transFKRefno;
            if (results?.Count > 0)
            {
                transFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception(string.Format("Unable to locate the transfer refno {0} in tActivityTransfer.", actRefno));
            }

            return await this.SendComm(await this.CreateRequest(transFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override async Task Save(string reqData, int actRefno)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityItemTransfer"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actItemRefno = 0;
                            var progValue = 0;
                            var prodUnitRefno = 0;
                            var locMapRefno = 0;
                            var locMapWhsFKRefno = 0;
                            var transItemFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemTransfer"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo), 0, Convert.ToInt32(actRefno));
                                actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                                locMapRefno = 0;
                                locMapWhsFKRefno = Convert.ToInt32(drow["TransferFromLocationMapWarehouseFKRefno"]);

                                var resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapWhsFKRefno));

                                if (resultsLocationMapWarehouse?.Count > 0)
                                {
                                    locMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                }

                                if (locMapRefno == 0)
                                {
                                    throw new Exception("Unable to indentify the location map reference number.");
                                }

                                transItemFKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                                var query = "INSERT INTO tActivityItemTransfer (" +
                                            "ActivityItemRefno, " +
                                            "ActivityItemSubType, " +
                                            "ActivityItemFKRefno, " +
                                            "ActivityItemStringFKRefno, " +
                                            "ProductUnitRefno, " +
                                            "TransferFromWarehouseLocationMapRefno, " +
                                            "ActivityItemTransferRequiredQuantity, " +
                                            "ActivityItemTransferQuantity, " +
                                            "ActivityItemTransferBaseQuantity, " +
                                            "ActivityItemTransferWeightQuantity" +
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
                                            "?" +
                                            ")";
                                var parameters = new object[]
                                {
                                    Convert.ToInt32(actItemRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.TransferTo),
                                    Convert.ToInt32(transItemFKRefno),
                                    Convert.ToString(transItemFKRefno),
                                    Convert.ToInt32(prodUnitRefno),
                                    Convert.ToInt32(locMapRefno),
                                    Convert.ToDecimal(drow["ActivityItemTransferRequiredQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemTransferQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemTransferBaseQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemTransferWeightQuantity"]),
                                };

                                transaction.Execute(query, parameters);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items failed.", false);
                    throw new Exception("Database exception occurred. Saving transfer items failed.", ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="transFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int transFKRefno)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TRANSFER TO ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + transFKRefno.ToString() + "</refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}