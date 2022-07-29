// <copyright file="SSkidInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// <see cref="SSkidInfo" /> Skid information.
    /// </summary>
    public class SSkidInfo
    {
        /// <summary>
        /// Define the location map reference number.
        /// </summary>
        private int locMapRefno = 0;

        /// <summary>
        /// Gets or sets the product scheme data.
        /// </summary>
        public SProductSchemeData SchemeData { get; set; } = new SProductSchemeData();

        /// <summary>
        /// Gets or sets a value indicating whether the skid is put away.
        /// </summary>
        public bool IsPutAway { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the skid is consumed.
        /// </summary>
        public bool IsConsumed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the skid is completed.
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Gets or sets the skid foreign key reference number.
        /// </summary>
        public int SkidFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid cube unit.
        /// </summary>
        public string CubeUnit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cube.
        /// </summary>
        public decimal Cube { get; set; } = 0;

        /// <summary>
        /// Gets or sets the height unit.
        /// </summary>
        public string HeightUnit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public decimal Height { get; set; } = 0;

        /// <summary>
        /// Gets or sets the Hi.
        /// </summary>
        public int Hi { get; set; } = 0;

        /// <summary>
        /// Gets or sets the Ti.
        /// </summary>
        public int Ti { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the skid is on hold.
        /// </summary>
        public bool IsHold { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the skid has a CHEP pallet.
        /// </summary>
        public bool IsCHEP { get; set; } = false;

        /// <summary>
        /// Gets or sets the user entered or form generated skid code.
        /// </summary>
        public string SkidCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the GS1 18 digit skid code. Including the company prefix.
        /// </summary>
        public string LogisticUnitSkidCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the skid quantity.
        /// </summary>
        public decimal Quantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid base quantity.
        /// </summary>
        public decimal BaseQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid weight quantity.
        /// </summary>
        public decimal WeightQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the skid product information.
        /// </summary>
        public ProductInfo ProdInfo { get; set; } = new ProductInfo();

        /// <summary>
        /// Gets the location map reference.
        /// </summary>
        public int LocMapRefno
        {
            get
            {
                if (this.LocMap != null && this.locMapRefno == 0)
                {
                    if (this.LocMap.SlotLocMapRefno > 0)
                    {
                        this.locMapRefno = this.LocMap.SlotLocMapRefno;
                    }
                    else if (this.LocMap.SectLocMapRefno > 0)
                    {
                        this.locMapRefno = this.LocMap.SectLocMapRefno;
                    }
                    else
                    {
                        this.locMapRefno = this.LocMap.WhsLocMapRefno;
                    }
                }

                return this.locMapRefno;
            }
        }

        /// <summary>
        /// Gets or sets the location map.
        /// </summary>
        public SLocMap LocMap { get; set; } = new SLocMap();
    }
}
