// <copyright file="ActivityItemBarCodeAssociationXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a cross-reference between an activity item and a product unit barcode.
    /// </summary>
    [Table("tActivityItemBarCodeAssociationXRef")]
    public class ActivityItemBarCodeAssociationXRefModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [Indexed(Name = "FK_ActivityItemBarCodeAssociationXRefOnActivityItemRefno", Order = 1, Unique = false)]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit barcode.
        /// </summary>
        [Indexed(Name = "FK_ActivityItemBarCodeAssociationXRefOnProductUnitBarCodeRefno", Order = 1, Unique = false)]
        [NotNull]
        [ForeignKey(typeof(ProductUnitBarCodeModel))]
        public int ProductUnitBarCodeRefno { get; set; }
    }
}