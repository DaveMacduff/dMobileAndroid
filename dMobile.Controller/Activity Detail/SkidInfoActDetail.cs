// <copyright file="SkidInfoActDetail.cs" company="InterSect Business Systems Inc">
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
using dMobile.Resources.Models;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Defines the <see cref="SkidInfoActDetail" />.
    /// </summary>
    public class SkidInfoActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkidInfoActDetail"/> class.
        /// </summary>
        /// <param name="actItem">.</param>
        public SkidInfoActDetail(ActItem actItem)
            : base(actItem)
        {
            this.FullClassName = typeof(SkidInfoActDetail).FullName;

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
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                    },
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

                var labelUnitCode = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                var labelSchemeID = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                labelQty.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
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

            var isAddKey = true;
            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplateSkidInfoDetail")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplateSkidInfoDetail", dataTemplate);
            }
        }

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
            SkidInfoDetail detail = null;
            if (this.ListDetails != null)
            {
                detail = new SkidInfoDetail();
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
            var detail = (SkidInfoDetail)this.ListDetails[listDetailIdx];
            ////if (detail.ActivityDetailStatus == (int)SysCommon.ActStatus.NoChange)
            ////{
            ////    detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Updated;
            ////}

            detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
        }

        /// <summary>
        /// Insert a new skid into the detail.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo"/> Skid information to insert.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task InsertSkid(SysSkidInfo skidInfo)
        {
            throw new NotSupportedException("Inserting a skid from within a skid detail is not supported.");
        }

        /// <summary>
        /// Delete a skid from the detail.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo"/> Skid information to delete.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task DeleteSkid(SysSkidInfo skidInfo)
        {
            throw new NotSupportedException("Deleting a skid from within a skid detail is not supported.");
        }

        /// <summary>
        /// Insert a new skid detail.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Detail to add to the skid.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data)
        {
            await this.FillActDetailObjFromXML(data);

            var xmlSend = await this.CreateSubmitXML(XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

            var reqData = SysCommon.SubmitComm(xmlSend);
            if (reqData.Length > 0)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(reqData);

                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
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
                    this.DetailRefno = this.DetailFKRefno;
                }
            }
        }

        /// <summary>
        /// Delete the currently selected skid activity detail.
        /// </summary>
        /// <returns><see cref="Task{int}"/> Always returns zero.</returns>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlDelete = await this.CreateDeleteXML(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                var reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        var errorDetail = docError.DocumentElement["detail"].InnerText;
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
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);
                        var tempSkidInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                        tempSkidInfo.IsConsumed = bool.Parse(xmlDoc.DocumentElement["IsConsumed"].InnerText);
                        this.ActItem.ActHdr.SkidInfo.CurrSkid = tempSkidInfo;
                    }
                }
                else
                {
                    throw new Exception("Unexpected delete activity detail error.");
                }
            }

            var skidInfoDetail = this.ListDetails.OfType<SkidInfoDetail>().Where(
                skidInfoDetail => skidInfoDetail.ActivityDetailFKRefno == this.DetailFKRefno).FirstOrDefault();

            if (skidInfoDetail != null)
            {
                this.ListDetails.Remove(skidInfoDetail);
            }

            return 0;
        }

        /// <summary>
        /// Submit a skid information activity detail.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected skid information activity detail.</param>
        /// <param name="data"><see cref="XDocument"/> Detail to add to the skid.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            var xmlDoc = new XmlDocument();
            var xmlSend = await this.CreateSubmitXML(XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            var reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                xmlDoc.LoadXml(reqData);

                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
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
                    this.DetailRefno = this.DetailFKRefno;

                    if (selectedActivityDetail is SkidInfoDetail skidInfoDetail)
                    {
                        skidInfoDetail.ActivityDetailFKRefno = this.DetailFKRefno;
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the list of all skid details.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Refresh()
        {
            if (this.ListDetails != null)
            {
                this.ListDetails.Clear();
            }

            var ds = new DataSet();
            SysSkidInfo.GetSkidDetail(this.ActItem.ActHdr.SkidInfo, ref ds);

            if (ds.Tables.Count != 0)
            {
                foreach (DataRow drow in ds.Tables["cskiddetail"].Rows)
                {
                    // NOTE: The drow data is all strings because it was filled from xml data in SysSkidInfo.GetSkidDetail.
                    // Need to do a data type conversion not just a cast.
                    this.ActItem.ActHdr.HdrFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                    this.ActItem.ActHdr.HdrRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                    this.ActItem.ItemFKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                    this.ActItem.ItemRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                    this.DetailFKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                    this.DetailRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                    this.SchemeID = Convert.ToString(drow["ActivityDetailSchemeID"]);
                    this.UniqueSchemeID = Convert.ToString(drow["ActivityDetailUniqueSchemeID"]);
                    this.Quantity = Convert.ToDecimal(drow["ActivityDetailQuantity"]);
                    this.BaseQuantity = Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]);
                    this.WeightQuantity = Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]);

                    var tempSchemeData = this.SchemeData;
                    tempSchemeData.LogisticUnitBatchLotNumber = Convert.ToString(
                        drow["ActivityDetailLogisticUnitBatchLotNumber"]);
                    tempSchemeData.SingleTradeItemSerialNumber = Convert.ToString(
                        drow["ActivityDetailSingleTradeItemSerialNumber"]);
                    tempSchemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno.ToString();
                    tempSchemeData.VariantNumber = Convert.ToInt32(drow["ProductSchemeVariantNo"]);

                    var results = await DatabaseConnection.GetInstance().SQLConn.
                        QueryAsync<tProductSchemeVariantModel>(
                        "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                        tempSchemeData.VariantNumber);

                    if (results?.Count > 0)
                    {
                        tempSchemeData.VariantCode = results[0].ProductSchemeVariantCode;
                        tempSchemeData.VariantDescription = results[0].ProductSchemeVariantDescription;
                    }

                    var newDate = "01-01-1900";

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitBestBeforeSellByDate = Convert.ToDateTime(
                            drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]);
                    }

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitExpiryUseByDate = Convert.ToDateTime(
                            drow["ActivityDetailLogisticUnitExpiryUseByDate"]);
                    }

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitProductionDate = Convert.ToDateTime(
                            drow["ActivityDetailLogisticUnitProductionDate"]);
                    }

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitDueDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitDueDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitDueDate = Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]);
                    }

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitPackagingDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitPackagingDate = Convert.ToDateTime(
                            drow["ActivityDetailLogisticUnitPackagingDate"]);
                    }

                    this.SchemeData = tempSchemeData;
                    this.FillDataView();
                }
            }
        }

        /// <summary>
        /// Get the selected activity detail reference number.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="int"/> Activity detail reference number.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as SkidInfoDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// Get the selected activity detail foreign key reference number.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="int"/> Selected activity detail foreign key reference number.</returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as SkidInfoDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// Get the activity skid detail list.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool"/> Are we in batch submission mode.</param>
        /// <returns><see cref="List{object}"/> List of skid details.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            var table = new DataTable("ActivitySkidDetail");

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
            table.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
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

            var details = table.Rows.Cast<SkidInfoDetail>().ToList();
            this.ListDetails = details.Cast<object>().ToList();
            return this.ListDetails;
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

            foreach (SkidInfoDetail skidInfoDetail in this.ListDetails)
            {
                observableCollection.Add(skidInfoDetail);
            }

            var labelQtyHeader = new Label
            {
                Text = "Qty",
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

            var labelUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelSchemeIDHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

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

            foreach (SkidInfoDetail skidInfoDetail in observableCollection)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", skidInfoDetail.ActivityDetailQuantity));
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", skidInfoDetail.ActivityDetailWeightQuantity));
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(skidInfoDetail.UnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    skidInfoDetail.ActivityDetailSchemeID);

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
            }

            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateSkidInfoDetail"];

            return observableCollection;
        }

        /// <summary>
        /// Perform a search for an activity detail.
        /// </summary>
        /// <param name="searchText"><see cref="string"/> Search text.</param>
        /// <returns><see cref="ObservableCollection{object}"/> Found object or null.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Select the activity detail.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task AssignActivityDetail(object selectedActivityDetail)
        {
            await base.AssignActivityDetail(selectedActivityDetail);
        }

        /// <summary>
        /// Create delete product from skid XML.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Skid product detail to delete.</param>
        /// <returns><see cref="string"/> Request string.</returns>
        public override async Task<string> CreateDeleteXML(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            ////await this.FillActDetailObjFromXML(data);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT REMOVE PRODUCT FROM SKID</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + this.GetRequestDetailDefaults()
                + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Add a product detail to a skid.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Skid and product info to add.</param>
        /// <returns><see cref="string"/> Request string.</returns>
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            ////await this.FillActDetailObjFromXML(data);
            var reqDetail = this.GetRequestDetailDefaults();
            var def = await SysCommon.GetRequestDefaults(false);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT ADD PRODUCT TO SKID</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + def
                + reqDetail
                + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Add to the detail list a skid detail information.
        /// </summary>
        public void FillDataView()
        {
            if (this.ListDetails != null)
            {
                var locMap = LocationMap.GetInstance().CurrLocMap;

                string locName;
                int locCode;
                if (locMap.SlotLocMapRefno > 0)
                {
                    locCode = locMap.SlotCode;
                    locName = locMap.SlotName;
                }
                else if (locMap.SectLocMapRefno > 0)
                {
                    locCode = locMap.SectCode;
                    locName = locMap.SectName;
                }
                else
                {
                    locCode = locMap.WhsCode;
                    locName = locMap.WhsName;
                }

                this.ListDetails.Add(new SkidInfoDetail()
                {
                    LocationMapRefno = LocationMap.GetInstance().CurrLocMapRefno,
                    LocationCode = locCode,
                    LocationName = locName,
                    ProductRefno = this.ActItem.ProdInfo.Refno,
                    ProductCode = this.ActItem.ProdInfo.Code,
                    ProductUnitBC = this.ActItem.ProdInfo.UnitBC,
                    ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBC,
                    ProductDescription = this.ActItem.ProdInfo.Desc,
                    ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc,
                    UnitCode = this.ActItem.ProdInfo.UnitCode,
                    BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode,
                    WtUnitCode = this.ActItem.ProdInfo.WeightUnitCode,
                    ProductUnitRefno = this.ActItem.ProdInfo.UnitRefno,
                    ActivityDetailQuantity = this.Quantity,
                    ActivityDetailBaseQuantity = this.BaseQuantity,
                    ActivityDetailWeightQuantity = this.WeightQuantity,
                    ActivityDetailSchemeID = this.SchemeID,
                    ActivityDetailUniqueSchemeID = this.UniqueSchemeID,
                    ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate,
                    ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate,
                    ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber,
                    ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber,
                    ProductSchemeVariantCode = this.SchemeData.VariantCode,
                    ProductSchemeVariantNo = this.SchemeData.VariantNumber,
                    ActivityDetailFKRefno = this.DetailFKRefno,
                    ActivityDetailRefno = this.DetailRefno,
                });
            }
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return string.Format("Skid # {0}", this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
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

            var labelQtyHeader = new Label
            {
                Text = "Qty",
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

            var labelUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelSchemeIDHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

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

            var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                string.Format("{0:F3}", actDetail.ActivityDetailQuantity));
            var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                string.Format("{0:F3}", actDetail.ActivityDetailWeightQuantity));
            var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                actDetail.ActivityDetailSchemeID);

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

            return (DataTemplate)this.ResourceDictionary["dataTemplateSkidInfoDetail"];
        }
    }
}
