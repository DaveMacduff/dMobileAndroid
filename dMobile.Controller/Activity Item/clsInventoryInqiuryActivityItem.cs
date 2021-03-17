namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class InvInquiryActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public InvInquiryActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(CycleActItem).FullName;
            this.IsAskCreateNewItem = false;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
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
                        new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                    }
                };

                Label label1 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label2 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label3 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label4 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label5 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label6 = new Label { VerticalOptions = LayoutOptions.Center };

                label1.SetBinding(Label.TextProperty, new Binding("Productcode"));
                label2.SetBinding(Label.TextProperty, new Binding("Productunitbc"));
                label3.SetBinding(Label.TextProperty, new Binding("Activityitemquantity", BindingMode.Default, null, null, "{0:F3}"));
                label4.SetBinding(Label.TextProperty, new Binding("Unitcode"));
                label5.SetBinding(Label.TextProperty, new Binding("Activityitemweightquantity", BindingMode.Default, null, null, "{0:F3}"));
                label6.SetBinding(Label.TextProperty, new Binding("Weightunitcode"));

                gridData.Children.Add(label1);
                gridData.Children.Add(label2);
                gridData.Children.Add(label3);
                gridData.Children.Add(label4);
                gridData.Children.Add(label5);
                gridData.Children.Add(label6);

                Grid.SetColumn(label1, 0);
                Grid.SetColumn(label2, 1);
                Grid.SetColumn(label3, 2);
                Grid.SetColumn(label4, 3);
                Grid.SetColumn(label5, 4);
                Grid.SetColumn(label6, 5);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
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
        public override int UpdateDataViewQuantity(
            int rowIdx,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
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

            // TODO: Resource strings
            Label labelHeader1 = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader2 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader3 = new Label { Text = "Quantity", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader4 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader5 = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader6 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);
            double detailWidth0 = 0;
            double detailWidth1 = 0;
            double detailWidth2 = 0;
            double detailWidth3 = 0;
            double detailWidth4 = 0;
            double detailWidth5 = 0;

            // Create header data template
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
                    new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelHeader1);
            gridHeader.Children.Add(labelHeader2);
            gridHeader.Children.Add(labelHeader3);
            gridHeader.Children.Add(labelHeader4);
            gridHeader.Children.Add(labelHeader5);
            gridHeader.Children.Add(labelHeader6);

            Grid.SetColumn(labelHeader1, 0);
            Grid.SetColumn(labelHeader2, 1);
            Grid.SetColumn(labelHeader3, 2);
            Grid.SetColumn(labelHeader4, 3);
            Grid.SetColumn(labelHeader5, 4);
            Grid.SetColumn(labelHeader6, 5);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                InventoryInquiryItem inventoryInquiryItem = new InventoryInquiryItem
                {
                    Productcode = Convert.ToString(row["Productcode"] != DBNull.Value ? row["Productcode"] : null),
                    Productunitbc = Convert.ToString(row["Productunitbc"] != DBNull.Value ? row["Productunitbc"] : null),
                    Unitcode = Convert.ToString(row["Unitcode"] != DBNull.Value ? row["Unitcode"] : null),
                    Weightunitcode = Convert.ToString(row["Weightunitcode"] != DBNull.Value ? row["Weightunitcode"] : null),
                    Activityitemquantity = Convert.ToDecimal(row["Activityitemquantity"] != DBNull.Value ? row["Activityitemquantity"] : null),
                    Activityitemweightquantity = Convert.ToDecimal(row["Activityitemweightquantity"] != DBNull.Value ? row["Activityitemweightquantity"] : null)
                };

                observableCollection.Add(inventoryInquiryItem);

                detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.Productcode);
                detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.Productunitbc);
                detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", inventoryInquiryItem.Activityitemquantity));
                detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.Unitcode);
                detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", inventoryInquiryItem.Activityitemweightquantity));
                detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.Weightunitcode);

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

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                }

                if (detailWidth5 > columnWidth5)
                {
                    columnWidth5 = detailWidth5;
                    gridHeader.ColumnDefinitions[5].Width = columnWidth5;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <param name="dataGridMain"></param>
        //        /// <remarks></remarks>
        //        public override void DesignDataGrid(DataGrid dataGridMain)
        //        {
        //            dataGridCol = new DataGridTextBoxColumn();
        //            dataGridCol.MappingName = "productcode";
        //            dataGridCol.HeaderText = "Product ID";
        //            if (0 < ProdInfo.Refno)
        //            {
        //                dataGridCol.Width = -1;
        //            }
        //            else
        //            {
        //                dataGridCol.Width = (int)(65 * columnWidthFactor);
        //            }
        //            dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //            dataGridCol.MappingName = "productunitbc";
        //            dataGridCol.HeaderText = string.Empty;

        //            dataGridCol.MappingName = "activityitemquantity";
        //            dataGridCol.HeaderText = "Quantity";
        //            dataGridCol.Format = "0.000";

        //            dataGridCol.MappingName = "unitcode";
        //            dataGridCol.HeaderText = string.Empty;

        //            dataGridCol.MappingName = "activityitemweightquantity";
        //            dataGridCol.HeaderText = "Weight";
        //            dataGridCol.Format = "0.000";

        //            dataGridCol.MappingName = "weightunitcode";
        //            dataGridCol.HeaderText = string.Empty;
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
                        using (DataSet ds = new DataSet())
                        {
                            ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);
                            if (ds.Tables.Count != 0)
                            {
                                this.DataViewItems = ds.Tables["tActivityItem"].DefaultView;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                throw new Exception("Refresh inventory inquiry items failed.", ex);
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

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST INVENTORY INQUIRY SUMMARY</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + this.ProdInfo.Code + "</prod_id>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
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
