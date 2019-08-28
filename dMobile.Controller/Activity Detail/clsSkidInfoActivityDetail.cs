namespace dMobile.Controller
{
    using System;
    using System.Data;
    
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class SkidInfoActDetail : ActDetail
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItem"></param>
        public SkidInfoActDetail(ActItem actItem)
            : base(actItem)
        {
            this.FullClassName = typeof(SkidInfoActDetail).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override void InsertSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override void DeleteSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Insert(XDocument data)
        {
            // string strLocationTypestring.Empty;
            string locCode = string.Empty;
            string locName = string.Empty;

            if (LocMap.CurrLocMap.SlotLocMapRefno > 0)
            {
                // strLocationType = "Slot";
                locCode = this.LocMap.CurrLocMap.SlotCode.ToString();
                locName = this.LocMap.CurrLocMap.SlotName;
            }
            else if (LocMap.CurrLocMap.SectLocMapRefno > 0)
            {
                // strLocationType = "Section";
                locCode = this.LocMap.CurrLocMap.SectCode.ToString();
                locName = this.LocMap.CurrLocMap.SectName;
            }
            else
            {
                // strLocationType = "Whs";
                locCode = this.LocMap.CurrLocMap.WhsCode.ToString();
                locName = this.LocMap.CurrLocMap.WhsName;
            }

            this.FillActDetailObjFromXML(data);

            string xmlSend = this.CreateSubmitXML(
                XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            string reqData = SysCommon.SubmitComm(xmlSend);
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
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.Refno = this.FKRefno;
                }

                this.FillDataView();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override void Delete()
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
                    else
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);
                        SSkidInfo tempSkidInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                        tempSkidInfo.IsConsumed = bool.Parse(xmlDoc.DocumentElement["IsConsumed"].InnerText);
                        this.ActItem.ActHdr.SkidInfo.CurrSkid = tempSkidInfo;
                    }
                }
            }

            // delete row from DataView
            string oldSort = this.DataViewDetails.Sort;
            this.DataViewDetails.Sort = "ActivityDetailFKRefno";
            int rowIdx = this.DataViewDetails.Find(this.FKRefno);
            if (rowIdx >= 0)
            {
                this.DataViewDetails.Delete(rowIdx);
            }

            this.DataViewDetails.Sort = oldSort;
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
                this.FKRefno = (int)this.DataViewDetails[idx]["ActivityDetailFKRefno"];
                ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(
                ////    (int)this.DataViewDetails[idx]["LocationMapRefno"]);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedRowIndex"></param>
        /// <param name="data"></param>
        public override void Submit(int idx, XDocument data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = this.CreateSubmitXML(
                XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
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
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.Refno = this.FKRefno;

                    if (this.DataViewDetails != null)
                    {
                        this.DataViewDetails[idx].BeginEdit();
                        this.DataViewDetails[idx]["ActivityDetailFKRefno"] = this.FKRefno;
                        this.DataViewDetails[idx].EndEdit();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Refresh()
        {
            if (this.DataViewDetails != null)
            {
                while (this.DataViewDetails.Count > 0)
                {
                    this.DataViewDetails.Delete(0);
                }
            }

            DataSet ds = new DataSet();
            SysSkidInfo.GetSkidDetail(this.ActItem.ActHdr.SkidInfo, ref ds);
            if (ds.Tables.Count != 0)
            {
                using (SqlCeCommand cmndProdSchemeVar = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndProdSchemeVar.CommandText = "tProductSchemeVariant";
                    cmndProdSchemeVar.CommandType = CommandType.TableDirect;
                    cmndProdSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                    using (SqlCeDataReader drProdSchemeVar = cmndProdSchemeVar.ExecuteReader())
                    {
                        foreach (DataRow drow in ds.Tables["cskiddetail"].Rows)
                        {
                            // NOTE: The drow data is all strings because it was filled from xml data in SysSkidInfo.GetSkidDetail.
                            // Need to do a data type conversion not just a cast.
                            this.ActItem.ActHdr.FKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            this.ActItem.ActHdr.Refno = Convert.ToInt32(drow["ActivityFKRefno"]);
                            this.ActItem.FKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                            this.ActItem.Refno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                            this.FKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                            this.Refno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                            this.SchemeID = (string)drow["ActivityDetailSchemeID"];
                            this.UniqueSchemeID = (string)drow["ActivityDetailUniqueSchemeID"];
                            this.Quantity = Convert.ToDecimal(drow["ActivityDetailQuantity"]);
                            this.BaseQuantity = Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]);
                            this.WeightQuantity = Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]);

                            SProductSchemeData tempSchemeData = this.SchemeData;
                            tempSchemeData.LogisticUnitBatchLotNumber = (string)drow["ActivityDetailLogisticUnitBatchLotNumber"];
                            tempSchemeData.SingleTradeItemSerialNumber = (string)drow["ActivityDetailSingleTradeItemSerialNumber"];
                            tempSchemeData.LogisticUnitSkidNumber = ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno.ToString();
                            tempSchemeData.VariantNumber = Convert.ToInt32(drow["ProductSchemeVariantNo"]);

                            if (drProdSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { tempSchemeData.VariantNumber }))
                            {
                                if (drProdSchemeVar.Read())
                                {
                                    tempSchemeData.VariantCode = drProdSchemeVar.GetString((int)SysCommon.ProdSchemeVariantFields.Code);
                                    tempSchemeData.VariantDescription = drProdSchemeVar.GetString(
                                        (int)SysCommon.ProdSchemeVariantFields.Description);
                                }
                                else
                                {
                                    throw new Exception(
                                        string.Format(
                                        "Unable to locate the product scheme variant no {0} in tProductSchemeVariant",
                                        tempSchemeData.VariantNumber));
                                }
                            }

                            string newDate = "01-01-1900";

                            if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString()))
                            {
                                tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(newDate);
                            }
                            else
                            {
                                tempSchemeData.LogisticUnitBestBeforeSellByDate
                                    = Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]);
                            }

                            if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString()))
                            {
                                tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(newDate);
                            }
                            else
                            {
                                tempSchemeData.LogisticUnitExpiryUseByDate
                                    = Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]);
                            }

                            if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString()))
                            {
                                tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(newDate);
                            }
                            else
                            {
                                tempSchemeData.LogisticUnitProductionDate
                                    = Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]);
                            }

                            if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitDueDate"].ToString()))
                            {
                                tempSchemeData.LogisticUnitDueDate = DateTime.Parse(newDate);
                            }
                            else
                            {
                                tempSchemeData.LogisticUnitDueDate = Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]);
                            }

                            if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitPackagingDate"].ToString()))
                            {
                                tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(newDate);
                            }
                            else
                            {
                                tempSchemeData.LogisticUnitPackagingDate
                                    = Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]);
                            }

                            this.SchemeData = tempSchemeData;
                            this.FillDataView();
                        }
                    }
                }
            }

            this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
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
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetFKRefno(int idx)
        {
            return (int)this.DataViewDetails[idx]["ActivityDetailFKRefno"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            DataTable table = new DataTable("ActivitySkidDetail");

            table.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ProductCode", Type.GetType("System.String"));
            table.Columns.Add("ProductUnitBC", Type.GetType("System.String"));
            table.Columns.Add("ProductUnitIsBC", Type.GetType("System.Boolean"));
            table.Columns.Add("UnitCode", Type.GetType("System.String"));
            table.Columns.Add("BaseUnitCode", Type.GetType("System.String"));
            table.Columns.Add("WeightUnitCode", Type.GetType("System.String"));
            table.Columns.Add("ProductDescription", Type.GetType("System.String"));
            table.Columns.Add("ProductUnitSizeDescription", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantNo", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));

            this.DataViewDetails = table.DefaultView;
            this.DataViewDetails.Table.TableName = this.MainGridMappingName;

            return this.DataViewDetails;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataGridMain"></param>
        public override void DesignDataGrid(DataGrid dataGridMain)
        {
            int colWidthFactor = 1;

            if (this.SysEnv.IsWindowsMobile6Device)
            {
                colWidthFactor = 2;
            }

            if (dataGridMain.TableStyles.Count > 0)
            {
                dataGridMain.TableStyles.Clear();
            }

            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
            dataGridStyle.MappingName = this.MainGridMappingName;

            DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "ActivityDetailQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
#else
            dataGridCol.HeaderText = "Qty";
#endif
            dataGridCol.Width = 50 * colWidthFactor;
            dataGridCol.Format = "0.000";
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "ActivityDetailWeightQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
#else
            dataGridCol.HeaderText = "Weight";
#endif
            dataGridCol.Width = 55 * colWidthFactor;
            dataGridCol.Format = "0.000";
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "UnitCode";
            dataGridCol.HeaderText = string.Empty;
            dataGridCol.Width = 30 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "ActivityDetailSchemeID";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.SchemeId);
#else
            dataGridCol.HeaderText = "Scheme ID";
#endif
            dataGridCol.Width = 500 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridMain.TableStyles.Add(dataGridStyle);
            dataGridMain.RowHeadersVisible = false;
            dataGridMain.ColumnHeadersVisible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override DataView Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectDetail(int idx)
        {
            if (idx >= 0)
            {
                DataRowView drow = this.DataViewDetails[idx];
                this.FKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                this.Refno = Convert.ToInt32(drow["ActivityDetailRefno"]);
                this.SchemeID = (string)drow["ActivityDetailSchemeID"];
                this.UniqueSchemeID = (string)drow["ActivityDetailUniqueSchemeID"];
                this.Quantity = Convert.ToDecimal(drow["ActivityDetailQuantity"]);
                this.BaseQuantity = Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]);
                this.WeightQuantity = Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]);
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
        public override string CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT REMOVE PRODUCT FROM SKID</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + this.GetRequestDetailDefaults()
                + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                + "</imclient>");

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
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT ADD PRODUCT TO SKID</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + this.GetRequestDetailDefaults()
                + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public void FillDataView()
        {
            if (this.DataViewDetails != null)
            {
                DataRowView rowView = this.DataViewDetails.AddNew();
                rowView["ProductRefno"] = this.ActItem.ProdInfo.Refno;
                rowView["ProductCode"] = this.ActItem.ProdInfo.Code;
                rowView["ProductUnitBC"] = this.ActItem.ProdInfo.UnitBC;
                rowView["ProductUnitIsBC"] = this.ActItem.ProdInfo.UnitIsBC;
                rowView["ProductDescription"] = this.ActItem.ProdInfo.Desc;
                rowView["ProductUnitSizeDescription"] = this.ActItem.ProdInfo.UnitSizeDesc;
                rowView["UnitCode"] = this.ActItem.ProdInfo.UnitCode;
                rowView["BaseUnitCode"] = this.ActItem.ProdInfo.BaseUnitCode;
                rowView["WeightUnitCode"] = this.ActItem.ProdInfo.WeightUnitCode;
                rowView["ProductUnitRefno"] = this.ActItem.ProdInfo.UnitRefno;
                rowView["ProductDescription"] = this.ActItem.ProdInfo.Desc;
                rowView["ProductUnitSizeDescription"] = this.ActItem.ProdInfo.UnitSizeDesc;
                rowView["ActivityDetailQuantity"] = this.Quantity;
                rowView["ActivityDetailBaseQuantity"] = this.BaseQuantity;
                rowView["ActivityDetailWeightQuantity"] = this.WeightQuantity;
                rowView["ActivityDetailSchemeID"] = this.SchemeID;
                rowView["ActivityDetailUniqueSchemeID"] = this.UniqueSchemeID;
                rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"] = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                rowView["ActivityDetailLogisticUnitProductionDate"] = this.SchemeData.LogisticUnitProductionDate;
                rowView["ActivityDetailSingleTradeItemSerialNumber"] = this.SchemeData.SingleTradeItemSerialNumber;
                rowView["ActivityDetailLogisticUnitBatchLotNumber"] = this.SchemeData.LogisticUnitBatchLotNumber;
                rowView["ProductSchemeVariantCode"] = this.SchemeData.VariantCode;
                rowView["ProductSchemeVariantNo"] = this.SchemeData.VariantNumber;
                rowView["ActivityDetailFKRefno"] = this.FKRefno;
                rowView["ActivityDetailRefno"] = this.Refno;
                rowView.EndEdit();
            }
        }

        #endregion
    }
}
