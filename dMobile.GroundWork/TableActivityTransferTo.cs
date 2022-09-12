// <copyright file="TableActivityTransferTo.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityTransferTo : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransferTo"/> class.
        /// </summary>
        public TableActivityTransferTo()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferTo";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers to.", true);

                var reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers to complete.", true);
            }
            catch (Exception ex)
            {
                throw new Exception("Seeding transfers to failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfers to.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Transfers to status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers to.", true);

            try
            {
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityTransfer WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers to complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers to failed.", true);

                throw new Exception("Database exception occurred. Purging transfers to failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override async Task Save(string reqData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers to.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityTransfer"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actRefno = 0;
                            var progValue = 0;
                            var whsLocMapRefno = 0;
                            var locMapWhsFKRefno = 0;
                            var transFKRefno = 0;
                            var transDesc = string.Empty;
                            var transDateTime = new DateTime(1900, 1, 1);
                            var arrivalDateTime = new DateTime(1900, 1, 1);

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                transFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                errMsg = "Trans fk refno " + transFKRefno;
                                transDesc = Convert.ToString(drow["ActivityTransferDescription"]);
                                errMsg += Environment.NewLine + "Trans desc \'" + transDesc + "\'";
                                transDateTime = Convert.ToDateTime(drow["ActivityTransferDateTime"]);
                                errMsg += Environment.NewLine + "Trans date " + transDateTime.ToShortDateString();
                                arrivalDateTime = Convert.ToDateTime(drow["ActivityTransferArrivalDateTime"]);
                                errMsg += Environment.NewLine + "Trans arrival date " + arrivalDateTime.ToShortDateString();
                                locMapWhsFKRefno = Convert.ToInt32(drow["TransferToLocationMapWarehouseFKRefno"]);
                                errMsg += Environment.NewLine + "Whs fk refno " + locMapWhsFKRefno;

                                var results = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                                if (results?.Count > 0)
                                {
                                    whsLocMapRefno = results[0].LocationMapRefno;
                                }
                                else
                                {
                                    throw new Exception("Unable to locate the warehouse fk refno in tLocationMapWarehouse." + Environment.NewLine + errMsg);
                                }

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));

                                var query = "INSERT INTO tActivityTransfer (" +
                                            "ActivityRefno, " +
                                            "ActivitySubType, " +
                                            "ActivityFKRefno, " +
                                            "ActivityStringFKRefno, " +
                                            "TransferToWarehouseLocationMapRefno, " +
                                            "ActivityTransferDescription, " +
                                            "ActivityTransferDateTime, " +
                                            "ActivityTransferArrivalDateTime" +
                                            ") VALUES (" +
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
                                    Convert.ToInt32(actRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.TransferTo),
                                    Convert.ToInt32(transFKRefno),
                                    Convert.ToString(transFKRefno),
                                    Convert.ToInt32(whsLocMapRefno),
                                    transDesc,
                                    Convert.ToDateTime(transDateTime),
                                    Convert.ToDateTime(arrivalDateTime),
                                };

                                transaction.Execute(query, parameters);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers to complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers to failed.", false);

                    throw new Exception("Database exception occurred. Saving transfers to failed.", ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TRANSFERS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>" + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}