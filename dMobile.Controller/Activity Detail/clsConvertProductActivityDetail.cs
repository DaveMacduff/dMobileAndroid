namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Text;
    using System.Xml;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;

    public class ConvertProductActDetail : ActDetail
    {

        #region Fields

        int _ID;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_actItem"></param>
        /// <remarks></remarks>
        public ConvertProductActDetail(ActItem _actItem)
            : base(_actItem)
        {
            ID = 0;
            this.FullClassName = typeof(ConvertProductActDetail).FullName;
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
            if (this.DataViewDetails != null)
            {
                // Update Detail data
                this.Quantity = Convert.ToDecimal(data.Root.Element("alt_qty").Value);
                this.BaseQuantity = Convert.ToDecimal(data.Root.Element("base_qty").Value);
                this.WeightQuantity = Convert.ToDecimal(data.Root.Element("wt_qty").Value);
                this.FKRefno = Convert.ToInt32(data.Root.Element("ser_refno").Value);
                this.ActItem.FKRefno = Convert.ToInt32(data.Root.Element("itm_refno").Value);
                this.ActItem.ActHdr.FKRefno = Convert.ToInt32(data.Root.Element("hdr_refno").Value);

                // Update Scheme data
                SProductSchemeData schemeData = new SProductSchemeData();
                schemeData.LogisticUnitProductionDate = Convert.ToDateTime(data.Root.Element("pdate").Value);
                schemeData.LogisticUnitBestBeforeSellByDate = Convert.ToDateTime(data.Root.Element("bdate").Value);
                schemeData.SingleTradeItemSerialNumber = Convert.ToString(data.Root.Element("serial").Value);
                schemeData.LogisticUnitBatchLotNumber = Convert.ToString(data.Root.Element("lot_no").Value);
                schemeData.VariantCode = Convert.ToString(data.Root.Element("variant_code").Value);
                schemeData.VariantNumber = Convert.ToInt32(data.Root.Element("variant_no").Value);
                this.SchemeData = schemeData;

                DataRowView rowView = this.DataViewDetails.AddNew();
                rowView["ProductRefno"] = this.ActItem.ProdInfo.Refno;
                rowView["ActivityDetailQuantity"] = Math.Round(this.Quantity, 3);
                rowView["ActivityDetailBaseQuantity"] = Math.Round(this.BaseQuantity, 3);
                rowView["ActivityDetailWeightQuantity"] = Math.Round(this.WeightQuantity, 3);
                rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"] = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                rowView["ActivityDetailLogisticUnitProductionDate"] = this.SchemeData.LogisticUnitProductionDate;
                rowView["ActivityDetailSingleTradeItemSerialNumber"] = this.SchemeData.SingleTradeItemSerialNumber;
                rowView["ActivityDetailLogisticUnitBatchLotNumber"] = this.SchemeData.LogisticUnitBatchLotNumber;
                rowView["ProductSchemeVariantCode"] = this.SchemeData.VariantCode;
                rowView["ProductSchemeVariantNumber"] = this.SchemeData.VariantNumber;
                rowView["ActivityDetailFKRefno"] = this.FKRefno;
                rowView["ActivityItemFKRefno"] = this.ActItem.FKRefno;
                rowView["ActivityHdrFKRefno"] = this.ActItem.ActHdr.FKRefno;

                rowView.EndEdit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Delete()
        {
            if (this.DataViewDetails != null)
            {
                while (this.DataViewDetails.Count > 0 &&
                    this.ActItem.CurrRowIdx >= 0 &&
                    this.ActItem.CurrRowIdx < this.DataViewDetails.Count)
                {
                    this.DataViewDetails.Delete(this.ActItem.CurrRowIdx);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="data"></param>
        public override void Submit(int rowIndex, XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        public override int GetRefno(int rowIdx)
        {
            return (int)this.DataViewDetails[rowIdx]["ActivityDetailRefno"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        public override int GetFKRefno(int rowIdx)
        {
            return (int)this.DataViewDetails[rowIdx]["ActivityDetailCountFKRefno"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            DataTable table = new DataTable("ActivityConvertProductItem");


            table.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantNumber", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityItemFKRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityHdrFKRefno", Type.GetType("System.Int32"));
            
            this.DataViewDetails = table.DefaultView;
            this.DataViewDetails.Table.TableName = this.MainGridMappingName;

            return this.DataViewDetails;
        }
        public override ActDetail GetInfo(int idx, bool isAdd)
        {
            base.GetInfo(idx, isAdd);

            if (!isAdd)
            {
                this.FKRefno = (int)DataViewDetails[idx]["ActivityDetailFKRefno"];
            }

            return this;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            throw new NotImplementedException();
        }

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <param name="dataGridMain"></param>
        //        /// <remarks></remarks>
        //        public override void DesignDataGrid(DataGrid dataGridMain)
        //        {
        //            int columnWidthFactor = 1;

        //            if (this.SysEnv.IsWindowsMobile6Device)
        //            {
        //                columnWidthFactor = 2;
        //            }

        //            if (dataGridMain.TableStyles.Count > 0)
        //            {
        //                dataGridMain.TableStyles.Clear();
        //            }

        //            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //            dataGridStyle.MappingName = this.MainGridMappingName;


        //            dataGridStyle.GridColumnStyles.Add(
        //                CreateColumn(
        //                "ActivityDetailQuantity",
        //#if MULTILANGUAGE
        //                Translator.GetInstance().Translate(TranslatorVocabulary.Quantity),
        //#else
        //                "Quantity",
        //#endif
        //                80 * columnWidthFactor,
        //                "0.000"
        //                ));

        //            dataGridStyle.GridColumnStyles.Add(
        //                CreateColumn(
        //                "ActivityDetailWeightQuantity",
        //#if MULTILANGUAGE
        //                Translator.GetInstance().Translate(TranslatorVocabulary.Weight),
        //#else
        //                "Weight",
        //#endif
        //                80 * columnWidthFactor,
        //                "0.000"
        //                ));

        //            if (this.ActItem.ProdInfo.Scheme.IsSerialTracked)
        //            {
        //                dataGridStyle.GridColumnStyles.Add(
        //                    CreateColumn(
        //                    "ActivityDetailSingleTradeItemSerialNumber",
        //#if MULTILANGUAGE
        //                    Translator.GetInstance().Translate(TranslatorVocabulary.Serialno),
        //#else
        //                    "SerialNo",
        //#endif
        //                    60 * columnWidthFactor,
        //                    null
        //                    ));
        //            }

        //            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
        //            {
        //                dataGridStyle.GridColumnStyles.Add(
        //                    CreateColumn(
        //                    "ActivityDetailLogisticUnitProductionDate",
        //#if MULTILANGUAGE
        //                    Translator.GetInstance().Translate(TranslatorVocabulary.ProdDate),
        //#else
        //                    "Prod Date",
        //#endif
        //                    60 * columnWidthFactor,
        //                    null
        //                    ));
        //            }
        //            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
        //            {
        //                dataGridStyle.GridColumnStyles.Add(
        //                    CreateColumn(
        //                    "ActivityDetailLogisticUnitBestBeforeSellByDate",
        //#if MULTILANGUAGE
        //                    Translator.GetInstance().Translate(TranslatorVocabulary.BbsbDate),
        //#else
        //                    "BBSB Date",
        //#endif
        //                    60 * columnWidthFactor,
        //                    null
        //                    ));
        //            }
        //            if (this.ActItem.ProdInfo.Scheme.IsLotTracked)
        //            {
        //                dataGridStyle.GridColumnStyles.Add(
        //                    CreateColumn(
        //                    "ActivityDetailLogisticUnitBatchLotNumber",
        //#if MULTILANGUAGE
        //                    Translator.GetInstance().Translate(TranslatorVocabulary.Lot),
        //#else
        //                    "Lot",
        //#endif
        //                    60 * columnWidthFactor,
        //                    null
        //                    ));
        //            }
        //            if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
        //            {
        //                dataGridStyle.GridColumnStyles.Add(
        //                    CreateColumn(
        //                    "ProductSchemeVariantCode",
        //#if MULTILANGUAGE
        //                    Translator.GetInstance().Translate(TranslatorVocabulary.Variant),
        //#else
        //                    "Variant",
        //#endif
        //                    60 * columnWidthFactor,
        //                    null
        //                    ));
        //            }


        //            dataGridMain.TableStyles.Add(dataGridStyle);
        //            dataGridMain.RowHeadersVisible = false;
        //            dataGridMain.ColumnHeadersVisible = true;
        //        }

        //        private static DataGridTextBoxColumn CreateColumn(string mappingName, string header, int width, string format)
        //        {
        //            DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //            dataGridCol.MappingName = mappingName;
        //            dataGridCol.HeaderText = header;
        //            dataGridCol.Width = width;
        //            if (format != null)
        //            {
        //                dataGridCol.Format = format;
        //            }
        //            return dataGridCol;
        //        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectDetail(int idx)
        {
            if (this.DataViewDetails != null && idx >= 0 && idx < this.DataViewDetails.Count)
            {
                DataRowView rowView = this.DataViewDetails[idx];
                // Update Detail data
                this.ActItem.ProdInfo.Refno = Convert.ToInt32(rowView["ProductRefno"]);
                this.Quantity = Convert.ToDecimal(rowView["ActivityDetailQuantity"]);
                this.BaseQuantity = Convert.ToDecimal(rowView["ActivityDetailBaseQuantity"]);
                this.WeightQuantity = Convert.ToDecimal(rowView["ActivityDetailWeightQuantity"]);

                //this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                //// Update Scheme data
                //SProductSchemeData schemeData = new SProductSchemeData();
                //schemeData.LogisticUnitProductionDate = Convert.ToDateTime(rowView["ActivityDetailLogisticUnitProductionDate"]);
                //schemeData.LogisticUnitBestBeforeSellByDate = Convert.ToDateTime(rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"]);
                //schemeData.SingleTradeItemSerialNumber = Convert.ToString(rowView["ActivityDetailSingleTradeItemSerialNumber"]);
                //schemeData.LogisticUnitBatchLotNumber = Convert.ToString(rowView["ActivityDetailLogisticUnitBatchLotNumber"]);
                //schemeData.VariantCode = Convert.ToString(rowView["ProductSchemeVariantCode"]);
                //schemeData.VariantNumber = Convert.ToInt32(rowView["ProductSchemeVariantNumber"]);
                //this.SchemeData = schemeData;
                
                // Update Refnos
                this.FKRefno = Convert.ToInt32(rowView["ActivityDetailFKRefno"]);
                //this.ActItem.FKRefno = Convert.ToInt32(rowView["ActivityItemFKRefno"]);
                //this.ActItem.ActHdr.FKRefno = Convert.ToInt32(rowView["ActivityHdrFKRefno"]);                
            }
        }

        /// <summary>
        /// Creates a request string to remove the fk_refno.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateDeleteXML(XDocument data)
        {
            // Expecting fk_refno as input
            //int fkRefno = Convert.ToInt32(data.Root.Element("fk_refno").Value);
            // or just append an xml line to the request

            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            // Set the FK refno to be the selected row
            int idx = this.ActItem.CurrRowIdx;
            this.ActItem.ActHdr.FKRefno = (int)DataViewDetails.Table.Rows[idx]["ActivityHdrFKRefno"];
            this.ActItem.FKRefno = (int)DataViewDetails.Table.Rows[idx]["ActivityItemFKRefno"];
            this.FKRefno = (int)DataViewDetails.Table.Rows[idx]["ActivityDetailFKRefno"];

            XDocument request = null;
            request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                  + "<imclient>"
                  + "<command>"
                  + "<action>DELETE ADJUSTMENT DETAIL</action>"
                  + "<whs_id>0</whs_id>"
                  + "<sect_refno>0</sect_refno>"
                  + "</command>"
                  + SysCommon.GetRequestDefaults(false)
                  + this.GetRequestDetailDefaults()
                  + ("<adjustment_type>" + SysCommon.AdjTypes.ConvertProduct + "</adjustment_type>")
                  + "<skid_fk_refno>0</skid_fk_refno>"
                  + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        
        /// <summary>
        /// Creates XML for creating a product label.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitProductLabelXML(XDocument data)
        {
            PrinterSettings prnSettings = new PrinterSettings().LoadData();
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = null;

            request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRINT PRODUCT LABEL</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + this.GetRequestDetailDefaults()
                + "<adjustment_type>" + SysCommon.AdjTypes.ConvertProduct + "</adjustment_type>"
                + "<mobile_printer_name>"
                + (prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty)
                + "</mobile_printer_name>"
                + "</imclient>");


            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Creates xml that submits the conversion between products
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            int fromProdRefno = Convert.ToInt32(this.ActItem.DataViewItems[0]["from_prod_fk_refno"]);
            int toProdRefno = Convert.ToInt32(this.ActItem.DataViewItems[0]["to_prod_fk_refno"]);

            ProductInfo fromProd = ProductInfoTasks.GetInfoFromRefno(fromProdRefno);
            ProductInfo toProd = ProductInfoTasks.GetInfoFromRefno(toProdRefno);
            XDocument request = null;

            DateTime prodDate = Convert.ToDateTime(data.Root.Element("pdate").Value);
            DateTime bestBeforeDate = Convert.ToDateTime(data.Root.Element("bdate").Value);

            string serial = data.Root.Element("serial").Value;
            string pdate = prodDate.ToString("MM/dd/yyyy");
            string bdate = bestBeforeDate.ToString("MM/dd/yyyy");
            string lotno = data.Root.Element("lot_no").Value;
            string varcode = data.Root.Element("variant_code").Value;
            string varno = data.Root.Element("variant_no").Value;

            string qty = data.Root.Element("alt_qty").Value;
            string wtQty = data.Root.Element("wt_qty").Value;
            string baseQty = data.Root.Element("base_qty").Value;

            StringBuilder req = new StringBuilder();
            req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            req.Append("<imclient>");
            req.Append("<command><action>SUBMIT CONVERT PRODUCT</action></command>");
            req.Append(SysCommon.GetRequestDefaults(false));
            req.Append("<from>");
            req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            req.AppendFormat("<hdr_refno>{0}</hdr_refno>", this.ActItem.ActHdr.FKRefno);
            req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
            req.AppendFormat("<prod_id>{0}</prod_id>", fromProd.Code);
            req.AppendFormat("<bc>{0}</bc>", fromProd.UnitBC);
            req.AppendFormat("<alt_unit>{0}</alt_unit>", fromProd.UnitCode);
            req.AppendFormat("<wt_unit>{0}</wt_unit>", fromProd.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", serial);
            req.AppendFormat("<pdate>{0}</pdate>", pdate);
            req.AppendFormat("<bdate>{0}</bdate>", bdate);
            req.AppendFormat("<lot_no>{0}</lot_no>", lotno);
            req.AppendFormat("<variant_code>{0}</variant_code>", varcode);
            req.AppendFormat("<variant_no>{0}</variant_no>", varno);
            req.AppendFormat("<alt_qty>{0}</alt_qty>", qty);
            req.AppendFormat("<wt_qty>{0}</wt_qty>", wtQty);
            req.AppendFormat("<base_qty>{0}</base_qty>", baseQty);
            req.AppendFormat("<base_unit>{0}</base_unit>", fromProd.BaseUnitCode);
            req.Append("</from>");

            
            req.Append("<to>");
            req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            req.AppendFormat("<hdr_refno>{0}</hdr_refno>", this.ActItem.ActHdr.FKRefno);
            req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
            req.AppendFormat("<prod_id>{0}</prod_id>", toProd.Code);
            req.AppendFormat("<bc>{0}</bc>", toProd.UnitBC);
            req.AppendFormat("<alt_unit>{0}</alt_unit>", toProd.UnitCode);
            req.AppendFormat("<wt_unit>{0}</wt_unit>", toProd.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", serial);
            req.AppendFormat("<pdate>{0}</pdate>", pdate);
            req.AppendFormat("<bdate>{0}</bdate>", bdate);
            req.AppendFormat("<lot_no>{0}</lot_no>", lotno);
            req.AppendFormat("<variant_code>{0}</variant_code>", varcode);
            req.AppendFormat("<variant_no>{0}</variant_no>", varno);
            req.AppendFormat("<alt_qty>{0}</alt_qty>", qty);
            req.AppendFormat("<wt_qty>{0}</wt_qty>", wtQty);
            req.AppendFormat("<base_qty>{0}</base_qty>", baseQty);
            req.AppendFormat("<base_unit>{0}</base_unit>", toProd.BaseUnitCode);
            req.Append("</to>");

            req.Append("</imclient>");

            request = XDocument.Parse(req.ToString());

            request.Save(xml);
            xml.Flush();
            xml.Close();


            return xmlString.ToString();
        }
        #endregion
    }
}
