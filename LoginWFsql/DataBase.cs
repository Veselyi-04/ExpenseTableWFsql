using MySql.Data.MySqlClient;

namespace LoginWFsql
{
    class DataBase
    {
        MySqlConnection connection = new MySqlConnection("server=localhost; port=3306; username=root; password=root; database=expense_table_bd");

        public void openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        public void closeConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }
    }
}
