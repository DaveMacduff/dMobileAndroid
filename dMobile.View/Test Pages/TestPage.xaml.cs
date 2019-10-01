using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View
{
	public partial class TestPage : ContentPage
	{
		public TestPage ()
		{
			InitializeComponent ();
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Test Title", "Test Message", "YES", "NO").ContinueWith(t =>
            {
                if (t.Result)
                {
                    Console.WriteLine(t.Result);
                } else
                {
                    Console.WriteLine("No result");
                }
            });
        }
    }
}