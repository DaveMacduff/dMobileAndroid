// <copyright file="ProductionTicketByProduct.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProductionTicketByProduct
    {
        private int locationMapWarehouseFKRefno;
        private string locationMapWarehouseName;

        public int ActivityStatus { get; set; }
        public int ActivityRefno { get; set; }
        public int ActivityProductionFKRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get => this.locationMapWarehouseFKRefno; set => this.locationMapWarehouseFKRefno = value; }
        public DateTime ActivityProductionDateTime { get; set; }
        public string ActivityProductionDescription { get; set; }
        public string ActivityProductionStringFKRefno { get; set; }
        public string LocationMapWarehouseName { get => this.locationMapWarehouseName; set => this.locationMapWarehouseName = value; }
    }
}
