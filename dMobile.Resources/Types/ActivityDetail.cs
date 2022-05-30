// <copyright file="ActivityDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    /// <summary>
    /// Define the ActivityDetail model.
    /// </summary>
    public class ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail reference number.
        /// </summary>
        public int ActivityDetailRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity detail foreign key reference number.
        /// </summary>
        public int ActivityDetailFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity detail foreign key reference number field name. Used for searching.
        /// </summary>
        public string ActDetailFKRefnoFldName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product reference number.
        /// </summary>
        public int ProductRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product unit reference number.
        /// </summary>
        public int ProductUnitRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit bc character.
        /// </summary>
        public string ProductUnitBC { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is broken case.
        /// </summary>
        public bool ProductUnitIsBC { get; set; } = false;

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        public string ProductDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit size description.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the product unit code.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the product base unit code.
        /// </summary>
        public string BaseUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the product weight unit code.
        /// </summary>
        public string WtUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the activity detail alt quantity.
        /// </summary>
        public decimal ActivityDetailQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity detail base quantity.
        /// </summary>
        public decimal ActivityDetailBaseQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity  detail weight quantity.
        /// </summary>
        public decimal ActivityDetailWeightQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the location map reference number.
        /// </summary>
        public int LocationMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the location map code.
        /// </summary>
        public int LocationCode { get; set; } = 0;

        /// <summary>
        /// Gets or sets the location name.
        /// </summary>
        /// <example>Main or Freezer or A12B02.</example>
        public string LocationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location. e.g. Whs, section, slot.
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product scheme variant code.
        /// </summary>
        public string ProductSchemeVariantCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product scheme variant number.
        /// </summary>
        public int ProductSchemeVariantNo { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product scheme variant description.
        /// </summary>
        public string ProductSchemeVariantDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail single trade item serial number.
        /// </summary>
        public string ActivityDetailSingleTradeItemSerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail logistic unit best before sell by date.
        /// </summary>
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; } = SysCommon.EmptyDateTime;

        /// <summary>
        /// Gets or sets the activity detail logistic unit batch lot number.
        /// </summary>
        public string ActivityDetailLogisticUnitBatchLotNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail logistic unit production date.
        /// </summary>
        public DateTime ActivityDetailLogisticUnitProductionDate { get; set; } = SysCommon.EmptyDateTime;

        /// <summary>
        /// Gets or sets the activity detail logistic unit skid number.
        /// </summary>
        public string ActivityDetailLogisticUnitSkidNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra 1 text.
        /// </summary>
        public string ActivityDetailExtra1Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra 2 text.
        /// </summary>
        public string ActivityDetailExtra2Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra 3 text.
        /// </summary>
        public string ActivityDetailExtra3Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra 4 text.
        /// </summary>
        public string ActivityDetailExtra4Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra 5 date time.
        /// </summary>
        public DateTime ActivityDetailExtra5Date { get; set; } = SysCommon.EmptyDateTime;

        /// <summary>
        /// Gets or sets the activity detail scheme id.
        /// </summary>
        public string ActivityDetailSchemeID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail unique scheme id.
        /// </summary>
        public string ActivityDetailUniqueSchemeID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the product label is batched.
        /// </summary>
        public bool IsBatchProductLabel { get; set; } = false;
    }
}
