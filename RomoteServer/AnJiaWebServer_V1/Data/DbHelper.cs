using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace AnJiaWebServer_V1.Data
{
    public class DbHelper
    {

        MySqlCommand command;
        MySqlConnection conn;
        public DbHelper(string connstring)
        {
           conn = new MySqlConnection(connstring);
      
        }

        public async Task SelectAsync(string query)
        {
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = query;
            DbDataReader BindedReader = await command.ExecuteReaderAsync();

        }


    }
}
