namespace dMobile.BarCodeScanner
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// This is the abstract barcode scanner class.
    /// </summary>
    public abstract class BarCodeScanner : IDisposable
    {
        #region Constructors

        public BarCodeScanner()
        {
            // If we can initialize the scanner...
            // if (Initialize())
            // ...start a scan on the scanner
            // this.Start();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await Initialize();
        }

        #endregion

        #region Delegates

        public delegate void BarCodeScanEventHandler(object sender, BarCodeScannerEventArgs e);

        #endregion

        #region Events

        public event BarCodeScanEventHandler BarCodeScan;

        #endregion

        #region Public Members

        // Methods that need to be implemented in subclass.
        public abstract Task<bool> Initialize();
        public abstract Task Start();
        public abstract Task Stop();
        public abstract void Terminate();

        /// <summary>
        /// Event that calls the delegate.
        /// </summary>
        public virtual void OnBarCodeScan(BarCodeScannerEventArgs e)
        {
            this.BarCodeScan?.Invoke(this, e);
        }

        #region IDisposable

        public abstract void Dispose(bool disposing);

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BarCodeScanner()
        {
            Dispose(false);
        }

        #endregion

        #endregion
    }
}
