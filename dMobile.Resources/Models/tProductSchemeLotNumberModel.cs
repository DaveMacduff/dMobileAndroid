// <copyright file="tProductSchemeLotNumberModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    [Table("tProductSchemeLotNumber")]
    public class tProductSchemeLotNumberModel
    {
        [PrimaryKey, NotNull]
        public int ProductSchemeLotNumberRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode", Order = 1, Unique = true)]
        public string ProductSchemeLotNumberCode { get; set; }

        [NotNull]
        public string ProductSchemeLotNumberCodeDefault { get; set; }
    }
}
