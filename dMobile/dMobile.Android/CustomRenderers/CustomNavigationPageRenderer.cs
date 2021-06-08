using Android.Content;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using dMobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
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
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            Element page = Element.Parent;
            MasterDetailPage masterDetailPage = null;
            
            while (page != null)
            {
                if (page is MasterDetailPage)
                {
                    masterDetailPage = page as MasterDetailPage;
                    break;
                }

                page = page.Parent;
            }

            if (masterDetailPage == null)
            {
                return;
            }

            var renderer = Platform.GetRenderer(masterDetailPage) as MasterDetailPageRenderer;
            
            if (renderer == null)
            {
                return;
            }

            var drawerLayout = (DrawerLayout)renderer;
            Support.Toolbar toolbar = this.FindViewById<Support.Toolbar>(Resource.Id.toolbar);

            toolbar?.SetNavigationOnClickListener(new MenuClickListener(Element, drawerLayout));
        }

        private class MenuClickListener : Java.Lang.Object, IOnClickListener
        {
            readonly NavigationPage navigationPage;
            private DrawerLayout layout;

            public MenuClickListener(NavigationPage navigationPage, DrawerLayout layout)
            {
                this.navigationPage = navigationPage;
                this.layout = layout;
            }

            public void OnClick(Android.Views.View v)
            {
                if (navigationPage.Navigation.NavigationStack.Count <= 1)
                {
                    layout.OpenDrawer((int)GravityFlags.Left);
                }
                else if (navigationPage.CurrentPage.Parent is NavigationPage page)
                {
                    page.SendBackButtonPressed();
                }
            }
        }
    }
}
