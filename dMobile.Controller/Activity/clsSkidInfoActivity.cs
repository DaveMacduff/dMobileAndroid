namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;

    public class SkidInfoActivity : Activity
    {
        #region Fields

        private LocationMap locMap;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SkidInfoActivity()
            : base()
        {
            this.FullClassName = typeof(SkidInfoActivity).FullName;
            this.LocMap = LocationMap.GetInstance();
        }

        #endregion

        #region Properties

        public LocationMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Skid Info";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Delete()
        {
            if (this.DataViewHdrs != null)
            {
                while (this.DataViewHdrs.Count > 1)
                {
                    this.DataViewHdrs.Delete(0);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectHdr(int idx)
        {
            if (idx >= 0)
            {
                this.FKRefno = (int)this.DataViewHdrs[idx]["SkidFKRefno"];
                this.Refno = (int)this.DataViewHdrs[idx]["SkidFKRefno"];

                ////this.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode((string)this.DataViewHdrs[idx]["SkidCode"]);

                SSkidInfo info = new SSkidInfo();
                info.SkidCode = Convert.ToString(this.DataViewHdrs[idx]["SkidCode"]);
                info.SkidFKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["SkidFKRefno"]);
                info.LocMapRefno = Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]);
                info.LocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(info.LocMapRefno);
                info.ProdInfo = ProductInfoTasks.GetInfoFromRefno(Convert.ToInt32(this.DataViewHdrs[idx]["ProductRefno"]));
                info.Quantity = Convert.ToDecimal(this.DataViewHdrs[idx]["Quantity"]);
                info.BaseQuantity = Convert.ToDecimal(this.DataViewHdrs[idx]["BaseQuantity"]);
                info.WeightQuantity = Convert.ToDecimal(this.DataViewHdrs[idx]["WeightQuantity"]);
                info.Cube = Convert.ToDecimal(this.DataViewHdrs[idx]["SkidCube"]);
                info.CubeUnit = Convert.ToString(this.DataViewHdrs[idx]["SkidCubeUnit"]);
                info.Height = Convert.ToDecimal(this.DataViewHdrs[idx]["SkidHeight"]);
                info.HeightUnit = Convert.ToString(this.DataViewHdrs[idx]["SkidHeightUnit"]);
                info.Hi = Convert.ToInt32(this.DataViewHdrs[idx]["SkidHi"]);
                info.Ti = Convert.ToInt32(this.DataViewHdrs[idx]["SkidTi"]);
                info.IsCHEP = Convert.ToBoolean(this.DataViewHdrs[idx]["SkidIsChep"]);
                info.IsHold = Convert.ToBoolean(this.DataViewHdrs[idx]["SkidIsOnHold"]);
                info.IsPutAway = false;
                info.IsCompleted = !SysCommon.IsEmptyDateTime((DateTime)this.DataViewHdrs[idx]["SkidCompleted"]);
                info.IsConsumed = false;

                // Note: did not fill in the scheme data, should call SysSkidInfo.GetSkidInfoFromBarCodeOrCode
                // instead of filling info from the DataView.
                this.SkidInfo.CurrSkid = info;
            }
            else
            {
                this.FKRefno = 0;
                this.Refno = 0;
                this.PONO = string.Empty;
                this.SkidInfo = new SysSkidInfo();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefnoFromRowIdx(int idx)
        {
            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            DataTable table = new DataTable("ActivitySkid");

            table.Columns.Add("SkidFKRefno", Type.GetType("System.Int32"));
            table.Columns.Add("SkidCode", Type.GetType("System.String"));
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
            table.Columns.Add("Quantity", Type.GetType("System.Decimal"));
            table.Columns.Add("BaseQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("WeightQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("LocationMapRefno", Type.GetType("System.Int32"));
            table.Columns.Add("LocationName", Type.GetType("System.String"));
            table.Columns.Add("SkidCreated", Type.GetType("System.DateTime"));
            table.Columns.Add("SkidCompleted", Type.GetType("System.DateTime"));
            table.Columns.Add("SkidTi", Type.GetType("System.Int32"));
            table.Columns.Add("SkidHi", Type.GetType("System.Int32"));
            table.Columns.Add("SkidCube", Type.GetType("System.Decimal"));
            table.Columns.Add("SkidCubeUnit", Type.GetType("System.String"));
            table.Columns.Add("SkidHeight", Type.GetType("System.Decimal"));
            table.Columns.Add("SkidHeightUnit", Type.GetType("System.String"));
            table.Columns.Add("SkidIsCHEP", Type.GetType("System.Boolean"));
            table.Columns.Add("SkidIsOnHold", Type.GetType("System.Boolean"));

            this.DataViewHdrs = table.DefaultView;
            this.DataViewHdrs.Table.TableName = this.MainGridMappingName;

            return this.DataViewHdrs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search( string searchText)
        {
            // TODO: Search
            throw new NotImplementedException();
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            // TODO: Design Grid
            throw new NotImplementedException();
        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="dataGridMain"></param>
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
//            dataGridCol.MappingName = "SkidCode";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Codezz);
//#else
//            dataGridCol.HeaderText = "Code";
//#endif
//            dataGridCol.Width = 200 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ProductCode";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = 80 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ProductUnitBC";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = 20 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "Quantity";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
//#else
//            dataGridCol.HeaderText = "Qty";
//#endif
//            dataGridCol.Width = 52 * colWidthFactor;
//            dataGridCol.Format = "0.000";
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "UnitCode";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = 30 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "WeightQuantity";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
//#else
//            dataGridCol.HeaderText = "Weight";
//#endif
//            dataGridCol.Width = 52 * colWidthFactor;
//            dataGridCol.Format = "0.000";
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "WeightUnitCode";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = 30 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "LocationName";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = 150 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridMain.TableStyles.Add(dataGridStyle);
//            dataGridMain.RowHeadersVisible = false;
//            dataGridMain.ColumnHeadersVisible = true;
//        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string GetFilter(XDocument data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
