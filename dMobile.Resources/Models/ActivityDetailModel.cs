// <copyright file="ActivityDetailModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the details of an activity.
    /// </summary>
    [Table("tActivityDetail")]
    public class ActivityDetailModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the activity detail.
        /// </summary>
        [Indexed(Name = "UQ_ActivityDetailOnActivityDetailRefnoAndActivityDetailSubType", Order = 1, Unique = true)]
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity detail.
        /// </summary>
        [Indexed(Name = "UQ_ActivityDetailOnActivityDetailRefnoAndActivityDetailSubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivityDetailSubType { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity detail.
        /// </summary>
        [NotNull]
        public int ActivityDetailStatus { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the associated activity item.
        /// </summary>
        [Indexed(Name = "FK_ActivityDetailOnActivityItemRefno", Order = 1, Unique = false)]
        [ForeignKey(typeof(ContactModel))]
        [NotNull]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the associated location map.
        /// </summary>
        [Indexed(Name = "FK_ActivityDetailOnLocationMapRefno", Order = 1, Unique = false)]
        [ForeignKey(typeof(LocationMapModel))]
        [NotNull]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the associated product unit.
        /// </summary>
        [ForeignKey(typeof(ProductUnitModel))]
        [NotNull]
        [Indexed(Name = "FK_ActivityDetailOnProductUnitRefno", Order = 1, Unique = false)]
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique scheme ID for the activity detail.
        /// </summary>
        [Indexed(Name = "ActivityDetailOnActivityDetailUniqueSchemeID", Order = 1, Unique = false)]

        // ReSharper disable once InconsistentNaming
        public string ActivityDetailUniqueSchemeID { get; set; }

        /// <summary>
        /// Gets or sets the scheme ID for the activity detail.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string ActivityDetailSchemeID { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the activity detail.
        /// </summary>
        [NotNull]
        public decimal ActivityDetailQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the activity detail.
        /// </summary>
        [NotNull]
        public decimal ActivityDetailBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the activity detail.
        /// </summary>
        [NotNull]
        public decimal ActivityDetailWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the serial number of the single trade item.
        /// </summary>
        [NotNull]
        public string ActivityDetailSingleTradeItemSerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the batch/lot number of the logistic unit.
        /// </summary>
        [NotNull]
        public string ActivityDetailLogisticUnitBatchLotNumber { get; set; }

        /// <summary>
        /// Gets or sets the best before/sell by date of the logistic unit.
        /// </summary>
        [NotNull]
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }

        /// <summary>
        /// Gets or sets the expiry/use by date of the logistic unit.
        /// </summary>
        [NotNull]
        public DateTime ActivityDetailLogisticUnitExpiryUseByDate { get; set; }

        /// <summary>
        /// Gets or sets the production date of the logistic unit.
        /// </summary>
        [NotNull]
        public DateTime ActivityDetailLogisticUnitProductionDate { get; set; }

        /// <summary>
        /// Gets or sets the due date of the logistic unit.
        /// </summary>
        [NotNull]
        public DateTime ActivityDetailLogisticUnitDueDate { get; set; }

        /// <summary>
        /// Gets or sets the packaging date of the logistic unit.
        /// </summary>
        [NotNull]
        public DateTime ActivityDetailLogisticUnitPackagingDate { get; set; }

        /// <summary>
        /// Gets or sets the skid number of the logistic unit.
        /// </summary>
        [NotNull]
        public string ActivityDetailLogisticUnitSkidNumber { get; set; }
    }
}