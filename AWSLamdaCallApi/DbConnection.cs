using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace AWSLamdaCallApi
{
    public class DbConnection
    {
        private DBConnection()
        {

        }

        private string databaseName = string.Empty;
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {

            if (String.IsNullOrEmpty(databaseName))
                return false;
            string connstring = string.Format("Server=dzdb.c8pfeggqi0qm.ap-southeast-1.rds.amazonaws.com; database={0}; UID=sa; password=12345678", databaseName);
            connection = new MySqlConnection(connstring);
            connection.Open();

            return true;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
