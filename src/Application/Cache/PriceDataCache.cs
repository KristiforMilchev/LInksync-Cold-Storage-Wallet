using Domain.Exceptions;
using Domain.Models;
using SQLite;
using SYNCWallet.Services.Definitions;

namespace Application.Cache
{
    public class PriceDataCache : ICacheRepository<RangeBarModel>
    {
        private ICommunication Communication { get; set; }
        public  string DB { get; set; }


        public PriceDataCache(ICommunication communication)
        {
            Communication = communication;

         
        }

        public void SelectDatabase(string database)
        {
            try
            {
                DB = Path.Combine(Communication.DefaultPath, $"{database}.db");
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    c.EnableWriteAheadLogging();
                    c.CreateTable<RangeBarModel>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public List<RangeBarModel> GetAll()
        {
            try
            {
                var listTDTO = default(List<RangeBarModel>);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    listTDTO = c.Table<RangeBarModel>().ToList();

                }
           
                return listTDTO;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<RangeBarModel> GetAllForAddress(string address)
        {
            throw new RepositoryObjectMethodNotSupported("GetAllForAddress is not supported for RangeBarModel," +
            " as each address is stored on it's on database, use GetAll to get all the entities in for a certain chart.");
        }

        public List<RangeBarModel> GetAllRange(string address, DateTime from, DateTime to)
        {
            try
            {
                var listTDto = default(List<RangeBarModel>);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    listTDto = c.Table<RangeBarModel>().Where(x=>x.Date >= from && x.Date <= to).ToList();

                }
           
                return listTDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public RangeBarModel GetEntity(string hash)
        {
            try
            {
                var dto = default(RangeBarModel);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    dto = c.Table<RangeBarModel>().FirstOrDefault(x=>x.Timestamp ==  int.Parse(hash));
                }
           
                return dto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public int CreateEntity(RangeBarModel entity)
        {
            try
            {
                var exists = default(int);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    if (c.Table<RangeBarModel>().FirstOrDefault(x => x.Date != x.Date) == null)
                    {
                        c.Insert(entity);
                        exists = 1;
                    }
 
                }

                return exists;
            }
            catch (Exception e)
            {

                return 0;
            }
        }

        public int UpdateEntity(RangeBarModel entity)
        {
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    c.Update(entity);
                }
            
                return 1;
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                return 0;
            }
        }

        public int DeleteEntity(RangeBarModel entity)
        {
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    c.Delete(entity);
                }
                return 1;
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                return 0;
            }

        }

        public int SoftDeleteEntity(RangeBarModel entity)
        {
            throw new RepositoryObjectMethodNotSupported("SoftDeleteEntity is not supported for RangeBarModel, as data is being collected externally..");
        }
    }
}