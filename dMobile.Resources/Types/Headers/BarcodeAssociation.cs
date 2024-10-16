// <copyright file="BarcodeAssociation.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Headers
{
    /// <summary>
    /// Defines the <see cref="BarcodeAssociation" />.
    /// </summary>
    public class BarcodeAssociation
    {

        /// <summary>
        /// Gets or sets the ActivityRefno.
        /// </summary>
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the ProductUnitBarCodeRefno.
        /// </summary>
        public int ProductUnitBarCodeRefno { get; set; }

        /// <summary>
        /// Gets or sets the ActivityStatus.
        /// </summary>
        public int ActivityStatus { get; set; }

        /// <summary>
        /// Gets or sets the ProductCode.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the ProductUnitBC.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets the UnitCode.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the ProductUnitBarCodeCode.
        /// </summary>
        public string ProductUnitBarCodeCode { get; set; }

        /// <summary>
        /// Gets or sets the ProductUnitBarCodeModifier1.
        /// </summary>
        public string ProductUnitBarCodeModifier1 { get; set; }
    }
}
