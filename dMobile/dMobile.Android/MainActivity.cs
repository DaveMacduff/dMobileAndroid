namespace dMobile.Droid
{
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Android.Views;
    using dMobile.Resources.Enums;
    using dMobile.View;
    using System;

    /// <summary>
    /// Defines the <see cref="MainActivity" />
    /// </summary>
    [Activity(Label = "dMobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
                Xamarin.Forms.MessagingCenter.Send(keyCode.ToString(), EventTypes.OnKeyUp.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return base.OnKeyUp(keyCode, e);
        }
    }
}
