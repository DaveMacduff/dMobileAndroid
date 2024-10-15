// <copyright file="ContactModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents a contact.
    /// </summary>
    [Table("tContact")]
    public class ContactModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the contact.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        [Indexed(Name = "UQ_ContactOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the contact.
        /// </summary>
        [Indexed(Name = "UQ_ContactOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        [NotNull]
        public int ContactSubType { get; set; }
    }
}