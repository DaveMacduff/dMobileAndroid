// <copyright file="ProductUnitRelationModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Products
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the relationship between product units.
    /// </summary>
    [Table("tProductUnitRelation")]
    public class ProductUnitRelationModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product unit relation.
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int ProductUnitRelationRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the first product unit.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        [Indexed(Name = "FK_ProductUnitRelationOnProductUnit1Refno", Order = 1, Unique = false)]
        public int ProductUnit1Refno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the second product unit.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        [Indexed(Name = "FK_ProductUnitRelationOnProductUnit2Refno", Order = 1, Unique = false)]
        public int ProductUnit2Refno { get; set; }

        /// <summary>
        /// Gets or sets the factor of the product unit relation.
        /// </summary>
        [NotNull]
        public decimal ProductUnitRelationFactor { get; set; }
    }
}