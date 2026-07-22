// <copyright file="ZebraDataWedgeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System.Threading.Tasks;

    public class ZebraDataWedgeScanner : BarCodeScanner
    {
        private readonly ZebraIntentScannerService scanner = ZebraIntentScannerService.Instance;
        

        public override Task OpenBarcodeReader()
        {
            return Task.CompletedTask;
        }

        public override Task CloseBarcodeReader()
        {
            return Task.CompletedTask;
        }

        public override Task EnableScanning()
        {
            return Task.CompletedTask;
        }

        public override Task DisableScanning()
        {
            return Task.CompletedTask;
        }

        public override void Terminate()
        {
        }

        public override void Dispose(bool disposing)
        {
        }
    }
}
