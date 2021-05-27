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
    /// Defines the <see cref="TableActivityAdjustmentDetail" />
    /// </summary>
    public class TableActivityAdjustmentDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityAdjustmentDetail"/> class.
        /// </summary>
        public TableActivityAdjustmentDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustmentDetail";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task Request(int actItemRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details.", true);

            if (actItemRefno == 0)
            {
                List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                if (results != null && results.Count > 0)
                {
                    foreach (tActivityItemModel activityItem in results)
                    {
                        actItemRefno = Convert.ToInt32(activityItem.ActivityItemRefno);
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
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details complete.", true);
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
                string query = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} adjustment details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Adjustment details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityDetailAdjustment "
                        + "WHERE tActivityDetailAdjustment.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging adjustment details failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemAdjRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actItemAdjRefno)
        {
            string reqData = string.Empty;
            int actItemAdjFKRefno = 0;

            string query = "SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = ?";
            List<tActivityItemAdjustmentModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemAdjustmentModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

            if (results != null && results.Count > 0)
            {
                actItemAdjFKRefno = results[0].ActivityItemAdjustmentFKRefno;
            }
            else
            {
                throw new Exception(string.Format("Unable to locate item adjustment {0} in tActivityItemAdjustment", actItemAdjRefno));
            }

            return await this.SendComm(await this.Create(actItemAdjFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemAdjRefno"></param>
        protected override async Task Save(string reqData, int actItemAdjRefno)
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailAdjustment"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailAdjFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemAdjustmentModel> adjustmentItemResults = transaction.Query<tActivityItemAdjustmentModel>("SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = ?", actItemAdjRefno);

                            if (adjustmentItemResults == null || adjustmentItemResults.Count == 0)
                            {
                                throw new Exception("drItemAdj.Read() failed.");
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailAdjustment"].Rows)
                            {
                                // TODO: Resource strings
                                errMsg = "Arguments:";

                                List<object> parametersInsertDetail = new List<object>();
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

                                bool useFNo = false;
                                int locMapRefno = 0;
                                int locMapWhsFKRefno = 0;
                                int locMapSectFKRefno = 0;
                                int locMapSlotFKRefno = 0;

                                if (useFNo)
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["f07"]);
                                    locMapSectFKRefno = Convert.ToInt32(drow["f08"]);
                                    locMapSlotFKRefno = Convert.ToInt32(drow["f09"]);
                                }
                                else
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                                    locMapSectFKRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                                    locMapSlotFKRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
                                }

                                if (locMapRefno == 0)
                                {
                                    List<tLocationMapSlotModel> resultSlots = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapSlotFKRefno);

                                    if (resultSlots?.Count > 0)
                                    {
                                        locMapRefno = resultSlots[0].LocationMapRefno;
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    List<tLocationMapSectionModel> resultSections = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapSectFKRefno);

                                    if (resultSections?.Count > 0)
                                    {
                                        locMapRefno = resultSections[0].LocationMapRefno;
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    List<tLocationMapWarehouseModel> resultWarehouses = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                                    if (resultWarehouses?.Count > 0)
                                    {
                                        locMapRefno = resultWarehouses[0].LocationMapRefno;
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    throw new Exception(string.Format("Unable to identify the location map refno. {0}Whs FK Refno {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}",
                                        Environment.NewLine,
                                        locMapWhsFKRefno,
                                        Environment.NewLine,
                                        locMapSectFKRefno,
                                        Environment.NewLine,
                                        locMapSlotFKRefno));
                                }

                                string emptyDate = "01-01-1900";

                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));
                                parametersInsertDetail.Add(Convert.ToInt32(0));
                                parametersInsertDetail.Add(actItemAdjRefno);
                                parametersInsertDetail.Add(locMapRefno);
                                parametersInsertDetail.Add(Convert.ToInt32(adjustmentItemResults[0].ProductUnitRefno));

                                if (useFNo)
                                {
                                    parametersInsertDetail.Add(drow["f05"].ToString());
                                    parametersInsertDetail.Add(drow["f06"].ToString());
                                    parametersInsertDetail.Add(Convert.ToDecimal(drow["f02"]));
                                    parametersInsertDetail.Add(Convert.ToDecimal(drow["f03"]));
                                    parametersInsertDetail.Add(Convert.ToDecimal(drow["f04"]));
                                    parametersInsertDetail.Add(drow["f10"].ToString());
                                    parametersInsertDetail.Add(drow["f11"].ToString());
                                    parametersInsertDetail.Add(drow["f18"].ToString());

                                    if (string.IsNullOrEmpty(drow["f13"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["f13"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["f14"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["f14"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["f12"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["f12"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["f15"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["f15"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["f16"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["f16"]));
                                    }
                                }
                                else
                                {
                                    parametersInsertDetail.Add(Convert.ToInt32(0));
                                    parametersInsertDetail.Add(drow["ActivityDetailUniqueSchemeID"].ToString());
                                    parametersInsertDetail.Add(drow["ActivityDetailSchemeID"].ToString());
                                    parametersInsertDetail.Add(drow["ActivityDetailQuantity"].ToString());
                                    parametersInsertDetail.Add(Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]));
                                    parametersInsertDetail.Add(Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]));
                                    parametersInsertDetail.Add(drow["ActivityDetailSingleTradeItemSerialNumber"].ToString());
                                    parametersInsertDetail.Add(drow["ActivityDetailLogisticUnitBatchLotNumber"].ToString());
                                    parametersInsertDetail.Add(drow["ActivityDetailLogisticUnitSkidCode"].ToString());

                                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitDueDate"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));
                                    }

                                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitPackagingDate"].ToString()))
                                    {
                                        parametersInsertDetail.Add(DateTime.Parse(emptyDate));
                                    }
                                    else
                                    {
                                        parametersInsertDetail.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));
                                    }
                                }

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailAdjFKRefno = Convert.ToInt32(drow["ActivityDetailAdjustmentFKRefno"]);

                                // TODO: Resource strings
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailAdjFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailAdjustment (ActivityDetailRefno, ActivityDetailAdjustmentFKRefno) VALUES (?, ?)", actDetailRefno, actDetailAdjFKRefno);

                                int schemeVariantNo = 0;
                                bool isNeedToSave = false;

                                if (useFNo)
                                {
                                    schemeVariantNo = Convert.ToInt32(drow["f17"]);
                                }
                                else
                                {
                                    schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                                }

                                if (0 < schemeVariantNo)
                                {
                                    List<tProductSchemeVariantModel> resultsProductSchemeVariant = transaction.Query<tProductSchemeVariantModel>("SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", DateTime.Parse(emptyDate));

                                    if (resultsProductSchemeVariant?.Count > 0)
                                    {
                                        transaction.Execute("INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)", actDetailRefno, resultsProductSchemeVariant[0].ProductSchemeVariantRefno);
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drSchemeVar.Read() failed.");
                                    }
                                }

                                List<object> parametersExtra = new List<object>();
                                string queryExtra = "INSERT INTO tActivityDetailExtra (" +
                                                        "ActivityDetailRefno, " +
                                                        "ActivityDetailSubType, " +
                                                        "ActivityDetailExtra1Text, " +
                                                        "ActivityDetailExtra2Text, " +
                                                        "ActivityDetailExtra3Text, " +
                                                        "ActivityDetailExtra4Text, " +
                                                        "ActivityDetailExtra5Date" +
                                                        ") VALUES (" +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?)";
                                parametersExtra .Add(actDetailRefno);
                                parametersExtra.Add(Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                                if (useFNo)
                                {
                                    parametersExtra.Add(drow["f19"].ToString());
                                    parametersExtra.Add(drow["f20"].ToString());
                                    parametersExtra.Add(drow["f21"].ToString());
                                    parametersExtra.Add(drow["f22"].ToString());
                                    parametersExtra.Add(drow["f23"].ToString());
                                }
                                else
                                {
                                    parametersExtra.Add(drow["ActivityDetailExtra1"].ToString());
                                    parametersExtra.Add(drow["ActivityDetailExtra2"].ToString());
                                    parametersExtra.Add(drow["ActivityDetailExtra3"].ToString());
                                    parametersExtra.Add(drow["ActivityDetailExtra4"].ToString());
                                    parametersExtra.Add(drow["ActivityDetailExtra5"].ToString());
                                }

                                transaction.Execute(queryExtra, parametersExtra.ToArray());

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

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details failed.", true);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving adjustment details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

    /// <summary>
    /// The Create
    /// </summary>
    /// <param name="actItemAdjFKRefno"></param>
    /// <returns></returns>
    protected override async Task<string> Create(int actItemAdjFKRefno)
    {
        StringBuilder xmlString = new StringBuilder();
        XmlWriter xml = XmlWriter.Create(xmlString);

        XDocument request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
            + "<imclient>"
            + "<command>"
            + "<action>REQUEST ADJUSTMENT DETAILS</action>"
            + SysCommon.GetRequestDefaults(false)
            + ("<itm_refno>" + actItemAdjFKRefno + "</itm_refno>")
            + "</command>"
            + "</imclient>");

        request.Save(xml);
        xml.Flush();
        xml.Close();

        return xmlString.ToString();
    }
}
}
