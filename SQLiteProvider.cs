using Dapper;
using APIForDB.Models.Options;
using Microsoft.Extensions.Options;
using System.Data.SQLite;

namespace APIForDB.Providers
{
    /// <summary>
    /// содержит метод подключения к БД
    /// </summary>
    public interface ISQLiteConnectionProvider
    {
        SQLiteConnection GetDbConnection();
    }
    /// <summary>
    /// тут БД создаётся и даётся к ней доступ
    /// </summary>
    public class SQLiteConnectionProvider : ISQLiteConnectionProvider
    {
  
        private readonly string _connectionString;

        public SQLiteConnectionProvider(IOptions<DbConnectionOptions> dbOptions)
        {
            _connectionString = dbOptions.Value.ConnectionString;             
            using (var connection = GetDbConnection())
            {
                var createDB =
                   @"CREATE TABLE IF NOT EXISTS Data
                   (
                     id                             integer primary key AUTOINCREMENT,
                     page                           varchar not null,
                     content                        varchar not null,
                     types                          varchar not null
                   )";

                connection.Execute(createDB);
            }
        }

        public SQLiteConnection GetDbConnection() 
            => new SQLiteConnection(_connectionString);
    }
}
