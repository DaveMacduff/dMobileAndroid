// <copyright file="ReplenishmentMoveType.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources
{
    using System;

    /// <summary>
    /// Replenishment move types.
    /// </summary>
    public class ReplenishmentMoveType
    {
        /// <summary>
        /// Replenishment move type codes.
        /// </summary>
        public enum Code : int
        {
            /// <summary>
            /// Defines the Manual
            /// </summary>
            Manual = 0,

            /// <summary>
            /// Defines the PullDown
            /// </summary>
            PullDown = 1,

            /// <summary>
            /// Defines the Restock
            /// </summary>
            Restock = 2,
        }

        /// <summary>
        /// Convert enum ReplenishmentMoveType.Code value to reason string.
        /// </summary>
        /// <param name="subType">ReplenishmentMoveType.Code enum value</param>
        /// <returns>Reason name string.</returns>
        public static string? Enum2String(Code subType)
        {
            return Enum.GetName(typeof(Code), subType);
        }

        /// <summary>
        /// Convert sub type name string to enum ReplenishmentMoveType.Code value.
        /// </summary>
        /// <param name="reason">Code name string.</param>
        /// <returns>ReplenishmentMoveType.Code enum.</returns>
        public static Code TypeString2Enum(string reason)
        {
            return (Code)Enum.Parse(typeof(Code), reason);
        }
    }
}
