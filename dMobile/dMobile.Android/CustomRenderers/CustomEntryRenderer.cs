using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using dMobile.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="CustomEntryRenderer" />
    /// </summary>
    public class CustomEntryRenderer : EntryRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEntryRenderer"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/></param>
        public CustomEntryRenderer(Context context) : base(context) { }

        /// <summary>
        /// The OnElementChanged
        /// </summary>
        /// <param name="e">The e<see cref="ElementChangedEventArgs{Entry}"/></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            Control?.SetSelectAllOnFocus(true);
        }
    }
}
