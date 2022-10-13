namespace SYNCWallet.Services.Definitions
{
    public interface ICacheRepository<T>
    {
        /// <summary>
        /// Sets the current database to be used for entity manipulations.
        /// </summary>
        ///
        public void SelectDatabase(string database);
        /// <summary>
        /// Takes in a generic T, opens a database reads all from T and returns a List<T>
        /// </summary>
        ///
        public List<T> GetAll();
        
        /// <summary>
        /// Takes in a generic T, opens a database reads all from T that match the address property  and returns a List<T>
        /// Parameters:
        /// <param name="address">Blockchain address.</param>
        /// </summary>
        ///
        public List<T> GetAllForAddress(string address);
        /// <summary>
        /// Takes in a generic T, opens a database reads all from T that match the address in the range
        /// of from  and to property  and returns a List<T>
        /// Parameters:
        /// <param name="address">Blockchain address.</param>
        /// <param name="from">DateTime UTC date start from.</param>
        /// <param name="to">DateTime UTC date end to.</param>
        /// </summary>
        ///
        public List<T> GetAllRange(string address, DateTime from, DateTime to);
        /// <summary>
        /// Takes in a string, opens a database reads a single entity from T that match the hash  and returns a T
        /// Parameters:
        /// <param name="hash">Any string that can be used as a predicate in the implementation.</param>
        /// </summary>
        ///
        public T GetEntity(string hash);
        /// <summary>
        /// Takes in a generic T, opens a database and saves T if T is not present in the collection.
        /// Parameters:
        /// <param name="entity">Generic object T, modeled in mind with SQLlite supported data formats..</param>
        /// </summary>
        ///
        public int CreateEntity(T entity);
        /// <summary>
        /// Takes in a generic T, opens a database and updates T if T is present in the collection.
        /// Parameters:
        /// <param name="entity">Generic object T, modeled in mind with SQLlite supported data formats..</param>
        /// </summary>
        ///
        public int UpdateEntity(T entity);
        /// <summary>
        /// Takes in a generic T, opens a database and deletes T if T is present in the collection, important it's hard
        /// delete do not call this unless you want to remove the entity entirely.
        /// Parameters:
        /// <param name="entity">Generic object T, modeled in mind with SQLlite supported data formats..</param>
        /// </summary>
        ///
        public int DeleteEntity(T entity);
        /// <summary>
        /// Takes in a generic T, opens a database and deletes T if T is present in the collection, important it's not a hard
        /// delete do call this only if you want to keep entities but don't want to appear on any of the query methods.
        /// Parameters:
        /// <param name="entity">Generic object T, modeled in mind with SQLlite supported data formats..</param>
        /// </summary>
        ///
        public int SoftDeleteEntity(T entity);
    }
}