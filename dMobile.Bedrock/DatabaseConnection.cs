namespace dMobile.Bedrock
{
    using SQLite;
    using System.Data;
    using System.IO;
    using System.Reflection;

    public class DatabaseConnection
    {
        #region Fields

        private static DatabaseConnection dataConnUniq;
        private static SQLiteConnection sqlConn = null;
        private static string dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";
        private static string dbName = "dMobile.sqlite";
        private static string dbASCIIZ = string.Empty;
        private static string dbPassword = "dMobile";
        private static string connString = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        private DatabaseConnection()
        {
        }

        #endregion

        #region Properties

        public DatabaseConnection DataConnUniq
        {
            get
            {
                return dataConnUniq;
            }

            set
            {
                dataConnUniq = value;
            }
        }

        public string ConnString
        {
            get
            {
                return connString;
            }

            set
            {
                connString = value;
            }
        }

        public string DBASCIIZ
        {
            get
            {
                return dbASCIIZ;
            }

            set
            {
                dbASCIIZ = value;
            }
        }

        public string DBPath
        {
            get
            {
                return dbPath;
            }

            set
            {
                dbPath = value;
            }
        }

        public string DBPassword
        {
            get
            {
                return dbPassword;
            }
        }

        public string DBName
        {
            get
            {
                return dbName;
            }
        }

        public SQLiteConnection SQLConn
        {
            get
            {
                return sqlConn;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DatabaseConnection GetInstance()
        {
            if (dataConnUniq == null)
            {
                dataConnUniq = new DatabaseConnection();
                dbASCIIZ = Path.Combine(dbPath, dbName);
                connString = "Data Source = '" + dbPath + dbName + "'; Password = " + dbPassword;
            }

            OpenDataBaseConnection();

            return dataConnUniq;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public static void OpenDataBaseConnection()
        {
            if (File.Exists(dbASCIIZ) & sqlConn == null)
            {
                sqlConn = new SQLiteConnection(dbASCIIZ);
            }

            //if (sqlConn != null)
            //{
                //if (sqlConn.State == ConnectionState.Closed)
                //{
                    //sqlConn.Open();
                //}
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public static void DisposeDataBaseConnection()
        {
            if (sqlConn != null)
            {
                //if (sqlConn.State == ConnectionState.Closed)
                //{
                    sqlConn.Dispose();
                    sqlConn = null;
                    dataConnUniq = null;
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public static void CloseDataBaseConnection()
        {
            if (sqlConn != null)
            {
                //if (sqlConn.State == ConnectionState.Open)
                //{
                    sqlConn.Close();
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public static bool IsDataBaseConnectionOpen()
        {
            //bool isConnOpen = false;

            //if (!(sqlConn.State == ConnectionState.Open))
            //{
            //    isConnOpen = true;
            //}

            return true;
        }

        #endregion
    }
}
