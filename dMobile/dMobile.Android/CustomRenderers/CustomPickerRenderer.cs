using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using dMobile.Droid.CustomRenderers;
using dMobile.View.CustomControls;
using PickerRenderer = Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer;
using Android.Graphics;
using Android.Views;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="CustomPickerRenderer" />
    /// </summary>
    public class CustomPickerRenderer : PickerRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPicker"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/></param>
        public CustomPickerRenderer(Context context) : base(context) { }

        /// <summary>
        /// The OnElementChanged
        /// </summary>
        /// <param name="e">The e<see cref="ElementChangedEventArgs{Picker}"/></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Focusable = false;

                // TODO: Center text?
                //Control.Gravity = GravityFlags.CenterHorizontal;
            }
        }
    }
}
