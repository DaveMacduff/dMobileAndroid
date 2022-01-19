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
            if (IsUpdateRequired())
            {
                DownloadAndInstallUpdate();
            }

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            

            // THIS SECTION WORKS FOR HITTING API ENDPOINTS
            //Uri uri = new Uri(string.Format(@"http://184.70.233.14:81/current/com.dmobile.android.apk", string.Empty));
            ////Uri uri = new Uri(string.Format(@"https://www.google.ca", string.Empty));

            //HttpClient client = new HttpClient();
            //HttpResponseMessage response = client.GetAsync(uri).GetAwaiter().GetResult();
            //if (response.IsSuccessStatusCode)
            //{
            //    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //    var tempo = JsonConvert.DeserializeObject(content, typeof(string));
            //}
        }
        
        public bool IsUpdateRequired()
        {
            PackageInfo pinfo = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0);
            string vnum = pinfo.VersionName;
            Uri uri = new Uri(string.Format("http://184.70.233.14:81/current/IsUpdateRequired?={0}", vnum));
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(uri).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error making Http Request");
                return false;
            }
            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var tempo = JsonConvert.DeserializeObject(content, typeof(string));
            bool result = Convert.ToBoolean(tempo.ToString());
            return result;
        }

        public bool DownloadAndInstallUpdate()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.WriteExternalStorage) || (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage)))
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 0);
            }
            try
            {
                var clients = new WebClient();
                clients.DownloadFile("http://184.70.233.14:81/current/com.dmobile.android.apk", ApplicationContext.GetExternalFilesDir(null).AbsolutePath + "/com.dmobile.android.update.apk");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            try
            {
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    Java.IO.File directory = ApplicationContext.GetExternalFilesDir(null);
                    Java.IO.File file = new Java.IO.File(directory, "com.dmobile.android.update.apk");
                    Android.Net.Uri fileuri = FileProvider.GetUriForFile(ApplicationContext, ApplicationContext.PackageName + ".authorityStr", file);
                    Intent intentS = new Intent(Intent.ActionInstallPackage);
                    intentS.SetData(fileuri);
                    intentS.SetFlags(ActivityFlags.GrantReadUriPermission);
                    StartActivity(intentS);
                }
                else
                {
                    Java.IO.File directory = ApplicationContext.GetExternalFilesDir(null);
                    Java.IO.File file = new Java.IO.File(directory, "com.dmobile.android.update.apk");
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
