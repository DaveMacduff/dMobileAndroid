// <copyright file="CustomButtonRenderer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

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
using Button = Xamarin.Forms.Button;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomButtonRenderer"/> class.
        /// </summary>
        /// <param name="context"></param>
        public CustomButtonRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (this.Control != null)
            {
                this.Control.SetPadding(10, 0, 10, 0);
            }
        }
    }
}