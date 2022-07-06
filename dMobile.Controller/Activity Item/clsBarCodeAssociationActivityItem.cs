// <copyright file="clsBarCodeAssociationActivityItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Models;
    using dMobile.Resources.Types;
    using SQLite;
    using Xamarin.Forms;

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
        public BarCodeActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(BarCodeActItem).FullName;
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
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <param name="askToInsertAsNewItem">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            int prodUnitBarCodeRefno = 0;

            // Look up product bc unit
            string unitCode = string.Empty;
            string prodCode = string.Empty;
            bool prodUnitIsBC = false;
            string prodUnitBC = string.Empty;

            string queryProductUnit = "SELECT tProduct.ProductCode,"
                + "tProductUnit.ProductUnitIsBC,"
                + "tProductUnit.ProductUnitBC,"
                + "tUnit.UnitCode "
                + "FROM tProductUnit "
                + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                + "WHERE tProductUnit.ProductUnitRefno = ?";

            List<ProductUnit> productUnitResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductUnit>(queryProductUnit, Convert.ToInt32(prodUnitRefno));

            if (productUnitResults != null && productUnitResults.Count > 0)
            {
                unitCode = Convert.ToString(productUnitResults[0].UnitCode);
                prodCode = Convert.ToString(productUnitResults[0].ProductCode);
                prodUnitIsBC = Convert.ToBoolean(productUnitResults[0].ProductUnitIsBC);
                prodUnitBC = Convert.ToString(productUnitResults[0].ProductUnitBC);
            }
            else
            {
                throw new Exception(string.Format("Unable to location the product unit refno {0} in tProductUnit.", prodUnitRefno));
            }

            // Check if the bar code is already associated to this product or another product
            bool isProdUnitBarCodeFound = false;
            string prodIdentifier = data.Root.Element("ProductIdentifier").Value;

            string queryProdUnitBarcode = "SELECT ProductUnitRefno, ProductUnitBarCodeRefno FROM tProductUnitBarCode WHERE ProductUnitBarCodeCode = ? AND ProductUnitBarCodeModifier1 = ?";
            List<tProductUnitBarCodeModel> prodUnitBarcodeResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductUnitBarCodeModel>(queryProdUnitBarcode, prodIdentifier, data.Root.Element("ProductUnitBarCodeModifier1").Value);

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

                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Bar Code",
                        string.Format("Scanned bar code is already associated to a different product {0} {1} {2}.", prodCode, prodUnitBC, unitCode), "Yes", "No"));
                }
            }

            if (!isProdUnitBarCodeFound)
            {
                await this.ActHdr.Create(SysCommon.ActSubTypes.BarCodeAssociation);
                await this.Create(SysCommon.ActSubTypes.BarCodeAssociation);

                await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        // Add the new Bar Code to Product Unit Bar Codes
                        string queryInsertProdUnitBarcode = "INSERT INTO tProductUnitBarCode ("
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
                        object[] parametersInsertProdUnitBarcode = new object[]
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
                            + "<ProductUnitIsBC>" + prodUnitIsBC + "</ProductUnitIsBC>"
                            + "<ProductCode>" + prodCode + "</ProductCode>"
                            + "<UnitCode>" + unitCode + "</UnitCode>"
                            + "<ProductUnitBarCodeModifier1>" + this.SysEnv.CurrentBarCodeModifier1
                            + "</ProductUnitBarCodeModifier1>"
                            + "</params>");

                        if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            string temp = this.CreateSubmitXML(data).Result;
                            // string temp = Task.Run(async () => await this.CreateSubmitXML(data)).ToString();
                            string reqData = SysCommon.SubmitComm(temp);
                            if (reqData.Length > 0)
                            {
                                // Check if result is an error message
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
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
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
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItem.ActivityItemRefno AS ActivityItemRefno,");
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
                sql.Append("SELECT tActivityItem.ActivityItemRefno AS ActivityItemRefno,");
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
            this.listBarcodeAssociationItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<BarcodeAssociationItem>(sql.ToString());
            this.ListItems = this.listBarcodeAssociationItems.Cast<object>().ToList();
            return this.ListItems;
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
        /// The GetActivityItemRefno.
        /// </summary>
        /// <param name="prodUnitRefno">.</param>
        /// <returns>.</returns>
        public override Task<int> GetActivityItemRefno(int prodUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SelectActivityItem.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is BarcodeAssociationItem barcodeAssociationItem)
            {
                this.SelectedActivityItem = barcodeAssociationItem;
                this.ItemRefno = Convert.ToInt32(barcodeAssociationItem.ActivityItemRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(barcodeAssociationItem.ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                this.ItemFKRefno = Convert.ToInt32(barcodeAssociationItem.ActivityItemRefno);
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
            XmlDocument xmlDoc = new XmlDocument();
            string send = await this.CreateSubmitXML(data);
            string receivedData = SysCommon.SubmitComm(send);
            if (receivedData.Length > 0)
            {
                xmlDoc.LoadXml(receivedData);

                // check if result is an error message
                if (receivedData.Contains("<message>ERROR</message>"))
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
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?", Convert.ToInt32(SysCommon.ActStatus.NoChange), this.ItemRefno);

                    if (this.ListItems != null && selectedActivityItem is BarcodeAssociationItem barcodeAssociationItem)
                    {
                        barcodeAssociationItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }
                }
            }
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder strXML = new StringBuilder();
            using XmlWriter writer = XmlWriter.Create(strXML);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string barCodeSymbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
            string barCodeModifier1 = data.Root.Element("ProductUnitBarCodeModifier1").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT BARCODE</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>"
                + "<symbology>" + barCodeSymbologyPrefix + "</symbology>"
                + "<alt_unit>" + unitCode + "</alt_unit>"
                + "<gtin>" + prodUnitBarCodeCode + "</gtin>"
                + "<modifier1>" + barCodeModifier1 + "</modifier1>"
                + "</imclient>");

            request.Save(writer);
            writer.Flush();
            writer.Close();

            return strXML.ToString();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// The UpdateQuantityFromActivityItemList.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="actItemRefno">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
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
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            throw new NotImplementedException();
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
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> CreateDeleteXML(XDocument data)
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
        /// <returns>The <see cref="Task{string}"/>.</returns>
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
            int locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
                whsLocMapRefno,
                prodRefno,
                SysCommon.ProdLocMapTypes.Picking);

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
    }
}
