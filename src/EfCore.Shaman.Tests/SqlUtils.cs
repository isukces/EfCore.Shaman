using System;
using System.Data.SqlClient;

namespace EfCore.Shaman.Tests
{
    public class SqlUtils
    {
        public static void CreateDb(string connectionString)
        {
            RunQuery(connectionString, (con, dbName) =>
            {
                var sql = $"create database [{dbName}]";
                Console.WriteLine(sql);
                using(var q = new SqlCommand(sql, con))
                {
                    var result = q.ExecuteNonQuery();
                    return result;
                }
            });
        }

        public static bool DbExists(string connectionString)
        {
            return RunQuery(connectionString, (con, dbName) =>
            {
                var sql = $"select name from sys.databases where name=\'{dbName}\'";
                Console.WriteLine(sql);
                using (var q = new SqlCommand(sql, con))
                {
                    var name = q.ExecuteScalar();
                    return name != null;
                }
            });
        }

        public static void DropDb(string connectionString)
        {
            RunQuery(connectionString, (con, dbName) =>
            {
                var sql = $"drop database [{dbName}]";
                Console.WriteLine(sql);
                using (var q = new SqlCommand(sql, con))
                {
                    var result = q.ExecuteNonQuery();
                    return result;
                }
            });
        }

        public static T RunQuery<T>(string connectionString, Func<SqlConnection, string, T> func)
        {
            var csb = new SqlConnectionStringBuilder(connectionString);
            var dbName = csb.InitialCatalog;
            csb.InitialCatalog = "master";
            using(var con = new SqlConnection(csb.ConnectionString))
            {
                con.Open();
                return func(con, dbName);
            }
        }
    }
}