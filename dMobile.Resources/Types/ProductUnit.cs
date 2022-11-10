// <copyright file="ProductUnit.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class ProductUnit
    {
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public string BaseUnitCode { get; set; }
        public string WeightUnitCode { get; set; }
        public string ProductUnitBC { get; set; }
        public bool ProductUnitIsBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public bool ProductUnitIsSellUnit { get; set; }
    }
}
