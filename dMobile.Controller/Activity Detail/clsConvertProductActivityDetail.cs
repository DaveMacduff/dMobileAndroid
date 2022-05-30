// <copyright file="clsConvertProductActivityDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="ConvertProductActDetail" />.
    /// </summary>
    public class ConvertProductActDetail : ActDetail
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 0 width....
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width....
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width....
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width....
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width....
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 5 width....
        /// </summary>
        public double ColumnWidth5 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 6 width....
        /// </summary>
        public double ColumnWidth6 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary....
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets the ListConvertProductDetails
        /// Gets or sets a list of picking details...
        /// </summary>
        public List<ConvertProductDetail> ListConvertProductDetails { get; set; } = new List<ConvertProductDetail>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertProductActDetail"/> class.
        /// </summary>
        /// <param name="_actItem">.</param>
        public ConvertProductActDetail(ActItem _actItem)
            : base(_actItem)
        {
            this.ID = 0;
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
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth5, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth6, GridUnitType.Absolute) },
                    },
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

            this.ResourceDictionary.Add("dataTemplate", dataTemplate);
        }

        /// <summary>
        /// The InsertSkid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task InsertSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The DeleteSkid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task DeleteSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
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
                    ProductSchemeVariantNo = this.SchemeData.VariantNumber,
                    ActivityDetailFKRefno = this.FKRefno,
                    ActivityItemFKRefno = this.ActItem.FKRefno,
                    ActivityHdrFKRefno = this.ActItem.ActHdr.FKRefno,
                });
            }
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            if (this.ListDetails != null)
            {
                this.ListDetails.Clear();
            }

            return 0;
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Submit(object selectedActivityDetail, XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Refresh()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return Convert.ToInt32((selectedActivityDetail as dynamic).ActivityDetailRefno);
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            return Convert.ToInt32((selectedActivityDetail as dynamic).ActivityDetailCountFKRefno);
        }

        /// <summary>
        /// The GetActivityDetailList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
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

            this.ListConvertProductDetails = table.Rows.Cast<ConvertProductDetail>().ToList();
            this.ListDetails = this.ListConvertProductDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <summary>
        /// The GetInfo.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="isAdd">The isAdd<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{ActDetail}"/>.</returns>
        public override async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            await base.GetInfo(selectedActivityDetail, isAdd);

            if (!isAdd && selectedActivityDetail is ConvertProductDetail convertProductDetail)
            {
                this.FKRefno = Convert.ToInt32(convertProductDetail.ActivityDetailFKRefno);
            }

            return this;
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (ConvertProductDetail convertProductDetail in this.ListDetails)
            {
                observableCollection.Add(convertProductDetail);
            }

            Label labelQtyHeader = new Label { Text = "Quantity", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSerialNoHeader = new Label { Text = "SerialNo", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProdDateHeader = new Label { Text = "Prod Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBBSBDateHeader = new Label { Text = "BBSB Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLotHeader = new Label { Text = "Lot", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelVariantHeader = new Label { Text = "Variant", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSerialNoHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProdDateHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBBSBDateHeader.Text);
            this.ColumnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLotHeader.Text);
            this.ColumnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelVariantHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth6, GridUnitType.Absolute) },
                },
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

            GridMainActivity.Children.Add(gridHeader);

            foreach (ConvertProductDetail convertProductDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", convertProductDetail.ActivityDetailQuantity));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", convertProductDetail.ActivityDetailWeightQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(convertProductDetail.ActivityDetailSingleTradeItemSerialNumber);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(convertProductDetail.ActivityDetailLogisticUnitProductionDate.ToString("MM/dd/yyyy")));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(convertProductDetail.ActivityDetailLogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy")));
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(convertProductDetail.ActivityDetailLogisticUnitBatchLotNumber);
                double detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(convertProductDetail.ProductSchemeVariantCode);

                if (detailWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }

                if (detailWidth1 > this.ColumnWidth1)
                {
                    this.ColumnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.ColumnWidth1;
                }

                if (this.ActItem.ProdInfo.Scheme.IsSerialTracked)
                {
                    if (detailWidth2 > this.ColumnWidth2)
                    {
                        this.ColumnWidth2 = detailWidth2;
                        gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
                    }
                }
                else
                {
                    this.ColumnWidth2 = 0;
                    gridHeader.ColumnDefinitions[2].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    if (detailWidth3 > this.ColumnWidth3)
                    {
                        this.ColumnWidth3 = detailWidth3;
                        gridHeader.ColumnDefinitions[3].Width = this.ColumnWidth3;
                    }
                }
                else
                {
                    this.ColumnWidth3 = 0;
                    gridHeader.ColumnDefinitions[3].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    if (detailWidth4 > this.ColumnWidth4)
                    {
                        this.ColumnWidth4 = detailWidth4;
                        gridHeader.ColumnDefinitions[4].Width = this.ColumnWidth4;
                    }
                }
                else
                {
                    this.ColumnWidth4 = 0;
                    gridHeader.ColumnDefinitions[4].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsLotTracked)
                {
                    if (detailWidth5 > this.ColumnWidth5)
                    {
                        this.ColumnWidth5 = detailWidth5;
                        gridHeader.ColumnDefinitions[5].Width = this.ColumnWidth5;
                    }
                }
                else
                {
                    this.ColumnWidth5 = 0;
                    gridHeader.ColumnDefinitions[5].Width = 0;
                }

                if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
                {
                    if (detailWidth6 > this.ColumnWidth6)
                    {
                        this.ColumnWidth6 = detailWidth6;
                        gridHeader.ColumnDefinitions[6].Width = this.ColumnWidth6;
                    }
                }
                else
                {
                    this.ColumnWidth6 = 0;
                    gridHeader.ColumnDefinitions[6].Width = 0;
                }
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Select the activity detail.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task SelectActivityDetail(object selectedActivityDetail)
        {
            await base.SelectActivityDetail(selectedActivityDetail);
        }

        /// <summary>
        /// Creates a request string to remove the fk_reference number.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateDeleteXML(XDocument data)
        {
            // Expecting fk_refno as input
            // int fkRefno = Convert.ToInt32(data.Root.Element("fk_refno").Value);
            // or just append an xml line to the request
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            // Set the FK refno to be the selected row
            dynamic selectedActivityItem = this.ActItem.SelectedActivityItem;
            this.ActItem.ActHdr.FKRefno = Convert.ToInt32(selectedActivityItem.ActivityHdrFKRefno);
            this.ActItem.FKRefno = Convert.ToInt32(selectedActivityItem.ActivityItemFKRefno);
            this.FKRefno = Convert.ToInt32(selectedActivityItem.ActivityDetailFKRefno);
            XDocument request = XDocument.Parse(
    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
      + "<imclient>"
      + "<command>"
      + "<action>DELETE ADJUSTMENT DETAIL</action>"
      + "<whs_id>0</whs_id>"
      + "<sect_refno>0</sect_refno>"
      + "</command>"
      + await SysCommon.GetRequestDefaults(false)
      + this.GetRequestDetailDefaults()
      + "<adjustment_type>" + SysCommon.AdjTypes.ConvertProduct + "</adjustment_type>"
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
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitProductLabelXML(XDocument data)
        {
            PrinterSettings prnSettings = new PrinterSettings().LoadData();
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request = XDocument.Parse(
    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
    + "<imclient>"
    + "<command>"
    + "<action>SUBMIT PRINT PRODUCT LABEL</action>"
    + "<whs_id>0</whs_id>"
    + "<sect_refno>0</sect_refno>"
    + "</command>"
    + await SysCommon.GetRequestDefaults(false)
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
        /// Creates xml that submits the conversion between products.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            int fromProdRefno = Convert.ToInt32((this.ActItem.ListItems[0] as ConvertProductItem).From_prod_fk_refno);
            int toProdRefno = Convert.ToInt32((this.ActItem.ListItems[0] as ConvertProductItem).To_prod_fk_refno);

            ProductInfo fromProd = await ProductInfoTasks.GetInfoFromRefno(fromProdRefno);
            ProductInfo toProd = await ProductInfoTasks.GetInfoFromRefno(toProdRefno);
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
            req.Append(await SysCommon.GetRequestDefaults(false));
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

            XDocument request = XDocument.Parse(req.ToString());
            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <param name="actDetailFKRefnoFldName">The actDetailFKRefnoFldName<see cref="string"/>.</param>
        /// <returns>The <see cref="ActivityDetail"/>.</returns>
        public override ActivityDetail InsertListDetail(string actDetailFKRefnoFldName)
        {
            ConvertProductDetail detail = null;
            if (this.IsUpdateView && this.ListDetails != null)
            {
                bool detailToBeAdded = false;
                if (this.FKRefno > 0)
                {
                    detail = this.ListDetails.OfType<ConvertProductDetail>().FirstOrDefault(adjDetail =>
                        adjDetail.ActivityDetailFKRefno == this.FKRefno);
                }

                if (detail == null)
                {
                    detail = new ConvertProductDetail();
                    detailToBeAdded = true;
                    detail.ActivityDetailRefno = 0;
                    detail.ProductRefno = this.ActItem.ProdInfo.Refno;
                    detail.ProductUnitRefno = this.ActItem.ProdInfo.UnitRefno;
                    detail.ProductCode = this.ActItem.ProdInfo.Code;
                    detail.ProductUnitBC = this.ActItem.ProdInfo.UnitBC;
                    detail.ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBC;
                    detail.ProductDescription = this.ActItem.ProdInfo.Desc;
                    detail.ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc;
                    detail.UnitCode = this.ActItem.ProdInfo.UnitCode;
                    detail.BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode;
                    detail.WtUnitCode = this.ActItem.ProdInfo.WeightUnitCode;
                    detail.ActivityDetailQuantity = 0;
                    detail.ActivityDetailBaseQuantity = 0;
                    detail.ActivityDetailWeightQuantity = 0;
                    detail.ActDetailFKRefnoFldName = actDetailFKRefnoFldName;
                    detail.ActivityHdrFKRefno = this.ActItem.ActHdr.FKRefno;
                    detail.ActivityItemFKRefno = this.ActItem.FKRefno;
                }

                detail.LocationMapRefno = this.LocMap.CurrLocMapRefno;
                detail.ActivityDetailQuantity = Convert.ToDecimal(detail.ActivityDetailQuantity) +
                    Math.Round(this.Quantity, 3);
                detail.ActivityDetailBaseQuantity = Convert.ToDecimal(detail.ActivityDetailBaseQuantity) +
                    Math.Round(this.BaseQuantity, 3);
                detail.ActivityDetailWeightQuantity = Convert.ToDecimal(detail.ActivityDetailWeightQuantity) +
                    Math.Round(this.WeightQuantity, 3);
                detail.ProductSchemeVariantCode = this.SchemeData.VariantCode;
                detail.ProductSchemeVariantNo = this.SchemeData.VariantNumber;
                detail.ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber;
                detail.ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                detail.ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber;
                detail.ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate;
                detail.ActivityDetailLogisticUnitSkidNumber = this.SchemeData.LogisticUnitSkidNumber;
                detail.ActivityDetailExtra1Text = this.ActExtra.Text1;
                detail.ActivityDetailExtra2Text = this.ActExtra.Text2;
                detail.ActivityDetailExtra3Text = this.ActExtra.Text3;
                detail.ActivityDetailExtra4Text = this.ActExtra.Text4;
                detail.ActivityDetailExtra5Date = this.ActExtra.DateTime1;

                int locCode = 0;
                string locName = string.Empty;

                if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SlotCode;
                    locName = this.LocMap.CurrLocMap.SlotName;
                }
                else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SectCode;
                    locName = this.LocMap.CurrLocMap.SectName;
                }
                else
                {
                    locCode = this.LocMap.CurrLocMap.WhsCode;
                    locName = this.LocMap.CurrLocMap.WhsName;
                }

                detail.LocationCode = locCode;
                detail.LocationName = locName;
                detail.ActivityDetailSchemeID = this.SchemeID;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeID;

                if (detailToBeAdded)
                {
                    this.ListDetails.Add(detail);
                }
            }

            return detail;
        }

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail"/> Activity detail.</param>
        /// <param name="mainActivityExtraInfoItems"><see cref="ObservableCollection(object)"/> Main activity items list.</param>
        /// <returns><see cref="DataTemplate"/> Data template.</returns>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityExtraInfoItems)
        {
            mainActivityExtraInfoItems.Add(actDetail);

            Label labelQtyHeader = new Label { Text = "Quantity", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSerialNoHeader = new Label { Text = "SerialNo", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProdDateHeader = new Label { Text = "Prod Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBBSBDateHeader = new Label { Text = "BBSB Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLotHeader = new Label { Text = "Lot", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelVariantHeader = new Label { Text = "Variant", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSerialNoHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProdDateHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBBSBDateHeader.Text);
            this.ColumnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLotHeader.Text);
            this.ColumnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelVariantHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth6, GridUnitType.Absolute) },
                },
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

            gridMain.Children.Add(gridHeader);

            double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", actDetail.ActivityDetailQuantity));
            double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", actDetail.ActivityDetailWeightQuantity));
            double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ActivityDetailSingleTradeItemSerialNumber);
            double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(actDetail.ActivityDetailLogisticUnitProductionDate.ToString("MM/dd/yyyy")));
            double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(actDetail.ActivityDetailLogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy")));
            double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ActivityDetailLogisticUnitBatchLotNumber);
            double detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ProductSchemeVariantCode);

            if (detailWidth0 > this.ColumnWidth0)
            {
                this.ColumnWidth0 = detailWidth0;
                gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
            }

            if (detailWidth1 > this.ColumnWidth1)
            {
                this.ColumnWidth1 = detailWidth1;
                gridHeader.ColumnDefinitions[1].Width = this.ColumnWidth1;
            }

            if (this.ActItem.ProdInfo.Scheme.IsSerialTracked)
            {
                if (detailWidth2 > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
                }
            }
            else
            {
                this.ColumnWidth2 = 0;
                gridHeader.ColumnDefinitions[2].Width = 0;
            }

            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
            {
                if (detailWidth3 > this.ColumnWidth3)
                {
                    this.ColumnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.ColumnWidth3;
                }
            }
            else
            {
                this.ColumnWidth3 = 0;
                gridHeader.ColumnDefinitions[3].Width = 0;
            }

            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                if (detailWidth4 > this.ColumnWidth4)
                {
                    this.ColumnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.ColumnWidth4;
                }
            }
            else
            {
                this.ColumnWidth4 = 0;
                gridHeader.ColumnDefinitions[4].Width = 0;
            }

            if (this.ActItem.ProdInfo.Scheme.IsLotTracked)
            {
                if (detailWidth5 > this.ColumnWidth5)
                {
                    this.ColumnWidth5 = detailWidth5;
                    gridHeader.ColumnDefinitions[5].Width = this.ColumnWidth5;
                }
            }
            else
            {
                this.ColumnWidth5 = 0;
                gridHeader.ColumnDefinitions[5].Width = 0;
            }

            if (this.ActItem.ProdInfo.Scheme.IsVariantTracked)
            {
                if (detailWidth6 > this.ColumnWidth6)
                {
                    this.ColumnWidth6 = detailWidth6;
                    gridHeader.ColumnDefinitions[6].Width = this.ColumnWidth6;
                }
            }
            else
            {
                this.ColumnWidth6 = 0;
                gridHeader.ColumnDefinitions[6].Width = 0;
            }

            return (DataTemplate)this.ResourceDictionary["dataTemplate"];
        }
    }
}
