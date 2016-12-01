using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;

namespace CourseworkServer
{
    class DatabaseHandler
    {

        public DataTable DoSQLQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(Server.connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
            {
                connection.Open();
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }
        public int DoSQLCommand(string command)
        {
            using (SqlConnection connection = new SqlConnection(Server.connectionString))
            using (SqlCommand sqlCommand = new SqlCommand(command, connection))
            {
                connection.Open();
                return sqlCommand.ExecuteNonQuery();
            }
        }
        public bool CheckLoginCredentials(string username, string passwordHash)
        {
            string query = "select * from Accounts where Username = '" + username + "' and PasswordHash = '" + passwordHash + "'";
            DataTable d = DoSQLQuery(query);
            return d.Rows.Count == 1;
        }
        public void CreateAccount(string username, string passwordHash)
        {
            string command = "insert into Accounts values('" + username + "','" + passwordHash + "')";
            Console.WriteLine(DoSQLCommand(command) + " row(s) changed"); 
        }
    }
}
