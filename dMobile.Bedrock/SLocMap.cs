// <copyright file="SLocMap.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Warehouse location map.
    /// </summary>
    public class SLocMap
    {
        /// <summary>
        /// Define the slot coordinate 1 name.
        /// </summary>
        private string slotCoord1Name = string.Empty;

        /// <summary>
        /// Define the slot coordinate 2 name.
        /// </summary>
        private string slotCoord2Name = string.Empty;

        /// <summary>
        /// Define the slot coordinate 3 name.
        /// </summary>
        private string slotCoord3Name = string.Empty;

        // SLOT

        /// <summary>
        /// Gets or sets the slot coordinate 1 mask.
        /// </summary>
        public string SlotCoord1Mask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the slot coordinate 2 mask.
        /// </summary>
        public string SlotCoord2Mask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the slot coordinate 3 mask.
        /// </summary>
        public string SlotCoord3Mask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the combined slot coordinates slot mask.
        /// </summary>
        public string SlotMask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether slot coordinate 1 is tracked.
        /// </summary>
        public bool IsSlotCoord1Tracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether slot coordinate 2 is tracked.
        /// </summary>
        public bool IsSlotCoord2Tracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether slot coordinate 3 is tracked.
        /// </summary>
        public bool IsSlotCoord3Tracked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether is slot tracked.
        /// </summary>
        public bool IsSlotTracked { get; set; } = false;

        /// <summary>
        /// Gets or sets the slot code.
        /// </summary>
        public int SlotCode { get; set; } = 0;

        /// <summary>
        /// Gets or sets the slot coordinate 1 name.
        /// </summary>
        public string SlotCoord1Name
        {
            get => this.slotCoord1Name;

            set
            {
                this.slotCoord1Name = value;
                if (string.IsNullOrEmpty(this.slotCoord1Name))
                {
                    this.IsSlotCoord1Tracked = false;
                }
                else
                {
                    this.IsSlotCoord1Tracked = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the slot coordinate 2 name.
        /// </summary>
        public string SlotCoord2Name
        {
            get => this.slotCoord2Name;

            set
            {
                this.slotCoord2Name = value;
                if (string.IsNullOrEmpty(this.slotCoord2Name))
                {
                    this.IsSlotCoord2Tracked = false;
                }
                else
                {
                    this.IsSlotCoord2Tracked = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the slot coordinate 3 name.
        /// </summary>
        public string SlotCoord3Name
        {
            get => this.slotCoord3Name;

            set
            {
                this.slotCoord3Name = value;
                if (string.IsNullOrEmpty(this.slotCoord3Name))
                {
                    this.IsSlotCoord3Tracked = false;
                }
                else
                {
                    this.IsSlotCoord3Tracked = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the slot location map reference number.
        /// </summary>
        public int SlotLocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the slot location map foreign key reference number.
        /// </summary>
        public int SlotFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the slot name.
        /// </summary>
        public string SlotName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the slot coordinate 1 location map reference number.
        /// </summary>
        public int SlotCoord1LocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the slot coordinate 2 location map reference number.
        /// </summary>
        public int SlotCoord2LocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the slot coordinate 3 location map reference number.
        /// </summary>
        public int SlotCoord3LocMapRefno { get; set; } = 0;

        // SECTION

        /// <summary>
        /// Gets or sets the section mask.
        /// </summary>
        public string SectMask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether is section tracked.
        /// </summary>
        public bool IsSectionTracked { get; set; } = false;

        /// <summary>
        /// Gets or sets the section location map reference number.
        /// </summary>
        public int SectLocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the section foreign key reference number.
        /// </summary>
        public int SectFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the section name.
        /// </summary>
        public string SectName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the section code.
        /// </summary>
        public int SectCode { get; set; } = 0;

        // WAREHOUSE

        /// <summary>
        /// Gets or sets the warehouse code mask.
        /// </summary>
        public string WhsCodeMask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the warehouse location map reference number.
        /// </summary>
        public int WhsLocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the warehouse location map foreign key reference number.
        /// </summary>
        public int WhsFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the warehouse code.
        /// </summary>
        public int WhsCode { get; set; } = 0;

        /// <summary>
        /// Gets or sets the warehouse name.
        /// </summary>
        public string WhsName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse is a WMS one.
        /// </summary>
        public bool IsWhsWMS { get; set; } = false;

        /// <summary>
        /// Create a new copy of SLocMap with values of the current instance.
        /// </summary>
        /// <returns><see cref="SLocMap"/> New copy of SLocMap.</returns>
        public SLocMap Clone()
        {
            var copy = new SLocMap
            {
                IsWhsWMS = this.IsWhsWMS,
                IsSectionTracked = this.IsSectionTracked,
                IsSlotTracked = this.IsSlotTracked,
                IsSlotCoord1Tracked = this.IsSlotCoord1Tracked,
                IsSlotCoord2Tracked = this.IsSlotCoord2Tracked,
                IsSlotCoord3Tracked = this.IsSlotCoord3Tracked,

                WhsCode = this.WhsCode,
                WhsCodeMask = this.WhsCodeMask,
                WhsFKRefno = this.WhsFKRefno,
                WhsLocMapRefno = this.WhsLocMapRefno,
                WhsName = this.WhsName,

                SectCode = this.SectCode,
                SectFKRefno = this.SectFKRefno,
                SectLocMapRefno = this.SectLocMapRefno,
                SectMask = this.SectMask,
                SectName = this.SectName,

                SlotCode = this.SlotCode,
                SlotCoord1LocMapRefno = this.SlotCoord1LocMapRefno,
                SlotCoord1Mask = this.SlotCoord1Mask,
                SlotCoord1Name = this.SlotCoord1Name,
                SlotCoord2LocMapRefno = this.SlotCoord2LocMapRefno,
                SlotCoord2Mask = this.SlotCoord2Mask,
                SlotCoord2Name = this.SlotCoord2Name,
                SlotCoord3LocMapRefno = this.SlotCoord3LocMapRefno,
                SlotCoord3Mask = this.SlotCoord3Mask,
                SlotCoord3Name = this.SlotCoord3Name,
                SlotFKRefno = this.SlotFKRefno,
                SlotLocMapRefno = this.SlotLocMapRefno,
                SlotMask = this.SlotMask,
                SlotName = this.SlotName,
            };

            return copy;
        }
    }
}
