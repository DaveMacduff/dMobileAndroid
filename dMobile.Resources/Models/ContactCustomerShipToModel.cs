// <copyright file="ContactCustomerShipToModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a contact customer ship-to.
    /// </summary>
    [Table("tContactCustomerShipTo")]
    public class ContactCustomerShipToModel
    {
        /// <summary>
        /// Gets or sets the reference number of the contact.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ContactModel))]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the contact.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the customer contact.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ContactCustomerModel))]
        public int CustomerContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the contact customer ship-to.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToFKRefno", Order = 1, Unique = true)]
        public int ContactCustomerShipToFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the contact customer ship-to.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToCode", Order = 1, Unique = true)]
        public string ContactCustomerShipToCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact customer ship-to.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToName", Order = 1, Unique = true)]
        public string ContactCustomerShipToName { get; set; }
    }
}