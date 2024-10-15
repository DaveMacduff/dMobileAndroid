// <copyright file="ActivityItemReplenishmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable UnusedMember.Global
namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity replenishment item.
    /// </summary>
    [Table("tActivityItemReplenishment")]
    public class ActivityItemReplenishmentModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the movement type for the replenishment.
        /// </summary>
        /// <remarks>ReplenishmentMoveType.Code.</remarks>
        [NotNull]
        public int MoveType { get; set; } = (int)ReplenishmentMoveType.Code.Manual;

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the parent foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemFKParRefno", Order = 1, Unique = false)]
        public int ActivityItemFKParRefno { get; set; }

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
        /// Gets or sets the quantity of the item replenished.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReplenishmentQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item replenished.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReplenishmentBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item replenished.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReplenishmentWeightQuantity { get; set; }
    }
}