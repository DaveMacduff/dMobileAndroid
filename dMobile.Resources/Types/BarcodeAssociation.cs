using System;
namespace dMobile.Resources.Types
{
    public class BarcodeAssociation
    {
        private int activityRefno;
        private int productUnitBarCodeRefno;
        private int activityStatus;
        private string productCode;
        private string productUnitBC;
        private string unitCode;
        private string productUnitBarCodeCode;
        private string productUnitBarCodeModifier1;

        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ProductUnitBarCodeRefno { get => productUnitBarCodeRefno; set => productUnitBarCodeRefno = value; }
        public int ActivityStatus { get => activityStatus; set => activityStatus = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductUnitBarCodeCode { get => productUnitBarCodeCode; set => productUnitBarCodeCode = value; }
        public string ProductUnitBarCodeModifier1 { get => productUnitBarCodeModifier1; set => productUnitBarCodeModifier1 = value; }
    }
}
