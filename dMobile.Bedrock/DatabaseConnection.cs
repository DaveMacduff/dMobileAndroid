// <copyright file="DatabaseConnection.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="DatabaseConnection" />.
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Defines the databaseName.
        /// </summary>
        private const string DatabaseNameValue = "dMobile.Sqlite";

        /// <summary>
        /// Defines the databasePassword.
        /// </summary>
        private const string DatabasePasswordValue = "dMobile";

        /// <summary>
        /// Defines the dataConnUniq.
        /// </summary>
        private static DatabaseConnection dataConnUniq;

        /// <summary>
        /// Defines the sqlConn.
        /// </summary>
        private static SQLiteAsyncConnection sqlConn;

        /// <summary>
        /// Defines the database path.
        /// </summary>
        private static string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        /// <summary>
        /// Defines the database full path.
        /// </summary>
        private static string databaseFullPath = string.Empty;

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
            get => dataConnUniq;

            set => dataConnUniq = value;
        }

        /// <summary>
        /// Gets or sets the ConnString.
        /// </summary>
        public SQLiteConnectionString ConnString
        {
            get => connString;

            set => connString = value;
        }

        /// <summary>
        /// Gets or sets the database full path.
        /// </summary>
        public string DatabaseFullPath
        {
            get => databaseFullPath;

            set => databaseFullPath = value;
        }

        /// <summary>
        /// Gets or sets the DatabasePath.
        /// </summary>
        public string DatabasePath
        {
            get => databasePath;

            set => databasePath = value;
        }

        /// <summary>
        /// Gets the DatabasePassword.
        /// </summary>
        public string DatabasePassword => DatabasePasswordValue;

        /// <summary>
        /// Gets the DatabaseName.
        /// </summary>
        public string DatabaseName => DatabaseNameValue;

        /// <summary>
        /// Gets the SqlConn.
        /// </summary>
        public SQLiteAsyncConnection SqlConn => sqlConn;

        /// <summary>
        /// The GetInstance.
        /// </summary>
        /// <returns>.</returns>
        public static DatabaseConnection GetInstance()
        {
            if (dataConnUniq == null)
            {
                dataConnUniq = new DatabaseConnection();
                databaseFullPath = Path.Combine(databasePath, DatabaseNameValue);
            }

            Open();

            return dataConnUniq;
        }

        /// <summary>
        /// The OpenDataBaseConnection.
        /// </summary>
        public static void Open()
        {
            if (File.Exists(databaseFullPath) && sqlConn == null)
            {
                // connString = new SQLiteConnectionString(databaseFullPath)
                // foreign keys=true HAS to be specified or SQLite will not enforce foreign key constraints
                // connString = "Data Source = '" + databaseFullPath + "'; foreign keys=true;"
                // connString = "Data Source = '" + databasePath + databaseName + "'; Password = " + databasePassword
                sqlConn = new SQLiteAsyncConnection(
                    databaseFullPath,
                    SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);
            }
        }

        /// <summary>
        /// The DisposeDataBaseConnection.
        /// </summary>
        public static void Dispose()
        {
            if (sqlConn != null)
            {
                sqlConn = null;
                dataConnUniq = null;
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
                await sqlConn.CloseAsync();
            }
        }
    }
}
