using Android.Content;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using dMobile.Resources.Services;
using dMobile.Droid.Services;
using Android.Graphics;
using Android.Widget;
using Android.Util;

[assembly: Dependency(typeof(GridService))]
namespace dMobile.Droid.Services
{
    public class GridService : IGridService
    {
        public double CalculateTextWidth(string text)
        {
            Rect bounds = new Rect();
            TextView textView = new TextView(Forms.Context);
            textView.Paint.GetTextBounds(text, 0, text.Length, bounds);
            // Adding 10 to accommodate cases where last letter being cut off
            var length = bounds.Width() + 10;
            return length / Forms.Context.Resources.DisplayMetrics.ScaledDensity;
        }
    }
}