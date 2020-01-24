using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Project.FC2J.DataStore.Internal.DataAccess
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

        public static async Task<T> GetOpenConnectionAsync<T>() where T : DbConnection, new()
        {
            var connection = new T
            {
                ConnectionString = ConnectionString()
            };
            await connection.OpenAsync();
            return connection;
        }

        public static async Task<DbConnection> GetOpenConnectionAsync() 
        {
            var connection = new SqlConnection
            {
                ConnectionString = ConnectionString()
            };
            await connection.OpenAsync();
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

        public static async Task ExecuteNonQueryAsync(this DbConnection connection, string commandText, params SqlParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeOut;
                command.Parameters.AddRange(parameters);
                await command.ExecuteNonQueryAsync();
                command.Parameters.Clear();
            }
        }

        public static async Task ExecuteNonQueryAsync(this string commandText, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = await DbHelper.GetOpenConnectionAsync())
                {
                    await connection.ExecuteNonQueryAsync(commandText, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
        public static async Task<DataTable> GetDataTable(this string sp, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var connection = await GetOpenConnectionAsync())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sp;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = _commandTimeOut;
                    command.Parameters.AddRange(parameters);
                    var reader = await command.ExecuteReaderAsync();
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public static async Task<List<T>> GetList<T>(this string sp, params SqlParameter[] parameters) where T : new()
        {

            var resultSet = new List<T>();
            using (var connection = await GetOpenConnectionAsync())
            {
                using (var reader = await connection.ExecuteReaderAsync(sp, parameters))
                {
                    while (reader.Read())
                    {
                        T row = new T();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetValue(i) != DBNull.Value)
                            {
                                PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                                try
                                {
                                    if (propertyInfo.PropertyType.Name == "Byte[]")
                                    {
                                        var b64string = reader.GetValue(i).ToString();

                                        byte[] buffer = new byte[((b64string.Length * 3) + 3) / 4 -
                                            (b64string.Length > 0 && b64string[b64string.Length - 1] == '=' ?
                                            b64string.Length > 1 && b64string[b64string.Length - 2] == '=' ?
                                            2 : 1 : 0)];

                                        bool success = Convert.TryFromBase64String(b64string, buffer, out int written);

                                        if (success)
                                        {
                                            propertyInfo.SetValue(row, buffer, null);
                                        }
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(row, Convert.ChangeType(reader.GetValue(i), propertyInfo.PropertyType), null);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        resultSet.Add(row);
                    }
                }
            }
            return resultSet;
        }

        public static async Task<T> GetRecord<T>(this string sp, params SqlParameter[] parameters) where T : new()
        {
            var result = default(T);
            try
            {
                using (var connection = await GetOpenConnectionAsync())
                {
                    using (var reader = await connection.ExecuteReaderAsync(sp, parameters))
                    {
                        while (reader.Read())
                        {
                            T row = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.GetValue(i) != DBNull.Value)
                                {
                                    PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                                    try
                                    {
                                        if (propertyInfo.PropertyType.Name == "Byte[]")
                                        {
                                            var b64string = reader.GetValue(i).ToString();

                                            byte[] buffer = new byte[((b64string.Length * 3) + 3) / 4 -
                                                (b64string.Length > 0 && b64string[b64string.Length - 1] == '=' ?
                                                b64string.Length > 1 && b64string[b64string.Length - 2] == '=' ?
                                                2 : 1 : 0)];

                                            bool success = Convert.TryFromBase64String(b64string, buffer, out int written);

                                            if (success)
                                            {
                                                propertyInfo.SetValue(row, buffer, null);
                                            }
                                        }
                                        else
                                        {
                                            propertyInfo.SetValue(row, Convert.ChangeType(reader.GetValue(i), propertyInfo.PropertyType) , null);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                            result = row;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public static async Task<Int64> GetRecordId(this string sp, params SqlParameter[] parameters) 
        {
            Int64 value = 0;
            try
            {
                using (var connection = await GetOpenConnectionAsync())
                {
                    using (var reader = await connection.ExecuteReaderAsync(sp, parameters))
                    {
                        while (reader.Read())
                        {
                            value = reader.GetAsync<Int64>("Id").Result;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return value;
        }

        
    }
}
