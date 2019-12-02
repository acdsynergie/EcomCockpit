using EcomCockpit.Utils.EFUtils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EcomCockpit.Web.Controllers
{
    /// <summary>
    /// Abstract controller used to inherit some basic actions on new controllers
    /// </summary>
    /// <typeparam name="T">The type of the entity the controller should handle</typeparam>
    public abstract class AbstractController<T> : ControllerBase where T : BasicEntity
    {
        [HttpPost]
        [Route("save")]
        public T Save(T content)
        {
            return new DataProvider<T>().Update(content);
        }

        [HttpPost]
        public IEnumerable<T> Update(IEnumerable<T> content)
        {
            return new DataProvider<T>().Update(content);
        }

        [HttpPost]
        public bool delete(T entity)
        {
            return new DataProvider<T>().Delete(entity);
        }

        [HttpGet]
        public T find(int id)
        {
            return new DataProvider<T>().Find(id);
        }
    }
}
