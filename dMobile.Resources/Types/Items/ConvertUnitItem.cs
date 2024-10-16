// <copyright file="ConvertUnitItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Items
{
    using System;

    public class ConvertUnitItem
    {
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public int ProductSchemeVariantNumber { get; set; }
        public int ActivityDetailFKRefno { get; set; }
        public int ActivityDetailRefno { get; set; }
        public string ProductCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string UnitCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public string ActivityDetailSchemeID { get; set; }
        public string ActivityDetailUniqueSchemeID { get; set; }
        public string ActivityDetailSingleTradeItemSerialNumber { get; set; }
        public string ActivityDetailLogisticUnitBatchLotNumber { get; set; }
        public string ProductSchemeVariantCode { get; set; }
        public decimal ActivityDetailQuantity { get; set; }
        public decimal ActivityDetailBaseQuantity { get; set; }
        public decimal ActivityDetailWeightQuantity { get; set; }
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }
        public DateTime ActivityDetailLogisticUnitProductionDate { get; set; }
    }
}
