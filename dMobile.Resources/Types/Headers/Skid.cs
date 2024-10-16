// <copyright file="Skid.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Headers
{
    using System;

    /// <summary>
    /// Represents a skid.
    /// </summary>
    public class Skid
    {
        /// <summary>
        /// Gets or sets the foreign key reference number of the skid.
        /// </summary>
        public int SkidFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the Ti (number of cases per layer) of the skid.
        /// </summary>
        public int SkidTi { get; set; }

        /// <summary>
        /// Gets or sets the Hi (number of layers) of the skid.
        /// </summary>
        public int SkidHi { get; set; }

        /// <summary>
        /// Gets or sets the code of the skid.
        /// </summary>
        public string SkidCode { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets the code of the unit.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the code of the base unit.
        /// </summary>
        public string BaseUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the code of the weight unit.
        /// </summary>
        public string WeightUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets the cube unit of the skid.
        /// </summary>
        public string SkidCubeUnit { get; set; }

        /// <summary>
        /// Gets or sets the height unit of the skid.
        /// </summary>
        public string SkidHeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the product.
        /// </summary>
        public decimal BaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the product.
        /// </summary>
        public decimal WeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the cube of the skid.
        /// </summary>
        public decimal SkidCube { get; set; }

        /// <summary>
        /// Gets or sets the height of the skid.
        /// </summary>
        public decimal SkidHeight { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the skid was created.
        /// </summary>
        public DateTime SkidCreated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the skid was completed.
        /// </summary>
        public DateTime SkidCompleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit has a barcode.
        /// </summary>
        public bool ProductUnitIsBC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the skid is CHEP.
        /// </summary>
        public bool SkidIsCHEP { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the skid is on hold.
        /// </summary>
        public bool SkidIsOnHold { get; set; }
    }
}