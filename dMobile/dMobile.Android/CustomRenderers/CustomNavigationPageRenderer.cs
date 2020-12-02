using Android.Content;
using Android.Widget;
using dMobile.Droid.CustomRenderers;
using Xamarin.Forms;
using NavigationRenderer = Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer;
using Support = Android.Support.V7.Widget;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationPageRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="CustomNavigationPageRenderer" />
    /// </summary>
    public class CustomNavigationPageRenderer : NavigationRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomNavigationPageRenderer"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/></param>
        public CustomNavigationPageRenderer(Context context) : base(context) { }

        ///// <summary>
        ///// The OnLayout
        ///// </summary>
        ///// <param name="changed">The changed<see cref="bool"/></param>
        ///// <param name="l">The l<see cref="int"/></param>
        ///// <param name="t">The t<see cref="int"/></param>
        ///// <param name="r">The r<see cref="int"/></param>
        ///// <param name="b">The b<see cref="int"/></param>
        //protected override void OnLayout(bool changed, int l, int t, int r, int b)
        //{
        //    base.OnLayout(changed, l, t, r, b);

        //    var toolbar = FindViewById<Support.Toolbar>(Resource.Id.toolbar);

        //    if (toolbar == null) return;

        //    for (var index = 0; index < toolbar.ChildCount; index++)
        //    {                
        //        if (toolbar.GetChildAt(index) is TextView title)
        //        {
        //            float toolbarCenter = toolbar.Width / 2;
        //            float titleCenter = title.Width / 2;
        //            title.SetX(toolbarCenter - titleCenter);
        //        }
        //    }
        //}
    }
}
