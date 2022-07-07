// <copyright file="BarCodeAssocActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="BarCodeAssocActDetailFactory" /> Bar code association activity detail factory.
    /// </summary>
    public class BarCodeAssocActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the bar code associate activity detail.
        /// </summary>
        /// <param name="activityItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns>The <see cref="ActDetail"/> Activity detail.</returns>
        public override ActDetail GetActDetail(ActItem activityItem)
        {
            return new BarCodeAssocActDetail(activityItem);
        }
    }
}
