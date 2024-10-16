// <copyright file="ActivityItemCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Items
{
    using LocationMap;
    using Products;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a count activity item.
    /// </summary>
    [Table("tActivityItemCount")]
    public class ActivityItemCountModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemCountOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the quantity on hand.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCountQuantityOnHand { get; set; }

        /// <summary>
        /// Gets or sets the base quantity on hand.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCountBaseQuantityOnHand { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity on hand.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCountWeightQuantityOnHand { get; set; }

        /// <summary>
        /// Gets or sets the count quantity.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCountQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base count quantity.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCountBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight count quantity.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCountWeightQuantity { get; set; }
    }
}
