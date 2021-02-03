namespace dMobile.Droid
{
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Android.Util;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View;
    using System;
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
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        ///// <summary>
        ///// The OnStop
        ///// </summary>
        protected override void OnStop()
        {
            base.OnStop();

            try
            {
                SystemSettings tempSystemSettings = SysEnvironment.GetInstance().SysSettings;
                SystemSettingsTasks.SaveData(ref tempSystemSettings);
                SysEnvironment.GetInstance().ContactLog.SubmitContactLogOut();
            }
            catch (Exception ex)
            {
                Log.Error(nameof(MainActivity), ex.Message);
            }
        }

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
