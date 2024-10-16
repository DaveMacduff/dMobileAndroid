// <copyright file="ProductSchemeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Products
{
    using SQLite;

    /// <summary>
    /// Represents a product scheme.
    /// </summary>
    [Table("tProductScheme")]
    public class ProductSchemeModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product scheme.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductSchemeRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the product scheme.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ProductSchemeOnProductSchemeCode", Order = 1, Unique = true)]
        public string ProductSchemeCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is serial tracked.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsSerialTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is production date tracked.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsProductionDateTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is lot tracked.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsLotTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is best before date tracked.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsBestBeforeDateTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is variant tracked.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsVariantTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme has a unique lot component.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsLotUniqueComponent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme has a unique production date component.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsProductionDateUniqueComponent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme has a unique best before date component.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsBestBeforeDateUniqueComponent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme has a unique variant component.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsVariantUniqueComponent { get; set; }

        /// <summary>
        /// Gets or sets the type of the first extra field.
        /// </summary>
        [NotNull]
        public int ProductSchemeExtra1Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the second extra field.
        /// </summary>
        [NotNull]
        public int ProductSchemeExtra2Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the third extra field.
        /// </summary>
        [NotNull]
        public int ProductSchemeExtra3Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the fourth extra field.
        /// </summary>
        [NotNull]
        public int ProductSchemeExtra4Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the fifth extra field.
        /// </summary>
        [NotNull]
        public int ProductSchemeExtra5Type { get; set; }

        /// <summary>
        /// Gets or sets the caption of the first extra field.
        /// </summary>
        [NotNull]
        public string ProductSchemeExtra1Caption { get; set; }

        /// <summary>
        /// Gets or sets the caption of the second extra field.
        /// </summary>
        [NotNull]
        public string ProductSchemeExtra2Caption { get; set; }

        /// <summary>
        /// Gets or sets the caption of the third extra field.
        /// </summary>
        [NotNull]
        public string ProductSchemeExtra3Caption { get; set; }

        /// <summary>
        /// Gets or sets the caption of the fourth extra field.
        /// </summary>
        [NotNull]
        public string ProductSchemeExtra4Caption { get; set; }

        /// <summary>
        /// Gets or sets the caption of the fifth extra field.
        /// </summary>
        [NotNull]
        public string ProductSchemeExtra5Caption { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the first extra field is required.
        /// </summary>
        [NotNull]
        public bool ProductSchemeExtra1IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the second extra field is required.
        /// </summary>
        [NotNull]
        public bool ProductSchemeExtra2IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the third extra field is required.
        /// </summary>
        [NotNull]
        public bool ProductSchemeExtra3IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the fourth extra field is required.
        /// </summary>
        [NotNull]
        public bool ProductSchemeExtra4IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the fifth extra field is required.
        /// </summary>
        [NotNull]
        public bool ProductSchemeExtra5IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is lot tracked by default.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsLotTrackedDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is production date tracked by default.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsProductionDateTrackedDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme is best before date tracked by default.
        /// </summary>
        [NotNull]
        public bool ProductSchemeIsBestBeforeDateTrackedDefault { get; set; }
    }
}