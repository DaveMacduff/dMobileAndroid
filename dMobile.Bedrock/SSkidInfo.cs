// <copyright file="SSkidInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// <see cref="SSkidInfo" /> Skid information.
    /// </summary>
    public class SSkidInfo : IEquatable<SSkidInfo>
    {
        /// <summary>
        /// Gets or sets the product scheme data.
        /// </summary>
        public ProductSchemeData SchemeData { get; set; } = new ProductSchemeData();

        /// <summary>
        /// Gets or sets a value indicating whether the skid is put away.
        /// </summary>
        public bool IsPutAway { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the skid is consumed.
        /// </summary>
        public bool IsConsumed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the skid is completed.
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Gets or sets the skid foreign key reference number.
        /// </summary>
        public int SkidFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid cube unit.
        /// </summary>
        public string CubeUnit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cube.
        /// </summary>
        public decimal Cube { get; set; } = 0;

        /// <summary>
        /// Gets or sets the height unit.
        /// </summary>
        public string HeightUnit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public decimal Height { get; set; } = 0;

        /// <summary>
        /// Gets or sets the Hi.
        /// </summary>
        public int Hi { get; set; } = 0;

        /// <summary>
        /// Gets or sets the Ti.
        /// </summary>
        public int Ti { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the skid is on hold.
        /// </summary>
        public bool IsHold { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the skid has a CHEP pallet.
        /// </summary>
        public bool IsCHEP { get; set; } = false;

        /// <summary>
        /// Gets or sets the user entered or form generated skid code.
        /// </summary>
        public string SkidCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the GS1 18 digit skid code. Including the company prefix.
        /// </summary>
        public string LogisticUnitSkidCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the skid quantity.
        /// </summary>
        public decimal Quantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid base quantity.
        /// </summary>
        public decimal BaseQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid weight quantity.
        /// </summary>
        public decimal WeightQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid product information.
        /// </summary>
        public ProductInfo ProdInfo { get; set; } = new ProductInfo();

        /// <summary>
        /// Gets the location map reference.
        /// </summary>
        public int LocMapRefno
        {
            get
            {
                var locMapRefno = 0;
                if (this.LocMap != null)
                {
                    if (this.LocMap.SlotLocMapRefno > 0)
                    {
                        locMapRefno = this.LocMap.SlotLocMapRefno;
                    }
                    else if (this.LocMap.SectLocMapRefno > 0)
                    {
                        locMapRefno = this.LocMap.SectLocMapRefno;
                    }
                    else
                    {
                        locMapRefno = this.LocMap.WhsLocMapRefno;
                    }
                }

                return locMapRefno;
            }
        }

        /// <summary>
        /// Gets or sets the location map.
        /// </summary>
        public SLocMap LocMap { get; set; } = new SLocMap();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SSkidInfo other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var isEquals = true;
            isEquals &= Equals(this.SchemeData, other.SchemeData);
            isEquals &= this.IsPutAway == other.IsPutAway;
            isEquals &= this.IsConsumed == other.IsConsumed;
            isEquals &= this.IsCompleted == other.IsCompleted;
            isEquals &= this.SkidFKRefno == other.SkidFKRefno;
            isEquals &= this.CubeUnit == other.CubeUnit;
            isEquals &= this.Cube == other.Cube;
            isEquals &= this.HeightUnit == other.HeightUnit;
            isEquals &= this.Height == other.Height;
            isEquals &= this.Hi == other.Hi;
            isEquals &= this.Ti == other.Ti;
            isEquals &= this.IsHold == other.IsHold;
            isEquals &= this.IsCHEP == other.IsCHEP;
            isEquals &= this.SkidCode == other.SkidCode;
            isEquals &= this.LogisticUnitSkidCode == other.LogisticUnitSkidCode;
            isEquals &= this.Quantity == other.Quantity;
            isEquals &= this.BaseQuantity == other.BaseQuantity;
            isEquals &= this.WeightQuantity == other.WeightQuantity;
            isEquals &= Equals(this.ProdInfo, other.ProdInfo);
            isEquals &= Equals(this.LocMap, other.LocMap);
            return isEquals;
        }

        /// <summary>
        ///    Compare two objects for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True or false for equality.</returns>
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

            return obj.GetType() == this.GetType() && this.Equals((SSkidInfo)obj);
        }

        /// <summary>
        ///     Calculate the hash code.
        /// </summary>
        /// <returns>Calculated hash value.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(this.SchemeData);
            hashCode.Add(this.IsPutAway);
            hashCode.Add(this.IsConsumed);
            hashCode.Add(this.IsCompleted);
            hashCode.Add(this.SkidFKRefno);
            hashCode.Add(this.CubeUnit);
            hashCode.Add(this.Cube);
            hashCode.Add(this.HeightUnit);
            hashCode.Add(this.Height);
            hashCode.Add(this.Hi);
            hashCode.Add(this.Ti);
            hashCode.Add(this.IsHold);
            hashCode.Add(this.IsCHEP);
            hashCode.Add(this.SkidCode);
            hashCode.Add(this.LogisticUnitSkidCode);
            hashCode.Add(this.Quantity);
            hashCode.Add(this.BaseQuantity);
            hashCode.Add(this.WeightQuantity);
            hashCode.Add(this.ProdInfo);
            hashCode.Add(this.LocMap);
            return hashCode.ToHashCode();
        }
    }
}
