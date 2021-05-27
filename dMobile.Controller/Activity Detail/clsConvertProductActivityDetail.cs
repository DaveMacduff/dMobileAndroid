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
    using dMobile.Resources.Types;
    using dMobile.Resources.Services;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;

    public class ConvertProductActDetail : ActDetail
    {
        #region Fields

        int id;
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private double columnWidth6 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<ConvertProductDetail> listConvertProductDetails = new List<ConvertProductDetail>();

        public int ID
        {
            get { return id; }
            set { id = value; }
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

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth6, GridUnitType.Absolute) },
                    }
                };

                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSerialNo = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProdDate = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelBBSBDate = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLot = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelVariant = new Label { VerticalOptions = LayoutOptions.Center };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelSerialNo.SetBinding(Label.TextProperty, new Binding("ActivityDetailSingleTradeItemSerialNumber"));
                labelProdDate.SetBinding(Label.TextProperty, new Binding("ActivityDetailLogisticUnitProductionDate", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelBBSBDate.SetBinding(Label.TextProperty, new Binding("ActivityDetailLogisticUnitBestBeforeSellByDate", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelLot.SetBinding(Label.TextProperty, new Binding("ActivityDetailLogisticUnitBatchLotNumber"));
                labelVariant.SetBinding(Label.TextProperty, new Binding("ProductSchemeVariantCode"));

                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWeight);

                Grid.SetColumn(labelQty, 0);
                Grid.SetColumn(labelWeight, 1);

                if (this.ActItem.ProdInfo.Scheme.IsSerialTracked)
                {
                    gridData.Children.Add(labelSerialNo);
                    Grid.SetColumn(labelSerialNo, 2);
                }

                if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    gridData.Children.Add(labelProdDate);
                    Grid.SetColumn(labelProdDate, 3);
                }

                if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    gridData.Children.Add(labelBBSBDate);
                    Grid.SetColumn(labelBBSBDate, 4);
                }

                if (this.ActItem.ProdInfo.Scheme.IsLotTracked)
                {
                    gridData.Children.Add(labelLot);
                    Grid.SetColumn(labelLot, 5);
                }

                if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
                {
                    gridData.Children.Add(labelVariant);
                    Grid.SetColumn(labelVariant, 6);
                }

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override Task InsertSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override Task DeleteSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override async Task Insert(XDocument data)
        {
            if (this.ListDetails != null)
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

                this.ListDetails.Add(new ConvertProductDetail()
                {
                    ProductRefno = this.ActItem.ProdInfo.Refno,
                    ActivityDetailQuantity = Math.Round(this.Quantity, 3),
                    ActivityDetailBaseQuantity = Math.Round(this.BaseQuantity, 3),
                    ActivityDetailWeightQuantity = Math.Round(this.WeightQuantity, 3),
                    ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate,
                    ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate,
                    ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber,
                    ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber,
                    ProductSchemeVariantCode = this.SchemeData.VariantCode,
                    ProductSchemeVariantNumber = this.SchemeData.VariantNumber,
                    ActivityDetailFKRefno = this.FKRefno,
                    ActivityItemFKRefno = this.ActItem.FKRefno,
                    ActivityHdrFKRefno = this.ActItem.ActHdr.FKRefno
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task Delete()
        {
            if (this.ListDetails != null)
            {
                this.ListDetails.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="data"></param>
        public override Task Submit(object selectedActivityDetail, XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Task Refresh()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return Convert.ToInt32((selectedActivityDetail as dynamic).ActivityDetailRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            return Convert.ToInt32((selectedActivityDetail as dynamic).ActivityDetailCountFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
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

            this.listConvertProductDetails = table.Rows.Cast<ConvertProductDetail>().ToList();
            this.ListDetails = this.listConvertProductDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        public override async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            await base.GetInfo(selectedActivityDetail, isAdd);

            if (!isAdd && selectedActivityDetail is ConvertProductDetail convertProductDetail)
            {
                this.FKRefno = Convert.ToInt32(convertProductDetail.ActivityDetailFKRefno);
            }

            return this;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (ConvertProductDetail convertProductDetail in this.ListDetails)
            {
                //ConvertProductDetail convertProductDetail = new ConvertProductDetail
                //{
                //    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                //    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                //    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                //    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                //    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                //    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                //    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                //    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                //    ActivityDetailFKRefno = Convert.ToInt32(row["ActivityDetailFKRefno"] != DBNull.Value ? row["ActivityDetailFKRefno"] : null),
                //    ActivityHdrFKRefno = Convert.ToInt32(row["ActivityHdrFKRefno"] != DBNull.Value ? row["ActivityHdrFKRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductSchemeVariantNumber = Convert.ToInt32(row["ProductSchemeVariantNumber"] != DBNull.Value ? row["ProductSchemeVariantNumber"] : null)
                //};

                observableCollection.Add(convertProductDetail);
            }

            // TODO: Resource strings
            Label labelQtyHeader = new Label { Text = "Quantity", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSerialNoHeader = new Label { Text = "SerialNo", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProdDateHeader = new Label { Text = "Prod Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBBSBDateHeader = new Label { Text = "BBSB Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLotHeader = new Label { Text = "Lot", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelVariantHeader = new Label { Text = "Variant", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSerialNoHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProdDateHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBBSBDateHeader.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLotHeader.Text);
            columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelVariantHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth6, GridUnitType.Absolute) },
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelQtyHeader);
            Grid.SetColumn(labelQtyHeader, 0);
            
            gridHeader.Children.Add(labelWeightHeader);
            Grid.SetColumn(labelWeightHeader, 1);

            if (this.ActItem.ProdInfo.Scheme.IsSerialTracked)
            {
                gridHeader.Children.Add(labelSerialNoHeader);
                Grid.SetColumn(labelSerialNoHeader, 2);
            }

            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
            {
                gridHeader.Children.Add(labelProdDateHeader);
                Grid.SetColumn(labelProdDateHeader, 3);
            }

            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                gridHeader.Children.Add(labelBBSBDateHeader);
                Grid.SetColumn(labelBBSBDateHeader, 4);
            }

            if (this.ActItem.ProdInfo.Scheme.IsLotTracked)
            {
                gridHeader.Children.Add(labelLotHeader);
                Grid.SetColumn(labelLotHeader, 5);
            }

            if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
            {
                gridHeader.Children.Add(labelVariantHeader);
                Grid.SetColumn(labelVariantHeader, 6);
            }

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (ConvertProductDetail convertProductDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", convertProductDetail.ActivityDetailQuantity));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", convertProductDetail.ActivityDetailWeightQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(convertProductDetail.ActivityDetailSingleTradeItemSerialNumber);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(convertProductDetail.ActivityDetailLogisticUnitProductionDate.ToString("MM/dd/yyyy")));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(convertProductDetail.ActivityDetailLogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy")));
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(convertProductDetail.ActivityDetailLogisticUnitBatchLotNumber);
                double detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(convertProductDetail.ProductSchemeVariantCode);

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (this.ActItem.ProdInfo.Scheme.IsSerialTracked)
                {
                    if (detailWidth2 > columnWidth2)
                    {
                        columnWidth2 = detailWidth2;
                        gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                    }
                } 
                else
                {
                    columnWidth2 = 0;
                    gridHeader.ColumnDefinitions[2].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    if (detailWidth3 > columnWidth3)
                    {
                        columnWidth3 = detailWidth3;
                        gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                    }
                }
                else
                {
                    columnWidth3 = 0;
                    gridHeader.ColumnDefinitions[3].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    if (detailWidth4 > columnWidth4)
                    {
                        columnWidth4 = detailWidth4;
                        gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                    }
                }
                else
                {
                    columnWidth4 = 0;
                    gridHeader.ColumnDefinitions[4].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsLotTracked)
                {
                    if (detailWidth5 > columnWidth5)
                    {
                        columnWidth5 = detailWidth5;
                        gridHeader.ColumnDefinitions[5].Width = columnWidth5;
                    }
                }
                else
                {
                    columnWidth5 = 0;
                    gridHeader.ColumnDefinitions[5].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
                {
                    if (detailWidth6 > columnWidth6)
                    {
                        columnWidth6 = detailWidth6;
                        gridHeader.ColumnDefinitions[6].Width = columnWidth6;
                    }
                }
                else
                {
                    columnWidth6 = 0;
                    gridHeader.ColumnDefinitions[6].Width = 0;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

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
        public override async Task SelectActivityDetail(object selectedActivityDetail)
        {
            if (selectedActivityDetail is ConvertProductDetail convertProductDetail)
            {
                // Update Detail data
                this.ActItem.ProdInfo.Refno = Convert.ToInt32(convertProductDetail.ProductRefno);
                this.Quantity = Convert.ToDecimal(convertProductDetail.ActivityDetailQuantity);
                this.BaseQuantity = Convert.ToDecimal(convertProductDetail.ActivityDetailBaseQuantity);
                this.WeightQuantity = Convert.ToDecimal(convertProductDetail.ActivityDetailWeightQuantity);
                
                // Update Refnos
                this.FKRefno = Convert.ToInt32(convertProductDetail.ActivityDetailFKRefno);          
            }
        }

        /// <summary>
        /// Creates a request string to remove the fk_refno.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<string> CreateDeleteXML(XDocument data)
        {
            // Expecting fk_refno as input
            //int fkRefno = Convert.ToInt32(data.Root.Element("fk_refno").Value);
            // or just append an xml line to the request

            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            // Set the FK refno to be the selected row
            dynamic selectedActivityItem = this.ActItem.SelectedActivityItem;
            this.ActItem.ActHdr.FKRefno = Convert.ToInt32(selectedActivityItem.ActivityHdrFKRefno);
            this.ActItem.FKRefno = Convert.ToInt32(selectedActivityItem.ActivityItemFKRefno);
            this.FKRefno = Convert.ToInt32(selectedActivityItem.ActivityDetailFKRefno);

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
        public override async Task<string> CreateSubmitProductLabelXML(XDocument data)
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
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            int fromProdRefno = Convert.ToInt32((this.ActItem.SelectedActivityItem as ConvertProductItem).From_prod_fk_refno);
            int toProdRefno = Convert.ToInt32((this.ActItem.SelectedActivityItem as ConvertProductItem).To_prod_fk_refno);

            ProductInfo fromProd = await ProductInfoTasks.GetInfoFromRefno(fromProdRefno);
            ProductInfo toProd = await ProductInfoTasks.GetInfoFromRefno(toProdRefno);
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
