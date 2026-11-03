// <copyright file="GridService.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using Android.Widget;
using dMobile.Droid.Services;
using dMobile.Resources.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(GridService))]

namespace dMobile.Droid.Services
{
    /// <summary>
    /// Defines the <see cref="GridService" />.
    /// </summary>
    public class GridService : IGridService
    {
        /// <summary>
        /// Cached <see cref="Android.Graphics.Paint"/> instance. Avoids allocating a new
        /// <see cref="TextView"/> on every call to <see cref="CalculateTextWidth(string)"/>.
        /// </summary>
        private readonly Android.Graphics.Paint paint = new TextView(Forms.Context).Paint;

        /// <summary>
        /// Cache of previously calculated text widths, keyed by the text measured.
        /// Many grid columns re-measure the same values (headers, repeated dates, etc.).
        /// </summary>
        private readonly ConcurrentDictionary<string, double> widthCache = new ConcurrentDictionary<string, double>();

        /// <inheritdoc/>
        public double CalculateTextWidth(string text)
        {
            text ??= string.Empty;

            if (this.widthCache.TryGetValue(text, out var cachedWidth))
            {
                return cachedWidth;
            }

            var bounds = new Android.Graphics.Rect();
            this.paint.GetTextBounds(text, 0, text.Length, bounds);

            // Adding 10 to accommodate cases where last letter being cut off
            var length = bounds.Width() + 10;
            var width = length / Forms.Context.Resources.DisplayMetrics.ScaledDensity;

            this.widthCache[text] = width;
            return width;
        }
    }
}