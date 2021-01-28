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
                while (this.DataViewDetails.Count > 0 && this.ActItem.CurrRowIdx >= 0 && this.ActItem.CurrRowIdx < this.DataViewDetails.Count)
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
            return Convert.ToInt32(this.DataViewDetails[rowIdx]["ActivityDetailRefno"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        public override int GetFKRefno(int rowIdx)
        {
            return Convert.ToInt32(this.DataViewDetails[rowIdx]["ActivityDetailCountFKRefno"]);
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
                this.FKRefno = Convert.ToInt32(DataViewDetails[idx]["ActivityDetailFKRefno"]);
            }

            return this;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewDetails.ToTable().Rows)
            {
                ConvertProductDetail convertProductDetail = new ConvertProductDetail
                {
                    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                    ActivityDetailFKRefno = Convert.ToInt32(row["ActivityDetailFKRefno"] != DBNull.Value ? row["ActivityDetailFKRefno"] : null),
                    ActivityHdrFKRefno = Convert.ToInt32(row["ActivityHdrFKRefno"] != DBNull.Value ? row["ActivityHdrFKRefno"] : null),
                    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductSchemeVariantNumber = Convert.ToInt32(row["ProductSchemeVariantNumber"] != DBNull.Value ? row["ProductSchemeVariantNumber"] : null)
                };

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

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSerialNoHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProdDateHeader.Text);
            double columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBBSBDateHeader.Text);
            double columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLotHeader.Text);
            double columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelVariantHeader.Text);

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
                    gridHeader.Children.Add(labelSerialNo);
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
                    gridHeader.Children.Add(labelLot);
                    Grid.SetColumn(labelLot, 5);
                }

                if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
                {
                    gridData.Children.Add(labelVariant);
                    Grid.SetColumn(labelVariant, 6);
                }

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplate;

            return observableCollection;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="dataGridMain"></param>
        ///// <remarks></remarks>
        //public override void DesignDataGrid(DataGrid dataGridMain)
        //{

        //    dataGridStyle.GridColumnStyles.Add("ActivityDetailQuantity", "Quantity");
        //    dataGridStyle.GridColumnStyles.Add("ActivityDetailWeightQuantity", "Weight");

        //    if (this.ActItem.ProdInfo.Scheme.IsSerialTracked)
        //    {
        //        dataGridStyle.GridColumnStyles.Add("ActivityDetailSingleTradeItemSerialNumber", "SerialNo");
        //    }

        //    if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
        //    {
        //        dataGridStyle.GridColumnStyles.Add("ActivityDetailLogisticUnitProductionDate", "Prod Date");
        //    }
        //    if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
        //    {
        //        dataGridStyle.GridColumnStyles.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", "BBSB Date");
        //    }
        //    if (this.ActItem.ProdInfo.Scheme.IsLotTracked)
        //    {
        //        dataGridStyle.GridColumnStyles.Add("ActivityDetailLogisticUnitBatchLotNumber", "Lot");
        //    }

        //    if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
        //    {
        //        dataGridStyle.GridColumnStyles.Add("ProductSchemeVariantCode", "Variant");
        //    }
        //}

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
