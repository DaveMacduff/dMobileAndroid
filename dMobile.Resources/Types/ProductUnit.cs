// <copyright file="ProductUnit.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class ProductUnit
    {
        private int productRefno;
        private string productUnitBC;
        private bool productUnitIsSellUnit;

        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get => this.productRefno; set => this.productRefno = value; }
        public string ProductDescription { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public string BaseUnitCode { get; set; }
        public string WeightUnitCode { get; set; }
        public string ProductUnitBC { get => this.productUnitBC; set => this.productUnitBC = value; }
        public bool ProductUnitIsBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public bool ProductUnitIsSellUnit { get => this.productUnitIsSellUnit; set => this.productUnitIsSellUnit = value; }
    }
}
