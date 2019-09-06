namespace dMobile.Controller
{
    using System;
    using System.Data;
    
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;

    public class CycleActItem : ActItem
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="activity"></param>
        /// <remarks></remarks>
        public CycleActItem(Activity activity)
            : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Cycle;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsCycleCreateNewItemNoAsk;
            this.FullClassName = typeof(CycleActItem).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool IsCatchAsk()
        {
            bool isCatchAsk = base.IsCatchAsk();
            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsCycleCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsCycleCountNoCatchAskZeroEnabled;
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
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParseCountItem), actItemStringFKRefno);
#else
            return string.Format("Unable To Parse Count Item# {0}", actItemStringFKRefno);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindCountItem), actItemStringFKRefno);
#else
            return string.Format("Unable To Find Count Item# {0}", actItemStringFKRefno);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemCountStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override void Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int foundActItemRefno = 0;
            var result = DialogResult.None;
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            int locMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno;
            if (this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
                locMapRefno = this.LocMap.CurrLocMapRefno;

            if (prodUnitRefno != 0)
            {
                foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0)
                {
                    result = MessageBox.Show(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.ALineItemAlreadyExistsForThisLocationAndProduct) + Environment.NewLine 
              + Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToCreateOne), 
              Translator.GetInstance().Translate(TranslatorVocabulary.NewItem), 
#else
"A line item already exists for this location and product." + Environment.NewLine
                        + "Do you wish to create one?",
                        "New Item",
#endif
 MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button3);
                }
                else
                {
                    // If the selected product to insert was not found, force the dialog result to insert it.
                    result = DialogResult.Yes;
                }

                if (result == DialogResult.Yes)
                {
                    using (SQLiteCommand cmndActCountItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndActCountItem.CommandText = "tActivityItemCount";
                        cmndActCountItem.CommandType = CommandType.TableDirect;
                        cmndActCountItem.IndexName = "UQ_ActivityItemCountOnActivityItemCountFKRefno";

                        cmndItem.CommandText = "tActivityItem";
                        cmndItem.CommandType = CommandType.TableDirect;

                        cmndID.CommandText = "SELECT @@IDENTITY";
                        cmndID.CommandType = CommandType.Text;

                        using (SqlCeResultSet rs = cmndActCountItem.ExecuteResultSet(ResultSetOptions.Updatable))
                        using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                            SqlCeUpdatableRecord rec = rs.CreateRecord();
                            SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                            recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.CycleCounts);
                            recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                            recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                            rsActItem.Insert(recActItem);

                            this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());

                            this.FKRefno = this.Refno * -1;

                            rec.SetInt32((int)SysCommon.ActItemCountFields.ActItemRefno, this.Refno);
                            rec.SetInt32(
                                (int)SysCommon.ActItemCountFields.ActItemSubType,
                                (int)SysCommon.ActSubTypes.CycleCounts);
                            rec.SetInt32((int)SysCommon.ActItemCountFields.FKRefno, this.FKRefno);
                            rec.SetString((int)SysCommon.ActItemCountFields.StringFKRefno, this.FKRefno.ToString());
                            rec.SetInt32((int)SysCommon.ActItemCountFields.ProdUnitRefno, prodUnitRefno);
                            rec.SetInt32((int)SysCommon.ActItemCountFields.LocMapRefno, locMapRefno);
                            rec.SetDecimal((int)SysCommon.ActItemCountFields.QuantityOnHand, 0);
                            rec.SetDecimal((int)SysCommon.ActItemCountFields.BaseQuantityOnHand, 0);
                            rec.SetDecimal((int)SysCommon.ActItemCountFields.WeightQuantityOnHand, 0);
                            rec.SetDecimal((int)SysCommon.ActItemCountFields.Quantity, 0);
                            rec.SetDecimal((int)SysCommon.ActItemCountFields.BaseQuantity, 0);
                            rec.SetDecimal((int)SysCommon.ActItemCountFields.WeightQuantity, 0);
                            rs.Insert(rec);

                            this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                            if (this.DataViewItems != null)
                            {
                                DataRowView rowView = this.DataViewItems.AddNew();
                                rowView["ActivityItemRefno"] = this.Refno;
                                rowView["ActivityItemCountFKRefno"] = this.FKRefno;
                                rowView["ActivityItemCountStringFKRefno"] = this.FKRefno.ToString();
                                rowView["ProductUnitRefno"] = prodUnitRefno;
                                rowView["ProductRefno"] = int.Parse(data.Root.Element("ProductRefno").Value);
                                rowView["ProductCode"] = data.Root.Element("ProductCode").Value;
                                rowView["ProductDescription"] = data.Root.Element("ProductDescription").Value;
                                rowView["UnitCode"] = data.Root.Element("UnitCode").Value;
                                rowView["ProductUnitBC"] = data.Root.Element("ProductUnitBC").Value;
                                rowView["ProductUnitSizeDescription"] = data.Root.Element("ProductUnitSizeDescription").Value;
                                rowView["LocationMapRefno"] = locMapRefno;
                                rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
                                rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                                rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
                                rowView["ActivityItemCountQuantityOnHand"] = 0;
                                rowView["ActivityItemCountBaseQuantityOnHand"] = 0;
                                rowView["ActivityItemCountWeightQuantityOnHand"] = 0;
                                rowView["ActivityItemCountQuantity"] = 0;
                                rowView["ActivityItemCountBaseQuantity"] = 0;
                                rowView["ActivityItemCountWeightQuantity"] = 0;
                                rowView.EndEdit();
                            }
                        }
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

                TableActivityCycleCountItem tableActCountItem = new TableActivityCycleCountItem();
                TableActivityCycleCountDetail tableActCountDetail = new TableActivityCycleCountDetail();
                tableActCountDetail.Purge();
                tableActCountItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActCountItem.RegisterDataAccessListenerProgressOut((CycleActItem)this);
                tableActCountItem.Request(this.ActHdr.Refno);
                tableActCountItem.UnregisterDataAccessListenerProgressOut((CycleActItem)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataGridMain"></param>
        /// <remarks></remarks>
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
            dataGridCol.MappingName = "ActivityItemRefno";
            dataGridCol.HeaderText = string.Empty;
            dataGridCol.Width = -1;
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "ProductCode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Codezz);
#else
            dataGridCol.HeaderText = "Code";
#endif
            dataGridCol.Width = 62 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "ProductUnitBC";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
#else
            dataGridCol.HeaderText = "BC";
#endif
            dataGridCol.Width = 20 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "UnitCode";
            dataGridCol.HeaderText = string.Empty;
            dataGridCol.Width = 22 * colWidthFactor;
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "ActivityItemCountQuantityOnHand";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.OnHandz);
#else
            dataGridCol.HeaderText = "On Hand";
