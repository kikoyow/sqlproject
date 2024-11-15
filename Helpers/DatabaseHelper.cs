using MySql.Data.MySqlClient;
using System.Data;

public class DatabaseHelper
{
    private MySqlConnection connection;

    public DatabaseHelper()
    {
        // Set up your connection string
        string connectionString = "server=localhost;database=gcy_ictprg431;user=root;password=;";
        connection = new MySqlConnection(connectionString);
    }

    public void OpenConnection()
    {
        if (connection.State == ConnectionState.Closed)
            connection.Open();
    }

    public void CloseConnection()
    {
        if (connection.State == ConnectionState.Open)
            connection.Close();
    }

    public MySqlDataAdapter ExecuteQuery(string query)
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
        return adapter;
    }

    public void ExecuteNonQuery(string query)
    {
        OpenConnection();
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.ExecuteNonQuery();
        CloseConnection();
    }
}
