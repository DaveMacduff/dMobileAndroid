// <copyright file="SysSerialNumber.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Provides a thread-safe, monotonically increasing serial number generator.
    /// </summary>
    /// <remarks>Each call to <see cref="GetNextSerialNumber"/> is guaranteed to return a value
    /// greater than any value previously returned by this generator. The value fits within a
    /// maximum of 20 digits (the number of digits in <see cref="ulong.MaxValue"/>).</remarks>
    public static class SysSerialNumber
    {
        /// <summary>
        /// Synchronizes access to <see cref="lastSerialNumber"/>.
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        /// Contains the last serial number value returned by <see cref="GetNextSerialNumber"/>.
        /// </summary>
        private static ulong lastSerialNumber;

        /// <summary>
        /// Gets a new serial number value that is unique relative to every other value previously
        /// returned by this method during the lifetime of the process.
        /// </summary>
        /// <remarks>The returned value is derived from <see cref="DateTime.UtcNow"/> ticks, which
        /// provides 100-nanosecond resolution. If called fast enough that the current tick count
        /// would not be greater than the last value returned, the value is incremented instead to
        /// preserve uniqueness and monotonic ordering. The result will never exceed 20 digits, since
        /// it is bounded by <see cref="ulong.MaxValue"/>.</remarks>
        /// <returns>A unique, monotonically increasing serial number of up to 20 digits.</returns>
        public static ulong GetNextSerialNumber()
        {
            lock (SyncLock)
            {
                var candidate = (ulong)DateTime.UtcNow.Ticks;
                lastSerialNumber = candidate > lastSerialNumber ? candidate : lastSerialNumber + 1;
                return lastSerialNumber;
            }
        }
    }
}