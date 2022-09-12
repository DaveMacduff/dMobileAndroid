// <copyright file="GridService.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using dMobile.Droid.Services;
using dMobile.Resources.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(GridService))]
namespace dMobile.Droid.Services
{
    public class GridService : IGridService
    {
        public double CalculateTextWidth(string text)
        {
            var bounds = new Android.Graphics.Rect();
            var textView = new TextView(Forms.Context);
            textView.Paint.GetTextBounds(text, 0, text.Length, bounds);
            // Adding 10 to accommodate cases where last letter being cut off
            var length = bounds.Width() + 10;
            return length / Forms.Context.Resources.DisplayMetrics.ScaledDensity;
        }
    }
}