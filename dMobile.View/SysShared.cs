// <copyright file="SysShared.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using BarCodeScanner;
    using BarCodeScanner.BarCodeParsing;
    using Bedrock;
    using Bedrock.Settings;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using CustomPages;
    using Controller;
    using Resources.Models.Products;
    using Forms;
    using Resources.Enums;
    using Resources.Models;
    using Xamarin.Forms;
    using static Bedrock.LocationMapTasks;
    using static Bedrock.ProductInfoTasks;

    /// <summary>
    ///     Defines the <see cref="SysShared" />.
    /// </summary>
    public class SysShared : DMobileContentPage
    {
        /// <summary>
        ///     Activity type. Sometimes we need to know what type of activity we are working with.
        /// </summary>
        public enum ActivityType
        {
            /// <summary>
            ///     Activity type is a header.
            /// </summary>
            Header = 0,

            /// <summary>
            ///    Activity type is items.
            /// </summary>
            Items = 1,

            /// <summary>
            ///    Activity type is details.
            /// </summary>
            Details = 2,

            /// <summary>
            ///   Activity type is details edit.
            /// </summary>
            DetailsEdit = 3,

            /// <summary>
            ///   Activity type is header filter.
            /// </summary>
            HeaderFilter = 4,
        }

        /// <summary>
        ///     Parse Nossack skid bar codes.
        /// </summary>
        /// <param name="parse">.</param>
        /// <param name="actDetail">.</param>
        /// <param name="unitQty">.</param>
        public static void EvalParse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount(
            BarCodeParse parse,
            ActDetail actDetail,
            ref decimal unitQty)
        {
            // Removed check for Nossack's GTIN Company Prefix WO#11478
            // Nossack's GTIN Company Prefix

            // if (parse.GTIN.Contains("67156"))
            // {
            var weight = actDetail.WeightQuantity;

            if (actDetail.ActItem.ProdInfo.WeightUnitCode == "LB")
            {
                if (parse.NetWeightMetricKg > 0)
                {
                    weight = Convert.ToDecimal(SysCommon.ConvertKg2Lb(Convert.ToDouble(parse.NetWeightMetricKg)));
                }
                else if (parse.NetWeightNonMetricLb > 0)
                {
                    weight = parse.NetWeightNonMetricLb;
                }
            }
            else if (actDetail.ActItem.ProdInfo.WeightUnitCode == "KG")
            {
                if (parse.NetWeightNonMetricLb > 0)
                {
                    weight = Convert.ToDecimal(SysCommon.ConvertLb2Kg(Convert.ToDouble(parse.NetWeightNonMetricLb)));
                }
                else if (parse.NetWeightMetricKg > 0)
                {
                    weight = parse.NetWeightMetricKg;
                }
            }

            // Magic number 4 times the average weight to determine if the code is a skid label.
            if (parse.SingleTradeItemSerialNumber.Length > 1 && weight > actDetail.ActItem.ProdInfo.UnitGrossWeight * 4)
            {
                // 2 = For the first digit past the 01 in the AI(21).
                unitQty = decimal.Parse(parse.SingleTradeItemSerialNumber.Substring(2));
            }
        }

        /// <summary>
        ///     The IdentifyProductScanned.
        /// </summary>
        /// <param name="actSubType">The actSubType<see cref="SysCommon.ActSubTypes" />.</param>
        /// <param name="barCodeParse">.</param>
        /// <returns>The <see cref="int" /> Task operation.</returns>
        public static async Task<int> IdentifyProductScanned(
            SysCommon.ActSubTypes actSubType,
            BarCodeParse barCodeParse)
        {
            var prodUnitRefno = 0;

            var prodUnitBarCodeRefno = 0;
            var actBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
            var actItemBarCode = ActItemFacade.GetActItem(actBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            ActDetailFacade.GetActDetail(actItemBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            var sysEnv = SysEnvironment.GetInstance();

            // Initialize by reference parameter variables
            prodUnitRefno = 0;

            // Attempt to find the product in local database.
            // Do not look up GTIN in bar code database for this rule, product will be identified by a range
            // of fixed weights.
            if (sysEnv.BarCodeSettings.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct &&
                barCodeParse.ProductIdentifier == "90695955110022")
            {
                BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct(barCodeParse);
                var prodInfo = await GetInfoFromCodeBcUnit(
                    barCodeParse.ProductIdentifier,
                    string.Empty,
                    "CS");
                prodUnitBarCodeRefno = 0;
                prodUnitRefno = prodInfo.UnitRefno;
            }
            else
            {
                const string query = @"
                    SELECT tProductUnitBarCode.ProductUnitBarCodeRefno,
                        tProductUnitBarCode.ProductUnitRefno
                    FROM tProductUnitBarCode
                    WHERE ProductUnitBarCodeCode = ?
                        AND ProductUnitBarCodeModifier1 = ?";

                var results =
                    await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductUnitBarCodeModel>(
                        query, barCodeParse.ProductIdentifier, sysEnv.CurrentBarCodeModifier1);

                if (results?.Count > 0)
                {
                    prodUnitBarCodeRefno = Convert.ToInt32(results[0].ProductUnitBarCodeRefno);
                    prodUnitRefno = Convert.ToInt32(results[0].ProductUnitRefno);
                }
            }

            // If product was not found in the database ask the comm server if it can identify it.
            // Deli PLU's will be handled by the comm server also.
            // <issue="13440" date="2016/02/18" author="RD"> - Added the condition fo
            // prodUnitRefno == 0 to fix Sumwa farms scanning issue where it was bringing in the
            // avg weight instead of barcode parsed weight.
            // This prevents the found prodInfo from being overwritten.</issue>
            if (prodUnitBarCodeRefno == 0 && prodUnitRefno == 0)
            {
                if (!sysEnv.SysSettings.IsDisconnectedMode)
                {
                    decimal deliScaleWeight = 0;
                    var unitRefnoFromBarCode = await BarCodeCommon.GetDeliProdInfoFromBarCode(
                        barCodeParse.ProductIdentifier,
                        sysEnv.CurrentBarCodeModifier1,
                        barCodeParse.Symbology,
                        barCodeParse.DeliScaleAmount);

                    var prod = unitRefnoFromBarCode.ProdInfo;
                    prodUnitRefno = unitRefnoFromBarCode.ProdInfo.UnitRefno;
                    deliScaleWeight = unitRefnoFromBarCode.DeliScaleWeight;
                    var deliScaleWeightUnit = unitRefnoFromBarCode.DeliScaleWeightUnit;

                    switch (deliScaleWeightUnit)
                    {
                        case "LB":
                            barCodeParse.NetWeightNonMetricLb = deliScaleWeight;
                            barCodeParse.NetWeightMetricKg = Convert.ToDecimal(
                                SysCommon.ConvertLb2Kg(Convert.ToDouble(
                                    barCodeParse.NetWeightNonMetricLb)));
                            break;

                        case "KG":
                            barCodeParse.NetWeightMetricKg = deliScaleWeight;
                            barCodeParse.NetWeightNonMetricLb = Convert.ToDecimal(
                                SysCommon.ConvertKg2Lb(Convert.ToDouble(
                                    barCodeParse.NetWeightMetricKg)));
                            break;

                        default:
                            deliScaleWeight = 0;
                            deliScaleWeightUnit = string.Empty;
                            break;
                    }
                }

                // If 'yes' to join bar code to a product
                if (prodUnitRefno == 0)
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

                    // Changed functionality to disallow product scanning everywhere.
                    // However, always prompt for password if not allowed to associate.
                    var isAllowedAssocProdBarCode = sysEnv.ContactLog.IsAllowedAssociateProductBarCode;

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

                    if (isAllowedAssocProdBarCode)
                    {
                        var frmSelectProduct = new FormDialogContainer(EDialogTypes.SelectProductUnit, actSubType);
                        if (await Device.InvokeOnMainThreadAsync(async () =>
                                await frmSelectProduct.DisplayControl.ShowDialog() == DialogResult.OK))
                        {
                            var xmlDoc = await frmSelectProduct.GetXml();
                            if (!(xmlDoc is { Root: { } }))
                            {
                                throw new MessageOnlyException("Product unit was not identified.");
                            }

                            prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno")?.Value!);

                            // If a product was selected, create the xml for the bar code
                            // the activity insert
                            if (prodUnitRefno != 0)
                            {
                                xmlDoc = XDocument.Parse(
                                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                    + "<params>"
                                    + "<ProductUnitRefno>" + prodUnitRefno + "</ProductUnitRefno>"
                                    + "<ProductUnitBarCodeRefno>" + prodUnitBarCodeRefno
                                    + "</ProductUnitBarCodeRefno>"
                                    + "<ProductIdentifier>" + barCodeParse.ProductIdentifier + "</ProductIdentifier>"
                                    + "<ProductUnitBarCodeModifier1>" + sysEnv.CurrentBarCodeModifier1
                                    + "</ProductUnitBarCodeModifier1>"
                                    + "<RawData>" + barCodeParse.Raw + "</RawData>"
                                    + "<enumBarCodeSymbology>" + Convert.ToInt32(barCodeParse.Symbology)
                                    + "</enumBarCodeSymbology>"
                                    + "<BarCodeSymbologyPrefix>" + barCodeParse.SymbologyPrefix +
                                    "</BarCodeSymbologyPrefix>"
                                    + "</params>");

                                await actItemBarCode.Insert(xmlDoc);
                            }
                        }
                        else
                        {
                            throw new MessageOnlyException("Product not identified.");
                        }
                    }
                    else
                    {
                        throw new MessageOnlyException("New bar code product association is not enabled.");
                    }
                }
            }

            return prodUnitRefno;
        }

        /// <summary>
        ///     Gather activity detail data from the scanned bar code.
        /// </summary>
        /// <param name="barParse"><see cref="BarCodeParse" /> Bar code parse scan information.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="actItem"><see cref="ActItem" /> Activity item instance.</param>
        /// <param name="defaultItemSchemeData"><see cref="ActItemSchemeData" /> Scheme data.</param>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail.</param>
        /// <param name="isForceStopForEditing"> Flag to force stopping at the edit screen.</param>
        /// <returns>True if ready to insert detail else false.</returns>
        public static async Task<bool> GatherProductData(
            BarCodeParse barParse,
            SysCommon.ActSubTypes actSubType,
            ActItem actItem,
            ActItemSchemeData defaultItemSchemeData,
            ActDetail actDetail,
            bool isForceStopForEditing = false)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var isInsertDetailReady = false;

            // Can't remember why this block is needed.
            if (!string.IsNullOrEmpty(barParse.ActDetailFkRefno) && !sysEnv.SysSettings.IsDisconnectedMode)
            {
                defaultItemSchemeData = new ActItemSchemeData();
                var locMap = LocationMap.GetInstance();
                var sLocMapAndRefno = await GetSLocMapFromLocMapFkRefno(
                    defaultItemSchemeData.LocMapWhsFkRefno,
                    defaultItemSchemeData.LocMapSectFkRefno,
                    defaultItemSchemeData.LocMapSlotFkRefno);
                locMap.CurrLocMap = sLocMapAndRefno.SLocMap;
            }

            var defaultSchemeDataAssigned = false;
            actDetail.SchemeData = await GatherSchemeData(actDetail, barParse, defaultItemSchemeData);

            var actDetailAndStopForDetail = await GatherProductQuantityData(
                actDetail,
                barParse,
                defaultItemSchemeData);

            if (defaultSchemeDataAssigned)
            {
                actDetailAndStopForDetail.IsForceStopForEdit = defaultSchemeDataAssigned;
            }
            else
            {
                actDetailAndStopForDetail.IsForceStopForEdit = IsStopForDetail();
            }

            // isForceStopForEditing may be set to true from the calling method.
            if (isForceStopForEditing)
            {
                actDetailAndStopForDetail.IsForceStopForEdit = true;
            }

            var isStopForLabelDetail = false;
            if (actDetail.ActSubType == SysCommon.ActSubTypes.Receiving && actDetail.ActItem.LocMap.IsWhsWms)
            {
                if (!actDetail.IsLabelSchemeComplete())
                {
                    isStopForLabelDetail = true;
                }
            }

            if (actDetailAndStopForDetail.IsForceStopForEdit)
            {
                var continueIsStopForDetail = false;

                // Check for duplicate occurs in form
                // Check for quantity over occurs in form
                var form = new FormDialogContainer(EDialogTypes.ActivityDetailEdit, actDetail, actSubType);

                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    continueIsStopForDetail = true;
                }

                if (continueIsStopForDetail)
                {
                    XDocument xmlDoc = null;
                    ////xmlDoc = form.DisplayControlData;
                    xmlDoc = await ((IAppArea)form.DisplayControl).GetXml();
                    UpdateActDetailFromXml(ref actDetail, xmlDoc);

                    // No need to call actItem.IsReadyToApply(xmlParam) the form will call it.
                    isInsertDetailReady = true;
                }
                else
                {
                    throw new GatherProductDataException("Cancelled from detail edit.", true);
                }
            }
            else if (isStopForLabelDetail)
            {
                var continueIsStopForLabelDetail = false;

                // No duplicate check needed for label printing
                // No quantity over check needed for label printing
                var form = new FormDialogContainer(
                    EDialogTypes.ActivityProductLabel,
                    actDetail,
                    actSubType);

                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    continueIsStopForLabelDetail = true;
                }

                if (continueIsStopForLabelDetail)
                {
                    XDocument xmlDoc = null;
                    xmlDoc = await ((IAppArea)form.DisplayControl).GetXml();
                    UpdateActDetailFromXml(ref actDetail, xmlDoc);
                }

                // No need to call actItem.IsReadyToApply(xmlParam) the form will call it.
                // Insert the activity detail even if the print product label was canceled.
                // All inventory tracked scheme components where complete before even call the
                // print product labels control.
                isInsertDetailReady = true;
            }
            else
            {
                actDetail.SchemeId = GetActSchemeIdFromSProdSchemeData(
                    actItem.ProdInfo.Code,
                    actItem.ProdInfo.Scheme,
                    actDetail.SchemeData);
                actDetail.UniqueSchemeId = GetUniqueSchemeIdFromSProdSchemeData(
                    actItem.ProdInfo.Code,
                    actItem.ProdInfo.Scheme,
                    actDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and
                // quantity validations.
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<itm_refno>" + actItem.ItemFkRefno + "</itm_refno>"
                    + "<alt_qty>" + actDetail.Quantity + "</alt_qty>"
                    + "<wt_qty>" + actDetail.WeightQuantity + "</wt_qty>"
                    + "<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeId + "</ActivityDetailUniqueSchemeID>"
                    + "</imclient>");

                await actItem.IsReadyToApply(xmlParam);

                isInsertDetailReady = true;
            }

            return isInsertDetailReady;

            //////////////////////////////////////////////////////////////////////////
            async Task<ProductSchemeData> GatherSchemeData(
                ActDetail actDetail,
                BarCodeParse barParse,
                ActItemSchemeData tempItemSchemeData)
            {
                var schemeData = actDetail.SchemeData;
                NebraskBeefRule(barParse);

                // <issue="13609"/>
                AssertSuppressedErrorsAgainstScheme(actDetail.ActItem.ProdInfo, barParse);
                AssignSkidNumberFromSelectedSkid(actDetail, schemeData);
                AssignBestBefore(actDetail, barParse, schemeData, tempItemSchemeData);
                AssignProductionDate(actDetail, barParse, schemeData, tempItemSchemeData);
                await AssignLotNumber(actDetail, barParse, schemeData, tempItemSchemeData);
                AssignSerialNumber(actDetail, barParse, schemeData, tempItemSchemeData);
                await AssignVariant(actDetail, barParse, schemeData, tempItemSchemeData);
                return schemeData;

                //////////////////////////////////////////////////////////////////////////
                static bool IsNebraskaBeefRuleMet(BarCodeParse barParse)
                {
                    return (barParse.Gtin.Contains("0740780")
                            || barParse.Gtin.Contains("19336"))
                           && !string.IsNullOrEmpty(barParse.LogisticUnitBatchLotNumber);
                }

                //////////////////////////////////////////////////////////////////////////
                static void NebraskBeefRule(BarCodeParse barCodeParse)
                {
                    var sysEnv = SysEnvironment.GetInstance();

                    // Decided to implement the bar code here. Could have made it a new BarCodeParseCode_128_NebraskaBeef class.
                    // The bar code was well-formed except it being a non GS1 code 128.
                    if (sysEnv.BarCodeSettings.IsRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 &&
                        IsNebraskaBeefRuleMet(barCodeParse))
                    {
                        // Lot Number is '00000021724739' serial number should be '724739'
                        barCodeParse.LogisticUnitBatchLotNumber = barCodeParse.LogisticUnitBatchLotNumber.Remove(0, 8);
                        barCodeParse.SingleTradeItemSerialNumber = string.Empty;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void AssignBestBefore(
                    ActDetail actDetail,
                    BarCodeParse barCodeParse,
                    ProductSchemeData schemeData,
                    ActItemSchemeData tempItemSchemeData)
                {
                    if (actDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked
                        || actDetail.ActItem.ProdInfo.Scheme.LabelInfo.IsBestBeforeDateTracked)
                    {
                        if (SysCommon.IsEmptyDateTime(barCodeParse.LogisticUnitBestBeforeSellByDate))
                        {
                            schemeData.LogisticUnitBestBeforeSellByDate =
                                SysCommon.IsEmptyDate(tempItemSchemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate)
                                    ? new DateTime(1900, 1, 1)
                                    : tempItemSchemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate;
                            defaultSchemeDataAssigned = true;
                        }
                        else
                        {
                            schemeData.LogisticUnitBestBeforeSellByDate = barCodeParse.LogisticUnitBestBeforeSellByDate;
                        }
                    }
                    else
                    {
                        schemeData.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void AssignProductionDate(
                    ActDetail actDetail,
                    BarCodeParse barCodeParse,
                    ProductSchemeData schemeData,
                    ActItemSchemeData tempItemSchemeData)
                {
                    if (actDetail.ActItem.ProdInfo.Scheme.IsProductionDateTracked
                        || actDetail.ActItem.ProdInfo.Scheme.LabelInfo.IsProductionDateTracked)
                    {
                        if (SysCommon.IsEmptyDateTime(barCodeParse.LogisticUnitProductionDate))
                        {
                            schemeData.LogisticUnitProductionDate =
                                SysCommon.IsEmptyDate(tempItemSchemeData.ProdSchemeData.LogisticUnitProductionDate)
                                    ? new DateTime(1900, 1, 1)
                                    : tempItemSchemeData.ProdSchemeData.LogisticUnitProductionDate;
                            defaultSchemeDataAssigned = true;
                        }
                        else
                        {
                            schemeData.LogisticUnitProductionDate = barCodeParse.LogisticUnitProductionDate;
                        }
                    }
                    else
                    {
                        schemeData.LogisticUnitProductionDate = new DateTime(1900, 1, 1);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task AssignLotNumber(
                    ActDetail actDetail,
                    BarCodeParse barCodeParse,
                    ProductSchemeData schemeData,
                    ActItemSchemeData tempItemSchemeData)
                {
                    if (actDetail.ActItem.ProdInfo.Scheme.IsLotTracked
                        || actDetail.ActItem.ProdInfo.Scheme.LabelInfo.IsLotTracked)
                    {
                        if (barCodeParse.LogisticUnitBatchLotNumber == string.Empty)
                        {
                            schemeData.LogisticUnitBatchLotNumber =
                                tempItemSchemeData.ProdSchemeData.LogisticUnitBatchLotNumber;
                            defaultSchemeDataAssigned = true;
                        }
                        else
                        {
                            schemeData.LogisticUnitBatchLotNumber = barCodeParse.LogisticUnitBatchLotNumber;
                        }

                        // This code is expected to have a speed issue.
                        // Comparing GTIN = '90000122345000' to "122345". The small string is on the right instead of the left.
                        // A SQL command using '122345' LIKE %90000122345000% will of course not work.
                        // Only way to find was to iterate through the table and use a 'contains' command.
                        var sysEnv = SysEnvironment.GetInstance();
                        if (sysEnv.SysSettings.IsEnableDefaultDetailValues &&
                            actDetail.ActItem.ProdInfo.Scheme.IsLotTrackedDefault)
                        {
                            var lotCodeDefault = await GetLotCodeDefaultFromSupplierCompanyPrefix(
                                actDetail.ActItem.ProdInfo.Scheme.SchemeRefno,
                                barCodeParse.Gtin);

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
                                        schemeData.LogisticUnitBatchLotNumber = lotCodeDefault + "-" +
                                            schemeData.LogisticUnitBatchLotNumber
                                                .TrimEnd();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        schemeData.LogisticUnitBatchLotNumber = string.Empty;
                    }

                    return;

                    //////////////////////////////////////////////////////////////////////////
                    static async Task<string> GetLotCodeDefaultFromSupplierCompanyPrefix(
                        int schemeReferenceNumber,
                        string globalTradeIdentifier)
                    {
                        schemeReferenceNumber = schemeReferenceNumber <= 0
                            ? throw new ArgumentOutOfRangeException(nameof(schemeReferenceNumber))
                            : schemeReferenceNumber;
                        var lotCodeDefault = string.Empty;

                        if (!string.IsNullOrEmpty(globalTradeIdentifier))
                        {
                            // Order by descending, we want to examine the longest string to compare for a match first.
                            const string query = @"
                                select tProductSchemeLotNumber.ProductSchemeLotNumberCode,
                                        tProductSchemeLotNumber.ProductSchemeLotNumberCodeDefault
                                    from tProductSchemeLotNumberXRef
                                    inner join tProductSchemeLotNumber ON tProductSchemeLotNumber.ProductSchemeLotNumberRefno =
                                            tProductSchemeLotNumberXRef.ProductSchemeLotNumberRefno
                                        where tProductSchemeLotNumberXRef.ProductSchemeRefno = ?
                                    order by ProductSchemeLotNumberCode DESC";
                            var results =
                                await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductSchemeLotNumberModel>(
                                    query,
                                    schemeReferenceNumber);

                            if (results?.Count > 0)
                            {
                                foreach (var xRef in results)
                                {
                                    if (globalTradeIdentifier.Contains(Convert.ToString(xRef.ProductSchemeLotNumberCode)
                                            .TrimEnd()))
                                    {
                                        lotCodeDefault = Convert.ToString(xRef.ProductSchemeLotNumberCodeDefault).TrimEnd();
                                        break;
                                    }
                                }
                            }
                        }

                        return lotCodeDefault;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void AssignSerialNumber(
                    ActDetail actDetail,
                    BarCodeParse barCodeParse,
                    ProductSchemeData schemeData,
                    ActItemSchemeData tempItemSchemeData)
                {
                    if (actDetail.ActItem.ProdInfo.Scheme.IsSerialTracked
                        || actDetail.ActItem.ProdInfo.Scheme.LabelInfo.IsSerialTracked)
                    {
                        if (barCodeParse.SingleTradeItemSerialNumber == string.Empty)
                        {
                            schemeData.SingleTradeItemSerialNumber =
                                tempItemSchemeData.ProdSchemeData.SingleTradeItemSerialNumber;
                            defaultSchemeDataAssigned = true;
                        }
                        else
                        {
                            schemeData.SingleTradeItemSerialNumber = barCodeParse.SingleTradeItemSerialNumber;
                        }
                    }
                    else
                    {
                        schemeData.SingleTradeItemSerialNumber = string.Empty;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task AssignVariant(
                    ActDetail actDetail,
                    BarCodeParse barCodeParse,
                    ProductSchemeData schemeData,
                    ActItemSchemeData tempItemSchemeData)
                {
                    if (actDetail.ActItem.ProdInfo.Scheme.IsVariantTracked
                        || actDetail.ActItem.ProdInfo.Scheme.LabelInfo.IsVariantTracked)
                    {
                        if (barCodeParse.LogisticUnitProductVariant == 0)
                        {
                            schemeData.VariantCode = tempItemSchemeData.ProdSchemeData.VariantCode;
                            schemeData.VariantDescription = tempItemSchemeData.ProdSchemeData.VariantDescription;
                            schemeData.VariantNumber = tempItemSchemeData.ProdSchemeData.VariantNumber;
                            defaultSchemeDataAssigned = true;
                        }
                        else
                        {
                            schemeData.VariantNumber = barCodeParse.LogisticUnitProductVariant;
                            await actDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(schemeData);
                        }
                    }
                    else
                    {
                        schemeData.VariantCode = string.Empty;
                        schemeData.VariantDescription = string.Empty;
                        schemeData.VariantNumber = 0;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                static void AssignSkidNumberFromSelectedSkid(ActDetail actDetail1, ProductSchemeData schemeData)
                {
                    // Decided not to use the CurrSkid.LogisticUnitSkidCode because is is the full GS1 bar code. 18 digits.
                    // Instead I am using the CurrSkid.SkidNo.
                    // The SkidInfo.CurrSkid.SkidNo could have been assigned by manually selecting or creating a new skid.
                    // Or it could have be scanned and the skid info looked up.
                    schemeData.LogisticUnitSkidNumber = actDetail1.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                }
            }

            //////////////////////////////////////////////////////////////////////////
            static async Task<ActDetailAndStopForDetail> GatherProductQuantityData(
                ActDetail actDetail,
                BarCodeParse barParse,
                ActItemSchemeData tempItemSchemeData)
            {
                var prodInfo = actDetail.ActItem.ProdInfo;
                var barCodeRuleForcedEdit = false;

                if (actDetail.ActItem.IsCatchAskZero())
                {
                    actDetail.Quantity = 0;
                    actDetail.BaseQuantity = 0;
                    actDetail.WeightQuantity = 0;
                }
                else
                {
                    GatherWeightQuantity(prodInfo, actDetail, barParse);
                    GatherQuantityFromItem();
                    await GatherUnitQuantity();
                    NegateAdjustmentQuantity();
                }

                // Fill activity detail and stop for detail.
                var actDetailAndStopForDetail = new ActDetailAndStopForDetail
                {
                    ActDetail = actDetail,
                    IsForceStopForEdit = barCodeRuleForcedEdit,
                };

                return actDetailAndStopForDetail;

                //////////////////////////////////////////////////////////////////////////
                static void GatherWeightQuantity(ProductInfo prodInfo, ActDetail actDetail, BarCodeParse barCodeParse)
                {
                    var sysEnv = SysEnvironment.GetInstance();

                    // Assign scanned weight and convert if needed
                    if (!(sysEnv.SysSettings.IsSetCatchWeightZero && prodInfo.IsCatchWeight))
                    {
                        actDetail.WeightQuantity = prodInfo.UnitGrossWeight;
                    }

                    if (prodInfo.IsCatchWeight)
                    {
                        switch (prodInfo.WeightUnitCode)
                        {
                            case "LB" when barCodeParse.NetWeightMetricKg != 0:
                                actDetail.WeightQuantity = SysCommon.ConvertKg2Lb(barCodeParse.NetWeightMetricKg);
                                break;

                            case "LB":
                            {
                                if (barCodeParse.NetWeightNonMetricLb != 0)
                                {
                                    actDetail.WeightQuantity = barCodeParse.NetWeightNonMetricLb;
                                }

                                break;
                            }

                            case "KG" when barCodeParse.NetWeightNonMetricLb != 0:
                                actDetail.WeightQuantity = SysCommon.ConvertLb2Kg(barCodeParse.NetWeightNonMetricLb);
                                break;

                            case "KG":
                            {
                                if (barCodeParse.NetWeightMetricKg != 0)
                                {
                                    actDetail.WeightQuantity = barCodeParse.NetWeightMetricKg;
                                }

                                break;
                            }
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task GatherBaseQuantity(decimal unitQuantity)
                {
                    var sysEnv = SysEnvironment.GetInstance();
                    if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                    {
                        actDetail.BaseQuantity = actDetail.WeightQuantity;
                    }
                    else
                    {
                        if (prodInfo.IsCatchCount
                            && barParse.LogisticUnitVariableCount != 0
                            && sysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive)
                        {
                            actDetail.BaseQuantity = barParse.LogisticUnitVariableCount;
                        }
                        else
                        {
                            actDetail.BaseQuantity = unitQuantity * await GetUnitRelationFactor(prodInfo);
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void NegateAdjustmentQuantity()
                {
                    // Negate the quantity if desired
                    if (actDetail.ActItem.ActHdr.ActSubType == SysCommon.ActSubTypes.Adjustment)
                    {
                        var adjSettings = new AdjustmentSettings().LoadData();
                        if (adjSettings.NegateScanQuantity)
                        {
                            actDetail.Quantity *= -1;
                            actDetail.BaseQuantity *= -1;
                            actDetail.WeightQuantity *= -1;
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task GatherUnitQuantity()
                {
                    decimal unitQuantity = 1;
                    await CheckBarCodeRuleAi21BoxCode(unitQuantity);

                    // If catch count is enabled the variable count is the base quantity not the unit quantity
                    if (!prodInfo.IsCatchCount)
                    {
                        var sysEnv = SysEnvironment.GetInstance();
                        if (barParse.LogisticUnitVariableCount != 0
                            && sysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive)
                        {
                            unitQuantity = barParse.LogisticUnitVariableCount;
                        }
                    }

                    await GatherBaseQuantity(unitQuantity);

                    if (prodInfo.UnitCode == "LB" ||
                        prodInfo.UnitCode == "KG")
                    {
                        actDetail.Quantity = actDetail.WeightQuantity;
                    }
                    else
                    {
                        actDetail.Quantity = unitQuantity;
                    }

                    return;

                    //////////////////////////////////////////////////////////////////////////
                    async Task<decimal> CheckBarCodeRuleAi21BoxCode(decimal unitQty)
                    {
                        var sysEnv = SysEnvironment.GetInstance();
                        if (sysEnv.BarCodeSettings.IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount)
                        {
                            EvalParse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount(
                                barParse,
                                actDetail,
                                ref unitQty);

                            // Adjust the weight if rule was applied.
                            if (unitQty > 1)
                            {
                                // See WO# 12203 Always stop and allow editing of detail.
                                barCodeRuleForcedEdit = true;

                                if (!prodInfo.IsCatchWeight)
                                {
                                    actDetail.WeightQuantity = unitQty * await GetUnitRelationFactor(
                                        prodInfo);
                                }
                            }
                        }

                        return unitQty;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void GatherQuantityFromItem()
                {
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
                }
            }

            //////////////////////////////////////////////////////////////////////////
            static void UpdateActDetailFromXml(ref ActDetail actDetail, XDocument xmlDoc)
            {
                var schemeData = actDetail.SchemeData;
                if (xmlDoc.Root != null)
                {
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
                    actDetail.ActExtra.DateTime1 = DateTime.Parse(xmlDoc.Root.Element("extra5").Value);
                }

                actDetail.SchemeId = GetActSchemeIdFromSProdSchemeData(
                    actDetail.ActItem.ProdInfo.Code,
                    actDetail.ActItem.ProdInfo.Scheme,
                    actDetail.SchemeData);

                actDetail.UniqueSchemeId = GetUniqueSchemeIdFromSProdSchemeData(
                    actDetail.ActItem.ProdInfo.Code,
                    actDetail.ActItem.ProdInfo.Scheme,
                    actDetail.SchemeData);
            }

            //////////////////////////////////////////////////////////////////////////
            bool IsStopForDetail()
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
                        if (actDetail.IsSchemeComplete(actDetail.ActItem))
                        {
                            if (actDetail.ActItem.IsCatchAsk())
                            {
                                isStopForDetail = true;
                            }
                        }
                        else
                        {
                            isStopForDetail = true;
                        }
                    }
                }

                if (!isStopForDetail)
                {
                    if (actSubType == SysCommon.ActSubTypes.Receiving && actDetail.ActItem.ProdInfo.ProductAcceptDays > 0)
                    {
                        if (actDetail.ActItem.ProdInfo.Scheme.IsProductionDateTracked &&
                            actDetail.SchemeData.LogisticUnitProductionDate > new DateTime(1900, 1, 1))
                        {
                            if (actDetail.SchemeData.LogisticUnitProductionDate.AddDays(
                                actDetail.ActItem.ProdInfo.ProductAcceptDays) < DateTime.Now.Date)
                            {
                                isStopForDetail = true;
                            }
                        }

                        if (actDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                            actDetail.SchemeData.LogisticUnitBestBeforeSellByDate > new DateTime(1900, 1, 1))
                        {
                            if (actDetail.SchemeData.LogisticUnitBestBeforeSellByDate >
                                DateTime.Now.AddDays(actDetail.ActItem.ProdInfo.ProductAcceptDays))
                            {
                                isStopForDetail = true;
                            }
                        }
                    }
                }

                if (!isStopForDetail)
                {
                    var scheme = actDetail.ActItem.ProdInfo.Scheme;
                    if (scheme.Extra1Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) && scheme.Extra1IsRequired)
                    {
                        isStopForDetail = true;
                    }
                    else if (scheme.Extra2Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) && scheme.Extra2IsRequired)
                    {
                        isStopForDetail = true;
                    }
                    else if (scheme.Extra3Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) && scheme.Extra3IsRequired)
                    {
                        isStopForDetail = true;
                    }
                    else if (scheme.Extra4Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) && scheme.Extra4IsRequired)
                    {
                        isStopForDetail = true;
                    }
                    else if (scheme.Extra5Type != Convert.ToInt32(SysCommon.SchemeExtraTypes.Disabled) && scheme.Extra5IsRequired)
                    {
                        isStopForDetail = true;
                    }
                }

                return isStopForDetail;
            }
        }

        /// <summary>
        ///     The GetAssemblyVersion.
        /// </summary>
        /// <returns>.</returns>
        public static string GetAssemblyVersion()
        {
            var major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return $"{major}.{minor}.{build}.{revision}";
        }

        /// <summary>
        ///     The BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIDProduct.
        /// </summary>
        /// <param name="barCodeParse">.</param>
        public static void BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct(
            BarCodeParse barCodeParse)
        {
            if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(11.00) &&
                barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(11.99))
            {
                barCodeParse.ProductIdentifier = "MC10448";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.00) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.49))
            {
                barCodeParse.ProductIdentifier = "MC10443";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.50) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.99))
            {
                barCodeParse.ProductIdentifier = "MC10441";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.00) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.49))
            {
                barCodeParse.ProductIdentifier = "MC10442";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.50) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.99))
            {
                barCodeParse.ProductIdentifier = "MC10440";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.00) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.49))
            {
                barCodeParse.ProductIdentifier = "MC10445";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.50) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.99))
            {
                barCodeParse.ProductIdentifier = "MC10446";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.00) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(15.49))
            {
                barCodeParse.ProductIdentifier = "MC10447";
            }
            else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.50) &&
                     barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(16.00))
            {
                barCodeParse.ProductIdentifier = "MC10450";
            }
            else
            {
                throw new Exception("Unable to identify product scanned." + Environment.NewLine +
                                    "ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct enabled.");
            }
        }

        /// <summary>
        ///     <para>Checks to see if we have to throw any suppressed errors from Bar Code parsing.</para>
        ///     <para>
        ///         Rule Enabled
        ///         If an error is tracked in the scheme, throw first error from a tracked field.
        ///     </para>
        ///     <para>
        ///         Rule Disabled
        ///         If there are errors, throw the first error.
        ///     </para>
        ///     <para>
        ///         <see cref="SysBarCodeSettings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents" />.
        ///     </para>
        ///     <seealso cref="BarCodeParseUcc_EAN_128_Symbol.ParseBarCodeData" />.
        /// </summary>
        /// <param name="actDetail">.</param>
        /// <param name="parse">.</param>
        public static void AssertSuppressedErrorsAgainstScheme(ProductInfo prodInfo, BarCodeParse parse)
        {
            // Currently only UCC EAN 128 will create these errors
            if (!parse.Details.HasErrors)
            {
                // no errors!
                return;
            }

            // UCC EAN 128
            // Suppress Rule Enabled
            if (parse.Symbology == BarCodeSymbology.UCC_EAN_128_Symbol &&
                parse.Settings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents)
            {
                ProdScheme scheme;
                if (prodInfo != null)
                {
                    scheme = prodInfo.Scheme;
                }
                else
                {
                    scheme = new ProdScheme();
                }

                // Check errors against scheme
                var details = parse.Details;
                var count = details.ErrorCount;
                for (var i = 0; i < count; ++i)
                {
                    var reason = details.GetParseExceptionReasonAt(i);

                    // Default/unhandled will be thrown
                    switch (reason)
                    {
                        case BarCodeExceptionReason.ActivityDetailFKRefno_InvalidFormat:
                            break;

                        case BarCodeExceptionReason.ActivityFKRefno_InvalidFormat:
                            break;

                        case BarCodeExceptionReason.ActivityItemFKRefno_InvalidFormat:
                            break;

                        case BarCodeExceptionReason.CountOfTradeItems_InvalidInteger:
                            break;

                        case BarCodeExceptionReason.DimensionMeters_InvalidDecimal:
                            break;

                        case BarCodeExceptionReason.DimensionMeters_UnknownDecimalPlaces:
                            break;

                        case BarCodeExceptionReason.DueDate_InvalidDate:
                            break;

                        case BarCodeExceptionReason.ExpiryUseByDate_InvalidDate:
                            break;

                        case BarCodeExceptionReason.GrossWeightMetricKG_InvalidDecimal:
                            break;

                        case BarCodeExceptionReason.GrossWeightMetricKG_UnknownDecimalPlaces:
                            break;

                        case BarCodeExceptionReason.GrossWeightNonMetricLB_InvalidDecimal:
                            break;

                        case BarCodeExceptionReason.GrossWeightNonMetricLB_UnknownDecimalPlaces:
                            break;

                        case BarCodeExceptionReason.NetWeightMetricKG_InvalidDecimal:
                            break;

                        case BarCodeExceptionReason.NetWeightMetricKG_UnknownDecimalPlaces:
                            break;

                        case BarCodeExceptionReason.NetWeightNonMetricLB_InvalidDecimal:
                            break;

                        case BarCodeExceptionReason.NetWeightNonMetricLB_UnknownDecimalPlaces:
                            break;

                        case BarCodeExceptionReason.PackagingDate_InvalidDate:
                            break;

                        case BarCodeExceptionReason.VariableCount_InvalidInteger:
                            break;

                        case BarCodeExceptionReason.BatchLotNumber_InvalidFormat:
                            if (scheme.IsLotTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.BestBeforeSellByDate_InvalidDate:
                            if (scheme.IsBestBeforeDateTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.PackagingDateAsProductionDate_InvalidDate:
                            break;

                        case BarCodeExceptionReason.ProductionDate_InvalidDate:
                            if (scheme.IsProductionDateTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.ProductVariant_InvalidInteger:
                            if (scheme.IsVariantTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.SingleTradeItemSerialNumber_InvalidFormat:
                            break;

                        case BarCodeExceptionReason.SingleTradeItemSerialNumber:
                            if (scheme.IsSerialTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

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