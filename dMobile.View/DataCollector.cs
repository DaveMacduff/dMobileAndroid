// <copyright file="DataCollector.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.BarCodeScanner;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.Bedrock;
    using dMobile.Bedrock.Settings;
    using dMobile.Controller.Activity;
    using dMobile.Controller.Activity_Detail;
    using dMobile.Controller.Activity_Item;
    using dMobile.Resources.Enums;
    using dMobile.View.Forms;
    using Xamarin.Forms;

    /// <summary>
    /// Class to collect data from various sources, bar codes or other input methods.
    /// </summary>
    public class DataCollector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollector"/> class.
        /// </summary>
        /// <param name="actSubType">The activity subtype.</param>
        /// <param name="actItem"><see cref="ActItem" /> Activity item instance.</param>
        public DataCollector(SysCommon.ActSubTypes actSubType, ActItem actItem)
        {
            this.ActBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
            this.ActItemBarCode = ActItemFacade.GetActItem(this.ActBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            ActSubType = actSubType;
            ActItem = actItem;
            ActDetail = null;
        }

        private static ActDetail ActDetail { get; set; }

        private static SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        private static SQLite.SQLiteAsyncConnection Conn { get; } = DatabaseConnection.GetInstance().SqlConn;

        private static LocationMap LocMap { get; } = LocationMap.GetInstance();

        private static ActItem ActItem { get; set; }

        private static SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.BarCodeAssociation;

        private static BarCodeParse BarCodeParse { get; set; }

        private Controller.Activity.Activity ActBarCode { get; }

        private ActItem ActItemBarCode { get; }

        private ProductUnitBarCodeModel ProdUnitBarCode { get; set; } = new ProductUnitBarCodeModel();

        /// <summary>
        ///     Gather activity detail data from the scanned bar code.
        /// </summary>
        /// <param name="barParse"><see cref="BarCodeParse" /> Bar code parse scan information or null.</param>
        /// <param name="defaultItemSchemeData"><see cref="ActItemSchemeData" /> Scheme data.</param>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail instance to fill.</param>
        /// <param name="isForceStopForEditing"> Flag to force stopping at the edit screen.</param>
        /// <returns>True if ready to insert detail else false.</returns>
        /// <remarks>This can be invoked also from an add detail button event with no bar code data.</remarks>
        public static async Task GatherProductData(
            BarCodeParse barParse,
            ActItemSchemeData defaultItemSchemeData,
            ActDetail actDetail,
            bool isForceStopForEditing = false)
        {
            ActDetail = actDetail;
            BarCodeParse = barParse;

            // Can't remember why this code is needed.
            if (!string.IsNullOrEmpty(BarCodeParse?.ActDetailFkRefno) && !SysEnv.SysSettings.IsDisconnectedMode)
            {
                defaultItemSchemeData = new ActItemSchemeData();
                var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                    defaultItemSchemeData.LocMapWhsFkRefno,
                    defaultItemSchemeData.LocMapSectFkRefno,
                    defaultItemSchemeData.LocMapSlotFkRefno);
                LocMap.CurrLocMap = sLocMapAndRefno.SLocMap;
            }

            var defaultLabelSchemeDataAssigned = GatherSchemeData(defaultItemSchemeData);
            var actDetailAndStopForDetail = await GatherProductQuantityData(defaultItemSchemeData);

            // isForceStopForEditing may be set to true from the calling method. Such as from an add detail button event.
            if (isForceStopForEditing)
            {
                actDetailAndStopForDetail.IsForceStopForEdit = true;
            }
            else
            {
                actDetailAndStopForDetail.IsForceStopForEdit = IsStopForDetail(
                    actDetailAndStopForDetail,
                    defaultLabelSchemeDataAssigned);
            }

            var isStopForLabelDetail = false;
            if ((ActSubType == SysCommon.ActSubTypes.Receiving ||
                    ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                    ActSubType == SysCommon.ActSubTypes.TransferTo) &&
                    ActItem.LocMap.IsWhsWms)
            {
                if (!ActDetail.IsLabelSchemeComplete())
                {
                    isStopForLabelDetail = true;
                }
                else if (defaultLabelSchemeDataAssigned)
                {
                    // If the label scheme data was assigned from the default item scheme data
                    // then we do not need to stop for label printing.
                    ////isStopForLabelDetail = true;
                    // actDetailAndStopForDetail.IsForceStopForEdit may be set to true because of an invalid
                    // production date for example. At some point we need to update the actDetail.LabelSchemeData
                    // with a new valid production date from the edit screen.
                }
            }

            if (actDetailAndStopForDetail.IsForceStopForEdit)
            {
                var continueIsStopForDetail = false;

                // Check for duplicate occurs in form
                // Check for quantity over occurs in form
                // Check for label printing occurs in form
                var form = new FormDialogContainer(EDialogTypes.ActivityDetailEdit, ActDetail, ActSubType);
                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    continueIsStopForDetail = true;
                }

                if (!continueIsStopForDetail)
                {
                    throw new GatherProductDataException("Cancelled from detail edit.", true);
                }

                var xmlDoc = await ((IAppArea)form.DisplayControl).GetXml();
                UpdateActDetailFromXml(xmlDoc);
            }
            else if (isStopForLabelDetail)
            {
                var continueIsStopForLabelDetail = false;

                // No duplicate check needed for label printing
                // No quantity over check needed for label printing
                var form = new FormDialogContainer(
                    EDialogTypes.ActivityProductLabel,
                    ActDetail,
                    ActSubType);

                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    continueIsStopForLabelDetail = true;
                }

                if (continueIsStopForLabelDetail)
                {
                    var xmlDoc = await ((IAppArea)form.DisplayControl).GetXml();
                    UpdateActDetailFromXml(xmlDoc);
                }
            }
            else
            {
                ActDetail.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                    ActItem.ProdInfo.Code,
                    ActItem.ProdInfo.Scheme,
                    ActDetail.SchemeData);
                ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                    ActItem.ProdInfo.Code,
                    ActItem.ProdInfo.Scheme,
                    ActDetail.SchemeData);

                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<itm_refno>" + ActItem.ItemFkRefno + "</itm_refno>"
                    + "<alt_qty>" + ActDetail.Quantity + "</alt_qty>"
                    + "<wt_qty>" + ActDetail.WeightQuantity + "</wt_qty>"
                    + "<ActivityDetailUniqueSchemeID>" + ActDetail.UniqueSchemeId + "</ActivityDetailUniqueSchemeID>"
                    + "</imclient>");

                await ActItem.IsReadyToApply(xmlParam);

                if (defaultLabelSchemeDataAssigned &&
                    (ActSubType == SysCommon.ActSubTypes.Receiving ||
                    ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                    ActSubType == SysCommon.ActSubTypes.TransferTo) &&
                    ActItem.LocMap.IsWhsWms)
                {
                    var sendReq = await ActDetail.CreateSubmitProductLabelXml(
                        await SysShared.GetXmlFromActivityDetailLabel(ActDetail));
                    _ = SysCommon.SubmitComm(sendReq);
                }
            }
        }

        /// <summary>
        ///     Identify the product scanned by bar code.
        /// </summary>
        /// <param name="barCodeParse">Captured scanned data.</param>
        /// <returns>The <see cref="int" />Product unit reference number.</returns>
        public async Task<int> IdentifyProductScanned(BarCodeParse barCodeParse)
        {
            barCodeParse = barCodeParse ?? throw new ArgumentNullException(nameof(barCodeParse));
            BarCodeParse = barCodeParse;
            this.ProdUnitBarCode = new ProductUnitBarCodeModel();
            await this.SumwaFarmWholeDuckRule();
            await this.SearchBarCodesForMatch();
            await this.SearchDeliBarCodeMatch();
            await this.AssociateNewBarCode();

            return this.ProdUnitBarCode.ProductUnitRefno;
        }

        private static bool GatherSchemeData(ActItemSchemeData defaultItemSchemeData)
        {
            var parseSchemeData = new ParseSchemeData(
                ActItem.ProdInfo.Scheme,
                BarCodeParse,
                defaultItemSchemeData,
                ActItem.ActHdr.SkidInfo.CurrSkid);

            var defaultSchemeDataAssigned = false;
            ActDetail.SchemeData = parseSchemeData.GatherSchemeData(
                ActDetail.SchemeData,
                ref defaultSchemeDataAssigned);

            var defaultLabelSchemeDataAssigned = false;
            if (ActItem.LocMap.IsWhsWms &&
                (ActSubType == SysCommon.ActSubTypes.Receiving || ActSubType == SysCommon.ActSubTypes.TransferTo))
            {
                // defaultLabelSchemeDataAssigned is assigned to true if the label scheme data is assigned
                var labelSchemeData = new ParseLabelSchemeData(
                    ActItem.ProdInfo.Scheme,
                    BarCodeParse,
                    defaultItemSchemeData,
                    ActItem.ActHdr.SkidInfo.CurrSkid);
                ActDetail.LabelSchemeData = labelSchemeData.GatherLabelSchemeData(
                    ActDetail.SchemeData,
                    ref defaultLabelSchemeDataAssigned);
            }

            return defaultLabelSchemeDataAssigned;
        }

        private static void UpdateActDetailFromXml(XDocument xmlDoc)
        {
            var schemeData = ActDetail.SchemeData;
            if (xmlDoc.Root != null)
            {
                schemeData.SingleTradeItemSerialNumber = xmlDoc.Root.Element("serial")?.Value;
                schemeData.LogisticUnitProductionDate = DateTime.Parse(xmlDoc.Root.Element("pdate")?.Value);
                schemeData.LogisticUnitBatchLotNumber = xmlDoc.Root.Element("lot_no")?.Value;
                schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(xmlDoc.Root.Element("bdate")?.Value);
                schemeData.VariantCode = xmlDoc.Root.Element("variant_code")?.Value;
                schemeData.VariantNumber = int.Parse(xmlDoc.Root.Element("variant_no")?.Value ?? "0");
                ActDetail.SchemeData = schemeData;

                ActDetail.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty")?.Value ?? "0");
                ActDetail.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty")?.Value ?? "0");
                ActDetail.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty")?.Value ?? "0");

                ActDetail.ActExtra.Text1 = xmlDoc.Root.Element("extra1")?.Value;
                ActDetail.ActExtra.Text2 = xmlDoc.Root.Element("extra2")?.Value;
                ActDetail.ActExtra.Text3 = xmlDoc.Root.Element("extra3")?.Value;
                ActDetail.ActExtra.Text4 = xmlDoc.Root.Element("extra4")?.Value;
                ActDetail.ActExtra.DateTime1 = DateTime.Parse(xmlDoc.Root.Element("extra5")?.Value);
            }

            ActDetail.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                ActItem.ProdInfo.Code,
                ActItem.ProdInfo.Scheme,
                ActDetail.SchemeData);

            ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                ActItem.ProdInfo.Code,
                ActItem.ProdInfo.Scheme,
                ActDetail.SchemeData);
        }

        private static async Task<ActDetailAndStopForDetail> GatherProductQuantityData(
            ActItemSchemeData defaultItemSchemeData)
        {
            var prodInfo = ActItem.ProdInfo;
            var barCodeRuleForcedEdit = false;

            if (ActItem.IsCatchAskZero())
            {
                ActDetail.Quantity = 0;
                ActDetail.BaseQuantity = 0;
                ActDetail.WeightQuantity = 0;
            }
            else
            {
                GatherWeightQuantity();
                GatherQuantityFromItem();
                await GatherUnitQuantity();
                NegateAdjustmentQuantity();
            }

            // Fill activity detail and stop for detail.
            return new ActDetailAndStopForDetail
            {
                ActDetail = ActDetail,
                IsForceStopForEdit = barCodeRuleForcedEdit,
            };

            void GatherWeightQuantity()
            {
                // Assign scanned weight and convert if needed
                if ((SysEnv.SysSettings.IsSetCatchWeightZero && prodInfo.IsCatchWeight) || BarCodeParse == null)
                {
                    barCodeRuleForcedEdit = true;
                }
                else
                {
                    decimal scanSysWeight = 0;
                    switch (prodInfo.WeightUnitCode)
                    {
                        case "LB" when BarCodeParse.NetWeightMetricKg != 0:
                            scanSysWeight = SysCommon.ConvertKg2Lb(BarCodeParse.NetWeightMetricKg);
                            break;

                        case "LB":
                        {
                            if (BarCodeParse.NetWeightNonMetricLb != 0)
                            {
                                scanSysWeight = BarCodeParse.NetWeightNonMetricLb;
                            }

                            break;
                        }

                        case "KG" when BarCodeParse.NetWeightNonMetricLb != 0:
                            scanSysWeight = SysCommon.ConvertLb2Kg(BarCodeParse.NetWeightNonMetricLb);
                            break;

                        case "KG":
                        {
                            if (BarCodeParse.NetWeightMetricKg != 0)
                            {
                                scanSysWeight = BarCodeParse.NetWeightMetricKg;
                            }

                            break;
                        }
                    }

                    // Calculate the weight quantity for non-catch weight products
                    var quantity = Convert.ToInt32(scanSysWeight / prodInfo.UnitGrossWeight);
                    ActDetail.WeightQuantity = prodInfo.UnitGrossWeight * (quantity == 0 ? 1 : quantity);
                }

                if (prodInfo.IsCatchWeight && BarCodeParse != null)
                {
                    switch (prodInfo.WeightUnitCode)
                    {
                        case "LB" when BarCodeParse.NetWeightMetricKg != 0:
                            ActDetail.WeightQuantity = SysCommon.ConvertKg2Lb(BarCodeParse.NetWeightMetricKg);
                            break;

                        case "LB":
                            {
                                if (BarCodeParse.NetWeightNonMetricLb != 0)
                                {
                                    ActDetail.WeightQuantity = BarCodeParse.NetWeightNonMetricLb;
                                }

                                break;
                            }

                        case "KG" when BarCodeParse.NetWeightNonMetricLb != 0:
                            ActDetail.WeightQuantity = SysCommon.ConvertLb2Kg(BarCodeParse.NetWeightNonMetricLb);
                            break;

                        case "KG":
                            {
                                if (BarCodeParse.NetWeightMetricKg != 0)
                                {
                                    ActDetail.WeightQuantity = BarCodeParse.NetWeightMetricKg;
                                }

                                break;
                            }
                    }
                }
            }

            static void NegateAdjustmentQuantity()
            {
                // Negate the quantity if desired
                if (ActItem.ActHdr.ActSubType == SysCommon.ActSubTypes.Adjustment)
                {
                    var adjSettings = new AdjustmentSettings().LoadData();
                    if (adjSettings.NegateScanQuantity)
                    {
                        ActDetail.Quantity *= -1;
                        ActDetail.BaseQuantity *= -1;
                        ActDetail.WeightQuantity *= -1;
                    }
                }
            }

            async Task GatherUnitQuantity()
            {
                decimal unitQuantity = 1;

                // Get the unit quantity, and set the weight quantity if the bar code rule for AI21 box code is applied.
                unitQuantity = await ApplyBarCodeRuleAi21BoxCode(unitQuantity);
                await GatherBaseQuantity(unitQuantity);

                if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    ActDetail.Quantity = ActDetail.WeightQuantity;
                }
                else
                {
                    if (!prodInfo.IsCatchCount &&
                        BarCodeParse != null && BarCodeParse.LogisticUnitVariableCount != 0)
                    {
                        ActDetail.Quantity = unitQuantity;
                    }
                    else
                    {
                        if (!prodInfo.IsCatchWeight &&
                            !SysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount)
                        {
                            var quantity = ActDetail.WeightQuantity / prodInfo.UnitGrossWeight;
                            if (quantity > 1)
                            {
                                unitQuantity = quantity;
                            }
                        }

                        ActDetail.Quantity = unitQuantity;
                    }
                }

                return;

                async Task<decimal> ApplyBarCodeRuleAi21BoxCode(decimal unitQty)
                {
                    if (SysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount)
                    {
                        unitQty = EvalParse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount(unitQty);

                        // Adjust the weight if rule was applied.
                        if (unitQty > 1)
                        {
                            // See WO# 12203 Always stop and allow editing of detail.
                            barCodeRuleForcedEdit = true;
                            if (!prodInfo.IsCatchWeight)
                            {
                                ActDetail.WeightQuantity =
                                    unitQty * await ProductInfoTasks.GetUnitRelationFactor(prodInfo);
                            }
                        }
                    }

                    return unitQty;
                }

                async Task GatherBaseQuantity(decimal unitQty)
                {
                    if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                    {
                        ActDetail.BaseQuantity = ActDetail.WeightQuantity;
                    }
                    else
                    {
                        if (prodInfo.IsCatchCount &&
                            BarCodeParse != null && BarCodeParse.LogisticUnitVariableCount != 0)
                        {
                            ActDetail.BaseQuantity = BarCodeParse.LogisticUnitVariableCount;
                        }
                        else
                        {
                            if (!prodInfo.IsCatchWeight &&
                                !SysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount)
                            {
                                var quantity = ActDetail.WeightQuantity / prodInfo.UnitGrossWeight;
                                if (quantity > 1)
                                {
                                    unitQty = quantity;
                                }
                            }

                            ActDetail.BaseQuantity = unitQty * await ProductInfoTasks.GetUnitRelationFactor(prodInfo);
                        }
                    }
                }
            }

            void GatherQuantityFromItem()
            {
                if (defaultItemSchemeData.ActDetailQuantity != 0 && ActDetail.Quantity == 0)
                {
                    ActDetail.Quantity = defaultItemSchemeData.ActDetailQuantity;
                }

                if (defaultItemSchemeData.ActDetailBaseQuantity != 0 && ActDetail.BaseQuantity == 0)
                {
                    ActDetail.BaseQuantity = defaultItemSchemeData.ActDetailBaseQuantity;
                }

                if (defaultItemSchemeData.ActDetailWeightQuantity != 0 && ActDetail.WeightQuantity == 0)
                {
                    ActDetail.WeightQuantity = defaultItemSchemeData.ActDetailWeightQuantity;
                }
            }
        }

        /// <summary>
        ///     Parse Nossack box count from the AI21 serial number and weight amount.
        /// </summary>
        /// <param name="unitQty">.</param>
        /// <remarks>
        /// Only Nossack usings this rule. But removed the check for Nossack's GTIN Company Prefix.
        /// Magic number 4 times the average weight to determine if the code is a skid label.
        /// 2 = For the first digit past the 01 in the AI(21).
        /// </remarks>
        private static decimal EvalParse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount(decimal unitQty)
        {
            // Removed check for Nossack's GTIN Company Prefix WO#11478
            // Nossack's GTIN Company Prefix
            // if (BarCodeParse.GTIN.Contains("67156"))
            if (BarCodeParse != null)
            {
                var weight = ActDetail.WeightQuantity;
                if (ActItem.ProdInfo.WeightUnitCode == "LB")
                {
                    if (BarCodeParse.NetWeightMetricKg > 0)
                    {
                        weight = Convert.ToDecimal(SysCommon.ConvertKg2Lb(Convert.ToDouble(BarCodeParse.NetWeightMetricKg)));
                    }
                    else if (BarCodeParse.NetWeightNonMetricLb > 0)
                    {
                        weight = BarCodeParse.NetWeightNonMetricLb;
                    }
                }
                else if (ActItem.ProdInfo.WeightUnitCode == "KG")
                {
                    if (BarCodeParse.NetWeightNonMetricLb > 0)
                    {
                        weight = Convert.ToDecimal(SysCommon.ConvertLb2Kg(Convert.ToDouble(BarCodeParse.NetWeightNonMetricLb)));
                    }
                    else if (BarCodeParse.NetWeightMetricKg > 0)
                    {
                        weight = BarCodeParse.NetWeightMetricKg;
                    }
                }

                // Magic number 4 times the average weight to determine if the code is a skid label.
                if (BarCodeParse.SingleTradeItemSerialNumber.Length > 1 && weight > ActItem.ProdInfo.UnitGrossWeight * 4)
                {
                    // 2 = For the first digit past the 01 in the AI(21).
                    unitQty = decimal.Parse(BarCodeParse.SingleTradeItemSerialNumber.Substring(2));
                }
            }

            return unitQty;
        }

        private static bool IsStopForDetail(
            ActDetailAndStopForDetail actDetailAndStopForDetail,
            bool defaultSchemeDataAssigned)
        {
            var isStopForDetail = actDetailAndStopForDetail.IsForceStopForEdit;
            if (!isStopForDetail)
            {
                if (actDetailAndStopForDetail.ActDetail.Quantity == 0 ||
                    actDetailAndStopForDetail.ActDetail.BaseQuantity == 0 ||
                    actDetailAndStopForDetail.ActDetail.WeightQuantity == 0)
                {
                    isStopForDetail = true;
                }
                else
                {
                    if (ActDetail.IsSchemeComplete(ActItem))
                    {
                        if (ActItem.IsCatchAsk())
                        {
                            isStopForDetail = true;
                        }
                        else if (defaultSchemeDataAssigned)
                        {
                            ////isStopForDetail = true;
                        }
                    }
                    else
                    {
                        isStopForDetail = true;
                    }
                }
            }

            if (!isStopForDetail && ActSubType == SysCommon.ActSubTypes.Receiving && ActItem.ProdInfo.ProductAcceptDays > 0)
            {
                if (ActItem.ProdInfo.Scheme.IsProductionDateTracked &&
                    ActDetail.SchemeData.LogisticUnitProductionDate > new DateTime(1900, 1, 1))
                {
                    if (ActDetail.SchemeData.LogisticUnitProductionDate.AddDays(
                        ActItem.ProdInfo.ProductAcceptDays) < DateTime.Now.Date)
                    {
                        isStopForDetail = true;
                    }
                }

                if (ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                    ActDetail.SchemeData.LogisticUnitBestBeforeSellByDate > new DateTime(1900, 1, 1))
                {
                    if (ActDetail.SchemeData.LogisticUnitBestBeforeSellByDate >
                        DateTime.Now.AddDays(ActItem.ProdInfo.ProductAcceptDays))
                    {
                        isStopForDetail = true;
                    }
                }
            }

            if (!isStopForDetail)
            {
                var scheme = ActItem.ProdInfo.Scheme;
                if (scheme.Extra1Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) &&
                    scheme.Extra1IsRequired)
                {
                    isStopForDetail = true;
                }
                else if (scheme.Extra2Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) &&
                    scheme.Extra2IsRequired)
                {
                    isStopForDetail = true;
                }
                else if (scheme.Extra3Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) &&
                    scheme.Extra3IsRequired)
                {
                    isStopForDetail = true;
                }
                else if (scheme.Extra4Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) &&
                    scheme.Extra4IsRequired)
                {
                    isStopForDetail = true;
                }
                else if (scheme.Extra5Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) &&
                    scheme.Extra5IsRequired)
                {
                    isStopForDetail = true;
                }
            }

            return isStopForDetail;
        }

        private async Task SumwaFarmWholeDuckRule()
        {
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 && this.ProdUnitBarCode.ProductUnitRefno == 0)
            {
                // Attempt to find the product in local database.
                // Do not look up GTIN in bar code database for this rule, product will be identified by a range
                // of fixed weights.
                if (SysEnv.BarCodeSettings.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct &&
                    BarCodeParse.ProductIdentifier == "90695955110022")
                {
                    BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct();
                    var prodInfo = await ProductInfoTasks.GetInfoFromCodeBcUnit(
                        BarCodeParse.ProductIdentifier,
                        string.Empty,
                        "CS");
                    this.ProdUnitBarCode.ProductUnitBarCodeRefno = 0;
                    this.ProdUnitBarCode.ProductUnitRefno = prodInfo.UnitRefno;
                }
            }

            return;

            static void BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct()
            {
                if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(11.00) &&
                    BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(11.99))
                {
                    BarCodeParse.ProductIdentifier = "MC10448";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.00) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.49))
                {
                    BarCodeParse.ProductIdentifier = "MC10443";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.50) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.99))
                {
                    BarCodeParse.ProductIdentifier = "MC10441";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.00) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.49))
                {
                    BarCodeParse.ProductIdentifier = "MC10442";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.50) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.99))
                {
                    BarCodeParse.ProductIdentifier = "MC10440";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.00) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.49))
                {
                    BarCodeParse.ProductIdentifier = "MC10445";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.50) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.99))
                {
                    BarCodeParse.ProductIdentifier = "MC10446";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.00) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(15.49))
                {
                    BarCodeParse.ProductIdentifier = "MC10447";
                }
                else if (BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.50) &&
                         BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(16.00))
                {
                    BarCodeParse.ProductIdentifier = "MC10450";
                }
                else
                {
                    throw new Exception(
                        "Unable to identify product scanned." + Environment.NewLine +
                        "ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct enabled.");
                }
            }
        }

        private async Task SearchBarCodesForMatch()
        {
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 &&
                this.ProdUnitBarCode.ProductUnitRefno == 0)
            {
                const string query = @"
                        SELECT tProductUnitBarCode.ProductUnitBarCodeRefno,
                            tProductUnitBarCode.ProductUnitRefno
                        FROM tProductUnitBarCode
                        WHERE ProductUnitBarCodeCode = ?
                            AND ProductUnitBarCodeModifier1 = ?";

                var results = await Conn.QueryAsync<ProductUnitBarCodeModel>(
                        query,
                        BarCodeParse.ProductIdentifier,
                        SysEnv.CurrentBarCodeModifier1);

                if (results?.Count > 0)
                {
                    this.ProdUnitBarCode.ProductUnitBarCodeRefno = Convert.ToInt32(results[0].ProductUnitBarCodeRefno);
                    this.ProdUnitBarCode.ProductUnitRefno = Convert.ToInt32(results[0].ProductUnitRefno);
                }
            }
        }

        private async Task SearchDeliBarCodeMatch()
        {
            // If product was not found in the database ask the comm server if it can identify it.
            // Deli PLU's will be handled by the comm server also.
            // <issue="13440" date="2016/02/18" author="RD"> - Added the condition fo
            // prodUnitRefno == 0 to fix Sumwa farms scanning issue where it was bringing in the
            // avg weight instead of barcode parsed weight.
            // This prevents the found prodInfo from being overwritten.</issue>
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 && this.ProdUnitBarCode.ProductUnitRefno == 0 &&
                !SysEnv.SysSettings.IsDisconnectedMode)
            {
                var unitRefnoFromBarCode = await BarCodeCommon.GetDeliProdInfoFromBarCode(
                    BarCodeParse.ProductIdentifier,
                    SysEnv.CurrentBarCodeModifier1,
                    BarCodeParse.Symbology,
                    BarCodeParse.DeliScaleAmount);

                this.ProdUnitBarCode.ProductUnitRefno = unitRefnoFromBarCode.ProdInfo.UnitRefno;
                var deliScaleWeight = unitRefnoFromBarCode.DeliScaleWeight;
                var deliScaleWeightUnit = unitRefnoFromBarCode.DeliScaleWeightUnit;

                switch (deliScaleWeightUnit)
                {
                    case "LB":
                        BarCodeParse.NetWeightNonMetricLb = deliScaleWeight;
                        BarCodeParse.NetWeightMetricKg = Convert.ToDecimal(
                            SysCommon.ConvertLb2Kg(Convert.ToDouble(BarCodeParse.NetWeightNonMetricLb)));
                        break;

                    case "KG":
                        BarCodeParse.NetWeightMetricKg = deliScaleWeight;
                        BarCodeParse.NetWeightNonMetricLb = Convert.ToDecimal(
                            SysCommon.ConvertKg2Lb(Convert.ToDouble(BarCodeParse.NetWeightMetricKg)));
                        break;

                    default:
                        deliScaleWeight = 0;
                        deliScaleWeightUnit = string.Empty;
                        break;
                }
            }
        }

        private async Task AssociateNewBarCode()
        {
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 && this.ProdUnitBarCode.ProductUnitRefno == 0)
            {
                AskToAssociateBarCode();
                if (!await CheckPermissionToAssociateBarCode())
                {
                    throw new MessageOnlyException("New bar code product association is not enabled.");
                }

                var frmSelectProduct = new FormDialogContainer(EDialogTypes.SelectProductUnit, ActSubType);
                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await frmSelectProduct.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    throw new MessageOnlyException("Product not identified.");
                }

                var xmlDoc = await frmSelectProduct.GetXml();
                if (!(xmlDoc is { Root: { } }))
                {
                    throw new MessageOnlyException("Product unit was not identified.");
                }

                this.ProdUnitBarCode.ProductUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno")?.Value!);

                // If a product was selected, create the xml for the bar code
                // the activity insert
                if (this.ProdUnitBarCode.ProductUnitRefno != 0)
                {
                    xmlDoc = await SaveNewAssociation();
                }
            }

            return;

            async static void AskToAssociateBarCode()
            {
                var msg = "Scanned product bar code is not associated to a product." +
                            Environment.NewLine + "Do you wish to associate it now? ";

                if (!await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            "Activity Item",
                            msg,
                            "Yes",
                            "No")))
                {
                    throw new TaskCanceledException();
                }
            }

            async Task<bool> CheckPermissionToAssociateBarCode()
            {
                // Changed functionality to disallow product scanning everywhere.
                // However, always prompt for password if not allowed to associate.
                var isAllowedAssocProdBarCode = SysEnv.ContactLog.IsAllowedAssociateProductBarCode;
                if (!isAllowedAssocProdBarCode)
                {
                    // Prompt for password
                    var formAssociateBarCodeOverride = new FormAssociateBarCodeOverride();
                    _ = await formAssociateBarCodeOverride.ShowDialog();
                    if (formAssociateBarCodeOverride.DialogResult == DialogResult.OK)
                    {
                        isAllowedAssocProdBarCode = true;
                    }
                }

                return isAllowedAssocProdBarCode;
            }

            async Task<XDocument> SaveNewAssociation()
            {
                var sb = new StringBuilder(192);
                using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = false, Indent = false }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("params");
                    writer.WriteElementString("ProductUnitRefno", this.ProdUnitBarCode.ProductUnitRefno.ToString());
                    writer.WriteElementString("ProductUnitBarCodeRefno", this.ProdUnitBarCode.ProductUnitBarCodeRefno.ToString());
                    writer.WriteElementString("ProductIdentifier", BarCodeParse.ProductIdentifier ?? string.Empty);
                    writer.WriteElementString("ProductUnitBarCodeModifier1", SysEnv.CurrentBarCodeModifier1 ?? string.Empty);
                    writer.WriteElementString("RawData", BarCodeParse.Raw ?? string.Empty);
                    writer.WriteElementString("enumBarCodeSymbology", Convert.ToInt32(BarCodeParse.Symbology).ToString());
                    writer.WriteElementString("BarCodeSymbologyPrefix", BarCodeParse.SymbologyPrefix ?? string.Empty);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                }

                var xmlDoc = XDocument.Parse(sb.ToString());
                await this.ActItemBarCode.Insert(xmlDoc);
                return xmlDoc;
            }
        }

        /// <summary>
        /// Product unit bar code model.
        /// </summary>
        private class ProductUnitBarCodeModel
        {
            public int ProductUnitBarCodeRefno { get; set; }

            public int ProductUnitRefno { get; set; }
        }

        /// <summary>
        ///     Defines the <see cref="ActDetailAndStopForDetail" /> Activity detail used for editing.
        /// </summary>
        private class ActDetailAndStopForDetail
        {
            /// <summary>
            ///     Gets or sets the activity detail.
            /// </summary>
            public ActDetail ActDetail { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether to stop for edit detail.
            /// </summary>
            public bool IsForceStopForEdit { get; set; }
        }
    }
}
