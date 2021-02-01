namespace dMobile.Droid
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Net;
    using Android.OS;
    using Android.Support.V7.App;
    using Android.Util;
    using Android.Widget;
    //using System;
    using System.Net;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Android.Text;
    using static Android.Icu.Text.IDNA;
    using System.IO;
    using Android.Support.V4.Content;
    using System;

    /// <summary>
    /// Defines the <see cref="SplashActivity" />
    /// </summary>
    [Activity(Theme = "@style/SplashScreen", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        /// <summary>
        /// The OnCreate
        /// </summary>
        /// <param name="savedInstanceState">The savedInstanceState<see cref="Bundle"/></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // TODO: Update?
            base.OnCreate(savedInstanceState);
        }

        /// <summary>
        /// The OnResume
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            Task startup = Task.Run(() =>
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            });

            startup.Wait();
        }

        //private bool Install()
        //{
        //    string filePath = string.Empty;

        //    try
        //    {
        //        if (TextUtils.IsEmpty(filePath) || !File.Exists(filePath))
        //        {
        //            return false;
        //        }

        //        Intent intent = new Intent(Intent.ActionView);
        //        intent.SetFlags(ActivityFlags.NewTask);

        //        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
        //        {
        //            // Add read and write permissions
        //            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
        //        }

        //        intent.SetDataAndType(this.GetPathUri(this,filePath), "application/vnd.android.package-archive");
        //        StartActivity(intent);
        //    }
        //    catch (Exception e)
        //    {
        //        Toast.MakeText(this, "Installation failed. Please download again", ToastLength.Short).Show();
        //        return false;
        //    }

        //    return true;
        //}


        //private Android.Net.Uri GetPathUri(Context context, string filePath)
        //{
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
        //}
    }
}