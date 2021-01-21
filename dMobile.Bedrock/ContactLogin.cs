namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    [Serializable]
    public class ContactLogin : IContactLoginEventPublisher, ISysSocketEventListener
    {
        #region Fields

        [NonSerialized]
        private string fileName = "dMobileLogin.xml";
        [NonSerialized]
        private SysSocket loginSocket = new SysSocket();
        [NonSerialized]
        private CommunicationSetup commSetup = new CommunicationSetup();
        private int loginID = 0;
        private string loginPassword = string.Empty;
        private int loginWarehouseLocationMapRefno = 0;
        private bool isLoggedIn;
        private bool isAllowedAssociateProductBarCode = true;
        private bool isAllowedPickingModuleOverridePicker = false;
        private bool isAllowedPickingModuleOverrideAddManualDetail = false;

        #endregion

        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public ContactLogin()
        {
            this.CommSetup = this.CommSetup.LoadData();

            this.LoginSocket.BasePort = this.CommSetup.iBaseSocketPort;
            this.LoginSocket.DefSocketPort = this.CommSetup.iDefaultSocketPort;
            this.LoginSocket.MaxPorts = this.CommSetup.iMaxSocketPorts;
            this.LoginSocket.ConnTotalRetries = this.CommSetup.iConnectionAttempts;
            this.LoginSocket.SendTotalRetries = this.CommSetup.iSendAttempts;
            this.LoginSocket.RecvTotalRetries = this.CommSetup.iReceiveAttempts;
            this.LoginSocket.SendTimeOutMilliseconds = this.CommSetup.iSendTimeOutSeconds * 1000;
            this.LoginSocket.ReceiveTimeOutMilliseconds = this.CommSetup.iReceiveTimeOutSeconds * 1000;
            this.LoginSocket.RecvBuffSize = this.CommSetup.iiReceiveBufferSize;
            this.LoginSocket.ServerIP = this.CommSetup.iHostIPAddressComponent1.ToString() + "." +
                    this.CommSetup.iHostIPAddressComponent2.ToString() + "." +
                    this.CommSetup.iHostIPAddressComponent3.ToString() + "." +
                    this.CommSetup.iHostIPAddressComponent4.ToString();
        }

        #endregion

        #region Delegates

        private delegate void OnContactLoginStatusOutEventHandler(
            IContactLoginEventPublisher contactLoginEventPublisher,
            ContactLoginEventArgs contactLoginEventArgs);

        #endregion

        #region Events

        private event OnContactLoginStatusOutEventHandler OnContactLoginStatusOut;

        #endregion

        #region Properties

        public int LoginID
        {
            get
            {
                return this.loginID;
            }

            set
            {
                this.loginID = value;
            }
        }

        public string LoginPassword
        {
            get
            {
                return this.loginPassword;
            }

            set
            {
                this.loginPassword = value;
            }
        }

        public int WarehouseLocationMapRefno
        {
            get
            {
                return this.loginWarehouseLocationMapRefno;
            }

            set
            {
                this.loginWarehouseLocationMapRefno = value;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return this.isLoggedIn;
            }

            set
            {
                this.isLoggedIn = value;
            }
        }

        public bool IsAllowedAssociateProductBarCode
        {
            get
            {
                return this.isAllowedAssociateProductBarCode;
            }

            set
            {
                this.isAllowedAssociateProductBarCode = value;
            }
        }

        public bool IsAllowedPickingModuleOverridePicker
        {
            get
            {
                return this.isAllowedPickingModuleOverridePicker;
            }

            set
            {
                this.isAllowedPickingModuleOverridePicker = value;
            }
        }

        public bool IsAllowedPickingModuleOverrideAddManualDetail
        {
            get
            {
                return this.isAllowedPickingModuleOverrideAddManualDetail;
            }

            set
            {
                this.isAllowedPickingModuleOverrideAddManualDetail = value;
            }
        }

        private SysSocket LoginSocket
        {
            get
            {
                return this.loginSocket;
            }

            set
            {
                this.loginSocket = value;
            }
        }

        private CommunicationSetup CommSetup
        {
            get
            {
                return this.commSetup;
            }

            set
            {
                this.commSetup = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public ContactLogin LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContactLogin));
            ContactLogin contactLogin = new ContactLogin();

            // use relection to get the current .≥assembly path for the database
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullPathToMyFile = Path.Combine(appDir, this.fileName);

            if (!File.Exists(fullPathToMyFile))
            {
                this.SaveData(ref contactLogin);
            }

            // Create a FileStream to read with.
            using (FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open))
            {
                // Deserialize the stream to the object, and close the TextReader
                contactLogin = (ContactLogin)serializer.Deserialize(reader);
                reader.Close();
            }

            return contactLogin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactLogin"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveData(ref ContactLogin contactLogin)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContactLogin));
            bool saveComplete = false;

            try
            {
                // use relection to get the current assembly path for the database
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fullPathToMyFile = Path.Combine(appDir, this.fileName);

                // Create a FileStream to write with.
                using (FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create))
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
        /// 
        /// </summary>
        /// <remarks></remarks>
        public void SubmitContactLogIn()
        {
            string reqData = this.SubmitContactLogInCommunication();
            this.SubmitContactLogInData(reqData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public void SubmitContactLogOut()
        {
            string reqData = this.SubmitContactLogOutCommunication();
            this.SubmitContactLogOutData(reqData);
        }

        #region IContactLoginEventPublisher Members

        public void RegisterContactLoginListenerStatusOut(IContactLoginEventListener contactLoginEventListener)
        {
            this.OnContactLoginStatusOut += new OnContactLoginStatusOutEventHandler(contactLoginEventListener.OnContactLoginStatusOut);
        }

        public void UnregisterContactLoginListenerStatusOut(IContactLoginEventListener contactLoginEventListener)
        {
            this.OnContactLoginStatusOut -= new OnContactLoginStatusOutEventHandler(contactLoginEventListener.OnContactLoginStatusOut);
        }

        #endregion

        #region ISysSocketEventListener Members
        public void OnSysSocketStatusOut(ISysSocketEventPublisher sysSocketEventPublisher, SysSocketEventArgs sysSocketEventArgs)
        {
            ContactLoginEventArgs args = new ContactLoginEventArgs(sysSocketEventArgs.StatusOutMsg);
            this.OnContactLoginStatusOut(this, args);
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string SubmitContactLogInCommunication()
        {
            string sendSubmit = null;
            Socket socket = default(Socket);
            int bytesSent = default(int);
            List<byte> bytRecvData = null;
            string reqData = string.Empty;
            ContactLoginEventArgs msgEventArg = new ContactLoginEventArgs();

            SysSocket sysSocket = new SysSocket();
            sysSocket.RegSockListenerStatusOut(this);

            try
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Creating request for contact log in";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                sendSubmit = this.CreateXMLSubmitContactLogIn();

                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Attempting to connect";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                socket = this.LoginSocket.Connect();

                if (socket != null)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Sending request";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytesSent = this.LoginSocket.SendRequest(socket, sendSubmit);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Waiting for response";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytRecvData = this.LoginSocket.ReceiveResponse(socket);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Response received";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    socket.Close();

                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Decompressing";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    reqData = Compress.UnZip(bytRecvData.ToArray());
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Failed to connect";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    // TODO: Resource strings
                    throw new Exception("Failed to connect");
                }
            }
            catch (SocketException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
#if MULTILANGUAGE
          msgEventArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SocketExceptionOccurredz);
#else
                    msgEventArg.StatusOutMsg = "Socket exception occurred";
#endif
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
#if MULTILANGUAGE
          msgEventArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.RequestForContactLogInFailed);
#else
                    msgEventArg.StatusOutMsg = "Request for contact log in failed";
#endif
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
#if MULTILANGUAGE
          msgEventArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.RequestForContactLogInFailed);
