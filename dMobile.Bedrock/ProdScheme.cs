// <copyright file="ProdScheme.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    ///     Defines the <see cref="ProdScheme" />.
    /// </summary>
    public class ProdScheme : IEquatable<ProdScheme>
    {
        /// <summary>
        ///     Gets or sets the scheme reference number.
        /// </summary>
        public int SchemeRefno { get; set; } = 0;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme is single trade item serial number tracked.
        /// </summary>
        public bool IsSerialTracked { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme is logistic unit production date tracked.
        /// </summary>
        public bool IsProductionDateTracked { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme is logistic unit batch lot number tracked.
        /// </summary>
        public bool IsLotTracked { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme is logistic unit best before sell by date tracked.
        /// </summary>
        public bool IsBestBeforeDateTracked { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme is logistic unit product variant tracked.
        /// </summary>
        public bool IsVariantTracked { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the tracked logistic unit batch lot number is a component of uniqueness.
        /// </summary>
        public bool IsLotUniqueComponent { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the tracked logistic unit production date is a component of uniqueness.
        /// </summary>
        public bool IsProductionDateUniqueComponent { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the tracked logistic unit best before sell by date is a component
        ///     of uniqueness.
        /// </summary>
        public bool IsBestBeforeDateUniqueComponent { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the tracked logistic unit product variant is a component of uniqueness.
        /// </summary>
        public bool IsVariantUniqueComponent { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the tracked logistic unit batch lot number can be defaulted.
        /// </summary>
        public bool IsLotTrackedDefault { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the tracked logistic unit best before sell by date can be defaulted.
        /// </summary>
        public bool IsBestBeforeDateTrackedDefault { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the tracked logistic unit production date can be defaulted.
        /// </summary>
        public bool IsProductionDateTrackedDefault { get; set; } = false;

        /// <summary>
        ///     Gets or sets the product scheme label settings.
        /// </summary>
        public ProdSchemeLabelSettings.ProdSchemeLabelInfo LabelInfo { get; set; } =
            new ProdSchemeLabelSettings.ProdSchemeLabelInfo();

        /// <summary>
        ///     Gets or sets the scheme extra 1 type.
        /// </summary>
        public int Extra1Type { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the scheme extra 1 caption.
        /// </summary>
        public string Extra1Caption { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme extra 1 is required.
        /// </summary>
        public bool Extra1IsRequired { get; set; } = false;

        /// <summary>
        ///     Gets or sets the scheme extra 2 type.
        /// </summary>
        public int Extra2Type { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the scheme extra 2 caption.
        /// </summary>
        public string Extra2Caption { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme extra 2 is required.
        /// </summary>
        public bool Extra2IsRequired { get; set; } = false;

        /// <summary>
        ///     Gets or sets the scheme extra 3 type.
        /// </summary>
        public int Extra3Type { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the scheme extra 3 caption.
        /// </summary>
        public string Extra3Caption { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme extra 3 is required.
        /// </summary>
        public bool Extra3IsRequired { get; set; } = false;

        /// <summary>
        ///     Gets or sets the scheme extra 4 type.
        /// </summary>
        public int Extra4Type { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the scheme extra 4 caption.
        /// </summary>
        public string Extra4Caption { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme extra 4 is required.
        /// </summary>
        public bool Extra4IsRequired { get; set; } = false;

        /// <summary>
        ///     Gets or sets the scheme extra 5 type.
        /// </summary>
        public int Extra5Type { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the scheme extra 5 caption.
        /// </summary>
        public string Extra5Caption { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme extra 5 is required.
        /// </summary>
        public bool Extra5IsRequired { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the scheme is base tracked quantity only.
        /// </summary>
        public bool IsBaseTracked { get; set; } = false;

        /// <summary>
        /// Perform a product scheme information deep copy.
        /// </summary>
        /// <param name="original"><see cref="ProdScheme"/> Original product scheme information.</param>
        /// <returns><see cref="ProductInfo"/> New copy of the original product scheme information.</returns>
        public static ProdScheme DeepCopy(ProdScheme original)
        {
            var serializer = new XmlSerializer(original.GetType());
            var stream = new MemoryStream();
            serializer.Serialize(stream, original);
            stream.Position = 0;

            return (ProdScheme)serializer.Deserialize(stream);
        }

        /// <inheritdoc/>
        public bool Equals(ProdScheme other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.SchemeRefno == other.SchemeRefno && this.IsSerialTracked == other.IsSerialTracked &&
                   this.IsProductionDateTracked == other.IsProductionDateTracked &&
                   this.IsLotTracked == other.IsLotTracked &&
                   this.IsBestBeforeDateTracked == other.IsBestBeforeDateTracked &&
                   this.IsVariantTracked == other.IsVariantTracked &&
                   this.IsLotUniqueComponent == other.IsLotUniqueComponent &&
                   this.IsProductionDateUniqueComponent == other.IsProductionDateUniqueComponent &&
                   this.IsBestBeforeDateUniqueComponent == other.IsBestBeforeDateUniqueComponent &&
                   this.IsVariantUniqueComponent == other.IsVariantUniqueComponent &&
                   this.IsLotTrackedDefault == other.IsLotTrackedDefault &&
                   this.IsBestBeforeDateTrackedDefault == other.IsBestBeforeDateTrackedDefault &&
                   this.IsProductionDateTrackedDefault == other.IsProductionDateTrackedDefault &&
                   this.LabelInfo.Equals(other.LabelInfo) && this.Extra1Type == other.Extra1Type &&
                   this.Extra1Caption == other.Extra1Caption && this.Extra1IsRequired == other.Extra1IsRequired &&
                   this.Extra2Type == other.Extra2Type && this.Extra2Caption == other.Extra2Caption &&
                   this.Extra2IsRequired == other.Extra2IsRequired && this.Extra3Type == other.Extra3Type &&
                   this.Extra3Caption == other.Extra3Caption && this.Extra3IsRequired == other.Extra3IsRequired &&
                   this.Extra4Type == other.Extra4Type && this.Extra4Caption == other.Extra4Caption &&
                   this.Extra4IsRequired == other.Extra4IsRequired && this.Extra5Type == other.Extra5Type &&
                   this.Extra5Caption == other.Extra5Caption && this.Extra5IsRequired == other.Extra5IsRequired &&
                   this.IsBaseTracked == other.IsBaseTracked;
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

            return obj.GetType() == this.GetType() && this.Equals((ProdScheme)obj);
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Not sure how to fix the warnings.")]
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(this.SchemeRefno);
            hashCode.Add(this.IsSerialTracked);
            hashCode.Add(this.IsProductionDateTracked);
            hashCode.Add(this.IsLotTracked);
            hashCode.Add(this.IsBestBeforeDateTracked);
            hashCode.Add(this.IsVariantTracked);
            hashCode.Add(this.IsLotUniqueComponent);
            hashCode.Add(this.IsProductionDateUniqueComponent);
            hashCode.Add(this.IsBestBeforeDateUniqueComponent);
            hashCode.Add(this.IsVariantUniqueComponent);
            hashCode.Add(this.IsLotTrackedDefault);
            hashCode.Add(this.IsBestBeforeDateTrackedDefault);
            hashCode.Add(this.IsProductionDateTrackedDefault);
            hashCode.Add(this.LabelInfo);
            hashCode.Add(this.Extra1Type);
            hashCode.Add(this.Extra1Caption);
            hashCode.Add(this.Extra1IsRequired);
            hashCode.Add(this.Extra2Type);
            hashCode.Add(this.Extra2Caption);
            hashCode.Add(this.Extra2IsRequired);
            hashCode.Add(this.Extra3Type);
            hashCode.Add(this.Extra3Caption);
            hashCode.Add(this.Extra3IsRequired);
            hashCode.Add(this.Extra4Type);
            hashCode.Add(this.Extra4Caption);
            hashCode.Add(this.Extra4IsRequired);
            hashCode.Add(this.Extra5Type);
            hashCode.Add(this.Extra5Caption);
            hashCode.Add(this.Extra5IsRequired);
            hashCode.Add(this.IsBaseTracked);
            return hashCode.ToHashCode();
        }
    }
}