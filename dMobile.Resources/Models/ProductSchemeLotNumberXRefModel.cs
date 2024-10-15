// <copyright file="ProductSchemeLotNumberXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the cross-reference between product schemes and their lot numbers.
    /// </summary>
    [Table("tProductSchemeLotNumberXRef")]
    public class ProductSchemeLotNumberXRefModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product scheme lot number cross-reference.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductSchemeLotNumberXRefRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductSchemeModel))]
        [Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme lot number.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductSchemeLotNumberModel))]
        [Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeLotNumberRefno", Order = 1, Unique = false)]
        public int ProductSchemeLotNumberRefno { get; set; }
    }
}