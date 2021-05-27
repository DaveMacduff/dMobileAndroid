namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using System.Linq;
    using System.Collections.Generic;

    public class CycleCountActDetail : ActDetail
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<CountDetail> listCountDetails = new List<CountDetail>();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItem"></param>
        public CycleCountActDetail(ActItem actItem)
            : base(actItem)
        {
            this.FullClassName = typeof(CycleCountActDetail).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                    }
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

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override async Task InsertSkid(SysSkidInfo skidInfo)
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>")
                + "</params>");

            string reqData = SysCommon.SubmitComm(await this.CreateSubmitXML(request));

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
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.NewItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                    this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
                    this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
                    this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);

                    if (this.NewItemFKRefno != this.ActItem.FKRefno)
                    {
                        string query = "UPDATE tActivityItemCount SET ActivityItemCountStringFKRefno = ?, ActivityItemCountFKRefno = ? WHERE ActivityItemRefno = ?";
                        await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToString(this.NewItemFKRefno), this.NewItemFKRefno, this.ActItem.Refno);

                        this.ActItem.FKRefno = this.NewItemFKRefno;

                        // TODO: Fix sorting
                        //string oldSort = this.ActItem.DataViewItems.Sort;
                        //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        CountItem countItem = this.ActItem.ListItems.OfType<CountItem>().Where(item => item.ActivityItemRefno == this.ActItem.Refno).FirstOrDefault();

                        if (countItem != null)
                        {
                            countItem.ActivityItemCountFKRefno = this.NewItemFKRefno;
                            countItem.ActivityItemCountStringFKRefno = this.NewItemFKRefno.ToString();
                        }

                        // TODO: Fix sorting
                        //this.ActItem.DataViewItems.Sort = oldSort;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override async Task DeleteSkid(SysSkidInfo skidInfo)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete = await this.CreateDeleteXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                string reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
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
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override async Task Insert(XDocument data)
        {
            await this.FillActDetailObjFromXML(data);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                int actDetailRefno = await this.InsertDetailDisconnected();
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailCountFKRefno) VALUES (?, ?)", Convert.ToInt32(actDetailRefno), Convert.ToInt32(this.FKRefno));
                this.IsUpdateView = true;
            }
            else
            {
                if (this.InsertDetail(await this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"))))
                {
                    bool newDetail = true;

                    // Check for a FK already existing with that value.
                    // if it does, add or subtract from it and then return false for new detail

                    CountItem countItem = this.ActItem.ListItems.OfType<CountItem>().Where(countItem => countItem.ActivityItemCountFKRefno == this.ActItem.FKRefno).FirstOrDefault();

                    if (countItem != null)
                    {
                        // There already exists a FK.
                        // Delete the newline and add the details to the existing line
                        newDetail = false;
                        countItem.ActivityItemCountQuantity += Quantity;
                        countItem.ActivityItemCountBaseQuantity += BaseQuantity;
                        countItem.ActivityItemCountWeightQuantity += WeightQuantity;
                    }

                    if (newDetail)
                    {
                        //<WO ID="14711" comment="Changed Refno to FKRefno. They are the same when no barcode modifier present. Opposite +/- when modifier exists." />
                        await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("UPDATE tActivityItemCount SET ActivityItemCountStringFKRefno = ?, ActivityItemCountFKRefno = ? WHERE ActivityItemRefno = ?", Convert.ToString(this.NewItemFKRefno), this.NewItemFKRefno, this.ActItem.Refno);

                        // TODO: Fix sorting
                        //string oldSort = this.ActItem.DataViewItems.Sort;
                        //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        CountItem countItemNew = this.ActItem.ListItems.OfType<CountItem>().Where(countItem => countItem.ActivityItemRefno == this.ActItem.Refno).FirstOrDefault();

                        if (countItemNew != null)
                        {
                            countItemNew.ActivityItemCountFKRefno = this.NewItemFKRefno;
                            countItemNew.ActivityItemCountStringFKRefno = this.NewItemFKRefno.ToString();
                        }

                        // TODO: Fix sorting
                        //this.ActItem.DataViewItems.Sort = oldSort;
                    }
                }
            }

            this.InsertUpdateDataViewDetail("ActivityDetailCountFKRefno", "ActivityDetailCountFKRefno");
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete = await this.CreateDeleteXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
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

            // delete row from DataView
            // TODO: Fix sorting
            //string oldSort = this.DataViewDetails.Sort;
            //this.DataViewDetails.Sort = "ActivityDetailCountFKRefno";

            CountDetail detail = this.ListDetails.OfType<CountDetail>().Where(countDetail => countDetail.ActivityDetailCountFKRefno == this.FKRefno).FirstOrDefault();

            if (detail != null)
            {
                this.ListDetails.Remove(detail);
            }

            // TODO: Fix sorting
            //this.DataViewDetails.Sort = oldSort;

            // Delete from database
            string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.Refno);

            query = "DELETE FROM tActivityDetailCount WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.Refno);

            query = "DELETE FROM tActivityDetailExtra WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.Refno);

            query = "DELETE FROM tActivityDetail WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.Refno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="data"></param>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = await this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                xmlDoc.LoadXml(reqData);

                // Check if result is an error message
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
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

                    if (selectedActivityDetail is CountDetail countDetail)
                    {
                        countDetail.ActivityDetailCountFKRefno = this.FKRefno;
                        countDetail.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?", Convert.ToInt32(SysCommon.ActStatus.NoChange), this.Refno);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public override async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            await base.GetInfo(selectedActivityDetail, isAdd);

            if (!isAdd && selectedActivityDetail is CountDetail countDetail)
            {
                this.FKRefno = Convert.ToInt32(countDetail.ActivityDetailCountFKRefno);
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(countDetail.LocationMapRefno));
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task Refresh()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityCycleCountDetail tableCountDet = new TableActivityCycleCountDetail();
                await tableCountDet.Purge();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableCountDet.RegisterDataAccessListenerProgressOut((CycleCountActDetail)this);
                await tableCountDet.Request(this.ActItem.Refno);
                tableCountDet.UnregisterDataAccessListenerProgressOut((CycleCountActDetail)this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CountDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CountDetail).ActivityDetailCountFKRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailCountFKRefno AS ActivityDetailCountFKRefno,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
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
                sql.Append("\'Slot\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailCountFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
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
                sql.Append("\'Section\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailCountFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
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
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("\'Whs\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }
            else
            {
                // This large query failed on large datasets like Prairie most of the time. Out of memory exception.
                // The queries are identical other than Slot, Section, Warehouse location labels
                // Run 3 seperate queries and then join them afterwards.
                List<CountDetail> slotList = await GetSlotDetails(ActItem.Refno);
                List<CountDetail> sectionList = await GetSectionDetails(ActItem.Refno);
                List<CountDetail> warehouseList = await GetWarehouseDetails(ActItem.Refno);

                //slotTable.Merge(sectionTable, true, MissingSchemaAction.Ignore);
                //slotTable.Merge(warehouseTable, true, MissingSchemaAction.Ignore);
                slotList.AddRange(sectionList);
                slotList.AddRange(warehouseList);

                //DataView view = slotTable.DefaultView;
                //view.Sort = "ActivityDetailRefno ASC";
                //DataViewDetails = view;
                //DataViewDetails.Table.TableName = MainGridMappingName;

                this.listCountDetails = slotList.Cast<CountDetail>().ToList();
                this.ListDetails = this.listCountDetails.Cast<object>().ToList();
                return this.ListDetails;
            }

            this.listCountDetails = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<CountDetail>(sql.ToString());
            this.ListDetails = this.listCountDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <summary>
        /// Gets all slot details for the refno.
        /// </summary>
        /// <returns></returns>
        private async Task<List<CountDetail>> GetSlotDetails(int refno)
        {
            string slotQuery = "SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno, " +
            "tActivityDetail.LocationMapRefno AS LocationMapRefno, " +
            "tActivityDetailCount.ActivityDetailCountFKRefno AS ActivityDetailCountFKRefno, " +
            "tUnit.UnitCode AS UnitCode, " +
            "aUnitWeight.UnitCode AS WtUnitCode, " +
            "aUnitBase.UnitCode AS BaseUnitCode, " +
            "tProduct.ProductCode AS ProductCode, " +
            "tProduct.ProductRefno AS ProductRefno, " +
            "tProduct.ProductDescription AS ProductDescription, " +
            "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
            "tProductUnit.ProductUnitIsBC AS ProductUnitIsBC, " +
            "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
            "tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity, " +
            "tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity, " +
            "tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity, " +
            "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate, " +
            "tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID, " +
            "tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID, " +
            "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber, " +
            "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber, " +
            "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate, " +
            "tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate, " +
            "tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber, " +
            "tLocationMapSlot.LocationMapSlotCode AS LocationCode, " +
            "CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName, " +
            "\'Slot\' AS Location, " +
            "tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode, " +
            "tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo, " +
            "tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription, " +
            "tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus, " +
            "tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text, " +
            "tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text, " +
            "tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text, " +
            "tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text, " +
            "tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date  " +
            "FROM tActivityDetailCount  " +
            "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
            "= tActivityDetailCount.ActivityDetailRefno  " +
            "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
            "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
            "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
            "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
            "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
            "INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno  " +
            "= tActivityDetail.LocationMapRefno  " +
            "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
            "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
            "= tActivityDetailCount.ActivityDetailRefno  " +
            "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
            "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
            "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
            "= tActivityDetail.ActivityDetailRefno  " +
           "WHERE tActivityDetail.ActivityItemRefno = " + refno +
           " AND tActivityDetail.ActivityDetailSubType = " + Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts);

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<CountDetail>(slotQuery);
        }

        private async Task<List<CountDetail>> GetSectionDetails(int refno)
        {
            string sectionQuery = "SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno, " +
             "tActivityDetail.LocationMapRefno AS LocationMapRefno, " +
             "tActivityDetailCount.ActivityDetailCountFKRefno AS ActivityDetailCountFKRefno, " +
             "tUnit.UnitCode AS UnitCode, " +
             "aUnitWeight.UnitCode AS WtUnitCode, " +
             "aUnitBase.UnitCode AS BaseUnitCode, " +
             "tProduct.ProductCode AS ProductCode, " +
             "tProduct.ProductRefno AS ProductRefno, " +
             "tProduct.ProductDescription AS ProductDescription, " +
             "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
             "tProductUnit.ProductUnitIsBC AS ProductUnitIsBC, " +
             "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
             "tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity, " +
             "tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity, " +
             "tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity, " +
             "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate, " +
             "tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID, " +
             "tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID, " +
             "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber, " +
             "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber, " +
             "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate, " +
             "tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate, " +
             "tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber, " +
             "tLocationMapSection.LocationMapSectionCode AS LocationCode, " +
             "CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName, " +
             "\'Section\' AS Location, " +
             "tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode, " +
             "tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo, " +
             "tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription, " +
             "tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus, " +
             "tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text, " +
             "tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text, " +
             "tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text, " +
             "tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text, " +
             "tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date  " +
             "FROM tActivityDetailCount  " +
             "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
             "= tActivityDetailCount.ActivityDetailRefno  " +
             "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
             "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
             "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
             "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
             "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
             "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno  " +
             "= tActivityDetail.LocationMapRefno  " +
             "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
             "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
             "= tActivityDetailCount.ActivityDetailRefno  " +
             "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
             "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
             "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
             "= tActivityDetail.ActivityDetailRefno  " +
            "WHERE tActivityDetail.ActivityItemRefno = " + refno +
            " AND tActivityDetail.ActivityDetailSubType = " + Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts);

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<CountDetail>(sectionQuery);
        }

        private async Task<List<CountDetail>> GetWarehouseDetails(int refno)
        {
            string warehouseQuery =
                "SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno, " +
                 "tActivityDetail.LocationMapRefno AS LocationMapRefno, " +
                 "tActivityDetailCount.ActivityDetailCountFKRefno AS ActivityDetailCountFKRefno, " +
                 "tUnit.UnitCode AS UnitCode, " +
                 "aUnitWeight.UnitCode AS WtUnitCode, " +
                 "aUnitBase.UnitCode AS BaseUnitCode, " +
                 "tProduct.ProductCode AS ProductCode, " +
                 "tProduct.ProductRefno AS ProductRefno, " +
                 "tProduct.ProductDescription AS ProductDescription, " +
                 "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                 "tProductUnit.ProductUnitIsBC AS ProductUnitIsBC, " +
                 "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                 "tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity, " +
                 "tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity, " +
                 "tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity, " +
                 "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                 "tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID, " +
                 "tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID, " +
                 "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber, " +
                 "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber, " +
                 "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate, " +
                 "tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate, " +
                 "tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber, " +
                 "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                 "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                 "\'Whs\' AS Location, " +
                 "tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode, " +
                 "tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo, " +
                 "tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription, " +
                 "tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus, " +
                 "tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date  " +
                 "FROM tActivityDetailCount  " +
                 "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
                 "= tActivityDetailCount.ActivityDetailRefno  " +
                 "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
                 "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
                 "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
                 "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
                 "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
                 "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno  " +
                 "= tActivityDetail.LocationMapRefno  " +
                 "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
                 "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
                 "= tActivityDetailCount.ActivityDetailRefno  " +
                 "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
                 "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
                 "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
                 "= tActivityDetail.ActivityDetailRefno  " +
                "WHERE tActivityDetail.ActivityItemRefno = " + refno +
                " AND tActivityDetail.ActivityDetailSubType = " + Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts);

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<CountDetail>(warehouseQuery);
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (CountDetail countDetail in this.ListDetails)
            {
                //CountDetail countDetail = new CountDetail
                //{
                //    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
                //    ActivityDetailLogisticUnitExpiryUseByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitExpiryUseByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitExpiryUseByDate"] : null),
                //    ActivityDetailStatus = Convert.ToInt32(row["ActivityDetailStatus"] != DBNull.Value ? row["ActivityDetailStatus"] : null),
                //    ActivityDetailCountFKRefno = Convert.ToInt32(row["ActivityDetailCountFKRefno"] != DBNull.Value ? row["ActivityDetailCountFKRefno"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    Location = Convert.ToString(row["Location"] != DBNull.Value ? row["Location"] : null),
                //    ProductSchemeVariantDescription = Convert.ToString(row["ProductSchemeVariantDescription"] != DBNull.Value ? row["ProductSchemeVariantDescription"] : null),
                //    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                //    WtUnitCode = Convert.ToString(row["WtUnitCode"] != DBNull.Value ? row["WtUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                //    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                //    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                //    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                //    ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                //    ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"] != DBNull.Value ? row["ActivityDetailUniqueSchemeID"] : null),
                //    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                //    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                //    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                //    ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitSkidNumber"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                //    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                //    ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"] != DBNull.Value ? row["ActivityDetailExtra1Text"] : null),
                //    ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"] != DBNull.Value ? row["ActivityDetailExtra2Text"] : null),
                //    ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"] != DBNull.Value ? row["ActivityDetailExtra3Text"] : null),
                //    ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"] != DBNull.Value ? row["ActivityDetailExtra4Text"] : null)
                //};

                //if (DateTime.TryParse(Convert.ToString(row["ActivityDetailExtra5Date"]), out DateTime _))
                //{
                //    countDetail.ActivityDetailExtra5Date = Convert.ToDateTime(row["ActivityDetailExtra5Date"] != DBNull.Value ? row["ActivityDetailExtra5Date"] : null);
                //}
                //else
                //{
                //    countDetail.ActivityDetailExtra5Date = new DateTime(1900, 1, 1);
                //}

                observableCollection.Add(countDetail);
            }

            // TODO: Resource strings
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelSchemeIDHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                }
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

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (CountDetail countDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", countDetail.ActivityDetailQuantity));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", countDetail.ActivityDetailWeightQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(countDetail.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(countDetail.ActivityDetailSchemeID);

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
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            this.ListDetails = this.listCountDetails.Where(countDetail => countDetail.ActivityDetailCountFKRefno.ToString().Contains(searchText.Trim())).Cast<object>().ToList();

            foreach (CountDetail countDetail in this.ListDetails)
            {
                //CountDetail countDetail = new CountDetail
                //{
                //    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
                //    ActivityDetailLogisticUnitExpiryUseByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitExpiryUseByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitExpiryUseByDate"] : null),
                //    ActivityDetailStatus = Convert.ToInt32(row["ActivityDetailStatus"] != DBNull.Value ? row["ActivityDetailStatus"] : null),
                //    ActivityDetailCountFKRefno = Convert.ToInt32(row["ActivityDetailCountFKRefno"] != DBNull.Value ? row["ActivityDetailCountFKRefno"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    Location = Convert.ToString(row["Location"] != DBNull.Value ? row["Location"] : null),
                //    ProductSchemeVariantDescription = Convert.ToString(row["ProductSchemeVariantDescription"] != DBNull.Value ? row["ProductSchemeVariantDescription"] : null),
                //    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                //    WtUnitCode = Convert.ToString(row["WtUnitCode"] != DBNull.Value ? row["WtUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                //    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                //    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                //    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                //    ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                //    ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"] != DBNull.Value ? row["ActivityDetailUniqueSchemeID"] : null),
                //    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                //    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                //    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                //    ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitSkidNumber"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ProductSchemeVariantCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                //    ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"] != DBNull.Value ? row["ActivityDetailExtra1Text"] : null),
                //    ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"] != DBNull.Value ? row["ActivityDetailExtra2Text"] : null),
                //    ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"] != DBNull.Value ? row["ActivityDetailExtra3Text"] : null),
                //    ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"] != DBNull.Value ? row["ActivityDetailExtra4Text"] : null)
                //};

                //if (DateTime.TryParse(Convert.ToString(row["ActivityDetailExtra5Date"]), out _))
                //{
                //    this.ActExtra.DateTime5 = Convert.ToDateTime(row["ActivityDetailExtra5Date"]);
                //}
                //else
                //{
                //    this.ActExtra.DateTime5 = Convert.ToDateTime(new DateTime(1900, 1, 1));
                //}

                observableCollection.Add(countDetail);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivityDetail(object selectedActivityDetail)
        {
            if (selectedActivityDetail is CountDetail countDetail)
            {
                this.SelectedLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(countDetail.LocationMapRefno));
                this.Refno = Convert.ToInt32(countDetail.ActivityDetailRefno);
                this.FKRefno = Convert.ToInt32(countDetail.ActivityDetailCountFKRefno);
                this.SchemeID = Convert.ToString(countDetail.ActivityDetailSchemeID);
                this.UniqueSchemeID = Convert.ToString(countDetail.ActivityDetailUniqueSchemeID);

                this.ActExtra.Text1 = Convert.ToString(countDetail.ActivityDetailExtra1Text);
                this.ActExtra.Text2 = Convert.ToString(countDetail.ActivityDetailExtra2Text);
                this.ActExtra.Text3 = Convert.ToString(countDetail.ActivityDetailExtra3Text);
                this.ActExtra.Text4 = Convert.ToString(countDetail.ActivityDetailExtra4Text);

                if (DateTime.TryParse(Convert.ToString(countDetail.ActivityDetailExtra5Date), out _))
                {
                    this.ActExtra.DateTime5 = Convert.ToDateTime(countDetail.ActivityDetailExtra5Date);
                }
                else
                {
                    this.ActExtra.DateTime5 = Convert.ToDateTime(new DateTime(1900, 1, 1));
                }
            }
            else
            {
                this.ClearProperties();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = null;
            if (data.Document.Root.Element("skid_data") == null)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT CYCLE COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + ("<skid_fk_refno>" + 0 + "</skid_fk_refno>")
                        + "</imclient>");
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT CYCLE COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                        + "</imclient>");
                }
            }
            else
            {
                // TODO: extra? elements may not be needed.
                // Note whs_id, sect_refno and slot_refno are sent because counting will use the selected location not the
                // skid location. This is unique to counting and not picking / transfers etc...
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT CYCLE COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>")
                    + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                    + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
                    + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                    + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                    + "<is_negate_qty>false</is_negate_qty>"
                    + ("<extra1>" + this.ActExtra.Text1 + "</extra1>")
                    + ("<extra2>" + this.ActExtra.Text2 + "</extra2>")
                    + ("<extra3>" + this.ActExtra.Text3 + "</extra3>")
                    + ("<extra4>" + this.ActExtra.Text4 + "</extra4>")
                    + ("<extra5>" + this.ActExtra.DateTime5 + "</extra5>")
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<string> CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = null;
            if (data.Document.Root.Element("skid_data") == null)
            {
                // TODO: skid_fk_refno element may not be needed.
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CYCLE COUNT DETAIL</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + this.GetRequestDetailDefaults()
                    + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                    + "</imclient>");
            }
            else
            {
                // TODO: is_negate_qty element may not be needed.
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CYCLE COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>")
                    + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                    + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
                    + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                    + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        #endregion
    }
}