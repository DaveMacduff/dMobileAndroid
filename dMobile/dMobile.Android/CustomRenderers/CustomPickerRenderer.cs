using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using dMobile.Droid.CustomRenderers;
using dMobile.View.CustomControls;
using PickerRenderer = Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer;
using Android.Graphics;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;

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

		CustomPicker element;

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			this.element = this.Element as CustomPicker;

			if (Control != null && this.Element != null && !string.IsNullOrEmpty(element.Image))
			{
				Control.Background = AddPickerStyles(element.Image);
			}
		}

		public LayerDrawable AddPickerStyles(string imagePath)
		{
			Drawable[] layers = { Control.Background, GetDrawable(imagePath) };
			LayerDrawable layerDrawable = new LayerDrawable(layers);
			layerDrawable.SetLayerInset(0, 0, 0, 0, 0);

			return layerDrawable;
		}

		private BitmapDrawable GetDrawable(string imagePath)
		{
			int resID = Resources.GetIdentifier(imagePath, "drawable", this.Context.PackageName);
			var drawable = ContextCompat.GetDrawable(this.Context, resID);
			var bitmap = ((BitmapDrawable)drawable).Bitmap;

			var result = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap, 20, 20, true))
			{
				Gravity = GravityFlags.Right
			};

			return result;
		}
	}
}
