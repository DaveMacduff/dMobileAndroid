// <copyright file="tProductSchemeLotNumberXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tProductSchemeLotNumberXRef")]
    public class tProductSchemeLotNumberXRefModel
    {
        [NotNull]
        [ForeignKey(typeof(tProductSchemeModel))]
        [Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tProductSchemeLotNumberModel))]
        [Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeLotNumberRefno", Order = 1, Unique = false)]
        public int ProductSchemeLotNumberRefno { get; set; }
    }
}