#else
                    msgEventArg.StatusOutMsg = "Request for contact log in failed";
#endif
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            finally
            {
                sysSocket.UnregSockListenerStatusOut(this);
            }

            return reqData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string SubmitContactLogOutCommunication()
        {
            string sendSubmit = null;
            Socket socket = default(Socket);
            int bytesSent = default(int);
            List<byte> bytRecvData = null;
            string reqData = string.Empty;
            ContactLoginEventArgs msgEventArg = new ContactLoginEventArgs();
            SysSocket sysSocket = new SysSocket();

            sysSocket.RegSockListenerStatusOut(this);

            try
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Creating request for contact log out";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                sendSubmit = this.CreateXMLSubmitContactLogOut();

                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Attempting to connect";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                socket = this.LoginSocket.Connect();

                if (socket != null)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Sending request";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytesSent = this.LoginSocket.SendRequest(socket, sendSubmit);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Waiting for response";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    bytRecvData = this.LoginSocket.ReceiveResponse(socket);

                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Response received";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    socket.Close();

                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Decompressing";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    reqData = Compress.UnZip(bytRecvData.ToArray());
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Failed to connect";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    // TODO: Resource strings
                    throw new Exception("Failed to connect");
                }
            }
            catch (SocketException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Socket exception occurred";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Request for contact log out failed";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Request for contact log out failed";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            finally
            {
                sysSocket.UnregSockListenerStatusOut(this);
            }

            return reqData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        private void SubmitContactLogInData(string reqData)
        {
            ContactLoginEventArgs msgEventArg = new ContactLoginEventArgs();

            SysSocket sysSocket = new SysSocket();
            sysSocket.RegSockListenerStatusOut(this);

            try
            {
                if (reqData.Length > 0)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Parsing received data";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);

                        // TODO: Logging
                        //#if MULTILANGUAGE
                        //            MessageBox.Show(Translator.GetInstance().Translate(TranslatorVocabulary.SubmitContactLogInError) + Strings.Chr(13) +
                        //#else
                        //                        MessageBox.Show("Submit contact log in error" + Strings.Chr(13) +
                        //#endif
                        // docError.DocumentElement["detail"].InnerText);
                        docError = null;
                    }
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Received nothing to process";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Database exception occurred";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Submit contact log in failed.";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
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
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        private void SubmitContactLogOutData(string reqData)
        {
            ContactLoginEventArgs msgEventArg = new ContactLoginEventArgs();

            SysSocket sockPublisher = new SysSocket();
            sockPublisher.RegSockListenerStatusOut(this);

            try
            {
                if (reqData.Length > 0)
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Parsing received data";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }

                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;

                        // TODO: Logging
                        //#if MULTILANGUAGE
                        //            MessageBox.Show(Translator.GetInstance().Translate(TranslatorVocabulary.SubmitContactLogOutError) + Strings.Chr(13) +
                        //#else
                        //                        MessageBox.Show("Submit contact log out error" + Strings.Chr(13) +
                        //#endif
                        // errorDetail);
                        docError = null;
                    }
                }
                else
                {
                    if (this.OnContactLoginStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Received nothing to process";
                        this.OnContactLoginStatusOut(this, msgEventArg);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Database exception occurred";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Submit contact log out failed.";
                    this.OnContactLoginStatusOut(this, msgEventArg);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                if (this.OnContactLoginStatusOut != null)
                {
                    // TODO: Resource strings
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
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string CreateXMLSubmitContactLogIn()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();
            RefreshDataOptions refreshOpt = new RefreshDataOptions().LoadData();
            string revision = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            ContactLogin login = new ContactLogin().LoadData();
            string loginID = login.LoginID.ToString();

            XDocument submit = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), string.Empty,
                  new XElement("imclient",
                      new XElement("command",
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
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string CreateXMLSubmitContactLogOut()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();
            RefreshDataOptions refreshOpt = new RefreshDataOptions().LoadData();
            string revision = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            ContactLogin login = new ContactLogin().LoadData();
            string loginID = login.LoginID.ToString();

            XDocument submit = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), string.Empty,
                    new XElement("imclient",
                        new XElement("command",
                            new XElement("action", "SUBMIT CONTACT LOG OUT"),
                            new XElement("device_id", deviceID),
                            new XElement("login_id", loginID),
                            new XElement("revision_datetime", revision))));

            submit.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        #endregion
    }
}
