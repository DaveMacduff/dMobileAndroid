// <copyright file="PickingItemSummary.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class PickingItemSummary
    {
        private decimal weightShipped;

        public decimal Ordered { get; set; }
        public decimal WeightOrdered { get; set; }
        public decimal Shipped { get; set; }
        public decimal WeightShipped { get => this.weightShipped; set => this.weightShipped = value; }
        public string WtUnitCode { get; set; }
    }
}
