using Domain.Models;
using SQLite;
using SYNCWallet.Services.Definitions;

namespace Application.Implementation
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
            return new List<RangeBarModel>();
        }

        public List<RangeBarModel> GetAllRange(string address, DateTime from, DateTime to)
        {
            try
            {
                var listTDTO = default(List<RangeBarModel>);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    listTDTO = c.Table<RangeBarModel>().Where(x=>x.Date >= from && x.Date <= to).ToList();

                }
           
                return listTDTO;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public RangeBarModel GetEntity(string hash)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}