// <copyright file="SplashActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;

namespace dMobile.Droid
{
    /// <summary>
    /// Defines the <see cref="SplashActivity" />.
    /// </summary>
    [Activity(
        Theme = "@style/SplashScreen",
        MainLauncher = true,
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        /// <summary>
        /// The OnResume.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            this.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}