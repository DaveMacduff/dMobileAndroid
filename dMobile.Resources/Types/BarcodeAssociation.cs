// <copyright file="BarcodeAssociation.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class BarcodeAssociation
    {
        private int activityStatus;
        private string productUnitBarCodeModifier1;

        public int ActivityRefno { get; set; }
        public int ProductUnitBarCodeRefno { get; set; }
        public int ActivityStatus { get => this.activityStatus; set => this.activityStatus = value; }
        public string ProductCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitBarCodeCode { get; set; }
        public string ProductUnitBarCodeModifier1 { get => this.productUnitBarCodeModifier1; set => this.productUnitBarCodeModifier1 = value; }
    }
}
