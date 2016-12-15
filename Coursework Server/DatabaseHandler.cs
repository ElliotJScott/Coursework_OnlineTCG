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
        public object[][] DoParameterizedSQLQuery(string query, params object[] parameters)
        {

            using (SqlConnection connection = new SqlConnection(Server.connectionString))
            using (SqlCommand sqlCommand = new SqlCommand(query, connection))
            {
                connection.Open();
                for (int i = 1; i <= parameters.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue("@p" + i, parameters[i-1]);
                }
                SqlDataReader reader = sqlCommand.ExecuteReader();
                List<object[]> objects = new List<object[]>();
                while (reader.Read())
                {
                    object[] row = new object[reader.FieldCount];
                    for (int i = 0; i < row.Length; i++)
                    {
                        row[i] = reader[i];
                    }
                    objects.Add(row);
                }
                return objects.ToArray();
            }


        }
        public int DoParameterizedSQLCommand(string command, params object[] parameters)
        {
            lock (command)
            {
                using (SqlConnection connection = new SqlConnection(Server.connectionString))
                using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                {
                    connection.Open();
                    for (int i = 1; i <= parameters.Length; i++)
                    {
                        sqlCommand.Parameters.AddWithValue("@p" + i, parameters[i-1]);
                    }
                    return sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
