// <copyright file="DataCollector.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.BarCodeScanner;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.Bedrock;
    using dMobile.Controller.Activity;
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
        /// <param name="actSubType">The activity sub type.</param>
        public DataCollector(SysCommon.ActSubTypes actSubType)
        {
            this.ActBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
            this.ActItemBarCode = ActItemFacade.GetActItem(this.ActBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            this.ActSubType = actSubType;
        }

        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        private Controller.Activity.Activity ActBarCode { get; } = ActivityFacade.GetActivity(
            SysCommon.ActSubTypes.BarCodeAssociation);

        private ActItem ActItemBarCode { get; } = null;

        private SQLite.SQLiteAsyncConnection Conn { get; } = DatabaseConnection.GetInstance().SqlConn;

        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.BarCodeAssociation;

        private BarCodeParse BarCodeParse { get; set; } = null;

        private ProductUnitBarCodeModel ProdUnitBarCode { get; set; } = new ProductUnitBarCodeModel();

        /// <summary>
        ///     Identify the product scanned by bar code.
        /// </summary>
        /// <param name="barCodeParse">Captured scanned data.</param>
        /// <returns>The <see cref="int" />Product unit reference number.</returns>
        public async Task<int> IdentifyProductScanned(BarCodeParse barCodeParse)
        {
            this.BarCodeParse = barCodeParse;
            this.ProdUnitBarCode = new ProductUnitBarCodeModel();

            await SumwaFarmWholeDuckRule();
            await SearchBarCodesForMatch();
            await SearchDeliBarCodeMatch();
            await AssociateNewBarCode(barCodeParse);

            return this.ProdUnitBarCode.ProductUnitRefno;
        }

        private async Task AssociateNewBarCode(BarCodeParse barCodeParse)
        {
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 && this.ProdUnitBarCode.ProductUnitRefno == 0)
            {
                AskToAssociateBarCode();
                if (!await CheckPermissionToAssociateBarCode())
                {
                    throw new MessageOnlyException("New bar code product association is not enabled.");
                }

                var frmSelectProduct = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);
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
                var isAllowedAssocProdBarCode = this.SysEnv.ContactLog.IsAllowedAssociateProductBarCode;
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
                XDocument xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<ProductUnitRefno>" + this.ProdUnitBarCode.ProductUnitRefno + "</ProductUnitRefno>"
                    + "<ProductUnitBarCodeRefno>" + this.ProdUnitBarCode.ProductUnitBarCodeRefno
                    + "</ProductUnitBarCodeRefno>"
                    + "<ProductIdentifier>" + this.BarCodeParse.ProductIdentifier + "</ProductIdentifier>"
                    + "<ProductUnitBarCodeModifier1>" + this.SysEnv.CurrentBarCodeModifier1
                    + "</ProductUnitBarCodeModifier1>"
                    + "<RawData>" + this.BarCodeParse.Raw + "</RawData>"
                    + "<enumBarCodeSymbology>" + Convert.ToInt32(barCodeParse.Symbology)
                    + "</enumBarCodeSymbology>"
                    + "<BarCodeSymbologyPrefix>" + this.BarCodeParse.SymbologyPrefix +
                    "</BarCodeSymbologyPrefix>"
                    + "</params>");
                await this.ActItemBarCode.Insert(xmlDoc);
                return xmlDoc;
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
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 && this.ProdUnitBarCode.ProductUnitRefno == 0)
            {
                if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    decimal deliScaleWeight = 0;
                    var unitRefnoFromBarCode = await BarCodeCommon.GetDeliProdInfoFromBarCode(
                        this.BarCodeParse.ProductIdentifier,
                        this.SysEnv.CurrentBarCodeModifier1,
                        this.BarCodeParse.Symbology,
                        this.BarCodeParse.DeliScaleAmount);

                    var prod = unitRefnoFromBarCode.ProdInfo;
                    this.ProdUnitBarCode.ProductUnitRefno = unitRefnoFromBarCode.ProdInfo.UnitRefno;
                    deliScaleWeight = unitRefnoFromBarCode.DeliScaleWeight;
                    var deliScaleWeightUnit = unitRefnoFromBarCode.DeliScaleWeightUnit;

                    switch (deliScaleWeightUnit)
                    {
                        case "LB":
                            this.BarCodeParse.NetWeightNonMetricLb = deliScaleWeight;
                            this.BarCodeParse.NetWeightMetricKg = Convert.ToDecimal(
                                SysCommon.ConvertLb2Kg(Convert.ToDouble(this.BarCodeParse.NetWeightNonMetricLb)));
                            break;

                        case "KG":
                            this.BarCodeParse.NetWeightMetricKg = deliScaleWeight;
                            this.BarCodeParse.NetWeightNonMetricLb = Convert.ToDecimal(
                                SysCommon.ConvertKg2Lb(Convert.ToDouble(this.BarCodeParse.NetWeightMetricKg)));
                            break;

                        default:
                            deliScaleWeight = 0;
                            deliScaleWeightUnit = string.Empty;
                            break;
                    }
                }
            }
        }

        private async Task SearchBarCodesForMatch()
        {
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 && this.ProdUnitBarCode.ProductUnitRefno == 0)
            {
                const string query = @"
                        SELECT tProductUnitBarCode.ProductUnitBarCodeRefno,
                            tProductUnitBarCode.ProductUnitRefno
                        FROM tProductUnitBarCode
                        WHERE ProductUnitBarCodeCode = ?
                            AND ProductUnitBarCodeModifier1 = ?";

                var results = await this.Conn.QueryAsync<ProductUnitBarCodeModel>(
                        query,
                        this.BarCodeParse.ProductIdentifier,
                        this.SysEnv.CurrentBarCodeModifier1);

                if (results?.Count > 0)
                {
                    this.ProdUnitBarCode.ProductUnitBarCodeRefno = Convert.ToInt32(results[0].ProductUnitBarCodeRefno);
                    this.ProdUnitBarCode.ProductUnitRefno = Convert.ToInt32(results[0].ProductUnitRefno);
                }
            }
        }

        private async Task SumwaFarmWholeDuckRule()
        {
            if (this.ProdUnitBarCode.ProductUnitBarCodeRefno == 0 && this.ProdUnitBarCode.ProductUnitRefno == 0)
            {
                this.ProdUnitBarCode.ProductUnitBarCodeRefno = 0;
                this.ProdUnitBarCode.ProductUnitRefno = 0;

                // Attempt to find the product in local database.
                // Do not look up GTIN in bar code database for this rule, product will be identified by a range
                // of fixed weights.
                if (this.SysEnv.BarCodeSettings.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct &&
                    this.BarCodeParse.ProductIdentifier == "90695955110022")
                {
                    this.BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct();
                    var prodInfo = await ProductInfoTasks.GetInfoFromCodeBcUnit(
                        this.BarCodeParse.ProductIdentifier,
                        string.Empty,
                        "CS");
                    this.ProdUnitBarCode.ProductUnitBarCodeRefno = 0;
                    this.ProdUnitBarCode.ProductUnitRefno = prodInfo.UnitRefno;
                }
            }
        }

        private void BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct()
        {
            if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(11.00) &&
                this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(11.99))
            {
                this.BarCodeParse.ProductIdentifier = "MC10448";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.00) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.49))
            {
                this.BarCodeParse.ProductIdentifier = "MC10443";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.50) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.99))
            {
                this.BarCodeParse.ProductIdentifier = "MC10441";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.00) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.49))
            {
                this.BarCodeParse.ProductIdentifier = "MC10442";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.50) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.99))
            {
                this.BarCodeParse.ProductIdentifier = "MC10440";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.00) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.49))
            {
                this.BarCodeParse.ProductIdentifier = "MC10445";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.50) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.99))
            {
                this.BarCodeParse.ProductIdentifier = "MC10446";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.00) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(15.49))
            {
                this.BarCodeParse.ProductIdentifier = "MC10447";
            }
            else if (this.BarCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.50) &&
                     this.BarCodeParse.NetWeightMetricKg <= Convert.ToDecimal(16.00))
            {
                this.BarCodeParse.ProductIdentifier = "MC10450";
            }
            else
            {
                throw new Exception("Unable to identify product scanned." + Environment.NewLine +
                                    "ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct enabled.");
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
    }
}
