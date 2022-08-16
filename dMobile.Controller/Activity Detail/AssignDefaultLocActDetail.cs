// <copyright file="AssignDefaultLocActDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="AssignDefaultLocActDetail" />.
    /// </summary>
    public class AssignDefaultLocActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignDefaultLocActDetail"/> class.
        /// </summary>
        /// <param name="activityItem"><see cref="ActItem"/> Activity item instance.</param>
        public AssignDefaultLocActDetail(ActItem activityItem)
          : base(activityItem)
        {
            this.FullClassName = typeof(AssignDefaultLocActDetail).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                Grid gridData = new Grid
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

                Label label1 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label2 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label3 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label4 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label5 = new Label { VerticalOptions = LayoutOptions.Center };
                Label label6 = new Label { VerticalOptions = LayoutOptions.Center };

                label1.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                label2.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                label3.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                label4.SetBinding(Label.TextProperty, new Binding(
                    "ActivityDetailQuantity",
                    BindingMode.Default,
                    null,
                    null,
                    "{0:F3}"));
                label5.SetBinding(Label.TextProperty, new Binding(
                    "ActivityDetailWeightQuantity",
                    BindingMode.Default,
                    null,
                    null,
                    "{0:F3}"));
                label6.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

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

            this.ResourceDictionary.Add("dataTemplate", dataTemplate);
        }

        /// <summary>
        /// Gets or sets the ListMoveDetails
        /// Gets or sets a list of move details.
        /// </summary>
        public List<MoveDetail> ListMoveDetails { get; set; } = new List<MoveDetail>();

        /// <summary>
        /// Gets or sets the grid column 0 width.
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width.
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width.
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width.
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width.
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 5 width.
        /// </summary>
        public double ColumnWidth5 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary.
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <param name="mainActivityItems"><see cref="ObservableCollection{object}"/> Main activity items.</param>
        /// <returns><see cref="ActivityDetail"/> Activity detail.</returns>
        public override ActivityDetail InsertListDetail(ObservableCollection<object> mainActivityItems)
        {
            ActivityDetail detail = null;
            if (this.ListDetails != null)
            {
                detail = new ActivityDetail();
                detail.ActivityDetailRefno = this.DetailRefno;
                detail.ActivityDetailFKRefno = this.DetailFKRefno;

                this.InsertListDetailProduct(detail);
                this.InsertListDetailQuantity(detail);
                this.InsertListDetailScheme(detail);
                this.InsertListDetailExtra(detail);
                this.InsertListDetailLocation(detail);

                detail.ActivityDetailSchemeID = this.SchemeID;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeID;
                ////detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
                this.ListDetails.Add(detail);
            }

            return detail;
        }

        /// <summary>
        /// Update detail in the detail list.
        /// </summary>
        /// <param name="listDetailIdx"><see cref="int"/> Zero based list detail index.</param>
        public override void UpdateListDetail(int listDetailIdx)
        {
            ActivityDetail detail = (ActivityDetail)this.ListDetails[listDetailIdx];
            ////if (detail.ActivityDetailStatus == (int)SysCommon.ActStatus.NoChange)
            ////{
            ////    detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Updated;
            ////}

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
        public override async Task Insert(XDocument data)
        {
            await Task.Run(() =>
            {
                if (this.ListDetails != null)
                {
                    this.ListDetails.Add(new MoveDetail()
                    {
                        ActivityDetailFKRefno = this.DetailFKRefno,
                        ActivityDetailRefno = this.DetailRefno,

                        ProductUnitRefno = this.ActItem.ProdInfo.UnitRefno,
                        ProductRefno = this.ActItem.ProdInfo.Refno,
                        ProductCode = this.ActItem.ProdInfo.Code,
                        ProductUnitBC = this.ActItem.ProdInfo.UnitBC,
                        UnitCode = this.ActItem.ProdInfo.UnitCode,
                        ProductDescription = this.ActItem.ProdInfo.Desc,
                        ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc,

                        ActivityDetailQuantity = Math.Round(this.Quantity, 3),
                        ActivityDetailBaseQuantity = Math.Round(this.BaseQuantity, 3),
                        ActivityDetailWeightQuantity = Math.Round(this.WeightQuantity, 3),

                        ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate,
                        ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate,
                        ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber,
                        ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber,
                        ProductSchemeVariantCode = this.SchemeData.VariantCode,
                        ProductSchemeVariantNo = this.SchemeData.VariantNumber,
                        ActivityDetailSchemeID = this.SchemeID,
                        ActivityDetailUniqueSchemeID = this.UniqueSchemeID,
                    });
                }

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            if (selectedActivityDetail is MoveDetail moveDetail)
            {
                await this.AssignThisFromDetailAsync(moveDetail, false);

                XmlDocument xmlDoc = new XmlDocument();
                string xmlSend = await this.CreateSubmitXML(data);
                string reqData = SysCommon.SubmitComm(xmlSend);

                if (reqData.Length > 0)
                {
                    xmlDoc.LoadXml(reqData);

                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail);
                        }
                        else
                        {
                            throw new Exception(errorDetail);
                        }
                    }
                    else
                    {
                        this.DetailFKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                        moveDetail.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);

                        await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                            "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                            Convert.ToInt32(SysCommon.ActStatus.NoChange),
                            this.DetailRefno);
                    }
                }

                this.ListDetails.Remove(moveDetail);
            }
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
            return Convert.ToInt32((selectedActivityDetail as MoveDetail).ActivityDetailRefno);
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create and fill the main grid data view.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>Filled main grid data view.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            DataTable table = new DataTable("ActivityMoveDetail");

            table.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ProductCode", Type.GetType("System.String"));
            table.Columns.Add("ProductUnitBC", Type.GetType("System.String"));
            table.Columns.Add("UnitCode", Type.GetType("System.String"));
            table.Columns.Add("ProductDescription", Type.GetType("System.String"));
            table.Columns.Add("ProductUnitSizeDescription", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailStatus", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantNo", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));

            this.ListMoveDetails = table.Rows.Cast<MoveDetail>().ToList();
            this.ListDetails = this.ListMoveDetails.Cast<object>().ToList();
            return this.ListDetails;
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

            Label labelHeader1 = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader2 = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader3 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader4 = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader5 = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader6 = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            this.ColumnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);

            // Create header data template
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
                },
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

            GridMainActivity.Children.Add(gridHeader);

            foreach (MoveDetail moveDetail in this.ListDetails)
            {
                observableCollection.Add(moveDetail);

                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.ProductUnitBC);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", moveDetail.ActivityDetailQuantity));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", moveDetail.ActivityDetailWeightQuantity));
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.ActivityDetailSchemeID);

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
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override Task<string> CreateDeleteXML(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string production = string.Empty;
            string bestBefore = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = this.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy");
            }

            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked)
            {
                production = this.SchemeData.LogisticUnitProductionDate.ToString("MM/dd/yyyy");
            }

            XDocument request = XDocument.Parse(
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
                + "<refno>" + this.ActItem.ActHdr.HdrFKRefno + "</refno>"
                + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
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
                + "<ser_refno>" + this.DetailFKRefno + "</ser_refno>"
                + "</imclient>");

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
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail"/> Activity detail.</param>
        /// <param name="mainActivityItems"><see cref="ObservableCollection(object)"/> Main activity items list.</param>
        /// <returns><see cref="DataTemplate"/> Data template.</returns>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityItems)
        {
            mainActivityItems.Add(actDetail);

            Label labelHeader1 = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader2 = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader3 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader4 = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader5 = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader6 = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            this.ColumnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);

            // Create header data template
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
                },
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

            gridMain.Children.Add(gridHeader);

            double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ProductCode);
            double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ProductUnitBC);
            double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", actDetail.ActivityDetailQuantity));
            double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", actDetail.ActivityDetailWeightQuantity));
            double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ActivityDetailSchemeID);

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

            return (DataTemplate)this.ResourceDictionary["dataTemplate"];
        }
    }
}
