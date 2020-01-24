using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Project.FC2J.Batch.Internal.DataAccess
{
    internal static class DbHelper
    {
        const int _commandTimeOut = 0;

        private static string ConnectionString()
        {
            var dbSettings = DBSettings.GetDBSettingsInstance();
            return dbSettings.Connection;
        }

        public static string EmptyNull(this string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        public static void SanitizeParameters(params SqlParameter[] parameters)
        {
            foreach (var param in parameters.Where(param => param.Value is string))
            {
                param.Value = ((string)param.Value).EmptyNull().Trim();
            }
        }

        public static void ExecuteNonQuery(this DbConnection connection, string commandText, params SqlParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeOut;
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
        }

        public static void ExecuteNonQuery(this string commandText, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = DbHelper.GetOpenConnection())
                {
                    connection.ExecuteNonQuery(commandText, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public static async Task<T> GetOpenConnectionAsync<T>() where T : DbConnection, new()
        {
            var connection = new T
            {
                ConnectionString = ConnectionString()
            };
            await connection.OpenAsync();
            return connection;
        }

        public static DbConnection GetOpenConnection() 
        {
            var connection = new SqlConnection
            {
                ConnectionString = ConnectionString()
            };
            connection.Open();
            return connection;
        }

        public static async Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, params SqlParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeOut;
                command.Parameters.AddRange(parameters);
                return await command.ExecuteReaderAsync();
            }
        }

        public static async Task<T> GetAsync<T>(this DbDataReader reader, string columnName)
        {
            var ord = reader.GetOrdinal(columnName);
            return await GetAsync<T>(reader, ord);
        }

        public static async Task<T> GetAsync<T>(this DbDataReader reader, int ordinal)
        {
            if(await reader.IsDBNullAsync(ordinal))
            {
                return default(T);
            }
            return (T)reader[ordinal];
        }

        

        public static async Task SaveData(string commandText, params SqlParameter[] parameters)
        {
            using (var connection = await GetOpenConnectionAsync<SqlConnection>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = _commandTimeOut;
                    command.Parameters.AddRange(parameters);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }



        
    }
}
