// <copyright file="HoneywellScannerService.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Android.Util;
using Android.Widget;
using Com.Honeywell.Aidc;
using dMobile.Droid.Services;
using dMobile.Resources.Services;
using Java.Interop;
using Xamarin.Forms;
using BarcodeReader = Com.Honeywell.Aidc.BarcodeReader;

[assembly: Dependency(typeof(HoneywellScannerService))]
namespace dMobile.Droid.Services
{
    public class HoneywellScannerService : IHoneywellScannerService
    {
        private Dictionary<string, object> barcodeReaderSettings = new Dictionary<string, object>();
        private event EventHandler OnProfileInfoRecieved;
        /// <inheritdoc/>
        public event EventHandler ProfileDataReady;
        public BarcodeReader BarcodeReader { get; set; }
        /// <inheritdoc/>
        public Dictionary<string, object> BarcodeReaderSettings { get => this.barcodeReaderSettings; set => this.barcodeReaderSettings = value; }
        private string packageProfileName = Android.App.Application.Context.PackageName;
        private string defaultProfileName = "DEFAULT";

        /// <inheritdoc/>
        public void GetScannerProfile()
        {
            try
            {
                this.OnProfileInfoRecieved += this.HoneywellScannerService_OnProfileInfoReceived;
                var callback = new Callback();
                callback.OnCreatedComplete += ScannerProfileRetrieved;
                AidcManager.Create(Android.App.Application.Context, callback);

                void ScannerProfileRetrieved(object sender, EventArgs e)
                {
                    this.BarcodeReader = callback.BarcodeReader;
                    this.OnProfileInfoRecieved?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetScannerProfile", ex.Message);
                Toast.MakeText(Android.App.Application.Context, "GetScannerProfile error " + ex.Message, ToastLength.Short);
            }
        }

        private void HoneywellScannerService_OnProfileInfoReceived(object sender, EventArgs e)
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
                                    Log.Debug("BARCODE READER PROPERTIES", "Invalid type in bar code reader properties. " + val != null ? val.GetType().ToString() : string.Empty);
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
                ////        else if (this.BarcodeReader.LoadProfile(this.defaultProfileName)) // "DEFAULT"
                ////        {
                ////            var test = this.BarcodeReader.ProfileNames;
                ////            foreach (var property in this.BarcodeReader.AllProperties)
                ////            {
                ////                var val = property.Value;

                //// if (val != null)
                ////                {
                ////                    try
                ////                    {
                ////                        if (val.GetType() == typeof(Java.Lang.Boolean))
                ////                        {
                ////                            this.BarcodeReaderSettings.TryAdd(property.Key, (bool)val);
                ////                        }
                ////                        else if (val.GetType() == typeof(Java.Lang.String))
                ////                        {
                ////                            this.BarcodeReaderSettings.TryAdd(property.Key, (string)val);
                ////                        }
                ////                        else if (val.GetType() == typeof(Java.Lang.Integer))
                ////                        {
                ////                            this.BarcodeReaderSettings.TryAdd(property.Key, (int)val);
                ////                        }
                ////                        else
                ////                        {
                ////                            Log.Debug("DEFAULT BARCODE READER PROPERTIES", "Invalid type in barcode reader properties. " + val != null ? val.GetType().ToString() : string.Empty);
                ////                        }
                ////                    }
                ////                    catch (Exception ex)
                ////                    {
                ////                        Toast.MakeText(Android.App.Application.Context, "Error loading default barcode reader profile. " + val != null ? val.ToString() : string.Empty + " " + ex.Message, ToastLength.Short);
                ////                    }
                ////                }
                ////            }
                ////        }
                ////    }
                this.ProfileDataReady?.Invoke(this, EventArgs.Empty);
            }
        }

        class Callback : Java.Lang.Object, AidcManager.ICreatedCallback
        {
            private AidcManager manager;
            public IntPtr Handle => throw new NotImplementedException();
            public int JniIdentityHashCode => throw new NotImplementedException();
            public JniObjectReference PeerReference => throw new NotImplementedException();
            public JniPeerMembers JniPeerMembers => throw new NotImplementedException();
            public JniManagedPeerStates JniManagedPeerState => throw new NotImplementedException();

            public BarcodeReader BarcodeReader { get; set; }
            public AidcManager Manager { get => this.manager; set => this.manager = value; }
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
                this.OnCreatedComplete?.Invoke(this, EventArgs.Empty);
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