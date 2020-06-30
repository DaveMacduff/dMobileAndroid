namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using Xamarin.Forms;

    public class POWorkSheetActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public POWorkSheetActivity()
          : base()
        {
            this.FullClassName = typeof(POWorkSheetActivity).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Work Sheet";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            // TODO: Design Grid
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        ///
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      int colWidthFactor = 1;

        //      if (SysEnv.IsWindowsMobile6Device)
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
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append(" tActivityItem.ActivityRefno,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno, ");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno, ");
                sql.Append(" tProductUnit.ProductUnitRefno,");
                sql.Append(" tProduct.ProductRefno,");
                sql.Append(" tProduct.ProductCode,");
                sql.Append(" tProduct.ProductDescription,");
                sql.Append(" tUnit.UnitCode,");
                sql.Append(" tProductUnit.ProductUnitSizeDescription,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append(" tLocationMapWarehouse.LocationMapRefno,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append(" tActivityItem.ActivityItemStatus");
                sql.Append(" FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
                sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno");
                sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
                sql.Append(" ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append(" tActivityItem.ActivityRefno,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno,");
                sql.Append(" tProductUnit.ProductUnitRefno,");
                sql.Append(" tProduct.ProductRefno,");
                sql.Append(" tProduct.ProductCode,");
                sql.Append(" tProduct.ProductDescription,");
                sql.Append(" tUnit.UnitCode,");
                sql.Append(" tProductUnit.ProductUnitSizeDescription,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append(" tLocationMapWarehouse.LocationMapRefno,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append(" tActivityItem.ActivityItemStatus");
                sql.Append(" FROM tActivityItemPurchaseOrderWorkSheet");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
                sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno");
                sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno");
                sql.Append(" ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            using (DataSet ds = new DataSet())
            {
                da.Fill(ds);
                this.DataViewHdrs = ds.Tables[0].DefaultView;
                this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                this.DataViewHdrs.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
            }

            return this.DataViewHdrs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);
            this.Refno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);

            // Set the selected warehouse to the activity header warehouse.
            if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]))
            {
                LocationMap.GetInstance().CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);
            this.Refno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);
                this.Purge();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            TableActivityPurchaseOrderWorkSheetItem tableWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();

            tableWorkSheetItem.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
