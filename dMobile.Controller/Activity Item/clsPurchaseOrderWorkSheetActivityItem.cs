namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using Xamarin.Forms;

    public class POWorkSheetActItem : ActItem
    {
        #region Fields

        private bool isUpdateView = true;
        private int newActItemFKRefno = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public POWorkSheetActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(POWorkSheetActItem).FullName;

            // Always allow adding product
            this.IsAllowAddItem = true;

            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPOWorkSheetCreateNewItemNoAsk;
        }

        #endregion

        #region Properties

        public int NewActItemFKRefno
        {
            get
            {
                return this.newActItemFKRefno;
            }

            set
            {
                this.newActItemFKRefno = value;
            }
        }

        public bool IsUpdateView
        {
            get
            {
                return this.isUpdateView;
            }

            set
            {
                this.isUpdateView = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsCatchAsk()
        {
            bool isCatchAsk = base.IsCatchAsk();
            
            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsPOWorkSheetCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsCatchAskZero()
        {
            bool isCatchAskZero = base.IsCatchAskZero();
            
            if (isCatchAskZero)
            {
                isCatchAskZero = !this.SysEnv.SysSettings.IsPOWorkSheetNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse purchase order work sheet item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find purchase order work sheet item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemPurchaseOrderWorkSheetStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            // TODO: Verify insert is correct
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            int actRefno = 0;

            this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);

            if (prodUnitRefno != 0)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    using (SQLiteCommand cmndActSheetItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    using (SQLiteCommand cmndAct = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    using (SQLiteCommand cmndIDItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        //cmndActSheetItem.CommandText = "tActivityItemPurchaseOrderWorkSheet";
                        //cmndActSheetItem.CommandType = CommandType.Text;
                        //cmndActSheetItem.IndexName = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno";

                        //cmndItem.CommandText = "tActivityItem";
                        //cmndItem.CommandType = CommandType.Text;

                        //cmndAct.CommandText = "tActivity";
                        //cmndAct.CommandType = CommandType.Text;

                        //using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                        //SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                        //cmndIDItem.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                        //cmndIDItem.CommandType = CommandType.Text;

                        //using (SqlCeResultSet rs = cmndActSheetItem.ExecuteResultSet(ResultSetOptions.Updatable))
                        //SqlCeUpdatableRecord rec = rs.CreateRecord();

                        //using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))

                        //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                        cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                        cmndAct.CommandType = CommandType.Text;
                        cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(SysCommon.ActStatus.Inserted)));
                        cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));

                        cmndAct.ExecuteNonQuery();

                        //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                        //recAct.SetInt32((int)SysCommon.ActFields.Status, (int)SysCommon.ActStatus.Inserted);
                        //rsAct.Insert(recAct);

                        cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                        cmndID.CommandType = CommandType.Text;

                        actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                        this.ActHdr.Refno = actRefno;
                        this.ActHdr.FKRefno = actRefno;

                        cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                        cmndItem.CommandType = CommandType.Text;
                        cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));
                        cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.Inserted)));
                        cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(this.ActHdr.Refno)));

                        cmndItem.ExecuteNonQuery();

                        //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                        //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.Inserted);
                        //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                        //rsActItem.Insert(recActItem);

                        cmndIDItem.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                        cmndIDItem.CommandType = CommandType.Text;

                        this.Refno = Convert.ToInt32(cmndIDItem.ExecuteScalar());

                        cmndActSheetItem.CommandText = "INSERT INTO tActivityItemPurchaseOrderWorkSheet (" +
                            "ActivityItemRefno, " +
                            "ActivityItemSubType, " +
                            "ActivityItemPurchaseOrderWorkSheetFKRefno, " +
                            "ActivityItemPurchaseOrderWorkSheetStringFKRefno, " +
                            "ProductUnitRefno, " +
                            "WarehouseLocationMapRefno, " +
                            "ActivityItemPurchaseOrderWorkSheetQuantityOrdered, " +
                            "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered, " +
                            "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered" +
                            ") VALUES (" +
                            "@activityItemRefno, " +
                            "@activityItemSubType, " +
                            "@activityItemPurchaseOrderWorkSheetFKRefno, " +
                            "@activityItemPurchaseOrderWorkSheetStringFKRefno, " +
                            "@productUnitRefno, " +
                            "@warehouseLocationMapRefno, " +
                            "@activityItemPurchaseOrderWorkSheetQuantityOrdered, " +
                            "@activityItemPurchaseOrderWorkSheetBaseQuantityOrdered, " +
                            "@activityItemPurchaseOrderWorkSheetWeightQuantityOrdered" +
                            ")";
                        cmndActSheetItem.CommandType = CommandType.Text;
                        //cmndActSheetItem.IndexName = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno";
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetFKRefno", Convert.ToInt32(this.Refno * -1)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetStringFKRefno", Convert.ToString(this.Refno * -1)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetQuantityOrdered", decimal.Parse(data.Root.Element("alt_qty").Value)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetBaseQuantityOrdered", decimal.Parse(data.Root.Element("base_qty").Value)));
                        cmndActSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetWeightQuantityOrdered", decimal.Parse(data.Root.Element("wt_qty").Value)));

                        cmndActSheetItem.ExecuteNonQuery();

                        //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.ActItemRefno, this.Refno);
                        //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.ActItemSubType, (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                        //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.FKRefno, this.Refno * -1);
                        //rec.SetString((int)SysCommon.ActItemWorkSheetFields.StringFKRefno, (this.Refno * -1).ToString());
                        //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.ProdUnitRefno, prodUnitRefno);
                        //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.WhsLocMapRefno, this.LocMap.CurrLocMap.WhsLocMapRefno);
                        //rec.SetDecimal((int)SysCommon.ActItemWorkSheetFields.QuantityOrdered, decimal.Parse(data.Root.Element("alt_qty").Value));
                        //rec.SetDecimal((int)SysCommon.ActItemWorkSheetFields.BaseQuantityOrdered, decimal.Parse(data.Root.Element("base_qty").Value));
                        //rec.SetDecimal((int)SysCommon.ActItemWorkSheetFields.WeightQuantityOrdered, decimal.Parse(data.Root.Element("wt_qty").Value));
                        //rs.Insert(rec);

                        this.FKRefno = this.Refno * -1;

                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                        this.IsUpdateView = true;
                    }
                }

                else
                {
                    string xmlSend = this.CreateSubmitXML(data);
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
                            this.IsUpdateView = true;
                            this.NewActItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                            this.FKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                            this.ActHdr.FKRefno = this.FKRefno;
                            this.Refno = this.FKRefno * -1;
                        }
                    }
                }
            }

            if (this.IsUpdateView && this.DataViewItems != null)
            {
                DataRowView rowView = null;
                string oldSort = DataViewItems.Sort;
                int rowIdx = 0;
                this.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";

                if (this.FKRefno > 0)
                {
                    rowIdx = this.DataViewItems.Find(new object[] { this.FKRefno });

                    if (rowIdx > -1)
                    {
                        rowView = this.DataViewItems[rowIdx];
                    }
                }

                if (rowView == null)
                {
                    rowView = this.DataViewItems.AddNew();
                    rowView["ActivityItemRefno"] = this.Refno;
                    rowView["ProductUnitRefno"] = this.ProdInfo.UnitRefno;
                    rowView["ProductRefno"] = this.ProdInfo.Refno;
                    rowView["ProductCode"] = this.ProdInfo.Code;
                    rowView["ProductDescription"] = this.ProdInfo.Desc;
                    rowView["ProductUnitSizeDescription"] = this.ProdInfo.UnitSizeDesc;
                    rowView["UnitCode"] = this.ProdInfo.UnitCode;
                    rowView["ActivityItemPurchaseOrderWorkSheetFKRefno"] = this.FKRefno;
                    rowView["ActivityItemPurchaseOrderWorkSheetStringFKRefno"] = this.FKRefno.ToString();
                }

                rowView["ActivityItemStatus"] = SysCommon.ActStatus.NoChange;
                rowView["LocationMapRefno"] = this.LocMap.LoginWhsLocMapRefno;
                rowView["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] = Math.Round(decimal.Parse(data.Root.Element("alt_qty").Value), 3);
                rowView["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] = Math.Round(decimal.Parse(data.Root.Element("base_qty").Value), 3);
                rowView["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] = Math.Round(decimal.Parse(data.Root.Element("wt_qty").Value), 3);
                rowView["LocationCode"] = this.LocMap.LoginWhsLocMapRefno;
                rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                rowView.EndEdit();

                this.DataViewItems.Sort = oldSort;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Update(XDocument data)
        {
            decimal decQty = Math.Round(decimal.Parse(data.Root.Element("alt_qty").Value), 3);
            decimal decBaseQty = Math.Round(decimal.Parse(data.Root.Element("base_qty").Value), 3);
            decimal decWeightQty = Math.Round(decimal.Parse(data.Root.Element("wt_qty").Value), 3);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                // Update the Activity Item quantity shipped
                using (SQLiteCommand cmndAct = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndAct.CommandType = CommandType.Text;
                    cmndAct.CommandText = "UPDATE tActivityItemPurchaseOrderWorkSheet "
                        + "SET ActivityItemPurchaseOrderWorkSheetQuantityOrdered = " + decQty + ","
                        + "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = " + decBaseQty + ","
                        + "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = " + decWeightQty
                        + " WHERE tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno = " + this.Refno;
                    cmndAct.ExecuteNonQuery();

                    cmndAct.CommandType = CommandType.Text;
                    cmndAct.CommandText = "UPDATE tActivityItem "
                        + "SET ActivityItemStatus = " + (int)SysCommon.ActStatus.Updated
                        + " WHERE tActivityItem.ActivityItemRefno = " + this.Refno;
                    cmndAct.ExecuteNonQuery();

                    cmndAct.CommandType = CommandType.Text;
                    cmndAct.CommandText = "UPDATE tActivity "
                        + "SET ActivityStatus = " + (int)SysCommon.ActStatus.Updated
                        + " WHERE tActivity.ActivityRefno = " + this.ActHdr.Refno;
                    cmndAct.ExecuteNonQuery();
                }
            }
            else
            {
                string xmlSend = this.CreateSubmitXML(data);
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
                        this.IsUpdateView = true;
                        this.NewActItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                        this.FKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                        this.ActHdr.FKRefno = this.FKRefno;
                    }
                }
            }

            if (this.IsUpdateView && this.DataViewItems != null)
            {
                DataRowView rowView = null;
                string oldSort = this.DataViewItems.Sort;
                int rowIdx = 0;
                this.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
                rowIdx = DataViewItems.Find(new object[] { this.FKRefno });
                
                if (rowIdx > -1)
                {
                    rowView = this.DataViewItems[rowIdx];
                }

                if (rowView == null)
                {
                    rowView = this.DataViewItems.AddNew();
                    rowView["ActivityItemRefno"] = this.Refno;
                    rowView["ProductUnitRefno"] = this.ProdInfo.UnitRefno;
                    rowView["ProductRefno"] = this.ProdInfo.Refno;
                    rowView["ProductCode"] = this.ProdInfo.Code;
                    rowView["ProductDescription"] = this.ProdInfo.Desc;
                    rowView["ProductUnitSizeDescription"] = this.ProdInfo.UnitSizeDesc;
                    rowView["UnitCode"] = this.ProdInfo.UnitCode;
                    rowView["ActivityItemPurchaseOrderWorkSheetFKRefno"] = this.FKRefno;
                    rowView["ActivityItemPurchaseOrderWorkSheetStringFKRefno"] = this.FKRefno.ToString();
                }

                rowView["ActivityItemStatus"] = SysCommon.ActStatus.NoChange;
                rowView["LocationMapRefno"] = this.LocMap.LoginWhsLocMapRefno;
                rowView["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] = decQty;
                rowView["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] = decBaseQty;
                rowView["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] = decWeightQty;
                rowView["LocationCode"] = this.LocMap.LoginWhsLocMapRefno;
                rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                rowView.EndEdit();

                this.DataViewItems.Sort = oldSort;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idx"></param>
        public override void Submit(XDocument data, int idx)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = this.CreateSubmitXML(data);
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

                    if (this.DataViewItems != null)
                    {
                        this.DataViewItems[idx].BeginEdit();
                        this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetFKRefno"] = this.FKRefno;
                        this.DataViewItems[idx]["ActivityItemStatus"] = SysCommon.ActStatus.NoChange;
                        this.DataViewItems[idx].EndEdit();
                    }

                    using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndItem.CommandType = CommandType.Text;
                        cmndItem.CommandText = "UPDATE tActivityItem "
                            + "SET tActivityItem.ActivityItemStatus = " + (int)SysCommon.ActStatus.NoChange
                            + " WHERE tActivityItem.ActivityItemRefno = " + this.Refno;
                        cmndItem.ExecuteNonQuery();
                    }

                    using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndItem.CommandType = CommandType.Text;
                        cmndItem.CommandText = "UPDATE tActivityItem "
                            + "SET tActivityItem.ActivityItemStatus = " + (int)SysCommon.ActStatus.NoChange
                            + " WHERE tActivityItem.ActivityItemRefno = " + this.Refno;
                        cmndItem.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityPurchaseOrderWorkSheetItem tableActPOWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();
                tableActPOWorkSheetItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActPOWorkSheetItem.RegisterDataAccessListenerProgressOut((POWorkSheetActItem)this);
                tableActPOWorkSheetItem.Request(ActHdr.Refno);
                tableActPOWorkSheetItem.UnregisterDataAccessListenerProgressOut((POWorkSheetActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity, ref ColumnDefinition dataGridWidth)
        {
            throw new NotImplementedException();
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <param name="dataGridMain"></param>
        //    /// <remarks></remarks>
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      int colWidthFactor = 1;

        //      if (this.SysEnv.IsWindowsMobile6Device)
        //      {
        //        colWidthFactor = 2;
        //      }

        //      if (dataGridMain.TableStyles.Count > 0)
        //      {
        //        dataGridMain.TableStyles.Clear();
        //      }

        //      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //      dataGridStyle.MappingName = this.MainGridMappingName;

        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemRefno";
        //      dataGridCol.HeaderText = string.Empty;
        //      dataGridCol.Width = -1;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductCode";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
        //#else
        //      dataGridCol.HeaderText = "Product ID";
        //#endif
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemPurchaseOrderWorkSheetQuantityOrdered";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Ordered);
        //#else
        //      dataGridCol.HeaderText = "Ordered";
        //#endif
        //      dataGridCol.Width = 52 * colWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
        //#else
        //      dataGridCol.HeaderText = "Weight";
        //#endif
        //      dataGridCol.Width = 52 * colWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridMain.TableStyles.Add(dataGridStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.Append("ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);
                    this.DataViewItems = ds.Tables[0].DefaultView;
                    this.DataViewItems.Table.TableName = this.MainGridMappingName;
                    this.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
                    this.DataViewItems.AllowNew = true;
                    this.DataViewItems.AllowEdit = true;
                    this.DataViewItems.AllowDelete = true;
                }
            }

            return this.DataViewItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActItemFilter))
            {
                this.DataViewItems.RowFilter = "ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            }

            // TODO: search
            //foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            //{
            //    ObjectType objectType = new ObjectType
            //    {
            //    };

            //    observableCollection.Add(objectType);
            //}

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>

        public override void SelectItem(int idx)
        {
            base.SelectItem(idx);
            this.FKRefno = Convert.ToInt32(DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetFKRefno"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;

            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                cmndItem.CommandType = CommandType.Text;
                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.AppendFormat(
                    "WHERE tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno = {0} ",
                    this.LocMap.CurrLocMap.WhsLocMapRefno);
                sql.AppendFormat("AND tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno = {0} ", prodUnitRefno);
                cmndItem.CommandText = sql.ToString();

                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(drItem["LocationMapRefno"]));
                        this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
                        this.FKRefno = Convert.ToInt32(drItem["ActivityItemPurchaseOrderWorkSheetFKRefno"]);
                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(drItem["ProductUnitRefno"]));
                        this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                        foundActItemRefno = this.Refno;
                    }

                    drItem.Close();
                }
            }

            return foundActItemRefno;
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
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PURCHASE ORDER WORK SHEET DETAIL</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>")
                + ("<symbology>" + symbologyPrefix + "</symbology>")
                + ("<gtin>" + barCodeCode + "</gtin>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + ("<refno>" + this.FKRefno + "</refno>")
                + ("<itm_refno>" + this.FKRefno + "</itm_refno>")
                + ("<alt_qty>" + data.Root.Element("alt_qty").Value + "</alt_qty>")
                + ("<base_qty>" + data.Root.Element("base_qty").Value + "</base_qty>")
                + ("<wt_qty>" + data.Root.Element("wt_qty").Value + "</wt_qty>")
                + ("<alt_unit>" + this.ProdInfo.UnitCode + "</alt_unit>")
                + ("<base_unit>" + this.ProdInfo.BaseUnitCode + "</base_unit>")
                + ("<wt_unit>" + this.ProdInfo.WeightUnitCode + "</wt_unit>")
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] = quantity;
            this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] = baseQuantity;
            this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] = weightQuantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            this.DataViewItems[idx].BeginEdit();
            this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"]
                = (decimal)this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] + quantity;
            this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"]
                = (decimal)this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] + baseQuantity;
            this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"]
                = (decimal)DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] + weightQuantity;
            this.DataViewItems[idx].EndEdit();

            return idx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandText = "tActivityItemPurchaseOrderWorkSheet";
                cmndItem.CommandType = CommandType.Text;
                //cmndItem.IndexName = string.Empty;
                cmndItem.CommandText = "UPDATE tActivityItemPurchaseOrderWorkSheet "
                    + "SET ActivityItemPurchaseOrderWorkSheetQuantityOrdered = "
                    + this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] + ","
                    + " ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = "
                    + this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] + ","
                    + " ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = "
                    + this.DataViewItems[idx]["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"]
                    + " WHERE tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno = " + actItemRefno;
                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
