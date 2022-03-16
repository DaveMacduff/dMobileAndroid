namespace dMobile.Droid
{
    using Android;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Support.V4.Content;
    using Android.Views;
    using dMobile.Resources.Enums;
    using dMobile.View;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="MainActivity" />
    /// </summary>
    [Activity(Label = "dMobile", Icon = "@drawable/dtec_logo", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// The OnCreate
        /// </summary>
        /// <param name="savedInstanceState">The savedInstanceState<see cref="Bundle"/></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {

            //  Runs on app startup once to check if an update should be performed and if so
            //  downloads and installs the updated apk.
            string update = CheckForUpdate();

            if (update != "NO" && update != "ERR")
            {
                DownloadAndInstallUpdate(update);
            }

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
       
        }
        
        //  Calls the CheckForUpdate function from HomeBase api.
        //  Returns the value received: either "NO" if no update is available,
        //                                     "ERR" if there was an error in the server logic
        //                                      OR the version it should update to as a string.
        public string CheckForUpdate()
        {
            try
            {
                // Retrieve the version number of currently installed package
                PackageInfo pinfo = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0);
                if (pinfo == null)
                {
                    return "ERR";
                }

                string vnum = pinfo.VersionName;

                string uri = @"http://184.70.233.14:2857/HomeBase/CheckForUpdate?version=" + vnum;

                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 0, 3);

                HttpResponseMessage response = client.GetAsync(uri).GetAwaiter().GetResult();

                if (response != null)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error making Http Request.\n Content: " + response.Content);
                        return "ERR";
                    }

                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return content;
                }

                return "ERR";

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "ERR";
            }
        }

        //  If the application should update, this function will download the latest .apk from the HomeBase GetApk method.
        //  If the function returns false that means there was a problem installing the update. If successful the user will be prompted 
        //  to install an update to this application, upon which the application will close during the install process. 
        ///  <param name="ver">The version that is should be installed.</param>
        public bool DownloadAndInstallUpdate(string ver)
        {
            // Check to make sure permissions are sufficient to install. Otherwise prompt user.
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.WriteExternalStorage) || (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage)))
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 0);
            }

            try
            {
                // Attempt to download file
                var clients = new WebClient();
                clients.DownloadFile("http://184.70.233.14:2857/HomeBase/GetApk?version=" + ver, ApplicationContext.GetExternalFilesDir(null).AbsolutePath + "/com.dmobile.android.update.apk");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            try
            {
                // Get file directory and file name
                Java.IO.File directory = ApplicationContext.GetExternalFilesDir(null);    
                Java.IO.File file = new Java.IO.File(directory, "com.dmobile.android.update.apk");

                // Check Android version to use different activity and/or file provider
                // Starts the install of new package
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    Android.Net.Uri fileuri = FileProvider.GetUriForFile(ApplicationContext, ApplicationContext.PackageName + ".authorityStr", file);
                    Intent intentS = new Intent(Intent.ActionInstallPackage);
                    intentS.SetData(fileuri);
                    intentS.SetFlags(ActivityFlags.GrantReadUriPermission);
                    StartActivity(intentS);
                }
                else
                {
                    Android.Net.Uri fileuri = FileProvider.GetUriForFile(ApplicationContext, ".authorityStr", file);
                    Intent intentS = new Intent(Intent.ActionView);
                    intentS.SetDataAndType(fileuri, "application/vnd.android.package-archive");
                    intentS.SetFlags(ActivityFlags.NewTask);
                    StartActivity(intentS);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// The OnStop
        ///// </summary>
        //protected override void OnStop()
        //{
        //try
        //{
        //    SystemSettings tempSystemSettings = SysEnvironment.GetInstance().SysSettings;
        //    SystemSettingsTasks.SaveData(ref tempSystemSettings);
        //    SysEnvironment.GetInstance().ContactLog.SubmitContactLogOut();
        //}
        //catch (Exception ex)
        //{
        //    Log.Error(nameof(MainActivity), ex.Message);
        //}

        //base.OnStop();
        //}

        /// <summary>
        /// The OnKeyUp
        /// </summary>
        /// <param name="keyCode">The keyCode<see cref="Keycode"/></param>
        /// <param name="e">The e<see cref="KeyEvent"/></param>
        /// <returns>The <see cref="bool"/></returns>
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
    }
}
