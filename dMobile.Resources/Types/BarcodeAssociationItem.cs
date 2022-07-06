// <copyright file="BarcodeAssociationItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    //sql.Append("SELECT tActivityItem.ActivityItemRefno,"); int
    //sql.Append("tProduct.ProductCode AS ProductCode,"); string
    //sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,"); int
    //sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,"); string
    //sql.Append("tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,"); bool
    //sql.Append("tUnit.UnitCode AS UnitCode,"); string 
    //sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno AS ProductUnitBarCodeRefno,"); int 
    //sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode AS ProductUnitBarCodeCode,"); string
    //sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1 AS ProductUnitBarCodeModifier1,"); string
    //sql.Append("tProductUnitBarCode.ProductUnitBarCodeSymbologyEnum AS ProductUnitBarCodeSymbologyEnum,"); int
    //sql.Append("tActivityItem.ActivityItemStatus AS ActivityStatus "); int
    //sql.Append("FROM tActivityItem ");
    public class BarcodeAssociationItem
    {
        private string productUnitBarCodeModifier1;
        private int activityItemStatus;

        public int ActivityItemRefno { get; set; }
        public string ProductCode { get; set; }
        public int ProductUnitRefno { get; set; }
        public string ProductUnitBC { get; set; }
        public bool ProductUnitIsBC { get; set; }
        public string UnitCode { get; set; }
        public int ProductUnitBarCodeRefno { get; set; }
        public string ProductUnitBarCodeCode { get; set; }
        public string ProductUnitBarCodeModifier1 { get => this.productUnitBarCodeModifier1; set => this.productUnitBarCodeModifier1 = value; }
        public int ProductUnitBarCodeSymbologyEnum { get; set; }
        public int ActivityItemStatus { get => this.activityItemStatus; set => this.activityItemStatus = value; }
    }
}