#endif
            dataGridCol.Width = 55 * colWidthFactor;
            dataGridCol.Format = "0.000";
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridCol = new DataGridTextBoxColumn();
            dataGridCol.MappingName = "ActivityItemCountQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Counted);
#else
            dataGridCol.HeaderText = "Counted";
#endif
            dataGridCol.Width = 45 * colWidthFactor;
            dataGridCol.Format = "0.000";
            dataGridStyle.GridColumnStyles.Add(dataGridCol);

            dataGridMain.TableStyles.Add(dataGridStyle);
            dataGridMain.RowHeadersVisible = false;
            dataGridMain.ColumnHeadersVisible = true;
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
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSlot.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName + \' \' + tLocationMapSlot.LocationMapSlotName ");
                sql.Append("AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("\' \' AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append(" SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSlot.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName + \' \' + tLocationMapSlot.LocationMapSlotName ");
                sql.Append("AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(\'\' AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> (0) ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("ORDER BY tActivityItemCount.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSlot.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName + \' \' + tLocationMapSlot.LocationMapSlotName ");
                sql.Append("AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("\' \' AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("UNION ");
                sql.Append(" SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat(" WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("ORDER BY tActivityItemCount.ActivityItemRefno");
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);
                    this.DataViewItems = ds.Tables[0].DefaultView;
                    this.DataViewItems.Table.TableName = this.MainGridMappingName;
                    this.DataViewItems.AllowNew = true;
                    this.DataViewItems.AllowEdit = true;
                    this.DataViewItems.AllowDelete = true;
                    this.DataViewItems.Sort = "ActivityItemRefno";
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
        public override DataView Search(string searchText)
        {
            if (string.IsNullOrEmpty(this.ActItemFilter))
            {
                this.DataViewItems.RowFilter = "ProductCode LIKE \'" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'" + searchText.Trim() + "%\'";
            }

            return this.DataViewItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectItem(int idx)
        {
            if (this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation)
            {
                this.CurrRowIdx = idx;
                this.Refno = (int)this.DataViewItems[idx]["ActivityItemRefno"];
                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)this.DataViewItems[idx]["ProductUnitRefno"]);
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                // Do not change the location to the skid location leave it alone for IsCycleDoNotDefaultProductLocation.
                // Clear the skid if the product changes.
                if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo != null && SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid)
                {
                    if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != this.ProdInfo.UnitRefno)
                    {
                        this.ActHdr.SkidInfo = new SysSkidInfo();
                    }
                }
            }
            else
            {
                base.SelectItem(idx);
            }

            this.FKRefno = (int)this.DataViewItems[idx]["ActivityItemCountFKRefno"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;

            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ProductUnitRefno,");
                sql.Append("tLocationMapSlot.LocationMapRefno ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItemCount.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.SlotLocMapRefno);
                sql.AppendFormat("AND tActivityItemCount.ProductUnitRefno = {0} ", prodUnitRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ProductUnitRefno,");
                sql.Append("tLocationMapSection.LocationMapRefno ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItemCount.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.SectLocMapRefno);
                sql.AppendFormat("AND tActivityItemCount.ProductUnitRefno = {0} ", prodUnitRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ProductUnitRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItemCount.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
                sql.AppendFormat("AND tActivityItemCount.ProductUnitRefno = {0}", prodUnitRefno);

                cmndItem.CommandType = CommandType.Text;
                cmndItem.CommandText = sql.ToString();

                using (SqliteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
                        this.Refno = (int)drItem["ActivityItemRefno"];
                        this.FKRefno = (int)drItem["ActivityItemCountFKRefno"];
                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)drItem["ProductUnitRefno"]);
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

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT CYCLE COUNT ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC + "</is_bc>")
                + ("<symbology>" + symbologyPrefix + "</symbology>")
                + ("<gtin>" + barCodeCode + "</gtin>")
                + "<whs_id></whs_id>"
                + "<sect_refno></sect_refno>"
                + "<slot_refno></slot_refno>"
                + "<refno></refno>"
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
            this.DataViewItems[idx].BeginEdit();
            this.DataViewItems[idx]["ActivityItemCountQuantity"] = quantity;
            this.DataViewItems[idx]["ActivityItemCountBaseQuantity"] = baseQuantity;
            this.DataViewItems[idx]["ActivityItemCountWeightQuantity"] = weightQuantity;
            this.DataViewItems[idx].EndEdit();
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
            this.DataViewItems[idx]["ActivityItemCountQuantity"]
                = (decimal)this.DataViewItems[idx]["ActivityItemCountQuantity"] + quantity;
            this.DataViewItems[idx]["ActivityItemCountBaseQuantity"]
                = (decimal)this.DataViewItems[idx]["ActivityItemCountBaseQuantity"] + baseQuantity;
            this.DataViewItems[idx]["ActivityItemCountWeightQuantity"]
                = (decimal)this.DataViewItems[idx]["ActivityItemCountWeightQuantity"] + weightQuantity;
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
                cmndItem.CommandText = "tActivityItemCount";
                cmndItem.CommandType = CommandType.Text;
                cmndItem.IndexName = string.Empty;
                cmndItem.CommandText = "UPDATE tActivityItemCount "
                  + "SET ActivityItemCountQuantity = " + this.DataViewItems[idx]["ActivityItemCountQuantity"] + ","
                  + "ActivityItemCountBaseQuantity = " + this.DataViewItems[idx]["ActivityItemCountBaseQuantity"] + ","
                  + "ActivityItemCountWeightQuantity = " + this.DataViewItems[idx]["ActivityItemCountWeightQuantity"]
                  + " WHERE tActivityItemCount.ActivityItemRefno = " + actItemRefno;
                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
