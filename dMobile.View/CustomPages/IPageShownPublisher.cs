namespace dMobile.View.CustomPages
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IPageShownPublisher
    {
        void RegisterPageShownListener(IPageShownListener pageShownListener);

        void UnregisterPageShownListener(IPageShownListener pageShownListener);
    }
}
