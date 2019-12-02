using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EcomCockpit.Utils.EFUtils
{
    /// <summary>
    /// The <see cref="DataManager" is designed to encapsulate the DBContext/EFContext in order to ensure
    /// on the hand that the DBContext is indirectly controllable from UI but on the other to hide it from layers above.
    /// The internal constructor must only be usable by other DataManagers. It is used to share open DbContexts between DataManagers
    /// but only between them!
    /// Configurations:
    /// - No-Tracking: (Auto-Detect-Changes)
    /// - Loading-Strategies (Eager,Lazy,Preloading)
    /// - Save-Strategie (Auto,Manual)
    /// />
    /// </summary>
    internal class DataManager<T> : IDisposable where T : class
    {
        private DbContext eFContext;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataManager()
        {
            eFContext = new DbContext("Server=(localdb)\\mssqllocaldb;Database=GKV_DB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        /// <summary>
        /// Do not use this unless you want to share DBContext for performance reasons
        /// </summary>
        /// <param name="eFContext"></param>
        public DataManager(DbContext aFContext)
        {
            eFContext = aFContext;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoSaveChanges { get; private set; }

        public void Dispose()
        {
            // Änderungen speichern
            eFContext.SaveChanges();

            if (eFContext != null && DisposeContext) eFContext.Dispose();
        }

        protected bool DisposeContext = true;
        protected bool preloading = false;
        protected bool lazyLoading = false;

        /// <summary>
        /// 
        /// </summary>
        public Action<string> Log
        {
            get
            {
                return eFContext.Database.Log;
            }
            set
            {
                eFContext.Database.Log = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lazyLoading"></param>
        /// <param name="context"></param>
        public void Init(bool lazyLoading = false, DbContext context = null)
        {
            this.lazyLoading = lazyLoading;

            // Falls ein context hineingereicht wurde, nehme diesen!
            if (context != null) { eFContext = context; DisposeContext = false; }
            else { eFContext = new DbContext("Server=(localdb)\\mssqllocaldb;Database=GKV_DB;Trusted_Connection=True;MultipleActiveResultSets=true"); }

            // (De-)aktiviert das transparent (automatische) Nachladen verbundener Objekte
            eFContext.Configuration.LazyLoadingEnabled = lazyLoading;
            // ... dafür braucht man aber auch Proxies!!!
            eFContext.Configuration.ProxyCreationEnabled = lazyLoading;

            //kann man ggf. abschalten, wenn man Proxies verwendet und alle Properties virtual sind
            //eFContext.Configuration.AutoDetectChangesEnabled = false;

        }

        /// <summary>
        /// Grundabfrage mit NoTracking-Optionen und optionalen Includes
        /// </summary>
        public DbQuery<T> Query(bool Tracking = false, List<string> includes = null)
        {
            DbQuery<T> q;
            if (Tracking) // Tracking erwünscht
            {
                q = eFContext.Set<T>();
            }
            else  // Tracking nicht erwünscht
            {
                q = eFContext.Set<T>().AsNoTracking();
            }
            if (includes != null) // Eager Loading?
            {
                foreach (var include in includes)
                {
                    q = q.Include(include);
                }
            }
            return q;
        }

        /// Die neu hinzugefügten Objekte muss die Speichern-Routine wieder zurückgeben, da die IDs für die 
        /// neuen Objekte erst beim Speichern von der Datenbank vergeben werden
        public List<T> Save(List<T> menge)
        {
            var addedEntities = new List<T>();

            // Änderungen für jeden einzelnen Passagier übernehmen
            foreach (dynamic p in menge)
            {
                // Anfügen an diesen context
                eFContext.Set<T>().Attach((T)p);
                if (p.Id == 0)
                {
                    eFContext.Entry(p).State = EntityState.Added;
                    // Neue Datensätze merken, da diese nach Speichern zurückgegeben werden müssen (haben dann erst ihre IDs!)
                    addedEntities.Add(p);
                }
                else
                {
                    eFContext.Entry(p).State = EntityState.Modified;
                }
            }


            return addedEntities;
        }

        /// Die neu hinzugefügten Objekte muss die Speichern-Routine wieder zurückgeben, da die IDs für die 
        /// neuen Objekte erst beim Speichern von der Datenbank vergeben werden
        public bool Delete(T entity)
        {
            bool tBack = false;
            try
            {
                T result = eFContext.Set<T>().Remove(entity);
                tBack = result != null;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
            }
            return tBack;
        }
    }
}
