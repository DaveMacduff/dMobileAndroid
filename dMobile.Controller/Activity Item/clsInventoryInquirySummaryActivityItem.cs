namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;
    using dMobile.Resources.Types;
    using dMobile.Resources.Services;

    public class InvInquirySummaryActItem : ActItem
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="activity"></param>
        public InvInquirySummaryActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(CycleActItem).FullName;
            this.IsAskCreateNewItem = false;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override int UpdateDataViewQuantity(int rowIdx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            if (prodUnitRefno != 0)
            {
                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // Add all items in dataview to observable collection for display
            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                ActivityItem activityItem = new ActivityItem
                {
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    WeightUnitCode = Convert.ToString(row["WeightUnitCode"] != DBNull.Value ? row["WeightUnitCode"] : null),
                    ActivityDetailLogisticUnitProductionDate = Convert.ToString(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                    ActivityDetailLogisticUnitBestbeforesellbydate = Convert.ToString(row["ActivityDetailLogisticUnitBestbeforesellbydate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestbeforesellbydate"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToString(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapSectionFKRefno = Convert.ToString(row["LocationMapSectionFKRefno"] != DBNull.Value ? row["LocationMapSectionFKRefno"] : null),
                    LocationMapSlotFKRefno = Convert.ToString(row["LocationMapSlotFKRefno"] != DBNull.Value ? row["LocationMapSlotFKRefno"] : null),
                    SkidCode = Convert.ToString(row["SkidCode"] != DBNull.Value ? row["SkidCode"] : null),
                    ActivityItemQuantity = Convert.ToDecimal(row["ActivityItemQuantity"] != DBNull.Value ? row["ActivityItemQuantity"] : null),
                    ActivityItemWeightQuantity = Convert.ToDecimal(row["ActivityItemWeightQuantity"] != DBNull.Value ? row["ActivityItemWeightQuantity"] : null),
                };

                observableCollection.Add(activityItem);
            }

            // TODO: Resource strings
            Label labelProductIDHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBCHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelQuantityHeader = new Label { Text = "Quantity", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProdDateHeader = new Label { Text = "Prod Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBBDateHeader = new Label { Text = "BB/SB Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSectionHeader = new Label { Text = "Section", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSlotHeader = new Label { Text = "Slot", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSkidHeader = new Label { Text = "Skid", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIDHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBCHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQuantityHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            double columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProdDateHeader.Text);
            double columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBBDateHeader.Text);
            double columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSectionHeader.Text);
            double columnWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSlotHeader.Text);
            double columnWidth8 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSkidHeader.Text);
            double columnWidth9 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            double columnWidth10 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightUnitCodeHeader.Text);

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
                    new ColumnDefinition { Width = new GridLength(columnWidth7, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth8, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth9, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth10, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // Add labels to grid
            gridHeader.Children.Add(labelProductIDHeader);
            gridHeader.Children.Add(labelBCHeader);
            gridHeader.Children.Add(labelQuantityHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);

            if (this.ProdInfo.Scheme.IsProductionDateTracked)
            {
                gridHeader.Children.Add(labelProdDateHeader);
            }

            if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                gridHeader.Children.Add(labelBBDateHeader);
            }

            if (this.LocMap.CurrLocMap.IsSectionTracked)
            {
                gridHeader.Children.Add(labelSectionHeader);
            }

            if (this.LocMap.CurrLocMap.IsSectionTracked && this.LocMap.CurrLocMap.IsSlotTracked)
            {
                gridHeader.Children.Add(labelSlotHeader);
            }

            gridHeader.Children.Add(labelSkidHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelWeightUnitCodeHeader);

            // Set label columns
            Grid.SetColumn(labelProductIDHeader, 0);
            Grid.SetColumn(labelBCHeader, 1);
            Grid.SetColumn(labelQuantityHeader, 2);
            Grid.SetColumn(labelUnitCodeHeader, 3);

            if (this.ProdInfo.Scheme.IsProductionDateTracked)
            {
                Grid.SetColumn(labelProdDateHeader, 4);
            }

            if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                Grid.SetColumn(labelBBDateHeader, 5);
            }

            if (this.LocMap.CurrLocMap.IsSectionTracked)
            {
                Grid.SetColumn(labelSectionHeader, 6);
            }

            if (this.LocMap.CurrLocMap.IsSectionTracked && this.LocMap.CurrLocMap.IsSlotTracked)
            {
                Grid.SetColumn(labelSlotHeader, 7);
            }

            Grid.SetColumn(labelSkidHeader, 8);
            Grid.SetColumn(labelWeightHeader, 9);
            Grid.SetColumn(labelWeightUnitCodeHeader, 10);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (ActivityItem activityItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ProductUnitBC);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", activityItem.ActivityItemQuantity));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.UnitCode);
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ActivityDetailLogisticUnitProductionDate);
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ActivityDetailLogisticUnitBestbeforesellbydate);
                double detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.LocationMapSectionFKRefno);
                double detailWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.LocationMapSlotFKRefno);
                double detailWidth8 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.SkidCode);
                double detailWidth9 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", activityItem.ActivityItemWeightQuantity));
                double detailWidth10 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.WeightUnitCode);

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

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }

                if (this.ProdInfo.Scheme.IsProductionDateTracked)
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
                    gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                }

                if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
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
                    gridHeader.ColumnDefinitions[5].Width = columnWidth5;
                }

                if (this.LocMap.CurrLocMap.IsSectionTracked)
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
                    gridHeader.ColumnDefinitions[6].Width = columnWidth6;
                }

                if (this.LocMap.CurrLocMap.IsSectionTracked && this.LocMap.CurrLocMap.IsSlotTracked)
                {
                    if (detailWidth7 > columnWidth7)
                    {
                        columnWidth7 = detailWidth7;
                        gridHeader.ColumnDefinitions[7].Width = columnWidth7;
                    }
                }
                else
                {
                    columnWidth7 = 0;
                    gridHeader.ColumnDefinitions[7].Width = columnWidth7;
                }

                if (detailWidth8 > columnWidth8)
                {
                    columnWidth8 = detailWidth8;
                    gridHeader.ColumnDefinitions[8].Width = columnWidth8;
                }

                if (detailWidth9 > columnWidth9)
                {
                    columnWidth9 = detailWidth9;
                    gridHeader.ColumnDefinitions[9].Width = columnWidth9;
                }

                if (detailWidth10 > columnWidth10)
                {
                    columnWidth10 = detailWidth10;
                    gridHeader.ColumnDefinitions[10].Width = columnWidth10;
                }
            }

            // Create item template
            DataTemplate dataTemplateCount = new DataTemplate(() =>
            {
                Grid gridData = new Grid
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
                        new ColumnDefinition { Width = new GridLength(columnWidth7, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth8, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth9, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth10, GridUnitType.Absolute) }
                    }
                };

                Label labelProductID = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelBC = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelQuantity = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProdDate = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelBBDate = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSection = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSlot = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSkid = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWeightUnitCode = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelProductID.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelQuantity.SetBinding(Label.TextProperty, new Binding("ActivityItemQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelProdDate.SetBinding(Label.TextProperty, new Binding("ActivityDetailLogisticUnitProductionDate"));
                labelBBDate.SetBinding(Label.TextProperty, new Binding("ActivityDetailLogisticUnitBestbeforesellbydate"));
                labelSection.SetBinding(Label.TextProperty, new Binding("LocationMapSectionFKRefno"));
                labelSlot.SetBinding(Label.TextProperty, new Binding("LocationMapSlotFKRefno"));
                labelSkid.SetBinding(Label.TextProperty, new Binding("SkidCode"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityItemWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeightUnitCode.SetBinding(Label.TextProperty, new Binding("WeightUnitCode"));

                // Add labels to grid
                gridData.Children.Add(labelProductID);
                gridData.Children.Add(labelBC);
                gridData.Children.Add(labelQuantity);
                gridData.Children.Add(labelUnitCode);

                if (this.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    gridData.Children.Add(labelProdDate);
                }

                if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    gridData.Children.Add(labelBBDate);
                }

                if (this.LocMap.CurrLocMap.IsSectionTracked)
                {
                    gridData.Children.Add(labelSection);
                }

                if (this.LocMap.CurrLocMap.IsSectionTracked && this.LocMap.CurrLocMap.IsSlotTracked)
                {
                    gridData.Children.Add(labelSlot);
                }

                gridData.Children.Add(labelSkid);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelWeightUnitCode);

                // Set label columns
                Grid.SetColumn(labelProductID, 0);
                Grid.SetColumn(labelBC, 1);
                Grid.SetColumn(labelQuantity, 2);
                Grid.SetColumn(labelUnitCode, 3);

                if (this.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    Grid.SetColumn(labelProdDate, 4);
                }

                if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    Grid.SetColumn(labelBBDate, 5);
                }

                if (this.LocMap.CurrLocMap.IsSectionTracked)
                {
                    Grid.SetColumn(labelSection, 6);
                }

                if (this.LocMap.CurrLocMap.IsSectionTracked && this.LocMap.CurrLocMap.IsSlotTracked)
                {
                    Grid.SetColumn(labelSlot, 7);
                }

                Grid.SetColumn(labelSkid, 8);
                Grid.SetColumn(labelWeight, 9);
                Grid.SetColumn(labelWeightUnitCode, 10);

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplateCount;

            return observableCollection;
        }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="dataGridMain"></param>
