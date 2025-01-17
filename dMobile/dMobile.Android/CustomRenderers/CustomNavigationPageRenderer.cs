// <copyright file="CustomNavigationPageRenderer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.Content;
using Android.Support.V4.Widget;
using Android.Views;
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
    /// Defines the <see cref="CustomNavigationPageRenderer" />.
    /// </summary>
    public class CustomNavigationPageRenderer : NavigationRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomNavigationPageRenderer"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/>.</param>
        public CustomNavigationPageRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            var page = this.Element.Parent;
            MasterDetailPage masterDetailPage = null;

            while (page != null)
            {
                if (page is MasterDetailPage detailPage)
                {
                    masterDetailPage = detailPage;
                    break;
                }

                page = page.Parent;
            }

            if (masterDetailPage == null)
            {
                return;
            }

            if (!(Platform.GetRenderer(masterDetailPage) is MasterDetailPageRenderer renderer))
            {
                return;
            }

            var drawerLayout = (DrawerLayout)renderer;
            var toolbar = this.FindViewById<Support.Toolbar>(Resource.Id.toolbar);

            toolbar?.SetNavigationOnClickListener(new MenuClickListener(this.Element, drawerLayout));
        }

        private class MenuClickListener : Java.Lang.Object, IOnClickListener
        {
            private readonly NavigationPage navigationPage;
            private readonly DrawerLayout layout;

            public MenuClickListener(NavigationPage navigationPage, DrawerLayout layout)
            {
                this.navigationPage = navigationPage;
                this.layout = layout;
            }

            public void OnClick(Android.Views.View v)
            {
                if (this.navigationPage.Navigation.NavigationStack.Count <= 1)
                {
                    this.layout.OpenDrawer((int)GravityFlags.Left);
                }
                else if (this.navigationPage.CurrentPage.Parent is NavigationPage page)
                {
                    page.SendBackButtonPressed();
                }
            }
        }
    }
}
