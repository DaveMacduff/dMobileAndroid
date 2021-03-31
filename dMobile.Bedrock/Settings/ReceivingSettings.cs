namespace dMobile.Bedrock
{
	using System;
	using System.IO;
	using System.Xml.Serialization;

	[Serializable]
	public class ReceivingSettings
	{
		// VARIABLES
		[NonSerialized()]
		private string m_strFullClassName = "dMobile.ReceivingSettings";
		[NonSerialized()]
		private string m_strFilePath = string.Empty;
		[NonSerialized()]
		private string m_strFileASCIIZ = string.Empty;
		[NonSerialized()]
		private string m_strFileName = "dMobileReceivingSettings.xml";
		[NonSerialized]
		private static readonly object lockObject = new object();
		private Int32 m_iSortBy1Index = 0;
		private string m_strSortBy1Value = "ActivityReceivingFKRefno ASC";
		private Int32 m_iSortBy2Index = -1;
		private string m_strSortBy2Value = string.Empty;
		private Int32 m_iSortBy3Index = -1;
		private string m_strSortBy3Value = string.Empty;
		private Int32 m_iSortBy4Index = -1;
		private string m_strSortBy4Value = string.Empty;
		private bool isApplyArrivalDateFilter = false;
		private System.DateTime m_dtArrivalDateFilter = new System.DateTime(1900, 1, 1);
		private bool isApplyIssuedDateFilter = false;
		private System.DateTime m_dtIssuedDateFilter = new System.DateTime(1900, 1, 1);

		//PROPERTIES
		public bool IsApplyIssuedDateFilter
		{
			get { return isApplyIssuedDateFilter; }
			set { isApplyIssuedDateFilter = value; }
		}

		public bool IsApplyArrivalDateFilter
		{
			get { return isApplyArrivalDateFilter; }
			set { isApplyArrivalDateFilter = value; }
		}

		public System.DateTime dtIssuedDateFilter
		{
			get { return m_dtIssuedDateFilter; }
			set { m_dtIssuedDateFilter = value; }
		}

		public System.DateTime dtArrivalDateFilter
		{
			get { return m_dtArrivalDateFilter; }
			set { m_dtArrivalDateFilter = value; }
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

		public Int32 iSortBy1Index
		{
			get { return m_iSortBy1Index; }
			set { m_iSortBy1Index = value; }
		}

		public string strSortBy1Value
		{
			get { return m_strSortBy1Value; }
			set { m_strSortBy1Value = value; }
		}

		public Int32 iSortBy2Index
		{
			get { return m_iSortBy2Index; }
			set { m_iSortBy2Index = value; }
		}

		public string strSortBy2Value
		{
			get { return m_strSortBy2Value; }
			set { m_strSortBy2Value = value; }
		}

		public Int32 iSortBy3Index
		{
			get { return m_iSortBy3Index; }
			set { m_iSortBy3Index = value; }
		}

		public string strSortBy3Value
		{
			get { return m_strSortBy3Value; }
			set { m_strSortBy3Value = value; }
		}

		public Int32 iSortBy4Index
		{
			get { return m_iSortBy4Index; }
			set { m_iSortBy4Index = value; }
		}

		public string strSortBy4Value
		{
			get { return m_strSortBy4Value; }
			set { m_strSortBy4Value = value; }
		}

		//METHODS

		/// <summary>
		/// 
		/// </summary>
		/// <remarks></remarks>
		public ReceivingSettings()
		{
			strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			strFileASCIIZ = Path.Combine(strFilePath, strFileName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <Receiving></Receiving>
		/// <remarks></remarks>
		public string GetDataViewSortBy()
		{
			string strSortBy = strSortBy1Value;

			if (!string.IsNullOrEmpty(strSortBy2Value))
			{
				strSortBy = strSortBy + ", " + strSortBy2Value;
			}
			if (!string.IsNullOrEmpty(strSortBy3Value))
			{
				strSortBy = strSortBy + ", " + strSortBy3Value;
			}
			if (!string.IsNullOrEmpty(strSortBy4Value))
			{
				strSortBy = strSortBy + ", " + strSortBy4Value;
			}
			return strSortBy;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <Receiving></Receiving>
		/// <remarks></remarks>
		public ReceivingSettings LoadData()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ReceivingSettings));
			ReceivingSettings oReceivingSettings = new ReceivingSettings();

			// use relection to get the current assembly path for the database
			string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

			if (!File.Exists(strFullPathToMyFile))
			{
				SaveData(ref oReceivingSettings);
			}

			// Create a FileStream to read with.
			using (FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open))
			{
				// Deserialize the stream to the object, and close the TextReader
				oReceivingSettings = (ReceivingSettings)serializer.Deserialize(reader);
			}

			return oReceivingSettings;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oReceivingSettings"></param>
		/// <Receiving></Receiving>
		/// <remarks></remarks>
		public bool SaveData(ref ReceivingSettings oReceivingSettings)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ReceivingSettings));
			bool bSaveComplete = false;

			lock (lockObject)
			{
				try
				{
					// use relection to get the current assembly path for the database
					string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

					// Create a FileStream to write with.
					using (FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create))
                    {
						// Serialize the object, and close the TextWriter
						serializer.Serialize(writer, oReceivingSettings);
                    }

					bSaveComplete = true;
				}
				catch
				{
					bSaveComplete = false;
				}
			}

			return bSaveComplete;
		}
	}
}
