namespace dMobile.Bedrock
{
    using System;

    public class BarCodeParseException : Exception
    {
        public BarCodeParseException(string message, string field, Exception innerException)
            : base(
            // TODO: Resource strings
            string.Format("Could not parse '{0}'. {1}", field, message),
            innerException)
        {
        }

        public BarCodeParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
