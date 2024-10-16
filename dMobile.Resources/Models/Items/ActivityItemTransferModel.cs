// <copyright file="ActivityItemTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Items
{
    using System;
    using dMobile.Resources.Models.LocationMap;
    using dMobile.Resources.Models.Products;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity item transfer.
    /// </summary>
    [Table("tActivityItemTransfer")]
    public class ActivityItemTransferModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemTransferOnActivityItemRefno", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemTransferOnActivityItemSubType", Order = 1, Unique = false)]
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemTransferOnActivityItemFKRefnoAndActivitySubType", Order = 1, Unique = false)]
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
        /// Gets or sets the reference number of the warehouse location map from which the item is transferred.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityItemTransferOnTransferFromWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int TransferFromWarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the required quantity for the item transfer.
        /// </summary>
        [NotNull]
        public decimal ActivityItemTransferRequiredQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item transferred.
        /// </summary>
        [NotNull]
        public decimal ActivityItemTransferQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item transferred.
        /// </summary>
        [NotNull]
        public decimal ActivityItemTransferBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item transferred.
        /// </summary>
        [NotNull]
        public decimal ActivityItemTransferWeightQuantity { get; set; }
    }
}