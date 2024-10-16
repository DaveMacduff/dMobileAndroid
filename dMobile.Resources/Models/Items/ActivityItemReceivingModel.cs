// <copyright file="ActivityItemReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Items
{
    using dMobile.Resources.Models.LocationMap;
    using dMobile.Resources.Models.Products;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity item receiving.
    /// </summary>
    [Table("tActivityItemReceiving")]
    public class ActivityItemReceivingModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemFKRefno", Order = 1, Unique = true)]
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
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityItemReceivingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item ordered.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item ordered.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item ordered.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item received.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingQuantityReceived { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item received.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityReceived { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item received.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityReceived { get; set; }

        /// <summary>
        /// Gets or sets the vendor product unit code for the item received.
        /// </summary>
        [NotNull]
        public string ActivityItemReceivingVendorProductUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the item received.
        /// </summary>
        [NotNull]
        public string ActivityItemReceivingDescription { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item received to date.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingQuantityReceivedToDate { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item received to date.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityReceivedToDate { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item received to date.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityReceivedToDate { get; set; }
    }
}