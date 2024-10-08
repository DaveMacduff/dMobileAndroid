// <copyright file="ProductUnitRelationModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tProductUnitRelation")]
    public class ProductUnitRelationModel
    {
        [PrimaryKey, AutoIncrement, NotNull]
        private int productUnitRelationRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        [Indexed(Name = "FK_ProductUnitRelationOnProductUnit1Refno", Order = 1, Unique = false)]
        public int ProductUnit1Refno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        [Indexed(Name = "FK_ProductUnitRelationOnProductUnit2Refno", Order = 1, Unique = false)]
        public int ProductUnit2Refno { get; set; }

        [NotNull]
        public decimal ProductUnitRelationFactor { get; set; }
    }
}
