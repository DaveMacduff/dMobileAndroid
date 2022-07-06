// <copyright file="clsPickingActivityDetail.cs" company="InterSect Business Systems Inc">
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
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Create the picking activity detail class.
    /// </summary>
    public class PickingActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingActDetail"/> class.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item instance.</param>
        public PickingActDetail(ActItem actItem)
          : base(actItem)
        {
            this.FullClassName = typeof(PickingActDetail).FullName;

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
                    },
                };

                Label labelPickingDetailConfirmed = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                Label labelPickingQuantity = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                Label labelPickingWeightQuantity = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                Label labelPickingUnitCode = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                Label labelPickingSchemeID = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                };

                labelPickingDetailConfirmed.SetBinding(Label.TextProperty, new Binding("ActivityDetailIsConfirmedString"));
                labelPickingQuantity.SetBinding(Label.TextProperty, new Binding(
                    "ActivityDetailQuantity",
                    BindingMode.Default,
                    null,
                    null,
                    "{0:F3}"));
                labelPickingWeightQuantity.SetBinding(Label.TextProperty, new Binding(
                    "ActivityDetailWeightQuantity",
                    BindingMode.Default,
                    null,
                    null,
                    "{0:F3}"));
                labelPickingUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelPickingSchemeID.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                if (this.SysEnv.SysSettings.IsPickConfirmDet)
                {
                    gridData.Children.Add(labelPickingDetailConfirmed);
                }

                gridData.Children.Add(labelPickingQuantity);
                gridData.Children.Add(labelPickingWeightQuantity);
                gridData.Children.Add(labelPickingUnitCode);
                gridData.Children.Add(labelPickingSchemeID);

                if (this.SysEnv.SysSettings.IsPickConfirmDet)
                {
                    Grid.SetColumn(labelPickingDetailConfirmed, 0);
                }

                Grid.SetColumn(labelPickingQuantity, 1);
                Grid.SetColumn(labelPickingWeightQuantity, 2);
                Grid.SetColumn(labelPickingUnitCode, 3);
                Grid.SetColumn(labelPickingSchemeID, 4);

                return new ViewCell { View = gridData };
            });

            this.ResourceDictionary.Add("dataTemplate", dataTemplate);
        }

        /// <summary>
        /// Gets or sets the grid column 0 width..
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width..
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width..
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width..
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width..
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary..
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets a list of picking details.
        /// </summary>
        public List<PickingDetail> ListPickingDetails { get; set; } = new List<PickingDetail>();

        /// <summary>
        /// Perform the insert skid call to the comm server, and fill the this activity detail instance.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo"/> Skid information.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task InsertSkid(SysSkidInfo skidInfo)
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>"
                + "</params>");

            string xmlSend = await this.CreateSubmitXML(request);
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
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
                    this.IsUpdateView = true;
                    this.DetailFKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.NewItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                    this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
                    this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
                    this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);

                    if (this.NewItemFKRefno != this.ActItem.ItemFKRefno)
                    {
                        await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                            "UPDATE tActivityItemPicking SET ActivityItemPickingStringFKRefno = ?," +
                            " ActivityItemPickingFKRefno = ? WHERE ActivityItemRefno = ?",
                            Convert.ToString(this.NewItemFKRefno),
                            this.NewItemFKRefno,
                            this.ActItem.ItemRefno);

                        this.ActItem.ItemFKRefno = this.NewItemFKRefno;

                        PickingItem pickingItem = this.ActItem.ListItems.OfType<PickingItem>().FirstOrDefault(
                            pickingItem => pickingItem.ActivityItemRefno == this.ActItem.ItemRefno);

                        if (pickingItem != null)
                        {
                            pickingItem.ActivityItemPickingFKRefno = this.NewItemFKRefno;
                            pickingItem.ActivityItemPickingStringFKRefno = this.NewItemFKRefno.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The DeleteSkid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task DeleteSkid(SysSkidInfo skidInfo)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete;
                xmlDelete = await this.CreateDeleteXML(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

                string reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;

                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail);
                        }
                        else
                        {
                            throw new Exception(errorDetail);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data)
        {
            await this.FillActDetailObjFromXML(data);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                int actDetailRefno = await this.InsertDetailDisconnected();
                string query = "INSERT INTO tActivityDetailPicking (ActivityDetailRefno, ActivityDetailPickingFKRefno," +
                    " ActivityDetailPickingIsConfirmed) VALUES (?, ?, ?)";
                _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    query,
                    Convert.ToInt32(actDetailRefno),
                    Convert.ToInt32(this.DetailFKRefno),
                    true);

                this.IsUpdateView = true;
            }
            else
            {
                if (this.InsertDetail(await this.CreateSubmitXML(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"))))
                {
                    string query = "UPDATE tActivityItemPicking SET ActivityItemPickingStringFKRefno = ?," +
                        " ActivityItemPickingFKRefno = ? WHERE ActivityItemRefno = ?";
                    _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                        query,
                        Convert.ToString(this.NewItemFKRefno),
                        this.NewItemFKRefno,
                        this.ActItem.ItemRefno);

                    // TODO: Fix sorting
                    // string oldSort = this.ActItem.DataViewItems.Sort;
                    // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                    // int rowIdx = this.ActItem.DataViewItems.Find(new object[] { this.ActItem.Refno });
                    PickingItem pickingItem = this.ActItem.ListItems.OfType<PickingItem>().Where(
                        pickingItem => pickingItem.ActivityItemRefno == this.ActItem.ItemRefno).FirstOrDefault();

                    if (pickingItem != null)
                    {
                        pickingItem.ActivityItemPickingFKRefno = this.NewItemFKRefno;
                        pickingItem.ActivityItemPickingStringFKRefno = Convert.ToString(this.NewItemFKRefno);
                    }

                    // TODO: Fix sorting
                    // this.ActItem.DataViewItems.Sort = oldSort;
                }
            }
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete = await this.CreateDeleteXML(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

                string reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;

                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail.Substring(7));
                        }
                        else
                        {
                            throw new Exception("Request delete detail error." + Environment.NewLine + errorDetail);
                        }
                    }
                }
                else
                {
                    throw new Exception("Unexpected delete activity detail error.");
                }
            }

            // TODO: Fix sorting
            // string currSort = this.DataViewDetails.Sort;
            // this.DataViewDetails.Sort = "ActivityDetailPickingFKRefno";
            // int rowIdx = this.DataViewDetails.Find(this.FKRefno);
            PickingDetail pickingDetail = this.ActItem.ListItems.OfType<PickingDetail>().FirstOrDefault(
                pickingDetail => pickingDetail.ActivityDetailFKRefno == this.DetailFKRefno);

            if (this.ListDetails != null && pickingDetail != null)
            {
                // delete row from List
                this.ListDetails.Remove(pickingDetail);
            }

            // TODO: Fix sorting
            // this.DataViewDetails.Sort = currSort;

            // Delete from database
            string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = ?";
            _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetailPicking WHERE ActivityDetailRefno = ?";
            _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetailExtra WHERE ActivityDetailRefno = ?";
            _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetail WHERE ActivityDetailRefno = ?";
            _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            return 0;
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = await this.CreateSubmitXML(XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
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
                        throw new MessageOnlyException(errorDetail.Substring(7));
                    }
                    else
                    {
                        throw new Exception("Request add detail error." + Environment.NewLine + errorDetail);
                    }
                }
                else
                {
                    this.DetailFKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

                    if (this.ListDetails != null && selectedActivityDetail is PickingDetail pickingDetail)
                    {
                        pickingDetail.ActivityDetailFKRefno = this.DetailFKRefno;
                        pickingDetail.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);
                }
            }
        }

        /// <summary>
        /// The GetInfo.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="isAdd">.</param>
        /// <returns>.</returns>
        public override async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            if (selectedActivityDetail is PickingDetail pickingDetail)
            {
                _ = await base.GetInfo(pickingDetail, isAdd);

                if (!isAdd)
                {
                    this.DetailFKRefno = Convert.ToInt32(pickingDetail.ActivityDetailFKRefno);
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        Convert.ToInt32(pickingDetail.LocationMapRefno));
                }
            }

            return this;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityPickingDetail tableActPickingDetail = new TableActivityPickingDetail();
                await tableActPickingDetail.PurgeData();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActPickingDetail.RegisterDataAccessListenerProgressOut((PickingActDetail)this);
                await tableActPickingDetail.RequestData(this.ActItem.ItemRefno);
                tableActPickingDetail.UnregisterDataAccessListenerProgressOut((PickingActDetail)this);
            }
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as PickingDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as PickingDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// Get a data view of the activity detail.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>Data view result of the query.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno AS ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSlot.LocationMapSlotCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Slot\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = ");
                sql.Append("tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Section\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Whs\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }
            else
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno AS ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSlot.LocationMapSlotCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Slot\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Section\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Whs\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }

            this.ListPickingDetails = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<PickingDetail>(sql.ToString());
            this.ListDetails = this.ListPickingDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="mainActivity"><see cref="ListView"/> Main activity.</param>
        /// <returns><see cref="ObservableCollection(object)"/> Observable collection.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid gridMain,
            ref ListView mainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (PickingDetail pickingDetail in this.ListDetails)
            {
                observableCollection.Add(pickingDetail);
            }

            Label labelPickingDetailConfirmedHeader = new Label
            {
                Text = "C",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingQuantityHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingWeightQuantityHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingSchemeIDHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingDetailConfirmedHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingQuantityHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingWeightQuantityHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingUnitCodeHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingSchemeIDHeader.Text);

            // Header grid
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
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                gridHeader.Children.Add(labelPickingDetailConfirmedHeader);
            }

            gridHeader.Children.Add(labelPickingQuantityHeader);
            gridHeader.Children.Add(labelPickingWeightQuantityHeader);
            gridHeader.Children.Add(labelPickingUnitCodeHeader);
            gridHeader.Children.Add(labelPickingSchemeIDHeader);

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                Grid.SetColumn(labelPickingDetailConfirmedHeader, 0);
            }

            Grid.SetColumn(labelPickingQuantityHeader, 1);
            Grid.SetColumn(labelPickingWeightQuantityHeader, 2);
            Grid.SetColumn(labelPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelPickingSchemeIDHeader, 4);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            foreach (PickingDetail pickingDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    pickingDetail.ActivityDetailIsConfirmedString);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", pickingDetail.ActivityDetailQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", pickingDetail.ActivityDetailWeightQuantity));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingDetail.UnitCode);
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    pickingDetail.ActivityDetailSchemeID);

                if (this.SysEnv.SysSettings.IsPickConfirmDet)
                {
                    if (detailWidth0 > this.ColumnWidth0)
                    {
                        this.ColumnWidth0 = detailWidth0;
                        gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                    }
                }
                else
                {
                    this.ColumnWidth0 = 0;
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
            }

            mainActivity.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplate"];

            return observableCollection;
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

            Label labelPickingDetailConfirmedHeader = new Label
            {
                Text = "C",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingQuantityHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingWeightQuantityHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelPickingSchemeIDHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingDetailConfirmedHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingQuantityHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingWeightQuantityHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingUnitCodeHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingSchemeIDHeader.Text);

            // Header grid
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
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                gridHeader.Children.Add(labelPickingDetailConfirmedHeader);
            }

            gridHeader.Children.Add(labelPickingQuantityHeader);
            gridHeader.Children.Add(labelPickingWeightQuantityHeader);
            gridHeader.Children.Add(labelPickingUnitCodeHeader);
            gridHeader.Children.Add(labelPickingSchemeIDHeader);

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                Grid.SetColumn(labelPickingDetailConfirmedHeader, 0);
            }

            Grid.SetColumn(labelPickingQuantityHeader, 1);
            Grid.SetColumn(labelPickingWeightQuantityHeader, 2);
            Grid.SetColumn(labelPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelPickingSchemeIDHeader, 4);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                ((PickingDetail)actDetail).ActivityDetailIsConfirmedString);
            double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                string.Format("{0:F3}", actDetail.ActivityDetailQuantity));
            double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                string.Format("{0:F3}", actDetail.ActivityDetailWeightQuantity));
            double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                actDetail.ActivityDetailSchemeID);
            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                if (detailWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }
            }
            else
            {
                this.ColumnWidth0 = 0;
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

            return (DataTemplate)this.ResourceDictionary["dataTemplate"];
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            this.ListDetails = this.ListPickingDetails.Where(pickDetail => pickDetail.ActivityDetailFKRefno.
                ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (PickingDetail pickingDetail in this.ListDetails)
            {
                observableCollection.Add(pickingDetail);
            }

            return observableCollection;
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
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="sendData">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXML(XDocument sendData)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request;
            if (this.ActItem.ActHdr.XMLDataCriteria == null)
            {
                if (sendData.Document.Root.Element("skid_data") == null)
                {
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id></whs_id>"
                            + "<sect_refno></sect_refno>"
                            + "</command>"
                            + await SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + "<skid_fk_refno>" + 0 + "</skid_fk_refno>"
                            + "</imclient>");
                    }
                    else
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id></whs_id>"
                            + "<sect_refno></sect_refno>"
                            + "</command>"
                            + await SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                            + "</imclient>");
                    }
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT SALES ORDER DETAIL SKID</action>"
                        + "<whs_id></whs_id>"
                        + "<sect_refno></sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<skid_data>" + sendData.Root.Element("skid_data").Value + "</skid_data>"
                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                        + "<refno>" + this.ActItem.ActHdr.HdrFKRefno + "</refno>"
                        + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                        + "<is_negate_qty>false</is_negate_qty>"
                        + "<extra1>" + this.ActExtra.Text1 + "</extra1>"
                        + "<extra2>" + this.ActExtra.Text2 + "</extra2>"
                        + "<extra3>" + this.ActExtra.Text3 + "</extra3>"
                        + "<extra4>" + this.ActExtra.Text4 + "</extra4>"
                        + "<extra5>" + this.ActExtra.DateTime1 + "</extra5>"
                        + "</imclient>");
                }
            }
            else
            {
                int whsRefno = Convert.ToInt32(this.ActItem.ActHdr.XMLDataCriteria.Root.Element("whs_id").Value);
                int sectRefno = Convert.ToInt32(this.ActItem.ActHdr.XMLDataCriteria.Root.Element("sect_refno").Value);

                if (sendData.Document.Root.Element("skid_data") == null)
                {
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id>" + whsRefno + "</whs_id>"
                            + "<sect_refno>" + sectRefno + "</sect_refno>"
                            + "</command>"
                            + await SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + "<skid_fk_refno>" + 0 + "</skid_fk_refno>"
                            + "</imclient>");
                    }
                    else
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id>" + whsRefno + "</whs_id>"
                            + "<sect_refno>" + sectRefno + "</sect_refno>"
                            + "</command>"
                            + await SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                            + "</imclient>");
                    }
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT SALES ORDER DETAIL SKID</action>"
                        + "<whs_id>" + whsRefno + "</whs_id>"
                        + "<sect_refno>" + sectRefno + "</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<skid_data>" + sendData.Root.Element("skid_data").Value + "</skid_data>"
                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                        + "<refno>" + this.ActItem.ActHdr.HdrFKRefno + "</refno>"
                        + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                        + "<is_negate_qty>false</is_negate_qty>"
                        + "<extra1>" + this.ActExtra.Text1 + "</extra1>"
                        + "<extra2>" + this.ActExtra.Text2 + "</extra2>"
                        + "<extra3>" + this.ActExtra.Text3 + "</extra3>"
                        + "<extra4>" + this.ActExtra.Text4 + "</extra4>"
                        + "<extra5>" + this.ActExtra.DateTime1 + "</extra5>"
                        + "</imclient>");
                }
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>DELETE SALES ORDER DETAIL</action>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<skid_fk_refno>" + 0 + "</skid_fk_refno>"
                        + "</imclient>");
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>DELETE SALES ORDER DETAIL</action>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                        + "</imclient>");
                }
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE SALES ORDER DETAIL SKID</action>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<refno>" + this.ActItem.ActHdr.HdrFKRefno + "</refno>"
                    + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Total all activity detail quantity from the data view.
        /// </summary>
        /// <returns>SFillQuantity structure.</returns>
        public override async Task<SFillQuantity> GetDataViewTotalQuantity()
        {
            SFillQuantity fillQty = new SFillQuantity();

            if (this.ListDetails != null && this.ListDetails.Count > 0)
            {
                foreach (PickingDetail pickingDetail in this.ListDetails)
                {
                    if (this.SysEnv.SysSettings.IsPickConfirmDet)
                    {
                        if (Convert.ToBoolean(pickingDetail.ActivityDetailIsConfirmed) == true)
                        {
                            fillQty.Quantity += Convert.ToDecimal(pickingDetail.ActivityDetailQuantity);
                            fillQty.BaseQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailBaseQuantity);
                            fillQty.WeightQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailWeightQuantity);
                        }
                    }
                    else
                    {
                        fillQty.Quantity += Convert.ToDecimal(pickingDetail.ActivityDetailQuantity);
                        fillQty.BaseQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailBaseQuantity);
                        fillQty.WeightQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailWeightQuantity);
                    }
                }
            }

            return fillQty;
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            ////string prod = $"Prod: {this.ActItem.ProdInfo.Code} {this.ActItem.ProdInfo.UnitBC}
            ////{this.ActItem.ProdInfo.UnitCode}";
            ////return prod;

            return string.Format("Pick # {0}", this.ActItem.ActHdr.HdrFKRefno);
        }

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <param name="actDetailFKRefnoFldName">The actDetailFKRefnoFldName<see cref="string"/>.</param>
        /// <returns>The <see cref="ActivityDetail"/>.</returns>
        public override ActivityDetail InsertListDetail(string actDetailFKRefnoFldName)
        {
            PickingDetail detail = null;
            if (this.IsUpdateView && this.ListDetails != null)
            {
                bool detailToBeAdded = false;
                if (this.DetailFKRefno > 0)
                {
                    detail = this.ListDetails.OfType<PickingDetail>().FirstOrDefault(adjDetail =>
                        adjDetail.ActivityDetailFKRefno == this.DetailFKRefno);
                }

                if (detail == null)
                {
                    detail = new PickingDetail();
                    detailToBeAdded = true;
                    detail.ActivityDetailRefno = 0;
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
                    detail.ActivityDetailQuantity = 0;
                    detail.ActivityDetailBaseQuantity = 0;
                    detail.ActivityDetailWeightQuantity = 0;
                    detail.ActDetailFKRefnoFldName = actDetailFKRefnoFldName;
                    detail.ProductBrandDescription = this.ActItem.ProdInfo.BrandDesc;
                    detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
                }
                else
                {
                    detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Updated;
                }

                detail.LocationMapRefno = this.LocMap.CurrLocMapRefno;
                detail.ActivityDetailQuantity = Convert.ToDecimal(detail.ActivityDetailQuantity) +
                    Math.Round(this.Quantity, 3);
                detail.ActivityDetailBaseQuantity = Convert.ToDecimal(detail.ActivityDetailBaseQuantity) +
                    Math.Round(this.BaseQuantity, 3);
                detail.ActivityDetailWeightQuantity = Convert.ToDecimal(detail.ActivityDetailWeightQuantity) +
                    Math.Round(this.WeightQuantity, 3);
                detail.ProductSchemeVariantCode = this.SchemeData.VariantCode;
                detail.ProductSchemeVariantNo = this.SchemeData.VariantNumber;
                detail.ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber;
                detail.ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                detail.ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber;
                detail.ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate;
                detail.ActivityDetailLogisticUnitSkidNumber = this.SchemeData.LogisticUnitSkidNumber;
                detail.ActivityDetailExtra1Text = this.ActExtra.Text1;
                detail.ActivityDetailExtra2Text = this.ActExtra.Text2;
                detail.ActivityDetailExtra3Text = this.ActExtra.Text3;
                detail.ActivityDetailExtra4Text = this.ActExtra.Text4;
                detail.ActivityDetailExtra5Date = this.ActExtra.DateTime1;

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
                detail.ActivityDetailSchemeID = this.SchemeID;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeID;

                if (detailToBeAdded)
                {
                    this.ListDetails.Add(detail);
                }
            }

            return detail;
        }
    }
}
