// <copyright file="CommunicationSetup.cs" company="InterSect Business Systems Inc.">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CommunicationSetup
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private string fullClassName = "dMobile.Bedrock.CommunicationSetup";

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private string filePath = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private string fileASCIIZ = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobileCommunicationSetup.xml";

        /// <summary>
        /// 
        /// </summary>
        private int hostIPAddressComponent1 = 127;

        /// <summary>
        /// 
        /// </summary>
        private int hostIPAddressComponent2 = 0;

        /// <summary>
        /// 
        /// </summary>
        private int hostIPAddressComponent3 = 0;

        /// <summary>
        /// 
        /// </summary>
        private int hostIPAddressComponent4 = 1;

        /// <summary>
        /// 
        /// </summary>
        private int baseSocketPort = 10000;

        /// <summary>
        /// 
        /// </summary>
        private int maxSocketPorts = 1;

        /// <summary>
        /// 
        /// </summary>
        private int defaultSocketPort = 10000;

        /// <summary>
        /// 
        /// </summary>
        private int connectionAttempts = 1;

        /// <summary>
        /// Seconds to wait for a send request.
        /// </summary>
        private int sendTimeOutSeconds = 60;

        /// <summary>
        /// Number of times to attempt a send.
        /// </summary>
        private int sendAttempts = 4;

        /// <summary>
        /// 
        /// </summary>
        private int receiveAttempts = 1;

        /// <summary>
        /// Seconds to wait for a receive request.
        /// </summary>
        private int receiveTimeOutSeconds = 99;

        /// <summary>
        /// 
        /// </summary>
        private int receiveBufferSize = 32768;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public CommunicationSetup()
        {
            //this.strFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";
            this.strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.strFileASCIIZ = Path.Combine(this.strFilePath, this.strFileName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string strFullClassName
        {
            get
            {
                return this.fullClassName;
            }

            set
            {
                this.fullClassName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string strFilePath
        {
            get
            {
                return this.filePath;
            }

            set
            {
                this.filePath = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string strFileASCIIZ
        {
            get
            {
                return this.fileASCIIZ;
            }

            set
            {
                this.fileASCIIZ = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string strFileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                this.fileName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iHostIPAddressComponent1
        {
            get
            {
                return this.hostIPAddressComponent1;
            }

            set
            {
                this.hostIPAddressComponent1 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iHostIPAddressComponent2
        {
            get
            {
                return this.hostIPAddressComponent2;
            }

            set
            {
                this.hostIPAddressComponent2 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iHostIPAddressComponent3
        {
            get
            {
                return this.hostIPAddressComponent3;
            }

            set
            {
                this.hostIPAddressComponent3 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iHostIPAddressComponent4
        {
            get
            {
                return this.hostIPAddressComponent4;
            }

            set
            {
                this.hostIPAddressComponent4 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iBaseSocketPort
        {
            get
            {
                return this.baseSocketPort;
            }

            set
            {
                this.baseSocketPort = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iMaxSocketPorts
        {
            get
            {
                return this.maxSocketPorts;
            }

            set
            {
                this.maxSocketPorts = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iDefaultSocketPort
        {
            get
            {
                return this.defaultSocketPort;
            }

            set
            {
                this.defaultSocketPort = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iConnectionAttempts
        {
            get
            {
                return this.connectionAttempts;
            }

            set
            {
                this.connectionAttempts = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iSendTimeOutSeconds
        {
            get
            {
                return this.sendTimeOutSeconds;
            }

            set
            {
                this.sendTimeOutSeconds = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iSendAttempts
        {
            get
            {
                return this.sendAttempts;
            }

            set
            {
                this.sendAttempts = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iReceiveAttempts
        {
            get
            {
                return this.receiveAttempts;
            }

            set
            {
                this.receiveAttempts = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iReceiveTimeOutSeconds
        {
            get
            {
                return this.receiveTimeOutSeconds;
            }

            set
            {
                this.receiveTimeOutSeconds = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int iiReceiveBufferSize
        {
            get
            {
                return this.receiveBufferSize;
            }

            set
            {
                this.receiveBufferSize = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string CreateXMLSubmitDeviceID()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), string.Empty,
                new XElement("imclient",
                    new XElement("command",
                        new XElement("action", "SUBMIT DEVICE ID"),
                        new XElement("device_id", deviceID))));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CommunicationSetup LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CommunicationSetup));
            CommunicationSetup commSetup = new CommunicationSetup();

            // use relection to get the current assembly path for the database
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullPathToMyFile = Path.Combine(appDir, this.fileName);

            if (!File.Exists(fullPathToMyFile))
            {
                this.SaveData(ref commSetup);
            }

            // Create a FileStream to read with.
            FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open);

            // Deserialize the stream to the object, and close the TextReader
            commSetup = (CommunicationSetup)serializer.Deserialize(reader);
            reader.Close();

            return commSetup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commSetup"></param>
        /// <returns></returns>
        public bool SaveData(ref CommunicationSetup commSetup)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CommunicationSetup));
            bool isSaveComplete = false;

            try
            {
                // use relection to get the current assembly path for the database
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fullPathToMyFile = Path.Combine(appDir, this.fileName);

                // Create a FileStream to write with.
                FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create);

                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, commSetup);
                writer.Close();

                isSaveComplete = true;
            }
            catch
            {
                isSaveComplete = false;
            }

            return isSaveComplete;
        }

        #endregion
    }
}
