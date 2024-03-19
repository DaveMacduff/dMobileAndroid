// <copyright file="ContactLogin.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using SQLite;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="ContactLogin" />.
    /// </summary>
    [Serializable]
    public class ContactLogin : IContactLoginEventPublisher, ISysSocketEventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactLogin"/> class.
        /// </summary>
        public ContactLogin()
        {
            this.CommSetup = this.CommSetup.LoadData();
            this.LoginSocket.BasePort = this.CommSetup.BaseSocketPort;
            this.LoginSocket.DefSocketPort = this.CommSetup.DefaultSocketPort;
            this.LoginSocket.MaxPorts = this.CommSetup.MaxSocketPorts;
            this.LoginSocket.ConnTotalRetries = this.CommSetup.ConnectionAttempts;
            this.LoginSocket.SendTotalRetries = this.CommSetup.SendAttempts;
            this.LoginSocket.RecvTotalRetries = this.CommSetup.ReceiveAttempts;
            this.LoginSocket.SendTimeOutMilliseconds = this.CommSetup.SendTimeOutSeconds * 1000;
            this.LoginSocket.ReceiveTimeOutMilliseconds = this.CommSetup.ReceiveTimeOutSeconds * 1000;
            this.LoginSocket.RecvBuffSize = this.CommSetup.ReceiveBufferSize;
            this.LoginSocket.ServerIP = this.CommSetup.HostIpAddressComponent1.ToString() + "." +
                    this.CommSetup.HostIpAddressComponent2.ToString() + "." +
                    this.CommSetup.HostIpAddressComponent3.ToString() + "." +
                    this.CommSetup.HostIpAddressComponent4.ToString();
        }

        /// <summary>
        /// Define the on contact login event publisher.
        /// </summary>
        /// <param name="publisher"><see cref="IContactLoginEventPublisher"/> Contact login event publisher.</param>
        /// <param name="eventArgs"><see cref="ContactLoginEventArgs"/> Event arguments.</param>
        private delegate void OnContactLoginStatusOutEventHandler(
            IContactLoginEventPublisher publisher,
            ContactLoginEventArgs eventArgs);

        /// <summary>
        /// Define the on contact login status out event.
        /// </summary>
        private event OnContactLoginStatusOutEventHandler OnContactLoginStatusOut;

        /// <summary>
        /// Gets or sets the contact login id.
        /// </summary>
        public int LoginID { get; set; } = 0;

        /// <summary>
        /// Gets or sets the contact login password.
        /// </summary>
        public string LoginPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the warehouse location map reference number.
        /// </summary>
        public int WarehouseLocationMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the warehouse location map warehouse foreign key reference number.
        /// </summary>
        public int WarehouseLocationMapFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether IsLoggedIn.
        /// </summary>
        public bool IsLoggedIn { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether is allowed to associate product bar codes.
        /// </summary>
        public bool IsAllowedAssociateProductBarCode { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether is allowed picking module override picker.
        /// </summary>
        public bool IsAllowedPickingModuleOverridePicker { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether is allowed picking module override add manual detail.
        /// </summary>
        public bool IsAllowedPickingModuleOverrideAddManualDetail { get; set; } = false;

        /// <summary>
        /// Gets or sets the login socket.
        /// </summary>
        private SysSocket LoginSocket { get; set; } = new SysSocket();

        /// <summary>
        /// Gets or sets the communication setup.
        /// </summary>
        private CommunicationSetup CommSetup { get; set; } = new CommunicationSetup();

        /// <summary>
        /// Gets or sets the contact login information file name.
        /// </summary>
        private string FileName { get; set; } = "dMobileLogin.xml";

        /// <summary>
        /// Load the contact login settings file.
        /// </summary>
        /// <returns><see cref="ContactLogin"/> .</returns>
        public ContactLogin LoadData()
        {
            var serializer = new XmlSerializer(typeof(ContactLogin));
            var contactLogin = new ContactLogin();

            // use reflection to get the current .≥assembly path for the database
            var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var fullPathToMyFile = Path.Combine(appDir, this.FileName);

            if (!File.Exists(fullPathToMyFile))
            {
                _ = this.SaveData(ref contactLogin);
            }

            // Create a FileStream to read with.
            using (var reader = new FileStream(fullPathToMyFile, FileMode.Open))
            {
                // Deserialize the stream to the object, and close the TextReader
                contactLogin = (ContactLogin)serializer.Deserialize(reader);
                reader.Close();
            }

            return contactLogin;
        }

        /// <summary>
        /// Save the contact login settings file.
        /// </summary>
        /// <param name="contactLogin"><see cref="ContactLogin"/> Contact login instance to serialize.</param>
        /// <returns><see cref="bool"/> True if save successful, else false.</returns>
        public bool SaveData(ref ContactLogin contactLogin)
        {
            var serializer = new XmlSerializer(typeof(ContactLogin));
            bool saveComplete;
            try
            {
                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var fullPathToMyFile = Path.Combine(appDir, this.FileName);

                // Create a FileStream to write with.
                using (var writer = new FileStream(fullPathToMyFile, FileMode.Create))
                {
                    // Serialize the object, and close the TextWriter
                    serializer.Serialize(writer, contactLogin);
                    writer.Close();
                }

                saveComplete = true;
            }
            catch
            {
                saveComplete = false;
            }

            return saveComplete;
        }

        /// <summary>
        /// Submit the contact login.
        /// </summary>
        public void SubmitContactLogIn()
        {
            var responseData = this.SubmitContactLogInCommunication();
            this.SubmitContactLogInData(responseData);
        }

        /// <summary>
        /// Submit contact logout.
        /// </summary>
        public void SubmitContactLogOut()
        {
            var responseData = this.SubmitContactLogOutCommunication();
            this.SubmitContactLogOutData(responseData);
        }

        /// <summary>
        /// Register the contact login status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IContactLoginEventListener"/> Contact login listener.</param>
        public void RegisterContactLoginListenerStatusOut(IContactLoginEventListener listener)
        {
            this.OnContactLoginStatusOut += new OnContactLoginStatusOutEventHandler(listener.OnContactLoginStatusOut);
        }

        /// <summary>
        /// Unregister contact login status out listener.
        /// </summary>
        /// <param name="contactLoginEventListener"><see cref="IContactLoginEventListener"/>.</param>
        public void UnregisterContactLoginListenerStatusOut(IContactLoginEventListener contactLoginEventListener)
        {
            this.OnContactLoginStatusOut -= new OnContactLoginStatusOutEventHandler(
                contactLoginEventListener.OnContactLoginStatusOut);
        }

        /// <summary>
        /// Raise the contact login status out event.
        /// </summary>
        /// <param name="publisher"><see cref="ISysSocketEventPublisher"/> Event publisher.</param>
        /// <param name="eventArgs"><see cref="SysSocketEventArgs"/> Event arguments.</param>
        public void OnSysSocketStatusOut(
            ISysSocketEventPublisher publisher,
            SysSocketEventArgs eventArgs)
        {
            var args = new ContactLoginEventArgs(eventArgs.StatusOutMsg);
            this.OnContactLoginStatusOut(this, args);
        }

        /// <summary>
        /// Submit contact log in communication.
        /// </summary>
        /// <returns><see cref="string"/> Submission result.</returns>
        private string SubmitContactLogInCommunication()
        {
            string sendSubmit = null;
            Socket socket = null;
            var bytesSent = 0;
            List<byte> bytRecvData = null;
            var responseData = string.Empty;
            var msgEventArg = new ContactLoginEventArgs();

            var sysSocket = new SysSocket();
            sysSocket.RegSockListenerStatusOut(this);

            try
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Creating request for contact log in";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                sendSubmit = this.CreateXMLSubmitContactLogIn();

                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Attempting to connect";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                socket = this.LoginSocket.Connect();

                if (socket != null)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Sending request";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytesSent = this.LoginSocket.SendRequest(socket, sendSubmit);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Waiting for response";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytRecvData = this.LoginSocket.ReceiveResponse(socket);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Response received";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    socket.Disconnect(true);
                    socket.Close();

                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Decompressing";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    responseData = Compress.UnZip(bytRecvData.ToArray());
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Failed to connect";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    throw new Exception("Failed to connect");
                }
            }
            catch (SocketException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Socket exception occurred";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Request for contact log in failed";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Request for contact log in failed";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            finally
            {
                sysSocket.UnregSockListenerStatusOut(this);
            }

            return responseData;
        }

        /// <summary>
        /// Submit contact log out communication.
        /// </summary>
        /// <returns><see cref="string"/> Submission result.</returns>
        private string SubmitContactLogOutCommunication()
        {
            string sendSubmit = null;
            var socket = default(Socket);
            var bytesSent = default(int);
            List<byte> bytRecvData = null;
            var responseData = string.Empty;
            var msgEventArg = new ContactLoginEventArgs();
            var sysSocket = new SysSocket();

            sysSocket.RegSockListenerStatusOut(this);

            try
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Creating request for contact log out";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                sendSubmit = this.CreateXMLSubmitContactLogOut();

                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Attempting to connect";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                socket = this.LoginSocket.Connect();

                if (socket != null)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Sending request";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytesSent = this.LoginSocket.SendRequest(socket, sendSubmit);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Waiting for response";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytRecvData = this.LoginSocket.ReceiveResponse(socket);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Response received";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    socket.Disconnect(true);
                    socket.Close();

                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Decompressing";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    responseData = Compress.UnZip(bytRecvData.ToArray());
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Failed to connect";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    throw new Exception("Failed to connect");
                }
            }
            catch (SocketException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Socket exception occurred";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Request for contact log out failed";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Request for contact log out failed";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            finally
            {
                sysSocket.UnregSockListenerStatusOut(this);
            }

            return responseData;
        }

        /// <summary>
        /// Submit contact log in communication.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Submission request data.</param>
        private void SubmitContactLogInData(string responseData)
        {
            var msgEventArg = new ContactLoginEventArgs();

            var sysSocket = new SysSocket();
            sysSocket.RegSockListenerStatusOut(this);

            try
            {
                if (responseData.Length > 0)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Parsing received data";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    // check if result is an error message
                    if (responseData.Contains("<message>ERROR</message>"))
                    {
                        var docError = new XmlDocument();
                        docError.LoadXml(responseData);

                        Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert(
                            "Login Error",
                            "Submit contact login error" + Environment.NewLine + docError.DocumentElement["detail"].InnerText,
                            "OK"));
                        docError = null;
                    }
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Received nothing to process";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Database exception occurred";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Submit contact log in failed.";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Submit contact log in failed.";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            finally
            {
                sysSocket.RegSockListenerStatusOut(this);
            }
        }

        /// <summary>
        /// Submit contact log out data.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Submission request data.</param>
        private void SubmitContactLogOutData(string responseData)
        {
            var msgEventArg = new ContactLoginEventArgs();

            var sockPublisher = new SysSocket();
            sockPublisher.RegSockListenerStatusOut(this);

            try
            {
                if (responseData.Length > 0)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Parsing received data";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    // check if result is an error message
                    if (responseData.Contains("<message>ERROR</message>"))
                    {
                        var docError = new XmlDocument();
                        docError.LoadXml(responseData);
                        var errorDetail = docError.DocumentElement["detail"].InnerText;

                        Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert(
                            "Logout Error",
                            "Submit contact logout error" + Environment.NewLine + errorDetail,
                            "OK"));
                        docError = null;
                    }
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = "Received nothing to process";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Database exception occurred";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Submit contact log out failed.";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Submit contact log out failed.";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            finally
            {
                sockPublisher.UnregSockListenerStatusOut(this);
            }
        }

        /// <summary>
        /// Create XML submission contact log in.
        /// </summary>
        /// <returns><see cref="string"/> Created XML for submission.</returns>
        private string CreateXMLSubmitContactLogIn()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var deviceID = SysDeviceID.GetDeviceID();
            var refreshOpt = new RefreshDataOptions().LoadData();
            var revision = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var login = new ContactLogin().LoadData();
            var loginID = login.LoginID.ToString();

            var submit = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "SUBMIT CONTACT LOG IN"),
                        new XElement("device_id", deviceID),
                        new XElement("login_id", loginID),
                        new XElement("revision_datetime", revision))));

            submit.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Create XML submission contact log out.
        /// </summary>
        /// <returns><see cref="string"/> Created XML for submission.</returns>
        private string CreateXMLSubmitContactLogOut()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var deviceID = SysDeviceID.GetDeviceID();
            var refreshOpt = new RefreshDataOptions().LoadData();
            var revision = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var login = new ContactLogin().LoadData();
            var loginID = login.LoginID.ToString();

            var submit = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "SUBMIT CONTACT LOG OUT"),
                        new XElement("device_id", deviceID),
                        new XElement("login_id", loginID),
                        new XElement("revision_datetime", revision))));

            submit.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
