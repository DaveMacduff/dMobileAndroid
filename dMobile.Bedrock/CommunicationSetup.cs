// <copyright file="CommunicationSetup.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the configuration settings required for establishing and managing communication parameters, such as
    /// network addresses, socket ports, timeouts, and retry attempts.
    /// </summary>
    /// <remarks>This class provides properties for specifying host IP address components, socket port ranges,
    /// file paths for configuration storage, and timeout values for send and receive operations. It also includes
    /// methods for serializing and deserializing the configuration to and from XML files, as well as generating device
    /// identification XML. Instances of this class can be used to persist and restore communication settings across
    /// application sessions.</remarks>
    [Serializable]
    public class CommunicationSetup
    {
        /// <summary>
        /// Defines the fullClassName.
        /// </summary>
        [NonSerialized]
        private string fullClassName = "dMobile.Bedrock.CommunicationSetup";

        /// <summary>
        /// Defines the filePath.
        /// </summary>
        [NonSerialized]
        private string filePath = string.Empty;

        /// <summary>
        /// Defines the file ASCIIZ.
        /// </summary>
        [NonSerialized]
        private string fileAsciiz = string.Empty;

        /// <summary>
        /// Defines the fileName.
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobileCommunicationSetup.xml";

        /// <summary>
        /// Defines the hostIPAddressComponent1.
        /// </summary>
        private int hostIpAddressComponent1 = 127;

        /// <summary>
        /// Defines the hostIPAddressComponent2.
        /// </summary>
        private int hostIpAddressComponent2;

        /// <summary>
        /// Defines the hostIPAddressComponent3.
        /// </summary>
        private int hostIpAddressComponent3;

        /// <summary>
        /// Defines the hostIPAddressComponent4.
        /// </summary>
        private int hostIpAddressComponent4 = 1;

        /// <summary>
        /// Defines the baseSocketPort.
        /// </summary>
        private int baseSocketPort = 10000;

        /// <summary>
        /// Defines the maxSocketPorts.
        /// </summary>
        private int maxSocketPorts = 1;

        /// <summary>
        /// Defines the defaultSocketPort.
        /// </summary>
        private int defaultSocketPort = 10000;

        /// <summary>
        /// Defines the connectionAttempts.
        /// </summary>
        private int connectionAttempts = 1;

        /// <summary>
        /// Seconds to wait for a send request.
        /// </summary>
        private int sendTimeOutSeconds = 10;

        /// <summary>
        /// Number of times to attempt to send.
        /// </summary>
        private int sendAttempts;

        /// <summary>
        /// Defines the receiveAttempts.
        /// </summary>
        private int receiveAttempts;

        /// <summary>
        /// Seconds to wait for a received response.
        /// </summary>
        private int receiveTimeOutSeconds = 30;

        /// <summary>
        /// Defines the receiveBufferSize.
        /// </summary>
        private int receiveBufferSize = 32768;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSetup"/> class.
        /// </summary>
        public CommunicationSetup()
        {
            // this.strFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileAsciiz = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets the FullClassName.
        /// </summary>
        public string FullClassName
        {
            get => this.fullClassName;

            set => this.fullClassName = value;
        }

        /// <summary>
        /// Gets or sets the strFilePath.
        /// </summary>
        public string FilePath
        {
            get => this.filePath;

            set => this.filePath = value;
        }

        /// <summary>
        /// Gets or sets the strFileASCIIZ.
        /// </summary>
        public string FileAsciiz
        {
            get => this.fileAsciiz;

            set => this.fileAsciiz = value;
        }

        /// <summary>
        /// Gets or sets the strFileName.
        /// </summary>
        public string FileName
        {
            get => this.fileName;

            set => this.fileName = value;
        }

        /// <summary>
        /// Gets or sets the iHostIPAddressComponent1.
        /// </summary>
        public int HostIpAddressComponent1
        {
            get => this.hostIpAddressComponent1;

            set => this.hostIpAddressComponent1 = value;
        }

        /// <summary>
        /// Gets or sets the iHostIPAddressComponent2.
        /// </summary>
        public int HostIpAddressComponent2
        {
            get => this.hostIpAddressComponent2;

            set => this.hostIpAddressComponent2 = value;
        }

        /// <summary>
        /// Gets or sets the iHostIPAddressComponent3.
        /// </summary>
        public int HostIpAddressComponent3
        {
            get => this.hostIpAddressComponent3;

            set => this.hostIpAddressComponent3 = value;
        }

        /// <summary>
        /// Gets or sets the iHostIPAddressComponent4.
        /// </summary>
        public int HostIpAddressComponent4
        {
            get => this.hostIpAddressComponent4;

            set => this.hostIpAddressComponent4 = value;
        }

        /// <summary>
        /// Gets or sets the iBaseSocketPort.
        /// </summary>
        public int BaseSocketPort
        {
            get => this.baseSocketPort;

            set => this.baseSocketPort = value;
        }

        /// <summary>
        /// Gets or sets the iMaxSocketPorts.
        /// </summary>
        public int MaxSocketPorts
        {
            get => this.maxSocketPorts;

            set => this.maxSocketPorts = value;
        }

        /// <summary>
        /// Gets or sets the iDefaultSocketPort.
        /// </summary>
        public int DefaultSocketPort
        {
            get => this.defaultSocketPort;

            set => this.defaultSocketPort = value;
        }

        /// <summary>
        /// Gets or sets the iConnectionAttempts.
        /// </summary>
        public int ConnectionAttempts
        {
            get => this.connectionAttempts;

            set => this.connectionAttempts = value;
        }

        /// <summary>
        /// Gets or sets the iSendTimeOutSeconds.
        /// </summary>
        public int SendTimeOutSeconds
        {
            get => this.sendTimeOutSeconds;

            set => this.sendTimeOutSeconds = value;
        }

        /// <summary>
        /// Gets or sets the iSendAttempts.
        /// </summary>
        public int SendAttempts
        {
            get => this.sendAttempts;

            set => this.sendAttempts = value;
        }

        /// <summary>
        /// Gets or sets the iReceiveAttempts.
        /// </summary>
        public int ReceiveAttempts
        {
            get => this.receiveAttempts;

            set => this.receiveAttempts = value;
        }

        /// <summary>
        /// Gets or sets the iReceiveTimeOutSeconds.
        /// </summary>
        public int ReceiveTimeOutSeconds
        {
            get => this.receiveTimeOutSeconds;

            set => this.receiveTimeOutSeconds = value;
        }

        /// <summary>
        /// Gets or sets the iiReceiveBufferSize.
        /// </summary>
        public int ReceiveBufferSize
        {
            get => this.receiveBufferSize;

            set => this.receiveBufferSize = value;
        }

        /// <summary>
        /// Create an xml submit device id string.
        /// </summary>
        /// <returns>.</returns>
        public static string CreateXmlSubmitDeviceId()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var deviceId = SysDeviceId.GetDeviceId();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "SUBMIT DEVICE ID"),
                        new XElement("device_id", deviceId))));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Load the communication setup data.
        /// </summary>
        /// <returns>.</returns>
        public CommunicationSetup LoadData()
        {
            var serializer = new XmlSerializer(typeof(CommunicationSetup));
            var commSetup = new CommunicationSetup();

            // use reflection to get the current assembly path for the database
            var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var settingsFileName = Path.Combine(appDir, this.fileName);

            if (!File.Exists(settingsFileName))
            {
                this.SaveData(ref commSetup);
            }

            // Create a FileStream to read with.
            var reader = new FileStream(settingsFileName, FileMode.Open);

            // Deserialize the stream to the object, and close the TextReader
            commSetup = (CommunicationSetup)serializer.Deserialize(reader);
            reader.Close();

            return commSetup;
        }

        /// <summary>
        /// The SaveData.
        /// </summary>
        /// <param name="commSetup">.</param>
        /// <returns>True if the save was successful, else false.</returns>
        public bool SaveData(ref CommunicationSetup commSetup)
        {
            var serializer = new XmlSerializer(typeof(CommunicationSetup));
            bool isSaveComplete;
            try
            {
                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var settingsFileName = Path.Combine(appDir, this.fileName);

                // Create a FileStream to write with.
                var writer = new FileStream(settingsFileName, FileMode.Create);

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
    }
}
