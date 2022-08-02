// <copyright file="clsReceivingActivityItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Models;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="RecvActItem" />.
    /// </summary>
    public class RecvActItem : ActItem
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
        /// Defines the columnWidth4.
        /// </summary>
        private double columnWidth4 = 0;

        /// <summary>
        /// Defines the columnWidth5.
        /// </summary>
        private double columnWidth5 = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listReceiveItems.
        /// </summary>
        private List<ReceiveItem> listReceiveItems = new List<ReceiveItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RecvActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
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
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                    },
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

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItem(int actItemFoundRefno)
        {
            if (actItemFoundRefno >= 0)
            {
                this.SelectedActivityItem = (ReceiveItem)(from item in this.ListItems.OfType<ReceiveItem>()
                                                             where item.ActivityItemRefno == actItemFoundRefno
                                                             select item);
            }
            else
            {
                this.SelectedActivityItem = null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allowed to add skid detail.
        /// </summary>
        /// <returns><see cref="bool"/> True if allowed to add skid detail.</returns>
        public override bool IsAllowAddSkidDetail()
        {
            return false;
        }

        /// <summary>
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAsk()
        {
            bool isCatchAsk = this.ProdInfo.IsCatchAsk;

            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsReceivingCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAskZero()
        {
            bool result = this.IsCatchAsk();
            if (result && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsReceivingNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string sendReq = await this.CreateDeleteXML(XDocument.Parse(
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

            return 0;
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE PURCHASE ORDER DETAIL SKID</action>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.WhsFKRefno + "</whs_id>"
                    + "<refno>" + this.ActHdr.HdrFKRefno + "</refno>"
                    + "<itm_refno>" + this.ItemFKRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }
            else
            {
                throw new NotImplementedException("Only skid deletion is supported.");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            string cmmt = string.Empty;

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
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            int locMapRefno = 0;
            if (this.SelectedActivityItem is ReceiveItem item)
            {
                if (Math.Sign(item.ActivityItemReceivingQuantityOrdered) == 0 ||
                    Math.Sign(item.ActivityItemReceivingQuantityOrdered) == 1)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Receiving);
                }
                else
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Picking);
                }
            }

            if (locMapRefno == 0)
            {
                locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    whsLocMapRefno,
                    prodRefno,
                    SysCommon.ProdLocMapTypes.Floor);
            }

            return locMapRefno;
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">The actItemStringFKRefno<see cref="string"/>.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to parse purchase order item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">The actItemStringFKRefno<see cref="string"/>.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to find purchase order item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemStringFKRefno";
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>
        /// <param name="askToInsertAsNewItem"><see cref="bool"/> Stop and ask to insert an item.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
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
                await tableActRecvDetail.PurgeData();
                await tableActRecvItem.PurgeData();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActRecvItem.RegisterDataAccessListenerProgressOut((RecvActItem)this);
                await tableActRecvItem.RequestData(this.ActHdr.HdrRefno);
                tableActRecvItem.UnregisterDataAccessListenerProgressOut((RecvActItem)this);
            }
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

            foreach (ReceiveItem receiveItem in this.ListItems)
            {
                // ReceiveItem receiveItem = new ReceiveItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"]!= DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"]!= DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
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
                // };
                observableCollection.Add(receiveItem);
            }

            Label labelIsActivityItemReceivingCommentReceiving = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelProductCodeReceiving = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelProductUnitBCReceiving = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeReceiving = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemReceivingQuantityOrderedReceiving = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemReceivingQuantityReceivedReceiving = new Label { Text = "Received", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsActivityItemReceivingCommentReceiving.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCodeReceiving.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductUnitBCReceiving.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeReceiving.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemReceivingQuantityOrderedReceiving.Text);
            this.columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemReceivingQuantityReceivedReceiving.Text);

            // Header grid
            Grid gridHeaderReceiving = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                },
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

            GridMainActivity.Children.Add(gridHeaderReceiving);

            foreach (ReceiveItem receiveItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.IsActivityItemReceivingComment);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.ProductCode);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.ProductUnitBC);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(receiveItem.UnitCode);
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", receiveItem.ActivityItemReceivingQuantityOrdered));
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", receiveItem.ActivityItemReceivingQuantityReceived));

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeaderReceiving.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeaderReceiving.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeaderReceiving.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeaderReceiving.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeaderReceiving.ColumnDefinitions[4].Width = this.columnWidth4;
                }

                if (detailWidth5 > this.columnWidth5)
                {
                    this.columnWidth5 = detailWidth5;
                    gridHeaderReceiving.ColumnDefinitions[5].Width = this.columnWidth5;
                }
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemFKRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemStringFKRefno,");
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
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemFKRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemStringFKRefno,");
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
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus = {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                sql.Append("ORDER BY tActivityItemReceiving.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemReceiving.ActivityItemRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemFKRefno,");
                sql.Append("tActivityItemReceiving.ActivityItemStringFKRefno,");
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
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.Append("ORDER BY tActivityItemReceiving.ActivityItemRefno");
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
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
            // }
            this.listReceiveItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReceiveItem>(sql.ToString());
            this.ListItems = this.listReceiveItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            this.ListItems = this.listReceiveItems.Where(receiveItem => receiveItem.ProductCode.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (ReceiveItem receiveItem in this.ListItems)
            {
                // ReceiveItem receiveItem = new ReceiveItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"]),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"]),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"]),
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
                // };
                observableCollection.Add(receiveItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// Select an activity item.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity object.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is ReceiveItem item)
            {
                this.SelectedActivityItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFKRefno = item.ActivityItemFKRefno;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);
                int activityItemLocMapRefno = item.LocationMapRefno;

                // This code needs to be before the call to GetSelectedItemLocation, because the skid location may be
                // used if the product unit and warehouse match.
                if (this.SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid &&
                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ProdInfo.Refno)
                {
                    this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                }

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // Only assign the current location if the map is complete else leave it alone.
                    SLocMap locMap = await this.GetSelectedItemLocation(activityItemLocMapRefno);
                    if (locMap != null && await LocationMapTasks.IsCompleteLocMap(locMap))
                    {
                        // Update the chosen location with the current when defaulting is turned on.
                        LocationMap.GetInstance().CurrLocMap = locMap.Clone();
                        LocationMap.GetInstance().ChosenLocMap = locMap.Clone();

                        // Assign the skid the location, because defaulting is enabled.
                        if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                        {
                            this.ActHdr.SkidInfo.CurrSkid.LocMap = locMap.Clone();
                        }
                    }
                }

                // LocationMap.GetInstance().CurrLocMap needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        /// <summary>
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item reference number else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemReceiving.ActivityItemFKRefno,");
            sql.Append("tActivityItemReceiving.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemReceiving ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
            sql.Append("tActivityItemReceiving.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
            sql.Append("tActivityItemReceiving.WarehouseLocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
            sql.AppendFormat("AND tActivityItemReceiving.WarehouseLocationMapRefno = {0} ", this.LocMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemReceiving.ProductUnitRefno = {0}", prodUnitRefno);

            List<ReceiveItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReceiveItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.ItemFKRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.ItemRefno;
            }

            return foundActItemRefno;
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXML(XDocument data)
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
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + prodUnitBarCodeCode + "</gtin>" + "<whs_id></whs_id>"
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
        /// The RefreshQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
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
        /// The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
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
        /// The UpdateQuantityFromActivityItemList.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="actItemRefno">The actItemRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
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
        /// The GetTotalQuantity.
        /// </summary>
        /// <returns>.</returns>
        public override async Task<string> GetTotalQuantity()
        {
            string msg = "Total received:";
            string query = "SELECT SUM(tActivityItemReceiving.ActivityItemReceivingQuantityOrdered) AS "
                + "Ordered,"
                + "SUM(tActivityItemReceiving.ActivityItemReceivingQuantityReceived) AS Received "
                + "FROM tActivityItem "
                + "INNER JOIN tActivityItemReceiving ON tActivityItemReceiving.ActivityItemRefno = "
                + "tActivityItem.ActivityItemRefno "
                + string.Format("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.HdrRefno);

            List<TotalQuantity> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<TotalQuantity>(query);

            if (results != null && results.Count > 0)
            {
                foreach (TotalQuantity result in results)
                {
                    // Because an aggregate sum function is used in a query. There will always be a result record.
                    // This causes problems detecting if there is a valid result.
                    // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
                    msg += string.Format("\r{0} / {1}", Convert.ToString(result.Ordered), Convert.ToString(result.Received));
                }
            }

            return msg;
        }

        /// <summary>
        /// The IsReadyToApply.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task IsReadyToApply(XDocument data)
        {
            await base.IsReadyToApply(data);

            // New items are identified with 0 < this.FKRefno, do not validate.
            if (this.SysEnv.SysSettings.RecvQtyOverWarn != SysCommon.ActReceivingQtyOverValLevels.NoValidationOrWarning && this.ItemFKRefno > 0)
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

        /// <summary>
        /// The ReceivingModuleQuantityOverCheck.
        /// </summary>
        /// <param name="actItemRecvQuantity">The actItemRecvQuantity<see cref="decimal"/>.</param>
        private void ReceivingModuleQuantityOverCheck(decimal actItemRecvQuantity)
        {
            if (this.SelectedActivityItem is ReceiveItem receiveItem)
            {
                decimal received = Convert.ToDecimal(receiveItem.ActivityItemReceivingQuantityReceived);
                decimal ordered = Convert.ToDecimal(receiveItem.ActivityItemReceivingQuantityOrdered);

                if (received + actItemRecvQuantity > ordered)
                {
                    throw new Exception(string.Format("The resulting quantity received {0} {1} will be greater than the quantity ordered {2} {3}.", received + actItemRecvQuantity, this.ProdInfo.UnitCode, ordered, this.ProdInfo.UnitCode));
                }
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            object tmpFKItRefno = ((ReceiveItem)resourceType).ActivityItemFKRefno;
            return tmpFKItRefno == null ? 0 : Convert.ToInt32(tmpFKItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return string.Format("Receive # {0}", this.ActHdr.HdrFKRefno);
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFKRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<SActItemSchemeData> GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Submit(XDocument data, object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Update.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Update(XDocument data)
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
    }
}
