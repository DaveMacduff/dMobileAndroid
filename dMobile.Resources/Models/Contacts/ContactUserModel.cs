// <copyright file="ContactUserModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Contacts
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a contact user.
    /// </summary>
    [Table("tContactUser")]
    public class ContactUserModel
    {
        /// <summary>
        /// Gets or sets the reference number of the contact.
        /// </summary>
        [PrimaryKey, NotNull]
        [ForeignKey(typeof(ContactModel))]
        [Indexed(Name = "UQ_ContactUserOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the contact.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactUserOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        /// <summary>
        /// Gets or sets the code of the contact user.
        /// </summary>
        [Indexed(Name = "UQ_ContactUserOnContactUserCode", Order = 1, Unique = true)]
        [NotNull]
        public int ContactUserCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact user.
        /// </summary>
        [Indexed(Name = "UQ_ContactUserOnContactUserName", Order = 1, Unique = true)]
        [NotNull]
        public string ContactUserName { get; set; }

        /// <summary>
        /// Gets or sets the password of the contact user.
        /// </summary>
        [NotNull]
        public string ContactUserPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to associate product barcodes.
        /// </summary>
        [NotNull]
        public bool ContactUserIsAllowedAssociateProductBarCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to override the picker in the picking module.
        /// </summary>
        [NotNull]
        public bool ContactUserIsAllowedPickingModuleOverridePicker { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to add manual details in the picking module.
        /// </summary>
        [NotNull]
        public bool ContactUserIsAllowedPickingModuleOverrideAddManualDetail { get; set; }

        /// <summary>
        /// Gets or sets the localization language of the contact user.
        /// </summary>
        public string ContactUserLocalizationLanguage { get; set; }
    }
}