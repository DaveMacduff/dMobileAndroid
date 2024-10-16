// <copyright file="ReplenishmentMoveType.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources
{
    using System;

    /// <summary>
    /// Represents replenishment move types.
    /// </summary>
    public class ReplenishmentMoveType
    {
        /// <summary>
        /// Replenishment move type codes.
        /// </summary>
        public enum Code : int
        {
            /// <summary>
            /// Defines the Manual move type.
            /// </summary>
            Manual = 0,

            /// <summary>
            /// Defines the PullDown move type.
            /// </summary>
            PullDown = 1,

            /// <summary>
            /// Defines the Restock move type.
            /// </summary>
            Restock = 2,
        }

        /// <summary>
        /// Converts an enum <see cref="ReplenishmentMoveType.Code"/> value to a reason string.
        /// </summary>
        /// <param name="subType">The <see cref="ReplenishmentMoveType.Code"/> enum value.</param>
        /// <returns>The reason name string.</returns>
        public static string? Enum2String(Code subType)
        {
            return Enum.GetName(typeof(Code), subType);
        }

        /// <summary>
        /// Converts a sub type name string to an enum <see cref="ReplenishmentMoveType.Code"/> value.
        /// </summary>
        /// <param name="reason">The code name string.</param>
        /// <returns>The <see cref="ReplenishmentMoveType.Code"/> enum value.</returns>
        public static Code TypeString2Enum(string reason)
        {
            return (Code)Enum.Parse(typeof(Code), reason);
        }
    }
}