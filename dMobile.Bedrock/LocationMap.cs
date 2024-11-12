// <copyright file="LocationMap.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Warehouse location map.
    /// </summary>
    [Serializable]
    public class LocationMap
    {
        /// <summary>
        /// Define the current location map reference number.
        /// </summary>
        private int currLocMapRefno;

        /// <summary>
        /// Define the user chosen location map reference number.
        /// </summary>
        private int chosenLocMapRefno;

        /// <summary>
        /// Prevents a default instance of the <see cref="LocationMap"/> class from being created.
        /// </summary>
        private LocationMap()
        {
        }

        /// <summary>
        /// Gets or sets the LoginWhsLocMapRefno.
        /// </summary>
        public int LoginWhsLocMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the current location map.
        /// </summary>
        /// <remarks>
        /// This is the current selected location. Set by the location defaulting, scanning, user selection screen,
        /// or assigned by the chosen location map.
        /// </remarks>
        public SLocMap CurrLocMap { get; set; } = new SLocMap();

        /// <summary>
        /// Gets or sets the chosen location map.
        /// </summary>
        /// <remarks>
        /// This is the user selected chosen location. Set by scanning or using the location selection screen.
        /// This is not the same as the current, because the current can be changed by defaulting. Where as the
        /// chosen location is set by the user and not changed except by the user.
        /// </remarks>
        public SLocMap ChosenLocMap { get; set; } = new SLocMap();

        /// <summary>
        /// Gets or sets the current location map reference number.
        /// </summary>
        public int CurrLocMapRefno
        {
            get
            {
                if (this.CurrLocMap.SlotLocMapRefno != 0)
                {
                    this.currLocMapRefno = this.CurrLocMap.SlotLocMapRefno;
                }
                else if (this.CurrLocMap.SectLocMapRefno != 0)
                {
                    this.currLocMapRefno = this.CurrLocMap.SectLocMapRefno;
                }
                else
                {
                    this.currLocMapRefno = this.CurrLocMap.WhsLocMapRefno;
                }

                return this.currLocMapRefno;
            }

            set => this.currLocMapRefno = value;
        }

        /// <summary>
        /// Gets or sets the current location map reference number.
        /// </summary>
        public int ChosenLocMapRefno
        {
            get
            {
                if (this.ChosenLocMap.SlotLocMapRefno != 0)
                {
                    this.chosenLocMapRefno = this.ChosenLocMap.SlotLocMapRefno;
                }
                else if (this.ChosenLocMap.SectLocMapRefno != 0)
                {
                    this.chosenLocMapRefno = this.ChosenLocMap.SectLocMapRefno;
                }
                else
                {
                    this.chosenLocMapRefno = this.ChosenLocMap.WhsLocMapRefno;
                }

                return this.chosenLocMapRefno;
            }

            set => this.chosenLocMapRefno = value;
        }

        /// <summary>
        /// Gets or sets the location map instance.
        /// </summary>
        private static LocationMap LocMap { get; set; }

        /// <summary>
        /// The GetInstance.
        /// </summary>
        /// <returns>.</returns>
        public static LocationMap GetInstance()
        {
            return LocMap ??= new LocationMap();
        }
    }
}
