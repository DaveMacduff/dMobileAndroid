// <copyright file="IPrinterSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Defines the <see cref="IPrnSettingsListener" />.
    /// </summary>
    public interface IPrnSettingsListener
    {
        /// <summary>
        /// The OnPrnSettingsStatusOut.
        /// </summary>
        /// <param name="prnSettingsPublisher">The prnSettingsPublisher<see cref="IPrnSettingsPublisher"/>.</param>
        /// <param name="prnSettingsArgs">The prnSettingsArgs<see cref="PrnSettingsArgs"/>.</param>
        void OnPrnSettingsStatusOut(
            IPrnSettingsPublisher prnSettingsPublisher,
            PrnSettingsArgs prnSettingsArgs);

        /// <summary>
        /// The OnPrnSettingsProgressOut.
        /// </summary>
        /// <param name="prnSettingsPublisher">The prnSettingsPublisher<see cref="IPrnSettingsPublisher"/>.</param>
        /// <param name="prnSettingsArgs">The prnSettingsArgs<see cref="PrnSettingsArgs"/>.</param>
        void OnPrnSettingsProgressOut(
            IPrnSettingsPublisher prnSettingsPublisher,
            PrnSettingsArgs prnSettingsArgs);
    }

    /// <summary>
    /// Defines the <see cref="IPrnSettingsPublisher" />.
    /// </summary>
    public interface IPrnSettingsPublisher
    {
        /// <summary>
        /// The RegPrnSettingsListenerStatusOut.
        /// </summary>
        /// <param name="prnSettingsListener">The prnSettingsListener<see cref="IPrnSettingsListener"/>.</param>
        void RegPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingsListener);

        /// <summary>
        /// The UnregPrnSettingsListenerStatusOut.
        /// </summary>
        /// <param name="prnSettingListener">The prnSettingListener<see cref="IPrnSettingsListener"/>.</param>
        void UnregPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingListener);

        /// <summary>
        /// The RegPrnSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prnSettingsListener">The prnSettingsListener<see cref="IPrnSettingsListener"/>.</param>
        void RegPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener);

        /// <summary>
        /// The UnregPrnSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prnSettingsListener">The prnSettingsListener<see cref="IPrnSettingsListener"/>.</param>
        void UnregPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener);
    }
}
