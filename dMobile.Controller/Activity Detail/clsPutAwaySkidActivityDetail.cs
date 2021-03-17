namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class PutAwaySkidActDetail : ActDetail
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="actItem"></param>
        /// <remarks></remarks>
        public PutAwaySkidActDetail(ActItem actItem)
          : base(actItem)
        {
            this.FullClassName = typeof(PutAwaySkidActDetail).FullName;

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
                Label labelSpacer = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelSpacer.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelSchemeID.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelSpacer);
                gridData.Children.Add(labelSchemeID);

                Grid.SetColumn(labelQty, 0);
                Grid.SetColumn(labelWeight, 1);
                Grid.SetColumn(labelSpacer, 2);
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
        public override void InsertSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override void DeleteSkid(SysSkidInfo skidInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detailData"></param>
        public override void Insert(XDocument detailData)
        {
            if (this.DataViewDetails != null)
            {
                DataRowView rowView = this.DataViewDetails.AddNew();
                rowView["ProductUnitRefno"] = this.ActItem.ProdInfo.UnitRefno;
                rowView["ProductRefno"] = this.ActItem.ProdInfo.Refno;
                rowView["ProductCode"] = this.ActItem.ProdInfo.Code;
                rowView["ProductUnitBC"] = this.ActItem.ProdInfo.UnitBC;
                rowView["UnitCode"] = this.ActItem.ProdInfo.UnitCode;
                rowView["ProductDescription"] = this.ActItem.ProdInfo.Desc;
                rowView["ProductUnitSizeDescription"] = this.ActItem.ProdInfo.UnitSizeDesc;

                rowView["ActivityDetailQuantity"] = Math.Round(this.Quantity, 3);
                rowView["ActivityDetailBaseQuantity"] = Math.Round(this.BaseQuantity, 3);
                rowView["ActivityDetailWeightQuantity"] = Math.Round(this.WeightQuantity, 3);

                rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"] = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                rowView["ActivityDetailLogisticUnitProductionDate"] = this.SchemeData.LogisticUnitProductionDate;
                rowView["ActivityDetailSingleTradeItemSerialNumber"] = this.SchemeData.SingleTradeItemSerialNumber;
                rowView["ActivityDetailLogisticUnitBatchLotNumber"] = this.SchemeData.LogisticUnitBatchLotNumber;
                rowView["ProductSchemeVariantCode"] = this.SchemeData.VariantCode;
                rowView["ProductSchemeVariantNo"] = this.SchemeData.VariantNumber;
                rowView["ActivityDetailLogisticUnitSkidNumber"] = this.SchemeData.LogisticUnitSkidNumber;
                rowView["ActivityDetailSchemeID"] = this.SchemeID;
                rowView["ActivityDetailUniqueSchemeID"] = this.UniqueSchemeID;

                rowView["ActivityDetailFKRefno"] = this.FKRefno;
                rowView["ActivityDetailRefno"] = this.Refno;

                rowView["ActivityDetailExtra1Text"] = this.ActExtra.Text1;
                rowView["ActivityDetailExtra2Text"] = this.ActExtra.Text2;
                rowView["ActivityDetailExtra3Text"] = this.ActExtra.Text3;
                rowView["ActivityDetailExtra4Text"] = this.ActExtra.Text4;
                rowView["ActivityDetailExtra5Date"] = this.ActExtra.DateTime5;

                string locCode = string.Empty;
                string locName = string.Empty;

                if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SlotCode.ToString();
                    locName = this.LocMap.CurrLocMap.SlotName;
                }
                else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SectCode.ToString();
                    locName = this.LocMap.CurrLocMap.SectName;
                }
                else
                {
                    locCode = this.LocMap.CurrLocMap.WhsCode.ToString();
                    locName = this.LocMap.CurrLocMap.WhsName;
                }

                rowView["LocationMapRefno"] = this.LocMap.CurrLocMapRefno;
                rowView["LocationCode"] = locCode;
                rowView["LocationName"] = locName;

                rowView.EndEdit();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override void Delete()
        {
            for (int i = 0; i <= this.DataViewDetails.Count; i++)
            {
                this.DataViewDetails.Delete(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public override ActDetail GetInfo(int idx, bool isAdd)
        {
            base.GetInfo(idx, isAdd);

            if (!isAdd)
            {
                this.FKRefno = Convert.ToInt32(this.DataViewDetails[idx]["ActivityDetailFKRefno"]);
                this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewDetails[idx]["LocationMapRefno"]));
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="data"></param>
        public override void Submit(int idx, XDocument data)
        {
            this.GetInfo(idx, false);

            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = this.CreateSubmitXML(data);
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                xmlDoc.LoadXml(reqData);

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

                    if (this.DataViewDetails != null)
                    {
                        this.DataViewDetails[idx].BeginEdit();
                        ////this.DataViewDetails[idx]["ActivityDetailProductionFinishedFKRefno"] = this.FKRefno;
                        this.DataViewDetails[idx]["ActivityDetailStatus"] = SysCommon.ActStatus.NoChange;
                        this.DataViewDetails[idx].EndEdit();
                    }

                    using (SQLiteCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndDetail.CommandType = CommandType.Text;
                        cmndDetail.CommandText = "UPDATE tActivityDetail "
                            + "SET ActivityDetailStatus = " + Convert.ToInt32(SysCommon.ActStatus.NoChange)
                            + " WHERE ActivityDetailRefno = " + this.Refno;
                        cmndDetail.ExecuteNonQuery();
                    }
                }
            }

            this.DataViewDetails.Delete(idx);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefno(int idx)
        {
            return Convert.ToInt32(DataViewDetails[idx]["ActivityDetailRefno"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public override int GetFKRefno(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
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
            table.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
            table.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));

            table.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
            table.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
            table.Columns.Add("ProductSchemeVariantNo", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailLogisticUnitSkidNumber", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));

            table.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
            table.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));

            table.Columns.Add("ActivityDetailExtra1Text", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailExtra2Text", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailExtra3Text", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailExtra4Text", Type.GetType("System.String"));
            table.Columns.Add("ActivityDetailExtra5Date", Type.GetType("System.DateTime"));

            table.Columns.Add("LocationMapRefno", Type.GetType("System.Int32"));
            table.Columns.Add("LocationCode", Type.GetType("System.String"));
            table.Columns.Add("LocationName", Type.GetType("System.String"));

            this.DataViewDetails = table.DefaultView;
            this.DataViewDetails.Table.TableName = this.MainGridMappingName;

            return this.DataViewDetails;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewDetails.ToTable().Rows)
            {
                SkidPutAwayDetail skidPutAwayDetail = new SkidPutAwayDetail
                {
                    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                    ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                    ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"] != DBNull.Value ? row["ActivityDetailUniqueSchemeID"] : null),
                    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                    ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitSkidNumber"] : null),
                    LocationCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityDetailFKRefno = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    ProductSchemeVariantCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                    ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"] != DBNull.Value ? row["ActivityDetailExtra1Text"] : null),
                    ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"] != DBNull.Value ? row["ActivityDetailExtra2Text"] : null),
                    ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"] != DBNull.Value ? row["ActivityDetailExtra3Text"] : null),
                    ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"] != DBNull.Value ? row["ActivityDetailExtra4Text"] : null),
                    ActivityDetailExtra5Date = Convert.ToDateTime(row["ActivityDetailExtra5Date"] != DBNull.Value ? row["ActivityDetailExtra5Date"] : null)
                };

                observableCollection.Add(skidPutAwayDetail);
            }

            // TODO: Resource strings
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelSpacerHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelSchemeIDHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSpacerHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

            // Header grid
            Grid gridHeaderSkidPutAway = new Grid
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

            gridHeaderSkidPutAway.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeaderSkidPutAway.Children.Add(labelQtyHeader);
            gridHeaderSkidPutAway.Children.Add(labelWeightHeader);
            gridHeaderSkidPutAway.Children.Add(labelSpacerHeader);
            gridHeaderSkidPutAway.Children.Add(labelSchemeIDHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelSpacerHeader, 2);
            Grid.SetColumn(labelSchemeIDHeader, 3);

            Grid.SetRow(gridHeaderSkidPutAway, 0);

            dataGridMainActivityHeader.Children.Add(gridHeaderSkidPutAway);

            foreach (SkidPutAwayDetail skidPutAwayDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", skidPutAwayDetail.ActivityDetailQuantity));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", skidPutAwayDetail.ActivityDetailWeightQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(skidPutAwayDetail.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(skidPutAwayDetail.ActivityDetailSchemeID);

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeaderSkidPutAway.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeaderSkidPutAway.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeaderSkidPutAway.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeaderSkidPutAway.ColumnDefinitions[3].Width = columnWidth3;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <param name="dataGridMain"></param>
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityDetailQuantity";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
        //#else
        //      dataGridCol.HeaderText = "Qty";
        //#endif
        //      dataGridCol.Width = (int)(50 * colWidthFactor);
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityDetailWeightQuantity";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
        //#else
        //      dataGridCol.HeaderText = "Weight";
        //#endif
        //      dataGridCol.Width = (int)(55 * colWidthFactor);
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityDetailSchemeID";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.SchemeId);
        //#else
        //      dataGridCol.HeaderText = "Scheme ID";
        //#endif
        //      dataGridCol.Width = (int)(500 * colWidthFactor);
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridMain.TableStyles.Add(dataGridStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

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
        /// <param name="selectedIndex"></param>
        public override void SelectDetail(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateDeleteXML(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
