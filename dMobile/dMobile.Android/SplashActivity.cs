// <copyright file="SplashActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
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
        /// The OnCreate.
        /// </summary>
        /// <param name="savedInstanceState">The savedInstanceState<see cref="Bundle"/>.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // TODO: Update?
            base.OnCreate(savedInstanceState);
        }

        /// <summary>
        /// The OnResume.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            this.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        // private bool Install()
        // {
        //    string filePath = string.Empty;

        // try
        //    {
        //        if (TextUtils.IsEmpty(filePath) || !File.Exists(filePath))
        //        {
        //            return false;
        //        }

        // Intent intent = new Intent(Intent.ActionView);
        //        intent.SetFlags(ActivityFlags.NewTask);

        // if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
        //        {
        //            // Add read and write permissions
        //            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
        //        }

        // intent.SetDataAndType(this.GetPathUri(this,filePath), "application/vnd.android.package-archive");
        //        StartActivity(intent);
        //    }
        //    catch (Exception e)
        //    {
        //        Toast.MakeText(this, "Installation failed. Please download again", ToastLength.Short).Show();
        //        return false;
        //    }

        // return true;
        // }

        // private Android.Net.Uri GetPathUri(Context context, string filePath)
        // {
        //    Android.Net.Uri uri;
        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
        //    {
        //        string packageName = context.PackageName;
        //        uri = FileProvider.GetUriForFile(context, packageName + ".fileProvider", new File(filePath));
        //    }
        //    else
        //    {
        //        uri = Android.Net.Uri.FromFile(new File(filePath));
        //    }
        //    return uri;
        // }
    }
}