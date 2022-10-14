using Domain.Exceptions;
using Domain.Models;
using LInksync_Cold_Storage_Wallet.Services.Implementation;
using SQLite;
using SYNCWallet.Services.Definitions;

namespace Application.Implementation
{
    public class CurrencySettingCache : ICacheRepository<CurrencyDataSetting>
    {
        private ICommunication Communication { get; set; }
        public  string DB { get; set; }

        public CurrencySettingCache(ICommunication communication) => Communication = communication;

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

        public List<CurrencyDataSetting> GetAll()
        {
            throw new RepositoryObjectMethodNotSupported("GetAll is not supported for Currency Settings, as one currency has only one setting file.");
        }

        public List<CurrencyDataSetting> GetAllForAddress(string address)
        {
            throw new RepositoryObjectMethodNotSupported("GetAllForAddress is not supported for Currency Settings.");
        }

        public List<CurrencyDataSetting> GetAllRange(string address, DateTime from, DateTime to)
        {
            throw new RepositoryObjectMethodNotSupported("GetAllRange is not supported for Currency Settings.");
        }

        public CurrencyDataSetting GetEntity(string hash)
        {
            try
            {
                var exists = default(CurrencyDataSetting);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    exists = c.Table<CurrencyDataSetting>().FirstOrDefault(x => x.ContractAddress == hash);
                }

                return exists;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int CreateEntity(CurrencyDataSetting entity)
        {
            try
            {
                var exists = default(int);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    if (c.Table<CurrencyDataSetting>().FirstOrDefault(x => x.ContractAddress != x.ContractAddress) == null)
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

        public int UpdateEntity(CurrencyDataSetting entity)
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

        public int DeleteEntity(CurrencyDataSetting entity)
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

        public int SoftDeleteEntity(CurrencyDataSetting entity)
        {
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    entity.IsEnabled = 0;
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
    }
}