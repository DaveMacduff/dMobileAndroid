// <copyright file="ProductionTicketByProduct.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Headers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProductionTicketByProduct
    {
        public int ActivityStatus { get; set; }
        public int ActivityRefno { get; set; }
        public int ActivityFKRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public DateTime ActivityProductionDateTime { get; set; }
        public string ActivityProductionDescription { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public string LocationMapWarehouseName { get; set; }
    }
}
