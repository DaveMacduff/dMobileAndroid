// <copyright file="Replenishment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    /// <summary>
    /// Replenishment header type.
    /// </summary>
    public class Replenishment
    {
        /// <summary>
        /// Gets or sets the activity reference number.
        /// </summary>
        public int ActivityRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity sub type. Inventory replenishment.
        /// </summary>
        public int ActivitySubType { get; set; } = 0;

        /// <summary>
        /// Gets or sets the warehouse location map reference number.
        /// </summary>
        public int WarehouseLocationMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity item reference number.
        /// </summary>
        public int ActivityItemRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity move type.
        /// </summary>
        public ReplenishmentMoveType.Code MoveType { get; set; } = ReplenishmentMoveType.Code.Manual;

        /// <summary>
        /// Gets or sets the move type string. 'Manual', 'Restock', 'Pull Down'.
        /// </summary>
        public string MoveTypeString { get; set; } = ReplenishmentMoveType.Enum2String(ReplenishmentMoveType.Code.Manual);

        /// <summary>
        /// Gets or sets the activity item foreign key reference.
        /// </summary>
        public int ActivityItemFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity item parent foreign key reference.
        /// </summary>
        public int ActivityItemFKParRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scheme identifier.
        /// </summary>
        public string ActivityDetailSchemeID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit reference.
        /// </summary>
        public int ProductUnitRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit bc.
        /// </summary>
        public string ProductUnitBC { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit code.
        /// </summary>
        public string ProductUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location map reference number.
        /// </summary>
        public int LocationMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the readable location.
        /// </summary>
        public string ReadableLocationMap { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity item replenishment quantity.
        /// </summary>
        public decimal ActivityItemReplenishmentQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity item replenishment base quantity.
        /// </summary>
        public decimal ActivityItemReplenishmentBaseQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity item replenishment weight quantity.
        /// </summary>
        public decimal ActivityItemReplenishmentWeightQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the activity detail is confirmed.
        /// </summary>
        public bool ActivityDetailIsConfirmed { get; set; } = false;

        /// <summary>
        /// Gets or sets the activity detail is confirmed string.
        /// </summary>
        public string ActivityDetailIsConfirmedString { get; set; } = string.Empty;

        /////// <summary>
        /////// Gets or sets the activity detail single trade item serial number.
        /////// </summary>
        ////public string ActivityDetailSingleTradeItemSerialNumber { get; set; } = string.Empty;

        /////// <summary>
        /////// Gets or sets the activity detail logistic unit batch lot number.
        /////// </summary>
        ////public string ActivityDetailLogisticUnitBatchLotNumber { get; set; } = string.Empty;

        /////// <summary>
        /////// Gets or sets the activity detail logistic unit best before sell by date.
        /////// </summary>
        ////public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; } = ResSysCommon.EmptyDateTime;

        /////// <summary>
        /////// Gets or sets the activity detail logistic unit production date.
        /////// </summary>
        ////public DateTime ActivityDetailLogisticUnitProductionDate { get; set; } = ResSysCommon.EmptyDateTime;

        /////// <summary>
        /////// Gets or sets the activity detail logistic unit skid number.
        /////// </summary>
        ////public string ActivityDetailLogisticUnitSkidNumber { get; set; } = string.Empty;
    }
}
