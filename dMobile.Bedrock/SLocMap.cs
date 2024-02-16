// <copyright file="SLocMap.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    ///     Warehouse location map.
    /// </summary>
    public class SLocMap : IEquatable<SLocMap>
    {
        /// <summary>
        ///    Define the location map reference number.
        /// </summary>
        private int locMapRefno = 0;

        /// <summary>
        ///     Define the slot coordinate 1 name.
        /// </summary>
        private string slotCoord1Name = string.Empty;

        /// <summary>
        ///     Define the slot coordinate 2 name.
        /// </summary>
        private string slotCoord2Name = string.Empty;

        /// <summary>
        ///     Define the slot coordinate 3 name.
        /// </summary>
        private string slotCoord3Name = string.Empty;

        // SLOT

        /// <summary>
        ///     Gets or sets the slot coordinate 1 mask.
        /// </summary>
        public string SlotCoord1Mask { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the slot coordinate 2 mask.
        /// </summary>
        public string SlotCoord2Mask { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the slot coordinate 3 mask.
        /// </summary>
        public string SlotCoord3Mask { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the combined slot coordinates slot mask.
        /// </summary>
        public string SlotMask { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether slot coordinate 1 is tracked.
        /// </summary>
        public bool IsSlotCoord1Tracked { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether slot coordinate 2 is tracked.
        /// </summary>
        public bool IsSlotCoord2Tracked { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether slot coordinate 3 is tracked.
        /// </summary>
        public bool IsSlotCoord3Tracked { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is slot tracked.
        /// </summary>
        public bool IsSlotTracked { get; set; }

        /// <summary>
        ///     Gets or sets the slot code.
        /// </summary>
        public int SlotCode { get; set; }

        /// <summary>
        ///     Gets or sets the slot coordinate 1 name.
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
        ///     Gets or sets the slot coordinate 2 name.
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
        ///     Gets or sets the slot coordinate 3 name.
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
        ///     Gets or sets the slot location map reference number.
        /// </summary>
        public int SlotLocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the slot location map foreign key reference number.
        /// </summary>
        public int SlotFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets the slot name.
        /// </summary>
        public string SlotName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the slot coordinate 1 location map reference number.
        /// </summary>
        public int SlotCoord1LocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the slot coordinate 2 location map reference number.
        /// </summary>
        public int SlotCoord2LocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the slot coordinate 3 location map reference number.
        /// </summary>
        public int SlotCoord3LocMapRefno { get; set; }

        // SECTION

        /// <summary>
        ///     Gets or sets the section mask.
        /// </summary>
        public string SectMask { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether is section tracked.
        /// </summary>
        public bool IsSectionTracked { get; set; }

        /// <summary>
        ///     Gets or sets the section location map reference number.
        /// </summary>
        public int SectLocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the section foreign key reference number.
        /// </summary>
        public int SectFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets the section name.
        /// </summary>
        public string SectName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the section code.
        /// </summary>
        public int SectCode { get; set; }

        // WAREHOUSE

        /// <summary>
        ///     Gets or sets the warehouse code mask.
        /// </summary>
        public string WhsCodeMask { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the warehouse location map reference number.
        /// </summary>
        public int WhsLocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the warehouse location map foreign key reference number.
        /// </summary>
        public int WhsFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets the warehouse code.
        /// </summary>
        public int WhsCode { get; set; }

        /// <summary>
        ///     Gets or sets the warehouse name.
        /// </summary>
        public string WhsName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the warehouse is a WMS one.
        /// </summary>
        public bool IsWhsWMS { get; set; }

        /// <summary>
        /// Gets the current location map reference number.
        /// </summary>
        public int LocMapRefno
        {
            get
            {
                if (this.SlotLocMapRefno != 0)
                {
                    this.locMapRefno = this.SlotLocMapRefno;
                }
                else if (this.SectLocMapRefno != 0)
                {
                    this.locMapRefno = this.SectLocMapRefno;
                }
                else
                {
                    this.locMapRefno = this.WhsLocMapRefno;
                }

                return this.locMapRefno;
            }
        }

        /// <inheritdoc/>
        public bool Equals(SLocMap other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.slotCoord1Name == other.slotCoord1Name
                && this.slotCoord2Name == other.slotCoord2Name
                && this.slotCoord3Name == other.slotCoord3Name
                && this.SlotCoord1Mask == other.SlotCoord1Mask
                && this.SlotCoord2Mask == other.SlotCoord2Mask
                && this.SlotCoord3Mask == other.SlotCoord3Mask
                && this.SlotMask == other.SlotMask
                && this.IsSlotCoord1Tracked == other.IsSlotCoord1Tracked
                && this.IsSlotCoord2Tracked == other.IsSlotCoord2Tracked
                && this.IsSlotCoord3Tracked == other.IsSlotCoord3Tracked
                && this.IsSlotTracked == other.IsSlotTracked
                && this.SlotCode == other.SlotCode
                && this.SlotLocMapRefno == other.SlotLocMapRefno
                && this.SlotFKRefno == other.SlotFKRefno
                && this.SlotName == other.SlotName
                && this.SlotCoord1LocMapRefno == other.SlotCoord1LocMapRefno
                && this.SlotCoord2LocMapRefno == other.SlotCoord2LocMapRefno
                && this.SlotCoord3LocMapRefno == other.SlotCoord3LocMapRefno
                && this.SectMask == other.SectMask
                && this.IsSectionTracked == other.IsSectionTracked
                && this.SectLocMapRefno == other.SectLocMapRefno
                && this.SectFKRefno == other.SectFKRefno
                && this.SectName == other.SectName
                && this.SectCode == other.SectCode
                && this.WhsCodeMask == other.WhsCodeMask
                && this.WhsLocMapRefno == other.WhsLocMapRefno
                && this.WhsFKRefno == other.WhsFKRefno
                && this.WhsCode == other.WhsCode
                && this.WhsName == other.WhsName
                && this.IsWhsWMS == other.IsWhsWMS;
        }

        /// <summary>
        ///     Create a new copy of SLocMap with values of the current instance.
        /// </summary>
        /// <returns><see cref="SLocMap" /> New copy of SLocMap.</returns>
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
                SlotName = this.SlotName
            };

            return copy;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((SLocMap)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(this.slotCoord1Name);
            hashCode.Add(this.slotCoord2Name);
            hashCode.Add(this.slotCoord3Name);
            hashCode.Add(this.SlotCoord1Mask);
            hashCode.Add(this.SlotCoord2Mask);
            hashCode.Add(this.SlotCoord3Mask);
            hashCode.Add(this.SlotMask);
            hashCode.Add(this.IsSlotCoord1Tracked);
            hashCode.Add(this.IsSlotCoord2Tracked);
            hashCode.Add(this.IsSlotCoord3Tracked);
            hashCode.Add(this.IsSlotTracked);
            hashCode.Add(this.SlotCode);
            hashCode.Add(this.SlotLocMapRefno);
            hashCode.Add(this.SlotFKRefno);
            hashCode.Add(this.SlotName);
            hashCode.Add(this.SlotCoord1LocMapRefno);
            hashCode.Add(this.SlotCoord2LocMapRefno);
            hashCode.Add(this.SlotCoord3LocMapRefno);
            hashCode.Add(this.SectMask);
            hashCode.Add(this.IsSectionTracked);
            hashCode.Add(this.SectLocMapRefno);
            hashCode.Add(this.SectFKRefno);
            hashCode.Add(this.SectName);
            hashCode.Add(this.SectCode);
            hashCode.Add(this.WhsCodeMask);
            hashCode.Add(this.WhsLocMapRefno);
            hashCode.Add(this.WhsFKRefno);
            hashCode.Add(this.WhsCode);
            hashCode.Add(this.WhsName);
            hashCode.Add(this.IsWhsWMS);
            return hashCode.ToHashCode();
        }
    }
}