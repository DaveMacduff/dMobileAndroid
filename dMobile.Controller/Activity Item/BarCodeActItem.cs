// <copyright file="BarCodeActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Resources.Models.Products;
using dMobile.Resources.Types.Items;
using dMobile.Resources.Types.Products;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="BarCodeActItem" />.
    /// </summary>
    public class BarCodeActItem : ActItem
    {
        /// <summary>
        /// Defines the listBarcodeAssociationItems.
        /// </summary>
        private List<BarcodeAssociationItem> listBarcodeAssociationItems = new List<BarcodeAssociationItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public BarCodeActItem(Activity.Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(BarCodeActItem).FullName;
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
        /// Insert a new bar code activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Bar code activity information to insert.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data)
        {
            var prodUnitBarCodeRefno = 0;

            // Look up product bc unit
            var unitCode = string.Empty;
            var prodCode = string.Empty;
            var prodUnitIsBc = false;
            var prodUnitBc = string.Empty;

            var queryProductUnit = "SELECT tProduct.ProductCode,"
                                   + "tProductUnit.ProductUnitIsBC,"
                                   + "tProductUnit.ProductUnitBC,"
                                   + "tUnit.UnitCode "
                                   + "FROM tProductUnit "
                                   + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                   + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                   + "WHERE tProductUnit.ProductUnitRefno = ?";

            var prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            var productUnitResults = await DatabaseConnection.GetInstance().SqlConn.
                QueryAsync<ProductUnit>(queryProductUnit, Convert.ToInt32(prodUnitRefno));

            if (productUnitResults != null && productUnitResults.Count > 0)
            {
                unitCode = Convert.ToString(productUnitResults[0].UnitCode);
                prodCode = Convert.ToString(productUnitResults[0].ProductCode);
                prodUnitIsBc = Convert.ToBoolean(productUnitResults[0].ProductUnitIsBC);
                prodUnitBc = Convert.ToString(productUnitResults[0].ProductUnitBC);
            }
            else
            {
                throw new Exception($"Unable to location the product unit refno {prodUnitRefno} in tProductUnit.");
            }

            // Check if the bar code is already associated to this product or another product
            var isProdUnitBarCodeFound = false;
            var prodIdentifier = data.Root.Element("ProductIdentifier").Value;

            var queryProdUnitBarcode = "SELECT ProductUnitRefno, ProductUnitBarCodeRefno FROM tProductUnitBarCode" +
                                       " WHERE ProductUnitBarCodeCode = ? AND ProductUnitBarCodeModifier1 = ?";
            var prodUnitBarcodeResults = await DatabaseConnection.GetInstance().SqlConn.
                QueryAsync<ProductUnitBarCodeModel>(queryProdUnitBarcode, prodIdentifier, data.Root.Element("ProductUnitBarCodeModifier1").Value);

            if (prodUnitBarcodeResults != null && prodUnitBarcodeResults.Count > 0)
            {
                if (prodUnitRefno == Convert.ToInt32(prodUnitBarcodeResults[0].ProductUnitRefno))
                {
                    // bar code is already associated to this product
                    isProdUnitBarCodeFound = true;
                }
                else
                {
                    // bar code is associated to a different product
                    isProdUnitBarCodeFound = true;

                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        "Bar Code",
                        $"Scanned bar code is already associated to a different product {prodCode} {prodUnitBc} {unitCode}.",
                        "Yes",
                        "No"));
                }
            }

            if (!isProdUnitBarCodeFound)
            {
                await this.ActHdr.Create(SysCommon.ActSubTypes.BarCodeAssociation);
                await this.Create(SysCommon.ActSubTypes.BarCodeAssociation);

                await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        // Add the new Bar Code to Product Unit Bar Codes
                        var queryInsertProdUnitBarcode = "INSERT INTO tProductUnitBarCode ("
                                                         + "ProductUnitRefno,"
                                                         + "ProductUnitBarCodeCode,"
                                                         + "ProductUnitBarCodeRaw,"
                                                         + "ProductUnitBarCodeSymbologyEnum,"
                                                         + "ProductUnitBarCodeModifier1,"
                                                         + "ProductUnitBarCodeStatus" +
                                                         " ) VALUES ( " +
                                                         "?, " +
                                                         "?, " +
                                                         "?, " +
                                                         "?, " +
                                                         "?, " +
                                                         "?)";
                        var parametersInsertProdUnitBarcode = new object[]
                        {
                            Convert.ToInt32(prodUnitRefno),
                            Convert.ToString(data.Root.Element("ProductIdentifier").Value),
                            Convert.ToString(data.Root.Element("RawData").Value),
                            Convert.ToString(data.Root.Element("enumBarCodeSymbology").Value),
                            Convert.ToString(this.SysEnv.CurrentBarCodeModifier1),
                            Convert.ToInt32(SysCommon.ActStatus.Inserted),
                        };

                        transaction.Execute(queryInsertProdUnitBarcode, parametersInsertProdUnitBarcode);

                        prodUnitBarCodeRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tProductUnitBarCode"));

                        // Create Activity to Bar Code XRef
                        transaction.Execute("INSERT INTO tActivityItemBarCodeAssociationXRef (ActivityItemRefno, ProductUnitBarCodeRefno) VALUES (?, ?)", Convert.ToInt32(this.ItemRefno), Convert.ToInt32(prodUnitBarCodeRefno));
                        transaction.Commit();

                        data = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<params>"
                            + "<ProductUnitBarCodeRefno>" + prodUnitBarCodeRefno + "</ProductUnitBarCodeRefno>"
                            + "<ProductUnitRefno>" + prodUnitRefno + "</ProductUnitRefno>"
                            + "<ProductUnitBarCodeSymbologyPrefix>" + data.Root.Element("BarCodeSymbologyPrefix").Value
                            + "</ProductUnitBarCodeSymbologyPrefix>"
                            + "<ProductUnitBarCodeCode>" + data.Root.Element("ProductIdentifier").Value
                            + "</ProductUnitBarCodeCode>"
                            + "<ProductUnitIsBC>" + prodUnitIsBc + "</ProductUnitIsBC>"
                            + "<ProductCode>" + prodCode + "</ProductCode>"
                            + "<UnitCode>" + unitCode + "</UnitCode>"
                            + "<ProductUnitBarCodeModifier1>" + this.SysEnv.CurrentBarCodeModifier1
                            + "</ProductUnitBarCodeModifier1>"
                            + "</params>");

                        if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            var temp = this.CreateSubmitXml(data).Result;

                            // string temp = Task.Run(async () => await this.CreateSubmitXML(data)).ToString();
                            _ = SysCommon.SubmitComm(temp);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno AS ProductUnitBarCodeRefno,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode AS ProductUnitBarCodeCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1 AS ProductUnitBarCodeModifier1,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeSymbologyEnum AS ProductUnitBarCodeSymbologyEnum,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityStatus ");
                sql.Append("FROM tActivityItem ");
                sql.Append("INNER JOIN tActivityItemBarCodeAssociationXRef ON ");
                sql.Append("tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno ");
                sql.Append("= tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItem.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno AS ProductUnitBarCodeRefno,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode AS ProductUnitBarCodeCode,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1 AS ProductUnitBarCodeModifier1,");
                sql.Append("tProductUnitBarCode.ProductUnitBarCodeSymbologyEnum AS ProductUnitBarCodeSymbologyEnum,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityStatus ");
                sql.Append("FROM tActivityItem ");
                sql.Append("INNER JOIN tActivityItemBarCodeAssociationXRef ");
                sql.Append("ON tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno ");
                sql.Append("= tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.Append("ORDER BY tActivityItem.ActivityItemRefno");
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
            //        TODO: Fix sorting
            //    }
            // }
            this.listBarcodeAssociationItems = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<BarcodeAssociationItem>(sql.ToString());
            this.ListItems = this.listBarcodeAssociationItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <inheritdoc cref="ActivityItem.Search(string)"/>
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
        public override async Task AssignActivityItem(object activityItem)
        {
            if (activityItem is BarcodeAssociationItem item)
            {
                this.SelectedItem = item;
                this.ItemRefno = Convert.ToInt32(item.ActivityItemRefno);
                this.ActStatus = (SysCommon.ActStatus)item.ActivityItemStatus;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(item.ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                this.ItemFkRefno = Convert.ToInt32(item.ActivityItemRefno);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="data">.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Submit(XDocument data, object selectedActivityItem)
        {
            var xmlDoc = new XmlDocument();
            var send = await this.CreateSubmitXml(data);
            var receivedData = SysCommon.SubmitComm(send);
            if (receivedData.Length > 0)
            {
                xmlDoc.LoadXml(receivedData);
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                    "UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange),
                    this.ItemRefno);

                if (this.ListItems != null && selectedActivityItem is BarcodeAssociationItem barcodeAssociationItem)
                {
                    barcodeAssociationItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                }
            }
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var strXml = new StringBuilder();
            using var writer = XmlWriter.Create(strXml);
            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var barCodeSymbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
            var barCodeModifier1 = data.Root.Element("ProductUnitBarCodeModifier1").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT BARCODE</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBc.ToString() + "</is_bc>"
                + "<symbology>" + barCodeSymbologyPrefix + "</symbology>"
                + "<alt_unit>" + unitCode + "</alt_unit>"
                + "<gtin>" + prodUnitBarCodeCode + "</gtin>"
                + "<modifier1>" + barCodeModifier1 + "</modifier1>"
                + "</imclient>");

            request.Save(writer);
            writer.Flush();
            writer.Close();

            return strXml.ToString();
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
            return "Bar code association.";
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
            var locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                whsLocMapRefno,
                prodRefno,
                SysCommon.ProdLocMapTypes.Picking);

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
                if (((BarCodeActItem)item).ItemFkRefno == activityItemFkRefno)
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

        public override string GetOrderedExpectedOnHandTitle()
        {
            throw new NotImplementedException();
        }

        public override string GetShippedReceivedCountedTitle()
        {
            throw new NotImplementedException();
        }

        public override string GetOrderedExpectedOnHandQuantity()
        {
            throw new NotImplementedException();
        }

        public override string GetShippedReceivedCountedQuantity()
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<object> CreateObservableCollectionFromListItems(List<object> listItems)
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<object> CreateObservableCollectionFromListItems(List<object> listItems, string filterSearch)
        {
            throw new NotImplementedException();
        }
    }
}
