// <copyright file="clsMoveSkidActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="clsMoveSkidActivityFactory" />.
    /// </summary>
    public class clsMoveSkidActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>.</returns>
        public override Activity GetActivity()
        {
            return new MoveSkidActivity();
        }
    }
}
