namespace dMobile.Bedrock
{
    using System.Data;
    using System.IO;
    using System;
    using SQLite;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="DatabaseConnection" />
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Defines the dataConnUniq
        /// </summary>
        private static DatabaseConnection dataConnUniq;

        /// <summary>
        /// Defines the sqlConn
        /// </summary>
        private static SQLiteAsyncConnection sqlConn = null;

        /// <summary>
        /// Defines the dbPath
        /// </summary>
        //private static string dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "//";
        private static string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        /// <summary>
        /// Defines the dbName
        /// </summary>
        private static string dbName = "dMobile.Sqlite";

        /// <summary>
        /// Defines the dbASCIIZ
        /// </summary>
        private static string dbASCIIZ = string.Empty;

        /// <summary>
        /// Defines the dbPassword
        /// </summary>
        private static string dbPassword = "dMobile";

        /// <summary>
        /// Defines the connString
        /// </summary>
        private static SQLiteConnectionString connString;

        /// <summary>
        /// Prevents a default instance of the <see cref="DatabaseConnection"/> class from being created.
        /// </summary>
        private DatabaseConnection()
        {
        }

        /// <summary>
        /// Gets or sets the DataConnUniq
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ConnString
        /// </summary>
        public SQLiteConnectionString ConnString
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

        /// <summary>
        /// Gets or sets the DBASCIIZ
        /// </summary>
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

        /// <summary>
        /// Gets or sets the DBPath
        /// </summary>
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

        /// <summary>
        /// Gets the DBPassword
        /// </summary>
        public string DBPassword
        {
            get
            {
                return dbPassword;
            }
        }

        /// <summary>
        /// Gets the DBName
        /// </summary>
        public string DBName
        {
            get
            {
                return dbName;
            }
        }

        /// <summary>
        /// Gets the SQLConn
        /// </summary>
        public SQLiteAsyncConnection SQLConn
        {
            get
            {
                return sqlConn;
            }
        }

        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <returns></returns>
        public static DatabaseConnection GetInstance()
        {
            if (dataConnUniq == null)
            {
                dataConnUniq = new DatabaseConnection();
                dbASCIIZ = Path.Combine(dbPath, dbName);
                //connString = new SQLiteConnectionString(dbASCIIZ);
                // foreign keys=true HAS to be specified or SQLite will not enforce foreign key constraints
                //connString = "Data Source = '" + dbASCIIZ + "'; foreign keys=true;";
                //connString = "Data Source = '" + dbPath + dbName + "'; Password = " + dbPassword;
            }

            OpenDataBaseConnection();

            return dataConnUniq;
        }
        
        /// <summary>
        /// The OpenDataBaseConnection
        /// </summary>
        public static void OpenDataBaseConnection()
        {
            if (File.Exists(dbASCIIZ) & sqlConn == null)
            {
                sqlConn = new SQLiteAsyncConnection(dbASCIIZ, SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);
            }

            //if (sqlConn != null)
            //{
            //    if (sqlConn.State == ConnectionState.Closed)
            //    {
            //        sqlConn.Open();
            //    }
            //}
        }

        /// <summary>   
        /// The DisposeDataBaseConnection
        /// </summary>
        public static void DisposeDataBaseConnection()
        {
            if (sqlConn != null)
            {
                //if (sqlConn.State == ConnectionState.Closed)
                //{
                //sqlConn.Dispose();
                sqlConn = null;
                dataConnUniq = null;
                //}
            }
        }

        /// <summary>
        /// The CloseDataBaseConnection
        /// </summary>
        public static async Task CloseDataBaseConnection()
        {
            if (sqlConn != null)
            {
                //if (sqlConn.State == ConnectionState.Open)
                //{
                await sqlConn.CloseAsync();
                //}
            }
        }

        ///// <summary>
        ///// The IsDataBaseConnectionOpen
        ///// </summary>
        ///// <returns>The <see cref="bool"/></returns>
        //public static bool IsDataBaseConnectionOpen()
        //{
        //    bool isConnOpen = false;

        //    if (!(sqlConn.State == ConnectionState.Open))
        //    {
        //        isConnOpen = true;
        //    }

        //    return isConnOpen;
        //}
    }
}
