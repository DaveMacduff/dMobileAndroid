// <copyright file="SkidInfoActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="SkidInfoActivity" /> skid information activity.
    /// </summary>
    public class SkidInfoActivity : Activity
    {
        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listSkids.
        /// </summary>
        private List<Skid> listSkids = new List<Skid>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SkidInfoActivity"/> class.
        /// </summary>
        public SkidInfoActivity()
            : base()
        {
            this.FullClassName = typeof(SkidInfoActivity).FullName;
            this.LocMap = LocationMap.GetInstance();

            var labelHeader1 = new Label
            {
                Text = "Code",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader2 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader3 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader4 = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader5 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader6 = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader7 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader8 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                var columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
                var columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
                var columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
                var columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
                var columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
                var columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);
                var columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader7.Text);
                var columnWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader8.Text);

                var gridData = new Grid
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
                    },
                };

                var label1 = new Label { VerticalOptions = LayoutOptions.Center };
                var label2 = new Label { VerticalOptions = LayoutOptions.Center };
                var label3 = new Label { VerticalOptions = LayoutOptions.Center };
                var label4 = new Label { VerticalOptions = LayoutOptions.Center };
                var label5 = new Label { VerticalOptions = LayoutOptions.Center };
                var label6 = new Label { VerticalOptions = LayoutOptions.Center };
                var label7 = new Label { VerticalOptions = LayoutOptions.Center };
                var label8 = new Label { VerticalOptions = LayoutOptions.Center };

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


            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplateSkidInfo")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateSkidInfo", dataTemplate);
            }
        }

        /// <summary>
        /// Gets or sets the LocMap.
        /// </summary>
        public LocationMap LocMap { get; set; }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
        }

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Skid Info";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Delete()
        {
            if (this.ListHdrs != null && this.ListHdrs.Count > 1)
            {
                this.ListHdrs.RemoveRange(1, this.ListHdrs.Count - 1);
            }
        }

        /// <summary>
        /// Select the activity.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Selected activity.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Skid skid)
            {
                this.SelectedActivity = skid;
                this.HdrFKRefno = Convert.ToInt32(skid.SkidFKRefno);
                this.HdrRefno = Convert.ToInt32(skid.SkidFKRefno);

                var info = new SSkidInfo();
                info.SkidCode = Convert.ToString(skid.SkidCode);
                info.SkidFKRefno = Convert.ToInt32(skid.SkidFKRefno);
                info.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(skid.LocationMapRefno);
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
                this.HdrFKRefno = 0;
                this.HdrRefno = 0;
                this.SkidInfo = new SysSkidInfo();
            }
        }

        /// <summary>
        /// The GetRefnoFromRowIdx.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            return this.HdrRefno;
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var table = new DataTable("ActivitySkid");

            _ = table.Columns.Add("SkidFKRefno", Type.GetType("System.Int32"));
            _ = table.Columns.Add("SkidCode", Type.GetType("System.String"));
            _ = table.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32"));
            _ = table.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
            _ = table.Columns.Add("ProductCode", Type.GetType("System.String"));
            _ = table.Columns.Add("ProductUnitBC", Type.GetType("System.String"));
            _ = table.Columns.Add("ProductUnitIsBC", Type.GetType("System.Boolean"));
            _ = table.Columns.Add("UnitCode", Type.GetType("System.String"));
            _ = table.Columns.Add("BaseUnitCode", Type.GetType("System.String"));
            _ = table.Columns.Add("WeightUnitCode", Type.GetType("System.String"));
            _ = table.Columns.Add("ProductDescription", Type.GetType("System.String"));
            _ = table.Columns.Add("ProductUnitSizeDescription", Type.GetType("System.String"));
            _ = table.Columns.Add("Quantity", Type.GetType("System.Decimal"));
            _ = table.Columns.Add("BaseQuantity", Type.GetType("System.Decimal"));
            _ = table.Columns.Add("WeightQuantity", Type.GetType("System.Decimal"));
            _ = table.Columns.Add("LocationMapRefno", Type.GetType("System.Int32"));
            _ = table.Columns.Add("LocationName", Type.GetType("System.String"));
            _ = table.Columns.Add("SkidCreated", Type.GetType("System.DateTime"));
            _ = table.Columns.Add("SkidCompleted", Type.GetType("System.DateTime"));
            _ = table.Columns.Add("SkidTi", Type.GetType("System.Int32"));
            _ = table.Columns.Add("SkidHi", Type.GetType("System.Int32"));
            _ = table.Columns.Add("SkidCube", Type.GetType("System.Decimal"));
            _ = table.Columns.Add("SkidCubeUnit", Type.GetType("System.String"));
            _ = table.Columns.Add("SkidHeight", Type.GetType("System.Decimal"));
            _ = table.Columns.Add("SkidHeightUnit", Type.GetType("System.String"));
            _ = table.Columns.Add("SkidIsCHEP", Type.GetType("System.Boolean"));
            _ = table.Columns.Add("SkidIsOnHold", Type.GetType("System.Boolean"));

            // this.DataViewHdrs = table.DefaultView;
            // this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
            this.listSkids = table.Rows.Cast<Skid>().ToList();
            this.ListHdrs = this.listSkids.Cast<object>().ToList();
            return this.ListHdrs;
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
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (Skid skid in this.ListHdrs)
            {
                observableCollection.Add(skid);
            }

            var labelHeader1 = new Label
            {
                Text = "Code",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader2 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader3 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader4 = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader5 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader6 = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader7 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelHeader8 = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            var columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            var columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            var columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            var columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            var columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);
            var columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader7.Text);
            var columnWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader8.Text);

            // Create header data template
            var gridHeader = new Grid
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
                },
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

            GridMainActivity.Children.Add(gridHeader);

            foreach (Skid skid in observableCollection)
            {
                // string.Format("{0:F3}", adjustmentDetail.ActivityDetailQuantity)
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.SkidCode);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.ProductCode);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.ProductUnitBC);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", skid.Quantity));
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.UnitCode);
                var detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", skid.WeightQuantity));
                var detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.WeightUnitCode);
                var detailWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(skid.LocationName);

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

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateSkidInfo"];

            return observableCollection;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">.</param>
        public override void GetFilter(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model (tActivity??Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="xmlParams">The xmlParams<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override Task<int> Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PrintLabel.
        /// </summary>
        /// <param name="xmlParam">The xmlParam<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task PrintLabel(XDocument xmlParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ChangeStatus.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public override Task<bool> ChangeStatus(object selectedActivity)
        {
            throw new NotImplementedException();
        }
    }
}
