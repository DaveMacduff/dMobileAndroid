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

    public class TicketFinishedActItem : ActItem
    {
        #region Constructors

        public TicketFinishedActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(TicketFinishedActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2TicketFinished;
            this.IsAskCreateNewItem = false;
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
                isCatchAsk = this.SysEnv.SysSettings.IsTicketFinishedCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsTicketFinishedNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <param name="prodRefno"></param>
        /// <returns></returns>
        public override int GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            int locMapRefno = LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
                whsLocMapRefno,
                prodRefno,
                SysCommon.ProdLocMapTypes.Receiving);

            return locMapRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParseProductionItem), actItemStringFKRefno);
#else
            return string.Format("Unable to parse production item# {0}", actItemStringFKRefno);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindProductionItem), actItemStringFKRefno);
#else
            return string.Format("Unable to find production item# {0}", actItemStringFKRefno);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemProductionFinishedStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            // TODO: SQL
            //      int foundActItemRefno = 0;
            //      var dlgResult = DialogResult.None;
            //      int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            //      if (prodUnitRefno != 0)
            //      {
            //        foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

            //        if (foundActItemRefno > 0)
            //        {
            //          dlgResult = MessageBox.Show(
            //#if MULTILANGUAGE
            //              Translator.GetInstance().Translate(TranslatorVocabulary.ALineItemAlreadyExistsForThisLocationAndProduct) + Environment.NewLine 
            //              + Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToCreateOne), 
            //              Translator.GetInstance().Translate(TranslatorVocabulary.NewItem), 
            //#else
            //              "A line item already exists for this location and product." + Environment.NewLine 
            //              + "Do you wish to create one?", 
            //              "New Item", 
            //#endif
            //              MessageBoxButtons.YesNoCancel, 
            //              MessageBoxIcon.Question, 
            //              MessageBoxDefaultButton.Button3);
            //        }
            //        else
            //        {
            //          // If the selected product to insert was not found, force the dialog result to insert it.
            //          dlgResult = DialogResult.Yes;
            //        }

            //        if (dlgResult == DialogResult.Yes)
            //        {
            //          using (SQLiteCommand cmndActItemFinished = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //          {
            //            cmndActItemFinished.CommandText = "tActivityItemProductionFinished";
            //            cmndActItemFinished.CommandType = CommandType.TableDirect;
            //            cmndActItemFinished.IndexName = "UQ_ActivityItemProductionFinishedOnActivityItemProductionFinishedFKRefno";

            //            using (SqlCeResultSet rsActItemFinished = cmndActItemFinished.ExecuteResultSet(ResultSetOptions.Updatable))
            //            {
            //              SqlCeUpdatableRecord recActItemFinished = rsActItemFinished.CreateRecord();

            //              using (SQLiteCommand cmndActProdItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //              {
            //                cmndActProdItem.CommandText = "tActivityItemProduction";
            //                cmndActProdItem.CommandType = CommandType.TableDirect;
            //                cmndActProdItem.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

            //                using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //                {
            //                  cmndItem.CommandText = "tActivityItem";
            //                  cmndItem.CommandType = CommandType.TableDirect;

            //                  using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //                  {
            //                    cmndID.CommandText = "SELECT @@IDENTITY";
            //                    cmndID.CommandType = CommandType.Text;

            //                    using (SqlCeResultSet rsProdItem = cmndActProdItem.ExecuteResultSet(ResultSetOptions.Updatable))
            //                    {
            //                      SqlCeUpdatableRecord recActProdItem = rsProdItem.CreateRecord();

            //                      using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
            //                      {
            //                        SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

            //                        recActItem.SetInt32(
            //                            (int)SysCommon.ActItemFields.SubType, 
            //                            (int)SysCommon.ActSubTypes.TicketFinished);
            //                        recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
            //                        recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);

            //                        rsActItem.Insert(recActItem);

            //                        this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());

            //                        recActProdItem.SetInt32((int)SysCommon.ActItemProdFields.ActItemRefno, this.Refno);
            //                        recActProdItem.SetInt32(
            //                            (int)SysCommon.ActItemProdFields.ActivityItemSubType, 
            //                            (int)SysCommon.ActSubTypes.TicketFinished);
            //                        recActProdItem.SetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno, prodUnitRefno);
            //                        recActProdItem.SetInt32(
            //                            (int)SysCommon.ActItemProdFields.LocMapRefno, 
            //                            this.LocMap.CurrLocMap.WhsLocMapRefno);
            //                        recActProdItem.SetDecimal((int)SysCommon.ActItemProdFields.Quantity, 0);
            //                        recActProdItem.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantity, 0);
            //                        recActProdItem.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantity, 0);
            //                        string desc = data.Root.Element("ProductDescription").Value.ToString().TrimEnd() + " "
            //                            + data.Root.Element("ProductUnitSizeDescription").Value;
            //                        recActProdItem.SetString(
            //                            (int)SysCommon.ActItemProdFields.Description, 
            //                            desc);
            //                        recActProdItem.SetDecimal((int)SysCommon.ActItemProdFields.QuantityExpected, 0);
            //                        recActProdItem.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantityExpected, 0);
            //                        recActProdItem.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantityExpected, 0);

            //                        rsProdItem.Insert(recActProdItem);

            //                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            //                        this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            //                        this.FKRefno = this.Refno * -1;
            //                        recActItemFinished.SetInt32((int)SysCommon.ActItemProdFinishedFields.ActItemRefno, this.Refno);
            //                        recActItemFinished.SetInt32((int)SysCommon.ActItemProdFinishedFields.FKRefno, this.FKRefno);
            //                        recActItemFinished.SetString(
            //                            (int)SysCommon.ActItemProdFinishedFields.StringFKRefno, 
            //                            this.FKRefno.ToString());

            //                        rsActItemFinished.Insert(recActItemFinished);

            //                        if (this.DataViewItems != null)
            //                        {
            //                          DataRowView rowView = this.DataViewItems.AddNew();
            //                          rowView["ActivityItemRefno"] = this.Refno;
            //                          rowView["ActivityItemProductionFinishedFKRefno"] = this.FKRefno;
            //                          rowView["ActivityItemProductionFinishedStringFKRefno"] = this.FKRefno.ToString();
            //                          rowView["ProductUnitRefno"] = prodUnitRefno;
            //                          rowView["ProductRefno"] = int.Parse(data.Root.Element("ProductRefno").Value);
            //                          rowView["ProductCode"] = data.Root.Element("ProductCode").Value;
            //                          rowView["ProductDescription"] = data.Root.Element("ProductDescription").Value;
            //                          rowView["BaseUnitCode"] = data.Root.Element("BaseUnitCode").Value;
            //                          rowView["ProductUnitBC"] = data.Root.Element("ProductUnitBC").Value;
            //                          rowView["ProductUnitSizeDescription"] = data.Root.Element("ProductUnitSizeDescription").Value;
            //                          rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
            //                          rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
            //                          rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
            //                          rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
            //                          rowView["ActivityItemProductionQuantity"] = 0;
            //                          rowView["ActivityItemProductionBaseQuantity"] = 0;
            //                          rowView["ActivityItemProductionWeightQuantity"] = 0;
            //                          rowView["ActivityItemProductionQuantityExpected"] = 0;
            //                          rowView["ActivityItemProductionBaseQuantityExpected"] = 0;
            //                          rowView["ActivityItemProductionWeightQuantityExpected"] = 0;
            //                          rowView.EndEdit();
            //                        }
            //                      }
            //                    }
            //                  }
            //                }
            //              }
            //            }
            //          }
            //        }
            //      }
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

                TableActivityProductionTicketFinishedItem tableFinishedItem = new TableActivityProductionTicketFinishedItem();
                TableActivityProductionTicketFinishedDetail tableFinishedDetail
                    = new TableActivityProductionTicketFinishedDetail();
                tableFinishedDetail.Purge();
                tableFinishedItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableFinishedItem.RegisterDataAccessListenerProgressOut((TicketFinishedActItem)this);
                tableFinishedItem.Request(ActHdr.Refno);
                tableFinishedItem.UnregisterDataAccessListenerProgressOut((TicketFinishedActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            throw new NotImplementedException();
        }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="dataGridMain"></param>
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
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Codezz);
        //#else
        //      dataGridCol.HeaderText = "Code";
        //#endif
        //      dataGridCol.Width = 62 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductUnitBC";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
        //#else
        //      dataGridCol.HeaderText = "BC";
        //#endif
        //      dataGridCol.Width = 20 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "BaseUnitCode";
        //      dataGridCol.HeaderText = string.Empty;
        //      dataGridCol.Width = 22 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemProductionBaseQuantity";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
        //#else
        //      dataGridCol.HeaderText = "Qty";
        //#endif
        //      dataGridCol.Width = 45 * colWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemProductionBaseQuantityExpected";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Expqty);
        //#else
        //      dataGridCol.HeaderText = "ExpQty";
        //#endif
        //      dataGridCol.Width = 45 * colWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridMain.TableStyles.Add(dataGridStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

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
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.TicketFinished);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.TicketFinished);
                sql.Append("ORDER BY tActivityItemProduction.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapSlot.LocationMapRefno,");
                sql.Append("tLocationMapSection.LocationMapSectionCode + \'\\\' + tLocationMapSlot.LocationMapSlotCode ");
                sql.Append("AS LocationCode,");
                sql.Append(" \' \'  AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.TicketFinished);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("tLocationMapSection.LocationMapSectionName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.TicketFinished);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.TicketFinished);
                sql.Append("ORDER BY tActivityItemProduction.ActivityItemRefno");
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
            this.FKRefno = (int)this.DataViewItems[idx]["ActivityItemProductionFinishedFKRefno"];
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
                cmndItem.CommandType = CommandType.Text;
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProduction.ProductUnitRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItemProduction.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
                sql.AppendFormat("AND tActivityItemProduction.ProductUnitRefno = {0} ", prodUnitRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0}", (int)SysCommon.ActSubTypes.TicketFinished);

                cmndItem.CommandText = sql.ToString();
                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
                        this.Refno = (int)drItem["ActivityItemRefno"];
                        this.FKRefno = (int)drItem["ActivityItemProductionFinishedFKRefno"];
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
            string strBaseUnitCode = data.Root.Element("BaseUnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRODUCTION TICKET FINISHED ITEM</action>"
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
            this.DataViewItems[idx]["ActivityItemProductionQuantity"] = quantity;
            this.DataViewItems[idx]["ActivityItemProductionBaseQuantity"] = baseQuantity;
            this.DataViewItems[idx]["ActivityItemProductionWeightQuantity"] = weightQuantity;
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
            DataRowView rowView = this.DataViewItems[idx];
            rowView.BeginEdit();
            rowView["ActivityItemProductionQuantity"]
                = (decimal)this.DataViewItems[idx]["ActivityItemProductionQuantity"] + quantity;
            rowView["ActivityItemProductionBaseQuantity"]
                = (decimal)this.DataViewItems[idx]["ActivityItemProductionBaseQuantity"] + baseQuantity;
            rowView["ActivityItemProductionWeightQuantity"]
                = (decimal)this.DataViewItems[idx]["ActivityItemProductionWeightQuantity"] + weightQuantity;
            rowView.EndEdit();

            return idx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="activityItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandText = "tActivityItemProduction";
                cmndItem.CommandType = CommandType.Text;
                //cmndItem.IndexName = string.Empty;
                cmndItem.CommandText = "UPDATE tActivityItemProduction "
                    + "SET ActivityItemProductionQuantity = " + this.DataViewItems[idx]["ActivityItemProductionQuantity"] + ","
                    + "ActivityItemProductionBaseQuantity = "
                    + this.DataViewItems[idx]["ActivityItemProductionBaseQuantity"] + ","
                    + "ActivityItemProductionWeightQuantity = "
                    + this.DataViewItems[idx]["ActivityItemProductionWeightQuantity"]
                    + " WHERE tActivityItemProduction.ActivityItemRefno = " + actItemRefno;
                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
