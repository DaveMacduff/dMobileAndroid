// ----------------------------------------------------------------------
// <copyright file="ProductInfo.cs" company="InterSect Business Systems Inc.">
//     Copyright (2013). All right reserved
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProductInfo
    {
        private int fkRefno = 0;
        private int unitRefno = 0;
        private int refno = 0;
        private string code = string.Empty;
        private string desc = string.Empty;
        private string brandDesc = string.Empty;
        private string unitBC = string.Empty;
        private bool unitIsBC = false;
        private string unitCode = string.Empty;
        private string unitSizeDesc = string.Empty;
        private string baseUnitCode = string.Empty;
        private string weightUnitCode = string.Empty;
        private decimal unit2BaseUnitFactor = 0;
        private decimal unitWeight = 0;
        private bool isCatchCount = false;
        private bool isCatchWeight = false;
        private bool isCatchAsk = false;
        private bool isCatchAskZero = false;
        private bool isAllowConvertProduct = false;
        private ProdScheme scheme = new ProdScheme();
        private bool unitIsDefault = false;
        private bool unitIsSellUnit = false;
        private decimal boxCount = 0;

        /// <summary>
        /// Checks if a product is allowed to be converted to another product.
        /// Part of ConvertProduct or ConvertLabel.
        /// </summary>
        public bool IsAllowConvertProduct
        {
            get
            {
                return this.isAllowConvertProduct;
            }
            set
            {
                this.isAllowConvertProduct = value;
            }
        }

        public bool IsCatchAskZero
        {
            get
            {
                return this.isCatchAskZero;
            }

            set
            {
                this.isCatchAskZero = value;
            }
        }

        public decimal BoxCount
        {
            get
            {
                return this.boxCount;
            }

            set
            {
                this.boxCount = value;
            }
        }

        public bool UnitIsSellUnit
        {
            get
            {
                return this.unitIsSellUnit;
            }

            set
            {
                this.unitIsSellUnit = value;
            }
        }

        public bool UnitIsDefault
        {
            get
            {
                return this.unitIsDefault;
            }

            set
            {
                this.unitIsDefault = value;
            }
        }

        public int UnitRefno
        {
            get
            {
                return this.unitRefno;
            }

            set
            {
                this.unitRefno = value;
            }
        }

        public int FKRefno
        {
            get
            {
                return this.fkRefno;
            }

            set
            {
                this.fkRefno = value;
            }
        }

        public int Refno
        {
            get
            {
                return this.refno;
            }

            set
            {
                this.refno = value;
            }
        }

        public string Code
        {
            get
            {
                return this.code;
            }

            set
            {
                this.code = value;
            }
        }

        public string Desc
        {
            get
            {
                return this.desc;
            }

            set
            {
                this.desc = value;
            }
        }

        public string BrandDesc
        {
            get
            {
                return this.brandDesc;
            }

            set
            {
                this.brandDesc = value;
            }
        }

        public bool UnitIsBC
        {
            get
            {
                return this.unitIsBC;
            }

            set
            {
                this.unitIsBC = value;
            }
        }

        public string UnitBC
        {
            get
            {
                return this.unitBC;
            }

            set
            {
                this.unitBC = value;
            }
        }

        public string UnitCode
        {
            get
            {
                return this.unitCode;
            }

            set
            {
                this.unitCode = value;
            }
        }

        public string UnitSizeDesc
        {
            get
            {
                return this.unitSizeDesc;
            }

            set
            {
                this.unitSizeDesc = value;
            }
        }

        public string BaseUnitCode
        {
            get
            {
                return this.baseUnitCode;
            }

            set
            {
                this.baseUnitCode = value;
            }
        }

        public string WeightUnitCode
        {
            get
            {
                return this.weightUnitCode;
            }

            set
            {
                this.weightUnitCode = value;
            }
        }

        public ProdScheme Scheme
        {
            get
            {
                return this.scheme;
            }

            set
            {
                this.scheme = value;
            }
        }

        public decimal Unit2BaseUnitFactor
        {
            get
            {
                return this.unit2BaseUnitFactor;
            }

            set
            {
                this.unit2BaseUnitFactor = value;
            }
        }

        public decimal UnitWeight
        {
            get
            {
                return this.unitWeight;
            }

            set
            {
                this.unitWeight = value;
            }
        }

        public bool IsCatchCount
        {
            get
            {
                return this.isCatchCount;
            }

            set
            {
                this.isCatchCount = value;
            }
        }

        public bool IsCatchWeight
        {
            get
            {
                return this.isCatchWeight;
            }

            set
            {
                this.isCatchWeight = value;
            }
        }

        public bool IsCatchAsk
        {
            get
            {
                return this.isCatchAsk;
            }

            set
            {
                this.isCatchAsk = value;
            }
        }
    }
}
