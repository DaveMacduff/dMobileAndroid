// <copyright file="ProductInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// <see cref="ProductInfo"/> Product information class.
    /// </summary>
    [Serializable]
    public class ProductInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether a product is allowed to be converted to another
        /// product. Part of ConvertProduct or ConvertLabel.
        /// </summary>
        public bool IsAllowConvertProduct { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether a product is a catch ask product.
        /// </summary>
        public bool IsCatchAskZero { get; set; } = false;

        /// <summary>
        /// Gets or sets the box count quantity.
        /// </summary>
        public decimal BoxCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is a sell unit.
        /// </summary>
        public bool UnitIsSellUnit { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is the default unit.
        /// </summary>
        public bool UnitIsDefault { get; set; } = false;

        /// <summary>
        /// Gets or sets the product unit reference number.
        /// </summary>
        public int UnitRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product foreign key reference number. (iv_mast.refno).
        /// </summary>
        public int FKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product reference number.
        /// </summary>
        public int Refno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        public string Desc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product brand description.
        /// </summary>
        public string BrandDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the unit is broken case.
        /// </summary>
        public bool UnitIsBC { get; set; } = false;

        /// <summary>
        /// Gets or sets the product unit broken case string. "X".
        /// </summary>
        public string UnitBC { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit of measure code.
        /// </summary>
        public string UnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit size description.
        /// </summary>
        public string UnitSizeDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product base unit of measure code.
        /// </summary>
        public string BaseUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product weight unit of measure code.
        /// </summary>
        public string WeightUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product tracking scheme information.
        /// </summary>
        public ProdScheme Scheme { get; set; } = new ProdScheme();

        /// <summary>
        /// Gets or sets the product unit to base unit factor.
        /// </summary>
        public decimal Unit2BaseUnitFactor { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product unit gross weight.
        /// </summary>
        public decimal UnitGrossWeight { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch count.
        /// </summary>
        public bool IsCatchCount { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch weight.
        /// </summary>
        public bool IsCatchWeight { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch ask.
        /// </summary>
        public bool IsCatchAsk { get; set; } = false;

        /// <summary>
        /// Gets or sets a product units weight tolerance.
        /// </summary>
        public decimal WeightTolerance { get; set; } = 0.00M;

        /// <summary>
        /// Gets a value indicating whether the base unit is a weight unit of measure unit.
        /// </summary>
        public bool IsBaseWeightUnit
        {
            get
            {
                var isWeightUnit = false;
                if (this.BaseUnitCode == "LB" || this.BaseUnitCode == "KG")
                {
                    isWeightUnit = true;
                }

                return isWeightUnit;
            }
        }
    }
}