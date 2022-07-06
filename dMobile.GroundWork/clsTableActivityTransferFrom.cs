// <copyright file="clsTableActivityTransferFrom.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityTransferFrom : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransferFrom"/> class.
        /// </summary>
        public TableActivityTransferFrom()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferFrom";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers from.", true);

                string reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers from complete.", true);
            }
            catch (Exception ex)
            {
                throw new Exception("Seeding transfers from failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfers from.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Transfers from status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers from.", true);

            try
            {
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityTransfer WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom));
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers from complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers from failed.", true);

                throw new Exception("Database exception occurred. Purging transfers from failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override async Task Save(string reqData)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers from.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityTransfer"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int transFKRefno = 0;
                            string transDesc = string.Empty;
                            DateTime transDateTime = new DateTime(1900, 1, 1);
                            DateTime arrivalDateTime = new DateTime(1900, 1, 1);

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                transFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                errMsg = "Trans fk refno " + transFKRefno;
                                transDesc = drow["ActivityTransferDescription"].ToString();
                                errMsg += Environment.NewLine + "Trans desc \'" + transDesc + "\'";
                                transDateTime = Convert.ToDateTime(drow["ActivityTransferDateTime"]);
                                errMsg += Environment.NewLine + "Trans date " + transDateTime.ToShortDateString();
                                arrivalDateTime = Convert.ToDateTime(drow["ActivityTransferArrivalDateTime"]);
                                errMsg += Environment.NewLine + "Trans arrival date " + arrivalDateTime.ToShortDateString();

                                locMapWhsFKRefno = Convert.ToInt32(drow["TransferToLocationMapWarehouseFKRefno"]);
                                errMsg += Environment.NewLine + "Whs fk refno " + locMapWhsFKRefno;

                                List<tLocationMapWarehouseModel> resultsWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapWhsFKRefno));

                                if (resultsWarehouse?.Count > 0)
                                {
                                    whsLocMapRefno = resultsWarehouse[0].LocationMapRefno;
                                }
                                else
                                {
                                    throw new Exception("No results in tLocationMapWarehouse for LocationMapWarehouseFKRefno " + locMapWhsFKRefno + Environment.NewLine + this.FullClassName + Environment.NewLine + errMsg);
                                }

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom));

                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));

                                string query = "INSERT INTO tActivityTransfer (" +
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
                                object[] parameters = new object[]
                                {
                                    Convert.ToInt32(actRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom),
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
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers from complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers from failed.", false);

                    throw new Exception("Database exception occurred. Saving transfers from failed.", ex);
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
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
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