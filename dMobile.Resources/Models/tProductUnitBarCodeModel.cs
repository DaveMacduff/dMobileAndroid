// <copyright file="tProductUnitBarCodeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tProductUnitBarCode")]
    public class tProductUnitBarCodeModel
    {
        [PrimaryKey]
        [NotNull]
        public int ProductUnitBarCodeRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tProductUnitModel))]
        [Indexed(Name = "FK_ProductUnitBarCodeOnProductUnitRefno", Order = 1, Unique = false)]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1", Order = 1, Unique = true)]
        public string ProductUnitBarCodeCode { get; set; }

        [NotNull]
        public string ProductUnitBarCodeRaw { get; set; }

        [NotNull]
        public int ProductUnitBarCodeSymbologyEnum { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1", Order = 2, Unique = true)]
        public string ProductUnitBarCodeModifier1 { get; set; }

        [NotNull]
        public int ProductUnitBarCodeStatus { get; set; }
    }
}
