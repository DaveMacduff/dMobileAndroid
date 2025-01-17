// <copyright file="CustomButtonRenderer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.Content;
using dMobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Button = Xamarin.Forms.Button;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]

namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="CustomButtonRenderer" />.
    /// </summary>
    public class CustomButtonRenderer : ButtonRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomButtonRenderer"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public CustomButtonRenderer(Context context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            this.Control?.SetPadding(10, 0, 10, 0);
        }
    }
}