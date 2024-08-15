namespace dMobile.View.CustomPages
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IPageShownListener
    {
        void OnPageShown(object sender, PageShownEventArgs e);
    }
}
