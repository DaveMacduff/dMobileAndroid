// <copyright file="ProductSchemeData.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    ///     Define the product scheme data.
    /// </summary>
    public class ProductSchemeData : IEquatable<ProductSchemeData>
    {
        /// <summary>
        ///     Gets or sets the single trade item serial number.
        /// </summary>
        public string SingleTradeItemSerialNumber { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the logistic unit batch lot number.
        /// </summary>
        public string LogisticUnitBatchLotNumber { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the logistic unit best before sell by date.
        /// </summary>
        public DateTime LogisticUnitBestBeforeSellByDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        ///     Gets or sets the logistic unit expiry use by date.
        /// </summary>
        public DateTime LogisticUnitExpiryUseByDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        ///     Gets or sets the logistic unit production date.
        /// </summary>
        public DateTime LogisticUnitProductionDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        ///     Gets or sets the logistic unit due date.
        /// </summary>
        public DateTime LogisticUnitDueDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        ///     Gets or sets the logistic unit packaging date.
        /// </summary>
        public DateTime LogisticUnitPackagingDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        ///     Gets or sets the logistic unit skid number.
        /// </summary>
        public string LogisticUnitSkidNumber { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the variant code.
        /// </summary>
        public string VariantCode { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the variant number.
        /// </summary>
        public int VariantNumber { get; set; }

        /// <summary>
        ///     Gets or sets variant description.
        /// </summary>
        public string VariantDescription { get; set; } = string.Empty;

        /// <inheritdoc/>
        public bool Equals(ProductSchemeData other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.SingleTradeItemSerialNumber == other.SingleTradeItemSerialNumber &&
                   this.LogisticUnitBatchLotNumber == other.LogisticUnitBatchLotNumber &&
                   this.LogisticUnitBestBeforeSellByDate.Equals(other.LogisticUnitBestBeforeSellByDate) &&
                   this.LogisticUnitExpiryUseByDate.Equals(other.LogisticUnitExpiryUseByDate) &&
                   this.LogisticUnitProductionDate.Equals(other.LogisticUnitProductionDate) &&
                   this.LogisticUnitDueDate.Equals(other.LogisticUnitDueDate) &&
                   this.LogisticUnitPackagingDate.Equals(other.LogisticUnitPackagingDate) &&
                   this.LogisticUnitSkidNumber == other.LogisticUnitSkidNumber &&
                   this.VariantCode == other.VariantCode && this.VariantNumber == other.VariantNumber &&
                   this.VariantDescription == other.VariantDescription;
        }

        /// <inheritdoc/>
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

            return obj.GetType() == this.GetType() && this.Equals((ProductSchemeData)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(this.SingleTradeItemSerialNumber);
            hashCode.Add(this.LogisticUnitBatchLotNumber);
            hashCode.Add(this.LogisticUnitBestBeforeSellByDate);
            hashCode.Add(this.LogisticUnitExpiryUseByDate);
            hashCode.Add(this.LogisticUnitProductionDate);
            hashCode.Add(this.LogisticUnitDueDate);
            hashCode.Add(this.LogisticUnitPackagingDate);
            hashCode.Add(this.LogisticUnitSkidNumber);
            hashCode.Add(this.VariantCode);
            hashCode.Add(this.VariantNumber);
            hashCode.Add(this.VariantDescription);
            return hashCode.ToHashCode();
        }
    }
}