// <copyright file="IDataAccess.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    /// <summary>
    /// Interface for data access event listeners.
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// Called when there is a status update from the data access event publisher.
        /// </summary>
        /// <param name="dataAccessEventPublisher">The data access event publisher.</param>
        /// <param name="dataAccessEventArgs">The data access event arguments.</param>
        void OnDataAccessStatusOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs);

        /// <summary>
        /// Called when there is a progress update from the data access event publisher.
        /// </summary>
        /// <param name="dataAccessEventPublisher">The data access event publisher.</param>
        /// <param name="dataAccessEventArgs">The data access event arguments.</param>
        void OnDataAccessProgressOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs);
    }

    /// <summary>
    /// Interface for data access event publishers.
    /// </summary>
    public interface IDataAccessEventPublisher
    {
        /// <summary>
        /// Registers a listener for status updates.
        /// </summary>
        /// <param name="dataAccess">The data access event listener.</param>
        void RegisterDataAccessListenerStatusOut(IDataAccess dataAccess);

        /// <summary>
        /// Unregisters a listener for status updates.
        /// </summary>
        /// <param name="dataAccess">The data access event listener.</param>
        void UnregisterDataAccessListenerStatusOut(IDataAccess dataAccess);

        /// <summary>
        /// Registers a listener for progress updates.
        /// </summary>
        /// <param name="dataAccess">The data access event listener.</param>
        void RegisterDataAccessListenerProgressOut(IDataAccess dataAccess);

        /// <summary>
        /// Unregisters a listener for progress updates.
        /// </summary>
        /// <param name="dataAccess">The data access event listener.</param>
        void UnregisterDataAccessListenerProgressOut(IDataAccess dataAccess);
    }
}