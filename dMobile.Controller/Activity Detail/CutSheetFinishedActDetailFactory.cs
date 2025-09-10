// <copyright file="CutSheetFinishedActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="CutSheetFinishedActDetailFactory" /> Cut sheet finished detail factory.
    /// </summary>
    public class CutSheetFinishedActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the cut sheet finished detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Cut sheet finished detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new CutSheetFinishedActDetail(actItem);
        }
    }
}
