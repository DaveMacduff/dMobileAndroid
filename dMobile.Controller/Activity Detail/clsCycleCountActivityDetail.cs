namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;

    public class CycleCountActDetail : ActDetail
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItem"></param>
        public CycleCountActDetail(ActItem actItem)
            : base(actItem)
        {
            this.FullClassName = typeof(CycleCountActDetail).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override void InsertSkid(SysSkidInfo skidInfo)
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>")
                + "</params>");

            string reqData = SysCommon.SubmitComm(this.CreateSubmitXML(request));

            if (reqData.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(reqData);

                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }
                else
                {
                    this.IsUpdateView = true;
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.NewItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                    this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
                    this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
                    this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);

                    if (this.NewItemFKRefno != this.ActItem.FKRefno)
                    {
                        using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.CommandText = "UPDATE tActivityItemCount "
                                + "SET ActivityItemCountStringFKRefno = \'" + this.NewItemFKRefno + "\', "
                                + "ActivityItemCountFKRefno = " + this.NewItemFKRefno
                                + " WHERE tActivityItemCount.ActivityItemRefno = " + ActItem.Refno;
                            cmndItem.ExecuteNonQuery();

                            this.ActItem.FKRefno = this.NewItemFKRefno;
                            string oldSort = this.ActItem.DataViewItems.Sort;
                            this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            int rowIdx = this.ActItem.DataViewItems.Find(new object[] { this.ActItem.Refno });
                            if (rowIdx != -1)
                            {
                                DataRowView rowView = this.ActItem.DataViewItems[rowIdx];
                                rowView.BeginEdit();
                                rowView["ActivityItemCountFKRefno"] = this.NewItemFKRefno;
                                rowView["ActivityItemCountStringFKRefno"] = this.NewItemFKRefno.ToString();
                                rowView.EndEdit();
                            }

                            this.ActItem.DataViewItems.Sort = oldSort;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override void DeleteSkid(SysSkidInfo skidInfo)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete = this.CreateDeleteXML(
                    XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

                string reqData = SysCommon.SubmitComm(xmlDelete);
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;
                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail);
                        }
                        else
                        {
                            throw new Exception(errorDetail);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Insert(XDocument data)
        {
            this.FillActDetailObjFromXML(data);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                int actDetailRefno = this.InsertDetailDisconnected();

                using (SQLiteCommand cmndDetailAdj = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndDetailAdj.CommandText = "INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailCountFKRefno) VALUES (@activityDetailRefno, @activityDetailCountFKRefno)";
                    cmndDetailAdj.CommandType = CommandType.Text;
                    //cmndDetailAdj.IndexName = "UQ_ActivityDetailCountOnActivityDetailCountFKRefno";
                    cmndDetailAdj.Parameters.Add(new SQLiteParameter("@activityDetailRefno", Convert.ToInt32(actDetailRefno)));
                    cmndDetailAdj.Parameters.Add(new SQLiteParameter("@activityDetailCountFKRefno", Convert.ToInt32(this.FKRefno)));

                    cmndDetailAdj.ExecuteNonQuery();

                    //recDetail.SetInt32((int)SysCommon.ActDetailAdjFields.ActDetailRefno, actDetailRefno);
                    //recDetail.SetInt32((int)SysCommon.ActDetailAdjFields.FKRefno, this.FKRefno);
                    //rsDetail.Insert(recDetail);

                    this.IsUpdateView = true;
                }
            }
            else
            {
                if (this.InsertDetail(this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"))))
                {
                    bool newDetail = true;

                    // Check for a FK already existing with that value.
                    // if it does, add or subtract from it and then return false for new detail
                    int x = -1;

                    for (int i = 0; i < ActItem.DataViewItems.Count; i++)
                    {
                        if ((int)ActItem.DataViewItems[i]["ActivityItemCountFKRefno"] == ActItem.FKRefno)
                        {
                            x = i;
                            break;
                        }
                    }

                    if (x != -1)
                    {
                        // There already exists a FK.
                        // Delete the newline and add the details to the existing line
                        newDetail = false;
                        ActItem.DataViewItems[x]["ActivityItemCountQuantity"] = (decimal)ActItem.DataViewItems[x]["ActivityItemCountQuantity"] + Quantity;
                        ActItem.DataViewItems[x]["ActivityItemCountBaseQuantity"] = (decimal)ActItem.DataViewItems[x]["ActivityItemCountBaseQuantity"] + BaseQuantity;
                        ActItem.DataViewItems[x]["ActivityItemCountWeightQuantity"] = (decimal)ActItem.DataViewItems[x]["ActivityItemCountWeightQuantity"] + WeightQuantity;
                        ActItem.DataViewItems[ActItem.DataViewItems.Count - 1].Delete();
                    }

                    if (newDetail)
                    {


                        using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.CommandText = "UPDATE tActivityItemCount "
                                + "SET ActivityItemCountStringFKRefno = \'" + this.NewItemFKRefno + "\',"
                                + "ActivityItemCountFKRefno = " + this.NewItemFKRefno
                                + " WHERE tActivityItemCount.ActivityItemRefno = " + this.ActItem.Refno; //<WO ID="14711" comment="Changed Refno to FKRefno. They are the same when no barcode modifier present. Opposite +/- when modifier exists." />
                            cmndItem.ExecuteNonQuery();
                        }

                        string oldSort = this.ActItem.DataViewItems.Sort;
                        this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        int rowIdx = this.ActItem.DataViewItems.Find(new object[] { this.ActItem.Refno });
                        if (rowIdx != -1)
                        {
                            DataRowView rowView = this.ActItem.DataViewItems[rowIdx];
                            rowView.BeginEdit();
                            rowView["ActivityItemCountFKRefno"] = this.NewItemFKRefno;
                            rowView["ActivityItemCountStringFKRefno"] = this.NewItemFKRefno.ToString();
                            rowView.EndEdit();
                        }

                        this.ActItem.DataViewItems.Sort = oldSort;
                    }
                }
            }

            this.InsertUpdateDataViewDetail("ActivityDetailCountFKRefno", "ActivityDetailCountFKRefno");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete = this.CreateDeleteXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                string reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;

                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail);
                        }
                        else
                        {
                            throw new Exception(errorDetail);
                        }
                    }
                }
            }

            // delete row from DataView
            string oldSort = this.DataViewDetails.Sort;
            this.DataViewDetails.Sort = "ActivityDetailCountFKRefno";
            int rowIdx = this.DataViewDetails.Find(this.FKRefno);

            if (rowIdx >= 0)
            {
                this.DataViewDetails.Delete(rowIdx);
            }

            this.DataViewDetails.Sort = oldSort;

            // Delete from database
            using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndPurge.CommandType = CommandType.Text;

                cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();

                cmndPurge.CommandText = "DELETE FROM tActivityDetailCount WHERE tActivityDetailCount.ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();

                cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra WHERE tActivityDetailExtra.ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();

                cmndPurge.CommandText = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="data"></param>
        public override void Submit(int rowIdx, XDocument data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                xmlDoc.LoadXml(reqData);

                // Check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }
                else
                {
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

                    if (this.DataViewDetails != null)
                    {
                        this.DataViewDetails[rowIdx].BeginEdit();
                        this.DataViewDetails[rowIdx]["ActivityDetailCountFKRefno"] = this.FKRefno;
                        this.DataViewDetails[rowIdx]["ActivityDetailStatus"] = SysCommon.ActStatus.NoChange;
                        this.DataViewDetails[rowIdx].EndEdit();
                    }

                    using (SQLiteCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndDetail.CommandType = CommandType.Text;
                        cmndDetail.CommandText = "UPDATE tActivityDetail "
                            + "SET tActivityDetail.ActivityDetailStatus = " + (int)SysCommon.ActStatus.NoChange
                            + "WHERE tActivityDetail.ActivityDetailRefno = " + this.Refno;
                        cmndDetail.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public override ActDetail GetInfo(int idx, bool isAdd)
        {
            base.GetInfo(idx, isAdd);

            if (!isAdd)
            {
                this.FKRefno = Convert.ToInt32(this.DataViewDetails[idx]["ActivityDetailCountFKRefno"]);
                this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)this.DataViewDetails[idx]["LocationMapRefno"]);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Refresh()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityCycleCountDetail tableCountDet = new TableActivityCycleCountDetail();
                tableCountDet.Purge();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableCountDet.RegisterDataAccessListenerProgressOut((CycleCountActDetail)this);
                tableCountDet.Request(this.ActItem.Refno);
                tableCountDet.UnregisterDataAccessListenerProgressOut((CycleCountActDetail)this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefno(int idx)
        {
            return (int)this.DataViewDetails[idx]["ActivityDetailRefno"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public override int GetFKRefno(int selectedIndex)
        {
            return (int)DataViewDetails[selectedIndex]["ActivityDetailCountFKRefno"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailCountFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSlot.LocationMapSlotCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("\'Slot\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailCountFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("\'Section\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailCountFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("\'Whs\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Text ");
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }
            else
            {
                // This large query failed on large datasets like Prairie most of the time. Out of memory exception.
                // The queries are identical other than Slot, Section, Warehouse location labels
                // Run 3 seperate queries and then join them afterwards.
                DataTable slotTable = GetSlotDetails(ActItem.Refno);
                DataTable sectionTable = GetSectionDetails(ActItem.Refno);
                DataTable warehouseTable = GetWarehouseDetails(ActItem.Refno);

                slotTable.Merge(sectionTable);
                slotTable.Merge(warehouseTable);


                DataView view = slotTable.DefaultView;
                view.Sort = "ActivityDetailRefno ASC";
                DataViewDetails = view;
                DataViewDetails.Table.TableName = MainGridMappingName;


                sectionTable.Dispose();
                warehouseTable.Dispose();
                slotTable.Dispose();

                return view;
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            using (DataTable ds = new DataTable())
            {
                da.Fill(ds);
                this.DataViewDetails = ds.DefaultView;
                this.DataViewDetails.Table.TableName = this.MainGridMappingName;
            }

            return this.DataViewDetails;
        }

        /// <summary>
        /// Gets all slot details for the refno.
        /// </summary>
        /// <returns></returns>
        private DataTable GetSlotDetails(int refno)
        {
            string slotQuery = "SELECT tActivityDetail.ActivityDetailRefno, " +
            "tActivityDetail.LocationMapRefno, " +
            "tActivityDetailCount.ActivityDetailCountFKRefno, " +
            "tUnit.UnitCode, " +
            "aUnitWeight.UnitCode AS WtUnitCode, " +
            "aUnitBase.UnitCode AS BaseUnitCode, " +
            "tProduct.ProductCode, " +
            "tProduct.ProductRefno, " +
            "tProduct.ProductDescription, " +
            "tProductUnit.ProductUnitBC, " +
            "tProductUnit.ProductUnitIsBC, " +
            "tProductUnit.ProductUnitSizeDescription, " +
            "tActivityDetail.ActivityDetailQuantity, " +
            "tActivityDetail.ActivityDetailBaseQuantity, " +
            "tActivityDetail.ActivityDetailWeightQuantity, " +
            "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate, " +
            "tActivityDetail.ActivityDetailSchemeID, " +
            "tActivityDetail.ActivityDetailUniqueSchemeID, " +
            "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber, " +
            "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber, " +
            "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate, " +
            "tActivityDetail.ActivityDetailLogisticUnitProductionDate, " +
            "tActivityDetail.ActivityDetailLogisticUnitSkidNumber, " +
            "tLocationMapSlot.LocationMapSlotCode AS LocationCode, " +
            "CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName, " +
            "\'Slot\' AS Location, " +
            "tProductSchemeVariant.ProductSchemeVariantCode, " +
            "tProductSchemeVariant.ProductSchemeVariantNo, " +
            "tProductSchemeVariant.ProductSchemeVariantDescription, " +
            "tActivityDetail.ActivityDetailStatus, " +
            "tActivityDetailExtra.ActivityDetailExtra1Text, " +
            "tActivityDetailExtra.ActivityDetailExtra2Text, " +
            "tActivityDetailExtra.ActivityDetailExtra3Text, " +
            "tActivityDetailExtra.ActivityDetailExtra4Text, " +
            "tActivityDetailExtra.ActivityDetailExtra5Text  " +
            "FROM tActivityDetailCount  " +
            "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
            "= tActivityDetailCount.ActivityDetailRefno  " +
            "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
            "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
            "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
            "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
            "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
            "INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno  " +
            "= tActivityDetail.LocationMapRefno  " +
            "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
            "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
            "= tActivityDetailCount.ActivityDetailRefno  " +
            "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
            "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
            "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
            "= tActivityDetail.ActivityDetailRefno  " +
           "WHERE tActivityDetail.ActivityItemRefno = " + refno +
           " AND tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;

            DataTable table = new DataTable();

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(slotQuery, DatabaseConnection.GetInstance().SQLConn))
            {
                da.Fill(table);
            }

            return table;
        }

        private DataTable GetSectionDetails(int refno)
        {
            string sectionQuery = "SELECT tActivityDetail.ActivityDetailRefno, " +
             "tActivityDetail.LocationMapRefno, " +
             "tActivityDetailCount.ActivityDetailCountFKRefno, " +
             "tUnit.UnitCode, " +
             "aUnitWeight.UnitCode AS WtUnitCode, " +
             "aUnitBase.UnitCode AS BaseUnitCode, " +
             "tProduct.ProductCode, " +
             "tProduct.ProductRefno, " +
             "tProduct.ProductDescription, " +
             "tProductUnit.ProductUnitBC, " +
             "tProductUnit.ProductUnitIsBC, " +
             "tProductUnit.ProductUnitSizeDescription, " +
             "tActivityDetail.ActivityDetailQuantity, " +
             "tActivityDetail.ActivityDetailBaseQuantity, " +
             "tActivityDetail.ActivityDetailWeightQuantity, " +
             "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate, " +
             "tActivityDetail.ActivityDetailSchemeID, " +
             "tActivityDetail.ActivityDetailUniqueSchemeID, " +
             "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber, " +
             "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber, " +
             "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate, " +
             "tActivityDetail.ActivityDetailLogisticUnitProductionDate, " +
             "tActivityDetail.ActivityDetailLogisticUnitSkidNumber, " +
             "tLocationMapSection.LocationMapSectionCode AS LocationCode, " +
             "CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName, " +
             "\'Section\' AS Location, " +
             "tProductSchemeVariant.ProductSchemeVariantCode, " +
             "tProductSchemeVariant.ProductSchemeVariantNo, " +
             "tProductSchemeVariant.ProductSchemeVariantDescription, " +
             "tActivityDetail.ActivityDetailStatus, " +
             "tActivityDetailExtra.ActivityDetailExtra1Text, " +
             "tActivityDetailExtra.ActivityDetailExtra2Text, " +
             "tActivityDetailExtra.ActivityDetailExtra3Text, " +
             "tActivityDetailExtra.ActivityDetailExtra4Text, " +
             "tActivityDetailExtra.ActivityDetailExtra5Text  " +
             "FROM tActivityDetailCount  " +
             "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
             "= tActivityDetailCount.ActivityDetailRefno  " +
             "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
             "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
             "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
             "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
             "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
             "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno  " +
             "= tActivityDetail.LocationMapRefno  " +
             "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
             "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
             "= tActivityDetailCount.ActivityDetailRefno  " +
             "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
             "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
             "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
             "= tActivityDetail.ActivityDetailRefno  " +
            "WHERE tActivityDetail.ActivityItemRefno = " + refno +
            " AND tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;

            DataTable table = new DataTable();
            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sectionQuery, DatabaseConnection.GetInstance().SQLConn))
            {
                da.Fill(table);
            }

            return table;
        }

        private DataTable GetWarehouseDetails(int refno)
        {
            string warehouseQuery =
                "SELECT tActivityDetail.ActivityDetailRefno, " +
                 "tActivityDetail.LocationMapRefno, " +
                 "tActivityDetailCount.ActivityDetailCountFKRefno, " +
                 "tUnit.UnitCode, " +
                 "aUnitWeight.UnitCode AS WtUnitCode, " +
                 "aUnitBase.UnitCode AS BaseUnitCode, " +
                 "tProduct.ProductCode, " +
                 "tProduct.ProductRefno, " +
                 "tProduct.ProductDescription, " +
                 "tProductUnit.ProductUnitBC, " +
                 "tProductUnit.ProductUnitIsBC, " +
                 "tProductUnit.ProductUnitSizeDescription, " +
                 "tActivityDetail.ActivityDetailQuantity, " +
                 "tActivityDetail.ActivityDetailBaseQuantity, " +
                 "tActivityDetail.ActivityDetailWeightQuantity, " +
                 "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                 "tActivityDetail.ActivityDetailSchemeID, " +
                 "tActivityDetail.ActivityDetailUniqueSchemeID, " +
                 "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber, " +
                 "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber, " +
                 "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate, " +
                 "tActivityDetail.ActivityDetailLogisticUnitProductionDate, " +
                 "tActivityDetail.ActivityDetailLogisticUnitSkidNumber, " +
                 "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                 "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                 "\'Whs\' AS Location, " +
                 "tProductSchemeVariant.ProductSchemeVariantCode, " +
                 "tProductSchemeVariant.ProductSchemeVariantNo, " +
                 "tProductSchemeVariant.ProductSchemeVariantDescription, " +
                 "tActivityDetail.ActivityDetailStatus, " +
                 "tActivityDetailExtra.ActivityDetailExtra1Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra2Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra3Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra4Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra5Text  " +
                 "FROM tActivityDetailCount  " +
                 "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
                 "= tActivityDetailCount.ActivityDetailRefno  " +
                 "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
                 "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
                 "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
                 "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
                 "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
                 "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno  " +
                 "= tActivityDetail.LocationMapRefno  " +
                 "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
                 "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
                 "= tActivityDetailCount.ActivityDetailRefno  " +
                 "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
                 "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
                 "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
                 "= tActivityDetail.ActivityDetailRefno  " +
                "WHERE tActivityDetail.ActivityItemRefno = " + refno +
                " AND tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;

            DataTable table = new DataTable();

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(warehouseQuery, DatabaseConnection.GetInstance().SQLConn))
            {
                da.Fill(table);
            }

            return table;
        }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="dataGridMain"></param>
//        /// <remarks></remarks>
//        public override void DesignDataGrid(DataGrid dataGridMain)
//        {
//            int colWidthFactor = 1;

//            if (this.SysEnv.IsWindowsMobile6Device)
//            {
//                colWidthFactor = 2;
//            }

//            if (dataGridMain.TableStyles.Count > 0)
//            {
//                dataGridMain.TableStyles.Clear();
//            }

//            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
//            dataGridStyle.MappingName = this.MainGridMappingName;

//            DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ActivityDetailQuantity";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
//#else
//            dataGridCol.HeaderText = "Qty";
//#endif
//            dataGridCol.Width = 50 * colWidthFactor;
//            dataGridCol.Format = "0.000";
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ActivityDetailWeightQuantity";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
//#else
//            dataGridCol.HeaderText = "Weight";
//#endif
//            dataGridCol.Width = 55 * colWidthFactor;
//            dataGridCol.Format = "0.000";
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "UnitCode";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = 30 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ActivityDetailSchemeID";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.SchemeId);
//#else
//            dataGridCol.HeaderText = "Scheme ID";
//#endif
//            dataGridCol.Width = 500 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridMain.TableStyles.Add(dataGridStyle);
//            dataGridMain.RowHeadersVisible = false;
//            dataGridMain.ColumnHeadersVisible = true;
//        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override DataView Search(string searchText)
        {
            this.DataViewDetails.RowFilter = "ActivityCountStringFKRefno LIKE \'" + searchText.Trim() + "%\'";

            return this.DataViewDetails;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectDetail(int idx)
        {
            if (idx >= 0)
            {
                this.SelectedLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)DataViewDetails[idx]["LocationMapRefno"]);
                this.Refno = (int)this.DataViewDetails[idx]["ActivityDetailRefno"];
                this.FKRefno = (int)this.DataViewDetails[idx]["ActivityDetailCountFKRefno"];
                this.SchemeID = (string)this.DataViewDetails[idx]["ActivityDetailSchemeID"];
                this.UniqueSchemeID = (string)this.DataViewDetails[idx]["ActivityDetailUniqueSchemeID"];

                this.ActExtra.Text1 = (string)this.DataViewDetails[idx]["ActivityDetailExtra1Text"];
                this.ActExtra.Text2 = (string)this.DataViewDetails[idx]["ActivityDetailExtra2Text"];
                this.ActExtra.Text3 = (string)this.DataViewDetails[idx]["ActivityDetailExtra3Text"];
                this.ActExtra.Text4 = (string)this.DataViewDetails[idx]["ActivityDetailExtra4Text"];
                this.ActExtra.Text5 = (string)this.DataViewDetails[idx]["ActivityDetailExtra5Text"];
            }
            else
            {
                this.ClearProperties();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = null;
            if (data.Document.Root.Element("skid_data") == null)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT CYCLE COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + ("<skid_fk_refno>" + 0 + "</skid_fk_refno>")
                        + "</imclient>");
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT CYCLE COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                        + "</imclient>");
                }
            }
            else
            {
                // TODO: extra? elements may not be needed.
                // Note whs_id, sect_refno and slot_refno are sent because counting will use the selected location not the
                // skid location. This is unique to counting and not picking / transfers etc...
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT CYCLE COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>")
                    + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                    + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
                    + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                    + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                    + "<is_negate_qty>false</is_negate_qty>"
                    + ("<extra1>" + this.ActExtra.Text1 + "</extra1>")
                    + ("<extra2>" + this.ActExtra.Text2 + "</extra2>")
                    + ("<extra3>" + this.ActExtra.Text3 + "</extra3>")
                    + ("<extra4>" + this.ActExtra.Text4 + "</extra4>")
                    + ("<extra5>" + this.ActExtra.Text5 + "</extra5>")
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = null;
            if (data.Document.Root.Element("skid_data") == null)
            {
                // TODO: skid_fk_refno element may not be needed.
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CYCLE COUNT DETAIL</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + this.GetRequestDetailDefaults()
                    + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                    + "</imclient>");
            }
            else
            {
                // TODO: is_negate_qty element may not be needed.
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CYCLE COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>")
                    + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                    + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
                    + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                    + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        #endregion
    }
}