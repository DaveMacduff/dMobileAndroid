namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
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
                this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["SkidFKRefno"]);
                this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["SkidFKRefno"]);

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
                info.IsCompleted = !SysCommon.IsEmptyDateTime(Convert.ToDateTime(this.DataViewHdrs[idx]["SkidCompleted"]));
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
            throw new NotImplementedException();
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Skid skid = new Skid
                {
                    SkidFKRefno = Convert.ToInt32(row["SkidFKRefno"] != DBNull.Value ? row["SkidFKRefno"] : null),
                    SkidCode = Convert.ToString(row["SkidCode"] != DBNull.Value ? row["SkidCode"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                    WeightUnitCode = Convert.ToString(row["WeightUnitCode"] != DBNull.Value ? row["WeightUnitCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    Quantity = Convert.ToDecimal(row["Quantity"] != DBNull.Value ? row["Quantity"] : null),
                    BaseQuantity = Convert.ToDecimal(row["BaseQuantity"] != DBNull.Value ? row["BaseQuantity"] : null),
                    WeightQuantity = Convert.ToDecimal(row["WeightQuantity"] != DBNull.Value ? row["WeightQuantity"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    SkidCreated = Convert.ToDateTime(row["SkidCreated"] != DBNull.Value ? row["SkidCreated"] : null),
                    SkidCompleted = Convert.ToDateTime(row["SkidCompleted"] != DBNull.Value ? row["SkidCompleted"] : null),
                    SkidTi = Convert.ToInt32(row["SkidTi"] != DBNull.Value ? row["SkidTi"] : null),
                    SkidHi = Convert.ToInt32(row["SkidHi"] != DBNull.Value ? row["SkidHi"] : null),
                    SkidCube = Convert.ToDecimal(row["SkidCube"] != DBNull.Value ? row["SkidCube"] : null),
                    SkidCubeUnit = Convert.ToString(row["SkidCubeUnit"] != DBNull.Value ? row["SkidCubeUnit"] : null),
                    SkidHeight = Convert.ToDecimal(row["SkidHeight"] != DBNull.Value ? row["SkidHeight"] : null),
                    SkidHeightUnit = Convert.ToString(row["SkidHeightUnit"] != DBNull.Value ? row["SkidHeightUnit"] : null),
                    SkidIsCHEP = Convert.ToBoolean(row["SkidIsCHEP"] != DBNull.Value ? row["SkidIsCHEP"] : null),
                    SkidIsOnHold = Convert.ToBoolean(row["SkidIsOnHold"] != DBNull.Value ? row["SkidIsOnHold"] : null),
                };

                observableCollection.Add(skid);
            }

            // TODO: Resource strings
            Label labelHeader1 = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader2 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader3 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader4 = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader5 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader6 = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader7 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader8 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            double columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            double columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);
            double columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader7.Text);
            double columnWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader8.Text);

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
                    new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth6, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth7, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelHeader1);
            gridHeader.Children.Add(labelHeader2);
            gridHeader.Children.Add(labelHeader3);
            gridHeader.Children.Add(labelHeader4);
            gridHeader.Children.Add(labelHeader5);
            gridHeader.Children.Add(labelHeader6);
            gridHeader.Children.Add(labelHeader7);
            gridHeader.Children.Add(labelHeader8);

            Grid.SetColumn(labelHeader1, 0);
            Grid.SetColumn(labelHeader2, 1);
            Grid.SetColumn(labelHeader3, 2);
            Grid.SetColumn(labelHeader4, 3);
            Grid.SetColumn(labelHeader5, 4);
            Grid.SetColumn(labelHeader6, 5);
            Grid.SetColumn(labelHeader7, 6);
            Grid.SetColumn(labelHeader8, 7);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (Skid skid in observableCollection)
            {
                //string.Format("{0:F3}", adjustmentDetail.ActivityDetailQuantity)
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.SkidCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.ProductCode);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.ProductUnitBC);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", skid.Quantity));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.UnitCode);
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", skid.WeightQuantity));
                double detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.WeightUnitCode);
                double detailWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.LocationName);

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

                if (detailWidth6 > columnWidth6)
                {
                    columnWidth6 = detailWidth6;
                    gridHeader.ColumnDefinitions[6].Width = columnWidth6;
                }

                if (detailWidth7 > columnWidth7)
                {
                    columnWidth7 = detailWidth7;
                    gridHeader.ColumnDefinitions[7].Width = columnWidth7;
                }
            }

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
                        new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth6, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth7, GridUnitType.Absolute) }
                    }
                };

                Label label1 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label2 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label3 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label4 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label5 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label6 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label7 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label8 = new Label { VerticalOptions = LayoutOptions.Center };
                
                label1.SetBinding(Label.TextProperty, new Binding("SkidCode"));
                label2.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                label3.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                label4.SetBinding(Label.TextProperty, new Binding("Quantity", BindingMode.Default, null, null, "{0:F3}"));
                label5.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                label6.SetBinding(Label.TextProperty, new Binding("WeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                label7.SetBinding(Label.TextProperty, new Binding("WeightUnitCode"));
                label8.SetBinding(Label.TextProperty, new Binding("LocationName"));

                gridData.Children.Add(label1);
                gridData.Children.Add(label2);
                gridData.Children.Add(label3);
                gridData.Children.Add(label4);
                gridData.Children.Add(label5);
                gridData.Children.Add(label6);
                gridData.Children.Add(label7);
                gridData.Children.Add(label8);

                Grid.SetColumn(label1, 0);
                Grid.SetColumn(label2, 1);
                Grid.SetColumn(label3, 2);
                Grid.SetColumn(label4, 3);
                Grid.SetColumn(label5, 4);
                Grid.SetColumn(label6, 5);
                Grid.SetColumn(label7, 6);
                Grid.SetColumn(label8, 7);

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplate;

            return observableCollection;
        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="dataGridMain"></param>
//        public override void DesignDataGrid(DataGrid dataGridMain)
//        {
//            dataGridCol.MappingName = "SkidCode";
//            dataGridCol.HeaderText = "Code";

//            dataGridCol.MappingName = "ProductCode";
//            dataGridCol.HeaderText = string.Empty;

//            dataGridCol.MappingName = "ProductUnitBC";
//            dataGridCol.HeaderText = string.Empty;

//            dataGridCol.MappingName = "Quantity";
//            dataGridCol.HeaderText = "Qty";
//            dataGridCol.Format = "0.000";

//            dataGridCol.MappingName = "UnitCode";
//            dataGridCol.HeaderText = string.Empty;

//            dataGridCol.MappingName = "WeightQuantity";
//            dataGridCol.HeaderText = "Weight";
//            dataGridCol.Format = "0.000";

//            dataGridCol.MappingName = "WeightUnitCode";
//            dataGridCol.HeaderText = string.Empty;

//            dataGridCol.MappingName = "LocationName";
//            dataGridCol.HeaderText = string.Empty;
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
