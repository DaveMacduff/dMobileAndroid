// <copyright file="ActivityItemProductionModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Items
{
    using dMobile.Resources.Models.LocationMap;
    using dMobile.Resources.Models.Products;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a production activity item.
    /// </summary>
    [Table("tActivityItemProduction")]
    public class ActivityItemProductionModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

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
        [Indexed(Name = "FK_ActivityItemProductionOnLocationMapRefno", Order = 2, Unique = false)]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemProductionQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemProductionBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemProductionWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the description of the production activity item.
        /// </summary>
        [NotNull]
        public string ActivityItemProductionDescription { get; set; }

        /// <summary>
        /// Gets or sets the expected quantity of the item to be produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemProductionQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected base quantity of the item to be produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemProductionBaseQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected weight quantity of the item to be produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemProductionWeightQuantityExpected { get; set; }
    }
}