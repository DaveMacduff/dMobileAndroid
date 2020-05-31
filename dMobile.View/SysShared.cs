using System;
using System.Data;
using System.Reflection;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using System.Data.SQLite;
using Xamarin.Forms;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Resource = dMobile.Resources.Properties.Resources;
using System.Threading.Tasks;

namespace dMobile.View
{
    public class SysShared : ContentPage
    {
        // TODO: Resource strings
        /// <summary>
        /// Parse Nossack skid bar codes.
        /// </summary>
        /// <remarks>
        /// Considered a skid if the total weight is 4 times the average of a single unit.
        /// Rule: Must be catch weight.
        /// Rule: Change Removed check for Nossack's GTIN Company Prefix WO#11478
        /// Rule: Change WO#11263
        /// Was:
        /// (01) 90067156705060 (11) 131031 (3102) 001231 (21) 0102
        /// Will be:
        /// (01) 90067156705060 (11) 131031 (3102) 001231 (21) 01002
        /// Extra digit added for number of boxes in AI 21
        /// </remarks>
        /// <param name="parse"></param>
        /// <param name="actDetail"></param>
        /// <param name="unitQty"></param>
        public static void EvalParse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount(
            BarCodeParse parse,
            ActDetail actDetail,
            ref decimal unitQty)
        {
            // Removed check for Nossack's GTIN Company Prefix WO#11478
            // Nossack's GTIN Company Prefix

            // if (parse.GTIN.Contains("67156"))
            // {
            decimal weight = actDetail.WeightQuantity;

            if (actDetail.ActItem.ProdInfo.WeightUnitCode == "LB")
            {
                if (0 < parse.NetWeightMetricKG)
                {
                    weight = (decimal)SysCommon.ConvertKG2LB((double)parse.NetWeightMetricKG);
                }
                else if (0 < parse.NetWeightNonMetricLB)
                {
                    weight = parse.NetWeightNonMetricLB;
                }
            }
            else if (actDetail.ActItem.ProdInfo.WeightUnitCode == "KG")
            {
                if (0 < parse.NetWeightNonMetricLB)
                {
                    weight = (decimal)SysCommon.ConvertLB2KG((double)parse.NetWeightNonMetricLB);
                }
                else if (0 < parse.NetWeightMetricKG)
                {
                    weight = parse.NetWeightMetricKG;
                }
            }

            // Magic number 4 times the average weight to determine if the code is a skid label.
            if (parse.SingleTradeItemSerialNumber.Length > 1 && weight > (actDetail.ActItem.ProdInfo.UnitWeight * 4))
            {
                // 2 = For the first digit past the 01 in the AI(21).
                unitQty = decimal.Parse(parse.SingleTradeItemSerialNumber.Substring(2));
            }

            // }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        /// <param name="activitySubType"></param>
        /// <param name="barCodeParse"></param>
        /// <param name="productUnitRefno"></param>
        public static async Task<int> IdentifyProductScanned(SysCommon.ActSubTypes actSubType, BarCodeParse barCodeParse, int prodUnitRefno)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            int prodUnitBarCodeRefno = 0;
            Controller.Activity actBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
            ActItem actItemBarCode = ActItemFacade.GetActItem(actBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            ActDetail actDetailBarCode = ActDetailFacade.GetActDetail(actItemBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            // Initialize by reference parameter variables
            prodUnitRefno = 0;

            // Attempt to find the product in local database.
            using (SQLiteCommand cmndProdUnitBarCode = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                // Do not look up gtin in barcode database for this rule, product will be identified by a range
                // of fixed weights.
                if (sysEnv.BarCodeSettings.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct && barCodeParse.ProductIdentifier == "90695955110022")
                {
                    BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIDProduct(barCodeParse);

                    ProductInfo prodInfo = ProductInfoTasks.GetInfoFromCodeBCUnit(barCodeParse.ProductIdentifier, " ", "CS");
                    prodUnitBarCodeRefno = 0;
                    prodUnitRefno = prodInfo.UnitRefno;
                }
                else
                {
                    cmndProdUnitBarCode.CommandType = CommandType.Text;
                    cmndProdUnitBarCode.CommandText = "SELECT ProductUnitBarCodeRefno, ProductUnitRefno "
                        + "FROM tProductUnitBarCode "
                        + "WHERE tProductUnitBarCode.ProductUnitBarCodeCode = @productUnitBarCodeCode "
                        + "AND tProductUnitBarCode.ProductUnitBarCodeModifier1 = @productUnitBarCodeModifier1";
                    // TODO: figure out why padRight is used here as it does not return any results.
                    //cmndProdUnitBarCode.Parameters.Add(new SQLiteParameter("@productUnitBarCodeCode", barCodeParse.ProductIdentifier.PadRight(20)));
                    cmndProdUnitBarCode.Parameters.Add(new SQLiteParameter("@productUnitBarCodeCode", barCodeParse.ProductIdentifier));
                    cmndProdUnitBarCode.Parameters.Add(new SQLiteParameter("@productUnitBarCodeModifier1", sysEnv.CurrentBarCodeModifier1));

                    using (SQLiteDataReader prodUnitBarCodeReader = cmndProdUnitBarCode.ExecuteReader())
                    {
                        if (prodUnitBarCodeReader.Read())
                        {
                            prodUnitBarCodeRefno = Convert.ToInt32(prodUnitBarCodeReader["ProductUnitBarCodeRefno"]);
                            prodUnitRefno = Convert.ToInt32(prodUnitBarCodeReader["ProductUnitRefno"]);

                            tcs.SetResult(prodUnitRefno);
                        }
                    }
                }

                // If product was not found in the database ask the comm server if it can identify it.
                // Deli PLU's will be handled by the comm server also.
                // <issue="13440" date="2016/02/18" author="RD"> - Added the condition for prodUnitRefno == 0 to fix Sumwa farms scanning
                // issue where it was bringing in the avg weight instead of barcode parsed weight.
                // This prevents the found prodInfo from being overwritten.</issue>
                if (prodUnitBarCodeRefno == 0 && prodUnitRefno == 0)
                {
                    if (!sysEnv.SysSettings.IsDisconnectedMode)
                    {
                        try
                        {
                            ProductInfo prod = new ProductInfo();
                            decimal deliScaleWeight = 0;
                            string deliScaleWeightUnit = string.Empty;
                            prodUnitRefno = ProductInfoTasks.GetUnitRefnoFromBarCode(
                                barCodeParse.ProductIdentifier,
                                sysEnv.CurrentBarCodeModifier1,
                                barCodeParse.Symbology,
                                barCodeParse.DeliScaleAmount,
                                ref deliScaleWeight,
                                ref deliScaleWeightUnit,
                                ref prod);

                            if (deliScaleWeightUnit == "LB")
                            {
                                barCodeParse.NetWeightNonMetricLB = deliScaleWeight;
                                barCodeParse.NetWeightMetricKG = Convert.ToDecimal(SysCommon.ConvertLB2KG(Convert.ToDouble(barCodeParse.NetWeightNonMetricLB)));
                            }
                            else if (deliScaleWeightUnit == "KG")
                            {
                                barCodeParse.NetWeightMetricKG = deliScaleWeight;
                                barCodeParse.NetWeightNonMetricLB = Convert.ToDecimal(SysCommon.ConvertKG2LB(Convert.ToDouble(barCodeParse.NetWeightMetricKG)));
                            }
                            else
                            {
                                deliScaleWeight = 0;
                                deliScaleWeightUnit = string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            tcs.SetCanceled();

                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Identify Product", ex.Message, "OK"));
                            return await tcs.Task;
                        }
                    }

                    // If 'yes' to join bar code to a product
                    if (prodUnitRefno == 0)
                    {
                        // TODO: Resource strings
                        string msg = "Scanned product bar code is not associated to a product." + Environment.NewLine + "Do you wish to associate it now? ";

                        bool selectedYes = false;

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            // TODO: Resource strings
                            if (await Application.Current.MainPage.DisplayAlert("Activity Item", msg, "Yes", "No"))
                            {
                                selectedYes = true;
                            };

                            // Changed functionality to disallow product scanning everywhere.
                            // However, always prompt for password if not allowed to associate.
                            bool isAllowedAssocProdBarCode = sysEnv.ContactLog.IsAllowedAssociateProductBarCode;

                            if (selectedYes)
                            {
                                if (!isAllowedAssocProdBarCode)
                                {
                                    // Prompt for password
                                    FormAssociateBarCodeOverride formAssociateBarCodeOverride = new FormAssociateBarCodeOverride();

                                    await formAssociateBarCodeOverride.ShowDialog();

                                    if (formAssociateBarCodeOverride.DialogResult == DialogResult.OK)
                                    {
                                        isAllowedAssocProdBarCode = true;
                                    }
                                }

                                if (isAllowedAssocProdBarCode)
                                {
                                    bool continueAssocBarCode = false;

                                    FormDialogContainer frmSelectProduct = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, actSubType);

                                    await frmSelectProduct.DisplayControl.ShowDialog();

                                    if (frmSelectProduct.DisplayControl.DialogResult == DialogResult.OK)
                                    {
                                        continueAssocBarCode = true;
                                    }

                                    if (continueAssocBarCode)
                                    {
                                        await frmSelectProduct.SaveChanges(frmSelectProduct, new EventArgs());
                                        XDocument xmlDoc = frmSelectProduct.DisplayControlData;
                                        prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);

                                        // If a product was selected, create the xml for the bar code
                                        // the activity insert
                                        if (prodUnitRefno != 0)
                                        {
                                            xmlDoc = XDocument.Parse(
                                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                                + "<params>"
                                                + ("<ProductUnitRefno>" + prodUnitRefno.ToString() + "</ProductUnitRefno>")
                                                + ("<ProductUnitBarCodeRefno>" + prodUnitBarCodeRefno.ToString()
                                                + "</ProductUnitBarCodeRefno>")
                                                + ("<ProductIdentifier>" + barCodeParse.ProductIdentifier + "</ProductIdentifier>")
                                                + ("<ProductUnitBarCodeModifier1>" + sysEnv.CurrentBarCodeModifier1
                                                + "</ProductUnitBarCodeModifier1>")
                                                + ("<RawData>" + barCodeParse.Raw + "</RawData>")
                                                + ("<enumBarCodeSymbology>" + Convert.ToInt32(barCodeParse.Symbology))
                                                + "</enumBarCodeSymbology>"
                                                + ("<BarCodeSymbologyPrefix>" + barCodeParse.SymbologyPrefix + "</BarCodeSymbologyPrefix>")
                                                + "</params>");

                                            await actItemBarCode.Insert(xmlDoc, false);

                                            tcs.SetResult(prodUnitRefno);
                                        }
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        Exception ex = new Exception("Product not identified.");
                                        tcs.SetException(ex);
                                        //throw ex;
                                    }
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    Exception ex = new Exception("Product not identified.");
                                    tcs.SetException(ex);
                                    //throw ex;
                                }
                            }
                            else
                            {
                                tcs.SetCanceled();
                            }
                        });
                    }
                    else
                    {
                        // TODO: Resource strings
                        Exception ex = new Exception("Product not identified.");
                        tcs.SetException(ex);

                        //throw ex;
                    }
                }
            }

