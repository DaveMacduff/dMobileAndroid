// <copyright file="SysSettingsEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="SysSettingsEventArgs" />.
    /// </summary>
    public class SysSettingsEventArgs : EventArgs
    {
        /// <summary>
        /// Defines the currentValue.
        /// </summary>
        private int currentValue = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysSettingsEventArgs"/> class.
        /// </summary>
        public SysSettingsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SysSettingsEventArgs"/> class.
        /// </summary>
        /// <param name="msgData">.</param>
        public SysSettingsEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        /// <summary>
        /// Gets or sets the StatusOutMsg.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MinValue.
        /// </summary>
        public int MinValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the MaxValue.
        /// </summary>
        public int MaxValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the CurrentValue.
        /// </summary>
        public int CurrentValue
        {
            get
            {
                return this.currentValue;
            }

            set
            {
                this.currentValue = value;
            }
        }
    }
}
