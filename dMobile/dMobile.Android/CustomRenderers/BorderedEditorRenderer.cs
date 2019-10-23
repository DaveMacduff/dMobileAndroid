using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using dMobile.Droid.CustomRenderers;
using dMobile.View.CustomControls;

[assembly: ExportRenderer(typeof(BorderedEditor), typeof(BorderedEditorRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="BorderedEditorRenderer" />
    /// </summary>
    public class BorderedEditorRenderer : EditorRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderedEditorRenderer"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/></param>
        public BorderedEditorRenderer(Context context) : base(context) { }

        /// <summary>
        /// The OnElementChanged
        /// </summary>
        /// <param name="e">The e<see cref="ElementChangedEventArgs{Editor}"/></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            { 
                Control.Background = Context.GetDrawable(Resource.Drawable.BorderedEditor);
                Control.SetTextIsSelectable(true);
            }
        }
    }
}
