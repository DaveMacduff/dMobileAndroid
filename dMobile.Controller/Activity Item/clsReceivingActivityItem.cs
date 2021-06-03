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
    using dMobile.Resources.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class RecvActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        private List<ReceiveItem> listReceiveItems = new List<ReceiveItem>();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public RecvActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(RecvActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Receiving;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsReceivingCreateNewItemNoAsk;

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
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                    }
                };

                Label labelIsActivityItemReceivingCommentItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelProductCodeItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelProductUnitBCItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelUnitCodeItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemReceivingQuantityOrderedItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemReceivingQuantityReceivedItem = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelIsActivityItemReceivingCommentItem.SetBinding(Label.TextProperty, new Binding("IsActivityItemReceivingComment"));
                labelProductCodeItem.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelProductUnitBCItem.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCodeItem.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelActivityItemReceivingQuantityOrderedItem.SetBinding(Label.TextProperty, new Binding("ActivityItemReceivingQuantityOrdered", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemReceivingQuantityReceivedItem.SetBinding(Label.TextProperty, new Binding("ActivityItemReceivingQuantityReceived", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelIsActivityItemReceivingCommentItem);
                gridData.Children.Add(labelProductCodeItem);
                gridData.Children.Add(labelProductUnitBCItem);
                gridData.Children.Add(labelUnitCodeItem);
                gridData.Children.Add(labelActivityItemReceivingQuantityOrderedItem);
                gridData.Children.Add(labelActivityItemReceivingQuantityReceivedItem);

                Grid.SetColumn(labelIsActivityItemReceivingCommentItem, 0);
                Grid.SetColumn(labelProductCodeItem, 1);
                Grid.SetColumn(labelProductUnitBCItem, 2);
                Grid.SetColumn(labelUnitCodeItem, 3);
                Grid.SetColumn(labelActivityItemReceivingQuantityOrderedItem, 4);
                Grid.SetColumn(labelActivityItemReceivingQuantityReceivedItem, 5);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool IsCatchAsk()
        {
            bool isCatchAsk = base.IsCatchAsk();

            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsReceivingCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsCatchAskZero()
        {
            bool isCatchAskZero = base.IsCatchAskZero();

            if (isCatchAskZero)
            {
                isCatchAskZero = !this.SysEnv.SysSettings.IsReceivingNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string sendReq = this.CreateDeleteXML(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_data>" + this.ActHdr.SkidInfo.CurrSkid.SkidCode + "</skid_data>"
                    + "</params>"));

                string reqData = SysCommon.SubmitComm(sendReq);

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
        /// <returns></returns>
        public override string CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request = null;

            if (data.Document.Root.Element("skid_data") == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE PURCHASE ORDER DETAIL SKID</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<refno>" + this.ActHdr.FKRefno + "</refno>"
                    + "<itm_refno>" + this.FKRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }
            else
            {
                // TODO: Resource strings
                throw new NotImplementedException("Only skid deletion is supported.");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iSelectedIndex"></param>
        /// <returns></returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            string cmmt = await base.GetComment(selectedActivityItem);

            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                List<tActivityItemReceivingCommentModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemReceivingCommentModel>("SELECT * FROM tActivityItemReceivingComment WHERE ActivityItemRefno = ?", receiveItem.ActivityItemRefno);

                if (results != null && results.Count > 0)
                {
                    cmmt = results[0].ActivityItemReceivingCommentComment;
                }
            }

            return cmmt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <param name="prodRefno"></param>
        /// <returns></returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            return await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(whsLocMapRefno, prodRefno, SysCommon.ProdLocMapTypes.Receiving); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse purchase order item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find purchase order item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetDataViewSortFKRefnoIdx()
        {
            // TODO: Resource strings
            return "ActivityItemReceivingStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityRecevingItem tableActRecvItem = new TableActivityRecevingItem();
                TableActivityReceivingDetail tableActRecvDetail = new TableActivityReceivingDetail();
                await tableActRecvDetail.Purge();
                await tableActRecvItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActRecvItem.RegisterDataAccessListenerProgressOut((RecvActItem)this);
                await tableActRecvItem.Request(this.ActHdr.Refno);
                tableActRecvItem.UnregisterDataAccessListenerProgressOut((RecvActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (ReceiveItem receiveItem in this.ListItems)
            {
                //ReceiveItem receiveItem = new ReceiveItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemReceivingFKRefno = Convert.ToInt32(row["ActivityItemReceivingFKRefno"]!= DBNull.Value ? row["ActivityItemReceivingFKRefno"] : null),
                //    ActivityItemReceivingStringFKRefno = Convert.ToString(row["ActivityItemReceivingStringFKRefno"]!= DBNull.Value ? row["ActivityItemReceivingStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"]!= DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"]!= DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"]!= DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"]!= DBNull.Value ? row["ProductDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"]!= DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"]!= DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"]!= DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemReceivingQuantityOrdered = Convert.ToDecimal(row["ActivityItemReceivingQuantityOrdered"]!= DBNull.Value ? row["ActivityItemReceivingQuantityOrdered"] : null),
                //    ActivityItemReceivingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemReceivingBaseQuantityOrdered"]!= DBNull.Value ? row["ActivityItemReceivingBaseQuantityOrdered"] : null),
                //    ActivityItemReceivingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemReceivingWeightQuantityOrdered"]!= DBNull.Value ? row["ActivityItemReceivingWeightQuantityOrdered"] : null),
                //    ActivityItemReceivingQuantityReceived = Convert.ToDecimal(row["ActivityItemReceivingQuantityReceived"]!= DBNull.Value ? row["ActivityItemReceivingQuantityReceived"] : null),
                //    ActivityItemReceivingBaseQuantityReceived = Convert.ToDecimal(row["ActivityItemReceivingBaseQuantityReceived"]!= DBNull.Value ? row["ActivityItemReceivingBaseQuantityReceived"] : null),
                //    ActivityItemReceivingWeightQuantityReceived = Convert.ToDecimal(row["ActivityItemReceivingWeightQuantityReceived"]!= DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemReceivingVendorProductUnitCode = Convert.ToString(row["ActivityItemReceivingVendorProductUnitCode"]!= DBNull.Value ? row["ActivityItemReceivingVendorProductUnitCode"] : null),
                //    ActivityItemReceivingDescription = Convert.ToString(row["ActivityItemReceivingDescription"]!= DBNull.Value ? row["ActivityItemReceivingDescription"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]!= DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"]!= DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"]!= DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemReceivingQuantityReceivedToDate = Convert.ToDecimal(row["ActivityItemReceivingQuantityReceivedToDate"]!= DBNull.Value ? row["ActivityItemReceivingQuantityReceivedToDate"] : null),
                //    ActivityItemReceivingWeightQuantityReceivedToDate = Convert.ToDecimal(row["ActivityItemReceivingWeightQuantityReceivedToDate"]!= DBNull.Value ? row["ActivityItemReceivingWeightQuantityReceivedToDate"] : null),
                //    IsActivityItemReceivingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemReceivingComment"])) ? "X" : ""
                //};

                observableCollection.Add(receiveItem);
            }

            // TODO: Resource strings
            Label labelIsActivityItemReceivingCommentReceiving = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelProductCodeReceiving = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelProductUnitBCReceiving = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeReceiving = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemReceivingQuantityOrderedReceiving = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemReceivingQuantityReceivedReceiving = new Label { Text = "Received", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsActivityItemReceivingCommentReceiving.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCodeReceiving.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductUnitBCReceiving.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeReceiving.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemReceivingQuantityOrderedReceiving.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemReceivingQuantityReceivedReceiving.Text);

            // Header grid
            Grid gridHeaderReceiving = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                }
            };

            gridHeaderReceiving.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeaderReceiving.Children.Add(labelIsActivityItemReceivingCommentReceiving);
            gridHeaderReceiving.Children.Add(labelProductCodeReceiving);
            gridHeaderReceiving.Children.Add(labelProductUnitBCReceiving);
            gridHeaderReceiving.Children.Add(labelUnitCodeReceiving);
            gridHeaderReceiving.Children.Add(labelActivityItemReceivingQuantityOrderedReceiving);
            gridHeaderReceiving.Children.Add(labelActivityItemReceivingQuantityReceivedReceiving);

            Grid.SetColumn(labelIsActivityItemReceivingCommentReceiving, 0);
            Grid.SetColumn(labelProductCodeReceiving, 1);
            Grid.SetColumn(labelProductUnitBCReceiving, 2);
            Grid.SetColumn(labelUnitCodeReceiving, 3);
            Grid.SetColumn(labelActivityItemReceivingQuantityOrderedReceiving, 4);
            Grid.SetColumn(labelActivityItemReceivingQuantityReceivedReceiving, 5);

            Grid.SetRow(gridHeaderReceiving, 0);

            dataGridMainActivityHeader.Children.Add(gridHeaderReceiving);

            foreach (ReceiveItem receiveItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.IsActivityItemReceivingComment);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.ProductCode);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.ProductUnitBC);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.UnitCode);
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", receiveItem.ActivityItemReceivingQuantityOrdered));
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", receiveItem.ActivityItemReceivingQuantityReceived));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeaderReceiving.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeaderReceiving.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeaderReceiving.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeaderReceiving.ColumnDefinitions[3].Width = columnWidth3;
                }

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeaderReceiving.ColumnDefinitions[4].Width = columnWidth4;
                }

                if (detailWidth5 > columnWidth5)
                {
                    columnWidth5 = detailWidth5;
                    gridHeaderReceiving.ColumnDefinitions[5].Width = columnWidth5;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno AS ActivityItemReceivingFKRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingStringFKRefno AS ActivityItemReceivingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityOrdered");
                sql.Append("- tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate ");
                sql.Append("AS ActivityItemReceivingQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityOrdered ");
                sql.Append("- tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceivedToDate ");
                sql.Append("AS ActivityItemReceivingBaseQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityOrdered ");
                sql.Append("- tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate ");
                sql.Append("AS ActivityItemReceivingWeightQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceived AS ActivityItemReceivingQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceived AS ActivityItemReceivingBaseQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceived AS ActivityItemReceivingWeightQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingVendorProductUnitCode AS ActivityItemReceivingVendorProductUnitCode,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingDescription AS ActivityItemReceivingDescription,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate AS ActivityItemReceivingQuantityReceivedToDate,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate AS ActivityItemReceivingWeightQuantityReceivedToDate,");
                sql.Append("CASE WHEN tActivityItemReceivingComment.ActivityItemReceivingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemReceivingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemReceiving ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemReceiving.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
                sql.Append("tActivityItemReceiving.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemReceivingComment ON tActivityItemReceivingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemReceiving.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityOrdered - ");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityOrdered - ");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingBaseQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityOrdered - ");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingWeightQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingVendorProductUnitCode,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingDescription,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate,");
                sql.Append("CASE WHEN tActivityItemReceivingComment.ActivityItemReceivingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemReceivingComment,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemReceiving ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemReceiving.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = ");
                sql.Append("tActivityItemReceiving.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
                sql.Append("tActivityItemReceiving.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemReceivingComment ON tActivityItemReceivingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemReceiving.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus = {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.Append("ORDER BY tActivityItemReceiving.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno AS ActivityItemReceivingFKRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingStringFKRefno AS ActivityItemReceivingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityOrdered - ");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityOrdered - ");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingBaseQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityOrdered - ");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate AS ");
                sql.Append("ActivityItemReceivingWeightQuantityOrdered,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceived AS ActivityItemReceivingQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingBaseQuantityReceived AS ActivityItemReceivingBaseQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceived AS ActivityItemReceivingWeightQuantityReceived,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingVendorProductUnitCode AS ActivityItemReceivingVendorProductUnitCode,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingDescription AS ActivityItemReceivingDescription,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingQuantityReceivedToDate AS ActivityItemReceivingQuantityReceivedToDate,");
                sql.Append("tActivityItemReceiving.ActivityItemReceivingWeightQuantityReceivedToDate AS ActivityItemReceivingWeightQuantityReceivedToDate,");
                sql.Append("CASE WHEN tActivityItemReceivingComment.ActivityItemReceivingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemReceivingComment ");
                sql.Append("FROM tActivityItemReceiving ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemReceiving.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = ");
                sql.Append("tActivityItemReceiving.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemReceivingComment ON tActivityItemReceivingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemReceiving.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("ORDER BY tActivityItemReceiving.ActivityItemRefno");
            }

            //using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);
            //        this.DataViewItems = ds.Tables[0].DefaultView;
            //        this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //        this.DataViewItems.AllowNew = true;
            //        this.DataViewItems.AllowEdit = true;
            //        this.DataViewItems.AllowDelete = true;
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //    }
            //}

            this.listReceiveItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReceiveItem>(sql.ToString());
            this.ListItems = this.listReceiveItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            this.ListItems = this.listReceiveItems.Where(receiveItem => receiveItem.ProductCode.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (ReceiveItem receiveItem in this.ListItems)
            {
                //ReceiveItem receiveItem = new ReceiveItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"]),
                //    ActivityItemReceivingFKRefno = Convert.ToInt32(row["ActivityItemReceivingFKRefno"]),
                //    ActivityItemReceivingStringFKRefno = Convert.ToString(row["ActivityItemReceivingStringFKRefno"]),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"]),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                //    ProductCode = Convert.ToString(row["ProductCode"]),
                //    ProductDescription = Convert.ToString(row["ProductDescription"]),
                //    UnitCode = Convert.ToString(row["UnitCode"]),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"]),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"]),
                //    ActivityItemReceivingQuantityOrdered = Convert.ToDecimal(row["ActivityItemReceivingQuantityOrdered"]),
                //    ActivityItemReceivingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemReceivingBaseQuantityOrdered"]),
                //    ActivityItemReceivingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemReceivingWeightQuantityOrdered"]),
                //    ActivityItemReceivingQuantityReceived = Convert.ToDecimal(row["ActivityItemReceivingQuantityReceived"]),
                //    ActivityItemReceivingBaseQuantityReceived = Convert.ToDecimal(row["ActivityItemReceivingBaseQuantityReceived"]),
                //    ActivityItemReceivingWeightQuantityReceived = Convert.ToDecimal(row["ActivityItemReceivingWeightQuantityReceived"]),
                //    ActivityItemReceivingVendorProductUnitCode = Convert.ToString(row["ActivityItemReceivingVendorProductUnitCode"]),
                //    ActivityItemReceivingDescription = Convert.ToString(row["ActivityItemReceivingDescription"]),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                //    LocationCode = Convert.ToInt32(row["LocationCode"]),
                //    LocationName = Convert.ToString(row["LocationName"]),
                //    ActivityItemReceivingQuantityReceivedToDate = Convert.ToDecimal(row["ActivityItemReceivingQuantityReceivedToDate"]),
                //    ActivityItemReceivingWeightQuantityReceivedToDate = Convert.ToDecimal(row["ActivityItemReceivingWeightQuantityReceivedToDate"]),
                //    IsActivityItemReceivingCommentString = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemReceivingComment"])) ? "X" : "",
                //};

                observableCollection.Add(receiveItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                await base.SelectActivityItem(receiveItem);
                this.FKRefno = Convert.ToInt32(receiveItem.ActivityItemReceivingFKRefno);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        public override async Task<int> GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemReceiving.ActivityItemReceivingFKRefno,");
            sql.Append("tActivityItemReceiving.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemReceiving ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
            sql.Append("tActivityItemReceiving.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
            sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
            sql.AppendFormat("AND tActivityItemReceiving.WarehouseLocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemReceiving.ProductUnitRefno = {0}", prodUnitRefno);

            List<ReceiveItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReceiveItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.Refno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.FKRefno = Convert.ToInt32(results[0].ActivityItemReceivingFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.Refno;
            }

            return foundActItemRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT RECEIVING ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>")
                + ("<symbology>" + symbologyPrefix + "</symbology>")
                + ("<gtin>" + prodUnitBarCodeCode + "</gtin>") + "<whs_id></whs_id>"
                + "<sect_refno></sect_refno>"
                + "<slot_refno></slot_refno>"
                + "<refno></refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                receiveItem.ActivityItemReceivingQuantityReceived = quantity;
                receiveItem.ActivityItemReceivingBaseQuantityReceived = baseQuantity;
                receiveItem.ActivityItemReceivingWeightQuantityReceived = weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                receiveItem.ActivityItemReceivingQuantityReceived += quantity;
                receiveItem.ActivityItemReceivingBaseQuantityReceived += baseQuantity;
                receiveItem.ActivityItemReceivingWeightQuantityReceived += weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="activityItemRefno"></param>
        public override async Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            if (selectedActivityItem is ReceiveItem receiveItem)
            {
                string query = "UPDATE tActivityItemReceiving SET ActivityItemReceivingQuantityReceived = ?, ActivityItemReceivingBaseQuantityReceived = ?, ActivityItemReceivingWeightQuantityReceived = ? WHERE ActivityItemRefno = ?";
                object[] parameters = new object[] { receiveItem.ActivityItemReceivingQuantityReceived, receiveItem.ActivityItemReceivingBaseQuantityReceived, receiveItem.ActivityItemReceivingWeightQuantityReceived, actItemRefno };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<string> GetTotalQuantity()
        {
            string msg = "Total received:";
            string query = "SELECT SUM(tActivityItemReceiving.ActivityItemReceivingQuantityOrdered) AS "
                + "Ordered,"
                + "SUM(tActivityItemReceiving.ActivityItemReceivingQuantityReceived) AS Received "
                + "FROM tActivityItem "
                + "INNER JOIN tActivityItemReceiving ON tActivityItemReceiving.ActivityItemRefno = "
                + "tActivityItem.ActivityItemRefno "
                + string.Format("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.Refno);

            List<TotalQuantity> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<TotalQuantity>(query);

            if (results != null && results.Count > 0)
            {
                foreach (TotalQuantity result in results)
                {
                    // Because an agregate sum function is used in a query. There will always be a result record.
                    // This causes problems detecting if there is a valid result. 
                    // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
                    msg += string.Format("\r{0} / {1}", Convert.ToString(result.Ordered), Convert.ToString(result.Received));
                }
            }

            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task IsReadyToApply(XDocument data)
        {
            await base.IsReadyToApply(data);

            // New items are identified with 0 < this.FKRefno, do not validate.
            if (this.SysEnv.SysSettings.RecvQtyOverWarn != SysCommon.ActReceivingQtyOverValLevels.NoValidationOrWarning && 0 < this.FKRefno)
            {
                try
                {
                    this.ReceivingModuleQuantityOverCheck(decimal.Parse(data.Root.Element("alt_qty").Value));
                }
                catch (Exception ex)
                {
                    switch (this.SysEnv.SysSettings.RecvQtyOverWarn)
                    {
                        case SysCommon.ActReceivingQtyOverValLevels.ValidateWarnAndContinue:
                            // TODO: Resource strings
                            if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Quantity Over", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                            {
                                throw new IsReadyToApplyException(ex.Message, true, true);
                            }
                            else
                            {
                                throw new IsReadyToApplyException(ex.Message, true, false);
                            }
                        case SysCommon.ActReceivingQtyOverValLevels.ValidateWarnAndStop:
                            throw new IsReadyToApplyException(ex.Message, false, false);
                    }
                }
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemPickFKRefno"></param>
        /// <param name="actItemPickQuantity"></param>
        private void ReceivingModuleQuantityOverCheck(decimal actItemRecvQuantity)
        {
            if (this.SelectedActivityItem is ReceiveItem receiveItem)
            {
                decimal received = Convert.ToDecimal(receiveItem.ActivityItemReceivingQuantityReceived);
                decimal ordered = Convert.ToDecimal(receiveItem.ActivityItemReceivingQuantityOrdered);

                if (received + actItemRecvQuantity > ordered)
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("The resulting quantity received {0} {1} will be greater than the quantity ordered {2} {3}.", received + actItemRecvQuantity, this.ProdInfo.UnitCode, ordered, this.ProdInfo.UnitCode));
                }
            }
        }

        #endregion
    }
}
