// <copyright file="Skid.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Skid
    {
        private int skidHi;
        private string skidHeightUnit;
        private decimal skidHeight;
        private DateTime skidCompleted;
        private bool skidIsOnHold;

        public int SkidFKRefno { get; set; }
        public int ProductRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int SkidTi { get; set; }
        public int SkidHi { get => this.skidHi; set => this.skidHi = value; }
        public string SkidCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string UnitCode { get; set; }
        public string BaseUnitCode { get; set; }
        public string WeightUnitCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public string LocationName { get; set; }
        public string SkidCubeUnit { get; set; }
        public string SkidHeightUnit { get => this.skidHeightUnit; set => this.skidHeightUnit = value; }
        public decimal Quantity { get; set; }
        public decimal BaseQuantity { get; set; }
        public decimal WeightQuantity { get; set; }
        public decimal SkidCube { get; set; }
        public decimal SkidHeight { get => this.skidHeight; set => this.skidHeight = value; }
        public DateTime SkidCreated { get; set; }
        public DateTime SkidCompleted { get => this.skidCompleted; set => this.skidCompleted = value; }
        public bool ProductUnitIsBC { get; set; }
        public bool SkidIsCHEP { get; set; }
        public bool SkidIsOnHold { get => this.skidIsOnHold; set => this.skidIsOnHold = value; }
    }
}
