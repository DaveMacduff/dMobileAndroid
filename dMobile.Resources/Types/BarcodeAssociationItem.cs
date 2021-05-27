using System;
namespace dMobile.Resources.Types
{
    //sql.Append("SELECT tActivityItem.ActivityItemRefno AS ActivityItemRefno,"); int
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
        private int activityItemRefno;
        private string productCode;
        private int productUnitRefno;
        private string productUnitBC;
        private bool productUnitIsBC;
        private string unitCode;
        private int productUnitBarCodeRefno;
        private string productUnitBarCodeCode;
        private string productUnitBarCodeModifier1;
        private int productUnitBarCodeSymbologyEnum;
        private int activityItemStatus;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public bool ProductUnitIsBC { get => productUnitIsBC; set => productUnitIsBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public int ProductUnitBarCodeRefno { get => productUnitBarCodeRefno; set => productUnitBarCodeRefno = value; }
        public string ProductUnitBarCodeCode { get => productUnitBarCodeCode; set => productUnitBarCodeCode = value; }
        public string ProductUnitBarCodeModifier1 { get => productUnitBarCodeModifier1; set => productUnitBarCodeModifier1 = value; }
        public int ProductUnitBarCodeSymbologyEnum { get => productUnitBarCodeSymbologyEnum; set => productUnitBarCodeSymbologyEnum = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
    }
}
