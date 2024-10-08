// <copyright file="ProductSchemeLotNumberXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tProductSchemeLotNumberXRef")]
    public class ProductSchemeLotNumberXRefModel
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductSchemeLotNumberXRefRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductSchemeModel))]
        [Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductSchemeLotNumberModel))]
        [Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeLotNumberRefno", Order = 1, Unique = false)]
        public int ProductSchemeLotNumberRefno { get; set; }
    }
}
