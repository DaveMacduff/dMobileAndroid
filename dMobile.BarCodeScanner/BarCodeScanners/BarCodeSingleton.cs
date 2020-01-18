namespace dMobile.BarCodeScanner
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BarCodeSingleton
    {
        #region Fields

        private static BarCodeSingleton barCodeUniq = new BarCodeSingleton();
        private static BarCodeScanner scanner = BarCodeScannerFacade.GetBarCodeScanner();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private BarCodeSingleton()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public BarCodeScanner Scanner
        {
            get
            {
                if (scanner == null)
                {
                    scanner = BarCodeScannerFacade.GetBarCodeScanner();
                }

                return scanner;
            }

            set
            {
                scanner = value;
            }
        }

        private static BarCodeSingleton BarCodeUniq
        {
            get
            {
                return barCodeUniq;
            }

            set
            {
                barCodeUniq = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BarCodeSingleton GetInstance()
        {
            if (BarCodeUniq == null)
            {
                BarCodeUniq = new BarCodeSingleton();
            }

            return BarCodeUniq;
        }

        #endregion
    }
}
