// <copyright file="ConvertProductItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class ConvertProductItem
    {
        private int location_map_refno;

        public int From_prod_fk_refno { get; set; }
        public int To_prod_fk_refno { get; set; }
        public int Location_map_refno { get => this.location_map_refno; set => this.location_map_refno = value; }
    }
}
