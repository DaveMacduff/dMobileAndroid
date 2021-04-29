namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;

    public class BarCodeAssocActivity : Activity
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        public BarCodeAssocActivity()
          : base()
        {
            this.FullClassName = typeof(BarCodeAssocActivity).FullName;

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
                            new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                    }
                };

                Label label1 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label2 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label3 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label4 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label5 = new Label { VerticalOptions = LayoutOptions.Center };

                label1.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                label2.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                label3.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                label4.SetBinding(Label.TextProperty, new Binding("ProductUnitBarCodeCode"));
                label5.SetBinding(Label.TextProperty, new Binding("ProductUnitBarCodeModifier1"));

                gridData.Children.Add(label1);
                gridData.Children.Add(label2);
                gridData.Children.Add(label3);
                gridData.Children.Add(label4);
                gridData.Children.Add(label5);

                Grid.SetColumn(label1, 0);
                Grid.SetColumn(label2, 1);
                Grid.SetColumn(label3, 2);
                Grid.SetColumn(label4, 3);
                Grid.SetColumn(label5, 4);

                return new ViewCell { View = gridData };
            });

            resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            TableActivityItemBarCodeAssociationXRef actBarCodeAssocXRef = new TableActivityItemBarCodeAssociationXRef();

            actBarCodeAssocXRef.Purge();

            try
            {
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation));
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation));
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging bar code association failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityCountStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Bar Code Association";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is BarcodeAssociation barcodeAssociation)
            {
                this.SelectedActivity = barcodeAssociation;
                this.FKRefno = Convert.ToInt32((selectedActivity as dynamic).ActivityRefno);
                this.Refno = Convert.ToInt32((selectedActivity as dynamic).ActivityRefno);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefnoFromRowIdx(object selectedObject)
        {
            this.FKRefno = Convert.ToInt32((selectedObject as dynamic).ActivityRefno);
            this.Refno = Convert.ToInt32((selectedObject as dynamic).ActivityRefno);

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
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivity.ActivityRefno AS ActivityRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno AS ProductUnitBarCodeRefno,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode AS ProductUnitBarCodeCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1 AS ProductUnitBarCodeModifier1,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivity");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.Append(" INNER JOIN tActivityItemBarCodeAssociationXRef ON ");
                sql.Append("tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno");
                sql.Append(" INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno = ");
                sql.Append("tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno");
                sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno");
                sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
                sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno");
                sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(" AND tActivityItem.ActivityItemSubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation));
            }
            else
            {
                sql.Append("SELECT tActivity.ActivityRefno AS ActivityRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno AS ProductUnitBarCodeRefno,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode AS ProductUnitBarCodeCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1 AS ProductUnitBarCodeModifier1,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                sql.Append(" FROM tActivity");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                sql.Append(" INNER JOIN tActivityItemBarCodeAssociationXRef ON ");
                sql.Append("tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno");
                sql.Append(" INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno = ");    
                sql.Append("tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno");
                sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno");
                sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
                sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno");
                sql.AppendFormat(" AND tActivityItem.ActivityItemSubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation));
            }

            this.ListHdrs = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<object>(sql.ToString());
            return this.ListHdrs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            //this.DataViewHdrs.RowFilter = "ProductUnitBarCodeCode LIKE \'%" + searchText.Trim() + "%\'";
            this.ListHdrs = (List<object>)this.ListHdrs.OfType<BarcodeAssociation>().Where(bca => bca.ProductUnitBarCodeCode.Contains(searchText.Trim()));

            foreach (BarcodeAssociation barcodeAssociation in this.ListHdrs)
            {
                //BarcodeAssociation barcodeAssociation = new BarcodeAssociation
                //{
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ProductUnitBarCodeCode = Convert.ToString(row["ProductUnitBarCodeCode"] != DBNull.Value ? row["ProductUnitBarCodeCode"] : null),
                //    ProductUnitBarCodeModifier1 = Convert.ToString(row["ProductUnitBarCodeModifier1"] != DBNull.Value ? row["ProductUnitBarCodeModifier1"] : null),
                //    ProductUnitBarCodeRefno = Convert.ToInt32(row["ProductUnitBarCodeRefno"] != DBNull.Value ? row["ProductUnitBarCodeRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                //};

                observableCollection.Add(barcodeAssociation);
            }

            return observableCollection;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource Strings
            Label labelHeader1 = new Label { Text = "Product", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader2 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader3 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader4 = new Label { Text = "Bar Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader5 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            double detailWidth0 = 0;
            double detailWidth1 = 0;
            double detailWidth2 = 0;
            double detailWidth3 = 0;
            double detailWidth4 = 0;

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
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelHeader1);
            gridHeader.Children.Add(labelHeader2);
            gridHeader.Children.Add(labelHeader3);
            gridHeader.Children.Add(labelHeader4);
            gridHeader.Children.Add(labelHeader5);

            Grid.SetColumn(labelHeader1, 0);
            Grid.SetColumn(labelHeader2, 1);
            Grid.SetColumn(labelHeader3, 2);
            Grid.SetColumn(labelHeader4, 3);
            Grid.SetColumn(labelHeader5, 4);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (BarcodeAssociation barcodeAssociation in this.ListHdrs)
            {
                //BarcodeAssociation barcodeAssociation = new BarcodeAssociation
                //{
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ProductUnitBarCodeCode = Convert.ToString(row["ProductUnitBarCodeCode"] != DBNull.Value ? row["ProductUnitBarCodeCode"] : null),
                //    ProductUnitBarCodeModifier1 = Convert.ToString(row["ProductUnitBarCodeModifier1"] != DBNull.Value ? row["ProductUnitBarCodeModifier1"] : null),
                //    ProductUnitBarCodeRefno = Convert.ToInt32(row["ProductUnitBarCodeRefno"] != DBNull.Value ? row["ProductUnitBarCodeRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                //};

                observableCollection.Add(barcodeAssociation);

                detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductCode);
                detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductUnitBC);
                detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.UnitCode);
                detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductUnitBarCodeCode);
                detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductUnitBarCodeModifier1);

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
            }

            dataGridMainActivity.ItemTemplate = dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlParams"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            return string.Empty;
        }

        #endregion
    }
}