            return await tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barParse"></param>
        /// <param name="actSubType"></param>
        /// <param name="actItem"></param>
        /// <param name="tempItemSchemeData"></param>
        /// <param name="callingControl"></param>
        /// <param name="isInsertDetailReady"></param>
        /// <param name="actDetail"></param>
        /// <param name="isForceStopForEdit"></param>
        /// <exception cref="GatherProductDataException">Throws when a user hits the cancel button on the manual input form.</exception>
        public static async Task<bool> GatherProductData(
            BarCodeParse barParse,
            SysCommon.ActSubTypes actSubType,
            ActItem actItem,
            SActItemSchemeData tempItemSchemeData,
            //UserControl callingControl,
            bool isInsertDetailReady,
            ActDetail actDetail,
            bool isForceStopForEdit)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            isInsertDetailReady = false;

            if (!string.IsNullOrEmpty(barParse.ActDetailFKRefno) && !sysEnv.SysSettings.IsDisconnectedMode)
            {
                LocationMap locMap = LocationMap.GetInstance();
                int locMapRefno = locMap.CurrLocMapRefno;
                locMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(tempItemSchemeData.LocMapWhsFKRefno, tempItemSchemeData.LocMapSectFKRefno, tempItemSchemeData.LocMapSlotFKRefno, ref locMapRefno);
            }

