// <copyright file="ProductSchemeLabelSettingsEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="ProdSchemeLabelSettingsEventArgs" />.
    /// </summary>
    public class ProdSchemeLabelSettingsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProdSchemeLabelSettingsEventArgs"/> class.
        /// </summary>
        public ProdSchemeLabelSettingsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProdSchemeLabelSettingsEventArgs"/> class.
        /// </summary>
        /// <param name="msgData">.</param>
        public ProdSchemeLabelSettingsEventArgs(string msgData)
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
        public int CurrentValue { get; set; } = 0;
    }
}
