using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using dMobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ListView = Xamarin.Forms.ListView;

// Not using this class at the moment
//[assembly: ExportRenderer(typeof(ListView), typeof(CustomListViewRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        private Android.Views.View header;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomNavigationPageRenderer"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/></param>
        public CustomListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (this.Control != null)
            {
                this.Control.Scroll += this.ListViewScrolled;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (this.Element != null && this.Element.HeaderTemplate != null)
            {
                if (this.Control.GetChildAt(0) != null && header == null)
                {
                    this.header = this.Control.GetChildAt(0);
                    header.SetY(0);
                    header.SetZ(1);
                }
            }
        }

        private void ListViewScrolled(object sender, AbsListView.ScrollEventArgs e)
        {
            if (this.header != null)
            {
                this.header.SetY(0);

                Console.WriteLine("Header X " + this.header.GetX());
                Console.WriteLine("Header Y " + this.header.GetY());
                Console.WriteLine("Header Z " + this.header.GetZ());

                if (this.Control.GetChildAt(0) != null && this.Control.GetChildAt(0) != this.header)
                {
                    Console.WriteLine(this.Control + " " + this.Element);
                    Console.WriteLine("Header's gone!" + this.header);
                }
            }
        }
    }
}