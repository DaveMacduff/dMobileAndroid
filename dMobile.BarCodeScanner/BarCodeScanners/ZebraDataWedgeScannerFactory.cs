// <copyright file="ZebraDataWedgeScannerFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ZebraDataWedgeScannerFactory : BarCodeScannerFactory
    {
        public override BarCodeScanner GetBarCodeScanner()
        {
            return new ZebraDataWedgeScanner();
        }
    }
}
