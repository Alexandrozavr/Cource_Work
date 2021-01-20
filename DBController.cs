using APIForDB.Models.DB;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APIForDB.Controllers
{
    /// <summary>
    /// Класс создающий контроллеры БД
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]    
    public class DBController : ControllerBase
    {
        private readonly DBService _DBService;

        public DBController(DBService DBService)
        {
            _DBService = DBService;
        }

        /// <summary>
        /// метод для осмотра определённой статьи по id. для пользования службой поддержки
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GetDBResponse GetIDPageDB([FromQuery]int id)
        {
            var result = _DBService.GetAllDB(id);
            return new GetDBResponse
            {
                Pages = result.pages,
                Error = result.error
            };
        }

        /// <summary>
        /// метод для осмотра всех статей. для пользования службой поддержки
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public GetDBResponse GetAllDB()
        {
            var result = _DBService.GetAllDB();
            return new GetDBResponse
            {
                Pages = result.pages,
                Error = result.error
            };
        }

        /// <summary>
        /// добавление новых статей. для пользования службой поддержки
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        public AddPageResponse AddPage([FromBody] AddPageRequest page)
        {
            return new AddPageResponse
            {
                Error = _DBService.AddPage(page)?.Error
            };
        }

        /// <summary>
        /// для обновления статей. для пользования службой поддержки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public UpdatePageResponse UpdatePage([FromQuery]int id, [FromBody] UpdatePageRequest request)
        {
            return _DBService.UpdatePage(id, request);
        }

        /// <summary>
        /// возвращение номеров статей по определённой тематике. Технический метод
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpGet("GetByTypes/{info}")]
        public IEnumerable<int> GetByTypes(string info)
        {
            return _DBService.GetByTypes(info);
        }


        /// <summary>
        /// получение размера. Технический метод для случайной генерации статей
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSize")]
        public string GetSize()
        {
            return _DBService.GetSize();
        }

        /// <summary>
        /// для получения визуальной части на главной. для пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetMain/{id}")]
        public string GetPageDB_User(int id)
        {
            return _DBService.GetMainDB(id);
        }

        /// <summary>
        /// для получения контента. для пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetContent/{id}")]
        public string GetContentDB_User(int id)
        {
            
            return _DBService.GetContentDB(id);
        }
    }
}