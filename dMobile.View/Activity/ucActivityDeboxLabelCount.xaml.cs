using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using dMobile.Bedrock;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.CustomPages.Modal;
using System.Threading.Tasks;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityDeboxLabelCount : DMobileModal, IAppArea
    {
        int quantity;

        public ucActivityDeboxLabelCount()
        {
            this.InitializeComponent();

            this.Title = "Debox Label Count";
            this.labelQuantity.Text = "Quantity";
        }

        public int Quantity
        {
            get
            {
                return quantity;
            }
            private set
            {
                quantity = value;
            }
        }

        public async Task ScannerDisable()
        {
        }

        public async Task ScannerEnable()
        {
        }

        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        public string GetControlDescription()
        {
            return "Debox Label Count";
        }

        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            if (await IsReadyToSave())
            {
                Quantity = Convert.ToInt32(this.entryQuantity.Text);
                saveComplete = true;
            }

            return saveComplete;
        }

        public async Task<bool> CancelChanges()
        {
            return await IsReadyToClose();
        }

        public async Task<bool> IsReadyToSave()
        {
            return true;
        }

        public async Task<XDocument> GetXML()
        {
            XDocument result = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<quantity>"
                    + Quantity
                    + "</quantity>");
            return result;
        }
    }
}