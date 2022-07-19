// <copyright file="tProductSchemeVariantXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tProductSchemeVariantXRef")]
    public class tProductSchemeVariantXRefModel
    {
        [NotNull]
        [ForeignKey(typeof(tProductSchemeModel))]
        [Indexed(Name = "FK_ProductSchemeVariantXRefOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tProductSchemeVariantModel))]
        [Indexed(Name = "FK_ProductSchemeVariantXRefOnProductSchemeVariantRefno", Order = 1, Unique = false)]
        public int ProductSchemeVariantRefno { get; set; }
    }
}
