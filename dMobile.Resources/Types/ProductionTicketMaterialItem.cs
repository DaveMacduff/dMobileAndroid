// <copyright file="ProductionTicketMaterialItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProductionTicketMaterialItem
    {
        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get; set; }
        public int ActivityItemStatus { get; set; }
        public decimal ActivityItemProductionQuantity { get; set; }
        public decimal ActivityItemProductionBaseQuantity { get; set; }
        public decimal ActivityItemProductionWeightQuantity { get; set; }
        public decimal ActivityItemProductionQuantityExpected { get; set; }
        public decimal ActivityItemProductionBaseQuantityExpected { get; set; }
        public decimal ActivityItemProductionWeightQuantityExpected { get; set; }
        public string ActivityItemStringFKRefno { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string BaseUnitCode { get; set; }
        public string LocationName { get; set; }
    }
}
