namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketMaterialDetail" />
    /// </summary>
    public class TableActivityProductionTicketMaterialDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketMaterialDetail"/> class.
        /// </summary>
        public TableActivityProductionTicketMaterialDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterialDetail";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material details.", true);

                if (actItemRefno == 0)
                {
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                    if (results?.Count > 0)
                    {
                        foreach (tActivityItemModel activityItemModel in results)
                        {
                            actItemRefno = Convert.ToInt32(activityItemModel.ActivityItemRefno);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket materials details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding ticket materials details failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket material details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Material ticket details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material details.", true);

            try
            {
                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef"
                    + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityDetailProductionMaterial"
                    + " WHERE tActivityDetailProductionMaterial.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityDetailExtra"
                    + " WHERE tActivityDetailExtra.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket material details failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemRefno">The actItemRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actItemRefno)
        {
            int actItemMaterialFKRefno = 0;
            List<tActivityItemProductionMaterialModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemProductionMaterialModel>("SELECT * FROM tActivityItemProductionMaterial WHERE ActivityItemRefno = ?", actItemRefno);

            if (results?.Count > 0)
            {
                actItemMaterialFKRefno = results[0].ActivityItemProductionMaterialFKRefno;
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drActItemMaterial.Read() failed.");
            }

            return await this.SendComm(await this.Create(actItemMaterialFKRefno));
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actDetailRefno = 0;
                            int progValue = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemProductionModel> results = transaction.Query<tActivityItemProductionModel>("SELECT * FROM tActivityItemProduction WHERE ActivityItemRefno = ?", actItemProductionRefno);

                            if (results == null || results?.Count == 0)
                            {
                                throw new Exception("No results in tActivityItemProduction for ActivityItemRefno " + actItemProductionRefno + Environment.NewLine + errMsg);
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
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
                                parametersInsertDetail.Add(Convert.ToInt32(0));
                                parametersInsertDetail.Add(actItemProductionRefno);
                                parametersInsertDetail.Add(Convert.ToInt32(this.SaveDetailLocation(drow, transaction, false)));
                                parametersInsertDetail.Add(results[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail);

                                actDetailRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                int actDetailProductionFKRefno = Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailProductionFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailProductionMaterial (ActivityDetailRefno, ActivityDetailProductionMaterialFKRefno) VALUES (?, ?)", actDetailRefno, actDetailProductionFKRefno);

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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material details failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket material details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actItemProductionFKRefno">The actItemProductionFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Create(int actItemProductionFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET MATERIAL DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemProductionFKRefno + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
