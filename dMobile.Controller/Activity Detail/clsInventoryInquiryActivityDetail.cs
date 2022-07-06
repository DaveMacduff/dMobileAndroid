// <copyright file="clsInventoryInquiryActivityDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="InventoryInquiryActDetail" />.
    /// </summary>
    public class InventoryInquiryActDetail : ActDetail
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
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryInquiryActDetail"/> class.
        /// </summary>
        /// <param name="actItem">.</param>
        public InventoryInquiryActDetail(ActItem actItem)
          : base(actItem)
        {
            this.FullClassName = typeof(InventoryInquiryActDetail).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    },
                };

                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelSchemeID.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelSchemeID);

                Grid.SetColumn(labelQty, 0);
                Grid.SetColumn(labelWeight, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelSchemeID, 3);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
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
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetInfo.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="isAdd">.</param>
        /// <returns>.</returns>
        public override async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            await base.GetInfo(selectedActivityDetail, isAdd);

            if (!isAdd && selectedActivityDetail != null)
            {
                this.DetailFKRefno = Convert.ToInt32(selectedActivityDetail.GetType().GetProperty("ActivityDetailFKRefno")?.GetValue(selectedActivityDetail));
            }

            return this;
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
        public override async Task Refresh()
        {
            try
            {
                XDocument xmldocParams = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "</params>");

                string reqData = SysCommon.SubmitComm(await this.CreateSubmitXML(xmldocParams));

                this.ListDetails = await this.GetActivityDetailList(false);

                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);
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
                        using (DataSet ds = new DataSet())
                        {
                            ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                            if (ds.Tables.Count != 0)
                            {
                                // TODO: Test
                                foreach (DataRow row in ds.Tables["tActivityDetail"].Rows)
                                {
                                    InventoryInquiryDetail inquiryDetail = new InventoryInquiryDetail
                                    {
                                        ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                                        ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                                        UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                                        WtUnitCode = Convert.ToString(row["WeightUnitCode"] != DBNull.Value ? row["WeightUnitCode"] : null),
                                        ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                                        ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                                        ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                                    };

                                    this.ListDetails.Add(inquiryDetail);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Refresh inventory inquiry details failed.", ex);
            }
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as dynamic).ActivityDetailRefno;
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
        /// The GetActivityDetailList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            this.ListDetails = new List<object>();
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

            foreach (InventoryInquiryDetail inquiryDetail in this.ListDetails)
            {
                observableCollection.Add(inquiryDetail);
            }

            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSchemeIDHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelSchemeIDHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelSchemeIDHeader, 3);

            Grid.SetRow(gridHeader, 0);

            GridMainActivity.Children.Add(gridHeader);

            foreach (InventoryInquiryDetail inquiryDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", inquiryDetail.ActivityDetailQuantity));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", inquiryDetail.ActivityDetailWeightQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(inquiryDetail.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(inquiryDetail.ActivityDetailSchemeID);

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
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

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

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST INVENTORY INQUIRY DETAIL</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>"
                + "<bc>" + this.ActItem.ProdInfo.UnitBC + "</bc>"
                + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>"
                + "<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>"
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
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <param name="actDetailFKRefnoFldName">The actDetailFKRefnoFldName<see cref="string"/>.</param>
        /// <returns>The <see cref="ActivityDetail"/>.</returns>
        public override ActivityDetail InsertListDetail(string actDetailFKRefnoFldName)
        {
            InventoryInquiryDetail detail = null;
            if (this.IsUpdateView && this.ListDetails != null)
            {
                bool detailToBeAdded = false;
                if (this.DetailFKRefno > 0)
                {
                    detail = this.ListDetails.OfType<InventoryInquiryDetail>().FirstOrDefault(adjDetail =>
                        adjDetail.ActivityDetailFKRefno == this.DetailFKRefno);
                }

                if (detail == null)
                {
                    detail = new InventoryInquiryDetail();
                    detailToBeAdded = true;
                    detail.ActivityDetailRefno = this.DetailRefno;
                    detail.ActivityDetailFKRefno = this.DetailFKRefno;
                    detail.ActDetailFKRefnoFldName = actDetailFKRefnoFldName;

                    InsertListDetailProduct(detail);
                }

                InsertListDetailQuantity(detail);
                InsertListDetailScheme(detail);
                InsertListDetailExtra(detail);
                InsertListDetailLocation(detail);

                detail.ActivityDetailSchemeID = this.SchemeID;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeID;

                if (detailToBeAdded)
                {
                    this.ListDetails.Add(detail);
                }
            }

            return detail;

            //////////////////////////////////////////////////////////////////////////
            void InsertListDetailProduct(InventoryInquiryDetail detail)
            {
                detail.ProductRefno = this.ActItem.ProdInfo.Refno;
                detail.ProductUnitRefno = this.ActItem.ProdInfo.UnitRefno;
                detail.ProductCode = this.ActItem.ProdInfo.Code;
                detail.ProductUnitBC = this.ActItem.ProdInfo.UnitBC;
                detail.ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBC;
                detail.ProductDescription = this.ActItem.ProdInfo.Desc;
                detail.ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc;

                detail.UnitCode = this.ActItem.ProdInfo.UnitCode;
                detail.BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode;
                detail.WtUnitCode = this.ActItem.ProdInfo.WeightUnitCode;
            }

            //////////////////////////////////////////////////////////////////////////
            void InsertListDetailLocation(InventoryInquiryDetail detail)
            {
                detail.LocationMapRefno = this.LocMap.CurrLocMapRefno;

                int locCode = 0;
                string locName = string.Empty;

                if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SlotCode;
                    locName = this.LocMap.CurrLocMap.SlotName;
                }
                else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SectCode;
                    locName = this.LocMap.CurrLocMap.SectName;
                }
                else
                {
                    locCode = this.LocMap.CurrLocMap.WhsCode;
                    locName = this.LocMap.CurrLocMap.WhsName;
                }

                detail.LocationCode = locCode;
                detail.LocationName = locName;
            }

            //////////////////////////////////////////////////////////////////////////
            void InsertListDetailScheme(InventoryInquiryDetail detail)
            {
                detail.ProductSchemeVariantCode = this.SchemeData.VariantCode;
                detail.ProductSchemeVariantNo = this.SchemeData.VariantNumber;
                detail.ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber;
                detail.ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                detail.ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber;
                detail.ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate;
                detail.ActivityDetailLogisticUnitSkidNumber = this.SchemeData.LogisticUnitSkidNumber;
            }

            //////////////////////////////////////////////////////////////////////////
            void InsertListDetailExtra(InventoryInquiryDetail detail)
            {
                detail.ActivityDetailExtra1Text = this.ActExtra.Text1;
                detail.ActivityDetailExtra2Text = this.ActExtra.Text2;
                detail.ActivityDetailExtra3Text = this.ActExtra.Text3;
                detail.ActivityDetailExtra4Text = this.ActExtra.Text4;
                detail.ActivityDetailExtra5Date = this.ActExtra.DateTime1;
            }

            //////////////////////////////////////////////////////////////////////////
            void InsertListDetailQuantity(InventoryInquiryDetail detail)
            {
                detail.ActivityDetailQuantity = Convert.ToDecimal(detail.ActivityDetailQuantity) +
                    Math.Round(this.Quantity, 3);
                detail.ActivityDetailBaseQuantity = Convert.ToDecimal(detail.ActivityDetailBaseQuantity) +
                    Math.Round(this.BaseQuantity, 3);
                detail.ActivityDetailWeightQuantity = Convert.ToDecimal(detail.ActivityDetailWeightQuantity) +
                    Math.Round(this.WeightQuantity, 3);
            }
        }

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail"/> Activity detail.</param>
        /// <param name="mainActivityExtraInfoItems"><see cref="ObservableCollection(object)"/> Main activity items list.</param>
        /// <returns><see cref="DataTemplate"/> Data template.</returns>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityExtraInfoItems)
        {
            mainActivityExtraInfoItems.Add(actDetail);

            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSchemeIDHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelSchemeIDHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelSchemeIDHeader, 3);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", actDetail.ActivityDetailQuantity));
            double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", actDetail.ActivityDetailWeightQuantity));
            double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ActivityDetailSchemeID);

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

            return (DataTemplate)this.resourceDictionary["dataTemplate"];
        }
    }
}
