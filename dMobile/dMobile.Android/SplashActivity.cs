namespace dMobile.Droid
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Support.V7.App;
    using System.Threading.Tasks;

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

        /// <summary>
        /// The OnBackPressed
        /// </summary>
        public override void OnBackPressed()
        {
        }
    }
}
