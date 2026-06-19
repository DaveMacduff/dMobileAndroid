// <copyright file="ZebraDataWedgeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class ZebraDataWedgeScanner : BarCodeScanner
    {
        private readonly ZebraIntentScannerService scanner = ZebraIntentScannerService.Instance;

        public override Task OpenBarcodeReader()
        {
            throw new NotImplementedException();
        }

        public override Task CloseBarcodeReader()
        {
            throw new NotImplementedException();
        }

        public override Task EnableScanning()
        {
            throw new NotImplementedException();
        }

        public override Task DisableScanning()
        {
            throw new NotImplementedException();
        }

        public override void Terminate()
        {
            throw new NotImplementedException();
        }

        public override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
