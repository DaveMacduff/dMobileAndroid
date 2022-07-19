// <copyright file="tProductSchemeVariantModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    [Table("tProductSchemeVariant")]
    public class tProductSchemeVariantModel
    {
        [PrimaryKey]
        [NotNull]
        public int ProductSchemeVariantRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ProductSchemeVariantOnProductSchemeVariantCode", Order = 1, Unique = true)]
        public string ProductSchemeVariantCode { get; set; }

        [NotNull]
        public string ProductSchemeVariantDescription { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ProductSchemeVariantOnProductSchemeVariantNo", Order = 1, Unique = true)]
        public int ProductSchemeVariantNo { get; set; }

        [NotNull]
        public bool ProductSchemeVariantIsDefault { get; set; }
    }
}
