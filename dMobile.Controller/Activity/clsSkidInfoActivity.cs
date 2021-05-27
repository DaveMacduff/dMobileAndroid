namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class SkidInfoActivity : Activity
    {
        #region Fields

        private LocationMap locMap;
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private double columnWidth6 = 0;
        private double columnWidth7 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<Skid> listSkids = new List<Skid>();

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

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
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
        public override void GetFilter()
        {
            //return string.Empty;
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
        public override async Task Delete()
        {
            if (this.ListHdrs != null && this.ListHdrs.Count > 1)
            {
                this.ListHdrs.RemoveRange(1, this.ListHdrs.Count - 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Skid skid)
            {
                this.SelectedActivity = skid;
                this.FKRefno = Convert.ToInt32(skid.SkidFKRefno);
                this.Refno = Convert.ToInt32(skid.SkidFKRefno);

                SSkidInfo info = new SSkidInfo();
                info.SkidCode = Convert.ToString(skid.SkidCode);
                info.SkidFKRefno = Convert.ToInt32(skid.SkidFKRefno);
                info.LocMapRefno = Convert.ToInt32(skid.LocationMapRefno);
                info.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(info.LocMapRefno);
                info.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(Convert.ToInt32(skid.ProductRefno));
                info.Quantity = Convert.ToDecimal(skid.Quantity);
                info.BaseQuantity = Convert.ToDecimal(skid.BaseQuantity);
                info.WeightQuantity = Convert.ToDecimal(skid.WeightQuantity);
                info.Cube = Convert.ToDecimal(skid.SkidCube);
                info.CubeUnit = Convert.ToString(skid.SkidCubeUnit);
                info.Height = Convert.ToDecimal(skid.SkidHeight);
                info.HeightUnit = Convert.ToString(skid.SkidHeightUnit);
                info.Hi = Convert.ToInt32(skid.SkidHi);
                info.Ti = Convert.ToInt32(skid.SkidTi);
                info.IsCHEP = Convert.ToBoolean(skid.SkidIsCHEP);
                info.IsHold = Convert.ToBoolean(skid.SkidIsOnHold);
                info.IsPutAway = false;
                info.IsCompleted = !SysCommon.IsEmptyDateTime(Convert.ToDateTime(skid.SkidCompleted));
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
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
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

            //this.DataViewHdrs = table.DefaultView;
            //this.DataViewHdrs.Table.TableName = this.MainGridMappingName;

            this.listSkids = table.Rows.Cast<Skid>().ToList();
            this.ListHdrs = this.listSkids.Cast<object>().ToList();
            return this.ListHdrs;
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

            foreach (Skid skid in this.ListHdrs)
            {
                //Skid skid = new Skid
                //{
                //    SkidFKRefno = Convert.ToInt32(row["SkidFKRefno"] != DBNull.Value ? row["SkidFKRefno"] : null),
                //    SkidCode = Convert.ToString(row["SkidCode"] != DBNull.Value ? row["SkidCode"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    WeightUnitCode = Convert.ToString(row["WeightUnitCode"] != DBNull.Value ? row["WeightUnitCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    Quantity = Convert.ToDecimal(row["Quantity"] != DBNull.Value ? row["Quantity"] : null),
                //    BaseQuantity = Convert.ToDecimal(row["BaseQuantity"] != DBNull.Value ? row["BaseQuantity"] : null),
                //    WeightQuantity = Convert.ToDecimal(row["WeightQuantity"] != DBNull.Value ? row["WeightQuantity"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    SkidCreated = Convert.ToDateTime(row["SkidCreated"] != DBNull.Value ? row["SkidCreated"] : null),
                //    SkidCompleted = Convert.ToDateTime(row["SkidCompleted"] != DBNull.Value ? row["SkidCompleted"] : null),
                //    SkidTi = Convert.ToInt32(row["SkidTi"] != DBNull.Value ? row["SkidTi"] : null),
                //    SkidHi = Convert.ToInt32(row["SkidHi"] != DBNull.Value ? row["SkidHi"] : null),
                //    SkidCube = Convert.ToDecimal(row["SkidCube"] != DBNull.Value ? row["SkidCube"] : null),
                //    SkidCubeUnit = Convert.ToString(row["SkidCubeUnit"] != DBNull.Value ? row["SkidCubeUnit"] : null),
                //    SkidHeight = Convert.ToDecimal(row["SkidHeight"] != DBNull.Value ? row["SkidHeight"] : null),
                //    SkidHeightUnit = Convert.ToString(row["SkidHeightUnit"] != DBNull.Value ? row["SkidHeightUnit"] : null),
                //    SkidIsCHEP = Convert.ToBoolean(row["SkidIsCHEP"] != DBNull.Value ? row["SkidIsCHEP"] : null),
                //    SkidIsOnHold = Convert.ToBoolean(row["SkidIsOnHold"] != DBNull.Value ? row["SkidIsOnHold"] : null),
                //};

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

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);
            columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader7.Text);
            columnWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader8.Text);

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

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override Task Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override void GetFilter(XDocument data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
