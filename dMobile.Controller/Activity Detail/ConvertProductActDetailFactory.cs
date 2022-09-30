// <copyright file="ConvertProductActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Convert product activity detail factory.
    /// </summary>
    public class ConvertProductActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get a convert product activity detail object.
        /// </summary>
        /// <param name="actItem"><see cref="ActDetail"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new ConvertProductActDetail(actItem);
        }
    }
}
