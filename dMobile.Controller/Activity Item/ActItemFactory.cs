// <copyright file="ActItemFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Activity item factory.
    /// </summary>
    public abstract class ActItemFactory
    {
        /// <summary>
        /// Get the activity item.
        /// </summary>
        /// <param name="activity">y<see cref="Activity"/> Activity header.</param>
        /// <returns>The <see cref="ActItem"/> Activity item object.</returns>
        public abstract ActItem GetActItem(Activity.Activity activity);
    }
}
