using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace EcomCockpit.Utils.EFUtils
{
    /// <summary>
    /// Class used to do queries and actions against a database or other persistence. Contains some basic crud operations, which can be used instantly without any further work. 
    /// But also provides more generic useable methods for custom queries.
    /// 
    /// TODO Examples
    /// TODO Dispose
    /// TODO ExceptionHandling
    /// TODO TrackChanges
    /// TODO Loading-Strategy
    /// TODO Sync on Datamanager if specified
    /// TODO async
    /// TODO logging, protocol
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the entity which needs to inherit from <seealso cref="BasicEntity"/></typeparam>
    public class DataProvider<T> where T : BasicEntity
    {
        /// <summary>
        /// Find and receive the entity matching the specified ID. May return null if nothing is found.
        /// </summary>
        /// <param name="ID">The entity ID to search for.</param>
        /// <returns>The entity matching the ID or <code>Null</code> if no one is found.</returns>
        public T Find(int ID)
        {
            T tBack = null;

            using (var dm = new DataManager<T>())
            {
                try
                {
                    tBack = dm.Query().First((entity) => entity.ID == ID);
                }
                catch (Exception ex)
                {
                    Log.Error(ex,ex.Message);
                }
            }

            return tBack;
        }

        /// <summary>
        /// Find and receive all entities matching the specified criteria. May return an empty list if nothing is found.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public IEnumerable<T> FindAll(List<int> IDs)
        {
            List<T> tBack = new List<T>();
            
            if (IDs != null && IDs.Count > 0 )
            {
                IDs.ForEach((id) =>
                {
                    var entity = Find(id);
                    if (entity != null)
                    {
                        tBack.Add(entity);
                    }
                });
            }
            else
            {
                Log.Warning("Empty list of ids!");
            }

            return tBack;
        }

        /// <summary>
        /// TODO join dm
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Update(T entity)
        {
            T tBack = entity;
            
            //TODO check if updated
            using(var dm = new DataManager<T>())
            {
                try
                {
                    dm.Query().First((x) => x.ID == entity.ID).update(entity);
                }
                catch(Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }

            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<T> Update(IEnumerable<T> entities)
        {
                List<T> tBack = new List<T>();
            if (entities != null && entities.Count() > 0)
            {

            tBack.AddRange(entities);

            using (var dataManager = new DataManager<T>())
            {
                entities.Each((x) => Update(x));

            }
            }
            else
            {
                Log.Warning("Empty list of entites!");
            }

            return tBack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Add(T entity)
        {
            T tBack = null;

            using (var dm = new DataManager<T>())
            {
                try
                {
                    string message = "";
                    var Result = dm.Save(new List<T>() { entity });

                    tBack = Result.ElementAt(0);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }

            return tBack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<T> Add(IEnumerable<T> entities)
        {
            List<T> tBack = new List<T>();
            if (entities != null && entities.Count() > 0)
            {

                tBack.AddRange(entities);

                using (var dataManager = new DataManager<T>())
                {
                    entities.Each((x) => Add(x));

                }
            }
            else
            {
                Log.Warning("Empty list of entites!");
            }

            return tBack;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            bool tBack = false;

            using (var dm = new DataManager<T>())
            {
                try
                {
                    tBack = dm.Delete(entity);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }

            return tBack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(IEnumerable<T> entities)
        {
            bool tBack = false;

            foreach (var entity in entities)
            {
               if (Delete(entity) == false)
                {
                    //breaks here because of exception or error
                    break;
                }

                tBack = true;
            }

            return tBack;
        }
    }
}
