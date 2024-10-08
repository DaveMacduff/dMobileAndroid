// <copyright file="ProductSchemeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    [Table("tProductScheme")]
    public class ProductSchemeModel
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductSchemeRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ProductSchemeOnProductSchemeCode", Order = 1, Unique = true)]
        public string ProductSchemeCode { get; set; }

        [NotNull]
        public bool ProductSchemeIsSerialTracked { get; set; }

        [NotNull]
        public bool ProductSchemeIsProductionDateTracked { get; set; }

        [NotNull]
        public bool ProductSchemeIsLotTracked { get; set; }

        [NotNull]
        public bool ProductSchemeIsBestBeforeDateTracked { get; set; }

        [NotNull]
        public bool ProductSchemeIsVariantTracked { get; set; }

        [NotNull]
        public bool ProductSchemeIsLotUniqueComponent { get; set; }

        [NotNull]
        public bool ProductSchemeIsProductionDateUniqueComponent { get; set; }

        [NotNull]
        public bool ProductSchemeIsBestBeforeDateUniqueComponent { get; set; }

        [NotNull]
        public bool ProductSchemeIsVariantUniqueComponent { get; set; }

        [NotNull]
        public int ProductSchemeExtra1Type { get; set; }

        [NotNull]
        public int ProductSchemeExtra2Type { get; set; }

        [NotNull]
        public int ProductSchemeExtra3Type { get; set; }

        [NotNull]
        public int ProductSchemeExtra4Type { get; set; }

        [NotNull]
        public int ProductSchemeExtra5Type { get; set; }

        [NotNull]
        public string ProductSchemeExtra1Caption { get; set; }

        [NotNull]
        public string ProductSchemeExtra2Caption { get; set; }

        [NotNull]
        public string ProductSchemeExtra3Caption { get; set; }

        [NotNull]
        public string ProductSchemeExtra4Caption { get; set; }

        [NotNull]
        public string ProductSchemeExtra5Caption { get; set; }

        [NotNull]
        public bool ProductSchemeExtra1IsRequired { get; set; }

        [NotNull]
        public bool ProductSchemeExtra2IsRequired { get; set; }

        [NotNull]
        public bool ProductSchemeExtra3IsRequired { get; set; }

        [NotNull]
        public bool ProductSchemeExtra4IsRequired { get; set; }

        [NotNull]
        public bool ProductSchemeExtra5IsRequired { get; set; }

        [NotNull]
        public bool ProductSchemeIsLotTrackedDefault { get; set; }

        [NotNull]
        public bool ProductSchemeIsProductionDateTrackedDefault { get; set; }

        [NotNull]
        public bool ProductSchemeIsBestBeforeDateTrackedDefault { get; set; }
    }
}
