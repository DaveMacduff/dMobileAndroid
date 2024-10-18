// <copyright file="IGridService.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface for grid services.
    /// </summary>
    public interface IGridService
    {
        /// <summary>
        /// Calculates the width of the given text.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>The width of the text.</returns>
        double CalculateTextWidth(string text);
    }
}