// <copyright file="BarCodeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// This is the abstract barcode scanner class.
    /// </summary>
    public abstract class BarCodeScanner : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeScanner"/> class.
        /// </summary>
        public BarCodeScanner()
        {
            _ = this.Initialize();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BarCodeScanner"/> class.
        /// </summary>
        ~BarCodeScanner()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Bar code scan event handler delegate.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated the event.</param>
        /// <param name="e"> <see cref="BarCodeScannerEventArgs"/> Scanner event arguments.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public delegate Task BarCodeScanEventHandler(object sender, BarCodeScannerEventArgs e);

        /// <summary>
        /// Bar code scan event handler.
        /// </summary>
        public event BarCodeScanEventHandler BarCodeScan;

        /// <summary>
        /// Initialize the scanner engine.
        /// </summary>
        /// <returns><see cref="bool"/> True if the scanner was initialized succesfully, else false.</returns>
        public abstract bool Initialize();

        /// <summary>
        /// Turn on bar code scanning.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public abstract Task Start();

        /// <summary>
        /// Turn off bar code scanning.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public abstract Task Stop();

        /// <summary>
        /// Unload the initialized bar code scanning engine.
        /// </summary>
        public abstract void Terminate();

        /// <summary>
        /// Raise the bar code scan event.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public virtual async Task OnBarCodeScan(BarCodeScannerEventArgs e)
        {
            await this.BarCodeScan?.Invoke(this, e);
        }

        /// <summary>
        /// Dispose the bar code scanner engine.
        /// </summary>
        /// <param name="disposing"><see cref="bool"/> True to indicating that the engine is
        /// currently disposing.</param>
        public abstract void Dispose(bool disposing);

        /// <summary>
        /// Dispose the bar code scanner engine.
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
