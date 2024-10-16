// <copyright file="ProductSchemeVariantXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Products
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the cross-reference between product schemes and their variants.
    /// </summary>
    [Table("tProductSchemeVariantXRef")]
    public class ProductSchemeVariantXRefModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product scheme variant cross-reference.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductSchemeVariantXRefRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductSchemeModel))]
        [Indexed(Name = "FK_ProductSchemeVariantXRefOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme variant.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductSchemeVariantModel))]
        [Indexed(Name = "FK_ProductSchemeVariantXRefOnProductSchemeVariantRefno", Order = 1, Unique = false)]
        public int ProductSchemeVariantRefno { get; set; }
    }
}