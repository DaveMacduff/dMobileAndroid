using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using dMobile.GroundWork;
using System.Data.SQLite;
using dMobile.Bedrock;
using System.Data;

namespace BarcodeReaderSample.Droid
{
    [Activity(Label = "BarcodeReaderSample", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        private BarcodeReaderSample.App mBarcodeReaderSampleApp;
        private BarcodeReaderSample.MainPage mMainPage;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication((mBarcodeReaderSampleApp = new BarcodeReaderSample.App()));
            mMainPage = (BarcodeReaderSample.MainPage)mBarcodeReaderSampleApp.MainPage;
            DatabaseNub test = new DatabaseNub();
            test.CreateTables();
        }

        /// <summary>
        /// When the ScanPage is about to go to the background, release the
        /// scanner.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            mMainPage.CloseBarcodeReader();
        }

        /// <summary>
        /// When the ScanPage is about to go to the foreground, claim the
        /// scanner.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            mMainPage.OpenBarcodeReader();

            //lock the orientationt to Portrait
            this.RequestedOrientation = ScreenOrientation.Portrait;
        }
    }

}

