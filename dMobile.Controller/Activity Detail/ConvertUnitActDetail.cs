// <copyright file="ConvertUnitActDetail.cs" company="InterSect Business Systems Inc">
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
using dMobile.Controller.Activity_Item;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Details;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Defines the <see cref="ConvertUnitActDetail" />.
    /// </summary>
    public class ConvertUnitActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertUnitActDetail"/> class.
        /// </summary>
        /// <param name="actItem">.</param>
        public ConvertUnitActDetail(ActItem actItem)
          : base(actItem)
        {
            this.FullClassName = typeof(ConvertUnitActDetail).FullName;

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    },
                };

                var labelUnitNumber = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };
                var labelQty = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };
                var labelWeight = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                labelUnitNumber.SetBinding(Label.TextProperty, new Binding("CountNumber"));
                labelQty.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelUnitNumber);
                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWeight);

                Grid.SetColumn(labelUnitNumber, 0);
                Grid.SetColumn(labelQty, 1);
                Grid.SetColumn(labelWeight, 2);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplateConvertUnitDetail")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplateConvertUnitDetail", dataTemplate);
            }
        }

        /// <summary>
        /// Gets or sets the grid column 0 width...
        /// </summary>
        private double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width...
        /// </summary>
        private double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width...
        /// </summary>
        private double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary...
        /// </summary>
        private ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets the ConvertUnitDetails
        /// Gets or sets a list of picking details..
        /// </summary>
        private List<ConvertUnitDetail> ConvertUnitDetails { get; set; } = new List<ConvertUnitDetail>();

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <returns><see cref="ActivityDetail"/> Activity detail.</returns>
        public override ActivityDetail GatherDetail()
        {
            var count = this.ListDetails.Count + 1;
            var detail = new ConvertUnitDetail
            {
                // KLUDGE: Restore the new detail reference number's that got over written somehow.
                ActivityDetailRefno = this.SysEnv.InsertedDetailRefno,
                ActivityDetailFKRefno = this.SysEnv.InsertedDetailFkRefno,
                CountNumber = count,
            };

            this.GatherProductDetail(detail);
            this.GatherQuantityDetail(detail);
            this.GatherSchemeDetail(detail);
            this.GatherExtraDetail(detail);
            this.GatherLocationDetail(detail);

            detail.ActivityDetailSchemeID = this.SchemeId;
            detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeId;

            return detail;
        }

        /// <summary>
        /// Update detail in the detail list.
        /// </summary>
        /// <param name="listDetailIdx"><see cref="int"/> Zero based list detail index.</param>
        public override void UpdateListDetail(int listDetailIdx)
        {
            var detail = (ConvertUnitDetail)this.ListDetails[listDetailIdx];
            detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
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
        public override Task Insert(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> Delete()
        {
            await Task.Run(() =>
            {
                this.ListDetails?.Clear();
            });

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
            ////if (this.SysEnv.SysSettings.IsDisconnectedMode)
            ////{
            ////    this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            ////}
            ////else
            ////{
            ////    this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

            ////    var tableDet = new TableActivityConvertUnitDetail();
            ////    tableDet.PurgeData();
            ////    this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

            ////    tableDet.RegisterDataAccessListenerProgressOut((ConvertUnitDetail)this);
            ////    ////await tableCountDet.RequestData(this.ActItem.ItemRefno);
            ////    tableDet.UnregisterDataAccessListenerProgressOut((CycleCountActDetail)this);
            ////}

            return Task.CompletedTask;
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return ((ConvertUnitDetail)selectedActivityDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFkRefno(object selectedActivityDetail)
        {
            return ((ConvertUnitDetail)selectedActivityDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// Get a list of activity details.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <exception cref="InvalidOperationException"> Invalid operation exception.</exception>
        /// <returns><see cref="List{T}"/> Activity details.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            await Task.Run(function: () =>
            {
                var table = new DataTable("ActivityConvertUnitItem");
                table.Columns.Add("CountNumber", Type.GetType("System.Int32") ?? throw new InvalidOperationException());
                table.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32") ?? throw new InvalidOperationException());
                table.Columns.Add("ProductRefno", Type.GetType("System.Int32") ?? throw new InvalidOperationException());
                table.Columns.Add("ProductCode", Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add("ProductUnitBC", Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add("UnitCode", Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ProductDescription",
                    Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ProductUnitSizeDescription",
                    Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailQuantity",
                    Type.GetType("System.Decimal") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailBaseQuantity",
                    Type.GetType("System.Decimal") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailWeightQuantity",
                    Type.GetType("System.Decimal") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailSchemeID",
                    Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailUniqueSchemeID",
                    Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailLogisticUnitBestBeforeSellByDate",
                    Type.GetType("System.DateTime") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailLogisticUnitProductionDate",
                    Type.GetType("System.DateTime") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailSingleTradeItemSerialNumber",
                    Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailLogisticUnitBatchLotNumber",
                    Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ProductSchemeVariantCode",
                    Type.GetType("System.String") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ProductSchemeVariantNumber",
                    Type.GetType("System.Int32") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailFKRefno",
                    Type.GetType("System.Int32") ?? throw new InvalidOperationException());
                table.Columns.Add(
                    "ActivityDetailRefno",
                    Type.GetType("System.Int32") ?? throw new InvalidOperationException());

                this.ConvertUnitDetails = table.Rows.Cast<ConvertUnitDetail>().ToList();
                this.ListDetails = table.Rows.Cast<object>().ToList();
                return Task.CompletedTask;
            });

            return this.ListDetails.Cast<object>().ToList();
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMain">The gridMain<see cref="Grid"/>.</param>
        /// <param name="listViewMain">The listViewMain<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMain, ref ListView listViewMain)
        {
            var observableCollection = new ObservableCollection<object>();
            foreach (var convertUnitDetail in this.ListDetails.Cast<ConvertUnitDetail>())
            {
                observableCollection.Add(convertUnitDetail);
            }

            var labelUnitNumber = new Label
            {
                Text = "Unit #",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            var labelQtyHeader = new Label
            {
                Text = "Quantity",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            var labelWeightHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitNumber.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelUnitNumber);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);

            Grid.SetColumn(labelUnitNumber, 0);
            Grid.SetColumn(labelQtyHeader, 1);
            Grid.SetColumn(labelWeightHeader, 2);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            foreach (var convertUnitDetail in observableCollection.Cast<ConvertUnitDetail>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(convertUnitDetail.CountNumber));
                var detailWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth($"{convertUnitDetail.ActivityDetailQuantity:F3}");
                var detailWidth2 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth($"{convertUnitDetail.ActivityDetailWeightQuantity:F3}");

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

                if (detailWidth2 > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
                }
            }

            listViewMain.VerticalScrollBarVisibility = ScrollBarVisibility.Always;
            listViewMain.HorizontalScrollBarVisibility = ScrollBarVisibility.Always;
            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateConvertUnitDetail"];

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
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override Task<string> CreateDeleteXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override Task<string> CreateSubmitXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            var msg = (this.ActItem.ProdInfo.Code + " " + this.ActItem.ProdInfo.UnitBc).TrimEnd() + " "
                + this.ActItem.ProdInfo.UnitCode;
            return $"Convert {msg}";
        }

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail"/> Activity detail.</param>
        /// <param name="mainActivityItems"><see cref="ObservableCollection{T}"/>Main activity items list.</param>
        /// <returns><see cref="DataTemplate"/> Data template.</returns>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityItems)
        {
            mainActivityItems.Add(actDetail);

            var labelUnitNumberHeader = new Label
            {
                Text = "Unit #",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            var labelQtyHeader = new Label
            {
                Text = "Quantity",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };
            var labelWeightHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitNumberHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelUnitNumberHeader);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);

            Grid.SetColumn(labelUnitNumberHeader, 0);
            Grid.SetColumn(labelQtyHeader, 1);
            Grid.SetColumn(labelWeightHeader, 2);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                Convert.ToString(((ConvertUnitDetail)actDetail).CountNumber));
            var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailQuantity:F3}");
            var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailWeightQuantity:F3}");

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

            if (detailWidth2 > this.ColumnWidth2)
            {
                this.ColumnWidth2 = detailWidth2;
                gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
            }

            return (DataTemplate)this.ResourceDictionary["dataTemplateConvertUnitDetail"];
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
