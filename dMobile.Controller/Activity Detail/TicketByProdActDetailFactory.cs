// <copyright file="TicketByProdActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="TicketByProdDetailFactory" /> Production ticket by-product activity detail factory.
    /// </summary>
    public class TicketByProdDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// The GetActDetail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new TicketByProdActDetail(actItem);
        }
    }
}
