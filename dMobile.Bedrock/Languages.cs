namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// List of supported languages.
    /// </summary>
    [Serializable]
    public enum Languages
    {
        /// <summary>
        /// Language cannot be parsed or displayed.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// American English
        /// </summary>
        ENU = 1,

        /// <summary>
        /// Canadian English
        /// Resolves to American English currently (ENU).
        /// </summary>
        ENC = 2,

        /// <summary>
        /// Canadian French
        /// </summary>
        FRC = 3,
    }
}
