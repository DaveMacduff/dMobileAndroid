namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;

    public class BarCodeParseException : Exception
    {
        public BarCodeParseException(string message, string field, Exception innerException)
            : base(
#if MULTILANGUAGE
            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.CouldNotParse), field, message),
#else
            string.Format("Could not parse '{0}'. {1}", field, message),
#endif
            innerException)
        {
        }

        public BarCodeParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
