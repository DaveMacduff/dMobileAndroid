// <copyright file="ProductUnitModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tProductUnit")]
    public class ProductUnitModel
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductModel))]
        [Indexed(Name = "FK_ProductUnitOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(UnitModel))]
        [Indexed(Name = "FK_ProductUnitOnUnitRefno", Order = 1, Unique = false)]
        public int UnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(UnitModel))]
        [Indexed(Name = "FK_ProductUnitOnBaseUnitRefno", Order = 1, Unique = false)]
        public int BaseUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(UnitModel))]
        [Indexed(Name = "FK_ProductUnitOnWtUnitRefno", Order = 1, Unique = false)]
        public int WtUnitRefno { get; set; }

        [NotNull]
        public bool ProductUnitIsBC { get; set; }

        [NotNull]
        public string ProductUnitBC { get; set; }

        [NotNull]
        public decimal ProductUnitWeight { get; set; }

        [NotNull]
        public string ProductUnitSizeDescription { get; set; }

        [NotNull]
        public bool ProductUnitIsSellUnit { get; set; }

        [NotNull]
        public bool ProductUnitIsDefault { get; set; }

        [NotNull]
        public decimal ProductUnitBoxCount { get; set; }

        [NotNull]
        public decimal ProductUnitWeightTolerance { get; set; }
    }
}
