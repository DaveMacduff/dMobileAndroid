// <copyright file="ConvertUnitItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class ConvertUnitItem
    {
        private int activityDetailRefno;
        private string productSchemeVariantCode;
        private decimal activityDetailWeightQuantity;
        private DateTime activityDetailLogisticUnitProductionDate;

        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public int ProductSchemeVariantNumber { get; set; }
        public int ActivityDetailFKRefno { get; set; }
        public int ActivityDetailRefno { get => this.activityDetailRefno; set => this.activityDetailRefno = value; }
        public string ProductCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string UnitCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public string ActivityDetailSchemeID { get; set; }
        public string ActivityDetailUniqueSchemeID { get; set; }
        public string ActivityDetailSingleTradeItemSerialNumber { get; set; }
        public string ActivityDetailLogisticUnitBatchLotNumber { get; set; }
        public string ProductSchemeVariantCode { get => this.productSchemeVariantCode; set => this.productSchemeVariantCode = value; }
        public decimal ActivityDetailQuantity { get; set; }
        public decimal ActivityDetailBaseQuantity { get; set; }
        public decimal ActivityDetailWeightQuantity { get => this.activityDetailWeightQuantity; set => this.activityDetailWeightQuantity = value; }
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }
        public DateTime ActivityDetailLogisticUnitProductionDate { get => this.activityDetailLogisticUnitProductionDate; set => this.activityDetailLogisticUnitProductionDate = value; }
    }
}
