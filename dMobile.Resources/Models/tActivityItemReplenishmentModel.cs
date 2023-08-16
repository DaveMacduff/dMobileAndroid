// <copyright file="tActivityItemReplenishmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Activity replenishment item model.
    /// </summary>
    [Table("tActivityItemReplenishment")]
    public class tActivityItemReplenishmentModel
    {
        /// <summary>
        /// Gets or sets the activity item reference number.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the replenishment move type.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }


        /// <summary>
        /// Gets or sets the replenishment movement type.
        /// </summary>
        /// <remarks>ReplenishmentMoveType.Code.</remarks>
        [NotNull]
        public int MoveType { get; set; } = (int)ReplenishmentMoveType.Code.Manual;

        /// <summary>
        /// Gets or sets the activity item foreign key reference number.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the activity item foreign key parent reference number.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReplenishmentOnActivityItemFKParRefno", Order = 1, Unique = false)]
        public int ActivityItemFKParRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tLocationMapModel))]
        public int LocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemReplenishmentQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemReplenishmentBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemReplenishmentWeightQuantity { get; set; }
    }
}
