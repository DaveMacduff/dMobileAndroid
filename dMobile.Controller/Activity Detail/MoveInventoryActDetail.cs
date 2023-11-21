// <copyright file="MoveInventoryActDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

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
using dMobile.Controller.Activity_Item;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    ///     Defines the <see cref="MoveInventoryActDetail" />.
    /// </summary>
    public class MoveInventoryActDetail : ActDetail
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MoveInventoryActDetail" /> class.
        /// </summary>
        /// <param name="activityItem">.</param>
        public MoveInventoryActDetail(ActItem activityItem)
            : base(activityItem)
        {
            this.FullClassName = typeof(MoveInventoryActDetail).FullName;

            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplateMoveInventoryDetail")
                {
                    this.ResourceDictionary.Remove("dataTemplateMoveInventoryDetail");
                }
            }

            this.ResourceDictionary.Add("dataTemplateMoveInventoryDetail", this.CreateTemplate());
        }

        /// <summary>
        ///     Gets or sets the grid column 0 width...
        /// </summary>
        private double ColumnWidth0 { get; set; }

        /// <summary>
        ///     Gets or sets the grid column 1 width...
        /// </summary>
        private double ColumnWidth1 { get; set; }

        /// <summary>
        ///     Gets or sets the grid column 2 width...
        /// </summary>
        private double ColumnWidth2 { get; set; }

        /// <summary>
        ///     Gets or sets the grid column 3 width...
        /// </summary>
        private double ColumnWidth3 { get; set; }

        /// <summary>
        ///     Gets or sets the grid column 4 width...
        /// </summary>
        private double ColumnWidth4 { get; set; }

        /// <summary>
        ///     Gets or sets the grid column 5 width...
        /// </summary>
        private double ColumnWidth5 { get; set; }

        /// <summary>
        ///     Gets or sets the resource dictionary...
        /// </summary>
        private ResourceDictionary ResourceDictionary { get; } = new ResourceDictionary();

        /// <summary>
        ///     Gets or sets the ListMoveInventoryDetails
        ///     Gets or sets a list of picking details..
        /// </summary>
        private List<MoveDetail> ListMoveInventoryDetails { get; set; } = new List<MoveDetail>();

        /// <summary>
        ///     Find or insert an activity detail into the list details.
        /// </summary>
        /// <returns><see cref="ActivityDetail" /> Activity detail.</returns>
        public override ActivityDetail GatherDetail()
        {
            MoveDetail detail = null;
            if (this.ListDetails != null)
            {
                detail = new MoveDetail
                {
                    // KLUDGE: Restore the new detail reference number's that got over written somehow.
                    ActivityDetailRefno = this.SysEnv.InsertedDetailRefno,
                    ActivityDetailFKRefno = this.SysEnv.InsertedDetailFkRefno,
                };

                this.GatherProductDetail(detail);
                this.GatherQuantityDetail(detail);
                this.GatherSchemeDetail(detail);
                this.GatherExtraDetail(detail);
                this.GatherLocationDetail(detail);

                detail.ActivityDetailSchemeID = this.SchemeId;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeId;
                detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
            }

            return detail;
        }

        /// <summary>
        ///     Update detail in the detail list.
        /// </summary>
        /// <param name="listDetailIdx"><see cref="int" /> Zero based list detail index.</param>
        public override void UpdateListDetail(int listDetailIdx)
        {
            var detail = (MoveDetail)this.ListDetails[listDetailIdx];
            detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
        }

        /// <summary>
        ///     Insert a skid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public override Task InsertSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Delete a skid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public override Task DeleteSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public override async Task Insert(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The Delete.
        /// </summary>
        /// <returns>The <see cref="int" />.</returns>
        public override async Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The Submit.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object" />.</param>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            if (selectedActivityDetail == null)
            {
                var xmlSend = await this.CreateSubmitXmlAll(data);
                _ = SysCommon.SubmitComm(xmlSend);
            }
            else
            {
                var detail = (MoveDetail)selectedActivityDetail;
                await this.AssignActivityDetail(detail);

                var xmlDoc = new XmlDocument();
                var xmlSend = await this.CreateSubmitXml(data);
                var responseData = SysCommon.SubmitComm(xmlSend);
                xmlDoc.LoadXml(responseData);
                ((MoveDetail)selectedActivityDetail).ActivityDetailStatus = (int)SysCommon.ActStatus.NoChange;
                if (xmlDoc.DocumentElement?["ser_refno"] != null)
                {
                    this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                }

                this.ListDetails.Remove(selectedActivityDetail);
            }
        }

        /// <summary>
        ///     The Refresh.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        public override Task Refresh()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object" />.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return ((MoveDetail)selectedActivityDetail).ActivityDetailRefno;
        }

        /// <summary>
        ///     The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object" />.</param>
        /// <returns>.</returns>
        public override int GetFkRefno(object selectedActivityDetail)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The GetActivityDetailList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            var table = new DataTable("ActivityMoveDetail");
            table.Columns.Add(
                "ProductUnitRefno",
                Type.GetType("System.Int32") ?? throw new InvalidOperationException());
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
                "ProductSchemeVariantNo",
                Type.GetType("System.Int32") ?? throw new InvalidOperationException());
            table.Columns.Add(
                "ActivityDetailFKRefno",
                Type.GetType("System.Int32") ?? throw new InvalidOperationException());
            table.Columns.Add(
                "ActivityDetailRefno",
                Type.GetType("System.Int32") ?? throw new InvalidOperationException());

            this.ListMoveInventoryDetails = table.Rows.Cast<MoveDetail>().ToList();
            this.ListDetails = this.ListMoveInventoryDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <summary>
        ///     The DesignGrid.
        /// </summary>
        /// <param name="gridMain">The gridMain<see cref="Grid" />.</param>
        /// <param name="listViewMain">The listViewMain<see cref="ListView" />.</param>
        /// <returns>Observable collection of objects.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMain, ref ListView listViewMain)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var moveInventoryDetail in this.ListDetails.Cast<MoveDetail>())
            {
                observableCollection.Add(moveInventoryDetail);
            }

            // Grid column header labels.
            // Grid column header labels.
            var labelCode = new Label
            {
                Text = "Code",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelBc = new Label
            {
                Text = "BC",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelUnitCode = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelQuantity = new Label
            {
                Text = "Quantity",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                FontAttributes = FontAttributes.Bold,
            };

            var labelWeight = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                FontAttributes = FontAttributes.Bold,
            };

            var labelSchemeId = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var col1Size = Math.Max(labelCode.Text.Length, 10);
            var col2Size = Math.Max(labelBc.Text.Length, 1);
            var col3Size = Math.Max(labelUnitCode.Text.Length, 2);
            var col4Size = Math.Max(labelQuantity.Text.Length, 11);
            var col5Size = Math.Max(labelWeight.Text.Length, 11);
            var col6Size = Math.Max(labelCode.Text.Length, 30);
            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCode.Text.PadRight(col1Size));
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBc.Text.PadRight(col2Size));
            this.ColumnWidth2 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelUnitCode.Text.PadRight(col3Size));
            this.ColumnWidth3 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelQuantity.Text.PadRight(col4Size));
            this.ColumnWidth4 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelWeight.Text.PadRight(col5Size));
            this.ColumnWidth5 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelSchemeId.Text.PadRight(col6Size));

            // Column definitions.
            const int thicknessUniformSize = 5;
            var gridData = new Grid
            {
                Padding = new Thickness(thicknessUniformSize),
                ColumnDefinitions =
                {
                    // Product code.
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },

                    // BC
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },

                    // Unit of measure.
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },

                    // Alt quantity.
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },

                    // Weight quantity.
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },

                    // Scheme ID.
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth5, GridUnitType.Absolute) },
                },
            };

            gridData.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridData.Children.Add(labelCode);
            gridData.Children.Add(labelBc);
            gridData.Children.Add(labelUnitCode);
            gridData.Children.Add(labelQuantity);
            gridData.Children.Add(labelWeight);
            gridData.Children.Add(labelSchemeId);

            Grid.SetColumn(labelCode, 0);
            Grid.SetColumn(labelBc, 1);
            Grid.SetColumn(labelUnitCode, 2);
            Grid.SetColumn(labelQuantity, 3);
            Grid.SetColumn(labelWeight, 4);
            Grid.SetColumn(labelSchemeId, 5);

            Grid.SetRow(gridData, 0);

            gridMain.Children.Add(gridData);

            foreach (MoveDetail moveInventoryDetail in observableCollection)
            {
                var detailWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(moveInventoryDetail.ProductCode);
                var detailWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(moveInventoryDetail.ProductUnitBC);
                var detailWidth2 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(moveInventoryDetail.UnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth($"{moveInventoryDetail.ActivityDetailQuantity:F3}");
                var detailWidth4 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth($"{moveInventoryDetail.ActivityDetailWeightQuantity:F3}");
                var detailWidth5 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(moveInventoryDetail.ActivityDetailSchemeID);

                if (detailWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidth0;
                    gridData.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }

                if (detailWidth1 > this.ColumnWidth1)
                {
                    this.ColumnWidth1 = detailWidth1;
                    gridData.ColumnDefinitions[1].Width = this.ColumnWidth1;
                }

                if (detailWidth2 > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = detailWidth2;
                    gridData.ColumnDefinitions[2].Width = this.ColumnWidth2;
                }

                if (detailWidth3 > this.ColumnWidth3)
                {
                    this.ColumnWidth3 = detailWidth3;
                    gridData.ColumnDefinitions[3].Width = this.ColumnWidth3;
                }

                if (detailWidth4 > this.ColumnWidth4)
                {
                    this.ColumnWidth4 = detailWidth4;
                    gridData.ColumnDefinitions[4].Width = this.ColumnWidth4;
                }

                if (detailWidth5 > this.ColumnWidth5)
                {
                    this.ColumnWidth5 = detailWidth5;
                    gridData.ColumnDefinitions[5].Width = this.ColumnWidth5;
                }
            }

            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateMoveInventoryDetail"];

            return observableCollection;
        }

        /// <summary>
        ///     The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateDeleteXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The CreateSubmitXMLAll.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async Task<string> CreateSubmitXmlAll(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString, new XmlWriterSettings { Async = true });

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT MOVE ALL</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<whs_id>" + data.Root.Element("whs_id").Value + "</whs_id>"
                + "<sect_refno>" + data.Root.Element("sect_refno").Value + "</sect_refno>"
                + "<slot_refno>" + data.Root.Element("slot_refno").Value + "</slot_refno>"
                + "<to_sect_refno>" + data.Root.Element("to_sect_refno").Value + "</to_sect_refno>"
                + "<to_slot_refno>" + data.Root.Element("to_slot_refno").Value + "</to_slot_refno>"
                + "<refno>" + this.ActItem.ActHdr.HdrFkRefno + "</refno>"
                + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString, new XmlWriterSettings { Async = true });
            var production = string.Empty;
            var bestBefore = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = this.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy");
            }

            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
            {
                production = this.SchemeData.LogisticUnitProductionDate.ToString("MM/dd/yyyy");
            }

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT MOVE</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>"
                + "<is_bc>" + this.ActItem.ProdInfo.UnitIsBC + "</is_bc>"
                + "<whs_id>" + data.Root.Element("whs_id").Value + "</whs_id>"
                + "<sect_refno>" + data.Root.Element("sect_refno").Value + "</sect_refno>"
                + "<slot_refno>" + data.Root.Element("slot_refno").Value + "</slot_refno>"
                + "<to_sect_refno>" + data.Root.Element("to_sect_refno").Value + "</to_sect_refno>"
                + "<to_slot_refno>" + data.Root.Element("to_slot_refno").Value + "</to_slot_refno>"
                + "<refno>" + this.ActItem.ActHdr.HdrFkRefno + "</refno>"
                + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                + "<alt_qty>" + this.Quantity + "</alt_qty>"
                + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>"
                + "<base_qty>" + this.BaseQuantity + "</base_qty>"
                + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>"
                + "<wt_qty>" + this.WeightQuantity + "</wt_qty>"
                + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>"
                + "<serial>" + this.SchemeData.SingleTradeItemSerialNumber + "</serial>"
                + "<lot_no>" + this.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                + "<pdate>" + production + "</pdate>"
                + "<bdate>" + bestBefore + "</bdate>"
                + "<variant_no>" + this.SchemeData.VariantNumber + "</variant_no>"
                + "<ser_refno>" + this.DetailFkRefno + "</ser_refno>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string" /> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return "Move Detail";
        }

        /// <summary>
        ///     Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid" /> Activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail" /> Activity detail.</param>
        /// <param name="mainActivityItems"><see cref="ObservableCollection(object)" /> Main activity items list.</param>
        /// <returns><see cref="DataTemplate" /> Data template.</returns>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityItems)
        {
            // Causes the view to refresh.
            mainActivityItems.Add(actDetail);

            // Grid column header labels.
            // Grid column header labels.
            var labelCode = new Label
            {
                Text = "Code",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelBc = new Label
            {
                Text = "BC",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelUnitCode = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelQuantity = new Label
            {
                Text = "Quantity",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                FontAttributes = FontAttributes.Bold,
            };

            var labelWeight = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                FontAttributes = FontAttributes.Bold,
            };

            var labelSchemeId = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var col1Size = Math.Max(labelCode.Text.Length, 10);
            var col2Size = Math.Max(labelBc.Text.Length, 1);
            var col3Size = Math.Max(labelUnitCode.Text.Length, 2);
            var col4Size = Math.Max(labelQuantity.Text.Length, 11);
            var col5Size = Math.Max(labelWeight.Text.Length, 11);
            var col6Size = Math.Max(labelCode.Text.Length, 30);
            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ProductCode);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBc.Text.PadRight(col2Size));
            this.ColumnWidth2 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelUnitCode.Text.PadRight(col3Size));
            this.ColumnWidth3 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelQuantity.Text.PadRight(col4Size));
            this.ColumnWidth4 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelWeight.Text.PadRight(col5Size));
            this.ColumnWidth5 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelSchemeId.Text.PadRight(col6Size));

            // Header grid
            var gridHeader = new Grid
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
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelCode);
            gridHeader.Children.Add(labelBc);
            gridHeader.Children.Add(labelUnitCode);
            gridHeader.Children.Add(labelQuantity);
            gridHeader.Children.Add(labelWeight);
            gridHeader.Children.Add(labelSchemeId);

            Grid.SetColumn(labelCode, 0);
            Grid.SetColumn(labelBc, 1);
            Grid.SetColumn(labelUnitCode, 2);
            Grid.SetColumn(labelQuantity, 3);
            Grid.SetColumn(labelWeight, 4);
            Grid.SetColumn(labelSchemeId, 5);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ProductCode);
            var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ProductUnitBC);
            var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            var detailWidth3 = DependencyService.Get<IGridService>()
                .CalculateTextWidth($"{actDetail.ActivityDetailQuantity:F3}");
            var detailWidth4 = DependencyService.Get<IGridService>()
                .CalculateTextWidth($"{actDetail.ActivityDetailWeightQuantity:F3}");
            var detailWidth5 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(actDetail.ActivityDetailSchemeID);

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

            if (detailWidth3 > this.ColumnWidth3)
            {
                this.ColumnWidth3 = detailWidth3;
                gridHeader.ColumnDefinitions[3].Width = this.ColumnWidth3;
            }

            if (detailWidth4 > this.ColumnWidth4)
            {
                this.ColumnWidth4 = detailWidth4;
                gridHeader.ColumnDefinitions[4].Width = this.ColumnWidth4;
            }

            if (detailWidth5 > this.ColumnWidth5)
            {
                this.ColumnWidth5 = detailWidth5;
                gridHeader.ColumnDefinitions[5].Width = this.ColumnWidth5;
            }

            return (DataTemplate)this.ResourceDictionary["dataTemplateMoveInventoryDetail"];
        }

        /// <summary>
        ///     Create initial data template.
        /// </summary>
        /// <returns>Move inventory data template.</returns>
        private DataTemplate CreateTemplate()
        {
            var dataTemplate = new DataTemplate(() =>
            {
                // Grid column header labels.
                var labelCode = new Label
                {
                    Text = "Code",
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };

                var labelBc = new Label
                {
                    Text = "BC",
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };

                var labelUnitCode = new Label
                {
                    Text = string.Empty,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };

                var labelQuantity = new Label
                {
                    Text = "Quantity",
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                    FontAttributes = FontAttributes.Bold,
                };

                var labelWeight = new Label
                {
                    Text = "Weight",
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                    FontAttributes = FontAttributes.Bold,
                };

                var labelSchemeId = new Label
                {
                    Text = "Scheme ID",
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                };

                var col1Size = Math.Max(labelCode.Text.Length, 10);
                var col2Size = Math.Max(labelBc.Text.Length, 1);
                var col3Size = Math.Max(labelUnitCode.Text.Length, 2);
                var col4Size = Math.Max(labelQuantity.Text.Length, 11);
                var col5Size = Math.Max(labelWeight.Text.Length, 11);
                var col6Size = Math.Max(labelCode.Text.Length, 40);
                this.ColumnWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(labelCode.Text.PadRight(col1Size));
                this.ColumnWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(labelBc.Text.PadRight(col2Size));
                this.ColumnWidth2 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(labelUnitCode.Text.PadRight(col3Size));
                this.ColumnWidth3 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(labelQuantity.Text.PadRight(col4Size));
                this.ColumnWidth4 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(labelWeight.Text.PadRight(col5Size));
                this.ColumnWidth5 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(labelSchemeId.Text.PadRight(col6Size));

                // Column definitions.
                const int thicknessUniformSize = 5;
                var gridData = new Grid
                {
                    Padding = new Thickness(thicknessUniformSize),
                    ColumnDefinitions =
                    {
                        // Product code.
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },

                        // BC
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },

                        // Unit of measure.
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },

                        // Alt quantity.
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },

                        // Weight quantity.
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },

                        // Scheme ID.
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth5, GridUnitType.Absolute) },
                    },
                };

                labelCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelQuantity.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelSchemeId.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                gridData.Children.Add(labelCode);
                gridData.Children.Add(labelBc);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelQuantity);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelSchemeId);

                Grid.SetColumn(labelCode, 0);
                Grid.SetColumn(labelBc, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelQuantity, 3);
                Grid.SetColumn(labelWeight, 4);
                Grid.SetColumn(labelSchemeId, 5);

                return new ViewCell { View = gridData };
            });

            return dataTemplate;
        }
    }
}