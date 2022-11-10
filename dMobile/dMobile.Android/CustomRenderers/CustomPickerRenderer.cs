// <copyright file="CustomPickerRenderer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using dMobile.Droid.CustomRenderers;
using dMobile.View.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using PickerRenderer = Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="CustomPickerRenderer" />.
    /// </summary>
    public class CustomPickerRenderer : PickerRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPicker"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/>.</param>
        public CustomPickerRenderer(Context context) : base(context) { }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (this.Control != null && this.Element != null)
            {
                this.Control.TextChanged += this.Control_TextChanged;
                this.Control.SetTextIsSelectable(true);
                this.Control.Focusable = true;
                this.Control.FocusableInTouchMode = true;
                this.Control.SetCursorVisible(true);
                this.Control.Background = this.AddPickerStyles(!string.IsNullOrEmpty((this.Element as CustomPicker).Image) ? (this.Element as CustomPicker).Image : "arrow_drop_down");
            }
        }

        private void Control_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (this.Control != null && this.Element != null)
            {
                (this.Element as CustomPicker).Text = this.Control.Text;
            }
        }

        public LayerDrawable AddPickerStyles(string imagePath)
        {
            Drawable[] layers = { this.Control.Background, this.GetDrawable(imagePath) };
            var layerDrawable = new LayerDrawable(layers);
            layerDrawable.SetLayerInset(0, 0, 0, 0, 0);

            return layerDrawable;
        }

        private BitmapDrawable GetDrawable(string imagePath)
        {
            var resID = this.Resources.GetIdentifier(imagePath, "drawable", this.Context.PackageName);
            var drawable = ContextCompat.GetDrawable(this.Context, resID);
            var bitmap = ((BitmapDrawable)drawable).Bitmap;

            var result = new BitmapDrawable(this.Resources, Bitmap.CreateScaledBitmap(bitmap, 20, 20, true))
            {
                Gravity = GravityFlags.Right,
            };

            return result;
        }
    }
}
