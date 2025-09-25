// <copyright file="ResSysCommon.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources
{
    using System;

    /// <summary>
    /// Class of common shared methods.
    /// </summary>
    public class ResSysCommon
    {
        /// <summary>
        /// Define the empty date time.
        /// </summary>
        public static readonly DateTime EmptyDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Local);

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public enum ActStatus : int
        {
            /// <summary>
            /// Defines the NoChange.
            /// </summary>
            NoChange = 0,

            /// <summary>
            /// Defines the Inserted.
            /// </summary>
            Inserted = 1,

            /// <summary>
            /// Defines the Updated.
            /// </summary>
            Updated = 2,

            /// <summary>
            /// Defines the Deleted.
            /// </summary>
            Deleted = 3,
        }
    }
}
