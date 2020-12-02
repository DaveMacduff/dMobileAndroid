using System;
using Xamarin.Forms;
using Switch = Xamarin.Forms.Switch;

namespace dMobile.View.CustomControls
{
    /// <summary>
    /// Defines the <see cref="CustomSwitch" />
    /// </summary>
    public class CustomSwitch : Switch
    {
        public EventHandler Click;

        public event EventHandler Clicked
        {
            add
            {
                lock (this)
                {
                    Click += value;
                }
            }
            remove
            {
                lock (this)
                {
                    Click -= value;
                }
            }
        }
    }
}
