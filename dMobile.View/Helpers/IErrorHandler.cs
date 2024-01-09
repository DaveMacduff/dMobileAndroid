// <copyright file="IErrorHandler.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Error handler interface.
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Handle error.
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> Exception.</param>
        void HandleError(Exception ex);
    }
}
