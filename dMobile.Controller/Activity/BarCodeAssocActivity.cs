// <copyright file="BarCodeAssocActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Headers;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="BarCodeAssocActivity" />.
    /// </summary>
    public class BarCodeAssocActivity : Activity
    {
        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double columnWidth0 = 0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double columnWidth1 = 0;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double columnWidth2 = 0;

        /// <summary>
        /// Defines the columnWidth3.
        /// </summary>
        private double columnWidth3 = 0;

        /// <summary>
        /// Defines the columnWidth4.
        /// </summary>
        private double columnWidth4 = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the barcodeAssociations.
        /// </summary>
        private List<BarcodeAssociation> barcodeAssociations = new List<BarcodeAssociation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeAssocActivity"/> class.
        /// </summary>
        public BarCodeAssocActivity()
          : base()
        {
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                            new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                    },
                };

                var label1 = new Label { VerticalOptions = LayoutOptions.Center };
                var label2 = new Label { VerticalOptions = LayoutOptions.Center };
                var label3 = new Label { VerticalOptions = LayoutOptions.Center };
                var label4 = new Label { VerticalOptions = LayoutOptions.Center };
                var label5 = new Label { VerticalOptions = LayoutOptions.Center };

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

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplateBarCodeAssoc")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateBarCodeAssoc", dataTemplate);
            }
        }

        /// <summary>
        /// Display the extra information for the selected activity.
        /// </summary>
        /// <param name="extraInfo">Extra information collection.</param>
        public override void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            base.DisplayExtraInfo(extraInfo);
            if (this.SelectedActivity is BarcodeAssociation activity)
            {
                extraInfo.Add(activity.ProductUnitBarCodeCode.TrimEnd());
                extraInfo.Add(activity.ProductUnitBarCodeModifier1.TrimEnd());
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            var actBarCodeAssocXRef = new TableActivityItemBarCodeAssociationXRef();

            await actBarCodeAssocXRef.PurgeData();

            try
            {
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation));
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.BarCodeAssociation));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Purging bar code association failed.", ex);
            }
        }

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Bar Code Association";
        }

        /// <summary>
        /// The SelectActivity.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is BarcodeAssociation barcodeAssociation)
            {
                this.SelectedActivity = barcodeAssociation;
                this.HdrFkRefno = barcodeAssociation.ActivityRefno;
                this.HdrRefno = barcodeAssociation.ActivityRefno;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// The AssignRefnoFromActivity.
        /// </summary>
        /// <param name="selectedObject">The selectedObject<see cref="object"/>.</param>
        public override void AssignRefnoFromActivity(object selectedObject)
        {
            if (selectedObject is BarcodeAssociation hdr)
            {
                this.HdrFkRefno = Convert.ToInt32(hdr.ActivityRefno);
                this.HdrRefno = Convert.ToInt32(hdr.ActivityRefno);
            }
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivity.ActivityRefno,");
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
                sql.Append("SELECT tActivity.ActivityRefno,");
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

            this.barcodeAssociations = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<BarcodeAssociation>(sql.ToString());
            this.ListHeaders = this.barcodeAssociations.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            this.ListHeaders = this.barcodeAssociations.Where(barcode => barcode.ProductUnitBarCodeCode.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (var barcodeAssociation in this.ListHeaders.Cast<BarcodeAssociation>())
            {
                // BarcodeAssociation barcodeAssociation = new BarcodeAssociation
                // {
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ProductUnitBarCodeCode = Convert.ToString(row["ProductUnitBarCodeCode"] != DBNull.Value ? row["ProductUnitBarCodeCode"] : null),
                //    ProductUnitBarCodeModifier1 = Convert.ToString(row["ProductUnitBarCodeModifier1"] != DBNull.Value ? row["ProductUnitBarCodeModifier1"] : null),
                //    ProductUnitBarCodeRefno = Convert.ToInt32(row["ProductUnitBarCodeRefno"] != DBNull.Value ? row["ProductUnitBarCodeRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                // };
                observableCollection.Add(barcodeAssociation);
            }

            return observableCollection;
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The gridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The listViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            var labelHeader1 = new Label { Text = "Product", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader2 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader3 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader4 = new Label { Text = "Bar Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader5 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);

            // Create header data template
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                },
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

            gridMainActivity.Children.Add(gridHeader);

            foreach (var barcodeAssociation in this.ListHeaders.Cast<BarcodeAssociation>())
            {
                // BarcodeAssociation barcodeAssociation = new BarcodeAssociation
                // {
                //    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ActivityStatus = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                //    ProductUnitBarCodeCode = Convert.ToString(row["ProductUnitBarCodeCode"] != DBNull.Value ? row["ProductUnitBarCodeCode"] : null),
                //    ProductUnitBarCodeModifier1 = Convert.ToString(row["ProductUnitBarCodeModifier1"] != DBNull.Value ? row["ProductUnitBarCodeModifier1"] : null),
                //    ProductUnitBarCodeRefno = Convert.ToInt32(row["ProductUnitBarCodeRefno"] != DBNull.Value ? row["ProductUnitBarCodeRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                // };
                observableCollection.Add(barcodeAssociation);

                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductCode);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductUnitBC);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.UnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductUnitBarCodeCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(barcodeAssociation.ProductUnitBarCodeModifier1);

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.columnWidth4;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateBarCodeAssoc"];

            return observableCollection;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        public override void GetFilter(XDocument data)
        {
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
        }
    }
}
