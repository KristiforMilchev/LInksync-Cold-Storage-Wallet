using Domain.Exceptions;
using Domain.Models;
using SQLite;
using SYNCWallet.Services.Definitions;

namespace Application.Cache
{
    public class UserBalanceCache : ICacheRepository<UserAssetBalance>
    {
         private ICommunication Communication { get; set; }
        public  string DB { get; set; }


        public UserBalanceCache(ICommunication communication)
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
                    c.CreateTable<UserAssetBalance>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public List<UserAssetBalance> GetAll()
        {
            try
            {
                var listTDTO = default(List<UserAssetBalance>);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    listTDTO = c.Table<UserAssetBalance>().Where(x=>x.WalletAddress == Communication.PublicAddress && x.NetworkId == Communication.ActiveNetwork.Id).ToList();

                }
           
                return listTDTO;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<UserAssetBalance> GetAllForAddress(string address)
        {
            try
            {
                var listTDTO = default(List<UserAssetBalance>);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    listTDTO = c.Table<UserAssetBalance>().Where(x=>x.Currency == address && x.WalletAddress == Communication.PublicAddress && x.NetworkId == Communication.ActiveNetwork.Id).ToList();
                }
           
                return listTDTO;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<UserAssetBalance> GetAllRange(string address, DateTime from, DateTime to)
        {
            try
            {
                var listTDto = default(List<UserAssetBalance>);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    listTDto = c.Table<UserAssetBalance>().Where(x=>x.Date >= from && x.Date <= to && x.WalletAddress == Communication.PublicAddress && x.NetworkId == Communication.ActiveNetwork.Id).ToList();

                }
           
                return listTDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public UserAssetBalance GetEntity(string hash)
        {
            try
            {
                var dto = default(UserAssetBalance);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    dto = c.Table<UserAssetBalance>().LastOrDefault(x=>x.Currency ==  hash  && x.WalletAddress == Communication.PublicAddress && x.NetworkId == Communication.ActiveNetwork.Id);
                }
           
                return dto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public int CreateEntity(UserAssetBalance entity)
        {
            try
            {
                var exists = default(int);
                using (SQLiteConnection c = new SQLiteConnection(DB))
                {
                    var lastEntity = c.Table<UserAssetBalance>().LastOrDefault(x => x.Currency == entity.Currency && x.WalletAddress == Communication.PublicAddress && x.NetworkId == Communication.ActiveNetwork.Id);
                    if (lastEntity == null)
                    {
                        Console.WriteLine($"Adding balance entry for {entity.Currency} : {entity.Balance}");
                        c.Insert(entity);
                        exists = 1;
                    }
                    else  if (lastEntity.Date.AddMinutes(5) < entity.Date) 
                    {
                        Console.WriteLine($"Adding balance entry for {entity.Currency} : {entity.Balance}");
                        c.Insert(entity);
                        exists = 1;
                    }
                    else
                    {
                        Console.WriteLine("Ignored too close to previous");
                    }
 
                }

                return exists;
            }
            catch (Exception e)
            {

                return 0;
            }
        }

        public int UpdateEntity(UserAssetBalance entity)
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

        public int DeleteEntity(UserAssetBalance entity)
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

        public int SoftDeleteEntity(UserAssetBalance entity)
        {
            throw new RepositoryObjectMethodNotSupported("SoftDeleteEntity is not supported for UserAssetBalance, as data is being collected externally..");
        }
    }
}