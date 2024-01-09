// <copyright file="ProdScheme.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Defines the <see cref="ProdScheme" />.
    /// </summary>
    public class ProdScheme
    {
        /// <summary>
        /// Gets or sets the scheme reference number.
        /// </summary>
        public int SchemeRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme is single trade item serial number tracked.
        /// </summary>
        public bool IsSerialTracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme is logistic unit production date tracked.
        /// </summary>
        public bool IsProductionDateTracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme is logistic unit batch lot number tracked.
        /// </summary>
        public bool IsLotTracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme is logistic unit best before sell by date tracked.
        /// </summary>
        public bool IsBestBeforeDateTracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme is logistic unit product variant tracked.
        /// </summary>
        public bool IsVariantTracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tracked logistic unit batch lot number is a component of uniqueness.
        /// </summary>
        public bool IsLotUniqueComponent { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tracked logistic unit production date is a component of uniqueness.
        /// </summary>
        public bool IsProductionDateUniqueComponent { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tracked logistic unit best before sell by date is a component
        /// of uniqueness.
        /// </summary>
        public bool IsBestBeforeDateUniqueComponent { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tracked logistic unit product variant is a component of uniqueness. 
        /// </summary>
        public bool IsVariantUniqueComponent { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tracked logistic unit batch lot number can be defaulted.
        /// </summary>
        public bool IsLotTrackedDefault { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tracked logistic unit best before sell by date can be defaulted.
        /// </summary>
        public bool IsBestBeforeDateTrackedDefault { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tracked logistic unit production date can be defaulted.
        /// </summary>
        public bool IsProductionDateTrackedDefault { get; set; } = false;

        /// <summary>
        /// Gets or sets the product scheme label settings.
        /// </summary>
        public ProdSchemeLabelSettings.ProdSchemeLabelInfo LabelInfo { get; set; } =
            new ProdSchemeLabelSettings.ProdSchemeLabelInfo();

        /// <summary>
        /// Gets or sets the scheme extra 1 type.
        /// </summary>
        public int Extra1Type { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scheme extra 1 caption.
        /// </summary>
        public string Extra1Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme extra 1 is required.
        /// </summary>
        public bool Extra1IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets the scheme extra 2 type.
        /// </summary>
        public int Extra2Type { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scheme extra 2 caption.
        /// </summary>
        public string Extra2Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme extra 2 is required.
        /// </summary>
        public bool Extra2IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets the scheme extra 3 type.
        /// </summary>
        public int Extra3Type { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scheme extra 3 caption.
        /// </summary>
        public string Extra3Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme extra 3 is required.
        /// </summary>
        public bool Extra3IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets the scheme extra 4 type.
        /// </summary>
        public int Extra4Type { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scheme extra 4 caption.
        /// </summary>
        public string Extra4Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme extra 4 is required.
        /// </summary>
        public bool Extra4IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets the scheme extra 5 type.
        /// </summary>
        public int Extra5Type { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scheme extra 5 caption.
        /// </summary>
        public string Extra5Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme extra 5 is required.
        /// </summary>
        public bool Extra5IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the scheme is base tracked quantity only.
        /// </summary>
        public bool IsBaseTracked { get; set; } = false;
    }
}
