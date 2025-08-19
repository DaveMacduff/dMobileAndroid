// <copyright file="ActivityItemCutSheetModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Items
{
    using dMobile.Resources.Models.LocationMap;
    using dMobile.Resources.Models.Products;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a cut sheet activity item.
    /// </summary>
    [Table("tActivityItemCutSheet")]
    public class ActivityItemCutSheetModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemCutSheetOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityItemCutSheetOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
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
        [Indexed(Name = "FK_ActivityItemCutSheetOnLocationMapRefno", Order = 2, Unique = false)]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCutSheetQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the item produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCutSheetBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the item produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCutSheetWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the description of the production activity item.
        /// </summary>
        [NotNull]
        public string ActivityItemCutSheetDescription { get; set; }

        /// <summary>
        /// Gets or sets the expected quantity of the item to be produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCutSheetQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected base quantity of the item to be produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCutSheetBaseQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected weight quantity of the item to be produced.
        /// </summary>
        [NotNull]
        public decimal ActivityItemCutSheetWeightQuantityExpected { get; set; }
    }
}