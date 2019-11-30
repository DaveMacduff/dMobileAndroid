namespace dMobile.View.Test_Pages
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Diagnostics;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="MemoryTestPage" />
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MemoryTestPage : dMobile.View.CustomPages.DMobileContentPage
    {
        /// <summary>
        /// Defines the timer
        /// </summary>
        private Stopwatch timer = new Stopwatch();

        /// <summary>
        /// Defines the count
        /// </summary>
        private int count = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryTestPage"/> class.
        /// </summary>
        public MemoryTestPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The Button_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            timer.Start();

            this.Loading = true;

            using (SQLiteCommand cmdInsertTest1 = new SQLiteConnection(DatabaseConnection.GetInstance().SQLConn).CreateCommand())
            {
                for (int i = 0; i < 2000; i++)
                {
                    string key = i.ToString();
                    int value = i;

                    Console.WriteLine(string.Format("Inserting into test 1 values {0} {1}", key, value));

                    cmdInsertTest1.CommandText = "INSERT INTO test1(Key, Value) VALUES (@key, @value)";
                    cmdInsertTest1.CommandType = CommandType.Text;

                    cmdInsertTest1.Parameters.Add(new SQLiteParameter("@key", key));
                    cmdInsertTest1.Parameters.Add(new SQLiteParameter("@value", value));

                    cmdInsertTest1.ExecuteNonQuery();
                }
            }

            this.Loading = false;

            timer.Stop();
            this.timeElapsed.Text = "Time elapsed: " + timer.Elapsed.ToString();
            timer.Reset();
        }

        /// <summary>
        /// The Button2_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Button2_Clicked(object sender, EventArgs e)
        {
            timer.Start();

            this.Loading = true;

            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

            using (SQLiteCommand cmdInsertTest2 = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                for (int i = 0; i < 2000; i++)
                {
                    string key = i.ToString();
                    int value = i;

                    Console.WriteLine(string.Format("Inserting into test 2 values {0} {1}", key, value));

                    cmdInsertTest2.CommandText = "INSERT INTO test2(Key, Value) VALUES (@key, @value)";
                    cmdInsertTest2.CommandType = CommandType.Text;

                    cmdInsertTest2.Parameters.Add(new SQLiteParameter("@key", key));
                    cmdInsertTest2.Parameters.Add(new SQLiteParameter("@value", value));

                    cmdInsertTest2.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            this.Loading = false;

            timer.Stop();
            this.timeElapsed2.Text = "Time elapsed: " + timer.Elapsed.ToString();
            timer.Reset();
        }

        /// <summary>
        /// The ButtonClear_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void ButtonClear_Clicked(object sender, EventArgs e)
        {
            this.Loading = true;

            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM test1";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM test2";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Deleted records");

            this.Loading = false;
        }
    }
}
