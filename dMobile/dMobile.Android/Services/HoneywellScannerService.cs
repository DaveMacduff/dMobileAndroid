using System;
using System.Collections.Generic;
using dMobile.Resources.Services;
using Com.Honeywell.Aidc;
using BarcodeReader = Com.Honeywell.Aidc.BarcodeReader;
using Java.Interop;
using Xamarin.Forms;
using dMobile.Droid.Services;
using Android.Widget;
using Android.Util;

[assembly: Dependency(typeof(HoneywellScannerService))]
namespace dMobile.Droid.Services
{
    public class HoneywellScannerService : IHoneywellScannerService
    {
        private BarcodeReader barcodeReader;
        private Dictionary<string, object> barcodeReaderSettings = new Dictionary<string, object>();
        private event EventHandler OnProfileInfoRecieved;
        public event EventHandler ProfileDataReady;
        public BarcodeReader BarcodeReader { get => barcodeReader; set => barcodeReader = value; }
        public Dictionary<string, object> BarcodeReaderSettings { get => this.barcodeReaderSettings; set => this.barcodeReaderSettings = value; }
        private string packageProfileName = Android.App.Application.Context.PackageName;
        private string defaultProfileName = "DEFAULT";

        public void GetScannerProfile()
        {
            try
            {
                this.OnProfileInfoRecieved += HoneywellScannerService_OnProfileInfoRecieved;
                Callback callback = new Callback();
                callback.OnCreatedComplete += ScannerProfileRetrieved;
                AidcManager.Create(Android.App.Application.Context, callback);

                void ScannerProfileRetrieved(object sender, EventArgs e)
                {
                    this.BarcodeReader = callback.BarcodeReader;
                    this.OnProfileInfoRecieved?.Invoke(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetScannerProfile", ex.Message);
                Toast.MakeText(Android.App.Application.Context, "GetScannerProfile error " + ex.Message, ToastLength.Short);
            }
        }

        private void HoneywellScannerService_OnProfileInfoRecieved(object sender, EventArgs e)
        {
            if (this.BarcodeReader != null)
            {
                if (this.BarcodeReader.LoadProfile(this.packageProfileName))
                {
                    foreach (var property in this.BarcodeReader.AllProperties)
                    {
                        var val = property.Value;

                        if (val != null)
                        {
                            try
                            {
                                if (val.GetType() == typeof(Java.Lang.Boolean))
                                {
                                    this.BarcodeReaderSettings.TryAdd(property.Key, (bool)val);
                                }
                                else if (val.GetType() == typeof(Java.Lang.String))
                                {
                                    this.BarcodeReaderSettings.TryAdd(property.Key, (string)val);
                                }
                                else if (val.GetType() == typeof(Java.Lang.Integer))
                                {
                                    this.BarcodeReaderSettings.TryAdd(property.Key, (int)val);
                                }
                                else
                                {
                                    Log.Debug("BARCODE READER PROPERTIES", "Invalid type in barcode reader properties. " + val != null ? val.GetType().ToString() : string.Empty);
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(Android.App.Application.Context, "Error loading " + Android.App.Application.Context.PackageName + " barcode reader profile. " + val != null ? val.ToString() : string.Empty + " " + ex.Message, ToastLength.Short);
                            }
                        }
                    }
                }

                // TODO: Loading the DEFAULT profile breaks the barcodeReader.SetAsync for some reason. For now we will have to create a scanner profile for the application
                //        else if (this.BarcodeReader.LoadProfile(this.defaultProfileName)) // "DEFAULT"
                //        {
                //            var test = this.BarcodeReader.ProfileNames;
                //            foreach (var property in this.BarcodeReader.AllProperties)
                //            {
                //                var val = property.Value;

                //                if (val != null)
                //                {
                //                    try
                //                    {
                //                        if (val.GetType() == typeof(Java.Lang.Boolean))
                //                        {
                //                            this.BarcodeReaderSettings.TryAdd(property.Key, (bool)val);
                //                        }
                //                        else if (val.GetType() == typeof(Java.Lang.String))
                //                        {
                //                            this.BarcodeReaderSettings.TryAdd(property.Key, (string)val);
                //                        }
                //                        else if (val.GetType() == typeof(Java.Lang.Integer))
                //                        {
                //                            this.BarcodeReaderSettings.TryAdd(property.Key, (int)val);
                //                        }
                //                        else
                //                        {
                //                            Log.Debug("DEFAULT BARCODE READER PROPERTIES", "Invalid type in barcode reader properties. " + val != null ? val.GetType().ToString() : string.Empty);
                //                        }
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        Toast.MakeText(Android.App.Application.Context, "Error loading default barcode reader profile. " + val != null ? val.ToString() : string.Empty + " " + ex.Message, ToastLength.Short);
                //                    }
                //                }
                //            }
                //        }
                //    }

                this.ProfileDataReady?.Invoke(this, new EventArgs());
            }
        }

        class Callback : Java.Lang.Object, AidcManager.ICreatedCallback
        {
            private BarcodeReader barcodeReader;
            private AidcManager manager;
            public IntPtr Handle => throw new NotImplementedException();
            public int JniIdentityHashCode => throw new NotImplementedException();
            public JniObjectReference PeerReference => throw new NotImplementedException();
            public JniPeerMembers JniPeerMembers => throw new NotImplementedException();
            public JniManagedPeerStates JniManagedPeerState => throw new NotImplementedException();

            public BarcodeReader BarcodeReader { get => barcodeReader; set => barcodeReader = value; }
            public AidcManager Manager { get => manager; set => manager = value; }
            public EventHandler OnCreatedComplete;


            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void Disposed()
            {
                throw new NotImplementedException();
            }

            public void DisposeUnlessReferenced()
            {
                throw new NotImplementedException();
            }

            public void Finalized()
            {
                throw new NotImplementedException();
            }

            public void OnCreated(AidcManager p0)
            {
                this.Manager = p0;
                this.BarcodeReader = this.Manager.CreateBarcodeReader();
                this.OnCreatedComplete?.Invoke(this, new EventArgs());
            }

            public void SetJniIdentityHashCode(int value)
            {
                throw new NotImplementedException();
            }

            public void SetJniManagedPeerState(JniManagedPeerStates value)
            {
                throw new NotImplementedException();
            }

            public void SetPeerReference(JniObjectReference reference)
            {
                throw new NotImplementedException();
            }

            public void UnregisterFromRuntime()
            {
                throw new NotImplementedException();
            }
        }
    }
}