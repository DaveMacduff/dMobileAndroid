// <copyright file="DatabaseConnection.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using SQLite;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="DatabaseConnection" />.
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Defines the dataConnUniq.
        /// </summary>
        private static DatabaseConnection dataConnUniq;

        /// <summary>
        /// Defines the sqlConn.
        /// </summary>
        private static SQLiteAsyncConnection sqlConn = null;

        /// <summary>
        /// Defines the dbPath.
        /// </summary>
        private static string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        /// <summary>
        /// Defines the dbName.
        /// </summary>
        private static string dbName = "dMobile.Sqlite";

        /// <summary>
        /// Defines the dbASCIIZ.
        /// </summary>
        private static string dbASCIIZ = string.Empty;

        /// <summary>
        /// Defines the dbPassword.
        /// </summary>
        private static string dbPassword = "dMobile";

        /// <summary>
        /// Defines the connString.
        /// </summary>
        private static SQLiteConnectionString connString;

        /// <summary>
        /// Prevents a default instance of the <see cref="DatabaseConnection"/> class from being created.
        /// </summary>
        private DatabaseConnection()
        {
        }

        /// <summary>
        /// Gets or sets the DataConnUniq.
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
        /// Gets or sets the ConnString.
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
        /// Gets or sets the DBASCIIZ.
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
        /// Gets or sets the DBPath.
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
        /// Gets the DBPassword.
        /// </summary>
        public string DBPassword
        {
            get
            {
                return dbPassword;
            }
        }

        /// <summary>
        /// Gets the DBName.
        /// </summary>
        public string DBName
        {
            get
            {
                return dbName;
            }
        }

        /// <summary>
        /// Gets the SQLConn.
        /// </summary>
        public SQLiteAsyncConnection SQLConn
        {
            get
            {
                return sqlConn;
            }
        }

        /// <summary>
        /// The GetInstance.
        /// </summary>
        /// <returns>.</returns>
        public static DatabaseConnection GetInstance()
        {
            if (dataConnUniq == null)
            {
                dataConnUniq = new DatabaseConnection();
                dbASCIIZ = Path.Combine(dbPath, dbName);

                // connString = new SQLiteConnectionString(dbASCIIZ);
                // foreign keys=true HAS to be specified or SQLite will not enforce foreign key constraints
                // connString = "Data Source = '" + dbASCIIZ + "'; foreign keys=true;";
                // connString = "Data Source = '" + dbPath + dbName + "'; Password = " + dbPassword;
            }

            Open();

            return dataConnUniq;
        }

        /// <summary>
        /// The OpenDataBaseConnection.
        /// </summary>
        public static void Open()
        {
            if (File.Exists(dbASCIIZ) & sqlConn == null)
            {
                sqlConn = new SQLiteAsyncConnection(dbASCIIZ, SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);
            }
        }

        /// <summary>
        /// The DisposeDataBaseConnection.
        /// </summary>
        public static void Dispose()
        {
            if (sqlConn != null)
            {
                // if (sqlConn.State == ConnectionState.Closed)
                // {
                // sqlConn.Dispose();
                sqlConn = null;
                dataConnUniq = null;
                // }
            }
        }

        /// <summary>
        /// The CloseDataBaseConnection.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public static async Task Close()
        {
            if (sqlConn != null)
            {
                // if (sqlConn.State == ConnectionState.Open)
                // {
                await sqlConn.CloseAsync();
                // }
            }
        }
    }
}
