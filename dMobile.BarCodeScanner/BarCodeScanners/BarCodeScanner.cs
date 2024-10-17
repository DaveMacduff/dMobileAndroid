// <copyright file="BarCodeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// This is the abstract barcode scanner class.
    /// </summary>
    public abstract class BarCodeScanner : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeScanner"/> class.
        /// </summary>
        protected BarCodeScanner()
        {
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
        public event BarCodeScanEventHandler OnBarCodeScan;

        /// <summary>
        /// Open the scanner reader and load symbology settings.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public abstract Task OpenBarcodeReader();

        /// <summary>
        /// Close the scanner reader.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public abstract Task CloseBarcodeReader();

        /// <summary>
        /// Turn on bar code scanning.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public abstract Task EnableScanning();

        /// <summary>
        /// Turn off bar code scanning.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public abstract Task DisableScanning();

        /// <summary>
        /// Unload the initialized bar code scanning engine.
        /// </summary>
        public abstract void Terminate();

        /// <summary>
        /// Raise the bar code scan event.
        /// </summary>
        /// <param name="scanEventArgs"><see cref="BarCodeScannerEventArgs"/> Scanner event arguments.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public virtual async Task RaiseBarCodeScan(BarCodeScannerEventArgs scanEventArgs)
        {
            if (this.OnBarCodeScan != null)
            {
                await this.OnBarCodeScan.Invoke(this, scanEventArgs);
            }
        }

        /// <summary>
        /// Clear all bar code scan event handlers.
        /// </summary>
        public virtual void ClearBarCodeScanEventHandlers()
        {
            if (this.OnBarCodeScan != null)
            {
                // Remove all event handlers. Hopefully this will prevent memory leaks, and possible multicast delegate issues.
                foreach (var handler in this.OnBarCodeScan.GetInvocationList())
                {
                    this.OnBarCodeScan -= (BarCodeScanEventHandler)handler;
                }

                this.OnBarCodeScan = null;
            }
        }

        /// <summary>
        /// Check if bar code scanning event have been added.
        /// </summary>
        /// <returns><see cref="bool"/> True if the event handler has been added, else false.</returns>
        public virtual bool IsBarCodeScanEventHandlerAdded()
        {
            var isAdded = false;
            if (this.OnBarCodeScan != null)
            {
                // Check if the event handler has been added to the delegate invocation list.
                if (this.OnBarCodeScan.GetInvocationList().Any())
                {
                    isAdded = true;
                }
            }

            return isAdded;
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
