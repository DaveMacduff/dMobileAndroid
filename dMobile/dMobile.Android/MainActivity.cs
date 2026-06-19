// <copyright file="MainActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.BarCodeScanner.BarCodeScanners;
using dMobile.View.Pages;

namespace dMobile.Droid
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Views;
    using Bedrock;
    using Resources.Enums;
    using System;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="MainActivity" /> class.
    /// </summary>
    [Activity(
        Label = "dMobile",
        Icon = "@drawable/dtec_logo",
        Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        // DataWedge intent action — configure this in your DataWedge profile
        private const string ZebraIntentAction = "com.dmobile.ACTION";  // ← must match DataWedge
        private const string ExtraDataKey = "com.symbol.datawedge.data_string";
        private const string ExtraTypeKey = "com.symbol.datawedge.label_type";

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            // Add this log line temporarily to confirm OnNewIntent IS being called at all
            Android.Util.Log.Debug("ZEBRA", $"OnNewIntent fired. Action: {intent?.Action}");

            if (intent?.Action != ZebraIntentAction)
            {
                Android.Util.Log.Debug("ZEBRA", $"Action mismatch. Expected: {ZebraIntentAction}, Got: {intent?.Action}");
                return;
            }

            var barcodeData = intent.GetStringExtra(ExtraDataKey);
            var barcodeType = intent.GetStringExtra(ExtraTypeKey);

            Android.Util.Log.Debug("ZEBRA", $"Barcode: {barcodeData}, Type: {barcodeType}");

            ZebraIntentScannerService.Instance.RaiseBarcodeScanned(
                barcodeData ?? string.Empty,
                barcodeType ?? string.Empty);
        }

        /// <summary>
        /// Define the on key up.
        /// </summary>
        /// <param name="keyCode"><see cref="Keycode"/> Key code.</param>
        /// <param name="e"><see cref="KeyEvent"/> key event.</param>
        /// <returns><see cref="bool"/> True if key was handled.</returns>
        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            try
            {
                MessagingCenter.Send(keyCode.ToString(), EventTypes.OnKeyUp.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return base.OnKeyUp(keyCode, e);
        }

        /// <summary>
        /// On create application event handler.
        /// </summary>
        /// <param name="savedInstanceState"><see cref="Bundle"/> Saved instance.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Forms.Init(this, savedInstanceState);
            this.LoadApplication(new App());
        }

        /// <summary>
        /// When the ScanPage is about to go to the background, release the scanner.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            var scanner = ScannerEngine.GetInstance().Scanner;
            scanner.CloseBarcodeReader().FireAndForgetSafeAsync();
        }

        /// <summary>
        /// When the ScanPage is about to go to the foreground, claim the scanner.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            var scanner = ScannerEngine.GetInstance().Scanner;
            scanner.OpenBarcodeReader().FireAndForgetSafeAsync();
        }
    }
}
