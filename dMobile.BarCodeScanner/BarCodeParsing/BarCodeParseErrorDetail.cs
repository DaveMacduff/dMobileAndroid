namespace dMobile.BarCodeScanner
{
    using dMobile.Bedrock;
    public class BarCodeParseErrorDetail
    {
        BarCodeExceptionReason reason;
        BarCodeParseException exception;

        public BarCodeParseErrorDetail(
            BarCodeExceptionReason reason,
            BarCodeParseException exception)
        {
            this.reason = reason;
            this.exception = exception;
        }

        /// <summary>
        /// Gets the Bar Code Exception type.
        /// </summary>
        public BarCodeExceptionReason Reason
        {
            get
            {
                return this.reason;
            }
        }

        /// <summary>
        /// Gets the Bar Code Parse Exception.
        /// </summary>
        public BarCodeParseException Exception
        {
            get
            {
                return this.exception;
            }
        }
    }
}
