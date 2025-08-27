// <copyright file="IBarCodeSettingsEventListener.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    /// <summary>
    /// Defines the <see cref="IBarCodeSettingsEventListener" />.
    /// </summary>
    public interface IBarCodeSettingsEventListener
    {
        /// <summary>
        /// The OnBarCodeSettingsStatusOut.
        /// </summary>
        /// <param name="barCodeSettingsEventPublisher">The barCodeSettingsEventPublisher<see cref="IBarCodeSettingsEventPublisher"/>.</param>
        /// <param name="barCodeSettingsEventArgs">The barCodeSettingsEventArgs<see cref="BarCodeSettingsEventArgs"/>.</param>
        void OnBarCodeSettingsStatusOut(
            IBarCodeSettingsEventPublisher barCodeSettingsEventPublisher,
            BarCodeSettingsEventArgs barCodeSettingsEventArgs);

        /// <summary>
        /// The OnBarCodeSettingsProgressOut.
        /// </summary>
        /// <param name="barCodeSettingsEventPublisher">The barCodeSettingsEventPublisher<see cref="IBarCodeSettingsEventPublisher"/>.</param>
        /// <param name="barCodeSettingsEventArgs">The barCodeSettingsEventArgs<see cref="BarCodeSettingsEventArgs"/>.</param>
        void OnBarCodeSettingsProgressOut(
            IBarCodeSettingsEventPublisher barCodeSettingsEventPublisher,
            BarCodeSettingsEventArgs barCodeSettingsEventArgs);
    }

    /// <summary>
    /// Defines the <see cref="IBarCodeSettingsEventPublisher" />.
    /// </summary>
    public interface IBarCodeSettingsEventPublisher
    {
        /// <summary>
        /// The RegisterBarCodeSettingsListenerStatusOut.
        /// </summary>
        /// <param name="barCodeSettingsEventListener">The barCodeSettingsEventListener<see cref="IBarCodeSettingsEventListener"/>.</param>
        void RegisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);

        /// <summary>
        /// The UnregisterBarCodeSettingsListenerStatusOut.
        /// </summary>
        /// <param name="barCodeSettingsEventListener">The barCodeSettingsEventListener<see cref="IBarCodeSettingsEventListener"/>.</param>
        void UnregisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);

        /// <summary>
        /// The RegisterBarCodeSettingsListenerProgressOut.
        /// </summary>
        /// <param name="barCodeSettingsEventListener">The barCodeSettingsEventListener<see cref="IBarCodeSettingsEventListener"/>.</param>
        void RegisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);

        /// <summary>
        /// The UnregisterBarCodeSettingsListenerProgressOut.
        /// </summary>
        /// <param name="barCodeSettingsEventListener">The barCodeSettingsEventListener<see cref="IBarCodeSettingsEventListener"/>.</param>
        void UnregisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);
    }
}
