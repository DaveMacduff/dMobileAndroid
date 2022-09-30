// <copyright file="clsSkidInfoActivityItemFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="SkidInfoActItemFactory" />.
    /// </summary>
    public class SkidInfoActItemFactory : ActItemFactory
    {
        /// <summary>
        /// The GetActItem.
        /// </summary>
        /// <param name="activity">The activity<see cref="Activity"/>.</param>
        /// <returns>The <see cref="ActItem"/>.</returns>
        public override ActItem GetActItem(Activity.Activity activity)
        {
            return new SkidInfoActItem(activity);
        }
    }
}
