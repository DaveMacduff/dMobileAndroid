// <copyright file="BorderedEditorRenderer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.Content;
using dMobile.Droid.CustomRenderers;
using dMobile.View.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderedEditor), typeof(BorderedEditorRenderer))]

namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="BorderedEditorRenderer" />.
    /// </summary>
    public class BorderedEditorRenderer : EditorRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderedEditorRenderer"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/>.</param>
        public BorderedEditorRenderer(Context context) : base(context)
        {
        }

        /// <summary>
        /// The OnElementChanged.
        /// </summary>
        /// <param name="e">The e<see cref="ElementChangedEventArgs{Editor}"/>.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (this.Control != null)
            {
                this.Control.Background = this.Context!.GetDrawable(Resource.Drawable.BorderedEditor);
                this.Control.SetTextIsSelectable(true);
            }
        }
    }
}
