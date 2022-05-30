// <copyright file="clsReturnsActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="ReturnsActDetailFactory" /> Returns activity detail factory.
    /// </summary>
    public class ReturnsActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the returns activity detail factory.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Returns activity details.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new ReturnsActDetail(actItem);
        }
    }
}
