// <copyright file="tContactUserModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tContactUser")]
    public class tContactUserModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tContactModel))]
        [Indexed(Name = "UQ_ContactUserOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactUserOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [Indexed(Name = "UQ_ContactUserOnContactUserCode", Order = 1, Unique = true)]
        [NotNull]
        public int ContactUserCode { get; set; }

        [Indexed(Name = "UQ_ContactUserOnContactUserName", Order = 1, Unique = true)]
        [NotNull]
        public string ContactUserName { get; set; }

        [NotNull]
        public string ContactUserPassword { get; set; }

        [NotNull]
        public bool ContactUserIsAllowedAssociateProductBarCode { get; set; }

        [NotNull]
        public bool ContactUserIsAllowedPickingModuleOverridePicker { get; set; }

        [NotNull]
        public bool ContactUserIsAllowedPickingModuleOverrideAddManualDetail { get; set; }

        public string ContactUserLocalizationLanguage { get; set; }
    }
}
