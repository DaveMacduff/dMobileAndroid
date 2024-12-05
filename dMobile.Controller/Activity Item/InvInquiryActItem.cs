// <copyright file="InvInquiryActItem.cs" company="InterSect Business Systems Inc">
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
using dMobile.Resources.Services;
using dMobile.Resources.Types.Items;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="InvInquiryActItem" />.
    /// </summary>
    public class InvInquiryActItem : ActItem
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
        /// Defines the listInventoryInquiryItems.
        /// </summary>
        private List<InventoryInquiryItem> listInventoryInquiryItems = new List<InventoryInquiryItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InvInquiryActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public InvInquiryActItem(Activity.Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(CycleActItem).FullName;
            this.IsAskCreateNewItem = false;

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
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

                var label1 = new Label { VerticalOptions = LayoutOptions.Center };
                var label2 = new Label { VerticalOptions = LayoutOptions.Center };
                var label3 = new Label { VerticalOptions = LayoutOptions.Center };
                var label4 = new Label { VerticalOptions = LayoutOptions.Center };
                var label5 = new Label { VerticalOptions = LayoutOptions.Center };
                var label6 = new Label { VerticalOptions = LayoutOptions.Center };

                label1.SetBinding(Label.TextProperty, new Binding("Productcode"));
                label2.SetBinding(Label.TextProperty, new Binding("Productunitbc"));
                label3.SetBinding(Label.TextProperty, new Binding("Activityitemquantity", BindingMode.Default, null, null, "{0:F3}"));
                label4.SetBinding(Label.TextProperty, new Binding("Unitcode"));
                label5.SetBinding(Label.TextProperty, new Binding("Activityitemweightquantity", BindingMode.Default, null, null, "{0:F3}"));
                label6.SetBinding(Label.TextProperty, new Binding("Weightunitcode"));

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

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplateInvInquiryItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateInvInquiryItem", dataTemplate);
            }
        }

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItemFromList(int actItemFoundRefno)
        {
            throw new NotImplementedException();
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
        /// The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void AddToQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFkRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data)
        {
            var prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            if (prodUnitRefno != 0)
            {
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            var labelHeader1 = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader2 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader3 = new Label { Text = "Quantity", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader4 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader5 = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelHeader6 = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader1.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader2.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader3.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader4.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader5.Text);
            this.columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelHeader6.Text);

            // Create header data template
            var gridHeader = new Grid
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

            gridMainActivity.Children.Add(gridHeader);

            foreach (var inventoryInquiryItem in this.ListItems.Cast<InventoryInquiryItem>())
            {
                // InventoryInquiryItem inventoryInquiryItem = new InventoryInquiryItem
                // {
                //    Productcode = Convert.ToString(row["Productcode"] != DBNull.Value ? row["Productcode"] : null),
                //    Productunitbc = Convert.ToString(row["Productunitbc"] != DBNull.Value ? row["Productunitbc"] : null),
                //    Unitcode = Convert.ToString(row["Unitcode"] != DBNull.Value ? row["Unitcode"] : null),
                //    Weightunitcode = Convert.ToString(row["Weightunitcode"] != DBNull.Value ? row["Weightunitcode"] : null),
                //    Activityitemquantity = Convert.ToDecimal(row["Activityitemquantity"] != DBNull.Value ? row["Activityitemquantity"] : null),
                //    Activityitemweightquantity = Convert.ToDecimal(row["Activityitemweightquantity"] != DBNull.Value ? row["Activityitemweightquantity"] : null)
                // };
                observableCollection.Add(inventoryInquiryItem);

                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.Productcode);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.Productunitbc);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{inventoryInquiryItem.ActivityItemQuantity:F3}");
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.UnitCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{inventoryInquiryItem.ActivityItemWeightQuantity:F3}");
                var detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(inventoryInquiryItem.WeightUnitCode);

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

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.columnWidth4;
                }

                if (detailWidth5 > this.columnWidth5)
                {
                    this.columnWidth5 = detailWidth5;
                    gridHeader.ColumnDefinitions[5].Width = this.columnWidth5;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateInvInquiryItem"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var dataTableItems = new DataTable("tActivityItem");

            dataTableItems.Columns.Add("productcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("productunitbc", Type.GetType("System.String"));
            dataTableItems.Columns.Add("unitcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("weightunitcode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("activityitemquantity", Type.GetType("System.Decimal"));
            dataTableItems.Columns.Add("activityitemweightquantity", Type.GetType("System.Decimal"));

            this.listInventoryInquiryItems = dataTableItems.Rows.Cast<InventoryInquiryItem>().ToList();
            this.ListItems = this.listInventoryInquiryItems.Cast<object>().ToList();
            return Task.FromResult(this.ListItems);
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
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item refno else 0.</returns>
        public override Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SelectActivityItem.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns><see cref="Task{T}"/> Activity item foreign key reference number.</returns>
        public override async Task<int> AssignActivityItem(object activityItem)
        {
            var itemFkRefno = Task.FromResult(0);
            this.SelectedItem = activityItem;
            return await itemFkRefno;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            try
            {
                var responseData = SysCommon.SubmitComm(await this.CreateSubmitXml(data));
                this.ListItems = await this.GetActivityItemList(false);
                using var ds = new DataSet();
                ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
                if (ds.Tables.Count != 0)
                {
                    this.ListItems = ds.Tables["tActivityItem"].Select().ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Refresh inventory inquiry items failed.", ex);
            }
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST INVENTORY INQUIRY SUMMARY</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + this.ProdInfo.Code + "</prod_id>"
                + "<whs_id>" + this.LocMap.WhsFkRefno + "</whs_id>"
                + "<sect_refno>" + this.LocMap.SectFkRefno + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.SlotFkRefno + "</slot_refno>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
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
        public override void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override Task UpdateItemTableQuantity(object activityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return "Inventory inquiry detail.";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFkRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
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
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> CreateDeleteXml(XDocument data)
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
        /// The GetTotalQuantity.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetTotalQuantity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is InventoryInquiryItem item)
            {
                locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    whsLocMapRefno,
                    prodRefno,
                    SysCommon.ProdLocMapTypes.Picking);
            }

            if (locMapRefno == 0)
            {
                locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    whsLocMapRefno,
                    prodRefno,
                    SysCommon.ProdLocMapTypes.Floor);
            }

            if (!await LocationMapTasks.IsCompleteLocMap(locMapRefno))
            {
                locMapRefno = 0;
            }

            return locMapRefno;
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

        /// <summary>
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool IsCatchAsk()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool IsCatchAskZero()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the list item index of the item that matches the activity item foreign key reference number.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item foreign key reference number.</param>
        /// <returns><see cref="int"/> Activity list item found index, else -1.</returns>
        public override int FindActivityItemIndex(int activityItemFkRefno)
        {
            var foundIdx = -1;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in this.ListItems)
            {
                foundIdx++;
                if (((InvInquiryActItem)item).ItemFkRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override DataTemplate GetDataTemplate()
        {
            throw new NotImplementedException();
        }
    }
}
