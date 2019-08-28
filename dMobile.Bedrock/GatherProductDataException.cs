namespace dMobile.Bedrock
{
    using System;

    public class GatherProductDataException : Exception
    {
        #region Fields

        private bool suppressError = false;

        #endregion

        #region Properties

        public bool SuppressError
        {
            get
            {
                return this.suppressError;
            }

            set
            {
                this.suppressError = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public GatherProductDataException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public GatherProductDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public GatherProductDataException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="suppressError"></param>
        public GatherProductDataException(string message, bool suppressError)
            : base(message)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="suppressError"></param>
        public GatherProductDataException(string message, Exception inner, bool suppressError)
            : base(message, inner)
        {
            this.SuppressError = suppressError;
        }

        #endregion
    }
}