//        /// <remarks></remarks>
//        public override void DesignDataGrid(DataGrid dataGridMain)
//        {
//            AddGridColumn(
//                (ProdInfo.Refno > 0) ? -1 : columnWidthFactor * 65,
//                "productcode",
//                "Product ID",
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 10,
//                "productunitbc",
//                string.Empty,
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 60,
//                "activityitemquantity",
//                "Quantity",
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 20,
//                "unitcode",
//                string.Empty,
//                dataGridStyle);


//            if (this.ProdInfo.Scheme.IsProductionDateTracked)
//            {
//                AddGridColumn(
//                    columnWidthFactor * 60,
//                    "activitydetaillogisticunitproductiondate",
//                    "Prod Date",
//                    dataGridStyle);
//            }

//            if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
//            {
//                AddGridColumn(
//                    columnWidthFactor * 60,
//                    "activitydetaillogisticunitbestbeforesellbydate",
//                    "BB/SB Date",
//                    dataGridStyle);
//            }

//            if (this.LocMap.CurrLocMap.IsSectionTracked)
//            {
//                AddGridColumn(
//                    columnWidthFactor * 60,
//                    "locationmapsectionfkrefno",
//                    "Section",
//                    dataGridStyle);

//                if (this.LocMap.CurrLocMap.IsSlotTracked)
//                {
//                    AddGridColumn(
//                        columnWidthFactor * 60,
//                        "locationmapslotfkrefno",
//                        "Slot",
//                        dataGridStyle);
//                }
//            }

