// <copyright file="ActivityDetailProductSchemeVariantXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailProductSchemeVariantXRef")]
    public class ActivityDetailProductSchemeVariantXRefModel
    {
        // TODO: Can only support one index right now
        [Indexed(Name = "UQ_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefno", Order = 1, Unique = true)]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }
        [Indexed(Name = "FK_ActivityDetailProductSchemeVariantXRefOnProductSchemeVariantRefno", Order = 1, Unique = false)]
        [NotNull]
        [ForeignKey(typeof(ProductSchemeVariantModel))]
        public int ProductSchemeVariantRefno { get; set; }
    }
}
