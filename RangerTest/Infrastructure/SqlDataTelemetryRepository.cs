using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Trigger.Signal;

namespace RangerTest.Infrastructure
{
    public class SqlDataTelemetryRepository : IRepository<Telemetry>
    {
        const string SqlConnectionString = @"Data Source=192.168.0.9;Initial Catalog=Shoppercoin;Persist Security Info=True;User ID=ivg;Password=ivg";
        SqlConnection _conn;

        public SqlDataTelemetryRepository()
        {
            _conn = new SqlConnection(SqlConnectionString);
            _conn.Open();
        }

        public void Dispose()
        {
            _conn.Close();
        }

        public IEnumerable<Telemetry> GetItems()
        {
            if (_conn?.State == System.Data.ConnectionState.Open)
            {
                var reader = new SqlCommand("select Data from signal", _conn).ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        yield return (Telemetry)reader.GetString(0);
                    }
                }
            }
        }
    }
}
