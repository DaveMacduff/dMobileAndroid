using System;
namespace dMobile.Resources.Types
{
    public class Skid
    {
        private int skidFKRefno;
        private int productRefno;
        private int productUnitRefno;
        private int locationMapRefno;
        private int skidTi;
        private int skidHi;
        private string skidCode;
        private string productCode;
        private string productUnitBC;
        private string unitCode;
        private string baseUnitCode;
        private string weightUnitCode;
        private string productDescription;
        private string productUnitSizeDescription;
        private string locationName;
        private string skidCubeUnit;
        private string skidHeightUnit;
        private decimal quantity;
        private decimal baseQuantity;
        private decimal weightQuantity;
        private decimal skidCube;
        private decimal skidHeight;
        private DateTime skidCreated;
        private DateTime skidCompleted;
        private bool productUnitIsBC;
        private bool skidIsCHEP;
        private bool skidIsOnHold;

        public int SkidFKRefno { get => skidFKRefno; set => skidFKRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int SkidTi { get => skidTi; set => skidTi = value; }
        public int SkidHi { get => skidHi; set => skidHi = value; }
        public string SkidCode { get => skidCode; set => skidCode = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string BaseUnitCode { get => baseUnitCode; set => baseUnitCode = value; }
        public string WeightUnitCode { get => weightUnitCode; set => weightUnitCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public string SkidCubeUnit { get => skidCubeUnit; set => skidCubeUnit = value; }
        public string SkidHeightUnit { get => skidHeightUnit; set => skidHeightUnit = value; }
        public decimal Quantity { get => quantity; set => quantity = value; }
        public decimal BaseQuantity { get => baseQuantity; set => baseQuantity = value; }
        public decimal WeightQuantity { get => weightQuantity; set => weightQuantity = value; }
        public decimal SkidCube { get => skidCube; set => skidCube = value; }
        public decimal SkidHeight { get => skidHeight; set => skidHeight = value; }
        public DateTime SkidCreated { get => skidCreated; set => skidCreated = value; }
        public DateTime SkidCompleted { get => skidCompleted; set => skidCompleted = value; }
        public bool ProductUnitIsBC { get => productUnitIsBC; set => productUnitIsBC = value; }
        public bool SkidIsCHEP { get => skidIsCHEP; set => skidIsCHEP = value; }
        public bool SkidIsOnHold { get => skidIsOnHold; set => skidIsOnHold = value; }
    }
}
