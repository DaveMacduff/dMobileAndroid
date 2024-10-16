// <copyright file="ActivityItemPickingModel.cs" company="InterSect Business Systems Inc">
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
    /// Represents a picking activity item.
    /// </summary>
    [Table("tActivityItemPicking")]
    public class ActivityItemPickingModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemPickingOnActivityItemFKRefno", Order = 1, Unique = true)]
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
        [Indexed(Name = "FK_ActivityItemPickingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item ordered.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item ordered.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingBaseQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item ordered.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingWeightQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item shipped.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingQuantityShipped { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item shipped.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingBaseQuantityShipped { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item shipped.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingWeightQuantityShipped { get; set; }

        /// <summary>
        /// Gets or sets the customer product unit code.
        /// </summary>
        [NotNull]
        public string ActivityItemPickingCustomerProductUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the picking activity item.
        /// </summary>
        [NotNull]
        public string ActivityItemPickingDescription { get; set; }

        /// <summary>
        /// Gets or sets the available quantity of the item.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingQuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the available base quantity of the item.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingBaseQuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the available weight quantity of the item.
        /// </summary>
        [NotNull]
        public decimal ActivityItemPickingWeightQuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picking activity item is reviewed.
        /// </summary>
        [NotNull]
        public bool ActivityItemPickingIsReviewed { get; set; }
    }
}