// <copyright file="ActivityItemReturnsModel.cs" company="InterSect Business Systems Inc">
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
    /// Represents an activity item return.
    /// </summary>
    [Table("tActivityItemReturns")]
    public class ActivityItemReturnsModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemReturnsOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReturnsOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReturnsOnActivityItemFKRefno", Order = 1, Unique = true)]
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
        [Indexed(Name = "FK_ActivityItemReturnsOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item returned.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReturnsQuantityReturned { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item returned.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReturnsBaseQuantityReturned { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item returned.
        /// </summary>
        [NotNull]
        public decimal ActivityItemReturnsWeightQuantityReturned { get; set; }

        /// <summary>
        /// Gets or sets the customer product unit code for the item returned.
        /// </summary>
        [NotNull]
        public string ActivityItemReturnsCustomerProductUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the item returned.
        /// </summary>
        [NotNull]
        public string ActivityItemReturnsDescription { get; set; }
    }
}