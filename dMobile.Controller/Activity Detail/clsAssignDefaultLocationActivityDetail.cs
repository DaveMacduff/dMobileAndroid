namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class AssignDefaultLocActDetail : ActDetail
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItem"></param>
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
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                    }
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
                label4.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                label5.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
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

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        public override void ClearProperties()
        {
            base.ClearProperties();
        }

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
        /// <param name="data"></param>
        public override void Insert(XDocument data)
        {
            if (DataViewDetails != null)
            {
                DataRowView rowView = DataViewDetails.AddNew();
                rowView["ProductUnitRefno"] = ActItem.ProdInfo.UnitRefno;
                rowView["ProductRefno"] = ActItem.ProdInfo.Refno;
                rowView["ProductCode"] = ActItem.ProdInfo.Code;
                rowView["ProductUnitBC"] = ActItem.ProdInfo.UnitBC;
                rowView["UnitCode"] = ActItem.ProdInfo.UnitCode;
                rowView["ProductDescription"] = ActItem.ProdInfo.Desc;
                rowView["ProductUnitSizeDescription"] = ActItem.ProdInfo.UnitSizeDesc;
                rowView["ActivityDetailQuantity"] = Math.Round(Quantity, 3);
                rowView["ActivityDetailBaseQuantity"] = Math.Round(BaseQuantity, 3);
                rowView["ActivityDetailWeightQuantity"] = Math.Round(WeightQuantity, 3);
                rowView["ActivityDetailSchemeID"] = SchemeID;
                rowView["ActivityDetailUniqueSchemeID"] = UniqueSchemeID;
                rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"] = SchemeData.LogisticUnitBestBeforeSellByDate;
                rowView["ActivityDetailLogisticUnitProductionDate"] = SchemeData.LogisticUnitProductionDate;
                rowView["ActivityDetailSingleTradeItemSerialNumber"] = SchemeData.SingleTradeItemSerialNumber;
                rowView["ActivityDetailLogisticUnitBatchLotNumber"] = SchemeData.LogisticUnitBatchLotNumber;
                rowView["ProductSchemeVariantCode"] = SchemeData.VariantCode;
                rowView["ProductSchemeVariantNo"] = SchemeData.VariantNumber;
                rowView["ActivityDetailFKRefno"] = FKRefno;
                rowView["ActivityDetailRefno"] = Refno;

                rowView.EndEdit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Delete()
        {
            throw new NotImplementedException();
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
                this.FKRefno = Convert.ToInt32(DataViewDetails[idx]["ActivityDetailFKRefno"]);
            }

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="data"></param>
        public override void Submit(int rowIdx, XDocument data)
        {
            this.GetInfo(rowIdx, false);

            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = CreateSubmitXML(data);
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

                    if (this.DataViewDetails != null)
                    {
                        this.DataViewDetails[rowIdx].BeginEdit();
                        ////this.DataViewDetails[rowIdx]["ActivityDetailAdjustmentFKRefno"] = this.FKRefno;
                        this.DataViewDetails[rowIdx]["ActivityDetailStatus"] = SysCommon.ActStatus.NoChange;
                        this.DataViewDetails[rowIdx].EndEdit();
                    }

                    using (SQLiteCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndDetail.CommandType = CommandType.Text;
                        cmndDetail.CommandText = "UPDATE tActivityDetail "
                            + "SET tActivityDetail.ActivityDetailStatus = " + (int)SysCommon.ActStatus.NoChange
                            + "WHERE tActivityDetail.ActivityDetailRefno = " + this.Refno;
                        cmndDetail.ExecuteNonQuery();
                    }
                }
            }

            this.DataViewDetails.Delete(rowIdx);
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
        /// Create and fill the main grid data view.
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns>Filled main grid data view.</returns>
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

            this.DataViewDetails = table.DefaultView;
            this.DataViewDetails.Table.TableName = this.MainGridMappingName;

            return this.DataViewDetails;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource strings
            Label labelHeader1= new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader2= new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader3= new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader4= new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader5= new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelHeader6= new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);
            double detailWidth0 = 0;
            double detailWidth1 = 0;
            double detailWidth2 = 0;
            double detailWidth3 = 0;
            double detailWidth4 = 0;
            double detailWidth5 = 0;

            // Create header data template
            Grid gridHeader = new Grid
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

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (DataRow row in this.DataViewDetails.ToTable().Rows)
            {
                MoveDetail moveDetail = new MoveDetail
                {
                    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
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
                    ProductSchemeVariantCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                    ActivityDetailFKRefno = Convert.ToInt32(row["ActivityDetailFKRefno"] != DBNull.Value ? row["ActivityDetailFKRefno"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null)

                };

                observableCollection.Add(moveDetail);

                detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.ProductCode);
                detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.ProductUnitBC);
                detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.UnitCode);
                detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", moveDetail.ActivityDetailQuantity));
                detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", moveDetail.ActivityDetailWeightQuantity));
                detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(moveDetail.ActivityDetailSchemeID);

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

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                }

                if (detailWidth5 > columnWidth5)
                {
                    columnWidth5 = detailWidth5;
                    gridHeader.ColumnDefinitions[5].Width = columnWidth5;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <param name="dataGridMain"></param>
        //    /// <remarks></remarks>
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        
        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductCode"; 
        //      dataGridCol.HeaderText = "Code";

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductUnitBC";
        //      dataGridCol.HeaderText = "BC";

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "UnitCode";
        //      dataGridCol.HeaderText = string.Empty;

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityDetailQuantity";
        //      dataGridCol.HeaderText = "Qty";
        //      dataGridCol.Format = "0.000";

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityDetailWeightQuantity";
        //      dataGridCol.HeaderText = "Weight";
        //      dataGridCol.Format = "0.000";

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityDetailSchemeID";
        //      dataGridCol.HeaderText = "Scheme ID";

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
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>")
                + ("<is_bc>" + this.ActItem.ProdInfo.UnitIsBC + "</is_bc>")
                + ("<whs_id>" + data.Root.Element("whs_id").Value + "</whs_id>")
                + ("<sect_refno>" + data.Root.Element("sect_refno").Value + "</sect_refno>")
                + ("<slot_refno>" + data.Root.Element("slot_refno").Value + "</slot_refno>")
                + ("<to_sect_refno>" + data.Root.Element("to_sect_refno").Value + "</to_sect_refno>")
                + ("<to_slot_refno>" + data.Root.Element("to_slot_refno").Value + "</to_slot_refno>")
                + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                + ("<alt_qty>" + this.Quantity + "</alt_qty>")
                + ("<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>")
                + ("<base_qty>" + this.BaseQuantity + "</base_qty>")
                + ("<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>")
                + ("<wt_qty>" + this.WeightQuantity + "</wt_qty>")
                + ("<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>")
                + ("<serial>" + this.SchemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<lot_no>" + this.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<pdate>" + production + "</pdate>")
                + ("<bdate>" + bestBefore + "</bdate>")
                + ("<variant_no>" + this.SchemeData.VariantNumber + "</variant_no>")
                + ("<ser_refno>" + this.FKRefno + "</ser_refno>")
                + "</imclient>"
            );

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        #endregion
    }
}
