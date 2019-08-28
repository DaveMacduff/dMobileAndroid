namespace dMobile.Bedrock
{
    using System;

    public class IsReadyToApplyException : Exception
    {
        #region Fields

        private bool suppressError = false;
        private bool isOverContinue = false;

        #endregion

        #region Properties

        public bool IsOverContinue
        {
            get
            {
                return this.isOverContinue;
            }

            set
            {
                this.isOverContinue = value;
            }
        }

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
        public IsReadyToApplyException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public IsReadyToApplyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public IsReadyToApplyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="suppressError"></param>
        public IsReadyToApplyException(string message, bool suppressError)
            : base(message)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="suppressError"></param>
        /// <param name="isOverContinue"></param>
        public IsReadyToApplyException(string message, bool suppressError, bool isOverContinue)
            : base(message)
        {
            this.SuppressError = suppressError;
            this.IsOverContinue = isOverContinue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="suppressError"></param>
        public IsReadyToApplyException(string message, Exception inner, bool suppressError)
            : base(message, inner)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="suppressError"></param>
        /// <param name="isOverContinue"></param>
        public IsReadyToApplyException(string message, Exception inner, bool suppressError, bool isOverContinue)
            : base(message, inner)
        {
            this.SuppressError = suppressError;
            this.IsOverContinue = isOverContinue;
        }

        #endregion
    }
}