            actDetail.SchemeData = GatherProductSchemeData(actDetail, barParse, tempItemSchemeData, actSubType);
            GatherProductQuantityData(ref actDetail, barParse, tempItemSchemeData, ref isForceStopForEdit);

            bool isStopForDetail = IsStopForDetail(actDetail, isForceStopForEdit);
            bool isStopForLabelDetail = false;

            if (actDetail.ActSubType == SysCommon.ActSubTypes.Receiving && actDetail.ActItem.LocMap.CurrLocMap.IsWhsWMS)
            {
                if (!actDetail.IsLabelSchemeComplete())
                {
                    isStopForLabelDetail = true;
                }
            }

            if (isStopForDetail)
            {
                bool continueIsStopForDetail = false;

                // Check for duplicate occurs in form
                // Check for quantity over occurrs in form
                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetailEdit, actDetail, actSubType);

                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    continueIsStopForDetail = true;
                }

                if (continueIsStopForDetail)
                {
                    XDocument xmlDoc = null;
                    //xmlDoc = form.DisplayControlData;
                    xmlDoc = ((IAppArea)form.DisplayControl).GetXML();
                    UpdateActDetailFromXML(ref actDetail, xmlDoc);

                    //No need to call actItem.IsReadyToApply(xmlParam) the form will call it.
                    isInsertDetailReady = true;
                }
                else
                {
                    //TODO: Resource strings
                    throw new GatherProductDataException("Cancelled from detail edit.", true);
                }
            }
            else if (isStopForLabelDetail)
            {
                bool continueIsStopForLabelDetail = false;

                // No duplicate check needed for label printing
                // No quantity over check needed for label printing
                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityProductLabel, actDetail, actSubType);

                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    continueIsStopForLabelDetail = true;
                }

                if (continueIsStopForLabelDetail)
                {
                    XDocument xmlDoc = null;
                    xmlDoc = ((IAppArea)form.DisplayControl).GetXML();
                    UpdateActDetailFromXML(ref actDetail, xmlDoc);
                }
                ////else
                ////{
                ////  throw new GatherProductDataException("Cancelled from detail edit.", true);
                ////}

                //    // No need to call actItem.IsReadyToApply(xmlParam) the form will call it.
                //    // Insert the activity detail even if the print product label was cancelled.
                //    // All inventory tracked scheme components where complete before even call the print product labels control.
                isInsertDetailReady = true;
            }
            else
            {
                actDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(actItem.ProdInfo.Code, actItem.ProdInfo.Scheme, actDetail.SchemeData);
                actDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(actItem.ProdInfo.Code, actItem.ProdInfo.Scheme, actDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                XDocument xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + ("<itm_refno>" + actItem.FKRefno + "</itm_refno>")
                    + ("<alt_qty>" + actDetail.Quantity + "</alt_qty>")
                    + ("<wt_qty>" + actDetail.WeightQuantity + "</wt_qty>")
                    + ("<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                    + "</imclient>");

                actItem.IsReadyToApply(xmlParam);

                isInsertDetailReady = true;
            }

            return isInsertDetailReady;
        }

        /// <summary>
        /// Fill an ActivityDetail object with the xml data. Data is returned from a form.
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="xmlDoc"></param>
        public static void UpdateActDetailFromXML(ref ActDetail actDetail, XDocument xmlDoc)
        {
            SProductSchemeData schemeData = new SProductSchemeData();
            schemeData = actDetail.SchemeData;
            schemeData.SingleTradeItemSerialNumber = xmlDoc.Root.Element("serial").Value;
            schemeData.LogisticUnitProductionDate = DateTime.Parse(xmlDoc.Root.Element("pdate").Value);
            schemeData.LogisticUnitBatchLotNumber = xmlDoc.Root.Element("lot_no").Value;
            schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(xmlDoc.Root.Element("bdate").Value);
            schemeData.VariantCode = xmlDoc.Root.Element("variant_code").Value;
            schemeData.VariantNumber = int.Parse(xmlDoc.Root.Element("variant_no").Value);
            actDetail.SchemeData = schemeData;

            actDetail.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value);
            actDetail.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value);
            actDetail.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value);

            actDetail.ActExtra.Text1 = xmlDoc.Root.Element("extra1").Value;
            actDetail.ActExtra.Text2 = xmlDoc.Root.Element("extra2").Value;
            actDetail.ActExtra.Text3 = xmlDoc.Root.Element("extra3").Value;
            actDetail.ActExtra.Text4 = xmlDoc.Root.Element("extra4").Value;
            actDetail.ActExtra.DateTime5 = DateTime.Parse(xmlDoc.Root.Element("extra5").Value);

            actDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                actDetail.ActItem.ProdInfo.Code,
                actDetail.ActItem.ProdInfo.Scheme,
                actDetail.SchemeData);

            actDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                actDetail.ActItem.ProdInfo.Code,
                actDetail.ActItem.ProdInfo.Scheme,
                actDetail.SchemeData);
        }

        /// <summary>
        /// Fill an ActivityDetailEditLabel object with the xml data. Data is returned from a form.
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="xmlDoc"></param>
        public static void UpdateActDetailLabelFromXML(ref ActDetailEdit actDetailEditLabel, XDocument xmlDoc)
        {
            SProductSchemeData schemeData = new SProductSchemeData();
            schemeData = actDetailEditLabel.SchemeData;
            schemeData.SingleTradeItemSerialNumber = xmlDoc.Root.Element("serial").Value;
            schemeData.LogisticUnitProductionDate = DateTime.Parse(xmlDoc.Root.Element("pdate").Value);
            schemeData.LogisticUnitBatchLotNumber = xmlDoc.Root.Element("lot_no").Value;
            schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(xmlDoc.Root.Element("bdate").Value);
            schemeData.VariantCode = xmlDoc.Root.Element("variant_code").Value;
            schemeData.VariantNumber = int.Parse(xmlDoc.Root.Element("variant_no").Value);
            actDetailEditLabel.SchemeData = schemeData;

            actDetailEditLabel.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value);
            actDetailEditLabel.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value);
            actDetailEditLabel.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value);

            actDetailEditLabel.ActExtra.Text1 = xmlDoc.Root.Element("extra1").Value;
            actDetailEditLabel.ActExtra.Text2 = xmlDoc.Root.Element("extra2").Value;
            actDetailEditLabel.ActExtra.Text3 = xmlDoc.Root.Element("extra3").Value;
            actDetailEditLabel.ActExtra.Text4 = xmlDoc.Root.Element("extra4").Value;
            actDetailEditLabel.ActExtra.DateTime5 = DateTime.Parse(xmlDoc.Root.Element("extra5").Value);

            // Do not fill in scheme id because the tracking scheme will not match the 'label scheme'
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyVersion()
        {
            int major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            int minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            int revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeParse"></param>
        public static void BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIDProduct(BarCodeParse barCodeParse)
        {
            if (barCodeParse.NetWeightMetricKG >= (decimal)11.00 && barCodeParse.NetWeightMetricKG <= (decimal)11.99)
            {
                barCodeParse.ProductIdentifier = "MC10448";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)12.00 && barCodeParse.NetWeightMetricKG <= (decimal)12.49)
            {
                barCodeParse.ProductIdentifier = "MC10443";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)12.50 && barCodeParse.NetWeightMetricKG <= (decimal)12.99)
            {
                barCodeParse.ProductIdentifier = "MC10441";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)13.00 && barCodeParse.NetWeightMetricKG <= (decimal)13.49)
            {
                barCodeParse.ProductIdentifier = "MC10442";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)13.50 && barCodeParse.NetWeightMetricKG <= (decimal)13.99)
            {
                barCodeParse.ProductIdentifier = "MC10440";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)14.00 && barCodeParse.NetWeightMetricKG <= (decimal)14.49)
            {
                barCodeParse.ProductIdentifier = "MC10445";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)14.50 && barCodeParse.NetWeightMetricKG <= (decimal)14.99)
            {
                barCodeParse.ProductIdentifier = "MC10446";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)15.00 && barCodeParse.NetWeightMetricKG <= (decimal)15.49)
            {
                barCodeParse.ProductIdentifier = "MC10447";
            }
            else if (barCodeParse.NetWeightMetricKG >= (decimal)15.50 && barCodeParse.NetWeightMetricKG <= (decimal)16.00)
            {
                barCodeParse.ProductIdentifier = "MC10450";
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("Unable to identify product scanned." + Environment.NewLine + "ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct enabled.");
            }
        }

        /// <summary>
        /// Fill the actDetail with the quantity gathered.
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="barParse"></param>
        /// <param name="tempItemSchemeData"></param>
        /// <param name="isForceStopForEdit"></param>
        private static void GatherProductQuantityData(
            ref ActDetail actDetail,
            BarCodeParse barParse,
            SActItemSchemeData tempItemSchemeData,
            ref bool isForceStopForEdit)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            // Assign scanned weight and convert if needed
            if (!(sysEnv.SysSettings.IsSetCatchWeightZero && actDetail.ActItem.ProdInfo.IsCatchWeight))
            {
                actDetail.WeightQuantity = actDetail.ActItem.ProdInfo.UnitWeight;
            }

            if (actDetail.ActItem.ProdInfo.IsCatchWeight)
            {
                if (actDetail.ActItem.ProdInfo.WeightUnitCode == "LB")
                {
                    if (barParse.NetWeightMetricKG != 0)
                    {
                        actDetail.WeightQuantity = SysCommon.ConvertKG2LB(barParse.NetWeightMetricKG);
                    }
                    else if (barParse.NetWeightNonMetricLB != 0)
                    {
                        actDetail.WeightQuantity = barParse.NetWeightNonMetricLB;
                    }
                }
                else if (actDetail.ActItem.ProdInfo.WeightUnitCode == "KG")
                {
                    if (barParse.NetWeightNonMetricLB != 0)
                    {
                        actDetail.WeightQuantity = SysCommon.ConvertLB2KG(barParse.NetWeightNonMetricLB);
                    }
                    else if (barParse.NetWeightMetricKG != 0)
                    {
                        actDetail.WeightQuantity = barParse.NetWeightMetricKG;
                    }
                }
            }

            if (tempItemSchemeData.ActDetailQuantity != 0 && actDetail.Quantity == 0)
            {
                actDetail.Quantity = tempItemSchemeData.ActDetailQuantity;
            }

            if (tempItemSchemeData.ActDetailBaseQuantity != 0 && actDetail.BaseQuantity == 0)
            {
                actDetail.BaseQuantity = tempItemSchemeData.ActDetailBaseQuantity;
            }

            if (tempItemSchemeData.ActDetailWeightQuantity != 0 && actDetail.WeightQuantity == 0)
            {
                actDetail.WeightQuantity = tempItemSchemeData.ActDetailWeightQuantity;
            }

            // XDocument xmlDoc = null;
            decimal unitQuantity = 1;

            if (sysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount &&
                barParse != null)
            {
                EvalParse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount(
                    barParse,
                    actDetail,
                    ref unitQuantity);

                // Adjust the weight if rule was applied.
                if (1 < unitQuantity)
                {
                    ////if (0 < actDetail.WeightQuantity && actDetail.ActItem.ProdInfo.IsCatchWeight)
                    ////{
                    ////  isForceStopForEdit = false;
                    ////}
                    ////else if (0 >= actDetail.WeightQuantity && actDetail.ActItem.ProdInfo.IsCatchWeight)
                    ////{
                    ////  isForceStopForEdit = true;
                    ////}
                    ////else if (!actDetail.ActItem.ProdInfo.IsCatchWeight)
                    ////{
                    ////  isForceStopForEdit = true;
                    ////  actDetail.WeightQuantity = unitQuantity * ProductInfoTasks.GetUnitRelationFactor(
                    ////      actDetail.ActItem.ProdInfo);
                    ////}

                    // See WO# 12203 Always stop and allow editing of detail.
                    isForceStopForEdit = true;
                    if (!actDetail.ActItem.ProdInfo.IsCatchWeight)
                    {
                        isForceStopForEdit = true;
                        actDetail.WeightQuantity = unitQuantity * ProductInfoTasks.GetUnitRelationFactor(
                            actDetail.ActItem.ProdInfo);
                    }
                }
            }

            // If catch count is enabled the variable count is the base quantity not the unit quantity
            if (!actDetail.ActItem.ProdInfo.IsCatchCount)
            {
                if (barParse.LogisticUnitVariableCount != 0
                    && sysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive)
                {
                    unitQuantity = barParse.LogisticUnitVariableCount;
                }
            }

            if (actDetail.ActItem.ProdInfo.BaseUnitCode == "LB" || actDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
            {
                actDetail.BaseQuantity = actDetail.WeightQuantity;
            }
            else
            {
                if (actDetail.ActItem.ProdInfo.IsCatchCount
                    && barParse.LogisticUnitVariableCount != 0
                    && sysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive)
                {
                    actDetail.BaseQuantity = barParse.LogisticUnitVariableCount;
                }
                else
                {
                    actDetail.BaseQuantity = unitQuantity * ProductInfoTasks.GetUnitRelationFactor(
                      actDetail.ActItem.ProdInfo);
                }
            }

            if (actDetail.ActItem.ProdInfo.UnitCode == "LB" || actDetail.ActItem.ProdInfo.UnitCode == "KG")
            {
                actDetail.Quantity = actDetail.WeightQuantity;
            }
            else
            {
                actDetail.Quantity = unitQuantity;
            }

            // Negate the quantity if desired
            if (actDetail.ActItem.ActHdr.ActSubType == SysCommon.ActSubTypes.Adjustment)
            {
                AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();
                if (adjSettings.NegateScanQuantity)
                {
                    actDetail.Quantity = actDetail.Quantity * -1;
                    actDetail.BaseQuantity = actDetail.BaseQuantity * -1;
                    actDetail.WeightQuantity = actDetail.WeightQuantity * -1;
                }
            }
        }

        public static bool IsNebraskaBeefRuleMet(BarCodeParse barParse)
        {
            return (barParse.GTIN.Contains("0740780")
                || barParse.GTIN.Contains("19336"))
                && !string.IsNullOrEmpty(barParse.LogisticUnitBatchLotNumber);
        }

        /// <summary>
        /// Fill a scheme data structure from the scanned barcode info that is 
        /// to be assigned to the activity detail info structure
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="barParse"></param>
        /// <param name="tempItemSchemeData"></param>
        /// <param name="actSubType"></param>
        /// <returns></returns>
        private static SProductSchemeData GatherProductSchemeData(
            ActDetail actDetail,
            BarCodeParse barParse,
            SActItemSchemeData tempItemSchemeData,
            SysCommon.ActSubTypes actSubType)
        {
            SProductSchemeData schemeData = actDetail.SchemeData;
            ProductInfo prodInfo = actDetail.ActItem.ProdInfo;
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            // Decided to implement the bar code here. Could have made it a new BarCodeParseCode_128_NebraskaBeef class.
            // The bar code was well formed except it being a non GS1 code 128.
            if (sysEnv.BarCodeSettings.IsRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128
                && IsNebraskaBeefRuleMet(barParse))
            {
                // Lot Number is '00000021724739' serial number should be '724739'
                barParse.LogisticUnitBatchLotNumber = barParse.LogisticUnitBatchLotNumber.Remove(0, 8);
                barParse.SingleTradeItemSerialNumber = string.Empty;
            }

            //<issue="13609"/>
            AssertSuppressedErrorsAgainstScheme(prodInfo, barParse);


            // Decided not to use the CurrSkid.LogisticUnitSkidCode because is is the full GS1 barcode. 18 digits.
            // Instead I am using the CurrSkid.SkidNo.
            // The SkidInfo.CurrSkid.SkidNo could have been assigned by manually selecting or creating a new skid. 
            // Or it could have be scanned and the skid info looked up.
            schemeData.LogisticUnitSkidNumber = actDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;

            if (prodInfo.Scheme.IsBestBeforeDateTracked || prodInfo.Scheme.LabelInfo.IsBestBeforeDateTracked)
            {
                if (SysCommon.IsEmptyDateTime(barParse.LogisticUnitBestBeforeSellByDate))
                {
                    if (SysCommon.IsEmptyDate(tempItemSchemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate))
                    {
                        schemeData.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
                    }
                    else
                    {
                        schemeData.LogisticUnitBestBeforeSellByDate =
                            tempItemSchemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate;
                    }
                }
                else
                {
                    schemeData.LogisticUnitBestBeforeSellByDate = barParse.LogisticUnitBestBeforeSellByDate;
                }
            }
            else
            {
                schemeData.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
            }

            // sProductSchemeData.dtActivityDetailLogisticUnitDueDate = oBarCodeParse.dtLogisticUnitDueDate

            // sProductSchemeData.dtActivityDetailLogisticUnitExpiryUseByDate = oBarCodeParse.
            // dtLogisticUnitExpiryUseByDate

            // sProductSchemeData.dtActivityDetailLogisticUnitPackagingDate = oBarCodeParse.
            //// dtLogisticUnitPackagingDate

            if (prodInfo.Scheme.IsProductionDateTracked || prodInfo.Scheme.LabelInfo.IsProductionDateTracked)
            {
                if (SysCommon.IsEmptyDateTime(barParse.LogisticUnitProductionDate))
                {
                    if (SysCommon.IsEmptyDate(tempItemSchemeData.ProdSchemeData.LogisticUnitProductionDate))
                    {
                        schemeData.LogisticUnitProductionDate = new DateTime(1900, 1, 1);
                    }
                    else
                    {
                        schemeData.LogisticUnitProductionDate = tempItemSchemeData.ProdSchemeData.LogisticUnitProductionDate;
                    }
                }
                else
                {
                    schemeData.LogisticUnitProductionDate = barParse.LogisticUnitProductionDate;
                }
            }
            else
            {
                schemeData.LogisticUnitProductionDate = new DateTime(1900, 1, 1);
            }

            if (prodInfo.Scheme.IsLotTracked || prodInfo.Scheme.LabelInfo.IsLotTracked)
            {
                if (barParse.LogisticUnitBatchLotNumber == string.Empty)
                {
                    schemeData.LogisticUnitBatchLotNumber = tempItemSchemeData.ProdSchemeData.LogisticUnitBatchLotNumber;
                }
                else
                {
                    schemeData.LogisticUnitBatchLotNumber = barParse.LogisticUnitBatchLotNumber;
                }

                // This code is expected to have a speed issue.
                // Comparing GTIN = '90000122345000' to "122345". The small string is on the right instead of the left.
                // A SQL command using '122345' LIKE %90000122345000% will of course not work.
                // Only way to find was to iterate through the table and use a 'contains' command.
                if (sysEnv.SysSettings.IsEnableDefaultDetailValues
                    && prodInfo.Scheme.IsLotTrackedDefault)
                {
                    string lotCodeDefault = GetLotCodeDefaultFromSupplierCompanyPrefix(
                        prodInfo.Scheme.SchemeRefno,
                        barParse.GTIN);

                    if (!string.IsNullOrEmpty(lotCodeDefault))
                    {
                        if (string.IsNullOrEmpty(schemeData.LogisticUnitBatchLotNumber))
                        {
                            schemeData.LogisticUnitBatchLotNumber = lotCodeDefault;
                        }
                        else
                        {
                            if (!schemeData.LogisticUnitBatchLotNumber.Contains(lotCodeDefault.Trim()))
                            {
                                schemeData.LogisticUnitBatchLotNumber = lotCodeDefault + "-"
                                    + schemeData.LogisticUnitBatchLotNumber.TrimEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                schemeData.LogisticUnitBatchLotNumber = string.Empty;
            }

            //// sProductSchemeData.strActivityDetailLogisticUnitSkidNumber = Nothing

            if (prodInfo.Scheme.IsSerialTracked || prodInfo.Scheme.LabelInfo.IsSerialTracked)
            {
                if (barParse.SingleTradeItemSerialNumber == string.Empty)
                {
                    schemeData.SingleTradeItemSerialNumber = tempItemSchemeData.ProdSchemeData.SingleTradeItemSerialNumber;
                }
                else
                {
                    schemeData.SingleTradeItemSerialNumber = barParse.SingleTradeItemSerialNumber;
                }
            }
            else
            {
                schemeData.SingleTradeItemSerialNumber = string.Empty;
            }

            if (prodInfo.Scheme.IsVariantTracked || prodInfo.Scheme.LabelInfo.IsVariantTracked)
            {
                if (barParse.LogisticUnitProductVariant == 0)
                {
                    schemeData.VariantCode = tempItemSchemeData.ProdSchemeData.VariantCode;
                    schemeData.VariantDescription = tempItemSchemeData.ProdSchemeData.VariantDescription;
                    schemeData.VariantNumber = tempItemSchemeData.ProdSchemeData.VariantNumber;
                }
                else
                {
                    schemeData.VariantNumber = barParse.LogisticUnitProductVariant;
                    schemeData = actDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(schemeData);
                }
            }
            else
            {
                schemeData.VariantCode = string.Empty;
                schemeData.VariantDescription = string.Empty;
                schemeData.VariantNumber = 0;
            }

            return schemeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="isStopForDetail"></param>
        /// <returns></returns>
        private static bool IsStopForDetail(ActDetail actDetail, bool isStopForDetail)
        {
            if (actDetail.ActItem.IsCatchAsk())// && !actDetail.ActItem.ProdInfo.IsCatchWeight) // April 14, 2016 - WO 13866 - If catch ask, stop.
            {
                isStopForDetail = true;
            }
            else if (actDetail.Quantity == 0 && actDetail.WeightQuantity == 0)
            {
                isStopForDetail = true;
            }
            else if (actDetail.ActItem.ProdInfo.IsCatchWeight && actDetail.WeightQuantity == 0)
            {
                isStopForDetail = true;
            }
            else if (actDetail.ActItem.ProdInfo.IsCatchCount && actDetail.BaseQuantity == 0)
            {
                isStopForDetail = true;
            }
            else if (!actDetail.IsSchemeComplete(actDetail.ActItem))
            {
                isStopForDetail = true;
            }
            else if (actDetail.ActItem.ProdInfo.Scheme.Extra1Type != (int)SysCommon.SchemeExtraTypes.Disabled
                && actDetail.ActItem.ProdInfo.Scheme.Extra2IsRequired)
            {
                isStopForDetail = true;
            }
            else if (actDetail.ActItem.ProdInfo.Scheme.Extra2Type != (int)SysCommon.SchemeExtraTypes.Disabled
                && actDetail.ActItem.ProdInfo.Scheme.Extra2IsRequired)
            {
                isStopForDetail = true;
            }
            else if (actDetail.ActItem.ProdInfo.Scheme.Extra3Type != (int)SysCommon.SchemeExtraTypes.Disabled
                && actDetail.ActItem.ProdInfo.Scheme.Extra3IsRequired)
            {
                isStopForDetail = true;
            }
            else if (actDetail.ActItem.ProdInfo.Scheme.Extra4Type != (int)SysCommon.SchemeExtraTypes.Disabled
                && actDetail.ActItem.ProdInfo.Scheme.Extra4IsRequired)
            {
                isStopForDetail = true;
            }
            else if (actDetail.ActItem.ProdInfo.Scheme.Extra5Type != (int)SysCommon.SchemeExtraTypes.Disabled
                && actDetail.ActItem.ProdInfo.Scheme.Extra4IsRequired)
            {
                isStopForDetail = true;
            }

            return isStopForDetail;
        }

        /// <summary>
        /// Get the lot code default value from the GTIN containing the supplier company prefix.
        /// </summary>
        /// <param name="schemeRefno"> Product Scheme Refno</param>
        /// <param name="gtin"> Bar Code GTIN</param>
        /// <returns>Lot Code Default.</returns>
        private static string GetLotCodeDefaultFromSupplierCompanyPrefix(int schemeRefno, string gtin)
        {
            string lotCodeDefault = string.Empty;

            if (!string.IsNullOrEmpty(gtin))
            {
                using (SQLiteCommand cmndSchemeLot = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndSchemeLot.CommandType = CommandType.Text;

                    // Order by descending, we want to examine the longest string to compare for a match first.
                    cmndSchemeLot.CommandText = "SELECT tProductSchemeLotNumber.ProductSchemeLotNumberCode,"
                        + "tProductSchemeLotNumber.ProductSchemeLotNumberCodeDefault "
                        + "FROM tProductSchemeLotNumberXRef "
                        + "INNER JOIN tProductSchemeLotNumber ON tProductSchemeLotNumber.ProductSchemeLotNumberRefno = "
                        + "tProductSchemeLotNumberXRef.ProductSchemeLotNumberRefno "
                        + string.Format("WHERE tProductSchemeLotNumberXRef.ProductSchemeRefno = {0}", schemeRefno)
                        + "ORDER BY ProductSchemeLotNumberCode DESC";

                    using (SQLiteDataReader dr = cmndSchemeLot.ExecuteReader())
                    {
                        string comp = string.Empty;
                        while (dr.Read())
                        {
                            if (gtin.Contains(Convert.ToString(dr["ProductSchemeLotNumberCode"]).TrimEnd()))
                            {
                                lotCodeDefault = Convert.ToString(dr["ProductSchemeLotNumberCodeDefault"]).TrimEnd();
                                break;
                            }
                        }
                    }
                }
            }

            return lotCodeDefault;
        }

        /// <summary>
        /// <para>Checks to see if we have to throw any suppressed errors from Bar Code parsing.</para>
        /// <para>
        /// Rule Enabled
        /// If an error is tracked in the scheme, throw first error from a tracked field.
        /// </para>
        /// <para>
        /// Rule Disabled
        /// If there are errors, throw the first error.
        /// </para>
        /// <para>
        /// <see cref="SysBarCodeSettings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents"/>
        /// </para>
        /// <seealso cref="BarCodeParseUcc_EAN_128_Symbol.ParseBarCodeData"/>
        /// </summary>
        /// <remarks>
        /// issue="13609"
        /// author="RD"
        /// date="2016/02/19"
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="parse"></param>
        /// <exception cref="BarCodeParseException"/>
        public static void AssertSuppressedErrorsAgainstScheme(ProductInfo info, BarCodeParse parse)
        {
            // Currently only UCC EAN 128 will create these errors
            if (!parse.Details.HasErrors)
            {
                // no errors!
                return;
            }

            // UCC EAN 128
            // Suppress Rule Enabled
            if (parse.Symbology == SysCommon.BarCodeSymbologies.UCC_EAN_128_Symbol &&
                parse.Settings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents)
            {
                ProdScheme scheme;
                if (info != null)
                    scheme = info.Scheme;
                else
                    scheme = new ProdScheme();

                // Check errors against scheme
                BarCodeParseDetails details = parse.Details;
                int count = details.ErrorCount;
                for (int i = 0; i < count; ++i)
                {
                    BarCodeExceptionReason t = details.GetParseExceptionReasonAt(i);
                    // Default/unhandled will be thrown
                    switch (t)
                    {
                        case BarCodeExceptionReason.ActivityDetailFKRefno_InvalidFormat:
                        case BarCodeExceptionReason.ActivityFKRefno_InvalidFormat:
                        case BarCodeExceptionReason.ActivityItemFKRefno_InvalidFormat:
                        case BarCodeExceptionReason.CountOfTradeItems_InvalidInteger:
                        case BarCodeExceptionReason.DimensionMeters_InvalidDecimal:
                        case BarCodeExceptionReason.DimensionMeters_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.DueDate_InvalidDate:
                        case BarCodeExceptionReason.ExpiryUseByDate_InvalidDate:
                        case BarCodeExceptionReason.GrossWeightMetricKG_InvalidDecimal:
                        case BarCodeExceptionReason.GrossWeightMetricKG_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.GrossWeightNonMetricLB_InvalidDecimal:
                        case BarCodeExceptionReason.GrossWeightNonMetricLB_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.NetWeightMetricKG_InvalidDecimal:
                        case BarCodeExceptionReason.NetWeightMetricKG_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.NetWeightNonMetricLB_InvalidDecimal:
                        case BarCodeExceptionReason.NetWeightNonMetricLB_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.PackagingDate_InvalidDate:
                        case BarCodeExceptionReason.VariableCount_InvalidInteger:
                            break;

                        case BarCodeExceptionReason.BatchLotNumber_InvalidFormat:
                            if (scheme.IsLotTracked)
                                details.ThrowOnlyOneError(i);
                            break;
                        case BarCodeExceptionReason.BestBeforeSellByDate_InvalidDate:
                            if (scheme.IsBestBeforeDateTracked)
                                details.ThrowOnlyOneError(i);
                            break;

                        case BarCodeExceptionReason.PackagingDateAsProductionDate_InvalidDate:
                        case BarCodeExceptionReason.ProductionDate_InvalidDate:
                            if (scheme.IsProductionDateTracked)
                                details.ThrowOnlyOneError(i);
                            break;
                        case BarCodeExceptionReason.ProductVariant_InvalidInteger:
                            if (scheme.IsVariantTracked)
                                details.ThrowOnlyOneError(i);
                            break;
                        case BarCodeExceptionReason.SerialNumber_InvalidFormat:
                        case BarCodeExceptionReason.SingleTradeItemSerialNumber:
                            if (scheme.IsSerialTracked)
                                details.ThrowOnlyOneError(i);
                            break;

                        default: // Assume default will be thrown
                        case BarCodeExceptionReason.InvalidFormat:
                            // Unknown error, throw
                            details.ThrowOnlyOneError(i);
                            break;
                    }
                }

            }
            else
            {
                // Suppress rule not enabled and/or not UCC EAN 128
                // Errors exist.
                // Throw first error
                parse.Details.ThrowOnlyOneError(0);
            }
        }

    }
}
