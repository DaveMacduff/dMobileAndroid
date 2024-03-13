// <copyright file="ProductInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// <see cref="ProductInfo"/> Product information class.
    /// </summary>
    [Serializable]
    public class ProductInfo : IEquatable<ProductInfo>
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
        public string UnitBc { get; set; } = string.Empty;

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
        public bool IsBaseWeightUnit => this.BaseUnitCode == "LB" || this.BaseUnitCode == "KG";

        /// <summary>
        ///     Gets or sets the product accept days.
        /// </summary>
        public int ProductAcceptDays { get; set; } = 0;

        /// <summary>
        /// Perform a product information deep copy.
        /// </summary>
        /// <param name="original"><see cref="ProductInfo"/> Original product information.</param>
        /// <returns><see cref="ProductInfo"/> New copy of the original product information.</returns>
        public static ProductInfo DeepCopy(ProductInfo original)
        {
            var serializer = new XmlSerializer(original.GetType());
            var stream = new MemoryStream();
            serializer.Serialize(stream, original);
            stream.Position = 0;

            return (ProductInfo)serializer.Deserialize(stream);
        }

        public bool Equals(ProductInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.IsAllowConvertProduct == other.IsAllowConvertProduct
                   && this.ProductAcceptDays == other.ProductAcceptDays
                   && this.IsCatchAskZero == other.IsCatchAskZero
                   && this.BoxCount == other.BoxCount
                   && this.UnitIsSellUnit == other.UnitIsSellUnit
                   && this.UnitIsDefault == other.UnitIsDefault
                   && this.UnitRefno == other.UnitRefno
                   && this.FKRefno == other.FKRefno
                   && this.Refno == other.Refno
                   && this.Code == other.Code
                   && this.Desc == other.Desc
                   && this.BrandDesc == other.BrandDesc
                   && this.UnitIsBC == other.UnitIsBC
                   && this.UnitBc == other.UnitBc
                   && this.UnitCode == other.UnitCode
                   && this.UnitSizeDesc == other.UnitSizeDesc
                   && this.BaseUnitCode == other.BaseUnitCode
                   && this.WeightUnitCode == other.WeightUnitCode
                   && Equals(this.Scheme, other.Scheme)
                   && this.Unit2BaseUnitFactor == other.Unit2BaseUnitFactor
                   && this.UnitGrossWeight == other.UnitGrossWeight
                   && this.IsCatchCount == other.IsCatchCount
                   && this.IsCatchWeight == other.IsCatchWeight
                   && this.IsCatchAsk == other.IsCatchAsk
                   && this.WeightTolerance == other.WeightTolerance;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((ProductInfo)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(this.IsAllowConvertProduct);
            hashCode.Add(this.ProductAcceptDays);
            hashCode.Add(this.IsCatchAskZero);
            hashCode.Add(this.BoxCount);
            hashCode.Add(this.UnitIsSellUnit);
            hashCode.Add(this.UnitIsDefault);
            hashCode.Add(this.UnitRefno);
            hashCode.Add(this.FKRefno);
            hashCode.Add(this.Refno);
            hashCode.Add(this.Code);
            hashCode.Add(this.Desc);
            hashCode.Add(this.BrandDesc);
            hashCode.Add(this.UnitIsBC);
            hashCode.Add(this.UnitBc);
            hashCode.Add(this.UnitCode);
            hashCode.Add(this.UnitSizeDesc);
            hashCode.Add(this.BaseUnitCode);
            hashCode.Add(this.WeightUnitCode);
            hashCode.Add(this.Scheme);
            hashCode.Add(this.Unit2BaseUnitFactor);
            hashCode.Add(this.UnitGrossWeight);
            hashCode.Add(this.IsCatchCount);
            hashCode.Add(this.IsCatchWeight);
            hashCode.Add(this.IsCatchAsk);
            hashCode.Add(this.WeightTolerance);
            return hashCode.ToHashCode();
        }
    }
}