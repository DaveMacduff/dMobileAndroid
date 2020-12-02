using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using dMobile.Droid.CustomRenderers;
using dMobile.View.CustomControls;
using System;
using Switch = Xamarin.Forms.Switch;

[assembly: ExportRenderer(typeof(CustomSwitch), typeof(CustomSwitchRenderer))]
namespace dMobile.Droid.CustomRenderers
{
    /// <summary>
    /// Defines the <see cref="CustomSwitchRenderer" />
    /// </summary>
    public class CustomSwitchRenderer : SwitchRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSwitchRenderer"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="Context"/></param>
        public CustomSwitchRenderer(Context context) : base(context)
        {
        }

        /// <summary>
        /// The OnElementChanged
        /// </summary>
        /// <param name="e">The e<see cref="ElementChangedEventArgs{Switch}"/></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Click += Control_Click;
            }
        }

        /// <summary>
        /// The Control_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Control_Click(object sender, EventArgs e)
        {
            CustomSwitch customSwitch = (CustomSwitch)this.Element;
            customSwitch.Click?.Invoke(this, new EventArgs());
        }
    }
}
