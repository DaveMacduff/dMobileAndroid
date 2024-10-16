// <copyright file="ConvertProductItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Items
{
    using System;

    public class ConvertProductItem
    {
        public int FromProdFKRefno { get; set; }

        public int ToProdFKRefno { get; set; }

        public int LocMapRefno { get; set; }
    }
}
