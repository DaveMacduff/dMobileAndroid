namespace dMobile.Bedrock
{
	using System;
	using System.IO;
	using System.Xml.Serialization;

	[Serializable()]
	public class SubmitBatchSettings
	{
		//VARIABLES
		[NonSerialized()]
		private string m_strFullClassName = "dMobile.SubmitBatchSettings";
		[NonSerialized()]
		private string m_strFilePath = string.Empty;
		[NonSerialized()]
		private string m_strFileASCIIZ = string.Empty;
		[NonSerialized()]
		private string m_strFileName = "dMobileSubmitBatchSettings.xml";
		private Int32 m_iActivityStatusIdx = 0;
		private SysCommon.ActStatus m_eActivityStatus = SysCommon.ActStatus.NoChange;
		private Int32 m_iActivitySubTypeIdx = 0;
		private SysCommon.ActSubTypes m_eActivitySubType = SysCommon.ActSubTypes.Picking;

		//PROPERTIES
		public SysCommon.ActSubTypes eActivitySubType
		{
			get { return m_eActivitySubType; }
			set { m_eActivitySubType = value; }
		}
		public SysCommon.ActStatus eActivityStatus
		{
			get { return m_eActivityStatus; }
			set { m_eActivityStatus = value; }
		}
		public Int32 iActivityStatusIdx
		{
			get { return m_iActivityStatusIdx; }
			set { m_iActivityStatusIdx = value; }
		}
		public Int32 iActivitySubTypeIdx
		{
			get { return m_iActivitySubTypeIdx; }
			set { m_iActivitySubTypeIdx = value; }
		}
		public string strFullClassName
		{
			get { return m_strFullClassName; }
			set { m_strFullClassName = value; }
		}
		public string strFilePath
		{
			get { return m_strFilePath; }
			set { m_strFilePath = value; }
		}
		public string strFileASCIIZ
		{
			get { return m_strFileASCIIZ; }
			set { m_strFileASCIIZ = value; }
		}
		public string strFileName
		{
			get { return m_strFileName; }
			set { m_strFileName = value; }
		}

		//METHODS

		/// <summary>
		/// 
		/// </summary>
		/// <remarks></remarks>
		public SubmitBatchSettings()
		{
			strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			strFileASCIIZ = Path.Combine(strFilePath, strFileName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public SubmitBatchSettings LoadData()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(SubmitBatchSettings));
			SubmitBatchSettings oSubmitBatchSettings = new SubmitBatchSettings();

			// use relection to get the current assembly path for the database
			string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

			if (!File.Exists(strFullPathToMyFile))
			{
				SaveData(ref oSubmitBatchSettings);
			}

			// Create a FileStream to read with.
			FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open);
			// Deserialize the stream to the object, and close the TextReader
			oSubmitBatchSettings = (SubmitBatchSettings)serializer.Deserialize(reader);
			reader.Close();

			return oSubmitBatchSettings;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oSubmitBatchSettings"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool SaveData(ref SubmitBatchSettings oSubmitBatchSettings)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(SubmitBatchSettings));
			bool bSaveComplete = false;

			try
			{
				// use relection to get the current assembly path for the database
				string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

				// Create a FileStream to write with.
				FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create);
				// Serialize the object, and close the TextWriter
				serializer.Serialize(writer, oSubmitBatchSettings);
				writer.Close();

				bSaveComplete = true;

			}
			catch
			{
				bSaveComplete = false;
			}

			return bSaveComplete;
		}
	}
}
