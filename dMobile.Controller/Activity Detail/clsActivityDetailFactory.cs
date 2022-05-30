// <copyright file="clsActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="ActDetailFactory" />.
    /// </summary>
    public abstract class ActDetailFactory
    {
        /// <summary>
        /// Get an activity detail instance.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns>The <see cref="ActDetail"/> Activity detail instance.</returns>
        public abstract ActDetail GetActDetail(ActItem actItem);
    }
}
