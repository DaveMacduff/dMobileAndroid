namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableActivityAdjustmentItem" />
    /// </summary>
    public class TableActivityAdjustmentItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityAdjustmentItem"/> class.
        /// </summary>
        public TableActivityAdjustmentItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustmentItem";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        public override async Task Request(int actRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items.", true);

                if (actRefno == 0)
                {
                    string query = "SELECT * FROM tActivity WHERE ActivitySubType = ?";

                    List<tActivityModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                    if (results != null && results.Count > 0)
                    {
                        actRefno = results[0].ActivityRefno;
                        reqData = await this.Send(actRefno);

                        if (0 < reqData.Length)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument reqErr = new XmlDocument();
                                reqErr.LoadXml(reqData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.Save(reqData, actRefno);
                        }
                    }
                }
                else
                {
                    reqData = await this.Send(actRefno);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items failed.", false);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding adjustment items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding adjustment items failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                string query = "SELECT COUNT(tActivityItem.ActivityItemRefno) FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, (int)SysCommon.ActSubTypes.Adjustment);

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} adjustment items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Adjustment items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging existing adjustment items.", true);

            try
            {
                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                    + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno "
                    + "FROM tActivityItem "
                    + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityItemAdjustment WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging existing adjustment items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging adjustment items failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actRefno)
        {
            int actAdjFKRefno = 0;
            List<tActivityAdjustmentModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityAdjustmentModel>("SELECT * FROM tActivityAdjustment WHERE ActivityRefno = ?", actRefno);

            if (results?.Count > 0)
            {
                actAdjFKRefno = results[0].ActivityAdjustmentFKRefno;
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("No results in tActivityAdjustment for ActivityRefno " + actRefno);
            }

            return await this.SendComm(this.Create(actAdjFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno"></param>
        protected override async Task Save(string reqData, int actRefno)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemAdjustment"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actItemRefno = 0;
                            int progValue = 0;
                            int actItemAdjFKRefno = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemAdjustment"].Rows)
                            {
                                // TODO: Resource strings
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment), Convert.ToInt32(SysCommon.ActStatus.NoChange), actRefno);

                                actItemRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, errMsg, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);
                                actItemAdjFKRefno = Convert.ToInt32(drow["ActivityItemAdjustmentFKRefno"]);

                                // TODO: Resource strings
                                errMsg += Environment.NewLine + "Item fk refno " + actItemAdjFKRefno;

                                string query = "INSERT INTO tActivityItemAdjustment " +
                                    "(ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemAdjustmentFKRefno, " +
                                    "ActivityItemAdjustmentStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemAdjustmentQuantity, " +
                                    "ActivityItemAdjustmentBaseQuantity, " +
                                    "ActivityItemAdjustmentWeightQuantity, " +
                                    "ActivityItemAdjustmentDescription) " +
                                    "VALUES " +
                                    "(?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?)";

                                List<object> parameters = new List<object>()
                                {
                                    actItemRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                                    prodUnitRefno,
                                    locMapRefno,
                                    actItemAdjFKRefno,
                                    Convert.ToString(actItemAdjFKRefno),
                                    Convert.ToDecimal(drow["ActivityItemAdjustmentQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemAdjustmentBaseQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemAdjustmentWeightQuantity"]),
                                    drow["ActivityItemAdjustmentDescription"].ToString(),
                                };

                                transaction.Execute(query, parameters);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            }
                        }

                        transaction.Commit();

                        DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "saving adjustment items failed.", true);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving adjustment items failed." + Environment.NewLine + errMsg, ex);
                    throw new Exception("Database exception occurred. Saving adjustment items failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actAdjFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int actAdjFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST ADJUSTMENT ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actAdjFKRefno.ToString() + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
