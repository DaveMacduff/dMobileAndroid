namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

    public class SkidInfoActDetail : ActDetail
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<SkidInfoDetail> listSkidInfoDetails = new List<SkidInfoDetail>();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItem"></param>
        public SkidInfoActDetail(ActItem actItem)
            : base(actItem)
        {
            this.FullClassName = typeof(SkidInfoActDetail).FullName;

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
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                    }
                };

                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override async Task DeleteSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override async Task Insert(XDocument data)
        {
            // string strLocationTypestring.Empty;
            string locCode = string.Empty;
            string locName = string.Empty;

            if (LocMap.CurrLocMap.SlotLocMapRefno > 0)
            {
                // strLocationType = "Slot";
                locCode = this.LocMap.CurrLocMap.SlotCode.ToString();
                locName = this.LocMap.CurrLocMap.SlotName;
            }
            else if (LocMap.CurrLocMap.SectLocMapRefno > 0)
            {
                // strLocationType = "Section";
                locCode = this.LocMap.CurrLocMap.SectCode.ToString();
                locName = this.LocMap.CurrLocMap.SectName;
            }
            else
            {
                // strLocationType = "Whs";
                locCode = this.LocMap.CurrLocMap.WhsCode.ToString();
                locName = this.LocMap.CurrLocMap.WhsName;
            }

            await this.FillActDetailObjFromXML(data);

            string xmlSend = await this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
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
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.Refno = this.FKRefno;
                }

                this.FillDataView();
            }
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
                    else
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);
                        SSkidInfo tempSkidInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                        tempSkidInfo.IsConsumed = bool.Parse(xmlDoc.DocumentElement["IsConsumed"].InnerText);
                        this.ActItem.ActHdr.SkidInfo.CurrSkid = tempSkidInfo;
                    }
                }
            }

            // delete row from DataView
            // TODO: Fix sorting
            //string oldSort = this.DataViewDetails.Sort;
            //this.DataViewDetails.Sort = "ActivityDetailFKRefno";
            //int rowIdx = this.DataViewDetails.Find(this.FKRefno);

            SkidInfoDetail skidInfoDetail = this.ListDetails.OfType<SkidInfoDetail>().Where(skidInfoDetail => skidInfoDetail.ActivityDetailFKRefno == this.FKRefno).FirstOrDefault();

            if (skidInfoDetail != null)
            {
                this.ListDetails.Remove(skidInfoDetail);
            }

            // TODO: Fix sorting
            //this.DataViewDetails.Sort = oldSort;
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

            if (!isAdd && selectedActivityDetail is SkidInfoDetail skidInfoDetail)
            {
                this.FKRefno = Convert.ToInt32(skidInfoDetail.ActivityDetailFKRefno);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedRowIndex"></param>
        /// <param name="data"></param>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = await this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
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
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.Refno = this.FKRefno;

                    if (selectedActivityDetail is SkidInfoDetail skidInfoDetail)
                    {
                        skidInfoDetail.ActivityDetailFKRefno = this.FKRefno;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task Refresh()
        {
            if (this.ListDetails != null)
            {
                this.ListDetails.Clear();
            }

            DataSet ds = new DataSet();
            SysSkidInfo.GetSkidDetail(this.ActItem.ActHdr.SkidInfo, ref ds);

            if (ds.Tables.Count != 0)
            {
                foreach (DataRow drow in ds.Tables["cskiddetail"].Rows)
                {
                    // NOTE: The drow data is all strings because it was filled from xml data in SysSkidInfo.GetSkidDetail.
                    // Need to do a data type conversion not just a cast.
                    this.ActItem.ActHdr.FKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                    this.ActItem.ActHdr.Refno = Convert.ToInt32(drow["ActivityFKRefno"]);
                    this.ActItem.FKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                    this.ActItem.Refno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                    this.FKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                    this.Refno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                    this.SchemeID = Convert.ToString(drow["ActivityDetailSchemeID"]);
                    this.UniqueSchemeID = Convert.ToString(drow["ActivityDetailUniqueSchemeID"]);
                    this.Quantity = Convert.ToDecimal(drow["ActivityDetailQuantity"]);
                    this.BaseQuantity = Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]);
                    this.WeightQuantity = Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]);

                    SProductSchemeData tempSchemeData = this.SchemeData;
                    tempSchemeData.LogisticUnitBatchLotNumber = Convert.ToString(drow["ActivityDetailLogisticUnitBatchLotNumber"]);
                    tempSchemeData.SingleTradeItemSerialNumber = Convert.ToString(drow["ActivityDetailSingleTradeItemSerialNumber"]);
                    tempSchemeData.LogisticUnitSkidNumber = ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno.ToString();
                    tempSchemeData.VariantNumber = Convert.ToInt32(drow["ProductSchemeVariantNo"]);

                    List<tProductSchemeVariantModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductSchemeVariantModel>("SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", tempSchemeData.VariantNumber);

                    if (results?.Count > 0)
                    {
                        tempSchemeData.VariantCode = results[0].ProductSchemeVariantCode;
                        tempSchemeData.VariantDescription = results[0].ProductSchemeVariantDescription;
                    }

                    string newDate = "01-01-1900";

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitBestBeforeSellByDate = Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]);
                    }

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitExpiryUseByDate = Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]);
                    }

                    if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString()))
                    {
                        tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(newDate);
                    }
                    else
                    {
                        tempSchemeData.LogisticUnitProductionDate = Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]);
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
                        tempSchemeData.LogisticUnitPackagingDate = Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]);
                    }

                    this.SchemeData = tempSchemeData;
                    this.FillDataView();
                }
            }

            this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as SkidInfoDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as SkidInfoDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            DataTable table = new DataTable("ActivitySkidDetail");

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

            this.listSkidInfoDetails = table.Rows.Cast<SkidInfoDetail>().ToList();
            this.ListDetails = this.listSkidInfoDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (SkidInfoDetail skidInfoDetail in this.ListDetails)
            {
                //SkidInfoDetail skidInfoDetail = new SkidInfoDetail
                //{
                //    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
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
                //    ActivityDetailFKRefno = Convert.ToInt32(row["ActivityDetailFKRefno"] != DBNull.Value ? row["ActivityDetailFKRefno"] : null),
                //    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                //    WeightUnitCode = Convert.ToString(row["WeightUnitCode"] != DBNull.Value ? row["WeightUnitCode"] : null)
                //};

                observableCollection.Add(skidInfoDetail);
            }

            // TODO: Resource strings
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelSchemeIDHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

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

            foreach (SkidInfoDetail skidInfoDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", skidInfoDetail.ActivityDetailQuantity));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", skidInfoDetail.ActivityDetailWeightQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(skidInfoDetail.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(skidInfoDetail.ActivityDetailSchemeID);

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivityDetail(object selectedActivityDetail)
        {
            if (selectedActivityDetail is SkidInfoDetail skidInfoDetail)
            {
                this.FKRefno = Convert.ToInt32(skidInfoDetail.ActivityDetailFKRefno);
                this.Refno = Convert.ToInt32(skidInfoDetail.ActivityDetailRefno);
                this.SchemeID = Convert.ToString(skidInfoDetail.ActivityDetailSchemeID);
                this.UniqueSchemeID = Convert.ToString(skidInfoDetail.ActivityDetailUniqueSchemeID);
                this.Quantity = Convert.ToDecimal(skidInfoDetail.ActivityDetailQuantity);
                this.BaseQuantity = Convert.ToDecimal(skidInfoDetail.ActivityDetailBaseQuantity);
                this.WeightQuantity = Convert.ToDecimal(skidInfoDetail.ActivityDetailWeightQuantity);
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
        public override async Task<string> CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT REMOVE PRODUCT FROM SKID</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + this.GetRequestDetailDefaults()
                + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                + "</imclient>");

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
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT ADD PRODUCT TO SKID</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + this.GetRequestDetailDefaults()
                + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public void FillDataView()
        {
            if (this.ListDetails != null)
            {
                this.ListDetails.Add(new SkidInfoDetail()
                {
                    ProductRefno = this.ActItem.ProdInfo.Refno,
                    ProductCode = this.ActItem.ProdInfo.Code,
                    ProductUnitBC = this.ActItem.ProdInfo.UnitBC,
                    ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBC,
                    ProductDescription = this.ActItem.ProdInfo.Desc,
                    ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc,
                    UnitCode = this.ActItem.ProdInfo.UnitCode,
                    BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode,
                    WeightUnitCode = this.ActItem.ProdInfo.WeightUnitCode,
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
                    ActivityDetailFKRefno = this.FKRefno,
                    ActivityDetailRefno = this.Refno,
                });
            }
        }

        #endregion
    }
}
