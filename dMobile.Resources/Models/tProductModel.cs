// <copyright file="tProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    [Table("tProduct")]
    public class tProductModel
    {
        [PrimaryKey, NotNull]
        public int ProductRefno { get; set; }

        [NotNull]
        public int ProductSchemeRefno { get; set; }

        [NotNull]
        public int FamilyRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductOnProductCode", Order = 1, Unique = true)]
        public string ProductCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductOnProductFKRefno", Order = 1, Unique = true)]
        public int ProductFKRefno { get; set; }

        [NotNull, Indexed(Name = "ProductOnProductDescription", Order = 1, Unique = false)]
        public string ProductDescription { get; set; }

        [NotNull]
        public bool ProductIsCatchCount { get; set; }

        [NotNull]
        public bool ProductIsCatchWeight { get; set; }

        [NotNull]
        public bool ProductIsCatchAsk { get; set; }

        [NotNull]
        public bool ProductIsCatchAskZero { get; set; }

        [NotNull]
        public string ProductBrandDescription { get; set; }

        [NotNull]
        public bool ProductAllowConvertProduct { get; set; }
    }
}