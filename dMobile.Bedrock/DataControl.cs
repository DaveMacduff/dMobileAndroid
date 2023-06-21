// <copyright file="DataControl.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System.Collections.Generic;

    /// <summary>
    ///  Data control singleton.
    /// </summary>
    public class DataControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataControl"/> class.
        /// </summary>
        private DataControl()
        {
        }

        /// <summary>
        /// Gets the data control instance.
        /// </summary>
        public static DataControl GetInstance => Instance;

        /// <summary>
        /// Gets or sets a value indicating whether the semaphore to indicate data is loaded.
        /// </summary>
        public IDictionary<SysCommon.ActSubTypes, bool> Loading { get; set; }
            = new Dictionary<SysCommon.ActSubTypes, bool>();

        /// <summary>
        /// Gets or sets the data control instance.
        /// </summary>
        private static DataControl Instance { get; set; } = new DataControl();
    }
}
