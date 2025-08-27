// <copyright file="IProdSchemeLabelSettingsListener.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    /// <summary>
    /// Defines the <see cref="IProdSchemeLabelSettingsListener" />.
    /// </summary>
    public interface IProdSchemeLabelSettingsListener
    {
        /// <summary>
        /// The OnProdSchemeLabelSettingsStatusOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/>.</param>
        /// <param name="prodSchemeLabelSettingsArgs">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/>.</param>
        void OnProdSchemeLabelSettingsStatusOut(
            IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
            ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

        /// <summary>
        /// The OnProdSchemeLabelSettingsProgressOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/>.</param>
        /// <param name="prodSchemeLabelSettingsArgs">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/>.</param>
        void OnProdSchemeLabelSettingsProgressOut(
            IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
            ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);
    }

    /// <summary>
    /// Defines the <see cref="IProdSchemeLabelSettingsPublisher" />.
    /// </summary>
    public interface IProdSchemeLabelSettingsPublisher
    {
        /// <summary>
        /// The RegProdSchemeLabelSettingsListenerStatusOut.
        /// </summary>
        /// <param name="rodSchemeLabelSettingsListener">The rodSchemeLabelSettingsListener<see cref="IProdSchemeLabelSettingsListener"/>.</param>
        void RegProdSchemeLabelSettingsListenerStatusOut(IProdSchemeLabelSettingsListener rodSchemeLabelSettingsListener);

        /// <summary>
        /// The UnregProdSchemeLabelSettingsListenerStatusOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingListener">The prodSchemeLabelSettingListener<see cref="IProdSchemeLabelSettingsListener"/>.</param>
        void UnregProdSchemeLabelSettingsListenerStatusOut(IProdSchemeLabelSettingsListener prodSchemeLabelSettingListener);

        /// <summary>
        /// The RegProdSchemeLabelSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener">The prodSchemeLabelSettingsListener<see cref="IProdSchemeLabelSettingsListener"/>.</param>
        void RegProdSchemeLabelSettingsListenerProgressOut(IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener);

        /// <summary>
        /// The UnregProdSchemeLabelSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener">The prodSchemeLabelSettingsListener<see cref="IProdSchemeLabelSettingsListener"/>.</param>
        void UnregProdSchemeLabelSettingsListenerProgressOut(IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener);
    }
}
