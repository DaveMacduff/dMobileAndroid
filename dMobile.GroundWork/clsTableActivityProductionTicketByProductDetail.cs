namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketByProductDetail" />
    /// </summary>
    public class TableActivityProductionTicketByProductDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProductDetail"/> class.
        /// </summary>
        public TableActivityProductionTicketByProductDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductDetail";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task Request(int actItemRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details.", true);

                if (actItemRefno == 0)
                {
                    string query = "SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?";
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                    if (results != null && results.Count > 0)
                    {
                        actItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                        reqData = await this.Send(actItemRefno);

                        if (0 < reqData.Length)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument reqErr = new XmlDocument();
                                reqErr.LoadXml(reqData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.Save(reqData, actItemRefno);
                        }
                    }
                }
                else
                {
                    reqData = await this.Send(actItemRefno);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actItemRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket add-product details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding ticket add-product details failed.", ex);
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
                string query = "SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket add-product details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Ticket add-product details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details.", true);

            try
            {
                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef"
                    + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityDetailProductionByProduct"
                    + " WHERE tActivityDetailProductionByProduct.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityDetailExtra"
                        + " WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno"
                        + " FROM tActivityDetail"
                        + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket add-product details failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actItemRefno)
        {
            try
            {
                int actItemFKRefno = 0;

                string query = "SELECT * FROM tActivityItemProductionByProduct WHERE ActivityItemRefno = ?";
                List<tActivityItemProductionByProductModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemProductionByProductModel>(query, actItemRefno);

                if (results != null && results.Count > 0)
                {
                    actItemRefno = results[0].ActivityItemProductionByProductFKRefno;
                }
                else
                {
                    throw new Exception("No results found in tActivityItemProductionByProduct");
                }

                return await this.SendComm(await this.Create(actItemFKRefno));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database Exception occurred. tActivityItemProductionByProductModel Send. ", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemProductionRefno"></param>
        protected override async Task Save(string reqData, int actItemProductionRefno)
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actDetailRefno = 0;
                            int progValue = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemProductionModel> results = transaction.Query<tActivityItemProductionModel>("SELECT * FROM tActivityItemProduction WHERE ActivityItemRefno = ?", actItemProductionRefno);

                            if (results == null || (results != null && results.Count == 0))
                            {
                                // TODO: Resource strings
                                throw new Exception("No results found in tActivityItemProduction for ActivityItemRefno " + actItemProductionRefno + Environment.NewLine + errMsg);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                string queryInsertDetail = "INSERT INTO tActivityDetail " +
                                "(" +
                                "ActivityDetailSubType, " +
                                "ActivityDetailStatus, " +
                                "ActivityItemRefno, " +
                                "LocationMapRefno, " +
                                "ProductUnitRefno, " +
                                "ActivityDetailUniqueSchemeID, " +
                                "ActivityDetailSchemeID, " +
                                "ActivityDetailQuantity, " +
                                "ActivityDetailBaseQuantity, " +
                                "ActivityDetailWeightQuantity, " +
                                "ActivityDetailSingleTradeItemSerialNumber, " +
                                "ActivityDetailLogisticUnitBatchLotNumber, " +
                                "ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                                "ActivityDetailLogisticUnitExpiryUseByDate, " +
                                "ActivityDetailLogisticUnitProductionDate, " +
                                "ActivityDetailLogisticUnitDueDate, " +
                                "ActivityDetailLogisticUnitPackagingDate, " +
                                "ActivityDetailLogisticUnitSkidNumber" +
                                ") " +
                                "VALUES" +
                                "(" +
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
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?" +
                                ")";
                                List<object> parametersInsertDetail = new List<object>();
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                                parametersInsertDetail.Add(0);
                                parametersInsertDetail.Add(actItemProductionRefno);
                                parametersInsertDetail.Add(this.SaveDetailLocation(drow, transaction, false));
                                parametersInsertDetail.Add(results[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                int actDetailProductionFKRefno = Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailProductionFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailProductionByProduct (ActivityDetailRefno, ActivityDetailProductionByProductFKRefno) VALUES (?, ?)", actDetailRefno, actDetailProductionFKRefno);

                                this.SaveCommonDetailVariant(drow, transaction, actDetailRefno, false);
                                this.SaveDetailExtra(drow, transaction, actDetailRefno, false);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket add-product details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actItemFKRefno">The actItemFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Create(int actItemFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemFKRefno + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