//            AddGridColumn(
//                columnWidthFactor * 60,
//                "skidcode",
//                "Skid",
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 60,
//                "activityitemweightquantity",
//                "Weight",
//                dataGridStyle);

//            AddGridColumn(
//                columnWidthFactor * 20,
//                "weightunitcode",
//                string.Empty,
//                dataGridStyle);

//            dataGridMain.TableStyles.Add(dataGridStyle);
//            dataGridMain.RowHeadersVisible = false;
//            dataGridMain.ColumnHeadersVisible = true;
//        }

//        private static void AddGridColumn(
//            int columnWidth,
//            string mappingName,
//            string headerText,
//            DataGridTableStyle dataGridStyle)
//        {
//            DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = mappingName;
//            dataGridCol.HeaderText = headerText;
//            dataGridCol.Width = columnWidth;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);
//        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            DataTable dataTableItems = new DataTable("tActivityItem");

            dataTableItems.Columns.Add("productcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("productunitbc", Type.GetType("System.String"));
            dataTableItems.Columns.Add("unitcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("weightunitcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("activitydetaillogisticunitproductiondate", Type.GetType("System.String"));
            dataTableItems.Columns.Add("activitydetaillogisticunitbestbeforesellbydate", Type.GetType("System.String"));
            dataTableItems.Columns.Add("locationmapwarehousefkrefno", Type.GetType("System.String"));
            dataTableItems.Columns.Add("locationmapsectionfkrefno", Type.GetType("System.String"));
            dataTableItems.Columns.Add("locationmapslotfkrefno", Type.GetType("System.String"));
            dataTableItems.Columns.Add("skidcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("activityitemquantity", Type.GetType("System.Decimal"));
            dataTableItems.Columns.Add("activityitemweightquantity", Type.GetType("System.Decimal"));

            this.DataViewItems = dataTableItems.DefaultView;
            this.DataViewItems.Table.TableName = this.MainGridMappingName;

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
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromProdUnitRefno(int productUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectItem(int idx)
        {
            this.CurrRowIdx = idx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Refresh(XDocument data)
        {

            try
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                string reqData = SysCommon.SubmitComm(this.CreateSubmitXML(data));

                this.DataViewItems = this.GetDataView(false);

                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);
                        throw new Exception(xmlDoc.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        this.DataLayerProgressOutHandler(this, new EventArgs(), 50);

                        using (DataSet ds = new DataSet())
                        {
                            ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                            if (ds.Tables.Count != 0)
                            {
                                this.DataViewItems = ds.Tables["tActivityItem"].DefaultView;
                            }
                        }

                        this.DataLayerProgressOutHandler(this, new EventArgs(), 80);
                    }
                }

                this.DataLayerProgressOutHandler(this, new EventArgs(), 95);
                FormatGrid();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 100);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                throw new Exception("Refresh inventory inquiry summary items failed.", ex);
            }
        }

        /// <summary>
        /// Hides the extra columns, changes the sectionFKRefno and slotFKRefno code to names.
        /// </summary>
        private void FormatGrid()
        {

            if (this.LocMap.CurrLocMap.IsSectionTracked)
            {
                // Format Section column
                // Use a dictionary to only hit the database once per unique section number this call.
                Dictionary<int, string> names = new Dictionary<int, string>();
                int rowCount = this.DataViewItems.Table.Rows.Count;

                for (int i = 0; i < rowCount; ++i)
                {
                    int sectionFKRefno = Convert.ToInt32(this.DataViewItems.Table.Rows[i]["locationmapsectionfkrefno"]);
                    string sectionName;

                    if (names.ContainsKey(sectionFKRefno))
                    {
                        sectionName = names[sectionFKRefno];
                    }
                    else
                    {
                        sectionName = LocationMapTasks.GetSectionName(sectionFKRefno);
                        names.Add(sectionFKRefno, sectionName);
                    }

                    this.DataViewItems.Table.Rows[i]["locationmapsectionfkrefno"] = sectionName;
                }

                names.Clear();
            }

            if (this.LocMap.CurrLocMap.IsSlotTracked)
            {
                // Format Slot column
                // Use a dictionary to only hit the database once per unique slot number this call.
                Dictionary<int, string> names = new Dictionary<int, string>();
                int rowCount = this.DataViewItems.Table.Rows.Count;

                for (int i = 0; i < rowCount; ++i)
                {
                    int slotFKRefno = Convert.ToInt32(this.DataViewItems.Table.Rows[i]["locationmapslotfkrefno"]);
                    string slotName;

                    if (names.ContainsKey(slotFKRefno))
                    {
                        slotName = names[slotFKRefno];
                    }
                    else
                    {
                        slotName = LocationMapTasks.GetSlotName(slotFKRefno);
                        names.Add(slotFKRefno, slotName);
                    }

                    this.DataViewItems.Table.Rows[i]["locationmapslotfkrefno"] = slotName;
                }

                names.Clear();
            }

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

            // WO 15547, WO 13770
            string sectionRefno;
            string slotRefno;

            if (SysEnvironment.GetInstance().SysSettings.InventoryInquirySearchEntireWarehouse)
            {
                sectionRefno = string.Empty;
                slotRefno = string.Empty;
            }
            else
            {
                sectionRefno = this.LocMap.CurrLocMap.SectFKRefno.ToString();
                slotRefno = this.LocMap.CurrLocMap.SlotFKRefno.ToString();
            }

            string sectionXml = "<sect_refno>" + sectionRefno + "</sect_refno>";
            string slotXml = "<slot_refno>" + slotRefno + "</slot_refno>";

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST INVENTORY INQUIRY SUMMARY SHORTCUT</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + this.ProdInfo.Code + "</prod_id>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + sectionXml
                + slotXml
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}