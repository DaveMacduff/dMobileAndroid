// <copyright file="ActivityDetailProductSchemeVariantXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Details
{
    using dMobile.Resources.Models.Products;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a cross-reference between an activity detail and a product scheme variant.
    /// </summary>
    [Table("tActivityDetailProductSchemeVariantXRef")]
    public class ActivityDetailProductSchemeVariantXRefModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity detail.
        /// </summary>
        [Indexed(Name = "UQ_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefno", Order = 1, Unique = true)]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme variant.
        /// </summary>
        [Indexed(Name = "FK_ActivityDetailProductSchemeVariantXRefOnProductSchemeVariantRefno", Order = 1, Unique = false)]
        [NotNull]
        [ForeignKey(typeof(ProductSchemeVariantModel))]
        public int ProductSchemeVariantRefno { get; set; }
    }
}