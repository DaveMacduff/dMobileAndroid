// <copyright file="SLabelQuantity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Structure to hold label quantites.
    /// </summary>
    /// <remarks>Used in requesting how many shipping labels to print.</remarks>
    public struct SLabelQuantity
    {

        /// <summary>
        /// Repack fresh quantity.
        /// </summary>
        private int repackFreshQuantity;

        /// <summary>
        /// Used as flag to validate if a 'fresh' repack quantity is required.
        /// </summary>
        private bool isRepackFresh;

        /// <summary>
        /// Gets or sets a value indicating whether is repack quantity.
        /// </summary>
        public bool IsRepack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is repack live quantity.
        /// </summary>
        public bool IsRepackLive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is repack fresh quantity.
        /// </summary>
        public bool IsRepackFresh
        {
            get
            {
                return this.isRepackFresh;
            }

            set
            {
                this.isRepackFresh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is repack frozen quantity.
        /// </summary>
        public bool IsRepackFrozen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is repack dry quantity.
        /// </summary>
        public bool IsRepackDry { get; set; }

        /// <summary>
        /// Gets or sets the bundle quantity.
        /// </summary>
        public int BundleQuantity { get; set; }

        /// <summary>
        /// Gets or sets the repack fresh quantity.
        /// </summary>
        public int RepackFreshQuantity
        {
            get
            {
                return this.repackFreshQuantity;
            }

            set
            {
                this.repackFreshQuantity = value;
            }
        }

        /// <summary>
        /// Gets or sets the repack live quantity.
        /// </summary>
        public int RepackLiveQuantity { get; set; }

        /// <summary>
        /// Gets or sets the frozen quantity.
        /// </summary>
        public int RepackFrozenQuantity { get; set; }

        /// <summary>
        /// Gets or sets the repack dry quantity.
        /// </summary>
        public int RepackDryQuantity { get; set; }

        /// <summary>
        /// Gets or sets the pallet quantity.
        /// </summary>
        public int PalletQuantity { get; set; }

        /// <summary>
        /// Gets or sets the fresh quantity.
        /// </summary>
        public int FreshQuantity { get; set; }

        /// <summary>
        /// Gets or sets the live quantity.
        /// </summary>
        public int LiveQuantity { get; set; }

        /// <summary>
        /// Gets or sets the frozen quantity.
        /// </summary>
        public int FrozenQuantity { get; set; }

        /// <summary>
        /// Gets or sets the dry quantity.
        /// </summary>
        public int DryQuantity { get; set; }

        /// <summary>
        /// Gets or sets the full quantity.
        /// </summary>
        public int FullQuantity { get; set; }

        /// <summary>
        /// Gets or sets the repack quantity.
        /// </summary>
        public int RepackQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the label is printed.
        /// </summary>
        public bool IsPrinted { get; set; }
    }
}