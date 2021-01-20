using Dapper;
using APIForDB.Providers;
using System;
using System.Collections.Generic;
using APIForDB.Models.DB;
using System.Linq;

namespace APIForDB
{
    /// <summary>
    /// интерфейс для активации методов из DBRepository в DBService
    /// </summary>
    public interface IDBRepository
    {
        string GetSize();
        IEnumerable<int> GetByTypes(string info);
        string GetMainDB(int id);
        string GetContentDB(int id);
        (IEnumerable<Page> pages, string error) GetAllDB(int count = -1);
        AddPageResponse AddPage(AddPageRequest page);
        UpdatePageResponse UpdatePage(int id, UpdatePageRequest request);
    }

    /// <summary>
    /// класс репозиторий. Отсюда идут все обращения в БД
    /// </summary>
    public class DBRepository : IDBRepository
    {
        private readonly ISQLiteConnectionProvider _connectionProvider;

        public DBRepository(ISQLiteConnectionProvider sqliteConnectionProvider)
        {
            _connectionProvider = sqliteConnectionProvider;
        }

        /// <summary>
        /// получение всей или определённой страницы БД
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public (IEnumerable<Page> pages, string error) GetAllDB(int count = -1)
        {
            using (var connection = _connectionProvider.GetDbConnection())
            {
                try
                {
                    connection.Open();
                    if (count == -1)
                    {
                        return (connection.Query<Page>(@"
                        SELECT 
                        id as Id,
                        page as Main,
                        content as Contents,
                        types as Type
                        FROM Data"), null);
                    }
                    else
                    {
                        return (
                            connection.Query<Page>(@"
                        SELECT 
                        id as Id,
                        page as Main,
                        content as Contents,
                        types as Type
                        FROM Data 
                        Where @count == id",
                            new { count = count }), null);
                    }
                }
                catch(Exception e)
                {
                    return (null, e.Message);
                }
            }
        }

        /// <summary>
        /// добавление статьи в БД. Для поддержки
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public AddPageResponse AddPage(AddPageRequest page)
        {
            try
            {
                using (var connection = _connectionProvider.GetDbConnection())
                {
                    connection.Open();
                    connection.Execute(
                        @"INSERT INTO Data 
                        ( page, content, types ) VALUES 
                        ( @Page, @Content, @Types );",
                        new { Page = page.Page, Content = page.Content, Types = page.Types });
                    return new AddPageResponse 
                    {
                        Error = null
                    };
                }
            }
            catch (Exception e)
            {
                return new AddPageResponse
                {
                    Error = e.Message
                };
            }
        }

        /// <summary>
        /// редакция статьи в БД. Для поддержки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public UpdatePageResponse UpdatePage(int id, UpdatePageRequest request)
        {
            try
            {
                using (var connection = _connectionProvider.GetDbConnection())
                {
                    connection.Open();
                    if (0 == connection.Execute(
                        @"UPDATE Data 
                        SET page = @Page, content = @Content, types = @Types
                        WHERE id = @Id;",
                        new { Page = request.Page, Content = request.Content, Types = request.Types, Id = id }))
                    {
                        throw new Exception("Нечего редактировать!");
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return new UpdatePageResponse
                {
                    Error = e.Message
                };
            }
        }

        /// <summary>
        /// получение числа строк
        /// </summary>
        /// <returns></returns>
        public string GetSize()
        {
            using (var connection = _connectionProvider.GetDbConnection())
            {
                connection.Open();
                return connection.Query<string>(@"
                        SELECT COUNT (*)
                        FROM Data").First();
            }

        }

        /// <summary>
        /// получение статей по определённому тегу. не успел реализовать в приложении. Оставил на будущее
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public IEnumerable<int> GetByTypes(string info)
        {
            using (var connection = _connectionProvider.GetDbConnection())
            {
                connection.Open();

                return connection.Query<int>(@"
                        SELECT 
                        id
                        FROM Data
                        WHERE name LIKE @searchTerm",
                    new { searchTerm = '%' + info + '%' });
            }
        }

        /// <summary>
        /// получение json для главной по id. для пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetMainDB(int id)
        {
            using (var connection = _connectionProvider.GetDbConnection())
            {
                connection.Open();
                return connection.Query<string>(@"
                        SELECT page
                        FROM Data
                        WHERE id = Id;", new { Id = id }).First();
            }
        }

        /// <summary>
        /// получение json для главной по id. для пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetContentDB(int id)
        {
            using (var connection = _connectionProvider.GetDbConnection())
            {
                connection.Open();
                return connection.Query<string>(@"
                        SELECT content
                        FROM Data
                        WHERE id = Id;", new { Id = id }).First();
            }
        }
    }
}
